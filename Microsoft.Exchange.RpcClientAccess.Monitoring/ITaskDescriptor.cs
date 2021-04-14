using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ITaskDescriptor
	{
		IPropertyBag Properties { get; }

		LocalizedString TaskTitle { get; }

		LocalizedString TaskDescription { get; }

		TaskType TaskType { get; }

		IEnumerable<ContextProperty> DependentProperties { get; }
	}
}
