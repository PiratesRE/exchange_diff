using System;

namespace Microsoft.Exchange.Diagnostics
{
	public interface IFailureObjectLoggable
	{
		Guid ObjectGuid { get; }

		string ObjectType { get; }

		int Flags { get; }

		string FailureContext { get; }
	}
}
