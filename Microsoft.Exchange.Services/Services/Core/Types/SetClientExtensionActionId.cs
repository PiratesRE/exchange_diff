using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SetClientExtensionActionId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum SetClientExtensionActionId
	{
		Install,
		Uninstall,
		Configure
	}
}
