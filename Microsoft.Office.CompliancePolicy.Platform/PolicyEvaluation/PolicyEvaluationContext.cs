using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy.Classification;
using Microsoft.Office.CompliancePolicy.ComplianceData;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public sealed class PolicyEvaluationContext
	{
		private PolicyEvaluationContext()
		{
		}

		public ComplianceItem SourceItem { get; internal set; }

		public ComplianceItemPagedReader ComplianceItemPagedReader { get; private set; }

		public IClassificationRuleStore ClassificationStore { get; set; }

		public ExecutionLog ExecutionLog { get; private set; }

		public Auditor Auditor { get; private set; }

		public ICollection<PolicyRule> Rules { get; set; }

		public ITracer Tracer { get; set; }

		public PolicyRule CurrentRule { get; set; }

		public RulesEvaluationHistory RulesEvaluationHistory { get; set; }

		public static PolicyEvaluationContext Create(ComplianceItem sourceItem)
		{
			return PolicyEvaluationContext.Create(sourceItem, null, null, null);
		}

		public static PolicyEvaluationContext Create(ComplianceItem sourceItem, string correlationId, ExecutionLog executionLog)
		{
			return PolicyEvaluationContext.Create(sourceItem, correlationId, executionLog, null);
		}

		public static PolicyEvaluationContext Create(ComplianceItem sourceItem, Auditor auditor)
		{
			return PolicyEvaluationContext.Create(sourceItem, null, null, auditor);
		}

		public static PolicyEvaluationContext Create(ComplianceItem sourceItem, string correlationId, ExecutionLog executionLog, Auditor auditor)
		{
			if (sourceItem == null)
			{
				throw new ArgumentNullException("sourceItem");
			}
			if (executionLog != null && string.IsNullOrWhiteSpace(correlationId))
			{
				throw new ArgumentException("correlationId must not be empty or null if ExecutionLog is supplied");
			}
			return new PolicyEvaluationContext
			{
				SourceItem = sourceItem,
				ExecutionLog = executionLog,
				Auditor = auditor,
				RulesEvaluationHistory = new RulesEvaluationHistory()
			};
		}

		public static PolicyEvaluationContext Create(ComplianceItemPagedReader complianceItemPagedReader)
		{
			return PolicyEvaluationContext.Create(complianceItemPagedReader, null, null, null);
		}

		public static PolicyEvaluationContext Create(ComplianceItemPagedReader complianceItemPagedReader, string correlationId, ExecutionLog executionLog)
		{
			return PolicyEvaluationContext.Create(complianceItemPagedReader, correlationId, executionLog, null);
		}

		public static PolicyEvaluationContext Create(ComplianceItemPagedReader complianceItemPagedReader, Auditor auditor)
		{
			return PolicyEvaluationContext.Create(complianceItemPagedReader, null, null, auditor);
		}

		public static PolicyEvaluationContext Create(ComplianceItemPagedReader complianceItemPagedReader, string correlationId, ExecutionLog executionLog, Auditor auditor)
		{
			if (complianceItemPagedReader == null)
			{
				throw new ArgumentNullException("complianceItemPagedReader");
			}
			if (executionLog != null && string.IsNullOrWhiteSpace(correlationId))
			{
				throw new ArgumentException("correlationId must not be empty or null if ExecutionLog is supplied");
			}
			return new PolicyEvaluationContext
			{
				ComplianceItemPagedReader = complianceItemPagedReader,
				ExecutionLog = executionLog,
				Auditor = auditor
			};
		}

		public void SetConditionEvaluationMode(ConditionEvaluationMode mode)
		{
			foreach (PolicyRule policyRule in this.Rules)
			{
				PolicyEvaluationContext.SetConditionTreeEvaluationMode(policyRule.Condition, mode);
			}
		}

		private static void SetConditionTreeEvaluationMode(Condition condition, ConditionEvaluationMode mode)
		{
			condition.EvaluationMode = mode;
			OrCondition orCondition = condition as OrCondition;
			if (orCondition != null)
			{
				using (List<Condition>.Enumerator enumerator = orCondition.SubConditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Condition condition2 = enumerator.Current;
						PolicyEvaluationContext.SetConditionTreeEvaluationMode(condition2, mode);
					}
					return;
				}
			}
			AndCondition andCondition = condition as AndCondition;
			if (andCondition != null)
			{
				foreach (Condition condition3 in andCondition.SubConditions)
				{
					PolicyEvaluationContext.SetConditionTreeEvaluationMode(condition3, mode);
				}
			}
		}
	}
}
