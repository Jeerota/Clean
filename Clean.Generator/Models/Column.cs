namespace Clean.Generator.Models
{
    public class Column
    {
        public string Name { get; set; }
        public string? DataType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimaryKey { get; set; }

        public int? IdentityIncrement { get; set; }
        public int? IdentitySeed { get; set; }
        public int? Length{ get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }

        public string? DefualtValue { get; set; }

        public Column(string name)
        {
            Name = name;
        }

        public bool Validate()
        {
            if (DataType == null)
                return false;
            return true;
        }
    }
}
