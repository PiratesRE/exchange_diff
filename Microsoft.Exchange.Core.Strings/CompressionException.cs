using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CompressionException : LocalizedException
	{
		public CompressionException(int errCode) : base(CoreStrings.CompressionError(errCode))
		{
			this.errCode = errCode;
		}

		public CompressionException(int errCode, Exception innerException) : base(CoreStrings.CompressionError(errCode), innerException)
		{
			this.errCode = errCode;
		}

		protected CompressionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errCode = (int)info.GetValue("errCode", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errCode", this.errCode);
		}

		public int ErrCode
		{
			get
			{
				return this.errCode;
			}
		}

		private readonly int errCode;
	}
}
