using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Web;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter.Entity;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class MeetingResponseCommand : Command
	{
		public MeetingResponseCommand()
		{
			this.shortIdRequests = new Dictionary<string, IList<MeetingResponseCommand.RequestNodeData>>();
			this.longIdRequests = new List<MeetingResponseCommand.RequestNodeData>();
			base.PerfCounter = AirSyncCounters.NumberOfMeetingResponse;
		}

		internal ICalendaringContainer CalendaringContainer { get; set; }

		internal Guid MailboxGuid { get; set; }

		internal IItemIdMapping CalendarItemIdMapping
		{
			get
			{
				if (this.itemIdMapping == null)
				{
					StoreObjectId defaultFolderId = base.MailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar);
					if (this.calendarSyncState == null)
					{
						MailboxSyncProviderFactory syncProviderFactory = new MailboxSyncProviderFactory(base.MailboxSession, defaultFolderId);
						this.calendarSyncState = base.SyncStateStorage.GetFolderSyncState(syncProviderFactory);
						MailboxSyncItemId mailboxSyncItemId = MailboxSyncItemId.CreateForNewItem(defaultFolderId);
						if (this.calendarSyncState == null)
						{
							string text = this.FolderIdMapping[mailboxSyncItemId];
							if (text == null)
							{
								text = this.FolderIdMapping.Add(mailboxSyncItemId);
								this.folderIdMappingSyncState.Commit();
							}
							this.calendarSyncState = base.SyncStateStorage.CreateFolderSyncState(syncProviderFactory, text);
							this.calendarSyncStateChangedFlag = true;
						}
						if (this.calendarSyncState[CustomStateDatumType.IdMapping] == null)
						{
							string parentSyncId = this.FolderIdMapping[mailboxSyncItemId];
							this.calendarSyncState[CustomStateDatumType.IdMapping] = new ItemIdMapping(parentSyncId);
							this.calendarSyncStateChangedFlag = true;
						}
						if (this.calendarSyncState.CustomVersion != null && this.calendarSyncState.CustomVersion.Value > 9)
						{
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MismatchedSyncStateInMR");
							throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.SyncStateVersionInvalid, EASServerStrings.MismatchSyncStateError, true);
						}
					}
					this.itemIdMapping = (ItemIdMapping)this.calendarSyncState[CustomStateDatumType.IdMapping];
				}
				return this.itemIdMapping;
			}
			set
			{
				this.itemIdMapping = value;
			}
		}

		protected override string RootNodeName
		{
			get
			{
				return "MeetingResponse";
			}
		}

		protected override bool IsInteractiveCommand
		{
			get
			{
				return true;
			}
		}

		private FolderIdMapping FolderIdMapping
		{
			get
			{
				if (this.folderIdMappingSyncState == null)
				{
					this.folderIdMappingSyncState = base.SyncStateStorage.GetCustomSyncState(new FolderIdMappingSyncStateInfo(), new PropertyDefinition[0]);
					if (this.folderIdMappingSyncState == null)
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NoSyncStateInMR");
						throw new AirSyncPermanentException(StatusCode.Sync_InvalidSyncKey, false);
					}
				}
				return (FolderIdMapping)this.folderIdMappingSyncState[CustomStateDatumType.IdMapping];
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			this.OnExecute();
			return Command.ExecutionState.Complete;
		}

		protected override bool HandleQuarantinedState()
		{
			base.XmlResponse = this.InitializeXmlResponse();
			this.AppendNonSuccessXmlNode(null, StatusCode.Sync_ProtocolError, null, "Device in Quarantined state", false);
			base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InQuarantine");
			return false;
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (MeetingResponseCommand.validationErrorXml == null)
			{
				XmlDocument commandXmlStub = base.GetCommandXmlStub();
				XmlElement newChild = commandXmlStub.CreateElement("Result", this.RootNodeNamespace);
				XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
				xmlElement.InnerText = XmlConvert.ToString(2);
				commandXmlStub[this.RootNodeName].AppendChild(newChild).AppendChild(xmlElement);
				MeetingResponseCommand.validationErrorXml = commandXmlStub;
			}
			return MeetingResponseCommand.validationErrorXml;
		}

		internal XmlDocument InitializeXmlResponse()
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			this.meetingResponseNode = xmlDocument.CreateElement("MeetingResponse", "MeetingResponse:");
			xmlDocument.AppendChild(this.meetingResponseNode);
			return xmlDocument;
		}

		internal void ParseXmlRequest()
		{
			if (base.XmlRequest.Name != "MeetingResponse")
			{
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "BadRequestNode(" + base.XmlRequest.Name + ")forMRCmd");
				throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false);
			}
			using (XmlNodeList elementsByTagName = base.XmlRequest.GetElementsByTagName("Request", "MeetingResponse:"))
			{
				if (elementsByTagName.Count < 1 || elementsByTagName.Count >= GlobalSettings.MaxRetrievedItems)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, (elementsByTagName.Count < 1) ? "IncompleteRequestinMRCmd" : "MRTooManyOperations");
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidXML, null, false);
				}
				foreach (object obj in elementsByTagName)
				{
					XmlNode xmlNode = (XmlNode)obj;
					try
					{
						this.ValidateRequestNodeData(xmlNode);
					}
					catch (RequestParsingException ex)
					{
						XmlNode xmlNode2 = xmlNode["UserResponse", "MeetingResponse:"];
						XmlNode xmlNode3 = xmlNode["RequestId", "MeetingResponse:"];
						XmlNode xmlNode4 = xmlNode["LongId", "Search:"];
						this.AppendNonSuccessXmlNode((xmlNode4 != null) ? xmlNode4.InnerText : ((xmlNode3 != null) ? xmlNode3.InnerText : null), StatusCode.Sync_ProtocolVersionMismatch, (xmlNode2 != null) ? xmlNode2.InnerText : null, ex.Message, xmlNode4 != null);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, ex.LogMessage);
					}
				}
			}
		}

		internal void ProcessCommand()
		{
			foreach (MeetingResponseCommand.RequestNodeData requestNodeData in this.longIdRequests)
			{
				StoreObjectId storeObjectId = null;
				try
				{
					storeObjectId = StoreObjectId.Deserialize(HttpUtility.UrlDecode(requestNodeData.RequestId));
					if (requestNodeData.InstanceId != ExDateTime.MinValue)
					{
						storeObjectId = this.GetOccurenceId(storeObjectId, requestNodeData.InstanceId);
					}
				}
				catch (ArgumentException)
				{
					base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MRErrors);
					this.AppendNonSuccessXmlNode(requestNodeData.RequestId, StatusCode.Sync_ProtocolVersionMismatch, requestNodeData.UserResponse, string.Format(CultureInfo.InvariantCulture, "The LongId '{0}' in the reqest is invalid!", new object[]
					{
						requestNodeData.RequestId
					}), true);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidLongId");
					base.PartialFailure = true;
					continue;
				}
				catch (CorruptDataException)
				{
					base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MRErrors);
					this.AppendNonSuccessXmlNode(requestNodeData.RequestId, StatusCode.Sync_ProtocolVersionMismatch, requestNodeData.UserResponse, string.Format(CultureInfo.InvariantCulture, "The LongId '{0}' in the reqest is invalid!", new object[]
					{
						requestNodeData.RequestId
					}), true);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidLongId2");
					base.PartialFailure = true;
					continue;
				}
				catch (FormatException)
				{
					base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MRErrors);
					this.AppendNonSuccessXmlNode(requestNodeData.RequestId, StatusCode.Sync_ProtocolVersionMismatch, requestNodeData.UserResponse, string.Format(CultureInfo.InvariantCulture, "The LongId '{0}' in the reqest is invalid!", new object[]
					{
						requestNodeData.RequestId
					}), true);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidLongId3");
					base.PartialFailure = true;
					continue;
				}
				catch (AirSyncPermanentException ex)
				{
					base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MRErrors);
					AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.RequestsTracer, this, "Exception caught while processing LongId {0}\r\n{1}", requestNodeData.RequestId, ex.ToString());
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, (!string.IsNullOrEmpty(ex.ErrorStringForProtocolLogger)) ? ex.ErrorStringForProtocolLogger : "InvalidLongId4");
					this.AppendNonSuccessXmlNode(requestNodeData.RequestId, ex.AirSyncStatusCode, requestNodeData.UserResponse, ex.ErrorStringForProtocolLogger, true);
					base.PartialFailure = true;
					continue;
				}
				try
				{
					if (!this.Respond(storeObjectId, requestNodeData.RequestId, requestNodeData, true))
					{
						base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MRErrors);
					}
				}
				catch (AirSyncPermanentException)
				{
					base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MRErrors);
					base.PartialFailure = true;
					throw;
				}
			}
			foreach (string text in this.shortIdRequests.Keys)
			{
				IList<MeetingResponseCommand.RequestNodeData> list = this.shortIdRequests[text];
				using (FolderSyncState folderSyncState = base.SyncStateStorage.GetFolderSyncState(text))
				{
					for (int i = 0; i < list.Count; i++)
					{
						try
						{
							string requestId = list[i].RequestId;
							if (folderSyncState == null || folderSyncState[CustomStateDatumType.IdMapping] == null)
							{
								this.AppendNonSuccessXmlNode(list[i].RequestId, StatusCode.Sync_ProtocolVersionMismatch, list[i].UserResponse, string.Format(CultureInfo.InvariantCulture, "The CollectionId '{0}' in the reqest has not been synced yet.", new object[]
								{
									text
								}), false);
								base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CollectionNotSynced");
							}
							else
							{
								StoreObjectId storeObjectId2 = null;
								MailboxSyncItemId mailboxSyncItemId = ((ItemIdMapping)folderSyncState[CustomStateDatumType.IdMapping])[requestId] as MailboxSyncItemId;
								if (mailboxSyncItemId != null)
								{
									if (list[i].InstanceId == ExDateTime.MinValue)
									{
										storeObjectId2 = (StoreObjectId)mailboxSyncItemId.NativeId;
									}
									else
									{
										storeObjectId2 = this.GetOccurenceId((StoreObjectId)mailboxSyncItemId.NativeId, list[i].InstanceId);
									}
								}
								if (storeObjectId2 == null)
								{
									this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolVersionMismatch, list[i].UserResponse, string.Format(CultureInfo.InvariantCulture, "Cannot look up the meeting-request '{0}' from ItemIdMapping.", new object[]
									{
										requestId
									}), false);
									base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "InvalidRequestId");
								}
								else if (!this.Respond(storeObjectId2, requestId, list[i], false))
								{
									base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MRErrors);
								}
							}
						}
						catch (AirSyncPermanentException ex2)
						{
							base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MRErrors);
							AirSyncDiagnostics.TraceDebug<string, string>(ExTraceGlobals.RequestsTracer, this, "Exception caught while processing meeting-request {0}\r\n{1}", list[i].RequestId, ex2.ToString());
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, (!string.IsNullOrEmpty(ex2.ErrorStringForProtocolLogger)) ? ex2.ErrorStringForProtocolLogger : "InvalidLongId4");
							this.AppendNonSuccessXmlNode(list[i].RequestId, ex2.AirSyncStatusCode, list[i].UserResponse, ex2.ErrorStringForProtocolLogger, false);
							base.PartialFailure = true;
						}
					}
				}
			}
		}

		private void OnExecute()
		{
			AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "MeetingRequest received. Processing the command ...");
			base.XmlResponse = this.InitializeXmlResponse();
			try
			{
				this.ParseXmlRequest();
				this.ProcessCommand();
				if (this.calendarSyncStateChangedFlag)
				{
					this.calendarSyncState.CustomVersion = new int?(9);
					this.calendarSyncState.Commit();
				}
			}
			catch (LocalizedException ex)
			{
				if (base.MailboxLogger != null)
				{
					base.MailboxLogger.SetData(MailboxLogDataName.MeetingResponseCommand_OnExecute_Exception, ex);
				}
				AirSyncPermanentException ex2 = ex as AirSyncPermanentException;
				StatusCode status;
				if (ex2 != null)
				{
					status = ex2.AirSyncStatusCode;
					if (string.IsNullOrEmpty(ex2.ErrorStringForProtocolLogger))
					{
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, ex2.GetType().Name);
					}
				}
				else
				{
					status = StatusCode.Sync_ProtocolError;
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, ex.GetType().Name);
				}
				base.XmlResponse = this.InitializeXmlResponse();
				this.AppendNonSuccessXmlNode(null, status, null, string.Format(CultureInfo.InvariantCulture, "Constructing the Xml response in case of LocalizedException: {0}", new object[]
				{
					ex.Message
				}), false);
			}
			finally
			{
				if (this.calendarSyncState != null)
				{
					this.calendarSyncState.Dispose();
				}
				if (this.folderIdMappingSyncState != null)
				{
					this.folderIdMappingSyncState.Dispose();
				}
			}
		}

		private void ValidateRequestNodeData(XmlNode requestNode)
		{
			XmlNode xmlNode = requestNode["UserResponse", "MeetingResponse:"];
			XmlNode xmlNode2 = requestNode["CollectionId", "MeetingResponse:"];
			XmlNode xmlNode3 = requestNode["RequestId", "MeetingResponse:"];
			XmlNode xmlNode4 = requestNode["LongId", "Search:"];
			XmlNode xmlNode5 = requestNode["InstanceId", "MeetingResponse:"];
			if (xmlNode4 != null)
			{
				if (string.IsNullOrEmpty(xmlNode4.InnerText))
				{
					throw new RequestParsingException("longIdNode cannot be empty", "NoLongId");
				}
				if (xmlNode3 != null || xmlNode2 != null)
				{
					throw new RequestParsingException("CollectionId/RequestId cannot be present when LongId supplied!", "TooManyIds");
				}
			}
			else
			{
				if (xmlNode3 == null || string.IsNullOrEmpty(xmlNode3.InnerText))
				{
					throw new RequestParsingException("RequestIdNode cannot be missing or null", "NoRequestId");
				}
				if (xmlNode2 == null || string.IsNullOrEmpty(xmlNode2.InnerText) || (AirSyncUtility.GetCollectionType(xmlNode2.InnerText) != SyncCollection.CollectionTypes.Mailbox && AirSyncUtility.GetCollectionType(xmlNode2.InnerText) != SyncCollection.CollectionTypes.Unknown))
				{
					throw new RequestParsingException("CollectionIdNode cannot be missing, null, or the collection type is other than mailbox or unknown.", "NoCollectionId");
				}
			}
			if (xmlNode == null || string.IsNullOrEmpty(xmlNode.InnerText) || (xmlNode.InnerText != "1" && xmlNode.InnerText != "2" && xmlNode.InnerText != "3"))
			{
				throw new RequestParsingException("UserResponseNode cannot be missing or null", "NoUserResponse");
			}
			if (xmlNode5 != null)
			{
				if (string.IsNullOrEmpty(xmlNode5.InnerText))
				{
					throw new RequestParsingException("InstanceIdNode cannot be empty", "InvalidInstanceId");
				}
				if (base.Version >= 160)
				{
					throw new RequestParsingException("InstanceIdNode is not supported", "InvalidInstanceId");
				}
			}
			MeetingResponseCommand.RequestNodeData requestNodeData;
			if (xmlNode4 != null)
			{
				requestNodeData = new MeetingResponseCommand.RequestNodeData(xmlNode, xmlNode4, xmlNode5);
				if (base.Version >= 160)
				{
					requestNodeData.FillRespondToEventParameters(requestNode);
				}
				this.longIdRequests.Add(requestNodeData);
				return;
			}
			if (!this.shortIdRequests.ContainsKey(xmlNode2.InnerText))
			{
				this.shortIdRequests.Add(xmlNode2.InnerText, new List<MeetingResponseCommand.RequestNodeData>());
			}
			requestNodeData = new MeetingResponseCommand.RequestNodeData(xmlNode, xmlNode3, xmlNode5);
			if (base.Version >= 160)
			{
				requestNodeData.FillRespondToEventParameters(requestNode);
			}
			this.shortIdRequests[xmlNode2.InnerText].Add(requestNodeData);
		}

		private void AppendNonSuccessXmlNode(string syncRequestId, StatusCode status, string userResponse, string errorMessage, bool usingLongId)
		{
			AirSyncDiagnostics.TraceWarning(ExTraceGlobals.RequestsTracer, this, errorMessage);
			this.AppendXmlNode(syncRequestId, status, null, userResponse, usingLongId);
		}

		private void AppendXmlNode(string requestId, StatusCode status, string calendarId, string userResponse, bool usingLongId)
		{
			XmlElement xmlElement = base.XmlResponse.CreateElement("Result", "MeetingResponse:");
			this.meetingResponseNode.AppendChild(xmlElement);
			XmlElement xmlElement2;
			if (usingLongId)
			{
				xmlElement2 = base.XmlResponse.CreateElement("LongId", "Search:");
			}
			else
			{
				xmlElement2 = base.XmlResponse.CreateElement("RequestId", "MeetingResponse:");
			}
			xmlElement2.InnerText = requestId;
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = base.XmlResponse.CreateElement("Status", "MeetingResponse:");
			XmlNode xmlNode = xmlElement3;
			int num = (int)status;
			xmlNode.InnerText = num.ToString(CultureInfo.InvariantCulture);
			xmlElement.AppendChild(xmlElement3);
			if (status == StatusCode.Success && userResponse != "3")
			{
				XmlElement xmlElement4 = base.XmlResponse.CreateElement("CalId", "MeetingResponse:");
				xmlElement4.InnerText = calendarId;
				xmlElement.AppendChild(xmlElement4);
			}
		}

		private StoreObjectId GetOccurenceId(StoreObjectId mailboxRequestId, ExDateTime instanceId)
		{
			Item item = null;
			CalendarItemOccurrence calendarItemOccurrence = null;
			StoreObjectId result;
			try
			{
				try
				{
					item = Item.Bind(base.MailboxSession, mailboxRequestId, null);
				}
				catch (ObjectNotFoundException)
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.Sync_ProtocolVersionMismatch, null, false)
					{
						ErrorStringForProtocolLogger = "GetOccurenceId+ItemNotFound"
					};
				}
				CalendarItem calendarItem = item as CalendarItem;
				if (calendarItem == null)
				{
					AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, this, "GetOccurenceId bound to a non-calendar item, return item itself");
					result = mailboxRequestId;
				}
				else
				{
					if (calendarItem.Recurrence == null)
					{
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.NoRecurrenceInCalendar, null, false)
						{
							ErrorStringForProtocolLogger = "NoRecurrenceInCalendar"
						};
					}
					if (!calendarItem.IsMeeting)
					{
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.Sync_ProtocolVersionMismatch, null, false)
						{
							ErrorStringForProtocolLogger = "NotAMeeting"
						};
					}
					try
					{
						try
						{
							calendarItemOccurrence = calendarItem.OpenOccurrenceByOriginalStartTime(instanceId, null);
						}
						catch (OccurrenceNotFoundException)
						{
							calendarItemOccurrence = null;
						}
						if (calendarItemOccurrence == null)
						{
							throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.Sync_ProtocolVersionMismatch, null, false)
							{
								ErrorStringForProtocolLogger = "NotAnOccurence"
							};
						}
						result = calendarItemOccurrence.Id.ObjectId;
					}
					catch (ObjectNotFoundException)
					{
						throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.Sync_ProtocolVersionMismatch, null, false)
						{
							ErrorStringForProtocolLogger = "ObjectNotFound"
						};
					}
				}
			}
			finally
			{
				if (calendarItemOccurrence != null)
				{
					calendarItemOccurrence.Dispose();
				}
				if (item != null)
				{
					item.Dispose();
				}
			}
			return result;
		}

		private bool Respond(StoreObjectId itemId, string requestId, MeetingResponseCommand.RequestNodeData response, bool usingLongId)
		{
			if (base.Version <= 141)
			{
				return this.LegacyRespond(itemId, requestId, response.UserResponse, usingLongId);
			}
			if (response.RespondToEventParameters == null)
			{
				throw new InvalidOperationException("Response.RespondToEventParameters must not be null at this point.");
			}
			StoreObjectId storeObjectId = null;
			string calendarId = null;
			if (this.CalendaringContainer == null)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Set default CalendaringContainer");
				this.CalendaringContainer = new CalendaringContainer(base.MailboxSession, null);
			}
			if (this.MailboxGuid == Guid.Empty)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Set default MailboxGuid");
				this.MailboxGuid = base.MailboxSession.MailboxGuid;
			}
			string key = EntitySyncItem.GetKey(this.MailboxGuid, itemId);
			IEvents events;
			if (itemId.ObjectType == StoreObjectType.CalendarItem || itemId.ObjectType == StoreObjectType.CalendarItemOccurrence || itemId.ObjectType == StoreObjectType.CalendarItemSeries)
			{
				storeObjectId = itemId;
				events = EntitySyncItem.GetEvents(this.CalendaringContainer, base.MailboxSession, itemId);
			}
			else
			{
				if (itemId.ObjectType != StoreObjectType.MeetingRequest)
				{
					this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolVersionMismatch, response.UserResponse, "The item is not meeting-request or calendar item.", usingLongId);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "UnsupportedObjectType" + itemId.ObjectType);
					return false;
				}
				using (MeetingMessage meetingMessage = MeetingMessage.Bind(base.MailboxSession, itemId))
				{
					if (meetingMessage == null)
					{
						this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolVersionMismatch, response.UserResponse, "Cannot find the meeting-request as specified in the reqest.", usingLongId);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemNotFound");
						return false;
					}
					using (CalendarItemBase correlatedItem = meetingMessage.GetCorrelatedItem())
					{
						if (meetingMessage == null)
						{
							this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolVersionMismatch, response.UserResponse, "Cannot find the correlated meeitng item.", usingLongId);
							base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NullCalItemInMRCmd");
							return false;
						}
						storeObjectId = correlatedItem.Id.ObjectId;
					}
				}
				response.RespondToEventParameters.MeetingRequestIdToBeDeleted = key;
				key = EntitySyncItem.GetKey(this.MailboxGuid, storeObjectId);
				events = this.CalendaringContainer.Calendars.Default.Events;
			}
			try
			{
				events.Respond(key, response.RespondToEventParameters, null);
			}
			catch (LocalizedException ex)
			{
				this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolError, response.UserResponse, ex.Message, usingLongId);
				base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "EntityRespondError");
				return false;
			}
			if (storeObjectId != null)
			{
				MailboxSyncItemId mailboxSyncItemId = MailboxSyncItemId.CreateForNewItem(storeObjectId);
				if (this.CalendarItemIdMapping.Contains(mailboxSyncItemId))
				{
					calendarId = this.CalendarItemIdMapping[mailboxSyncItemId];
				}
				else
				{
					calendarId = this.CalendarItemIdMapping.Add(mailboxSyncItemId);
					this.calendarSyncStateChangedFlag = true;
				}
			}
			this.AppendXmlNode(requestId, StatusCode.Success, calendarId, response.UserResponse, usingLongId);
			return true;
		}

		private bool LegacyRespond(StoreObjectId itemId, string requestId, string userResponse, bool usingLongId)
		{
			MeetingRequest meetingRequest = null;
			CalendarItemBase calendarItemBase = null;
			StoreObjectId storeObjectId = null;
			StoreObjectId storeObjectId2 = null;
			Item item = null;
			try
			{
				base.ProtocolLogger.IncrementValue(ProtocolLoggerData.MRItems);
				try
				{
					item = Item.Bind(base.MailboxSession, itemId, null);
				}
				catch (ObjectNotFoundException)
				{
					this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolVersionMismatch, userResponse, "Cannot find the meeting-request as specified in the reqest.", usingLongId);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemNotFound");
					return false;
				}
				meetingRequest = (item as MeetingRequest);
				if (base.Version >= 141 && meetingRequest == null)
				{
					calendarItemBase = (item as CalendarItemBase);
					if (calendarItemBase != null)
					{
						calendarItemBase.OpenAsReadWrite();
					}
				}
				if (meetingRequest == null && calendarItemBase == null)
				{
					this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolVersionMismatch, userResponse, "The item is not meeting-request or calendar item.", usingLongId);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "ItemNotValid");
					return false;
				}
				item = null;
				if (meetingRequest != null)
				{
					if (meetingRequest.IsOrganizer())
					{
						this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolError, userResponse, "The organizer of this meeting request is the mailbox owner. Checking meetingRequest.", usingLongId);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MRIsOrganizer");
						return false;
					}
					if (meetingRequest.IsDelegated())
					{
						this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolError, userResponse, "Cannot respond to a delegated meeting request.  Use Outlook", usingLongId);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MRIsDelegated");
						return false;
					}
					if (meetingRequest.IsOutOfDate())
					{
						this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolError, userResponse, "Cannot respond to a OutOfDated meeting request.", usingLongId);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "MRIsOutOfDate");
						return false;
					}
					meetingRequest.OpenAsReadWrite();
					storeObjectId = meetingRequest.Id.ObjectId;
					calendarItemBase = meetingRequest.UpdateCalendarItem(false);
				}
				if (calendarItemBase == null)
				{
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "NullCalItemInMRCmd");
					throw new AirSyncPermanentException(HttpStatusCode.InternalServerError, StatusCode.ServerError, null, false);
				}
				if (calendarItemBase.IsOrganizer())
				{
					this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolError, userResponse, "The organizer of this meeting request is the mailbox owner. Checking calendarItem.", usingLongId);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CalIsOrganizer");
					return false;
				}
				if (calendarItemBase.IsCancelled)
				{
					this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolError, userResponse, "Cannot respond to a canceled meeting.", usingLongId);
					base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "CalIsCancelled");
					return false;
				}
				if (userResponse != null)
				{
					ResponseType responseType;
					if (!(userResponse == "1"))
					{
						if (!(userResponse == "2"))
						{
							if (!(userResponse == "3"))
							{
								goto IL_22F;
							}
							responseType = ResponseType.Decline;
						}
						else
						{
							responseType = ResponseType.Tentative;
						}
					}
					else
					{
						responseType = ResponseType.Accept;
					}
					try
					{
						using (calendarItemBase.RespondToMeetingRequest(responseType))
						{
						}
					}
					catch (SaveConflictException ex)
					{
						this.AppendNonSuccessXmlNode(requestId, StatusCode.Sync_ProtocolError, userResponse, "A conflict calendar item has been detected: " + ex.Message, usingLongId);
						base.ProtocolLogger.SetValue(ProtocolLoggerData.Error, "SavedWithConflicts");
						return false;
					}
					calendarItemBase.Load();
					if (meetingRequest != null)
					{
						meetingRequest.Save(SaveMode.ResolveConflicts);
					}
					if (responseType != ResponseType.Decline)
					{
						if (calendarItemBase is CalendarItemOccurrence)
						{
							storeObjectId2 = ((CalendarItemOccurrence)calendarItemBase).MasterId.ObjectId;
						}
						else
						{
							storeObjectId2 = calendarItemBase.Id.ObjectId;
						}
					}
					else
					{
						base.MailboxSession.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
						{
							calendarItemBase.Id
						});
					}
					calendarItemBase.Dispose();
					calendarItemBase = null;
					if (storeObjectId != null)
					{
						base.MailboxSession.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
						{
							storeObjectId
						});
					}
					goto IL_339;
				}
				IL_22F:
				throw new ArgumentException(string.Format("Unexpected userResponse value \"{0}\"", userResponse));
			}
			finally
			{
				if (calendarItemBase != null)
				{
					calendarItemBase.Dispose();
				}
				if (meetingRequest != null)
				{
					meetingRequest.Dispose();
				}
				if (item != null)
				{
					item.Dispose();
				}
			}
			IL_339:
			string calendarId = null;
			if (storeObjectId2 != null)
			{
				MailboxSyncItemId mailboxSyncItemId = MailboxSyncItemId.CreateForNewItem(storeObjectId2);
				if (this.CalendarItemIdMapping.Contains(mailboxSyncItemId))
				{
					calendarId = this.CalendarItemIdMapping[mailboxSyncItemId];
				}
				else
				{
					calendarId = this.CalendarItemIdMapping.Add(mailboxSyncItemId);
					this.calendarSyncStateChangedFlag = true;
				}
			}
			this.AppendXmlNode(requestId, StatusCode.Success, calendarId, userResponse, usingLongId);
			return true;
		}

		private static XmlDocument validationErrorXml;

		private FolderSyncState calendarSyncState;

		private bool calendarSyncStateChangedFlag;

		private CustomSyncState folderIdMappingSyncState;

		private IDictionary<string, IList<MeetingResponseCommand.RequestNodeData>> shortIdRequests;

		private IList<MeetingResponseCommand.RequestNodeData> longIdRequests;

		private XmlElement meetingResponseNode;

		private IItemIdMapping itemIdMapping;

		private struct UserResponses
		{
			public const string Accepted = "1";

			public const string TentativelyAccepted = "2";

			public const string Declined = "3";
		}

		private class RequestNodeData
		{
			public RequestNodeData(XmlNode userResponseNode, XmlNode requestIdNode, XmlNode instanceIdNode)
			{
				this.UserResponse = userResponseNode.InnerText;
				this.RequestId = requestIdNode.InnerText;
				if (instanceIdNode == null || string.IsNullOrEmpty(instanceIdNode.InnerText))
				{
					this.InstanceId = ExDateTime.MinValue;
					return;
				}
				ExDateTime instanceId;
				if (!ExDateTime.TryParseExact(instanceIdNode.InnerText, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out instanceId))
				{
					throw new AirSyncPermanentException(HttpStatusCode.BadRequest, StatusCode.InvalidDateTime, null, false)
					{
						ErrorStringForProtocolLogger = "InvalidInstanceIdFormat"
					};
				}
				this.InstanceId = instanceId;
			}

			public string UserResponse { get; private set; }

			public string RequestId { get; private set; }

			public ExDateTime InstanceId { get; private set; }

			public RespondToEventParameters RespondToEventParameters { get; private set; }

			public void FillRespondToEventParameters(XmlNode requestNode)
			{
				this.RespondToEventParameters = new RespondToEventParameters();
				string userResponse;
				if ((userResponse = this.UserResponse) != null)
				{
					if (!(userResponse == "1"))
					{
						if (!(userResponse == "2"))
						{
							if (!(userResponse == "3"))
							{
								goto IL_6C;
							}
							this.RespondToEventParameters.Response = ResponseType.Declined;
						}
						else
						{
							this.RespondToEventParameters.Response = ResponseType.TentativelyAccepted;
						}
					}
					else
					{
						this.RespondToEventParameters.Response = ResponseType.Accepted;
					}
					XmlNode xmlNode = requestNode["SendResponse", "MeetingResponse:"];
					if (xmlNode == null)
					{
						this.RespondToEventParameters.SendResponse = false;
						return;
					}
					this.RespondToEventParameters.SendResponse = true;
					XmlNode xmlNode2 = xmlNode["Importance", "MeetingResponse:"];
					if (xmlNode2 != null)
					{
						this.RespondToEventParameters.Importance = EventParametersParser.ParseImportance(xmlNode2);
					}
					XmlNode xmlNode3 = xmlNode["Body", "AirSyncBase:"];
					if (xmlNode3 != null)
					{
						this.RespondToEventParameters.Notes = EventParametersParser.ParseBody(xmlNode3);
					}
					XmlNode xmlNode4 = xmlNode["ProposedStartTime", "MeetingResponse:"];
					if (xmlNode4 != null)
					{
						this.RespondToEventParameters.ProposedStartTime = new ExDateTime?(EventParametersParser.ParseDateTime(xmlNode4, "ProposedStartTime"));
					}
					XmlNode xmlNode5 = xmlNode["ProposedEndTime", "MeetingResponse:"];
					if (xmlNode5 != null)
					{
						this.RespondToEventParameters.ProposedEndTime = new ExDateTime?(EventParametersParser.ParseDateTime(xmlNode5, "ProposedEndTime"));
					}
					return;
				}
				IL_6C:
				throw new RequestParsingException("InvalidUserResponse:" + this.UserResponse);
			}
		}
	}
}
