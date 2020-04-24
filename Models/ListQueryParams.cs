using System.Runtime.Serialization;

namespace Blackbaud.HeadlessDataSync.Models
{
    [DataContract]
    public class ListQueryParams
    {
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(LastModified) && string.IsNullOrEmpty(SortToken);
        }

        [DataMember(Name = "last_modified")]
        public string LastModified { get; set; }

        [DataMember(Name = "sort_token")]
        public string SortToken { get; set; }
    }
}
