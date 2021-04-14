using System;

namespace Microsoft.Isam.Esent.Interop
{
	public abstract class ColumnValueOfStruct<T> : ColumnValue where T : struct, IEquatable<T>
	{
		public override object ValueAsObject
		{
			get
			{
				return BoxedValueCache<T>.GetBoxedValue(this.Value);
			}
		}

		public T? Value { get; set; }

		public override int Length
		{
			get
			{
				if (this.Value == null)
				{
					return 0;
				}
				return this.Size;
			}
		}

		public override string ToString()
		{
			return this.Value.ToString();
		}

		protected void CheckDataCount(int count)
		{
			if (this.Size != count)
			{
				throw new EsentInvalidColumnException();
			}
		}
	}
}
