using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class FolderBasedView : View
	{
		protected FolderBasedView(Logon logon, ReferenceCount<CoreFolder> coreFolderReference, TableFlags tableFlags, View.Capabilities capabilities, ViewType viewType, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle, QueryFilter defaultQueryFilter = null) : base(logon, tableFlags, capabilities, viewType, notificationHandler, returnNotificationHandle, defaultQueryFilter)
		{
			this.coreFolderReference = coreFolderReference;
			this.coreFolderReference.AddRef();
			this.coreFolderId = coreFolderReference.ReferencedObject.Id.ObjectId;
		}

		protected CoreFolder CoreFolder
		{
			get
			{
				return this.coreFolderReference.ReferencedObject;
			}
		}

		protected override ICoreObject CoreObject
		{
			get
			{
				return this.coreFolderReference.ReferencedObject;
			}
		}

		protected override IViewDataSource CreateDataSource()
		{
			IViewDataSource result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IQueryResult queryResult = this.CreateQueryResult(this.ColumnPropertyDefinitions);
				disposeGuard.Add<IQueryResult>(queryResult);
				bool useUnicodeForRestrictions = (byte)(base.TableFlags & TableFlags.MapiUnicode) != 0;
				QueryResultViewDataSource queryResultViewDataSource = new QueryResultViewDataSource(this.coreFolderReference.ReferencedObject.Session, base.ServerColumns, queryResult, useUnicodeForRestrictions);
				disposeGuard.Success();
				result = queryResultViewDataSource;
			}
			return result;
		}

		protected override NativeStorePropertyDefinition[] ColumnPropertyDefinitions
		{
			get
			{
				return base.GetColumnPropertyDefinitions(this.coreFolderReference.ReferencedObject.Session, this.coreFolderReference.ReferencedObject.PropertyBag);
			}
		}

		protected StoreObjectId ContainerStoreObjectId
		{
			get
			{
				return this.coreFolderId;
			}
		}

		protected override StoreId? ContainerFolderId
		{
			get
			{
				return new StoreId?(new StoreId(base.LogonObject.Session.IdConverter.GetFidFromId(this.coreFolderId)));
			}
		}

		protected abstract IQueryResult CreateQueryResult(NativeStorePropertyDefinition[] propertyDefinitions);

		protected override void InternalDispose()
		{
			base.InternalDispose();
			this.coreFolderReference.Release();
		}

		private readonly ReferenceCount<CoreFolder> coreFolderReference;

		private readonly StoreObjectId coreFolderId;
	}
}
