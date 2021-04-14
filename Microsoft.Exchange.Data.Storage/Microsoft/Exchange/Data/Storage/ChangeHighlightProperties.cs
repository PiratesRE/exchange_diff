using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ChangeHighlightProperties
	{
		None = 0,
		MapiStartTime = 1,
		MapiEndTime = 2,
		Duration = 3,
		RecurrenceProps = 4,
		Location = 8,
		Subject = 16,
		Recipients = 32,
		BodyProps = 128,
		BillMilesCompany = 256,
		IsSilent = 512,
		DisallowNewTimeProposal = 1024,
		NetMeetingProps = 2048,
		NetShowProps = 4096,
		OtherProps = 134217728,
		FullUpdateFlags = 7
	}
}
