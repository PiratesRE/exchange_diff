using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class UserRecipientFilterPredicate : PredicateCondition
	{
		public UserRecipientFilterPredicate(Property property, ShortList<string> valueEntries, RulesCreationContext creationContext) : base(property, valueEntries, creationContext)
		{
			if (!base.Property.IsString && !typeof(string).IsAssignableFrom(base.Property.Type))
			{
				throw new RulesValidationException(RulesTasksStrings.ClientAccessRulesFilterPropertyRequired(this.Name));
			}
		}

		public override string Name
		{
			get
			{
				return "UserRecipientFilterPredicate";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return UserRecipientFilterPredicate.PredicateBaseVersion;
			}
		}

		public string UserRecipientFilter
		{
			get
			{
				return ((IEnumerable<string>)base.Value.ParsedValue).FirstOrDefault<string>();
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			ClientAccessRulesEvaluationContext clientAccessRulesEvaluationContext = (ClientAccessRulesEvaluationContext)baseContext;
			if (clientAccessRulesEvaluationContext.UserSchema != null && clientAccessRulesEvaluationContext.User != null)
			{
				try
				{
					QueryParser queryParser = new QueryParser(this.UserRecipientFilter, clientAccessRulesEvaluationContext.UserSchema, QueryParser.Capabilities.All, null, new QueryParser.ConvertValueFromStringDelegate(QueryParserUtils.ConvertValueFromString));
					return OpathFilterEvaluator.FilterMatches(queryParser.ParseTree, clientAccessRulesEvaluationContext.User);
				}
				catch (ParsingException ex)
				{
					ExTraceGlobals.ClientAccessRulesTracer.TraceDebug(0L, string.Format("Unexpected exception: {0}", ex.ToString()));
				}
				catch (DataSourceOperationException ex2)
				{
					ExTraceGlobals.ClientAccessRulesTracer.TraceDebug(0L, string.Format("Missing information in property bag to process Monad Filter rule", ex2.ToString()));
				}
				return false;
			}
			return false;
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(entries);
		}

		public const string Tag = "UserRecipientFilterPredicate";

		private static readonly Version PredicateBaseVersion = new Version("15.00.0015.00");
	}
}
