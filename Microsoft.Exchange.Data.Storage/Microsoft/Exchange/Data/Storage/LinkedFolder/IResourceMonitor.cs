using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IResourceMonitor
	{
		void CheckResourceHealth();

		DelayInfo GetDelay();

		void StartChargingBudget();

		void ResetBudget();

		IBudget GetBudget();
	}
}
