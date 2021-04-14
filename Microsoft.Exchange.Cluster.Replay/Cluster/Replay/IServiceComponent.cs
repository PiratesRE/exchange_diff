using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IServiceComponent
	{
		string Name { get; }

		FacilityEnum Facility { get; }

		bool IsCritical { get; }

		bool IsEnabled { get; }

		bool IsRetriableOnError { get; }

		bool Start();

		void Stop();

		void Invoke(Action toInvoke);
	}
}
