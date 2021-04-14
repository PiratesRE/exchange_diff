using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "BodyTypeResponseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum BodyResponseType
	{
		[XmlEnum("Best")]
		Best,
		[XmlEnum("Text")]
		Text,
		[XmlEnum("HTML")]
		HTML
	}
}
