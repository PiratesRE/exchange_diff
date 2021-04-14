using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMailboxData : IMigrationSerializable
	{
		string MailboxIdentifier { get; }

		MigrationUserRecipientType RecipientType { get; }

		OrganizationId OrganizationId { get; }

		TIdParameter GetIdParameter<TIdParameter>() where TIdParameter : IIdentityParameter;

		void Update(string identifier, OrganizationId organizationId);
	}
}
