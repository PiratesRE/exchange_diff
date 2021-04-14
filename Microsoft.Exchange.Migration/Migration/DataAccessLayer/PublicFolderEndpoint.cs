using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration.DataAccessLayer
{
	internal class PublicFolderEndpoint : MigrationEndpointBase
	{
		public PublicFolderEndpoint(MigrationEndpoint presentationObject) : base(presentationObject)
		{
		}

		public PublicFolderEndpoint() : base(MigrationType.PublicFolder)
		{
		}

		public override ConnectionSettingsBase ConnectionSettings
		{
			get
			{
				return ExchangeConnectionSettings.Create(base.Username, base.Domain, base.EncryptedPassword, this.RpcProxyServer, this.PublicFolderDatabaseServerLegacyDN, this.SourceMailboxLegacyDN, base.AuthenticationMethod);
			}
		}

		public override MigrationType PreferredMigrationType
		{
			get
			{
				return MigrationType.PublicFolder;
			}
		}

		public Fqdn RpcProxyServer
		{
			get
			{
				return this.RemoteServer;
			}
			set
			{
				this.RemoteServer = value;
			}
		}

		public string SourceMailboxLegacyDN
		{
			get
			{
				return base.ExtendedProperties.Get<string>("SourceMailboxLegacyDN");
			}
			set
			{
				base.ExtendedProperties.Set<string>("SourceMailboxLegacyDN", value);
			}
		}

		public string PublicFolderDatabaseServerLegacyDN
		{
			get
			{
				return base.ExtendedProperties.Get<string>("PublicFolderDatabaseServerLegacyDN");
			}
			set
			{
				base.ExtendedProperties.Set<string>("PublicFolderDatabaseServerLegacyDN", value);
			}
		}

		public static IMailbox ConnectToLocalSourceDatabase(Guid databaseGuid)
		{
			IMailbox result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MapiSourceMailbox mapiSourceMailbox = disposeGuard.Add<MapiSourceMailbox>(new MapiSourceMailbox(LocalMailboxFlags.LegacyPublicFolders | LocalMailboxFlags.ParallelPublicFolderMigration));
				((IMailbox)mapiSourceMailbox).Config(null, databaseGuid, databaseGuid, CommonUtils.GetPartitionHint(OrganizationId.ForestWideOrgId), databaseGuid, MailboxType.SourceMailbox, null);
				((IMailbox)mapiSourceMailbox).Connect(MailboxConnectFlags.None);
				PublicFolderEndpoint.ThrowIfMinimumRequiredVersionNotInstalled(mapiSourceMailbox.ServerVersion);
				disposeGuard.Success();
				result = mapiSourceMailbox;
			}
			return result;
		}

		public IMailbox ConnectToSourceDatabase()
		{
			IMailbox result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MapiSourceMailbox mapiSourceMailbox = disposeGuard.Add<MapiSourceMailbox>(new MapiSourceMailbox(LocalMailboxFlags.PureMAPI | LocalMailboxFlags.LegacyPublicFolders | LocalMailboxFlags.ParallelPublicFolderMigration));
				mapiSourceMailbox.ConfigRPCHTTP(this.SourceMailboxLegacyDN, null, this.PublicFolderDatabaseServerLegacyDN, this.RpcProxyServer, base.NetworkCredentials, true, base.AuthenticationMethod == AuthenticationMethod.Ntlm);
				((IMailbox)mapiSourceMailbox).Connect(MailboxConnectFlags.None);
				PublicFolderEndpoint.ThrowIfMinimumRequiredVersionNotInstalled(mapiSourceMailbox.ServerVersion);
				disposeGuard.Success();
				result = mapiSourceMailbox;
			}
			return result;
		}

		public override void VerifyConnectivity()
		{
			try
			{
				using (IMailbox mailbox = this.ConnectToSourceDatabase())
				{
					mailbox.Disconnect();
				}
			}
			catch (MigrationTransientException)
			{
				throw;
			}
			catch (LocalizedException innerException)
			{
				throw new MigrationServerConnectionFailedException(this.RpcProxyServer.ToString(), innerException);
			}
		}

		protected override void ApplyAdditionalProperties(MigrationEndpoint presentationObject)
		{
			presentationObject.Authentication = new AuthenticationMethod?(base.AuthenticationMethod);
			presentationObject.Credentials = this.Credentials;
			presentationObject.RpcProxyServer = this.RpcProxyServer;
			presentationObject.PublicFolderDatabaseServerLegacyDN = this.PublicFolderDatabaseServerLegacyDN;
			presentationObject.SourceMailboxLegacyDN = this.SourceMailboxLegacyDN;
		}

		protected override void InitializeFromPresentationObject(MigrationEndpoint endpoint)
		{
			base.InitializeFromPresentationObject(endpoint);
			this.RpcProxyServer = endpoint.RpcProxyServer;
			this.PublicFolderDatabaseServerLegacyDN = endpoint.PublicFolderDatabaseServerLegacyDN;
			this.SourceMailboxLegacyDN = endpoint.SourceMailboxLegacyDN;
		}

		private static void ThrowIfMinimumRequiredVersionNotInstalled(int sourceServerVersion)
		{
			LocalizedString? localizedString = ParallelPublicFolderMigrationVersionChecker.CheckForMinimumRequiredVersion(sourceServerVersion);
			if (localizedString != null)
			{
				throw new MigrationTransientException(localizedString.Value);
			}
		}
	}
}
