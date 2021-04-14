using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADServerWrapper : ADObjectWrapperBase, IADServer, IADObjectCommon
	{
		private void FinishConstruction(ADObject sourceObj)
		{
			this.Fqdn = (string)sourceObj[ServerSchema.Fqdn];
			this.Edition = (ServerEditionType)sourceObj[ServerSchema.Edition];
			this.VersionNumber = (int)sourceObj[ServerSchema.VersionNumber];
			this.MajorVersion = (int)sourceObj[ServerSchema.MajorVersion];
			this.AdminDisplayVersion = (ServerVersion)sourceObj[ServerSchema.AdminDisplayVersion];
			this.IsExchange2007OrLater = (bool)sourceObj[ServerSchema.IsExchange2007OrLater];
			this.IsE14OrLater = (bool)sourceObj[ServerSchema.IsE14OrLater];
			string value = (string)sourceObj[ActiveDirectoryServerSchema.MailboxRelease];
			MailboxRelease mailboxRelease;
			this.MailboxRelease = (Enum.TryParse<MailboxRelease>(value, true, out mailboxRelease) ? mailboxRelease : MailboxRelease.None);
			this.CurrentServerRole = (ServerRole)sourceObj[ServerSchema.CurrentServerRole];
			this.IsMailboxServer = (bool)sourceObj[ServerSchema.IsMailboxServer];
			this.ServerSite = (ADObjectId)sourceObj[ServerSchema.ServerSite];
			this.ExchangeLegacyDN = (string)sourceObj[ServerSchema.ExchangeLegacyDN];
			this.DatabaseAvailabilityGroup = (ADObjectId)sourceObj[ServerSchema.DatabaseAvailabilityGroup];
			this.AutoDatabaseMountDial = (AutoDatabaseMountDial)sourceObj[ActiveDirectoryServerSchema.AutoDatabaseMountDialType];
			this.DatabaseCopyAutoActivationPolicy = (DatabaseCopyAutoActivationPolicyType)sourceObj[ActiveDirectoryServerSchema.DatabaseCopyAutoActivationPolicy];
			this.DatabaseCopyActivationDisabledAndMoveNow = (bool)sourceObj[ActiveDirectoryServerSchema.DatabaseCopyActivationDisabledAndMoveNow];
			this.AutoDagServerConfigured = (bool)sourceObj[ActiveDirectoryServerSchema.AutoDagServerConfigured];
			this.MaximumActiveDatabases = (int?)sourceObj[ServerSchema.MaxActiveMailboxDatabases];
			this.MaximumPreferredActiveDatabases = (int?)sourceObj[ServerSchema.MaxPreferredActiveDatabases];
			this.ContinuousReplicationMaxMemoryPerDatabase = (long?)sourceObj[ActiveDirectoryServerSchema.ContinuousReplicationMaxMemoryPerDatabase];
			this.ComponentStates = (MultiValuedProperty<string>)sourceObj[ServerSchema.ComponentStates];
		}

		internal ADServerWrapper(IADServer source) : base(source)
		{
			this.Fqdn = source.Fqdn;
			this.Edition = source.Edition;
			this.VersionNumber = source.VersionNumber;
			this.MajorVersion = source.MajorVersion;
			this.AdminDisplayVersion = source.AdminDisplayVersion;
			this.IsExchange2007OrLater = source.IsExchange2007OrLater;
			this.IsE14OrLater = source.IsE14OrLater;
			this.MailboxRelease = source.MailboxRelease;
			this.CurrentServerRole = source.CurrentServerRole;
			this.IsMailboxServer = source.IsMailboxServer;
			this.ServerSite = source.ServerSite;
			this.ExchangeLegacyDN = source.ExchangeLegacyDN;
			this.DatabaseAvailabilityGroup = source.DatabaseAvailabilityGroup;
			this.AutoDatabaseMountDial = source.AutoDatabaseMountDial;
			this.DatabaseCopyAutoActivationPolicy = source.DatabaseCopyAutoActivationPolicy;
			this.DatabaseCopyActivationDisabledAndMoveNow = source.DatabaseCopyActivationDisabledAndMoveNow;
			this.AutoDagServerConfigured = source.AutoDagServerConfigured;
			this.MaximumActiveDatabases = source.MaximumActiveDatabases;
			this.MaximumPreferredActiveDatabases = source.MaximumPreferredActiveDatabases;
			this.ContinuousReplicationMaxMemoryPerDatabase = source.ContinuousReplicationMaxMemoryPerDatabase;
			this.ComponentStates = source.ComponentStates;
		}

		private ADServerWrapper(Server server) : base(server)
		{
			this.FinishConstruction(server);
		}

		public static ADServerWrapper CreateWrapper(Server server)
		{
			if (server == null)
			{
				return null;
			}
			return new ADServerWrapper(server);
		}

		private ADServerWrapper(MiniServer server) : base(server)
		{
			this.FinishConstruction(server);
		}

		public static ADServerWrapper CreateWrapper(MiniServer server)
		{
			if (server == null)
			{
				return null;
			}
			return new ADServerWrapper(server);
		}

		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
			private set
			{
				if (value != null)
				{
					this.fqdn = string.Intern(value);
					return;
				}
				this.fqdn = null;
			}
		}

		public bool IsE14OrLater { get; private set; }

		public ServerVersion AdminDisplayVersion { get; private set; }

		public ServerRole CurrentServerRole { get; private set; }

		public ADObjectId ServerSite { get; private set; }

		public ADObjectId DatabaseAvailabilityGroup { get; private set; }

		public DatabaseCopyAutoActivationPolicyType DatabaseCopyAutoActivationPolicy { get; private set; }

		public bool DatabaseCopyActivationDisabledAndMoveNow { get; private set; }

		public bool AutoDagServerConfigured { get; private set; }

		public bool IsMailboxServer { get; private set; }

		public ServerEditionType Edition { get; private set; }

		public int VersionNumber { get; private set; }

		public int? MaximumActiveDatabases { get; private set; }

		public int? MaximumPreferredActiveDatabases { get; private set; }

		public AutoDatabaseMountDial AutoDatabaseMountDial { get; private set; }

		public long? ContinuousReplicationMaxMemoryPerDatabase { get; private set; }

		public int MajorVersion { get; private set; }

		public MailboxRelease MailboxRelease { get; private set; }

		public bool IsExchange2007OrLater { get; private set; }

		public string ExchangeLegacyDN { get; private set; }

		public MultiValuedProperty<string> ComponentStates
		{
			get
			{
				return this.componentStates;
			}
			private set
			{
				this.componentStates = value;
			}
		}

		public override void Minimize()
		{
			base.Minimize();
		}

		public static readonly ADPropertyDefinition[] PropertiesNeededForServer = new ADPropertyDefinition[]
		{
			ServerSchema.Fqdn,
			ServerSchema.Edition,
			ServerSchema.VersionNumber,
			ServerSchema.MajorVersion,
			ServerSchema.AdminDisplayVersion,
			ServerSchema.IsExchange2007OrLater,
			ServerSchema.IsE14OrLater,
			ActiveDirectoryServerSchema.MailboxRelease,
			ServerSchema.CurrentServerRole,
			ServerSchema.IsMailboxServer,
			ServerSchema.ServerSite,
			ServerSchema.ExchangeLegacyDN,
			ServerSchema.DatabaseAvailabilityGroup,
			ActiveDirectoryServerSchema.AutoDatabaseMountDialType,
			ActiveDirectoryServerSchema.DatabaseCopyAutoActivationPolicy,
			ActiveDirectoryServerSchema.DatabaseCopyActivationDisabledAndMoveNow,
			ActiveDirectoryServerSchema.AutoDagServerConfigured,
			ServerSchema.MaxActiveMailboxDatabases,
			ServerSchema.MaxPreferredActiveDatabases,
			ActiveDirectoryServerSchema.ContinuousReplicationMaxMemoryPerDatabase,
			ServerSchema.ComponentStates
		};

		private string fqdn;

		private MultiValuedProperty<string> componentStates;
	}
}
