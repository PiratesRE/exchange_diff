using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "PathToIndexedFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class DictionaryPropertyUri : DictionaryPropertyUriBase
	{
		public DictionaryPropertyUri()
		{
		}

		internal DictionaryPropertyUri(DictionaryUriEnum fieldUri, string key) : base(fieldUri)
		{
			this.key = key;
		}

		[XmlAttribute(AttributeName = "FieldIndex", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember(Name = "FieldIndex", IsRequired = true)]
		public string Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		internal DictionaryPropertyUriBase GetDictionaryPropertyUriBase()
		{
			return new DictionaryPropertyUriBase(this.FieldUri);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				PropertyUriMapper.GetXmlEnumValue(this.FieldUri),
				this.key
			});
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			DictionaryPropertyUri dictionaryPropertyUri = obj as DictionaryPropertyUri;
			if (dictionaryPropertyUri != null)
			{
				result = (this.FieldUri == dictionaryPropertyUri.FieldUri && this.key == dictionaryPropertyUri.key);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.FieldUri.GetHashCode() + this.key.GetHashCode();
		}

		internal static bool IsDictionaryPropertyUriXml(XmlElement element)
		{
			return element.LocalName == "IndexedFieldURI";
		}

		internal new static DictionaryPropertyUri Parse(XmlElement dictionaryUriElement)
		{
			XmlAttribute xmlAttribute = (XmlAttribute)dictionaryUriElement.Attributes.GetNamedItem("FieldURI");
			XmlAttribute xmlAttribute2 = (XmlAttribute)dictionaryUriElement.Attributes.GetNamedItem("FieldIndex");
			DictionaryUriEnum fieldUri;
			PropertyUriMapper.TryGetDictionaryUriEnum(xmlAttribute.Value, out fieldUri);
			return new DictionaryPropertyUri(fieldUri, xmlAttribute2.Value);
		}

		internal override XmlElement ToXml(XmlElement parentElement)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(parentElement, "IndexedFieldURI", "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateAttribute(xmlElement, "FieldURI", PropertyUriMapper.GetXmlEnumValue(base.Uri));
			ServiceXml.CreateAttribute(xmlElement, "FieldIndex", this.Key);
			return xmlElement;
		}

		private const string DictionaryPropertyUriFormat = "{0}:{1}";

		private string key = string.Empty;
	}
}
