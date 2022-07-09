using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Mail
{
    public class SourceMail
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Mail { get; set; }

        public string AppPassword { get; set; }
    }
}
