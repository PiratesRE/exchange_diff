using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class PolicyScopeEditor : RecipientConditionEditorBase
	{
		public PolicyScopeEditor()
		{
			this.UseExceptions = false;
		}

		protected override RulePhrase[] SupportedConditions
		{
			get
			{
				string name = this.UseExceptions ? "ExceptIfSentTo" : "SentTo";
				string name2 = this.UseExceptions ? "ExceptIfRecipientDomainIs" : "RecipientDomainIs";
				string name3 = this.UseExceptions ? "ExceptIfSentToMemberOf" : "SentToMemberOf";
				return new RulePhrase[]
				{
					new RulePhrase(name, Strings.ConditionalRecipientIs, new FormletParameter[]
					{
						new PeopleParameter(name, PickerType.PickTo)
					}, null, false),
					new RulePhrase(name2, Strings.ConditionalRecipientDomain, new FormletParameter[]
					{
						new ObjectsParameter(name2, Strings.DomainDialogTitle, Strings.StringArrayDialogLabel, Strings.TransportRuleRecipientDomainContainsWordsPredicateText, "~/pickers/AcceptedDomainPicker.aspx")
						{
							ValueProperty = "DisplayName",
							DialogWidth = 445,
							DialogHeight = 530
						}
					}, null, false),
					new RulePhrase(name3, Strings.ConditionalRecipientMemberOf, new FormletParameter[]
					{
						new PeopleParameter(name3, PickerType.PickTo)
					}, null, false)
				};
			}
		}

		public bool UseExceptions { get; set; }
	}
}
