using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetMailboxRegionalConfigurationRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public SetMailboxRegionalConfigurationData Options { get; set; }

		public override string ToString()
		{
			return string.Format("SetMailboxRegionalConfigurationRequest: {0}", this.Options);
		}
	}
}
