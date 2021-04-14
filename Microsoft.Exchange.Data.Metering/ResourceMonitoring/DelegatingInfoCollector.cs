using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class DelegatingInfoCollector : IExecutionInfo
	{
		public DelegatingInfoCollector(IEnumerable<IExecutionInfo> executionInfos)
		{
			ArgumentValidator.ThrowIfNull("executionInfos", executionInfos);
			this.executionInfos = executionInfos;
		}

		public IEnumerable<IExecutionInfo> ExecutionInfos
		{
			get
			{
				return this.executionInfos;
			}
		}

		public void OnStart()
		{
			foreach (IExecutionInfo executionInfo in this.executionInfos)
			{
				executionInfo.OnStart();
			}
		}

		public void OnException(Exception ex)
		{
			ArgumentValidator.ThrowIfNull("ex", ex);
			foreach (IExecutionInfo executionInfo in this.executionInfos)
			{
				executionInfo.OnException(ex);
			}
		}

		public void OnFinish()
		{
			foreach (IExecutionInfo executionInfo in this.executionInfos)
			{
				executionInfo.OnFinish();
			}
		}

		private readonly IEnumerable<IExecutionInfo> executionInfos;
	}
}
