using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RuleCondition : RulePhrase
	{
		public RuleCondition(string name, LocalizedString displayText, FormletParameter[] ruleParameters, string additionalRoles, LocalizedString namingFormat, bool isDisplayedInSimpleMode) : this(name, displayText, ruleParameters, additionalRoles, namingFormat, LocalizedString.Empty, LocalizedString.Empty, LocalizedString.Empty, LocalizedString.Empty, isDisplayedInSimpleMode)
		{
		}

		public RuleCondition(string name, LocalizedString displayText, FormletParameter[] ruleParameters, string additionalRoles, LocalizedString namingFormat, LocalizedString groupText, LocalizedString flyOutText, LocalizedString preCannedText, bool isDisplayedInSimpleMode) : this(name, displayText, ruleParameters, additionalRoles, namingFormat, groupText, flyOutText, preCannedText, LocalizedString.Empty, isDisplayedInSimpleMode)
		{
		}

		public RuleCondition(string name, LocalizedString displayText, FormletParameter[] ruleParameters, string additionalRoles, LocalizedString namingFormat, LocalizedString groupText, LocalizedString flyOutText, LocalizedString preCannedText, LocalizedString explanationText, bool isDisplayedInSimpleMode) : base(name, displayText, ruleParameters, additionalRoles, groupText, flyOutText, explanationText, isDisplayedInSimpleMode)
		{
			this.namingFormat = namingFormat;
			this.preCannedText = preCannedText;
		}

		[DataMember]
		public string NamingFormat
		{
			get
			{
				return this.namingFormat.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string PreCannedText
		{
			get
			{
				return this.preCannedText.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private LocalizedString namingFormat;

		private LocalizedString preCannedText;
	}
}
