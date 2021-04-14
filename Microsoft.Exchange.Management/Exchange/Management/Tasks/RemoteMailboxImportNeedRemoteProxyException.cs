using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoteMailboxImportNeedRemoteProxyException : MailboxReplicationPermanentException
	{
		public RemoteMailboxImportNeedRemoteProxyException() : base(Strings.ErrorRemoteMailboxImportNeedRemoteProxy)
		{
		}

		public RemoteMailboxImportNeedRemoteProxyException(Exception innerException) : base(Strings.ErrorRemoteMailboxImportNeedRemoteProxy, innerException)
		{
		}

		protected RemoteMailboxImportNeedRemoteProxyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
