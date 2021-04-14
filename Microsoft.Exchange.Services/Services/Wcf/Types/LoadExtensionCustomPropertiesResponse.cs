using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class LoadExtensionCustomPropertiesResponse
	{
		public LoadExtensionCustomPropertiesResponse(string customProperties, string customPropertyNames, string errorMessage)
		{
			if (errorMessage != null)
			{
				this.WasSuccessful = false;
				this.ErrorMessage = errorMessage;
				return;
			}
			this.WasSuccessful = true;
			this.CustomProperties = customProperties;
			this.CustomPropertyNames = customPropertyNames;
		}

		[DataMember]
		public bool WasSuccessful { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }

		[DataMember]
		public string CustomProperties { get; set; }

		[DataMember]
		public string CustomPropertyNames { get; set; }
	}
}
