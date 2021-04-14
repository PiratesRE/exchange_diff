using System;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal interface IExecutionInfo
	{
		void OnStart();

		void OnException(Exception ex);

		void OnFinish();
	}
}
