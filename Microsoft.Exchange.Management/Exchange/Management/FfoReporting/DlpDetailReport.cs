using System;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[Serializable]
	public class DlpDetailReport : DetailObject
	{
		[DalConversion("OrganizationFromTask", "Organization", new string[]
		{

		})]
		public string Organization { get; private set; }

		[DalConversion("DefaultSerializer", "PolicyMatchTime", new string[]
		{

		})]
		public DateTime Date { get; private set; }

		[Redact]
		[DalConversion("DefaultSerializer", "Title", new string[]
		{

		})]
		public string Title { get; private set; }

		[DalConversion("DefaultSerializer", "Location", new string[]
		{

		})]
		public string Location { get; private set; }

		[DalConversion("DefaultSerializer", "Severity", new string[]
		{

		})]
		public string Severity { get; private set; }

		[DalConversion("DefaultSerializer", "Size", new string[]
		{

		})]
		public long Size { get; private set; }

		[ODataInput("Source")]
		[DalConversion("DefaultSerializer", "DataSource", new string[]
		{

		})]
		public string Source { get; private set; }

		[ODataInput("Actor")]
		[Redact]
		[DalConversion("DefaultSerializer", "Actor", new string[]
		{

		})]
		public string Actor { get; private set; }

		[DalConversion("DefaultSerializer", "PolicyName", new string[]
		{

		})]
		[ODataInput("DlpPolicy")]
		public string DlpPolicy { get; private set; }

		[ODataInput("TransportRule")]
		[DalConversion("DefaultSerializer", "TransportRuleName", new string[]
		{

		})]
		public string TransportRule { get; private set; }

		[DalConversion("DefaultSerializer", "OverrideType", new string[]
		{

		})]
		[Redact]
		public string UserAction { get; private set; }

		[DalConversion("DefaultSerializer", "OverrideJustification", new string[]
		{

		})]
		[Redact]
		public string Justification { get; private set; }

		[Redact]
		[DalConversion("DefaultSerializer", "DataClassification", new string[]
		{

		})]
		public string SensitiveInformationType { get; private set; }

		[DalConversion("DefaultSerializer", "ClassificationCount", new string[]
		{

		})]
		public int SensitiveInformationCount { get; private set; }

		[DalConversion("DefaultSerializer", "ClassificationConfidence", new string[]
		{

		})]
		public int SensitiveInformationConfidence { get; private set; }

		[ODataInput("EventType")]
		[DalConversion("DefaultSerializer", "EventType", new string[]
		{

		})]
		public string EventType { get; private set; }

		[DalConversion("DefaultSerializer", "Action", new string[]
		{

		})]
		[ODataInput("Action")]
		public string Action { get; private set; }

		[DalConversion("DefaultSerializer", "ObjectId", new string[]
		{

		})]
		public Guid ObjectId { get; private set; }
	}
}
