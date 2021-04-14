using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class CustomAttributeEditor : RecipientConditionEditorBase
	{
		protected override RulePhrase[] SupportedConditions
		{
			get
			{
				return CustomAttributeEditor.allSupportedRules;
			}
		}

		private static RulePhrase[] allSupportedRules = new RulePhrase[]
		{
			new RulePhrase("CustomAttribute1", Strings.ConditionalCustomAttribute1Text, new FormletParameter[]
			{
				new StringParameter("customattribute1", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute2", Strings.ConditionalCustomAttribute2Text, new FormletParameter[]
			{
				new StringParameter("customattribute2", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute3", Strings.ConditionalCustomAttribute3Text, new FormletParameter[]
			{
				new StringParameter("customattribute3", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute4", Strings.ConditionalCustomAttribute4Text, new FormletParameter[]
			{
				new StringParameter("customattribute4", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute5", Strings.ConditionalCustomAttribute5Text, new FormletParameter[]
			{
				new StringParameter("customattribute5", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute6", Strings.ConditionalCustomAttribute6Text, new FormletParameter[]
			{
				new StringParameter("customattribute6", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute7", Strings.ConditionalCustomAttribute7Text, new FormletParameter[]
			{
				new StringParameter("customattribute7", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute8", Strings.ConditionalCustomAttribute8Text, new FormletParameter[]
			{
				new StringParameter("customattribute8", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute9", Strings.ConditionalCustomAttribute9Text, new FormletParameter[]
			{
				new StringParameter("customattribute9", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute10", Strings.ConditionalCustomAttribute10Text, new FormletParameter[]
			{
				new StringParameter("customattribute10", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextShort), false)
			}, null, false),
			new RulePhrase("CustomAttribute11", Strings.ConditionalCustomAttribute11Text, new FormletParameter[]
			{
				new StringParameter("customattribute11", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextLong), false)
			}, null, false),
			new RulePhrase("CustomAttribute12", Strings.ConditionalCustomAttribute12Text, new FormletParameter[]
			{
				new StringParameter("customattribute12", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextLong), false)
			}, null, false),
			new RulePhrase("CustomAttribute13", Strings.ConditionalCustomAttribute13Text, new FormletParameter[]
			{
				new StringParameter("customattribute13", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextLong), false)
			}, null, false),
			new RulePhrase("CustomAttribute14", Strings.ConditionalCustomAttribute14Text, new FormletParameter[]
			{
				new StringParameter("customattribute14", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextLong), false)
			}, null, false),
			new RulePhrase("CustomAttribute15", Strings.ConditionalCustomAttribute15Text, new FormletParameter[]
			{
				new StringParameter("customattribute15", Strings.CustomAttributeDialogTitle, Strings.CustomAttributeDialogTitle, typeof(FilterFieldTextLong), false)
			}, null, false)
		};
	}
}
