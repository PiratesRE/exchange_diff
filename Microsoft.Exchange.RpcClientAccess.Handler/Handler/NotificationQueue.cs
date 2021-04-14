using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotificationQueue : BaseObject
	{
		public NotificationQueue(Logon logon)
		{
			if (logon == null)
			{
				throw new ArgumentNullException("logon");
			}
			this.logon = logon;
			this.registeredNotifications = NotificationQueue.CreateNotificationSinkList();
			this.notificationDataQueue = new NotificationDataQueue(128, () => this.TraceContext);
		}

		internal bool IsEmpty
		{
			get
			{
				return this.notificationDataQueue.IsEmpty;
			}
		}

		internal Logon Logon
		{
			get
			{
				return this.logon;
			}
		}

		private string TraceContext
		{
			get
			{
				if (this.traceContext == null)
				{
					string clientInfoString = this.Logon.Session.ClientInfoString;
					string actAsLegacyDN = this.Logon.Connection.ActAsLegacyDN;
					this.traceContext = string.Format("[{0}, {1}] ", actAsLegacyDN, clientInfoString);
				}
				return this.traceContext;
			}
		}

		internal NotificationSink Register(NotificationHandler notificationHandler, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, StoreId folderId, StoreId messageId, ServerObjectHandle returnNotificationHandleValue, Encoding string8Encoding)
		{
			NotificationSink result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				NotificationSink notificationSink = NotificationSink.CreateObjectNotificationSink(this, notificationHandler, returnNotificationHandleValue, flags, eventFlags, wantGlobalScope, folderId, messageId, string8Encoding);
				disposeGuard.Add<NotificationSink>(notificationSink);
				this.registeredNotifications.Add(notificationSink);
				this.TraceRegister("Object", flags, eventFlags, folderId, messageId, returnNotificationHandleValue);
				disposeGuard.Success();
				result = notificationSink;
			}
			return result;
		}

		internal NotificationSink Register(NotificationHandler notificationHandler, View view, TableFlags tableFlags, StoreId? folderId, ServerObjectHandle returnNotificationHandleValue, Encoding string8Encoding)
		{
			NotificationSink result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				NotificationSink notificationSink = NotificationSink.CreateQueryNotificationSink(notificationHandler, this, returnNotificationHandleValue, view, tableFlags, folderId, string8Encoding);
				disposeGuard.Add<NotificationSink>(notificationSink);
				this.registeredNotifications.Add(notificationSink);
				this.TraceRegister("Table", tableFlags, folderId, returnNotificationHandleValue);
				disposeGuard.Success();
				result = notificationSink;
			}
			return result;
		}

		internal bool UnRegister(NotificationSink removeNotificationSink)
		{
			return this.registeredNotifications.Remove(removeNotificationSink);
		}

		internal void Enqueue(ServerObjectHandle handle, Logon logon, Encoding string8Encoding, Notification notification, StoreId? rootFolderId, View view)
		{
			NotificationData data = new NotificationData(handle, logon, notification, rootFolderId, view, string8Encoding);
			this.notificationDataQueue.Enqueue(data);
		}

		internal bool Peek(out ServerObjectHandle notificationHandle, out byte logonId, out Encoding string8Encoding, out Notification notification)
		{
			notificationHandle = default(ServerObjectHandle);
			logonId = 0;
			notification = null;
			string8Encoding = Encoding.ASCII;
			if (this.IsEmpty)
			{
				return false;
			}
			for (;;)
			{
				NotificationData data;
				if (!this.notificationDataQueue.Peek(out data))
				{
					break;
				}
				notificationHandle = data.NotificationHandleValue;
				logonId = data.Logon.LogonId;
				Encoding tempEncoding = String8Encodings.TemporaryDefault;
				notification = RopHandlerHelper.CallHandler<Notification>(this, delegate()
				{
					tempEncoding = data.String8Encoding;
					return this.FromNotificationData(data);
				});
				string8Encoding = tempEncoding;
				if (notification != null)
				{
					goto Block_3;
				}
				NotificationData notificationData;
				if (!this.notificationDataQueue.Dequeue(out notificationData))
				{
					return false;
				}
				string arg = notificationData.ToString();
				ExTraceGlobals.NotificationDeliveryTracer.TraceError<string, string>(Activity.TraceId, "{0}: Notification data is corrupted. We need to fetch XSO fields to understand the issue. Notification = {1}.", this.TraceContext, arg);
			}
			return false;
			Block_3:
			this.TraceNotification("Peek", notification, notificationHandle);
			return true;
		}

		internal bool Dequeue()
		{
			NotificationData notificationData;
			return this.notificationDataQueue.Dequeue(out notificationData);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationQueue>(this);
		}

		protected override void InternalDispose()
		{
			int count = this.registeredNotifications.Count;
			for (int i = count; i > 0; i--)
			{
				Util.DisposeIfPresent(this.registeredNotifications[0]);
			}
			base.InternalDispose();
		}

		private static Notification.TableModifiedNotificationType FromXsoQueryNotificationType(QueryNotificationType type)
		{
			switch (type)
			{
			case QueryNotificationType.QueryResultChanged:
				return Notification.TableModifiedNotificationType.TableChanged;
			case QueryNotificationType.Error:
				return Notification.TableModifiedNotificationType.TableError;
			case QueryNotificationType.RowAdded:
				return Notification.TableModifiedNotificationType.TableRowAdded;
			case QueryNotificationType.RowDeleted:
				return Notification.TableModifiedNotificationType.TableRowDeleted;
			case QueryNotificationType.RowModified:
				return Notification.TableModifiedNotificationType.TableRowModified;
			case QueryNotificationType.SortDone:
				return Notification.TableModifiedNotificationType.TableSortDone;
			case QueryNotificationType.RestrictDone:
				return Notification.TableModifiedNotificationType.TableRestrictDone;
			case QueryNotificationType.SetColumnDone:
				return Notification.TableModifiedNotificationType.TableSetColumnDone;
			case QueryNotificationType.Reload:
				return Notification.TableModifiedNotificationType.TableReload;
			default:
				throw new RopExecutionException(string.Format("Unexpected query notification type. QueryNotificationType = {0}", type), (ErrorCode)2147746075U);
			}
		}

		private static IList<NotificationSink> CreateNotificationSinkList()
		{
			return new List<NotificationSink>(32);
		}

		private void GetIdsFromXsoIds(ObjectNotification notification, out StoreId? folderId, out StoreId? objectId, out StoreId? previousFolderId, out StoreId? previousObjectId)
		{
			folderId = null;
			previousFolderId = null;
			previousObjectId = null;
			IdConverter idConverter = this.Logon.Session.IdConverter;
			if (notification.ParentFolderId != null)
			{
				folderId = new StoreId?(new StoreId(idConverter.GetFidFromId(notification.ParentFolderId)));
			}
			if (notification.PreviousParentFolderId != null)
			{
				previousFolderId = new StoreId?(new StoreId(idConverter.GetFidFromId(notification.PreviousParentFolderId)));
			}
			NotificationObjectType objectType = notification.ObjectType;
			if (objectType != (NotificationObjectType)0)
			{
				switch (objectType)
				{
				case NotificationObjectType.Folder:
					objectId = new StoreId?(new StoreId(idConverter.GetFidFromId(notification.NotifyingItemId)));
					if (notification.PreviousId != null)
					{
						previousObjectId = new StoreId?(new StoreId(idConverter.GetFidFromId(notification.PreviousId)));
						return;
					}
					return;
				case NotificationObjectType.Message:
					objectId = new StoreId?(new StoreId(idConverter.GetMidFromMessageId(notification.NotifyingItemId)));
					folderId = new StoreId?(new StoreId(idConverter.GetFidFromId(notification.NotifyingItemId)));
					if (notification.PreviousId != null)
					{
						previousObjectId = new StoreId?(new StoreId(idConverter.GetMidFromMessageId(notification.PreviousId)));
						return;
					}
					return;
				case NotificationObjectType.Attachment:
					objectId = null;
					Feature.Stubbed(88730, string.Format("NotificationObjectType.Attachment is not supported.", new object[0]));
					return;
				}
				throw new NotSupportedException();
			}
			if (notification.Type != NotificationType.SearchComplete)
			{
				throw new NotSupportedException(string.Format("Unknown notification object type. {0}.", notification.ObjectType));
			}
			objectId = null;
		}

		private void GetIdsFromXsoIds(QueryNotification notification, out StoreId? folderId, out StoreId? objectId, out StoreId? previousFolderId, out StoreId? previousObjectId, out int instance, out int previousInstance)
		{
			instance = 0;
			previousInstance = 0;
			if (notification.Index.Length > 0)
			{
				long nativeId;
				long nativeId2;
				ServerIdConverter.CrackInstanceKey(notification.Index, out nativeId, out nativeId2, out instance);
				folderId = new StoreId?(new StoreId(nativeId));
				objectId = new StoreId?(new StoreId(nativeId2));
			}
			else
			{
				folderId = null;
				objectId = null;
			}
			if (notification.Prior.Length > 0)
			{
				long nativeId;
				long nativeId2;
				ServerIdConverter.CrackInstanceKey(notification.Prior, out nativeId, out nativeId2, out previousInstance);
				previousFolderId = new StoreId?(new StoreId(nativeId));
				previousObjectId = new StoreId?(new StoreId(nativeId2));
				return;
			}
			previousFolderId = null;
			previousObjectId = null;
		}

		private Notification FromNotificationData(NotificationData data)
		{
			TestInterceptor.Intercept(TestInterceptorLocation.NotificationQueue_FromNotificationData, new object[]
			{
				this.logon.HasActiveAsyncOperation
			});
			Notification notification = data.Notification;
			NotificationType type = data.Notification.Type;
			if (type == NotificationType.NewMail)
			{
				return this.CreateNewMailNotification((NewMailNotification)notification, data.NotificationHandleValue);
			}
			if (type != NotificationType.SearchComplete)
			{
				if (type == NotificationType.Query)
				{
					return this.CreateTableNotification((QueryNotification)notification, data.NotificationHandleValue, data.RootFolderId, data.View);
				}
				return this.CreateObjectNotification((ObjectNotification)notification, data.NotificationHandleValue);
			}
			else
			{
				if (data.RootFolderId != null)
				{
					return new Notification.SearchCompleteNotification(data.RootFolderId.Value);
				}
				throw new RopExecutionException("The RootFolder cannot be null for a SearchComplete notification.", (ErrorCode)2147746075U);
			}
		}

		private Notification CreateNewMailNotification(NewMailNotification newMailNotification, ServerObjectHandle returnNotificationHandleValue)
		{
			return new Notification.NewMailNotification(new StoreId(this.Logon.Session.IdConverter.GetFidFromId(newMailNotification.ParentFolderId)), new StoreId(this.Logon.Session.IdConverter.GetMidFromMessageId(newMailNotification.NewMailItemId)), (uint)newMailNotification.MessageFlags, newMailNotification.MessageClass);
		}

		private Notification CreateObjectNotification(ObjectNotification xsoNotification, ServerObjectHandle returnNotificationHandleValue)
		{
			StoreId? storeId = null;
			StoreId? storeId2 = null;
			StoreId? storeId3 = null;
			StoreId? storeId4 = null;
			if (xsoNotification != null)
			{
				this.GetIdsFromXsoIds(xsoNotification, out storeId, out storeId2, out storeId3, out storeId4);
			}
			if (storeId == null)
			{
				storeId = new StoreId?(default(StoreId));
			}
			if (storeId3 == null)
			{
				storeId3 = new StoreId?(default(StoreId));
			}
			StoreId folderId = storeId.Value;
			StoreId? messageId = storeId2;
			StoreId oldFolderId = storeId3.Value;
			StoreId? oldMessageId = storeId4;
			PropertyConverter propertyConverter;
			if (xsoNotification.ObjectType == NotificationObjectType.Folder)
			{
				folderId = (storeId2 ?? default(StoreId));
				messageId = null;
				oldFolderId = (storeId4 ?? default(StoreId));
				oldMessageId = null;
				propertyConverter = PropertyConverter.Folder;
			}
			else
			{
				propertyConverter = PropertyConverter.Message;
			}
			NotificationType type = xsoNotification.Type;
			if (type <= NotificationType.Modified)
			{
				switch (type)
				{
				case NotificationType.Created:
					return new Notification.ObjectCreatedNotification(folderId, messageId, new StoreId(this.Logon.Session.IdConverter.GetFidFromId(xsoNotification.ParentFolderId)), MEDSPropertyTranslator.PropertyTagsFromUnresolvedPropertyDefinitions(xsoNotification.PropertyDefinitions, propertyConverter));
				case NotificationType.NewMail | NotificationType.Created:
					break;
				case NotificationType.Deleted:
					return new Notification.ObjectDeletedNotification(folderId, messageId, new StoreId(this.Logon.Session.IdConverter.GetFidFromId(xsoNotification.ParentFolderId)));
				default:
					if (type == NotificationType.Modified)
					{
						int totalItemsChanged = 0;
						int unreadItemsChanged = 0;
						if (storeId2 != null)
						{
							totalItemsChanged = -1;
							unreadItemsChanged = -1;
						}
						return new Notification.ObjectModifiedNotification(folderId, messageId, MEDSPropertyTranslator.PropertyTagsFromUnresolvedPropertyDefinitions(xsoNotification.PropertyDefinitions, propertyConverter), totalItemsChanged, unreadItemsChanged);
					}
					break;
				}
			}
			else
			{
				if (type == NotificationType.Moved)
				{
					return new Notification.ObjectMovedNotification(folderId, messageId, new StoreId(this.Logon.Session.IdConverter.GetFidFromId(xsoNotification.ParentFolderId)), oldFolderId, oldMessageId, new StoreId(this.Logon.Session.IdConverter.GetFidFromId(xsoNotification.PreviousParentFolderId)));
				}
				if (type == NotificationType.Copied)
				{
					return new Notification.ObjectCopiedNotification(folderId, messageId, new StoreId(this.Logon.Session.IdConverter.GetFidFromId(xsoNotification.ParentFolderId)), oldFolderId, oldMessageId, new StoreId(this.Logon.Session.IdConverter.GetFidFromId(xsoNotification.PreviousParentFolderId)));
				}
				if (type == NotificationType.ConnectionDropped)
				{
					throw new NotSupportedException("Client should not have asked for this notification.");
				}
			}
			throw new NotSupportedException(string.Format("Unknown notification type. {0}.", xsoNotification.Type));
		}

		private bool IsRowWithinUnreadCache(QueryNotification queryNotification, View view)
		{
			if (view != null)
			{
				switch (queryNotification.EventType)
				{
				case QueryNotificationType.RowAdded:
					return view.IsRowWithinUnreadCache(queryNotification.Prior, View.RowLookupPosition.Previous);
				case QueryNotificationType.RowDeleted:
				case QueryNotificationType.RowModified:
					return view.IsRowWithinUnreadCache(queryNotification.Index, View.RowLookupPosition.Current);
				}
			}
			return false;
		}

		private void TraceColumnMismatch(IEnumerable<PropertyTag> currentColumns, IEnumerable<PropertyTag> notificationColumns)
		{
			ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string>(Activity.TraceId, "{0}: {1}", this.TraceContext, "The view has been updated. This notification is dropped. Output the current columns (first) and the columns from the notifications (second).");
			this.TracePropertyTagArray(currentColumns);
			this.TracePropertyTagArray(notificationColumns);
		}

		private Notification CreateTableNotification(QueryNotification queryNotification, ServerObjectHandle returnNotificationHandleValue, StoreId? rootFolderId, View view)
		{
			if (!view.IsAvailable)
			{
				return null;
			}
			if (this.IsRowWithinUnreadCache(queryNotification, view))
			{
				return new Notification.TableModifiedNotification(Notification.TableModifiedNotificationType.TableReload);
			}
			switch (queryNotification.EventType)
			{
			case QueryNotificationType.QueryResultChanged:
			case QueryNotificationType.Error:
			case QueryNotificationType.RestrictDone:
			case QueryNotificationType.Reload:
				return new Notification.TableModifiedNotification(NotificationQueue.FromXsoQueryNotificationType(queryNotification.EventType));
			case QueryNotificationType.RowAdded:
			case QueryNotificationType.RowModified:
			{
				PropertyValue[] array;
				if (!this.TryGetServerPropertyValuesFromNotification(queryNotification, view.ServerColumns, out array))
				{
					return null;
				}
				PropertyValue[] propertyValues;
				if (!view.TryConvertOriginalRowFromServerRow(array, out propertyValues))
				{
					this.TraceColumnMismatch(view.Columns, from var in array
					select var.PropertyTag);
					return null;
				}
				StoreId? storeId;
				StoreId? storeId2;
				StoreId? storeId3;
				StoreId? storeId4;
				int instance;
				int insertAfterInstance;
				this.GetIdsFromXsoIds(queryNotification, out storeId, out storeId2, out storeId3, out storeId4, out instance, out insertAfterInstance);
				StoreId? storeId5 = storeId;
				long? num = (storeId5 != null) ? new long?(storeId5.GetValueOrDefault()) : null;
				StoreId? storeId6 = rootFolderId;
				bool isInSearchFolder = num != ((storeId6 != null) ? new long?(storeId6.GetValueOrDefault()) : null);
				return new Notification.TableModifiedNotification.TableRowAddModifiedNotification(NotificationQueue.FromXsoQueryNotificationType(queryNotification.EventType), storeId ?? default(StoreId), storeId2 ?? default(StoreId), (uint)instance, storeId3 ?? default(StoreId), storeId4 ?? default(StoreId), (uint)insertAfterInstance, view.Columns, propertyValues, isInSearchFolder);
			}
			case QueryNotificationType.RowDeleted:
			{
				long nativeId;
				long nativeId2;
				int instance2;
				ServerIdConverter.CrackInstanceKey(queryNotification.Index, out nativeId, out nativeId2, out instance2);
				StoreId storeId7 = new StoreId(nativeId);
				long num2 = storeId7;
				StoreId? storeId8 = rootFolderId;
				bool isInSearchFolder2 = num2 != ((storeId8 != null) ? new long?(storeId8.GetValueOrDefault()) : null);
				return new Notification.TableModifiedNotification.TableRowDeletedModifiedNotification(NotificationQueue.FromXsoQueryNotificationType(queryNotification.EventType), storeId7, new StoreId(nativeId2), (uint)instance2, isInSearchFolder2);
			}
			}
			throw new RopExecutionException(string.Format("The server data is not expected. EventType = {0}.", queryNotification.EventType), (ErrorCode)2147746075U);
		}

		private bool TryGetServerPropertyValuesFromNotification(QueryNotification queryNotification, PropertyTag[] viewServerColumns, out PropertyValue[] serverPropertyValues)
		{
			serverPropertyValues = null;
			ICollection<PropertyTag> collection = MEDSPropertyTranslator.PropertyTagsFromPropertyDefinitions<PropertyDefinition>(this.logon.Session, queryNotification.PropertyDefinitions, true);
			if (collection.Count != viewServerColumns.Length)
			{
				this.TraceColumnMismatch(viewServerColumns, collection);
				return false;
			}
			int num = 0;
			foreach (PropertyTag serverPropertyTag in collection)
			{
				PropertyTag normalizedPropertyTag = this.GetNormalizedPropertyTag(serverPropertyTag, viewServerColumns[num].IsMultiValueInstanceProperty);
				if (viewServerColumns[num].PropertyId != normalizedPropertyTag.PropertyId)
				{
					this.TraceColumnMismatch(viewServerColumns, collection);
					return false;
				}
				bool flag = viewServerColumns[num].IsMultiValuedProperty && !viewServerColumns[num].IsMultiValueInstanceProperty;
				if (flag != normalizedPropertyTag.IsMultiValuedProperty)
				{
					this.TraceColumnMismatch(viewServerColumns, collection);
					return false;
				}
				num++;
			}
			try
			{
				serverPropertyValues = MEDSPropertyTranslator.TranslatePropertyValues(this.logon.Session, viewServerColumns, queryNotification.Row, true);
			}
			catch (InvalidPropertyValueTypeException arg)
			{
				ExTraceGlobals.NotificationDeliveryTracer.TraceError<string, string, InvalidPropertyValueTypeException>(Activity.TraceId, "{0}: {1} - {2}", this.TraceContext, "An error occurred when translating property values for the notification", arg);
				return false;
			}
			return true;
		}

		private PropertyTag GetNormalizedPropertyTag(PropertyTag serverPropertyTag, bool isMultiValueInstance)
		{
			if (isMultiValueInstance)
			{
				return new PropertyTag(serverPropertyTag.PropertyId, serverPropertyTag.ElementPropertyType);
			}
			return serverPropertyTag;
		}

		private void TraceRegister(string method, NotificationFlags flags, NotificationEventFlags eventFlags, StoreId folderId, StoreId messageId, ServerObjectHandle returnNotificationHandleValue)
		{
			ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<Register_{1}>> [{2}, {3}] [{4}:{5}] [{6}]", new object[]
			{
				this.TraceContext,
				method,
				flags.ToString(),
				eventFlags.ToString(),
				folderId,
				messageId,
				returnNotificationHandleValue
			});
			this.TraceMessageOrFolderInfo(folderId, new StoreId?(messageId));
		}

		private void TraceRegister(string method, TableFlags tableFlags, StoreId? folderId, ServerObjectHandle returnNotificationHandleValue)
		{
			ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<{1}>> [{2}, {3}] [{4}]", new object[]
			{
				this.TraceContext,
				method,
				tableFlags.ToString(),
				folderId,
				returnNotificationHandleValue
			});
			if (folderId != null)
			{
				this.TraceFolderInfo(folderId.Value);
			}
		}

		private void TraceNotification(string method, Notification notification, ServerObjectHandle returnNotificationHandleValue)
		{
			if (!ExTraceGlobals.NotificationDeliveryTracer.IsTraceEnabled(TraceType.InfoTrace) && !ExTraceGlobals.NotificationDeliveryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			if (notification is Notification.ObjectNotification)
			{
				this.TraceObjectNotification(method, (Notification.ObjectNotification)notification, returnNotificationHandleValue);
				return;
			}
			if (notification is Notification.TableModifiedNotification)
			{
				this.TraceTableNotification(method, (Notification.TableModifiedNotification)notification, returnNotificationHandleValue);
			}
		}

		private void TraceObjectNotification(string method, Notification.ObjectNotification objNotification, ServerObjectHandle returnNotificationHandleValue)
		{
			if (objNotification is Notification.ObjectModifiedNotification)
			{
				Notification.ObjectModifiedNotification objectModifiedNotification = objNotification as Notification.ObjectModifiedNotification;
				ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<{1}>> [{2}] [{3}]", new object[]
				{
					this.TraceContext,
					method,
					Notification.NotificationModifiers.ObjectModified,
					returnNotificationHandleValue
				});
				this.TraceMessageOrFolderInfo(objectModifiedNotification.FolderId, objectModifiedNotification.MessageId);
				this.TracePropertyTagArray(objectModifiedNotification.PropertyTags);
				return;
			}
			if (objNotification is Notification.ObjectCopiedNotification)
			{
				Notification.ObjectCopiedNotification objectCopiedNotification = objNotification as Notification.ObjectCopiedNotification;
				ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<{1}>> [{2}] [{3}]", new object[]
				{
					this.TraceContext,
					method,
					Notification.NotificationModifiers.ObjectCopied,
					returnNotificationHandleValue
				});
				this.TraceMessageOrFolderInfo(objectCopiedNotification.FolderId, objectCopiedNotification.MessageId);
				this.TraceFolderInfo(objectCopiedNotification.OldFolderId);
				this.TraceFolderInfo(objectCopiedNotification.FolderId);
				return;
			}
			if (objNotification is Notification.ObjectCreatedNotification)
			{
				Notification.ObjectCreatedNotification objectCreatedNotification = objNotification as Notification.ObjectCreatedNotification;
				ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<{1}>> [{2}] [{3}]", new object[]
				{
					this.TraceContext,
					method,
					Notification.NotificationModifiers.ObjectCreated,
					returnNotificationHandleValue
				});
				this.TraceMessageOrFolderInfo(objectCreatedNotification.FolderId, objectCreatedNotification.MessageId);
				return;
			}
			if (objNotification is Notification.ObjectDeletedNotification)
			{
				Notification.ObjectDeletedNotification objectDeletedNotification = objNotification as Notification.ObjectDeletedNotification;
				ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<{1}>> [{2}] [{3}]", new object[]
				{
					this.TraceContext,
					method,
					Notification.NotificationModifiers.ObjectDeleted,
					returnNotificationHandleValue
				});
				this.TraceMessageOrFolderInfo(objectDeletedNotification.FolderId, objectDeletedNotification.MessageId);
				return;
			}
			if (objNotification is Notification.ObjectMovedNotification)
			{
				Notification.ObjectMovedNotification objectMovedNotification = objNotification as Notification.ObjectMovedNotification;
				ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<{1}>> [{2}] [{3}]", new object[]
				{
					this.TraceContext,
					method,
					Notification.NotificationModifiers.ObjectMoved,
					returnNotificationHandleValue
				});
				this.TraceMessageOrFolderInfo(objectMovedNotification.FolderId, objectMovedNotification.MessageId);
				this.TraceFolderInfo(objectMovedNotification.OldFolderId);
				this.TraceFolderInfo(objectMovedNotification.FolderId);
				return;
			}
			if (objNotification is Notification.NewMailNotification)
			{
				Notification.NewMailNotification newMailNotification = objNotification as Notification.NewMailNotification;
				ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<{1}>> [{2}] [{3}] [{4}]", new object[]
				{
					this.TraceContext,
					method,
					Notification.NotificationModifiers.NewMail,
					newMailNotification.MessageClass,
					returnNotificationHandleValue
				});
				this.TraceMessageOrFolderInfo(newMailNotification.FolderId, newMailNotification.MessageId);
				return;
			}
			if (objNotification is Notification.SearchCompleteNotification)
			{
				Notification.SearchCompleteNotification searchCompleteNotification = objNotification as Notification.SearchCompleteNotification;
				ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<{1}>> [{2}] [{3}]", new object[]
				{
					this.TraceContext,
					method,
					Notification.NotificationModifiers.SearchComplete,
					returnNotificationHandleValue
				});
				this.TraceMessageOrFolderInfo(searchCompleteNotification.FolderId, searchCompleteNotification.MessageId);
			}
		}

		private void TraceTableNotification(string method, Notification.TableModifiedNotification tableNotification, ServerObjectHandle returnNotificationHandleValue)
		{
			ExTraceGlobals.NotificationDeliveryTracer.Information(Activity.TraceId, "{0}: <<{1}>> [{2}] [{3}]", new object[]
			{
				this.TraceContext,
				method,
				tableNotification.NotificationType,
				returnNotificationHandleValue
			});
			if (tableNotification is Notification.TableModifiedNotification.TableRowAddModifiedNotification)
			{
				Notification.TableModifiedNotification.TableRowAddModifiedNotification tableRowAddModifiedNotification = tableNotification as Notification.TableModifiedNotification.TableRowAddModifiedNotification;
				ExTraceGlobals.NotificationDeliveryTracer.Information<string, Notification.TableModifiedNotificationType, bool>(Activity.TraceId, "{0}: [{1}, {2}]", this.traceContext, tableRowAddModifiedNotification.NotificationType, tableRowAddModifiedNotification.IsInSearchFolder);
				this.TracePropertyRow(tableRowAddModifiedNotification.PropertyRow);
				return;
			}
			if (tableNotification is Notification.TableModifiedNotification.TableRowDeletedModifiedNotification)
			{
				Notification.TableModifiedNotification.TableRowDeletedModifiedNotification tableRowDeletedModifiedNotification = tableNotification as Notification.TableModifiedNotification.TableRowDeletedModifiedNotification;
				ExTraceGlobals.NotificationDeliveryTracer.Information<string, Notification.TableModifiedNotificationType, bool>(Activity.TraceId, "{0}: [{1}, {2}]", this.traceContext, tableRowDeletedModifiedNotification.NotificationType, tableRowDeletedModifiedNotification.IsInSearchFolder);
				this.TraceMessageOrFolderInfo(tableRowDeletedModifiedNotification.FolderId, tableRowDeletedModifiedNotification.MessageId);
			}
		}

		private void TraceMessageOrFolderInfo(StoreId folderId, StoreId? messageId)
		{
			if (!ExTraceGlobals.NotificationDeliveryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			if (messageId == null || messageId.Value.Equals(default(StoreId)))
			{
				this.TraceFolderInfo(folderId);
				return;
			}
			try
			{
				using (CoreItem coreItem = CoreItem.Bind(this.Logon.Session, this.Logon.Session.IdConverter.CreateMessageId(folderId, messageId.Value)))
				{
					string arg = coreItem.PropertyBag.TryGetProperty(CoreItemSchema.Subject) as string;
					string arg2 = coreItem.PropertyBag.TryGetProperty(CoreItemSchema.ItemClass) as string;
					ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string, string>(Activity.TraceId, "{0}: Item [{1}]: {2}", this.TraceContext, arg2, arg);
				}
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string>(Activity.TraceId, "{0}: {1}", this.TraceContext, "ItemId is not valid.");
			}
			catch (StoragePermanentException)
			{
				ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string>(Activity.TraceId, "{0}: {1}", this.TraceContext, "Item is not available.");
			}
		}

		private void TraceFolderInfo(StoreId folderId)
		{
			if (!ExTraceGlobals.NotificationDeliveryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			if (folderId.Equals(default(StoreId)))
			{
				ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string>(Activity.TraceId, "{0}: {1}", this.TraceContext, "FolderId is zero.");
				return;
			}
			try
			{
				using (CoreFolder coreFolder = CoreFolder.Bind(this.Logon.Session, this.Logon.Session.IdConverter.CreateFolderId(folderId)))
				{
					string arg = coreFolder.PropertyBag.TryGetProperty(CoreFolderSchema.DisplayName) as string;
					ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string>(Activity.TraceId, "{0}: Folder \"{1}\"", this.TraceContext, arg);
				}
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string>(Activity.TraceId, "{0}: {1}", this.TraceContext, "ItemId is not valid.");
			}
			catch (StoragePermanentException)
			{
				ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string>(Activity.TraceId, "{0}: {1}", this.TraceContext, "Folder is not available");
			}
		}

		private void TracePropertyTagArray(IEnumerable<PropertyTag> propertyTags)
		{
			if (!ExTraceGlobals.NotificationDeliveryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			string arg;
			if (propertyTags != null)
			{
				StringBuilder stringBuilder = new StringBuilder(1000);
				foreach (PropertyTag propertyTag in propertyTags)
				{
					stringBuilder.AppendFormat("[{0:X} : {1}] ", propertyTag.PropertyId, propertyTag.PropertyType);
				}
				arg = stringBuilder.ToString();
			}
			else
			{
				arg = "<Null propertyTags>";
			}
			ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string>(Activity.TraceId, "{0}: {1}", this.TraceContext, arg);
		}

		private void TracePropertyRow(PropertyRow propertyRow)
		{
			if (!ExTraceGlobals.NotificationDeliveryTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PropertyValue propertyValue in propertyRow.PropertyValues)
			{
				stringBuilder.Append(propertyValue);
			}
			ExTraceGlobals.NotificationDeliveryTracer.TraceDebug<string, string>(Activity.TraceId, "{0}: {1}", this.TraceContext, stringBuilder.ToString());
		}

		private const int MaxNotificationsPerQueue = 128;

		private readonly Logon logon;

		private readonly IList<NotificationSink> registeredNotifications;

		private readonly NotificationDataQueue notificationDataQueue;

		private string traceContext;
	}
}
