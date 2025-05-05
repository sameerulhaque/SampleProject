using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SampleProject.Shared.Models
{
    public enum ApplicationFieldTypeEnum
    {
        INPUT = 1,
        TEXT_AREA = 2,
        SELECT = 3,
        AUTO_COMPLETE = 4,
        RADIO_BUTTON = 5,
        SWITCH = 6,
        CHECKBOX = 7,
        DATE_TIME = 8,
        SELECTABLE_LIST = 9,
        TREEVIEW_LIST = 10,
        ADD_REMOVE_SEARCH_TABLE = 11
    }
    public enum ApplicationFieldSubTypeEnum
    {
        NONE = 1,
        SINGLE = 2,
        MULTI = 3,
        TEXT = 4,
        PASSWORD = 5,
        NUMBER = 6,
        DATE = 7,
        TIME = 8,
        DATE_TIME = 9
    }
    public enum ApplicationValueTypeEnum
    {
        STRING = 1,
        NUMBER = 2,
        BOOL = 3,
        OBJECT = 4,
        ARRAY = 5
    }

    public class FieldModel
    {
        public string? CssID { get; set; }
        public string? CssClasses { get; set; }
        public string? HelperText { get; set; }
        public string? PlaceholderEn { get; set; }
        public string? PlaceholderAr { get; set; }
        public string? Id { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsVisible { get; set; }
        public ApplicationFieldTypeEnum FieldType { get; set; }
        public ApplicationFieldSubTypeEnum FieldSubType { get; set; }
        public FieldValue InpValue { get; set; }
        public List<FieldOption>? Options { get; set; }
        public List<ValidationRule>? Validations { get; set; }
        public List<FieldAction>? Actions { get; set; }
        public string TableName { get; set; }
        public string LabelAr { get; set; }
        public string LabelEn { get; set; }
        public int? AttributeTypeId { get; set; }
        public string? ParentTableName { get; set; }
        public string? ParentTableNameColumn { get; set; }
        public string? ParentCondition { get; set; }
        public int DisplayableSeqNo { get; set; }
        public int? SearchableSeqNo { get; set; }
        //public JittorColumnInfo? CurrentColumn { get; set; }
        public int? PageId { get; set; }
        public int? TableId { get; set; }
        public int SectionId { get; set; }
        public string? ProjectId { get; set; }
        public bool IsRequired { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsAutoIncreament { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool Searchable { get; set; }
        public int MaxLength { get; set; }
        public List<FieldOption> HardCodedOptions { get; set; }

        public FieldModel()
        {
            Options = new List<FieldOption>();
            Validations = new List<ValidationRule>();
            Actions = new List<FieldAction>();
            HardCodedOptions = new List<FieldOption>();
            InpValue = new FieldValue();
        }
    }
    public class FieldOption
    {
        public int Value { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
        public bool IsSelected { get; set; }
    }
    public class FieldValue
    {
        public string? ActualValue { get; set; }
        public ApplicationValueTypeEnum ValueType { get; set; }
    }
    public class FieldAction
    {
        public int ApplyOn { get; set; }
        public string TargetSection { get; set; }
        public string TargetInput { get; set; }
        public int ActionType { get; set; }
        public int RunOn { get; set; }
    }
    public class FormPageModel
    {
        public FormPageModel()
        {
            Sections = new List<FormSection>();
        }
        public Form Form { get; set; }
        public List<FormSection> Sections { get; set; }
        public string? ProjectId { get; set; }
    }
    public class Form
    {
        public Form()
        {
            FormTables = new List<FormTable>();
        }
        public string FormName { get; set; }
        public List<string>? ClassesName { get; set; }
        public string? SoftDeleteColumn { get; set; }
        public bool ShowListing { get; set; }
        public bool ShowSearch { get; set; }
        public string? ListingTitle { get; set; }

        public string? Description { get; set; }
        public bool ShowFilters { get; set; }
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string TableName { get; set; }
        public string? ListerTableName { get; set; }
        public string? TableAlias { get; set; }
        public string? SelectColumns { get; set; }
        public string? Filters { get; set; }
        public string? Orders { get; set; }
        public string? Joins { get; set; }
        public string? Extender { get; set; }
        public int PageID { get; set; }
        public string? ProjectId { get; set; }
        public bool ForView { get; set; }
        public List<FormTable>? FormTables { get; set; }
    }
    public class FormSection
    {
        public FormSection()
        {
            Fields = new List<FieldModel>();
        }
        public int PageSectionId { get; set; }
        public string Label { get; set; }
        public string? CssId { get; set; }
        public string? CssClasses { get; set; }
        public bool IsVisible { get; set; }
        public int DisplayableSeqNo { get; set; }
        public List<FieldModel> Fields { get; set; }
        public string? ProjectId { get; set; }
        public int PageID { get; set; }

    }
    public class FormTable
    {
        public string Value { get; set; }
        public string Label { get; set; }

    }

    public class ResponseModel
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public string Page { get; set; }
        public object Data { get; set; } // Use a specific type instead of object if you know the type of Data
        public int Created { get; set; }
        public int Updared { get; set; }
    }
    public class FormBuilderListerModel
    {
        public string PageName { get; set; }
        public string UrlFriendlyName { get; set; }
        public string Title { get; set; }
        public string ForOperation { get; set; }
        public string ForView { get; set; }
    }
    public class PageFilterModel
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string? Value { get; set; }
        public string Operation { get; set; }
        public bool? ExternalSearch { get; set; }
        public string? TryConvertType { get; set; }
        public bool? FixedFilter { get; set; }
    }
    public class PageJoinModel
    {
        public string JoinType { get; set; }
        public string JoinTable { get; set; }
        public string ParentTableColumn { get; set; }
        public string JoinTableColumn { get; set; }
        public bool? FixedJoin { get; set; }
    }
    public class ValidationRule
    {
        public string Type { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class TableColumns
    {
        public string Field { get; set; }
        public string HeaderName { get; set; }
        public string TableName { get; set; }
        public bool Hideable { get; set; }
    }
}
