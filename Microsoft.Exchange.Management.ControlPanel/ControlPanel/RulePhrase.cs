using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(RuleCondition))]
	public class RulePhrase
	{
		public RulePhrase(string name, LocalizedString displayText, FormletParameter[] ruleParameters, string additionalRoles, bool isDisplayedInSimpleMode) : this(name, displayText, ruleParameters, additionalRoles, LocalizedString.Empty, LocalizedString.Empty, LocalizedString.Empty, isDisplayedInSimpleMode, false)
		{
		}

		public RulePhrase(string name, LocalizedString displayText, FormletParameter[] ruleParameters, string additionalRoles, LocalizedString groupText, LocalizedString flyOutText, bool isDisplayedInSimpleMode, bool stopProcessingRulesByDefault) : this(name, displayText, ruleParameters, additionalRoles, groupText, flyOutText, LocalizedString.Empty, isDisplayedInSimpleMode, stopProcessingRulesByDefault)
		{
		}

		public RulePhrase(string name, LocalizedString displayText, FormletParameter[] ruleParameters, string additionalRoles, LocalizedString groupText, LocalizedString flyOutText, bool isDisplayedInSimpleMode) : this(name, displayText, ruleParameters, additionalRoles, groupText, flyOutText, LocalizedString.Empty, isDisplayedInSimpleMode, false)
		{
		}

		public RulePhrase(string name, LocalizedString displayText, FormletParameter[] ruleParameters, string additionalRoles, LocalizedString groupText, LocalizedString flyOutText, LocalizedString explanationText, bool isDisplayedInSimpleMode) : this(name, displayText, ruleParameters, additionalRoles, groupText, flyOutText, explanationText, isDisplayedInSimpleMode, false)
		{
		}

		public RulePhrase(string name, LocalizedString displayText, FormletParameter[] ruleParameters, string additionalRoles, LocalizedString groupText, LocalizedString flyOutText, LocalizedString explanationText, bool isDisplayedInSimpleMode, bool stopProcessingRulesByDefault)
		{
			this.Name = name;
			this.displayText = displayText;
			this.Parameters = ruleParameters;
			this.AdditionalRoles = additionalRoles;
			this.groupText = groupText;
			this.flyOutText = flyOutText;
			this.DisplayedInSimpleMode = isDisplayedInSimpleMode;
			this.explanationText = explanationText;
			this.StopProcessingRulesByDefault = stopProcessingRulesByDefault;
		}

		[DataMember]
		public string Name { get; private set; }

		[DataMember]
		public string DisplayText
		{
			get
			{
				return this.displayText.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string GroupText
		{
			get
			{
				return this.groupText.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string FlyOutText
		{
			get
			{
				return this.flyOutText.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ExplanationText
		{
			get
			{
				return this.explanationText.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public FormletParameter[] Parameters { get; private set; }

		[DataMember]
		public bool DisplayedInSimpleMode { get; internal set; }

		[DataMember]
		public bool StopProcessingRulesByDefault { get; private set; }

		public string AdditionalRoles { get; private set; }

		private LocalizedString displayText;

		private LocalizedString groupText;

		private LocalizedString flyOutText;

		private LocalizedString explanationText;
	}
}
