using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	public class NotificationHandler : INotificationHandler
	{
		internal NotificationHandler(MapiSession mapiSession)
		{
			this.mapiSession = mapiSession;
		}

		bool INotificationHandler.HasPendingNotifications()
		{
			foreach (MapiLogon mapiLogon in this.mapiSession.Logons)
			{
				if (mapiLogon.PendingNotificationsCount != 0)
				{
					return true;
				}
			}
			return false;
		}

		void INotificationHandler.CollectNotifications(NotificationCollector collector)
		{
			foreach (MapiLogon mapiLogon in this.mapiSession.Logons)
			{
				while (mapiLogon.PendingNotificationsCount != 0)
				{
					uint num;
					NotificationEvent notificationEvent = mapiLogon.PendingNotifications.Peek(out num);
					ServerObjectHandle notificationHandle = new ServerObjectHandle(num);
					Notification notification = NotificationHandler.CreateRCANotificationObject(notificationEvent, mapiLogon.Session.ClientVersion);
					if (!collector.TryAddNotification(notificationHandle, (byte)mapiLogon.Index, String8Encodings.TemporaryDefault, notification))
					{
						return;
					}
					if (ExTraceGlobals.RpcOperationTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("MARK OP [Mapi][Notify]");
						ExTraceGlobals.RpcOperationTracer.TraceFunction(0L, stringBuilder.ToString());
					}
					if (ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder2 = new StringBuilder(100);
						stringBuilder2.Append("OUTPUT Rop.Notify: hsot:[");
						stringBuilder2.Append(num);
						stringBuilder2.Append("] ");
						notificationEvent.AppendToString(stringBuilder2);
						ExTraceGlobals.RpcDetailTracer.TraceDebug(0L, stringBuilder2.ToString());
					}
					if (ExTraceGlobals.RpcOperationTracer.IsTraceEnabled(TraceType.FunctionTrace))
					{
						StringBuilder stringBuilder3 = new StringBuilder(100);
						stringBuilder3.Append("MARK OP END [Mapi][Notify]");
						ExTraceGlobals.RpcOperationTracer.TraceFunction(0L, stringBuilder3.ToString());
					}
					mapiLogon.PendingNotifications.Dequeue(out num);
				}
			}
		}

		void INotificationHandler.RegisterCallback(Action callback)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
		}

		void INotificationHandler.CancelCallback()
		{
		}

		private static Notification CreateRCANotificationObject(NotificationEvent notificationEvent, Microsoft.Exchange.Protocols.MAPI.Version clientVersion)
		{
			ObjectNotificationEvent objectNotificationEvent = notificationEvent as ObjectNotificationEvent;
			TableModifiedNotificationEvent tableModifiedNotificationEvent = notificationEvent as TableModifiedNotificationEvent;
			if (objectNotificationEvent != null)
			{
				if ((objectNotificationEvent.EventType & EventType.ObjectMoved) != (EventType)0)
				{
					ObjectMovedCopiedNotificationEvent objectMovedCopiedNotificationEvent = objectNotificationEvent as ObjectMovedCopiedNotificationEvent;
					return new Notification.ObjectMovedNotification(RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent.Fid), (!objectMovedCopiedNotificationEvent.Mid.IsNull) ? new StoreId?(RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent.Mid)) : null, (!objectMovedCopiedNotificationEvent.ParentFid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent.ParentFid) : default(StoreId), RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent.OldFid), (!objectMovedCopiedNotificationEvent.OldMid.IsNull) ? new StoreId?(RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent.OldMid)) : null, (!objectMovedCopiedNotificationEvent.OldParentFid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent.OldParentFid) : default(StoreId));
				}
				if ((objectNotificationEvent.EventType & EventType.ObjectCopied) != (EventType)0)
				{
					ObjectMovedCopiedNotificationEvent objectMovedCopiedNotificationEvent2 = objectNotificationEvent as ObjectMovedCopiedNotificationEvent;
					return new Notification.ObjectCopiedNotification(RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent2.Fid), (!objectMovedCopiedNotificationEvent2.Mid.IsNull) ? new StoreId?(RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent2.Mid)) : null, (!objectMovedCopiedNotificationEvent2.ParentFid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent2.ParentFid) : default(StoreId), RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent2.OldFid), (!objectMovedCopiedNotificationEvent2.OldMid.IsNull) ? new StoreId?(RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent2.OldMid)) : null, (!objectMovedCopiedNotificationEvent2.OldParentFid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(objectMovedCopiedNotificationEvent2.OldParentFid) : default(StoreId));
				}
				if ((objectNotificationEvent.EventType & EventType.ObjectDeleted) != (EventType)0)
				{
					return new Notification.ObjectDeletedNotification(RcaTypeHelpers.ExchangeIdToStoreId(objectNotificationEvent.Fid), (!objectNotificationEvent.Mid.IsNull) ? new StoreId?(RcaTypeHelpers.ExchangeIdToStoreId(objectNotificationEvent.Mid)) : null, (!objectNotificationEvent.ParentFid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(objectNotificationEvent.ParentFid) : default(StoreId));
				}
				if ((objectNotificationEvent.EventType & EventType.ObjectCreated) != (EventType)0)
				{
					ObjectCreatedModifiedNotificationEvent objectCreatedModifiedNotificationEvent = objectNotificationEvent as ObjectCreatedModifiedNotificationEvent;
					PropertyTag[] propertyTags = (objectCreatedModifiedNotificationEvent.ChangedPropTags != null) ? RcaTypeHelpers.PropertyTagsFromStorePropTags(objectCreatedModifiedNotificationEvent.ChangedPropTags).ToArray() : Array<PropertyTag>.Empty;
					return new Notification.ObjectCreatedNotification(RcaTypeHelpers.ExchangeIdToStoreId(objectNotificationEvent.Fid), (!objectNotificationEvent.Mid.IsNull) ? new StoreId?(RcaTypeHelpers.ExchangeIdToStoreId(objectNotificationEvent.Mid)) : null, (!objectNotificationEvent.ParentFid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(objectNotificationEvent.ParentFid) : default(StoreId), propertyTags);
				}
				if ((objectNotificationEvent.EventType & EventType.ObjectModified) != (EventType)0)
				{
					ObjectCreatedModifiedNotificationEvent objectCreatedModifiedNotificationEvent2 = objectNotificationEvent as ObjectCreatedModifiedNotificationEvent;
					PropertyTag[] propertyTags2 = (objectCreatedModifiedNotificationEvent2.ChangedPropTags != null) ? RcaTypeHelpers.PropertyTagsFromStorePropTags(objectCreatedModifiedNotificationEvent2.ChangedPropTags).ToArray() : Array<PropertyTag>.Empty;
					int totalItemsChanged = -1;
					int unreadItemsChanged = -1;
					FolderModifiedNotificationEvent folderModifiedNotificationEvent = objectNotificationEvent as FolderModifiedNotificationEvent;
					if (folderModifiedNotificationEvent != null)
					{
						totalItemsChanged = folderModifiedNotificationEvent.MessageCount;
						unreadItemsChanged = folderModifiedNotificationEvent.UnreadMessageCount;
					}
					return new Notification.ObjectModifiedNotification(RcaTypeHelpers.ExchangeIdToStoreId(objectNotificationEvent.Fid), (!objectNotificationEvent.Mid.IsNull) ? new StoreId?(RcaTypeHelpers.ExchangeIdToStoreId(objectNotificationEvent.Mid)) : null, propertyTags2, totalItemsChanged, unreadItemsChanged);
				}
				if ((objectNotificationEvent.EventType & EventType.NewMail) != (EventType)0)
				{
					NewMailNotificationEvent newMailNotificationEvent = objectNotificationEvent as NewMailNotificationEvent;
					return new Notification.NewMailNotification(RcaTypeHelpers.ExchangeIdToStoreId(newMailNotificationEvent.Fid), (!newMailNotificationEvent.Mid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(newMailNotificationEvent.Mid) : default(StoreId), (uint)newMailNotificationEvent.MessageFlags, newMailNotificationEvent.MessageClass);
				}
				if ((objectNotificationEvent.EventType & EventType.SearchComplete) != (EventType)0)
				{
					return new Notification.SearchCompleteNotification(RcaTypeHelpers.ExchangeIdToStoreId(objectNotificationEvent.Fid));
				}
				return null;
			}
			else
			{
				switch (tableModifiedNotificationEvent.TableEventType)
				{
				case TableEventType.Changed:
				case TableEventType.Error:
				case TableEventType.SortDone:
				case TableEventType.RestrictDone:
				case TableEventType.SetcolDone:
				case TableEventType.Reload:
					return new Notification.TableModifiedNotification((Notification.TableModifiedNotificationType)tableModifiedNotificationEvent.TableEventType);
				case TableEventType.RowAdded:
				case TableEventType.RowModified:
				{
					bool flag;
					return new Notification.TableModifiedNotification.TableRowAddModifiedNotification((Notification.TableModifiedNotificationType)tableModifiedNotificationEvent.TableEventType, RcaTypeHelpers.ExchangeIdToStoreId(tableModifiedNotificationEvent.Fid), (!tableModifiedNotificationEvent.Mid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(tableModifiedNotificationEvent.Mid) : default(StoreId), (uint)tableModifiedNotificationEvent.Inst, RcaTypeHelpers.ExchangeIdToStoreId(tableModifiedNotificationEvent.PreviousFid), (!tableModifiedNotificationEvent.PreviousMid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(tableModifiedNotificationEvent.PreviousMid) : default(StoreId), (uint)tableModifiedNotificationEvent.PreviousInst, RcaTypeHelpers.PropertyTagsFromStorePropTags(tableModifiedNotificationEvent.Row.GetPropTags()).ToArray(), RcaTypeHelpers.MassageOutgoingProperties(tableModifiedNotificationEvent.Row, out flag), (tableModifiedNotificationEvent.EventFlags & EventFlags.SearchFolder) != EventFlags.None);
				}
				case TableEventType.RowDeleted:
				{
					if (clientVersion < Microsoft.Exchange.Protocols.MAPI.Version.SupportsTableRowDeletedExtendedVersion || tableModifiedNotificationEvent.Row.Count == 0)
					{
						return new Notification.TableModifiedNotification.TableRowDeletedModifiedNotification((Notification.TableModifiedNotificationType)tableModifiedNotificationEvent.TableEventType, RcaTypeHelpers.ExchangeIdToStoreId(tableModifiedNotificationEvent.Fid), (!tableModifiedNotificationEvent.Mid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(tableModifiedNotificationEvent.Mid) : default(StoreId), (uint)tableModifiedNotificationEvent.Inst, (tableModifiedNotificationEvent.EventFlags & EventFlags.SearchFolder) != EventFlags.None);
					}
					bool flag;
					return new Notification.TableModifiedNotification.TableRowDeletedExtendedNotification(Notification.TableModifiedNotificationType.TableRowDeletedExtended, RcaTypeHelpers.ExchangeIdToStoreId(tableModifiedNotificationEvent.Fid), (!tableModifiedNotificationEvent.Mid.IsNull) ? RcaTypeHelpers.ExchangeIdToStoreId(tableModifiedNotificationEvent.Mid) : default(StoreId), (uint)tableModifiedNotificationEvent.Inst, RcaTypeHelpers.MassageOutgoingProperties(tableModifiedNotificationEvent.Row, out flag), (tableModifiedNotificationEvent.EventFlags & EventFlags.SearchFolder) != EventFlags.None);
				}
				default:
					return null;
				}
			}
		}

		private MapiSession mapiSession;
	}
}
