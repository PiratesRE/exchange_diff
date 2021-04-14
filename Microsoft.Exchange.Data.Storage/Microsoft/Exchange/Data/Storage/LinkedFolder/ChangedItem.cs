using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ChangedItem
	{
		public Guid Id { get; private set; }

		public string Version { get; private set; }

		public ExDateTime LastModified { get; private set; }

		public ExDateTime WhenCreated { get; private set; }

		public string LeafNode { get; private set; }

		public string RelativePath { get; private set; }

		public Uri Authority { get; private set; }

		public Uri Path { get; private set; }

		public int Level { get; private set; }

		public Uri ParentPath { get; private set; }

		public int ParentLevel { get; private set; }

		public Exception InducedException { get; set; }

		protected ChangedItem(Uri authority, Guid id, string version, string relativePath, string leafNode, ExDateTime whenCreated, ExDateTime lastModified)
		{
			if (authority == null)
			{
				throw new ArgumentNullException("authority");
			}
			if (string.IsNullOrEmpty(relativePath))
			{
				throw new ArgumentNullException("relativePath");
			}
			int num = relativePath.LastIndexOf('/');
			if (num == -1)
			{
				throw new ArgumentException("Unexpected relativePath as " + relativePath);
			}
			if (relativePath[0] == '/')
			{
				relativePath = relativePath.Substring(1, relativePath.Length - 1);
			}
			this.Id = id;
			this.Version = version;
			this.RelativePath = relativePath;
			this.LeafNode = leafNode;
			this.WhenCreated = whenCreated;
			this.LastModified = lastModified;
			this.Path = new Uri(authority, this.RelativePath);
			this.Authority = authority;
			num = this.RelativePath.LastIndexOf('/');
			if (num != -1)
			{
				this.Level = this.RelativePath.Split(new char[]
				{
					'/'
				}).Length;
				this.ParentLevel = this.Level - 1;
				this.ParentPath = new Uri(this.Authority, this.RelativePath.Substring(0, num));
			}
		}
	}
}
