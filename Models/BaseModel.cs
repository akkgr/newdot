

using System;

namespace newdot.Models
{
    public abstract class BaseModel
    {
        public string Id { get; set; }
        public DateTime Updated { get; set; }
    }
}