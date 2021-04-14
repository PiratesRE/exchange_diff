using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Write", "ExchangeSetupLog", DefaultParameterSetName = "Info")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class WriteExchangeSetupLog : Task
	{
		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "Info")]
		public SwitchParameter Info
		{
			get
			{
				return (SwitchParameter)base.Fields["Info"];
			}
			set
			{
				base.Fields["Info"] = value;
			}
		}

		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "Warning")]
		public SwitchParameter Warning
		{
			get
			{
				return (SwitchParameter)base.Fields["Warning"];
			}
			set
			{
				base.Fields["Warning"] = value;
			}
		}

		[Parameter(Mandatory = true, Position = 0, ParameterSetName = "Error")]
		public SwitchParameter Error
		{
			get
			{
				return (SwitchParameter)base.Fields["Error"];
			}
			set
			{
				base.Fields["Error"] = value;
			}
		}

		[Parameter(Mandatory = true, Position = 1, ParameterSetName = "Info")]
		[Parameter(Mandatory = true, Position = 1, ParameterSetName = "Warning")]
		public string Message
		{
			get
			{
				return (string)base.Fields["Message"];
			}
			set
			{
				base.Fields["Message"] = value;
			}
		}

		[Parameter(Mandatory = true, Position = 1, ParameterSetName = "Error")]
		public Exception Exception
		{
			get
			{
				return (Exception)base.Fields["Exception"];
			}
			set
			{
				base.Fields["Exception"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			if (base.ParameterSetName == "Warning")
			{
				this.WriteWarning(new LocalizedString(this.Message));
			}
			else if (base.ParameterSetName == "Error")
			{
				base.WriteError(this.Exception, ErrorCategory.InvalidOperation, null);
			}
			else
			{
				base.WriteVerbose(new LocalizedString(this.Message));
			}
			TaskLogger.LogExit();
		}

		private const string ParameterSetInfo = "Info";

		private const string ParameterSetWarning = "Warning";

		private const string ParameterSetError = "Error";
	}
}
