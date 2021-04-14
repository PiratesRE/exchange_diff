using System;
using System.Security.Principal;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class WebServiceClientBinding : ClientBinding
	{
		public WebServiceClientBinding(string protocolSequence, WindowsIdentity userIdentity)
		{
			this.protocolSequence = protocolSequence;
			this.userIdentity = userIdentity;
		}

		public override Guid AssociationGuid
		{
			get
			{
				return Guid.Empty;
			}
		}

		public override AuthenticationService AuthenticationType
		{
			get
			{
				return AuthenticationService.None;
			}
		}

		public override string ClientAddress
		{
			get
			{
				return string.Empty;
			}
		}

		public override string ClientEndpoint
		{
			get
			{
				return string.Empty;
			}
		}

		public override bool IsEncrypted
		{
			get
			{
				return true;
			}
		}

		public override string ProtocolSequence
		{
			get
			{
				return this.protocolSequence;
			}
		}

		public override string ServerAddress
		{
			get
			{
				return string.Empty;
			}
		}

		public override string MailboxId
		{
			get
			{
				return null;
			}
		}

		internal override ClientSecurityContext GetClientSecurityContext()
		{
			if (this.userIdentity == null)
			{
				return null;
			}
			return new ClientSecurityContext(this.userIdentity);
		}

		private readonly string protocolSequence;

		private readonly WindowsIdentity userIdentity;
	}
}
