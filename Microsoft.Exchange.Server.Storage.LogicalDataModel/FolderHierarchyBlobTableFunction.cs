using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class FolderHierarchyBlobTableFunction
	{
		internal FolderHierarchyBlobTableFunction()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.parentFolderId = Factory.CreatePhysicalColumn("ParentFolderId", "ParentFolderId", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.folderId = Factory.CreatePhysicalColumn("FolderId", "FolderId", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.displayName = Factory.CreatePhysicalColumn("DisplayName", "DisplayName", typeof(string), false, false, false, false, false, Visibility.Public, 512, 0, 512);
			this.depth = Factory.CreatePhysicalColumn("Depth", "Depth", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.sortPosition = Factory.CreatePhysicalColumn("SortPosition", "SortPosition", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			string name = "PrimaryKey";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			Index index = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.SortPosition,
				this.FolderId
			});
			Index[] indexes = new Index[]
			{
				index
			};
			this.tableFunction = Factory.CreateTableFunction("FolderHierarchyBlob", new TableFunction.GetTableContentsDelegate(this.GetTableContents), new TableFunction.GetColumnFromRowDelegate(this.GetColumnFromRow), Visibility.Redacted, new Type[]
			{
				typeof(IEnumerable<FolderHierarchyBlob>)
			}, indexes, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.MailboxNumber,
				this.ParentFolderId,
				this.FolderId,
				this.DisplayName,
				this.Depth,
				this.SortPosition
			});
		}

		public TableFunction TableFunction
		{
			get
			{
				return this.tableFunction;
			}
		}

		public PhysicalColumn MailboxPartitionNumber
		{
			get
			{
				return this.mailboxPartitionNumber;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public PhysicalColumn ParentFolderId
		{
			get
			{
				return this.parentFolderId;
			}
		}

		public PhysicalColumn FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public PhysicalColumn DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public PhysicalColumn Depth
		{
			get
			{
				return this.depth;
			}
		}

		public PhysicalColumn SortPosition
		{
			get
			{
				return this.sortPosition;
			}
		}

		public object GetTableContents(IConnectionProvider connectionProvider, object[] parameters)
		{
			return parameters[0];
		}

		public object GetColumnFromRow(IConnectionProvider connectionProvider, object row, PhysicalColumn columnToFetch)
		{
			FolderHierarchyBlob folderHierarchyBlob = (FolderHierarchyBlob)row;
			if (columnToFetch == this.MailboxPartitionNumber)
			{
				return folderHierarchyBlob.MailboxPartitionNumber;
			}
			if (columnToFetch == this.MailboxNumber)
			{
				return folderHierarchyBlob.MailboxNumber;
			}
			if (columnToFetch == this.ParentFolderId)
			{
				return folderHierarchyBlob.ParentFolderId;
			}
			if (columnToFetch == this.FolderId)
			{
				return folderHierarchyBlob.FolderId;
			}
			if (columnToFetch == this.DisplayName)
			{
				return folderHierarchyBlob.DisplayName;
			}
			if (columnToFetch == this.Depth)
			{
				return folderHierarchyBlob.Depth;
			}
			if (columnToFetch == this.SortPosition)
			{
				return folderHierarchyBlob.SortPosition;
			}
			return null;
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string MailboxNumberName = "MailboxNumber";

		public const string ParentFolderIdName = "ParentFolderId";

		public const string FolderIdName = "FolderId";

		public const string DisplayNameName = "DisplayName";

		public const string DepthName = "Depth";

		public const string SortPositionName = "SortPosition";

		public const string TableFunctionName = "FolderHierarchyBlob";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn parentFolderId;

		private PhysicalColumn folderId;

		private PhysicalColumn displayName;

		private PhysicalColumn depth;

		private PhysicalColumn sortPosition;

		private TableFunction tableFunction;
	}
}
