using System;
using System.Security.Principal;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.MapiHttp
{
	internal class MapiHttpClientBinding : ClientBinding
	{
		public MapiHttpClientBinding(string mailboxId, string clientAddress, string serverAddress, bool isSecureConnection, IIdentity userIdentity, Func<ClientSecurityContext> clientSecurityContextGetter)
		{
			this.clientSecurityContextGetter = clientSecurityContextGetter;
			this.mailboxId = mailboxId;
			this.clientAddress = clientAddress;
			this.serverAddress = serverAddress;
			this.isSecureConnection = isSecureConnection;
			this.userIdentity = userIdentity;
			if (userIdentity == null)
			{
				throw ProtocolException.FromResponseCode((LID)64928, "User identity null", ResponseCode.AnonymousNotAllowed, null);
			}
			if (!(userIdentity is LiveIDIdentity) && !(userIdentity is SidBasedIdentity) && !(userIdentity is WindowsTokenIdentity) && !(userIdentity is WindowsIdentity))
			{
				throw ProtocolException.FromResponseCode((LID)58912, string.Format("Unable to determine user identity [{0}]", userIdentity.GetType().ToString()), ResponseCode.UnknownFailure, null);
			}
		}

		public override string ClientAddress
		{
			get
			{
				return this.clientAddress;
			}
		}

		public override string ServerAddress
		{
			get
			{
				return this.serverAddress;
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
				return this.isSecureConnection;
			}
		}

		public override string ProtocolSequence
		{
			get
			{
				return "MapiHttp";
			}
		}

		public override AuthenticationService AuthenticationType
		{
			get
			{
				return AuthenticationService.None;
			}
		}

		public override Guid AssociationGuid
		{
			get
			{
				return this.associationGuid;
			}
		}

		public override string MailboxId
		{
			get
			{
				return this.mailboxId;
			}
		}

		internal void ClearClientSecurityContextGetter()
		{
			this.clientSecurityContextGetter = null;
		}

		internal override ClientSecurityContext GetClientSecurityContext()
		{
			Func<ClientSecurityContext> func = this.clientSecurityContextGetter;
			this.clientSecurityContextGetter = null;
			if (func != null)
			{
				ClientSecurityContext clientSecurityContext = func();
				if (clientSecurityContext != null)
				{
					return clientSecurityContext;
				}
			}
			return this.userIdentity.CreateClientSecurityContext(false);
		}

		private readonly string mailboxId;

		private readonly string clientAddress;

		private readonly string serverAddress;

		private readonly Guid associationGuid = Guid.NewGuid();

		private readonly bool isSecureConnection;

		private readonly IIdentity userIdentity;

		private Func<ClientSecurityContext> clientSecurityContextGetter;
	}
}
