using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderHierarchyContainsParentChainLoopPermanentException : FolderHierarchyIsInconsistentPermanentException
	{
		public FolderHierarchyContainsParentChainLoopPermanentException(string folderIdStr) : base(MrsStrings.FolderHierarchyContainsParentChainLoop(folderIdStr))
		{
			this.folderIdStr = folderIdStr;
		}

		public FolderHierarchyContainsParentChainLoopPermanentException(string folderIdStr, Exception innerException) : base(MrsStrings.FolderHierarchyContainsParentChainLoop(folderIdStr), innerException)
		{
			this.folderIdStr = folderIdStr;
		}

		protected FolderHierarchyContainsParentChainLoopPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderIdStr = (string)info.GetValue("folderIdStr", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderIdStr", this.folderIdStr);
		}

		public string FolderIdStr
		{
			get
			{
				return this.folderIdStr;
			}
		}

		private readonly string folderIdStr;
	}
}
