using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management.Migration;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationEndpoint : ConfigurableObject
	{
		public MigrationEndpoint() : base(new SimplePropertyBag(MigrationEndpointSchema.Identity, MigrationEndpointSchema.ObjectState, MigrationEndpointSchema.ExchangeVersion))
		{
		}

		public new MigrationEndpointId Identity
		{
			get
			{
				return (MigrationEndpointId)base.Identity;
			}
			set
			{
				this[MigrationEndpointSchema.Identity] = value;
			}
		}

		public MigrationType EndpointType
		{
			get
			{
				return (MigrationType)this[MigrationEndpointSchema.EndpointType];
			}
			set
			{
				this[MigrationEndpointSchema.EndpointType] = value;
			}
		}

		public Unlimited<int> MaxConcurrentMigrations
		{
			get
			{
				return (Unlimited<int>)this[MigrationEndpointSchema.MaxConcurrentMigrations];
			}
			set
			{
				this[MigrationEndpointSchema.MaxConcurrentMigrations] = value;
			}
		}

		public Unlimited<int> MaxConcurrentIncrementalSyncs
		{
			get
			{
				return (Unlimited<int>)this[MigrationEndpointSchema.MaxConcurrentIncrementalSyncs];
			}
			set
			{
				this[MigrationEndpointSchema.MaxConcurrentIncrementalSyncs] = value;
			}
		}

		public int? ActiveMigrationCount { get; set; }

		public int? ActiveIncrementalSyncCount { get; set; }

		public Fqdn RemoteServer
		{
			get
			{
				return (Fqdn)this[MigrationEndpointSchema.RemoteServer];
			}
			set
			{
				this[MigrationEndpointSchema.RemoteServer] = value;
			}
		}

		public string Username
		{
			get
			{
				return (string)this[MigrationEndpointSchema.Username];
			}
			set
			{
				this[MigrationEndpointSchema.Username] = value;
			}
		}

		public int? Port
		{
			get
			{
				return (int?)this[MigrationEndpointSchema.Port];
			}
			set
			{
				this[MigrationEndpointSchema.Port] = value;
			}
		}

		public AuthenticationMethod? Authentication
		{
			get
			{
				return (AuthenticationMethod?)this[MigrationEndpointSchema.AuthenticationMethod];
			}
			set
			{
				this[MigrationEndpointSchema.AuthenticationMethod] = value;
			}
		}

		public IMAPSecurityMechanism? Security
		{
			get
			{
				return (IMAPSecurityMechanism?)this[MigrationEndpointSchema.Security];
			}
			set
			{
				this[MigrationEndpointSchema.Security] = value;
			}
		}

		public Fqdn RpcProxyServer
		{
			get
			{
				return (Fqdn)this[MigrationEndpointSchema.RPCProxyServer];
			}
			set
			{
				this[MigrationEndpointSchema.RPCProxyServer] = value;
			}
		}

		public string ExchangeServer
		{
			get
			{
				return (string)this[MigrationEndpointSchema.ExchangeServer];
			}
			set
			{
				this[MigrationEndpointSchema.ExchangeServer] = value;
			}
		}

		public string NspiServer
		{
			get
			{
				return (string)this[MigrationEndpointSchema.NspiServer];
			}
			set
			{
				this[MigrationEndpointSchema.NspiServer] = value;
			}
		}

		public bool? UseAutoDiscover
		{
			get
			{
				return (bool?)this[MigrationEndpointSchema.UseAutoDiscover];
			}
			set
			{
				this[MigrationEndpointSchema.UseAutoDiscover] = value;
			}
		}

		public MigrationMailboxPermission MailboxPermission
		{
			get
			{
				return (MigrationMailboxPermission)this[MigrationEndpointSchema.MailboxPermission];
			}
			set
			{
				this[MigrationEndpointSchema.MailboxPermission] = value;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.Identity.Guid;
			}
		}

		public bool IsRemote
		{
			get
			{
				MigrationType endpointType = this.EndpointType;
				if (endpointType <= MigrationType.ExchangeOutlookAnywhere)
				{
					if (endpointType != MigrationType.IMAP && endpointType != MigrationType.ExchangeOutlookAnywhere)
					{
						return false;
					}
				}
				else if (endpointType != MigrationType.ExchangeRemoteMove && endpointType != MigrationType.PSTImport && endpointType != MigrationType.PublicFolder)
				{
					return false;
				}
				return true;
			}
		}

		public string SourceMailboxLegacyDN
		{
			get
			{
				return (string)this[MigrationEndpointSchema.SourceMailboxLegacyDN];
			}
			set
			{
				this[MigrationEndpointSchema.SourceMailboxLegacyDN] = value;
			}
		}

		public string PublicFolderDatabaseServerLegacyDN
		{
			get
			{
				return (string)this[MigrationEndpointSchema.PublicFolderDatabaseServerLegacyDN];
			}
			set
			{
				this[MigrationEndpointSchema.PublicFolderDatabaseServerLegacyDN] = value;
			}
		}

		public string DiagnosticInfo
		{
			get
			{
				return (string)this[MigrationBatchSchema.DiagnosticInfo];
			}
			internal set
			{
				this[MigrationBatchSchema.DiagnosticInfo] = value;
			}
		}

		internal SmtpAddress EmailAddress { get; set; }

		internal PSCredential Credentials
		{
			get
			{
				return this.credentials;
			}
			set
			{
				this.credentials = value;
				if (value == null)
				{
					this.Username = null;
					return;
				}
				this.Username = value.UserName;
			}
		}

		internal DateTime LastModifiedTime { get; set; }

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<MigrationEndpointSchema>();
			}
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return base.ToString();
		}

		[NonSerialized]
		private PSCredential credentials;
	}
}
