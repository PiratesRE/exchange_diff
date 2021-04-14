using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "BodyTypeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum BodyType
	{
		[XmlEnum("Text")]
		Text,
		[XmlEnum("HTML")]
		HTML
	}
}
