using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface ICountTrackerDiagnostics<TEntityType, TCountType> where TEntityType : struct, IConvertible where TCountType : struct, IConvertible
	{
		void EntityAdded(ICountedEntity<TEntityType> entity);

		void EntityRemoved(ICountedEntity<TEntityType> entity);

		void MeasureAdded(TCountType measure);

		void MeasureRemoved(TCountType measure);

		void MeasurePromoted(TCountType measure);

		void MeasureExpired(TCountType measure);
	}
}
