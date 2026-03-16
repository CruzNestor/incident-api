using System.Runtime.Serialization;

namespace Incident.Domain.Enums
{
    public enum IncidentStatus
    {
        [EnumMember]
        OPEN = 1,

        [EnumMember]
        IN_PROGRESS = 2,

        [EnumMember]
        RESOLVED = 3
    }

}
