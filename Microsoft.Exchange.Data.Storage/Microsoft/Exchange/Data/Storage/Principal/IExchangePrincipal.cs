using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExchangePrincipal
	{
		string PrincipalId { get; }

		ADObjectId ObjectId { get; }

		string LegacyDn { get; }

		string Alias { get; }

		ADObjectId DefaultPublicFolderMailbox { get; }

		SecurityIdentifier Sid { get; }

		SecurityIdentifier MasterAccountSid { get; }

		IEnumerable<SecurityIdentifier> SidHistory { get; }

		IEnumerable<ADObjectId> Delegates { get; }

		IEnumerable<CultureInfo> PreferredCultures { get; }

		RecipientType RecipientType { get; }

		RecipientTypeDetails RecipientTypeDetails { get; }

		bool? IsResource { get; }

		ModernGroupObjectType ModernGroupType { get; }

		IEnumerable<SecurityIdentifier> PublicToGroupSids { get; }

		string ExternalDirectoryObjectId { get; }

		IEnumerable<Guid> AggregatedMailboxGuids { get; }

		IMailboxInfo MailboxInfo { get; }

		IEnumerable<IMailboxInfo> AllMailboxes { get; }

		bool IsCrossSiteAccessAllowed { get; }

		bool IsMailboxInfoRequired { get; }
	}
}
