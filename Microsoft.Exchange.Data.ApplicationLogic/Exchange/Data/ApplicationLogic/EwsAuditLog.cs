using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class EwsAuditLog : IAuditLog
	{
		public EwsAuditLog(EwsAuditClient ewsClient, FolderIdType auditFolderId, DateTime logRangeStart, DateTime logRangeEnd)
		{
			this.ewsClient = ewsClient;
			this.auditFolderId = auditFolderId;
			this.EstimatedLogStartTime = logRangeStart;
			this.EstimatedLogEndTime = logRangeEnd;
		}

		public DateTime EstimatedLogStartTime { get; private set; }

		public DateTime EstimatedLogEndTime { get; private set; }

		public bool IsAsynchronous
		{
			get
			{
				return false;
			}
		}

		public IAuditQueryContext<TFilter> CreateAuditQueryContext<TFilter>()
		{
			if (typeof(TFilter) != typeof(RestrictionType) && typeof(TFilter) != typeof(QueryStringType))
			{
				throw new NotSupportedException();
			}
			return (IAuditQueryContext<TFilter>)new EwsAuditLog.EwsAuditLogQueryContext(this);
		}

		public int WriteAuditRecord(IAuditLogRecord auditRecord)
		{
			this.ewsClient.RefreshUrl(false);
			int result = 0;
			CreateItemType createItemType = EwsAuditLog.GetCreateItemType(auditRecord, this.auditFolderId, out result);
			this.ewsClient.CreateItem(createItemType);
			return result;
		}

		private static CreateItemType GetCreateItemType(IAuditLogRecord auditRecord, BaseFolderIdType targetFolderId, out int recordSize)
		{
			string asString = AuditLogParseSerialize.GetAsString(auditRecord);
			string text = string.Format("{0} : {1}", auditRecord.UserId, auditRecord.Operation);
			string text2 = string.Format("{0}{1}", auditRecord.UserId, "audit");
			string text3 = string.Format("{0}{1}", auditRecord.ObjectId, "audit");
			recordSize = Encoding.Unicode.GetByteCount(text) + Encoding.Unicode.GetByteCount(asString) + Encoding.Unicode.GetByteCount(text2) + Encoding.Unicode.GetByteCount(text3);
			return new CreateItemType
			{
				MessageDisposition = MessageDispositionType.SaveOnly,
				MessageDispositionSpecified = true,
				SavedItemFolderId = new TargetFolderIdType
				{
					Item = targetFolderId
				},
				Items = new NonEmptyArrayOfAllItemsType
				{
					Items = new ItemType[]
					{
						new MessageType
						{
							ItemClass = "IPM.AuditLog",
							Subject = text,
							Body = new BodyType
							{
								Value = asString,
								BodyType1 = BodyTypeType.Text
							},
							From = new SingleRecipientType
							{
								Item = new EmailAddressType
								{
									Name = text2
								}
							},
							ToRecipients = new EmailAddressType[]
							{
								new EmailAddressType
								{
									Name = text3
								}
							}
						}
					}
				}
			};
		}

		private EwsAuditClient ewsClient;

		private FolderIdType auditFolderId;

		private class EwsAuditLogQueryContext : DisposableObject, IAuditQueryContext<RestrictionType>, IAuditQueryContext<QueryStringType>, IDisposable
		{
			public EwsAuditLogQueryContext(EwsAuditLog auditLog)
			{
				this.auditLog = auditLog;
				this.pendingAsyncResult = null;
			}

			public IAsyncResult BeginAuditLogQuery(RestrictionType queryFilter, int maximumResultsCount)
			{
				if (this.pendingAsyncResult != null)
				{
					throw new InvalidOperationException("Asynchronous query is already pending.");
				}
				this.pendingAsyncResult = new CompletedAsyncResult();
				this.queryFilter = queryFilter;
				return this.pendingAsyncResult;
			}

			public IAsyncResult BeginAuditLogQuery(QueryStringType queryString, int maximumResultsCount)
			{
				if (this.pendingAsyncResult != null)
				{
					throw new InvalidOperationException("Asynchronous query is already pending.");
				}
				this.pendingAsyncResult = new CompletedAsyncResult();
				this.queryString = queryString;
				return this.pendingAsyncResult;
			}

			public IEnumerable<T> EndAuditLogQuery<T>(IAsyncResult asyncResult, IAuditQueryStrategy<T> queryStrategy)
			{
				foreach (ItemType item in this.FindItemsPaged())
				{
					EwsAuditLog.EwsItemPropertyBag itemAsPropertyBag = new EwsAuditLog.EwsItemPropertyBag(item);
					bool stopNow;
					bool match = queryStrategy.RecordFilter(itemAsPropertyBag, out stopNow);
					if (stopNow)
					{
						break;
					}
					if (match)
					{
						yield return queryStrategy.Convert(itemAsPropertyBag);
					}
				}
				yield break;
			}

			private IEnumerable<ItemType> FindItemsPaged()
			{
				QueryStringType searchQueryStringToUse = this.queryString;
				bool keepLooking;
				do
				{
					keepLooking = false;
					ItemType[] items;
					if (this.queryString != null)
					{
						items = this.auditLog.ewsClient.FindItemsWithFAST(this.auditLog.auditFolderId, null, EwsAuditLog.EwsAuditLogQueryContext.SortByReceivedTime, searchQueryStringToUse);
					}
					else
					{
						items = this.auditLog.ewsClient.FindItems(this.auditLog.auditFolderId, null, EwsAuditLog.EwsAuditLogQueryContext.SortByReceivedTime, this.queryFilter);
					}
					DateTime? earliestReceivedTime = null;
					bool useLessOrEqualFilter = false;
					if (this.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug<int>(0L, "Query results returned: {0}.", items.Length);
					}
					if (EwsAuditLog.EwsAuditLogQueryContext.IsSearchResultLimitedByFast(this.queryString, items))
					{
						if (this.IsTraceEnabled(TraceType.DebugTrace))
						{
							this.Tracer.TraceDebug(0L, "Query results may be limited by FAST.");
						}
						if (items[items.Length - 1].DateTimeReceivedSpecified)
						{
							earliestReceivedTime = new DateTime?(items[items.Length - 1].DateTimeReceived);
							useLessOrEqualFilter = (items[0].DateTimeReceived != earliestReceivedTime.Value);
							keepLooking = true;
							if (this.IsTraceEnabled(TraceType.DebugTrace))
							{
								this.Tracer.TraceDebug<DateTime?, bool>(0L, "Earliest item seen=[{0}] useLessOrEqualFilter=[{1}].", earliestReceivedTime, useLessOrEqualFilter);
							}
						}
					}
					foreach (ItemType item in items)
					{
						yield return item;
					}
					if (keepLooking)
					{
						searchQueryStringToUse = null;
						if (this.queryString != null && this.queryString.Value != null && earliestReceivedTime != null)
						{
							ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, earliestReceivedTime.Value);
							StringBuilder stringBuilder = new StringBuilder(this.queryString.Value);
							AqsQueryBuilder.AppendDateClause(stringBuilder, PropertyKeyword.Received, useLessOrEqualFilter ? DateRangeQueryOperation.LessThanOrEqual : DateRangeQueryOperation.LessThan, exDateTime.UniversalTime);
							searchQueryStringToUse = new QueryStringType
							{
								Value = stringBuilder.ToString()
							};
						}
						keepLooking = (searchQueryStringToUse != null);
					}
					if (this.IsTraceEnabled(TraceType.DebugTrace))
					{
						this.Tracer.TraceDebug<bool>(0L, "Make another query: [{0}]", keepLooking);
					}
				}
				while (keepLooking);
				yield break;
			}

			private static bool IsSearchResultLimitedByFast(QueryStringType queryFilter, ItemType[] searchResult)
			{
				return queryFilter != null && queryFilter.Value != null && searchResult.Length == 250;
			}

			private Trace Tracer
			{
				get
				{
					return this.auditLog.ewsClient.Tracer;
				}
			}

			private bool IsTraceEnabled(TraceType traceType)
			{
				return this.Tracer != null && this.Tracer.IsTraceEnabled(traceType);
			}

			protected override DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<EwsAuditLog.EwsAuditLogQueryContext>(this);
			}

			private static readonly FieldOrderType[] SortByReceivedTime = new FieldOrderType[]
			{
				new FieldOrderType
				{
					Item = new PathToUnindexedFieldType
					{
						FieldURI = UnindexedFieldURIType.itemDateTimeReceived
					},
					Order = SortDirectionType.Descending
				}
			};

			private EwsAuditLog auditLog;

			private IAsyncResult pendingAsyncResult;

			private RestrictionType queryFilter;

			private QueryStringType queryString;
		}

		private class EwsItemPropertyBag : IReadOnlyPropertyBag
		{
			public EwsItemPropertyBag(ItemType ewsItem)
			{
				this.ewsItem = ewsItem;
			}

			public object this[PropertyDefinition propertyDefinition]
			{
				get
				{
					object obj = null;
					StorePropertyDefinition storePropertyDefinition = propertyDefinition as StorePropertyDefinition;
					if (storePropertyDefinition != null)
					{
						if (ItemSchema.TextBody.CompareTo(storePropertyDefinition) == 0)
						{
							if (this.ewsItem.ExtendedProperty != null && this.ewsItem.ExtendedProperty.Length > 0)
							{
								foreach (ExtendedPropertyType extendedPropertyType in this.ewsItem.ExtendedProperty)
								{
									if (string.Equals("0x1000", extendedPropertyType.ExtendedFieldURI.PropertyTag))
									{
										obj = (extendedPropertyType.Item as string);
									}
								}
							}
						}
						else if (ItemSchema.Id.CompareTo(storePropertyDefinition) == 0)
						{
							obj = new ConfigObjectId(this.ewsItem.ItemId.Id);
						}
						else if (StoreObjectSchema.CreationTime.CompareTo(storePropertyDefinition) == 0 && this.ewsItem.DateTimeCreatedSpecified)
						{
							obj = new ExDateTime(ExTimeZone.UtcTimeZone, this.ewsItem.DateTimeCreated);
						}
					}
					if (obj == null)
					{
						obj = new PropertyError(propertyDefinition, PropertyErrorCode.NotFound);
					}
					return obj;
				}
			}

			public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
			{
				throw new NotSupportedException();
			}

			private const string BodyPropertyTag = "0x1000";

			private ItemType ewsItem;
		}
	}
}
