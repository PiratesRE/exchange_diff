using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.MessagingPolicies.Journaling;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class JournalRuleRow : RuleRow
	{
		public JournalRuleRow(JournalRuleObject rule) : base(rule)
		{
			this.JournalEmailAddress = rule.JournalEmailAddress.ToString();
			this.Scope = rule.Scope.ToString();
			this.Global = this.Scope.Equals("Global");
			this.Internal = this.Scope.Equals("Internal");
			this.External = this.Scope.Equals("External");
			this.RecipientString = ((rule.Recipient != null) ? rule.Recipient.ToString() : string.Empty);
			base.Supported = true;
		}

		[DataMember]
		public string JournalEmailAddress { get; private set; }

		[DataMember]
		public string Scope { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public bool Global { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public bool Internal { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public bool External { get; private set; }

		[DataMember]
		public string RecipientString { get; private set; }
	}
}
