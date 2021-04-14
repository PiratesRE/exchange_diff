using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class StoreIntegFailedTransientException : MailboxReplicationTransientException
	{
		public StoreIntegFailedTransientException(int error) : base(MrsStrings.StoreIntegError(error))
		{
			this.error = error;
		}

		public StoreIntegFailedTransientException(int error, Exception innerException) : base(MrsStrings.StoreIntegError(error), innerException)
		{
			this.error = error;
		}

		protected StoreIntegFailedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (int)info.GetValue("error", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public int Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly int error;
	}
}
