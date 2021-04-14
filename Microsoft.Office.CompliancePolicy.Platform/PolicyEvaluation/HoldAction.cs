using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class HoldAction : Action
	{
		public HoldAction(List<Argument> arguments, string externalName = null) : base(arguments, externalName)
		{
			if (arguments == null)
			{
				throw new CompliancePolicyValidationException("Argument list is null");
			}
			string text = arguments[0].GetValue(null).ToString();
			int num;
			this.HoldDurationDays = ((!int.TryParse(text, out num)) ? int.MaxValue : num);
			if (arguments.Count == 2)
			{
				text = arguments[1].GetValue(null).ToString();
				HoldDurationHint holdDurationHint;
				this.HoldDurationDisplayHint = ((!Enum.TryParse<HoldDurationHint>(text, out holdDurationHint)) ? HoldDurationHint.Days : holdDurationHint);
				return;
			}
			this.HoldDurationDisplayHint = HoldDurationHint.Days;
		}

		public override string Name
		{
			get
			{
				return "Hold";
			}
		}

		public HoldDurationHint HoldDurationDisplayHint { get; private set; }

		public int HoldDurationDays { get; private set; }

		public override Type[] ArgumentsType
		{
			get
			{
				return HoldAction.argumentTypes;
			}
		}

		public override void ValidateArguments(List<Argument> inputArguments)
		{
			if (inputArguments == null)
			{
				throw new CompliancePolicyValidationException("Argument list is null - action '{0}'", new object[]
				{
					this.Name
				});
			}
			if (inputArguments.Count == this.ArgumentsType.Count<Type>())
			{
				if (!this.ArgumentsType.SequenceEqual(from x in inputArguments
				select x.Type))
				{
					throw new CompliancePolicyValidationException("Argument list mismatches - action '{0}'", new object[]
					{
						this.Name
					});
				}
			}
			else
			{
				if (inputArguments.Count != this.ArgumentsType.Count<Type>() - 1)
				{
					throw new CompliancePolicyValidationException("Argument list mismatches - action '{0}'", new object[]
					{
						this.Name
					});
				}
				if (this.ArgumentsType.First<Type>() != inputArguments.First<Argument>().Type)
				{
					throw new CompliancePolicyValidationException("Argument list mismatches - action '{0}'", new object[]
					{
						this.Name
					});
				}
			}
		}

		protected override ExecutionControl OnExecute(PolicyEvaluationContext context)
		{
			throw new NotImplementedException("The HoldAction can only be used for PS object model serialization. Workloads must implement OnExecute.");
		}

		public const string ActionName = "Hold";

		internal const int PermanentHoldDurationValue = 0;

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string),
			typeof(string)
		};
	}
}
