using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TaskOperationFailedException : TaskServerException
	{
		public TaskOperationFailedException(string errMessage) : base(ServerStrings.TaskOperationFailedException(errMessage))
		{
			this.errMessage = errMessage;
		}

		public TaskOperationFailedException(string errMessage, Exception innerException) : base(ServerStrings.TaskOperationFailedException(errMessage), innerException)
		{
			this.errMessage = errMessage;
		}

		protected TaskOperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMessage = (string)info.GetValue("errMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMessage", this.errMessage);
		}

		public string ErrMessage
		{
			get
			{
				return this.errMessage;
			}
		}

		private readonly string errMessage;
	}
}
