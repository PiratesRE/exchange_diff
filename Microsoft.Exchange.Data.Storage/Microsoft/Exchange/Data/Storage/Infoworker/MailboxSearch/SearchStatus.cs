using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class SearchStatus : SearchObjectBase
	{
		public SearchState Status
		{
			get
			{
				return (SearchState)this[SearchStatusSchema.Status];
			}
			set
			{
				this[SearchStatusSchema.Status] = value;
			}
		}

		public ADObjectId LastRunBy
		{
			get
			{
				return (ADObjectId)this[SearchStatusSchema.LastRunBy];
			}
			set
			{
				this[SearchStatusSchema.LastRunBy] = value;
			}
		}

		public string LastRunByEx
		{
			get
			{
				return (string)this[SearchStatusSchema.LastRunByEx];
			}
			set
			{
				this[SearchStatusSchema.LastRunByEx] = value;
			}
		}

		public ExDateTime? LastStartTime
		{
			get
			{
				return (ExDateTime?)this[SearchStatusSchema.LastStartTime];
			}
			set
			{
				this[SearchStatusSchema.LastStartTime] = value;
			}
		}

		public ExDateTime? LastEndTime
		{
			get
			{
				return (ExDateTime?)this[SearchStatusSchema.LastEndTime];
			}
			set
			{
				this[SearchStatusSchema.LastEndTime] = value;
			}
		}

		public int NumberMailboxesToSearch
		{
			get
			{
				return (int)this[SearchStatusSchema.NumberMailboxesToSearch];
			}
			set
			{
				this[SearchStatusSchema.NumberMailboxesToSearch] = value;
			}
		}

		public int PercentComplete
		{
			get
			{
				return (int)this[SearchStatusSchema.PercentComplete];
			}
			set
			{
				this[SearchStatusSchema.PercentComplete] = value;
			}
		}

		public long ResultNumber
		{
			get
			{
				return (long)this[SearchStatusSchema.ResultNumber];
			}
			set
			{
				this[SearchStatusSchema.ResultNumber] = value;
			}
		}

		public long ResultNumberEstimate
		{
			get
			{
				return (long)this[SearchStatusSchema.ResultNumberEstimate];
			}
			set
			{
				this[SearchStatusSchema.ResultNumberEstimate] = value;
			}
		}

		public ByteQuantifiedSize ResultSize
		{
			get
			{
				return (ByteQuantifiedSize)this[SearchStatusSchema.ResultSize];
			}
			set
			{
				this[SearchStatusSchema.ResultSize] = value;
			}
		}

		public ByteQuantifiedSize ResultSizeEstimate
		{
			get
			{
				return (ByteQuantifiedSize)this[SearchStatusSchema.ResultSizeEstimate];
			}
			set
			{
				this[SearchStatusSchema.ResultSizeEstimate] = value;
			}
		}

		public ByteQuantifiedSize ResultSizeCopied
		{
			get
			{
				return (ByteQuantifiedSize)this[SearchStatusSchema.ResultSizeCopied];
			}
			set
			{
				this[SearchStatusSchema.ResultSizeCopied] = value;
			}
		}

		public string ResultsPath
		{
			get
			{
				return (string)this[SearchStatusSchema.ResultsPath];
			}
			set
			{
				this[SearchStatusSchema.ResultsPath] = value;
			}
		}

		public string ResultsLink
		{
			get
			{
				return (string)this[SearchStatusSchema.ResultsLink];
			}
			set
			{
				this[SearchStatusSchema.ResultsLink] = value;
			}
		}

		public MultiValuedProperty<string> Errors
		{
			get
			{
				return (MultiValuedProperty<string>)this[SearchStatusSchema.Errors];
			}
			set
			{
				this[SearchStatusSchema.Errors] = value;
			}
		}

		public MultiValuedProperty<KeywordHit> KeywordHits
		{
			get
			{
				return (MultiValuedProperty<KeywordHit>)this[SearchStatusSchema.KeywordHits];
			}
			set
			{
				this[SearchStatusSchema.KeywordHits] = value;
			}
		}

		public MultiValuedProperty<string> CompletedMailboxes
		{
			get
			{
				return (MultiValuedProperty<string>)this[SearchStatusSchema.CompletedMailboxes];
			}
			set
			{
				this[SearchStatusSchema.CompletedMailboxes] = value;
			}
		}

		internal override ObjectType ObjectType
		{
			get
			{
				return ObjectType.SearchStatus;
			}
		}

		internal override SearchObjectBaseSchema Schema
		{
			get
			{
				return SearchStatus.schema;
			}
		}

		private static SearchStatusSchema schema = ObjectSchema.GetInstance<SearchStatusSchema>();
	}
}
