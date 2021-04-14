using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(OlcRuleActionAssignCategory))]
	[KnownType(typeof(OlcRuleActionForward))]
	[KnownType(typeof(OlcRuleActionRemoveCategory))]
	[KnownType(typeof(OlcRuleActionMobileAlert))]
	[KnownType(typeof(OlcRuleCategoryActionBase))]
	[DataContract]
	[KnownType(typeof(OlcRuleActionMarkAsRead))]
	[KnownType(typeof(OlcRuleActionMoveToFolder))]
	internal abstract class OlcRuleActionBase
	{
		[DataMember]
		public int TypeInt { get; set; }

		public OlcActionType Type
		{
			get
			{
				return (OlcActionType)this.TypeInt;
			}
			set
			{
				this.TypeInt = (int)value;
			}
		}

		[DataMember]
		public uint DeferredThresholdCountOrDays { get; set; }
	}
}
