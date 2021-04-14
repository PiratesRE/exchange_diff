using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions
{
	public interface ICalendarViewParameters
	{
		ExDateTime EffectiveEndTime { get; }

		ExDateTime EffectiveStartTime { get; }

		bool HasExplicitEndTime { get; }

		bool HasExplicitStartTime { get; }

		bool IsDefaultView { get; }
	}
}
