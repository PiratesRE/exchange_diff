using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration.DataAccessLayer
{
	internal class ExchangeRemoteMoveEndpoint : MigrationEndpointBase
	{
		public ExchangeRemoteMoveEndpoint(MigrationEndpoint presentationObject) : base(presentationObject)
		{
		}

		public ExchangeRemoteMoveEndpoint() : base(MigrationType.ExchangeRemoteMove)
		{
		}

		public override ConnectionSettingsBase ConnectionSettings
		{
			get
			{
				return ExchangeConnectionSettings.Create(base.Username, base.Domain, base.EncryptedPassword, this.RemoteServer.ToString(), this.RemoteServer.ToString(), base.AuthenticationMethod, true, true);
			}
		}

		public override MigrationType PreferredMigrationType
		{
			get
			{
				return MigrationType.ExchangeRemoteMove;
			}
		}

		public override void VerifyConnectivity()
		{
			LocalizedException innerException;
			if (!MigrationServiceFactory.Instance.GetMigrationMrsClient().CanConnectToMrsProxy(this.RemoteServer, Guid.Empty, base.NetworkCredentials, out innerException))
			{
				throw new MigrationServerConnectionFailedException(this.RemoteServer, innerException);
			}
		}

		protected override void ApplyAutodiscoverSettings(AutodiscoverClientResponse response)
		{
			this.RemoteServer = new Fqdn(response.RPCProxyServer);
		}
	}
}
