using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using SampleProject.Infrastructure.Caching;
using SampleProject.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace SampleProject.Infrastructure.Dapper
{
    public class DapperQueryBuilder
    {
        private readonly IDapperDbConnectionFactory _dbConnectionFactory;
        private readonly ICacheService _cacheService;
        private Dictionary<string, List<JittorColumnInfo>> tableColumns = new Dictionary<string, List<JittorColumnInfo>>();
        private List<TableNode> tableNodes = new List<TableNode>();
        private readonly string _projectId;
        private static readonly Regex ValidAliasRegex = new Regex(@"^[\w]+$", RegexOptions.Compiled);  // Allow alphanumeric and underscores
        private IDictionary<string, object> paginationParamsDict = new Dictionary<string, object>();

        List<JITPageTable> tables = new List<JITPageTable>
        {
            new JITPageTable
            {
                TableID = 470,
                TableName = "Users",
                PageID = 209,
                ForView = true,
                ForOperation = true,
                SelectColumns = @"Users.Id,
                                Users.FullName,
                                Users.Email,
                                Users.PasswordHash,
                                Users.Role,
                                Users.CreatedAt",
                Filters = "[{\"operation\":\"\",\"field\":\"Users.IsDeleted\",\"operator\":\"equals\",\"value\":\"0\"}]",
                Orders = "Users.Id",
                Page = null,
                //Joins = "[{\"joinType\":\"LEFT JOIN\",\"joinTable\":\"ArticleSources\",\"parentTableColumn\":\"Users.ArticleSourceID\",\"joinTableColumn\":\"ArticleSources.ArticleSourceID\"}," +
                //        "{\"joinType\":\"LEFT JOIN\",\"joinTable\":\"ArticleStatuses\",\"parentTableColumn\":\"Users.ArticleStatusID\",\"joinTableColumn\":\"ArticleStatuses.ArticleStatusID\"}," +
                //        "{\"joinType\":\"LEFT JOIN\",\"joinTable\":\"Languages\",\"parentTableColumn\":\"Users.LanguageID\",\"joinTableColumn\":\"Languages.LanguageID\"}]",
                TableAlias = "",
                ProjectId = "123e4567-e89b-12d3-a456-426614174000",
                IsSelectable = false,
                IsDeleteable = true,
                IsUpdateable = false
            }
        };



        public DapperQueryBuilder(IDapperDbConnectionFactory dbConnectionFactory, ICacheService cacheService)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _cacheService = cacheService;
            GetAllTableStructures();
        }
        public List<JittorColumnInfo> GetTableSchema(List<string> tables, string? schemaName = "dbo")
        {
            tables = tables.Select(x => x.ToLower().Replace("pub.", "")).ToList();
            var tablesToGet = tables.Where(x => !tableColumns.Any(y => y.Key == x));

            List<JittorColumnInfo> tableColumnList = new List<JittorColumnInfo>();
            if (tablesToGet.Count() > 0)
            {
                using var tableContext = _dbConnectionFactory.GetInstance();
                var sql = $@"SELECT 
                            c.TABLE_NAME AS TableName,
                            c.COLUMN_NAME AS ColumnName,
                            c.DATA_TYPE AS DataType,
                            CASE WHEN c.DATA_TYPE IN ('int', 'bigint', 'smallint', 'tinyint', 'float', 'real') THEN c.NUMERIC_PRECISION ELSE NULL END AS NumericPrecision,
                            CASE WHEN c.DATA_TYPE IN ('int', 'bigint', 'smallint', 'tinyint', 'float', 'real') THEN c.NUMERIC_SCALE ELSE NULL END AS NumericScale,
                            c.CHARACTER_MAXIMUM_LENGTH AS MaxLength,
                            c.IS_NULLABLE AS IsNullable,
                            CASE WHEN COLUMNPROPERTY(OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity') = 1 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsAutoIncrement,
                            c.COLUMN_DEFAULT AS DefaultValue,
                            CASE WHEN EXISTS (
                                SELECT 1
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                                JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
                                ON tc.CONSTRAINT_NAME = ccu.CONSTRAINT_NAME
                                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                                AND ccu.TABLE_NAME = c.TABLE_NAME
                                AND ccu.COLUMN_NAME = c.COLUMN_NAME
                            ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsPrimaryKey,
                            CASE WHEN EXISTS (
                                SELECT 1
                                FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
                                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                                ON rc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
                                WHERE kcu.TABLE_NAME = c.TABLE_NAME
                                AND kcu.COLUMN_NAME = c.COLUMN_NAME
                            ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsForeignKey,
                            rf.ReferencedTableName,
                            rf.ReferencedTableNameColumnName,
                            ep.value AS ColumnDescription
                            FROM 
                                INFORMATION_SCHEMA.COLUMNS c
                            LEFT JOIN 
                                sys.columns AS sc
                            ON 
                                sc.object_id = OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME)
                                AND sc.name = c.COLUMN_NAME
                            LEFT JOIN 
                                sys.extended_properties AS ep
                            ON 
                                ep.major_id = sc.object_id 
                                AND ep.minor_id = sc.column_id 
                                AND ep.name = 'MS_Description'
                            LEFT JOIN (
                                SELECT 
                                fkc.parent_object_id,
                                fkc.parent_column_id,
                                OBJECT_NAME(fkc.referenced_object_id) AS ReferencedTableName,
                                (SELECT TOP 1 col.name 
                                    FROM sys.columns col 
                                    WHERE col.object_id = fkc.referenced_object_id 
				                            AND col.system_type_id IN (167, 175, 231, 35)
                                    ORDER BY col.column_id) AS ReferencedTableNameColumnName
                                FROM 
                                    sys.foreign_key_columns AS fkc
                            ) AS rf
                            ON 
                            rf.parent_object_id = OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME)
                            AND rf.parent_column_id = sc.column_id
                            WHERE c.TABLE_NAME in ({string.Join(',', tablesToGet.Select(t => $"'{t}'"))})
                            ORDER BY 
                            TableName, ColumnName";
                var newRenderedTables = tableContext.Query<JittorColumnInfo>(sql).ToList().ToList();

                foreach (var item in newRenderedTables.GroupBy(x => x.TableName))
                {
                    if (!tableColumns.ContainsKey(item.Key))
                    {
                        tableColumns.Add(item.Key.ToLower(), item.ToList());
                    }
                }
            }

            tables.ForEach(x =>
            {
                tableColumns.TryGetValue(x, out List<JittorColumnInfo>? value);
                if (value != null)
                {
                    tableColumnList.AddRange(value);
                }
            });
            return tableColumnList;
        }
        public List<JittorColumnInfo> GetTableAndChildTableColumns(string tableName, string? schemaName = "dbo")
        {
            var tablesToGet = tableName.Contains(",") ? tableName.ToLower().Split(",").ToList() : GetAllRelatedTables(tableName);
            var parentData = GetTableSchema(tablesToGet, schemaName);
            return parentData;

        }
        public DataListerResponse<dynamic>? GetPageLister(DataListerRequest request, string? externalTable = null, string? externalSelectedColumns = null, List<PageJoinModel>? externalJoins = null, string? externalScripts = null)
        {
            try
            {

                var table = new JITPageTable();
                if (externalTable != null)
                    table.TableName = externalTable;
                else
                    table = tables.FirstOrDefault(x => x.PageID == request.PageId);
                if (table == null)
                {
                    return new DataListerResponse<dynamic>();
                }

                var selectClause = string.IsNullOrEmpty(table.SelectColumns) ? (string.IsNullOrEmpty(externalSelectedColumns) ? (table.TableName + ".*") : externalSelectedColumns) : table.SelectColumns;
                var joins = externalJoins != null ? externalJoins : (JsonConvert.DeserializeObject<List<PageJoinModel>>(table.Joins ?? "[]")?.Select(x =>
                {
                    x.FixedJoin = true;
                    return x;
                }).ToList() ?? new List<PageJoinModel>());
                request.Filters = request.Filters ?? new List<PageFilterModel>();
                if (!string.IsNullOrEmpty(table.Filters))
                    request.Filters = request.Filters.Concat((JsonConvert.DeserializeObject<List<PageFilterModel>>(table.Filters ?? "[]"))?.Select(x =>
                    {
                        x.FixedFilter = true;
                        return x;
                    }) ?? new List<PageFilterModel>()).ToList();
                if (table.Orders != null || request.Sort != null)
                    request.Sort = request.Sort ?? (table.Orders ?? "");
                request.PageSize = (table.Page > 0 ? table.Page.Value : request.PageSize ?? 0);

                var newRequest = new DropdownListerRequest()
                {
                    TableName = table.TableName,
                    Joins = joins,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    Sort = request.Sort,
                    Filters = request.Filters,
                    PageId = request.PageId,
                    idColumn = request.idColumn
                };
                var listerQuery = BuildListerQuery(newRequest, selectClause, joins, externalScripts);
                using var tableContext = _dbConnectionFactory.GetInstance();
                var count = tableContext.ExecuteScalar<int>(listerQuery.CountSql);
                var list = tableContext.Query(listerQuery.Sql).ToList();
                var columnsList = listerQuery.SelectColumnList.Select(x =>
                {
                    var splittedKey = x.Split(".");
                    return new TableColumns()
                    {
                        Field = listerQuery.ColumnDictionary.GetValueOrDefault<string, string?>(x) ?? splittedKey[1],
                        HeaderName = listerQuery.ColumnDictionary.GetValueOrDefault<string, string?>(x) ?? splittedKey[1],
                        TableName = splittedKey[0],
                        Hideable = splittedKey[1] == "id" ? false : true,
                    };
                }).ToList();

                listerQuery.SelectColumnList.Add(table.TableName + ".id");
                return new DataListerResponse<dynamic>()
                {
                    Items = list,
                    PageNumber = request.PageNumber ?? 0,
                    PageSize = request.PageSize ?? 0,
                    TotalItemCount = count,
                    IsSelectable = table.IsSelectable,
                    HideAddUpdate = (table.IsUpdateable != null && table.IsUpdateable == false) ? true : false,
                    Columns = columnsList,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                _dbConnectionFactory.Dispose();
            }
        }
        public DropdownListerResponse PoplulateDropDowns(DropdownListerRequest request)
        {
            try
            {
                var joins = request.Joins ?? new List<PageJoinModel>();
                request.Filters = request.Filters ?? new List<PageFilterModel>();

                var listerQuery = BuildListerQuery(request, (request.ColumnName ?? ""), joins, null, true);

                using (var db = _dbConnectionFactory.GetInstance())
                {
                    var list = db.Query<FieldOption>(listerQuery.Sql).ToList().ToList();

                    var defaultValues = (request.Values ?? "").Split(",").ToList();
                    if (defaultValues.Any())
                        list.Where(x => defaultValues.Contains(x.Value.ToString())).ToList().ForEach(x => x.IsSelected = true);

                    return new DropdownListerResponse()
                    {
                        Items = list
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        #region Db Structure
        private List<string> GetAllRelatedTables(string mainTable)
        {
            mainTable = mainTable.ToLower();
            List<string> allRelatedTableNames = new List<string>() { mainTable };
            var childTables = GetChildTableNames(mainTable, tableNodes);
            allRelatedTableNames.AddRange(childTables);
            var parentTables = tableNodes.Where(x => x.ChildTables.Any(x => x.TableName.ToLower() == mainTable)).Select(x => x.TableName).ToList();
            foreach (var item in parentTables)
            {
                allRelatedTableNames.Add(item.ToLower());
                var itemChildTables = GetChildTableNames(mainTable, tableNodes, allRelatedTableNames);
                allRelatedTableNames.AddRange(itemChildTables);
            }
            return allRelatedTableNames;
        }
        private void GetAllTableStructures()
        {
            var relationships = _dbConnectionFactory.GetInstance()
                .Query<TableRelationship>(@"
                        SELECT 
                            TP.name AS ParentTable,
                            TR.name AS ChildTable
                        FROM 
                            sys.tables AS TP 
                        LEFT JOIN 
                            sys.foreign_keys AS FK
                            ON FK.referenced_object_id = TP.object_id
                        LEFT JOIN 
                            sys.tables AS TR ON FK.parent_object_id = TR.object_id
                        ORDER BY 
                            TP.name, TR.name").ToList();


            var tableDict = relationships
                .SelectMany(r => new[] { r.ParentTable, r.ChildTable })
                .Distinct().Where(x => x != null)
                .ToDictionary(t => t, t => new TableNode { TableName = t });

            var visited = new HashSet<string>();
            foreach (var table in tableDict.Keys)
            {
                if (!visited.Contains(table))
                {
                    BuildTableTree(table, relationships, tableDict, visited);
                }
            }
        }
        private static void BuildTableTree(string parentTable, List<TableRelationship> relationships, Dictionary<string, TableNode> tableDict, HashSet<string> visited)
        {
            if (visited.Contains(parentTable))
                return;

            visited.Add(parentTable);

            var childRelationships = relationships
                .Where(r => r.ParentTable == parentTable)
                .ToList();

            var parentNode = tableDict[parentTable];
            foreach (var rel in childRelationships.Where(x => x.ChildTable != null))
            {
                var childNode = tableDict[rel.ChildTable];
                parentNode.ChildTables.Add(childNode);
                BuildTableTree(rel.ChildTable, relationships, tableDict, visited);
            }
        }
        private static List<string> GetChildTableNames(string nodeName, List<TableNode> rootNodes, List<string>? excludeTables = null)
        {
            var childTableNames = new List<string>();
            var node = rootNodes.FirstOrDefault(n => n.TableName.ToLower() == nodeName);
            if (node != null)
            {
                foreach (var node2 in node.ChildTables.Where(x => excludeTables != null ? !excludeTables.Contains(x.TableName.ToLower()) : true))
                {
                    childTableNames.Add(node2.TableName.ToLower());
                    GetChildTableNames(node2.TableName, node2.ChildTables)
                        .ForEach(t => childTableNames.Add(t));
                }
            }
            return childTableNames;
        }
        private static string? ValidAlias(string alias)
        {
            if (ValidAliasRegex.IsMatch(alias))
            {
                return alias;
            }
            return null;
        }
        public BuildListerQueryResponse BuildListerQuery(DropdownListerRequest request, string selectClause, List<PageJoinModel>? joins = null, string? externalScripts = null, bool isDropDown = false)
        {
            List<string> JoinTypes = new List<string>() { "inner join", "outer join", "cross join", "left join", "right join" };

            selectClause = selectClause.ToLower();
            var selectColumnList = selectClause.Split(',').Select(x => x.Split(" as ")[0].Trim()).ToList();
            var asColumnDictionary = selectClause.Split(',').Select(column => column.Split(" as ")).Where(parts => parts.Length == 2).ToDictionary(parts => parts[0], parts => ValidAlias(parts[1])) ?? new Dictionary<string, string?>();

            var tableName = (request.TableName ?? "").Split(",")[0];
            request.TableName = (request.TableName ?? "").Split(".").Length == 2 ? (request.TableName ?? "").Split(".")[1] : request.TableName;

            var tables = request.TableName + (joins != null ? (joins.Count > 0 ? "," : "") + string.Join(",", joins.Select(x => x.JoinTable)) : "");
            request.TableName = (request.TableName ?? "").Split(",")[0];
            var tableColumns = GetTableAndChildTableColumns(tables ?? "", "dbo");
            selectColumnList = selectColumnList.ValidateTableColumns(tableColumns);
            request.Filters = request.Filters != null ? request.Filters.ValidateTableColumns(tableColumns) : new List<PageFilterModel>();

            var orders = (request.Sort ?? "").Split(",").Where(x => !string.IsNullOrEmpty(x)).ToList().ValidateTableColumns(tableColumns, true);

            if (selectColumnList.Any(x => x.Contains("*")))
            {
                selectColumnList.AddRange(tableColumns.Where(y => selectColumnList.Where(x => x.Contains("*")).Select(x => x.Replace(".*", "").Trim()).Contains(y.TableName)).Select(x => x.TableName + "." + x.ColumnName).ToList());
                selectColumnList.RemoveAll(x => x.Contains("*"));
                selectColumnList = selectColumnList.GroupBy(x => x.Split(".")[1]).Select(x => x.FirstOrDefault() ?? "").ToList();
            }

            string selectColumnsString = "";
            if (isDropDown)
                selectColumnsString = (selectColumnList.Count > 1 ? string.Join(" + ' - ' + ", selectColumnList.Select(column => $"ISNULL(CAST({column} as NVARCHAR(MAX)), '')")) : (selectColumnList.FirstOrDefault() ?? "")) + " as Label";
            else
            {
                selectColumnsString = string.Join(',', selectColumnList.Select(column =>
                {
                    var alias = asColumnDictionary.GetValueOrDefault<string, string?>(column);
                    return column + (alias == null ? "" : " as " + alias);
                }));
            }
            string primaryKey = "";
            if (tableColumns.FirstOrDefault(x => x.IsPrimaryKey == true & x.TableName.ToLower() == (request.TableName ?? "").ToLower()) != null)
            {
                primaryKey = request.TableName + "." + (tableColumns.FirstOrDefault(x => x.IsPrimaryKey == true & x.TableName.ToLower() == (request.TableName ?? "").ToLower())!.ColumnName ?? "") + (isDropDown ? " as Value, " : " as id, ");
            }
            else
            {
                primaryKey = request.TableName + "." + request.idColumn + (isDropDown ? " as Value, " : " as id, ");
            }
            var sql = ($"SELECT {primaryKey} {selectColumnsString} FROM {tableName} ");
            var countSql = ($"SELECT COUNT(1) FROM {tableName}");
            if (joins != null)
            {
                foreach (var join in joins.ValidateTableColumns(tableColumns))
                {
                    bool tableExists = tableColumns.Select(x => x.TableName).Contains(join.JoinTable.Replace("pub.", ""));
                    if (JoinTypes.Contains(join.JoinType.ToLower()) && tableExists)
                    {
                        sql += ($" {join.JoinType} {join.JoinTable} on {join.ParentTableColumn} = {join.JoinTableColumn} ");
                        countSql += ($" {join.JoinType} {join.JoinTable} on {join.ParentTableColumn} = {join.JoinTableColumn} ");
                    }
                }
            }

            if ((request.Filters != null && request.Filters.Count > 0) || !string.IsNullOrEmpty(externalScripts))
            {
                sql += (" WHERE ");
                countSql += (" WHERE ");
                if ((request.Filters != null && request.Filters.Count > 0))
                {
                    request.Filters.Where(x => !(x.ExternalSearch == true)).ToList().ForEach(filter =>
                    {
                        var newFilter = JsonConvert.DeserializeObject<PageFilterModel>(JsonConvert.SerializeObject(filter));
                        sql = sql.BuildWhereClause(filter, request.Filters.IndexOf(filter));
                        countSql = countSql.BuildWhereClause(newFilter ?? new PageFilterModel(), request.Filters.IndexOf(filter));
                    });
                }
                if (!string.IsNullOrEmpty(externalScripts))
                {
                    sql += (externalScripts);
                    countSql += (externalScripts);
                }
            }

            var customOrderingCases = request.CustomOrdering?.Split(",").Select(x => new { id = (x.Split(":")[0]), position = x.Split(":")[1] }).ToList() ?? null;
            string? primaryKeyColumn = request.idColumn;
            if (tableColumns.FirstOrDefault(x => x.IsPrimaryKey == true & x.TableName.ToLower() == (request.TableName ?? "").ToLower()) != null)
            {
                primaryKeyColumn = request.TableName + "." + (tableColumns.FirstOrDefault(x => x.IsPrimaryKey == true & x.TableName.ToLower() == (request.TableName ?? "").ToLower())!.ColumnName ?? "");
            }
            var orderString = orders.Count() > 0 ? string.Join(',', orders) : (primaryKeyColumn) + " DESC ";
            if (request.IsDistinct == true)
            {
                sql += ($" GROUP BY {primaryKey.ToLower().Split("as")[0].Replace(",", "")}, {selectColumnsString.ToLower().Split("as")[0].Replace(",", "")}, {orderString.Split(" ")[0]} ");
            }
            var customOrderingSQL = "";
            if (customOrderingCases != null && customOrderingCases.Count > 0)
            {
                customOrderingSQL += (" CASE ");
                customOrderingCases.ForEach(x => customOrderingSQL += ($" WHEN {primaryKeyColumn} = {x.id} THEN {x.position} "));
                customOrderingSQL += ($" ELSE {(int.Parse(customOrderingCases.OrderByDescending(x => x.position).Select(x => x.position)?.FirstOrDefault() ?? "0") + 1)} ");
                customOrderingSQL += (" END ");
                customOrderingSQL += ($" ,{(string.IsNullOrEmpty(orderString?.Split(" ")[0] ?? null) ? primaryKeyColumn : orderString?.Split(" ")[0])} ");
            }
            sql += " Order By " + ((customOrderingCases != null && customOrderingCases.Count > 0) ? customOrderingSQL : orderString);

            if (!isDropDown && request.PageSize > 0)
            {
                int offset = ((request.PageNumber ?? 0) - 1) * (request.PageSize ?? 0);
                sql += ($" OFFSET {offset} ROWS FETCH NEXT {(request.PageSize ?? 0)} ROWS ONLY ");
            }
            return new BuildListerQueryResponse()
            {
                Sql = sql,
                CountSql = countSql,
                SelectColumnList = selectColumnList,
                ColumnDictionary = asColumnDictionary
            };
        }

        #endregion
    }
    public class TableRelationship
    {
        public string ParentTable { get; set; }
        public string ChildTable { get; set; }
    }

    public class TableNode
    {
        public string TableName { get; set; }
        public List<TableNode> ChildTables { get; set; } = new List<TableNode>();
    }
}
