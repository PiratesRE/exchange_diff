using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "PathToUnindexedFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PropertyUri : PropertyPath
	{
		public PropertyUri()
		{
		}

		internal PropertyUri(PropertyUriEnum uri)
		{
			this.Uri = uri;
		}

		[XmlAttribute(AttributeName = "FieldURI", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[IgnoreDataMember]
		public PropertyUriEnum Uri { get; set; }

		[DataMember(Name = "FieldURI", IsRequired = true)]
		[XmlIgnore]
		public string UriString
		{
			get
			{
				return EnumUtilities.ToString<PropertyUriEnum>(this.Uri);
			}
			set
			{
				this.Uri = EnumUtilities.Parse<PropertyUriEnum>(value);
			}
		}

		public override string ToString()
		{
			return PropertyUriMapper.GetXmlEnumValue(this.Uri);
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			PropertyUri propertyUri = obj as PropertyUri;
			if (propertyUri != null)
			{
				result = (this.Uri == propertyUri.Uri);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.Uri.GetHashCode();
		}

		internal static bool IsPropertyUriXml(XmlElement propertyUriElement)
		{
			return propertyUriElement.LocalName == "FieldURI";
		}

		internal new static PropertyUri Parse(XmlElement propertyUriElement)
		{
			XmlAttribute xmlAttribute = (XmlAttribute)propertyUriElement.Attributes.GetNamedItem("FieldURI");
			PropertyUriEnum uri;
			PropertyUriMapper.TryGetPropertyUriEnum(xmlAttribute.Value, out uri);
			return new PropertyUri(uri);
		}

		internal override XmlElement ToXml(XmlElement parentElement)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(parentElement, "FieldURI", "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateAttribute(xmlElement, "FieldURI", this.ToString());
			return xmlElement;
		}
	}
}
