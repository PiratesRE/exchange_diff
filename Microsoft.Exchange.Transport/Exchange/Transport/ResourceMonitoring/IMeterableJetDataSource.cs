using System;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal interface IMeterableJetDataSource
	{
		string DatabasePath { get; }

		long GetAvailableFreeSpace();

		long GetDatabaseFileSize();

		long GetCurrentVersionBuckets();
	}
}
