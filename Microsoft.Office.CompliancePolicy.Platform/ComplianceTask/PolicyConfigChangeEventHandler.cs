using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.ComplianceTask
{
	public class PolicyConfigChangeEventHandler
	{
		public PolicyConfigChangeEventHandler(DarServiceProvider provider)
		{
			this.darServiceProvider = provider;
		}

		public void EventHandler(object sender, PolicyConfigChangeEventArgs e)
		{
			string correlationId = (e.PolicyConfig != null && e.PolicyConfig.Version != null) ? e.PolicyConfig.Version.InternalStorage.ToString() : null;
			if (e.ChangeType != ChangeType.Delete && e.ChangeType != ChangeType.Update)
			{
				this.darServiceProvider.ExecutionLog.LogError("PolicyConfigChangeEventHandler", null, correlationId, null, string.Format("e.ChangeType not supported: {0}", e.ChangeType), new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler0.ToString())
				});
				throw new ArgumentException(string.Format("e.ChangeType not supported: {0}", e.ChangeType.ToString()));
			}
			try
			{
				bool flag = false;
				PolicyConfigProvider policyConfigProvider = sender as PolicyConfigProvider;
				if (policyConfigProvider != null)
				{
					BindingTask bindingTask = this.darServiceProvider.DarTaskFactory.CreateTask("Common.BindingApplication") as BindingTask;
					bindingTask.TenantId = policyConfigProvider.GetLocalOrganizationId();
					bindingTask.TaskSynchronizationOption = TaskSynchronizationOption.Serialize;
					bindingTask.WorkloadData = bindingTask.GetWorkloadDataFromWorkload();
					PolicyBindingSetConfig policyBindingSetConfig = e.PolicyConfig as PolicyBindingSetConfig;
					if (policyBindingSetConfig != null)
					{
						bindingTask.SetTriggerObject(policyBindingSetConfig, new Guid?(policyBindingSetConfig.PolicyDefinitionConfigId), ConfigurationObjectType.Binding, Mode.Enforce);
						flag = this.HandleBindingSet(policyBindingSetConfig, e, bindingTask, policyConfigProvider);
						bindingTask.TaskSynchronizationKey = policyBindingSetConfig.PolicyDefinitionConfigId.ToString();
					}
					else
					{
						PolicyBindingConfig policyBindingConfig = e.PolicyConfig as PolicyBindingConfig;
						if (policyBindingConfig != null)
						{
							bindingTask.SetTriggerObject(policyBindingConfig, new Guid?(policyBindingConfig.PolicyDefinitionConfigId), ConfigurationObjectType.Scope, policyBindingConfig.Mode);
							flag = this.HandleBinding(policyBindingConfig, e, bindingTask, policyConfigProvider);
							bindingTask.TaskSynchronizationKey = policyBindingConfig.PolicyDefinitionConfigId.ToString();
						}
						else
						{
							PolicyDefinitionConfig policyDefinitionConfig = e.PolicyConfig as PolicyDefinitionConfig;
							if (policyDefinitionConfig != null)
							{
								bindingTask.SetTriggerObject(policyDefinitionConfig, new Guid?(policyDefinitionConfig.Identity), ConfigurationObjectType.Policy, policyDefinitionConfig.Mode);
								flag = this.HandleDefinition(policyDefinitionConfig, e, bindingTask, policyConfigProvider);
								bindingTask.TaskSynchronizationKey = policyDefinitionConfig.Identity.ToString();
							}
							else
							{
								PolicyRuleConfig policyRuleConfig = e.PolicyConfig as PolicyRuleConfig;
								if (policyRuleConfig != null)
								{
									bindingTask.SetTriggerObject(policyRuleConfig, new Guid?(policyRuleConfig.PolicyDefinitionConfigId), ConfigurationObjectType.Rule, policyRuleConfig.Mode);
									flag = this.HandleRule(policyRuleConfig, e, bindingTask, policyConfigProvider);
									bindingTask.TaskSynchronizationKey = policyRuleConfig.PolicyDefinitionConfigId.ToString();
								}
							}
						}
					}
					if (flag)
					{
						this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, correlationId, "Queueing Binding Task", new KeyValuePair<string, object>[]
						{
							new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler1.ToString())
						});
					}
					else
					{
						this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, correlationId, "Event ignored, produced no changes", new KeyValuePair<string, object>[]
						{
							new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler2.ToString())
						});
					}
					if (bindingTask.GetCompletedBindings().Count<UnifiedPolicyStatus>() > 0)
					{
						foreach (UnifiedPolicyStatus unifiedPolicyStatus in bindingTask.GetCompletedBindings())
						{
							this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, correlationId, unifiedPolicyStatus.ToString(), new KeyValuePair<string, object>[]
							{
								new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler17.ToString())
							});
						}
						DarTaskManager darTaskManager = new DarTaskManager(this.darServiceProvider);
						darTaskManager.Enqueue(bindingTask);
						e.Handled = true;
					}
					else
					{
						this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, correlationId, "Not queueing anything from change", new KeyValuePair<string, object>[]
						{
							new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler3.ToString())
						});
						e.Handled = false;
					}
				}
			}
			catch (Exception exception)
			{
				this.darServiceProvider.ExecutionLog.LogError("PolicyConfigChangeEventHandler", null, correlationId, exception, "Unhandled exception in PolicyConfigChangeEventHandler.", new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler14.ToString())
				});
				throw;
			}
		}

		private static IEnumerable<string> GetBindingScopesFromBindingSet(PolicyBindingSetConfig bindingSet)
		{
			if (bindingSet.AppliedScopes != null)
			{
				return from binding in bindingSet.AppliedScopes
				select binding.Scope.ImmutableIdentity;
			}
			return null;
		}

		private bool HandleBindingSet(PolicyBindingSetConfig bindingSet, PolicyConfigChangeEventArgs e, BindingTask bindingTask, PolicyConfigProvider provider)
		{
			bool result = false;
			PolicyDefinitionConfig policyDefinitionConfig = null;
			if (bindingTask.ChangeAffectsWorkload(bindingSet, e.ChangeType, provider, out policyDefinitionConfig))
			{
				if (bindingSet.AppliedScopes != null)
				{
					List<PolicyBindingConfig> list = new List<PolicyBindingConfig>();
					List<PolicyBindingConfig> list2 = new List<PolicyBindingConfig>();
					foreach (PolicyBindingConfig policyBindingConfig in bindingSet.AppliedScopes)
					{
						if ((policyBindingConfig.ObjectState == ChangeType.Add || policyBindingConfig.ObjectState == ChangeType.Update) && policyBindingConfig.Mode == Mode.Enforce)
						{
							list.Add(policyBindingConfig);
							bindingTask.AddCompletedBinding(policyBindingConfig, new Guid?(bindingSet.PolicyDefinitionConfigId));
						}
						else if (policyBindingConfig.Mode != Mode.Enforce || policyBindingConfig.ObjectState == ChangeType.Delete)
						{
							list2.Add(policyBindingConfig);
							bindingTask.AddCompletedBinding(policyBindingConfig, new Guid?(bindingSet.PolicyDefinitionConfigId));
						}
					}
					this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, bindingTask.CorrelationId, string.Format("{0} scopes added, {1} scopes removed", list.Count.ToString(), list2.Count.ToString()), new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler15.ToString())
					});
					result = this.UpdateBindingTask(bindingTask, provider, list, list2, bindingSet.PolicyDefinitionConfigId, Guid.Empty);
				}
			}
			else
			{
				this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, bindingTask.CorrelationId, "Binding set change ignored", new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler4.ToString())
				});
			}
			if (policyDefinitionConfig != null)
			{
				this.EnableTaskRecurrence(provider.GetLocalOrganizationId(), policyDefinitionConfig.Scenario, bindingSet);
			}
			return result;
		}

		private bool HandleBinding(PolicyBindingConfig binding, PolicyConfigChangeEventArgs e, BindingTask bindingTask, PolicyConfigProvider provider)
		{
			bool result = false;
			if (bindingTask.ChangeAffectsWorkload(binding, e.ChangeType, provider))
			{
				List<PolicyBindingConfig> list = new List<PolicyBindingConfig>();
				List<PolicyBindingConfig> list2 = new List<PolicyBindingConfig>();
				if (e.ChangeType == ChangeType.Delete)
				{
					list2.Add(binding);
				}
				else if (e.ChangeType == ChangeType.Update)
				{
					list.Add(binding);
				}
				Guid ruleId;
				if (binding.PolicyRuleConfigId != null)
				{
					ruleId = binding.PolicyRuleConfigId.Value;
				}
				else
				{
					ruleId = Guid.Empty;
				}
				result = this.UpdateBindingTask(bindingTask, provider, list, list2, binding.PolicyDefinitionConfigId, ruleId);
			}
			else
			{
				this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, bindingTask.CorrelationId, "Binding change ignored", new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler5.ToString())
				});
			}
			return result;
		}

		private bool HandleDefinition(PolicyDefinitionConfig definition, PolicyConfigChangeEventArgs e, BindingTask bindingTask, PolicyConfigProvider provider)
		{
			bool result = false;
			if (bindingTask.ChangeAffectsWorkload(definition, e.ChangeType, provider))
			{
				IEnumerable<PolicyConfigBase> enumerable = provider.FindByPolicyDefinitionConfigId<PolicyBindingSetConfig>(definition.Identity);
				IEnumerable<PolicyConfigBase> enumerable2 = provider.FindByPolicyDefinitionConfigId<PolicyRuleConfig>(definition.Identity);
				if (enumerable != null && enumerable2 != null)
				{
					result = this.UpdateBindingTask(bindingTask, definition, enumerable2, enumerable, e.ChangeType, false);
				}
				else
				{
					this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, bindingTask.CorrelationId, "Policy Definition change ignored, no rules or bindings", new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler6.ToString())
					});
				}
			}
			else
			{
				this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, bindingTask.CorrelationId, "Policy Definition change ignored", new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler7.ToString())
				});
			}
			return result;
		}

		private bool HandleRule(PolicyRuleConfig rule, PolicyConfigChangeEventArgs e, BindingTask bindingTask, PolicyConfigProvider provider)
		{
			bool result = false;
			if (bindingTask.ChangeAffectsWorkload(rule, e.ChangeType, provider))
			{
				PolicyDefinitionConfig policyDefinitionConfig = provider.FindByIdentity<PolicyDefinitionConfig>(rule.PolicyDefinitionConfigId);
				if (policyDefinitionConfig != null)
				{
					List<PolicyRuleConfig> list = new List<PolicyRuleConfig>();
					list.Add(rule);
					IEnumerable<PolicyConfigBase> enumerable = provider.FindByPolicyDefinitionConfigId<PolicyBindingSetConfig>(policyDefinitionConfig.Identity);
					if (enumerable != null)
					{
						bool otherRulesExist = false;
						if (rule.Mode == Mode.PendingDeletion)
						{
							IEnumerable<PolicyRuleConfig> enumerable2 = provider.FindByPolicyDefinitionConfigId<PolicyRuleConfig>(policyDefinitionConfig.Identity);
							if (enumerable2 != null)
							{
								foreach (PolicyRuleConfig policyRuleConfig in enumerable2)
								{
									if (policyRuleConfig != null && policyRuleConfig.Identity != rule.Identity && policyRuleConfig.Enabled && policyRuleConfig.Mode == Mode.Enforce)
									{
										otherRulesExist = true;
										break;
									}
								}
							}
						}
						result = this.UpdateBindingTask(bindingTask, policyDefinitionConfig, list, enumerable, e.ChangeType, otherRulesExist);
					}
					else
					{
						this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, bindingTask.CorrelationId, "Policy rule change ignored, no binding sets", new KeyValuePair<string, object>[]
						{
							new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler8.ToString())
						});
					}
				}
				else
				{
					this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, bindingTask.CorrelationId, "Policy rule change ignored, no rule definition", new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler9.ToString())
					});
				}
			}
			else
			{
				this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, bindingTask.CorrelationId, "Policy rule change ignored", new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler10.ToString())
				});
			}
			return result;
		}

		private bool UpdateBindingTask(BindingTask task, PolicyDefinitionConfig definition, IEnumerable<PolicyConfigBase> rules, IEnumerable<PolicyConfigBase> bindingSets, ChangeType changeType, bool otherRulesExist = false)
		{
			bool flag = false;
			foreach (PolicyConfigBase policyConfigBase in rules)
			{
				PolicyRuleConfig policyRuleConfig = (PolicyRuleConfig)policyConfigBase;
				foreach (PolicyConfigBase policyConfigBase2 in bindingSets)
				{
					PolicyBindingSetConfig policyBindingSetConfig = (PolicyBindingSetConfig)policyConfigBase2;
					IEnumerable<PolicyBindingConfig> enumerable = Enumerable.Empty<PolicyBindingConfig>();
					IEnumerable<PolicyBindingConfig> enumerable2 = Enumerable.Empty<PolicyBindingConfig>();
					if (changeType == ChangeType.Update && definition.Enabled && definition.Mode == Mode.Enforce && policyRuleConfig.Enabled && policyRuleConfig.Mode == Mode.Enforce)
					{
						List<PolicyBindingConfig> list = new List<PolicyBindingConfig>();
						foreach (PolicyBindingConfig policyBindingConfig in policyBindingSetConfig.AppliedScopes)
						{
							if (policyBindingConfig.Mode == Mode.Enforce)
							{
								list.Add(policyBindingConfig);
							}
						}
						enumerable = list;
					}
					else if ((changeType == ChangeType.Delete || !definition.Enabled || definition.Mode != Mode.Enforce || !policyRuleConfig.Enabled || policyRuleConfig.Mode != Mode.Enforce) && (!otherRulesExist || policyRuleConfig.Mode != Mode.PendingDeletion))
					{
						enumerable2 = policyBindingSetConfig.AppliedScopes;
					}
					this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, task.CorrelationId, string.Format("{0} scopes added, {1} scopes removed", enumerable.Count<PolicyBindingConfig>().ToString(), enumerable2.Count<PolicyBindingConfig>().ToString()), new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler16.ToString())
					});
					flag = (this.UpdateBindingTask(task, definition, policyRuleConfig, enumerable, enumerable2) || flag);
				}
			}
			return flag;
		}

		private bool UpdateBindingTask(BindingTask task, PolicyConfigProvider provider, IEnumerable<PolicyBindingConfig> scopesToAdd, IEnumerable<PolicyBindingConfig> scopesToRemove, Guid definitionId, Guid ruleId)
		{
			bool flag = false;
			PolicyDefinitionConfig policyDefinitionConfig = provider.FindByIdentity<PolicyDefinitionConfig>(definitionId);
			if (policyDefinitionConfig != null)
			{
				if (ruleId == Guid.Empty)
				{
					IEnumerable<PolicyConfigBase> enumerable = provider.FindByPolicyDefinitionConfigId<PolicyRuleConfig>(definitionId);
					if (enumerable != null)
					{
						using (IEnumerator<PolicyConfigBase> enumerator = enumerable.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								PolicyConfigBase policyConfigBase = enumerator.Current;
								PolicyRuleConfig rule = (PolicyRuleConfig)policyConfigBase;
								flag = (this.UpdateBindingTask(task, policyDefinitionConfig, rule, scopesToAdd, scopesToRemove) || flag);
							}
							return flag;
						}
					}
					this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, task.CorrelationId, "Change ignored, no rules", new KeyValuePair<string, object>[]
					{
						new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler11.ToString())
					});
				}
				else
				{
					PolicyRuleConfig policyRuleConfig = provider.FindByIdentity<PolicyRuleConfig>(ruleId);
					if (policyRuleConfig != null)
					{
						flag = this.UpdateBindingTask(task, policyDefinitionConfig, policyRuleConfig, scopesToAdd, scopesToRemove);
					}
					else
					{
						this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, task.CorrelationId, "Change ignored, no rules", new KeyValuePair<string, object>[]
						{
							new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler12.ToString())
						});
					}
				}
			}
			else
			{
				this.darServiceProvider.ExecutionLog.LogInformation("PolicyConfigChangeEventHandler", null, task.CorrelationId, "Change ignored, no policy definition", new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.PolicyConfigChangeEventHandler13.ToString())
				});
			}
			return flag;
		}

		private bool UpdateBindingTask(BindingTask task, PolicyDefinitionConfig definition, PolicyRuleConfig rule, IEnumerable<PolicyBindingConfig> scopesToAdd, IEnumerable<PolicyBindingConfig> scopesToRemove)
		{
			bool result = false;
			if (definition.Enabled && definition.Mode == Mode.Enforce && rule.Enabled && rule.Mode == Mode.Enforce)
			{
				foreach (PolicyBindingConfig binding in scopesToAdd)
				{
					task.InsertBindingToAdd(definition, rule, binding);
					result = true;
				}
			}
			foreach (PolicyBindingConfig binding2 in scopesToRemove)
			{
				task.InsertBindingToRemove(definition, rule, binding2);
				result = true;
			}
			return result;
		}

		private void EnableTaskRecurrence(string tenantId, PolicyScenario scenario, PolicyBindingSetConfig bindingSet)
		{
			if (this.ShouldEnableTaskRecurrence(scenario, bindingSet))
			{
				if (this.EnableTaskRecurrence(tenantId, "Common.TaskGenerator"))
				{
					this.EnqueueTaskGeneratorTask(tenantId);
				}
				this.EnableTaskRecurrence(tenantId, this.GetTaskTypeForScenario(scenario));
				return;
			}
			this.darServiceProvider.ExecutionLog.LogOneEntry(ExecutionLog.EventType.Information, DarExecutionLogClientIDs.PolicyConfigChangeEventHandler21.ToString(), null, "Not enabling Recurrence as it is not required", new object[0]);
		}

		private bool EnableTaskRecurrence(string tenantId, string taskType)
		{
			DarTaskAggregate darTaskAggregate = this.darServiceProvider.DarTaskAggregateProvider.Find(tenantId, taskType);
			if (darTaskAggregate == null)
			{
				this.darServiceProvider.ExecutionLog.LogOneEntry(ExecutionLog.EventType.Error, DarExecutionLogClientIDs.PolicyConfigChangeEventHandler18.ToString(), null, "Error when enabling TaskRecurrence: Unable to find Task Aggregate for TaskType: {0}", new object[]
				{
					taskType
				});
				return false;
			}
			if (darTaskAggregate.RecurrenceType == RecurrenceType.None)
			{
				this.darServiceProvider.ExecutionLog.LogOneEntry(ExecutionLog.EventType.Information, DarExecutionLogClientIDs.PolicyConfigChangeEventHandler19.ToString(), null, "Enabling Recurrence for TaskType : {0}", new object[]
				{
					taskType
				});
				darTaskAggregate.RecurrenceType = RecurrenceType.Recurrent;
				this.darServiceProvider.DarTaskAggregateProvider.Save(darTaskAggregate);
				return true;
			}
			this.darServiceProvider.ExecutionLog.LogOneEntry(ExecutionLog.EventType.Information, DarExecutionLogClientIDs.PolicyConfigChangeEventHandler20.ToString(), null, "Recurrence for TaskType : {0} is already enabled", new object[]
			{
				taskType
			});
			return false;
		}

		private void EnqueueTaskGeneratorTask(string tenantId)
		{
			DarTask darTask = this.darServiceProvider.DarTaskFactory.CreateTask("Common.TaskGenerator");
			darTask.MinTaskScheduleTime = DateTime.UtcNow;
			darTask.TenantId = tenantId;
			darTask.TaskSynchronizationOption = TaskSynchronizationOption.Serialize;
			darTask.TaskSynchronizationKey = "Common.TaskGenerator";
			this.darServiceProvider.DarTaskQueue.Enqueue(darTask);
			this.darServiceProvider.ExecutionLog.LogOneEntry(ExecutionLog.EventType.Information, DarExecutionLogClientIDs.PolicyConfigChangeEventHandler22.ToString(), null, "Enqueued TaskGenerator Task", new object[0]);
		}

		private bool ShouldEnableTaskRecurrence(PolicyScenario scenario, PolicyBindingSetConfig bindingSet)
		{
			return scenario == PolicyScenario.Retention || scenario != PolicyScenario.Hold;
		}

		private string GetTaskTypeForScenario(PolicyScenario scenario)
		{
			switch (scenario)
			{
			case PolicyScenario.Retention:
				return "Common.Retention";
			case PolicyScenario.Dlp:
				return "Common.DLPApplication";
			}
			return string.Empty;
		}

		private const string LoggingClientId = "PolicyConfigChangeEventHandler";

		private DarServiceProvider darServiceProvider;
	}
}
