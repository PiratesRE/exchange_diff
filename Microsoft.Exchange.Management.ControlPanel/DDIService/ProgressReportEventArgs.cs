using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class ProgressReportEventArgs : EventArgs
	{
		public ProgressReportEventArgs(IList<ErrorRecord> errors, int percent, string status)
		{
			this.Errors = errors;
			this.Percent = percent;
			this.Status = status;
		}

		public IList<ErrorRecord> Errors { get; internal set; }

		public int Percent { get; internal set; }

		public string Status { get; private set; }

		public override string ToString()
		{
			return string.Format("Errors: {0}, Percent: {1}, Status: {2}", this.Errors.ToJsonString(DDIService.KnownTypes.Value), this.Percent, this.Status);
		}
	}
}
