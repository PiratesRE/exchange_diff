using System;

namespace Microsoft.Exchange.Services.Core.Search
{
	public enum ViewFilter
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
