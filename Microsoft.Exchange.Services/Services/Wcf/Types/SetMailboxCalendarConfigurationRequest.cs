using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SetMailboxCalendarConfigurationRequest : BaseJsonRequest
	{
		[DataMember(IsRequired = true)]
		public MailboxCalendarConfiguration Options { get; set; }

		public override string ToString()
		{
			return string.Format("SetMailboxCalendarConfigurationRequest: {0}", this.Options);
		}
	}
}
