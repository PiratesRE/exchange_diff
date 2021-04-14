using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring
{
	internal interface IReplicationCheck
	{
		string Title { get; }

		LocalizedString Description { get; }

		CheckCategory Category { get; }

		IEventManager EventManager { get; }

		bool HasRun { get; }

		bool HasError { get; }

		bool HasPassed { get; }

		ReplicationCheckOutcome Outcome { get; }

		List<ReplicationCheckOutputObject> OutputObjects { get; }

		void Run();

		void Skip();

		void LogEvents();
	}
}
