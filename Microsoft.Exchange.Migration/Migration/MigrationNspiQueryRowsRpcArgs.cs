using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationNspiQueryRowsRpcArgs : MigrationNspiRpcArgs
	{
		public MigrationNspiQueryRowsRpcArgs(ExchangeOutlookAnywhereEndpoint endpoint, int? batchSize, int? startIndex, long[] longPropTags) : base(endpoint, MigrationProxyRpcType.QueryRows)
		{
			this.BatchSize = batchSize;
			this.StartIndex = startIndex;
			this.LongPropTags = longPropTags;
		}

		public MigrationNspiQueryRowsRpcArgs(byte[] requestBlob) : base(requestBlob, MigrationProxyRpcType.QueryRows)
		{
		}

		public int? BatchSize
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2416312323U, out obj) && obj is int)
				{
					return new int?((int)obj);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.PropertyCollection[2416312323U] = value.Value;
					return;
				}
				this.PropertyCollection.Remove(2416312323U);
			}
		}

		public int? StartIndex
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2416377859U, out obj) && obj is int)
				{
					return new int?((int)obj);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.PropertyCollection[2416377859U] = value.Value;
					return;
				}
				this.PropertyCollection.Remove(2416377859U);
			}
		}

		public long[] LongPropTags
		{
			private get
			{
				return base.GetProperty<long[]>(2416447508U);
			}
			set
			{
				base.SetProperty(2416447508U, value);
			}
		}

		public PropTag[] PropTags
		{
			get
			{
				long[] longPropTags = this.LongPropTags;
				if (longPropTags == null)
				{
					return null;
				}
				PropTag[] array = new PropTag[longPropTags.Length];
				for (int i = 0; i < longPropTags.Length; i++)
				{
					array[i] = (PropTag)longPropTags[i];
				}
				return array;
			}
		}

		public override bool Validate(out string errorMsg)
		{
			if (!base.Validate(out errorMsg))
			{
				return false;
			}
			if (this.StartIndex == null || this.StartIndex.Value < 0)
			{
				errorMsg = "Invalid Start Index.";
				return false;
			}
			if (this.BatchSize == null || this.BatchSize.Value < 1)
			{
				errorMsg = "Invalid Batch Size.";
				return false;
			}
			if (this.LongPropTags == null || this.LongPropTags.Length == 0)
			{
				errorMsg = "PropTags cannot be null.";
				return false;
			}
			errorMsg = null;
			return true;
		}
	}
}
