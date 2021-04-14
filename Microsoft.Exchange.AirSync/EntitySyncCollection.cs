using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Xml;
using Microsoft.Exchange.AirSync.SchemaConverter;
using Microsoft.Exchange.AirSync.SchemaConverter.AirSync;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.SchemaConverter.Entity;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Entities.Calendaring;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EntitySyncCollection : SyncCollection
	{
		public EntitySyncCollection(StoreSession storeSession, int protocolVersion) : base(storeSession, protocolVersion)
		{
		}

		public override void CreateSyncProvider()
		{
			base.SyncProviderFactory = new EntitySyncProviderFactory(base.StoreSession);
		}

		public override void OpenSyncState(bool autoLoadFilterAndSyncKey, SyncStateStorage syncStateStorage)
		{
			base.OpenSyncState(autoLoadFilterAndSyncKey, syncStateStorage);
		}

		public override void OpenFolderSync()
		{
			base.FolderSync = base.SyncState.GetFolderSync(base.ConflictResolutionPolicy, (ISyncProvider tempProvider, FolderSyncState tempFolderSyncState, ConflictResolutionPolicy tempConflictResolutionPolicy, bool tempDeferStateModifications) => new EntityFolderSync(tempProvider, tempFolderSyncState, tempConflictResolutionPolicy, tempDeferStateModifications));
		}

		public override uint GetServerChanges(uint maxWindowSize, bool enumerateAllOperations)
		{
			EntitySyncProvider entitySyncProvider = (EntitySyncProvider)base.FolderSync.SyncProvider;
			entitySyncProvider.WindowStart = ExDateTime.Today;
			entitySyncProvider.WindowEnd = entitySyncProvider.WindowStart.AddMonths(13);
			entitySyncProvider.CalendarSyncState = ((EntityFolderSync)base.FolderSync).AirSyncCalendarSyncState;
			uint serverChanges = base.GetServerChanges(maxWindowSize, enumerateAllOperations);
			if (base.GetChanges)
			{
				((EntityFolderSync)base.FolderSync).AirSyncCalendarSyncState = entitySyncProvider.CalendarSyncState;
			}
			return serverChanges;
		}

		private EntityDataObject EntityDataObject { get; set; }

		protected IEvents Events
		{
			get
			{
				if (this.events == null)
				{
					EntitySyncProviderFactory entitySyncProviderFactory = (EntitySyncProviderFactory)base.SyncProviderFactory;
					StoreObjectId folderId = entitySyncProviderFactory.FolderId;
					string key = EntitySyncItem.GetKey(base.StoreSession.MailboxGuid, folderId);
					ICalendaringContainer calendaringContainer = new CalendaringContainer(base.StoreSession, null);
					this.events = calendaringContainer.Calendars[key].Events;
				}
				return this.events;
			}
		}

		public override bool AllowGetChangesOnSyncKeyZero()
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "EntitySyncCollection.AllowGetChangesOnSyncKeyZero:false");
			return false;
		}

		public override void ConvertServerToClientObject(ISyncItem syncItem, XmlNode airSyncParentNode, SyncOperation changeObject, GlobalInfo globalInfo)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "EntitySyncCollection.ConvertServerToClientObject");
			IItem item = ((EntitySyncItem)syncItem).Item;
			Item item2 = ((EntitySyncItem)syncItem).NativeItem as Item;
			if (item2 == null || !this.EntityDataObject.CanConvertItemClassUsingCurrentSchema(item2.ClassName))
			{
				throw new ConversionException(string.Format("Cannot convert item '{0}' of .NET type \"{1}\" using current schema.  ClassName: '{2}'", item.Id, item.GetType().FullName, (item2 == null) ? "<NULL>" : item2.ClassName));
			}
			try
			{
				base.AirSyncDataObject.Bind(airSyncParentNode);
				this.EntityDataObject.Bind(item);
				AirSyncDiagnostics.FaultInjectionTracer.TraceTest(2170957117U);
				base.AirSyncDataObject.CopyFrom(this.EntityDataObject);
			}
			finally
			{
				this.EntityDataObject.Unbind();
				base.AirSyncDataObject.Unbind();
			}
			base.ApplyChangeTrackFilter(changeObject, airSyncParentNode);
			base.SetHasChanges(changeObject);
		}

		public override ISyncItemId ConvertClientToServerObjectAndSave(SyncCommandItem syncCommandItem, ref uint maxWindowSize, ref bool mergeToClient)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.ConvertClientToServerObjectAndSave");
			base.CheckFullAccess();
			ItemIdMapping itemIdMapping = base.ItemIdMapping;
			IItem item = ((EntitySyncItem)syncCommandItem.Item).Item;
			try
			{
				this.EntityDataObject.Bind(item);
				base.AirSyncDataObject.Bind(syncCommandItem.XmlNode);
				this.EntityDataObject.CopyFrom(base.AirSyncDataObject);
			}
			finally
			{
				base.AirSyncDataObject.Unbind();
				this.EntityDataObject.Unbind();
			}
			ISyncItemId syncItemId = this.ApplyChanges(syncCommandItem);
			syncCommandItem.ChangeTrackingInformation = base.ChangeTrackFilter.UpdateChangeTrackingInformation(syncCommandItem.XmlNode, syncCommandItem.ChangeTrackingInformation);
			syncCommandItem.SyncId = base.ItemIdMapping[syncItemId];
			return syncItemId;
		}

		public override OperationResult DeleteSyncItem(SyncCommandItem syncCommandItem, bool deletesAsMoves)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.DeleteSyncItem(SyncCommandItem)");
			EntitySyncProviderFactory entitySyncProviderFactory = (EntitySyncProviderFactory)base.SyncProviderFactory;
			StoreObjectId folderId = entitySyncProviderFactory.FolderId;
			if (AirSyncUtility.GetAirSyncFolderTypeClass(folderId) == "Calendar")
			{
				CancelEventParameters parameters;
				if (syncCommandItem.XmlNode != null)
				{
					try
					{
						parameters = EventParametersParser.ParseCancel(syncCommandItem.XmlNode);
						goto IL_72;
					}
					catch (RequestParsingException ex)
					{
						throw new AirSyncPermanentException(StatusCode.Sync_ProtocolError, ex.LocalizedString, ex, false)
						{
							ErrorStringForProtocolLogger = ex.LogMessage
						};
					}
				}
				parameters = new CancelEventParameters();
				IL_72:
				this.DeleteItem(syncCommandItem.Id, parameters);
				return OperationResult.Succeeded;
			}
			return this.DeleteSyncItem(syncCommandItem.ServerId, deletesAsMoves);
		}

		public override OperationResult DeleteSyncItem(ISyncItemId syncItemId, bool deletesAsMoves)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.DeleteSyncItem(ISyncItemId)");
			this.DeleteItem(syncItemId, new CancelEventParameters());
			return OperationResult.Succeeded;
		}

		public override bool HasSchemaPropertyChanged(ISyncItem syncItem, int?[] oldChangeTrackingInformation, XmlDocument xmlResponse, MailboxLogger mailboxLogger)
		{
			bool flag = false;
			XmlNode xmlItemRoot = xmlResponse.CreateElement("ApplicationData", "AirSync:");
			try
			{
				this.EntityDataObject.Bind(syncItem.NativeItem);
				base.AirSyncDataObject.Bind(xmlItemRoot);
				base.AirSyncDataObject.CopyFrom(this.EntityDataObject);
			}
			catch (Exception ex)
			{
				if (!SyncCommand.IsItemSyncTolerableException(ex))
				{
					throw;
				}
				if (mailboxLogger != null)
				{
					mailboxLogger.SetData(MailboxLogDataName.MailboxSyncCommand_HasSchemaPropertyChanged_Exception, ex);
				}
				AirSyncUtility.ExceptionToStringHelper arg = new AirSyncUtility.ExceptionToStringHelper(ex);
				AirSyncDiagnostics.TraceError<AirSyncUtility.ExceptionToStringHelper>(ExTraceGlobals.RequestsTracer, this, "Sync-tolerable Entity conversion Exception was thrown. HasSchemaPropertyChanged() {0}", arg);
				flag = true;
			}
			finally
			{
				this.EntityDataObject.Unbind();
				base.AirSyncDataObject.Unbind();
			}
			if (!flag)
			{
				int?[] array = base.ChangeTrackFilter.UpdateChangeTrackingInformation(xmlItemRoot, oldChangeTrackingInformation);
				AirSyncDiagnostics.TraceDebug<int?[], int?[]>(ExTraceGlobals.RequestsTracer, this, "HasSchemaPropertyChanged oldCCI {0} newCCI {1}", oldChangeTrackingInformation, array);
				flag = !ChangeTrackingFilter.IsEqual(array, oldChangeTrackingInformation);
			}
			return flag;
		}

		public override void SetSchemaConverterOptions(IDictionary schemaConverterOptions, IAirSyncVersionFactory versionFactory)
		{
			AirSyncEntitySchemaState airSyncEntitySchemaState = (AirSyncEntitySchemaState)base.SchemaState;
			IAirSyncMissingPropertyStrategy missingPropertyStrategy = versionFactory.CreateMissingPropertyStrategy(base.SupportedTags);
			base.ChangeTrackFilter = ChangeTrackingFilterFactory.CreateFilter(base.ClassType, base.ProtocolVersion);
			base.AirSyncDataObject = airSyncEntitySchemaState.GetAirSyncDataObject(schemaConverterOptions, missingPropertyStrategy);
			this.EntityDataObject = airSyncEntitySchemaState.GetEntityDataObject();
		}

		public override ISyncItem CreateSyncItem(SyncCommandItem item)
		{
			string classType = item.ClassType;
			AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "EntitySyncCollection.CreateSyncItem({0})", classType);
			base.CheckFullAccess();
			string a;
			if ((a = classType) != null && a == "Calendar")
			{
				return EntitySyncItem.Bind(new Event());
			}
			throw new AirSyncPermanentException(HttpStatusCode.NotImplemented, StatusCode.UnexpectedItemClass, null, false)
			{
				ErrorStringForProtocolLogger = "BadClassType(" + classType + ")onSync"
			};
		}

		public override ISyncItem BindToSyncItem(ISyncItemId syncItemId, bool prefetchProperties)
		{
			return base.FolderSync.GetItem(syncItemId, new PropertyDefinition[0]);
		}

		protected override ISyncItem CreateSyncItem(Item mailboxItem)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "EntitySyncCollection.CreateSyncItem(mailboxItem)");
			return EntitySyncItem.Bind(mailboxItem);
		}

		protected override void AddExtraNodes(XmlNode responseNode, SyncCommandItem item)
		{
			if (item.Status != "1")
			{
				return;
			}
			XmlNode xmlNode = responseNode.OwnerDocument.CreateElement("ApplicationData", "AirSync:");
			if (item.ChangeType == ChangeType.Add && !string.IsNullOrEmpty(item.UID))
			{
				XmlNode xmlNode2 = responseNode.OwnerDocument.CreateElement("UID", "Calendar:");
				xmlNode2.InnerText = item.UID;
				xmlNode.AppendChild(xmlNode2);
			}
			if (item.AddedAttachments != null && item.AddedAttachments.Count > 0)
			{
				XmlNode xmlNode3 = responseNode.OwnerDocument.CreateElement("Attachments", "AirSyncBase:");
				foreach (KeyValuePair<string, string> keyValuePair in item.AddedAttachments)
				{
					XmlNode xmlNode4 = responseNode.OwnerDocument.CreateElement("Attachment", "AirSyncBase:");
					XmlNode xmlNode5 = responseNode.OwnerDocument.CreateElement("ClientId", "AirSyncBase:");
					xmlNode5.InnerText = keyValuePair.Key;
					xmlNode4.AppendChild(xmlNode5);
					XmlNode xmlNode6 = responseNode.OwnerDocument.CreateElement("FileReference", "AirSyncBase:");
					xmlNode6.InnerText = keyValuePair.Value;
					xmlNode4.AppendChild(xmlNode6);
					xmlNode3.AppendChild(xmlNode4);
				}
				xmlNode.AppendChild(xmlNode3);
			}
			if (xmlNode.HasChildNodes)
			{
				responseNode.AppendChild(xmlNode);
			}
		}

		public override void SetFolderSyncOptions(IAirSyncVersionFactory versionFactory, bool isQuarantineMailAvailable, GlobalInfo globalInfo)
		{
		}

		protected ISyncItemId ApplyChanges(SyncCommandItem syncCommandItem)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.ApplyChanges");
			EntitySyncItem entitySyncItem = (EntitySyncItem)syncCommandItem.Item;
			EntitySyncProviderFactory factory = (EntitySyncProviderFactory)base.SyncProviderFactory;
			Event @event = entitySyncItem.Item as Event;
			try
			{
				if (@event == null)
				{
					throw new NotImplementedException(string.Format("SyncCollection.ApplyChanges item type {0} is not supported", entitySyncItem.GetType().FullName));
				}
				switch (syncCommandItem.ChangeType)
				{
				case ChangeType.Add:
				{
					List<IAttachment> list = null;
					if (@event.Attachments != null && @event.Attachments.Count > 0)
					{
						list = @event.Attachments;
						@event.Attachments = null;
						@event.IsDraft = true;
					}
					@event = this.Events.Create(@event, null);
					entitySyncItem.UpdateId(factory, @event.Id);
					base.ItemIdMapping.Add(entitySyncItem.Id);
					syncCommandItem.UID = @event.Id;
					if (list == null)
					{
						goto IL_16A;
					}
					@event = new Event
					{
						Id = @event.Id,
						Attachments = list
					};
					entitySyncItem = EntitySyncItem.Bind(@event);
					break;
				}
				case ChangeType.Change:
					break;
				case (ChangeType)3:
					goto IL_134;
				case ChangeType.Delete:
					throw new InvalidOperationException("SyncCollection.ApplyChanges does not support Delete, use DeleteItem.");
				default:
					goto IL_134;
				}
				syncCommandItem.AddedAttachments = this.ApplyAttachmentChanges(entitySyncItem);
				@event.Id = this.Events.Update(@event, null).Id;
				((EntitySyncItem)syncCommandItem.Item).Reload();
				goto IL_16A;
				IL_134:
				throw new NotImplementedException(string.Format("SyncCollection.ApplyChanges ChangeType {0} is not supported", syncCommandItem.ChangeType));
				IL_16A:;
			}
			finally
			{
				if (!object.ReferenceEquals(entitySyncItem, (EntitySyncItem)syncCommandItem.Item))
				{
					entitySyncItem.Dispose();
				}
			}
			return syncCommandItem.Item.Id;
		}

		protected Dictionary<string, string> ApplyAttachmentChanges(EntitySyncItem item)
		{
			Event @event = item.Item as Event;
			Dictionary<string, string> dictionary = null;
			if (@event.Attachments != null && @event.Attachments.Count > 0)
			{
				dictionary = new Dictionary<string, string>(@event.Attachments.Count);
				@event.IsDraft = true;
				List<IAttachment> attachments = @event.Attachments;
				@event.Attachments = null;
				IAttachments attachments2 = this.Events[@event.Id].Attachments;
				foreach (IAttachment attachment in attachments)
				{
					if (attachment is EntityDeleteAttachment)
					{
						attachments2.Delete(attachment.Id, null);
					}
					else
					{
						string id = attachment.Id;
						attachment.Id = null;
						string id2 = attachments2.Create(attachment, null).Id;
						dictionary[id] = HttpUtility.UrlEncode(item.Item.Id + ":" + id2);
					}
				}
				@event.IsDraft = false;
				@event.ChangeKey = null;
			}
			return dictionary;
		}

		protected void DeleteItem(ISyncItemId syncItemId, CancelEventParameters parameters)
		{
			AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "SyncCollection.DeleteItem");
			string key = EntitySyncItem.GetKey(base.StoreSession.MailboxGuid, (StoreId)syncItemId.NativeId);
			this.Events.Cancel(key, parameters, null);
			base.ItemIdMapping.Delete(new ISyncItemId[]
			{
				syncItemId
			});
		}

		private IEvents events;
	}
}
