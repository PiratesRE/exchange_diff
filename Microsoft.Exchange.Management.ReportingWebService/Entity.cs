using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Services.Common;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Management.ReportingWebService.PowerShell;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal sealed class Entity : IEntity
	{
		public Entity(string name, TaskInvocationInfo taskInvocationInfo, Dictionary<string, List<string>> reportPropertyCmdletParamsMap, IReportAnnotation annotation)
		{
			this.Name = name;
			this.TaskInvocationInfo = taskInvocationInfo;
			this.ReportPropertyCmdletParamsMap = reportPropertyCmdletParamsMap;
			this.Annotation = annotation;
		}

		public string Name { get; private set; }

		public TaskInvocationInfo TaskInvocationInfo { get; private set; }

		public Dictionary<string, List<string>> ReportPropertyCmdletParamsMap { get; private set; }

		public IReportAnnotation Annotation { get; private set; }

		public string[] KeyMembers { get; private set; }

		public Type ClrType { get; private set; }

		public void Initialize(IPSCommandResolver commandResolver)
		{
			ReadOnlyCollection<PSTypeName> outputType = commandResolver.GetOutputType(this.TaskInvocationInfo.CmdletName);
			this.ClrType = outputType[0].Type;
			this.KeyMembers = this.ClrType.GetCustomAttributes(typeof(DataServiceKeyAttribute), true).Cast<DataServiceKeyAttribute>().SelectMany((DataServiceKeyAttribute attr) => attr.KeyNames).ToArray<string>();
		}
	}
}
