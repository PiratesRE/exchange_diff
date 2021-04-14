using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class ServiceCommandBase
	{
		internal int CurrentStep { get; set; }

		internal CallContext CallContext { get; private set; }

		protected virtual IParticipantResolver ConstructParticipantResolver()
		{
			return Microsoft.Exchange.Services.Core.Types.ParticipantResolver.Create(this.CallContext, int.MaxValue);
		}

		private protected IdConverter IdConverter { protected get; private set; }

		protected IParticipantResolver ParticipantResolver
		{
			get
			{
				if (this.participantResolver == null)
				{
					this.participantResolver = this.ConstructParticipantResolver();
				}
				return this.participantResolver;
			}
		}

		protected MailboxSession MailboxIdentityMailboxSession
		{
			get
			{
				if (this.mailboxIdentityMailboxSession == null)
				{
					this.mailboxIdentityMailboxSession = this.GetMailboxIdentityMailboxSession();
				}
				return this.mailboxIdentityMailboxSession;
			}
		}

		public ServiceCommandBase(CallContext callContext)
		{
			ServiceCommandBase.ThrowIfNull(callContext, "callContext", "ServiceCommandBase::ServiceCommandBase");
			this.CallContext = callContext;
			this.IdConverter = new IdConverter(callContext);
			this.InternalInitialize();
		}

		internal bool PreExecuteSucceeded { get; private set; }

		internal static Item GetXsoItemForUpdate(IdAndSession idAndSession, ToXmlPropertyList propertyList)
		{
			Item xsoItem = ServiceCommandBase.GetXsoItem(idAndSession.Session, idAndSession.Id, propertyList);
			return ServiceCommandBase.OpenForUpdate(xsoItem);
		}

		internal static string GetFolderLogString(StoreObjectId folderId, StoreSession session)
		{
			string result;
			if (!(session is MailboxSession))
			{
				result = "Other";
			}
			else if (ServiceCommandBase.IsDefaultFolderId(folderId, session, DefaultFolderType.Inbox))
			{
				result = "Inbox";
			}
			else if (ServiceCommandBase.IsDefaultFolderId(folderId, session, DefaultFolderType.Drafts))
			{
				result = "Drafts";
			}
			else if (ServiceCommandBase.IsDefaultFolderId(folderId, session, DefaultFolderType.SentItems))
			{
				result = "SentItems";
			}
			else if (ServiceCommandBase.IsDefaultFolderId(folderId, session, DefaultFolderType.DeletedItems))
			{
				result = "DeletedItems";
			}
			else
			{
				result = "Other";
			}
			return result;
		}

		internal static Item GetXsoItem(StoreSession session, StoreId id, params PropertyDefinition[] properties)
		{
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(id);
			StoreObjectType objectType = asStoreObjectId.ObjectType;
			if (!IdConverter.IsItemId(asStoreObjectId))
			{
				throw new CannotUseFolderIdForItemIdException();
			}
			Item item = null;
			bool flag = false;
			Item result;
			try
			{
				try
				{
					item = Item.Bind(session, id, properties);
					if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
					{
						ServiceCommandBase.RebindAsMessageIfNecessary(ref item, properties);
					}
					XsoDataConverter.VerifyObjectTypeAssumptions(objectType, item);
				}
				catch (WrongObjectTypeException innerException)
				{
					throw new InvalidStoreIdException(CoreResources.IDs.ErrorInvalidId, innerException);
				}
				catch (StoragePermanentException ex)
				{
					if (ex.InnerException is MapiExceptionNoSupport || ex.InnerException is MapiExceptionCallFailed)
					{
						if (ExTraceGlobals.ServiceCommandBaseCallTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder = new StringBuilder();
							if (properties == null)
							{
								stringBuilder.AppendLine("<Property Array is NULL>");
							}
							else
							{
								foreach (PropertyDefinition propertyDefinition in properties)
								{
									stringBuilder.AppendLine((propertyDefinition == null) ? "<NULL>" : propertyDefinition.ToString());
								}
							}
							ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<string, string, string>(0L, "[ServiceCommandBase::GetXsoItem] Encountered StoragePermanentException when trying to bind to an item.  Inner exception class: {0}\r\n, Inner exception message: {1}\r\n,Properties fetched: \r\n{2}", ex.InnerException.GetType().FullName, ex.InnerException.Message, stringBuilder.ToString());
						}
						throw new ObjectCorruptException(ex, true);
					}
					if (ExTraceGlobals.ServiceCommandBaseCallTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						string arg = (ex.InnerException == null) ? string.Empty : string.Format(CultureInfo.InvariantCulture, ".  Inner exception was Class: {0}, Message {1}", new object[]
						{
							ex.InnerException.GetType().FullName,
							ex.InnerException.Message
						});
						ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<string, string, string>(0L, "[ServiceCommandBase::GetXsoItem] encountered exception - Class: {0}, Message: {1}{2}", ex.GetType().FullName, ex.Message, arg);
					}
					throw;
				}
				XsoDataConverter.VerifyObjectTypeAssumptions(objectType, item);
				flag = true;
				result = item;
			}
			finally
			{
				if (!flag && item != null)
				{
					item.Dispose();
				}
			}
			return result;
		}

		protected static bool IsDefaultFolderId(StoreObjectId folderId, StoreSession session, DefaultFolderType type)
		{
			StoreObjectId defaultFolderId = session.GetDefaultFolderId(type);
			return defaultFolderId != null && defaultFolderId.Equals(folderId);
		}

		protected static ExDateTime? ParseExDateTimeString(string dateTime)
		{
			if (!string.IsNullOrEmpty(dateTime))
			{
				return new ExDateTime?(ExDateTimeConverter.ParseTimeZoneRelated(dateTime, EWSSettings.RequestTimeZone));
			}
			return null;
		}

		protected static DateTime? GetUtcDateTime(ExDateTime? exDateTime)
		{
			if (exDateTime != null)
			{
				return new DateTime?(exDateTime.Value.UniversalTime);
			}
			return null;
		}

		private static void RebindAsMessageIfNecessary(ref Item item, PropertyDefinition[] properties)
		{
			if (item is CalendarItemBase || item is ContactBase || item is MessageItem || item is PostItem || item is Task)
			{
				return;
			}
			using (Item item2 = item)
			{
				item = Item.BindAsMessage(item2.Session, item2.Id, properties);
			}
		}

		internal static Item GetXsoItem(StoreSession session, StoreId id, ToXmlPropertyList propertyList)
		{
			return ServiceCommandBase.GetXsoItem(session, id, propertyList.GetPropertyDefinitions());
		}

		internal static Folder GetXsoFolder(StoreSession session, StoreId id, ToXmlPropertyList propertyList)
		{
			return ServiceCommandBase.GetXsoFolder(session, id, ref propertyList);
		}

		internal static Folder GetXsoFolder(StoreSession session, StoreId id, ref ToXmlPropertyList propertyList)
		{
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(id);
			StoreObjectType objectType = asStoreObjectId.ObjectType;
			if (!asStoreObjectId.IsFolderId)
			{
				throw new CannotUseItemIdForFolderIdException();
			}
			Folder folder = Folder.Bind(session, id, propertyList.GetPropertyDefinitions());
			if (folder is SearchFolder)
			{
				propertyList = XsoDataConverter.GetPropertyList(folder, propertyList.ResponseShape);
				folder.Load(propertyList.GetPropertyDefinitions());
			}
			try
			{
				XsoDataConverter.VerifyObjectTypeAssumptions(objectType, folder);
			}
			catch (ObjectNotFoundException)
			{
				folder.Dispose();
				throw;
			}
			if (objectType == StoreObjectType.Folder && folder.Id.ObjectId.ObjectType != objectType)
			{
				propertyList = XsoDataConverter.GetPropertyList(folder, propertyList.ResponseShape);
				folder.Load(propertyList.GetPropertyDefinitions());
			}
			return folder;
		}

		internal static Folder GetXsoFolder(StoreSession session, StoreId id, ref ToServiceObjectPropertyList propertyList)
		{
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(id);
			StoreObjectType objectType = asStoreObjectId.ObjectType;
			if (!asStoreObjectId.IsFolderId)
			{
				throw new CannotUseItemIdForFolderIdException();
			}
			Folder folder = Folder.Bind(session, id, propertyList.GetPropertyDefinitions());
			if (folder is SearchFolder)
			{
				propertyList = XsoDataConverter.GetToServiceObjectPropertyList(folder, propertyList.ResponseShape);
				folder.Load(propertyList.GetPropertyDefinitions());
			}
			try
			{
				XsoDataConverter.VerifyObjectTypeAssumptions(objectType, folder);
			}
			catch (ObjectNotFoundException)
			{
				folder.Dispose();
				throw;
			}
			if (objectType == StoreObjectType.Folder && folder.Id.ObjectId.ObjectType != objectType)
			{
				propertyList = XsoDataConverter.GetToServiceObjectPropertyList(folder, propertyList.ResponseShape);
				folder.Load(propertyList.GetPropertyDefinitions());
			}
			return folder;
		}

		internal FolderId GetServiceFolderIdFromStoreId(StoreId storeId, IdAndSession idAndSession)
		{
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, idAndSession, null);
			return new FolderId
			{
				Id = concatenatedId.Id,
				ChangeKey = concatenatedId.ChangeKey
			};
		}

		internal ItemId GetServiceItemIdFromStoreId(StoreId storeId, IdAndSession idAndSession)
		{
			ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, idAndSession, null);
			return new ItemId
			{
				Id = concatenatedId.Id,
				ChangeKey = concatenatedId.ChangeKey
			};
		}

		public IEwsBudget CallerBudget
		{
			get
			{
				return this.CallContext.Budget;
			}
		}

		public int ObjectsChanged
		{
			get
			{
				return this.objectsChanged;
			}
		}

		internal static void ThrowIfNull(object objectToCheck, string parameterName, string methodName)
		{
			if (objectToCheck == null)
			{
				string message = ServiceCommandBase.BuildExceptionMessage(methodName, parameterName, "is null.");
				throw new ArgumentNullException(parameterName, message);
			}
		}

		internal virtual bool SupportsExternalUsers
		{
			get
			{
				return false;
			}
		}

		internal virtual Offer ExpectedOffer
		{
			get
			{
				return null;
			}
		}

		internal virtual bool IsDelayExecuted
		{
			get
			{
				return false;
			}
		}

		internal virtual TimeSpan? MaxExecutionTime
		{
			get
			{
				return null;
			}
		}

		internal static void ThrowIfNullOrEmpty(string stringToCheck, string parameterName, string methodName)
		{
			ServiceCommandBase.ThrowIfNull(stringToCheck, parameterName, methodName);
			ServiceCommandBase.ThrowIfEmpty(stringToCheck, parameterName, methodName);
		}

		protected static void ThrowIfNullOrEmpty<T>(IList<T> list, string parameterName, string methodName)
		{
			ServiceCommandBase.ThrowIfNull(list, parameterName, methodName);
			ServiceCommandBase.ThrowIfEmpty<T>(list, parameterName, methodName);
		}

		protected static void ThrowIfEmpty<T>(IList<T> list, string parameterName, string methodName)
		{
			if (list.Count == 0)
			{
				string message = ServiceCommandBase.BuildExceptionMessage(methodName, parameterName, "is empty.");
				throw new ArgumentException(message, parameterName);
			}
		}

		protected static void ThrowIfEmpty(string stringToCheck, string parameterName, string methodName)
		{
			if (stringToCheck.Length == 0)
			{
				string message = ServiceCommandBase.BuildExceptionMessage(methodName, parameterName, "is empty.");
				throw new ArgumentException(message, parameterName);
			}
		}

		protected static void RequireUpToDateItem(StoreId incomingId, Item item)
		{
			if (item.Id == null)
			{
				item.OpenAsReadWrite();
				VersionedId id = item.Id;
			}
			ServiceCommandBase.RequireUpToDateObject(incomingId, item);
		}

		protected static bool IsAssociated(Item storeItem)
		{
			MessageFlags messageFlags = (MessageFlags)storeItem[MessageItemSchema.Flags];
			return (messageFlags & MessageFlags.IsAssociated) == MessageFlags.IsAssociated;
		}

		protected static Item GetXsoItemForUpdate(IdAndSession idAndSession, params PropertyDefinition[] properties)
		{
			Item xsoItem = ServiceCommandBase.GetXsoItem(idAndSession.Session, idAndSession.Id, properties);
			return ServiceCommandBase.OpenForUpdate(xsoItem);
		}

		protected static bool IsOrganizerMeeting(CalendarItemBase calendarItemBase)
		{
			return calendarItemBase != null && calendarItemBase.IsOrganizer();
		}

		protected DelegateSessionHandleWrapper GetDelegateSessionHandleWrapper(IdAndSession idAndSession)
		{
			return this.GetDelegateSessionHandleWrapper(idAndSession, false);
		}

		protected DelegateSessionHandleWrapper GetDelegateSessionHandleWrapper(IdAndSession idAndSession, bool checkSameAccountForOwnerLogon)
		{
			if (idAndSession == null)
			{
				return null;
			}
			MailboxSession mailboxSession = idAndSession.Session as MailboxSession;
			if (mailboxSession == null || this.CallContext.AccessingPrincipal == null || (mailboxSession.LogonType == LogonType.Owner && (!checkSameAccountForOwnerLogon || string.Equals(mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), this.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), StringComparison.OrdinalIgnoreCase))) || mailboxSession.LogonType == LogonType.Admin || mailboxSession.LogonType == LogonType.SystemService || !ExchangeVersionDeterminer.MatchesLocalServerVersion(this.CallContext.AccessingPrincipal.MailboxInfo.Location.ServerVersion))
			{
				return null;
			}
			this.LogDelegateSession(mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
			return new DelegateSessionHandleWrapper(this.MailboxIdentityMailboxSession.GetDelegateSessionHandleForEWS(mailboxSession.MailboxOwner));
		}

		protected MailboxSession GetMailboxSession(string smtpAddress)
		{
			return this.CallContext.SessionCache.GetMailboxSessionBySmtpAddress(smtpAddress);
		}

		protected MailboxSession GetMailboxIdentityMailboxSession()
		{
			return this.CallContext.SessionCache.GetMailboxIdentityMailboxSession();
		}

		protected void SetProperties(StoreObject storeObject, ServiceObject serviceObject)
		{
			if (serviceObject.LoadedProperties.Count > 0)
			{
				string className = storeObject.ClassName;
				XsoDataConverter.SetProperties(storeObject, serviceObject, this.IdConverter);
				ServiceCommandBase.ValidateClassChange(storeObject, className);
			}
		}

		protected virtual void UpdateProperties(StoreObject storeObject, PropertyUpdate[] propertyUpdates, bool suppressReadReceipts)
		{
			this.UpdateProperties(storeObject, propertyUpdates, suppressReadReceipts, null);
		}

		protected virtual void UpdateProperties(StoreObject storeObject, PropertyUpdate[] propertyUpdates, bool suppressReadReceipts, IFeaturesManager featuresManager)
		{
			string className = storeObject.ClassName;
			try
			{
				XsoDataConverter.UpdateProperties(storeObject, propertyUpdates, this.IdConverter, suppressReadReceipts, featuresManager);
			}
			catch (PropertyValidationException ex)
			{
				throw new PropertyUpdateException(SearchSchemaMap.GetPropertyPaths(ex.PropertyValidationErrors), ex);
			}
			ServiceCommandBase.ValidateClassChange(storeObject, className);
		}

		protected MailboxTarget GetMailboxTarget(StoreSession session)
		{
			if (Util.IsArchiveMailbox(session))
			{
				return MailboxTarget.Archive;
			}
			if (session is PublicFolderSession)
			{
				return MailboxTarget.PublicFolder;
			}
			if (session is MailboxSession && !object.Equals(this.MailboxIdentityMailboxSession, session))
			{
				return MailboxTarget.SharedFolder;
			}
			return MailboxTarget.Primary;
		}

		internal static void ValidateClassChange(StoreObject storeObject, string preUpdateClassName)
		{
			StoreObjectType objectType = ObjectClass.GetObjectType(preUpdateClassName);
			StoreObjectType objectType2 = ObjectClass.GetObjectType(storeObject.ClassName);
			ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<string, string>(0L, "ServiceCommandBase.ValidateClassChange().  pre update class name: '{0}' post update class name: {1}", preUpdateClassName, storeObject.ClassName);
			if (objectType != objectType2)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(CallContext.Current.ProtocolLog, "ObjectTypeChanged", string.Format("{0}:{1} {2}:{3}", new object[]
				{
					objectType,
					objectType2,
					preUpdateClassName,
					storeObject.ClassName
				}));
				if (objectType == StoreObjectType.Unknown)
				{
					if (ServiceCommandBase.IsBaseObjectTypeChange(storeObject, objectType2))
					{
						ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug(0L, "ServiceCommandBase.ValidateClassChange() detected change to base object.");
						throw new ObjectTypeChangedException();
					}
				}
				else
				{
					if (objectType == StoreObjectType.Message)
					{
						if (objectType2 == StoreObjectType.MeetingMessage || objectType2 == StoreObjectType.MeetingRequest || objectType2 == StoreObjectType.MeetingResponse || objectType2 == StoreObjectType.MeetingCancellation)
						{
							return;
						}
						if (objectType2 == StoreObjectType.Report)
						{
							return;
						}
					}
					if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
					{
						if (objectType == StoreObjectType.Unknown && objectType2 == StoreObjectType.Message)
						{
							return;
						}
						if (objectType == StoreObjectType.Message && objectType2 == StoreObjectType.Unknown)
						{
							return;
						}
					}
					if (ObjectClass.IsCalendarItemOccurrence(preUpdateClassName) && ObjectClass.IsRecurrenceException(storeObject.ClassName))
					{
						return;
					}
					ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug(0L, "ServiceCommandBase.ValidateClassChange() detected change of object type.");
					throw new ObjectTypeChangedException();
				}
			}
		}

		private static bool IsBaseObjectTypeChange(StoreObject storeObject, StoreObjectType postUpdateObjectType)
		{
			bool flag = storeObject is Folder;
			bool flag2 = false;
			switch (postUpdateObjectType)
			{
			case StoreObjectType.Folder:
			case StoreObjectType.CalendarFolder:
			case StoreObjectType.ContactsFolder:
			case StoreObjectType.TasksFolder:
			case StoreObjectType.NotesFolder:
			case StoreObjectType.JournalFolder:
			case StoreObjectType.SearchFolder:
			case StoreObjectType.OutlookSearchFolder:
				flag2 = true;
				break;
			}
			return flag != flag2 || (!flag && ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1) && postUpdateObjectType != StoreObjectType.Unknown && postUpdateObjectType != StoreObjectType.Message);
		}

		private static string BuildExceptionMessage(string methodName, string parameterName, string message)
		{
			if (!string.IsNullOrEmpty(methodName))
			{
				return string.Format(CultureInfo.InvariantCulture, "[{0}] {1} {2}", new object[]
				{
					methodName,
					parameterName,
					message
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "{1} {2}", new object[]
			{
				parameterName,
				message
			});
		}

		private static void RequireUpToDateObject(StoreId incomingId, StoreObject storeObject)
		{
			VersionedId versionedId = incomingId as VersionedId;
			if (versionedId == null)
			{
				throw new StaleObjectException();
			}
			VersionedId id = storeObject.Id;
			byte[] array = id.ChangeKeyAsByteArray();
			byte[] array2 = versionedId.ChangeKeyAsByteArray();
			if (array.Length != array2.Length)
			{
				throw new StaleObjectException();
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != array2[i])
				{
					throw new StaleObjectException();
				}
			}
		}

		private static Item OpenForUpdate(Item storeItem)
		{
			bool flag = false;
			try
			{
				storeItem.OpenAsReadWrite();
				flag = true;
			}
			finally
			{
				if (!flag && storeItem != null)
				{
					storeItem.Dispose();
				}
			}
			return storeItem;
		}

		protected virtual void SaveXsoFolder(Folder xsoFolder)
		{
			this.ExecuteFolderSave(xsoFolder);
			xsoFolder.Load(null);
		}

		protected virtual void LogDelegateSession(string principal)
		{
		}

		private void ExecuteFolderSave(Folder folder)
		{
			FolderSaveResult folderSaveResult = null;
			this.ExecuteStoreObjectSave(delegate
			{
				folderSaveResult = folder.Save();
			}, false);
			this.ValidateFolderSaveResult(folderSaveResult);
		}

		private void ValidateFolderSaveResult(FolderSaveResult folderSaveResult)
		{
			if (folderSaveResult.OperationResult == OperationResult.Succeeded)
			{
				return;
			}
			string arg = (folderSaveResult.OperationResult == OperationResult.Failed) ? "FAILED" : "PARTIALLY SUCCESSFUL";
			if (folderSaveResult.PropertyErrors == null)
			{
				ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<string>((long)this.GetHashCode(), "ServiceCommandBase.SaveXsoFolder Folder.Save() had a '{0}' operation result with <NULL> property errors.", arg);
			}
			else
			{
				ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<string, int>((long)this.GetHashCode(), "ServiceCommandBase.SaveXsoFolder Folder.Save() had a '{0}' operation result with '{1}' property errors.", arg, folderSaveResult.PropertyErrors.Length);
			}
			if (folderSaveResult.PropertyErrors != null)
			{
				foreach (PropertyError propertyError in folderSaveResult.PropertyErrors)
				{
					ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug((long)this.GetHashCode(), "PropertyError: Class:{0}, PropDefName: {1}, PropErrorCode: {2}, ErrorDescription: {3}", new object[]
					{
						propertyError.GetType(),
						(propertyError.PropertyDefinition == null) ? "NULL" : propertyError.PropertyDefinition.Name,
						propertyError.PropertyErrorCode,
						propertyError.PropertyErrorDescription
					});
				}
				throw new ObjectSavePropertyErrorException(SearchSchemaMap.GetPropertyPaths(folderSaveResult.PropertyErrors), PropertyError.ToException(folderSaveResult.PropertyErrors), false);
			}
			throw new FolderSaveException();
		}

		protected void ExecuteStoreObjectSave(ServiceCommandBase.SaveStoreObject saveStoreObject, bool useItemError)
		{
			try
			{
				saveStoreObject();
			}
			catch (DumpsterOperationException innerException)
			{
				throw new ObjectSaveException(innerException, useItemError);
			}
			catch (PropertyErrorException ex)
			{
				throw new ObjectSavePropertyErrorException(SearchSchemaMap.GetPropertyPaths(ex.PropertyErrors), ex, useItemError);
			}
			catch (PropertyValidationException ex2)
			{
				throw new ObjectSavePropertyErrorException(SearchSchemaMap.GetPropertyPaths(ex2.PropertyValidationErrors), ex2, useItemError);
			}
			catch (ObjectValidationException ex3)
			{
				throw new ObjectSavePropertyErrorException(SearchSchemaMap.GetPropertyPaths(ex3.Errors), ex3, useItemError);
			}
			catch (StoragePermanentException ex4)
			{
				bool flag = ex4.InnerException != null;
				ExTraceGlobals.ExceptionTracer.TraceError<StoragePermanentException>((long)this.GetHashCode(), "[ServiceCommandBase::ExecuteStoreObjectSave] encountered StoragePermanentException: {0}", ex4);
				if (!flag)
				{
					throw;
				}
				if (ex4.InnerException is MapiExceptionCrossPostDenied)
				{
					throw new ObjectSaveException(ex4, useItemError);
				}
				if (ex4.InnerException is MapiExceptionInvalidParameter)
				{
					throw new ObjectSaveException(ex4, useItemError);
				}
				if (ex4.InnerException is MapiExceptionJetErrorColumnTooBig)
				{
					throw new ObjectSaveException(ex4, useItemError);
				}
				if (ex4.InnerException is MapiExceptionJetWarningColumnMaxTruncated)
				{
					throw new ObjectSaveException(ex4, useItemError);
				}
				if (ex4.InnerException is MapiExceptionUnexpectedType)
				{
					throw new ObjectCorruptException(ex4, useItemError);
				}
				if (ex4.InnerException is MapiExceptionFailCallback)
				{
					throw new ObjectSaveException(ex4, useItemError);
				}
				throw;
			}
		}

		protected ConflictResolutionResult ExecuteItemSave(ServiceCommandBase.SaveItem saveItem, ConflictResolutionType conflictResolutionType)
		{
			ConflictResolutionResult conflictResolutionResult = null;
			SaveMode saveMode = this.GetSaveMode(conflictResolutionType);
			this.ExecuteStoreObjectSave(delegate
			{
				conflictResolutionResult = saveItem(saveMode);
			}, true);
			this.ValidateConflictResolutionResult(conflictResolutionResult, saveMode);
			return conflictResolutionResult;
		}

		protected void ValidateConflictResolutionResult(ConflictResolutionResult conflictResolutionResult, SaveMode saveMode)
		{
			if (saveMode != SaveMode.NoConflictResolution)
			{
				if (conflictResolutionResult.PropertyConflicts != null)
				{
					ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<int>((long)this.GetHashCode(), "ServiceCommandBase.SaveXsoItem item.Save returned '{0}' property conflicts.", conflictResolutionResult.PropertyConflicts.Length);
					foreach (PropertyConflict propertyConflict in conflictResolutionResult.PropertyConflicts)
					{
						ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug((long)this.GetHashCode(), "Property conflict: DisplayName: '{0}', Resolvable: '{1}', OriginalValue: '{2}', ClientValue: '{3}', ServerValue: '{4}', ResolvedValue: '{5}'", new object[]
						{
							(propertyConflict.PropertyDefinition != null) ? propertyConflict.PropertyDefinition.Name : ServiceDiagnostics.HandleNullObjectTrace(propertyConflict.PropertyDefinition),
							propertyConflict.ConflictResolvable,
							ServiceDiagnostics.HandleNullObjectTrace(propertyConflict.OriginalValue),
							ServiceDiagnostics.HandleNullObjectTrace(propertyConflict.ClientValue),
							ServiceDiagnostics.HandleNullObjectTrace(propertyConflict.ServerValue),
							ServiceDiagnostics.HandleNullObjectTrace(propertyConflict.ResolvedValue)
						});
					}
				}
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new IrresolvableConflictException(conflictResolutionResult.PropertyConflicts);
				}
				SaveResult saveStatus = conflictResolutionResult.SaveStatus;
			}
		}

		protected ConflictResolutionResult SaveXsoItem(Item xsoItem, ConflictResolutionType conflictResolutionType)
		{
			return this.SaveXsoItem(xsoItem, (SaveMode saveModeDelegate) => xsoItem.Save(saveModeDelegate), conflictResolutionType, null);
		}

		protected ConflictResolutionResult SaveXsoItem(Item xsoItem, ServiceCommandBase.SaveItem saveItem, ConflictResolutionType conflictResolutionType, PropertyDefinition[] propsToLoad)
		{
			ConflictResolutionResult result;
			if (xsoItem.IsDirty)
			{
				result = this.ExecuteItemSave(saveItem, conflictResolutionType);
				xsoItem.Load(propsToLoad);
			}
			else
			{
				result = new ConflictResolutionResult(SaveResult.Success, null);
			}
			List<IPostSavePropertyCommand> list;
			if (EWSSettings.PostSavePropertyCommands.TryGetValue(xsoItem.StoreObjectId, out list))
			{
				foreach (IPostSavePropertyCommand postSavePropertyCommand in list)
				{
					postSavePropertyCommand.ExecutePostSaveOperation(xsoItem);
				}
			}
			return result;
		}

		protected SaveMode GetSaveMode(ConflictResolutionType conflictResolutionType)
		{
			SaveMode result = SaveMode.FailOnAnyConflict;
			switch (conflictResolutionType)
			{
			case ConflictResolutionType.NeverOverwrite:
				result = SaveMode.FailOnAnyConflict;
				break;
			case ConflictResolutionType.AutoResolve:
				result = SaveMode.ResolveConflicts;
				break;
			case ConflictResolutionType.AlwaysOverwrite:
				result = SaveMode.NoConflictResolutionForceSave;
				break;
			}
			return result;
		}

		protected void LoadServiceObject(ServiceObject serviceObject, StoreObject storeObject, IdAndSession idAndSession, ResponseShape responseShape)
		{
			ServiceCommandBase.LoadServiceObject(serviceObject, storeObject, idAndSession, responseShape, null);
		}

		internal static void LoadServiceObject(ServiceObject serviceObject, StoreObject storeObject, IdAndSession idAndSession, ResponseShape responseShape, ToServiceObjectPropertyList toServiceObjectPropertyList)
		{
			if (toServiceObjectPropertyList == null)
			{
				toServiceObjectPropertyList = XsoDataConverter.GetToServiceObjectPropertyList(storeObject, responseShape);
			}
			PropertyDefinition[] propertyDefinitions = toServiceObjectPropertyList.GetPropertyDefinitions();
			storeObject.Load(propertyDefinitions);
			toServiceObjectPropertyList.ConvertStoreObjectPropertiesToServiceObject(idAndSession, storeObject, serviceObject);
		}

		protected void RequireExchange14OrLater()
		{
			ExchangePrincipal accessingPrincipal = this.CallContext.AccessingPrincipal;
			if (accessingPrincipal == null)
			{
				ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug((long)this.GetHashCode(), "ServiceCommandBase.RequireExchange14OrLater: AccessingPrincipal is null, caller is allowed.");
				return;
			}
			ServerVersion serverVersion = new ServerVersion(accessingPrincipal.MailboxInfo.Location.ServerVersion);
			ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug<ServerVersion, ServerVersion>((long)this.GetHashCode(), "ServiceCommandBase.RequireExchange14OrLater: caller version is {0}, required server version is {1}", serverVersion, ServiceCommandBase.exchange14ServerVersion);
			if (serverVersion.Major < ServiceCommandBase.exchange14ServerVersion.Major)
			{
				ExTraceGlobals.GetMailTipsCallTracer.TraceError((long)this.GetHashCode(), "ServiceCommandBase.RequireExchange14OrLater: access not allowed.");
				throw new ServiceInvalidOperationException((CoreResources.IDs)3336001063U);
			}
			ExTraceGlobals.ServiceCommandBaseCallTracer.TraceDebug((long)this.GetHashCode(), "ServiceCommandBase.RequireExchange14OrLater: access allowed.");
		}

		protected void SafeSetProtocolLogMetadata(Enum key, object value)
		{
			if (this.CallContext != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.CallContext.ProtocolLog, key, value);
			}
		}

		protected void SafeAppendLogGenericInfo(string key, object value)
		{
			if (this.CallContext != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(this.CallContext.ProtocolLog, key, value);
			}
		}

		internal bool PreExecute()
		{
			bool success = false;
			ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
			{
				this.serviceRequestId = Trace.TraceCasStart(CasTraceEventType.Ews);
				ExternalCallContext externalCallContext = this.CallContext as ExternalCallContext;
				if (externalCallContext != null)
				{
					bool flag = true;
					if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
					{
						ExTraceGlobals.ExternalUserTracer.TraceError(0L, "External user calls are only supported on Exchange2010 version and later");
						flag = false;
					}
					if (!this.SupportsExternalUsers)
					{
						ExTraceGlobals.ExternalUserTracer.TraceError<ServiceCommandBase>(0L, "Service command {0} does not support external user calls", this);
						flag = false;
					}
					if (externalCallContext.Offer != this.ExpectedOffer)
					{
						ExTraceGlobals.ExternalUserTracer.TraceError<ServiceCommandBase, Offer>(0L, "Service command {0} expects offer {1}, but received other offer instead.", this, this.ExpectedOffer);
						flag = false;
					}
					if (!flag)
					{
						throw FaultExceptionUtilities.CreateFault(new ServiceAccessDeniedException(), FaultParty.Sender);
					}
				}
				success = this.InternalPreExecute();
			});
			this.PreExecuteSucceeded = success;
			return this.PreExecuteSucceeded;
		}

		internal virtual int StepCount
		{
			get
			{
				throw new NotImplementedException("ServiceCommandBase.StepCount");
			}
		}

		internal TaskExecuteResult ExecuteStep()
		{
			return this.ExecuteHelper(delegate
			{
				bool result;
				this.InternalExecuteStep(out result);
				return result;
			});
		}

		internal virtual bool InternalPreExecute()
		{
			return true;
		}

		internal virtual void InternalPostExecute()
		{
		}

		internal virtual void InternalExecuteStep(out bool isBatchStopResponse)
		{
			throw new NotImplementedException("ServiceCommandBase.InternalExecuteStep");
		}

		internal TaskExecuteResult CancelStep(LocalizedException exception)
		{
			return this.ExecuteHelper(delegate
			{
				bool result;
				this.InternalCancelStep(exception, out result);
				return result;
			});
		}

		internal virtual void InternalCancelStep(LocalizedException exception, out bool isBatchStopResponse)
		{
			throw new NotImplementedException("ServiceCommandBase.InternalCancelStep");
		}

		private TaskExecuteResult ExecuteHelper(Func<bool> action)
		{
			bool isBatchStopResponse = false;
			if (this.StepCount > 0)
			{
				ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
				{
					try
					{
						isBatchStopResponse = action();
					}
					finally
					{
						this.CurrentStep++;
					}
				});
			}
			if (this.CurrentStep < this.StepCount && !isBatchStopResponse)
			{
				return TaskExecuteResult.StepComplete;
			}
			return TaskExecuteResult.ProcessingComplete;
		}

		internal IExchangeWebMethodResponse PostExecute()
		{
			if (this.PreExecuteSucceeded)
			{
				this.InternalPostExecute();
			}
			IExchangeWebMethodResponse response = this.GetResponse();
			if (response != null)
			{
				this.UpdatePerformanceCounters(response);
				if (!this.IsDelayExecuted)
				{
					this.LogResponseCode(response);
				}
			}
			if (ETWTrace.ShouldTraceCasStop(this.serviceRequestId))
			{
				Global.TraceCasStop(base.GetType(), this.CallContext, this.serviceRequestId);
			}
			return response;
		}

		internal virtual void UpdatePerformanceCounters(IExchangeWebMethodResponse response)
		{
			PerformanceMonitor.UpdateResponseCounters(response, this.ObjectsChanged);
		}

		internal void LogResponseCode(IExchangeWebMethodResponse response)
		{
			ResponseCodeType errorCodeToLog = response.GetErrorCodeToLog();
			if (errorCodeToLog != ResponseCodeType.NoError)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.CallContext.ProtocolLog, ServiceCommonMetadata.ErrorCode, errorCodeToLog);
			}
		}

		internal abstract IExchangeWebMethodResponse GetResponse();

		internal abstract ResourceKey[] GetResources();

		private protected bool IsRequestTracingEnabled { protected get; private set; }

		private protected XmlDocument XmlDocument { protected get; private set; }

		protected void InternalInitialize()
		{
			this.XmlDocument = new SafeXmlDocument();
			this.IsRequestTracingEnabled = this.CallContext.IsRequestTracingEnabled;
		}

		protected virtual void LogTracesForCurrentRequest()
		{
		}

		protected void LogRequestTraces()
		{
			if (this.IsRequestTracingEnabled)
			{
				this.LogTracesForCurrentRequest();
			}
		}

		protected void SetProperties(StoreObject storeObject, XmlNode serviceItem)
		{
			if (serviceItem.ChildNodes.Count > 0)
			{
				string className = storeObject.ClassName;
				XsoDataConverter.SetProperties(storeObject, (XmlElement)serviceItem, this.IdConverter);
				ServiceCommandBase.ValidateClassChange(storeObject, className);
			}
		}

		protected XmlElement CreateServiceItemXml(StoreObject storeObject, IdAndSession idAndSession, ResponseShape responseShape)
		{
			return this.CreateServiceItemXml(storeObject, idAndSession, responseShape, null);
		}

		internal XmlElement CreateServiceItemXml(StoreObject storeObject, IdAndSession idAndSession, ResponseShape responseShape, ToXmlPropertyList toXmlPropertyList)
		{
			if (toXmlPropertyList == null)
			{
				toXmlPropertyList = XsoDataConverter.GetPropertyList(storeObject, responseShape);
			}
			PropertyDefinition[] propertyDefinitions = toXmlPropertyList.GetPropertyDefinitions();
			storeObject.Load(propertyDefinitions);
			return toXmlPropertyList.ConvertStoreObjectPropertiesToXml(idAndSession, storeObject, this.XmlDocument);
		}

		private Guid serviceRequestId;

		private IParticipantResolver participantResolver;

		protected int objectsChanged;

		protected static readonly ItemResponseShape DefaultItemResponseShape = new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, false, null);

		protected static readonly ItemResponseShape DefaultItemResponseShapeWithAttachments = new ItemResponseShape(ShapeEnum.IdOnly, BodyResponseType.Best, false, new PropertyPath[]
		{
			new PropertyUri(PropertyUriEnum.Attachments)
		});

		protected static readonly FolderResponseShape DefaultFolderResponseShape = new FolderResponseShape(ShapeEnum.IdOnly, null);

		private static readonly ServerVersion exchange14ServerVersion = new ServerVersion(14, 0, 0, 0);

		private MailboxSession mailboxIdentityMailboxSession;

		protected static readonly TraceToHeadersLoggerFactory TraceLoggerFactory = new TraceToHeadersLoggerFactory(VariantConfiguration.InvariantNoFlightingSnapshot.Diagnostics.TraceToHeadersLogger.Enabled);

		internal delegate void SaveStoreObject();

		internal delegate ConflictResolutionResult SaveItem(SaveMode saveMode);
	}
}
