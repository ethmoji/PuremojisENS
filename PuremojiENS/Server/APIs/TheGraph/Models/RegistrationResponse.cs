namespace PuremojiENS.Server.APIs.TheGraph.Models
{
    public class Domain
    {
        public string __typename { get; set; }
        public string name { get; set; }
    }

    public class Registrant
    {
        public string __typename { get; set; }
        public string id { get; set; }
    }

    public class Registration
    {
        public string __typename { get; set; }
        public Domain domain { get; set; }
        public string id { get; set; }
        public Registrant registrant { get; set; }
    }

    public class Data
    {
        public Registration registration { get; set; }
    }

    public class RegistrationResponse
    {
        public Data data { get; set; }
    }
}