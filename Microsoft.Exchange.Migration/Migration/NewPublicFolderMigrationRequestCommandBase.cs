using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class NewPublicFolderMigrationRequestCommandBase : NewMrsRequestCommandBase
	{
		protected NewPublicFolderMigrationRequestCommandBase(string cmdletName, ICollection<Type> exceptionsToIgnore) : base(cmdletName, exceptionsToIgnore)
		{
		}

		public PSCredential RemoteCredential
		{
			set
			{
				base.AddParameter("RemoteCredential", value);
			}
		}

		public string OutlookAnywhereHostName
		{
			set
			{
				base.AddParameter("OutlookAnywhereHostName", value);
			}
		}

		public string RemoteMailboxLegacyDN
		{
			set
			{
				base.AddParameter("RemoteMailboxLegacyDN", value);
			}
		}

		public string RemoteMailboxServerLegacyDN
		{
			set
			{
				base.AddParameter("RemoteMailboxServerLegacyDN", value);
			}
		}

		public DatabaseIdParameter SourceDatabase
		{
			set
			{
				base.AddParameter("SourceDatabase", value);
			}
		}

		public AuthenticationMethod AuthenticationMethod
		{
			set
			{
				base.AddParameter("AuthenticationMethod", value);
			}
		}

		private const string RemoteMailboxLegacyDNParameter = "RemoteMailboxLegacyDN";

		private const string RemoteServerLegacyDNParameter = "RemoteMailboxServerLegacyDN";

		internal const string AuthenticationMethodParameter = "AuthenticationMethod";

		private const string SourceDatabaseParameter = "SourceDatabase";
	}
}
