using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal struct ValuePosition
	{
		public ValuePosition(int line, int offset)
		{
			this.Line = line;
			this.Offset = offset;
		}

		public static bool operator ==(ValuePosition pos1, ValuePosition pos2)
		{
			return pos1.Line == pos2.Line && pos1.Offset == pos2.Offset;
		}

		public static bool operator !=(ValuePosition pos1, ValuePosition pos2)
		{
			return !(pos1 == pos2);
		}

		public override bool Equals(object rhs)
		{
			if (rhs is ValuePosition)
			{
				ValuePosition valuePosition = (ValuePosition)rhs;
				return this.Line == valuePosition.Line && this.Offset == valuePosition.Offset;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Line * 1000 + this.Offset;
		}

		public int Line;

		public int Offset;
	}
}
