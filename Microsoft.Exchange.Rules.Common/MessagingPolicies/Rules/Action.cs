using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public abstract class Action
	{
		public Action(ShortList<Argument> arguments)
		{
			this.ValidateArguments(arguments);
			this.arguments = arguments;
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
				return Rule.BaseVersion;
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

		public virtual void ValidateArguments(ShortList<Argument> inputArguments)
		{
			Type[] argumentsType = this.ArgumentsType;
			if (argumentsType.Length != inputArguments.Count)
			{
				throw new RulesValidationException(RulesStrings.ActionArgumentMismatch(this.Name));
			}
			if (argumentsType.Where((Type t, int index) => t != inputArguments[index].Type).Any<Type>())
			{
				throw new RulesValidationException(RulesStrings.ActionArgumentMismatch(this.Name));
			}
			if (inputArguments.OfType<Value>().Any((Value value) => value.RawValues.Count > 1))
			{
				throw new RulesValidationException(RulesStrings.NoMultiValueForActionArgument);
			}
		}

		public virtual bool ShouldExecute(RuleMode mode, RulesEvaluationContext context)
		{
			return RuleMode.Enforce == mode && context.ShouldExecuteActions;
		}

		public virtual int GetEstimatedSize()
		{
			int num = 18;
			if (this.arguments != null)
			{
				num += 18;
				foreach (Argument argument in this.arguments)
				{
					num += argument.GetEstimatedSize();
				}
			}
			if (this.externalName != null)
			{
				num += this.externalName.Length * 2;
			}
			return num;
		}

		public ExecutionControl Execute(RulesEvaluationContext context)
		{
			ExecutionControl result = this.OnExecute(context);
			this.LogActionExecution(context);
			return result;
		}

		protected abstract ExecutionControl OnExecute(RulesEvaluationContext context);

		private void LogActionExecution(RulesEvaluationContext context)
		{
			if (context.NeedsLogging)
			{
				StringBuilder stringBuilder = new StringBuilder(this.Arguments.Count * 10);
				for (int i = 0; i < this.Arguments.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(";");
					}
					stringBuilder.Append(this.Arguments[i].GetValue(context).ToString());
				}
				context.LogActionExecution(this.Name, stringBuilder.ToString());
			}
		}

		internal static readonly Type[] DefaultArgumentsTypeList = new Type[0];

		private ShortList<Argument> arguments = new ShortList<Argument>();

		private string externalName;
	}
}
