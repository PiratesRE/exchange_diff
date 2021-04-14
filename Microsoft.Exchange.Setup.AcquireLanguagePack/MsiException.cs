using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	[Serializable]
	internal sealed class MsiException : Exception
	{
		public MsiException()
		{
		}

		public MsiException(string message) : base(message)
		{
		}

		public MsiException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public MsiException(uint errorCode)
		{
			this.ErrorCode = errorCode;
		}

		public MsiException(uint errorCode, string message) : base(message)
		{
			this.ErrorCode = errorCode;
		}

		private MsiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info != null)
			{
				this.ErrorCode = info.GetUInt32("ErrorCode");
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info != null)
			{
				info.AddValue("ErrorCode", this.ErrorCode);
			}
			base.GetObjectData(info, context);
		}

		public uint ErrorCode { get; set; }
	}
}
