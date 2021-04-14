using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public struct FidMid : IEquatable<FidMid>, IComparable<FidMid>
	{
		public FidMid(ExchangeId folderId, ExchangeId messageId)
		{
			this.folderId = folderId;
			this.messageId = messageId;
		}

		public ExchangeId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public ExchangeId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public bool Equals(FidMid other)
		{
			return this.MessageId == other.MessageId && this.FolderId == other.FolderId;
		}

		public override bool Equals(object obj)
		{
			return obj is FidMid && ((FidMid)obj).Equals(this);
		}

		public override int GetHashCode()
		{
			return this.folderId.GetHashCode() ^ this.messageId.GetHashCode();
		}

		public int CompareTo(FidMid other)
		{
			int num = this.FolderId.CompareTo(other.FolderId);
			if (num == 0)
			{
				num = this.MessageId.CompareTo(other.MessageId);
			}
			return num;
		}

		public override string ToString()
		{
			return string.Format("FidMid({0}:{1})", this.folderId, this.messageId);
		}

		private readonly ExchangeId folderId;

		private readonly ExchangeId messageId;
	}
}
