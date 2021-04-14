using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.AirSync;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.ComposeMail
{
	[XmlType(Namespace = "ComposeMail", TypeName = "SendMail")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class SendMail
	{
		[XmlElement(ElementName = "ClientId")]
		public string ClientId { get; set; }

		[XmlElement(ElementName = "AccountId")]
		public string AccountId { get; set; }

		[XmlElement(ElementName = "SaveInSentItems")]
		public EmptyTag SerializableSaveInSentItems
		{
			get
			{
				if (!this.SaveInSentItems)
				{
					return null;
				}
				return new EmptyTag();
			}
			set
			{
				this.SaveInSentItems = (value != null);
			}
		}

		[XmlIgnore]
		public bool SaveInSentItems { get; set; }

		[XmlElement(ElementName = "Mime")]
		public XmlCDataSection SerializableMime
		{
			get
			{
				return new XmlDocument().CreateCDataSection(this.Mime);
			}
			set
			{
				this.Mime = value.Value;
			}
		}

		[XmlIgnore]
		public string Mime { get; set; }
	}
}
