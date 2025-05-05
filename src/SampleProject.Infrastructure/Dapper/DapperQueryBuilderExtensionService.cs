


using SampleProject.Shared.Models;

namespace SampleProject.Infrastructure.Dapper
{
    public static class DapperQueryBuilderExtensionService
    {
        public static ApplicationValueTypeEnum GetApplicationValueTypeEnum(this string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "varchar":
                case "nvarchar":
                case "char":
                case "nchar":
                case "text":
                case "ntext":
                    return ApplicationValueTypeEnum.STRING;

                case "int":
                case "bigint":
                case "smallint":
                case "tinyint":
                case "decimal":
                case "numeric":
                case "float":
                case "real":
                    return ApplicationValueTypeEnum.NUMBER;

                case "bit":
                    return ApplicationValueTypeEnum.BOOL;

                default:
                    return ApplicationValueTypeEnum.OBJECT;
            }
        }
        public static ApplicationFieldSubTypeEnum GetApplicationFieldSubTypeEnum(this string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "varchar":
                case "nvarchar":
                case "char":
                case "nchar":
                case "text":
                case "ntext":
                    return ApplicationFieldSubTypeEnum.TEXT;

                case "int":
                case "bigint":
                case "smallint":
                case "tinyint":
                case "decimal":
                case "numeric":
                case "float":
                case "real":
                    return ApplicationFieldSubTypeEnum.NUMBER;

                case "date":
                    return ApplicationFieldSubTypeEnum.DATE;

                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return ApplicationFieldSubTypeEnum.DATE_TIME;

                case "time":
                    return ApplicationFieldSubTypeEnum.TIME;

                case "bit":
                    return ApplicationFieldSubTypeEnum.SINGLE;

                default:
                    return ApplicationFieldSubTypeEnum.TEXT;
            }
        }

        public static ApplicationFieldTypeEnum GetApplicationFieldTypeEnum(this string sqlType)
        {
            switch (sqlType.ToLower())
            {
                case "varchar":
                case "nvarchar":
                case "char":
                case "nchar":
                case "text":
                case "ntext":
                case "int":
                case "bigint":
                case "smallint":
                case "tinyint":
                case "decimal":
                case "numeric":
                case "float":
                case "real":
                    return ApplicationFieldTypeEnum.INPUT;

                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "time":
                    return ApplicationFieldTypeEnum.DATE_TIME;

                case "bit":
                    return ApplicationFieldTypeEnum.CHECKBOX;

                default:
                    return ApplicationFieldTypeEnum.INPUT;
            }
        }

        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        public static Dictionary<string, object> GetValuesFromDynamicDictionary(dynamic item, List<string> keys)
        {
            var results = new Dictionary<string, object>();
            if (item is IDictionary<string, object> dictionary)
            {
                foreach (var key in keys)
                {
                    results.Add(key, dictionary[key]);
                }
            }
            return results;
        }
        public static string BuildWhereClause(this string sql, PageFilterModel filter, int index)
        {
            switch (filter.Operator.ToLower())
            {
                case "contains":
                    filter.Operator = "LIKE";
                    filter.Value = "%" + Uri.UnescapeDataString(filter.Value ?? "") + "%";
                    break;

                case "equals":
                    filter.Operator = "=";
                    filter.Value = Uri.UnescapeDataString(filter.Value ?? "");
                    break;

                case "startswith":
                    filter.Operator = "LIKE";
                    filter.Value = Uri.UnescapeDataString(filter.Value ?? "") + "%";
                    break;

                case "endswith":
                    filter.Operator = "LIKE";
                    filter.Value = "%" + Uri.UnescapeDataString(filter.Value ?? "");
                    break;

                case "isempty":
                    filter.Operator = "=";
                    filter.Value = "";
                    break;

                case "isnotempty":
                    filter.Operator = $"<>";
                    filter.Value = "";
                    break;

                case "isnotnullorempty":
                    filter.Operator = $"IS NOT NULL AND {filter.Field} <>";
                    filter.Value = "";
                    break;

                case "isnullorempty":
                    filter.Operator = $"IS NULL AND {filter.Field} <>";
                    filter.Value = "";
                    break;

                case "isnull":
                    filter.Operator = $"IS NULL";
                    filter.Value = "";
                    break;

                case "isanyof":
                    filter.Operator = "IN";
                    filter.Value = Uri.UnescapeDataString(filter.Value ?? "");
                    break;

                case "isnotanyof":
                    filter.Operator = "NOT IN";
                    filter.Value = Uri.UnescapeDataString(filter.Value ?? "");
                    break;

                case "greaterorequal":
                    filter.Operator = ">=";
                    filter.Value = Uri.UnescapeDataString(filter.Value ?? "");
                    break;
                case "lessorequal":
                    filter.Operator = "<=";
                    filter.Value = Uri.UnescapeDataString(filter.Value ?? "");
                    break;

                case "greater":
                    filter.Operator = ">";
                    filter.Value = Uri.UnescapeDataString(filter.Value ?? "");
                    break;
                case "less":
                    filter.Operator = "<";
                    filter.Value = Uri.UnescapeDataString(filter.Value ?? "");
                    break;

                default:
                    return sql;
            }
            if(filter.Operator == "IN" || filter.Operator == "NOT IN")
            {
                var param = filter.Value!.Split(',').Select((x, index) => "@" + index);
                var values = filter.Value!.Split(',').Select((x, index) => x);
                sql += $" {(index == 0 ? "" : filter.Operation)} ({filter.Field} {filter.Operator} ( {values} )) ";
                return sql;
            }
            sql += ($" {(index == 0 ? "" : string.IsNullOrEmpty(filter.Operation) ? "AND" : filter.Operation)} ({filter.Field} {filter.Operator} {(filter.Operator == "IS NULL" ? "" : ((string.IsNullOrEmpty(filter.TryConvertType) ? (filter.Value ?? "") : $"TRY_CONVERT({filter.TryConvertType}, {filter.Value ?? ""})")))} ) ");
            return sql;
        }

        public static List<string> ValidateTableColumns(this List<string> value, List<JittorColumnInfo> columns, bool isOrderBy = false)
        {
            return value.Where(item =>
            {
                var parts = item.Split('.');
                if (isOrderBy)
                    parts[1] = parts[1].ToLower().Replace("asc", "").Replace("desc", "");
                return parts.Length == 2 && columns.Any(x => {
                    if(x.TableName.ToLower() == parts[0].Trim().ToLower() && (x.ColumnName.ToLower() == parts[1].Trim().ToLower() || parts[1].Trim() == "*"))
                    {
                        return true;
                    }
                    return false;
                    });
            }).ToList();
        }
        public static List<PageFilterModel> ValidateTableColumns(this List<PageFilterModel> value, List<JittorColumnInfo> columns)
        {
            return value.Where(item =>
            {
                var parts = item.Field.Split('.');
                return parts.Length == 2 && columns.Any(x => x.TableName.ToLower() == parts[0].Trim().ToLower() && (x.ColumnName.ToLower() == parts[1].Trim().ToLower() || parts[1].Trim() == "*"));
            }).ToList();
        }
        public static List<PageJoinModel> ValidateTableColumns(this List<PageJoinModel> value, List<JittorColumnInfo> columns)
        {
            return value.Where(item =>
            {
                var part1 = item.ParentTableColumn.Split('.');
                var part2 = item.JoinTableColumn.Split('.');

                bool validated = part1.Length == 2 && columns.Any(x => x.TableName.ToLower() == part1[0].Trim().ToLower() && (x.ColumnName.ToLower() == part1[1].Trim().ToLower() || part1[1].Trim() == "*"));
                if (!validated)
                {
                    return validated;
                }
                validated = part2.Length == 2 && columns.Any(x => x.TableName.ToLower() == part2[0].Trim().ToLower() && (x.ColumnName.ToLower() == part2[1].Trim().ToLower() || part2[1].Trim() == "*"));
                return validated;
            }).ToList();
        }

        public static T? GetValidationParams<T>(this List<ValidationRule> validations, string type, string param)
        {
            var validation = validations.FirstOrDefault(x => x.Type == type);
            if (validation != null && validation.Type == type && validation.Parameters != null && validation.Parameters.TryGetValue(param, out var value))
            {
                if (value is T typedValue)
                {
                    return typedValue;
                }
            }
            return default;
        }
        public static List<ValidationRule> SetValidationParams(this List<ValidationRule> validations, string type, string message, Dictionary<string, object>? parameters = null)
        {
            var validation = validations.FirstOrDefault(x => x.Type == type);

            if (validation != null)
            {
                validation.ErrorMessage = message;

                if (validation.Parameters == null)
                    validation.Parameters = new Dictionary<string, object>();

                if (parameters != null)
                {
                    foreach (var kvp in parameters)
                    {
                        validation.Parameters[kvp.Key] = kvp.Value;
                    }
                }
            }
            else
            {
                validation = new ValidationRule
                {
                    Type = type,
                    ErrorMessage = message,
                    Parameters = parameters ?? new Dictionary<string, object>()
                };
                validations.Add(validation);
            }
            return validations;
        }


    }


    public class ValidationParameters
    {
        public int? MaxLength { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

}
