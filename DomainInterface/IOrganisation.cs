using System;

namespace DomainInterface
{
    public interface IOrganisation
    {
        int OrganisationId { get; set; }
        String OrganisationCode { get; set; }
        String OrganisationName { get; set; }
        String UniqueConfigId { get; set; }
        String UniqueOrganisationId { get; set; }
    }
}