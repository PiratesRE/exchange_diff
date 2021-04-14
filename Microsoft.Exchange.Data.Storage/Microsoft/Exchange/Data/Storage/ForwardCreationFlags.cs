using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ForwardCreationFlags
	{
		None = 0,
		PreserveSender = 1,
		PreserveSubject = 2,
		TreatAsMeetingMessage = 4,
		ResourceDelegationMessage = 8
	}
}
