using System;
using System.Globalization;

namespace Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection
{
	internal sealed class RightsProtectMessageAction : Action
	{
		public RightsProtectMessageAction(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public static RightsProtectMessageAction Create(Guid templateId, string templateName)
		{
			return new RightsProtectMessageAction(new ShortList<Argument>
			{
				new Value(templateId.ToString("D", CultureInfo.InvariantCulture)),
				new Value(templateName)
			});
		}

		public override string Name
		{
			get
			{
				return "RightsProtectMessage";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return RightsProtectMessageAction.ArgumentTypes;
			}
		}

		public string TemplateId
		{
			get
			{
				if (base.Arguments == null || base.Arguments.Count < 1)
				{
					throw new InvalidOperationException("argument list is corrupted.");
				}
				return (string)base.Arguments[0].GetValue(null);
			}
		}

		public string TemplateName
		{
			get
			{
				if (base.Arguments == null || base.Arguments.Count < 2)
				{
					throw new InvalidOperationException("argument list is corrupted.");
				}
				return (string)base.Arguments[1].GetValue(null);
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext context)
		{
			throw new NotSupportedException("Outlook Protection rules are only executed on Outlook.");
		}

		private static readonly Type[] ArgumentTypes = new Type[]
		{
			typeof(string),
			typeof(string)
		};
	}
}
