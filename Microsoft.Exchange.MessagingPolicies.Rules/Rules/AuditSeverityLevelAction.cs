using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AuditSeverityLevelAction : TransportAction
	{
		public AuditSeverityLevelAction(ShortList<Argument> arguments) : base(arguments)
		{
			string text = arguments[0].GetValue(null).ToString();
			if (Enum.TryParse<AuditSeverityLevel>(text, out this.severityLevel))
			{
				return;
			}
			if (text.Equals("Informational", StringComparison.OrdinalIgnoreCase))
			{
				this.severityLevel = AuditSeverityLevel.Low;
				return;
			}
			if (text.Equals("AuditOff", StringComparison.OrdinalIgnoreCase))
			{
				this.severityLevel = AuditSeverityLevel.DoNotAudit;
				return;
			}
			throw new RulesValidationException(TransportRulesStrings.InvalidAuditSeverityLevel(text));
		}

		public override Version MinimumVersion
		{
			get
			{
				return HasSenderOverridePredicate.ComplianceProgramsBaseVersion;
			}
		}

		public override string Name
		{
			get
			{
				return "AuditSeverityLevel";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return AuditSeverityLevelAction.argumentTypes;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.CurrentAuditSeverityLevel = this.severityLevel;
			return ExecutionControl.Execute;
		}

		private static readonly Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};

		private readonly AuditSeverityLevel severityLevel;
	}
}
