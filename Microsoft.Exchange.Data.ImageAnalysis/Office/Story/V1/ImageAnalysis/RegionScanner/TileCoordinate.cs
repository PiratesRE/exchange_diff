using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Office.Story.V1.ImageAnalysis.RegionScanner
{
	[DataContract]
	[Serializable]
	internal struct TileCoordinate
	{
		[DataMember]
		public ushort Index { get; private set; }

		public TileCoordinate(int index)
		{
			this = default(TileCoordinate);
			this.Index = checked((ushort)index);
		}

		public override bool Equals(object obj)
		{
			return obj is TileCoordinate && this == (TileCoordinate)obj;
		}

		public override int GetHashCode()
		{
			return this.Index.GetHashCode();
		}

		public static bool operator ==(TileCoordinate one, TileCoordinate another)
		{
			return one.Index == another.Index;
		}

		public static bool operator !=(TileCoordinate one, TileCoordinate another)
		{
			return one.Index != another.Index;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "@{0}", new object[]
			{
				this.Index
			});
		}
	}
}
