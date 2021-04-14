using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal delegate IEntity CreateEntityDelegate(string name, TaskInvocationInfo taskInvocationInfo, Dictionary<string, List<string>> reportPropertyCmdletParamsMap, IReportAnnotation annotation);
}
