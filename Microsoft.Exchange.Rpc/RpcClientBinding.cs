using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Rpc
{
	public class RpcClientBinding : ClientBinding
	{
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
				return this.clientEndpoint;
			}
		}

		public override bool IsEncrypted
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.isEncrypted;
			}
		}

		public override string ProtocolSequence
		{
			get
			{
				return this.protocolSequence;
			}
		}

		public override AuthenticationService AuthenticationType
		{
			get
			{
				return this.authenticationType;
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
				return null;
			}
		}

		internal RpcClientBinding(IntPtr binding, SafeRpcAsyncStateHandle asyncState)
		{
			this.bindingHandle = binding;
			RpcServerBase.GetBindingInformation(this.bindingHandle, out this.clientAddress, out this.clientEndpoint, out this.serverAddress, out this.serverEndpoint, out this.protocolSequence);
			if (string.Equals(this.protocolSequence, "ncacn_http", StringComparison.InvariantCultureIgnoreCase))
			{
				Guid clientIdentifier = RpcServerBase.GetClientIdentifier(this.bindingHandle.ToPointer());
				this.associationGuid = clientIdentifier;
			}
			else
			{
				this.associationGuid = Guid.Empty;
			}
			this.authenticationType = RpcServerBase.GetAuthenticationType(this.bindingHandle.ToPointer());
			this.isEncrypted = RpcServerBase.IsConnectionEncrypted(this.bindingHandle.ToPointer());
		}

		internal override ClientSecurityContext GetClientSecurityContext()
		{
			if (this.bindingHandle != IntPtr.Zero)
			{
				return RpcServerBase.GetClientSecurityContext(this.bindingHandle);
			}
			return null;
		}

		private IntPtr bindingHandle;

		private AuthenticationService authenticationType;

		private string protocolSequence;

		private string clientAddress;

		private string serverAddress;

		private string clientEndpoint;

		private string serverEndpoint;

		private bool isEncrypted;

		private Guid associationGuid;

		private ClientSecurityContext clientSecurityContext;
	}
}
