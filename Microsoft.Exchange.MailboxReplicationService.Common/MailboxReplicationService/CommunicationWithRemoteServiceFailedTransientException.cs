using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CommunicationWithRemoteServiceFailedTransientException : MailboxReplicationTransientException
	{
		public CommunicationWithRemoteServiceFailedTransientException(string endpoint) : base(MrsStrings.CommunicationWithRemoteServiceFailed(endpoint))
		{
			this.endpoint = endpoint;
		}

		public CommunicationWithRemoteServiceFailedTransientException(string endpoint, Exception innerException) : base(MrsStrings.CommunicationWithRemoteServiceFailed(endpoint), innerException)
		{
			this.endpoint = endpoint;
		}

		protected CommunicationWithRemoteServiceFailedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.endpoint = (string)info.GetValue("endpoint", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("endpoint", this.endpoint);
		}

		public string Endpoint
		{
			get
			{
				return this.endpoint;
			}
		}

		private readonly string endpoint;
	}
}
