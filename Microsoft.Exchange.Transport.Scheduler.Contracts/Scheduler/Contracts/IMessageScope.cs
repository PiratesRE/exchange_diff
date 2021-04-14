using System;

namespace Microsoft.Exchange.Transport.Scheduler.Contracts
{
	internal interface IMessageScope : IEquatable<IMessageScope>
	{
		string Display { get; }

		MessageScopeType Type { get; }

		object Value { get; }
	}
}
