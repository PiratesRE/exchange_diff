using System;
using Microsoft.Exchange.TextProcessing;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class RulesCreationContext
	{
		internal MatchFactory MatchFactory
		{
			get
			{
				return this.matchFactory.Value;
			}
		}

		public int ConditionAndActionSize { get; set; }

		private Lazy<MatchFactory> matchFactory = new Lazy<MatchFactory>();
	}
}
