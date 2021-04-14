using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.Core.EventLog;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal static class CriticalFeatureHelper
	{
		internal static void Execute(this ICriticalFeature feature, Action action, TaskContext taskContext, string methodNameInLog)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				Guid uniqueId = taskContext.UniqueId;
				bool flag = false;
				Exception ex2;
				bool needReportException = !TaskHelper.IsTaskKnownException(ex2) && TaskHelper.ShouldReportException(ex2, out flag);
				if (!needReportException)
				{
					CmdletLogger.SafeAppendGenericError(uniqueId, methodNameInLog, ex2.ToString(), false);
				}
				else
				{
					CmdletLogger.SafeAppendGenericError(uniqueId, methodNameInLog, ex2.ToString(), true);
					CmdletLogger.SafeSetLogger(uniqueId, RpsCmdletMetadata.ErrorType, "UnHandled");
				}
				if (feature == null || feature.IsCriticalException(ex2))
				{
					throw;
				}
				if (!flag)
				{
					taskContext.CommandShell.WriteWarning(Strings.WarningTaskModuleSkipped(methodNameInLog, ex2.Message));
				}
				Diagnostics.ReportException(ex2, Constants.CoreEventLogger, TaskEventLogConstants.Tuple_UnhandledException, (object ex) => needReportException, null, ExTraceGlobals.InstrumentationTracer, "Exception from  " + methodNameInLog + ": {0}");
			}
		}
	}
}
