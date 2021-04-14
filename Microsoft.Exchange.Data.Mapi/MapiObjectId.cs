using System;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public abstract class MapiObjectId : ObjectId, IEquatable<MapiObjectId>, IComparable<MapiObjectId>, IComparable
	{
		public static bool operator ==(MapiObjectId operand1, MapiObjectId operand2)
		{
			return object.Equals(operand1, operand2);
		}

		public static bool operator !=(MapiObjectId operand1, MapiObjectId operand2)
		{
			return !object.Equals(operand1, operand2);
		}

		public virtual bool Equals(MapiObjectId other)
		{
			if (object.ReferenceEquals(null, other))
			{
				return false;
			}
			if (this.MapiEntryId != null)
			{
				return object.Equals(this.MapiEntryId, other.MapiEntryId);
			}
			return base.Equals(other);
		}

		public int CompareTo(MapiObjectId other)
		{
			if (null == other)
			{
				throw new ArgumentException("The object is not a MapiObjectId");
			}
			if (!(null == this.MapiEntryId))
			{
				return this.MapiEntryId.CompareTo(other.MapiEntryId);
			}
			if (!(null == other.MapiEntryId))
			{
				return -1;
			}
			return 0;
		}

		int IComparable.CompareTo(object obj)
		{
			return this.CompareTo(obj as MapiObjectId);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MapiObjectId);
		}

		public override int GetHashCode()
		{
			if (!(null == this.MapiEntryId))
			{
				return this.MapiEntryId.GetHashCode();
			}
			return base.GetHashCode();
		}

		public override byte[] GetBytes()
		{
			return (byte[])this.MapiEntryId;
		}

		public MapiEntryId MapiEntryId
		{
			get
			{
				return this.mapiEntryId;
			}
		}

		public MapiObjectId()
		{
		}

		public MapiObjectId(byte[] bytes)
		{
			if (bytes != null)
			{
				this.mapiEntryId = new MapiEntryId(bytes);
			}
		}

		public MapiObjectId(MapiEntryId mapiEntryId)
		{
			this.mapiEntryId = mapiEntryId;
		}

		private readonly MapiEntryId mapiEntryId;
	}
}
