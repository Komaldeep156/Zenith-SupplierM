namespace Zenith.Repository.DomainModels
{
    public class Fields
    {
        public Guid SecurityGroupCode { get; set; }
        public string WindowName { get; set; }
        public string SectionName { get; set; }
        public string FieldName { get; set; }
        public bool AllowToEdit { get; set; }
        public bool AllowToView {  get; set; }
        public bool AllowToDelete { get; set; }
    }
}
