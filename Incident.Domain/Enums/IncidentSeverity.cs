using System.Runtime.Serialization;

namespace Incident.Domain.Enums
{
    public enum IncidentSeverity
    {
        [EnumMember]
        LOW = 1,

        [EnumMember]
        MEDIUM = 2,

        [EnumMember]
        HIGH = 3
    }
}
