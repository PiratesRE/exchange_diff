using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	[XmlRoot("CompletedCondition")]
	[XmlType("CompletedCondition")]
	public class ConditionalMetadataResult
	{
		public string Cookie { get; set; }

		public string User { get; set; }

		[XmlArrayItem("HitFile")]
		public string[] Files { get; set; }

		public int CurrentHits { get; set; }

		public DateTime Created { get; set; }

		public int MaxHits { get; set; }

		public string Description { get; set; }

		public string SelectClause { get; set; }

		public string WhereClause { get; set; }
	}
}
