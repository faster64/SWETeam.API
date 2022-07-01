using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Device
{
    public class Connection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string IP { get; set; }

        public string Browser { get; set; }

        public string Device { get; set; }

        public string OS { get; set; }

        public string OSDescription { get; set; }

        public object More { get; set; }
    }
}
