using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AttachmentView : View
	{
		internal AttachmentView(Logon logon, ReferenceCount<CoreItem> coreItemReference, TableFlags tableFlags, NotificationHandler notificationHandler, ServerObjectHandle returnNotificationHandle) : base(logon, tableFlags, View.Capabilities.Basic, ViewType.MessageView, notificationHandler, returnNotificationHandle, null)
		{
			this.coreItemReference = coreItemReference;
			this.coreItemReference.AddRef();
		}

		protected override ICoreObject CoreObject
		{
			get
			{
				return this.coreItemReference.ReferencedObject;
			}
		}

		protected override IViewDataSource CreateDataSource()
		{
			IViewDataSource result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				QueryResult queryResult = AttachmentTable.GetQueryResult(this.coreItemReference.ReferencedObject, this.ColumnPropertyDefinitions);
				disposeGuard.Add<QueryResult>(queryResult);
				bool useUnicodeForRestrictions = (byte)(base.TableFlags & TableFlags.MapiUnicode) != 0;
				QueryResultViewDataSource queryResultViewDataSource = new QueryResultViewDataSource(this.coreItemReference.ReferencedObject.Session, base.ServerColumns, queryResult, useUnicodeForRestrictions);
				disposeGuard.Success();
				result = queryResultViewDataSource;
			}
			return result;
		}

		protected override NativeStorePropertyDefinition[] ColumnPropertyDefinitions
		{
			get
			{
				return base.GetColumnPropertyDefinitions(this.coreItemReference.ReferencedObject.Session, this.coreItemReference.ReferencedObject.PropertyBag);
			}
		}

		protected override StoreId? ContainerFolderId
		{
			get
			{
				return null;
			}
		}

		protected override PropertyConverter PropertyConverter
		{
			get
			{
				return PropertyConverter.Attachment;
			}
		}

		protected override void CheckPropertiesAllowed(PropertyTag[] propertyTags)
		{
		}

		protected override ClientSideProperties ClientSideProperties
		{
			get
			{
				return ClientSideProperties.AttachmentInstance;
			}
		}

		protected override void InternalDispose()
		{
			this.coreItemReference.Release();
			base.InternalDispose();
		}

		private readonly ReferenceCount<CoreItem> coreItemReference;
	}
}
