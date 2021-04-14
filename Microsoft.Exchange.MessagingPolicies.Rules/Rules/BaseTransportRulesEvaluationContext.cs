using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Filtering;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal abstract class BaseTransportRulesEvaluationContext : RulesEvaluationContext
	{
		protected BaseTransportRulesEvaluationContext(RuleCollection rules, ITracer tracer) : base(rules)
		{
			if (tracer == null)
			{
				throw new ArgumentNullException("tracer");
			}
			base.Tracer = tracer;
			this.ActionName = string.Empty;
			this.PredicateName = string.Empty;
		}

		protected abstract FilteringServiceInvokerRequest FilteringServiceInvokerRequest { get; }

		public abstract IStringComparer UserComparer { get; }

		public abstract IStringComparer MembershipChecker { get; }

		public string RuleName { get; set; }

		public string ActionName { get; set; }

		public string PredicateName { get; set; }

		public ExecutionStatus ExecutionStatus { get; set; }

		public Dictionary<string, string> MatchedClassifications
		{
			get
			{
				RuleEvaluationResult currentRuleResult = base.RulesEvaluationHistory.GetCurrentRuleResult(this);
				if (currentRuleResult == null)
				{
					return new Dictionary<string, string>();
				}
				IList<PredicateEvaluationResult> predicateEvaluationResult = RuleEvaluationResult.GetPredicateEvaluationResult(typeof(ContainsDataClassificationPredicate), currentRuleResult.Predicates);
				List<string> list = new List<string>();
				foreach (PredicateEvaluationResult predicateEvaluationResult2 in from mcdc in predicateEvaluationResult
				where mcdc.SupplementalInfo == 2
				select mcdc)
				{
					list.AddRange(predicateEvaluationResult2.MatchResults);
				}
				List<string> list2 = new List<string>();
				foreach (PredicateEvaluationResult predicateEvaluationResult3 in from mcdc in predicateEvaluationResult
				where mcdc.SupplementalInfo == 0
				select mcdc)
				{
					list2.AddRange(predicateEvaluationResult3.MatchResults);
				}
				if (list.Count != list2.Count)
				{
					string message = string.Format("Mismatching classification ID and Name collections IDs count: {0} Names count: {1}", list.Count, list2.Count);
					throw new TransportRulePermanentException(message);
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>(list.Count);
				for (int i = 0; i < list2.Count; i++)
				{
					if (!dictionary.ContainsKey(list[i]))
					{
						dictionary.Add(list[i], list2[i]);
					}
				}
				return dictionary;
			}
		}

		public bool HaveDataClassificationsBeenRetrieved { get; private set; }

		public IEnumerable<DiscoveredDataClassification> DataClassifications
		{
			get
			{
				if (!this.HaveDataClassificationsBeenRetrieved)
				{
					FilteringResults textExtractionResults = null;
					if (this.ShouldInvokeFips())
					{
						this.dataClassifications = FipsFilteringServiceInvoker.GetDataClassifications(base.Rules, this.FilteringServiceInvokerRequest, base.Tracer, out textExtractionResults);
					}
					this.HaveDataClassificationsBeenRetrieved = true;
					this.OnDataClassificationsRetrieved(textExtractionResults);
				}
				return this.dataClassifications;
			}
		}

		protected virtual void OnDataClassificationsRetrieved(FilteringResults textExtractionResults)
		{
		}

		public virtual bool ShouldInvokeFips()
		{
			return true;
		}

		public virtual void ResetPerRuleData()
		{
			this.ActionName = string.Empty;
			this.PredicateName = string.Empty;
		}

		internal void SetTestDataClassifications(IEnumerable<DiscoveredDataClassification> classifications)
		{
			this.SetDataClassifications(classifications);
		}

		internal void SetDataClassifications(IEnumerable<DiscoveredDataClassification> classifications)
		{
			this.dataClassifications = classifications;
			this.HaveDataClassificationsBeenRetrieved = true;
		}

		private IEnumerable<DiscoveredDataClassification> dataClassifications;
	}
}
