using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class SearchResults : ISearchResults
	{
		internal SearchResults(SourceInformationCollection sources, ITarget target)
		{
			this.sourceMailboxes = sources;
			this.target = target;
		}

		public IEnumerable<IDictionary<string, object>> SearchResultItems
		{
			get
			{
				foreach (SourceInformation source in this.sourceMailboxes.Values)
				{
					IItemIdList itemIdList = this.target.CreateItemIdList(source.Configuration.Id, false);
					if (itemIdList.Exists)
					{
						foreach (ItemId itemId in itemIdList.ReadItemIds())
						{
							if (!itemId.IsDuplicate)
							{
								yield return new Dictionary<string, object>
								{
									{
										"Id",
										itemId.Id
									},
									{
										"DocumentId",
										itemId.DocumentId
									},
									{
										"Size",
										itemId.Size
									},
									{
										"OriginalPath",
										itemId.ParentFolder
									},
									{
										"Subject",
										itemId.Subject
									},
									{
										"Sender",
										itemId.Sender
									},
									{
										"SenderSmtpAddress",
										itemId.SenderSmtpAddress
									},
									{
										"SentTime",
										itemId.SentTime
									},
									{
										"ReceivedTime",
										itemId.ReceivedTime
									},
									{
										"Summary",
										itemId.BodyPreview
									},
									{
										"ToRecipients",
										itemId.ToRecipients
									},
									{
										"CcRecipients",
										itemId.CcRecipients
									},
									{
										"BccRecipients",
										itemId.BccRecipients
									},
									{
										"ToGroupExpansionRecipients",
										itemId.ToGroupExpansionRecipients
									},
									{
										"CcGroupExpansionRecipients",
										itemId.CcGroupExpansionRecipients
									},
									{
										"BccGroupExpansionRecipients",
										itemId.BccGroupExpansionRecipients
									},
									{
										"GroupExpansionError",
										(itemId.DGGroupExpansionError == null) ? string.Empty : itemId.DGGroupExpansionError.ToString()
									}
								};
							}
						}
					}
				}
				yield break;
			}
		}

		public IEnumerable<IDictionary<string, object>> UnsearchableItems
		{
			get
			{
				foreach (SourceInformation source in this.sourceMailboxes.Values)
				{
					IItemIdList itemIdList = this.target.CreateItemIdList(source.Configuration.Id, true);
					if (itemIdList.Exists)
					{
						foreach (ItemId itemId2 in itemIdList.ReadItemIds())
						{
							UnsearchableItemId itemId = (UnsearchableItemId)itemId2;
							if (!itemId.IsDuplicate)
							{
								yield return new Dictionary<string, object>
								{
									{
										"Id",
										itemId.Id
									},
									{
										"Size",
										itemId.Size
									},
									{
										"OriginalPath",
										itemId.ParentFolder
									},
									{
										"Subject",
										itemId.Subject
									},
									{
										"Sender",
										itemId.Sender
									},
									{
										"SentTime",
										itemId.SentTime
									},
									{
										"ReceivedTime",
										itemId.ReceivedTime
									},
									{
										"Summary",
										itemId.BodyPreview
									},
									{
										"ToRecipients",
										itemId.ToRecipients
									},
									{
										"CcRecipients",
										itemId.CcRecipients
									},
									{
										"BccRecipients",
										itemId.BccRecipients
									},
									{
										"ErrorCode",
										itemId.ErrorCode
									},
									{
										"ErrorDescription",
										itemId.ErrorDescription
									},
									{
										"AdditionalInformation",
										itemId.AdditionalInformation
									},
									{
										"LastAttemptTime",
										itemId.LastAttemptTime
									}
								};
							}
						}
					}
				}
				yield break;
			}
		}

		public int SearchResultItemsCount
		{
			get
			{
				return this.sourceMailboxes.Values.Sum(delegate(SourceInformation x)
				{
					if (x.Status.ItemCount < 0)
					{
						return 0;
					}
					return x.Status.ItemCount;
				});
			}
		}

		public int ProcessedItemCount
		{
			get
			{
				return this.sourceMailboxes.Values.Sum((SourceInformation x) => x.Status.ProcessedItemCount);
			}
		}

		public int UnsearchableItemsCount
		{
			get
			{
				return this.sourceMailboxes.Values.Sum(delegate(SourceInformation x)
				{
					if (x.Status.UnsearchableItemCount < 0)
					{
						return 0;
					}
					return x.Status.UnsearchableItemCount;
				});
			}
		}

		public int ProcessedUnsearchableItemCount
		{
			get
			{
				return this.sourceMailboxes.Values.Sum((SourceInformation x) => x.Status.ProcessedUnsearchableItemCount);
			}
		}

		public long TotalSize
		{
			get
			{
				return this.sourceMailboxes.Values.Sum((SourceInformation x) => x.Status.TotalSize);
			}
		}

		public string ItemIdKey
		{
			get
			{
				return "Id";
			}
		}

		public long EstimatedItemCount { get; set; }

		public long EstimatedUnsearchableItemCount { get; set; }

		public long DuplicateItemCount
		{
			get
			{
				return this.sourceMailboxes.Values.Sum((SourceInformation x) => x.Status.DuplicateItemCount);
			}
		}

		public long UnsearchableDuplicateItemCount
		{
			get
			{
				return this.sourceMailboxes.Values.Sum((SourceInformation x) => x.Status.UnsearchableDuplicateItemCount);
			}
		}

		public long ErrorItemCount
		{
			get
			{
				return this.sourceMailboxes.Values.Sum((SourceInformation x) => x.Status.ErrorItemCount);
			}
		}

		public void IncrementErrorItemCount(string sourceId)
		{
			if (!string.IsNullOrWhiteSpace(sourceId))
			{
				this.sourceMailboxes[sourceId].Status.ErrorItemCount += 1L;
			}
		}

		public ISourceStatus GetSourceStatusBySourceId(string sourceId)
		{
			SourceInformation sourceInformation = this.sourceMailboxes.Values.FirstOrDefault((SourceInformation x) => x.Configuration.Id == sourceId);
			if (sourceInformation != null)
			{
				return sourceInformation.Status;
			}
			return null;
		}

		private SourceInformationCollection sourceMailboxes;

		private ITarget target;
	}
}
