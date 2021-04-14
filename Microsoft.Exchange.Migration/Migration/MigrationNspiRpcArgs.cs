using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MigrationNspiRpcArgs : MigrationProxyRpcArgs
	{
		protected MigrationNspiRpcArgs(ExchangeOutlookAnywhereEndpoint endpoint, MigrationProxyRpcType type) : base(endpoint.Username, endpoint.EncryptedPassword, endpoint.Domain, type)
		{
			this.RpcHostServer = endpoint.NspiServer;
			this.RpcProxyServer = endpoint.RpcProxyServer;
			this.RpcAuthenticationMethod = ((endpoint.AuthenticationMethod == AuthenticationMethod.Ntlm) ? HTTPAuthentication.Ntlm : HTTPAuthentication.Basic);
		}

		protected MigrationNspiRpcArgs(byte[] requestBlob, MigrationProxyRpcType type) : base(requestBlob, type)
		{
		}

		public string RpcHostServer
		{
			get
			{
				return base.GetProperty<string>(2415984671U);
			}
			set
			{
				base.SetPropertyAsString(2415984671U, value);
			}
		}

		public string RpcProxyServer
		{
			get
			{
				return base.GetProperty<string>(2416050207U);
			}
			set
			{
				base.SetPropertyAsString(2416050207U, value);
			}
		}

		public HTTPAuthentication RpcAuthenticationMethod
		{
			get
			{
				string property = base.GetProperty<string>(2416836639U);
				if (!string.IsNullOrEmpty(property))
				{
					return (HTTPAuthentication)Enum.Parse(typeof(HTTPAuthentication), property);
				}
				return HTTPAuthentication.Basic;
			}
			set
			{
				base.SetPropertyAsString(2416836639U, value.ToString());
			}
		}

		public override bool Validate(out string errorMsg)
		{
			if (!base.Validate(out errorMsg))
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.RpcHostServer))
			{
				errorMsg = "RPC Host cannot be null or empty.";
				return false;
			}
			if (string.IsNullOrEmpty(this.RpcProxyServer))
			{
				errorMsg = "RPC Http Proxy cannot be null or empty.";
				return false;
			}
			return true;
		}
	}
}
