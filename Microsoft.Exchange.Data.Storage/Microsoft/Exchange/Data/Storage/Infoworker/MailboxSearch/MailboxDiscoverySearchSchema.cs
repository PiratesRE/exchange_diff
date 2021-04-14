using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxDiscoverySearchSchema : EwsStoreObjectSchema
	{
		public static readonly EwsStoreObjectPropertyDefinition Target = new EwsStoreObjectPropertyDefinition("Target", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, null, MailboxDiscoverySearchExtendedStoreSchema.Target);

		public static readonly EwsStoreObjectPropertyDefinition Status = new EwsStoreObjectPropertyDefinition("Status", ExchangeObjectVersion.Exchange2012, typeof(SearchState), PropertyDefinitionFlags.PersistDefaultValue, SearchState.NotStarted, SearchState.NotStarted, ExtendedEwsStoreObjectSchema.Status);

		public static readonly EwsStoreObjectPropertyDefinition StatisticsOnly = new EwsStoreObjectPropertyDefinition("StatisticsOnly", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.StatisticsOnly);

		public static readonly EwsStoreObjectPropertyDefinition IncludeUnsearchableItems = new EwsStoreObjectPropertyDefinition("IncludeUnsearchableItems", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.IncludeUnsearchableItems);

		public static readonly EwsStoreObjectPropertyDefinition Resume = new EwsStoreObjectPropertyDefinition("Resume", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.Resume);

		public static readonly EwsStoreObjectPropertyDefinition LastStartTime = new EwsStoreObjectPropertyDefinition("LastStartTime", ExchangeObjectVersion.Exchange2012, typeof(DateTime), PropertyDefinitionFlags.None, default(DateTime), default(DateTime), MailboxDiscoverySearchExtendedStoreSchema.LastStartTime);

		public static readonly EwsStoreObjectPropertyDefinition LastEndTime = new EwsStoreObjectPropertyDefinition("LastEndTime", ExchangeObjectVersion.Exchange2012, typeof(DateTime), PropertyDefinitionFlags.None, default(DateTime), default(DateTime), MailboxDiscoverySearchExtendedStoreSchema.LastEndTime);

		public static readonly EwsStoreObjectPropertyDefinition PercentComplete = new EwsStoreObjectPropertyDefinition("PercentComplete", ExchangeObjectVersion.Exchange2012, typeof(int), PropertyDefinitionFlags.None, 0, 0, MailboxDiscoverySearchExtendedStoreSchema.PercentComplete);

		public static readonly EwsStoreObjectPropertyDefinition NumberOfMailboxes = new EwsStoreObjectPropertyDefinition("NumberOfMailboxes", ExchangeObjectVersion.Exchange2012, typeof(int), PropertyDefinitionFlags.None, 0, 0, MailboxDiscoverySearchExtendedStoreSchema.NumberOfMailboxes);

		public static readonly EwsStoreObjectPropertyDefinition ResultItemCountCopied = new EwsStoreObjectPropertyDefinition("ResultItemCountCopied", ExchangeObjectVersion.Exchange2012, typeof(long), PropertyDefinitionFlags.None, 0L, 0L, MailboxDiscoverySearchExtendedStoreSchema.ResultItemCountCopied);

		public static readonly EwsStoreObjectPropertyDefinition ResultItemCountEstimate = new EwsStoreObjectPropertyDefinition("ResultItemCountEstimate", ExchangeObjectVersion.Exchange2012, typeof(long), PropertyDefinitionFlags.None, 0L, 0L, MailboxDiscoverySearchExtendedStoreSchema.ResultItemCountEstimate);

		public static readonly EwsStoreObjectPropertyDefinition ResultUnsearchableItemCountEstimate = new EwsStoreObjectPropertyDefinition("ResultUnsearchableItemCountEstimate", ExchangeObjectVersion.Exchange2012, typeof(long), PropertyDefinitionFlags.None, 0L, 0L, MailboxDiscoverySearchExtendedStoreSchema.ResultUnsearchableItemCountEstimate);

		public static readonly EwsStoreObjectPropertyDefinition ResultSizeCopied = new EwsStoreObjectPropertyDefinition("ResultSizeCopied", ExchangeObjectVersion.Exchange2012, typeof(long), PropertyDefinitionFlags.None, 0L, 0L, MailboxDiscoverySearchExtendedStoreSchema.ResultSizeCopied);

		public static readonly EwsStoreObjectPropertyDefinition ResultSizeEstimate = new EwsStoreObjectPropertyDefinition("ResultSizeEstimate", ExchangeObjectVersion.Exchange2012, typeof(long), PropertyDefinitionFlags.None, 0L, 0L, MailboxDiscoverySearchExtendedStoreSchema.ResultSizeEstimate);

		public static readonly EwsStoreObjectPropertyDefinition ResultsPath = new EwsStoreObjectPropertyDefinition("ResultsPath", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.ReturnOnBind, null, null, MailboxDiscoverySearchExtendedStoreSchema.ResultsPath);

		public static readonly EwsStoreObjectPropertyDefinition ResultsLink = new EwsStoreObjectPropertyDefinition("ResultsLink", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.ReturnOnBind, null, null, MailboxDiscoverySearchExtendedStoreSchema.ResultsLink);

		public static readonly EwsStoreObjectPropertyDefinition PreviewResultsLink = new EwsStoreObjectPropertyDefinition("PreviewResultsLink", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.ReturnOnBind, null, null, MailboxDiscoverySearchExtendedStoreSchema.PreviewResultsLink);

		public static readonly EwsStoreObjectPropertyDefinition LogLevel = new EwsStoreObjectPropertyDefinition("LogLevel", ExchangeObjectVersion.Exchange2012, typeof(LoggingLevel), PropertyDefinitionFlags.None, LoggingLevel.Suppress, LoggingLevel.Suppress, MailboxDiscoverySearchExtendedStoreSchema.LogLevel);

		public static readonly EwsStoreObjectPropertyDefinition CompletedMailboxes = new EwsStoreObjectPropertyDefinition("CompletedMailboxes", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, ItemSchema.Attachments);

		public static readonly EwsStoreObjectPropertyDefinition StatusMailRecipients = new EwsStoreObjectPropertyDefinition("StatusMailRecipients", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), PropertyDefinitionFlags.MultiValued, null, null, MailboxDiscoverySearchExtendedStoreSchema.StatusMailRecipients);

		public static readonly EwsStoreObjectPropertyDefinition ManagedBy = new EwsStoreObjectPropertyDefinition("ManagedBy", ExchangeObjectVersion.Exchange2012, typeof(ADObjectId), PropertyDefinitionFlags.MultiValued, null, null, MailboxDiscoverySearchExtendedStoreSchema.ManagedBy);

		public static readonly EwsStoreObjectPropertyDefinition Query = new EwsStoreObjectPropertyDefinition("Query", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.ReturnOnBind, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.Query);

		public static readonly EwsStoreObjectPropertyDefinition Senders = new EwsStoreObjectPropertyDefinition("Senders", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, MailboxDiscoverySearchExtendedStoreSchema.Senders);

		public static readonly EwsStoreObjectPropertyDefinition Recipients = new EwsStoreObjectPropertyDefinition("Recipients", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, MailboxDiscoverySearchExtendedStoreSchema.Recipients);

		public static readonly EwsStoreObjectPropertyDefinition StartDate = new EwsStoreObjectPropertyDefinition("StartDate", ExchangeObjectVersion.Exchange2012, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, null, MailboxDiscoverySearchExtendedStoreSchema.StartDate);

		public static readonly EwsStoreObjectPropertyDefinition EndDate = new EwsStoreObjectPropertyDefinition("EndDate", ExchangeObjectVersion.Exchange2012, typeof(ExDateTime?), PropertyDefinitionFlags.None, null, null, MailboxDiscoverySearchExtendedStoreSchema.EndDate);

		public static readonly EwsStoreObjectPropertyDefinition MessageTypes = new EwsStoreObjectPropertyDefinition("MessageTypes", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, MailboxDiscoverySearchExtendedStoreSchema.MessageTypes);

		public static readonly EwsStoreObjectPropertyDefinition CalculatedQuery = new EwsStoreObjectPropertyDefinition("CalculatedQuery", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.ReturnOnBind, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.CalculatedQuery);

		public static readonly EwsStoreObjectPropertyDefinition Language = new EwsStoreObjectPropertyDefinition("Language", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, "en-US", "en-US", MailboxDiscoverySearchExtendedStoreSchema.Language);

		public static readonly EwsStoreObjectPropertyDefinition Sources = new EwsStoreObjectPropertyDefinition("Sources", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, ItemSchema.Attachments);

		public static readonly EwsStoreObjectPropertyDefinition AllSourceMailboxes = new EwsStoreObjectPropertyDefinition("AllSourceMailboxes", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.AllSourceMailboxes);

		public static readonly EwsStoreObjectPropertyDefinition PublicFolderSources = new EwsStoreObjectPropertyDefinition("PublicFolderSources", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, ItemSchema.Attachments);

		public static readonly EwsStoreObjectPropertyDefinition AllPublicFolderSources = new EwsStoreObjectPropertyDefinition("AllPublicFolderSources", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.AllPublicFolderSources);

		public static readonly EwsStoreObjectPropertyDefinition SearchStatistics = new EwsStoreObjectPropertyDefinition("SearchStatistics", ExchangeObjectVersion.Exchange2012, typeof(DiscoverySearchStats), PropertyDefinitionFlags.MultiValued, null, null, MailboxDiscoverySearchExtendedStoreSchema.SearchStatistics);

		public static readonly EwsStoreObjectPropertyDefinition Version = new EwsStoreObjectPropertyDefinition("Version", ExchangeObjectVersion.Exchange2012, typeof(SearchObjectVersion), PropertyDefinitionFlags.None, SearchObjectVersion.Original, SearchObjectVersion.Original, MailboxDiscoverySearchExtendedStoreSchema.Version);

		public static readonly EwsStoreObjectPropertyDefinition CreatedTime = new EwsStoreObjectPropertyDefinition("CreatedTime", ExchangeObjectVersion.Exchange2012, typeof(DateTime), PropertyDefinitionFlags.ReadOnly, default(DateTime), default(DateTime), ItemSchema.DateTimeCreated);

		public static readonly EwsStoreObjectPropertyDefinition CreatedBy = new EwsStoreObjectPropertyDefinition("CreatedBy", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.CreatedBy);

		public static readonly EwsStoreObjectPropertyDefinition LastModifiedTime = new EwsStoreObjectPropertyDefinition("LastModifiedTime", ExchangeObjectVersion.Exchange2012, typeof(DateTime), PropertyDefinitionFlags.ReadOnly, default(DateTime), default(DateTime), ItemSchema.LastModifiedTime);

		public static readonly EwsStoreObjectPropertyDefinition LastModifiedBy = new EwsStoreObjectPropertyDefinition("LastModifiedBy", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.LastModifiedBy);

		public static readonly EwsStoreObjectPropertyDefinition ExcludeDuplicateMessages = new EwsStoreObjectPropertyDefinition("ExcludeDuplicateMessages", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.ExcludeDuplicateMessages);

		public static readonly EwsStoreObjectPropertyDefinition Errors = new EwsStoreObjectPropertyDefinition("Errors", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, ItemSchema.Attachments);

		public static readonly EwsStoreObjectPropertyDefinition InPlaceHoldEnabled = new EwsStoreObjectPropertyDefinition("InPlaceHoldEnabled", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.InPlaceHoldEnabled);

		public static readonly EwsStoreObjectPropertyDefinition ItemHoldPeriod = new EwsStoreObjectPropertyDefinition("ItemHoldPeriod", ExchangeObjectVersion.Exchange2012, typeof(Unlimited<EnhancedTimeSpan>), PropertyDefinitionFlags.None, Unlimited<EnhancedTimeSpan>.UnlimitedValue, Unlimited<EnhancedTimeSpan>.UnlimitedValue, MailboxDiscoverySearchExtendedStoreSchema.ItemHoldPeriod);

		public static readonly EwsStoreObjectPropertyDefinition InPlaceHoldIdentity = new EwsStoreObjectPropertyDefinition("InPlaceHoldIdentity", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.InPlaceHoldIdentity);

		public static readonly EwsStoreObjectPropertyDefinition LegacySearchObjectIdentity = new EwsStoreObjectPropertyDefinition("LegacySearchObjectIdentity", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.LegacySearchObjectIdentity);

		public static readonly EwsStoreObjectPropertyDefinition Description = new EwsStoreObjectPropertyDefinition("Description", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.Description);

		public static readonly EwsStoreObjectPropertyDefinition FailedToHoldMailboxes = new EwsStoreObjectPropertyDefinition("FailedToHoldMailboxes", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, ItemSchema.Attachments);

		public static readonly EwsStoreObjectPropertyDefinition InPlaceHoldErrors = new EwsStoreObjectPropertyDefinition("InPlaceHoldErrors", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, ItemSchema.Attachments);

		public static readonly EwsStoreObjectPropertyDefinition ManagedByOrganization = new EwsStoreObjectPropertyDefinition("ManagedByOrganization", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.ManagedByOrganization);

		public static readonly EwsStoreObjectPropertyDefinition IncludeKeywordStatistics = new EwsStoreObjectPropertyDefinition("IncludeKeywordStatistics", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.IncludeKeywordStatistics);

		public static readonly EwsStoreObjectPropertyDefinition StatisticsStartIndex = new EwsStoreObjectPropertyDefinition("StatisticsStartIndex", ExchangeObjectVersion.Exchange2012, typeof(int), PropertyDefinitionFlags.None, 1, 1, MailboxDiscoverySearchExtendedStoreSchema.StatisticsStartIndex);

		public static readonly EwsStoreObjectPropertyDefinition UserKeywords = new EwsStoreObjectPropertyDefinition("UserKeywords", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, MailboxDiscoverySearchExtendedStoreSchema.UserKeywords);

		public static readonly EwsStoreObjectPropertyDefinition KeywordsQuery = new EwsStoreObjectPropertyDefinition("KeywordsQuery", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.ReturnOnBind, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.KeywordsQuery);

		public static readonly EwsStoreObjectPropertyDefinition TotalKeywords = new EwsStoreObjectPropertyDefinition("TotalKeywords", ExchangeObjectVersion.Exchange2012, typeof(int), PropertyDefinitionFlags.None, 0, 0, MailboxDiscoverySearchExtendedStoreSchema.TotalKeywords);

		public static readonly EwsStoreObjectPropertyDefinition KeywordHits = new EwsStoreObjectPropertyDefinition("KeywordHits", ExchangeObjectVersion.Exchange2012, typeof(KeywordHit), PropertyDefinitionFlags.MultiValued, null, null, MailboxDiscoverySearchExtendedStoreSchema.KeywordHits);

		public static readonly EwsStoreObjectPropertyDefinition KeywordStatisticsDisabled = new EwsStoreObjectPropertyDefinition("KeywordStatisticsDisabled", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.KeywordStatisticsDisabled);

		public static readonly EwsStoreObjectPropertyDefinition PreviewDisabled = new EwsStoreObjectPropertyDefinition("PreviewDisabled", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, false, MailboxDiscoverySearchExtendedStoreSchema.PreviewDisabled);

		public static readonly EwsStoreObjectPropertyDefinition Information = new EwsStoreObjectPropertyDefinition("Information", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued, null, null, MailboxDiscoverySearchExtendedStoreSchema.Information);

		public static readonly EwsStoreObjectPropertyDefinition ScenarioId = new EwsStoreObjectPropertyDefinition("ScenarioId", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, string.Empty, MailboxDiscoverySearchExtendedStoreSchema.ScenarioId);
	}
}
