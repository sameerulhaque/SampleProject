using System.Linq.Expressions;
using SampleProject.Infrastructure.EF;
using SampleProject.Infrastructure.Authentication;
using SampleProject.Shared.Models.Misc;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SampleProject.Core.Exceptions;
using SampleProject.Infrastructure.EF.Models;
using SampleProject.Core.Interfaces;
using Azure.Core;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.Fluent;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SampleProject.Infrastructure.Exceptions;
using SampleProject.API.Attributes;


namespace SampleProject.API.Controllers
{
    [ApiController]
    [CustomAuthorize]
    [ApiVersion("1.0")]
    [Route("/v{version:apiVersion}/[controller]")]
    public class BaseController<TEntity, TRequest> : ControllerBase
    where TEntity : Entity
    where TRequest : class, new()
    {
        protected readonly IReadOnlyRepository<TEntity> _service;
        protected readonly IWriteRepository<TEntity> _writeService;
        protected readonly IConfiguration configuration;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly ICacheService _cacheService;
        protected readonly string DP_PATH, CUST_DOCS_PATH, ENCRYPT_KEY;
        protected BaseController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IReadOnlyRepository<TEntity> service, 
            IWriteRepository<TEntity> writeService, ICacheService cacheService)
        {
            this.configuration = configuration;
            CUST_DOCS_PATH = configuration.GetValue<string>("custdocs") ?? "";
            DP_PATH = configuration.GetValue<string>("DPPath") ?? "";
            ENCRYPT_KEY = configuration.GetValue<string>("EncryptKey") ?? "";
            _service = service;
            _webHostEnvironment = webHostEnvironment;
            _writeService = writeService;
            _cacheService = cacheService;
        }

        protected IActionResult ApiResult(APIResponseModel result)
        {
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var paginationParams = GetHeaderValues();
            var paginationParamsDict = new Dictionary<string, object>();
            foreach (var property in paginationParams.GetType().GetProperties())
            {
                var value = property.GetValue(paginationParams);

                if (value is List<object> list && !(value is string))
                {
                    var concatenatedValue = string.Join(",", list.Select(item => item?.ToString() ?? string.Empty));
                    paginationParamsDict.Add(property.Name, concatenatedValue);
                }
                else
                {
                    paginationParamsDict.Add(property.Name, value);
                }
            }

            var (totalCount, data) = await _cacheService.GetDataAsync(async () =>
            {
                return await _service.ListAsync(CreateSpecificationFromHeaders(paginationParams));
            }, typeof(TEntity).FullName ?? "", paginationParamsDict, 300);

            var pagedList = PagedList<TEntity>.Create(paginationParams.PageSize, paginationParams.PageNumber, totalCount, data);
            var result = new APIResponseModel<PagedList<TEntity>>(pagedList);
            result.OK();
            return ApiResult(result);
        }

        [Idempotent]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"Key not found {id}");

            var result = new APIResponseModel<TEntity>(entity);
            result.OK();
            return ApiResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TRequest entity)
        {
            if (entity == null)
                throw new BadRequestException("Request Error", new Dictionary<string, string[]>());

            Validate.ValidateRequest(entity);

            var res = await _writeService.AddAsync(entity);
            if (res == null)
            {
                throw new BadRequestException("Unable to save", new Dictionary<string, string[]>());
            }
            else
            {
                var result = new APIResponseModel<TEntity>(res);
                result.OK();
                return ApiResult(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TRequest entity)
        {
            if (entity == null)
                throw new BadRequestException("Request Error", new Dictionary<string, string[]>());

            var res = await _writeService.UpdateAsync(id, entity);
            if (res == null)
            {
                throw new BadRequestException("Unable to save", new Dictionary<string, string[]>());
            }
            else
            {
                var result = new APIResponseModel<TEntity>(res);
                result.OK();
                return ApiResult(result);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.SoftDeleteAsync(id);
            var result = new APIResponseModel();
            result.OK();
            return ApiResult(result);
        }

        private PaginationParameters GetHeaderValues()
        {
            var _request = Request;
            var paginationParams = new PaginationParameters();

            // Extract page number and size with validation
            if (_request.Headers.ContainsKey("X-Page-Number") && int.TryParse(_request.Headers["X-Page-Number"].ToString(), out int pageNumber))
            {
                paginationParams.PageNumber = pageNumber;
            }
            else
            {
                paginationParams.PageNumber = 1;  // Default value
            }

            if (_request.Headers.ContainsKey("X-Page-Size") && int.TryParse(_request.Headers["X-Page-Size"].ToString(), out int pageSize))
            {
                paginationParams.PageSize = pageSize;
            }
            else
            {
                paginationParams.PageSize = 10;  // Default value
            }

            // Extract sorting information
            paginationParams.SortColumn = _request.Headers["X-Sort-Column"].ToString() ?? "defaultColumn";  // Default value
            paginationParams.SortDirection = _request.Headers["X-Sort-Direction"].ToString() ?? "asc";  // Default to "asc"

            // Check for search query in the headers and deserialize if present
            if (_request.Headers.ContainsKey("X-Search-Query"))
            {
                try
                {
                    var searchQuery = JsonConvert.DeserializeObject<Dictionary<string, string>>(_request.Headers["X-Search-Query"].ToString())
                                      ?? new Dictionary<string, string>();
                    paginationParams.SearchQuery = searchQuery;
                }
                catch (JsonException)
                {
                    paginationParams.SearchQuery = new Dictionary<string, string>();  // Default to empty if JSON is invalid
                }
            }

            return paginationParams;
        }


        private Specification<TEntity> CreateSpecificationFromHeaders(PaginationParameters paginationParams)
        {
            var specification = new ConcreteSpecification<TEntity>();

            specification.AddPaging(paginationParams.PageSize, paginationParams.PageNumber);

            var sortColumn = paginationParams.SortColumn;
            var sortDirection = paginationParams.SortDirection;

            var sortProperty = typeof(TEntity).GetProperty(sortColumn);
            if (sortProperty != null)
            {
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var property = Expression.Property(parameter, sortProperty.Name);
                Expression orderByExpression;
                if (property.Type != typeof(object))
                    orderByExpression = Expression.Convert(property, typeof(object));
                else
                    orderByExpression = property;
                var lambda = Expression.Lambda<Func<TEntity, object>>(orderByExpression, parameter);
                if (sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase))
                    specification.AddOrderBy(lambda, OrderType.Descending);
                else
                    specification.AddOrderBy(lambda, OrderType.Ascending);
            }

            if (paginationParams.SearchQuery != null && paginationParams.SearchQuery.Count > 0)
            {
                specification.AddSearchCriteria(paginationParams.SearchQuery);
            }
            return specification;
        }
    }
}