namespace OpenAI
{
    public class Model
    {
        public string ModelId { get; set; }
        public string InternalId { get; set; }

        public string AdditionalNotes { get; set; } = string.Empty;

        public Model(string model, string internalId)
        {
            ModelId = model;
            InternalId = internalId;
        }

        public Model(string model, string internalId, string additionalNotes)
        {
            ModelId = model;
            InternalId = internalId;
            AdditionalNotes = additionalNotes;
        }
    }
}
