using SampleProject.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace SampleProject.Shared.Models
{
    public class JittorPageModel : JITPage
    {
        public JittorPageModel()
        {
            PageAttributes = new List<JITPageAttribute>();
            SearchAttributes = new List<JITPageAttribute>();
            PageAttributes.Add(new JITPageAttribute());
            PageAttributes.Add(new JITPageAttribute());
            SearchAttributes.Add(new JITPageAttribute());
            SearchAttributes.Add(new JITPageAttribute());
            PageTables = new List<JITPageTable>();
            PageTablesData = new Dictionary<string, object>();
            AttributeDisplayGroups = new List<AttributeDisplayGroup>();
            PageSections = new List<JITPageSection>();

        }
        public object SelectedRecord { get; set; }
        public object FilterModel
        {
            get
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var att in PageAttributes.Where(x => x.Displayable && x.Filterable))
                {
                    dic.Add(att.AttributeName, "");
                }
                return dic;
            }
        }
        public List<JITPageAttribute> PageAttributes { get; set; }
        public List<JITPageTable> PageTables { get; set; }
        public List<JITPageSection> PageSections { get; set; }
        public List<JITPageAttribute> SearchAttributes { get; set; }
        public Dictionary<string, object> PageTablesData { get; set; }
        public List<AttributeDisplayGroup> AttributeDisplayGroups { get; set; }
        public Dictionary<string, dynamic> ValidationRule
        {
            get
            {
                Dictionary<string, dynamic> dic = new Dictionary<string, dynamic>();
                foreach (var att in this.PageAttributes.Where(x => x.Editable))
                {
                    if (!string.IsNullOrEmpty(att.ValidationExpression))
                    {
                        object? o = JsonSerializer.Deserialize<Dictionary<string, object>>(att.ValidationExpression);
                        o = o ?? att.ValidationExpression;
                        dic.Add(att.AttributeName.ToLower(), o);
                    }
                    else if (att.IsRequired)
                    {
                        dic.Add(att.AttributeName.ToLower(), new { required = true });
                    }
                }
                return dic;
            }
        }
    }
    public class AttributeDisplayGroup : JITAttributeDisplayGroup
    {
        public string TypeName { get; set; }
        public string CssClass { get; set; }
    }
    public class ProcessEntityModel
    {
        public string TableName { get; set; } = string.Empty;
        public List<JITPageAttribute> TableAttributes { get; set; } = new List<JITPageAttribute>();
        public Dictionary<string, object> OtherValues { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object>? PrimaryValues { get; set; }
        public bool ValidToCreate { get; set; } = false;
        public bool ValidToUpdate { get; set; } = false;
        public bool ValidToDelete { get; set; } = false;
        public List<JITAttributeType> AttributeTypes { get; set; } = new List<JITAttributeType>();
        public bool InsertCompulsaryFields { get; set; }
        public Dictionary<string, object> ForeignKeyValues { get; set; }
        public string InsertCommand
        {
            get
            {
                if (!this.ValidToCreate)
                {
                    return String.Empty; // Not valid to create !!!
                }

                StringBuilder sb = new StringBuilder();
                string[] attribs = this.OtherValues.Keys.Select(x => x).ToArray();
                var parameters = Enumerable.Range(0, attribs.Length).Select(x => $"@{x}");

                // Append the additional fields to attribs
                string[] allAttribs = InsertCompulsaryFields ? attribs.Concat(new[] { "CreatedOn", "CreatedBy", "ModifiedOn", "ModifiedBy" }).ToArray() : attribs;

                // Get the index of the first additional parameter
                int startIndex = attribs.Length;

                // Append the additional parameters to parameters using dynamic indices
                var allParameters = InsertCompulsaryFields ? parameters.Concat(
                    Enumerable.Range(startIndex, 4).Select(x => $"@{x}")
                ) : parameters;

                sb.Append($"INSERT INTO {this.TableName} ({string.Join(",", allAttribs)}) VALUES({string.Join(",", allParameters)});");
                return sb.ToString();
            }
        }

        public string SelectCommand
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.ValidToCreate)
                {
                    string[] attribs = this.OtherValues.Keys.Select(x => x).ToArray();
                    var parameters = Enumerable.Range(0, attribs.Length).Select(x => $"{attribs[x]} = @{x}");
                    sb.Append($"Select * From {this.TableName} Where {string.Join(" And ", parameters)}");
                }
                else if (this.PrimaryValues != null)
                {
                    string[] attribs = this.OtherValues.Keys.Select((x, i) => $"{x} = @{i}").ToArray();
                    string[] whClaus = this.PrimaryValues.Keys.Select((x, i) => $"{x} = @{i + attribs.Length}").ToArray();
                    sb.Append($"Select * From {this.TableName} Where {string.Join(" AND ", whClaus)}");
                }
                return sb.ToString();
            }
        }

        public string UpdateCommand
        {
            get
            {
                if (!this.ValidToUpdate || this.PrimaryValues == null)
                {
                    return String.Empty; //Not valid to create !!!
                }
                StringBuilder sb = new StringBuilder();
                string[] attribs = this.OtherValues.Keys.Select((x, i) => $"{x} = @{i}").ToArray();
                string[] whClaus = this.PrimaryValues.Keys.Select((x, i) => $"{x} = @{i + attribs.Length}").ToArray();
                sb.AppendLine($"Update {this.TableName} Set {string.Join(",", attribs)} ");
                sb.AppendLine($"Where {string.Join(" AND ", whClaus)}");
                return sb.ToString();
            }
        }
        public object[] InsertParamaters
        {
            get
            {
                List<object> list = new List<object>();
                foreach (var key in this.OtherValues.Keys)
                {
                    var attrib = this.TableAttributes.Where(x => x.AttributeName == key).First();
                    var t = this.AttributeTypes.Where(x => x.AttributeTypeID == attrib.AttributeTypeID).First();
                    if (ForeignKeyValues.ContainsKey(key))
                    {
                        list.Add(ForeignKeyValues[key]);
                    }
                    else if (OtherValues[key] == null)
                    {
                        list.Add(t.GetDefaultValue());
                    }
                    else
                    {
                        list.Add(t.GetDefaultValue(OtherValues[key]));
                    }
                }

                // Add the additional field values to the parameters list
                if (InsertCompulsaryFields)
                {
                    list.Add(DateTime.Now);  // CreatedOn
                    list.Add(11000);         // CreatedBy
                    list.Add(DateTime.Now);  // ModifiedOn
                    list.Add(11000);         // ModifiedBy
                }

                return list.ToArray();
            }
        }
        public object[] UpdateParamaters
        {
            get
            {
                List<object> list = new List<object>();

                foreach (var key in this.OtherValues.Keys)
                {
                    var attrib = this.TableAttributes.Where(x => x.AttributeName == key).First();
                    var t = this.AttributeTypes.Where(x => x.AttributeTypeID == attrib.AttributeTypeID).First();
                    if (ForeignKeyValues.ContainsKey(key))
                    {
                        list.Add(ForeignKeyValues[key]);
                    }
                    else if (OtherValues[key] == null)
                    {
                        list.Add(t.GetDefaultValue());
                    }
                    //else if(OtherValues[key] == "ModifiedOn")
                    //{
                    //    list.Add(t.GetDefaultValue(OtherValues[key]));
                    //}
                    else
                    {
                        list.Add(t.GetDefaultValue(OtherValues[key]));
                    }
                }
                if (PrimaryValues != null)
                    foreach (var key in PrimaryValues.Keys)
                    {
                        var attrib = this.TableAttributes.Where(x => x.AttributeName == key).First();
                        var t = this.AttributeTypes.Where(x => x.AttributeTypeID == attrib.AttributeTypeID).First();
                        list.Add(t.GetDefaultValue(PrimaryValues[key]));
                    }
                return list.ToArray();
            }
        }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public static List<ProcessEntityModel> ProcessFrom(Dictionary<string, object> keyValuePairs, JittorPageModel model, List<JITAttributeType> types)
        {
            List<ProcessEntityModel> list = new List<ProcessEntityModel>();
            foreach (var table in model.PageTables.Where(x => x.ForOperation))
            {
                ProcessEntityModel entity = new ProcessEntityModel();
                entity.TableName = table.TableName;
                entity.ValidToCreate = true;
                entity.AttributeTypes = types;
                entity.TableAttributes = model.PageAttributes.Where(x => x.TableID == table.TableID).ToList();
                foreach (var att in entity.TableAttributes)
                {
                    if (keyValuePairs.ContainsKey(att.AttributeName))
                    {
                        if (att.IsPrimaryKey)
                        {
                            if (keyValuePairs[att.AttributeName] != null && keyValuePairs.TryGetValue(att.AttributeName, out var value))
                            {
                                if (int.TryParse(value?.ToString() ?? "0", out int intValue) && intValue != 0)
                                {
                                    entity.PrimaryValues = entity.PrimaryValues ?? new Dictionary<string, object>();
                                    entity.PrimaryValues.Add(att.AttributeName, keyValuePairs[att.AttributeName]);
                                    entity.ValidToUpdate = true;
                                    entity.ValidToCreate = false;
                                    entity.ValidToDelete = model.DeleteRecord && string.IsNullOrEmpty(model.SoftDeleteColumn);
                                }
                            }
                        }
                        else
                        {
                            entity.OtherValues.Add(att.AttributeName, keyValuePairs[att.AttributeName]);
                        }
                    }
                    else if (att.IsRequired && !att.IsPrimaryKey && string.IsNullOrEmpty(att.DefaultValue))
                    {
                        entity.ValidToCreate = false;
                        entity.ErrorMessages.Add($"{att.DisplayNameEn} is required");
                    }
                    else if (att.IsRequired == false && (!string.IsNullOrEmpty(att.DefaultValue) || model.PageTables.Any(x => x.TableName == att.ParentTableName)))
                    {
                        entity.OtherValues.Add(att.AttributeName, att.DefaultValue);
                    }
                }
                if (entity.OtherValues.Count == 0)
                {
                    entity.ValidToCreate = false;
                }
                list.Add(entity);
            }
            return list;
        }



        public class jitterDeleteModel
        {
            public List<string> ColumnNames { get; set; }
            public List<object> IdValues { get; set; }

        }
        public class SortableItem
        {
            public int DisplaySeqNo { get; set; }
            // Add other properties if needed
        }

    }
    public class JittorColumnInfo
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
        public bool IsAutoIncrement { get; set; }
        public string DefaultValue { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public string ColumnDescription { get; set; }
        public int MaxLength { get; set; }
        public int NumericPrecision { get; set; }
        public int NumericScale { get; set; }
        public string ReferencedTableName { get; set; }
        public string ReferencedTableNameColumnName { get; set; }
    }


    public class JittorQueryInfo
    {
        public List<string> SelectColumns { get; set; }
        public Dictionary<string, object> Filters { get; set; }
        public Dictionary<string, string> Orders { get; set; }
        public JittorQueryPageInfo Page { get; set; }
        public List<JittorQueryForeignKeys> ForeignKeys { get; set; }
    }

    public class JittorQueryPageInfo
    {
        public int PageSize { get; set; }
        public int PageNo { get; set; }
    }
    public class JittorQueryForeignKeys
    {
        public string? ForeignKey { get; set; }
        public string? ForeignTableName { get; set; }
        public string? ForeignTablePrimaryKey { get; set; }
    }


    public class DataListerRequest
    {
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string? Sort { get; set; }
        public List<PageFilterModel>? Filters { get; set; } = null;
        public int? PageId { get; set; }
        public string? idColumn { get; set; }
    }
    public class DropdownListerRequest : DataListerRequest
    {
        public bool? IsArgaamContext { get; set; }
        public string? ColumnName { get; set; }
        public string? TableName { get; set; }
        public List<PageJoinModel>? Joins { get; set; }
        public bool? IsDistinct { get; set; }
        public bool? TryConvert { get; set; }
        public string? Values { get; set; }
        public string? CustomOrdering { get; set; }

    }

    public class DataListerResponse<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public long TotalItemCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<TableColumns> Columns { get; set; }
        public bool? IsSelectable { get; set; }
        public bool? HideAddUpdate { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItemCount / PageSize);

    }
    public class DropdownListerResponse
    {
        public List<FieldOption> Items { get; set; } = new List<FieldOption>();
    }
    public class BuildListerQueryResponse
    {
        public string Sql { get; set; }
        public string CountSql { get; set; }
        public List<string> SelectColumnList { get; set; }
        public Dictionary<string, string?> ColumnDictionary { get; set; }
    }

    public class JitPageTableExtended : JITPageTable
    {
        [Column] public string UrlFriendlyName { get; set; }
    }
}
