using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal struct StoreIdPair : IEquatable<StoreIdPair>
	{
		public StoreIdPair(StoreId first, StoreId second)
		{
			this.first = first;
			this.second = second;
		}

		public StoreId First
		{
			get
			{
				return this.first;
			}
		}

		public StoreId Second
		{
			get
			{
				return this.second;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is StoreIdPair && this.Equals((StoreIdPair)obj);
		}

		public override int GetHashCode()
		{
			int hashCode = this.first.GetHashCode();
			int hashCode2 = this.second.GetHashCode();
			return hashCode ^ hashCode2;
		}

		public bool Equals(StoreIdPair other)
		{
			return this.first == other.first && this.second == other.second;
		}

		public override string ToString()
		{
			return string.Format("{0}/{1}", this.first.ToString(), this.second.ToString());
		}

		internal static StoreIdPair Parse(Reader reader)
		{
			StoreId storeId = StoreId.Parse(reader);
			StoreId storeId2 = StoreId.Parse(reader);
			return new StoreIdPair(storeId, storeId2);
		}

		internal void Serialize(Writer writer)
		{
			this.first.Serialize(writer);
			this.second.Serialize(writer);
		}

		private readonly StoreId first;

		private readonly StoreId second;

		public static readonly StoreIdPair Empty = new StoreIdPair(StoreId.Empty, StoreId.Empty);
	}
}
