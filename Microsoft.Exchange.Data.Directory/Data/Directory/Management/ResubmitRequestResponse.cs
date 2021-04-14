using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	internal sealed class ResubmitRequestResponse
	{
		public ResubmitRequestResponse(ResubmitRequestResponseCode responseCode, string message)
		{
			this.properties["ResponseCode"] = (int)responseCode;
			this.properties["ErrorMessage"] = message;
		}

		public ResubmitRequestResponseCode ResponseCode
		{
			get
			{
				return (ResubmitRequestResponseCode)this.properties["ResponseCode"];
			}
		}

		public string ErrorMessage
		{
			get
			{
				return (string)this.properties["ErrorMessage"];
			}
		}

		private const string ResponseCodeParameterName = "ResponseCode";

		private const string ErrorMessageParameterName = "ErrorMessage";

		public static readonly ResubmitRequestResponse SuccessResponse = new ResubmitRequestResponse(ResubmitRequestResponseCode.Success, string.Empty);

		private readonly Dictionary<string, object> properties = new Dictionary<string, object>();
	}
}
