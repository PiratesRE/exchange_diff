using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class PingCommand : Command, IAsyncCommand
	{
		public PingCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfPing;
		}

		public bool ProcessingEventsEnabled { get; set; }

		public string DevicePhoneNumberForSms { get; set; }

		public bool DeviceEnableOutboundSMS { get; set; }

		internal NotificationManager Notifier { get; set; }

		internal override bool RequiresPolicyCheck
		{
			get
			{
				return false;
			}
		}

		internal override bool ShouldOpenGlobalSyncState
		{
			get
			{
				return true;
			}
		}

		protected override string RootNodeName
		{
			get
			{
				return "Ping";
			}
		}

		private static XmlDocument NoChangesXml
		{
			get
			{
				if (PingCommand.nothingChangedXml == null)
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Ping xmlns=\"Ping:\"><Status>1</Status></Ping>");
					PingCommand.nothingChangedXml = xmlDocument;
				}
				return PingCommand.nothingChangedXml;
			}
		}

		private static XmlDocument FoldersOutOfRangeXml
		{
			get
			{
				if (PingCommand.foldersOutOfRangeXml == null)
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Ping xmlns=\"Ping:\"><Status>6</Status><MaxFolders>" + GlobalSettings.MaxNumOfFolders + "</MaxFolders></Ping>");
					PingCommand.foldersOutOfRangeXml = xmlDocument;
				}
				return PingCommand.foldersOutOfRangeXml;
			}
		}

		private static XmlDocument FolderSyncRequiredXml
		{
			get
			{
				if (PingCommand.folderSyncRequiredXml == null)
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Ping xmlns=\"Ping:\"><Status>7</Status></Ping>");
					PingCommand.folderSyncRequiredXml = xmlDocument;
				}
				return PingCommand.folderSyncRequiredXml;
			}
		}

		private static XmlDocument SendParametersXml
		{
			get
			{
				if (PingCommand.sendParametersXml == null)
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Ping xmlns=\"Ping:\"><Status>3</Status></Ping>");
					PingCommand.sendParametersXml = xmlDocument;
				}
				return PingCommand.sendParametersXml;
			}
		}

		private static XmlDocument HbiTooLowXml
		{
			get
			{
				if (PingCommand.hbiTooLowXml == null)
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Ping xmlns=\"Ping:\"><Status>5</Status><HeartbeatInterval>" + GlobalSettings.HeartbeatInterval.LowInterval + "</HeartbeatInterval></Ping>");
					PingCommand.hbiTooLowXml = xmlDocument;
				}
				return PingCommand.hbiTooLowXml;
			}
		}

		private static XmlDocument HbiTooHighXml
		{
			get
			{
				if (PingCommand.hbiTooHighXml == null)
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Ping xmlns=\"Ping:\"><Status>5</Status><HeartbeatInterval>" + GlobalSettings.HeartbeatInterval.HighInterval + "</HeartbeatInterval></Ping>");
					PingCommand.hbiTooHighXml = xmlDocument;
				}
				return PingCommand.hbiTooHighXml;
			}
		}

		private static XmlDocument ServerErrorXml
		{
			get
			{
				if (PingCommand.serverErrorXml == null)
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><Ping xmlns=\"Ping:\"><Status>8</Status></Ping>");
					PingCommand.serverErrorXml = xmlDocument;
				}
				return PingCommand.serverErrorXml;
			}
		}

		public void ReleaseNotificationManager(bool wasStolen)
		{
			if (!this.ProcessingEventsEnabled)
			{
				throw new InvalidOperationException("Release NotificationManager called while processing events wasn't enabled!");
			}
			this.Notifier = null;
			if (this.requestCompleted)
			{
				return;
			}
			try
			{
				if (base.RequestWaitWatch != null)
				{
					base.RequestWaitWatch.Stop();
					base.ProtocolLogger.SetValue(ProtocolLoggerData.RequestHangTime, base.RequestWaitWatch.ElapsedMilliseconds / 1000L);
				}
				if (wasStolen)
				{
					AirSyncDiagnostics.TraceWarning(ExTraceGlobals.RequestsTracer, this, "PingCollisionDetected");
					base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NMStolen");
					base.XmlResponse = PingCommand.NoChangesXml;
				}
				this.CompleteRequest(base.XmlResponse, false);
			}
			finally
			{
				this.ReleaseResources();
			}
		}

		public void Consume(Event evt)
		{
			using (ExPerfTrace.RelatedActivity(base.GetTraceActivityId()))
			{
				AirSyncDiagnostics.TraceInfo<EventObjectType, EventType>(ExTraceGlobals.RequestsTracer, this, "Ping.Consume evt.ObjectType:{0} evt.EventType:{1}", evt.ObjectType, evt.EventType);
				if (!this.ProcessingEventsEnabled)
				{
					throw new InvalidOperationException("Release NotificationManager called while processing events wasn't enabled!");
				}
				if (evt == null)
				{
					throw new ArgumentNullException("evt");
				}
				if (!this.requestCompleted)
				{
					if (base.RequestWaitWatch != null)
					{
						base.RequestWaitWatch.Stop();
						base.ProtocolLogger.SetValue(ProtocolLoggerData.RequestHangTime, base.RequestWaitWatch.ElapsedMilliseconds / 1000L);
					}
					base.ProtocolLogger.SetValue(ProtocolLoggerData.TimeContinued, ExDateTime.UtcNow);
					bool flag = false;
					SyncCollection syncCollection = null;
					PingCommand.PingInformationForNotifier pingInformationForNotifier = (PingCommand.PingInformationForNotifier)this.Notifier.Information;
					if (!this.isDirectPushAllowed)
					{
						throw new InvalidOperationException("PingCommand.Consume called when DirectPush not enabled!");
					}
					string action = null;
					try
					{
						ActivityContext.SetThreadScope(base.User.Context.ActivityScope);
						action = base.User.Context.ActivityScope.Action;
						base.User.Context.ActivityScope.Action = this.RootNodeName;
						using (GetItemEstimateCommand getItemEstimateCommand = new GetItemEstimateCommand())
						{
							this.OpenResources();
							getItemEstimateCommand.Context = base.Context;
							syncCollection = SyncCollection.CreateSyncCollection(base.MailboxSession, base.Version, null);
							getItemEstimateCommand.DeviceEnableOutboundSMS = pingInformationForNotifier.DeviceEnableOutboundSMS;
							getItemEstimateCommand.DevicePhoneNumberForSms = pingInformationForNotifier.DevicePhoneNumberForSms;
							switch (evt.ObjectType)
							{
							case EventObjectType.Item:
								if (base.IsInQuarantinedState)
								{
									AirSyncDiagnostics.TraceError<EventObjectType>(ExTraceGlobals.RequestsTracer, this, "Ping: Consume(): Should not hit in quarantined state! : {0}", evt.ObjectType);
									flag = true;
									this.CompleteRequest(PingCommand.ServerErrorXml, false);
								}
								else if (pingInformationForNotifier.HangingVirtualFolderIds == null || !pingInformationForNotifier.HangingVirtualFolderIds.Contains(evt.ObjectId))
								{
									if (((evt.EventType & EventType.ObjectMoved) == EventType.ObjectMoved && !pingInformationForNotifier.DictFoldersByStoreId.ContainsKey(evt.ParentObjectId) && !pingInformationForNotifier.DictFoldersByStoreId.ContainsKey(evt.OldParentObjectId)) || ((evt.EventType & EventType.ObjectMoved) != EventType.ObjectMoved && !pingInformationForNotifier.DictFoldersByStoreId.ContainsKey(evt.ParentObjectId)))
									{
										AirSyncDiagnostics.TraceDebug<EventType, StoreObjectId, StoreObjectId>(ExTraceGlobals.AlgorithmTracer, this, "Received event in PingCommand.Consume() for an unknown folder.  EventType: {0}\nParentObjectId: {1}\nOldParentObjectId: {2}", evt.EventType, evt.ParentObjectId, evt.OldParentObjectId);
									}
									else if ((evt.EventType & EventType.ObjectMoved) == EventType.ObjectMoved && !pingInformationForNotifier.DictFoldersByStoreId.ContainsKey(evt.ParentObjectId))
									{
										if (!pingInformationForNotifier.DictFoldersByStoreId[evt.OldParentObjectId].HasChanges)
										{
											PingCommand.DPFolderInfo dpfolderInfo = pingInformationForNotifier.DictFoldersByStoreId[evt.OldParentObjectId];
											syncCollection.CollectionId = dpfolderInfo.ShortId;
											syncCollection.ClassType = dpfolderInfo.Class;
											syncCollection.WindowSize = 1;
											pingInformationForNotifier.DictFoldersByStoreId[evt.OldParentObjectId].HasChanges = this.FolderChangedSinceLastSync(dpfolderInfo, getItemEstimateCommand, syncCollection, false, true);
										}
									}
									else
									{
										PingCommand.DPFolderInfo dpfolderInfo = pingInformationForNotifier.DictFoldersByStoreId[evt.ParentObjectId];
										syncCollection.CollectionId = dpfolderInfo.ShortId;
										syncCollection.ClassType = dpfolderInfo.Class;
										syncCollection.WindowSize = 1;
										if ((evt.EventType & EventType.ObjectDeleted) == EventType.ObjectDeleted || ((evt.EventType & EventType.ObjectModified) == EventType.ObjectModified && pingInformationForNotifier.DictFoldersByStoreId[evt.ParentObjectId].Class == "Email"))
										{
											if (!pingInformationForNotifier.DictFoldersByStoreId[evt.ParentObjectId].HasChanges)
											{
												pingInformationForNotifier.DictFoldersByStoreId[evt.ParentObjectId].HasChanges = this.FolderChangedSinceLastSync(dpfolderInfo, getItemEstimateCommand, syncCollection, false, true);
											}
										}
										else if (this.FolderChangedSinceLastSync(dpfolderInfo, getItemEstimateCommand, syncCollection, false, true) || base.XmlResponse != null)
										{
											pingInformationForNotifier.DictFoldersByStoreId[evt.ParentObjectId].HasChanges = true;
											flag = true;
											this.CompleteRequest(this.ComposeChangesResponse(), false);
										}
									}
								}
								break;
							case EventObjectType.Folder:
								if ((evt.EventType & EventType.ObjectCreated) != EventType.ObjectCreated && (evt.EventType & EventType.ObjectMoved) != EventType.ObjectMoved && (evt.EventType & EventType.ObjectDeleted) != EventType.ObjectDeleted)
								{
									string message = string.Format("A hanging sync or ping session was notified of an unexpected folder change event of type {0}.", evt.EventType.ToString());
									throw new InvalidOperationException(message);
								}
								if ((evt.EventFlags & EventFlags.NonIPMChange) == EventFlags.NonIPMChange)
								{
									try
									{
										if (!evt.ParentObjectId.Equals(pingInformationForNotifier.SyncStateStorageStoreObjectId))
										{
											AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "PingCommand:Consume Return as folder is in NON_IPM tree and is not under SyncStateStorageStoreObjectId");
											break;
										}
										this.OpenResources();
										using (Folder folder = Folder.Bind(base.MailboxSession, evt.ObjectId))
										{
											if (folder.DisplayName != "AutdTrigger" || folder.DisplayName != SyncStateStorage.MailboxLoggingTriggerFolder)
											{
												AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "PingCommand:Consume Return as folder is in NON_IPM tree and is under SyncStateStorageStoreObjectId, but not wipe or mailboxlogging");
												break;
											}
										}
									}
									catch (ObjectNotFoundException)
									{
									}
								}
								flag = true;
								base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.FHSyncRequired");
								this.CompleteRequest(PingCommand.FolderSyncRequiredXml, false);
								break;
							default:
								flag = true;
								base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.UnknownNotification");
								this.CompleteRequest(PingCommand.ServerErrorXml, false);
								AirSyncDiagnostics.TraceError<EventObjectType>(ExTraceGlobals.RequestsTracer, this, "Ping: Consume(): unknown object type: {0}", evt.ObjectType);
								break;
							}
						}
					}
					catch (Exception ex)
					{
						AirSyncUtility.ProcessException(ex, this, base.Context);
						flag = true;
						this.CompleteRequest(base.XmlResponse, ex, false, false);
					}
					finally
					{
						if (syncCollection != null)
						{
							syncCollection.Dispose();
						}
						this.ReleaseResources();
						ActivityContext.ClearThreadScope();
						if (base.User != null && base.User.Context != null && base.User.Context.ActivityScope != null)
						{
							base.User.Context.ActivityScope.Action = action;
						}
						if (!flag)
						{
							if (base.RequestWaitWatch != null)
							{
								base.RequestWaitWatch.Start();
							}
							base.Context.PrepareToHang();
						}
					}
				}
			}
		}

		public void HandleAccountTerminated(NotificationManager.AsyncEvent evt)
		{
			using (ExPerfTrace.RelatedActivity(base.GetTraceActivityId()))
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "Ping.HandleAccountTerminated account state: {0}", evt.AccountState.ToString());
				if (!this.ProcessingEventsEnabled)
				{
					throw new InvalidOperationException("HandleAccountTerminated called while processing events wasn't enabled!");
				}
				if (!this.requestCompleted)
				{
					string action = null;
					try
					{
						ActivityContext.SetThreadScope(base.User.Context.ActivityScope);
						action = base.User.Context.ActivityScope.Action;
						base.User.Context.ActivityScope.Action = this.RootNodeName;
						if (base.RequestWaitWatch != null)
						{
							base.RequestWaitWatch.Stop();
							base.ProtocolLogger.SetValue(ProtocolLoggerData.RequestHangTime, base.RequestWaitWatch.ElapsedMilliseconds / 1000L);
						}
						base.ProtocolLogger.SetValue(ProtocolLoggerData.AccountTerminated, evt.AccountState.ToString());
						base.SetHttpStatusCodeForTerminatedAccount(evt.AccountState);
						this.CompleteRequest(base.XmlResponse, null, false, false);
					}
					finally
					{
						this.ReleaseResources();
						ActivityContext.ClearThreadScope();
						if (base.User != null && base.User.Context != null && base.User.Context.ActivityScope != null)
						{
							base.User.Context.ActivityScope.Action = action;
						}
					}
				}
			}
		}

		public void HandleException(Exception ex)
		{
			using (ExPerfTrace.RelatedActivity(base.GetTraceActivityId()))
			{
				AirSyncDiagnostics.TraceInfo<Exception>(ExTraceGlobals.RequestsTracer, this, "Ping.HandleException\r\n{0}", ex);
				if (!this.ProcessingEventsEnabled)
				{
					throw new InvalidOperationException("Release NotificationManager called while processing events wasn't enabled!");
				}
				if (this.requestCompleted)
				{
					if (!AirSyncUtility.HandleNonCriticalException(ex, false))
					{
						throw ex;
					}
				}
				else
				{
					try
					{
						try
						{
							if (base.RequestWaitWatch != null)
							{
								base.RequestWaitWatch.Stop();
								base.ProtocolLogger.SetValue(ProtocolLoggerData.RequestHangTime, base.RequestWaitWatch.ElapsedMilliseconds / 1000L);
							}
							base.XmlResponse = null;
							AirSyncUtility.ProcessException(ex, this, base.Context);
							this.CompleteRequest(base.XmlResponse, ex, false, false);
						}
						finally
						{
							this.ReleaseResources();
						}
					}
					catch (Exception ex2)
					{
						if (!AirSyncUtility.HandleNonCriticalException(ex2, true))
						{
							throw;
						}
					}
				}
			}
		}

		public void HeartbeatCallback()
		{
			using (ExPerfTrace.RelatedActivity(base.GetTraceActivityId()))
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.HeartbeatCallback");
				if (!this.ProcessingEventsEnabled)
				{
					throw new InvalidOperationException("Release NotificationManager called while processing events wasn't enabled!");
				}
				if (!this.requestCompleted)
				{
					string action = null;
					try
					{
						ActivityContext.SetThreadScope(base.User.Context.ActivityScope);
						base.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.HangTimedOut, true);
						action = base.User.Context.ActivityScope.Action;
						base.User.Context.ActivityScope.Action = this.RootNodeName;
						if (base.RequestWaitWatch != null)
						{
							base.RequestWaitWatch.Stop();
							base.ProtocolLogger.SetValue(ProtocolLoggerData.RequestHangTime, base.RequestWaitWatch.ElapsedMilliseconds / 1000L);
						}
						base.ProtocolLogger.SetValue(ProtocolLoggerData.RequestTimedOut, this.isDirectPushAllowedByGeo ? 2 : (this.isDirectPushAllowed ? 1 : 3));
						this.CompleteRequest((this.changesExist && this.isDirectPushAllowed) ? this.ComposeChangesResponse() : PingCommand.NoChangesXml, null, !this.changesExist, true);
					}
					finally
					{
						this.ReleaseResources();
						ActivityContext.ClearThreadScope();
						if (base.User != null && base.User.Context != null && base.User.Context.ActivityScope != null)
						{
							base.User.Context.ActivityScope.Action = action;
						}
					}
				}
			}
		}

		public uint GetHeartbeatInterval()
		{
			uint result = 0U;
			try
			{
				this.ReadXmlRequest(null, out result);
			}
			catch (AirSyncPermanentException arg)
			{
				AirSyncDiagnostics.TraceError<AirSyncPermanentException>(ExTraceGlobals.RequestsTracer, null, "Exception in Ping.GetHeartbeatInterval: {0}", arg);
			}
			return result;
		}

		internal override bool ValidateXml()
		{
			return base.XmlRequest == null || base.ValidateXml();
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (PingCommand.validationErrorXml == null)
			{
				XmlDocument commandXmlStub = base.GetCommandXmlStub();
				XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
				xmlElement.InnerText = XmlConvert.ToString(4);
				commandXmlStub[this.RootNodeName].AppendChild(xmlElement);
				PingCommand.validationErrorXml = commandXmlStub;
			}
			return PingCommand.validationErrorXml;
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			Command.ExecutionState executionState = Command.ExecutionState.Complete;
			if (base.CurrentAccessState == DeviceAccessState.DeviceDiscovery)
			{
				base.GlobalInfo.DeviceInformationPromoted = true;
				if (!base.IsDeviceAccessAllowed())
				{
					return executionState;
				}
			}
			executionState = Command.ExecutionState.Invalid;
			PingCommand.PingInformationForNotifier pingInformationForNotifier = new PingCommand.PingInformationForNotifier(this.DevicePhoneNumberForSms, this.DeviceEnableOutboundSMS);
			this.Notifier.Information = pingInformationForNotifier;
			Command.ExecutionState result;
			try
			{
				this.rootState = base.SyncStateStorage.GetFolderHierarchySyncState();
				uint num;
				if (this.rootState == null)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Ping: missing root state.");
					base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.MissingRootState1");
					base.XmlResponse = PingCommand.FolderSyncRequiredXml;
					executionState = Command.ExecutionState.Complete;
					result = executionState;
				}
				else if (!this.ReadXmlRequest(pingInformationForNotifier, out num))
				{
					executionState = Command.ExecutionState.Complete;
					result = executionState;
				}
				else
				{
					AirSyncCounters.HeartbeatInterval.RawValue = (long)((ulong)num);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.HeartBeatInterval, num);
					PingCommand.PingHbiMonitor.Instance.RegisterSample(num, base.Context);
					pingInformationForNotifier.DictFoldersByStoreId = new Dictionary<StoreObjectId, PingCommand.DPFolderInfo>();
					foreach (PingCommand.DPFolderInfo dpfolderInfo in pingInformationForNotifier.DictFoldersByShortId.Values)
					{
						if (dpfolderInfo.StoreId != null)
						{
							pingInformationForNotifier.DictFoldersByStoreId[dpfolderInfo.StoreId] = dpfolderInfo;
						}
					}
					if (this.isDirectPushAllowed)
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "PingCommand:ExecuteCommand creating backend subscriptions");
						this.CreateSubscriptions();
					}
					if (this.ItemsChangedSinceLastSync())
					{
						AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Ping: changes have occurred since last sync.");
						base.XmlResponse = this.ComposeChangesResponse();
						executionState = Command.ExecutionState.Complete;
						result = executionState;
					}
					else if (base.XmlResponse != null)
					{
						executionState = Command.ExecutionState.Complete;
						result = executionState;
					}
					else
					{
						this.Notifier.StartTimer((num > 0U) ? num : 1U, base.Context.RequestTime, base.NextPolicyRefreshTime);
						base.RequestWaitWatch = Stopwatch.StartNew();
						pingInformationForNotifier.SyncStateStorageStoreObjectId = base.SyncStateStorage.FolderId;
						pingInformationForNotifier.ExchangeSyncDataStoreObjectId = base.SyncStateStorage.SyncRootFolderId;
						executionState = Command.ExecutionState.Pending;
						result = executionState;
					}
				}
			}
			catch (Exception)
			{
				base.ProtocolLogger.IncrementValue(ProtocolLoggerData.NumErrors);
				throw;
			}
			finally
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Ping: entered Execute()'s finally block.");
				switch (executionState)
				{
				case Command.ExecutionState.Pending:
					Interlocked.Increment(ref PingCommand.numberOfOutstandingPings);
					AirSyncCounters.CurrentlyPendingPing.RawValue = (long)PingCommand.numberOfOutstandingPings;
					base.Context.PrepareToHang();
					break;
				case Command.ExecutionState.Complete:
					if (!base.Context.Response.IsClientConnected)
					{
						AirSyncCounters.NumberOfDroppedPing.Increment();
					}
					if (base.XmlResponse != null)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.StatusCode, int.Parse(base.XmlResponse["Ping"]["Status"].InnerText, CultureInfo.InvariantCulture));
					}
					this.CleanupNotificationManager(false);
					break;
				}
				if (this.rootState != null)
				{
					this.rootState.Dispose();
					this.rootState = null;
				}
				if (this.folderIdMappingState != null)
				{
					this.folderIdMappingState.Dispose();
					this.folderIdMappingState = null;
				}
				if (executionState == Command.ExecutionState.Complete)
				{
					this.requestCompleted = true;
				}
			}
			return result;
		}

		internal override void SetStateData(Command.StateData data)
		{
			this.DevicePhoneNumberForSms = data.DevicePhoneNumberForSms;
			this.DeviceEnableOutboundSMS = data.DeviceEnableOutboundSMS;
		}

		protected override void Dispose(bool disposing)
		{
			NotificationManager notifier = this.Notifier;
			if (notifier != null)
			{
				notifier.ReleaseCommand(this);
			}
			base.Dispose(disposing);
		}

		protected override void CompleteHttpRequest()
		{
			if (!this.isDirectPushAllowed)
			{
				base.Context.Response.AppendHeader("X-MS-NoPush", string.Empty);
			}
			this.requestCompleted = true;
			base.CompleteHttpRequest();
		}

		protected override void ProcessQueuedEvents()
		{
			if (this.Notifier != null)
			{
				this.Notifier.ProcessQueuedEvents(this);
			}
		}

		protected override void GetOrCreateNotificationManager(out bool notificationManagerWasTaken)
		{
			this.isDirectPushAllowed = DeviceCapability.IsDirectPushAllowed(base.Context, out this.isDirectPushAllowedByGeo);
			if (base.Context.Request.IsEmpty && this.isDirectPushAllowed)
			{
				this.Notifier = NotificationManager.GetOrCreateNotificationManager(base.Context, this, out notificationManagerWasTaken);
				if (notificationManagerWasTaken)
				{
					base.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.StolenNM, true);
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "PingCommand:GetOrCreateNotificationManager stole a notification manager");
					Interlocked.Increment(ref PingCommand.numberOfOutstandingPings);
					AirSyncCounters.CurrentlyPendingPing.RawValue = (long)PingCommand.numberOfOutstandingPings;
					AirSyncCounters.HeartbeatInterval.RawValue = (long)((ulong)this.Notifier.RequestedWaitTime);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.HeartBeatInterval, this.Notifier.RequestedWaitTime);
					PingCommand.PingHbiMonitor.Instance.RegisterSample(this.Notifier.RequestedWaitTime, base.Context);
					return;
				}
			}
			else
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "PingCommand:GetOrCreateNotificationManager created a new notification manager");
				notificationManagerWasTaken = false;
				this.Notifier = NotificationManager.CreateNotificationManager(base.Context, this);
			}
		}

		protected override void SetNotificationManagerMailboxLogging(bool mailboxLogging)
		{
			this.Notifier.MailboxLoggingEnabled = mailboxLogging;
		}

		protected override bool HandleQuarantinedState()
		{
			return true;
		}

		private static bool FolderDictionaryEquals(Dictionary<string, PingCommand.DPFolderInfo> dictionary1, Dictionary<string, PingCommand.DPFolderInfo> dictionary2)
		{
			if (dictionary1 == null || dictionary2 == null || dictionary1.Count != dictionary2.Count)
			{
				return false;
			}
			foreach (string key in dictionary1.Keys)
			{
				if (!dictionary2.ContainsKey(key) || !dictionary2[key].Equals(dictionary1[key]))
				{
					return false;
				}
			}
			return true;
		}

		private bool ReadXmlRequest(PingCommand.PingInformationForNotifier pingInfoForNotifier, out uint heartbeatInterval)
		{
			heartbeatInterval = 0U;
			int? num = null;
			Dictionary<string, PingCommand.DPFolderInfo> dictionary = null;
			IAutdStatusData autdStatusData = null;
			try
			{
				autdStatusData = (base.User.Features.IsEnabled(EasFeature.SyncStatusOnGlobalInfo) ? NewAutdStatusData.Load(base.GlobalInfo, base.SyncStateStorage) : AutdStatusData.Load(base.SyncStateStorage, false, true));
				if (base.XmlRequest == null)
				{
					if (base.Request.ContentType != null || base.Request.ContentLength != 0)
					{
						base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidContent");
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.First140Error, null, false);
					}
					num = autdStatusData.LastPingHeartbeat;
					dictionary = autdStatusData.DPFolderList;
				}
				else
				{
					XmlElement xmlRequest = base.XmlRequest;
					xmlRequest.Normalize();
					if (!xmlRequest.HasChildNodes)
					{
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Ping: requests of the form <Ping/> and <Ping></Ping> are prohibited.");
						base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.NoChilds");
						base.XmlResponse = this.GetValidationErrorXml();
						return false;
					}
					if (xmlRequest.ChildNodes.Count == 1 && xmlRequest.ChildNodes[0].NodeType != XmlNodeType.Element)
					{
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Ping: requests like <Ping>foo</Ping> are prohibited.");
						base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.NoChildNodes");
						base.XmlResponse = this.GetValidationErrorXml();
						return false;
					}
					if (!this.ParsePingElement(base.XmlRequest, out heartbeatInterval))
					{
						AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Ping: failed to parse Ping element.");
						return false;
					}
					int? lastPingHeartbeat = autdStatusData.LastPingHeartbeat;
					if (heartbeatInterval != 0U && (lastPingHeartbeat == null || (long)lastPingHeartbeat.Value != (long)((ulong)heartbeatInterval)))
					{
						autdStatusData.LastPingHeartbeat = new int?((int)heartbeatInterval);
					}
					Dictionary<string, PingCommand.DPFolderInfo> dpfolderList = autdStatusData.DPFolderList;
					if (pingInfoForNotifier != null && pingInfoForNotifier.DictFoldersByShortId != null && (dpfolderList == null || !PingCommand.FolderDictionaryEquals(pingInfoForNotifier.DictFoldersByShortId, dpfolderList)))
					{
						autdStatusData.DPFolderList = pingInfoForNotifier.DictFoldersByShortId;
					}
					num = autdStatusData.LastPingHeartbeat;
					dictionary = autdStatusData.DPFolderList;
					autdStatusData.SaveAndDispose();
					autdStatusData = null;
					base.Context.Request.XmlDocument = null;
				}
			}
			finally
			{
				if (autdStatusData != null)
				{
					IDisposable disposable = autdStatusData as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (num == null || dictionary == null)
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Ping: client needs to send all parameters.");
				base.XmlResponse = PingCommand.SendParametersXml;
				return false;
			}
			if (heartbeatInterval == 0U)
			{
				heartbeatInterval = (uint)num.Value;
			}
			if (pingInfoForNotifier != null && pingInfoForNotifier.DictFoldersByShortId == null)
			{
				pingInfoForNotifier.DictFoldersByShortId = dictionary;
			}
			return true;
		}

		private bool ParsePingElement(XmlElement pingElement, out uint heartbeatInterval)
		{
			heartbeatInterval = 0U;
			using (XmlNodeList childNodes = pingElement.ChildNodes)
			{
				foreach (object obj in childNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string localName;
					if ((localName = xmlNode.LocalName) != null)
					{
						if (!(localName == "HeartbeatInterval"))
						{
							if (localName == "Folders")
							{
								if (!this.ParseFoldersElement((XmlElement)xmlNode))
								{
									return false;
								}
								continue;
							}
						}
						else
						{
							int num;
							if (!int.TryParse(xmlNode.InnerText, out num))
							{
								throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), null, false)
								{
									ErrorStringForProtocolLogger = "Ping.InvalidHBI"
								};
							}
							if (num < GlobalSettings.HeartbeatInterval.LowInterval)
							{
								base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.HbiTooLow");
								base.XmlResponse = PingCommand.HbiTooLowXml;
								return false;
							}
							if (num > GlobalSettings.HeartbeatInterval.HighInterval)
							{
								base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.HbiTooHigh");
								base.XmlResponse = PingCommand.HbiTooHighXml;
								return false;
							}
							heartbeatInterval = (uint)num;
							continue;
						}
					}
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), null, false)
					{
						ErrorStringForProtocolLogger = "UnexpectedPingNode:" + xmlNode.Name
					};
				}
			}
			return true;
		}

		private bool ParseFoldersElement(XmlElement folderselement)
		{
			PingCommand.PingInformationForNotifier pingInformationForNotifier = (PingCommand.PingInformationForNotifier)this.Notifier.Information;
			if (pingInformationForNotifier == null)
			{
				return true;
			}
			pingInformationForNotifier.DictFoldersByShortId = new Dictionary<string, PingCommand.DPFolderInfo>();
			if (this.folderIdMappingState == null)
			{
				this.folderIdMappingState = base.SyncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]);
			}
			if (this.folderIdMappingState == null || (FolderIdMapping)this.folderIdMappingState[CustomStateDatumType.IdMapping] == null)
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Ping: missing folder id mapping state.");
				base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.MissingFolderIdMapping");
				base.XmlResponse = PingCommand.FolderSyncRequiredXml;
				return false;
			}
			FolderIdMapping folderIdMapping = (FolderIdMapping)this.folderIdMappingState[CustomStateDatumType.IdMapping];
			using (XmlNodeList childNodes = folderselement.ChildNodes)
			{
				if (childNodes.Count >= GlobalSettings.MaxNumOfFolders)
				{
					AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "Ping: client specified too many folders: {0}", GlobalSettings.MaxNumOfFolders);
					base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.TooManyFolders");
					base.XmlResponse = PingCommand.FoldersOutOfRangeXml;
					return false;
				}
				foreach (object obj in childNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.LocalName != "Folder")
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), null, false)
						{
							ErrorStringForProtocolLogger = "Ping.UnexpectedFoldersNode:" + xmlNode.Name
						};
					}
					PingCommand.DPFolderInfo dpfolderInfo = new PingCommand.DPFolderInfo();
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						string localName;
						if ((localName = xmlNode2.LocalName) != null)
						{
							if (localName == "Id")
							{
								dpfolderInfo.ShortId = xmlNode2.InnerText;
								continue;
							}
							if (localName == "Class")
							{
								dpfolderInfo.Class = xmlNode2.InnerText;
								continue;
							}
						}
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), null, false)
						{
							ErrorStringForProtocolLogger = "UnexpectedFolderNode:" + xmlNode2.Name
						};
					}
					if (string.IsNullOrEmpty(dpfolderInfo.ShortId))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), null, false)
						{
							ErrorStringForProtocolLogger = "Ping:MissingShortId"
						};
					}
					if (string.IsNullOrEmpty(dpfolderInfo.Class))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), null, false)
						{
							ErrorStringForProtocolLogger = "Ping:MissingClass"
						};
					}
					MailboxSyncItemId mailboxSyncItemId = folderIdMapping[dpfolderInfo.ShortId] as MailboxSyncItemId;
					dpfolderInfo.StoreId = ((mailboxSyncItemId == null) ? null : ((StoreObjectId)mailboxSyncItemId.NativeId));
					if (dpfolderInfo.StoreId == null && (AirSyncUtility.GetCollectionType(dpfolderInfo.ShortId) == SyncCollection.CollectionTypes.Mailbox || AirSyncUtility.GetCollectionType(dpfolderInfo.ShortId) == SyncCollection.CollectionTypes.Unknown))
					{
						AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "Ping: no mapping for short id: {0}", dpfolderInfo.ShortId);
						base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.NoMappingFor:" + dpfolderInfo.ShortId);
						base.XmlResponse = PingCommand.FolderSyncRequiredXml;
						return false;
					}
					if (pingInformationForNotifier.DictFoldersByShortId.ContainsKey(dpfolderInfo.ShortId))
					{
						AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "Ping: client specified duplicate folder id: {0}", dpfolderInfo.ShortId);
						base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.DuplicateId:" + dpfolderInfo.ShortId);
						base.XmlResponse = this.GetValidationErrorXml();
						return false;
					}
					pingInformationForNotifier.DictFoldersByShortId.Add(dpfolderInfo.ShortId, dpfolderInfo);
					base.Context.ProtocolLogger.SetValue(dpfolderInfo.ShortId, PerFolderProtocolLoggerData.FolderId, dpfolderInfo.ShortId);
					base.Context.ProtocolLogger.IncrementValue(ProtocolLoggerData.TotalFolders);
				}
			}
			return true;
		}

		private void CreateSubscriptions()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.CreateSubscriptions");
			EventCondition eventCondition = new EventCondition();
			eventCondition.ObjectType = EventObjectType.Folder;
			eventCondition.EventType = (EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectMoved);
			this.Notifier.Add(EventSubscription.Create(base.MailboxSession, eventCondition, this.Notifier));
			PingCommand.PingInformationForNotifier pingInformationForNotifier = (PingCommand.PingInformationForNotifier)this.Notifier.Information;
			if (!base.IsInQuarantinedState)
			{
				using (Dictionary<string, PingCommand.DPFolderInfo>.ValueCollection.Enumerator enumerator = pingInformationForNotifier.DictFoldersByShortId.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PingCommand.DPFolderInfo dpfolderInfo = enumerator.Current;
						if (dpfolderInfo.StoreId != null)
						{
							eventCondition = new EventCondition();
							eventCondition.ObjectType = EventObjectType.Item;
							eventCondition.EventType = (EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectModified | EventType.ObjectMoved);
							eventCondition.ContainerFolderIds.Add(dpfolderInfo.StoreId);
							this.Notifier.Add(EventSubscription.Create(base.MailboxSession, eventCondition, this.Notifier));
						}
					}
					return;
				}
			}
			AirSyncDiagnostics.TraceInfo<DeviceAccessState>(ExTraceGlobals.RequestsTracer, this, "PingCommand: Skip item level subscription creation. Current AccessState = {0}", base.CurrentAccessState);
		}

		private void CreateSubscriptionForVirtualFolder(SyncCollection syncCollection)
		{
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "Ping.CreateSubscriptionForVirtualFolder CollectionId:{0}", syncCollection.CollectionId);
			if (!base.IsInQuarantinedState)
			{
				PingCommand.PingInformationForNotifier pingInformationForNotifier = (PingCommand.PingInformationForNotifier)this.Notifier.Information;
				if (syncCollection.NativeStoreObjectId != null && pingInformationForNotifier.HangingVirtualFolderIds != null && pingInformationForNotifier.HangingVirtualFolderIds.Contains(syncCollection.NativeStoreObjectId))
				{
					return;
				}
				EventCondition eventCondition = syncCollection.CreateEventCondition();
				if (eventCondition != null)
				{
					this.Notifier.Add(EventSubscription.Create(base.MailboxSession, eventCondition, this.Notifier));
					pingInformationForNotifier.AddToHangingVirtualFolderIds(syncCollection.NativeStoreObjectId);
					return;
				}
			}
			else
			{
				AirSyncDiagnostics.TraceInfo<DeviceAccessState>(ExTraceGlobals.RequestsTracer, this, "Skip subscription creation for virtual folder. Current AccessState = {0}", base.CurrentAccessState);
			}
		}

		private bool FolderHierarchyChangedSinceLastSync(bool setResponseOnChange)
		{
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "Ping.FolderHierarchyChangedSinceLastSync setResponseOnChange:{0}", setResponseOnChange);
			bool flag = false;
			bool flag2 = false;
			FolderHierarchyChangeDetector.SyncHierarchyManifestState latestState = null;
			bool result;
			try
			{
				this.OpenResources();
				Command.IcsFolderCheckResults icsFolderCheckResults = base.PerformICSFolderHierarchyChangeCheck(ref this.folderIdMappingState, out latestState);
				if (icsFolderCheckResults == Command.IcsFolderCheckResults.NoChanges)
				{
					result = flag2;
				}
				else if (icsFolderCheckResults == Command.IcsFolderCheckResults.ChangesNoDeepCheck)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.FolderHierarchyChangedSinceLastSync:QuickCheck:true");
					flag2 = true;
					result = flag2;
				}
				else
				{
					if (this.rootState == null)
					{
						flag = true;
						this.rootState = base.SyncStateStorage.GetFolderHierarchySyncState();
						if (this.rootState == null)
						{
							base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.RootStateMissing2");
							AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.FolderHierarchyChangedSinceLastSync:1:true");
							flag2 = true;
							return flag2;
						}
					}
					FolderHierarchySync folderHierarchySync = this.rootState.GetFolderHierarchySync(new ChangeTrackingDelegate(FolderCommand.ComputeChangeTrackingHash));
					if (folderHierarchySync == null)
					{
						base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Ping.FolderHierarchySyncMissing");
						AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.FolderHierarchyChangedSinceLastSync:2:true");
						flag2 = true;
						result = flag2;
					}
					else
					{
						folderHierarchySync.AcknowledgeServerOperations();
						HierarchySyncOperations hierarchySyncOperations = folderHierarchySync.EnumerateServerOperations(base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root), false);
						if (hierarchySyncOperations != null && hierarchySyncOperations.Count > 0 && FolderCommand.FolderSyncRequired(base.SyncStateStorage, hierarchySyncOperations, this.folderIdMappingState))
						{
							AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.FolderHierarchyChangedSinceLastSync:3:true");
							flag2 = true;
							result = flag2;
						}
						else
						{
							base.SaveLatestIcsFolderHierarchySnapshot(latestState);
							AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.FolderHierarchyChangedSinceLastSync:false");
							result = flag2;
						}
					}
				}
			}
			finally
			{
				if (flag2 && setResponseOnChange)
				{
					base.XmlResponse = PingCommand.FolderSyncRequiredXml;
				}
				if (flag && this.rootState != null)
				{
					this.rootState.Dispose();
					this.rootState = null;
				}
			}
			return result;
		}

		private bool FolderChangedSinceLastSync(PingCommand.DPFolderInfo folder, GetItemEstimateCommand command, SyncCollection collection, bool tryNullSync, bool enumerateAllChanges)
		{
			AirSyncDiagnostics.TraceInfo<string, bool, bool>(ExTraceGlobals.RequestsTracer, this, "Ping.FolderChangedSinceLastSync collectionId:{0} tryNullSync:{1} enumerateAllChanges:{2}", collection.CollectionId, tryNullSync, enumerateAllChanges);
			if (!base.IsInQuarantinedState)
			{
				bool result = false;
				try
				{
					this.OpenResources();
					command.BorrowSecurityContextAndSession(this);
					collection.StoreSession = base.MailboxSession;
					collection.CreateSyncProvider();
					if (AirSyncUtility.IsVirtualFolder(collection) && this.isDirectPushAllowed)
					{
						this.CreateSubscriptionForVirtualFolder(collection);
					}
					command.GetChanges(collection, true, tryNullSync, true, enumerateAllChanges);
					if (collection.ServerChanges.Count > 0)
					{
						AirSyncDiagnostics.TraceDebug<int, string, StoreObjectId>(ExTraceGlobals.RequestsTracer, this, "Ping: {0} changes have occurred since the last sync: {1} {2}", collection.ServerChanges.Count, folder.ShortId, folder.StoreId);
						result = true;
						this.changesExist = true;
						folder.HasChanges = true;
					}
				}
				catch (AirSyncPermanentException ex)
				{
					if (string.IsNullOrEmpty(ex.ErrorStringForProtocolLogger))
					{
						ex.ErrorStringForProtocolLogger = "Ping.FCSLS:" + ex.Message;
					}
					base.PartialFailure = true;
					if (SyncBase.ErrorCodeStatus.InvalidSyncKey != collection.Status)
					{
						AirSyncDiagnostics.TraceDebug<AirSyncPermanentException, SyncBase.ErrorCodeStatus>(ExTraceGlobals.RequestsTracer, this, "Ping: GIE.GetChanges() failed! {0} {1}", ex, collection.Status);
						if (base.MailboxLogger != null)
						{
							base.MailboxLogger.SetData(MailboxLogDataName.PingCommand__ItemChangesSinceLastSync_Exception, ex);
						}
						base.XmlResponse = PingCommand.ServerErrorXml;
						return false;
					}
					AirSyncDiagnostics.TraceDebug<AirSyncPermanentException, SyncBase.ErrorCodeStatus>(ExTraceGlobals.RequestsTracer, this, "Ping: GIE.GetChanges() failed with InvalidSyncKey! {0} {1}", ex, collection.Status);
					result = true;
					this.changesExist = true;
					folder.HasChanges = true;
				}
				finally
				{
					if (collection.SyncState != null)
					{
						if (!collection.SyncState.IsColdStateDeserialized())
						{
							base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.SyncStateKbLeftCompressed, (int)collection.SyncState.GetColdStateCompressedSize() >> 10);
							AirSyncCounters.SyncStateKbLeftCompressed.IncrementBy(collection.SyncState.GetColdStateCompressedSize() >> 10);
						}
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.SyncStateKb, (int)collection.SyncState.GetTotalCompressedSize() >> 10);
						AirSyncCounters.SyncStateKbTotal.IncrementBy(collection.SyncState.GetTotalCompressedSize() >> 10);
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.TotalSaveCount, collection.SyncState.TotalSaveCount);
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.ColdSaveCount, collection.SyncState.ColdSaveCount);
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.ColdCopyCount, collection.SyncState.ColdCopyCount);
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.TotalLoadCount, collection.SyncState.TotalLoadCount);
					}
					collection.Dispose();
				}
				return result;
			}
			if (base.IsQuarantineMailAvailable && base.IsInboxFolder(folder.StoreId))
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "FolderChangedSinceLastSync returning true due to quarantine mail available");
				return true;
			}
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "FolderChangedSinceLastSync returning false due to quarantine");
			return false;
		}

		private bool ItemsChangedSinceLastSync()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.ItemsChangedSinceLastSync");
			if (this.FolderHierarchyChangedSinceLastSync(true))
			{
				return false;
			}
			if (base.IsInQuarantinedState && !base.IsQuarantineMailAvailable)
			{
				AirSyncDiagnostics.TraceInfo<DeviceAccessState>(ExTraceGlobals.RequestsTracer, this, "PingCommand: Quarantined state, return no item level changes in ItemChangedSinceLastSync. Current AccessState = {0}", base.CurrentAccessState);
				return false;
			}
			bool flag = false;
			using (GetItemEstimateCommand getItemEstimateCommand = new GetItemEstimateCommand())
			{
				getItemEstimateCommand.Context = base.Context;
				PingCommand.PingInformationForNotifier pingInformationForNotifier = (PingCommand.PingInformationForNotifier)this.Notifier.Information;
				getItemEstimateCommand.DeviceEnableOutboundSMS = pingInformationForNotifier.DeviceEnableOutboundSMS;
				getItemEstimateCommand.DevicePhoneNumberForSms = pingInformationForNotifier.DevicePhoneNumberForSms;
				foreach (PingCommand.DPFolderInfo dpfolderInfo in pingInformationForNotifier.DictFoldersByShortId.Values)
				{
					SyncCollection syncCollection = SyncCollection.CreateSyncCollection(base.MailboxSession, base.Version, dpfolderInfo.ShortId);
					syncCollection.CollectionId = dpfolderInfo.ShortId;
					syncCollection.ClassType = dpfolderInfo.Class;
					syncCollection.WindowSize = 1;
					getItemEstimateCommand.Collections.Add(dpfolderInfo.ShortId, syncCollection);
				}
				foreach (PingCommand.DPFolderInfo dpfolderInfo2 in pingInformationForNotifier.DictFoldersByShortId.Values)
				{
					SyncCollection syncCollection2 = getItemEstimateCommand.Collections[dpfolderInfo2.ShortId];
					flag |= this.FolderChangedSinceLastSync(dpfolderInfo2, getItemEstimateCommand, syncCollection2, true, false);
					if (base.XmlResponse != null)
					{
						return false;
					}
					if (base.Context.DeviceBehavior != null)
					{
						base.Context.DeviceBehavior.AddSyncKey(this.pingAttemptTime, syncCollection2.CollectionId, syncCollection2.SyncKey);
					}
				}
			}
			return flag;
		}

		private XmlDocument ComposeChangesResponse()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.ComposeChangesResponse");
			StringBuilder stringBuilder = new StringBuilder(300);
			stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			stringBuilder.Append("<Ping xmlns=\"Ping:\"><Status>");
			stringBuilder.Append("2");
			stringBuilder.Append("</Status><Folders>");
			PingCommand.PingInformationForNotifier pingInformationForNotifier = (PingCommand.PingInformationForNotifier)this.Notifier.Information;
			foreach (PingCommand.DPFolderInfo dpfolderInfo in pingInformationForNotifier.DictFoldersByShortId.Values)
			{
				if (dpfolderInfo.HasChanges)
				{
					stringBuilder.Append("<Folder>").Append(dpfolderInfo.ShortId).Append("</Folder>");
					base.Context.ProtocolLogger.SetValue(dpfolderInfo.ShortId, PerFolderProtocolLoggerData.ServerChanges, 1);
				}
			}
			stringBuilder.Append("</Folders></Ping>");
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.LoadXml(stringBuilder.ToString());
			return xmlDocument;
		}

		private void CompleteRequest(XmlDocument xmlresponse, bool keepNotificationsGoing)
		{
			this.CompleteRequest(xmlresponse, null, keepNotificationsGoing, false);
		}

		private void CompleteRequest(XmlDocument xmlresponse, Exception ex, bool keepNotificationsGoing, bool timedOut)
		{
			AirSyncDiagnostics.TraceInfo<Exception, bool, bool>(ExTraceGlobals.RequestsTracer, this, "Ping.CompleteRequest ex:{0}, keepNotificationsGoing:{1} timedOut:{2}", ex, keepNotificationsGoing, timedOut);
			this.CleanupNotificationManager(keepNotificationsGoing);
			if (this.requestCompleted)
			{
				return;
			}
			Interlocked.Decrement(ref PingCommand.numberOfOutstandingPings);
			AirSyncCounters.CurrentlyPendingPing.RawValue = (long)PingCommand.numberOfOutstandingPings;
			if (!base.Context.Response.IsClientConnected)
			{
				AirSyncCounters.NumberOfDroppedPing.Increment();
			}
			if (base.Context.Response.HttpStatusCode == HttpStatusCode.OK)
			{
				if (xmlresponse != null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.StatusCode, int.Parse(xmlresponse["Ping"]["Status"].InnerText, CultureInfo.InvariantCulture));
					base.XmlResponse = xmlresponse;
					base.Context.Response.IssueWbXmlResponse();
				}
				else if (string.IsNullOrEmpty(base.Context.Response.ContentType) || string.Equals("text/html", base.Context.Response.ContentType, StringComparison.OrdinalIgnoreCase))
				{
					base.Context.Response.ContentType = "application/vnd.ms-sync.wbxml";
				}
			}
			try
			{
				if (base.MailboxLoggingEnabled)
				{
					this.OpenResources();
					if (ex != null)
					{
						base.MailboxLogger.SetData(MailboxLogDataName.PingCommand_Consume_Exception, ex);
					}
					base.MailboxLogger.SetData(MailboxLogDataName.WasPending, "[Response was pending]");
					base.MailboxLogger.LogResponseHead(base.Context.Response);
					base.MailboxLogger.SetData(MailboxLogDataName.ResponseBody, (base.XmlResponse == null) ? "[No XmlResponse]" : AirSyncUtility.BuildOuterXml(base.XmlResponse, !GlobalSettings.EnableMailboxLoggingVerboseMode));
					base.MailboxLogger.SetData(MailboxLogDataName.ResponseTime, ExDateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo));
					base.MailboxLogger.AppendToLogInMailbox();
				}
				base.CommitSyncStatusSyncState();
			}
			catch (LocalizedException arg)
			{
				AirSyncDiagnostics.TraceError<LocalizedException>(ExTraceGlobals.RequestsTracer, this, "Ping: CompleteRequest() failed to log response to the mailbox. Ex: {0}", arg);
			}
			finally
			{
				if (timedOut)
				{
					base.Context.Response.TimeToRespond = base.Context.RequestTime.AddSeconds(this.Notifier.RequestedWaitTime);
				}
				else if (base.Context.Response.XmlDocument != null)
				{
					XmlNode xmlNode = base.Context.Response.XmlDocument["Status"];
					if (xmlNode != null && xmlNode.InnerText == "1")
					{
						uint num = this.Notifier.RequestedWaitTime / 2U;
						base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.EmptyResponseDelayed, num);
						AirSyncDiagnostics.TraceError<uint>(ExTraceGlobals.RequestsTracer, this, "Empty ping response delayed for {0} seconds", num);
						base.Context.Response.TimeToRespond = base.Context.RequestTime.AddSeconds(num);
					}
					else
					{
						base.Context.Response.TimeToRespond = ExDateTime.UtcNow;
					}
				}
				else
				{
					base.Context.Response.TimeToRespond = ExDateTime.UtcNow;
				}
				this.CompleteHttpRequest();
			}
		}

		private void CleanupNotificationManager(bool keepNotificationsGoing)
		{
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "Ping.CleanupNotificationManager keepNotificationsGoing:{0}", keepNotificationsGoing);
			if (this.Notifier != null)
			{
				if (keepNotificationsGoing)
				{
					this.Notifier.ReleaseCommand(this);
					return;
				}
				this.Notifier.EnqueueDispose();
			}
		}

		private void OpenResources()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Ping.OpenResources");
			base.OpenMailboxSession(base.User);
			base.OpenSyncStorage(base.Context.User.Features.IsEnabled(EasFeature.SyncStatusOnGlobalInfo));
		}

		INotificationManagerContext IAsyncCommand.Context
		{
			get
			{
				return base.Context;
			}
		}

		private static XmlDocument nothingChangedXml;

		private static XmlDocument validationErrorXml;

		private static XmlDocument foldersOutOfRangeXml;

		private static XmlDocument folderSyncRequiredXml;

		private static XmlDocument hbiTooLowXml;

		private static XmlDocument hbiTooHighXml;

		private static XmlDocument sendParametersXml;

		private static XmlDocument serverErrorXml;

		private static int numberOfOutstandingPings;

		private FolderHierarchySyncState rootState;

		private SyncState folderIdMappingState;

		private bool changesExist;

		private bool requestCompleted;

		private ExDateTime pingAttemptTime = ExDateTime.UtcNow;

		private bool isDirectPushAllowed;

		private bool isDirectPushAllowedByGeo;

		private struct Status
		{
			public const string NoChanges = "1";

			public const string Changes = "2";

			public const string SendParameters = "3";

			public const string Protocol = "4";

			public const string HbiOutOfRange = "5";

			public const string FoldersOutOfRange = "6";

			public const string FolderSyncRequired = "7";

			public const string ServerError = "8";
		}

		internal class PingHbiMonitor : NotificationManager.HbiMonitor
		{
			protected PingHbiMonitor()
			{
			}

			public static PingCommand.PingHbiMonitor Instance
			{
				get
				{
					return PingCommand.PingHbiMonitor.instance;
				}
			}

			private static PingCommand.PingHbiMonitor instance = new PingCommand.PingHbiMonitor();
		}

		internal sealed class DPFolderInfo : ICustomSerializableBuilder, ICustomSerializable
		{
			public string ShortId
			{
				get
				{
					return this.shortId;
				}
				set
				{
					this.shortId = value;
				}
			}

			public string Class
			{
				get
				{
					return this.classString;
				}
				set
				{
					this.classString = value;
				}
			}

			public StoreObjectId StoreId
			{
				get
				{
					return this.storeId;
				}
				set
				{
					this.storeId = value;
				}
			}

			public bool HasChanges
			{
				get
				{
					return this.hasChanges;
				}
				set
				{
					this.hasChanges = value;
				}
			}

			public ushort TypeId
			{
				get
				{
					return PingCommand.DPFolderInfo.typeId;
				}
				set
				{
					PingCommand.DPFolderInfo.typeId = value;
				}
			}

			public override bool Equals(object obj)
			{
				PingCommand.DPFolderInfo dpfolderInfo = obj as PingCommand.DPFolderInfo;
				return dpfolderInfo != null && dpfolderInfo.ShortId == this.ShortId && dpfolderInfo.Class == this.Class && dpfolderInfo.ShortId.Equals(this.ShortId);
			}

			public override int GetHashCode()
			{
				return this.ShortId.GetHashCode() ^ this.Class.GetHashCode() ^ ((this.StoreId == null) ? 0 : this.StoreId.GetHashCode());
			}

			public void SerializeData(BinaryWriter writer, ComponentDataPool componentDataPool)
			{
				componentDataPool.GetStringDataInstance().Bind(this.ShortId).SerializeData(writer, componentDataPool);
				componentDataPool.GetStringDataInstance().Bind(this.Class).SerializeData(writer, componentDataPool);
				componentDataPool.GetStoreObjectIdDataInstance().Bind(this.StoreId).SerializeData(writer, componentDataPool);
			}

			public void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
			{
				StringData stringDataInstance = componentDataPool.GetStringDataInstance();
				stringDataInstance.DeserializeData(reader, componentDataPool);
				this.ShortId = stringDataInstance.Data;
				stringDataInstance.DeserializeData(reader, componentDataPool);
				this.Class = stringDataInstance.Data;
				StoreObjectIdData storeObjectIdDataInstance = componentDataPool.GetStoreObjectIdDataInstance();
				storeObjectIdDataInstance.DeserializeData(reader, componentDataPool);
				this.StoreId = storeObjectIdDataInstance.Data;
			}

			public ICustomSerializable BuildObject()
			{
				return new PingCommand.DPFolderInfo();
			}

			private static ushort typeId;

			private string shortId;

			private string classString;

			private StoreObjectId storeId;

			[NonSerialized]
			private bool hasChanges;
		}

		private class PingInformationForNotifier
		{
			public PingInformationForNotifier(string devicePhoneNumberForSms, bool deviceEnableOutboundSMS)
			{
				this.DevicePhoneNumberForSms = devicePhoneNumberForSms;
				this.DeviceEnableOutboundSMS = deviceEnableOutboundSMS;
			}

			public string DevicePhoneNumberForSms { get; set; }

			public bool DeviceEnableOutboundSMS { get; set; }

			public Dictionary<string, PingCommand.DPFolderInfo> DictFoldersByShortId { get; set; }

			public Dictionary<StoreObjectId, PingCommand.DPFolderInfo> DictFoldersByStoreId { get; set; }

			public StoreObjectId SyncStateStorageStoreObjectId { get; set; }

			public StoreObjectId ExchangeSyncDataStoreObjectId { get; set; }

			public HashSet<StoreObjectId> HangingVirtualFolderIds { get; set; }

			public bool AddToHangingVirtualFolderIds(StoreObjectId storeObjectId)
			{
				if (this.HangingVirtualFolderIds == null)
				{
					this.HangingVirtualFolderIds = new HashSet<StoreObjectId>();
				}
				return this.HangingVirtualFolderIds.Add(storeObjectId);
			}
		}
	}
}
