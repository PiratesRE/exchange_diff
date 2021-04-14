using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class OlcRuleActionMarkAsRead : OlcRuleActionBase
	{
		[DataMember]
		public int ReadStateInt { get; set; }

		public OlcMessageReadState ReadState
		{
			get
			{
				return (OlcMessageReadState)this.ReadStateInt;
			}
			set
			{
				this.ReadStateInt = (int)value;
			}
		}
	}
}
