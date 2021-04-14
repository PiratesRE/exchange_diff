using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.AirSync
{
	internal interface IEasDeviceBudget : IStandardBudget, IBudget, IDisposable
	{
		void AddInteractiveCall();

		void AddCall();

		float Percentage { get; }
	}
}
