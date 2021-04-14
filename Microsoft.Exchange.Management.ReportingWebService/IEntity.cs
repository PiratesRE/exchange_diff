using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.ReportingWebService.PowerShell;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal interface IEntity
	{
		string Name { get; }

		TaskInvocationInfo TaskInvocationInfo { get; }

		Dictionary<string, List<string>> ReportPropertyCmdletParamsMap { get; }

		IReportAnnotation Annotation { get; }

		string[] KeyMembers { get; }

		Type ClrType { get; }

		void Initialize(IPSCommandResolver commandResolver);
	}
}
