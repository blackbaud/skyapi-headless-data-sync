using System;
using System.Runtime.Serialization;

namespace Blackbaud.HeadlessDataSync.Models
{
    [DataContract]
    public class StorageData
    {
        [DataMember(Name = "last_sync_date")]
        public DateTimeOffset LastSyncDate { get; set; }

        [DataMember(Name = "constituent_query_params")]
        public ListQueryParams ConstituentQueryParams { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}
