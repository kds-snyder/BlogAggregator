namespace BlogAggregator.Core.Models
{
    public class ExternalLoginModel
    {
        public int ExternalLoginID { get; set; }

        public int UserID {get; set;}

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }      
    }
}
