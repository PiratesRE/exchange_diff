using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class AnyOfProtocolsPredicate : PredicateCondition
	{
		public AnyOfProtocolsPredicate(Property property, ShortList<string> valueEntries, RulesCreationContext creationContext) : base(property, valueEntries, creationContext)
		{
			if (!base.Property.IsString && !typeof(ClientAccessProtocol).IsAssignableFrom(base.Property.Type))
			{
				throw new RulesValidationException(RulesTasksStrings.ClientAccessRulesProtocolPropertyRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "anyOfProtocolsPredicate";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return AnyOfProtocolsPredicate.PredicateBaseVersion;
			}
		}

		public IEnumerable<ClientAccessProtocol> ProtocolList
		{
			get
			{
				return (IEnumerable<ClientAccessProtocol>)base.Value.ParsedValue;
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			ClientAccessProtocol protocol = clientAccessRulesEvaluationContext.Protocol;
			return this.ProtocolList.Contains(protocol);
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(entries.Select(delegate(string protocolString)
			{
				int result;
				if (int.TryParse(protocolString, out result))
				{
					return (ClientAccessProtocol)result;
				}
				if (protocolString != null)
				{
					if (protocolString == "EWS")
					{
						return ClientAccessProtocol.ExchangeWebServices;
					}
					if (protocolString == "RPS")
					{
						return ClientAccessProtocol.RemotePowerShell;
					}
					if (protocolString == "OA")
					{
						return ClientAccessProtocol.OutlookAnywhere;
					}
				}
				return (ClientAccessProtocol)Enum.Parse(typeof(ClientAccessProtocol), protocolString);
			}));
		}

		public const string Tag = "anyOfProtocolsPredicate";

		private static readonly Version PredicateBaseVersion = new Version("15.00.0010.00");
	}
}
