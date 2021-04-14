using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class OptInRPTsFilter : AllAssociatedRPTsFilter
	{
		public OptInRPTsFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base["OptionalInMailbox"] = true;
		}

		public new const string RbacParameters = "?Types&Mailbox&OptionalInMailbox";
	}
}
