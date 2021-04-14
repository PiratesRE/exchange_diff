using System;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips
{
	internal interface IFileDeletionPolicy
	{
		bool ShouldDelete(string filePath);
	}
}
