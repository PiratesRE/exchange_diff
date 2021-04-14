using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetMailboxRegionalConfigurationRequest : BaseJsonRequest
	{
		[DataMember]
		public bool VerifyDefaultFolderNameLanguage { get; set; }

		public override string ToString()
		{
			return string.Format("GetMailboxRegionalConfigurationRequest: VerifyDefaultFolderNameLanguage = {0}", this.VerifyDefaultFolderNameLanguage);
		}
	}
}
