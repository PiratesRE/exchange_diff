using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequestQueueIsTooLongTransientException : MailboxReplicationTransientException
	{
		public RequestQueueIsTooLongTransientException(string requestQueueName, int currentQueueLength) : base(Strings.ErrorRequestQueueIsTooLong(requestQueueName, currentQueueLength))
		{
			this.requestQueueName = requestQueueName;
			this.currentQueueLength = currentQueueLength;
		}

		public RequestQueueIsTooLongTransientException(string requestQueueName, int currentQueueLength, Exception innerException) : base(Strings.ErrorRequestQueueIsTooLong(requestQueueName, currentQueueLength), innerException)
		{
			this.requestQueueName = requestQueueName;
			this.currentQueueLength = currentQueueLength;
		}

		protected RequestQueueIsTooLongTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.requestQueueName = (string)info.GetValue("requestQueueName", typeof(string));
			this.currentQueueLength = (int)info.GetValue("currentQueueLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("requestQueueName", this.requestQueueName);
			info.AddValue("currentQueueLength", this.currentQueueLength);
		}

		public string RequestQueueName
		{
			get
			{
				return this.requestQueueName;
			}
		}

		public int CurrentQueueLength
		{
			get
			{
				return this.currentQueueLength;
			}
		}

		private readonly string requestQueueName;

		private readonly int currentQueueLength;
	}
}
