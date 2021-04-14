using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class SaveExtensionCustomPropertiesResponse
	{
		public SaveExtensionCustomPropertiesResponse(string errorMessage)
		{
			this.WasSuccessful = false;
			this.ErrorMessage = errorMessage;
		}

		public SaveExtensionCustomPropertiesResponse()
		{
			this.WasSuccessful = true;
		}

		[DataMember]
		public bool WasSuccessful { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }
	}
}
