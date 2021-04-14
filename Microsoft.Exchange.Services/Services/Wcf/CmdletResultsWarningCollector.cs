using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class CmdletResultsWarningCollector : TaskIOPipelineBase
	{
		public override bool WriteWarning(LocalizedString input, string helperUrl, out LocalizedString output)
		{
			output = input;
			if (this.capturedWarnings == null)
			{
				this.capturedWarnings = new Dictionary<int, List<string>>();
			}
			if (!this.capturedWarnings.ContainsKey(this.currentResultIndex))
			{
				this.capturedWarnings[this.currentResultIndex] = new List<string>();
			}
			this.capturedWarnings[this.currentResultIndex].Add(input);
			return true;
		}

		public override bool WriteObject(object input, out object output)
		{
			this.currentResultIndex++;
			return base.WriteObject(input, out output);
		}

		internal string[] GetWarningMessagesForResult(int resultIndex)
		{
			if (this.capturedWarnings != null && this.capturedWarnings.ContainsKey(resultIndex))
			{
				return this.capturedWarnings[resultIndex].ToArray();
			}
			return null;
		}

		private int currentResultIndex;

		private Dictionary<int, List<string>> capturedWarnings;
	}
}
