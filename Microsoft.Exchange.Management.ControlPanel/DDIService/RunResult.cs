using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	public class RunResult
	{
		public List<string> DataObjectes
		{
			get
			{
				return this.dataObjects;
			}
		}

		public bool ErrorOccur { get; set; }

		private List<string> dataObjects = new List<string>();
	}
}
