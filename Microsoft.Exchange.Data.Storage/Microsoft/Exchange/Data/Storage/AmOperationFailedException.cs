using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Storage
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmOperationFailedException : AmServerException
	{
		public AmOperationFailedException(string errMessage) : base(ServerStrings.AmOperationFailedException(errMessage))
		{
			this.errMessage = errMessage;
		}

		public AmOperationFailedException(string errMessage, Exception innerException) : base(ServerStrings.AmOperationFailedException(errMessage), innerException)
		{
			this.errMessage = errMessage;
		}

		protected AmOperationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
