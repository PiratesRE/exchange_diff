using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IFailureEntry : IDocEntry, IEquatable<IDocEntry>, IEquatable<IFailureEntry>
	{
		IIdentity ItemId { get; }

		EvaluationErrors ErrorCode { get; }

		LocalizedString ErrorDescription { get; }

		string AdditionalInfo { get; }

		bool IsPartiallyIndexed { get; }

		DateTime? LastAttemptTime { get; }

		int AttemptCount { get; }

		bool IsPermanentFailure { get; }
	}
}
