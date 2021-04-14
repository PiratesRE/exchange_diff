using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class SaveExtensionSettingsResponse
	{
		public SaveExtensionSettingsResponse(string errorMessage)
		{
			this.WasSuccessful = false;
			this.ErrorMessage = errorMessage;
		}

		public SaveExtensionSettingsResponse()
		{
			this.WasSuccessful = true;
		}

		[DataMember]
		public bool WasSuccessful { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }
	}
}
