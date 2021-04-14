using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Search.Performance;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class NotificationsFeeder : Executable, IFeeder, IExecutable, IDiagnosable, IDisposable
	{
		public NotificationsFeeder(MdbPerfCountersInstance mdbFeedingPerfCounters, MdbInfo mdbInfo, ISearchServiceConfig config, ISubmitDocument indexFeeder, IWatermarkStorage watermarkStorage, IIndexStatusStore indexStatusStore) : base(config)
		{
			Util.ThrowOnNullArgument(mdbFeedingPerfCounters, "mdbFeedingPerfCounters");
			Util.ThrowOnNullArgument(mdbInfo, "mdbInfo");
			Util.ThrowOnNullArgument(config, "config");
			Util.ThrowOnNullArgument(indexFeeder, "indexFeeder");
			Util.ThrowOnNullArgument(watermarkStorage, "watermarkStorage");
			Util.ThrowOnNullArgument(indexStatusStore, "indexStatusStore");
			this.pollingActivityId = Guid.NewGuid();
			base.DiagnosticsSession.ComponentName = "NotificationsFeeder";
			base.DiagnosticsSession.Tracer = ExTraceGlobals.MdbNotificationsFeederTracer;
			this.mdbInfo = mdbInfo;
			base.InstanceName = mdbInfo.Name;
			this.indexFeeder = indexFeeder;
			this.watermarkStorage = watermarkStorage;
			this.indexStatusStore = indexStatusStore;
			this.perfCounterInstance = mdbFeedingPerfCounters;
			this.notificationsEventSource = Factory.Current.CreateNotificationsEventSource(this.mdbInfo);
			this.notificationQueue = new NotificationsQueue(base.Config.MaxNotificationQueueSize, base.Config.QueueSize, this.perfCounterInstance.NotificationsStallTime);
			this.watermarkManager = new NotificationsWatermarkManager(base.Config.QueueSize);
			this.exceptionOccurred = Strings.ExceptionOccurred(this.mdbInfo.Guid);
		}

		public FeederType FeederType
		{
			get
			{
				return FeederType.Notifications;
			}
		}

		internal Guid? SystemMailboxGuid { get; set; }

		internal Guid? SystemAttendantGuid { get; set; }

		internal long HighWatermark { get; private set; }

		internal long LowWatermark { get; private set; }

		internal long LastWatermarkWritten { get; private set; }

		internal long LastWatermarkInDatabase { get; private set; }

		internal int OutstandingLength
		{
			get
			{
				return this.notificationQueue.OutstandingLength;
			}
		}

		public override XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(parameters);
			diagnosticInfo.Add(new XElement("LowWatermark", this.LowWatermark));
			diagnosticInfo.Add(new XElement("HighWatermark", this.HighWatermark));
			diagnosticInfo.Add(new XElement("LastEvent", this.LastWatermarkInDatabase));
			diagnosticInfo.Add(new XElement("NotificationLastPollTime", this.lastSuccessfulPollTime));
			diagnosticInfo.Add(new XElement("AgeOfLastNotificationProcessed", this.ageOfLastNotificationProcessed));
			return diagnosticInfo;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationsFeeder>(this);
		}

		internal NotificationData CreateNotificationData(MapiEvent mapiEvent)
		{
			NotificationData notificationData = new NotificationData(mapiEvent);
			ObjectType itemType = mapiEvent.ItemType;
			StoreObjectId itemId;
			if (itemType != ObjectType.MAPI_STORE)
			{
				if (itemType == ObjectType.MAPI_MESSAGE)
				{
					this.UpdateNotificationDataForMessageEvent(notificationData);
					itemId = StoreObjectId.FromProviderSpecificId(mapiEvent.ItemEntryId, ObjectClass.GetObjectType(mapiEvent.ObjectClass));
				}
				else
				{
					notificationData.Type = NotificationType.Uninteresting;
					itemId = StoreObjectId.DummyId;
				}
			}
			else
			{
				this.UpdateNotificationDataForStoreEvent(notificationData);
				itemId = StoreObjectId.DummyId;
			}
			notificationData.Identity = new MdbItemIdentity(mapiEvent.TenantHint, this.mdbInfo.Guid, mapiEvent.MailboxGuid, mapiEvent.MailboxNumber, itemId, mapiEvent.DocumentId, (mapiEvent.ExtendedEventFlags & MapiExtendedEventFlags.PublicFolderMailbox) == MapiExtendedEventFlags.PublicFolderMailbox);
			return notificationData;
		}

		internal void PollForEvents()
		{
			this.timeUntilNextPoll = base.Config.NotificationsPollInterval;
			this.UpdateWatermark(false);
			int maxNotificationsToReadPerRpc = base.Config.MaxNotificationsToReadPerRpc;
			int num = base.Config.MaxNotificationQueueSize - this.notificationQueue.Length;
			if (num < maxNotificationsToReadPerRpc)
			{
				base.DiagnosticsSession.TraceDebug<int, int>("Not enough space (free:{0}) in the queue for {1} more events.", num, maxNotificationsToReadPerRpc);
				this.SchedulePoll();
				return;
			}
			base.DiagnosticsSession.TraceDebug<long, int>("Polling for new events. High watermark: {0}, requested: {1}", this.HighWatermark, maxNotificationsToReadPerRpc);
			long endCounter;
			MapiEvent[] array;
			try
			{
				array = this.notificationsEventSource.ReadEvents(this.HighWatermark + 1L, maxNotificationsToReadPerRpc, ReadEventsFlags.FailIfEventsDeleted | ReadEventsFlags.IncludeMoveDestinationEvents, out endCounter);
			}
			catch (ObjectDisposedException result)
			{
				base.DiagnosticsSession.TraceDebug("Object disposed exception, calling CompleteExecute on NotificationsFeeder", new object[0]);
				base.CompleteExecute(result);
				return;
			}
			this.lastSuccessfulPollTime = DateTime.UtcNow;
			base.DiagnosticsSession.SetCounterRawValue(this.perfCounterInstance.LastSuccessfulPollTimestamp, this.lastSuccessfulPollTime.Ticks);
			base.DiagnosticsSession.TraceDebug<int, long>("Events read: {0}, last watermark: {1}", array.Length, endCounter);
			if (array.Length != 0)
			{
				if (array.Length == maxNotificationsToReadPerRpc)
				{
					Exception objectDiposedExceptionInsideHandler = null;
					if (base.TryRunUnderExceptionHandler(delegate()
					{
						try
						{
							MapiEvent mapiEvent2 = this.notificationsEventSource.ReadLastEvent();
							this.DiagnosticsSession.TraceDebug<long>("Last event: {0}", mapiEvent2.EventCounter);
							endCounter = mapiEvent2.EventCounter;
						}
						catch (ObjectDisposedException objectDiposedExceptionInsideHandler)
						{
							objectDiposedExceptionInsideHandler = objectDiposedExceptionInsideHandler;
						}
					}, this.exceptionOccurred))
					{
						if (objectDiposedExceptionInsideHandler != null)
						{
							base.DiagnosticsSession.TraceDebug("Object disposed exception, calling CompleteExecute on NotificationsFeeder", new object[0]);
							base.CompleteExecute(objectDiposedExceptionInsideHandler);
							return;
						}
						this.LastWatermarkInDatabase = endCounter;
						base.DiagnosticsSession.TraceDebug<long>("Last event in database: {0}", this.LastWatermarkInDatabase);
					}
					this.timeUntilNextPoll = TimeSpan.Zero;
				}
				else
				{
					this.LastWatermarkInDatabase = endCounter;
				}
				this.HighWatermark = array[array.Length - 1].EventCounter;
				foreach (MapiEvent mapiEvent in array)
				{
					if (base.Stopping)
					{
						return;
					}
					base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfNotifications);
					NotificationData notificationData = this.CreateNotificationData(mapiEvent);
					if (base.DiagnosticsSession.IsTraceEnabled(TraceType.DebugTrace))
					{
						base.DiagnosticsSession.TraceDebug("Event: {0} {1} {2},{3} {4}, Interesting: {5}", new object[]
						{
							mapiEvent.EventCounter,
							mapiEvent.EventMask,
							mapiEvent.MailboxGuid,
							mapiEvent.DocumentId,
							mapiEvent.ObjectClass ?? "<null>",
							notificationData.Type
						});
					}
					this.notificationQueue.Enqueue(notificationData);
				}
				try
				{
					this.SendDocuments("PollForEvents");
				}
				finally
				{
					this.SchedulePoll();
				}
				return;
			}
			this.HighWatermark = endCounter - 1L;
			this.LastWatermarkInDatabase = endCounter - 1L;
			this.SchedulePoll();
		}

		protected override void InternalExecutionStart()
		{
			try
			{
				using (IDisposable disposable = this.activeThreadCount.AcquireReference())
				{
					if (disposable == null)
					{
						return;
					}
					MapiEvent mapiEvent;
					try
					{
						mapiEvent = this.notificationsEventSource.ReadLastEvent();
					}
					catch (ObjectDisposedException result)
					{
						base.DiagnosticsSession.TraceDebug("Object disposed exception, calling CompleteExecute on NotificationsFeeder", new object[0]);
						base.CompleteExecute(result);
						return;
					}
					this.LastWatermarkInDatabase = mapiEvent.EventCounter;
					long notificationsWatermark = this.watermarkStorage.GetNotificationsWatermark();
					base.DiagnosticsSession.Assert(notificationsWatermark >= 0L, "Notifications watermark missing.", new object[0]);
					base.DiagnosticsSession.TraceDebug<long, long>("PrepareToStart: Current watermark: {0}, Last event in database: {1}.  Low watermark set to current.", notificationsWatermark, this.LastWatermarkInDatabase);
					this.LowWatermark = notificationsWatermark;
					this.HighWatermark = notificationsWatermark;
					this.LastWatermarkWritten = notificationsWatermark;
					this.nextWatermarkUpdate = DateTime.UtcNow + base.Config.WatermarkUpdateFrequency;
					if (this.mdbInfo.Guid != Guid.Empty)
					{
						this.SystemMailboxGuid = new Guid?(this.mdbInfo.SystemMailboxGuid);
						this.SystemAttendantGuid = this.mdbInfo.SystemAttendantGuid;
					}
				}
			}
			catch (ComponentException result2)
			{
				base.CompleteExecute(result2);
				return;
			}
			this.timeUntilNextPoll = TimeSpan.Zero;
			this.SchedulePoll();
		}

		protected override void InternalExecutionFinish()
		{
			this.activeThreadCount.DisableAddRef();
		}

		protected override void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (!this.activeThreadCount.TryWaitForZero(base.Config.MaxOperationTimeout))
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, "Waiting for threads to stop: Did not shut down in a timely manner.", new object[0]);
				}
				if (this.notificationsEventSource != null)
				{
					this.notificationsEventSource.Dispose();
				}
			}
			base.Dispose(calledFromDispose);
		}

		private static bool IsSearchFolderEvent(MapiEvent notification)
		{
			if ((notification.EventFlags & MapiEventFlags.SearchFolder) != MapiEventFlags.None)
			{
				return true;
			}
			byte[] parentEntryId = notification.ParentEntryId;
			byte[] itemEntryId = notification.ItemEntryId;
			for (int i = 22; i < 46; i++)
			{
				if (parentEntryId[i] != itemEntryId[i])
				{
					return true;
				}
			}
			return false;
		}

		private void SchedulePoll()
		{
			base.DiagnosticsSession.TraceDebug<double>("SchedulePoll in {0} milliseconds", this.timeUntilNextPoll.TotalMilliseconds);
			RegisteredWaitHandleWrapper.RegisterWaitForSingleObject(base.StopEvent, CallbackWrapper.WaitOrTimerCallback(new WaitOrTimerCallback(this.ProcessingProcedure)), null, this.timeUntilNextPoll, true);
		}

		private void ProcessingProcedure(object state, bool timerFired)
		{
			this.RunUnderExceptionHandler(delegate
			{
				using (ExPerfTrace.RelatedActivity(this.pollingActivityId))
				{
					using (IDisposable disposable = this.activeThreadCount.AcquireReference())
					{
						if (disposable != null)
						{
							try
							{
								if (timerFired)
								{
									this.DiagnosticsSession.TraceDebug<MdbInfo>("ProcessingProcedure: Polling {0}", this.mdbInfo);
									this.PollForEvents();
								}
								else
								{
									this.DiagnosticsSession.TraceDebug("ProcessingProcedure: Stop event has been signaled. Shutting down", new object[0]);
									this.UpdateWatermark(true);
								}
								goto IL_CB;
							}
							catch (ObjectDisposedException result)
							{
								this.DiagnosticsSession.TraceError("ProcessingProcedure: Disposed while ProcessingProcedure was in progress.", new object[0]);
								this.CompleteExecute(result);
								goto IL_CB;
							}
						}
						this.DiagnosticsSession.TraceDebug("ProcessingProcedure: Failed to acquire reference. Shutting down", new object[0]);
						IL_CB:;
					}
				}
			});
		}

		private void RunUnderExceptionHandler(Action action)
		{
			base.TryRunUnderExceptionHandler(action, this.exceptionOccurred);
		}

		private void UpdateWatermark(bool forceUpdate)
		{
			base.DiagnosticsSession.TraceDebug<bool>("UpdateWatermark, forceUpdate={0}", forceUpdate);
			long lowWatermark = this.LowWatermark;
			if (lowWatermark == this.LastWatermarkWritten)
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (forceUpdate || utcNow > this.nextWatermarkUpdate)
			{
				this.watermarkStorage.BeginSetNotificationsWatermark(lowWatermark, new AsyncCallback(this.FinishSetNotificationsWatermark), null);
				this.LastWatermarkWritten = lowWatermark;
				this.nextWatermarkUpdate = utcNow + base.Config.WatermarkUpdateFrequency;
			}
		}

		private void FinishSetNotificationsWatermark(IAsyncResult ar)
		{
			base.TryRunUnderExceptionHandler(delegate()
			{
				this.watermarkStorage.EndSetNotificationsWatermark(ar);
			}, NotificationsFeeder.ErrorAccessingStateStorageTag);
		}

		private void SendDocuments(object state)
		{
			base.DiagnosticsSession.TraceDebug("SendDocuments from state '{0}'", new object[]
			{
				state
			});
			long lowWatermark = this.LowWatermark;
			DateTime utcNow = DateTime.UtcNow;
			DateTime t = utcNow - base.Config.FreshnessInterval;
			base.DiagnosticsSession.SetCounterRawValue(this.perfCounterInstance.NumberOfItemsInNotificationQueue, (long)this.notificationQueue.Length);
			base.DiagnosticsSession.SetCounterRawValue(this.perfCounterInstance.NumberOfNotificationsNotYetProcessed, this.LastWatermarkInDatabase - lowWatermark);
			bool flag = false;
			using (IDisposable disposable = this.activeThreadCount.AcquireReference())
			{
				if (disposable != null)
				{
					lock (this.notificationQueue)
					{
						Interlocked.Exchange(ref this.flagPendingSendDocuments, 0);
						IEnumerable<NotificationData> enumerable;
						if (this.notificationQueue.Dequeue(out enumerable))
						{
							base.DiagnosticsSession.SetCounterRawValue(this.perfCounterInstance.NumberOfItemsInNotificationQueue, (long)this.notificationQueue.Length);
							foreach (NotificationData notificationData in enumerable)
							{
								base.DiagnosticsSession.TraceDebug<NotificationData>("SendDocument: Process Notification: {0}", notificationData);
								MapiEvent mapiEvent = notificationData.MapiEvent;
								long value = Math.Max(0L, (long)(utcNow - mapiEvent.CreateTime).TotalSeconds);
								base.DiagnosticsSession.SetCounterRawValue(this.perfCounterInstance.AgeOfLastNotificationProcessed, value);
								this.ageOfLastNotificationProcessed = value;
								this.indexStatusStore.UpdateIndexStatus(this.mdbInfo.Guid, IndexStatusIndex.AgeOfLastNotificationProcessed, this.ageOfLastNotificationProcessed);
								this.watermarkManager.Add(notificationData.MapiEvent.EventCounter, notificationData.Type != NotificationType.Uninteresting);
								if (notificationData.Type != NotificationType.Uninteresting)
								{
									base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfDocumentsSentForProcessingNotifications);
									this.UpdateClientTypePerformanceCounters(notificationData.MapiEvent);
								}
								IFastDocument fastDocument = null;
								switch (notificationData.Type)
								{
								case NotificationType.Uninteresting:
									base.DiagnosticsSession.TraceDebug<NotificationData>("SendDocument:  We do not send non-interesting events, so remove it from outstanding set immediately. Notification: {0}", notificationData);
									this.notificationQueue.Remove(notificationData);
									this.CompleteNotification(notificationData, true);
									flag = true;
									break;
								case NotificationType.Insert:
								case NotificationType.Update:
									fastDocument = this.indexFeeder.CreateFastDocument(notificationData.Operation);
									if (mapiEvent.CreateTime < t)
									{
										this.indexFeeder.DocumentHelper.PopulateFastDocumentForIndexing(fastDocument, this.mdbInfo.CatalogVersion.FeedingVersion, notificationData.Identity.MailboxGuid, notificationData.Identity.MailboxNumber, notificationData.IsMoveDestination, !this.mdbInfo.IsLagCopy, notificationData.Identity.DocumentId, notificationData.Identity, 3, 0);
										base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfBackloggedItemsAddedToRetryTable);
									}
									else
									{
										this.indexFeeder.DocumentHelper.PopulateFastDocumentForIndexing(fastDocument, this.mdbInfo.CatalogVersion.FeedingVersion, notificationData.Identity.MailboxGuid, notificationData.Identity.MailboxNumber, notificationData.IsMoveDestination, !this.mdbInfo.IsLagCopy, notificationData.Identity.DocumentId, notificationData.Identity);
									}
									break;
								case NotificationType.Delete:
									fastDocument = this.indexFeeder.CreateFastDocument(notificationData.Operation);
									this.indexFeeder.DocumentHelper.PopulateFastDocumentForDelete(fastDocument, notificationData.Identity.MailboxGuid, IndexId.CreateIndexId(notificationData.Identity.MailboxNumber, notificationData.Identity.DocumentId));
									break;
								case NotificationType.Move:
								case NotificationType.ReadFlagChange:
									fastDocument = this.indexFeeder.CreateFastDocument(notificationData.Operation);
									this.indexFeeder.DocumentHelper.PopulateFastDocumentForFolderUpdate(fastDocument, notificationData.Identity.MailboxGuid, notificationData.Identity.MailboxNumber, notificationData.IsMoveDestination, !this.mdbInfo.IsLagCopy, notificationData.Identity.DocumentId, notificationData.Identity, FolderIdHelper.GetIndexForFolderEntryId(notificationData.MapiEvent.ParentEntryId));
									break;
								case NotificationType.DeleteMailbox:
									fastDocument = this.indexFeeder.CreateFastDocument(notificationData.Operation);
									this.indexFeeder.DocumentHelper.PopulateFastDocumentForDeleteSelection(fastDocument, notificationData.Identity.MailboxGuid);
									break;
								default:
									throw new InvalidOperationException("Missing NotificationClassifcation." + notificationData.Type.ToString());
								}
								if (fastDocument != null)
								{
									try
									{
										this.indexFeeder.BeginSubmitDocument(fastDocument, new AsyncCallback(this.DocumentCompleteCallback), notificationData);
									}
									catch (ObjectDisposedException result)
									{
										base.DiagnosticsSession.TraceError("FastFeeder has been disposed", new object[0]);
										base.CompleteExecute(result);
										return;
									}
								}
							}
						}
					}
					if (flag && Interlocked.CompareExchange(ref this.flagPendingSendDocuments, 1, 0) == 0)
					{
						ThreadPool.QueueUserWorkItem(CallbackWrapper.WaitCallback(new WaitCallback(this.SendDocuments)), "SendDocuments");
					}
				}
			}
		}

		private void UpdateClientTypePerformanceCounters(MapiEvent mapievent)
		{
			MapiEventClientTypes clientType = mapievent.ClientType;
			switch (clientType)
			{
			case MapiEventClientTypes.Transport:
				base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.MessagesFromTransport);
				return;
			case MapiEventClientTypes.AirSync:
				break;
			case MapiEventClientTypes.EventBasedAssistants:
				base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.MessagesFromEventBasedAssistants);
				return;
			default:
				switch (clientType)
				{
				case MapiEventClientTypes.TimeBasedAssistants:
					base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.MessagesFromTimeBasedAssistants);
					return;
				case MapiEventClientTypes.MOMT:
					break;
				case MapiEventClientTypes.Migration:
					base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.MessagesFromMigration);
					break;
				default:
					return;
				}
				break;
			}
		}

		private void CompleteNotification(NotificationData notification, bool watermarkOnly)
		{
			base.DiagnosticsSession.TraceDebug<NotificationData, bool>("CompleteNotification. {0}, WatermarkOnly: {1}", notification, watermarkOnly);
			if (!watermarkOnly)
			{
				this.notificationQueue.Remove(notification);
				base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfDocumentsProcessed);
			}
			long num;
			if (this.watermarkManager.TryComplete(notification.MapiEvent.EventCounter, out num))
			{
				this.LowWatermark = num;
				base.DiagnosticsSession.TraceDebug<long>("CompleteNotification: Assign LowWatermark: {0}", num);
			}
		}

		private void DocumentCompleteCallback(IAsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug("DocumentCompleteCallback", new object[0]);
			NotificationData notificationData = (NotificationData)asyncResult.AsyncState;
			using (IDisposable disposable = this.activeThreadCount.AcquireReference())
			{
				try
				{
					if (!this.indexFeeder.EndSubmitDocument(asyncResult))
					{
						base.CompleteExecute(null);
						base.DiagnosticsSession.TraceDebug<NotificationData>("DocumentCompleteCallback: The document was canceled. This should only happen on shutdown! Just return from here to make sure we do not update the Watermarks. {0}", notificationData);
						return;
					}
					base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfDocumentsIndexedNotifications);
				}
				catch (DocumentValidationException ex)
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Document Validation Failure: NotificationData: {0}, Failure: {1}", new object[]
					{
						notificationData.ToMergeDebuggingString(),
						ex
					});
					base.CompleteExecute(ex);
					return;
				}
				catch (Exception ex2)
				{
					base.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, "Document failure: ID: {0}, Failure: {1}", new object[]
					{
						notificationData.MapiEvent,
						ex2
					});
					base.CompleteExecute(ex2);
					return;
				}
				if (disposable == null)
				{
					base.DiagnosticsSession.TraceDebug<NotificationData>("DocumentCompleteCallback: If we couldn't acquire the handle, we're done. No need to update the watermark manager. {0}", notificationData);
				}
				else
				{
					base.DiagnosticsSession.TraceDebug<NotificationData>("DocumentCompleteCallback: Document has been processed. {0}", notificationData);
					this.CompleteNotification(notificationData, false);
					if (Interlocked.CompareExchange(ref this.flagPendingSendDocuments, 1, 0) == 0)
					{
						ThreadPool.QueueUserWorkItem(CallbackWrapper.WaitCallback(new WaitCallback(this.SendDocuments)), "DocumentCompleteCallback");
					}
				}
			}
		}

		private void MailboxDeletedUpdateCallback(IAsyncResult asyncResult)
		{
			base.DiagnosticsSession.TraceDebug("MailboxDeletedUpdateCallback", new object[0]);
			NotificationData notificationData = (NotificationData)asyncResult.AsyncState;
			using (IDisposable disposable = this.activeThreadCount.AcquireReference())
			{
				if (base.TryRunUnderExceptionHandler(delegate()
				{
					this.watermarkStorage.EndSetMailboxDeletionPending(asyncResult);
				}, NotificationsFeeder.ErrorAccessingStateStorageTag))
				{
					if (disposable != null)
					{
						base.DiagnosticsSession.TraceDebug<NotificationData>("MailboxDeletedUpdateCallback: Document has been processed. {0}", notificationData);
						this.CompleteNotification(notificationData, false);
						if (Interlocked.CompareExchange(ref this.flagPendingSendDocuments, 1, 0) == 0)
						{
							ThreadPool.QueueUserWorkItem(CallbackWrapper.WaitCallback(new WaitCallback(this.SendDocuments)), "DocumentCompleteCallback");
						}
					}
				}
			}
		}

		private void UpdateNotificationDataForMessageEvent(NotificationData notificationData)
		{
			notificationData.Type = NotificationType.Uninteresting;
			MapiEvent mapiEvent = notificationData.MapiEvent;
			if ((mapiEvent.EventFlags & MapiEventFlags.FolderAssociated) != MapiEventFlags.None)
			{
				return;
			}
			if ((mapiEvent.EventMask & (MapiEventTypeFlags.ObjectCreated | MapiEventTypeFlags.ObjectDeleted | MapiEventTypeFlags.ObjectModified | MapiEventTypeFlags.ObjectMoved | MapiEventTypeFlags.ObjectCopied)) == (MapiEventTypeFlags)0)
			{
				return;
			}
			if (mapiEvent.MailboxGuid == Guid.Empty)
			{
				return;
			}
			if (mapiEvent.MailboxGuid == this.SystemMailboxGuid)
			{
				return;
			}
			if (mapiEvent.MailboxGuid == this.SystemAttendantGuid)
			{
				return;
			}
			if (NotificationsFeeder.IsSearchFolderEvent(mapiEvent))
			{
				return;
			}
			if (XsoUtil.ShouldSkipMessageClass(mapiEvent.ObjectClass))
			{
				return;
			}
			if (mapiEvent.DocumentId <= 0)
			{
				return;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectModified) != (MapiEventTypeFlags)0 && (mapiEvent.ExtendedEventFlags & MapiExtendedEventFlags.NoContentIndexingPropertyModified) != MapiExtendedEventFlags.None)
			{
				if (base.Config.ReadFlagEnabled)
				{
					notificationData.Type = NotificationType.ReadFlagChange;
					notificationData.Operation = DocumentOperation.Update;
				}
				return;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectCreated) != (MapiEventTypeFlags)0 || (mapiEvent.EventMask & MapiEventTypeFlags.ObjectCopied) != (MapiEventTypeFlags)0)
			{
				base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfCreateNotifications);
				notificationData.Type = NotificationType.Insert;
				notificationData.Operation = DocumentOperation.Insert;
				return;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectModified) != (MapiEventTypeFlags)0)
			{
				base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfUpdateNotifications);
				notificationData.Type = NotificationType.Update;
				notificationData.Operation = DocumentOperation.Update;
				return;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectMoved) != (MapiEventTypeFlags)0)
			{
				base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfMoveNotifications);
				notificationData.Type = NotificationType.Move;
				notificationData.Operation = DocumentOperation.Move;
				return;
			}
			if ((mapiEvent.EventMask & MapiEventTypeFlags.ObjectDeleted) != (MapiEventTypeFlags)0)
			{
				base.DiagnosticsSession.IncrementCounter(this.perfCounterInstance.NumberOfDeleteNotifications);
				notificationData.Type = NotificationType.Delete;
				notificationData.Operation = DocumentOperation.Delete;
				return;
			}
			throw new ArgumentException("mapiEvent.EventMask");
		}

		private void UpdateNotificationDataForStoreEvent(NotificationData notificationData)
		{
			notificationData.Type = NotificationType.Uninteresting;
			MapiEvent mapiEvent = notificationData.MapiEvent;
			Guid mailboxGuid = mapiEvent.MailboxGuid;
			if ((mapiEvent.EventMask & (MapiEventTypeFlags.MailboxCreated | MapiEventTypeFlags.MailboxDeleted | MapiEventTypeFlags.MailboxDisconnected | MapiEventTypeFlags.MailboxReconnected | MapiEventTypeFlags.MailboxMoveStarted | MapiEventTypeFlags.MailboxMoveSucceeded | MapiEventTypeFlags.MailboxMoveFailed)) != (MapiEventTypeFlags)0)
			{
				base.DiagnosticsSession.TraceDebug<Guid, MapiEventTypeFlags>("Mailbox with mailboxGuid {0} notified an event {1}", mailboxGuid, mapiEvent.EventMask);
				MapiEventTypeFlags eventMask = mapiEvent.EventMask;
				if (eventMask != MapiEventTypeFlags.MailboxDeleted)
				{
					if (eventMask != MapiEventTypeFlags.MailboxMoveSucceeded)
					{
						if (eventMask != MapiEventTypeFlags.MailboxMoveFailed)
						{
							return;
						}
						if (mapiEvent.EventFlags == MapiEventFlags.Destination)
						{
							base.DiagnosticsSession.TraceDebug<Guid, MdbInfo>("Mailbox with mailboxGuid {0} failed to be moved to database {1}.", mailboxGuid, this.mdbInfo);
							notificationData.Type = NotificationType.DeleteMailbox;
							notificationData.Operation = DocumentOperation.DeleteSelection;
							return;
						}
						base.DiagnosticsSession.TraceDebug<Guid, MdbInfo>("Mailbox {0} failed to be moved from database {1}.", mailboxGuid, this.mdbInfo);
						return;
					}
					else
					{
						if (mapiEvent.EventFlags != MapiEventFlags.Destination)
						{
							base.DiagnosticsSession.TraceDebug<Guid, MdbInfo>("Mailbox {0} successfully moved from database {1}. Marking this mailbox as 'DeletionPending' in property store.", mailboxGuid, this.mdbInfo);
							notificationData.Type = NotificationType.DeleteMailbox;
							notificationData.Operation = DocumentOperation.DeleteSelection;
							return;
						}
						base.DiagnosticsSession.TraceDebug<Guid, MdbInfo>("Mailbox {0} successfully moved to database {1}.", mailboxGuid, this.mdbInfo);
						return;
					}
				}
				else
				{
					ExTraceGlobals.NotificationQueueTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Mailbox with mailboxGuid {0} is deleted. Marking this mailbox 'DeletionPending'.", mapiEvent.MailboxGuid);
					notificationData.Type = NotificationType.DeleteMailbox;
					notificationData.Operation = DocumentOperation.DeleteSelection;
				}
			}
		}

		internal const MapiEventTypeFlags InterestingEventMask = MapiEventTypeFlags.ObjectCreated | MapiEventTypeFlags.ObjectDeleted | MapiEventTypeFlags.ObjectModified | MapiEventTypeFlags.ObjectMoved | MapiEventTypeFlags.ObjectCopied;

		internal const MapiEventTypeFlags MailboxNotificationEventMask = MapiEventTypeFlags.MailboxCreated | MapiEventTypeFlags.MailboxDeleted | MapiEventTypeFlags.MailboxDisconnected | MapiEventTypeFlags.MailboxReconnected | MapiEventTypeFlags.MailboxMoveStarted | MapiEventTypeFlags.MailboxMoveSucceeded | MapiEventTypeFlags.MailboxMoveFailed;

		internal static readonly MdbItemIdentity DummyMdbItemIdentity = new MdbItemIdentity(null, Guid.Empty, Guid.Empty, 0, StoreObjectId.DummyId, 0, false);

		private static readonly LocalizedString ErrorAccessingStateStorageTag = Strings.ErrorAccessingStateStorage;

		private readonly RefCount activeThreadCount = new RefCount();

		private readonly IWatermarkStorage watermarkStorage;

		private readonly MdbInfo mdbInfo;

		private readonly ISubmitDocument indexFeeder;

		private readonly IIndexStatusStore indexStatusStore;

		private readonly LocalizedString exceptionOccurred;

		private readonly MdbPerfCountersInstance perfCounterInstance;

		private readonly NotificationsQueue notificationQueue;

		private readonly NotificationsWatermarkManager watermarkManager;

		private readonly Guid pollingActivityId;

		private readonly INotificationsEventSource notificationsEventSource;

		private DateTime nextWatermarkUpdate;

		private TimeSpan timeUntilNextPoll;

		private DateTime lastSuccessfulPollTime;

		private int flagPendingSendDocuments;

		private long ageOfLastNotificationProcessed;
	}
}
