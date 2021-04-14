using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal interface IFederatedIdentityParameters
	{
		ADObjectId ObjectId { get; }

		OrganizationId OrganizationId { get; }

		string ImmutableId { get; }

		SmtpAddress WindowsLiveID { get; }

		string ImmutableIdPartial { get; }
	}
}
