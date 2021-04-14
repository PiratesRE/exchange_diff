using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PhotoRetrievalFailedIOException : LocalizedException
	{
		public PhotoRetrievalFailedIOException(string innerExceptionMessage) : base(Strings.PhotoRetrievalFailedIOError(innerExceptionMessage))
		{
			this.innerExceptionMessage = innerExceptionMessage;
		}

		public PhotoRetrievalFailedIOException(string innerExceptionMessage, Exception innerException) : base(Strings.PhotoRetrievalFailedIOError(innerExceptionMessage), innerException)
		{
			this.innerExceptionMessage = innerExceptionMessage;
		}

		protected PhotoRetrievalFailedIOException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.innerExceptionMessage = (string)info.GetValue("innerExceptionMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("innerExceptionMessage", this.innerExceptionMessage);
		}

		public string InnerExceptionMessage
		{
			get
			{
				return this.innerExceptionMessage;
			}
		}

		private readonly string innerExceptionMessage;
	}
}
