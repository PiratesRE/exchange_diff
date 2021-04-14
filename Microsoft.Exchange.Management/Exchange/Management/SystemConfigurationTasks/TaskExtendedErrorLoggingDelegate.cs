using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal delegate void TaskExtendedErrorLoggingDelegate(Exception exception, ErrorCategory category, object target, bool reThrow);
}
