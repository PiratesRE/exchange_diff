using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ConflictResolutionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum ConflictResolutionType
	{
		NeverOverwrite,
		AutoResolve,
		AlwaysOverwrite
	}
}
