using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Rpc
{
	public abstract class ClientBinding
	{
		public abstract string ClientAddress { get; }

		public abstract string ServerAddress { get; }

		public abstract string ClientEndpoint { get; }

		public abstract bool IsEncrypted { [return: MarshalAs(UnmanagedType.U1)] get; }

		public abstract string ProtocolSequence { get; }

		public abstract AuthenticationService AuthenticationType { get; }

		public abstract Guid AssociationGuid { get; }

		public abstract string MailboxId { get; }

		internal abstract ClientSecurityContext GetClientSecurityContext();

		public ClientBinding()
		{
		}
	}
}
