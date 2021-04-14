using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxDiscoverySearchExtendedStoreSchema : ObjectSchema
	{
		internal static readonly Guid PropertySetId = new Guid("E27E00C0-86D5-4306-AC73-50F5397A8321");

		public static readonly ExtendedPropertyDefinition Target = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "Target", 25);

		public static readonly ExtendedPropertyDefinition StatisticsOnly = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "StatisticsOnly", 4);

		public static readonly ExtendedPropertyDefinition AllSourceMailboxes = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "AllSourceMailboxes", 4);

		public static readonly ExtendedPropertyDefinition AllPublicFolderSources = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "AllPublicFolderSources", 4);

		public static readonly ExtendedPropertyDefinition SearchStatistics = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "SearchStatistics", 26);

		public static readonly ExtendedPropertyDefinition Version = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "Version", 14);

		public static readonly ExtendedPropertyDefinition IncludeUnsearchableItems = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "IncludeUnsearchableItems", 4);

		public static readonly ExtendedPropertyDefinition Resume = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "Resume", 4);

		public static readonly ExtendedPropertyDefinition LastStartTime = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "LastStartTime", 23);

		public static readonly ExtendedPropertyDefinition LastEndTime = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "LastEndTime", 23);

		public static readonly ExtendedPropertyDefinition NumberOfMailboxes = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "NumberOfMailboxes", 14);

		public static readonly ExtendedPropertyDefinition PercentComplete = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "PercentComplete", 14);

		public static readonly ExtendedPropertyDefinition ResultItemCountCopied = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ResultItemCountCopied", 16);

		public static readonly ExtendedPropertyDefinition ResultItemCountEstimate = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ResultItemCountEstimate", 16);

		public static readonly ExtendedPropertyDefinition ResultUnsearchableItemCountEstimate = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ResultUnsearchableItemCountEstimate", 16);

		public static readonly ExtendedPropertyDefinition ResultSizeCopied = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ResultSizeCopied", 16);

		public static readonly ExtendedPropertyDefinition ResultSizeEstimate = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ResultSizeEstimate", 16);

		public static readonly ExtendedPropertyDefinition ResultsPath = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ResultsPath", 25);

		public static readonly ExtendedPropertyDefinition ResultsLink = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ResultsLink", 25);

		public static readonly ExtendedPropertyDefinition PreviewResultsLink = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "PreviewResultsLink", 25);

		public static readonly ExtendedPropertyDefinition LogLevel = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "LogLevel", 14);

		public static readonly ExtendedPropertyDefinition StatusMailRecipients = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "StatusMailRecipients", 26);

		public static readonly ExtendedPropertyDefinition ManagedBy = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ManagedBy", 26);

		public static readonly ExtendedPropertyDefinition Query = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "Query", 25);

		public static readonly ExtendedPropertyDefinition Senders = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "Senders", 26);

		public static readonly ExtendedPropertyDefinition Recipients = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "Recipients", 26);

		public static readonly ExtendedPropertyDefinition StartDate = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "StartDate", 23);

		public static readonly ExtendedPropertyDefinition EndDate = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "EndDate", 23);

		public static readonly ExtendedPropertyDefinition MessageTypes = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "MessageTypes", 26);

		public static readonly ExtendedPropertyDefinition CalculatedQuery = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "CalculatedQuery", 25);

		public static readonly ExtendedPropertyDefinition Language = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "Language", 25);

		public static readonly ExtendedPropertyDefinition CreatedBy = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "CreatedBy", 25);

		public static readonly ExtendedPropertyDefinition LastModifiedBy = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "LastModifiedBy", 25);

		public static readonly ExtendedPropertyDefinition ExcludeDuplicateMessages = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ExcludeDuplicateMessages", 4);

		public static readonly ExtendedPropertyDefinition InPlaceHoldEnabled = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "InPlaceHoldEnabled", 4);

		public static readonly ExtendedPropertyDefinition ItemHoldPeriod = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ItemHoldPeriod", 16);

		public static readonly ExtendedPropertyDefinition InPlaceHoldIdentity = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "InPlaceHoldIdentity", 25);

		public static readonly ExtendedPropertyDefinition LegacySearchObjectIdentity = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "LegacySearchObjectIdentity", 25);

		public static readonly ExtendedPropertyDefinition Description = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "Description", 25);

		public static readonly ExtendedPropertyDefinition ManagedByOrganization = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ManagedByOrganization", 25);

		public static readonly ExtendedPropertyDefinition IncludeKeywordStatistics = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "IncludeKeywordStatistics", 4);

		public static readonly ExtendedPropertyDefinition StatisticsStartIndex = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "StatisticsStartIndex", 14);

		public static readonly ExtendedPropertyDefinition UserKeywords = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "UserKeywords", 26);

		public static readonly ExtendedPropertyDefinition KeywordsQuery = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "KeywordsQuery", 25);

		public static readonly ExtendedPropertyDefinition TotalKeywords = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "TotalKeywords", 14);

		public static readonly ExtendedPropertyDefinition KeywordHits = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "KeywordHits", 26);

		public static readonly ExtendedPropertyDefinition KeywordStatisticsDisabled = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "KeywordStatisticsDisabled", 4);

		public static readonly ExtendedPropertyDefinition PreviewDisabled = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "PreviewDisabled", 4);

		public static readonly ExtendedPropertyDefinition Information = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "Information", 26);

		public static readonly ExtendedPropertyDefinition ScenarioId = new ExtendedPropertyDefinition(MailboxDiscoverySearchExtendedStoreSchema.PropertySetId, "ScenarioId", 25);
	}
}
