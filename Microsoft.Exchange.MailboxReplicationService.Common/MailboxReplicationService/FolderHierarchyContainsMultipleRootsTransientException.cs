using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderHierarchyContainsMultipleRootsTransientException : FolderHierarchyIsInconsistentTransientException
	{
		public FolderHierarchyContainsMultipleRootsTransientException(string root1str, string root2str) : base(MrsStrings.FolderHierarchyContainsMultipleRoots(root1str, root2str))
		{
			this.root1str = root1str;
			this.root2str = root2str;
		}

		public FolderHierarchyContainsMultipleRootsTransientException(string root1str, string root2str, Exception innerException) : base(MrsStrings.FolderHierarchyContainsMultipleRoots(root1str, root2str), innerException)
		{
			this.root1str = root1str;
			this.root2str = root2str;
		}

		protected FolderHierarchyContainsMultipleRootsTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.root1str = (string)info.GetValue("root1str", typeof(string));
			this.root2str = (string)info.GetValue("root2str", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("root1str", this.root1str);
			info.AddValue("root2str", this.root2str);
		}

		public string Root1str
		{
			get
			{
				return this.root1str;
			}
		}

		public string Root2str
		{
			get
			{
				return this.root2str;
			}
		}

		private readonly string root1str;

		private readonly string root2str;
	}
}
