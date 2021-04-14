using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Inference;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Mdb;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SentItemsTrainingSubDocumentGenerator
	{
		public SentItemsTrainingSubDocumentGenerator()
		{
			this.NumberOfDaysToSkip = 0;
			this.ItemsToFetchForDefaultModel = 400;
			this.ItemsToFetchForTrainedModel = 1000;
			this.DiagnosticsSession = Microsoft.Exchange.Search.Core.Diagnostics.DiagnosticsSession.CreateComponentDiagnosticsSession("SentItemsTrainingSubDocumentGenerator", ExTraceGlobals.MdbTrainingFeederTracer, (long)this.GetHashCode());
		}

		public IDiagnosticsSession DiagnosticsSession { get; private set; }

		public int NumberOfDaysToSkip { get; set; }

		public int ItemsToFetchForDefaultModel { get; set; }

		public int ItemsToFetchForTrainedModel { get; set; }

		public CrawlerItemIterator<ExDateTime> ItemIterator { get; private set; }

		public ICrawlerFolderIterator FolderIterator { get; private set; }

		internal IDocument RunTrainingQuery(MailboxSession session, PeopleModelItem modelItem)
		{
			int num = this.IsDefaultModel(modelItem) ? this.ItemsToFetchForDefaultModel : this.ItemsToFetchForTrainedModel;
			this.InitializeForQuery(num);
			ExDateTime queryWindowStartTime = this.GetQueryWindowStartTime(modelItem);
			ExDateTime queryWindowEndTime = this.GetQueryWindowEndTime(modelItem);
			IDocument document = MdbInferenceFactory.Current.CreateTrainingSubDocument(this.GetItems(session, queryWindowStartTime, queryWindowEndTime, modelItem), num, session.MailboxGuid, session.MdbGuid);
			this.DiagnosticsSession.TraceDebug<int>("CrawlerItemIterator returned the following number of items for training {0}", document.NestedDocuments.Count);
			return document;
		}

		private void InitializeForQuery(int totalItemsToFetch)
		{
			this.DiagnosticsSession.TraceDebug<int>("Creating CrawlerIterator with selection criteria for default model. Batch Size = {0}", totalItemsToFetch);
			this.FolderIterator = new SentItemsTrainingSubDocumentGenerator.SentItemsFolderIterator();
			this.ItemIterator = new CrawlerItemIterator<ExDateTime>(this.FolderIterator, totalItemsToFetch, ItemSchema.SentTime, (object[] values) => ObjectClass.IsMessage(values[0] as string, false), new PropertyDefinition[]
			{
				StoreObjectSchema.ItemClass
			});
		}

		private IEnumerable<MdbCompositeItemIdentity> GetItems(MailboxSession session, ExDateTime startTime, ExDateTime endTime, PeopleModelItem modelItem)
		{
			return this.ItemIterator.GetItems(session, startTime, endTime);
		}

		private ExDateTime GetQueryWindowStartTime(PeopleModelItem modelItem)
		{
			if (!modelItem.IsDefaultModel)
			{
				return this.LastProcessedTime(modelItem);
			}
			return ExDateTime.UtcNow - SentItemsTrainingSubDocumentGenerator.DefaultTimeSpanBacklogToProcess;
		}

		private ExDateTime GetQueryWindowEndTime(PeopleModelItem modelItem)
		{
			return ExDateTime.UtcNow.AddDays((double)(-(double)this.NumberOfDaysToSkip)).ToUtc();
		}

		private bool IsDefaultModel(PeopleModelItem modelItem)
		{
			return modelItem.IsDefaultModel;
		}

		private ExDateTime LastProcessedTime(PeopleModelItem modelItem)
		{
			return new ExDateTime(ExTimeZone.UtcTimeZone, modelItem.LastProcessedMessageSentTime.ToUniversalTime());
		}

		private const string ComponentName = "SentItemsTrainingSubDocumentGenerator";

		private const int DefaultBatchSizeForDefaultModel = 400;

		private const int DefaultBatchSizeForTrainedModel = 1000;

		private const int DefaultDaysToSkipFromCurrent = 0;

		internal static readonly TimeSpan DefaultTimeSpanBacklogToProcess = TimeSpan.FromDays(180.0);

		private class SentItemsFolderIterator : ICrawlerFolderIterator
		{
			public IEnumerable<StoreObjectId> GetFolders(MailboxSession session)
			{
				yield return session.GetDefaultFolderId(DefaultFolderType.SentItems);
				yield break;
			}
		}
	}
}
