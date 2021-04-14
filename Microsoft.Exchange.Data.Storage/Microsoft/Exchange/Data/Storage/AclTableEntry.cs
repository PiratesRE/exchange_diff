using System;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AclTableEntry : FolderSecurity.AclTableEntry
	{
		internal AclTableEntry(long id, byte[] entryId, string name, MemberRights rights) : base(id, entryId, name, (FolderSecurity.ExchangeFolderRights)rights)
		{
		}

		private AclTableEntry(BinaryDeserializer deserializer) : base(deserializer.Reader)
		{
		}

		private AclTableEntry(BinaryReader deserializer) : base(deserializer)
		{
		}

		public string MemberName
		{
			get
			{
				return base.Name;
			}
		}

		public long MemberId
		{
			get
			{
				return base.RowId;
			}
		}

		public byte[] MemberEntryId
		{
			get
			{
				return base.EntryId;
			}
		}

		public MemberRights MemberRights
		{
			get
			{
				return (MemberRights)base.Rights;
			}
			set
			{
				base.Rights = (FolderSecurity.ExchangeFolderRights)value;
			}
		}

		internal static AclTableEntry Parse(BinaryDeserializer deserializer)
		{
			return new AclTableEntry(deserializer);
		}

		internal new static AclTableEntry Parse(BinaryReader deserializer)
		{
			return new AclTableEntry(deserializer);
		}

		internal void Serialize(BinarySerializer serializer)
		{
			base.Serialize(serializer.Writer);
		}

		internal void SetMemberId(long id)
		{
			base.RowId = id;
		}

		internal void SetMemberName(string name)
		{
			base.Name = name;
		}

		internal class ModifyOperation
		{
			internal ModifyOperation(ModifyTableOperationType operation, AclTableEntry tableEntry)
			{
				this.operation = operation;
				this.tableEntry = tableEntry;
			}

			public static AclTableEntry.ModifyOperation FromModifyTableOperation(ModifyTableOperation modifyTableOperation)
			{
				long? num = null;
				byte[] array = null;
				MemberRights? memberRights = null;
				string text = null;
				byte[] array2 = null;
				bool? flag = null;
				foreach (PropValue propValue in modifyTableOperation.Properties)
				{
					if (propValue.Property == PermissionSchema.MemberId)
					{
						num = new long?((long)propValue.Value);
					}
					else if (propValue.Property == PermissionSchema.MemberEntryId)
					{
						array = (byte[])propValue.Value;
					}
					else if (propValue.Property == PermissionSchema.MemberRights)
					{
						memberRights = new MemberRights?((MemberRights)((int)propValue.Value));
					}
					else if (propValue.Property == PermissionSchema.MemberName)
					{
						text = (string)propValue.Value;
					}
					else if (propValue.Property == PermissionSchema.MemberSecurityIdentifier)
					{
						array2 = (byte[])propValue.Value;
					}
					else
					{
						if (propValue.Property != PermissionSchema.MemberIsGroup)
						{
							throw new InvalidParamException(new LocalizedString("Unexpected property in modification entry"));
						}
						flag = new bool?((bool)propValue.Value);
					}
				}
				AclTableEntry aclTableEntry = null;
				switch (modifyTableOperation.Operation)
				{
				case ModifyTableOperationType.Add:
					if (array == null || memberRights == null || num != null || (text != null && (array2 == null || flag == null)))
					{
						throw new InvalidParamException(new LocalizedString("Invalid modification(add) entry"));
					}
					aclTableEntry = new AclTableEntry(0L, array, string.Empty, memberRights.Value);
					break;
				case ModifyTableOperationType.Modify:
					if (num == null || memberRights == null || array != null || text != null)
					{
						throw new InvalidParamException(new LocalizedString("Invalid modification(modify) entry"));
					}
					aclTableEntry = new AclTableEntry(num.Value, null, string.Empty, memberRights.Value);
					break;
				case ModifyTableOperationType.Remove:
					if (num == null || memberRights != null || array != null || text != null)
					{
						throw new InvalidParamException(new LocalizedString("Invalid modification(remove) entry"));
					}
					aclTableEntry = new AclTableEntry(num.Value, null, string.Empty, MemberRights.None);
					break;
				}
				return new AclTableEntry.ModifyOperation(modifyTableOperation.Operation, aclTableEntry);
			}

			public ModifyTableOperationType Operation
			{
				get
				{
					return this.operation;
				}
			}

			public AclTableEntry Entry
			{
				get
				{
					return this.tableEntry;
				}
			}

			private readonly ModifyTableOperationType operation;

			private readonly AclTableEntry tableEntry;
		}
	}
}
