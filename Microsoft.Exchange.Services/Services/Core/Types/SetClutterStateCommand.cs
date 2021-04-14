using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SetClutterStateCommand", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum SetClutterStateCommand
	{
		EnableClutter,
		DisableClutter
	}
}
