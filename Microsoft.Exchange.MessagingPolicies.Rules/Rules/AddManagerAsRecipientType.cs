using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class AddManagerAsRecipientType : TransportAction
	{
		public AddManagerAsRecipientType(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Version MinimumVersion
		{
			get
			{
				return TransportRuleConstants.VersionedContainerBaseVersion;
			}
		}

		public override string Name
		{
			get
			{
				return "AddManagerAsRecipientType";
			}
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return AddManagerAsRecipientType.argumentTypes;
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.BifurcationNeeded;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			string senderManagerAddress = TransportUtils.GetSenderManagerAddress(baseContext);
			if (string.IsNullOrEmpty(senderManagerAddress))
			{
				return ExecutionControl.Execute;
			}
			TransportRulesEvaluationContext context = (TransportRulesEvaluationContext)baseContext;
			TransportAction transportAction = null;
			ShortList<Argument> shortList = new ShortList<Argument>();
			shortList.Add(new Value(senderManagerAddress));
			string text = (string)base.Arguments[0].GetValue(context);
			string a;
			if ((a = text.ToLower()) != null)
			{
				if (!(a == "to"))
				{
					if (!(a == "cc"))
					{
						if (!(a == "bcc"))
						{
							if (a == "redirect")
							{
								transportAction = new RedirectMessage(shortList);
							}
						}
						else
						{
							transportAction = new AddEnvelopeRecipient(shortList);
						}
					}
					else
					{
						transportAction = new AddCcRecipientSmtpOnly(shortList);
					}
				}
				else
				{
					transportAction = new AddToRecipientSmtpOnly(shortList);
				}
			}
			return transportAction.Execute(baseContext);
		}

		private static Type[] argumentTypes = new Type[]
		{
			typeof(string)
		};
	}
}
