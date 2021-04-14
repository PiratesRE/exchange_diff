using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PhotoRetrievalFailedWin32Exception : LocalizedException
	{
		public PhotoRetrievalFailedWin32Exception(string innerExceptionMessage) : base(Strings.PhotoRetrievalFailedWin32Error(innerExceptionMessage))
		{
			this.innerExceptionMessage = innerExceptionMessage;
		}

		public PhotoRetrievalFailedWin32Exception(string innerExceptionMessage, Exception innerException) : base(Strings.PhotoRetrievalFailedWin32Error(innerExceptionMessage), innerException)
		{
			this.innerExceptionMessage = innerExceptionMessage;
		}

		protected PhotoRetrievalFailedWin32Exception(SerializationInfo info, StreamingContext context) : base(info, context)
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
