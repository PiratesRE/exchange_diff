using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.AirSync
{
	internal class MailboxItemFetchProvider : IItemOperationsProvider, IReusable, IDisposeTrackable, IDisposable, IMultipartResponse
	{
		internal MailboxItemFetchProvider(SyncStateStorage syncStateStorage, int protocolVersion, MailboxSession mailboxSession)
		{
			this.disposeTracker = this.GetDisposeTracker();
			AirSyncCounters.NumberOfMailboxItemFetches.Increment();
			this.syncStateStorage = syncStateStorage;
			this.mailboxSession = mailboxSession;
			IAirSyncVersionFactory airSyncVersionFactory = AirSyncProtocolVersionParserBuilder.FromVersion(protocolVersion);
			this.syncStates = new Dictionary<string, FolderSyncState>();
			this.schemaStates = new Dictionary<string, AirSyncSchemaState>();
			this.bodyPreferences = new List<BodyPreference>();
			this.bodyPartPreferences = new List<BodyPartPreference>();
			this.schemaConverterOptions = new Hashtable();
			this.schemaStates.Add("Email", airSyncVersionFactory.CreateEmailSchema(null));
			this.schemaStates.Add("Calendar", airSyncVersionFactory.CreateCalendarSchema());
			this.schemaStates.Add("Contacts", airSyncVersionFactory.CreateContactsSchema());
			this.schemaStates.Add("Tasks", airSyncVersionFactory.CreateTasksSchema());
			AirSyncSchemaState airSyncSchemaState = airSyncVersionFactory.CreateNotesSchema();
			if (airSyncSchemaState != null)
			{
				this.schemaStates.Add("Notes", airSyncSchemaState);
			}
			airSyncSchemaState = airSyncVersionFactory.CreateSmsSchema();
			if (airSyncSchemaState != null)
			{
				this.schemaStates.Add("SMS", airSyncSchemaState);
			}
		}

		public bool RightsManagementSupport
		{
			get
			{
				return this.rightsManagementSupport;
			}
		}

		public void BuildErrorResponse(string statusCode, XmlNode responseNode, ProtocolLogger protocolLogger)
		{
			if (protocolLogger != null)
			{
				protocolLogger.IncrementValue(ProtocolLoggerData.IOFetchItemErrors);
			}
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Fetch", "ItemOperations:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Status", "ItemOperations:");
			xmlNode2.InnerText = statusCode;
			xmlNode.AppendChild(xmlNode2);
			if (!string.IsNullOrEmpty(this.longId))
			{
				XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("LongId", "Search:");
				xmlNode3.InnerText = this.longId;
				xmlNode.AppendChild(xmlNode3);
			}
			if (!string.IsNullOrEmpty(this.collectionId))
			{
				XmlNode xmlNode4 = responseNode.OwnerDocument.CreateElement("CollectionId", "AirSync:");
				xmlNode4.InnerText = this.collectionId;
				xmlNode.AppendChild(xmlNode4);
			}
			if (!string.IsNullOrEmpty(this.serverId))
			{
				XmlNode xmlNode5 = responseNode.OwnerDocument.CreateElement("ServerId", "AirSync:");
				xmlNode5.InnerText = this.serverId;
				xmlNode.AppendChild(xmlNode5);
			}
			responseNode.AppendChild(xmlNode);
		}

		public void BuildResponse(XmlNode responseNode)
		{
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("Fetch", "ItemOperations:");
			XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("Status", "ItemOperations:");
			XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("Class", "AirSync:");
			XmlNode xmlNode4 = responseNode.OwnerDocument.CreateElement("Properties", "ItemOperations:");
			XmlNode xmlNode5 = responseNode.OwnerDocument.CreateElement("ServerId", "AirSync:");
			XmlNode xmlNode6 = responseNode.OwnerDocument.CreateElement("CollectionId", "AirSync:");
			XmlNode xmlNode7 = responseNode.OwnerDocument.CreateElement("LongId", "Search:");
			FlexibleSchemaStrategy missingPropertyStrategy = new FlexibleSchemaStrategy(this.schemaTags);
			this.schemaConverterOptions["BodyPreference"] = this.bodyPreferences;
			this.schemaConverterOptions["BodyPartPreference"] = this.bodyPartPreferences;
			this.schemaConverterOptions["MIMESupport"] = this.mimeSupport;
			bool flag = false;
			try
			{
				using (Item item = Item.Bind(this.mailboxSession, this.storeObjectId))
				{
					if (this.removeRightsManagementProtection)
					{
						this.IrmRemoveRestriction(item);
					}
					if (this.bodyPartPreferences != null && this.bodyPartPreferences.Count > 0)
					{
						ConversationId valueOrDefault = item.GetValueOrDefault<ConversationId>(ItemSchema.ConversationId, null);
						if (valueOrDefault != null)
						{
							Conversation conversation;
							Command.CurrentCommand.GetOrCreateConversation(valueOrDefault, true, out conversation);
							conversation.LoadItemParts(new List<StoreObjectId>
							{
								item.StoreObjectId
							});
						}
					}
					foreach (KeyValuePair<string, AirSyncSchemaState> keyValuePair in this.schemaStates)
					{
						AirSyncSchemaState value = keyValuePair.Value;
						AirSyncXsoSchemaState airSyncXsoSchemaState = value as AirSyncXsoSchemaState;
						AirSyncEntitySchemaState airSyncEntitySchemaState = value as AirSyncEntitySchemaState;
						IServerDataObject serverDataObject;
						if (airSyncXsoSchemaState != null)
						{
							serverDataObject = airSyncXsoSchemaState.GetXsoDataObject();
						}
						else
						{
							if (airSyncEntitySchemaState == null)
							{
								throw new NotImplementedException(string.Format("Schema state for \"{0}\" is not implemented yet.", keyValuePair.Key));
							}
							serverDataObject = airSyncEntitySchemaState.GetEntityDataObject();
						}
						if (serverDataObject.CanConvertItemClassUsingCurrentSchema(item.ClassName))
						{
							AirSyncDataObject airSyncDataObject = value.GetAirSyncDataObject(this.schemaConverterOptions, missingPropertyStrategy);
							if (this.rightsManagementSupport)
							{
								Command.CurrentCommand.DecodeIrmMessage(item, true);
							}
							item.Load(serverDataObject.GetPrefetchProperties());
							try
							{
								serverDataObject.Bind(item);
								airSyncDataObject.Bind(xmlNode4);
								airSyncDataObject.CopyFrom(serverDataObject);
							}
							finally
							{
								airSyncDataObject.Unbind();
								serverDataObject.Unbind();
							}
							if (this.rightsManagementSupport)
							{
								Command.CurrentCommand.SaveLicense(item);
							}
							xmlNode3.InnerText = keyValuePair.Key;
							flag = true;
							break;
						}
					}
				}
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, innerException, false)
				{
					ErrorStringForProtocolLogger = "NotFoundInMbxFetch"
				};
			}
			catch (AirSyncPermanentException)
			{
				throw;
			}
			catch (Exception ex)
			{
				if (!SyncCommand.IsItemSyncTolerableException(ex))
				{
					throw;
				}
				AirSyncDiagnostics.TraceError<Exception>(ExTraceGlobals.ConversionTracer, this, "Sync-tolerable Item conversion Exception was thrown. Location BuildResponse(). {0}", ex);
			}
			if (!flag)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_InvalidWaitTime, false)
				{
					ErrorStringForProtocolLogger = "ConvertErrorInMbxFetch"
				};
			}
			xmlNode2.InnerText = 1.ToString(CultureInfo.InvariantCulture);
			xmlNode.AppendChild(xmlNode2);
			if (this.longId != null)
			{
				xmlNode7.InnerText = this.longId;
				xmlNode.AppendChild(xmlNode7);
			}
			else
			{
				xmlNode6.InnerText = this.collectionId;
				xmlNode.AppendChild(xmlNode6);
				xmlNode5.InnerText = this.serverId;
				xmlNode.AppendChild(xmlNode5);
			}
			xmlNode.AppendChild(xmlNode3);
			xmlNode.AppendChild(xmlNode4);
			if (this.multiPartResponse)
			{
				XmlNode xmlNode8 = xmlNode4["Body"];
				if (xmlNode8 != null)
				{
					XmlNode xmlNode9 = xmlNode8["Data"];
					AirSyncBlobXmlNode airSyncBlobXmlNode = xmlNode9 as AirSyncBlobXmlNode;
					if (xmlNode9 != null && (!string.IsNullOrEmpty(xmlNode9.InnerText) || (airSyncBlobXmlNode != null && airSyncBlobXmlNode.Stream != null)))
					{
						XmlNode xmlNode10 = responseNode.OwnerDocument.CreateElement("Part", "ItemOperations:");
						xmlNode10.InnerText = this.partNumber.ToString(CultureInfo.InvariantCulture);
						xmlNode8.ReplaceChild(xmlNode10, xmlNode9);
						if (airSyncBlobXmlNode != null && airSyncBlobXmlNode.Stream != null)
						{
							this.outStream = airSyncBlobXmlNode.Stream;
							if (airSyncBlobXmlNode.Stream is AirSyncStream)
							{
								this.delayStreamDispose = true;
							}
							AirSyncDiagnostics.TraceError<Type>(ExTraceGlobals.ConversionTracer, this, "blobNode.Stream is of {0} type.", airSyncBlobXmlNode.Stream.GetType());
						}
						else
						{
							UTF8Encoding utf8Encoding = new UTF8Encoding();
							this.outStream = new MemoryStream(utf8Encoding.GetBytes(xmlNode9.InnerText));
						}
					}
				}
			}
			responseNode.AppendChild(xmlNode);
		}

		public void Execute()
		{
			if (this.longId != null)
			{
				try
				{
					this.storeObjectId = StoreObjectId.Deserialize(HttpUtility.UrlDecode(this.longId));
					return;
				}
				catch (ArgumentException innerException)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, innerException, false)
					{
						ErrorStringForProtocolLogger = "BadLongIdArgInMbxFetch"
					};
				}
				catch (FormatException innerException2)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, innerException2, false)
					{
						ErrorStringForProtocolLogger = "BadLongIdFmtInMbxFetch"
					};
				}
				catch (CorruptDataException innerException3)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, innerException3, false)
					{
						ErrorStringForProtocolLogger = "CorruptLongIdInMbxFetch"
					};
				}
			}
			this.storeObjectId = this.GetStoreObjectId(this.collectionId, this.serverId);
			if (this.storeObjectId == null)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ClientServerConversion, false)
				{
					ErrorStringForProtocolLogger = "NotFoundIdInMbxFetch"
				};
			}
		}

		public void ParseRequest(XmlNode fetchNode)
		{
			foreach (object obj in fetchNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string name;
				switch (name = xmlNode.Name)
				{
				case "LongId":
					this.longId = xmlNode.InnerText;
					continue;
				case "ServerId":
					this.serverId = xmlNode.InnerText;
					continue;
				case "CollectionId":
					this.collectionId = xmlNode.InnerText;
					continue;
				case "Options":
					this.ParseOptions(xmlNode);
					continue;
				case "RemoveRightsManagementProtection":
					if (string.IsNullOrEmpty(xmlNode.InnerText))
					{
						this.removeRightsManagementProtection = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("1"))
					{
						this.removeRightsManagementProtection = true;
						continue;
					}
					if (xmlNode.InnerText.Equals("0"))
					{
						this.removeRightsManagementProtection = false;
						continue;
					}
					throw new AirSyncPermanentException(false)
					{
						ErrorStringForProtocolLogger = "InvalidRemoveRightsManagementProtection(" + xmlNode.InnerText + ")"
					};
				case "Store":
					continue;
				}
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "BadRequestNode(" + xmlNode.Name + ")InMbxFetch"
				};
			}
			if (this.longId != null)
			{
				if (this.serverId != null || this.collectionId != null)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
					{
						ErrorStringForProtocolLogger = "IdCollisionInMbxFetch"
					};
				}
			}
			else
			{
				if (string.IsNullOrEmpty(this.serverId) || string.IsNullOrEmpty(this.collectionId))
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
					{
						ErrorStringForProtocolLogger = "NoIdInMbxFetch"
					};
				}
				SyncCollection.CollectionTypes collectionType = AirSyncUtility.GetCollectionType(this.collectionId);
				if (collectionType != SyncCollection.CollectionTypes.Mailbox && collectionType != SyncCollection.CollectionTypes.Unknown)
				{
					throw new AirSyncPermanentException(StatusCode.InvalidCombinationOfIDs, false)
					{
						ErrorStringForProtocolLogger = "BadCollectionIdInMbxFetch"
					};
				}
			}
			if (this.removeRightsManagementProtection && !this.rightsManagementSupport)
			{
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "RemoveRestrictionWithoutRightsManagementSupport"
				};
			}
		}

		public void BuildResponse(XmlNode responseNode, int partNumber)
		{
			this.partNumber = partNumber;
			this.multiPartResponse = true;
			this.BuildResponse(responseNode);
		}

		public Stream GetResponseStream()
		{
			Stream result = this.outStream;
			this.outStream = null;
			return result;
		}

		public void Reset()
		{
			this.multiPartResponse = false;
			this.partNumber = 0;
			this.longId = null;
			this.serverId = null;
			this.collectionId = null;
			this.schemaTags = null;
			this.storeObjectId = null;
			this.bodyPreferences.Clear();
			this.schemaConverterOptions.Clear();
			this.removeRightsManagementProtection = false;
			if (this.outStream != null && !this.delayStreamDispose)
			{
				this.outStream.Dispose();
				this.outStream = null;
			}
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxItemFetchProvider>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				foreach (SyncState syncState in this.syncStates.Values)
				{
					syncState.Dispose();
				}
				this.syncStates.Clear();
				if (this.outStream != null && !this.delayStreamDispose)
				{
					this.outStream.Dispose();
					this.outStream = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
				this.disposed = true;
			}
		}

		private StoreObjectId GetStoreObjectId(string collectionId, string serverId)
		{
			StoreObjectId result = null;
			FolderSyncState folderSyncState;
			if (this.syncStates.ContainsKey(collectionId))
			{
				folderSyncState = this.syncStates[collectionId];
			}
			else
			{
				folderSyncState = this.syncStateStorage.GetFolderSyncState(collectionId);
				if (folderSyncState != null)
				{
					this.syncStates.Add(collectionId, folderSyncState);
				}
			}
			if (folderSyncState != null)
			{
				ItemIdMapping itemIdMapping = (ItemIdMapping)folderSyncState[CustomStateDatumType.IdMapping];
				if (itemIdMapping != null)
				{
					MailboxSyncItemId mailboxSyncItemId = itemIdMapping[serverId] as MailboxSyncItemId;
					if (mailboxSyncItemId != null)
					{
						result = (StoreObjectId)mailboxSyncItemId.NativeId;
					}
				}
			}
			return result;
		}

		private void ParseOptions(XmlNode optionsNode)
		{
			MailboxSchemaOptionsParser mailboxSchemaOptionsParser = new MailboxSchemaOptionsParser();
			try
			{
				mailboxSchemaOptionsParser.Parse(optionsNode);
			}
			catch (AirSyncPermanentException ex)
			{
				if (StatusCode.Sync_ProtocolError == ex.AirSyncStatusCode)
				{
					throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, ex, false)
					{
						ErrorStringForProtocolLogger = "GeneralErrorInMbxFetch"
					};
				}
				throw;
			}
			this.bodyPreferences = mailboxSchemaOptionsParser.BodyPreferences;
			this.bodyPartPreferences = mailboxSchemaOptionsParser.BodyPartPreferences;
			this.schemaTags = mailboxSchemaOptionsParser.SchemaTags;
			this.mimeSupport = mailboxSchemaOptionsParser.MIMESupport;
			this.rightsManagementSupport = mailboxSchemaOptionsParser.RightsManagementSupport;
		}

		private void IrmRemoveRestriction(Item item)
		{
			if (!Command.CurrentCommand.Context.User.IrmEnabled || !Command.CurrentCommand.Context.Request.IsSecureConnection)
			{
				AirSyncDiagnostics.TraceError<string, bool, bool>(ExTraceGlobals.RequestsTracer, null, "User {0} is not IrmEnabled or the client access server is not IrmEnabled or request received over non SSL. IrmEnabled: {1}, SSLConnection: {2}.", Command.CurrentCommand.Context.User.DisplayName, Command.CurrentCommand.Context.User.IrmEnabled, Command.CurrentCommand.Context.Request.IsSecureConnection);
				throw new AirSyncPermanentException(StatusCode.IRM_FeatureDisabled, false)
				{
					ErrorStringForProtocolLogger = "RemoveRestrictionFeatureDisabled"
				};
			}
			if (AirSyncUtility.IsProtectedVoicemailItem(item))
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "User {0} called remove restrictions on protected voice mail - not supported", Command.CurrentCommand.Context.User.DisplayName);
				throw new AirSyncPermanentException(StatusCode.IRM_OperationNotPermitted, false)
				{
					ErrorStringForProtocolLogger = "RemoveRestrictionOnProtectedVoicemail"
				};
			}
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			if (rightsManagedMessageItem == null)
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "User {0} called remove restrictions on non IRM message", Command.CurrentCommand.Context.User.DisplayName);
				throw new AirSyncPermanentException(StatusCode.IRM_OperationNotPermitted, false)
				{
					ErrorStringForProtocolLogger = "RemoveRestrictionOnNonIRMMessage"
				};
			}
			rightsManagedMessageItem.OpenAsReadWrite();
			RightsManagedMessageDecryptionStatus rightsManagedMessageDecryptionStatus = RightsManagedMessageDecryptionStatus.Success;
			try
			{
				rightsManagedMessageItem.Decode(AirSyncUtility.GetOutboundConversionOptions(), true);
			}
			catch (RightsManagementPermanentException exception)
			{
				rightsManagedMessageDecryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception);
			}
			catch (RightsManagementTransientException exception2)
			{
				rightsManagedMessageDecryptionStatus = RightsManagedMessageDecryptionStatus.CreateFromException(exception2);
			}
			if (rightsManagedMessageDecryptionStatus.Failed)
			{
				AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "Failed to decode item : {0}", rightsManagedMessageDecryptionStatus.FailureCode.ToString());
				throw new AirSyncPermanentException(StatusCode.IRM_PermanentError, rightsManagedMessageDecryptionStatus.Exception, false)
				{
					ErrorStringForProtocolLogger = "RemoveRestrictionDecodeFailed"
				};
			}
			try
			{
				rightsManagedMessageItem.SetRestriction(null);
			}
			catch (RightsManagementPermanentException ex)
			{
				if (ex.FailureCode == RightsManagementFailureCode.UserRightNotGranted)
				{
					AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, null, "Failed to remove restrictions on item : {0}", ex.ToString());
					throw new AirSyncPermanentException(StatusCode.IRM_OperationNotPermitted, ex, false)
					{
						ErrorStringForProtocolLogger = "RemoveRestrictionRightNotGranted"
					};
				}
				throw;
			}
			ConflictResolutionResult conflictResolutionResult = rightsManagedMessageItem.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				AirSyncDiagnostics.TraceError(ExTraceGlobals.RequestsTracer, null, "Failed to remove restrictions on item because of conflicts");
				throw new AirSyncPermanentException(StatusCode.IRM_PermanentError, false)
				{
					ErrorStringForProtocolLogger = "RemoveRestrictionConflict"
				};
			}
			item.Load();
		}

		private List<BodyPreference> bodyPreferences;

		private List<BodyPartPreference> bodyPartPreferences;

		private string collectionId;

		private string longId;

		private bool removeRightsManagementProtection;

		private MailboxSession mailboxSession;

		private MIMESupportValue mimeSupport;

		private bool rightsManagementSupport;

		private bool multiPartResponse;

		private Stream outStream;

		private int partNumber;

		private IDictionary schemaConverterOptions;

		private Dictionary<string, AirSyncSchemaState> schemaStates;

		private Dictionary<string, bool> schemaTags;

		private string serverId;

		private StoreObjectId storeObjectId;

		private IDictionary<string, FolderSyncState> syncStates;

		private SyncStateStorage syncStateStorage;

		private bool disposed;

		private bool delayStreamDispose;

		private DisposeTracker disposeTracker;
	}
}
