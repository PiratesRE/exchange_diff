using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal interface ILegacyResourceHealthProvider
	{
		void Update(ConstraintCheckResultType constraintResult, ConstraintCheckAgent agent, LocalizedString failureReason);

		ConstraintCheckResultType ConstraintResult { get; }

		ConstraintCheckAgent Agent { get; }

		LocalizedString FailureReason { get; }
	}
}
