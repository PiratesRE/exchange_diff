using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class DecisionLogger : IEnumerable
	{
		public DecisionLogger(TextWriter decisionLog)
		{
			this.decisionLog = decisionLog;
			this.Decision = true;
		}

		public void Add(bool precondition, LocalizedString failureLog)
		{
			this.Decision = (this.Decision && precondition);
			if (!precondition)
			{
				this.decisionLog.Write('\t');
				this.decisionLog.WriteLine(failureLog);
			}
		}

		public bool Decision { get; private set; }

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		private TextWriter decisionLog;
	}
}
