using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.TextProcessing;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public abstract class RulesEvaluationContext
	{
		public RulesEvaluationContext(RuleCollection rules)
		{
			this.rules = rules;
		}

		public RuleCollection Rules
		{
			get
			{
				return this.rules;
			}
		}

		public virtual bool NeedsLogging
		{
			get
			{
				return false;
			}
		}

		public ITracer Tracer { get; set; }

		public Rule CurrentRule
		{
			get
			{
				return this.currentRule;
			}
			set
			{
				this.currentRule = value;
			}
		}

		public Dictionary<string, TextScanContext> RegexMatcherCache
		{
			get
			{
				return this.regexMatcherCache;
			}
		}

		public bool ShouldExecuteActions
		{
			get
			{
				return this.shouldExecuteActions;
			}
			set
			{
				this.shouldExecuteActions = value;
			}
		}

		internal RulesEvaluationHistory RulesEvaluationHistory
		{
			get
			{
				return this.rulesEvaluationHistory;
			}
		}

		internal void Trace(string traceMessageFormat, params object[] args)
		{
			if (this.Tracer != null)
			{
				this.Tracer.TraceDebug(traceMessageFormat, args);
			}
		}

		public void AddTextProcessingContext(string textId, TextScanContext context)
		{
			if (!this.regexMatcherCache.ContainsKey(textId))
			{
				this.regexMatcherCache.Add(textId, context);
			}
		}

		public void ResetTextProcessingContext(string textId, bool isMultiValue)
		{
			if (!isMultiValue)
			{
				if (this.regexMatcherCache.ContainsKey(textId))
				{
					this.regexMatcherCache.Remove(textId);
				}
				return;
			}
			List<string> list = new List<string>();
			list.AddRange(from cacheItem in this.RegexMatcherCache
			where cacheItem.Key.StartsWith(textId)
			select cacheItem.Key);
			foreach (string key in list)
			{
				if (this.regexMatcherCache.ContainsKey(key))
				{
					this.regexMatcherCache.Remove(key);
				}
			}
		}

		public virtual void LogActionExecution(string actionName, string details)
		{
		}

		public void SetConditionEvaluationMode(ConditionEvaluationMode mode)
		{
			foreach (Rule rule in this.Rules)
			{
				RulesEvaluationContext.SetConditionTreeEvaluationMode(rule.Condition, mode);
			}
		}

		private static void SetConditionTreeEvaluationMode(Condition condition, ConditionEvaluationMode mode)
		{
			condition.EvaluationMode = mode;
			OrCondition orCondition = condition as OrCondition;
			if (orCondition != null)
			{
				foreach (Condition condition2 in orCondition.SubConditions)
				{
					RulesEvaluationContext.SetConditionTreeEvaluationMode(condition2, mode);
				}
			}
			AndCondition andCondition = condition as AndCondition;
			if (andCondition != null)
			{
				foreach (Condition condition3 in andCondition.SubConditions)
				{
					RulesEvaluationContext.SetConditionTreeEvaluationMode(condition3, mode);
				}
			}
		}

		private RuleCollection rules;

		private Rule currentRule;

		private Dictionary<string, TextScanContext> regexMatcherCache = new Dictionary<string, TextScanContext>();

		private RulesEvaluationHistory rulesEvaluationHistory = new RulesEvaluationHistory();

		private bool shouldExecuteActions = true;
	}
}
