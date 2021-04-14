using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum OwaViewFilter
	{
		All,
		Flagged,
		HasAttachment,
		ToOrCcMe,
		Unread,
		TaskActive,
		TaskOverdue,
		TaskCompleted,
		DeprecatedSuggestions,
		DeprecatedSuggestionsRespond,
		DeprecatedSuggestionsDelete,
		NoClutter,
		Clutter
	}
}
