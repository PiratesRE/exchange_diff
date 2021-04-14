using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IStepSnapshot
	{
		ISnapshotId Id { get; }

		SnapshotStatus Status { get; }

		LocalizedString? ErrorMessage { get; }

		ExDateTime? InjectionCompletedTime { get; }
	}
}
