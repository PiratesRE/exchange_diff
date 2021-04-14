using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class AsyncGetListContext
	{
		public List<PSObject> PsObjectCollection { get; set; }

		public int NextFetchStartAt { get; set; }

		public bool Completed { get; set; }

		public string WorkflowOutput { get; set; }

		public DDIParameters Parameters { get; set; }

		public List<string> UnicodeOutputColumnNames { get; set; }

		public List<Tuple<int, string[], string>> UnicodeColumns { get; set; }
	}
}
