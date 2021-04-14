using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IRecoveryEventSink
	{
		bool RecoveryConsume(MapiEvent mapiEvent);

		void EndRecovery();

		EventWatermark FirstMissedEventWatermark { get; }

		long LastMissedEventWatermark { get; }
	}
}
