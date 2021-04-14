using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PhotoRetrievalFailedUnauthorizedAccessException : LocalizedException
	{
		public PhotoRetrievalFailedUnauthorizedAccessException(string innerExceptionMessage) : base(Strings.PhotoRetrievalFailedUnauthorizedAccessError(innerExceptionMessage))
		{
			this.innerExceptionMessage = innerExceptionMessage;
		}

		public PhotoRetrievalFailedUnauthorizedAccessException(string innerExceptionMessage, Exception innerException) : base(Strings.PhotoRetrievalFailedUnauthorizedAccessError(innerExceptionMessage), innerException)
		{
			this.innerExceptionMessage = innerExceptionMessage;
		}

		protected PhotoRetrievalFailedUnauthorizedAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
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
