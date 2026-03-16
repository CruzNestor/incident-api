using System.Runtime.Serialization;

namespace Incident.Domain.Enums
{
    public enum SortEnum
    {
        [EnumMember]
        DESC = 1,

        [EnumMember]
        ASC = 2,
    }
}
