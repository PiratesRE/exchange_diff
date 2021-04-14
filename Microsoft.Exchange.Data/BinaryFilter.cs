using System;
using System.Text;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class BinaryFilter : ContentFilter
	{
		public BinaryFilter(PropertyDefinition property, byte[] binaryData, MatchOptions matchOptions, MatchFlags matchFlags) : base(property, matchOptions, matchFlags)
		{
			this.binaryData = binaryData;
		}

		public byte[] BinaryData
		{
			get
			{
				return this.binaryData;
			}
		}

		public override SinglePropertyFilter CloneWithAnotherProperty(PropertyDefinition property)
		{
			base.CheckClonable(property);
			return new BinaryFilter(property, this.binaryData, base.MatchOptions, base.MatchFlags);
		}

		public override bool Equals(object obj)
		{
			BinaryFilter binaryFilter = obj as BinaryFilter;
			return binaryFilter != null && base.MatchFlags == binaryFilter.MatchFlags && base.MatchOptions == binaryFilter.MatchOptions && this.BinaryEquals(binaryFilter.binaryData) && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.ComputeHashCode();
		}

		protected override string StringValue
		{
			get
			{
				if (this.binaryData != null)
				{
					return Encoding.UTF8.GetString(this.binaryData, 0, this.binaryData.Length);
				}
				return "<null>";
			}
		}

		private bool BinaryEquals(byte[] bytes)
		{
			ArrayComparer<byte> comparer = ArrayComparer<byte>.Comparer;
			return comparer.Equals(this.binaryData, bytes);
		}

		private int ComputeHashCode()
		{
			ArrayComparer<byte> comparer = ArrayComparer<byte>.Comparer;
			return comparer.GetHashCode(this.binaryData);
		}

		private readonly byte[] binaryData;
	}
}
