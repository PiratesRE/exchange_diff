using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateFavoriteFolderResponse
	{
		public UpdateFavoriteFolderResponse(string errorMessage)
		{
			this.WasSuccessful = false;
			this.ErrorMessage = errorMessage;
		}

		public UpdateFavoriteFolderResponse()
		{
			this.WasSuccessful = true;
		}

		[DataMember]
		public bool WasSuccessful { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }
	}
}
