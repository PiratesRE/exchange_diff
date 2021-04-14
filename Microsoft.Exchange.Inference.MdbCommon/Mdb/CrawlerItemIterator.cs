using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.MdbCommon;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Inference.Mdb
{
	internal class CrawlerItemIterator<TSort> where TSort : struct, IComparable<TSort>
	{
		public CrawlerItemIterator(ICrawlerFolderIterator folderIterator, int maxRowCount, PropertyDefinition sortProperty, Predicate<object[]> filterPredicate, params PropertyDefinition[] predicateProperties)
		{
			Util.ThrowOnNullArgument(folderIterator, "folderIterator");
			Util.ThrowOnNullArgument(sortProperty, "sortProperty");
			if (filterPredicate != null)
			{
				Util.ThrowOnNullArgument(predicateProperties, "predicateProperties");
			}
			else if (predicateProperties != null && predicateProperties.Length > 0)
			{
				throw new ArgumentException("Predicate properties should not be specified when there's no filter supplied.");
			}
			this.folderIterator = folderIterator;
			this.maxRowCount = Math.Min(maxRowCount, 10000);
			this.sortProperty = sortProperty;
			this.filterPredicate = filterPredicate;
			this.predicateProperties = predicateProperties;
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("CrawlerItemIterator", ComponentInstance.Globals.Search.ServiceName, ExTraceGlobals.MdbCrawlerFeederTracer, (long)this.GetHashCode());
		}

		public IEnumerable<MdbCompositeItemIdentity> GetItems(MailboxSession session, TSort intervalStart, TSort intervalStop)
		{
			Util.ThrowOnNullArgument(session, "session");
			List<CrawlerItemIterator<TSort>.QueryResultWrapper> resultsToDispose = new List<CrawlerItemIterator<TSort>.QueryResultWrapper>();
			SortedList<CrawlerItemIterator<TSort>.SortKey, CrawlerItemIterator<TSort>.QueryResultWrapper> mergeList = new SortedList<CrawlerItemIterator<TSort>.SortKey, CrawlerItemIterator<TSort>.QueryResultWrapper>();
			try
			{
				using (IEnumerator<StoreObjectId> enumerator = this.folderIterator.GetFolders(session).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StoreObjectId folderId = enumerator.Current;
						CrawlerItemIterator<TSort>.QueryResultWrapper queryResultWrapper = new CrawlerItemIterator<TSort>.QueryResultWrapper(this.diagnosticsSession, session, folderId, this.sortProperty, intervalStart, intervalStop, this.filterPredicate, this.predicateProperties, this.maxRowCount);
						resultsToDispose.Add(queryResultWrapper);
						queryResultWrapper.CanCacheQueryResult = (50 < mergeList.Count);
						if (queryResultWrapper.MoveNext())
						{
							mergeList.Add(queryResultWrapper.CurrentValue, queryResultWrapper);
						}
					}
					goto IL_217;
				}
				IL_11D:
				int cacheCounter = 0;
				using (IEnumerator<CrawlerItemIterator<TSort>.QueryResultWrapper> enumerator2 = mergeList.Values.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						CrawlerItemIterator<TSort>.QueryResultWrapper result = enumerator2.Current;
						yield return result.CurrentIdentity;
						mergeList.RemoveAt(0);
						if (result.MoveNext())
						{
							mergeList.Add(result.CurrentValue, result);
							if (cacheCounter > 0 && !result.CanCacheQueryResult)
							{
								result.CanCacheQueryResult = true;
								cacheCounter--;
							}
						}
						else if (result.CanCacheQueryResult)
						{
							cacheCounter++;
						}
					}
				}
				IL_217:
				if (mergeList.Count > 0)
				{
					goto IL_11D;
				}
			}
			finally
			{
				foreach (CrawlerItemIterator<TSort>.QueryResultWrapper queryResultWrapper2 in resultsToDispose)
				{
					queryResultWrapper2.Dispose();
				}
				resultsToDispose.Clear();
			}
			yield break;
		}

		private const int MaxOutstandingQueries = 50;

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly ICrawlerFolderIterator folderIterator;

		private readonly int maxRowCount;

		private readonly PropertyDefinition sortProperty;

		private readonly Predicate<object[]> filterPredicate;

		private readonly PropertyDefinition[] predicateProperties;

		private struct SortKey : IComparable<CrawlerItemIterator<TSort>.SortKey>
		{
			public SortKey(TSort value, int documentid, StoreObjectId folderId, bool isAscending)
			{
				this.value = value;
				this.documentId = documentid;
				this.folderId = folderId;
				this.isAscending = isAscending;
			}

			public TSort Value
			{
				get
				{
					return this.value;
				}
			}

			public int CompareTo(CrawlerItemIterator<TSort>.SortKey other)
			{
				TSort tsort = this.value;
				int num = tsort.CompareTo(other.value);
				if (num == 0)
				{
					num = this.documentId.CompareTo(other.documentId);
				}
				if (num == 0)
				{
					num = this.folderId.CompareTo(other.folderId);
				}
				if (!this.isAscending)
				{
					num = -num;
				}
				return num;
			}

			private readonly TSort value;

			private readonly int documentId;

			private readonly bool isAscending;

			private readonly StoreObjectId folderId;
		}

		private class QueryResultWrapper : Disposable
		{
			public QueryResultWrapper(IDiagnosticsSession tracer, MailboxSession session, StoreObjectId folderId, PropertyDefinition sortProperty, TSort intervalStart, TSort intervalStop, Predicate<object[]> filterPredicate, PropertyDefinition[] predicateProperties, int maxRowCount)
			{
				Util.ThrowOnNullArgument(session, "session");
				Util.ThrowOnNullArgument(folderId, "folderId");
				Util.ThrowOnNullArgument(sortProperty, "sortProperty");
				int num = 0;
				if (filterPredicate != null)
				{
					Util.ThrowOnNullArgument(predicateProperties, "predicateProperties");
					num = predicateProperties.Length;
				}
				else if (predicateProperties != null && predicateProperties.Length > 0)
				{
					throw new ArgumentException("Predicate properties should not be specified when there's no filter supplied.");
				}
				this.diagnosticsSession = tracer;
				this.maxRowCount = maxRowCount;
				this.session = session;
				this.folderId = folderId;
				this.sortProperty = sortProperty;
				this.intervalStart = intervalStart;
				this.intervalStop = intervalStop;
				this.filterPredicate = filterPredicate;
				this.propertiesToRequest = new PropertyDefinition[3 + num];
				this.propertiesToRequest[0] = ItemSchema.Id;
				this.propertiesToRequest[1] = sortProperty;
				this.propertiesToRequest[2] = ItemSchema.DocumentId;
				for (int i = 0; i < num; i++)
				{
					this.propertiesToRequest[i + 3] = predicateProperties[i];
				}
				this.isAscending = (this.intervalStart.CompareTo(this.intervalStop) < 0);
				if (this.sortProperty == ItemSchema.DocumentId)
				{
					this.sortOrder = new SortBy[]
					{
						new SortBy(ItemSchema.DocumentId, this.isAscending ? SortOrder.Ascending : SortOrder.Descending)
					};
					return;
				}
				this.sortOrder = new SortBy[]
				{
					new SortBy(sortProperty, this.isAscending ? SortOrder.Ascending : SortOrder.Descending),
					new SortBy(ItemSchema.DocumentId, SortOrder.Ascending)
				};
			}

			public bool CanCacheQueryResult
			{
				get
				{
					return this.canCacheResult;
				}
				set
				{
					this.canCacheResult = value;
				}
			}

			public CrawlerItemIterator<TSort>.SortKey CurrentValue
			{
				get
				{
					if (this.enumerator == null)
					{
						throw new InvalidOperationException();
					}
					return new CrawlerItemIterator<TSort>.SortKey((TSort)((object)this.enumerator.Current[1]), this.CurrentDocumentId, this.folderId, this.isAscending);
				}
			}

			public MdbCompositeItemIdentity CurrentIdentity
			{
				get
				{
					if (this.enumerator == null)
					{
						throw new InvalidOperationException();
					}
					return new MdbCompositeItemIdentity(this.session.MdbGuid, this.session.MailboxGuid, (int)this.session.Mailbox.TryGetProperty(MailboxSchema.MailboxNumber), (this.enumerator.Current[0] as VersionedId).ObjectId, this.CurrentDocumentId);
				}
			}

			private int CurrentDocumentId
			{
				get
				{
					if (this.enumerator == null)
					{
						throw new InvalidOperationException();
					}
					return (int)this.enumerator.Current[2];
				}
			}

			private IEnumerable<object[]> Entries
			{
				get
				{
					Folder folder = null;
					QueryResult result = null;
					try
					{
						TSort lastReturnedSortKey = this.intervalStart;
						int lastReturnedDocumentId = 0;
						object[][] entries = this.GetRowsFromQuery(ref folder, ref result, lastReturnedSortKey, lastReturnedDocumentId);
						while (entries.Length > 0)
						{
							foreach (object[] entry in entries)
							{
								yield return entry;
							}
							object[] lastEntry = entries[entries.Length - 1];
							lastReturnedSortKey = (TSort)((object)lastEntry[1]);
							lastReturnedDocumentId = (int)lastEntry[2];
							entries = this.GetRowsFromQuery(ref folder, ref result, lastReturnedSortKey, lastReturnedDocumentId);
						}
					}
					finally
					{
						if (result != null)
						{
							result.Dispose();
						}
						if (folder != null)
						{
							folder.Dispose();
						}
					}
					yield break;
				}
			}

			public bool MoveNext()
			{
				if (this.isCompleted)
				{
					return false;
				}
				if (this.enumerator == null)
				{
					this.enumerator = this.Entries.GetEnumerator();
				}
				object[] array = (this.filterPredicate != null) ? new object[this.propertiesToRequest.Length - 3] : null;
				while (this.enumerator.MoveNext())
				{
					if (this.isAscending)
					{
						TSort tsort = this.intervalStop;
						if (tsort.CompareTo(this.CurrentValue.Value) < 0)
						{
							break;
						}
					}
					if (!this.isAscending)
					{
						TSort tsort2 = this.intervalStop;
						if (tsort2.CompareTo(this.CurrentValue.Value) > 0)
						{
							break;
						}
					}
					if (this.filterPredicate != null)
					{
						Array.Copy(this.enumerator.Current, 3, array, 0, array.Length);
						if (!this.filterPredicate(array))
						{
							continue;
						}
					}
					return true;
				}
				this.isCompleted = true;
				this.enumerator.Dispose();
				this.enumerator = null;
				return false;
			}

			protected override void InternalDispose(bool calledFromDispose)
			{
				if (calledFromDispose && this.enumerator != null)
				{
					this.enumerator.Dispose();
					this.enumerator = null;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<CrawlerItemIterator<TSort>.QueryResultWrapper>(this);
			}

			private object[][] GetRowsFromQuery(ref Folder folder, ref QueryResult result, TSort lastReturnedSortKey, int lastReturnedDocumentId)
			{
				QueryResult tempQueryResult = result;
				Folder tempFolder = folder;
				folder = null;
				result = null;
				object[][] result2;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					if (tempFolder == null)
					{
						tempFolder = XsoUtil.TranslateXsoExceptionsWithReturnValue<Folder>(this.diagnosticsSession, Strings.ConnectionToMailboxFailed(this.session.MailboxGuid), () => Folder.Bind(this.session, this.folderId));
						if (tempFolder == null)
						{
							return CrawlerItemIterator<TSort>.QueryResultWrapper.EmptyResult;
						}
						disposeGuard.Add<Folder>(tempFolder);
						tempQueryResult = XsoUtil.TranslateXsoExceptionsWithReturnValue<QueryResult>(this.diagnosticsSession, Strings.ConnectionToMailboxFailed(this.session.MailboxGuid), XsoUtil.XsoExceptionHandlingFlags.DoNotExpectObjectNotFound, () => tempFolder.ItemQuery(ItemQueryType.None, null, this.sortOrder, this.propertiesToRequest));
						if (tempQueryResult == null)
						{
							return CrawlerItemIterator<TSort>.QueryResultWrapper.EmptyResult;
						}
						disposeGuard.Add<QueryResult>(tempQueryResult);
						bool isSeekSuccessful = false;
						XsoUtil.TranslateXsoExceptions(this.diagnosticsSession, Strings.ConnectionToMailboxFailed(this.session.MailboxGuid), XsoUtil.XsoExceptionHandlingFlags.DoNotExpectObjectNotFound | XsoUtil.XsoExceptionHandlingFlags.DoNotExpectCorruptData, delegate()
						{
							if (this.sortProperty == ItemSchema.DocumentId)
							{
								tempQueryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(this.isAscending ? ComparisonOperator.GreaterThan : ComparisonOperator.LessThan, ItemSchema.DocumentId, lastReturnedSortKey), SeekToConditionFlags.None);
							}
							else
							{
								tempQueryResult.SeekToCondition(SeekReference.OriginBeginning, new OrFilter(new QueryFilter[]
								{
									new AndFilter(new QueryFilter[]
									{
										new ComparisonFilter(this.isAscending ? ComparisonOperator.GreaterThanOrEqual : ComparisonOperator.LessThanOrEqual, this.sortProperty, lastReturnedSortKey),
										new ComparisonFilter(ComparisonOperator.GreaterThan, ItemSchema.DocumentId, lastReturnedDocumentId)
									}),
									new ComparisonFilter(this.isAscending ? ComparisonOperator.GreaterThan : ComparisonOperator.LessThan, this.sortProperty, lastReturnedSortKey)
								}), SeekToConditionFlags.AllowExtendedFilters);
							}
							isSeekSuccessful = true;
						});
						if (!isSeekSuccessful)
						{
							return CrawlerItemIterator<TSort>.QueryResultWrapper.EmptyResult;
						}
					}
					else
					{
						disposeGuard.Add<Folder>(tempFolder);
						disposeGuard.Add<QueryResult>(tempQueryResult);
					}
					object[][] array = XsoUtil.TranslateXsoExceptionsWithReturnValue<object[][]>(this.diagnosticsSession, Strings.ConnectionToMailboxFailed(this.session.MailboxGuid), XsoUtil.XsoExceptionHandlingFlags.DoNotExpectObjectNotFound | XsoUtil.XsoExceptionHandlingFlags.DoNotExpectCorruptData, () => tempQueryResult.GetRows(this.maxRowCount));
					if (array == null)
					{
						result2 = CrawlerItemIterator<TSort>.QueryResultWrapper.EmptyResult;
					}
					else
					{
						if (this.canCacheResult)
						{
							folder = tempFolder;
							result = tempQueryResult;
							disposeGuard.Success();
						}
						result2 = array;
					}
				}
				return result2;
			}

			private static readonly object[][] EmptyResult = new object[0][];

			private readonly IDiagnosticsSession diagnosticsSession;

			private readonly int maxRowCount;

			private readonly MailboxSession session;

			private readonly StoreObjectId folderId;

			private readonly PropertyDefinition sortProperty;

			private readonly TSort intervalStart;

			private readonly TSort intervalStop;

			private readonly Predicate<object[]> filterPredicate;

			private readonly PropertyDefinition[] propertiesToRequest;

			private readonly SortBy[] sortOrder;

			private readonly bool isAscending;

			private IEnumerator<object[]> enumerator;

			private bool isCompleted;

			private bool canCacheResult = true;

			private enum PropertyValueIndex
			{
				Id,
				SortProperty,
				DocumentId,
				PredicatePropertiesBase
			}
		}
	}
}
