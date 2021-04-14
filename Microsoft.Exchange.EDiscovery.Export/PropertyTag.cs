using System;
using System.Globalization;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal struct PropertyTag
	{
		public uint Value
		{
			get
			{
				return this.tagValue;
			}
			set
			{
				this.tagValue = value;
			}
		}

		public uint NormalizedValueForPst
		{
			get
			{
				PropertyTag.PropertyType propertyType = PropertyTag.PropertyType.Multivalued & this.Type;
				if ((ushort)(this.Type & PropertyTag.PropertyType.Unicode) == 33968)
				{
					propertyType |= PropertyTag.PropertyType.String;
				}
				else if ((ushort)(this.Type & PropertyTag.PropertyType.Ascii) == 34020)
				{
					propertyType |= PropertyTag.PropertyType.AnsiString;
				}
				else
				{
					propertyType = this.Type;
				}
				return (uint)((int)this.Id << 16 | (int)propertyType);
			}
		}

		public ushort Id
		{
			get
			{
				return (ushort)((this.Value & 4294901760U) >> 16);
			}
		}

		public PropertyTag.PropertyType Type
		{
			get
			{
				return (PropertyTag.PropertyType)(this.Value & 65535U);
			}
		}

		public bool IsFixedSize
		{
			get
			{
				PropertyTag.PropertyType propertyType = this.Type & ~PropertyTag.PropertyType.Multivalued;
				switch (propertyType)
				{
				case PropertyTag.PropertyType.Unspecified:
				case PropertyTag.PropertyType.Null:
				case PropertyTag.PropertyType.Int16:
				case PropertyTag.PropertyType.Int32:
				case PropertyTag.PropertyType.Float:
				case PropertyTag.PropertyType.Double:
				case PropertyTag.PropertyType.Currency:
				case PropertyTag.PropertyType.AppTime:
				case PropertyTag.PropertyType.Error:
				case PropertyTag.PropertyType.Boolean:
				case PropertyTag.PropertyType.Int64:
					break;
				case (PropertyTag.PropertyType)8:
				case (PropertyTag.PropertyType)9:
				case (PropertyTag.PropertyType)12:
				case PropertyTag.PropertyType.Object:
				case (PropertyTag.PropertyType)14:
				case (PropertyTag.PropertyType)15:
				case (PropertyTag.PropertyType)16:
				case (PropertyTag.PropertyType)17:
				case (PropertyTag.PropertyType)18:
				case (PropertyTag.PropertyType)19:
					return false;
				default:
					if (propertyType != PropertyTag.PropertyType.SysTime && propertyType != PropertyTag.PropertyType.ClassId)
					{
						return false;
					}
					break;
				}
				return true;
			}
		}

		public bool IsMultivalued
		{
			get
			{
				return (ushort)(this.Type & PropertyTag.PropertyType.Multivalued) == 4096;
			}
		}

		public bool IsNamedProperty
		{
			get
			{
				return this.Id <= 65534 && this.Id >= 32768;
			}
		}

		public static int GetSizeOfFixedSizeProperty(PropertyTag.PropertyType propType)
		{
			PropertyTag.PropertyType propertyType = propType & ~PropertyTag.PropertyType.Multivalued;
			switch (propertyType)
			{
			case PropertyTag.PropertyType.Unspecified:
			case PropertyTag.PropertyType.Null:
				return 0;
			case PropertyTag.PropertyType.Int16:
			case PropertyTag.PropertyType.Boolean:
				return 2;
			case PropertyTag.PropertyType.Int32:
			case PropertyTag.PropertyType.Float:
			case PropertyTag.PropertyType.Error:
				return 4;
			case PropertyTag.PropertyType.Double:
			case PropertyTag.PropertyType.Currency:
			case PropertyTag.PropertyType.AppTime:
			case PropertyTag.PropertyType.Int64:
				break;
			case (PropertyTag.PropertyType)8:
			case (PropertyTag.PropertyType)9:
			case (PropertyTag.PropertyType)12:
			case PropertyTag.PropertyType.Object:
			case (PropertyTag.PropertyType)14:
			case (PropertyTag.PropertyType)15:
			case (PropertyTag.PropertyType)16:
			case (PropertyTag.PropertyType)17:
			case (PropertyTag.PropertyType)18:
			case (PropertyTag.PropertyType)19:
				goto IL_7A;
			default:
				if (propertyType != PropertyTag.PropertyType.SysTime)
				{
					if (propertyType != PropertyTag.PropertyType.ClassId)
					{
						goto IL_7A;
					}
					return 16;
				}
				break;
			}
			return 8;
			IL_7A:
			throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "Mapi property type '{0}' is not a supported fixed size property", new object[]
			{
				propType
			}), null);
		}

		public static implicit operator PropertyTag(uint tagValue)
		{
			return new PropertyTag
			{
				Value = tagValue
			};
		}

		private const ushort MinUserDefinedNamed = 32768;

		private const ushort MaxUserDefinedNamed = 65534;

		public static readonly PropertyTag DisplayName = 805371935U;

		public static readonly PropertyTag DisplayTo = 235143199U;

		public static readonly PropertyTag DisplayCc = 235077663U;

		public static readonly PropertyTag DisplayBcc = 235012127U;

		public static readonly PropertyTag Importance = 1507331U;

		public static readonly PropertyTag MessageFlags = 235339779U;

		public static readonly PropertyTag RecipientType = 202702851U;

		private uint tagValue;

		public enum ContextPropertyId : ushort
		{
			PropTagNewAttachment = 16384,
			PropTagStartEmbed,
			PropTagEndEmbed,
			PropTagStartRecipient,
			PropTagEndRecipient,
			PropTagEndAttachment = 16398,
			PropTagFastTransferDel = 16406
		}

		[Flags]
		public enum PropertyType : ushort
		{
			Unspecified = 0,
			Null = 1,
			Int16 = 2,
			Int32 = 3,
			Float = 4,
			Double = 5,
			Currency = 6,
			AppTime = 7,
			Error = 10,
			Boolean = 11,
			Object = 13,
			Int64 = 20,
			AnsiString = 30,
			String = 31,
			SysTime = 64,
			ClassId = 72,
			ServerEntryId = 251,
			Restriction = 253,
			Actions = 254,
			Binary = 258,
			Multivalued = 4096,
			Unicode = 33968,
			Ascii = 34020
		}
	}
}
