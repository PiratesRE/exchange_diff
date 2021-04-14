using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class EwsProxyResponse
	{
		public EwsProxyResponse(string errorMessage)
		{
			this.WasProxySuccessful = false;
			this.ErrorMessage = errorMessage;
		}

		public EwsProxyResponse(int statusCode, string statusDescription, string body)
		{
			this.WasProxySuccessful = true;
			this.StatusCode = statusCode;
			this.StatusDescription = statusDescription;
			this.Body = body;
		}

		[DataMember]
		public bool WasProxySuccessful { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }

		[DataMember]
		public int StatusCode { get; set; }

		[DataMember]
		public string StatusDescription { get; set; }

		[DataMember]
		public string Body { get; set; }
	}
}
