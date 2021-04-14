using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IMonitoringADConfigProvider
	{
		IMonitoringADConfig GetConfig(bool waitForInit = true);

		IMonitoringADConfig GetRecentConfig(bool waitForInit = true);

		IMonitoringADConfig GetConfigIgnoringStaleness(bool waitForInit = true);
	}
}
