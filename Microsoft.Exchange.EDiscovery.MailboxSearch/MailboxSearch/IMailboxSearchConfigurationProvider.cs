using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.EDiscovery.Export;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal interface IMailboxSearchConfigurationProvider
	{
		IDiscoverySearchDataProvider SearchDataProvider { get; }

		MailboxDiscoverySearch SearchObject { get; }

		ADUser DiscoverySystemMailboxUser { get; }

		IRecipientSession RecipientSession { get; }

		uint MaxMailboxesToSearch { get; }

		uint MaxNumberOfMailboxesForKeywordStatistics { get; }

		uint MaxMailboxSearches { get; }

		uint MaxQueryKeywords { get; }

		string SearchName { get; }

		string ExecutingUserId { get; set; }

		string ExecutingUserPrimarySmtpAddress { get; }

		bool UserCanRunMailboxSearch { get; }

		void UpdateSearchObject([CallerMemberName] string callerMember = null, [CallerLineNumber] int callerLine = 0);

		void ResetSearchObject([CallerMemberName] string callerMember = null, [CallerLineNumber] int callerLine = 0);

		string GenerateOWASearchResultsLink();

		string GenerateOWAPreviewResultsLink();

		string GetExecutingUserName();

		void CheckDiscoveryBudget(bool isEstimateOnly, MailboxSearchServer server);

		bool IsKeywordStatsAllowed();

		bool IsPreviewAllowed();

		bool ValidateKeywordsLimit();

		IList<ISource> ValidateAndGetFinalSourceMailboxes(string searchQuery, IList<string> sourceMailboxes, IList<string> notFoundMailboxes, IList<string> versionSkippedMailboxes, IList<string> rbacDeniedMailboxes, IList<string> crossPremiseFailedMailboxes, IDictionary<Uri, string> crossPremiseUrls);

		IList<ISource> GetFinalSources(string searchObjectName, string searchQuery, string executingUserPrimarySmtpAddress, Uri discoveryUserUri);
	}
}
