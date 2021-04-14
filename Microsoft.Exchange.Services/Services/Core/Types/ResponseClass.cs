using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ResponseClassType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum ResponseClass
	{
		Success,
		Warning,
		Error
	}
}
