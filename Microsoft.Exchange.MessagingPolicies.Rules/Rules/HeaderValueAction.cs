using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal abstract class HeaderValueAction : TransportAction
	{
		protected HeaderValueAction(ShortList<Argument> arguments) : base(arguments)
		{
			if (!(base.Arguments[0] is Value) || !(base.Arguments[1] is Value))
			{
				throw new RulesValidationException(RulesStrings.ActionRequiresConstantArguments(this.Name));
			}
			string text = (string)base.Arguments[0].GetValue(null);
			string value = (string)base.Arguments[1].GetValue(null);
			if (!TransportUtils.IsHeaderValid(text))
			{
				throw new RulesValidationException(TransportRulesStrings.InvalidHeaderName(text));
			}
			if (!TransportUtils.IsHeaderSettable(text, value))
			{
				throw new RulesValidationException(TransportRulesStrings.CannotSetHeader(text, value));
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return HeaderValueAction.argumentTypes;
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.BifurcationNeeded;
			}
		}

		public const string SubjectHeader = "subject";

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string),
			typeof(string)
		};
	}
}
