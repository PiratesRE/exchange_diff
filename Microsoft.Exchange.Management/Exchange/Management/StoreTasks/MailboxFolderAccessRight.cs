using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Serializable]
	public struct MailboxFolderAccessRight : IEquatable<MailboxFolderAccessRight>, IComparable, IComparable<MailboxFolderAccessRight>
	{
		internal int Permission
		{
			get
			{
				return this.permission;
			}
		}

		internal bool IsRole
		{
			get
			{
				return Enum.IsDefined(typeof(MailboxFolderPermissionRole), this.permission);
			}
		}

		internal static Collection<MailboxFolderAccessRight> CreateMailboxFolderAccessRightCollection(int permission)
		{
			Collection<MailboxFolderAccessRight> collection = new Collection<MailboxFolderAccessRight>();
			int num = permission & 8187;
			MailboxFolderAccessRight? mailboxFolderAccessRight;
			if (MailboxFolderAccessRight.TryGetMailboxFolderPermissionRole(num, out mailboxFolderAccessRight))
			{
				collection.Add(mailboxFolderAccessRight.Value);
			}
			else
			{
				int num2 = permission & 2043;
				if (num != num2 && MailboxFolderAccessRight.TryGetMailboxFolderPermissionRole(num2, out mailboxFolderAccessRight))
				{
					collection.Add(mailboxFolderAccessRight.Value);
				}
				else if (Enum.IsDefined(typeof(MailboxFolderMemberRights), num2))
				{
					collection.Add(new MailboxFolderAccessRight(num2));
				}
				else
				{
					foreach (object obj in Enum.GetValues(typeof(MailboxFolderMemberRights)))
					{
						int num3 = (int)obj;
						if ((num3 & num2) != 0)
						{
							collection.Add(new MailboxFolderAccessRight(num3));
						}
					}
				}
			}
			return collection;
		}

		private static bool TryGetMailboxFolderPermissionRole(int permission, out MailboxFolderAccessRight? role)
		{
			role = null;
			if (Enum.IsDefined(typeof(MailboxFolderPermissionRole), permission))
			{
				role = new MailboxFolderAccessRight?(new MailboxFolderAccessRight(permission));
			}
			else if (MailboxFolderAccessRight.RoleOptionalMap.Value.ContainsKey(permission))
			{
				role = new MailboxFolderAccessRight?(new MailboxFolderAccessRight(MailboxFolderAccessRight.RoleOptionalMap.Value[permission]));
			}
			return role != null;
		}

		internal static int CalculateMemberRights(IEnumerable<MailboxFolderAccessRight> accessRights, bool isCalendarFolder)
		{
			int num = 0;
			if (accessRights != null)
			{
				bool flag = isCalendarFolder && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.IncludeFBOnlyForCalendarContributor.Enabled;
				foreach (MailboxFolderAccessRight accessRight in accessRights)
				{
					if (isCalendarFolder && accessRight.IsRole && (int)accessRight != 2048 && (int)accessRight != 6144)
					{
						if (flag && (int)accessRight == 1026)
						{
							num |= ((int)accessRight | 2048);
						}
						else
						{
							num |= ((int)accessRight | 6144);
						}
					}
					else
					{
						num |= (int)accessRight;
					}
				}
			}
			return num;
		}

		public override string ToString()
		{
			if (!this.IsRole)
			{
				return ((MailboxFolderMemberRights)(this.permission & -6145)).ToString();
			}
			return ((MailboxFolderPermissionRole)this.permission).ToString();
		}

		public static explicit operator int(MailboxFolderAccessRight accessRight)
		{
			return accessRight.Permission;
		}

		public MailboxFolderAccessRight(string accessRight)
		{
			if (accessRight == null)
			{
				throw new ArgumentNullException("accessRight");
			}
			try
			{
				this.permission = (int)Enum.Parse(typeof(MailboxFolderMemberRights), accessRight, true);
			}
			catch (ArgumentException)
			{
				this.permission = (int)((MailboxFolderPermissionRole)Enum.Parse(typeof(MailboxFolderPermissionRole), accessRight, true));
			}
			if (!Enum.IsDefined(typeof(MailboxFolderMemberRights), this.permission) && !Enum.IsDefined(typeof(MailboxFolderPermissionRole), this.permission))
			{
				throw new ArgumentOutOfRangeException(Strings.ErrorUnknownMailboxFolderAccessRights(accessRight));
			}
		}

		public MailboxFolderAccessRight(MailboxFolderPermissionRole mailboxFolderPermissionRole)
		{
			this.permission = (int)mailboxFolderPermissionRole;
		}

		public static MailboxFolderAccessRight Parse(string accessRight)
		{
			return new MailboxFolderAccessRight(accessRight);
		}

		internal MailboxFolderAccessRight(int permission)
		{
			if (!Enum.IsDefined(typeof(MailboxFolderMemberRights), permission) && !Enum.IsDefined(typeof(MailboxFolderPermissionRole), permission))
			{
				throw new ArgumentOutOfRangeException(Strings.ErrorUnknownMailboxFolderAccessRights(permission.ToString()));
			}
			this.permission = permission;
		}

		public override int GetHashCode()
		{
			return this.permission.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is MailboxFolderAccessRight && this.permission == ((MailboxFolderAccessRight)obj).permission;
		}

		public bool Equals(MailboxFolderAccessRight other)
		{
			return this.permission == other.permission;
		}

		public static bool operator ==(MailboxFolderAccessRight operand1, MailboxFolderAccessRight operand2)
		{
			return operand1.permission == operand2.permission;
		}

		public static bool operator !=(MailboxFolderAccessRight operand1, MailboxFolderAccessRight operand2)
		{
			return operand1.permission != operand2.permission;
		}

		public int CompareTo(MailboxFolderAccessRight other)
		{
			return this.permission.CompareTo(other.permission);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is MailboxFolderAccessRight))
			{
				throw new ArgumentException(Strings.ErrorArgumentIsOfWrongType(typeof(MailboxFolderAccessRight).ToString()), "obj");
			}
			return this.permission.CompareTo(((MailboxFolderAccessRight)obj).permission);
		}

		private int permission;

		private static LazilyInitialized<Dictionary<int, int>> RoleOptionalMap = new LazilyInitialized<Dictionary<int, int>>(() => new Dictionary<int, int>
		{
			{
				3072,
				2048
			},
			{
				2560,
				2048
			},
			{
				3584,
				2048
			},
			{
				7168,
				6144
			},
			{
				6656,
				6144
			},
			{
				7680,
				6144
			},
			{
				1531,
				2043
			},
			{
				1787,
				1275
			},
			{
				1659,
				1147
			},
			{
				1691,
				1179
			},
			{
				1563,
				1051
			},
			{
				1555,
				1043
			},
			{
				1537,
				1025
			},
			{
				1538,
				1026
			}
		});
	}
}
