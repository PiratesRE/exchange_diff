using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class MailboxAuditLogEvent : MailboxAuditLogRecord
	{
		public MailboxAuditLogEvent()
		{
		}

		public MailboxAuditLogEvent(MailboxAuditLogRecordId identity, string mailboxResolvedName, string guid, DateTime? lastAccessed) : base(identity, mailboxResolvedName, guid, lastAccessed)
		{
		}

		public string Operation
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.Operation] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.Operation] = value;
			}
		}

		public string OperationResult
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.OperationResult] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.OperationResult] = value;
			}
		}

		public string LogonType
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.LogonType] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.LogonType] = value;
			}
		}

		public bool? ExternalAccess
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.ExternalAccess] as bool?;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.ExternalAccess] = value;
			}
		}

		public string DestFolderId
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.DestFolderId] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.DestFolderId] = value;
			}
		}

		public string DestFolderPathName
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.DestFolderPathName] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.DestFolderPathName] = value;
			}
		}

		public string FolderId
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.FolderId] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.FolderId] = value;
			}
		}

		public string FolderPathName
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.FolderPathName] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.FolderPathName] = value;
			}
		}

		public string ClientInfoString
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.ClientInfoString] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.ClientInfoString] = value;
			}
		}

		public string ClientIPAddress
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.ClientIPAddress] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.ClientIPAddress] = value;
			}
		}

		public string ClientMachineName
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.ClientMachineName] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.ClientMachineName] = value;
			}
		}

		public string ClientProcessName
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.ClientProcessName] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.ClientProcessName] = value;
			}
		}

		public string ClientVersion
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.ClientVersion] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.ClientVersion] = value;
			}
		}

		public string InternalLogonType
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.InternalLogonType] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.InternalLogonType] = value;
			}
		}

		public string MailboxOwnerUPN
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.MailboxOwnerUPN] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.MailboxOwnerUPN] = value;
			}
		}

		public string MailboxOwnerSid
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.MailboxOwnerSid] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.MailboxOwnerSid] = value;
			}
		}

		public string DestMailboxOwnerUPN
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.DestMailboxOwnerUPN] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.DestMailboxOwnerUPN] = value;
			}
		}

		public string DestMailboxOwnerSid
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.DestMailboxOwnerSid] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.DestMailboxOwnerSid] = value;
			}
		}

		public string DestMailboxGuid
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.DestMailboxGuid] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.DestMailboxGuid] = value;
			}
		}

		public bool? CrossMailboxOperation
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.CrossMailboxOperation] as bool?;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.CrossMailboxOperation] = value;
			}
		}

		public string LogonUserDisplayName
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.LogonUserDisplayName] as string;
			}
			set
			{
				this.propertyBag[MailboxAuditLogEventSchema.LogonUserDisplayName] = value;
			}
		}

		public string LogonUserSid
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.LogonUserSid] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.LogonUserSid] = value;
			}
		}

		public MultiValuedProperty<MailboxAuditLogSourceItem> SourceItems
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.SourceItems] as MultiValuedProperty<MailboxAuditLogSourceItem>;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.SourceItems] = value;
			}
		}

		public MultiValuedProperty<MailboxAuditLogSourceFolder> SourceFolders
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.SourceFolders] as MultiValuedProperty<MailboxAuditLogSourceFolder>;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.SourceFolders] = value;
			}
		}

		public string SourceItemIdsList
		{
			get
			{
				List<string> list = new List<string>();
				if (this.SourceItems != null)
				{
					foreach (MailboxAuditLogSourceItem mailboxAuditLogSourceItem in this.SourceItems.Added)
					{
						list.Add(mailboxAuditLogSourceItem.SourceItemId);
					}
				}
				return string.Join(",", list.ToArray());
			}
		}

		public string SourceItemSubjectsList
		{
			get
			{
				List<string> list = new List<string>();
				if (this.SourceItems != null)
				{
					foreach (MailboxAuditLogSourceItem mailboxAuditLogSourceItem in this.SourceItems.Added)
					{
						list.Add(mailboxAuditLogSourceItem.SourceItemSubject);
					}
				}
				return string.Join(",", list.ToArray());
			}
		}

		public string SourceItemFolderPathNamesList
		{
			get
			{
				List<string> list = new List<string>();
				if (this.SourceItems != null)
				{
					foreach (MailboxAuditLogSourceItem mailboxAuditLogSourceItem in this.SourceItems.Added)
					{
						string item = this.FixFolderPathName(mailboxAuditLogSourceItem.SourceItemFolderPathName);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
				return string.Join(",", list.ToArray());
			}
		}

		public string SourceFolderIdsList
		{
			get
			{
				List<string> list = new List<string>();
				if (this.SourceFolders != null)
				{
					foreach (MailboxAuditLogSourceFolder mailboxAuditLogSourceFolder in this.SourceFolders.Added)
					{
						string item = this.FixFolderPathName(mailboxAuditLogSourceFolder.SourceFolderId);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
				return string.Join(",", list.ToArray());
			}
		}

		public string SourceFolderPathNamesList
		{
			get
			{
				List<string> list = new List<string>();
				if (this.SourceFolders != null)
				{
					foreach (MailboxAuditLogSourceFolder mailboxAuditLogSourceFolder in this.SourceFolders.Added)
					{
						string item = this.FixFolderPathName(mailboxAuditLogSourceFolder.SourceFolderPathName);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
				return string.Join(",", list.ToArray());
			}
		}

		public string ItemId
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.ItemId] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.ItemId] = value;
			}
		}

		public string ItemSubject
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.ItemSubject] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.ItemSubject] = value;
			}
		}

		public string DirtyProperties
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.DirtyProperties] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.DirtyProperties] = value;
			}
		}

		public string OriginatingServer
		{
			get
			{
				return this.propertyBag[MailboxAuditLogEventSchema.OriginatingServer] as string;
			}
			private set
			{
				this.propertyBag[MailboxAuditLogEventSchema.OriginatingServer] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxAuditLogEvent.schema;
			}
		}

		private string FixFolderPathName(string folderName)
		{
			folderName = folderName.Trim();
			folderName = folderName.TrimStart(new char[]
			{
				'/',
				'\\'
			});
			return folderName;
		}

		private const string LabelValueSeparator = ":";

		private const string PowerShellSeparator = ",";

		private static readonly ObjectSchema schema = ObjectSchema.GetInstance<MailboxAuditLogEventSchema>();

		internal static class Labels
		{
			public const string ClientInfoString = "ClientInfoString";

			public const string ClientIPAddress = "ClientIPAddress";

			public const string ClientMachineName = "ClientMachineName";

			public const string ClientProcessName = "ClientProcessName";

			public const string FolderPathName = "FolderPathName";

			public const string ClientVersion = "ClientVersion";

			public const string InternalLogonType = "InternalLogonType";

			public const string ExternalAccess = "ExternalAccess";

			public const string Operation = "Operation";

			public const string OperationResult = "OperationResult";

			public const string LogonType = "LogonType";

			public const string SourceFolderPathName = ".SourceFolderPathName";

			public const string SourceFolderId = ".SourceFolderId";

			public const string SourceItemSubject = ".SourceItemSubject";

			public const string SourceItemId = ".SourceItemId";

			public const string SourceItemFolderPathName = ".SourceItemFolderPathName";

			public const string MailboxOwnerUPN = "MailboxOwnerUPN";

			public const string MailboxOwnerSid = "MailboxOwnerSid";

			public const string MailboxGuid = "MailboxGuid";

			public const string LogonUserSid = "LogonUserSid";

			public const string LogonUserDisplayName = "LogonUserDisplayName";

			public const string DestFolderId = "DestFolderId";

			public const string DestFolderPathName = "DestFolderPathName";

			public const string FolderId = "FolderId";

			public const string ItemId = "ItemId";

			public const string ItemSubject = "ItemSubject";

			public const string DestinationFolderId = "DestinationFolderId";

			public const string DestinationFolderPathName = "DestinationFolderPathName";

			public const string CrossMailboxOperation = "CrossMailboxOperation";

			public const string DestMailboxGuid = "DestMailboxGuid";

			public const string DestMailboxOwnerUPN = "DestMailboxOwnerUPN";

			public const string DestMailboxOwnerSid = "DestMailboxOwnerSid";

			public const string DirtyProperties = "DirtyProperties";

			public const string OriginatingServer = "OriginatingServer";
		}
	}
}
