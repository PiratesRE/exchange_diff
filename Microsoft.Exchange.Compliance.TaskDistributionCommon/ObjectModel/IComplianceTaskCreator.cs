using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal interface IComplianceTaskCreator
	{
		IEnumerable<CompositeTask> CreateTasks(IConfigDataProvider dataProvider, ComplianceJob job, CreateTaskOptions createTaskOptions, Action<string, ComplianceBindingErrorType> invalidBindingHandler);
	}
}
