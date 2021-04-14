using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Office.CompliancePolicy.ComplianceData;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public sealed class PolicyEngine
	{
		private PolicyEngine()
		{
		}

		public static PolicyEngine Instance
		{
			get
			{
				return PolicyEngine.instance;
			}
		}

		public void Execute(PolicyEvaluationContext context, ICollection<PolicyRule> policyRules)
		{
			if (context.ComplianceItemPagedReader != null)
			{
				PolicyEngine.ExecuteOnPagedReader(context, policyRules);
				return;
			}
			PolicyEngine.ExecuteOnSingleItem(context, policyRules);
		}

		internal static void ExecuteOnPagedReader(PolicyEvaluationContext context, ICollection<PolicyRule> policyRules)
		{
			PolicyEngine.Trace(context, "Evaluating rule collection on a paged reader", new object[0]);
			if (PolicyEngine.EnterRuleCollection(context) == ExecutionControl.Execute)
			{
				foreach (PolicyRule policyRule in policyRules)
				{
					if (PolicyEngine.ShouldEvaluateRule(context, policyRule))
					{
						PolicyEngine.Trace(context, "Evaluating rule '{0}'", new object[]
						{
							policyRule.Name
						});
						context.CurrentRule = policyRule;
						ExecutionControl executionControl = PolicyEngine.EnterRule(policyRule, context);
						if (executionControl == ExecutionControl.SkipThis)
						{
							PolicyEngine.Trace(context, "Skip rule '{0}' after calling EnterRule", new object[]
							{
								policyRule.Name
							});
						}
						else
						{
							if (executionControl == ExecutionControl.SkipAll)
							{
								PolicyEngine.Trace(context, "Skip rule collection after calling EnterRule", new object[0]);
								break;
							}
							bool flag = PolicyEngine.ApplyActionsOnPagedReaderResults(context, policyRule);
							if (PolicyEngine.ExitRule(policyRule, context) == ExecutionControl.SkipAll || flag)
							{
								PolicyEngine.Trace(context, "Skip rule collection after calling ExitRule", new object[0]);
								break;
							}
						}
					}
				}
			}
			PolicyEngine.Trace(context, "Finished rule collection evaluation", new object[0]);
			PolicyEngine.ExitRuleCollection(context);
		}

		internal static bool ApplyActionsOnPagedReaderResults(PolicyEvaluationContext context, PolicyRule rule)
		{
			QueryPredicate queryPredicate = rule.Condition as QueryPredicate;
			if (queryPredicate == null)
			{
				throw new CompliancePolicyValidationException("The Query based rule outer predicate must be of a QueryBasedPredicate type");
			}
			context.ComplianceItemPagedReader.Condition = queryPredicate;
			IEnumerable<ComplianceItem> nextPage = context.ComplianceItemPagedReader.GetNextPage();
			while (nextPage != null && nextPage.Any<ComplianceItem>())
			{
				foreach (ComplianceItem sourceItem in nextPage)
				{
					context.SourceItem = sourceItem;
					PolicyEngine.ExecuteActions(context, rule);
				}
				nextPage = context.ComplianceItemPagedReader.GetNextPage();
			}
			context.SourceItem = null;
			return false;
		}

		internal static void ExecuteOnSingleItem(PolicyEvaluationContext context, ICollection<PolicyRule> policyRules)
		{
			bool flag = false;
			PolicyEngine.Trace(context, "Evaluating rule collection", new object[0]);
			if (PolicyEngine.EnterRuleCollection(context) == ExecutionControl.Execute)
			{
				foreach (PolicyRule policyRule in policyRules)
				{
					if (PolicyEngine.ShouldEvaluateRule(context, policyRule))
					{
						PolicyEngine.Trace(context, "Evaluating rule '{0}'", new object[]
						{
							policyRule.Name
						});
						context.CurrentRule = policyRule;
						if (context.ComplianceItemPagedReader == null)
						{
							context.RulesEvaluationHistory.AddRuleEvaluationResult(context);
						}
						ExecutionControl executionControl = PolicyEngine.EnterRule(policyRule, context);
						if (executionControl == ExecutionControl.SkipThis)
						{
							PolicyEngine.Trace(context, "Skip rule '{0}' after calling EnterRule", new object[]
							{
								policyRule.Name
							});
						}
						else
						{
							if (executionControl == ExecutionControl.SkipAll)
							{
								PolicyEngine.Trace(context, "Skip rule collection after calling EnterRule", new object[0]);
								break;
							}
							if (PolicyEngine.EvaluateCondition(policyRule.Condition, context))
							{
								flag = PolicyEngine.ExecuteActions(context, policyRule);
							}
							if (PolicyEngine.ExitRule(policyRule, context) == ExecutionControl.SkipAll || flag)
							{
								PolicyEngine.Trace(context, "Skip rule collection after calling ExitRule", new object[0]);
								break;
							}
						}
					}
				}
			}
			PolicyEngine.Trace(context, "Finished rule collection evaluation", new object[0]);
			PolicyEngine.ExitRuleCollection(context);
		}

		private static bool EvaluateCondition(Condition condition, PolicyEvaluationContext context)
		{
			if (condition == null)
			{
				return true;
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool flag = condition.Evaluate(context);
			stopwatch.Stop();
			if (context.ComplianceItemPagedReader == null)
			{
				RuleEvaluationResult currentRuleResult = context.RulesEvaluationHistory.GetCurrentRuleResult(context);
				currentRuleResult.Predicates.Add(new PredicateEvaluationResult(condition.GetType(), flag, new List<string>(), 0, stopwatch.Elapsed));
			}
			return flag;
		}

		private static bool ShouldEvaluateRule(PolicyEvaluationContext context, PolicyRule rule)
		{
			if (rule.IsTooAdvancedToParse)
			{
				PolicyEngine.Trace(context, "Skip rule which cannot be parsed due to server being too low of a version '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			if (rule.Enabled != RuleState.Enabled)
			{
				PolicyEngine.Trace(context, "Skip disabled rule '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			if (rule.Mode == RuleMode.PendingDeletion)
			{
				PolicyEngine.Trace(context, "Skip deleted rule '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			if (rule.ExpiryDate != null && DateTime.UtcNow > rule.ExpiryDate.Value.ToUniversalTime())
			{
				PolicyEngine.Trace(context, "Skip rule past expiration date '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			if (rule.ActivationDate != null && DateTime.UtcNow < rule.ActivationDate.Value.ToUniversalTime())
			{
				PolicyEngine.Trace(context, "Skip rule that has not reached activation date '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			if (rule.Actions.Count == 0)
			{
				PolicyEngine.Trace(context, "Skip rule without actions '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			return true;
		}

		private static bool ExecuteActions(PolicyEvaluationContext context, PolicyRule rule)
		{
			PolicyEngine.Trace(context, "Execute Actions for rule '{0}' ", new object[]
			{
				rule.Name
			});
			bool result = false;
			if (PolicyEngine.EnterRuleActionBlock(rule, context))
			{
				using (IEnumerator<Action> enumerator = rule.Actions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Action action = enumerator.Current;
						if (action.ShouldExecute(rule.Mode))
						{
							PolicyEngine.Trace(context, "Execute Action '{0}' ", new object[]
							{
								action.Name
							});
							if (PolicyEngine.ExecuteAction(action, context) == ExecutionControl.SkipAll)
							{
								PolicyEngine.Trace(context, "Action '{0}' halted rules evaluation", new object[]
								{
									action.Name
								});
								result = true;
							}
						}
						else
						{
							PolicyEngine.Trace(context, "Audit Action '{0}'", new object[]
							{
								action.Name
							});
							PolicyEngine.AuditAction(action, context);
						}
					}
					goto IL_FA;
				}
			}
			result = true;
			PolicyEngine.Trace(context, "Actions execution for rule '{0}' skipped by EnterRuleActionBlock result", new object[]
			{
				rule.Name
			});
			IL_FA:
			PolicyEngine.Trace(context, "Finished execution of Actions for rule '{0}' ", new object[]
			{
				rule.Name
			});
			PolicyEngine.ExitRuleActionBlock(rule, context);
			return result;
		}

		private static ExecutionControl EnterRuleCollection(PolicyEvaluationContext context)
		{
			return ExecutionControl.Execute;
		}

		private static ExecutionControl EnterRule(PolicyRule rule, PolicyEvaluationContext context)
		{
			return ExecutionControl.Execute;
		}

		private static ExecutionControl ExecuteAction(Action action, PolicyEvaluationContext context)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			ExecutionControl result = action.Execute(context);
			stopwatch.Stop();
			if (context.ComplianceItemPagedReader == null)
			{
				RuleEvaluationResult currentRuleResult = context.RulesEvaluationHistory.GetCurrentRuleResult(context);
				currentRuleResult.Actions.Add(new PolicyHistoryResult(action.GetType(), new List<string>(), 0, stopwatch.Elapsed));
			}
			return result;
		}

		private static void AuditAction(Action action, PolicyEvaluationContext context)
		{
		}

		private static ExecutionControl ExitRule(PolicyRule rule, PolicyEvaluationContext context)
		{
			return ExecutionControl.Execute;
		}

		private static bool EnterRuleActionBlock(PolicyRule rule, PolicyEvaluationContext context)
		{
			return true;
		}

		private static void ExitRuleActionBlock(PolicyRule rule, PolicyEvaluationContext context)
		{
		}

		private static void ExitRuleCollection(PolicyEvaluationContext context)
		{
		}

		private static void Trace(PolicyEvaluationContext context, string traceMessageFormat, params object[] args)
		{
			if (context.Tracer != null)
			{
				context.Tracer.TraceDebug(traceMessageFormat, args);
			}
		}

		private static readonly PolicyEngine instance = new PolicyEngine();
	}
}
