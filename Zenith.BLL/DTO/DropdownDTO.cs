namespace Zenith.BLL.DTO
{
    public class GetDropdownListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public virtual List<GetDropdownValueDTO> Values { get; set; }
    }

    public class GetDropdownValueDTO
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }
        public string Description { set; get; }
    }

    public class DropdownValueDTO
    {
        public List<GetDropdownValueDTO> Records { get; set; }
        public string Description { set; get; }
        public Guid DropdownParentNameId { get; set; }
    }


}
