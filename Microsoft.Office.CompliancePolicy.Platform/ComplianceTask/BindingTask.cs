using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.ComplianceData;
using Microsoft.Office.CompliancePolicy.Dar;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.ComplianceTask
{
	public class BindingTask : ComplianceTask
	{
		public BindingTask()
		{
			this.Category = DarTaskCategory.Medium;
			base.TaskRetryTotalCount = 5;
			base.TaskRetryInterval = new TimeSpan(0, 0, 135);
		}

		public override string TaskType
		{
			get
			{
				return "Common.BindingApplication";
			}
		}

		public override string WorkloadData
		{
			get
			{
				return this.Bindings.WorkloadData;
			}
			set
			{
				this.Bindings.WorkloadData = value;
			}
		}

		public override string CorrelationId
		{
			get
			{
				if (this.Bindings.TriggerObjectStatus != null && this.Bindings.TriggerObjectStatus.Version != null)
				{
					return this.Bindings.TriggerObjectStatus.Version.InternalStorage.ToString();
				}
				return base.CorrelationId;
			}
		}

		[SerializableTaskData]
		public BindingTask.StoredBindings Bindings
		{
			get
			{
				if (this.storedBindings == null)
				{
					this.storedBindings = new BindingTask.StoredBindings();
				}
				return this.storedBindings;
			}
			set
			{
				this.storedBindings = value;
			}
		}

		public void InsertBindingToAdd(PolicyDefinitionConfig definition, PolicyRuleConfig rule, PolicyBindingConfig binding)
		{
			this.InsertBinding(this.Bindings.BindingsToAdd, definition, rule, binding);
		}

		public void InsertBindingToRemove(PolicyDefinitionConfig definition, PolicyRuleConfig rule, PolicyBindingConfig binding)
		{
			this.InsertBinding(this.Bindings.BindingsToRemove, definition, rule, binding);
		}

		public void SetTriggerObject(PolicyConfigBase configObject, Guid? policyId, ConfigurationObjectType type, Mode mode)
		{
			if (this.Bindings.TriggerObjectStatus == null)
			{
				UnifiedPolicyStatus policyStatusFromPolicyConfig = this.GetPolicyStatusFromPolicyConfig(configObject, policyId, type, mode);
				this.Bindings.TriggerObjectStatus = policyStatusFromPolicyConfig;
				return;
			}
			throw new CompliancePolicyException("Cannot set the trigger object after status has already created");
		}

		public void AddCompletedBinding(PolicyBindingConfig binding, Guid? policyId)
		{
			UnifiedPolicyStatus policyStatusFromPolicyConfig = this.GetPolicyStatusFromPolicyConfig(binding, policyId, ConfigurationObjectType.Scope, binding.Mode);
			this.Bindings.CompletedBindings[binding.Identity] = policyStatusFromPolicyConfig;
		}

		public IEnumerable<UnifiedPolicyStatus> GetCompletedBindings()
		{
			return this.Bindings.CompletedBindings.Values.Concat(new UnifiedPolicyStatus[]
			{
				this.Bindings.TriggerObjectStatus
			});
		}

		public override DarTaskExecutionResult Execute(DarTaskManager darTaskManager)
		{
			DarTaskExecutionResult darTaskExecutionResult = DarTaskExecutionResult.Yielded;
			int num = this.Bindings.BindingsToAdd.Count + this.Bindings.BindingsToRemove.Count;
			int num2 = num;
			DarTaskExecutionResult result;
			try
			{
				this.Log(darTaskManager, "Binding task executing", DarExecutionLogClientIDs.BindingTask0);
				if (this.Bindings.SkippedDueToRetryNeed >= num)
				{
					this.Bindings.SkippedDueToRetryNeed = 0;
				}
				int skippedDueToRetryNeed = this.Bindings.SkippedDueToRetryNeed;
				using (PolicyConfigProvider policyStore = this.ComplianceServiceProvider.GetPolicyStore(base.TenantId))
				{
					if (this.ForEachBindings(this.Bindings.BindingsToRemove, new Func<ComplianceItemContainer, BindingTask.StoredBinding, PolicyConfigProvider, DarTaskManager, bool>(this.RemoveBinding), darTaskManager, policyStore, ref skippedDueToRetryNeed))
					{
						this.ForEachBindings(this.Bindings.BindingsToAdd, new Func<ComplianceItemContainer, BindingTask.StoredBinding, PolicyConfigProvider, DarTaskManager, bool>(this.AddBinding), darTaskManager, policyStore, ref skippedDueToRetryNeed);
					}
				}
				num2 = this.Bindings.BindingsToAdd.Count + this.Bindings.BindingsToRemove.Count;
				if (num2 == 0)
				{
					darTaskExecutionResult = (result = (this.Bindings.CompletedBindings.Values.Any((UnifiedPolicyStatus status) => status.ErrorCode != UnifiedPolicyErrorCode.Success) ? DarTaskExecutionResult.Failed : DarTaskExecutionResult.Completed));
				}
				else if (skippedDueToRetryNeed > this.Bindings.SkippedDueToRetryNeed)
				{
					this.Bindings.SkippedDueToRetryNeed = skippedDueToRetryNeed;
					darTaskExecutionResult = (result = DarTaskExecutionResult.TransientError);
				}
				else
				{
					darTaskExecutionResult = (result = DarTaskExecutionResult.Yielded);
				}
			}
			finally
			{
				this.Log(darTaskManager, string.Format("Binding task exited with result {0}, Processed {1}/{2}, Need retry: {3}", new object[]
				{
					darTaskExecutionResult,
					num - num2,
					num,
					this.Bindings.SkippedDueToRetryNeed
				}), DarExecutionLogClientIDs.BindingTask1);
			}
			return result;
		}

		public override void CompleteTask(DarTaskManager darTaskManager)
		{
			try
			{
				this.PublishStatus();
			}
			catch (PolicyConfigProviderPermanentException ex)
			{
				this.Bindings.StatusPublishingError = "PolicyConfigProviderPermanentException occured: " + ex.Message;
				this.LogError(darTaskManager, ex, "Binding task failed during status publishing", DarExecutionLogClientIDs.BindingTask14);
				base.TaskState = DarTaskState.Failed;
			}
		}

		protected internal virtual bool ChangeAffectsWorkload(PolicyDefinitionConfig definition, ChangeType changeType, PolicyConfigProvider provider)
		{
			return definition.Scenario == PolicyScenario.Hold && (definition.IsModified(PolicyDefinitionConfigSchema.Enabled) || definition.IsModified(PolicyDefinitionConfigSchema.Mode) || changeType == ChangeType.Delete);
		}

		protected internal virtual bool ChangeAffectsWorkload(PolicyRuleConfig rule, ChangeType changeType, PolicyConfigProvider provider)
		{
			if (rule.IsModified(PolicyRuleConfigSchema.Enabled) || rule.IsModified(PolicyRuleConfigSchema.RuleBlob) || rule.IsModified(PolicyRuleConfigSchema.Mode) || changeType == ChangeType.Delete)
			{
				PolicyDefinitionConfig policyDefinitionConfig = provider.FindByIdentity<PolicyDefinitionConfig>(rule.PolicyDefinitionConfigId);
				if (policyDefinitionConfig != null && policyDefinitionConfig.Scenario == PolicyScenario.Hold)
				{
					return true;
				}
			}
			return false;
		}

		protected internal virtual bool ChangeAffectsWorkload(PolicyBindingConfig binding, ChangeType changeType, PolicyConfigProvider provider)
		{
			return false;
		}

		protected internal virtual bool ChangeAffectsWorkload(PolicyBindingSetConfig binding, ChangeType changeType, PolicyConfigProvider provider, out PolicyDefinitionConfig definition)
		{
			definition = null;
			if (binding.IsModified(PolicyBindingSetConfigSchema.AppliedScopes) || changeType == ChangeType.Delete)
			{
				definition = provider.FindByIdentity<PolicyDefinitionConfig>(binding.PolicyDefinitionConfigId);
				if (definition != null && definition.Scenario == PolicyScenario.Hold)
				{
					return true;
				}
			}
			return false;
		}

		protected override IEnumerable<Type> GetKnownTypes()
		{
			return base.GetKnownTypes().Concat(new Type[]
			{
				typeof(BindingTask.StoredBindings),
				typeof(BindingTask.StoredBinding),
				typeof(List<BindingTask.StoredBinding>),
				typeof(Dictionary<Guid, UnifiedPolicyStatus>)
			});
		}

		private bool AddBinding(ComplianceItemContainer container, BindingTask.StoredBinding storedBinding, PolicyConfigProvider pcp, DarTaskManager darTaskManager)
		{
			bool result = false;
			string errorMessage = string.Empty;
			UnifiedPolicyErrorCode errorCode = UnifiedPolicyErrorCode.Success;
			if (container != null)
			{
				if (container.SupportsBinding)
				{
					PolicyDefinitionConfig policyDefinitionConfig = pcp.FindByIdentity<PolicyDefinitionConfig>(storedBinding.DefinitionId);
					if (policyDefinitionConfig != null)
					{
						PolicyRuleConfig policyRuleConfig = pcp.FindByIdentity<PolicyRuleConfig>(storedBinding.RuleId);
						if (policyRuleConfig != null)
						{
							if (storedBinding.RuleVersion == Guid.Empty || (policyRuleConfig.Version != null && policyRuleConfig.Version.CompareTo(PolicyVersion.Create(storedBinding.RuleVersion)) >= 0))
							{
								if (!(storedBinding.DefinitionVersion == Guid.Empty))
								{
									if (!(policyDefinitionConfig.Version != null) || policyDefinitionConfig.Version.CompareTo(PolicyVersion.Create(storedBinding.DefinitionVersion)) < 0)
									{
										goto IL_180;
									}
								}
								try
								{
									container.AddPolicy(policyDefinitionConfig, policyRuleConfig);
									this.Log(darTaskManager, string.Format("Policy application applied successfully.  Scope: {0}", storedBinding.Scope), DarExecutionLogClientIDs.BindingTask11);
									goto IL_24D;
								}
								catch (ComplianceTaskTransientException ex)
								{
									this.LogError(darTaskManager, ex, string.Format("Policy application failed with transient failure.  Scope: {0}", storedBinding.Scope), DarExecutionLogClientIDs.BindingTask2);
									result = true;
									errorMessage = ex.Message;
									errorCode = ex.ErrorCode;
									goto IL_24D;
								}
								catch (ComplianceTaskPermanentException ex2)
								{
									this.LogError(darTaskManager, ex2, string.Format("Policy application failed with permanent failure.  Scope: {0}", storedBinding.Scope), DarExecutionLogClientIDs.BindingTask3);
									errorMessage = ex2.Message;
									errorCode = ex2.ErrorCode;
									goto IL_24D;
								}
								catch (Exception ex3)
								{
									this.LogError(darTaskManager, ex3, string.Format("Policy application failed with unknown failure.  Scope: {0}", storedBinding.Scope), DarExecutionLogClientIDs.BindingTask13);
									result = true;
									errorMessage = ex3.Message;
									errorCode = UnifiedPolicyErrorCode.Unknown;
									goto IL_24D;
								}
							}
							IL_180:
							string text = "Policy store not sync to most recent changes";
							this.Log(darTaskManager, text, DarExecutionLogClientIDs.BindingTask4);
							result = true;
							errorMessage = text;
							errorCode = UnifiedPolicyErrorCode.InternalError;
						}
						else
						{
							string text2 = "Policy rule does not exist";
							this.Log(darTaskManager, string.Format("{0}: {1}", text2, storedBinding.RuleId), DarExecutionLogClientIDs.BindingTask5);
							result = true;
							errorMessage = text2;
							errorCode = UnifiedPolicyErrorCode.InternalError;
						}
					}
					else
					{
						string text3 = "Policy definition does not exist";
						this.Log(darTaskManager, string.Format("{0}: {1}", text3, storedBinding.DefinitionId.ToString()), DarExecutionLogClientIDs.BindingTask6);
						result = true;
						errorMessage = text3;
						errorCode = UnifiedPolicyErrorCode.InternalError;
					}
				}
				else
				{
					string text4 = "Container does not support binding so failing the application";
					this.Log(darTaskManager, text4, DarExecutionLogClientIDs.BindingTask7);
					errorMessage = text4;
					errorCode = UnifiedPolicyErrorCode.InternalError;
				}
			}
			else
			{
				this.Log(darTaskManager, string.Format("{0}: {1}", "Policy container does not exist", storedBinding.Scope), DarExecutionLogClientIDs.BindingTask8);
				result = true;
				errorMessage = "Policy container does not exist";
				errorCode = UnifiedPolicyErrorCode.FailedToOpenContainer;
			}
			IL_24D:
			this.HandleCompletedBinding(storedBinding, errorMessage, errorCode);
			return result;
		}

		private bool RemoveBinding(ComplianceItemContainer container, BindingTask.StoredBinding storedBinding, PolicyConfigProvider pcp, DarTaskManager darTaskManager)
		{
			bool result = false;
			string errorMessage = string.Empty;
			UnifiedPolicyErrorCode errorCode = UnifiedPolicyErrorCode.Success;
			if (container != null)
			{
				try
				{
					container.RemovePolicy(storedBinding.DefinitionId, storedBinding.Scenario);
					this.Log(darTaskManager, string.Format("Policy application removed successfully.  Scope: {0}", storedBinding.Scope), DarExecutionLogClientIDs.BindingTask12);
					goto IL_C7;
				}
				catch (PolicyConfigProviderTransientException ex)
				{
					result = true;
					errorMessage = ex.Message;
					errorCode = UnifiedPolicyErrorCode.Unknown;
					goto IL_C7;
				}
				catch (PolicyConfigProviderPermanentException ex2)
				{
					errorMessage = ex2.Message;
					errorCode = UnifiedPolicyErrorCode.Unknown;
					goto IL_C7;
				}
			}
			darTaskManager.ServiceProvider.ExecutionLog.LogInformation("BindingTask", null, this.CorrelationId, string.Format("{0}: {1}", "Policy container does not exist", storedBinding.Scope), new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("Tag", DarExecutionLogClientIDs.BindingTask9.ToString())
			});
			result = true;
			errorMessage = "Policy container does not exist";
			errorCode = UnifiedPolicyErrorCode.FailedToOpenContainer;
			IL_C7:
			this.HandleCompletedBinding(storedBinding, errorMessage, errorCode);
			return result;
		}

		private UnifiedPolicyStatus GetPolicyStatusFromPolicyConfig(PolicyConfigBase policyConfig, Guid? policyId, ConfigurationObjectType type, Mode mode)
		{
			return new UnifiedPolicyStatus
			{
				ErrorCode = UnifiedPolicyErrorCode.Success,
				ErrorMessage = string.Empty,
				ObjectId = policyConfig.Identity,
				ParentObjectId = policyId,
				ObjectType = type,
				Version = policyConfig.Version,
				WhenProcessedUTC = DateTime.UtcNow,
				Mode = mode
			};
		}

		private bool ForEachBindings(List<BindingTask.StoredBinding> bindingList, Func<ComplianceItemContainer, BindingTask.StoredBinding, PolicyConfigProvider, DarTaskManager, bool> processPolicyDelegate, DarTaskManager darTaskManager, PolicyConfigProvider pcp, ref int skippedDueToNeedToRetry)
		{
			while (skippedDueToNeedToRetry < bindingList.Count)
			{
				if (!base.ShouldContinue(darTaskManager))
				{
					return false;
				}
				BindingTask.StoredBinding storedBinding = bindingList[skippedDueToNeedToRetry];
				ComplianceItemContainer complianceItemContainer = this.ComplianceServiceProvider.GetComplianceItemContainer(base.TenantId, storedBinding.Scope);
				bool flag = processPolicyDelegate(complianceItemContainer, storedBinding, pcp, darTaskManager);
				if (flag)
				{
					skippedDueToNeedToRetry++;
				}
				else
				{
					bindingList.RemoveAt(skippedDueToNeedToRetry);
				}
			}
			return true;
		}

		private void HandleCompletedBinding(BindingTask.StoredBinding storedBinding, string errorMessage, UnifiedPolicyErrorCode errorCode)
		{
			UnifiedPolicyStatus policyStatusFromBinding = this.GetPolicyStatusFromBinding(storedBinding);
			if (!string.IsNullOrEmpty(errorMessage))
			{
				policyStatusFromBinding.ErrorMessage = errorMessage;
				policyStatusFromBinding.ErrorCode = errorCode;
			}
			if (this.Bindings.CompletedBindings.ContainsKey(policyStatusFromBinding.ObjectId))
			{
				policyStatusFromBinding.Mode = this.Bindings.CompletedBindings[policyStatusFromBinding.ObjectId].Mode;
				if (policyStatusFromBinding.Mode == Mode.PendingDeletion && policyStatusFromBinding.ErrorCode == UnifiedPolicyErrorCode.Success)
				{
					policyStatusFromBinding.Mode = Mode.Deleted;
				}
			}
			this.Bindings.CompletedBindings[policyStatusFromBinding.ObjectId] = policyStatusFromBinding;
		}

		private UnifiedPolicyStatus GetPolicyStatusFromBinding(BindingTask.StoredBinding binding)
		{
			return new UnifiedPolicyStatus
			{
				ObjectId = binding.BindingId,
				ObjectType = ConfigurationObjectType.Scope,
				ParentObjectId = new Guid?(binding.DefinitionId),
				Version = binding.BindingVersion,
				WhenProcessedUTC = DateTime.UtcNow,
				ErrorCode = UnifiedPolicyErrorCode.Success,
				ErrorMessage = string.Empty
			};
		}

		private void InsertBinding(List<BindingTask.StoredBinding> bindings, PolicyDefinitionConfig definition, PolicyRuleConfig rule, PolicyBindingConfig binding)
		{
			if (definition == null)
			{
				throw new ArgumentNullException("definition");
			}
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			DateTime minValue = DateTime.MinValue;
			DateTime minValue2 = DateTime.MinValue;
			Guid definitionVersion = Guid.Empty;
			Guid ruleVersion = Guid.Empty;
			Guid bindingVersion = Guid.Empty;
			if (definition.Version != null)
			{
				definitionVersion = definition.Version.InternalStorage;
			}
			if (rule.Version != null)
			{
				ruleVersion = rule.Version.InternalStorage;
			}
			if (binding.Version != null)
			{
				bindingVersion = binding.Version.InternalStorage;
			}
			bindings.Add(new BindingTask.StoredBinding(definition.Identity, definitionVersion, rule.Identity, ruleVersion, binding.Identity, bindingVersion, binding.Scope.ImmutableIdentity, definition.Scenario));
		}

		private void PublishStatus()
		{
			bool flag = this.Bindings.TriggerObjectStatus.Mode == Mode.PendingDeletion;
			bool flag2 = true;
			foreach (UnifiedPolicyStatus unifiedPolicyStatus in this.Bindings.CompletedBindings.Values)
			{
				if (unifiedPolicyStatus.ErrorCode != UnifiedPolicyErrorCode.Success)
				{
					flag2 = false;
				}
				else if (unifiedPolicyStatus.Mode == Mode.PendingDeletion)
				{
					unifiedPolicyStatus.Mode = Mode.Deleted;
				}
			}
			if (flag && flag2)
			{
				this.Bindings.TriggerObjectStatus.Mode = Mode.Deleted;
			}
			using (PolicyConfigProvider policyStore = this.ComplianceServiceProvider.GetPolicyStore(base.TenantId))
			{
				policyStore.PublishStatus(this.GetCompletedBindings());
			}
		}

		private void Log(DarTaskManager darTaskManager, string msg, DarExecutionLogClientIDs tag)
		{
			darTaskManager.ServiceProvider.ExecutionLog.LogInformation("BindingTask", null, this.CorrelationId, msg, new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("Tag", tag.ToString())
			});
		}

		private void LogError(DarTaskManager darTaskManager, Exception ex, string msg, DarExecutionLogClientIDs tag)
		{
			darTaskManager.ServiceProvider.ExecutionLog.LogError("BindingTask", null, this.CorrelationId, ex, msg, new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("Tag", tag.ToString())
			});
		}

		private const string PolicyContainerDoesNotExist = "Policy container does not exist";

		private const string LoggingClientId = "BindingTask";

		private const int DefaultRetryCount = 5;

		private const int DefaultRetryIntervalInSeconds = 135;

		private BindingTask.StoredBindings storedBindings;

		[DataContract]
		public class StoredBinding
		{
			public StoredBinding(Guid definitionId, Guid definitionVersion, Guid ruleId, Guid ruleVersion, Guid bindingId, Guid bindingVersion, string scope, PolicyScenario scenario)
			{
				this.DefinitionId = definitionId;
				this.DefinitionVersion = definitionVersion;
				this.RuleId = ruleId;
				this.RuleVersion = ruleVersion;
				this.BindingId = bindingId;
				this.BindingVersion = bindingVersion;
				this.Scope = scope;
				this.Scenario = scenario;
			}

			[DataMember]
			public Guid DefinitionId { get; set; }

			[DataMember]
			public Guid DefinitionVersion { get; set; }

			[DataMember]
			public Guid RuleId { get; set; }

			[DataMember]
			public Guid RuleVersion { get; set; }

			[DataMember]
			public Guid BindingId { get; set; }

			[DataMember]
			public Guid BindingVersion { get; set; }

			[DataMember]
			public string Scope { get; set; }

			[DataMember]
			public PolicyScenario Scenario { get; set; }
		}

		[DataContract]
		public class StoredBindings
		{
			public StoredBindings()
			{
				this.Init(default(StreamingContext));
			}

			[DataMember]
			public List<BindingTask.StoredBinding> BindingsToAdd
			{
				get
				{
					return this.bindingsToAdd;
				}
			}

			[DataMember]
			public List<BindingTask.StoredBinding> BindingsToRemove
			{
				get
				{
					return this.bindingsToRemove;
				}
			}

			[DataMember]
			public UnifiedPolicyStatus TriggerObjectStatus { get; set; }

			[DataMember]
			public Dictionary<Guid, UnifiedPolicyStatus> CompletedBindings
			{
				get
				{
					return this.completedBindings;
				}
			}

			[DataMember]
			public string WorkloadData { get; set; }

			[DataMember]
			public int SkippedDueToRetryNeed { get; set; }

			[DataMember]
			public string StatusPublishingError { get; set; }

			[OnDeserializing]
			private void Init(StreamingContext ctx)
			{
				this.bindingsToAdd = new List<BindingTask.StoredBinding>();
				this.bindingsToRemove = new List<BindingTask.StoredBinding>();
				this.completedBindings = new Dictionary<Guid, UnifiedPolicyStatus>();
			}

			private List<BindingTask.StoredBinding> bindingsToAdd;

			private List<BindingTask.StoredBinding> bindingsToRemove;

			private Dictionary<Guid, UnifiedPolicyStatus> completedBindings;
		}
	}
}
