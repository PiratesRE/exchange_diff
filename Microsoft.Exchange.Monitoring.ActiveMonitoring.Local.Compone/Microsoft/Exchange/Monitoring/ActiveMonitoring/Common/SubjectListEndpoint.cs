using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal class SubjectListEndpoint : IEndpoint
	{
		public List<string> AllHistoricalSubjects
		{
			get
			{
				return this.allHistoricalSubjects;
			}
		}

		public bool RestartOnChange
		{
			get
			{
				return this.signalForRestart;
			}
			set
			{
				this.signalForRestart = value;
			}
		}

		public Exception Exception { get; set; }

		public void Initialize()
		{
		}

		public bool DetectChange()
		{
			return this.signalForRestart;
		}

		private List<string> allHistoricalSubjects = new List<string>();

		private bool signalForRestart;
	}
}
