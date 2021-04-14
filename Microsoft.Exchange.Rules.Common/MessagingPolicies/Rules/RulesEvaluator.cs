using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class RulesEvaluator
	{
		public RulesEvaluator(RulesEvaluationContext context)
		{
			this.context = context;
		}

		public RuleCollection Rules
		{
			get
			{
				return this.Context.Rules;
			}
		}

		public RulesEvaluationContext Context
		{
			get
			{
				return this.context;
			}
		}

		public void Run()
		{
			bool flag = false;
			this.Trace("Evaluating rule collection '{0}'", new object[]
			{
				this.Rules.Name
			});
			this.EnterRuleCollection();
			if (this.EnterRuleCollection() == ExecutionControl.Execute)
			{
				foreach (Rule rule in this.Rules)
				{
					if (this.ShouldEvaluateRule(rule))
					{
						this.Trace("Evaluating rule '{0}'", new object[]
						{
							rule.Name
						});
						this.Context.CurrentRule = rule;
						this.Context.ShouldExecuteActions = true;
						ExecutionControl executionControl = this.EnterRule();
						if (executionControl == ExecutionControl.SkipThis)
						{
							this.Trace("Skip rule '{0}' after calling EnterRule", new object[]
							{
								rule.Name
							});
						}
						else
						{
							if (executionControl == ExecutionControl.SkipAll)
							{
								this.Trace("Skip rule collection '{0}' after calling EnterRule", new object[]
								{
									this.Rules.Name
								});
								break;
							}
							if (this.EvaluateCondition(rule.Condition, this.Context))
							{
								flag = this.ExecuteActions();
							}
							else if (this.context.NeedsLogging)
							{
								this.context.LogActionExecution("NoAction", "Conditions evaluated to false.  Rule skipped.");
							}
							if (this.ExitRule() == ExecutionControl.SkipAll || flag)
							{
								this.Trace("Skip rule collection '{0}' after calling ExitRule", new object[]
								{
									this.Rules.Name
								});
								break;
							}
						}
					}
				}
			}
			this.Trace("Finished evaluation of rule collection '{0}'", new object[]
			{
				this.Rules.Name
			});
			this.ExitRuleCollection();
		}

		protected virtual bool EvaluateCondition(Condition condition, RulesEvaluationContext evaluationContext)
		{
			this.Context.RulesEvaluationHistory.AddRuleEvaluationResult(this.Context);
			bool flag = condition.Evaluate(evaluationContext);
			this.context.RulesEvaluationHistory.SetCurrentRuleIsMatch(this.Context, flag);
			this.Trace("Condition evaluated as {0}Match", new object[]
			{
				flag ? string.Empty : "Not "
			});
			return flag;
		}

		private bool ShouldEvaluateRule(Rule rule)
		{
			if (rule.IsTooAdvancedToParse)
			{
				this.Trace("Skip rule which cannot be parsed due to server being too low of a version '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			if (rule.Enabled != RuleState.Enabled)
			{
				this.Trace("Skip disabled rule '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			if (rule.ExpiryDate != null && DateTime.UtcNow > rule.ExpiryDate.Value.ToUniversalTime())
			{
				this.Trace("Skip rule past expiration date '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			if (rule.ActivationDate != null && DateTime.UtcNow < rule.ActivationDate.Value.ToUniversalTime())
			{
				this.Trace("Skip rule that has not reached activation date '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			if (rule.Actions.Count == 0)
			{
				this.Trace("Skip rule without actions '{0}'", new object[]
				{
					rule.Name
				});
				return false;
			}
			return true;
		}

		private bool ExecuteActions()
		{
			Rule currentRule = this.Context.CurrentRule;
			this.Trace("Execute Actions for rule '{0}' ", new object[]
			{
				currentRule.Name
			});
			bool result = false;
			if (this.EnterRuleActionBlock())
			{
				using (ShortList<Action>.Enumerator enumerator = currentRule.Actions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Action action = enumerator.Current;
						if (action.ShouldExecute(currentRule.Mode, this.Context))
						{
							this.Trace("Execute Action '{0}' ", new object[]
							{
								action.Name
							});
							if (this.ExecuteAction(action, this.context) == ExecutionControl.SkipAll)
							{
								this.Trace("Action '{0}' halted rules evaluation", new object[]
								{
									action.Name
								});
								result = true;
							}
						}
						else
						{
							this.Trace("Audit Action '{0}'", new object[]
							{
								action.Name
							});
							this.AuditAction(action, this.context);
						}
						RuleEvaluationResult currentRuleResult = this.Context.RulesEvaluationHistory.GetCurrentRuleResult(this.Context);
						if (currentRuleResult != null)
						{
							currentRuleResult.Actions.Add(action.Name);
						}
					}
					goto IL_14C;
				}
			}
			result = true;
			this.Trace("Actions execution for rule '{0}' skipped by EnterRuleActionBlock result", new object[]
			{
				currentRule.Name
			});
			IL_14C:
			this.Trace("Finished execution of Actions for rule '{0}' ", new object[]
			{
				currentRule.Name
			});
			this.ExitRuleActionBlock();
			return result;
		}

		protected virtual ExecutionControl EnterRuleCollection()
		{
			return ExecutionControl.Execute;
		}

		protected virtual ExecutionControl EnterRule()
		{
			return ExecutionControl.Execute;
		}

		protected virtual ExecutionControl ExecuteAction(Action action, RulesEvaluationContext context)
		{
			return action.Execute(context);
		}

		protected virtual void AuditAction(Action action, RulesEvaluationContext context)
		{
		}

		protected virtual ExecutionControl ExitRule()
		{
			return ExecutionControl.Execute;
		}

		protected virtual bool EnterRuleActionBlock()
		{
			return true;
		}

		protected virtual void ExitRuleActionBlock()
		{
		}

		protected virtual void ExitRuleCollection()
		{
		}

		private void Trace(string traceMessageFormat, params object[] args)
		{
			this.context.Trace(traceMessageFormat, args);
		}

		private RulesEvaluationContext context;
	}
}
