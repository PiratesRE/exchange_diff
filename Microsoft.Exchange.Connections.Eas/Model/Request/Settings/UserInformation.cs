using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.AirSync;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.Settings
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Settings", TypeName = "UserInformation")]
	public class UserInformation
	{
		[XmlElement(ElementName = "Get")]
		public EmptyTag SerializableGet
		{
			get
			{
				if (!this.Get)
				{
					return null;
				}
				return new EmptyTag();
			}
			set
			{
				this.Get = (value != null);
			}
		}

		[XmlIgnore]
		public bool Get { get; set; }
	}
}
