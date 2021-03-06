using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetMailboxJunkEmailConfigurationRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public MailboxJunkEmailConfigurationOptions Options { get; set; }

		public override string ToString()
		{
			return string.Format("SetMailboxJunkEmailConfigurationRequest: {0}", this.Options);
		}
	}
}
