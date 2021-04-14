using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MailboxChangesManifest
	{
		public MailboxChangesManifest()
		{
			this.changedFolders = null;
			this.deletedFolders = null;
			this.HasMoreHierarchyChanges = false;
		}

		[DataMember(Name = "changedFolders", EmitDefaultValue = false)]
		public List<byte[]> ChangedFolders
		{
			get
			{
				return this.changedFolders;
			}
			set
			{
				this.changedFolders = value;
			}
		}

		[DataMember(Name = "deletedFolders", EmitDefaultValue = false)]
		public List<byte[]> DeletedFolders
		{
			get
			{
				return this.deletedFolders;
			}
			set
			{
				this.deletedFolders = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Name = "FolderChanges")]
		public ICollection<FolderChangesManifest> FolderChangesList
		{
			get
			{
				return new FolderChangesManifest[0];
			}
			set
			{
			}
		}

		[DataMember(Name = "hasMoreHierarchyChanges", EmitDefaultValue = false)]
		public bool HasMoreHierarchyChanges { get; set; }

		public override string ToString()
		{
			return string.Format("{0} changed; {1} deleted", (this.changedFolders == null) ? 0 : this.changedFolders.Count, (this.deletedFolders == null) ? 0 : this.deletedFolders.Count);
		}

		private List<byte[]> changedFolders;

		private List<byte[]> deletedFolders;
	}
}
