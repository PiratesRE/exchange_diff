using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorExceededTextScanLimitException : LocalizedException
	{
		public ErrorExceededTextScanLimitException(int textScanLimits) : base(Strings.ErrorExceededTextScanLimit(textScanLimits))
		{
			this.textScanLimits = textScanLimits;
		}

		public ErrorExceededTextScanLimitException(int textScanLimits, Exception innerException) : base(Strings.ErrorExceededTextScanLimit(textScanLimits), innerException)
		{
			this.textScanLimits = textScanLimits;
		}

		protected ErrorExceededTextScanLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.textScanLimits = (int)info.GetValue("textScanLimits", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("textScanLimits", this.textScanLimits);
		}

		public int TextScanLimits
		{
			get
			{
				return this.textScanLimits;
			}
		}

		private readonly int textScanLimits;
	}
}
