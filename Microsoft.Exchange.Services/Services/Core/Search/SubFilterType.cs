using System;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal enum SubFilterType
	{
		None,
		RecipientTo,
		RecipientCc,
		RecipientBcc,
		AttendeeRequired,
		AttendeeOptional,
		AttendeeResource,
		Attachment
	}
}
