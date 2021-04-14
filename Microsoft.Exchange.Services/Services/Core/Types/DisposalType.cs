using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("DisposalType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum DisposalType
	{
		HardDelete = 2,
		SoftDelete = 1,
		MoveToDeletedItems = 4
	}
}
