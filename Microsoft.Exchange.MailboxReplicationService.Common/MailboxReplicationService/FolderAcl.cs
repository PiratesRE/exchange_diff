using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class FolderAcl : ItemPropertiesBase
	{
		[DataMember(Name = "Flags")]
		public int FlagsInt { get; set; }

		[DataMember(Name = "Value")]
		public PropValueData[][] Value { get; set; }

		public FolderAcl(AclFlags flags, PropValueData[][] pvda)
		{
			this.Flags = flags;
			this.Value = pvda;
		}

		public AclFlags Flags
		{
			get
			{
				return (AclFlags)this.FlagsInt;
			}
			set
			{
				this.FlagsInt = (int)value;
			}
		}

		public override void Apply(CoreFolder folder)
		{
			if (this.Value == null)
			{
				return;
			}
			ModifyTableOptions modifyTableOptions = this.Flags.HasFlag(AclFlags.FreeBusyAcl) ? ModifyTableOptions.FreeBusyAware : ModifyTableOptions.None;
			modifyTableOptions |= ModifyTableOptions.ExtendedPermissionInformation;
			using (IModifyTable permissionTableDoNotLoadEntries = folder.GetPermissionTableDoNotLoadEntries(modifyTableOptions))
			{
				foreach (PropValueData[] array in this.Value)
				{
					List<PropValue> list = new List<PropValue>();
					int j = 0;
					while (j < array.Length)
					{
						PropValueData propValueData = array[j];
						int propTag = propValueData.PropTag;
						if (propTag <= 1718747166)
						{
							if (propTag != 268370178)
							{
								if (propTag != 1718681620)
								{
									if (propTag != 1718747166)
									{
										goto IL_168;
									}
									list.Add(new PropValue(PermissionSchema.MemberName, (string)propValueData.Value));
								}
							}
							else
							{
								byte[] array2 = (byte[])propValueData.Value;
								if (array2 != null)
								{
									list.Add(new PropValue(PermissionSchema.MemberEntryId, array2));
								}
							}
						}
						else if (propTag != 1718812675)
						{
							if (propTag != 1718878466)
							{
								if (propTag != 1718943755)
								{
									goto IL_168;
								}
								list.Add(new PropValue(PermissionSchema.MemberIsGroup, (bool)propValueData.Value));
							}
							else
							{
								list.Add(new PropValue(PermissionSchema.MemberSecurityIdentifier, (byte[])propValueData.Value));
							}
						}
						else
						{
							list.Add(new PropValue(PermissionSchema.MemberRights, (MemberRights)propValueData.Value));
						}
						IL_191:
						j++;
						continue;
						IL_168:
						MrsTracer.Provider.Warning("StorageDestinationFolder.SetAcl: Unknown PropTag 0x{0:x}", new object[]
						{
							propValueData.PropTag
						});
						goto IL_191;
					}
					permissionTableDoNotLoadEntries.AddRow(list.ToArray());
				}
				permissionTableDoNotLoadEntries.ApplyPendingChanges();
			}
		}
	}
}
