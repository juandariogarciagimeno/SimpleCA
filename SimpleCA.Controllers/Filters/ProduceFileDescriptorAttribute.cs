namespace SimpleCA.Controllers.Filters
{
    public class ProduceFileDescriptorAttribute : Attribute
    {
        public ProduceFileDescriptorAttribute(string contentType, string exampleFileName, string description)
        {
            ContentType = contentType;
            ExampleFileName = exampleFileName;
            Description = description;
        }

        public string ContentType { get; }
        public string ExampleFileName { get; }
        public string Description { get; }
    }
}
