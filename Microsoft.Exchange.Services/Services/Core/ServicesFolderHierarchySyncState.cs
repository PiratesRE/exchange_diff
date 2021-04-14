using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ServicesFolderHierarchySyncState : ServicesSyncStateBase, IFolderHierarchySyncState, ISyncState
	{
		public ServicesFolderHierarchySyncState(MailboxSession session, StoreObjectId folderId, ISyncProvider syncProvider, string base64SyncData) : base(folderId, syncProvider)
		{
			ExTraceGlobals.SynchronizationTracer.TraceDebug((long)this.GetHashCode(), "ServicesFolderHierarchySyncState constructor called");
			this.session = session;
			base.Version = 1;
			base.Load(base64SyncData);
		}

		public FolderHierarchySync GetFolderHierarchySync()
		{
			return this.GetFolderHierarchySync(new ChangeTrackingDelegate(ServicesFolderHierarchySyncState.ComputeChangeTrackingHash));
		}

		public FolderHierarchySync GetFolderHierarchySync(ChangeTrackingDelegate changeTrackingDelegate)
		{
			return new FolderHierarchySync(this.session, this, changeTrackingDelegate);
		}

		private static int ComputeChangeTrackingHash(MailboxSession session, StoreObjectId folderId, IStorePropertyBag propertyBag)
		{
			if (propertyBag != null)
			{
				StringBuilder stringBuilder = new StringBuilder(128);
				string text;
				stringBuilder.Append(FolderHierarchySync.TryGetPropertyFromBag<string>(propertyBag, FolderSchema.DisplayName, null, out text) ? text : string.Empty);
				StoreObjectId storeObjectId;
				stringBuilder.Append(FolderHierarchySync.TryGetPropertyFromBag<StoreObjectId>(propertyBag, StoreObjectSchema.ParentItemId, null, out storeObjectId) ? storeObjectId.ToString() : string.Empty);
				string text2;
				stringBuilder.Append(FolderHierarchySync.TryGetPropertyFromBag<string>(propertyBag, StoreObjectSchema.ContainerClass, null, out text2) ? text2 : string.Empty);
				return stringBuilder.ToString().GetHashCode();
			}
			int hashCode;
			using (Folder folder = Folder.Bind(session, folderId, null))
			{
				string text3 = folder.DisplayName + folder.ParentId + folder.ClassName;
				hashCode = text3.GetHashCode();
			}
			return hashCode;
		}

		internal override StringData SyncStateTag
		{
			get
			{
				return ServicesFolderHierarchySyncState.FolderHierarchySyncTagValue;
			}
		}

		private const int CurrentFolderHierarchySyncStateVersion = 1;

		internal static StringData FolderHierarchySyncTagValue = new StringData("WS.FolderHierarchySync");

		private MailboxSession session;
	}
}
