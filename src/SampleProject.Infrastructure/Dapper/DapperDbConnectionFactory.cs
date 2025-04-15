using System;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using Finbuckle.MultiTenant.Abstractions;
using SampleProject.Infrastructure.Tenant;
using Microsoft.Data.SqlClient;

namespace SampleProject.Infrastructure.Dapper
{
    public interface IDapperDbConnectionFactory
    {
        IDbConnection GetInstance();
    }

    public class DapperDbConnectionFactory : IDapperDbConnectionFactory, IDisposable
    {
        private readonly string _connectionString;
        private readonly AsyncLocal<IDbConnection?> _connection = new();
        private readonly ILogger<DapperDbConnectionFactory> _logger;

        public DapperDbConnectionFactory(ITenantService tenantInfo, ILogger<DapperDbConnectionFactory> logger)
        {
            _connectionString = tenantInfo?.GetTenant()?.ConnectionString ?? "";
            _logger = logger;
        }

        public IDbConnection GetInstance()
        {
            if (_connection.Value == null)
            {
                _logger.LogInformation("Creating new database connection.");
                var connection = new SqlConnection(_connectionString);
                connection.Open();
                _connection.Value = connection;
            }
            return _connection.Value;
        }

        public void Dispose()
        {
            if (_connection.Value != null)
            {
                _logger.LogInformation("Disposing database connection.");
                _connection.Value.Dispose();
                _connection.Value = null;
            }
        }
    }
}