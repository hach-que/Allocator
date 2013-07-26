namespace AllocatorLib
{
    public class Specification
    {
        public string ResourceType { get; private set; }
        public dynamic Requirements { get; private set; }

        public Specification(string resourceType, dynamic requirements)
        {
            this.ResourceType = resourceType;
            this.Requirements = requirements;
        }
    }
}
