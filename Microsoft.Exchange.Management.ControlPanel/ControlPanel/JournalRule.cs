using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.MessagingPolicies.Journaling;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(JournalRule))]
	[DataContract]
	public class JournalRule : JournalRuleRow
	{
		public JournalRule(JournalRuleObject rule) : base(rule)
		{
			this.Rule = rule;
		}

		public JournalRuleObject Rule { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public PeopleIdentity[] Recipient
		{
			get
			{
				if (this.Rule.Recipient != null)
				{
					return new PeopleIdentity[]
					{
						this.Rule.Recipient.ToPeopleIdentity()
					};
				}
				return null;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
