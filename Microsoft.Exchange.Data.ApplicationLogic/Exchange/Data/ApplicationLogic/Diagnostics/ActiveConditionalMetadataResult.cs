using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	[XmlRoot("ActiveCondition")]
	[XmlType("ActiveCondition")]
	public class ActiveConditionalMetadataResult : ConditionalMetadataResult
	{
		public string TimeRemaining { get; set; }
	}
}
