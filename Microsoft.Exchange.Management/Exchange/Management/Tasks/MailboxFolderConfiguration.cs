using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public class MailboxFolderConfiguration : ConfigurableObject
	{
		public MailboxFolderConfiguration() : base(new MapiFolderConfigurationPropertyBag())
		{
			this.DeletedItemsInFolder = 0;
			this.DeletedItemsInFolderAndSubfolders = 0;
			this.ItemsInFolder = 0;
			this.ItemsInFolderAndSubfolders = 0;
			this.FolderAndSubfolderSize = ByteQuantifiedSize.FromBytes(0UL);
			this.FolderSize = ByteQuantifiedSize.FromBytes(0UL);
			this.Date = DateTime.MinValue;
			this.NewestDeletedItemReceivedDate = null;
			this.NewestItemReceivedDate = null;
			this.OldestDeletedItemReceivedDate = null;
			this.OldestItemReceivedDate = null;
			this.NewestDeletedItemLastModifiedDate = null;
			this.NewestItemLastModifiedDate = null;
			this.OldestDeletedItemLastModifiedDate = null;
			this.OldestItemLastModifiedDate = null;
			this.FolderId = null;
			this.FolderPath = null;
			this.FolderType = null;
			this.Name = null;
			this.ManagedFolder = null;
			this.DeletePolicy = null;
			this.ArchivePolicy = null;
			this.TopSubjectCount = 0;
			this.TopSubjectSize = ByteQuantifiedSize.FromBytes(0UL);
			this.TopClientInfoForSubject = string.Empty;
			this.TopClientInfoCountForSubject = 0;
			this.TopSubjectPath = string.Empty;
			this.TopSubject = string.Empty;
			this.TopSubjectReceivedTime = null;
			this.TopSubjectFrom = string.Empty;
			this.TopSubjectClass = string.Empty;
			this.SearchFolders = null;
		}

		public DateTime Date
		{
			get
			{
				return (DateTime)this.propertyBag[MapiFolderConfigurationSchema.Date];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.Date] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[MapiFolderConfigurationSchema.Name];
			}
			internal set
			{
				this[MapiFolderConfigurationSchema.Name] = value;
			}
		}

		internal void SetIdentity(MailboxFolderId value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("Identity_set");
			}
			this.propertyBag[MapiFolderConfigurationSchema.Identity] = value;
		}

		public string FolderPath
		{
			get
			{
				return (string)this[MapiFolderConfigurationSchema.FolderPath];
			}
			internal set
			{
				this[MapiFolderConfigurationSchema.FolderPath] = value;
			}
		}

		public string FolderId
		{
			get
			{
				return (string)this.propertyBag[MapiFolderConfigurationSchema.FolderId];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.FolderId] = value;
			}
		}

		public string FolderType
		{
			get
			{
				return (string)this.propertyBag[MapiFolderConfigurationSchema.FolderType];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.FolderType] = value;
			}
		}

		public int ItemsInFolder
		{
			get
			{
				return (int)this.propertyBag[MapiFolderConfigurationSchema.ItemsInFolder];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.ItemsInFolder] = value;
			}
		}

		public int DeletedItemsInFolder
		{
			get
			{
				return (int)this.propertyBag[MapiFolderConfigurationSchema.DeletedItemsInFolder];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.DeletedItemsInFolder] = value;
			}
		}

		public ByteQuantifiedSize FolderSize
		{
			get
			{
				return (ByteQuantifiedSize)this.propertyBag[MapiFolderConfigurationSchema.FolderSize];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.FolderSize] = value;
			}
		}

		public int ItemsInFolderAndSubfolders
		{
			get
			{
				return (int)this.propertyBag[MapiFolderConfigurationSchema.ItemsInFolderAndSubfolders];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.ItemsInFolderAndSubfolders] = value;
			}
		}

		public int DeletedItemsInFolderAndSubfolders
		{
			get
			{
				return (int)this.propertyBag[MapiFolderConfigurationSchema.DeletedItemsInFolderAndSubfolders];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.DeletedItemsInFolderAndSubfolders] = value;
			}
		}

		public ByteQuantifiedSize FolderAndSubfolderSize
		{
			get
			{
				return (ByteQuantifiedSize)this.propertyBag[MapiFolderConfigurationSchema.FolderAndSubfolderSize];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.FolderAndSubfolderSize] = value;
			}
		}

		public DateTime? OldestItemReceivedDate
		{
			get
			{
				return (DateTime?)this.propertyBag[MapiFolderConfigurationSchema.OldestItemReceivedDate];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.OldestItemReceivedDate] = value;
			}
		}

		public DateTime? NewestItemReceivedDate
		{
			get
			{
				return (DateTime?)this.propertyBag[MapiFolderConfigurationSchema.NewestItemReceivedDate];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.NewestItemReceivedDate] = value;
			}
		}

		public DateTime? OldestDeletedItemReceivedDate
		{
			get
			{
				return (DateTime?)this.propertyBag[MapiFolderConfigurationSchema.OldestDeletedItemReceivedDate];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.OldestDeletedItemReceivedDate] = value;
			}
		}

		public DateTime? NewestDeletedItemReceivedDate
		{
			get
			{
				return (DateTime?)this.propertyBag[MapiFolderConfigurationSchema.NewestDeletedItemReceivedDate];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.NewestDeletedItemReceivedDate] = value;
			}
		}

		public DateTime? OldestItemLastModifiedDate
		{
			get
			{
				return (DateTime?)this.propertyBag[MapiFolderConfigurationSchema.OldestItemLastModifiedDate];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.OldestItemLastModifiedDate] = value;
			}
		}

		public DateTime? NewestItemLastModifiedDate
		{
			get
			{
				return (DateTime?)this.propertyBag[MapiFolderConfigurationSchema.NewestItemLastModifiedDate];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.NewestItemLastModifiedDate] = value;
			}
		}

		public DateTime? OldestDeletedItemLastModifiedDate
		{
			get
			{
				return (DateTime?)this.propertyBag[MapiFolderConfigurationSchema.OldestDeletedItemLastModifiedDate];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.OldestDeletedItemLastModifiedDate] = value;
			}
		}

		public DateTime? NewestDeletedItemLastModifiedDate
		{
			get
			{
				return (DateTime?)this.propertyBag[MapiFolderConfigurationSchema.NewestDeletedItemLastModifiedDate];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.NewestDeletedItemLastModifiedDate] = value;
			}
		}

		public ELCFolderIdParameter ManagedFolder
		{
			get
			{
				return (ELCFolderIdParameter)this.propertyBag[MapiFolderConfigurationSchema.ManagedFolder];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.ManagedFolder] = value;
			}
		}

		public RetentionPolicyTagIdParameter DeletePolicy
		{
			get
			{
				return (RetentionPolicyTagIdParameter)this.propertyBag[MapiFolderConfigurationSchema.DeletePolicy];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.DeletePolicy] = value;
			}
		}

		public RetentionPolicyTagIdParameter ArchivePolicy
		{
			get
			{
				return (RetentionPolicyTagIdParameter)this.propertyBag[MapiFolderConfigurationSchema.ArchivePolicy];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.ArchivePolicy] = value;
			}
		}

		public string TopSubject
		{
			get
			{
				return (string)this[MapiFolderConfigurationSchema.TopSubject];
			}
			internal set
			{
				this[MapiFolderConfigurationSchema.TopSubject] = value;
			}
		}

		public ByteQuantifiedSize TopSubjectSize
		{
			get
			{
				return (ByteQuantifiedSize)this.propertyBag[MapiFolderConfigurationSchema.TopSubjectSize];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.TopSubjectSize] = value;
			}
		}

		public int TopSubjectCount
		{
			get
			{
				return (int)this.propertyBag[MapiFolderConfigurationSchema.TopSubjectCount];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.TopSubjectCount] = value;
			}
		}

		public string TopSubjectClass
		{
			get
			{
				return (string)this.propertyBag[MapiFolderConfigurationSchema.TopSubjectClass];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.TopSubjectClass] = value;
			}
		}

		public string TopSubjectPath
		{
			get
			{
				return (string)this[MapiFolderConfigurationSchema.TopSubjectPath];
			}
			internal set
			{
				this[MapiFolderConfigurationSchema.TopSubjectPath] = value;
			}
		}

		public DateTime? TopSubjectReceivedTime
		{
			get
			{
				return (DateTime?)this.propertyBag[MapiFolderConfigurationSchema.TopSubjectReceivedTime];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.TopSubjectReceivedTime] = value;
			}
		}

		public string TopSubjectFrom
		{
			get
			{
				return (string)this[MapiFolderConfigurationSchema.TopSubjectFrom];
			}
			internal set
			{
				this[MapiFolderConfigurationSchema.TopSubjectFrom] = value;
			}
		}

		public string TopClientInfoForSubject
		{
			get
			{
				return (string)this.propertyBag[MapiFolderConfigurationSchema.TopClientInfoForSubject];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.TopClientInfoForSubject] = value;
			}
		}

		public int TopClientInfoCountForSubject
		{
			get
			{
				return (int)this.propertyBag[MapiFolderConfigurationSchema.TopClientInfoCountForSubject];
			}
			internal set
			{
				this.propertyBag[MapiFolderConfigurationSchema.TopClientInfoCountForSubject] = value;
			}
		}

		public string[] SearchFolders
		{
			get
			{
				return (string[])this[MapiFolderConfigurationSchema.SearchFolders];
			}
			internal set
			{
				this[MapiFolderConfigurationSchema.SearchFolders] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				ObjectId objectId = (ObjectId)this[MapiFolderConfigurationSchema.Identity];
				if (SuppressingPiiContext.NeedPiiSuppression && objectId is MailboxFolderId)
				{
					string text = objectId.ToString();
					int num = text.IndexOf('\\');
					if (num <= 0 || num >= text.Length - 1)
					{
						return objectId;
					}
					string value = text.Substring(0, num);
					string value2 = text.Substring(num);
					objectId = new MailboxFolderId(SuppressingPiiData.Redact(value), SuppressingPiiData.Redact(value2));
				}
				return objectId;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxFolderConfiguration.schema;
			}
		}

		private static MapiFolderConfigurationSchema schema = new MapiFolderConfigurationSchema();
	}
}
