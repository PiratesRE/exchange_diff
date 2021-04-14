using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class DictionaryPropertyUriBase : PropertyPath
	{
		public DictionaryPropertyUriBase()
		{
		}

		internal DictionaryPropertyUriBase(DictionaryUriEnum fieldUri)
		{
			this.FieldUri = fieldUri;
		}

		[IgnoreDataMember]
		[XmlAttribute(AttributeName = "FieldURI", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public DictionaryUriEnum Uri
		{
			get
			{
				return this.FieldUri;
			}
			set
			{
				this.FieldUri = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "FieldURI", IsRequired = true)]
		public string UriString
		{
			get
			{
				return EnumUtilities.ToString<DictionaryUriEnum>(this.FieldUri);
			}
			set
			{
				this.FieldUri = EnumUtilities.Parse<DictionaryUriEnum>(value);
			}
		}

		public override string ToString()
		{
			return PropertyUriMapper.GetXmlEnumValue(this.FieldUri);
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			DictionaryPropertyUriBase dictionaryPropertyUriBase = obj as DictionaryPropertyUriBase;
			if (dictionaryPropertyUriBase != null)
			{
				result = (this.FieldUri == dictionaryPropertyUriBase.FieldUri);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.FieldUri.GetHashCode();
		}

		internal override XmlElement ToXml(XmlElement parentElement)
		{
			return null;
		}

		internal DictionaryUriEnum FieldUri;
	}
}
