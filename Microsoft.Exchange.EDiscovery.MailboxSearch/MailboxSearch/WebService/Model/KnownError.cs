using System;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal enum KnownError
	{
		NA,
		ErrorDiscoverySearchesDisabledException,
		ErrorSearchQueryCannotBeEmpty,
		ErrorNoMailboxSpecifiedForSearchOperation,
		ErrorInvalidSearchQuerySyntax,
		ErrorQueryLanguageNotValid,
		ErrorSortByPropertyIsNotFoundOrNotSupported,
		ErrorInvalidPropertyForSortBy,
		ErrorInvalidSearchId,
		ErrorSearchTimedOut,
		TooManyMailboxQueryObjects,
		TooManyMailboxesException,
		TooManyKeywordsException,
		ErrorRecipientTypeNotSupported,
		ErrorMailboxVersionNotSupported,
		ErrorNoPermissionToSearchOrHoldMailbox,
		ErrorSearchableObjectNotFound,
		ErrorWildcardAndGroupExpansionNotAllowed,
		ErrorSuffixSearchNotAllowed
	}
}
