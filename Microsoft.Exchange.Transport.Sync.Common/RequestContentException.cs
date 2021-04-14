using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RequestContentException : PermanentOperationLevelForItemsException
	{
		public RequestContentException(int statusCode) : base(Strings.RequestContentException(statusCode))
		{
			this.statusCode = statusCode;
		}

		public RequestContentException(int statusCode, Exception innerException) : base(Strings.RequestContentException(statusCode), innerException)
		{
			this.statusCode = statusCode;
		}

		protected RequestContentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.statusCode = (int)info.GetValue("statusCode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("statusCode", this.statusCode);
		}

		public int StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		private readonly int statusCode;
	}
}
