using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public abstract class Action
	{
		public Action(List<Argument> arguments, string externalName = null)
		{
			this.ValidateArguments(arguments);
			this.arguments = arguments;
			if (externalName != null)
			{
				this.ExternalName = externalName;
			}
		}

		public abstract string Name { get; }

		public string ExternalName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.externalName))
				{
					return this.Name;
				}
				return this.externalName;
			}
			set
			{
				this.externalName = value;
			}
		}

		public bool HasExternalName
		{
			get
			{
				return !string.IsNullOrWhiteSpace(this.externalName) && !this.Name.Equals(this.externalName);
			}
		}

		public virtual Version MinimumVersion
		{
			get
			{
				return PolicyRule.BaseVersion;
			}
		}

		public IList<Argument> Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		public virtual Type[] ArgumentsType
		{
			get
			{
				return Action.DefaultArgumentsTypeList;
			}
		}

		public virtual void ValidateArguments(List<Argument> inputArguments)
		{
			if (inputArguments == null)
			{
				throw new CompliancePolicyValidationException("Argument list is null - action '{0}'", new object[]
				{
					this.Name
				});
			}
			Type[] argumentsType = this.ArgumentsType;
			if (argumentsType.Length != inputArguments.Count)
			{
				throw new CompliancePolicyValidationException("Argument list mismatches - action '{0}'", new object[]
				{
					this.Name
				});
			}
			if (argumentsType.Where((Type t, int index) => t != inputArguments[index].Type).Any<Type>())
			{
				throw new CompliancePolicyValidationException("Argument list mismatches - action '{0}'", new object[]
				{
					this.Name
				});
			}
		}

		public virtual bool ShouldExecute(RuleMode mode)
		{
			return RuleMode.Enforce == mode;
		}

		public ExecutionControl Execute(PolicyEvaluationContext context)
		{
			return this.OnExecute(context);
		}

		protected abstract ExecutionControl OnExecute(PolicyEvaluationContext context);

		internal static readonly Type[] DefaultArgumentsTypeList = new Type[0];

		private List<Argument> arguments = new List<Argument>();

		private string externalName;
	}
}
