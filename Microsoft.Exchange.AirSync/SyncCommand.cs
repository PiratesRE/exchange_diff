using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime.Encoders;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class SyncCommand : SyncBase, IAsyncCommand
	{
		internal SyncCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfSyncRequests;
		}

		public bool ProcessingEventsEnabled { get; set; }

		internal NotificationManager Notifier
		{
			get
			{
				return this.notifier;
			}
			set
			{
				this.notifier = value;
			}
		}

		internal NotificationManager PreviousNotifier { get; set; }

		internal override bool ShouldSaveSyncStatus
		{
			get
			{
				return true;
			}
		}

		internal List<XmlNode> ItemLevelProtocolErrorNodes
		{
			get
			{
				return this.itemLevelProtocolErrorNodes;
			}
		}

		internal bool FailOnItemLevelProtocolErrors { get; set; }

		protected override string RootNodeName
		{
			get
			{
				return "Sync";
			}
		}

		protected override string RootNodeNamespace
		{
			get
			{
				return "AirSync:";
			}
		}

		public static bool IsObjectNotFound(Exception ex)
		{
			return ex is ObjectNotFoundException;
		}

		public static bool IsItemSyncTolerableException(Exception exception)
		{
			if (GlobalSettings.SendWatsonReport && (exception is ArgumentException || exception is FormatException || exception is InvalidOperationException || exception is InvalidProgramException || exception is NullReferenceException || exception is InvalidCastException || exception is NotSupportedException || exception is NotImplementedException || (exception is UnexpectedTypeException && ((UnexpectedTypeException)exception).SendInformationalWatson)) && exception != Constants.FaultInjectionFormatException)
			{
				AirSyncDiagnostics.SendInMemoryTraceWatson(exception);
			}
			AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.RequestsTracer, null, "Exception in IsItemSyncTolerableException: {0}", exception);
			return (exception is LocalizedException || exception is ArgumentException || exception is FormatException || exception is InvalidOperationException || exception is InvalidProgramException || exception is NullReferenceException || exception is InvalidCastException || exception is NotSupportedException || exception is NotImplementedException) && !(exception is TransientException) && !(exception is QuotaExceededException);
		}

		public static bool IsConversionFailedTolerableException(Exception exception)
		{
			if (exception == null || exception.InnerException == null)
			{
				return false;
			}
			while (exception.InnerException != null)
			{
				exception = exception.InnerException;
			}
			return exception is TextConvertersException || exception is ByteEncoderException || (!string.IsNullOrEmpty(exception.StackTrace) && (exception.StackTrace.Contains("CalculateSmimeMimeStructure") || exception.StackTrace.Contains("WriteMimeBody") || exception.StackTrace.Contains("WriteMimeAttachment") || exception.StackTrace.Contains("WriteFromHeader")));
		}

		public static string GetStaticStatusString(SyncBase.ErrorCodeStatus error)
		{
			string result;
			switch (error)
			{
			case SyncBase.ErrorCodeStatus.Success:
				result = "1";
				break;
			case SyncBase.ErrorCodeStatus.ProtocolVersionMismatch:
				result = "2";
				break;
			case SyncBase.ErrorCodeStatus.InvalidSyncKey:
				result = "3";
				break;
			case SyncBase.ErrorCodeStatus.ProtocolError:
				result = "4";
				break;
			case SyncBase.ErrorCodeStatus.ServerError:
				result = "5";
				break;
			case SyncBase.ErrorCodeStatus.ClientServerConversion:
				result = "6";
				break;
			case SyncBase.ErrorCodeStatus.Conflict:
				result = "7";
				break;
			case SyncBase.ErrorCodeStatus.ObjectNotFound:
			case SyncBase.ErrorCodeStatus.InvalidCollection:
				result = "8";
				break;
			case SyncBase.ErrorCodeStatus.OutOfDisk:
				result = "9";
				break;
			case SyncBase.ErrorCodeStatus.NotificationGUID:
				result = "10";
				break;
			case SyncBase.ErrorCodeStatus.NotificationsNotProvisioned:
				result = "11";
				break;
			default:
				result = "5";
				break;
			}
			return result;
		}

		public void ReleaseNotificationManager(bool wasStolen)
		{
			if (!this.ProcessingEventsEnabled)
			{
				throw new InvalidOperationException("Release NotificationManager called while processing events wasn't enabled!");
			}
			if (this.Notifier != null)
			{
				AirSyncDiagnostics.TraceDebug<string, bool>(ExTraceGlobals.ThreadingTracer, this, "Releasing notification manager for {0}, was Stolen? {1}", this.ToString(), wasStolen);
				this.PreviousNotifier = this.Notifier;
				if (wasStolen)
				{
					this.syncEvents.Enqueue(new SyncCommand.SyncEventAndTime(SyncCommand.SyncEvent.NMStolen));
				}
				this.syncEvents.Enqueue(new SyncCommand.SyncEventAndTime(SyncCommand.SyncEvent.NMReleased));
				this.Notifier = null;
			}
			if (!this.requestCompleted)
			{
				if (wasStolen)
				{
					base.Context.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, "NMStolen");
				}
				this.SyncCollisionDetected();
			}
		}

		public void Consume(Event evt)
		{
			using (ExPerfTrace.RelatedActivity(base.GetTraceActivityId()))
			{
				this.syncEvents.Enqueue(new SyncCommand.SyncEventAndTime(SyncCommand.SyncEvent.XsoEvent));
				AirSyncDiagnostics.TraceInfo<EventObjectType, EventType>(ExTraceGlobals.RequestsTracer, this, "Sync.Consume evt.ObjectType:{0} evt.EventType:{1}", evt.ObjectType, evt.EventType);
				if (!this.ProcessingEventsEnabled)
				{
					throw new InvalidOperationException("Internal Consume called while processing events wasn't enabled!");
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
							AirSyncDiagnostics.TraceInfo<TimeSpan>(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.Consume] Request wait time {0}", base.RequestWaitWatch.Elapsed);
						}
						base.ProtocolLogger.SetValue(ProtocolLoggerData.TimeContinued, ExDateTime.UtcNow);
						try
						{
							SyncCommand.SyncInformationForNotifier syncInformationForNotifier = (SyncCommand.SyncInformationForNotifier)this.Notifier.Information;
							switch (evt.ObjectType)
							{
							case EventObjectType.Item:
								if (base.IsInQuarantinedState)
								{
									AirSyncDiagnostics.TraceError<EventObjectType>(ExTraceGlobals.RequestsTracer, this, "Sync.Consume(): Should not hit item level consume in quarantined state! : {0}", evt.ObjectType);
									this.CompleteRequest(SyncCommand.serverErrorXml, false, false);
								}
								else
								{
									bool flag = true;
									if (syncInformationForNotifier.HangingVirtualFolderIds == null || !syncInformationForNotifier.HangingVirtualFolderIds.Contains(evt.ObjectId))
									{
										if (((evt.EventType & EventType.ObjectMoved) == EventType.ObjectMoved && !syncInformationForNotifier.StoreIdToCollectionId.ContainsKey(evt.ParentObjectId) && !syncInformationForNotifier.StoreIdToCollectionId.ContainsKey(evt.OldParentObjectId)) || ((evt.EventType & EventType.ObjectMoved) != EventType.ObjectMoved && !syncInformationForNotifier.StoreIdToCollectionId.ContainsKey(evt.ParentObjectId)))
										{
											AirSyncDiagnostics.TraceDebug<EventType, StoreObjectId, StoreObjectId>(ExTraceGlobals.AlgorithmTracer, this, "Received event in SyncCommand.Consume() for an unknown folder.  EventType: {0}\nParentObjectId: {1}\nOldParentObjectId: {2}", evt.EventType, evt.ParentObjectId, evt.OldParentObjectId);
										}
										else
										{
											if (((evt.EventType & EventType.ObjectMoved) == EventType.ObjectMoved && !syncInformationForNotifier.StoreIdToCollectionId.ContainsKey(evt.ParentObjectId)) || (evt.EventType & EventType.ObjectDeleted) == EventType.ObjectDeleted)
											{
												bool flag2 = true;
												if ((evt.EventType & EventType.ObjectDeleted) == EventType.ObjectDeleted)
												{
													if (syncInformationForNotifier.StoreIdToType[evt.ParentObjectId] == "Calendar" || syncInformationForNotifier.StoreIdToType[evt.ParentObjectId] == "Tasks")
													{
														flag2 = false;
													}
													else
													{
														this.collectionIdsDelayed.Add(syncInformationForNotifier.StoreIdToCollectionId[evt.ParentObjectId]);
													}
												}
												else if (syncInformationForNotifier.StoreIdToType[evt.OldParentObjectId] == "Calendar" || syncInformationForNotifier.StoreIdToType[evt.OldParentObjectId] == "Tasks")
												{
													flag2 = false;
													flag = false;
												}
												else
												{
													this.collectionIdsDelayed.Add(syncInformationForNotifier.StoreIdToCollectionId[evt.OldParentObjectId]);
												}
												if (flag2)
												{
													break;
												}
											}
											if ((evt.EventType & EventType.ObjectMoved) == EventType.ObjectMoved && syncInformationForNotifier.StoreIdToCollectionId.ContainsKey(evt.OldParentObjectId))
											{
												this.collectionIdsDelayed.Add(syncInformationForNotifier.StoreIdToCollectionId[evt.OldParentObjectId]);
											}
											if ((evt.EventType & EventType.ObjectModified) == EventType.ObjectModified && syncInformationForNotifier.StoreIdToType[evt.ParentObjectId] == "Email")
											{
												this.collectionIdsDelayed.Add(syncInformationForNotifier.StoreIdToCollectionId[evt.ParentObjectId]);
											}
											else
											{
												this.InitializeOnEventSubscription();
												if (base.SyncStatusSyncData != null)
												{
													string lastSyncRequestRandomString = base.SyncStatusSyncData.LastSyncRequestRandomString;
													if (lastSyncRequestRandomString == null || lastSyncRequestRandomString != syncInformationForNotifier.RandomNumberString)
													{
														AirSyncDiagnostics.TraceDebug<SyncCommand.SyncInformationForNotifier, string>(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.Consume] Newer sync request came in.  Old Random Number: {0}, New: {1}", syncInformationForNotifier, lastSyncRequestRandomString);
														base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "OldSync");
														this.SyncCollisionDetected();
														this.shouldHang = false;
														break;
													}
												}
												string text = syncInformationForNotifier.StoreIdToCollectionId[flag ? evt.ParentObjectId : evt.OldParentObjectId];
												HashSet<string> hashSet = new HashSet<string>(this.collectionIdsDelayed);
												hashSet.Add(text);
												List<string> list;
												this.ReadXmlRequest(this.itemLevelProtocolErrorNodes, hashSet, true, out list);
												this.InitializeResponseXmlDocument();
												SyncCollection syncCollection = base.Collections[text];
												this.SyncTheCollection(syncCollection, false, false);
												if (this.shouldHang)
												{
													syncCollection.Dispose();
													this.PrepareToHang(false);
												}
												else
												{
													if (list[0] == syncCollection.InternalName && syncCollection.ClientCommands == null)
													{
														this.collectionIdsDelayed.Remove(text);
														ItemIdMapping itemIdMapping = (ItemIdMapping)syncCollection.SyncState[CustomStateDatumType.IdMapping];
														itemIdMapping.Flush();
														this.SaveSyncStateAndDispose(syncCollection);
													}
													else
													{
														this.DisposeOpenCollections();
														this.collectionIdsDelayed.Add(text);
														this.InitializeResponseXmlDocument();
														this.ReadXmlRequest(this.itemLevelProtocolErrorNodes, this.collectionIdsDelayed, true, out list);
													}
													foreach (string text2 in list)
													{
														if (this.collectionIdsDelayed.Contains(text2))
														{
															syncCollection = base.Collections[text2];
															this.SyncTheCollection(syncCollection, false, false);
															if (syncCollection.HaveChanges)
															{
																ItemIdMapping itemIdMapping2 = (ItemIdMapping)syncCollection.SyncState[CustomStateDatumType.IdMapping];
																itemIdMapping2.Flush();
																this.SaveSyncStateAndDispose(syncCollection);
															}
															else
															{
																syncCollection.Dispose();
															}
														}
													}
													this.shouldHang = false;
													this.CompleteRequest(null, false, false);
												}
											}
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
								if (evt.ParentObjectId.CompareTo(syncInformationForNotifier.ExchangeSyncDataStoreObjectId) != 0)
								{
									this.InitializeOnEventSubscription();
									if (evt.ParentObjectId.CompareTo(syncInformationForNotifier.SyncStateStorageStoreObjectId) == 0)
									{
										try
										{
											using (Folder folder = Folder.Bind(base.MailboxSession, evt.ObjectId))
											{
												if (folder.DisplayName != "AutdTrigger" || folder.DisplayName != SyncStateStorage.MailboxLoggingTriggerFolder)
												{
													break;
												}
											}
											goto IL_69C;
										}
										catch (ObjectNotFoundException)
										{
											goto IL_69C;
										}
									}
									try
									{
										using (Folder folder2 = Folder.Bind(base.MailboxSession, evt.ParentObjectId))
										{
											if (folder2.ParentId.CompareTo(syncInformationForNotifier.ExchangeSyncDataStoreObjectId) == 0)
											{
												break;
											}
										}
									}
									catch (ObjectNotFoundException)
									{
									}
									IL_69C:
									this.shouldHang = false;
									base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Sync.FHSyncRequired");
									this.CompleteRequest(SyncCommand.GetFolderHierarchySyncRequiredXml(), false, false);
								}
								break;
							default:
								AirSyncDiagnostics.TraceError<EventObjectType>(ExTraceGlobals.RequestsTracer, this, "Sync.Consume(): unknown object type: {0}", evt.ObjectType);
								base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Sync.UnknownNotification");
								this.CompleteRequest(SyncCommand.serverErrorXml, false, false);
								break;
							}
						}
						finally
						{
							if (this.shouldHang)
							{
								this.PrepareToHang();
							}
							this.ReleaseResources();
						}
					}
					catch (Exception ex)
					{
						this.HandleException(ex);
					}
					finally
					{
						if (base.User != null && base.User.Context != null && base.User.Context.ActivityScope != null)
						{
							base.User.Context.ActivityScope.Action = action;
						}
						this.DisposeOpenCollections();
					}
				}
			}
		}

		public void HandleAccountTerminated(NotificationManager.AsyncEvent evt)
		{
			using (ExPerfTrace.RelatedActivity(base.GetTraceActivityId()))
			{
				AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "Sync.HandleAccountTerminated account state: {0}", evt.AccountState.ToString());
				if (!this.ProcessingEventsEnabled)
				{
					throw new InvalidOperationException("HandleAccountTerminated called while processing events wasn't enabled!");
				}
				if (!this.requestCompleted)
				{
					try
					{
						if (base.RequestWaitWatch != null)
						{
							base.RequestWaitWatch.Stop();
							base.ProtocolLogger.SetValue(ProtocolLoggerData.RequestHangTime, base.RequestWaitWatch.ElapsedMilliseconds / 1000L);
						}
						base.ProtocolLogger.SetValue(ProtocolLoggerData.AccountTerminated, evt.AccountState.ToString());
						base.SetHttpStatusCodeForTerminatedAccount(evt.AccountState);
						try
						{
							this.CompleteRequest(base.XmlResponse, false, false);
						}
						finally
						{
							this.ReleaseResources();
						}
					}
					catch (Exception ex)
					{
						if (!AirSyncUtility.HandleNonCriticalException(ex, true))
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
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.HeartbeatCallback");
				if (!this.ProcessingEventsEnabled)
				{
					throw new InvalidOperationException("HeartbeatCallback called while processing events wasn't enabled!");
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
						if ((this.collectionIdsDelayed.Count == 0 && this.requestWasCached) || !this.isDirectPushAllowed)
						{
							AirSyncDiagnostics.TraceDebug<int, bool>(ExTraceGlobals.RequestsTracer, this, "Hanging SyncCommand timed out and is completing the request prior to delayed sync. DelayOps count: {0}, DirectPushAllowed: {1}", this.collectionIdsDelayed.Count, this.isDirectPushAllowed);
							base.XmlResponse = null;
							this.CompleteRequest(null, this.isDirectPushAllowed, true);
						}
						else
						{
							this.InitializeOnEventSubscription();
							if (base.SyncStatusSyncData != null)
							{
								string lastSyncRequestRandomString = base.SyncStatusSyncData.LastSyncRequestRandomString;
								if (lastSyncRequestRandomString == null || lastSyncRequestRandomString != ((SyncCommand.SyncInformationForNotifier)this.Notifier.Information).RandomNumberString)
								{
									base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "OldSync2");
									this.SyncCollisionDetected();
									return;
								}
							}
							else
							{
								base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Sync.NoSyncState");
								this.SyncCollisionDetected();
							}
							bool flag = false;
							bool flag2 = this.collectionIdsDelayed.Count == 0;
							HashSet<string> collectionIdsToMatch = new HashSet<string>(this.collectionIdsDelayed);
							List<string> list;
							this.ReadXmlRequest(this.itemLevelProtocolErrorNodes, collectionIdsToMatch, true, out list);
							this.shouldHang = false;
							this.InitializeResponseXmlDocument();
							try
							{
								foreach (string text in list)
								{
									if (this.collectionIdsDelayed.Contains(text))
									{
										SyncCollection syncCollection = base.Collections[text];
										flag |= this.SyncTheCollection(syncCollection, false, false);
										flag2 &= syncCollection.IsLogicallyEmptyResponse;
										if (syncCollection.HaveChanges)
										{
											ItemIdMapping itemIdMapping = (ItemIdMapping)syncCollection.SyncState[CustomStateDatumType.IdMapping];
											itemIdMapping.Flush();
											this.SaveSyncStateAndDispose(syncCollection);
										}
										else
										{
											syncCollection.Dispose();
										}
									}
								}
							}
							finally
							{
								this.DisposeOpenCollections();
							}
							if (flag2 && !this.requestWasCached)
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.HeartbeatCallback] Logically empty response.  Marking as able to send up empty requests.");
								base.SyncStatusSyncData.ClientCanSendUpEmptyRequests = true;
							}
							if (!flag)
							{
								base.XmlResponse = null;
							}
							this.CompleteRequest(null, !flag, true);
						}
					}
					catch (Exception ex)
					{
						this.HandleException(ex);
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
				AirSyncDiagnostics.TraceInfo<Exception>(ExTraceGlobals.RequestsTracer, this, "Sync.HandleException\r\n{0}", ex);
				if (!this.ProcessingEventsEnabled)
				{
					throw new InvalidOperationException("HandleException called while processing events wasn't enabled!");
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
						if (base.RequestWaitWatch != null)
						{
							base.RequestWaitWatch.Stop();
							base.ProtocolLogger.SetValue(ProtocolLoggerData.RequestHangTime, base.RequestWaitWatch.ElapsedMilliseconds / 1000L);
						}
						base.XmlResponse = null;
						Exception ex2 = ex;
						AirSyncPermanentException ex3 = ex as AirSyncPermanentException;
						if (ex3 != null && (ex3.AirSyncStatusCode == StatusCode.Sync_FolderHierarchyRequired || ex3.AirSyncStatusCode == StatusCode.Sync_ObjectNotFound))
						{
							AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex3);
							AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "Exception thrown during Sync\r\n{0}\r\nReturning object not found error.", arg);
							base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Sync.ObjectNotFound");
							this.CompleteRequest(SyncCommand.GetObjectNotFoundErrorXml(), false, false);
							this.ReleaseResources();
						}
						else
						{
							base.LastException = ex;
							SyncBase.ErrorCodeStatus globalStatus = base.GlobalStatus;
							switch (globalStatus)
							{
							case SyncBase.ErrorCodeStatus.ProtocolError:
								ex2 = new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), ex, false);
								((AirSyncPermanentException)ex2).ErrorStringForProtocolLogger = "Sync.ProtocolError";
								goto IL_17B;
							case SyncBase.ErrorCodeStatus.ServerError:
								ex2 = new AirSyncPermanentException(StatusCode.Sync_ServerError, SyncCommand.GetServerErrorXml(), ex, false);
								((AirSyncPermanentException)ex2).ErrorStringForProtocolLogger = "Sync.ServerError";
								goto IL_17B;
							case SyncBase.ErrorCodeStatus.ClientServerConversion:
							case SyncBase.ErrorCodeStatus.Conflict:
								goto IL_17B;
							case SyncBase.ErrorCodeStatus.ObjectNotFound:
								break;
							default:
								if (globalStatus != SyncBase.ErrorCodeStatus.InvalidCollection)
								{
									goto IL_17B;
								}
								break;
							}
							ex2 = new AirSyncPermanentException(StatusCode.Sync_ObjectNotFound, SyncCommand.GetObjectNotFoundErrorXml(), ex, false);
							((AirSyncPermanentException)ex2).ErrorStringForProtocolLogger = "Sync.ObjectNotFound2";
							IL_17B:
							AirSyncUtility.ProcessException(ex2, this, base.Context);
							try
							{
								this.CompleteRequest(base.XmlResponse, false, false);
							}
							finally
							{
								this.ReleaseResources();
							}
						}
					}
					catch (Exception ex4)
					{
						if (!AirSyncUtility.HandleNonCriticalException(ex4, true))
						{
							throw;
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
				List<string> list;
				this.ReadXmlRequest(this.itemLevelProtocolErrorNodes, out list, out result);
			}
			catch (AirSyncPermanentException arg)
			{
				AirSyncDiagnostics.TraceError<AirSyncPermanentException>(ExTraceGlobals.RequestsTracer, null, "Exception in Sync.GetHeartbeatInterval: {0}", arg);
			}
			return result;
		}

		internal override XmlDocument GetInvalidParametersXml()
		{
			if (SyncCommand.invalidParametersXml == null)
			{
				string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>" + 13 + "</Status></Sync>";
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(xml);
				SyncCommand.invalidParametersXml = xmlDocument;
			}
			return (XmlDocument)SyncCommand.invalidParametersXml.Clone();
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			Command.ExecutionState result = this.OnExecute();
			switch (result)
			{
			case Command.ExecutionState.Pending:
				break;
			case Command.ExecutionState.Complete:
				base.ProtocolLogger.SetValue(ProtocolLoggerData.StatusCode, (int)base.GlobalStatus);
				break;
			default:
				throw new InvalidOperationException();
			}
			return result;
		}

		protected override void CompleteHttpRequest()
		{
			if (this.hangSpecified && !this.isDirectPushAllowed)
			{
				base.Context.Response.AppendHeader("X-MS-NoPush", string.Empty);
			}
			this.requestCompleted = true;
			base.CompleteHttpRequest();
		}

		protected bool LoadCachedRequest(bool allowPartialRequest, out XmlDocument xmlDocument)
		{
			xmlDocument = null;
			if (base.SyncStatusSyncData == null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.LoadCachedRequest] Returning false because SyncStatusSyncData is null.");
				return false;
			}
			if (!allowPartialRequest && !base.SyncStatusSyncData.ClientCanSendUpEmptyRequests)
			{
				AirSyncDiagnostics.TraceDebug<bool, bool>(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.LoadCachedRequest] Returning false allowPartialRequest == '{0}' and ClientCanSendUpEmptyRequests == '{1}'.", allowPartialRequest, base.SyncStatusSyncData.ClientCanSendUpEmptyRequests);
				return false;
			}
			byte[] lastCachableWbxmlDocument = base.SyncStatusSyncData.LastCachableWbxmlDocument;
			if (lastCachableWbxmlDocument == null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.LoadCachedRequest] Returning false because LastCachableWbxmlDocument is missing from sync state or is null.");
				return false;
			}
			using (MemoryStream memoryStream = new MemoryStream(lastCachableWbxmlDocument))
			{
				using (WbxmlReader wbxmlReader = new WbxmlReader(memoryStream))
				{
					xmlDocument = wbxmlReader.ReadXmlDocument();
				}
			}
			if (xmlDocument.DocumentElement.LocalName != "Sync")
			{
				AirSyncDiagnostics.TraceDebug<string>(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.LoadCachedRequest] Found cached request, but it wasn't a Sync request, it was a {0} request", xmlDocument.DocumentElement.LocalName);
				xmlDocument = null;
				return false;
			}
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.LoadCachedRequest] Yay!  Loaded cached request.");
			return true;
		}

		protected override void LogRequestToMailboxLog(string requestToLog)
		{
			if (this.requestWasCached)
			{
				base.MailboxLogger.SetData(MailboxLogDataName.RequestBody, "Empty");
				base.MailboxLogger.SetData(MailboxLogDataName.LogicalRequest, requestToLog);
				return;
			}
			base.LogRequestToMailboxLog(requestToLog);
		}

		protected override bool PreProcessRequest()
		{
			bool flag = base.PreProcessRequest();
			if (flag && base.Request.IsEmpty)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Request was cached.");
				this.requestWasCached = true;
				XmlDocument xmlDocument;
				flag = this.LoadCachedRequest(false, out xmlDocument);
				base.Request.XmlDocument = xmlDocument;
			}
			return flag;
		}

		protected override void ReleaseResources()
		{
			this.DisposeOpenCollections();
			base.ReleaseResources();
		}

		protected override void Dispose(bool disposing)
		{
			NotificationManager notificationManager = this.Notifier;
			if (notificationManager != null)
			{
				notificationManager.ReleaseCommand(this);
			}
			base.Dispose(disposing);
		}

		protected override void ProcessQueuedEvents()
		{
			if (this.Notifier != null)
			{
				this.Notifier.ProcessQueuedEvents(this);
			}
		}

		protected override string GetStatusString(SyncBase.ErrorCodeStatus error)
		{
			return SyncCommand.GetStaticStatusString(error);
		}

		protected override void GetOrCreateNotificationManager(out bool notificationManagerWasTaken)
		{
			if (!this.IsHangingRequest())
			{
				notificationManagerWasTaken = false;
				return;
			}
			this.isDirectPushAllowed = DeviceCapability.IsDirectPushAllowed(base.Context, out this.isDirectPushAllowedByGeo);
			if (base.Context.Request.IsEmpty && this.isDirectPushAllowed)
			{
				this.Notifier = NotificationManager.GetOrCreateNotificationManager(base.Context, this, out notificationManagerWasTaken);
				if (notificationManagerWasTaken)
				{
					base.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.StolenNM, true);
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Sync.GetOrCreateNotificationManager stole a notification manager");
					Interlocked.Increment(ref SyncCommand.numberOfOutstandingSyncs);
					AirSyncCounters.CurrentlyPendingSync.RawValue = (long)SyncCommand.numberOfOutstandingSyncs;
					base.ProtocolLogger.SetValue(ProtocolLoggerData.HeartBeatInterval, this.Notifier.RequestedWaitTime);
					SyncCommand.SyncHbiMonitor.Instance.RegisterSample(this.Notifier.RequestedWaitTime, base.Context);
					this.hanging = true;
					return;
				}
			}
			else
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Sync.GetOrCreateNotificationManager created a new notification manager");
				notificationManagerWasTaken = false;
				this.Notifier = NotificationManager.CreateNotificationManager(base.Context, this);
			}
		}

		protected override void SetNotificationManagerMailboxLogging(bool mailboxLogging)
		{
			if (this.Notifier != null)
			{
				this.Notifier.MailboxLoggingEnabled = mailboxLogging;
			}
		}

		protected override bool HandleQuarantinedState()
		{
			return true;
		}

		internal override bool RightsManagementSupportFlag
		{
			get
			{
				return this.currentSyncCollection != null && this.currentSyncCollection.RightsManagementSupport;
			}
		}

		private static XmlDocument GetServerErrorXml()
		{
			if (SyncCommand.serverErrorXml == null)
			{
				string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>" + 5 + "</Status></Sync>";
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(xml);
				SyncCommand.serverErrorXml = xmlDocument;
			}
			return (XmlDocument)SyncCommand.serverErrorXml.Clone();
		}

		private static XmlDocument GetMaxFoldersExceededXml()
		{
			if (SyncCommand.maxFoldersExceededXml == null)
			{
				string xml = string.Concat(new object[]
				{
					"<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>",
					15,
					"</Status><Limit>",
					GlobalSettings.MaxNumOfFolders,
					"</Limit></Sync>"
				});
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(xml);
				SyncCommand.maxFoldersExceededXml = xmlDocument;
			}
			return (XmlDocument)SyncCommand.maxFoldersExceededXml.Clone();
		}

		private static XmlDocument GetMaxWaitExceededXml(bool respondInSeconds)
		{
			if (SyncCommand.maxWaitExceededXmlInSeconds == null)
			{
				string xml = string.Concat(new object[]
				{
					"<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>",
					14,
					"</Status><Limit>",
					GlobalSettings.HeartbeatInterval.HighInterval,
					"</Limit></Sync>"
				});
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(xml);
				SyncCommand.maxWaitExceededXmlInSeconds = xmlDocument;
			}
			if (SyncCommand.maxWaitExceededXmlInMinutes == null)
			{
				string xml2 = string.Concat(new object[]
				{
					"<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>",
					14,
					"</Status><Limit>",
					GlobalSettings.HeartbeatInterval.HighInterval / 60,
					"</Limit></Sync>"
				});
				XmlDocument xmlDocument2 = new SafeXmlDocument();
				xmlDocument2.LoadXml(xml2);
				SyncCommand.maxWaitExceededXmlInMinutes = xmlDocument2;
			}
			if (!respondInSeconds)
			{
				return (XmlDocument)SyncCommand.maxWaitExceededXmlInMinutes.Clone();
			}
			return (XmlDocument)SyncCommand.maxWaitExceededXmlInSeconds.Clone();
		}

		private static XmlDocument GetMinWaitExceededXml(bool respondInSeconds)
		{
			if (SyncCommand.minWaitExceededXmlInSeconds == null)
			{
				string xml = string.Concat(new object[]
				{
					"<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>",
					14,
					"</Status><Limit>",
					GlobalSettings.HeartbeatInterval.LowInterval,
					"</Limit></Sync>"
				});
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(xml);
				SyncCommand.minWaitExceededXmlInSeconds = xmlDocument;
			}
			if (SyncCommand.minWaitExceededXmlInMinutes == null)
			{
				string xml2 = string.Concat(new object[]
				{
					"<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>",
					14,
					"</Status><Limit>",
					GlobalSettings.HeartbeatInterval.LowInterval / 60,
					"</Limit></Sync>"
				});
				XmlDocument xmlDocument2 = new SafeXmlDocument();
				xmlDocument2.LoadXml(xml2);
				SyncCommand.minWaitExceededXmlInMinutes = xmlDocument2;
			}
			if (!respondInSeconds)
			{
				return (XmlDocument)SyncCommand.minWaitExceededXmlInMinutes.Clone();
			}
			return (XmlDocument)SyncCommand.minWaitExceededXmlInSeconds.Clone();
		}

		private static XmlDocument GetFolderHierarchySyncRequiredXml()
		{
			if (SyncCommand.folderHierarchySyncRequiredXml == null)
			{
				string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>" + 12 + "</Status></Sync>";
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(xml);
				SyncCommand.folderHierarchySyncRequiredXml = xmlDocument;
			}
			return (XmlDocument)SyncCommand.folderHierarchySyncRequiredXml.Clone();
		}

		private static XmlDocument GetObjectNotFoundErrorXml()
		{
			if (SyncCommand.objectNotFoundErrorXml == null)
			{
				string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>" + 8 + "</Status></Sync>";
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(xml);
				SyncCommand.objectNotFoundErrorXml = xmlDocument;
			}
			return (XmlDocument)SyncCommand.objectNotFoundErrorXml.Clone();
		}

		private static XmlDocument GetRetryErrorXml()
		{
			if (SyncCommand.retryErrorXml == null)
			{
				string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Sync xmlns=\"AirSync:\"><Status>" + 16 + "</Status></Sync>";
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(xml);
				SyncCommand.retryErrorXml = xmlDocument;
			}
			return (XmlDocument)SyncCommand.retryErrorXml.Clone();
		}

		internal override bool ValidateXml()
		{
			bool flag = base.ValidateXml();
			if (!flag)
			{
				base.GlobalStatus = SyncBase.ErrorCodeStatus.ProtocolError;
			}
			return flag;
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (SyncCommand.validationErrorXml == null)
			{
				XmlDocument commandXmlStub = base.GetCommandXmlStub();
				XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
				xmlElement.InnerText = XmlConvert.ToString(4);
				commandXmlStub[this.RootNodeName].AppendChild(xmlElement);
				SyncCommand.validationErrorXml = commandXmlStub;
			}
			return (XmlDocument)SyncCommand.validationErrorXml.Clone();
		}

		internal void InitializeResponseXmlDocument()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.InitializeResponseXmlDocument");
			base.XmlResponse = new SafeXmlDocument();
			XmlElement xmlElement = base.XmlResponse.CreateElement("Sync", "AirSync:");
			base.XmlResponse.AppendChild(xmlElement);
			XmlElement newChild = base.XmlResponse.CreateElement("Collections", "AirSync:");
			xmlElement.AppendChild(newChild);
			this.collectionsResponseXmlNode = newChild;
		}

		private bool IsBadItemReportingEnabled()
		{
			bool result = false;
			if (!GlobalSettings.IsMultiTenancyEnabled)
			{
				ADMobileVirtualDirectory admobileVirtualDirectory = ADNotificationManager.ADMobileVirtualDirectory;
				if (admobileVirtualDirectory != null)
				{
					result = admobileVirtualDirectory.BadItemReportingEnabled;
				}
			}
			return result;
		}

		private void ParseSyncKey(SyncCollection collection)
		{
			AirSyncDiagnostics.TraceInfo<SyncCollection>(ExTraceGlobals.RequestsTracer, this, "Sync.ParseSyncKey {0}", collection);
			collection.AllowRecovery = collection.ParseSynckeyAndDetermineRecovery();
			if (base.Context.DeviceBehavior != null)
			{
				base.Context.DeviceBehavior.AddSyncKey(this.syncAttemptTime, collection.CollectionId, collection.SyncKey);
			}
			AirSyncDiagnostics.Assert(collection != null);
			if (collection.SyncKey == 0U && ((collection.ClientCommands != null && collection.ClientCommands.Length > 0) || collection.GetChanges))
			{
				collection.Status = SyncBase.ErrorCodeStatus.ProtocolError;
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, false)
				{
					ErrorStringForProtocolLogger = "Sk0WithActions"
				};
			}
		}

		protected override Validator GetValidator()
		{
			return new SyncCommand.SyncValidator(base.Version, GlobalSettings.MaxClientSentBadItems, this.itemLevelProtocolErrorNodes, this.FailOnItemLevelProtocolErrors, base.HasOutlookExtensions);
		}

		private Command.ExecutionState OnExecute()
		{
			Command.ExecutionState result = Command.ExecutionState.Complete;
			int nextNumber = base.GetNextNumber(0, true);
			string text = null;
			if (base.Version >= 121)
			{
				if (!this.requestWasCached)
				{
					base.SyncStatusSyncData.ClientCanSendUpEmptyRequests = false;
				}
				text = Command.MachineName + ":" + nextNumber.ToString(CultureInfo.InvariantCulture);
				base.SyncStatusSyncData.LastSyncRequestRandomString = text;
			}
			try
			{
				base.InitializeVersionFactory(base.Version);
				List<string> list;
				uint num;
				this.ReadXmlRequest(this.itemLevelProtocolErrorNodes, out list, out num);
				if (this.shouldHang && base.CurrentAccessState == DeviceAccessState.DeviceDiscovery)
				{
					base.GlobalInfo.DeviceInformationPromoted = true;
					if (!base.IsDeviceAccessAllowed())
					{
						return result;
					}
				}
				this.InitializeResponseXmlDocument();
				if (this.shouldHang)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: start hanging!");
					try
					{
						SyncCommand.SyncInformationForNotifier syncInformationForNotifier = new SyncCommand.SyncInformationForNotifier(text);
						this.Notifier.Information = syncInformationForNotifier;
						this.Notifier.StartTimer((num > 0U) ? num : 1U, base.Context.RequestTime, base.NextPolicyRefreshTime);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.HeartBeatInterval, num);
						SyncCommand.SyncHbiMonitor.Instance.RegisterSample(num, base.Context);
						if (this.isDirectPushAllowed)
						{
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: creating backend subscriptions");
							this.CreateSubscription(null);
						}
						if (this.FolderHierarchyChangedSinceLastSync())
						{
							this.shouldHang = false;
						}
						else
						{
							foreach (string key in list)
							{
								SyncCollection syncCollection = base.Collections[key];
								this.SyncTheCollection(syncCollection, this.isDirectPushAllowed, true);
								syncInformationForNotifier.CollectionIdToSyncType[syncCollection.InternalName] = syncCollection.SyncTypeString;
								if (syncCollection.HaveChanges || syncCollection.OptionsSentAreDifferentForV121AndLater)
								{
									AirSyncDiagnostics.TraceDebug<SyncCollection>(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: Delay collection {0} since there are client commands", syncCollection);
									this.collectionIdsDelayed.Add(syncCollection.InternalName);
								}
								else
								{
									syncCollection.Dispose();
								}
							}
							AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: save any collection which has changes and hasn't been saved yet");
							foreach (string key2 in this.collectionIdsDelayed)
							{
								SyncCollection syncCollection2 = base.Collections[key2];
								if (this.shouldHang && this.isDirectPushAllowed)
								{
									if ((syncCollection2.ClientCommands != null && syncCollection2.ClientCommands.Length > 0) || syncCollection2.OptionsSentAreDifferentForV121AndLater)
									{
										AirSyncDiagnostics.TraceDebug<SyncCollection>(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: Call UndoServerOperations on collection {0}", syncCollection2);
										syncCollection2.FolderSync.UndoServerOperations();
										this.SaveSyncStateAndDispose(syncCollection2);
									}
									else
									{
										syncCollection2.Dispose();
									}
								}
								else
								{
									AirSyncDiagnostics.TraceDebug<SyncCollection>(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: save collection {0}", syncCollection2);
									if (syncCollection2.SyncState != null)
									{
										ItemIdMapping itemIdMapping = (ItemIdMapping)syncCollection2.SyncState[CustomStateDatumType.IdMapping];
										itemIdMapping.Flush();
									}
									this.SaveSyncStateAndDispose(syncCollection2);
									this.shouldHang = false;
								}
							}
							if (this.shouldHang)
							{
								AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: shouldHang");
								base.RequestWaitWatch = new Stopwatch();
								result = Command.ExecutionState.Pending;
								Interlocked.Increment(ref SyncCommand.numberOfOutstandingSyncs);
								AirSyncCounters.CurrentlyPendingSync.RawValue = (long)SyncCommand.numberOfOutstandingSyncs;
								syncInformationForNotifier.SyncStateStorageStoreObjectId = base.SyncStateStorage.FolderId;
								syncInformationForNotifier.ExchangeSyncDataStoreObjectId = base.SyncStateStorage.SyncRootFolderId;
								this.hanging = true;
							}
						}
						goto IL_449;
					}
					finally
					{
						if (!this.hanging)
						{
							this.CleanupNotificationManager(false);
						}
					}
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: no hanging!");
				bool flag = false;
				bool flag2 = true;
				if (base.Version >= 121 && this.FolderHierarchyChangedSinceLastSync())
				{
					flag = true;
				}
				else
				{
					foreach (string key3 in list)
					{
						SyncCollection syncCollection3 = base.Collections[key3];
						flag |= this.SyncTheCollection(syncCollection3, false, true);
						flag2 &= syncCollection3.IsLogicallyEmptyResponse;
						this.SaveSyncStateAndDispose(syncCollection3);
					}
				}
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: Save away the last request number");
				base.SyncStatusSyncData.LastSyncRequestRandomString = text;
				if (base.Version >= 121)
				{
					if (flag2 && !this.requestWasCached)
					{
						base.SyncStatusSyncData.ClientCanSendUpEmptyRequests = true;
					}
					if (!flag)
					{
						base.XmlResponse = null;
					}
				}
				IL_449:
				if (!this.requestWasCached && base.Version >= 121)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: cache the request");
					using (MemoryStream memoryStream = new MemoryStream(400))
					{
						WbxmlWriter wbxmlWriter = new WbxmlWriter(memoryStream);
						wbxmlWriter.WriteXmlDocument(base.Context.Request.XmlDocument);
						SyncRequestWasHangingCache.Set(base.Context.User.MailboxGuid, base.Context.Request.DeviceIdentity, this.hangSpecified);
						base.SyncStatusSyncData.LastCachableWbxmlDocument = memoryStream.ToArray();
					}
				}
				this.syncSuccessTime = new ExDateTime?(ExDateTime.UtcNow);
			}
			catch (Exception ex)
			{
				AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: Exception caught {0}", ex);
				result = Command.ExecutionState.Complete;
				base.XmlResponse = null;
				if (ex is AirSyncPermanentException)
				{
					if (base.MailboxLogger != null)
					{
						base.MailboxLogger.SetData(MailboxLogDataName.SyncCommand_OnExecute_Exception, ex);
					}
					AirSyncPermanentException ex2 = ex as AirSyncPermanentException;
					AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex2);
					AirSyncDiagnostics.TraceError<StatusCode, AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "Exception thrown during Sync, status code {0}\r\n{1}", ex2.AirSyncStatusCode, arg);
					if (ex2.AirSyncStatusCode == StatusCode.Sync_ProtocolError)
					{
						base.XmlResponse = this.GetValidationErrorXml();
						return result;
					}
					if (ex2.AirSyncStatusCode == StatusCode.Sync_FolderHierarchyRequired)
					{
						base.XmlResponse = SyncCommand.GetFolderHierarchySyncRequiredXml();
						return result;
					}
					if (ex2.AirSyncStatusCode == StatusCode.Sync_ObjectNotFound)
					{
						base.XmlResponse = SyncCommand.GetObjectNotFoundErrorXml();
						return result;
					}
					if (ex2.AirSyncStatusCode == StatusCode.BodyPartPreferenceTypeNotSupported)
					{
						throw;
					}
				}
				base.LastException = ex;
				AirSyncDiagnostics.TraceDebug<SyncBase.ErrorCodeStatus>(ExTraceGlobals.RequestsTracer, null, "Sync.OnExecute: GlobalStatus = {0}", base.GlobalStatus);
				SyncBase.ErrorCodeStatus globalStatus = base.GlobalStatus;
				switch (globalStatus)
				{
				case SyncBase.ErrorCodeStatus.ProtocolError:
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), ex, false)
					{
						ErrorStringForProtocolLogger = "Sync.ProtocolError2"
					};
				case SyncBase.ErrorCodeStatus.ServerError:
					throw new AirSyncPermanentException(StatusCode.Sync_ServerError, SyncCommand.GetServerErrorXml(), ex, false)
					{
						ErrorStringForProtocolLogger = "Sync.ServerError2"
					};
				case SyncBase.ErrorCodeStatus.ClientServerConversion:
				case SyncBase.ErrorCodeStatus.Conflict:
					break;
				case SyncBase.ErrorCodeStatus.ObjectNotFound:
					throw new AirSyncPermanentException(StatusCode.Sync_ObjectNotFound, SyncCommand.GetObjectNotFoundErrorXml(), ex, false)
					{
						ErrorStringForProtocolLogger = "Sync.ObjectNotFound3"
					};
				default:
					if (globalStatus == SyncBase.ErrorCodeStatus.InvalidCollection)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_FolderHierarchyRequired, SyncCommand.GetFolderHierarchySyncRequiredXml(), ex, false)
						{
							ErrorStringForProtocolLogger = "Sync.InvalidCollection"
						};
					}
					break;
				}
				throw;
			}
			finally
			{
				try
				{
					this.LogRequest();
				}
				catch (Exception ex3)
				{
					AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.RequestsTracer, null, "Failed to log logical request: {0}", ex3);
					if (!AirSyncUtility.HandleNonCriticalException(ex3, true))
					{
						throw;
					}
				}
				this.DisposeOpenCollections();
				this.SaveSyncStatusData();
				if (this.hanging)
				{
					this.PrepareToHang();
				}
			}
			return result;
		}

		private void SaveSyncStatusData()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.SaveSyncStatusData");
			AirSyncDiagnostics.Assert(base.SyncStateStorage != null, "SyncStateStorage should not be null here.", new object[0]);
			AirSyncDiagnostics.Assert(base.SyncStatusSyncData != null, "SyncStatusSyncState should not be null here.", new object[0]);
			base.SyncStatusSyncData.LastSyncAttemptTime = new ExDateTime?(this.syncAttemptTime);
			if (this.syncSuccessTime != null)
			{
				base.SyncStatusSyncData.LastSyncSuccessTime = new ExDateTime?(this.syncSuccessTime.Value);
			}
			base.SyncStatusSyncData.LastSyncUserAgent = base.EffectiveUserAgent;
			base.SyncStatusSyncData.SaveAndDispose();
			base.SyncStatusSyncData = null;
		}

		private void ReadXmlRequest(List<XmlNode> itemLevelProtocolErrorNodes, out List<string> collectionsInOrder, out uint waitTime)
		{
			this.ReadXmlRequest(itemLevelProtocolErrorNodes, null, false, out collectionsInOrder, out waitTime);
		}

		private void ReadXmlRequest(List<XmlNode> itemLevelProtocolErrorNodes, HashSet<string> collectionIdsToMatch, bool partialRequest, out List<string> collectionsInOrder)
		{
			uint num;
			this.ReadXmlRequest(itemLevelProtocolErrorNodes, collectionIdsToMatch, partialRequest, out collectionsInOrder, out num);
		}

		private void ReadXmlRequest(List<XmlNode> itemLevelProtocolErrorNodes, HashSet<string> collectionIdsToMatch, bool partialRequest, out List<string> collectionsInOrder, out uint waitTime)
		{
			int arg = Interlocked.Increment(ref this.readCount);
			AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.RequestsTracer, this, "Sync.ReadXmlRequest.  Read count: {0}", arg);
			base.GlobalWindowSize = ((this.requestedGlobalWindowSize > 0U) ? this.requestedGlobalWindowSize : ((base.Version > 120) ? 100U : 512U));
			collectionsInOrder = new List<string>(6);
			XmlNode xmlNode = null;
			Dictionary<string, XmlNode> nameToNode = new Dictionary<string, XmlNode>(10);
			waitTime = 0U;
			if (base.XmlRequest != null)
			{
				XmlNode xmlRequest = base.XmlRequest;
				xmlNode = xmlRequest["Collections", "AirSync:"];
				XmlNode xmlNode2;
				if (xmlNode != null)
				{
					xmlNode2 = xmlNode.NextSibling;
				}
				else
				{
					xmlNode2 = xmlRequest.FirstChild;
				}
				if (xmlNode2 != null && "Wait" == xmlNode2.LocalName)
				{
					if (!uint.TryParse(xmlNode2.InnerText, out waitTime))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), null, false)
						{
							ErrorStringForProtocolLogger = "BadWaitValue(" + xmlNode2.InnerText + ")"
						};
					}
					waitTime *= 60U;
					if ((ulong)waitTime < (ulong)((long)GlobalSettings.HeartbeatInterval.LowInterval))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidWaitTime, SyncCommand.GetMinWaitExceededXml(false), null, false)
						{
							ErrorStringForProtocolLogger = "WaitIsLessThanHBI"
						};
					}
					if ((ulong)waitTime > (ulong)((long)GlobalSettings.HeartbeatInterval.HighInterval))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidWaitTime, SyncCommand.GetMaxWaitExceededXml(false), null, false)
						{
							ErrorStringForProtocolLogger = "WaitIsGreaterThanHBI"
						};
					}
					this.hangSpecified = true;
					this.shouldHang = true;
					xmlNode2 = xmlNode2.NextSibling;
				}
				if (xmlNode2 != null && xmlNode2.LocalName == "HeartbeatInterval")
				{
					if (!uint.TryParse(xmlNode2.InnerText, out waitTime))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), null, false)
						{
							ErrorStringForProtocolLogger = "BadHBI(" + xmlNode2.InnerText + ")"
						};
					}
					if ((ulong)waitTime < (ulong)((long)GlobalSettings.HeartbeatInterval.LowInterval))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidWaitTime, SyncCommand.GetMinWaitExceededXml(true), null, false)
						{
							ErrorStringForProtocolLogger = "MinHBIIsGreaterThanWait"
						};
					}
					if ((ulong)waitTime > (ulong)((long)GlobalSettings.HeartbeatInterval.HighInterval))
					{
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidWaitTime, SyncCommand.GetMaxWaitExceededXml(true), null, false)
						{
							ErrorStringForProtocolLogger = "MaxHBIIsLessThanWait"
						};
					}
					this.hangSpecified = true;
					this.shouldHang = true;
					xmlNode2 = xmlNode2.NextSibling;
				}
				if (xmlNode2 != null && "WindowSize" == xmlNode2.LocalName)
				{
					uint num;
					if (!uint.TryParse(xmlNode2.InnerText, out num))
					{
						base.GlobalStatus = SyncBase.ErrorCodeStatus.ProtocolError;
						throw new AirSyncPermanentException(false)
						{
							ErrorStringForProtocolLogger = "BadWindowSizeInSyncCmd"
						};
					}
					if (num == 0U || num > 512U)
					{
						num = 512U;
					}
					base.GlobalWindowSize = num;
					this.requestedGlobalWindowSize = num;
					xmlNode2 = xmlNode2.NextSibling;
				}
				if (xmlNode2 != null && "Partial" == xmlNode2.LocalName)
				{
					base.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.PartialRequest, true);
					if (!string.IsNullOrEmpty(xmlNode2.InnerText))
					{
						base.GlobalStatus = SyncBase.ErrorCodeStatus.ProtocolError;
						throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ProtocolError, null, false)
						{
							ErrorStringForProtocolLogger = "BadPartialValueInSyncCmd"
						};
					}
					partialRequest = true;
					xmlRequest.RemoveChild(xmlNode2);
				}
				if (xmlNode != null)
				{
					if (xmlNode.ChildNodes.Count > GlobalSettings.MaxNumOfFolders)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_TooManyFolders, SyncCommand.GetMaxFoldersExceededXml(), null, false)
						{
							ErrorStringForProtocolLogger = "TooManyFldrsToSync"
						};
					}
					if (xmlNode.ChildNodes.Count > 1 && base.Version < 121)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, this.GetValidationErrorXml(), null, false)
						{
							ErrorStringForProtocolLogger = "TooManyFldrsToSyncV25"
						};
					}
					using (IEnumerator enumerator = xmlNode.ChildNodes.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							XmlNode collectionNode = (XmlNode)obj;
							this.ParseCollection(itemLevelProtocolErrorNodes, collectionIdsToMatch, collectionNode, false, null, nameToNode, collectionsInOrder);
						}
						goto IL_41E;
					}
				}
				xmlNode = base.XmlRequest.OwnerDocument.CreateElement("Collections", "AirSync:");
				xmlRequest.InsertBefore(xmlNode, xmlRequest.FirstChild);
			}
			IL_41E:
			if (partialRequest)
			{
				XmlDocument xmlDocument;
				if (!this.LoadCachedRequest(true, out xmlDocument) || xmlDocument.FirstChild == null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.ReadXmlRequest] We have a partial request, were unable to load the cached request (or cached request was empty).");
					throw new AirSyncPermanentException(StatusCode.Sync_InvalidParameters, this.GetInvalidParametersXml(), null, false)
					{
						ErrorStringForProtocolLogger = "CantLoadPartialCache"
					};
				}
				XmlNode xmlNode3 = xmlDocument.FirstChild["Collections", "AirSync:"];
				if (xmlNode3.ChildNodes.Count == 0)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.ReadXmlRequest] We have a partial request, but the cached request had no child collections.  Failing with InvalidParameters.");
					throw new AirSyncPermanentException(StatusCode.Sync_InvalidParameters, this.GetInvalidParametersXml(), null, false)
					{
						ErrorStringForProtocolLogger = "NoCollectionInPartialCache"
					};
				}
				foreach (object obj2 in xmlNode3.ChildNodes)
				{
					XmlNode collectionNode2 = (XmlNode)obj2;
					if (collectionIdsToMatch != null && collectionIdsToMatch.Count == collectionsInOrder.Count)
					{
						break;
					}
					this.ParseCollection(itemLevelProtocolErrorNodes, collectionIdsToMatch, collectionNode2, true, xmlNode, nameToNode, collectionsInOrder);
				}
				if (collectionsInOrder.Count > GlobalSettings.MaxNumOfFolders)
				{
					AirSyncDiagnostics.TraceDebug<int>(ExTraceGlobals.RequestsTracer, this, "[SyncCommand.ReadXmlRequest] We have a partial request, but cached request has too many collections to sync.  Count: {0}", collectionsInOrder.Count);
					throw new AirSyncPermanentException(StatusCode.Sync_TooManyFolders, SyncCommand.GetMaxFoldersExceededXml(), null, false)
					{
						ErrorStringForProtocolLogger = "TooManyFldrsToSync2"
					};
				}
			}
		}

		protected bool IsHangingRequest()
		{
			if (base.XmlRequest != null)
			{
				bool flag = this.IsHangingRequest(base.XmlRequest);
				base.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.SyncHangingHint, SyncRequestWasHangingCache.BuildDiagnosticsString(flag ? "H" : "N", "-", "-"));
				return flag;
			}
			if (base.Version < 121)
			{
				return false;
			}
			bool flag2 = false;
			if (SyncRequestWasHangingCache.TryGet(base.Context.User.MailboxGuid, base.Context.Request.DeviceIdentity, out flag2))
			{
				base.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.SyncHangingHint, SyncRequestWasHangingCache.BuildDiagnosticsString("E", flag2 ? "H" : "N", "-"));
				return flag2;
			}
			base.OpenMailboxSession(base.User);
			base.OpenSyncStorage(true);
			XmlDocument xmlDocument;
			if (this.LoadCachedRequest(false, out xmlDocument) && xmlDocument != null)
			{
				base.Request.XmlDocument = xmlDocument;
				base.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.LoadedCachedRequest, true);
				XmlElement documentElement = xmlDocument.DocumentElement;
				flag2 = this.IsHangingRequest(documentElement);
				SyncRequestWasHangingCache.Set(base.Context.User.MailboxGuid, base.Context.Request.DeviceIdentity, flag2);
				base.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.SyncHangingHint, SyncRequestWasHangingCache.BuildDiagnosticsString("E", "M", flag2 ? "H" : "N"));
				return flag2;
			}
			base.Context.SetDiagnosticValue(AirSyncConditionalHandlerSchema.SyncHangingHint, SyncRequestWasHangingCache.BuildDiagnosticsString("E", "M", "M"));
			return false;
		}

		private bool IsHangingRequest(XmlElement requestElement)
		{
			if (requestElement == null)
			{
				throw new ArgumentNullException("requestElement", "requestElement cannot be null");
			}
			XmlNode xmlNode = requestElement["Wait"];
			return xmlNode != null || requestElement["HeartbeatInterval"] != null;
		}

		private void ParseCollection(List<XmlNode> itemLevelProtocolErrorNodes, HashSet<string> collectionIdsToMatch, XmlNode collectionNode, bool nodeFromCache, XmlNode requestCollectionsNode, Dictionary<string, XmlNode> nameToNode, List<string> collectionsInOrder)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.ParseCollection");
			SyncCollection syncCollection = null;
			SyncCollection syncCollection2 = null;
			try
			{
				try
				{
					syncCollection = SyncCollection.ParseCollection(itemLevelProtocolErrorNodes, collectionNode, base.Version, base.MailboxSession);
					syncCollection2 = syncCollection;
				}
				catch (AirSyncPermanentException)
				{
					base.GlobalStatus = SyncBase.ErrorCodeStatus.ProtocolError;
					throw;
				}
				if (collectionIdsToMatch == null || collectionIdsToMatch.Contains(syncCollection.CollectionId))
				{
					if (base.Collections.ContainsKey(syncCollection.InternalName))
					{
						if (!nodeFromCache)
						{
							throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ProtocolError, null, false)
							{
								ErrorStringForProtocolLogger = "TooManyCollectionsToSync"
							};
						}
						XmlNode xmlNode = nameToNode[syncCollection.InternalName];
						requestCollectionsNode.RemoveChild(xmlNode);
						requestCollectionsNode.AppendChild(xmlNode);
						collectionsInOrder.Remove(syncCollection.InternalName);
						collectionsInOrder.Add(syncCollection.InternalName);
					}
					else
					{
						if (syncCollection.CollectionId == null && base.Version >= 121)
						{
							throw new AirSyncPermanentException(HttpStatusCode.OK, StatusCode.Sync_ProtocolError, null, false)
							{
								ErrorStringForProtocolLogger = "NamedSyncNotAllowed"
							};
						}
						collectionsInOrder.Add(syncCollection.InternalName);
						base.Collections[syncCollection.InternalName] = syncCollection;
						nameToNode[syncCollection.InternalName] = collectionNode;
						syncCollection2 = null;
						if (nodeFromCache && !this.hanging)
						{
							XmlNode xmlNode2 = base.Request.XmlDocument.ImportNode(collectionNode, true);
							requestCollectionsNode.AppendChild(xmlNode2);
							syncCollection.CollectionNode = xmlNode2;
						}
						base.ProtocolLogger.SetValue(syncCollection.InternalName, PerFolderProtocolLoggerData.ClientSyncKey, syncCollection.SyncKeyString);
					}
				}
			}
			finally
			{
				if (syncCollection2 != null)
				{
					syncCollection2.Dispose();
					syncCollection2 = null;
				}
			}
		}

		private void ConvertRequestsAndApply(SyncCollection collection)
		{
			AirSyncDiagnostics.TraceInfo<SyncCollection>(ExTraceGlobals.RequestsTracer, this, "Sync.ConvertRequestsAndApply {0}", collection);
			collection.SetAllSchemaConverterOptions(SyncCommand.emptyPropertyCollection, base.VersionFactory);
			foreach (SyncCommandItem syncCommandItem in collection.ClientCommands)
			{
				if (syncCommandItem.ClassType == null)
				{
					syncCommandItem.ClassType = collection.ClassType;
				}
				if (!collection.IsClassLegal(syncCommandItem.ClassType))
				{
					syncCommandItem.Status = "6";
					collection.Responses.Add(syncCommandItem);
				}
				else
				{
					if (syncCommandItem.CommandType != SyncBase.SyncCommandType.Add)
					{
						syncCommandItem.ServerId = collection.TryGetSyncItemIdFromStringId(syncCommandItem.SyncId);
						if (syncCommandItem.ServerId == null)
						{
							if (syncCommandItem.CommandType != SyncBase.SyncCommandType.Delete)
							{
								syncCommandItem.Status = "8";
								collection.Responses.Add(syncCommandItem);
								goto IL_A24;
							}
							goto IL_A24;
						}
					}
					switch (syncCommandItem.CommandType)
					{
					case SyncBase.SyncCommandType.Add:
						break;
					case SyncBase.SyncCommandType.Change:
					case SyncBase.SyncCommandType.Fetch:
					{
						string classFromISyncItemId = collection.GetClassFromISyncItemId(syncCommandItem.ServerId, base.VersionFactory);
						if (classFromISyncItemId != null)
						{
							syncCommandItem.ClassType = classFromISyncItemId;
						}
						break;
					}
					case SyncBase.SyncCommandType.Delete:
					case SyncBase.SyncCommandType.SoftDelete:
						goto IL_12F;
					default:
						goto IL_12F;
					}
					if (!collection.SelectSchemaConverterByAirsyncClass(syncCommandItem.ClassType))
					{
						syncCommandItem.Status = "4";
						collection.Responses.Add(syncCommandItem);
						goto IL_A24;
					}
					IL_12F:
					bool flag = false;
					switch (syncCommandItem.CommandType)
					{
					case SyncBase.SyncCommandType.Add:
					{
						ConflictResult conflictResult = collection.FolderSync.DetectConflict(null, syncCommandItem.ClientAddId, ChangeType.Add);
						if (ConflictResult.AcceptClientChange == conflictResult)
						{
							try
							{
								try
								{
									if (syncCommandItem.ClientAddId == null || syncCommandItem.ServerId != null)
									{
										throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false);
									}
									syncCommandItem.Item = collection.CreateSyncItem(syncCommandItem);
									syncCommandItem.Status = "1";
									if (syncCommandItem.SendEnabled)
									{
										collection.ConvertClientToServerObjectAndSendIfNeeded(syncCommandItem, true);
										base.UpdateRecipientInfoCache(((MessageItem)syncCommandItem.Item.NativeItem).Recipients, null);
										base.ProtocolLogger.IncrementValue(collection.InternalName, PerFolderProtocolLoggerData.ClientSends);
									}
									else
									{
										uint globalWindowSize = base.GlobalWindowSize;
										syncCommandItem.ServerId = collection.ConvertClientToServerObjectAndSave(syncCommandItem, ref globalWindowSize, ref flag);
										base.GlobalWindowSize = globalWindowSize;
										collection.Responses.Add(syncCommandItem);
										if (syncCommandItem.Status == "1" && syncCommandItem.ServerId != null && !flag)
										{
											collection.FolderSync.RecordClientOperation(syncCommandItem);
										}
										base.ProtocolLogger.IncrementValue(collection.InternalName, PerFolderProtocolLoggerData.ClientAdds);
									}
								}
								catch (SaveConflictException innerException)
								{
									AirSyncCounters.NumberOfConflictingConcurrentSync.Increment();
									AirSyncPermanentException ex = (base.Version >= 121) ? new AirSyncPermanentException(StatusCode.Sync_Retry, SyncCommand.GetRetryErrorXml(), innerException, false) : new AirSyncPermanentException(HttpStatusCode.ServiceUnavailable, StatusCode.None, innerException, false);
									ex.ErrorStringForProtocolLogger = "SaveConflict";
									throw ex;
								}
								catch (IrresolvableConflictException innerException2)
								{
									AirSyncCounters.NumberOfConflictingConcurrentSync.Increment();
									AirSyncPermanentException ex2 = (base.Version >= 121) ? new AirSyncPermanentException(StatusCode.Sync_Retry, SyncCommand.GetRetryErrorXml(), innerException2, false) : new AirSyncPermanentException(HttpStatusCode.ServiceUnavailable, StatusCode.None, innerException2, false);
									ex2.ErrorStringForProtocolLogger = "EntitySaveConflict";
									throw ex2;
								}
								catch (Exception ex3)
								{
									if (syncCommandItem.Item != null && syncCommandItem.Item.NativeItem != null)
									{
										try
										{
											syncCommandItem.Item.Load();
											object obj = ((Item)syncCommandItem.Item.NativeItem).TryGetProperty(ItemSchema.Id);
											if (obj is PropertyError)
											{
												AirSyncDiagnostics.TraceError(ExTraceGlobals.ConversionTracer, this, "Could not retrieve Id for the item. {0}", new object[]
												{
													syncCommandItem.Item.NativeItem
												});
											}
											else
											{
												StoreObjectId[] ids = new StoreObjectId[]
												{
													((VersionedId)obj).ObjectId
												};
												base.MailboxSession.Delete(DeleteItemFlags.SoftDelete, ids);
											}
										}
										catch (LocalizedException ex4)
										{
											base.PartialFailure = true;
											AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex4);
											AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.ConversionTracer, this, "An exception was thrown while deleting an item that failed conversion. Ex {0}", arg);
										}
									}
									if (!SyncCommand.IsItemSyncTolerableException(ex3) && !(ex3 is SaveConflictException) && !(ex3 is InvalidRecipientsException))
									{
										throw;
									}
									base.PartialFailure = true;
									AirSyncUtility.ExceptionToStringHelper arg2 = new AirSyncUtility.ExceptionToStringHelper(ex3);
									AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.ConversionTracer, this, "Sync-tolerable Item conversion Exception was thrown. Location ConvertRequestsAndApply1.\r\n{0}", arg2);
									if (ex3 is InvalidRecipientsException)
									{
										syncCommandItem.Status = "185";
									}
									else
									{
										syncCommandItem.Status = "6";
									}
									collection.Responses.Add(syncCommandItem);
									base.ProtocolLogger.IncrementValue(collection.InternalName, syncCommandItem.SendEnabled ? PerFolderProtocolLoggerData.ClientFailedToSend : PerFolderProtocolLoggerData.ClientFailedToConvert);
									base.ProtocolLogger.IncrementValue(ProtocolLoggerData.NumErrors);
									if (base.MailboxLogger != null)
									{
										base.MailboxLogger.SetData(MailboxLogDataName.SyncCommand_ConvertRequestsAndApply_Add_Exception, ex3);
									}
								}
								break;
							}
							finally
							{
								if (syncCommandItem.Item != null)
								{
									syncCommandItem.Dispose();
								}
							}
						}
						syncCommandItem.Status = "7";
						collection.Responses.Add(syncCommandItem);
						break;
					}
					case SyncBase.SyncCommandType.Change:
					{
						ConflictResult conflictResult = collection.FolderSync.DetectConflict(syncCommandItem.ServerId, null, ChangeType.Change);
						UpdateClientOperationResult updateClientOperationResult = collection.FolderSync.UpdateClientOperation(syncCommandItem);
						if (updateClientOperationResult == UpdateClientOperationResult.ClientStateMissingObject && conflictResult == ConflictResult.RejectClientChange)
						{
							if (!(collection.ClassType == "Email"))
							{
								syncCommandItem.Status = "7";
								collection.Responses.Add(syncCommandItem);
								break;
							}
							flag = true;
							conflictResult = ConflictResult.AcceptClientChange;
						}
						if (conflictResult == ConflictResult.RejectClientChange)
						{
							try
							{
								using (syncCommandItem.Item = collection.BindToSyncItem(syncCommandItem.ServerId, true))
								{
									collection.SelectSchemaConverterByAirsyncClass(syncCommandItem.ClassType);
									collection.SetSchemaOptionsConvertServerToClient(base.Context.Request.DeviceIdentity.DeviceType, base.VersionFactory);
									bool flag2 = collection.HasSchemaPropertyChanged(syncCommandItem.Item, syncCommandItem.ChangeTrackingInformation, base.XmlResponse, base.MailboxLogger);
									collection.SetSchemaConverterOptions(SyncCommand.emptyPropertyCollection, base.VersionFactory);
									if (!flag2)
									{
										conflictResult = ConflictResult.AcceptClientChange;
									}
									else if (collection.ClassType == "Email")
									{
										if (base.Version <= 25)
										{
											break;
										}
										if (collection.ClientConflictResolutionPolicy == ConflictResolutionPolicy.ClientWins)
										{
											conflictResult = ConflictResult.AcceptClientChange;
											flag = true;
										}
										else
										{
											conflictResult = ConflictResult.RejectClientChange;
										}
									}
								}
							}
							catch (Exception ex5)
							{
								if (!SyncCommand.IsObjectNotFound(ex5))
								{
									throw;
								}
								conflictResult = ConflictResult.ObjectNotFound;
							}
						}
						if (conflictResult == ConflictResult.AcceptClientChange)
						{
							try
							{
								using (syncCommandItem.Item = collection.BindToSyncItem(syncCommandItem.ServerId, true))
								{
									object obj2 = ((Item)syncCommandItem.Item.NativeItem).TryGetProperty(MessageItemSchema.IsDraft);
									if (!(obj2 is PropertyError))
									{
										syncCommandItem.IsDraft = (bool)obj2;
									}
									else
									{
										AirSyncDiagnostics.TraceError<PropertyErrorCode>(ExTraceGlobals.RequestsTracer, this, "SyncCommand::ConvertRequestAndApply:Error retrieving IsDraft from Changed email item. ErrorCode{0}", (obj2 as PropertyError).PropertyErrorCode);
									}
									if (!syncCommandItem.IsDraft && syncCommandItem.ClassType == "Email")
									{
										this.VerifyXmlNodesForEmailChange(syncCommandItem);
									}
									if (syncCommandItem.SendEnabled)
									{
										if (!syncCommandItem.IsDraft)
										{
											throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.Sync_InvalidParameters, null, false);
										}
										collection.ConvertClientToServerObjectAndSendIfNeeded(syncCommandItem, true);
										base.UpdateRecipientInfoCache(((MessageItem)syncCommandItem.Item.NativeItem).Recipients, null);
										syncCommandItem.Status = "1";
										collection.FolderSync.RecordClientOperation(syncCommandItem);
										collection.DeleteId(syncCommandItem.Id);
										base.ProtocolLogger.IncrementValue(collection.InternalName, PerFolderProtocolLoggerData.ClientSends);
									}
									else
									{
										uint globalWindowSize2 = base.GlobalWindowSize;
										syncCommandItem.ServerId = collection.ConvertClientToServerObjectAndSave(syncCommandItem, ref globalWindowSize2, ref flag);
										base.GlobalWindowSize = globalWindowSize2;
										if (!flag && updateClientOperationResult != UpdateClientOperationResult.ClientStateMissingObject)
										{
											collection.FolderSync.RecordClientOperation(syncCommandItem);
										}
										base.ProtocolLogger.IncrementValue(collection.InternalName, PerFolderProtocolLoggerData.ClientChanges);
										if ((syncCommandItem.IsDraft && syncCommandItem.XmlNode.SelectSingleNode("//*[contains(name(), 'Attachments')]") != null) || (syncCommandItem.AddedAttachments != null && syncCommandItem.AddedAttachments.Count > 0))
										{
											syncCommandItem.Status = "1";
											collection.Responses.Add(syncCommandItem);
										}
									}
								}
								break;
							}
							catch (SaveConflictException innerException3)
							{
								AirSyncCounters.NumberOfConflictingConcurrentSync.Increment();
								AirSyncPermanentException ex6 = (base.Version >= 121) ? new AirSyncPermanentException(StatusCode.Sync_Retry, SyncCommand.GetRetryErrorXml(), innerException3, false) : new AirSyncPermanentException(HttpStatusCode.ServiceUnavailable, StatusCode.None, innerException3, false);
								ex6.ErrorStringForProtocolLogger = "SaveConflict2";
								throw ex6;
							}
							catch (Exception ex7)
							{
								if (SyncCommand.IsObjectNotFound(ex7))
								{
									base.ProtocolLogger.IncrementValue(collection.InternalName, PerFolderProtocolLoggerData.ClientFailedToConvert);
									base.ProtocolLogger.IncrementValue(ProtocolLoggerData.NumErrors);
									if (base.MailboxLogger != null)
									{
										base.MailboxLogger.SetData(MailboxLogDataName.SyncCommand_ConvertRequestsAndApply_Change_AcceptClientChange_Exception, ex7);
									}
								}
								else
								{
									if (!SyncCommand.IsItemSyncTolerableException(ex7) && !(ex7 is InvalidRecipientsException))
									{
										throw;
									}
									base.PartialFailure = true;
									AirSyncUtility.ExceptionToStringHelper arg3 = new AirSyncUtility.ExceptionToStringHelper(ex7);
									AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.ConversionTracer, this, "Sync-tolerable Item conversion Exception was thrown. Location ConvertRequestsAndApply2.\r\n{0}", arg3);
									if (ex7 is InvalidRecipientsException)
									{
										syncCommandItem.Status = "185";
									}
									else
									{
										syncCommandItem.Status = "6";
									}
									collection.Responses.Add(syncCommandItem);
									if (base.MailboxLogger != null)
									{
										base.MailboxLogger.SetData(MailboxLogDataName.SyncCommand_ConvertRequestsAndApply_Change_AcceptClientChange_Exception, ex7);
									}
								}
								break;
							}
						}
						if (conflictResult == ConflictResult.ObjectNotFound)
						{
							base.ProtocolLogger.IncrementValue(collection.InternalName, PerFolderProtocolLoggerData.ClientFailedToConvert);
							base.ProtocolLogger.IncrementValue(ProtocolLoggerData.NumErrors);
						}
						else
						{
							syncCommandItem.Status = "7";
							collection.Responses.Add(syncCommandItem);
						}
						break;
					}
					case SyncBase.SyncCommandType.Delete:
					{
						base.AddInteractiveCall();
						ConflictResult conflictResult = collection.FolderSync.DetectConflict(syncCommandItem.ServerId, null, ChangeType.Delete);
						collection.SelectSchemaConverterByAirsyncClass(syncCommandItem.ClassType);
						bool flag3 = true;
						UpdateClientOperationResult updateClientOperationResult = collection.FolderSync.UpdateClientOperation(syncCommandItem);
						if (updateClientOperationResult == UpdateClientOperationResult.ClientStateMissingObject)
						{
							flag3 = false;
						}
						if (ConflictResult.AcceptClientChange == conflictResult)
						{
							try
							{
								OperationResult operationResult = collection.DeleteSyncItem(syncCommandItem, collection.DeletesAsMoves);
								if (operationResult == OperationResult.Failed)
								{
									syncCommandItem.Status = "5";
									collection.Responses.Add(syncCommandItem);
									base.ProtocolLogger.IncrementValue(ProtocolLoggerData.NumErrors);
									break;
								}
								if (flag3)
								{
									collection.FolderSync.RecordClientOperation(syncCommandItem);
								}
								base.ProtocolLogger.IncrementValue(collection.InternalName, PerFolderProtocolLoggerData.ClientDeletes);
								break;
							}
							catch (Exception ex8)
							{
								if (SyncCommand.IsItemSyncTolerableException(ex8))
								{
									base.PartialFailure = true;
									AirSyncUtility.ExceptionToStringHelper arg4 = new AirSyncUtility.ExceptionToStringHelper(ex8);
									AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.ConversionTracer, this, "Sync-tolerable Item conversion Exception was thrown. Location ConvertRequestsAndApply3.\r\n{0}", arg4);
									syncCommandItem.Status = "5";
									collection.Responses.Add(syncCommandItem);
									base.ProtocolLogger.IncrementValue(ProtocolLoggerData.NumErrors);
									if (base.MailboxLogger != null)
									{
										base.MailboxLogger.SetData(MailboxLogDataName.SyncCommand_ConvertRequestsAndApply_Delete_Exception, ex8);
									}
									break;
								}
								throw;
							}
						}
						syncCommandItem.Status = "7";
						collection.Responses.Add(syncCommandItem);
						break;
					}
					case SyncBase.SyncCommandType.Fetch:
						collection.Responses.Add(syncCommandItem);
						collection.ClientFetchedItems[syncCommandItem.ServerId] = syncCommandItem;
						break;
					}
				}
				IL_A24:;
			}
		}

		private void VerifyXmlNodesForEmailChange(SyncCommandItem commandItem)
		{
			foreach (object obj in commandItem.XmlNode)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (!(xmlNode.Name == "Read") && !(xmlNode.Name == "Flag") && !(xmlNode.Name == "Categories"))
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
					{
						ErrorStringForProtocolLogger = string.Format("InvalidXmlNodeForNonDraftEmail:{0}", xmlNode.Name)
					};
				}
			}
		}

		private bool UpdateSyncKey(SyncCollection collection)
		{
			AirSyncDiagnostics.TraceInfo<SyncCollection>(ExTraceGlobals.RequestsTracer, this, "Sync.UpdateSyncKey {0}", collection);
			bool flag = collection.SyncKey == 0U || (collection.GetChanges && collection.HasAddsOrChangesToReturnToClientImmediately);
			bool flag2 = flag || collection.DupeList.Count > 0 || collection.ClientCommands != null || collection.HasServerChanges || (this.hanging && this.collectionIdsWithHangableSynckeyChange != null && this.collectionIdsWithHangableSynckeyChange.Contains(collection.InternalName));
			if (flag2 && !flag && this.shouldHang)
			{
				StoreObjectId storeObjectId = collection.SyncState.TryGetStoreObjectId();
				if (storeObjectId != null)
				{
					this.collectionIdsDelayed.Add(collection.InternalName);
					if (this.Notifier != null)
					{
						this.Notifier.SubscriptionsCannotBeTaken();
					}
				}
				else
				{
					this.shouldHang = false;
				}
			}
			else if (this.shouldHang && (flag || collection.MoreAvailable))
			{
				this.shouldHang = false;
			}
			if (flag2)
			{
				if (!this.hangingCollectionSynckeys.ContainsKey(collection.InternalName))
				{
					collection.ResponseSyncKey = (uint)base.GetNextNumber((int)collection.SyncKey, base.Version >= 121);
				}
				else
				{
					collection.ResponseSyncKey = this.hangingCollectionSynckeys[collection.InternalName];
				}
				collection.SyncState[CustomStateDatumType.RecoverySyncKey] = new UInt32Data(collection.SyncKey);
			}
			else
			{
				collection.ResponseSyncKey = collection.SyncKey;
			}
			collection.SyncState[CustomStateDatumType.SyncKey] = new UInt32Data(collection.ResponseSyncKey);
			return flag2 || collection.MoreAvailable || (collection.Responses != null && collection.Responses.Count > 0) || (collection.ServerChanges != null && collection.ServerChanges.Count > 0);
		}

		private bool SyncTheCollection(SyncCollection collection, bool createSubscription, bool tryNullSync)
		{
			bool result;
			try
			{
				int num = this.syncCollectionSyncs.Increment(collection.InternalName, 1);
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection {0}, createSubscription {1}, tryNullSync: {2}, callCount: {3}", new object[]
				{
					collection,
					createSubscription,
					tryNullSync,
					num
				});
				if (collection.CollectionId != null)
				{
					base.ProtocolLogger.SetValue(collection.InternalName, PerFolderProtocolLoggerData.FolderId, collection.CollectionId);
				}
				this.currentSyncCollection = collection;
				this.ParseSyncKey(collection);
				collection.SetDeviceSettings(this);
				collection.CreateSyncProvider();
				bool flag = collection.ClientCommands != null && collection.ClientCommands.Length != 0;
				flag = (flag || (collection.Responses != null && collection.Responses.Count != 0));
				bool nullSyncAllowed = !base.IsInQuarantinedState || base.IsQuarantineMailAvailable;
				flag = (flag || !tryNullSync || collection.CollectionRequiresSync(false, nullSyncAllowed));
				AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection shouldSyncFolder {0}", flag);
				if (!flag && (base.Version < 121 || !collection.HasOptionsNodes))
				{
					this.FinalizeCollectionDueToNullSync(collection, createSubscription);
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection We have no changes, we can bail out early");
					result = false;
				}
				else
				{
					collection.OpenSyncState(false, base.SyncStateStorage);
					if (base.Context.Request.Version < 160 && collection.FolderType == DefaultFolderType.Drafts)
					{
						collection.LogCollectionData(base.ProtocolLogger);
						if (base.Version < 121 || collection.SyncKey == 0U)
						{
							collection.SetEmptyServerChanges();
							collection.ResponseSyncKey = (uint)((collection.SyncKey == 0U) ? base.GetNextNumber((int)collection.SyncKey, base.Version >= 121) : ((int)collection.SyncKey));
							collection.FinalizeCollectionXmlNode(base.XmlResponse);
							this.collectionsResponseXmlNode.AppendChild(collection.CollectionResponseXmlNode);
						}
						AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "[Sync.SyncTheCollection] Ignoring request to sync drafts.");
						result = (collection.SyncKey == 0U);
					}
					else
					{
						if (collection.HasOptionsNodes)
						{
							try
							{
								collection.ParseSyncOptions();
							}
							catch (AirSyncPermanentException)
							{
								base.GlobalStatus = SyncBase.ErrorCodeStatus.ProtocolError;
								throw;
							}
							if (base.Version >= 121)
							{
								byte[] array = collection.SerializeOptions();
								ByteArrayData byteArrayData = (ByteArrayData)collection.SyncState[CustomStateDatumType.CachedOptionsNode];
								if (byteArrayData == null || !ArrayComparer<byte>.Comparer.Equals(byteArrayData.Data, array))
								{
									collection.SyncState[CustomStateDatumType.CachedOptionsNode] = new ByteArrayData(array);
									collection.OptionsSentAreDifferentForV121AndLater = true;
									AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection OptionsSentAreDifferentForV121AndLater");
								}
								else if (!flag)
								{
									collection.NullSyncWorked = true;
									this.FinalizeCollectionDueToNullSync(collection, createSubscription);
									AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection The client sent up an Options node, it wasn't different");
									return false;
								}
							}
						}
						else if (base.Version >= 121)
						{
							try
							{
								collection.ParseStickyOptions();
							}
							catch (AirSyncPermanentException)
							{
								base.GlobalStatus = SyncBase.ErrorCodeStatus.ProtocolError;
								throw;
							}
						}
						collection.AddDefaultOptions();
						if (collection.SyncKey == 0U)
						{
							collection.GetChanges = collection.AllowGetChangesOnSyncKeyZero();
						}
						collection.InitializeSchemaConverter(base.VersionFactory, base.GlobalInfo);
						bool syncProviderOptions = base.IsQuarantineMailAvailable && (base.GlobalInfo.DeviceAccessStateReason == DeviceAccessStateReason.ExternalCompliance || base.GlobalInfo.DeviceAccessStateReason == DeviceAccessStateReason.ExternalEnrollment);
						collection.SetSyncProviderOptions(syncProviderOptions);
						collection.OpenFolderSync();
						collection.FilterTypeInSyncState = (AirSyncV25FilterTypes)collection.SyncState.GetData<Int32Data, int>(CustomStateDatumType.FilterType, 0);
						collection.ConversationModeInSyncState = collection.SyncState.GetData<BooleanData, bool>(CustomStateDatumType.ConversationMode, false);
						collection.SetFolderSyncOptions(base.VersionFactory, base.IsQuarantineMailAvailable, base.GlobalInfo);
						try
						{
							collection.VerifySyncKey(this.hangingCollectionSynckeys.ContainsKey(collection.InternalName), base.GlobalInfo);
							base.ProtocolLogger.SetValue(collection.InternalName, PerFolderProtocolLoggerData.ClientSyncKey, collection.SyncKey.ToString(CultureInfo.InvariantCulture));
						}
						catch (AirSyncPermanentException innerException)
						{
							if (this.hanging && collection.SyncTypeString == "I")
							{
								AirSyncCounters.NumberOfConflictingConcurrentSync.Increment();
								throw new AirSyncPermanentException(StatusCode.Sync_Retry, SyncCommand.GetRetryErrorXml(), innerException, false);
							}
							throw;
						}
						if (this.hanging && collection.SyncTypeString == "R" && ((SyncCommand.SyncInformationForNotifier)this.Notifier.Information).CollectionIdToSyncType[collection.InternalName] == "S")
						{
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "Sync.SyncTheCollection SubsequentSync");
							AirSyncCounters.NumberOfConflictingConcurrentSync.Increment();
							throw new AirSyncPermanentException(StatusCode.Sync_Retry, SyncCommand.GetRetryErrorXml(), null, false);
						}
						collection.LogCollectionData(base.ProtocolLogger);
						if (!this.hanging || this.collectionIdsWithHangableSynckeyChange == null || !this.collectionIdsWithHangableSynckeyChange.Contains(collection.InternalName))
						{
							collection.CommitOrClearItemIdMapping();
						}
						if (collection.ClientCommands != null && !this.hanging)
						{
							this.ConvertRequestsAndApply(collection);
							if (this.collectionIdsWithHangableSynckeyChange == null)
							{
								this.collectionIdsWithHangableSynckeyChange = new HashSet<string>();
							}
							this.collectionIdsWithHangableSynckeyChange.Add(collection.InternalName);
						}
						if (collection.GetChanges)
						{
							if (base.IsInQuarantinedState && (!base.IsQuarantineMailAvailable || !base.IsInboxFolder(collection.NativeStoreObjectId)))
							{
								AirSyncDiagnostics.TraceInfo<DeviceAccessState>(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection Skip sync for quarantined state. Current AccessState = {0}", base.CurrentAccessState);
							}
							else
							{
								if (createSubscription)
								{
									this.CreateSubscription(collection);
								}
								if (this.shouldHang)
								{
									ItemIdMapping itemIdMapping = (ItemIdMapping)collection.SyncState[CustomStateDatumType.IdMapping];
									itemIdMapping.UseWriteBuffer();
								}
								if (!collection.DupesFilledWindowSize)
								{
									base.GlobalWindowSize -= collection.GetServerChanges(base.GlobalWindowSize, false);
									StoreObjectId storeObjectId = collection.SyncState.TryGetStoreObjectId();
									if (this.shouldHang && storeObjectId != null)
									{
										AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection Save away the store object Id to collection Id map");
										SyncCommand.SyncInformationForNotifier syncInformationForNotifier = (SyncCommand.SyncInformationForNotifier)this.Notifier.Information;
										syncInformationForNotifier.StoreIdToCollectionId[storeObjectId] = collection.InternalName;
										syncInformationForNotifier.StoreIdToType[storeObjectId] = collection.ClassType;
									}
								}
								else
								{
									AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection DupesFilledWindowSize");
								}
							}
						}
						List<SyncCommand.BadItem> list = collection.GenerateCommandsXmlNode(base.XmlResponse, base.VersionFactory, base.Context.Request.DeviceIdentity.DeviceType, base.GlobalInfo, base.ProtocolLogger, base.MailboxLogger);
						if (list != null && list.Count > 0)
						{
							foreach (SyncCommand.BadItem badItem in list)
							{
								ExWatson.SendReport(badItem.Exception, ReportOptions.DoNotCollectDumps | ReportOptions.DoNotFreezeThreads, null);
							}
							if (this.IsBadItemReportingEnabled())
							{
								this.GenerateReport(list);
								AirSyncCounters.NumberOfBadItemReportsGenerated.Increment();
							}
						}
						collection.HaveChanges = this.UpdateSyncKey(collection);
						if (!this.hanging)
						{
							this.shouldHang = (!collection.GenerateResponsesXmlNode(base.XmlResponse, base.VersionFactory, base.Context.Request.DeviceIdentity.DeviceType, base.GlobalInfo, base.ProtocolLogger, base.MailboxLogger) && this.shouldHang);
						}
						if (base.Version < 121 || (base.Version >= 121 && collection.HaveChanges))
						{
							collection.FinalizeCollectionXmlNode(base.XmlResponse);
							this.collectionsResponseXmlNode.AppendChild(collection.CollectionResponseXmlNode);
						}
						if (!collection.HaveChanges && base.IsQuarantineMailAvailable)
						{
							AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection no changes but there should be quarantine mail");
							StoreObjectId folderId = collection.SyncState.TryGetStoreObjectId();
							if (base.IsInboxFolder(folderId))
							{
								base.GlobalInfo.ABQMailState = ABQMailState.MailNotSent;
							}
						}
						AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "Sync.SyncTheCollection returns HaveChanges:{0}", collection.HaveChanges);
						result = collection.HaveChanges;
					}
				}
			}
			catch (AirSyncPermanentException ex)
			{
				base.ProtocolLogger.AppendValue(ProtocolLoggerData.Error, string.Format("FId:{0}:{1}", collection.CollectionId, ex.ErrorStringForProtocolLogger));
				base.ProtocolLogger.IncrementValue(ProtocolLoggerData.NumErrors);
				base.PartialFailure = true;
				if (collection.Status == SyncBase.ErrorCodeStatus.Success)
				{
					throw;
				}
				if (collection.Status == SyncBase.ErrorCodeStatus.ProtocolError || collection.Status == SyncBase.ErrorCodeStatus.ObjectNotFound || collection.Status == SyncBase.ErrorCodeStatus.InvalidCollection)
				{
					if (collection.Status != SyncBase.ErrorCodeStatus.ObjectNotFound || !(collection is RecipientInfoCacheSyncCollection))
					{
						base.GlobalStatus = collection.Status;
						throw;
					}
					collection.Status = SyncBase.ErrorCodeStatus.Success;
					collection.HasBeenSaved = true;
					collection.FinalizeCollectionXmlNode(base.XmlResponse);
					this.collectionsResponseXmlNode.AppendChild(collection.CollectionResponseXmlNode);
					result = false;
				}
				else
				{
					AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex);
					AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "Exception thrown while processing a collection\r\n{0}\r\nSkipping to next collection.", arg);
					collection.FinalizeCollectionXmlNode(base.XmlResponse);
					this.collectionsResponseXmlNode.AppendChild(collection.CollectionResponseXmlNode);
					this.shouldHang = false;
					if (this.hanging)
					{
						base.GlobalStatus = SyncBase.ErrorCodeStatus.ServerError;
						throw new AirSyncPermanentException(StatusCode.Sync_Retry, SyncCommand.GetRetryErrorXml(), ex, false);
					}
					result = true;
				}
			}
			catch (ConversionException ex2)
			{
				base.PartialFailure = true;
				if (collection.Status == SyncBase.ErrorCodeStatus.Success)
				{
					throw;
				}
				if (collection.Status == SyncBase.ErrorCodeStatus.ProtocolError)
				{
					base.GlobalStatus = collection.Status;
					throw;
				}
				AirSyncUtility.ExceptionToStringHelper arg2 = new AirSyncUtility.ExceptionToStringHelper(ex2);
				AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "Exception thrown while procesing a collection\r\n{0}\r\nSkipping to next collection.", arg2);
				collection.FinalizeCollectionXmlNode(base.XmlResponse);
				this.collectionsResponseXmlNode.AppendChild(collection.CollectionResponseXmlNode);
				this.shouldHang = false;
				if (this.hanging)
				{
					base.GlobalStatus = SyncBase.ErrorCodeStatus.ServerError;
					base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ConversionException");
					throw new AirSyncPermanentException(StatusCode.Sync_Retry, SyncCommand.GetRetryErrorXml(), ex2, false);
				}
				result = true;
			}
			finally
			{
				base.ProtocolLogger.SetValue(collection.InternalName, PerFolderProtocolLoggerData.PerFolderStatus, (int)collection.Status);
				base.ProtocolLogger.SetValue(collection.InternalName, PerFolderProtocolLoggerData.ServerSyncKey, collection.ResponseSyncKey);
				if (collection.SyncState != null)
				{
					if (!collection.SyncState.IsColdStateDeserialized())
					{
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.SyncStateKbLeftCompressed, (int)collection.SyncState.GetColdStateCompressedSize() >> 10);
						AirSyncCounters.SyncStateKbLeftCompressed.IncrementBy(collection.SyncState.GetColdStateCompressedSize() >> 10);
					}
					base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.SyncStateKb, (int)collection.SyncState.GetTotalCompressedSize() >> 10);
					AirSyncCounters.SyncStateKbTotal.IncrementBy(collection.SyncState.GetTotalCompressedSize() >> 10);
				}
			}
			return result;
		}

		private void FinalizeCollectionDueToNullSync(SyncCollection collection, bool createSubscription)
		{
			AirSyncDiagnostics.TraceInfo<SyncCollection, bool>(ExTraceGlobals.RequestsTracer, this, "Sync.FinalizeCollectionDueToNullSync {0}, createSubscription:{1}", collection, createSubscription);
			collection.FilterTypeInSyncState = collection.FilterType;
			collection.ConversationModeInSyncState = collection.ConversationMode;
			collection.SyncTypeString = "S";
			base.ProtocolLogger.SetProviderSyncType(collection.CollectionId, ProviderSyncType.N);
			collection.LogCollectionData(base.ProtocolLogger);
			collection.ResponseSyncKey = collection.SyncKey;
			if (base.Version < 121)
			{
				collection.SetEmptyServerChanges();
				collection.FinalizeCollectionXmlNode(base.XmlResponse);
				this.collectionsResponseXmlNode.AppendChild(collection.CollectionResponseXmlNode);
				return;
			}
			if (collection.GetChanges && collection.SupportsSubscriptions)
			{
				if (createSubscription)
				{
					this.CreateSubscription(collection);
				}
				if (!base.IsInQuarantinedState)
				{
					StoreObjectId nativeStoreObjectId = collection.NativeStoreObjectId;
					if (nativeStoreObjectId == null)
					{
						this.shouldHang = false;
					}
					if (this.shouldHang)
					{
						SyncCommand.SyncInformationForNotifier syncInformationForNotifier = (SyncCommand.SyncInformationForNotifier)this.Notifier.Information;
						syncInformationForNotifier.StoreIdToCollectionId[nativeStoreObjectId] = collection.InternalName;
						syncInformationForNotifier.StoreIdToType[nativeStoreObjectId] = collection.ClassType;
					}
				}
			}
		}

		private void GenerateReport(List<SyncCommand.BadItem> itemFailureList)
		{
			AirSyncDiagnostics.TraceInfo<int>(ExTraceGlobals.RequestsTracer, this, "Sync.GenerateReport itemFailureList.Count:{0}", itemFailureList.Count);
			string deviceId = base.Context.Request.DeviceIdentity.DeviceId;
			string deviceType = base.Context.Request.DeviceIdentity.DeviceType;
			CultureInfo preferedCulture = base.MailboxSession.PreferedCulture;
			int capacity = 2048;
			if (GlobalSettings.BadItemIncludeStackTrace)
			{
				capacity = 25600;
			}
			StringBuilder stringBuilder = new StringBuilder(capacity);
			string text = string.Format(base.Request.Culture, Strings.ReportSubject.ToString(preferedCulture), new object[]
			{
				deviceType,
				itemFailureList.Count.ToString(base.Request.Culture)
			});
			stringBuilder.Append("<html><head>   <style>a:link{   color: rgb(51,153,255); }a:visited{   color: rgb(51,102,204); } a:active {   color: rgb(255,153,0);}table, td, tr{color: #000000;   border-width: 0in;   font-size:x-small;   vertical-align: top}body{   font-family: Tahoma;   background-color: rgb(255,255,255);   color: #000000;   font-size:x-small;   width: 600px}p{   margin:0in; }h1{   font-family: Arial;   color: #000066;   margin: 0in;    font-size: medium; font-weight:bold}.footer{   font-family:Arial; font-size:smaller;color:#808080 }.header{   color: #808080; font-size:x-small;border-width: 0in; font-weight:bold; text-align:left;}.timezone{   color: #808080}</style></head><body>   <p> <span style='font-size:10.0pt;font-family:Tahoma; color:red'>");
			if (GlobalSettings.BadItemIncludeEmailToText)
			{
				stringBuilder.Append(Strings.ReportForward1.ToString(preferedCulture));
				stringBuilder.Append("</p><p>&nbsp;</p><p>");
				stringBuilder.Append(Strings.ReportForward2.ToString(preferedCulture));
				stringBuilder.Append(Strings.ReportForward3.ToString(preferedCulture));
				stringBuilder.Append(Strings.ReportForward4.ToString(preferedCulture));
				stringBuilder.Append(Strings.ReportForward5.ToString(preferedCulture));
				stringBuilder.Append("</p><p>&nbsp;</p><p><a style='color:#0099FF' href=\"http://watson.microsoft.com/dw/dcp.asp\">");
				stringBuilder.Append(Strings.ReportForward6.ToString(preferedCulture));
				stringBuilder.Append("</a></p><p>&nbsp;</p><p>");
				stringBuilder.Append(string.Format(base.Request.Culture, Strings.ReportForward7.ToString(preferedCulture), new object[]
				{
					GlobalSettings.BadItemEmailToText
				}));
				stringBuilder.Append("</span><hr/></p>   <h1>");
			}
			else
			{
				stringBuilder.Append("</span></p>   <h1>");
			}
			stringBuilder.Append("</h1><p class=\"Header\"><b>");
			if (GlobalSettings.BadItemIncludeStackTrace)
			{
				stringBuilder.Append(Strings.ReportDebugInfo.ToString(preferedCulture));
				stringBuilder.Append("</b><br/>");
				stringBuilder.Append(Strings.ReportDeviceId.ToString(preferedCulture));
				stringBuilder.Append(deviceId);
				stringBuilder.Append("<br/>");
				stringBuilder.Append(Strings.ReportAssemblyInfo.ToString(preferedCulture));
				object[] customAttributes = typeof(SyncCommand).Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					AssemblyFileVersionAttribute assemblyFileVersionAttribute = (AssemblyFileVersionAttribute)customAttributes[0];
					stringBuilder.Append(AirSyncUtility.HtmlEncode(Regex.Replace(assemblyFileVersionAttribute.Version, "0*([0-9]+)", "$1"), true));
				}
				stringBuilder.Append("<br/>");
				stringBuilder.Append(Strings.ReportSyncInfo.ToString(preferedCulture));
				stringBuilder.Append(AirSyncUtility.HtmlEncode(((double)base.Context.Request.Version / 10.0).ToString(preferedCulture), true));
				stringBuilder.Append("<br/>");
				stringBuilder.Append(Strings.ReportMailboxInfo.ToString(preferedCulture));
				if (string.IsNullOrEmpty(SyncCommand.mailBoxBuildVersion))
				{
					int serverVersion = base.MailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion;
					int num = serverVersion & 32767;
					int num2 = serverVersion >> 16 & 63;
					int num3 = serverVersion >> 22 & 63;
					SyncCommand.mailBoxBuildVersion = string.Concat(new object[]
					{
						num3,
						".",
						num2,
						".",
						num,
						".0"
					});
				}
				stringBuilder.Append(AirSyncUtility.HtmlEncode(SyncCommand.mailBoxBuildVersion, true));
				stringBuilder.Append("<br/>");
				stringBuilder.Append(Strings.ReportCASInfo.ToString(preferedCulture));
				stringBuilder.Append(AirSyncUtility.HtmlEncode(Environment.MachineName, true));
				stringBuilder.Append("</p><hr/><h1>");
			}
			else
			{
				stringBuilder.Append("</b></p><h1>");
			}
			stringBuilder.Append(text);
			stringBuilder.Append("</h1>   <p>");
			stringBuilder.Append(Strings.ReportPrefix.ToString(preferedCulture));
			stringBuilder.Append("</p>   <p>       &nbsp;</p>");
			int num4 = 0;
			foreach (SyncCommand.BadItem badItem in itemFailureList)
			{
				if (badItem.ClassType == "IPM.Note.Exchange.ActiveSync.Report")
				{
					AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_BadItemReportConversionException, new string[]
					{
						badItem.Subject,
						badItem.CreationTime
					});
				}
				else
				{
					num4++;
					stringBuilder.Append("   <table border=\"0\" width=\"615\">       <tr>           <td width=\"29\">               &nbsp;</td>           <td class=\"header\" width=\"132\">               ");
					stringBuilder.Append(Strings.ReportItemFolder.ToString(preferedCulture));
					stringBuilder.Append("</td>           <td width=\"440\">               ");
					stringBuilder.Append(badItem.FolderName);
					stringBuilder.Append("</td>       </tr>       <tr>           <td width=\"29\">               &nbsp;</td>           <td class=\"header\" width=\"132\">               ");
					stringBuilder.Append(Strings.ReportItemType.ToString(preferedCulture));
					stringBuilder.Append("</td>           <td width=\"440\">               ");
					stringBuilder.Append(badItem.ClassType);
					stringBuilder.Append("</td>       </tr>       <tr>           <td width=\"29\">               &nbsp;</td>           <td class=\"header\" width=\"132\">               ");
					stringBuilder.Append(Strings.ReportItemCreated.ToString(preferedCulture));
					stringBuilder.Append("</td>           <td width=\"440\">               ");
					stringBuilder.Append(badItem.CreationTime);
					stringBuilder.Append("</td>       </tr>       <tr>           <td width=\"29\">               &nbsp;</td>           <td class=\"header\" width=\"132\">               ");
					stringBuilder.Append(Strings.ReportItemSubject.ToString(preferedCulture));
					stringBuilder.Append("</td>           <td width=\"440\">               ");
					stringBuilder.Append(badItem.Subject);
					if (GlobalSettings.BadItemIncludeStackTrace)
					{
						stringBuilder.Append("</td>       </tr>       <tr>           <td width=\"29\">               &nbsp;</td>           <td class=\"header\" width=\"132\">               ");
						stringBuilder.Append(Strings.ReportStackTrace.ToString(preferedCulture));
						stringBuilder.Append("</td>           <td width=\"440\">               ");
						stringBuilder.Append(badItem.Message);
						stringBuilder.Append(badItem.StackTrace);
					}
					stringBuilder.Append("</td>       </tr>   </table>");
				}
			}
			if (num4 == 0)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.GenerateReport Do nothing to prevent looping");
				return;
			}
			stringBuilder.Append("   <p>&nbsp;</p>   <!-- insert any additional custom text here-->   <hr /></body></html>");
			using (MessageItem messageItem = MessageItem.Create(base.MailboxSession, base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox)))
			{
				messageItem.ClassName = "IPM.Note.Exchange.ActiveSync.Report";
				messageItem.Subject = text;
				using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextHtml))
				{
					textWriter.Write(stringBuilder.ToString());
				}
				string displayName = string.Format(base.Request.Culture, Strings.ReportAndLogSender.ToString(preferedCulture), new object[]
				{
					base.User.DisplayName
				});
				Participant from = new Participant(displayName, base.User.SmtpAddress, "SMTP");
				Participant participant = new Participant(base.User.DisplayName, base.User.SmtpAddress, "SMTP");
				messageItem.From = from;
				messageItem.Recipients.Add(participant);
				messageItem.IsRead = false;
				messageItem.IsDraft = false;
				messageItem.Save(SaveMode.ResolveConflicts);
			}
		}

		private void SaveSyncStateAndDispose(SyncCollection collection)
		{
			AirSyncDiagnostics.TraceInfo<SyncCollection>(ExTraceGlobals.RequestsTracer, this, "Sync.SaveSyncStateAndDispose {0}", collection);
			try
			{
				if (!collection.HasBeenSaved && (collection.HaveChanges || collection.FilterType != collection.FilterTypeInSyncState || collection.OptionsSentAreDifferentForV121AndLater || collection.ConversationMode != collection.ConversationModeInSyncState || !collection.NullSyncWorked) && collection.SyncState != null)
				{
					collection.SyncState.CustomVersion = new int?(9);
					try
					{
						if (!base.IsInQuarantinedState && !collection.HaveChanges && !collection.MoreAvailable)
						{
							object[] nullSyncPropertiesToSave = collection.GetNullSyncPropertiesToSave();
							collection.SyncState.CommitState(collection.PropertiesToSaveForNullSync, nullSyncPropertiesToSave);
							collection.UpdateSavedNullSyncPropertiesInCache(nullSyncPropertiesToSave);
						}
						else
						{
							collection.SyncState.CommitState(null, null);
						}
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.SyncStateKbCommitted, (int)collection.SyncState.GetLastCommittedSize() >> 10);
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.TotalSaveCount, collection.SyncState.TotalSaveCount);
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.ColdSaveCount, collection.SyncState.ColdSaveCount);
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.ColdCopyCount, collection.SyncState.ColdCopyCount);
						base.ProtocolLogger.IncrementValueBy(collection.InternalName, PerFolderProtocolLoggerData.TotalLoadCount, collection.SyncState.TotalLoadCount);
					}
					catch (SyncStateSaveConflictException innerException)
					{
						AirSyncCounters.NumberOfConflictingConcurrentSync.Increment();
						AirSyncPermanentException ex = (base.Version >= 121) ? new AirSyncPermanentException(StatusCode.Sync_Retry, SyncCommand.GetRetryErrorXml(), innerException, false) : new AirSyncPermanentException(HttpStatusCode.ServiceUnavailable, StatusCode.None, innerException, false);
						ex.ErrorStringForProtocolLogger = "SaveConflict3";
						throw ex;
					}
					if (collection.ResponseSyncKey != collection.SyncKey && collection.ResponseSyncKey != 0U)
					{
						this.hangingCollectionSynckeys[collection.InternalName] = collection.ResponseSyncKey;
					}
					collection.HasBeenSaved = true;
				}
			}
			finally
			{
				collection.Dispose();
			}
		}

		private void CreateSubscription(SyncCollection collection)
		{
			AirSyncDiagnostics.TraceInfo<SyncCollection>(ExTraceGlobals.RequestsTracer, this, "Sync.CreateSubscription {0}", collection);
			if (collection == null)
			{
				EventCondition eventCondition = new EventCondition();
				eventCondition.ObjectType = EventObjectType.Folder;
				eventCondition.EventType = (EventType.ObjectCreated | EventType.ObjectDeleted | EventType.ObjectMoved);
				this.Notifier.Add(EventSubscription.Create(base.MailboxSession, eventCondition, this.Notifier));
				return;
			}
			if (!base.IsInQuarantinedState)
			{
				EventCondition eventCondition2 = collection.CreateEventCondition();
				if (eventCondition2 != null)
				{
					this.Notifier.Add(EventSubscription.Create(base.MailboxSession, eventCondition2, this.Notifier));
					if (AirSyncUtility.IsVirtualFolder(collection))
					{
						SyncCommand.SyncInformationForNotifier syncInformationForNotifier = (SyncCommand.SyncInformationForNotifier)this.Notifier.Information;
						if (syncInformationForNotifier.HangingVirtualFolderIds == null)
						{
							syncInformationForNotifier.HangingVirtualFolderIds = new HashSet<StoreObjectId>();
						}
						syncInformationForNotifier.HangingVirtualFolderIds.Add(collection.NativeStoreObjectId);
						return;
					}
				}
			}
			else
			{
				AirSyncDiagnostics.TraceInfo<DeviceAccessState>(ExTraceGlobals.RequestsTracer, this, "Sync. Skip item level subscription creation in quarantined state. Current AccessState = {0}", base.CurrentAccessState);
			}
		}

		private void SyncCollisionDetected()
		{
			if (this.requestCompleted)
			{
				return;
			}
			AirSyncDiagnostics.TraceWarning(ExTraceGlobals.RequestsTracer, this, "SyncCollisionDetected");
			if (base.RequestWaitWatch != null)
			{
				base.RequestWaitWatch.Stop();
				base.ProtocolLogger.SetValue(ProtocolLoggerData.RequestHangTime, base.RequestWaitWatch.ElapsedMilliseconds / 1000L);
			}
			AirSyncCounters.NumberOfConflictingConcurrentSync.Increment();
			base.Context.Response.HttpStatusCode = HttpStatusCode.OK;
			base.XmlResponse = SyncCommand.GetRetryErrorXml();
			this.CompleteRequest(null, true, false);
		}

		private void DisposeOpenCollections()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.DisposeOpenCollections");
			foreach (KeyValuePair<string, SyncCollection> keyValuePair in base.Collections)
			{
				if (keyValuePair.Value != null)
				{
					keyValuePair.Value.Dispose();
				}
			}
			base.Collections.Clear();
		}

		private bool FolderHierarchyChangedSinceLastSync()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.FolderHierarchyChangedSinceLastSync");
			FolderHierarchySyncState folderHierarchySyncState = null;
			SyncState syncState = null;
			FolderHierarchyChangeDetector.SyncHierarchyManifestState latestState = null;
			bool flag = false;
			bool result;
			try
			{
				Command.IcsFolderCheckResults icsFolderCheckResults = base.PerformICSFolderHierarchyChangeCheck(ref syncState, out latestState);
				if (icsFolderCheckResults == Command.IcsFolderCheckResults.NoChanges)
				{
					result = flag;
				}
				else if (icsFolderCheckResults == Command.IcsFolderCheckResults.ChangesNoDeepCheck)
				{
					AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.FolderHierarchyChangedSinceLastSync:QuickCheck:true");
					flag = true;
					result = flag;
				}
				else
				{
					folderHierarchySyncState = base.SyncStateStorage.GetFolderHierarchySyncState();
					if (folderHierarchySyncState == null)
					{
						AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.FolderHierarchyChangedSinceLastSync:1:true");
						flag = true;
						result = flag;
					}
					else
					{
						FolderHierarchySync folderHierarchySync = folderHierarchySyncState.GetFolderHierarchySync(new ChangeTrackingDelegate(FolderCommand.ComputeChangeTrackingHash));
						if (folderHierarchySync == null)
						{
							AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.FolderHierarchyChangedSinceLastSync:2:true");
							flag = true;
							result = flag;
						}
						else
						{
							folderHierarchySync.AcknowledgeServerOperations();
							HierarchySyncOperations hierarchySyncOperations = folderHierarchySync.EnumerateServerOperations(base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Root), false);
							if (hierarchySyncOperations != null && hierarchySyncOperations.Count > 0 && FolderCommand.FolderSyncRequired(base.SyncStateStorage, hierarchySyncOperations, syncState))
							{
								AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.FolderHierarchyChangedSinceLastSync:3:true");
								flag = true;
								result = flag;
							}
							else
							{
								AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.FolderHierarchyChangedSinceLastSync:false");
								base.SaveLatestIcsFolderHierarchySnapshot(latestState);
								result = false;
							}
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					base.XmlResponse = SyncCommand.GetFolderHierarchySyncRequiredXml();
				}
				if (folderHierarchySyncState != null)
				{
					folderHierarchySyncState.Dispose();
					folderHierarchySyncState = null;
				}
				if (syncState != null)
				{
					syncState.Dispose();
					syncState = null;
				}
			}
			return result;
		}

		private void CompleteRequest(XmlDocument xmlresponse, bool keepNotificationsGoing, bool timedOut)
		{
			AirSyncDiagnostics.TraceInfo<bool, bool>(ExTraceGlobals.RequestsTracer, this, "Sync.CompleteRequest keepNotificationsGoing:{0} timedOut:{1}", keepNotificationsGoing, timedOut);
			this.CleanupNotificationManager(keepNotificationsGoing);
			if (this.requestCompleted)
			{
				return;
			}
			this.requestCompleted = true;
			if (this.hanging)
			{
				Interlocked.Decrement(ref SyncCommand.numberOfOutstandingSyncs);
			}
			AirSyncCounters.CurrentlyPendingSync.RawValue = (long)SyncCommand.numberOfOutstandingSyncs;
			if (!base.Context.Response.IsClientConnected)
			{
				AirSyncCounters.NumberOfDroppedSync.Increment();
			}
			if (base.Context.Response.HttpStatusCode == HttpStatusCode.OK)
			{
				if (xmlresponse != null)
				{
					base.XmlResponse = xmlresponse;
				}
				if (base.XmlResponse != null)
				{
					base.Context.Response.IssueWbXmlResponse();
				}
				else if (string.IsNullOrEmpty(base.Context.Response.ContentType) || string.Equals("text/html", base.Context.Response.ContentType, StringComparison.OrdinalIgnoreCase))
				{
					base.Context.Response.ContentType = "application/vnd.ms-sync.wbxml";
				}
			}
			try
			{
				if (base.MailboxLoggingEnabled && base.MailboxLogger != null && base.MailboxLogger.Enabled)
				{
					base.OpenMailboxSession(base.User);
					if (base.MailboxLogger.MailboxSession == null)
					{
						base.MailboxLogger.MailboxSession = base.MailboxSession;
					}
					base.MailboxLogger.SetData(MailboxLogDataName.WasPending, "[Response was pending]");
					base.MailboxLogger.LogResponseHead(base.Context.Response);
					base.MailboxLogger.SetData(MailboxLogDataName.ResponseBody, (base.XmlResponse == null) ? "[No XmlResponse]" : AirSyncUtility.BuildOuterXml(base.XmlResponse, !GlobalSettings.EnableMailboxLoggingVerboseMode));
					base.MailboxLogger.SetData(MailboxLogDataName.ResponseTime, ExDateTime.Now.ToString(DateTimeFormatInfo.InvariantInfo));
					base.MailboxLogger.AppendToLogInMailbox();
				}
				base.CommitSyncStatusSyncState();
			}
			catch (LocalizedException ex)
			{
				AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex);
				AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "Exception thrown during CompleteRequest\r\n{0}\r\n", arg);
			}
			finally
			{
				uint num = (this.Notifier == null) ? 0U : this.Notifier.RequestedWaitTime;
				if (timedOut)
				{
					base.Context.Response.TimeToRespond = base.Context.RequestTime.AddSeconds(num);
				}
				else if (base.Context.Response.XmlDocument == null)
				{
					base.Context.Response.TimeToRespond = base.Context.RequestTime.AddSeconds(num / 2U);
				}
				else
				{
					XmlNode xmlNode = base.Context.Response.XmlDocument["Status"];
					XmlNode xmlNode2 = base.Context.Response.XmlDocument["Collections"];
					if (xmlNode != null && xmlNode.InnerText == "1" && (xmlNode2 == null || xmlNode2.ChildNodes.Count == 0))
					{
						uint num2 = num / 2U;
						base.Context.ProtocolLogger.SetValue(ProtocolLoggerData.EmptyResponseDelayed, num2);
						AirSyncDiagnostics.TraceError<uint>(ExTraceGlobals.RequestsTracer, this, "Empty sync response delayed for {0} seconds", num2);
						base.Context.Response.TimeToRespond = base.Context.RequestTime.AddSeconds(num2);
					}
					else
					{
						base.Context.Response.TimeToRespond = ExDateTime.UtcNow;
					}
				}
				this.CompleteHttpRequest();
			}
		}

		private void CleanupNotificationManager(bool keepNotificationsGoing)
		{
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "Sync.CleanupNotificationManager({0})", keepNotificationsGoing);
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

		private void InitializeOnEventSubscription()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.InitializeOnEventSubscription");
			base.OpenMailboxSession(base.User);
			base.OpenSyncStorage(base.Context.User.Features.IsEnabled(EasFeature.SyncStatusOnGlobalInfo));
			base.InitializeVersionFactory(base.Version);
		}

		private void PrepareToHang(bool startStopWatch)
		{
			if (this.Notifier == null)
			{
				throw new InvalidOperationException("[SyncCommand.PrepareToHang] Cannot hang when notifier is null.");
			}
			AirSyncDiagnostics.TraceInfo<bool>(ExTraceGlobals.RequestsTracer, this, "Sync.PrepareToHang startStopWatch:{0}", startStopWatch);
			this.collectionsResponseXmlNode = null;
			base.XmlResponse = null;
			this.DisposeOpenCollections();
			base.Context.PrepareToHang();
			base.VersionFactory = null;
			if (startStopWatch && base.RequestWaitWatch != null)
			{
				base.RequestWaitWatch.Start();
			}
		}

		private void PrepareToHang()
		{
			this.PrepareToHang(true);
		}

		private void LogRequest()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "Sync.LogRequest");
			if (!this.hanging && base.MailboxLogger != null)
			{
				foreach (SyncCollection syncCollection in base.Collections.Values)
				{
					syncCollection.InsertOptionsNode();
				}
				base.MailboxLogger.SetData(MailboxLogDataName.LogicalRequest, AirSyncUtility.BuildOuterXml(base.XmlRequest.OwnerDocument, true));
			}
		}

		INotificationManagerContext IAsyncCommand.Context
		{
			get
			{
				return base.Context;
			}
		}

		private static readonly PropertyCollection emptyPropertyCollection = new PropertyCollection();

		private static XmlDocument validationErrorXml;

		private static XmlDocument serverErrorXml;

		private static XmlDocument objectNotFoundErrorXml;

		private static XmlDocument retryErrorXml;

		private static XmlDocument maxFoldersExceededXml;

		private static XmlDocument maxWaitExceededXmlInSeconds;

		private static XmlDocument minWaitExceededXmlInSeconds;

		private static XmlDocument maxWaitExceededXmlInMinutes;

		private static XmlDocument minWaitExceededXmlInMinutes;

		private static XmlDocument folderHierarchySyncRequiredXml;

		private static XmlDocument invalidParametersXml;

		private static int numberOfOutstandingSyncs;

		private static string mailBoxBuildVersion;

		private XmlElement collectionsResponseXmlNode;

		private Dictionary<string, uint> hangingCollectionSynckeys = new Dictionary<string, uint>(4);

		private ExDateTime syncAttemptTime = ExDateTime.UtcNow;

		private int readCount;

		private CountingDictionary<string> syncCollectionSyncs = new CountingDictionary<string>();

		private ExDateTime? syncSuccessTime;

		private bool hangSpecified;

		private bool shouldHang;

		private bool hanging;

		private HashSet<string> collectionIdsDelayed = new HashSet<string>();

		private ConcurrentQueue<SyncCommand.SyncEventAndTime> syncEvents = new ConcurrentQueue<SyncCommand.SyncEventAndTime>();

		private List<XmlNode> itemLevelProtocolErrorNodes = new List<XmlNode>();

		private SyncCollection currentSyncCollection;

		private HashSet<string> collectionIdsWithHangableSynckeyChange;

		private uint requestedGlobalWindowSize;

		private bool requestCompleted;

		private bool isDirectPushAllowed;

		private bool isDirectPushAllowedByGeo;

		private bool requestWasCached;

		private NotificationManager notifier;

		internal class SyncHbiMonitor : NotificationManager.HbiMonitor
		{
			protected SyncHbiMonitor()
			{
			}

			public static SyncCommand.SyncHbiMonitor Instance
			{
				get
				{
					return SyncCommand.SyncHbiMonitor.instance;
				}
			}

			private static SyncCommand.SyncHbiMonitor instance = new SyncCommand.SyncHbiMonitor();
		}

		internal class SyncStatusCode
		{
			public const string Success = "1";

			public const string ProtocolVersionMismatch = "2";

			public const string InvalidSyncKey = "3";

			public const string ProtocolError = "4";

			public const string ServerError = "5";

			public const string ClientServerConversion = "6";

			public const string Conflict = "7";

			public const string ObjectNotFound = "8";

			public const string OutOfDisk = "9";

			public const string NotificationGUID = "10";

			public const string NotificationsNotProvisioned = "11";

			public const string FolderHierarchyRequired = "12";

			public const string InvalidParameters = "13";

			public const string InvalidWaitTime = "14";

			public const string TooManyFolders = "15";

			public const string Retry = "16";

			public const string InvalidRecipients = "185";
		}

		internal enum SyncEvent
		{
			NMCreated,
			NMStolen,
			NMReleased,
			CommandHang,
			Timeout,
			XsoEvent,
			XsoException
		}

		internal class SyncEventAndTime
		{
			public SyncEventAndTime(SyncCommand.SyncEvent syncEvent)
			{
				this.SyncEvent = syncEvent;
				this.DateTime = DateTime.UtcNow;
				this.ThreadId = Thread.CurrentThread.ManagedThreadId;
			}

			public SyncCommand.SyncEvent SyncEvent { get; private set; }

			public DateTime DateTime { get; private set; }

			public int ThreadId { get; private set; }
		}

		internal class BadItem
		{
			public string FolderName { get; set; }

			public string ClassType { get; set; }

			public string Subject { get; set; }

			public string CreationTime { get; set; }

			public bool RecoverySync { get; set; }

			public Exception Exception { get; set; }

			public string StackTrace
			{
				get
				{
					if (this.Exception.InnerException == null)
					{
						return AirSyncUtility.HtmlEncode(this.Exception.StackTrace, true);
					}
					StringBuilder stringBuilder = new StringBuilder();
					for (Exception ex = this.Exception; ex != null; ex = ex.InnerException)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.AppendFormat("\r\n----- {0} {1} -----\r\n", ex.GetType().FullName, ex.Message);
						}
						stringBuilder.Append(ex.StackTrace);
					}
					return AirSyncUtility.HtmlEncode(stringBuilder.ToString(), true);
				}
			}

			public string Message
			{
				get
				{
					string text = this.Exception.Message;
					if (string.IsNullOrEmpty(text))
					{
						AirSyncPermanentException ex = this.Exception as AirSyncPermanentException;
						if (ex != null && !string.IsNullOrEmpty(ex.ErrorStringForProtocolLogger))
						{
							text = ex.ErrorStringForProtocolLogger;
						}
						else if (this.Exception.InnerException != null)
						{
							text = this.Exception.InnerException.Message;
						}
					}
					if (this.RecoverySync)
					{
						return AirSyncUtility.HtmlEncode(this.Exception.GetType().FullName + "\nDuring RecoverySync\n" + text, true);
					}
					return AirSyncUtility.HtmlEncode(this.Exception.GetType().FullName + "\n" + text, true);
				}
			}

			public static SyncCommand.BadItem CreateFromItem(Item dataItem, bool recoverySync, Exception ex)
			{
				SyncCommand.BadItem result;
				try
				{
					SyncCommand.BadItem badItem = new SyncCommand.BadItem();
					badItem.ClassType = AirSyncUtility.HtmlEncode(dataItem.ClassName, true);
					badItem.CreationTime = AirSyncUtility.HtmlEncode(dataItem.CreationTime.ToString(), true);
					using (Folder folder = Folder.Bind(dataItem.Session, dataItem.ParentId))
					{
						badItem.FolderName = AirSyncUtility.HtmlEncode(folder.DisplayName, true);
					}
					if (dataItem is MessageItem)
					{
						badItem.Subject = ((MessageItem)dataItem).Subject;
					}
					else if (dataItem is CalendarItemBase)
					{
						badItem.Subject = ((CalendarItemBase)dataItem).Subject;
					}
					else if (dataItem is ContactBase)
					{
						badItem.Subject = ((ContactBase)dataItem).DisplayName;
					}
					else if (dataItem is PostItem)
					{
						badItem.Subject = ((PostItem)dataItem).Subject;
					}
					else if (dataItem is Task)
					{
						badItem.Subject = ((Task)dataItem).Subject;
					}
					else
					{
						CultureInfo preferedCulture = dataItem.Session.PreferedCulture;
						badItem.Subject = Strings.ReportUnknown.ToString(preferedCulture);
					}
					badItem.Subject = AirSyncUtility.HtmlEncode(badItem.Subject, true);
					badItem.RecoverySync = recoverySync;
					badItem.Exception = ex;
					AirSyncDiagnostics.TraceInfo<string, bool, Exception>(ExTraceGlobals.CorruptItemTracer, Command.CurrentCommand, "Create a bad item report for item with subject \"{0}\", recovery sync {1}, exception {2}", badItem.Subject, recoverySync, ex);
					result = badItem;
				}
				catch (PropertyErrorException ex2)
				{
					AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex2);
					AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.CorruptItemTracer, null, "PropertyError exception caught while looking up the badItem's basic properties.\r\n{0}", arg);
					result = null;
				}
				return result;
			}
		}

		private class SyncInformationForNotifier
		{
			public SyncInformationForNotifier(string randomString)
			{
				this.StoreIdToCollectionId = new Dictionary<StoreObjectId, string>(5);
				this.StoreIdToType = new Dictionary<StoreObjectId, string>(5);
				this.CollectionIdToSyncType = new Dictionary<string, string>(5);
				this.RandomNumberString = randomString;
			}

			public string RandomNumberString { get; set; }

			public StoreObjectId SyncStateStorageStoreObjectId { get; set; }

			public StoreObjectId ExchangeSyncDataStoreObjectId { get; set; }

			public HashSet<StoreObjectId> HangingVirtualFolderIds { get; set; }

			public Dictionary<StoreObjectId, string> StoreIdToCollectionId { get; set; }

			public Dictionary<StoreObjectId, string> StoreIdToType { get; set; }

			public Dictionary<string, string> CollectionIdToSyncType { get; set; }
		}

		private class SyncValidator : Validator
		{
			internal SyncValidator(int version, int maxBadClientItems, List<XmlNode> itemLevelProtocolErrorNodes, bool failOnItemLevelProtocolErrors, bool hasExtensions = false) : base(version, hasExtensions)
			{
				if (itemLevelProtocolErrorNodes == null)
				{
					throw new ArgumentNullException("itemLevelProtocolErrorNodes");
				}
				this.maxBadClientItems = maxBadClientItems;
				this.itemLevelProtocolErrorNodes = itemLevelProtocolErrorNodes;
				this.FailOnItemLevelProtocolErrors = failOnItemLevelProtocolErrors;
			}

			internal bool FailOnItemLevelProtocolErrors { get; set; }

			internal override bool ValidateXml(XmlElement rootNode, string rootNodeName)
			{
				try
				{
					base.ValidationErrors.Clear();
					XmlNode xmlNode = null;
					bool flag = false;
					if (this.maxBadClientItems == 0)
					{
						base.ValidationErrors.Add(new Validator.XmlValidationError("Maximum number of invalid items is set to 0.", new object[0]));
						return false;
					}
					if (rootNode == null || rootNodeName != rootNode.LocalName)
					{
						base.ValidationErrors.Add(new Validator.XmlValidationError("<Sync> node could not be found at XML root", new object[0]));
						return false;
					}
					string namespaceURI = rootNode.NamespaceURI;
					XmlNode xmlNode2 = rootNode.FirstChild;
					if (xmlNode2 == null)
					{
						base.ValidationErrors.Add(new Validator.XmlValidationError("<Sync> node has no children", new object[0]));
						return false;
					}
					if ("Collections" != xmlNode2.LocalName)
					{
						if (base.Version < 121)
						{
							base.ValidationErrors.Add(new Validator.XmlValidationError("First child of <Sync> node must be the <Collections> node for protocol versions before 12.1", new object[0]));
							return false;
						}
						flag = true;
					}
					else
					{
						xmlNode = xmlNode2;
						if (xmlNode.ChildNodes == null || xmlNode.ChildNodes.Count < 1)
						{
							base.ValidationErrors.Add(new Validator.XmlValidationError("<Collections> node has no children", new object[0]));
							return false;
						}
						int num = 0;
						for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
						{
							xmlNode2 = xmlNode.ChildNodes[i];
							if (xmlNode2 == null || "Collection" != xmlNode2.LocalName)
							{
								base.ValidationErrors.Add(new Validator.XmlValidationError("Child nodes of <Collections> node must be <Collection> nodes", new object[0]));
								return false;
							}
							if (xmlNode2.FirstChild == null)
							{
								base.ValidationErrors.Add(new Validator.XmlValidationError("<Collection> node has no children", new object[0]));
								return false;
							}
							foreach (object obj in xmlNode2.ChildNodes)
							{
								XmlNode xmlNode3 = (XmlNode)obj;
								if ("Commands" == xmlNode3.LocalName)
								{
									num += xmlNode3.ChildNodes.Count;
									if (num > GlobalSettings.MaxNumberOfClientOperations)
									{
										AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "Number of client operation exceeded the MaxNumberOfClientOperations!");
										Command.CurrentCommand.Context.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "TooManyClientOperations");
										return false;
									}
								}
							}
						}
						for (int j = 0; j < xmlNode.ChildNodes.Count; j++)
						{
							xmlNode2 = xmlNode.ChildNodes[j];
							int num2 = 0;
							foreach (object obj2 in xmlNode2.ChildNodes)
							{
								XmlNode xmlNode4 = (XmlNode)obj2;
								if (SyncCommand.SyncValidator.collectionChildNodes.Length == num2)
								{
									base.ValidationErrors.Add(new Validator.XmlValidationError("<Collection> node contains more children than allowed", new object[0]));
									return false;
								}
								while (SyncCommand.SyncValidator.collectionChildNodes[num2] != xmlNode4.LocalName)
								{
									num2++;
									if (SyncCommand.SyncValidator.collectionChildNodes.Length == num2)
									{
										base.ValidationErrors.Add(new Validator.XmlValidationError("<Collection> node contains child node <{0}> which appears out of order", new object[]
										{
											xmlNode4.LocalName
										}));
										return false;
									}
								}
								if (xmlNode4.LocalName != "Options")
								{
									num2++;
								}
								if ("Commands" == xmlNode4.LocalName)
								{
									using (IEnumerator enumerator3 = xmlNode4.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											object obj3 = enumerator3.Current;
											XmlNode xmlNode5 = (XmlNode)obj3;
											if (!base.ValidateXmlNode(xmlNode4, namespaceURI))
											{
												XmlNode xmlNode6 = xmlNode5["ServerId"];
												XmlNode xmlNode7 = xmlNode5["ClientId"];
												if ((xmlNode6 == null && xmlNode7 == null) || (xmlNode6 != null && xmlNode7 != null))
												{
													base.ValidationErrors.Add(new Validator.XmlValidationError("<{0}>: Command nodes must contain either server ID or client ID, but not both", new object[]
													{
														xmlNode5.LocalName
													}));
													return false;
												}
												if ((!(xmlNode5.LocalName == "Add") || xmlNode7 == null) && (!(xmlNode5.LocalName == "Change") || xmlNode6 == null))
												{
													return false;
												}
												this.itemLevelProtocolErrorNodes.Add(xmlNode5);
												if (this.itemLevelProtocolErrorNodes.Count > this.maxBadClientItems)
												{
													base.ValidationErrors.Add(new Validator.XmlValidationError("Exceeded maximum number of bad items: {0}", new object[]
													{
														this.maxBadClientItems
													}));
													return false;
												}
											}
										}
										continue;
									}
								}
								if ("Class" == xmlNode4.LocalName)
								{
									string innerText;
									if ((innerText = xmlNode4.InnerText) != null)
									{
										if (innerText == "Calendar" || innerText == "Email" || innerText == "Contacts")
										{
											continue;
										}
										if (!(innerText == "Tasks"))
										{
											if (innerText == "Notes")
											{
												if (base.Version < 140)
												{
													base.ValidationErrors.Add(new Validator.XmlValidationError("Notes class is only supported for protocol versions 14.0 and above", new object[0]));
													return false;
												}
												continue;
											}
										}
										else
										{
											if (base.Version < 25)
											{
												base.ValidationErrors.Add(new Validator.XmlValidationError("Tasks class is only supported for protocol versions 2.5 and above", new object[0]));
												return false;
											}
											continue;
										}
									}
									base.ValidationErrors.Add(new Validator.XmlValidationError("Unsupported class name: {0}", new object[]
									{
										xmlNode4.InnerText
									}));
									return false;
								}
								if (!base.ValidateXmlNode(xmlNode4, namespaceURI))
								{
									return false;
								}
							}
						}
						xmlNode2 = xmlNode.NextSibling;
					}
					if (xmlNode2 != null && "Wait" == xmlNode2.LocalName)
					{
						if (base.Version <= 120)
						{
							base.ValidationErrors.Add(new Validator.XmlValidationError("<Wait> node is not permitted in Version 12.0 and below.", new object[0]));
							return false;
						}
						if (xmlNode2.NextSibling != null && "HeartbeatInterval" == xmlNode2.NextSibling.LocalName)
						{
							base.ValidationErrors.Add(new Validator.XmlValidationError("<Wait> node and <HeartbeatInterval> node are not supported in the same command invocation", new object[0]));
							return false;
						}
						xmlNode2 = xmlNode2.NextSibling;
					}
					if (xmlNode2 != null && xmlNode2.LocalName == "HeartbeatInterval")
					{
						if (base.Version <= 120)
						{
							base.ValidationErrors.Add(new Validator.XmlValidationError("<HeartbeatInterval> node is not permitted in Version 12.0 and below.", new object[0]));
							return false;
						}
						xmlNode2 = xmlNode2.NextSibling;
					}
					if (xmlNode2 != null && "WindowSize" == xmlNode2.LocalName)
					{
						xmlNode2 = xmlNode2.NextSibling;
					}
					if (xmlNode2 != null && "Partial" == xmlNode2.LocalName)
					{
						if (xmlNode2.PreviousSibling == null)
						{
							base.ValidationErrors.Add(new Validator.XmlValidationError("<Partial> node cannot be the only child of the <Sync> node", new object[0]));
							return false;
						}
						xmlNode2 = xmlNode2.NextSibling;
					}
					else if (flag)
					{
						base.ValidationErrors.Add(new Validator.XmlValidationError("<Collections> node is not present so <Partial> node must be specified", new object[0]));
						return false;
					}
					if (xmlNode2 != null)
					{
						AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "Unexpected node {0} was found in the request!", xmlNode2.LocalName);
						base.ValidationErrors.Add(new Validator.XmlValidationError("Unexpected node: {0}", new object[]
						{
							xmlNode2.LocalName
						}));
						return false;
					}
				}
				catch (XmlSchemaValidationException ex)
				{
					AirSyncDiagnostics.TraceError<XmlSchemaValidationException>(ExTraceGlobals.RequestsTracer, this, "Exception thrown during Sync\r\n{0}\r\n", ex);
					base.ValidationErrors.Add(new Validator.XmlValidationError(ex.Message, XmlSeverityType.Error, ex.LineNumber, ex.LinePosition));
					return false;
				}
				return !this.FailOnItemLevelProtocolErrors || this.itemLevelProtocolErrorNodes.Count <= 0;
			}

			private static readonly string[] collectionChildNodes = new string[]
			{
				"Class",
				"SyncKey",
				"NotifyGUID",
				"CollectionId",
				"Supported",
				"DeletesAsMoves",
				"GetChanges",
				"WindowSize",
				"ConversationMode",
				"Options",
				"Commands"
			};

			private int maxBadClientItems;

			private List<XmlNode> itemLevelProtocolErrorNodes;
		}
	}
}
