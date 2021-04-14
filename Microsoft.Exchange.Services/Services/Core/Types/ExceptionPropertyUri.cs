using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "PathToExceptionFieldType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ExceptionPropertyUri : PropertyPath
	{
		public ExceptionPropertyUri()
		{
		}

		internal ExceptionPropertyUri(ExceptionPropertyUriEnum uri)
		{
			this.uri = uri;
		}

		[IgnoreDataMember]
		[XmlAttribute(AttributeName = "FieldURI", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public ExceptionPropertyUriEnum Uri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
			}
		}

		[DataMember(Name = "FieldURI", IsRequired = true)]
		[XmlIgnore]
		public string UriString
		{
			get
			{
				return EnumUtilities.ToString<ExceptionPropertyUriEnum>(this.Uri);
			}
			set
			{
				this.Uri = EnumUtilities.Parse<ExceptionPropertyUriEnum>(value);
			}
		}

		public override string ToString()
		{
			return PropertyUriMapper.GetXmlEnumValue(this.uri);
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			ExceptionPropertyUri exceptionPropertyUri = obj as ExceptionPropertyUri;
			if (exceptionPropertyUri != null)
			{
				result = (this.uri == exceptionPropertyUri.Uri);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.uri.GetHashCode();
		}

		internal override XmlElement ToXml(XmlElement parentElement)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(parentElement, "ExceptionFieldURI", "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateAttribute(xmlElement, "FieldURI", this.ToString());
			return xmlElement;
		}

		private ExceptionPropertyUriEnum uri;
	}
}
