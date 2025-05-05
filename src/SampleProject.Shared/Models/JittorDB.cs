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
    public partial class JITAttributeType
    {
        [Column] public int AttributeTypeID { get; set; }
        [Column] public string TypeName { get; set; }
        [Column] public int DBTypeID { get; set; }
        [Column] public string DotNetType { get; set; }
        [Column] public string DotNetAlias { get; set; }

        public object GetDefaultValue()
        {
            switch (this.DotNetAlias)
            {
                case "String": return string.Empty;
                case "Boolean": return false;
                case "DateTime": return DateTime.Now;
                case "Byte": return new byte();
                case "Byte[]": return new byte[] { };
                case "DateTimeOffset": return DateTimeOffset.Now;
                case "Decimal": return 0.0M;
                case "Double": return 0.0M;
                case "Int16": return 0;
                case "Int32": return 0;
                case "Int64": return 0;
                case "Object ": return new object();
                case "Single": return 0;
                case "TimeSpan": return new TimeSpan();
                case "Xml": return string.Empty;
                default: return null;
            }
        }
        public object GetDefaultValue(object value)
        {
            switch (this.DotNetAlias)
            {
                case "String": return value.ToString();
                case "Boolean": return bool.Parse(value.ToString());
                case "DateTime": return DateTime.Parse(value.ToString());
                case "Byte": return byte.Parse(value.ToString());
                case "Byte[]": return (byte[])value;
                case "DateTimeOffset": return (DateTimeOffset)value;
                case "Decimal": return decimal.Parse(value.ToString());
                case "Double": return double.Parse(value.ToString());
                case "Int16": return Int16.Parse(value.ToString());
                case "Int32": return Int32.Parse(value.ToString());
                case "Int64": return Int64.Parse(value.ToString());
                case "Object ": return value;
                case "Single": return int.Parse(value.ToString());
                case "TimeSpan": return TimeSpan.Parse(value.ToString());
                case "Xml": return value.ToString();
                default: return null;
            }
        }
    }

    public partial class JITPageAttribute
    {

        [Column] public int PageAttributeID { get; set; }
        [Column] public string AttributeName { get; set; }
        [Column] public string DisplayNameAr { get; set; }
        [Column] public string DisplayNameEn { get; set; }
        [Column] public int PageID { get; set; }
        [Column] public int TableID { get; set; }
        [Column] public int AttributeTypeID { get; set; }
        [Column] public bool IsRequired { get; set; }
        [Column] public bool IsForeignKey { get; set; }
        [Column] public string ParentTableName { get; set; }
        [Column] public bool? AutoComplete { get; set; }
        [Column] public string ParentTableNameColumn { get; set; }
        [Column] public string ParentCondition { get; set; }
        [Column] public bool? AddNewParentRecord { get; set; }

        [Column] public string ValidationExpression { get; set; }
        [Column] public bool IsAutoIncreament { get; set; }
        [Column] public bool IsPrimaryKey { get; set; }
        [Column] public bool Editable { get; set; }
        [Column] public bool Searchable { get; set; }
        [Column] public bool Displayable { get; set; }
        [Column] public bool Sortable { get; set; }
        [Column] public bool Filterable { get; set; }
        [Column] public int EditableSeqNo { get; set; }
        [Column] public int SearchableSeqNo { get; set; }
        [Column] public int DisplayableSeqNo { get; set; }
        [Column] public int MaxLength { get; set; }
        [Column] public string PlaceholderText { get; set; }
        [Column] public string DisplayFormat { get; set; }
        [Column] public string InputPartialView { get; set; }
        [Column] public string DefaultValue { get; set; }
        [Column] public bool? IsRange { get; set; }
        [Column] public string Range { get; set; }
        [Column] public int DisplayGroupID { get; set; }
        [Column] public string DisplayStyle { get; set; }
        [Column] public bool? IsFile { get; set; }
        [Column] public bool? AllowListInput { get; set; }
        [Column] public string UploadPath { get; set; }
        [Column] public string FileType { get; set; }
        [Column] public string PartialURLTemplate { get; set; }
        [Column] public string AlternateValues { get; set; }
        public List<ForigenValue> ForigenValues { get; set; }
        [Column] public string AlternameValuesQuery { get; set; }
        [Column] public string HasChildLookUp { get; set; }
        [Column] public string LookUpQuery { get; set; }
        [Column] public string HasLookUpData { get; set; }
        [Column] public string LookUpColumn { get; set; }
        [Column] public string LookUpTable { get; set; }
        [Column] public string ProjectId { get; set; }
        [Column] public int FieldType { get; set; }
        [Column] public int FieldSubType { get; set; }
        [Column] public string FieldActions { get; set; }
        [Column] public int SectionId { get; set; }
        [Column] public string HardCodedOptions { get; set; }
    }

    public partial class JITPage
    {
        [Column] public int PageID { get; set; }
        [Column] public string PageName { get; set; }
        [Column] public string UrlFriendlyName { get; set; }
        [Column] public string Title { get; set; }
        [Column] public int GroupID { get; set; }
        [Column] public int RecordsPerPage { get; set; }
        [Column] public int CurrentPage { get; set; }
        [Column] public bool AddNew { get; set; }
        [Column] public bool EditRecord { get; set; }
        [Column] public bool DeleteRecord { get; set; }
        [Column] public string SoftDeleteColumn { get; set; }
        [Column] public bool Preview { get; set; }
        [Column] public bool ShowSearch { get; set; }
        [Column] public bool ShowFilters { get; set; }
        [Column] public bool ShowListing { get; set; }
        [Column] public bool IsSelectable { get; set; }
        [Column] public string ListingPartialView { get; set; }
        [Column] public string ListingTitle { get; set; }
        [Column] public string OrderBy { get; set; }
        [Column] public string ConditionalClause { get; set; }
        [Column] public string Extender { get; set; }
        [Column] public string Description { get; set; }
        [Column] public string PageView { get; set; }
        [Column] public string ListingCommands { get; set; }
        [Column] public bool InsertCompulsaryFields { get; set; }
        [Column] public string ProjectId { get; set; }
    }


    public partial class JITPagesGroup
    {
        [Column] public int GroupID { get; set; }
        [Column] public int? ParentGroupID { get; set; }
        [Column] public string GroupName { get; set; }
        [Column] public string Icon { get; set; }
    }

    public partial class JITPageSection
    {
        [Column] public int PageSectionId { get; set; }
        [Column] public string Label { get; set; }
        [Column] public string Id { get; set; }
        [Column] public string Classes { get; set; }
        [Column] public bool? IsVisible { get; set; }
        [Column] public int? DisplayableSeqNo { get; set; }
        [Column] public int PageId { get; set; }
        [Column] public string ProjectId { get; set; }
    }

    public partial class JITPageTable
    {
        [Column] public int TableID { get; set; }
        [Column] public string TableName { get; set; }
        [Column] public string TableAlias { get; set; }
        [Column] public int PageID { get; set; }
        [Column] public bool ForView { get; set; }
        [Column] public bool ForOperation { get; set; }
        [Column] public bool IsSelectable { get; set; }
        [Column] public string SelectColumns { get; set; }
        [Column] public string Filters { get; set; }
        [Column] public string Orders { get; set; }
        [Column] public string Joins { get; set; }
        [Column] public int? Page { get; set; }
        [Column] public string ProjectId { get; set; }
        [Column] public bool? IsUpdateable { get; set; }
        [Column] public bool? IsDeleteable { get; set; }

    }

    public partial class JITAttributeDisplayGroup
    {

        [Column] public int DisplayGroupID { get; set; }
        [Column] public string GroupName { get; set; }
        [Column] public string Title { get; set; }
        [Column] public int DisplayGroupTypeID { get; set; }
        [Column] public int DisplaySeqNo { get; set; }
        [Column] public bool EnableDefaultValues { get; set; }
    }


    public partial class JITDisplayGroupType 
    {
        [Column] public int DisplayGroupTypeID { get; set; }
        [Column] public string TypeName { get; set; }
        [Column] public string CssClass { get; set; }
    }


    public partial class EventLogs
    {

        [Column] public int EventLogId { get; set; }
        [Column] public string? EntityType { get; set; }
        [Column] public int RowID { get; set; }
        [Column] public string? OperationType { get; set; }
        [Column] public string? LogMessage { get; set; }
        [Column] public int UserId { get; set; }
        [Column] public byte[] OldRecord { get; set; }

        [Column] public byte[] Changes { get; set; }
        [Column] public DateTime CreatedOn { get; set; }

    }


    public partial class AppConfig
    {


        [Column] public string AppConfigKey { get; set; }

        [Column] public string AppConfigValue { get; set; }

        [Column] public bool IsEditableByUser { get; set; }

        [Column] public string Description { get; set; }
    }


 
    public partial class AppExceptionAdditionalDatum
    {


        [Column] public int AppExceptionID { get; set; }

        [Column] public string Variable { get; set; }

        [Column] public string Value { get; set; }
    }


    public partial class AppException
    {


        [Column] public int AppExceptionID { get; set; }

        [Column] public string SourceApp { get; set; }

        [Column] public DateTime OccuredOn { get; set; }

        [Column] public string Message { get; set; }

        [Column] public string OriginatedAt { get; set; }

        [Column] public string StackTrace { get; set; }

        [Column] public string InnerExceptionMessage { get; set; }

        [Column] public string HostMachine { get; set; }

        [Column] public string Level { get; set; }

        [Column] public string CustomMessage { get; set; }
    }
    public partial class ForigenValue
    {
        [Column] public int ID { get; set; }
        [Column] public string Name { get; set; }

        [Column]
        public string Value
        {
            get; set;

        }
    }
}
