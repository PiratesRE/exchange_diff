using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ThirdPartyReplication;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal abstract class TagHandler
	{
		protected TagHandler(string description, FailureItemWatcher watcher, DatabaseFailureItem dbfi)
		{
			this.m_description = description;
			this.m_dbfi = dbfi;
			this.m_tracePrefix = string.Format("[TH] ({0}):", watcher.Database.Guid);
			this.m_database = Dependencies.ADConfig.GetDatabase(watcher.Database.Guid);
			this.PostProcessingAction = null;
		}

		internal string TagDescription
		{
			get
			{
				return this.m_description;
			}
		}

		internal abstract bool IsTPRMoveTheActiveRecoveryAction { get; }

		internal Action PostProcessingAction { get; set; }

		internal virtual ExEventLog.EventTuple? Event9aSrc
		{
			get
			{
				return null;
			}
		}

		internal virtual ExEventLog.EventTuple? Event9aTgt
		{
			get
			{
				return null;
			}
		}

		internal virtual ExEventLog.EventTuple? Event9bSrc
		{
			get
			{
				return null;
			}
		}

		internal virtual ExEventLog.EventTuple? Event9bTgt
		{
			get
			{
				return null;
			}
		}

		internal virtual bool RaiseMANotificationItem
		{
			get
			{
				return false;
			}
		}

		internal IADDatabase Database
		{
			get
			{
				return this.m_database;
			}
		}

		internal DatabaseFailureItem FailureItem
		{
			get
			{
				return this.m_dbfi;
			}
		}

		internal bool CopyRoleIsActive { get; set; }

		internal static TagHandler GetInstance(FailureItemWatcher watcher, DatabaseFailureItem dbfi)
		{
			TagHandler result = null;
			switch (dbfi.Tag)
			{
			case FailureTag.NoOp:
				result = new NoOpTagHandler(watcher, dbfi);
				break;
			case FailureTag.Configuration:
				result = new ConfigurationTagHandler(watcher, dbfi);
				break;
			case FailureTag.Repairable:
				result = new RepairableTagHandler(watcher, dbfi);
				break;
			case FailureTag.Space:
				result = new SpaceTagHandler(watcher, dbfi);
				break;
			case FailureTag.IoHard:
				result = new IoHardTagHandler(watcher, dbfi);
				break;
			case FailureTag.SourceCorruption:
				result = new SourceCorruptionTagHandler(watcher, dbfi);
				break;
			case FailureTag.Corruption:
				result = new CorruptionTagHandler(watcher, dbfi);
				break;
			case FailureTag.Hard:
				result = new HardTagHandler(watcher, dbfi);
				break;
			case FailureTag.Unrecoverable:
				result = new UnrecoverableTagHandler(watcher, dbfi);
				break;
			case FailureTag.Remount:
				result = new RemountTagHandler(watcher, dbfi);
				break;
			case FailureTag.Reseed:
				result = new ReseedTagHandler(watcher, dbfi);
				break;
			case FailureTag.Performance:
				result = new PerformanceTagHandler(watcher, dbfi);
				break;
			case FailureTag.MoveLoad:
				result = new MoveLoadTagHandler(watcher, dbfi);
				break;
			case FailureTag.Memory:
				result = new MemoryTagHandler(watcher, dbfi);
				break;
			case FailureTag.CatalogReseed:
				result = new CatalogReseedTagHandler(watcher, dbfi);
				break;
			case FailureTag.AlertOnly:
				result = new AlertOnlyTagHandler(watcher, dbfi);
				break;
			case FailureTag.ExpectedDismount:
				result = new ExpectedDismountTagHandler(watcher, dbfi);
				break;
			case FailureTag.UnexpectedDismount:
				result = new UnexpectedDismountTagHandler(watcher, dbfi);
				break;
			case FailureTag.ExceededMaxDatabases:
				result = new ExceededMaxDatabasesTagHandler(watcher, dbfi);
				break;
			case FailureTag.GenericMountFailure:
				result = new GenericMountFailureTagHandler(watcher, dbfi);
				break;
			case FailureTag.PagePatchRequested:
				result = new PagePatchRequestedTagHandler(watcher, dbfi);
				break;
			case FailureTag.PagePatchCompleted:
				result = new PagePatchCompletedTagHandler(watcher, dbfi);
				break;
			case FailureTag.LostFlushDetected:
				result = new LostFlushDetectedTagHandler(watcher, dbfi);
				break;
			case FailureTag.SourceLogCorruption:
				result = new SourceLogCorruptionTagHandler(watcher, dbfi);
				break;
			case FailureTag.FailedToRepair:
				result = new FailedToRepairTagHandler(watcher, dbfi);
				break;
			case FailureTag.LostFlushDbTimeTooOld:
				result = new LostFlushDbTimeTooOldTagHandler(watcher, dbfi);
				break;
			case FailureTag.LostFlushDbTimeTooNew:
				result = new LostFlushDbTimeTooNewTagHandler(watcher, dbfi);
				break;
			case FailureTag.ExceededMaxActiveDatabases:
				result = new ExceededMaxActiveDatabasesTagHandler(watcher, dbfi);
				break;
			case FailureTag.SourceLogCorruptionOutsideRequiredRange:
				result = new SourceLogCorruptionOutsideRequiredRangeTagHandler(watcher, dbfi);
				break;
			case FailureTag.HungIoExceededThreshold:
				result = new HungIoExceededThresholdTagHandler(watcher, dbfi);
				break;
			case FailureTag.HungIoCancelSucceeded:
				result = new HungIoCancelSucceededTagHandler(watcher, dbfi);
				break;
			case FailureTag.HungIoCancelFailed:
				result = new HungIoCancelFailedTagHandler(watcher, dbfi);
				break;
			case FailureTag.RecoveryRedoLogCorruption:
				result = new RecoveryRedoLogCorruptionTagHandler(watcher, dbfi);
				break;
			case FailureTag.ReplayFailedToPagePatch:
				result = new ReplayFailedToPagePatchTagHandler(watcher, dbfi);
				break;
			case FailureTag.FileSystemCorruption:
				result = new FileSystemCorruptionTagHandler(watcher, dbfi);
				break;
			case FailureTag.HungIoLowThreshold:
				result = new HungIoLowThresholdTagHandler(watcher, dbfi);
				break;
			case FailureTag.HungIoMediumThreshold:
				result = new HungIoMediumThresholdTagHandler(watcher, dbfi);
				break;
			case FailureTag.HungIoExceededThresholdDoubleDisk:
				result = new HungIoExceededThresholdDoubleDiskTagHandler(watcher, dbfi);
				break;
			case FailureTag.HungStoreWorker:
				result = new HungStoreWorkerTagHandler(watcher, dbfi);
				break;
			case FailureTag.UnaccessibleStoreWorker:
				result = new UnaccessibleStoreWorkerTagHandler(watcher, dbfi);
				break;
			case FailureTag.MonitoredDatabaseFailed:
				result = new MonitoredDatabaseFailedTagHandler(watcher, dbfi);
				break;
			case FailureTag.LogGapFatal:
				result = new LogGapFatalTagHandler(watcher, dbfi);
				break;
			case FailureTag.ExceededDatabaseMaxSize:
				result = new ExceededDatabaseMaxSizeTagHandler(watcher, dbfi);
				break;
			case FailureTag.LowDiskSpaceStraggler:
				result = new LowDiskSpaceStragglerTagHandler(watcher, dbfi);
				break;
			case FailureTag.LockedVolume:
				result = new LockedVolumeTagHandler(watcher, dbfi);
				break;
			default:
				ExTraceGlobals.FailureItemTracer.TraceDebug<FailureTag>(0L, "[TH] Invalid failure item tag '{0}'", dbfi.Tag);
				break;
			}
			return result;
		}

		internal abstract void ActiveRecoveryActionInternal();

		internal abstract void PassiveRecoveryActionInternal();

		internal bool Execute()
		{
			bool isSuccess = false;
			LatencyChecker.Measure("FailureItem.TagHandler.Execute", this.m_dbfi.ToString(), TimeSpan.FromSeconds((double)RegistryParameters.FailureItemProcessingAllowedLatencyInSec), delegate()
			{
				isSuccess = this.ExecuteInternal();
			});
			return isSuccess;
		}

		private bool ExecuteInternal()
		{
			Exception ex = null;
			this.CopyRoleIsActive = false;
			if (this.m_database == null)
			{
				return true;
			}
			if (RegistryParameters.DisableFailureItemProcessing)
			{
				this.Trace("Execute(): FailureItem processing has been disabled by a registry override.", new object[0]);
				return true;
			}
			try
			{
				AmConfig config = AmSystemManager.Instance.Config;
				if (config.IsUnknown)
				{
					this.TraceError("Execute(): AmConfig is unknown. Returning false.", new object[0]);
					return false;
				}
				if (config.IsIgnoreServerDebugOptionEnabled(AmServerName.LocalComputerName))
				{
					ReplayCrimsonEvents.OperationNotPerformedDueToDebugOption.Log<string, AmDebugOptions, string>(AmServerName.LocalComputerName.NetbiosName, AmDebugOptions.IgnoreServerFromAutomaticActions, "FailureItemProcessing");
					return true;
				}
				if (ThirdPartyManager.IsThirdPartyReplicationEnabled)
				{
					if (!this.IsMountingServerTheLocalServer(config))
					{
						return true;
					}
					this.CopyRoleIsActive = true;
					if (this.IsTPRMoveTheActiveRecoveryAction)
					{
						this.Trace("Execute(): TPR FailureItem with move action. Forwarding to vendor", new object[0]);
						AmDbStateInfo amDbStateInfo = config.DbState.Read(this.m_dbfi.Guid);
						if (ThirdPartyManager.Instance.DatabaseMoveNeeded(this.m_dbfi.Guid, AmServerName.LocalComputerName.Fqdn, !amDbStateInfo.IsAdminDismounted) == NotificationResponse.Complete)
						{
							return true;
						}
						this.TraceError("Execute(): TPR FailureItem was incomplete. Need to retry later", new object[0]);
						return false;
					}
					else
					{
						this.Trace("Execute(): TPR FailureItem is not a move, so executing the appropriate handler", new object[0]);
						this.ActiveRecoveryAction();
					}
				}
				else if (this.IsMountingServerTheLocalServer(config))
				{
					this.CopyRoleIsActive = true;
					this.ActiveRecoveryAction();
				}
				else
				{
					this.PassiveRecoveryAction();
				}
				this.RaiseSuccessActionEvent9a(this.CopyRoleIsActive);
			}
			catch (ClusterException ex2)
			{
				this.TraceError("Execute(): ClusterException occurred. Returning false. Exception: {0}", new object[]
				{
					ex2
				});
				return false;
			}
			catch (TPRInitException ex3)
			{
				this.TraceError("Execute(): TPRInitException. Returning false to retry later: {0}", new object[]
				{
					ex3
				});
				return false;
			}
			catch (DatabaseFailoverFailedException ex4)
			{
				ex = ex4;
			}
			catch (DatabaseRemountFailedException ex5)
			{
				ex = ex5;
			}
			catch (ActiveRecoveryNotApplicableException ex6)
			{
				ex = ex6;
			}
			catch (DatabaseCopySuspendException ex7)
			{
				ex = ex7;
			}
			catch (DatabaseLogCorruptRecoveryException ex8)
			{
				ex = ex8;
			}
			catch (FailureItemRecoveryException ex9)
			{
				ex = ex9;
			}
			catch (TransientException ex10)
			{
				ex = ex10;
			}
			if (ex != null)
			{
				this.RaiseFailureActionEvent9b(ex, this.CopyRoleIsActive);
			}
			return true;
		}

		internal bool IsMountingServerTheLocalServer(AmConfig cfg)
		{
			AmDbStateInfo amDbStateInfo = cfg.DbState.Read(this.m_dbfi.Guid);
			return amDbStateInfo.IsActiveServerValid && amDbStateInfo.ActiveServer.IsLocalComputerName;
		}

		internal void ActiveRecoveryAction()
		{
			this.Trace("{0}: Starting active recovery action for ", new object[]
			{
				this.TagDescription
			});
			if (this.Database.ReplicationType == ReplicationType.Remote)
			{
				this.ActiveRecoveryActionInternal();
				this.Trace("{0}: Finished active recovery action", new object[]
				{
					this.TagDescription
				});
				return;
			}
			throw new ActiveRecoveryNotApplicableException(this.Database.Name);
		}

		internal void PassiveRecoveryAction()
		{
			this.Trace("{0}: Starting passive recovery action", new object[]
			{
				this.TagDescription
			});
			this.PassiveRecoveryActionInternal();
			this.Trace("{0}: Finished passive recovery action", new object[]
			{
				this.TagDescription
			});
		}

		internal void RaiseSuccessActionEvent9a(bool isSource)
		{
			ExEventLog.EventTuple? eventTuple = isSource ? this.Event9aSrc : this.Event9aTgt;
			if (eventTuple != null)
			{
				TagHandler.EventLog.LogEvent(eventTuple.Value, null, this.GetFailureItemParameters(null));
				if (this.RaiseMANotificationItem)
				{
					EventNotificationItem manotificationItem = this.GetMANotificationItem(null);
					manotificationItem.Publish(false);
				}
			}
		}

		internal ExEventLog.EventTuple? GetSuccessActionMessageTuple()
		{
			if (!this.CopyRoleIsActive)
			{
				return this.Event9aTgt;
			}
			return this.Event9aSrc;
		}

		internal uint GetSuccessActionMessageId()
		{
			ExEventLog.EventTuple? successActionMessageTuple = this.GetSuccessActionMessageTuple();
			if (successActionMessageTuple == null)
			{
				return 0U;
			}
			return DiagCore.GetEventViewerEventId(successActionMessageTuple.Value);
		}

		internal string FormatSuccessActionMessage()
		{
			ExEventLog.EventTuple? successActionMessageTuple = this.GetSuccessActionMessageTuple();
			if (successActionMessageTuple == null)
			{
				throw new ArgumentException("A success message should be defined for tagHandler {0}", this.TagDescription);
			}
			string text = DiagCore.DbMsgEventTupleToString(successActionMessageTuple.Value, this.GetFailureItemParameters(null));
			if (text == null)
			{
				text = ReplayStrings.TagHandlerFormatMsgFailed(DiagCore.GetEventViewerEventId(successActionMessageTuple.Value));
			}
			return text;
		}

		internal void RaiseFailureActionEvent9b(Exception ex, bool isSource)
		{
			ExEventLog.EventTuple? eventTuple = isSource ? this.Event9bSrc : this.Event9bTgt;
			if (eventTuple != null)
			{
				TagHandler.EventLog.LogEvent(eventTuple.Value, null, this.GetFailureItemParameters(ex));
				if (this.RaiseMANotificationItem)
				{
					EventNotificationItem manotificationItem = this.GetMANotificationItem(ex);
					manotificationItem.Publish(false);
				}
			}
		}

		internal object[] GetFailureItemParameters(Exception ex)
		{
			List<object> list = new List<object>();
			list.Add(this.Database.Name);
			list.Add(this.m_dbfi.Guid);
			list.Add(this.m_dbfi.NameSpace.ToString());
			list.Add(this.m_dbfi.Tag.ToString());
			list.Add(this.m_dbfi.InstanceName);
			list.Add(this.m_dbfi.ComponentName);
			list.Add((ex != null) ? ex.Message : string.Empty);
			list.Add(this.m_dbfi.CreationTime.ToString());
			if (this.m_dbfi.IoError != null)
			{
				list.Add(this.m_dbfi.IoError.Category.ToString());
				list.Add(this.m_dbfi.IoError.FileName);
				list.Add(this.m_dbfi.IoError.Offset);
				list.Add(this.m_dbfi.IoError.Size);
			}
			return list.ToArray();
		}

		internal EventNotificationItem GetMANotificationItem(Exception ex)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem("msexchangerepl", "DbFailureItem", this.Database.Name, ResultSeverityLevel.Critical);
			eventNotificationItem.AddCustomProperty("Database", this.Database.Name);
			eventNotificationItem.AddCustomProperty("DatabaseGuid", this.m_dbfi.Guid);
			eventNotificationItem.AddCustomProperty("Namespace", this.m_dbfi.NameSpace.ToString());
			eventNotificationItem.AddCustomProperty("Tag", this.m_dbfi.Tag.ToString());
			eventNotificationItem.AddCustomProperty("InstanceName", this.m_dbfi.InstanceName);
			eventNotificationItem.AddCustomProperty("ComponentName", this.m_dbfi.ComponentName);
			eventNotificationItem.AddCustomProperty("Exception", (ex != null) ? ex.Message : string.Empty);
			eventNotificationItem.AddCustomProperty("CreationTime", this.m_dbfi.CreationTime.ToString());
			eventNotificationItem.AddCustomProperty("IoErrorCategory", (this.m_dbfi.IoError != null) ? this.m_dbfi.IoError.Category.ToString() : string.Empty);
			eventNotificationItem.AddCustomProperty("IoErrorFilename", (this.m_dbfi.IoError != null) ? this.m_dbfi.IoError.FileName : string.Empty);
			eventNotificationItem.AddCustomProperty("IoErrorOffset", (this.m_dbfi.IoError != null) ? this.m_dbfi.IoError.Offset.ToString() : string.Empty);
			eventNotificationItem.AddCustomProperty("IoErrorSize", (this.m_dbfi.IoError != null) ? this.m_dbfi.IoError.Size.ToString() : string.Empty);
			return eventNotificationItem;
		}

		protected void SuspendLocalCopy()
		{
			if (ThirdPartyManager.IsThirdPartyReplicationEnabled)
			{
				return;
			}
			string suspendMsg = ReplayStrings.TagHandlerSuspendCopy(this.FormatSuccessActionMessage());
			DatabaseTasks.SuspendLocalDatabaseCopy(this.Database, suspendMsg);
		}

		protected Exception SuspendAndFailLocalCopyNoThrow(bool blockResume = false, bool blockReseed = false, bool blockInPlaceReseed = false)
		{
			if (ThirdPartyManager.IsThirdPartyReplicationEnabled)
			{
				return null;
			}
			string text = this.FormatSuccessActionMessage();
			uint successActionMessageId = this.GetSuccessActionMessageId();
			string suspendMsg = ReplayStrings.TagHandlerSuspendCopy(text);
			Exception ex = DatabaseTasks.SuspendAndFailLocalDatabaseCopy(this.Database, suspendMsg, text, successActionMessageId, blockResume, blockReseed, blockInPlaceReseed);
			if (ex != null)
			{
				ReplayCrimsonEvents.FailureItemProcessingErrorDuringSuspend.Log<IADDatabase, string>(this.Database, ex.Message);
			}
			return ex;
		}

		protected void SuspendAndFailLocalCopy(bool blockResume = false, bool blockReseed = false, bool blockInPlaceReseed = false)
		{
			Exception ex = this.SuspendAndFailLocalCopyNoThrow(blockResume, blockReseed, blockInPlaceReseed);
			if (ex != null)
			{
				throw ex;
			}
		}

		protected void MoveAfterSuspendAndFailLocalCopy(bool blockResume = false, bool blockReseed = false, bool blockInPlaceReseed = false)
		{
			this.SuspendAndFailLocalCopyNoThrow(blockResume, blockReseed, blockInPlaceReseed);
			DatabaseTasks.Move(this.Database, Environment.MachineName);
		}

		protected void Trace(string formatStr, params object[] args)
		{
			if (ExTraceGlobals.FailureItemTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string formatString = this.m_tracePrefix + formatStr;
				ExTraceGlobals.FailureItemTracer.TraceDebug(0L, formatString, args);
			}
		}

		private void TraceError(string formatStr, params object[] args)
		{
			if (ExTraceGlobals.FailureItemTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				string formatString = this.m_tracePrefix + formatStr;
				ExTraceGlobals.FailureItemTracer.TraceError(0L, formatString, args);
			}
		}

		private const string WinInitProcess = "wininit";

		private static readonly ExEventLog EventLog = new ExEventLog(ExTraceGlobals.FailureItemTracer.Category, "ExchangeStoreDB");

		private string m_tracePrefix;

		private string m_description;

		private DatabaseFailureItem m_dbfi;

		private IADDatabase m_database;
	}
}
