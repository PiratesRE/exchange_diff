using System;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public class StorePropInfo
	{
		public StorePropInfo(string descriptiveName, ushort propId, PropertyType propType, StorePropInfo.Flags flags, ulong groupMask, PropertyCategories categories)
		{
			this.descriptiveName = descriptiveName;
			this.propId = propId;
			this.propType = propType;
			this.groupMask = groupMask;
			this.categories = categories;
			this.visibility = StorePropInfo.GetVisibility(flags);
		}

		public string DescriptiveName
		{
			get
			{
				return this.descriptiveName;
			}
		}

		public virtual ushort PropId
		{
			get
			{
				return this.propId;
			}
		}

		public PropertyType PropType
		{
			get
			{
				return this.propType;
			}
		}

		public ulong GroupMask
		{
			get
			{
				return this.groupMask;
			}
		}

		public bool IsNamedProperty
		{
			get
			{
				return this.propId >= 32768;
			}
		}

		public virtual StorePropName PropName
		{
			get
			{
				if (this.IsNamedProperty)
				{
					return StorePropName.Invalid;
				}
				return new StorePropName(StorePropName.UnnamedPropertyNamespaceGuid, (uint)this.propId);
			}
		}

		public Visibility Visibility
		{
			get
			{
				return this.visibility;
			}
		}

		public bool IsCategory(int category)
		{
			return this.categories.CheckCategory(category);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		public virtual void AppendToString(StringBuilder sb)
		{
			if (this.descriptiveName != null)
			{
				sb.Append(this.descriptiveName);
				return;
			}
			sb.Append("generic");
		}

		private static Visibility GetVisibility(StorePropInfo.Flags flags)
		{
			if ((byte)(flags & StorePropInfo.Flags.Private) == 1)
			{
				return Visibility.Private;
			}
			if ((byte)(flags & StorePropInfo.Flags.Redacted) == 2)
			{
				return Visibility.Redacted;
			}
			return Visibility.Public;
		}

		public const ulong OtherGroupMask = 9223372036854775808UL;

		private readonly string descriptiveName;

		private readonly ushort propId;

		private readonly PropertyType propType;

		private readonly ulong groupMask;

		private readonly PropertyCategories categories;

		private readonly Visibility visibility;

		[Flags]
		public enum Flags : byte
		{
			None = 0,
			Private = 1,
			Redacted = 2
		}
	}
}
