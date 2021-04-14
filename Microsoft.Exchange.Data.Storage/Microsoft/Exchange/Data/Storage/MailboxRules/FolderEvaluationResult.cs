using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderEvaluationResult
	{
		public FolderEvaluationResult(IRuleEvaluationContext context, List<Rule> rules)
		{
			this.context = context;
			this.targetFolder = context.CurrentFolder;
			this.hasOofRules = RuleLoader.HasOofRule(rules);
			this.ruleSet = new FolderEvaluationResult.RuleSet(this, rules);
			this.workItems = new List<WorkItem>();
		}

		public RuleAction.Bounce.BounceCode? BounceCode
		{
			get
			{
				return this.bounceCode;
			}
			set
			{
				this.bounceCode = value;
			}
		}

		public bool ExitExecution
		{
			get
			{
				return this.exitExecution;
			}
			set
			{
				this.exitExecution = value;
			}
		}

		public bool HasDeferredMoveOrCopy
		{
			get
			{
				foreach (WorkItem workItem in this.workItems)
				{
					DeferredActionWorkItem deferredActionWorkItem = workItem as DeferredActionWorkItem;
					if (deferredActionWorkItem != null && deferredActionWorkItem.HasMoveOrCopy)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsMessageDelegated
		{
			get
			{
				return (this.status & FolderEvaluationResult.FolderEvaluationStatus.IsMessageDelegated) != (FolderEvaluationResult.FolderEvaluationStatus)0;
			}
		}

		public bool IsMessageMoved
		{
			get
			{
				return (this.status & FolderEvaluationResult.FolderEvaluationStatus.IsMessageMoved) != (FolderEvaluationResult.FolderEvaluationStatus)0;
			}
			set
			{
				if (value)
				{
					this.status |= FolderEvaluationResult.FolderEvaluationStatus.IsMessageMoved;
					return;
				}
				this.status &= ~FolderEvaluationResult.FolderEvaluationStatus.IsMessageMoved;
			}
		}

		public bool HasOofRules
		{
			get
			{
				return this.hasOofRules;
			}
		}

		public IEnumerable<Rule> Rules
		{
			get
			{
				return this.ruleSet;
			}
		}

		public bool ShouldContinue
		{
			get
			{
				return this.BounceCode == null && (!this.ExitExecution || this.HasOofRules);
			}
		}

		public Folder TargetFolder
		{
			get
			{
				return this.targetFolder;
			}
			set
			{
				this.targetFolder = value;
			}
		}

		internal IList<WorkItem> WorkItems
		{
			get
			{
				return new ReadOnlyCollection<WorkItem>(this.workItems);
			}
		}

		public void AddWorkItem(WorkItem workItem)
		{
			this.workItems.Add(workItem);
			if (workItem is DelegateWorkItem)
			{
				this.status |= FolderEvaluationResult.FolderEvaluationStatus.IsMessageDelegated;
			}
		}

		public void ClearWorkItems()
		{
			this.workItems.Clear();
		}

		public void Execute(ExecutionStage stage)
		{
			foreach (WorkItem workItem in this.workItems)
			{
				if ((stage & workItem.Stage) != (ExecutionStage)0)
				{
					string stageDescription = FolderEvaluationResult.GetStageDescription(stage);
					try
					{
						this.context.CurrentRule = workItem.Rule;
						RuleUtil.FaultInjection((FaultInjectionLid)4175834429U);
						workItem.Execute();
						this.context.LogWorkItemExecution(workItem);
					}
					catch (InvalidRuleException exception)
					{
						this.context.RecordError(exception, stageDescription);
						this.context.DisableAndMarkRuleInError(workItem.Rule, workItem.Rule.Actions[workItem.ActionIndex].ActionType, workItem.ActionIndex, DeferredError.RuleError.Execution);
					}
					catch (StoragePermanentException exception2)
					{
						this.context.RecordError(exception2, stageDescription);
					}
					catch (ExchangeDataException exception3)
					{
						this.context.RecordError(exception3, stageDescription);
					}
					catch (MapiPermanentException exception4)
					{
						this.context.RecordError(exception4, stageDescription);
					}
					catch (DataValidationException exception5)
					{
						this.context.RecordError(exception5, stageDescription);
					}
					finally
					{
						this.context.CurrentRule = null;
					}
				}
			}
		}

		public DeferredActionWorkItem GetDeferredActionWorkItem(string provider)
		{
			foreach (WorkItem workItem in this.workItems)
			{
				DeferredActionWorkItem deferredActionWorkItem = workItem as DeferredActionWorkItem;
				if (deferredActionWorkItem != null && string.Equals(deferredActionWorkItem.Provider, provider, StringComparison.OrdinalIgnoreCase))
				{
					return deferredActionWorkItem;
				}
			}
			return null;
		}

		public bool HasCopyWorkItemTo(StoreId folderId)
		{
			foreach (WorkItem workItem in this.workItems)
			{
				CopyWorkItem copyWorkItem = workItem as CopyWorkItem;
				if (copyWorkItem != null && copyWorkItem.TargetFolderId.Equals(folderId))
				{
					return true;
				}
			}
			return false;
		}

		private static string GetStageDescription(ExecutionStage stage)
		{
			string result = null;
			if (stage == ExecutionStage.OnPromotedMessage)
			{
				result = ServerStrings.FolderRuleStageOnPromotedMessage;
			}
			else if (stage == ExecutionStage.OnCreatedMessage)
			{
				result = ServerStrings.FolderRuleStageOnCreatedMessage;
			}
			else if (stage == ExecutionStage.OnDeliveredMessage)
			{
				result = ServerStrings.FolderRuleStageOnDeliveredMessage;
			}
			else if (stage == ExecutionStage.OnPublicFolderAfter)
			{
				result = ServerStrings.FolderRuleStageOnPublicFolderAfter;
			}
			else if (stage == ExecutionStage.OnPublicFolderBefore)
			{
				result = ServerStrings.FolderRuleStageOnPublicFolderBefore;
			}
			return result;
		}

		private RuleAction.Bounce.BounceCode? bounceCode;

		private IRuleEvaluationContext context;

		private bool exitExecution;

		private bool hasOofRules;

		private FolderEvaluationResult.RuleSet ruleSet;

		private FolderEvaluationResult.FolderEvaluationStatus status;

		private Folder targetFolder;

		private List<WorkItem> workItems;

		[Flags]
		private enum FolderEvaluationStatus
		{
			IsMessageDelegated = 1,
			IsMessageMoved = 2
		}

		private class RuleSet : IEnumerable<Rule>, IEnumerable
		{
			public RuleSet(FolderEvaluationResult evaluationResult, List<Rule> rules)
			{
				this.evaluationResult = evaluationResult;
				this.rules = rules;
			}

			public IEnumerator<Rule> GetEnumerator()
			{
				foreach (Rule rule in this.rules)
				{
					if (this.evaluationResult.ExitExecution)
					{
						if (RuleLoader.IsOofRule(rule))
						{
							yield return rule;
						}
					}
					else
					{
						yield return rule;
					}
				}
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private FolderEvaluationResult evaluationResult;

			private List<Rule> rules;
		}
	}
}
