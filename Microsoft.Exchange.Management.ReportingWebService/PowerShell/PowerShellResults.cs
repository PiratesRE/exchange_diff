using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ReportingWebService.PowerShell
{
	internal class PowerShellResults
	{
		public PowerShellResults() : this(null, null)
		{
		}

		public PowerShellResults(Collection<PSObject> output) : this(output, null)
		{
		}

		public PowerShellResults(Collection<ErrorRecord> errors) : this(null, errors)
		{
		}

		public PowerShellResults(Collection<PSObject> output, Collection<ErrorRecord> errors)
		{
			this.Output = output;
			this.Errors = errors;
		}

		public Collection<PSObject> Output { get; private set; }

		public Collection<ErrorRecord> Errors { get; private set; }

		public bool Succeeded
		{
			get
			{
				return this.Errors == null || this.Errors.Count == 0;
			}
		}
	}
}
