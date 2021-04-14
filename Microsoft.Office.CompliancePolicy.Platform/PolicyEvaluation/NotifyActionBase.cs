using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public abstract class NotifyActionBase : Action
	{
		public NotifyActionBase(List<Argument> arguments, string externalName = null) : base(arguments, externalName)
		{
			List<string> list = (List<string>)arguments[0].GetValue(null);
			this.RecipientList = new ReadOnlyCollection<string>(list);
		}

		protected internal ReadOnlyCollection<string> RecipientList { get; private set; }

		public override void ValidateArguments(List<Argument> inputArguments)
		{
			if (inputArguments == null || !inputArguments.Any<Argument>())
			{
				throw new CompliancePolicyValidationException("Argument list is empty - action '{0}'", new object[]
				{
					this.Name
				});
			}
			NotifyActionBase.ValidateStringListArgument(inputArguments[0], "recipient list", this.Name);
		}

		protected override ExecutionControl OnExecute(PolicyEvaluationContext context)
		{
			throw new NotImplementedException("The NotifyActionBase-based actions can only be used for PS object model serialization. Workloads must implement OnExecute.");
		}

		private static void ValidateStringListArgument(Argument argument, string parameterName, string actionName)
		{
			Value value = argument as Value;
			if (value == null || value.ParsedValue == null)
			{
				throw new CompliancePolicyValidationException("Argument '{0}' must not be empty - action '{1}'", new object[]
				{
					parameterName,
					actionName
				});
			}
			List<string> list = value.ParsedValue as List<string>;
			if (list == null)
			{
				throw new CompliancePolicyValidationException("Argument '{0}' has the wrong type - action '{1}'", new object[]
				{
					parameterName,
					actionName
				});
			}
			if (list.Any<string>())
			{
				if (!list.Any((string p) => string.IsNullOrWhiteSpace(p)))
				{
					return;
				}
			}
			throw new CompliancePolicyValidationException("Argument '{0}' must not be empty or contain empty items - action '{1}'", new object[]
			{
				parameterName,
				actionName
			});
		}
	}
}
