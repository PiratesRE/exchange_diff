using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class HierarchyView : FolderBasedView
	{
		internal HierarchyView(Logon logon, ReferenceCount<CoreFolder> coreFolderReference, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle) : base(logon, coreFolderReference, tableFlags, View.Capabilities.CanRestrict, ViewType.FolderView, notificationHandler, returnNotificationHandle, null)
		{
		}

		protected override PropertyConverter PropertyConverter
		{
			get
			{
				return PropertyConverter.HierarchyView;
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

		protected override IQueryResult CreateQueryResult(NativeStorePropertyDefinition[] propertyDefinitions)
		{
			FolderQueryFlags folderQueryFlags = FolderQueryFlags.None;
			if ((byte)(base.TableFlags & TableFlags.SoftDeletes) == 32)
			{
				folderQueryFlags |= FolderQueryFlags.SoftDeleted;
			}
			if ((byte)(base.TableFlags & TableFlags.Depth) == 4)
			{
				folderQueryFlags |= FolderQueryFlags.DeepTraversal;
			}
			if ((byte)(base.TableFlags & TableFlags.SuppressNotifications) == 128)
			{
				folderQueryFlags |= FolderQueryFlags.SuppressNotificationsOnMyActions;
			}
			byte b = (byte)(base.TableFlags & TableFlags.DeferredErrors);
			return base.CoreFolder.QueryExecutor.FolderQuery(folderQueryFlags, base.Filter, base.SortBys, propertyDefinitions);
		}

		protected override void CheckPropertiesAllowed(PropertyTag[] propertyTags)
		{
			foreach (PropertyTag propertyTag in propertyTags)
			{
				if (Array.IndexOf<PropertyTag>(ViewClientProperties.HierarchyViewClientProperties.DisallowList, propertyTag) >= 0)
				{
					throw new RopExecutionException(string.Format("This client side property is not supported on HierarchyView. Property = {0}.", propertyTag), (ErrorCode)2147746050U);
				}
			}
		}

		protected override ClientSideProperties ClientSideProperties
		{
			get
			{
				if ((byte)(base.TableFlags & TableFlags.Depth) != 0)
				{
					return ClientSideProperties.DeepHierarchyViewInstance;
				}
				return ClientSideProperties.HierarchyViewInstance;
			}
		}
	}
}
