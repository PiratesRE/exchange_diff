using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics.Components.Monad;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class CommandInteractionHandler
	{
		public CommandInteractionHandler()
		{
			ExTraceGlobals.HostTracer.Information((long)this.GetHashCode(), "new CommandInteractionHandler()");
		}

		public virtual ConfirmationChoice ShowConfirmationDialog(string message, ConfirmationChoice defaultChoice)
		{
			ExTraceGlobals.HostTracer.Information<string>((long)this.GetHashCode(), "CommandInteractionHandler.ShowConfirmationDialog({0})", message);
			return defaultChoice;
		}

		public virtual void ReportProgress(ProgressReportEventArgs e)
		{
		}

		public virtual void ReportErrors(ErrorReportEventArgs e)
		{
			ExTraceGlobals.HostTracer.Information<ErrorRecord>((long)this.GetHashCode(), "CommandInteractionHandler.ReportErrors({0})", e.ErrorRecord);
		}

		public virtual void ReportException(Exception e)
		{
			ExTraceGlobals.HostTracer.Information<Exception>((long)this.GetHashCode(), "CommandInteractionHandler.ReportException({0})", e);
		}

		public virtual void ReportWarning(WarningReportEventArgs e)
		{
			ExTraceGlobals.HostTracer.Information<string>((long)this.GetHashCode(), "CommandInteractionHandler.ReportWarning({0})", e.WarningMessage);
		}

		public virtual void ReportVerboseOutput(string message)
		{
			ExTraceGlobals.HostTracer.Information<string>((long)this.GetHashCode(), "CommandInteractionHandler.ReportVerboseOutput({0})", message);
		}

		public virtual Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
		{
			throw new NotSupportedException();
		}

		public virtual string WrapText(string text)
		{
			return text;
		}
	}
}
