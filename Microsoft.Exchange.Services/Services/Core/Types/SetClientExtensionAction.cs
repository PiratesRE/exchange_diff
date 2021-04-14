using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SetClientExtensionActionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class SetClientExtensionAction
	{
		[XmlAttribute]
		public SetClientExtensionActionId ActionId { get; set; }

		[XmlAttribute]
		public string ExtensionId { get; set; }

		[XmlElement]
		public ClientExtension ClientExtension { get; set; }
	}
}
