using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class NotifyAuthorsAction : NotifyActionBase
	{
		public NotifyAuthorsAction(List<Argument> arguments, string externalName = null) : base(arguments, externalName)
		{
			this.OverrideOption = (RuleOverrideOptions)Enum.Parse(typeof(RuleOverrideOptions), ((Value)arguments[1]).RawValues[0]);
			if (arguments.Count == 3)
			{
				this.CustomizedContent = (string)arguments[2].GetValue(null);
			}
		}

		public override string Name
		{
			get
			{
				return "NotifyAuthors";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return NotifyAuthorsAction.minVersion;
			}
		}

		protected internal RuleOverrideOptions OverrideOption { get; private set; }

		protected internal string CustomizedContent { get; private set; }

		protected internal string DefaultContent
		{
			get
			{
				return NotifyAuthorsAction.defaultContent;
			}
		}

		public override void ValidateArguments(List<Argument> inputArguments)
		{
			base.ValidateArguments(inputArguments);
			if (inputArguments.Count < 2 || inputArguments.Count > 3)
			{
				throw new CompliancePolicyValidationException("Wrong argument count: Expected 2 or 3, but {0} actually. The - action '{1}'", new object[]
				{
					inputArguments.Count,
					this.Name
				});
			}
			Value value = inputArguments[1] as Value;
			if (value == null || value.ParsedValue == null)
			{
				throw new CompliancePolicyValidationException("Argument 'allow override' must not be empty - action '{0}'", new object[]
				{
					this.Name
				});
			}
			RuleOverrideOptions ruleOverrideOptions;
			if (!Enum.TryParse<RuleOverrideOptions>(value.RawValues[0], true, out ruleOverrideOptions))
			{
				throw new CompliancePolicyValidationException("Argument {0} has the wrong type - action '{1}'", new object[]
				{
					value.RawValues[0],
					this.Name
				});
			}
			if (inputArguments.Count != 3)
			{
				return;
			}
			value = (inputArguments[2] as Value);
			if (value == null || value.ParsedValue == null || !(value.ParsedValue is string))
			{
				throw new CompliancePolicyValidationException("Argument 'notify text' must not be empty - action '{0}'", new object[]
				{
					this.Name
				});
			}
			ArgumentValidator.ThrowIfNullOrWhiteSpace("NotifyText", (string)value.ParsedValue);
		}

		private static readonly Version minVersion = new Version("1.00.0002.000");

		private static readonly string defaultContent = "toBeDefinedContent-NotifyAuthors";
	}
}
