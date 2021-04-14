using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderHierarchyContainsDuplicatesPermanentException : FolderHierarchyIsInconsistentPermanentException
	{
		public FolderHierarchyContainsDuplicatesPermanentException(string folder1str, string folder2str) : base(MrsStrings.FolderHierarchyContainsDuplicates(folder1str, folder2str))
		{
			this.folder1str = folder1str;
			this.folder2str = folder2str;
		}

		public FolderHierarchyContainsDuplicatesPermanentException(string folder1str, string folder2str, Exception innerException) : base(MrsStrings.FolderHierarchyContainsDuplicates(folder1str, folder2str), innerException)
		{
			this.folder1str = folder1str;
			this.folder2str = folder2str;
		}

		protected FolderHierarchyContainsDuplicatesPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folder1str = (string)info.GetValue("folder1str", typeof(string));
			this.folder2str = (string)info.GetValue("folder2str", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folder1str", this.folder1str);
			info.AddValue("folder2str", this.folder2str);
		}

		public string Folder1str
		{
			get
			{
				return this.folder1str;
			}
		}

		public string Folder2str
		{
			get
			{
				return this.folder2str;
			}
		}

		private readonly string folder1str;

		private readonly string folder2str;
	}
}
