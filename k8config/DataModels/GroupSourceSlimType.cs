namespace k8config.DataModels
{
    public class GroupSourceSlimType
    {
        public string name { get; set;  }
        public bool isRequired { get; set; }
        public FieldFormat format { get; set; }
        public FieldType type { get; set; }
    }
}
