using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[Serializable]
	public struct PublicFolderAccessRight : IEquatable<PublicFolderAccessRight>, IComparable, IComparable<PublicFolderAccessRight>
	{
		public PublicFolderPermission Permission
		{
			get
			{
				return this.permission;
			}
		}

		public bool IsRole
		{
			get
			{
				return Enum.IsDefined(typeof(PublicFolderPermissionRole), (int)this.permission);
			}
		}

		public static MultiValuedProperty<PublicFolderAccessRight> CreatePublicFolderAccessRightCollection(PublicFolderPermission permission)
		{
			permission &= (PublicFolderPermission.ReadItems | PublicFolderPermission.CreateItems | PublicFolderPermission.EditOwnedItems | PublicFolderPermission.DeleteOwnedItems | PublicFolderPermission.EditAllItems | PublicFolderPermission.DeleteAllItems | PublicFolderPermission.CreateSubfolders | PublicFolderPermission.FolderOwner | PublicFolderPermission.FolderContact | PublicFolderPermission.FolderVisible);
			MultiValuedProperty<PublicFolderAccessRight> multiValuedProperty = new MultiValuedProperty<PublicFolderAccessRight>();
			if (Enum.IsDefined(typeof(PublicFolderPermission), permission) || Enum.IsDefined(typeof(PublicFolderPermissionRole), (int)permission))
			{
				multiValuedProperty.Add(new PublicFolderAccessRight(permission));
			}
			else
			{
				int num = (int)permission;
				int num2 = 1;
				int num3 = 0;
				while (24 > num3)
				{
					if ((num2 & num) != 0 && Enum.IsDefined(typeof(PublicFolderPermission), num2))
					{
						multiValuedProperty.Add(new PublicFolderAccessRight((PublicFolderPermission)num2));
					}
					num2 <<= 1;
					num3++;
				}
			}
			return multiValuedProperty;
		}

		public static PublicFolderPermission CalculatePublicFolderPermission(ICollection<PublicFolderAccessRight> accessRights)
		{
			int num = 0;
			if (accessRights != null)
			{
				foreach (PublicFolderAccessRight accessRight in accessRights)
				{
					num |= (int)accessRight;
				}
			}
			return (PublicFolderPermission)num;
		}

		public override string ToString()
		{
			if (!this.IsRole)
			{
				return this.permission.ToString();
			}
			return ((PublicFolderPermissionRole)this.permission).ToString();
		}

		public static explicit operator int(PublicFolderAccessRight accessRight)
		{
			return (int)accessRight.Permission;
		}

		public PublicFolderAccessRight(string accessRight)
		{
			if (accessRight == null)
			{
				throw new ArgumentNullException("accessRight");
			}
			try
			{
				this.permission = (PublicFolderPermission)Enum.Parse(typeof(PublicFolderPermission), accessRight, true);
			}
			catch (ArgumentException)
			{
				this.permission = (PublicFolderPermission)((PublicFolderPermissionRole)Enum.Parse(typeof(PublicFolderPermissionRole), accessRight, true));
			}
			if (!Enum.IsDefined(typeof(PublicFolderPermission), this.permission) && !Enum.IsDefined(typeof(PublicFolderPermissionRole), (int)this.permission))
			{
				throw new ArgumentOutOfRangeException(Strings.ErrorUnknownAccessRights(accessRight));
			}
		}

		public static PublicFolderAccessRight Parse(string accessRight)
		{
			return new PublicFolderAccessRight(accessRight);
		}

		public PublicFolderAccessRight(PublicFolderPermission permission)
		{
			if (!Enum.IsDefined(typeof(PublicFolderPermission), permission) && !Enum.IsDefined(typeof(PublicFolderPermissionRole), (int)permission))
			{
				throw new ArgumentOutOfRangeException(Strings.ErrorUnknownAccessRights(permission.ToString()));
			}
			this.permission = permission;
		}

		public override int GetHashCode()
		{
			return this.permission.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is PublicFolderAccessRight && this.permission == ((PublicFolderAccessRight)obj).permission;
		}

		public bool Equals(PublicFolderAccessRight other)
		{
			return this.permission == other.permission;
		}

		public static bool operator ==(PublicFolderAccessRight operand1, PublicFolderAccessRight operand2)
		{
			return operand1.permission == operand2.permission;
		}

		public static bool operator !=(PublicFolderAccessRight operand1, PublicFolderAccessRight operand2)
		{
			return operand1.permission != operand2.permission;
		}

		public int CompareTo(PublicFolderAccessRight other)
		{
			return this.permission.CompareTo(other.permission);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is PublicFolderAccessRight))
			{
				throw new ArgumentException(Strings.ErrorArgumentIsOfWrongType(typeof(PublicFolderAccessRight).ToString()), "obj");
			}
			return this.permission.CompareTo(((PublicFolderAccessRight)obj).permission);
		}

		private PublicFolderPermission permission;
	}
}
