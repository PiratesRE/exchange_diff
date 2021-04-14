using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AnalysisItemsQueryData
	{
		public AnalysisItemsQueryData(object[] item)
		{
			this.key = new AnalysisGroupKey(item);
			this.item = item;
		}

		public AnalysisGroupKey Key
		{
			get
			{
				return this.key;
			}
		}

		public StoreObjectId Id
		{
			get
			{
				return StoreId.GetStoreObjectId((StoreId)this.item[4]);
			}
		}

		public int Size
		{
			get
			{
				return (int)this.item[5];
			}
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				return (ExDateTime)this.item[6];
			}
		}

		public string ClientInfo
		{
			get
			{
				string text = (this.item[7] as string) ?? string.Empty;
				string text2 = (this.item[8] as string) ?? string.Empty;
				string text3 = (this.item[9] as string) ?? string.Empty;
				return string.Concat(new string[]
				{
					text,
					" \\ ",
					text2,
					" \\ ",
					text3
				});
			}
		}

		private AnalysisGroupKey key;

		private object[] item;
	}
}
