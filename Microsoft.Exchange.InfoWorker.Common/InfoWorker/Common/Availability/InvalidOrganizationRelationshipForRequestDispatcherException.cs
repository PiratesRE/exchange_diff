using System;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal class InvalidOrganizationRelationshipForRequestDispatcherException : AvailabilityException
	{
		public InvalidOrganizationRelationshipForRequestDispatcherException(string organizationRelationship) : base(ErrorConstants.InvalidOrganizationRelationshipForFreeBusy, Strings.descMisconfiguredOrganizationRelationship(organizationRelationship))
		{
		}
	}
}
