using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExternalUserIdAndSession : IdAndSession
	{
		public ExternalUserIdAndSession(StoreId storeId, StoreSession session, Permission permission) : base(storeId, session)
		{
			this.permission = permission;
		}

		public ExternalUserIdAndSession(StoreId storeId, StoreSession session, IList<AttachmentId> attachmentIds, Permission permission) : base(storeId, session, attachmentIds)
		{
			this.permission = permission;
		}

		public ExternalUserIdAndSession(StoreId storeId, StoreId parentFolderId, StoreSession session, Permission permission) : base(storeId, parentFolderId, session)
		{
			this.permission = permission;
		}

		public ExternalUserIdAndSession(StoreId storeId, StoreId parentFolderId, StoreSession session, IList<AttachmentId> attachmentIds, Permission permission) : base(storeId, parentFolderId, session, attachmentIds)
		{
			this.permission = permission;
		}

		public Permission PermissionGranted
		{
			get
			{
				return this.permission;
			}
		}

		private Permission permission;
	}
}
