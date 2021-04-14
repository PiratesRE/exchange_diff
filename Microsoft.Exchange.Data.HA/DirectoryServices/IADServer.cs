using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADServer : IADObjectCommon
	{
		string Fqdn { get; }

		bool IsE14OrLater { get; }

		ServerVersion AdminDisplayVersion { get; }

		ServerRole CurrentServerRole { get; }

		ADObjectId ServerSite { get; }

		ADObjectId DatabaseAvailabilityGroup { get; }

		DatabaseCopyAutoActivationPolicyType DatabaseCopyAutoActivationPolicy { get; }

		bool DatabaseCopyActivationDisabledAndMoveNow { get; }

		bool AutoDagServerConfigured { get; }

		bool IsMailboxServer { get; }

		ServerEditionType Edition { get; }

		int VersionNumber { get; }

		int? MaximumActiveDatabases { get; }

		int? MaximumPreferredActiveDatabases { get; }

		AutoDatabaseMountDial AutoDatabaseMountDial { get; }

		long? ContinuousReplicationMaxMemoryPerDatabase { get; }

		int MajorVersion { get; }

		bool IsExchange2007OrLater { get; }

		string ExchangeLegacyDN { get; }

		MailboxRelease MailboxRelease { get; }

		MultiValuedProperty<string> ComponentStates { get; }
	}
}
