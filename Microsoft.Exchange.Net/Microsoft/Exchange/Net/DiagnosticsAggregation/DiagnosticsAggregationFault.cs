using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class DiagnosticsAggregationFault
	{
		public DiagnosticsAggregationFault(ErrorCode errorCode, string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				throw new ArgumentException("message");
			}
			this.ErrorCode = errorCode.ToString();
			this.Message = message;
		}

		[DataMember(IsRequired = true)]
		public string ErrorCode { get; private set; }

		[DataMember(IsRequired = true)]
		public string Message { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ErrorCode: {0}, Message: {1}", new object[]
			{
				this.ErrorCode,
				this.Message
			});
		}
	}
}
