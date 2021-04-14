using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnknownDeltaSyncException : PermanentOperationLevelForItemsException
	{
		public UnknownDeltaSyncException(int statusCode) : base(Strings.UnknownDeltaSyncException(statusCode))
		{
			this.statusCode = statusCode;
		}

		public UnknownDeltaSyncException(int statusCode, Exception innerException) : base(Strings.UnknownDeltaSyncException(statusCode), innerException)
		{
			this.statusCode = statusCode;
		}

		protected UnknownDeltaSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
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
