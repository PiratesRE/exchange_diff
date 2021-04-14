using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlInclude(typeof(ContactType))]
	[XmlInclude(typeof(MeetingSuggestionType))]
	[XmlInclude(typeof(UrlEntityType))]
	[XmlInclude(typeof(EmailAddressEntityType))]
	[XmlInclude(typeof(PhoneEntityType))]
	[XmlInclude(typeof(AddressEntityType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[XmlInclude(typeof(TaskSuggestionType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class EntityType
	{
		[XmlElement("Position")]
		public EmailPositionType[] Position
		{
			get
			{
				return this.positionField;
			}
			set
			{
				this.positionField = value;
			}
		}

		private EmailPositionType[] positionField;
	}
}
