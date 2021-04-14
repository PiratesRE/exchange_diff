using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface ICallGetAfterExecuteWorkflow
	{
		[DefaultValue(false)]
		bool IgnoreGetObject { get; set; }
	}
}
