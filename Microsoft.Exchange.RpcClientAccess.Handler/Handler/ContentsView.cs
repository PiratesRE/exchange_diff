using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ContentsView : FolderBasedView
	{
		internal ContentsView(Logon logon, ReferenceCount<CoreFolder> coreFolderReference, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle) : base(logon, coreFolderReference, tableFlags, View.Capabilities.All, ViewType.MessageView, notificationHandler, returnNotificationHandle, ModernCalendarItemFilteringHelper.GetDefaultContentsViewFilter(coreFolderReference.ReferencedObject, logon))
		{
		}

		protected override PropertyConverter PropertyConverter
		{
			get
			{
				return PropertyConverter.Message;
			}
		}

		internal override byte[] CreateBookmark()
		{
			return base.GetViewCache().CreateBookmark();
		}

		internal override void FreeBookmark(byte[] bookmark)
		{
			base.GetViewCache().FreeBookmark(bookmark);
		}

		internal override int SeekRowBookmark(byte[] bookmark, int rowCount, bool wantMoveCount, out bool soughtLess, out bool positionChanged)
		{
			return base.InternalSeekRowBookmark(bookmark, rowCount, wantMoveCount, out soughtLess, out positionChanged);
		}

		internal byte[] GetCollapseState(long rowId, uint rowInstanceNumber)
		{
			return base.GetViewCache().GetCollapseState(ServerIdConverter.MakeInstanceKey(this.ContainerFolderId.Value, rowId, (int)rowInstanceNumber));
		}

		internal byte[] SetCollapseState(byte[] collapseState)
		{
			return base.GetViewCache().SetCollapseState(collapseState);
		}

		protected override IQueryResult CreateQueryResult(NativeStorePropertyDefinition[] propertyDefinitions)
		{
			NativeStorePropertyDefinition[] array = propertyDefinitions;
			SortBy[] array2 = base.SortBys;
			bool flag = false;
			PrivateLogon privateLogon = base.LogonObject as PrivateLogon;
			if (privateLogon != null)
			{
				flag = base.ContainerStoreObjectId.Equals(privateLogon.MailboxSession.GetDefaultFolderId(DefaultFolderType.RecoverableItemsDeletions));
			}
			else
			{
				PublicLogon publicLogon = base.LogonObject as PublicLogon;
				if (publicLogon != null)
				{
					flag = PublicFolderCOWSession.IsDumpsterFolder(base.CoreFolder);
				}
			}
			if (flag && (byte)(base.TableFlags & (TableFlags.Associated | TableFlags.SoftDeletes)) == 0)
			{
				for (int i = 0; i < propertyDefinitions.Length; i++)
				{
					if (propertyDefinitions[i] == CoreObjectSchema.DeletedOnTime)
					{
						if (array == propertyDefinitions)
						{
							array = (NativeStorePropertyDefinition[])propertyDefinitions.Clone();
						}
						array[i] = (NativeStorePropertyDefinition)CoreObjectSchema.LastModifiedTime;
					}
				}
				if (base.SortBys != null)
				{
					for (int j = 0; j < array2.Length; j++)
					{
						if (array2[j].ColumnDefinition == CoreObjectSchema.DeletedOnTime)
						{
							if (array2 == base.SortBys)
							{
								array2 = (SortBy[])base.SortBys.Clone();
							}
							array2[j] = new SortBy(CoreObjectSchema.LastModifiedTime, base.SortBys[j].SortOrder);
						}
					}
				}
			}
			ItemQueryType itemQueryType = ItemQueryType.None;
			if ((byte)(base.TableFlags & TableFlags.Associated) == 2)
			{
				itemQueryType |= ItemQueryType.Associated;
			}
			if ((byte)(base.TableFlags & TableFlags.SoftDeletes) == 32)
			{
				itemQueryType |= ItemQueryType.SoftDeleted;
			}
			byte b = (byte)(base.TableFlags & TableFlags.DeferredErrors);
			if ((byte)(base.TableFlags & TableFlags.SuppressNotifications) == 128)
			{
				itemQueryType |= ItemQueryType.ConversationViewMembers;
			}
			if ((byte)(base.TableFlags & TableFlags.Depth) == 4)
			{
				itemQueryType |= ItemQueryType.ConversationView;
			}
			if ((byte)(base.TableFlags & TableFlags.RetrieveFromIndex) == 1)
			{
				itemQueryType |= ItemQueryType.RetrieveFromIndex;
			}
			if (base.GroupBys != null)
			{
				return base.CoreFolder.QueryExecutor.GroupedItemQuery(base.Filter, itemQueryType, base.GroupBys, base.ExpandedCount, array2, array);
			}
			return base.CoreFolder.QueryExecutor.ItemQuery(itemQueryType, base.Filter, array2, array);
		}

		protected override void CheckPropertiesAllowed(PropertyTag[] propertyTags)
		{
			foreach (PropertyTag propertyTag in propertyTags)
			{
				if (Array.IndexOf<PropertyTag>(ViewClientProperties.ContentsViewClientProperties.DisallowList, propertyTag) >= 0)
				{
					throw new RopExecutionException(string.Format("This client side property is not supported on ContentsView. Property = {0}.", propertyTag), (ErrorCode)2147746050U);
				}
			}
		}

		protected override ClientSideProperties ClientSideProperties
		{
			get
			{
				return ClientSideProperties.ContentViewInstance;
			}
		}
	}
}
