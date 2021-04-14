using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PipelineDispatcher : IUMAsyncComponent
	{
		private PipelineDispatcher()
		{
			this.resources = new PipelineResource[3];
			this.resources[0] = PipelineResource.CreatePipelineResource(PipelineDispatcher.PipelineResourceType.LowPriorityCpuBound);
			this.resources[1] = PipelineResource.CreatePipelineResource(PipelineDispatcher.PipelineResourceType.CpuBound);
			this.resources[2] = PipelineResource.CreatePipelineResource(PipelineDispatcher.PipelineResourceType.NetworkBound);
		}

		internal event EventHandler<EventArgs> OnShutdown;

		public bool IsInitialized
		{
			get
			{
				return this.isInitialized;
			}
		}

		public AutoResetEvent StoppedEvent
		{
			get
			{
				return this.stageShutdownEvent;
			}
		}

		public string Name
		{
			get
			{
				return base.GetType().Name;
			}
		}

		internal static PipelineDispatcher Instance
		{
			get
			{
				return PipelineDispatcher.instance;
			}
		}

		internal bool IsShuttingDown
		{
			get
			{
				bool result;
				lock (this.lockObj)
				{
					result = this.isShuttingDown;
				}
				return result;
			}
		}

		public bool IsPipelineHealthy
		{
			get
			{
				bool isPipelineHealthy;
				lock (this.lockObj)
				{
					isPipelineHealthy = this.throttleManager.IsPipelineHealthy;
				}
				return isPipelineHealthy;
			}
		}

		public void CleanupAfterStopped()
		{
			lock (this.lockObj)
			{
				if (this.diskQueueSemaphore != null)
				{
					this.diskQueueSemaphore.Release();
				}
				this.CleanUp();
			}
		}

		public void StartNow(StartupStage stage)
		{
			if (stage == StartupStage.WPActivation)
			{
				lock (this.lockObj)
				{
					CallIdTracer.TracePfd(ExTraceGlobals.ServiceStartTracer, 0, "PFD UMS {0} - Initializing SMTP Submission Services.", new object[]
					{
						9786
					});
					CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "{0} starting in stage {1}", new object[]
					{
						this.Name,
						stage
					});
					if (this.isInitialized)
					{
						throw new InvalidOperationException();
					}
					this.isInitialized = true;
				}
				if (!ThreadPool.QueueUserWorkItem(new WaitCallback(this.AcquireDiskQueueAndStartProcessing)))
				{
					throw new PipelineInitializationException();
				}
			}
		}

		public void StopAsync()
		{
			lock (this.lockObj)
			{
				this.isShuttingDown = true;
			}
			if (this.OnShutdown != null)
			{
				this.OnShutdown(this, null);
			}
			if (this.numberOfActiveStages <= 0)
			{
				this.stageShutdownEvent.Set();
			}
		}

		internal int GetTotalResourceCount(PipelineDispatcher.PipelineResourceType resourceType)
		{
			return this.resources[(int)resourceType].TotalCount;
		}

		public PipelineSubmitStatus CanSubmitWorkItem(string key, PipelineDispatcher.ThrottledWorkItemType wiType)
		{
			ValidateArgument.NotNullOrEmpty(key, "key");
			PipelineSubmitStatus result;
			lock (this.lockObj)
			{
				result = this.throttleManager.GetWorkItemThrottler(wiType).CanSubmitWorkItem(key, null);
			}
			return result;
		}

		public PipelineSubmitStatus CanSubmitWorkItem(string key, string recipientId, PipelineDispatcher.ThrottledWorkItemType wiType)
		{
			ValidateArgument.NotNullOrEmpty(key, "key");
			ValidateArgument.NotNullOrEmpty(recipientId, "recipientId");
			PipelineSubmitStatus result;
			lock (this.lockObj)
			{
				result = this.throttleManager.GetWorkItemThrottler(wiType).CanSubmitWorkItem(key, recipientId);
			}
			return result;
		}

		public PipelineSubmitStatus CanSubmitLowPriorityWorkItem(string key, PipelineDispatcher.ThrottledWorkItemType wiType)
		{
			ValidateArgument.NotNullOrEmpty(key, "key");
			PipelineSubmitStatus result;
			lock (this.lockObj)
			{
				result = this.throttleManager.GetWorkItemThrottler(wiType).CanSubmitLowPriorityWorkItem(key, null);
			}
			return result;
		}

		public PipelineSubmitStatus CanSubmitLowPriorityWorkItem(string key, string recipientId, PipelineDispatcher.ThrottledWorkItemType wiType)
		{
			ValidateArgument.NotNullOrEmpty(key, "key");
			ValidateArgument.NotNullOrEmpty(recipientId, "recipientId");
			PipelineSubmitStatus result;
			lock (this.lockObj)
			{
				result = this.throttleManager.GetWorkItemThrottler(wiType).CanSubmitLowPriorityWorkItem(key, recipientId);
			}
			return result;
		}

		private static bool IsWatsonNeeded(Exception exception)
		{
			return GlobCfg.GenerateWatsonsForPipelineCleanup || !PipelineDispatcher.IsExpectedUMException(exception);
		}

		private static bool IsExpectedUMException(Exception exception)
		{
			return exception == null || exception is CDROperationException || exception is UserNotUmEnabledException || exception is InvalidObjectGuidException || exception is InvalidTenantGuidException || exception is InvalidAcceptedDomainException || exception is IOException || exception is UnauthorizedAccessException || exception is SmtpSubmissionException || exception is TransientException || exception is StoragePermanentException || exception is PipelineFullException;
		}

		private static bool MoveVoicemailToBadVoiceMailFolder(string filename)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
			DirectoryInfo directoryInfo = new DirectoryInfo(Utils.VoiceMailFilePath);
			FileInfo[] files = directoryInfo.GetFiles(fileNameWithoutExtension + ".*");
			bool result = false;
			int num;
			if (Utils.TryReadRegValue("System\\CurrentControlSet\\Services\\MSExchange Unified Messaging\\Parameters", "EnableBadVoiceMailFolder", out num) && num == 1)
			{
				result = true;
				PipelineDispatcher.MoveVoicemailToBadVoiceMailFolderHelper(filename, files, true);
			}
			else
			{
				PipelineDispatcher.MoveVoicemailToBadVoiceMailFolderHelper(filename, files, false);
			}
			return result;
		}

		private static void MoveVoicemailToBadVoiceMailFolderHelper(string headerFilePath, FileInfo[] allCorruptedFiles, bool movetoBVM)
		{
			for (int i = 0; i < allCorruptedFiles.Length; i++)
			{
				try
				{
					if (movetoBVM)
					{
						File.Move(allCorruptedFiles[i].FullName, Path.Combine(Utils.UMBadMailFilePath, allCorruptedFiles[i].Name));
					}
					else if (Path.GetFileName(headerFilePath).Equals(allCorruptedFiles[i].Name))
					{
						File.Move(allCorruptedFiles[i].FullName, Path.Combine(Utils.UMTempPath, allCorruptedFiles[i].Name));
					}
					else
					{
						Util.TryDeleteFile(allCorruptedFiles[i].FullName);
					}
				}
				catch (IOException)
				{
					Util.TryDeleteFile(allCorruptedFiles[i].FullName);
				}
				catch (UnauthorizedAccessException)
				{
					Util.TryDeleteFile(allCorruptedFiles[i].FullName);
				}
			}
		}

		private static void LogKillWorkItem(string fileName, Exception exception, bool moveToBVMFolder)
		{
			CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "Work Item with header file {0} is going to be killed. Final exception responsible: {1}. Moving to BVM: {2}", new object[]
			{
				fileName,
				exception,
				moveToBVMFolder
			});
			if (moveToBVMFolder)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_KillWorkItemAndMoveToBadVMFolder, null, new object[]
				{
					fileName,
					Utils.UMBadMailFilePath,
					CommonUtil.ToEventLogString(exception)
				});
				return;
			}
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_KillWorkItemAndDelete, null, new object[]
			{
				fileName,
				CommonUtil.ToEventLogString(exception)
			});
		}

		private void AcquireDiskQueueAndStartProcessing(object o)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "PipelineDispatcher.AcquireDiskQueueAndStartProcessing", new object[0]);
			while (this.diskQueueSemaphore == null && !this.IsShuttingDown)
			{
				Semaphore semaphore = null;
				try
				{
					semaphore = new Semaphore(1, 1, "0a90da68-66cb-11dc-8314-0800200c9a66");
					if (!semaphore.WaitOne(PipelineDispatcher.DiskQueueAcquisitionWaitInterval, false))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Unable to acquire exclusive access to disk queue.", new object[0]);
					}
					else
					{
						lock (this.lockObj)
						{
							if (!this.IsShuttingDown)
							{
								this.diskQueueSemaphore = semaphore;
								semaphore = null;
							}
						}
					}
				}
				catch (AbandonedMutexException)
				{
				}
				finally
				{
					if (semaphore != null)
					{
						semaphore.Close();
						semaphore = null;
					}
				}
			}
			lock (this.lockObj)
			{
				if (!this.IsShuttingDown)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "Successfully acquired access to disk queue and starting to process files", new object[0]);
					HealthCheckPipelineContext.TryDeleteHealthCheckFiles();
					this.InitialializeDiskQueueWatchers();
					this.CreateWorkItemsFromDiskQueue();
					this.DispatchWork();
				}
			}
		}

		private void InitialializeDiskQueueWatchers()
		{
			lock (this.lockObj)
			{
				if (!this.IsShuttingDown)
				{
					this.diskQueueWatcher = new FileSystemWatcher(Utils.VoiceMailFilePath, "*.txt");
					this.diskQueueWatcher.NotifyFilter = NotifyFilters.LastWrite;
					this.diskQueueWatcher.Changed += this.OnDiskQueueFileChanged;
					this.diskQueueWatcher.IncludeSubdirectories = false;
					this.diskQueueWatcher.EnableRaisingEvents = true;
					this.diskQueueFullScanTimer = new Timer(new TimerCallback(this.OnDiskQueueFullScanTimeout), null, GlobCfg.VoiceMessagePollingTime, GlobCfg.VoiceMessagePollingTime);
				}
			}
		}

		private void OnDiskQueueFileChanged(object source, FileSystemEventArgs args)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "OnDiskQueueFileChanged file={0}, changeType={1}", new object[]
			{
				args.FullPath,
				args.ChangeType
			});
			lock (this.lockObj)
			{
				if (!this.IsShuttingDown)
				{
					FileInfo diskQueueItem = new FileInfo(args.FullPath);
					this.CreateAndQueueWorkItem(diskQueueItem);
					this.DispatchWork();
				}
			}
		}

		private void OnDiskQueueFullScanTimeout(object o)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "OnDiskQueueFullScanTimeout @{0}", new object[]
			{
				ExDateTime.UtcNow
			});
			lock (this.lockObj)
			{
				if (!this.IsShuttingDown)
				{
					this.CreateWorkItemsFromDiskQueue();
					this.DispatchWork();
				}
			}
		}

		private void CreateWorkItemsFromDiskQueue()
		{
			lock (this.lockObj)
			{
				if (!this.IsShuttingDown)
				{
					try
					{
						Util.SetCounter(AvailabilityCounters.TotalQueuedMessages, (long)this.queuedWorkItemIds.Count);
						DirectoryInfo directoryInfo = new DirectoryInfo(Utils.VoiceMailFilePath);
						FileInfo[] files = directoryInfo.GetFiles("*.txt");
						List<FileInfo> list = new List<FileInfo>(files.Length);
						for (int i = 0; i < files.Length; i++)
						{
							try
							{
								new ExDateTime(ExTimeZone.UtcTimeZone, files[i].LastWriteTimeUtc);
								list.Add(files[i]);
							}
							catch (IOException)
							{
							}
						}
						list.Sort((FileInfo lhs, FileInfo rhs) => Comparer<ExDateTime>.Default.Compare(new ExDateTime(ExTimeZone.UtcTimeZone, lhs.LastWriteTimeUtc), new ExDateTime(ExTimeZone.UtcTimeZone, rhs.LastWriteTimeUtc)));
						foreach (FileInfo diskQueueItem in list)
						{
							this.CreateAndQueueWorkItem(diskQueueItem);
						}
					}
					catch (IOException ex)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "ProccessDiskQueue IOException={0}", new object[]
						{
							ex
						});
					}
				}
			}
		}

		private void CreateAndQueueWorkItem(FileInfo diskQueueItem)
		{
			lock (this.lockObj)
			{
				if (!this.IsShuttingDown)
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(diskQueueItem.Name);
					Guid guid;
					if (!GuidHelper.TryParseGuid(fileNameWithoutExtension, out guid))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "CreateAndQueueWorkItem ignoring invalid GUID ={0}", new object[]
						{
							fileNameWithoutExtension
						});
						this.KillWorkItem(new LocalizedException(Strings.KillWorkItemInvalidGuid(fileNameWithoutExtension)), diskQueueItem.FullName, null);
					}
					if (this.queuedWorkItemIds.ContainsKey(guid))
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "CreateAndQueueWorkItem ignoring known file={0}", new object[]
						{
							fileNameWithoutExtension
						});
					}
					else
					{
						PipelineWorkItem pipelineWorkItem = null;
						try
						{
							if (PipelineWorkItem.TryCreate(diskQueueItem, guid, out pipelineWorkItem))
							{
								if (pipelineWorkItem.Message is HealthCheckPipelineContext)
								{
									this.workQueue.Insert(0, pipelineWorkItem);
								}
								else
								{
									this.workQueue.Add(pipelineWorkItem);
								}
								this.queuedWorkItemIds.Add(guid, null);
								this.throttleManager.AddWorkItem(pipelineWorkItem);
								Util.SetCounter(AvailabilityCounters.TotalQueuedMessages, (long)this.queuedWorkItemIds.Count);
							}
						}
						catch (TransientException)
						{
						}
						catch (Exception ex)
						{
							if (!GrayException.IsGrayException(ex))
							{
								throw ex;
							}
							this.KillWorkItem(ex, diskQueueItem.FullName, pipelineWorkItem);
						}
					}
				}
			}
		}

		private void DispatchWork()
		{
			lock (this.lockObj)
			{
				if (!this.IsShuttingDown)
				{
					for (int i = 0; i < this.workQueue.Count; i++)
					{
						PipelineWorkItem pipelineWorkItem = this.workQueue[i];
						if (!pipelineWorkItem.IsRunning)
						{
							this.UpdateWorkItemSLACounterIfRequired(pipelineWorkItem);
							if (!pipelineWorkItem.HeaderFileExists)
							{
								this.KillWorkItem(new LocalizedException(Strings.KillWorkItemHeaderFileNotExist(pipelineWorkItem.HeaderFilename)), pipelineWorkItem.HeaderFilename, pipelineWorkItem);
							}
							else
							{
								PipelineStageBase currentStage = pipelineWorkItem.CurrentStage;
								PipelineResource pipelineResource = this.resources[(int)currentStage.ResourceType];
								if ((currentStage.RetrySchedule.TimeToTry || currentStage.MarkedForLastChanceHandling) && pipelineResource.TryAcquire(pipelineWorkItem))
								{
									CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "DispatchWork. about to run DispatchWorkAsync for stage={0}, workId={1}.", new object[]
									{
										currentStage,
										pipelineWorkItem.WorkId
									});
									currentStage.DispatchWorkAsync(new StageCompletionCallback(this.OnStageCompleted));
									this.numberOfActiveStages++;
									pipelineWorkItem.IsRunning = true;
								}
							}
						}
					}
				}
			}
		}

		private void OnStageCompleted(PipelineStageBase stage, PipelineWorkItem workItem, Exception error)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "OnStageCompleted.  sendingState={0}, workId={1}, NumActiveStages={2}, ", new object[]
			{
				stage,
				workItem.WorkId,
				this.numberOfActiveStages
			});
			lock (this.lockObj)
			{
				this.numberOfActiveStages--;
				this.resources[(int)stage.ResourceType].Release(workItem);
				workItem.IsRunning = false;
				if (this.IsShuttingDown)
				{
					if (this.numberOfActiveStages == 0 && this.stageShutdownEvent != null)
					{
						this.stageShutdownEvent.Set();
					}
				}
				else
				{
					if (error == null)
					{
						this.HandleSuccessfulStageCompletion(workItem);
					}
					else
					{
						this.HandleFailedStageCompletion(workItem, error);
					}
					this.DispatchWork();
				}
			}
		}

		private void HandleSuccessfulStageCompletion(PipelineWorkItem workItem)
		{
			workItem.AdvanceToNextStage();
			if (workItem.IsComplete)
			{
				this.StopProcessing(workItem);
			}
		}

		private void HandleFailedStageCompletion(PipelineWorkItem workItem, Exception error)
		{
			CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "PipeLineDispatcher. HandleError: Last error ={0}", new object[]
			{
				error
			});
			string text = CommonUtil.ToEventLogString(error);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_PipeLineError, text.GetHashCode().ToString(), new object[]
			{
				workItem.HeaderFilename,
				text
			});
			if (!GrayException.IsGrayException(error) && !PipelineDispatcher.IsExpectedUMException(error))
			{
				throw error;
			}
			if (error is WorkItemNeedsToBeRequeuedException)
			{
				this.StopProcessing(workItem);
				return;
			}
			if (workItem.CurrentStage.MarkedForLastChanceHandling)
			{
				CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "An Optional stage couldnt handle its last lifeline", new object[0]);
				this.KillWorkItem(error, workItem.HeaderFilename, workItem);
				return;
			}
			if (!workItem.CurrentStage.RetrySchedule.IsTimeToGiveUp)
			{
				CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "Leaving the stage unchanged. The stage still has more time to try.", new object[0]);
				return;
			}
			if (workItem.CurrentStage.RetrySchedule.IsStageOptional)
			{
				CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "Tried the WI for max number of retries. Still it didnt succeed. It was an optional WI. Last error ={0}, total delay ={1}", new object[]
				{
					error,
					workItem.CurrentStage.RetrySchedule.TotalDelayDueToThisStage
				});
				workItem.CurrentStage.MarkedForLastChanceHandling = true;
				return;
			}
			CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "Tried the WI for max number of retries. Still it didnt succeed. It was a NON optional WI. Last error ={0}, total delay ={1}", new object[]
			{
				error,
				workItem.CurrentStage.RetrySchedule.TotalDelayDueToThisStage
			});
			this.KillWorkItem(error, workItem.HeaderFilename, workItem);
		}

		private void KillWorkItem(Exception e, string headerFileName, PipelineWorkItem workItem)
		{
			if (workItem != null)
			{
				workItem.IsRejected = true;
				workItem.SLARecorded = false;
			}
			this.StopProcessing(workItem);
			bool flag = PipelineDispatcher.MoveVoicemailToBadVoiceMailFolder(headerFileName);
			PipelineDispatcher.LogKillWorkItem(headerFileName, e, flag);
			string text = flag ? Path.Combine(Utils.UMBadMailFilePath, Path.GetFileName(headerFileName)) : Path.Combine(Utils.UMTempPath, Path.GetFileName(headerFileName));
			if (PipelineDispatcher.IsWatsonNeeded(e))
			{
				if (!GrayException.IsGrayException(e))
				{
					throw e;
				}
				ExceptionHandling.SendWatsonWithExtraData(new PipelineCleanupGeneratedWatson(e), text, false);
			}
			if (!flag)
			{
				Util.TryDeleteFile(text);
			}
		}

		private void CleanUp()
		{
			lock (this.lockObj)
			{
				if (this.diskQueueSemaphore != null)
				{
					this.diskQueueSemaphore.Close();
				}
				if (this.stageShutdownEvent != null)
				{
					this.stageShutdownEvent.Close();
				}
				if (this.diskQueueWatcher != null)
				{
					this.diskQueueWatcher.Dispose();
				}
				if (this.diskQueueFullScanTimer != null)
				{
					this.diskQueueFullScanTimer.Dispose();
				}
			}
		}

		private void StopProcessing(PipelineWorkItem workItem)
		{
			this.UpdateWorkItemSLACounterIfRequired(workItem);
			if (workItem != null)
			{
				try
				{
					lock (this.lockObj)
					{
						this.queuedWorkItemIds.Remove(workItem.WorkId);
						bool flag2 = this.workQueue.Remove(workItem);
						if (flag2)
						{
							this.throttleManager.RemoveWorkItem(workItem);
						}
					}
					Util.SetCounter(AvailabilityCounters.TotalQueuedMessages, (long)this.queuedWorkItemIds.Count);
					this.UpdateWorkItemCompletionLatency(workItem);
				}
				finally
				{
					workItem.Dispose();
				}
			}
		}

		private void UpdateWorkItemSLACounterIfRequired(PipelineWorkItem workItem)
		{
			if (workItem != null)
			{
				if (!workItem.SLARecorded)
				{
					bool flag = !workItem.IsRejected && workItem.TimeInQueue <= workItem.ExpectedRunTime;
					if (workItem.IsComplete || !flag)
					{
						if (workItem.IsRejected)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "PipelineDispatcher::UpdateWorkItemSLA() SLA not met because the WorkItem {0} has been rejected. ActualRunTime = {1}", new object[]
							{
								workItem.WorkId,
								workItem.TimeInQueue.ToString()
							});
						}
						else
						{
							if (!flag)
							{
								UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_PipelineWorkItemSLAFailure, null, new object[]
								{
									workItem.HeaderFilename,
									workItem.ExpectedRunTime.TotalMinutes
								});
							}
							CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "PipelineDispatcher::UpdateWorkItemSLA() WorkItem = {0} ExpectedRunTime (SLA) = {1} ActualRunTime = {2}", new object[]
							{
								workItem.WorkId,
								workItem.ExpectedRunTime,
								workItem.IsComplete ? workItem.TimeInQueue.ToString() : "WorkItem still in progress"
							});
						}
						Util.SetCounter(AvailabilityCounters.UMPipelineSLA, (long)PipelineDispatcher.workItemsMeetingSLA.Update(flag));
						workItem.SLARecorded = true;
						return;
					}
				}
			}
			else
			{
				Util.SetCounter(AvailabilityCounters.UMPipelineSLA, (long)PipelineDispatcher.workItemsMeetingSLA.Update(false));
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "PipelineDispatcher::UpdateWorkItemSLA(). SLA not met because WorkItem could not be created.", new object[0]);
			}
		}

		private void UpdateWorkItemCompletionLatency(PipelineWorkItem workItem)
		{
			ValidateArgument.NotNull(workItem, "workItem");
			if (workItem.IsComplete)
			{
				long num = PipelineDispatcher.averageProcessingLatency.Update(workItem.TimeInQueue.TotalSeconds);
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "PipelineDispatcher::UpdateWorkItemLatency() AvgProcessingLatency = {0} after processing WorkItem = {1} ", new object[]
				{
					num,
					workItem.WorkId
				});
				Util.SetCounter(AvailabilityCounters.UMPipelineAverageLatency, num);
			}
		}

		private const string DiskQueueSemaphoreName = "0a90da68-66cb-11dc-8314-0800200c9a66";

		private static readonly TimeSpan DiskQueueAcquisitionWaitInterval = new TimeSpan(0, 0, 10);

		private readonly PipelineResource[] resources;

		private static TimeSpan diskQueueFullScanInterval = new TimeSpan(0, 1, 0);

		private static PipelineDispatcher instance = new PipelineDispatcher();

		private static MovingAverage averageProcessingLatency = new MovingAverage(50);

		private static PercentageBooleanSlidingCounter workItemsMeetingSLA = PercentageBooleanSlidingCounter.CreateSuccessCounter(1000, TimeSpan.FromHours(1.0));

		private Semaphore diskQueueSemaphore;

		private AutoResetEvent stageShutdownEvent = new AutoResetEvent(false);

		private bool isShuttingDown;

		private bool isInitialized;

		private FileSystemWatcher diskQueueWatcher;

		private Timer diskQueueFullScanTimer;

		private int numberOfActiveStages;

		private Dictionary<Guid, object> queuedWorkItemIds = new Dictionary<Guid, object>(64);

		private List<PipelineWorkItem> workQueue = new List<PipelineWorkItem>(64);

		private PipelineDispatcher.WorkItemThrottleManager throttleManager = new PipelineDispatcher.WorkItemThrottleManager();

		private object lockObj = new object();

		internal enum PipelineResourceType
		{
			LowPriorityCpuBound,
			CpuBound,
			NetworkBound,
			Count
		}

		public enum ThrottledWorkItemType
		{
			CDRWorkItem,
			NonCDRWorkItem
		}

		public class WIThrottleData
		{
			public string Key { get; set; }

			public string RecipientId { get; set; }

			public PipelineDispatcher.ThrottledWorkItemType WorkItemType { get; set; }
		}

		private class WorkItemThrottleManager
		{
			public bool IsPipelineHealthy
			{
				get
				{
					return this.nonCDRWIThrottler.Count < GlobCfg.MaxNonCDRMessagesPendingInPipeline;
				}
			}

			public PipelineDispatcher.WorkItemThrottler GetWorkItemThrottler(PipelineDispatcher.ThrottledWorkItemType workItemType)
			{
				switch (workItemType)
				{
				case PipelineDispatcher.ThrottledWorkItemType.CDRWorkItem:
					return this.cdrWIThrottler;
				case PipelineDispatcher.ThrottledWorkItemType.NonCDRWorkItem:
					return this.nonCDRWIThrottler;
				default:
					throw new NotImplementedException("Unknown ThrottledWorkItemType");
				}
			}

			public void AddWorkItem(PipelineWorkItem wi)
			{
				PipelineDispatcher.WIThrottleData throttlingData = wi.GetThrottlingData();
				if (throttlingData != null)
				{
					this.GetWorkItemThrottler(throttlingData.WorkItemType).AddWorkItem(throttlingData.Key, throttlingData.RecipientId);
				}
			}

			public void RemoveWorkItem(PipelineWorkItem wi)
			{
				PipelineDispatcher.WIThrottleData throttlingData = wi.GetThrottlingData();
				if (throttlingData != null)
				{
					this.GetWorkItemThrottler(throttlingData.WorkItemType).RemoveWorkItem(throttlingData.Key, throttlingData.RecipientId);
				}
			}

			private PipelineDispatcher.CDRWorkItemThrottler cdrWIThrottler = new PipelineDispatcher.CDRWorkItemThrottler();

			private PipelineDispatcher.NonCDRWorkItemThrottler nonCDRWIThrottler = new PipelineDispatcher.NonCDRWorkItemThrottler();
		}

		internal abstract class WorkItemThrottler
		{
			public abstract int MaxItemsInTablePerKey { get; }

			public abstract int MaxItemsInPipeline { get; }

			public int Count { get; protected set; }

			public abstract void AddWorkItem(string key, string recipientId);

			public abstract void RemoveWorkItem(string key, string recipientId);

			public PipelineSubmitStatus CanSubmitWorkItem(string key, string recipientId)
			{
				return this.CanSubmitWorkItem(key, recipientId, 1f);
			}

			public PipelineSubmitStatus CanSubmitLowPriorityWorkItem(string key, string recipientId)
			{
				return this.CanSubmitWorkItem(key, recipientId, 0.5f);
			}

			protected abstract PipelineSubmitStatus CanSubmitWorkItem(string key, string recipientId, float allowedPipelinePercentageFull);

			private const float NormalPriorityAllowedPipelinePercentageFull = 1f;

			private const float LowPriorityAllowedPipelinePercentageFull = 0.5f;
		}

		internal class CDRWorkItemThrottler : PipelineDispatcher.WorkItemThrottler
		{
			public override int MaxItemsInTablePerKey
			{
				get
				{
					return AppConfig.Instance.Service.MaxCDRMessagesInPipeline / 4;
				}
			}

			public override int MaxItemsInPipeline
			{
				get
				{
					return AppConfig.Instance.Service.MaxCDRMessagesInPipeline;
				}
			}

			public override void AddWorkItem(string key, string recipientId)
			{
				ValidateArgument.NotNull(key, "key");
				if (recipientId != null)
				{
					throw new ArgumentException("CDRWorkItemThrottler.AddWorkItem: recipientId not null");
				}
				if (!this.workItemTable.ContainsKey(key))
				{
					this.workItemTable[key] = 0;
				}
				Dictionary<string, int> dictionary;
				(dictionary = this.workItemTable)[key] = dictionary[key] + 1;
				base.Count++;
			}

			public override void RemoveWorkItem(string key, string recipientId)
			{
				ValidateArgument.NotNull(key, "key");
				if (recipientId != null)
				{
					throw new ArgumentException("CDRWorkItemThrottler.RemoveWorkItem: recipientId not null");
				}
				Dictionary<string, int> dictionary;
				(dictionary = this.workItemTable)[key] = dictionary[key] - 1;
				base.Count--;
				if (this.workItemTable[key] == 0)
				{
					this.workItemTable.Remove(key);
				}
			}

			protected override PipelineSubmitStatus CanSubmitWorkItem(string key, string recipientId, float allowedPipelinePercentageFull)
			{
				ValidateArgument.NotNull(key, "key");
				if (recipientId != null)
				{
					throw new ArgumentException("CDRWorkItemThrottler.CanSubmitWorkItem: recipientId not null");
				}
				int num = 0;
				this.workItemTable.TryGetValue(key, out num);
				if ((double)base.Count < Math.Round((double)((float)this.MaxItemsInPipeline * allowedPipelinePercentageFull)) && (double)num < Math.Round((double)((float)this.MaxItemsInTablePerKey * allowedPipelinePercentageFull)))
				{
					return PipelineSubmitStatus.Ok;
				}
				return PipelineSubmitStatus.PipelineFull;
			}

			private Dictionary<string, int> workItemTable = new Dictionary<string, int>(16);
		}

		internal class NonCDRWorkItemThrottler : PipelineDispatcher.WorkItemThrottler
		{
			public override int MaxItemsInPipeline
			{
				get
				{
					return GlobCfg.MaxNonCDRMessagesPendingInPipeline;
				}
			}

			public override int MaxItemsInTablePerKey
			{
				get
				{
					return AppConfig.Instance.Service.MaxMessagesPerMailboxServer;
				}
			}

			public int RecipientStartThrottlingThresholdPercent
			{
				get
				{
					return AppConfig.Instance.Service.RecipientStartThrottlingThresholdPercent;
				}
			}

			public int RecipientThrottlingPercent
			{
				get
				{
					return AppConfig.Instance.Service.RecipientThrottlingPercent;
				}
			}

			public override void AddWorkItem(string key, string recipientId)
			{
				ValidateArgument.NotNull(key, "key");
				ValidateArgument.NotNull(recipientId, "recipientId");
				base.Count++;
				PipelineDispatcher.NonCDRWorkItemThrottler.KeyEntryValue keyEntryValue;
				if (!this.workItemTable.TryGetValue(key, out keyEntryValue))
				{
					keyEntryValue = new PipelineDispatcher.NonCDRWorkItemThrottler.KeyEntryValue();
					this.workItemTable.Add(key, keyEntryValue);
				}
				keyEntryValue.Count++;
				if (!keyEntryValue.RecipientTable.ContainsKey(recipientId))
				{
					keyEntryValue.RecipientTable[recipientId] = 0;
				}
				Dictionary<string, int> recipientTable;
				(recipientTable = keyEntryValue.RecipientTable)[recipientId] = recipientTable[recipientId] + 1;
				this.Log("AddWorkItem", string.Empty, key, recipientId, keyEntryValue.Count, keyEntryValue.RecipientTable[recipientId]);
			}

			public override void RemoveWorkItem(string key, string recipientId)
			{
				ValidateArgument.NotNull(key, "key");
				ValidateArgument.NotNull(recipientId, "recipientId");
				base.Count--;
				PipelineDispatcher.NonCDRWorkItemThrottler.KeyEntryValue keyEntryValue = this.workItemTable[key];
				keyEntryValue.Count--;
				Dictionary<string, int> recipientTable;
				(recipientTable = keyEntryValue.RecipientTable)[recipientId] = recipientTable[recipientId] - 1;
				int recipientCount = keyEntryValue.RecipientTable[recipientId];
				if (keyEntryValue.RecipientTable[recipientId] == 0)
				{
					keyEntryValue.RecipientTable.Remove(recipientId);
				}
				if (keyEntryValue.Count == 0)
				{
					this.workItemTable.Remove(key);
				}
				this.Log("RemoveWorkItem", string.Empty, key, recipientId, keyEntryValue.Count, recipientCount);
			}

			public int GetRecipientThrottlingThreshold(float allowedPipelinePercentageFull)
			{
				return (int)Math.Round((double)((float)(this.GetMaxItemsPerKey(allowedPipelinePercentageFull) * this.RecipientStartThrottlingThresholdPercent) / 100f));
			}

			public int GetMaxItemsPerRecipient(float allowedPipelinePercentageFull)
			{
				return (int)Math.Round((double)((float)(this.GetMaxItemsPerKey(allowedPipelinePercentageFull) * this.RecipientThrottlingPercent) / 100f));
			}

			protected override PipelineSubmitStatus CanSubmitWorkItem(string key, string recipientId, float allowedPipelinePercentageFull)
			{
				ValidateArgument.NotNull(key, "key");
				ValidateArgument.NotNull(recipientId, "recipientId");
				int count = base.Count;
				int num = 0;
				int num2 = 0;
				PipelineDispatcher.NonCDRWorkItemThrottler.KeyEntryValue keyEntryValue;
				if (this.workItemTable.TryGetValue(key, out keyEntryValue))
				{
					num = keyEntryValue.Count;
					keyEntryValue.RecipientTable.TryGetValue(recipientId, out num2);
				}
				if ((double)count >= Math.Round((double)((float)this.MaxItemsInPipeline * allowedPipelinePercentageFull)))
				{
					this.Log("CanSubmitWorkItem", "no - pipeline full", key, recipientId, num, num2);
					return PipelineSubmitStatus.PipelineFull;
				}
				if (num >= this.GetMaxItemsPerKey(allowedPipelinePercentageFull))
				{
					this.Log("CanSubmitWorkItem", "no - reached max per key", key, recipientId, num, num2);
					return PipelineSubmitStatus.PipelineFull;
				}
				if (num < this.GetRecipientThrottlingThreshold(allowedPipelinePercentageFull))
				{
					this.Log("CanSubmitWorkItem", "yes (throttling inactive)", key, recipientId, num, num2);
					return PipelineSubmitStatus.Ok;
				}
				if (num2 < this.GetMaxItemsPerRecipient(allowedPipelinePercentageFull))
				{
					this.Log("CanSubmitWorkItem", "yes - (throttling active)", key, recipientId, num, num2);
					return PipelineSubmitStatus.Ok;
				}
				this.Log("CanSubmitWorkItem", "no - (throttling active)", key, recipientId, num, num2);
				return PipelineSubmitStatus.RecipientThrottled;
			}

			private int GetMaxItemsPerKey(float allowedPipelinePercentageFull)
			{
				return (int)Math.Round((double)((float)this.MaxItemsInTablePerKey * allowedPipelinePercentageFull));
			}

			private void Log(string method, string msg, string mailboxServer, string recipientId, int mailboxServerCount, int recipientCount)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "NonCDRWorkItemThrottler.{0} {1} [{2}] [{3}] Pipeline: {4} Mailbox server {5} Recipient: {6}", new object[]
				{
					method,
					msg,
					mailboxServer,
					recipientId,
					base.Count,
					mailboxServerCount,
					recipientCount
				});
			}

			private Dictionary<string, PipelineDispatcher.NonCDRWorkItemThrottler.KeyEntryValue> workItemTable = new Dictionary<string, PipelineDispatcher.NonCDRWorkItemThrottler.KeyEntryValue>(16);

			private class KeyEntryValue
			{
				public int Count { get; set; }

				public Dictionary<string, int> RecipientTable = new Dictionary<string, int>(128);
			}
		}
	}
}
