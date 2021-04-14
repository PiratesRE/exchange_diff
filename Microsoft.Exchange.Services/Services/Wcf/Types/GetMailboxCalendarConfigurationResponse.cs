using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetMailboxCalendarConfigurationResponse : OptionsResponseBase
	{
		[DataMember(IsRequired = true)]
		public MailboxCalendarConfiguration Options { get; set; }

		public override string ToString()
		{
			return string.Format("GetMailboxCalendarConfigurationResponse: {0}", this.Options);
		}
	}
}
