using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderSyncJobRpcInParameters : RpcParameters
	{
		public OrganizationId OrganizationId { get; private set; }

		public Guid ContentMailboxGuid { get; private set; }

		public PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction SyncAction { get; private set; }

		public byte[] FolderId { get; private set; }

		public PublicFolderSyncJobRpcInParameters(byte[] data) : base(data)
		{
			this.OrganizationId = (OrganizationId)base.GetParameterValue("OrganizationId");
			this.ContentMailboxGuid = (Guid)base.GetParameterValue("ContentMailboxGuid");
			this.SyncAction = (PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction)base.GetParameterValue("PublicFolderSyncAction");
			this.FolderId = (byte[])base.GetParameterValue("PublicFolderId");
		}

		public PublicFolderSyncJobRpcInParameters(OrganizationId organizationId, Guid contentMailboxGuid, PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction syncAction) : this(organizationId, contentMailboxGuid, syncAction, null)
		{
		}

		public PublicFolderSyncJobRpcInParameters(OrganizationId organizationId, Guid contentMailboxGuid, byte[] folderId) : this(organizationId, contentMailboxGuid, PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction.SyncFolder, folderId)
		{
		}

		private PublicFolderSyncJobRpcInParameters(OrganizationId organizationId, Guid contentMailboxGuid, PublicFolderSyncJobRpcInParameters.PublicFolderSyncAction syncAction, byte[] folderId)
		{
			this.OrganizationId = organizationId;
			this.ContentMailboxGuid = contentMailboxGuid;
			this.SyncAction = syncAction;
			this.FolderId = folderId;
			base.SetParameterValue("ContentMailboxGuid", this.ContentMailboxGuid);
			base.SetParameterValue("OrganizationId", this.OrganizationId);
			base.SetParameterValue("PublicFolderSyncAction", this.SyncAction);
			base.SetParameterValue("PublicFolderId", this.FolderId);
		}

		private const string ContentMailboxGuidParameterName = "ContentMailboxGuid";

		private const string OrganizationIdParameterName = "OrganizationId";

		private const string PublicFolderSyncActionParameterName = "PublicFolderSyncAction";

		private const string PublicFolderIdParameterName = "PublicFolderId";

		public enum PublicFolderSyncAction : uint
		{
			StartSyncHierarchy,
			QueryStatusSyncHierarchy,
			SyncFolder,
			StartSyncHierarchyWithFolderReconciliation
		}
	}
}
