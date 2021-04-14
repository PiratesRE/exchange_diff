using System;
using System.Text;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public struct StorePropTag : IComparable<StorePropTag>, IEquatable<StorePropTag>
	{
		public StorePropTag(uint propTag, StorePropInfo propertyInfo, PropertyType externalType, ObjectType baseObjectType)
		{
			this = new StorePropTag(propTag, propertyInfo, externalType, baseObjectType, true);
		}

		public StorePropTag(uint propTag, StorePropInfo propertyInfo, ObjectType baseObjectType)
		{
			this = new StorePropTag(propTag, propertyInfo, (PropertyType)(propTag & 65535U), baseObjectType, true);
		}

		public StorePropTag(ushort propId, PropertyType propType, StorePropInfo propertyInfo, PropertyType externalType, ObjectType baseObjectType)
		{
			this = new StorePropTag(StorePropTag.BuildNumPropTag(propId, propType), propertyInfo, externalType, baseObjectType, true);
		}

		public StorePropTag(ushort propId, PropertyType propType, StorePropInfo propertyInfo, ObjectType baseObjectType)
		{
			this = new StorePropTag(StorePropTag.BuildNumPropTag(propId, propType), propertyInfo, propType, baseObjectType, true);
		}

		private StorePropTag(uint propTag, ObjectType baseObjectType)
		{
			this = new StorePropTag(propTag, null, (PropertyType)(propTag & 65535U), baseObjectType, false);
		}

		private StorePropTag(uint propTag, PropertyType externalType, ObjectType baseObjectType)
		{
			this = new StorePropTag(propTag, null, externalType, baseObjectType, false);
		}

		private StorePropTag(StorePropTag originalTag, PropertyType newPropType)
		{
			this = new StorePropTag(StorePropTag.BuildNumPropTag(originalTag.PropId, newPropType), originalTag.propertyInfo, originalTag.externalType, originalTag.baseObjectType, false);
		}

		private StorePropTag(StorePropTag originalTag, ObjectType newBaseObjectType)
		{
			this = new StorePropTag(originalTag.PropTag, originalTag.propertyInfo, originalTag.externalType, newBaseObjectType, false);
		}

		private StorePropTag(uint propTag, StorePropInfo propertyInfo, PropertyType externalType, ObjectType baseObjectType, bool checkInfo)
		{
			this.propTag = propTag;
			this.externalType = externalType;
			this.baseObjectType = baseObjectType;
			this.propertyInfo = propertyInfo;
		}

		public uint PropTag
		{
			get
			{
				return this.propTag;
			}
		}

		public ushort PropId
		{
			get
			{
				return (ushort)(this.propTag >> 16);
			}
		}

		public PropertyType PropType
		{
			get
			{
				return (PropertyType)(this.propTag & 65535U);
			}
		}

		public PropertyType ExternalType
		{
			get
			{
				return this.externalType;
			}
		}

		public uint ExternalTag
		{
			get
			{
				return (uint)((int)this.PropId << 16 | (int)this.externalType);
			}
		}

		public bool IsNamedProperty
		{
			get
			{
				return this.propTag >= 2147483648U;
			}
		}

		public bool IsMultiValued
		{
			get
			{
				return 4096 == (ushort)(this.PropType & (PropertyType)12288);
			}
		}

		public bool IsMultiValueInstance
		{
			get
			{
				return 12288 == (ushort)(this.PropType & (PropertyType)12288);
			}
		}

		public ulong GroupMask
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return 9223372036854775808UL;
				}
				return this.propertyInfo.GroupMask;
			}
		}

		public string DescriptiveName
		{
			get
			{
				if (this.propertyInfo == null)
				{
					return null;
				}
				return this.propertyInfo.DescriptiveName;
			}
		}

		public StorePropName PropName
		{
			get
			{
				if (this.propertyInfo != null)
				{
					return this.propertyInfo.PropName;
				}
				if (this.IsNamedProperty)
				{
					return StorePropName.Invalid;
				}
				return new StorePropName(StorePropName.UnnamedPropertyNamespaceGuid, (uint)this.PropId);
			}
		}

		public StorePropInfo PropInfo
		{
			get
			{
				return this.propertyInfo;
			}
		}

		public ObjectType BaseObjectType
		{
			get
			{
				return this.baseObjectType;
			}
		}

		public static StorePropTag CreateWithoutInfo(ushort propId, PropertyType propType)
		{
			return new StorePropTag(StorePropTag.BuildNumPropTag(propId, propType), ObjectType.Invalid);
		}

		public static StorePropTag CreateWithoutInfo(ushort propId, PropertyType propType, ObjectType baseObjectType)
		{
			return new StorePropTag(StorePropTag.BuildNumPropTag(propId, propType), baseObjectType);
		}

		public static StorePropTag CreateWithoutInfo(ushort propId, PropertyType propType, PropertyType externalPropertyType, ObjectType baseObjectType)
		{
			return new StorePropTag(StorePropTag.BuildNumPropTag(propId, propType), externalPropertyType, baseObjectType);
		}

		public static StorePropTag CreateWithoutInfo(uint propTag)
		{
			return new StorePropTag(propTag, ObjectType.Invalid);
		}

		public static StorePropTag CreateWithoutInfo(uint propTag, ObjectType baseObjectType)
		{
			return new StorePropTag(propTag, baseObjectType);
		}

		public static uint BuildNumPropTag(ushort propId, PropertyType propType)
		{
			return (uint)((int)propId << 16 | (int)propType);
		}

		public static bool operator ==(StorePropTag tag1, StorePropTag tag2)
		{
			return tag1.Equals(tag2);
		}

		public static bool operator !=(StorePropTag tag1, StorePropTag tag2)
		{
			return !tag1.Equals(tag2);
		}

		[Obsolete]
		public static bool operator ==(StorePropTag tag1, object tag2)
		{
			throw new InvalidOperationException();
		}

		[Obsolete]
		public static bool operator !=(StorePropTag tag1, object tag2)
		{
			throw new InvalidOperationException();
		}

		[Obsolete]
		public static bool operator ==(object tag1, StorePropTag tag2)
		{
			throw new InvalidOperationException();
		}

		[Obsolete]
		public static bool operator !=(object tag1, StorePropTag tag2)
		{
			throw new InvalidOperationException();
		}

		public bool IsCategory(int category)
		{
			return this.propertyInfo != null && this.propertyInfo.IsCategory(category);
		}

		public StorePropTag ChangeType(PropertyType propType)
		{
			return new StorePropTag(this, propType);
		}

		public StorePropTag ChangeBaseObjectTypeForTest(ObjectType newBaseObjectType)
		{
			return new StorePropTag(this, newBaseObjectType);
		}

		public StorePropTag ConvertToError()
		{
			return this.ChangeType(PropertyType.Error);
		}

		public bool Equals(StorePropTag other)
		{
			return this.PropTag == other.PropTag;
		}

		public override bool Equals(object other)
		{
			return other is StorePropTag && this.Equals((StorePropTag)other);
		}

		public int CompareTo(StorePropTag other)
		{
			return this.PropTag.CompareTo(other.PropTag);
		}

		public override int GetHashCode()
		{
			return (int)this.PropTag;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		public void AppendToString(StringBuilder sb)
		{
			this.AppendToString(sb, false);
		}

		public void AppendToString(StringBuilder sb, bool includeDetails)
		{
			if (this.propertyInfo != null && this.propertyInfo.DescriptiveName != null)
			{
				sb.Append(this.propertyInfo.DescriptiveName);
				if (includeDetails || this.IsNamedProperty)
				{
					sb.Append(":");
				}
			}
			if (includeDetails || this.propertyInfo == null || this.propertyInfo.DescriptiveName == null || this.IsNamedProperty)
			{
				sb.Append(this.PropId.ToString("X4"));
				if (includeDetails || this.propertyInfo == null || this.propertyInfo.DescriptiveName == null)
				{
					sb.Append(":");
					sb.Append(PropertyTypeHelper.PropertyTypeToString(this.PropType));
				}
			}
			if (includeDetails && this.propertyInfo != null && this.IsNamedProperty)
			{
				sb.Append("(");
				this.propertyInfo.PropName.AppendToString(sb);
				sb.Append(")");
			}
		}

		public static readonly StorePropTag Invalid = default(StorePropTag);

		private readonly uint propTag;

		private readonly PropertyType externalType;

		private readonly ObjectType baseObjectType;

		private readonly StorePropInfo propertyInfo;
	}
}
