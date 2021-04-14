using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DecompressionException : LocalizedException
	{
		public DecompressionException(int errCode) : base(CoreStrings.DecompressionError(errCode))
		{
			this.errCode = errCode;
		}

		public DecompressionException(int errCode, Exception innerException) : base(CoreStrings.DecompressionError(errCode), innerException)
		{
			this.errCode = errCode;
		}

		protected DecompressionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
