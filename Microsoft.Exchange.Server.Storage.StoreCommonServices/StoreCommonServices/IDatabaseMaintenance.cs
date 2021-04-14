using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IDatabaseMaintenance
	{
		bool MarkForMaintenance(Context context);

		void ScheduleMarkForMaintenance(Context context, TimeSpan interval);
	}
}
