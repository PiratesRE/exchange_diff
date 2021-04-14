using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "PathToExtendedFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class ExtendedPropertyUri : PropertyPath
	{
		static ExtendedPropertyUri()
		{
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDMeeting, DistinguishedPropertySet.Meeting, ExchangeVersion.Exchange2007);
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDAppointment, DistinguishedPropertySet.Appointment, ExchangeVersion.Exchange2007);
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDCommon, DistinguishedPropertySet.Common, ExchangeVersion.Exchange2007);
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDPublicStrings, DistinguishedPropertySet.PublicStrings, ExchangeVersion.Exchange2007);
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDAddress, DistinguishedPropertySet.Address, ExchangeVersion.Exchange2007);
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDInternetHeaders, DistinguishedPropertySet.InternetHeaders, ExchangeVersion.Exchange2007);
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDCalendarAssistant, DistinguishedPropertySet.CalendarAssistant, ExchangeVersion.Exchange2007);
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDUnifiedMessaging, DistinguishedPropertySet.UnifiedMessaging, ExchangeVersion.Exchange2007);
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDTask, DistinguishedPropertySet.Task, ExchangeVersion.Exchange2007SP1);
			ExtendedPropertyUri.AddPropertySetEntry(ExtendedPropertyUri.PSETIDSharing, DistinguishedPropertySet.Sharing, ExchangeVersion.Exchange2012);
			ExtendedPropertyUri.mapiTypeToTypeMap = new Dictionary<MapiPropertyType, Type>();
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.ApplicationTime, typeof(double));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.ApplicationTimeArray, typeof(double[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.Binary, typeof(byte[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.BinaryArray, typeof(byte[][]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.Boolean, typeof(bool));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.CLSID, typeof(Guid));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.CLSIDArray, typeof(Guid[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.Currency, typeof(long));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.CurrencyArray, typeof(long[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.Double, typeof(double));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.DoubleArray, typeof(double[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.Float, typeof(float));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.FloatArray, typeof(float[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.Integer, typeof(int));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.IntegerArray, typeof(int[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.Long, typeof(long));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.LongArray, typeof(long[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.Short, typeof(short));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.ShortArray, typeof(short[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.SystemTime, typeof(ExDateTime));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.SystemTimeArray, typeof(ExDateTime[]));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.String, typeof(string));
			ExtendedPropertyUri.AddMapiTypeEntry(MapiPropertyType.StringArray, typeof(string[]));
		}

		private static void AddMapiTypeEntry(MapiPropertyType mapiType, Type dotNetType)
		{
			ExtendedPropertyUri.mapiTypeToTypeMap.Add(mapiType, dotNetType);
		}

		private static void AddPropertySetEntry(Guid guid, DistinguishedPropertySet propertySet, ExchangeVersion minimumSupportedVersion)
		{
			ExtendedPropertyUri.guidToDistinguishedMap.Add(guid, new ExtendedPropertyUri.DistinguishedPropertySetInformation(propertySet, minimumSupportedVersion));
			ExtendedPropertyUri.distinguishedToGuidMap.Add(propertySet, guid);
		}

		public ExtendedPropertyUri()
		{
		}

		internal ExtendedPropertyUri(NativeStorePropertyDefinition propertyDefinition)
		{
			this.ResetSpecified();
			GuidIdPropertyDefinition guidIdPropertyDefinition = propertyDefinition as GuidIdPropertyDefinition;
			if (guidIdPropertyDefinition != null)
			{
				this.Initialize(guidIdPropertyDefinition);
			}
			GuidNamePropertyDefinition guidNamePropertyDefinition = propertyDefinition as GuidNamePropertyDefinition;
			if (guidNamePropertyDefinition != null)
			{
				this.Initialize(guidNamePropertyDefinition);
			}
			PropertyTagPropertyDefinition propertyTagPropertyDefinition = propertyDefinition as PropertyTagPropertyDefinition;
			if (propertyTagPropertyDefinition != null)
			{
				this.Initialize(propertyTagPropertyDefinition);
			}
			this.ConfigureCommonProperties(propertyDefinition);
			this.Classify();
		}

		private void Initialize(PropertyTagPropertyDefinition propertyDefinition)
		{
			this.BreakApartPropertyTag(propertyDefinition);
		}

		private void Initialize(GuidNamePropertyDefinition propertyDefinition)
		{
			this.ExtractPropertySet(propertyDefinition.Guid);
			this.PropertyName = propertyDefinition.PropertyName;
		}

		private void Initialize(GuidIdPropertyDefinition propertyDefinition)
		{
			this.ExtractPropertySet(propertyDefinition.Guid);
			this.PropertyId = propertyDefinition.Id;
		}

		[IgnoreDataMember]
		[XmlAttribute(AttributeName = "DistinguishedPropertySetId")]
		public DistinguishedPropertySet DistinguishedPropertySetId
		{
			get
			{
				return this.distinguishedPropertySet;
			}
			set
			{
				this.distinguishedPropertySetSpecified = true;
				this.distinguishedPropertySet = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "DistinguishedPropertySetId", IsRequired = false, EmitDefaultValue = false)]
		public string DistinguishedPropertySetIdString
		{
			get
			{
				if (this.DistinguishedPropertySetIdSpecified)
				{
					return EnumUtilities.ToString<DistinguishedPropertySet>(this.DistinguishedPropertySetId);
				}
				return null;
			}
			set
			{
				this.DistinguishedPropertySetId = EnumUtilities.Parse<DistinguishedPropertySet>(value);
			}
		}

		[DataMember(Name = "PropertySetId", IsRequired = false, EmitDefaultValue = false)]
		[XmlAttribute("PropertySetId")]
		public string PropertySetId
		{
			get
			{
				if (!(this.propertySetId == Guid.Empty))
				{
					return this.propertySetId.ToString("D");
				}
				return null;
			}
			set
			{
				this.propertySetId = ((value == null) ? Guid.Empty : new Guid(value));
			}
		}

		[DataMember(Name = "PropertyTag", IsRequired = false, EmitDefaultValue = false)]
		[XmlAttribute("PropertyTag")]
		public string PropertyTag
		{
			get
			{
				if (!this.propertyTagSpecified)
				{
					return null;
				}
				return "0x" + this.propertyTagId.ToString("x", CultureInfo.InvariantCulture);
			}
			set
			{
				NumberStyles style = NumberStyles.Integer;
				string s;
				if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				{
					s = value.Replace("0x", string.Empty).Replace("0X", string.Empty);
					style = NumberStyles.HexNumber;
				}
				else
				{
					s = value;
				}
				this.propertyTagId = ushort.Parse(s, style, CultureInfo.InvariantCulture);
				this.propertyTagSpecified = true;
			}
		}

		[DataMember(Name = "PropertyName", IsRequired = false, EmitDefaultValue = false)]
		[XmlAttribute("PropertyName")]
		public string PropertyName { get; set; }

		[DataMember(Name = "PropertyId", IsRequired = false, EmitDefaultValue = false)]
		[XmlAttribute("PropertyId")]
		public int PropertyId
		{
			get
			{
				return this.propertyId;
			}
			set
			{
				this.propertyIdSpecified = true;
				this.propertyId = value;
			}
		}

		[XmlAttribute("PropertyType")]
		[IgnoreDataMember]
		public MapiPropertyType PropertyType { get; set; }

		[XmlIgnore]
		[DataMember(Name = "PropertyType")]
		public string PropertyTypeString
		{
			get
			{
				return EnumUtilities.ToString<MapiPropertyType>(this.PropertyType);
			}
			set
			{
				this.PropertyType = EnumUtilities.Parse<MapiPropertyType>(value);
			}
		}

		internal Guid PropertySetIdGuid
		{
			get
			{
				return this.propertySetId;
			}
		}

		internal ushort PropertyTagId
		{
			get
			{
				return this.propertyTagId;
			}
		}

		internal static bool IsRequestableType(MapiPropertyType propertyType)
		{
			return propertyType != MapiPropertyType.Error && propertyType != MapiPropertyType.Null && propertyType != MapiPropertyType.Object && propertyType != MapiPropertyType.ObjectArray;
		}

		private Guid GetEffectivePropertySet()
		{
			if (this.propertySetId != Guid.Empty)
			{
				return this.propertySetId;
			}
			return ExtendedPropertyUri.GuidForDistinguishedPropertySet(this.distinguishedPropertySet);
		}

		internal NativeStorePropertyDefinition ToPropertyDefinition()
		{
			if (!ExtendedPropertyUri.IsRequestableType(this.PropertyType))
			{
				throw new UnsupportedMapiPropertyTypeException();
			}
			ExtendedPropertyClassification extendedPropertyClassification = this.Classify();
			NativeStorePropertyDefinition result;
			try
			{
				switch (extendedPropertyClassification)
				{
				case ExtendedPropertyClassification.PropertyTag:
				{
					uint num = this.ConstructPropertyTag();
					if (num >= 2147483648U)
					{
						throw new NoPropertyTagForCustomPropertyException();
					}
					result = PropertyTagPropertyDefinition.CreateCustom("PropTag_" + num.ToString("X", CultureInfo.InvariantCulture), num);
					break;
				}
				case ExtendedPropertyClassification.GuidId:
					result = GuidIdPropertyDefinition.CreateCustom("GuidId_" + this.PropertyId.ToString(CultureInfo.InvariantCulture), this.GetTypeForMapiPropertyType(), this.GetEffectivePropertySet(), this.propertyId, PropertyFlags.None);
					break;
				case ExtendedPropertyClassification.GuidName:
					result = GuidNamePropertyDefinition.CreateCustom("GuidName_" + this.PropertyName, this.GetTypeForMapiPropertyType(), this.GetEffectivePropertySet(), this.PropertyName, PropertyFlags.None);
					break;
				default:
					result = null;
					break;
				}
			}
			catch (InvalidPropertyTypeException innerException)
			{
				throw new InvalidExtendedPropertyException(this, innerException);
			}
			catch (ArgumentException innerException2)
			{
				throw new InvalidExtendedPropertyException(this, innerException2);
			}
			return result;
		}

		public static bool AreEqual(ExtendedPropertyUri first, ExtendedPropertyUri second)
		{
			if (object.ReferenceEquals(first, second))
			{
				return true;
			}
			if (first == null || second == null)
			{
				return false;
			}
			ExtendedPropertyClassification extendedPropertyClassification;
			ExtendedPropertyClassification extendedPropertyClassification2;
			if (!first.TryClassify(out extendedPropertyClassification) || !second.TryClassify(out extendedPropertyClassification2))
			{
				return false;
			}
			if (extendedPropertyClassification != extendedPropertyClassification2)
			{
				return false;
			}
			switch (extendedPropertyClassification)
			{
			case ExtendedPropertyClassification.PropertyTag:
				return first.PropertyTagId == second.PropertyTagId;
			case ExtendedPropertyClassification.GuidId:
				return first.PropertyId == second.PropertyId && first.GetEffectivePropertySet().Equals(second.GetEffectivePropertySet());
			case ExtendedPropertyClassification.GuidName:
				return first.PropertyName.Equals(second.PropertyName) && first.GetEffectivePropertySet().Equals(second.GetEffectivePropertySet());
			default:
				return false;
			}
		}

		private void BreakApartPropertyTag(PropertyTagPropertyDefinition propertyDefinition)
		{
			if (propertyDefinition.PropertyTag >= 2147483648U)
			{
				throw new UnsupportedPropertyDefinitionException(propertyDefinition.Name);
			}
			ushort num = (ushort)((propertyDefinition.PropertyTag & 4294901760U) >> 16);
			ushort num2 = (ushort)(propertyDefinition.PropertyTag & 65535U);
			if (!EnumValidator.IsValidEnum<MapiPropertyType>((MapiPropertyType)num2))
			{
				throw new UnsupportedMapiPropertyTypeException();
			}
			this.PropertyType = (MapiPropertyType)num2;
			this.propertyTagId = num;
			this.propertyTagSpecified = true;
		}

		private uint ConstructPropertyTag()
		{
			return (uint)(((int)this.propertyTagId << 16) + this.PropertyType);
		}

		private static Guid GuidForDistinguishedPropertySet(DistinguishedPropertySet distinguishedPropertySet)
		{
			return ExtendedPropertyUri.distinguishedToGuidMap[distinguishedPropertySet];
		}

		private static bool TryGetDistinguishedPropertySetForGuid(Guid guid, out DistinguishedPropertySet distinguishedPropertySetToReturn)
		{
			distinguishedPropertySetToReturn = DistinguishedPropertySet.Meeting;
			ExtendedPropertyUri.DistinguishedPropertySetInformation distinguishedPropertySetInformation;
			if (!ExtendedPropertyUri.guidToDistinguishedMap.TryGetValue(guid, out distinguishedPropertySetInformation))
			{
				return false;
			}
			if (!ExchangeVersion.Current.Supports(distinguishedPropertySetInformation.MinimumSupportedVersion))
			{
				return false;
			}
			distinguishedPropertySetToReturn = distinguishedPropertySetInformation.DistinguishedPropertySet;
			return true;
		}

		private Type GetTypeForMapiPropertyType()
		{
			return ExtendedPropertyUri.mapiTypeToTypeMap[this.PropertyType];
		}

		private ExtendedPropertyClassification Classify()
		{
			ExtendedPropertyClassification result;
			if (!this.TryClassify(out result))
			{
				throw new InvalidExtendedPropertyException(this);
			}
			return result;
		}

		private bool TryClassify(out ExtendedPropertyClassification classification)
		{
			classification = ExtendedPropertyClassification.PropertyTag;
			if (this.distinguishedPropertySetSpecified || this.propertySetId != Guid.Empty)
			{
				if (this.distinguishedPropertySetSpecified && this.propertySetId != Guid.Empty)
				{
					return false;
				}
				if (this.propertyTagSpecified)
				{
					return false;
				}
				if (!string.IsNullOrEmpty(this.PropertyName) && this.PropertyIdSpecified)
				{
					return false;
				}
				if (string.IsNullOrEmpty(this.PropertyName) && !this.PropertyIdSpecified)
				{
					return false;
				}
				classification = (this.PropertyIdSpecified ? ExtendedPropertyClassification.GuidId : ExtendedPropertyClassification.GuidName);
			}
			else
			{
				if (!this.propertyTagSpecified)
				{
					return false;
				}
				if (!string.IsNullOrEmpty(this.PropertyName) || this.PropertyIdSpecified)
				{
					return false;
				}
				classification = ExtendedPropertyClassification.PropertyTag;
			}
			return true;
		}

		private void ConfigureCommonProperties(NativeStorePropertyDefinition propertyDefinition)
		{
			MapiPropertyType mapiPropertyType = (MapiPropertyType)propertyDefinition.MapiPropertyType;
			if (!EnumValidator.IsValidEnum<MapiPropertyType>(mapiPropertyType))
			{
				throw new UnsupportedMapiPropertyTypeException();
			}
			this.PropertyType = mapiPropertyType;
		}

		private void ExtractPropertySet(Guid guid)
		{
			DistinguishedPropertySet distinguishedPropertySetId;
			if (ExtendedPropertyUri.TryGetDistinguishedPropertySetForGuid(guid, out distinguishedPropertySetId))
			{
				this.DistinguishedPropertySetId = distinguishedPropertySetId;
				return;
			}
			this.propertySetId = guid;
		}

		private void ResetSpecified()
		{
			this.distinguishedPropertySetSpecified = false;
			this.propertyTagSpecified = false;
			this.propertyIdSpecified = false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.distinguishedPropertySetSpecified)
			{
				stringBuilder.AppendFormat("DistinguishedPropertySetId = {0}, ", this.distinguishedPropertySet);
			}
			if (this.propertySetId != Guid.Empty)
			{
				stringBuilder.AppendFormat("PropertySetId = {0}, ", this.PropertySetId);
			}
			if (this.propertyIdSpecified)
			{
				stringBuilder.AppendFormat("PropertyId = {0}, ", this.propertyId);
			}
			if (!string.IsNullOrEmpty(this.PropertyName))
			{
				stringBuilder.AppendFormat("PropertyName = {0}, ", this.PropertyName);
			}
			if (this.propertyTagSpecified)
			{
				stringBuilder.AppendFormat("PropertyTag = {0}, ", this.PropertyTag);
			}
			if (stringBuilder.Length == 0)
			{
				stringBuilder.Append("[Missing all optional attributes], ");
			}
			stringBuilder.Append("PropertyType = " + this.PropertyType);
			return stringBuilder.ToString();
		}

		[XmlIgnore]
		public bool DistinguishedPropertySetIdSpecified
		{
			get
			{
				return this.distinguishedPropertySetSpecified;
			}
			set
			{
				this.distinguishedPropertySetSpecified = value;
			}
		}

		[XmlIgnore]
		public bool PropertyIdSpecified
		{
			get
			{
				return this.propertyIdSpecified;
			}
			set
			{
				this.propertyIdSpecified = value;
			}
		}

		internal static bool IsExtendedPropertyUriXml(XmlElement extendedPropertyXml)
		{
			return extendedPropertyXml.LocalName == "ExtendedFieldURI";
		}

		internal new static ExtendedPropertyUri Parse(XmlElement extendedPropertyXml)
		{
			ExtendedPropertyUri extendedPropertyUri = new ExtendedPropertyUri();
			try
			{
				foreach (object obj in extendedPropertyXml.Attributes)
				{
					XmlAttribute xmlAttribute = (XmlAttribute)obj;
					if (xmlAttribute.LocalName == "DistinguishedPropertySetId")
					{
						extendedPropertyUri.DistinguishedPropertySetId = (DistinguishedPropertySet)Enum.Parse(typeof(DistinguishedPropertySet), xmlAttribute.Value);
					}
					else if (xmlAttribute.LocalName == "PropertyId")
					{
						extendedPropertyUri.PropertyId = int.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
					}
					else if (xmlAttribute.LocalName == "PropertyName")
					{
						extendedPropertyUri.PropertyName = xmlAttribute.Value;
					}
					else if (xmlAttribute.LocalName == "PropertySetId")
					{
						extendedPropertyUri.PropertySetId = xmlAttribute.Value;
					}
					else if (xmlAttribute.LocalName == "PropertyTag")
					{
						extendedPropertyUri.PropertyTag = xmlAttribute.Value;
					}
					else if (xmlAttribute.LocalName == "PropertyType")
					{
						extendedPropertyUri.PropertyType = (MapiPropertyType)Enum.Parse(typeof(MapiPropertyType), xmlAttribute.Value);
					}
				}
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidExtendedPropertyException(extendedPropertyUri, innerException);
			}
			extendedPropertyUri.Classify();
			return extendedPropertyUri;
		}

		internal override XmlElement ToXml(XmlElement parentElement)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(parentElement, "ExtendedFieldURI", "http://schemas.microsoft.com/exchange/services/2006/types");
			ExtendedPropertyClassification extendedPropertyClassification = this.Classify();
			switch (extendedPropertyClassification)
			{
			case ExtendedPropertyClassification.PropertyTag:
				ServiceXml.CreateAttribute(xmlElement, "PropertyTag", this.PropertyTag);
				break;
			case ExtendedPropertyClassification.GuidId:
			case ExtendedPropertyClassification.GuidName:
			{
				DistinguishedPropertySet distinguishedPropertySet;
				if (ExtendedPropertyUri.TryGetDistinguishedPropertySetForGuid(this.GetEffectivePropertySet(), out distinguishedPropertySet))
				{
					ServiceXml.CreateAttribute(xmlElement, "DistinguishedPropertySetId", distinguishedPropertySet.ToString());
				}
				else
				{
					ServiceXml.CreateAttribute(xmlElement, "PropertySetId", this.propertySetId.ToString());
				}
				if (extendedPropertyClassification == ExtendedPropertyClassification.GuidId)
				{
					ServiceXml.CreateAttribute(xmlElement, "PropertyId", this.propertyId.ToString(CultureInfo.InvariantCulture));
				}
				else
				{
					ServiceXml.CreateAttribute(xmlElement, "PropertyName", this.PropertyName);
				}
				break;
			}
			}
			ServiceXml.CreateAttribute(xmlElement, "PropertyType", this.PropertyType.ToString());
			return xmlElement;
		}

		internal static readonly Guid PSETIDMeeting = new Guid("{6ED8DA90-450B-101B-98DA-00AA003F1305}");

		internal static readonly Guid PSETIDAppointment = new Guid("{00062002-0000-0000-C000-000000000046}");

		internal static readonly Guid PSETIDCommon = new Guid("{00062008-0000-0000-C000-000000000046}");

		internal static readonly Guid PSETIDPublicStrings = new Guid("{00020329-0000-0000-C000-000000000046}");

		internal static readonly Guid PSETIDAddress = new Guid("{00062004-0000-0000-C000-000000000046}");

		internal static readonly Guid PSETIDInternetHeaders = new Guid("{00020386-0000-0000-C000-000000000046}");

		internal static readonly Guid PSETIDCalendarAssistant = new Guid("{11000E07-B51B-40D6-AF21-CAA85EDAB1D0}");

		internal static readonly Guid PSETIDUnifiedMessaging = new Guid("{4442858E-A9E3-4E80-B900-317A210CC15B}");

		internal static readonly Guid PSETIDTask = new Guid("{00062003-0000-0000-C000-000000000046}");

		internal static readonly Guid PSETIDSharing = new Guid("{00062040-0000-0000-C000-000000000046}");

		internal static ExtendedPropertyUri Placeholder = new ExtendedPropertyUri();

		private static Dictionary<Guid, ExtendedPropertyUri.DistinguishedPropertySetInformation> guidToDistinguishedMap = new Dictionary<Guid, ExtendedPropertyUri.DistinguishedPropertySetInformation>();

		private static Dictionary<DistinguishedPropertySet, Guid> distinguishedToGuidMap = new Dictionary<DistinguishedPropertySet, Guid>();

		private static Dictionary<MapiPropertyType, Type> mapiTypeToTypeMap;

		private DistinguishedPropertySet distinguishedPropertySet;

		private bool distinguishedPropertySetSpecified;

		private Guid propertySetId;

		private ushort propertyTagId;

		private bool propertyTagSpecified;

		private int propertyId;

		private bool propertyIdSpecified;

		private class DistinguishedPropertySetInformation
		{
			internal DistinguishedPropertySetInformation(DistinguishedPropertySet distinguishedPropertySet, ExchangeVersion minimumSupportedVersion)
			{
				this.distinguishedPropertySet = distinguishedPropertySet;
				this.minimumSupportedVersion = minimumSupportedVersion;
			}

			internal DistinguishedPropertySet DistinguishedPropertySet
			{
				get
				{
					return this.distinguishedPropertySet;
				}
			}

			internal ExchangeVersion MinimumSupportedVersion
			{
				get
				{
					return this.minimumSupportedVersion;
				}
			}

			private DistinguishedPropertySet distinguishedPropertySet;

			private ExchangeVersion minimumSupportedVersion;
		}
	}
}
