using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RequestFormatException : PermanentOperationLevelForItemsException
	{
		public RequestFormatException(int statusCode) : base(Strings.RequestFormatException(statusCode))
		{
			this.statusCode = statusCode;
		}

		public RequestFormatException(int statusCode, Exception innerException) : base(Strings.RequestFormatException(statusCode), innerException)
		{
			this.statusCode = statusCode;
		}

		protected RequestFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
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
