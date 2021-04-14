using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Office.CompliancePolicy.Monitor;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal sealed class SyncJob : JobBase
	{
		public SyncJob(IEnumerable<WorkItemBase> workItems, Action<JobBase> onJobCompleted, SyncAgentContext syncAgentContext) : base(workItems, onJobCompleted, syncAgentContext)
		{
			if (base.WorkItems.Count<WorkItemBase>() != 1)
			{
				throw new ArgumentException("For sync job, only one work item per job is allowed. The input work item count is instead " + base.WorkItems.Count<WorkItemBase>());
			}
			this.nextSyncCycleWork = new Dictionary<ConfigurationObjectType, List<SyncChangeInfo>>();
			this.CurrentWorkItem = (SyncWorkItem)base.WorkItems.First<WorkItemBase>();
			this.Errors = new List<SyncAgentExceptionBase>();
			this.DeletedObjectList = new HashSet<Guid>();
			this.MonitorEventTracker = new SyncMonitorEventTracker(this);
		}

		internal SyncJob(IEnumerable<WorkItemBase> workItems, Action<JobBase> onJobCompleted, SyncAgentContext syncAgentContext, bool noCleanUp = false) : this(workItems, onJobCompleted, syncAgentContext)
		{
			this.noCleanUp = noCleanUp;
		}

		internal SyncWorkItem CurrentWorkItem { get; set; }

		internal List<SyncAgentExceptionBase> Errors { get; set; }

		internal HashSet<Guid> DeletedObjectList { get; set; }

		internal IPolicySyncWebserviceClient SyncSvcClient { get; set; }

		internal ITenantInfoProvider TenantInfoProvider { get; set; }

		internal PolicyConfigProvider PolicyConfigProvider { get; set; }

		internal TenantInfo TenantInfo { get; set; }

		internal List<Guid> LocalPolicyIdList { get; set; }

		internal bool IsLastTry
		{
			get
			{
				return !base.SyncAgentContext.SyncAgentConfig.RetryStrategy.CanRetry(this.CurrentWorkItem.TryCount);
			}
		}

		internal SyncMonitorEventTracker MonitorEventTracker { get; set; }

		private bool TenantPermanentErrorOccurs
		{
			get
			{
				if (this.Errors.Any<SyncAgentExceptionBase>())
				{
					SyncAgentExceptionBase syncAgentExceptionBase = this.Errors.Last<SyncAgentExceptionBase>();
					return !syncAgentExceptionBase.IsPerObjectException && (syncAgentExceptionBase is SyncAgentPermanentException || this.IsLastTry);
				}
				return false;
			}
		}

		private bool AreAllErrorsObjectPermanentErrors
		{
			get
			{
				if (this.Errors.Any<SyncAgentExceptionBase>())
				{
					foreach (SyncAgentExceptionBase syncAgentExceptionBase in this.Errors)
					{
						if (!syncAgentExceptionBase.IsPerObjectException || syncAgentExceptionBase is SyncAgentTransientException)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		public override void Begin(object state)
		{
			base.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", this.CurrentWorkItem.TenantContext.TenantId.ToString(), this.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync Job Begin", "Unified Policy Sync Job Begin. " + Utility.GetThreadPoolStatus(), null, new KeyValuePair<string, object>[0]);
			this.MonitorEventTracker.MarkNotificationPickedUp();
			this.subWorkItemQueue = this.BuildSubWorkitemQueue(this.CurrentWorkItem);
			if (base.SyncAgentContext.SyncAgentConfig.AsyncCallSyncSvc)
			{
				SubWorkItemBase subWorkItem = this.subWorkItemQueue.Dequeue();
				if (this.ExecuteSubWorkItemWrapper(delegate
				{
					subWorkItem.BeginExecute(new Action<SubWorkItemBase>(this.OnSubWorkItemEnd));
				}, subWorkItem, true))
				{
					this.OnWorkItemCompleted(this.CurrentWorkItem);
					return;
				}
			}
			else
			{
				bool flag = false;
				while (!flag)
				{
					SubWorkItemBase subWorkItem = this.subWorkItemQueue.Dequeue();
					flag = this.ExecuteSubWorkItemWrapper(delegate
					{
						subWorkItem.Execute();
					}, subWorkItem, false);
				}
				this.OnWorkItemCompleted(this.CurrentWorkItem);
			}
		}

		private Queue<SubWorkItemBase> BuildSubWorkitemQueue(SyncWorkItem workItem)
		{
			Queue<SubWorkItemBase> queue = new Queue<SubWorkItemBase>();
			queue.Enqueue(new InitializationSubWorkItem(this));
			foreach (ConfigurationObjectType key in new ConfigurationObjectType[]
			{
				ConfigurationObjectType.Policy,
				ConfigurationObjectType.Rule,
				ConfigurationObjectType.Binding,
				ConfigurationObjectType.Association
			})
			{
				if (workItem.WorkItemInfo.ContainsKey(key))
				{
					foreach (SyncChangeInfo syncChangeInfo in workItem.WorkItemInfo[key])
					{
						queue.Enqueue((syncChangeInfo.ObjectId != null) ? new ObjectSyncSubWorkItem(this, syncChangeInfo) : new TypeSyncSubWorkItem(this, syncChangeInfo));
					}
				}
			}
			return queue;
		}

		private void OnSubWorkItemEnd(SubWorkItemBase subWorkItem)
		{
			SyncJob.<>c__DisplayClass8 CS$<>8__locals1 = new SyncJob.<>c__DisplayClass8();
			CS$<>8__locals1.subWorkItem = subWorkItem;
			CS$<>8__locals1.<>4__this = this;
			if (this.ExecuteSubWorkItemWrapper(delegate
			{
				CS$<>8__locals1.subWorkItem.EndExecute();
			}, CS$<>8__locals1.subWorkItem, false))
			{
				this.OnWorkItemCompleted(this.CurrentWorkItem);
				return;
			}
			SubWorkItemBase nextSubWorkItem = this.subWorkItemQueue.Dequeue();
			if (this.ExecuteSubWorkItemWrapper(delegate
			{
				nextSubWorkItem.BeginExecute(new Action<SubWorkItemBase>(CS$<>8__locals1.<>4__this.OnSubWorkItemEnd));
			}, nextSubWorkItem, true))
			{
				this.OnWorkItemCompleted(this.CurrentWorkItem);
			}
		}

		private bool ExecuteSubWorkItemWrapper(Action executeDelegate, SubWorkItemBase subWorkItem, bool isAsyncBegin = false)
		{
			bool stopEntireWorkItem = false;
			bool currentSubWorkItemSuccess = false;
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						executeDelegate();
						currentSubWorkItemSuccess = true;
					}
					catch (SyncAgentTransientException ex2)
					{
						this.Errors.Add(ex2);
						if (!this.IsLastTry)
						{
							this.AddToNextSyncCycle(subWorkItem);
						}
						if (!ex2.IsPerObjectException)
						{
							stopEntireWorkItem = true;
						}
					}
					catch (SyncAgentPermanentException ex3)
					{
						this.Errors.Add(ex3);
						stopEntireWorkItem = !ex3.IsPerObjectException;
					}
				});
			}
			catch (GrayException ex)
			{
				SyncAgentPermanentException item = new SyncAgentPermanentException(ex.Message, ex, false, SyncAgentErrorCode.Generic);
				this.Errors.Add(item);
				stopEntireWorkItem = true;
			}
			if (!stopEntireWorkItem)
			{
				if ((!currentSubWorkItemSuccess && subWorkItem is InitializationSubWorkItem) || base.HostStateProvider.IsShuttingDown())
				{
					stopEntireWorkItem = true;
				}
				if (!stopEntireWorkItem)
				{
					if (!isAsyncBegin)
					{
						stopEntireWorkItem = !this.subWorkItemQueue.Any<SubWorkItemBase>();
					}
					else if (!currentSubWorkItemSuccess)
					{
						if (this.subWorkItemQueue.Any<SubWorkItemBase>())
						{
							SubWorkItemBase nextSubWorkItem = this.subWorkItemQueue.Dequeue();
							ThreadPool.QueueUserWorkItem(delegate(object state)
							{
								if (this.ExecuteSubWorkItemWrapper(delegate
								{
									nextSubWorkItem.BeginExecute(new Action<SubWorkItemBase>(this.OnSubWorkItemEnd));
								}, nextSubWorkItem, true))
								{
									this.OnWorkItemCompleted(this.CurrentWorkItem);
								}
							});
						}
						else
						{
							stopEntireWorkItem = true;
						}
					}
				}
			}
			return stopEntireWorkItem;
		}

		private void OnWorkItemCompleted(SyncWorkItem workItem)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					try
					{
						if (this.Errors.Any<SyncAgentExceptionBase>())
						{
							workItem.Status = WorkItemStatus.Fail;
							using (List<SyncAgentExceptionBase>.Enumerator enumerator = this.Errors.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									SyncAgentExceptionBase exception = enumerator.Current;
									this.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", this.CurrentWorkItem.TenantContext.TenantId.ToString(), this.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Error, "Unified Policy Sync Job Error", "Unified Policy Sync Job Error", exception, new KeyValuePair<string, object>[0]);
								}
								goto IL_D7;
							}
						}
						workItem.Status = (this.nextSyncCycleWork.Any<KeyValuePair<ConfigurationObjectType, List<SyncChangeInfo>>>() ? WorkItemStatus.Stopped : WorkItemStatus.Success);
						IL_D7:
						workItem.Errors = this.Errors;
						int tryCount = workItem.TryCount;
						if (this.TenantPermanentErrorOccurs || this.AreAllErrorsObjectPermanentErrors)
						{
							workItem.TryCount = -1;
						}
						else
						{
							while (this.subWorkItemQueue.Any<SubWorkItemBase>())
							{
								this.AddToNextSyncCycle(this.subWorkItemQueue.Dequeue());
							}
						}
						if (this.nextSyncCycleWork.Any<KeyValuePair<ConfigurationObjectType, List<SyncChangeInfo>>>())
						{
							workItem.WorkItemInfo = this.nextSyncCycleWork;
						}
						if (this.TenantPermanentErrorOccurs)
						{
							this.MonitorEventTracker.ReportTenantLevelFailure(this.Errors.Last<SyncAgentExceptionBase>());
						}
						if (this.TenantInfo != null)
						{
							DateTime utcNow = DateTime.UtcNow;
							switch (workItem.Status)
							{
							case WorkItemStatus.Success:
								this.TenantInfo.LastSuccessfulSyncUTC = new DateTime?(utcNow);
								this.TenantInfo.LastErrors = null;
								break;
							case WorkItemStatus.Fail:
								this.TenantInfo.LastErrorTimeUTC = new DateTime?(utcNow);
								this.TenantInfo.LastErrors = (from p in workItem.Errors
								select new SerializableException(p).ExceptionChain).ToArray<string>();
								break;
							}
							this.TenantInfo.LastAttemptedSyncUTC = new DateTime?(utcNow);
							this.MonitorEventTracker.TrackLatencyWrapper(LatencyType.TenantInfo, delegate()
							{
								this.TenantInfoProvider.Save(this.TenantInfo);
							});
						}
						if (!this.noCleanUp)
						{
							if (this.SyncSvcClient != null)
							{
								this.SyncSvcClient.Dispose();
								this.SyncSvcClient = null;
							}
							if (this.PolicyConfigProvider != null)
							{
								this.PolicyConfigProvider.Dispose();
								this.PolicyConfigProvider = null;
							}
							if (this.TenantInfoProvider != null)
							{
								this.TenantInfoProvider.Dispose();
								this.TenantInfoProvider = null;
							}
						}
						if (this.SyncAgentContext.SyncAgentConfig.EnableMonitor)
						{
							this.MonitorEventTracker.TriggerAlertIfNecessary();
						}
						this.LogProvider.LogOneEntry("UnifiedPolicySyncAgent", this.CurrentWorkItem.TenantContext.TenantId.ToString(), this.CurrentWorkItem.ExternalIdentity, ExecutionLog.EventType.Information, "Unified Policy Sync Job End", string.Format(string.Format("Unified Policy Sync Job End. Result is {0}. TryCount is {1}.", workItem.Status, tryCount), new object[0]), null, new KeyValuePair<string, object>[0]);
						this.MonitorEventTracker.TrackLatencyWrapper(LatencyType.PersistentQueue, delegate()
						{
							this.OnJobCompleted(this);
						});
						this.MonitorEventTracker.PublishPerfData();
					}
					catch (SyncAgentExceptionBase)
					{
					}
				});
			}
			catch (GrayException)
			{
			}
		}

		private void AddToNextSyncCycle(SubWorkItemBase subWorkItem)
		{
			if (!(subWorkItem is ObjectSyncSubWorkItem) && !(subWorkItem is TypeSyncSubWorkItem))
			{
				return;
			}
			ConfigurationObjectType objectType = subWorkItem.ChangeInfo.ObjectType;
			if (!this.nextSyncCycleWork.ContainsKey(objectType))
			{
				this.nextSyncCycleWork[objectType] = new List<SyncChangeInfo>();
			}
			this.nextSyncCycleWork[objectType].Add(subWorkItem.ChangeInfo);
		}

		private readonly bool noCleanUp;

		private Queue<SubWorkItemBase> subWorkItemQueue;

		private Dictionary<ConfigurationObjectType, List<SyncChangeInfo>> nextSyncCycleWork;
	}
}
