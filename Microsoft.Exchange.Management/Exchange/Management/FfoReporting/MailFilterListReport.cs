using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting
{
	[DataServiceKey("SelectionTarget")]
	[Serializable]
	public class MailFilterListReport : FfoReportObject
	{
		[Column(Name = "Organization")]
		public string Organization { get; internal set; }

		[ODataInput("SelectionTarget")]
		[Column(Name = "SelectionTarget")]
		public string SelectionTarget { get; internal set; }

		[Column(Name = "Display")]
		public string Display { get; internal set; }

		[Column(Name = "Value")]
		public string Value { get; internal set; }

		[Column(Name = "ParentTarget")]
		public string ParentTarget { get; internal set; }

		[Column(Name = "ParentValue")]
		public string ParentValue { get; internal set; }

		[ODataInput("Domain")]
		public string Domain { get; private set; }
	}
}
