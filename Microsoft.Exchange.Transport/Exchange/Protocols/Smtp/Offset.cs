using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class Offset : IEquatable<Offset>
	{
		public int Start { get; set; }

		public int End { get; set; }

		public int Length
		{
			get
			{
				return this.End - this.Start;
			}
		}

		public bool Empty
		{
			get
			{
				return this.Length == 0;
			}
		}

		public Offset(int start = 0, int end = 0)
		{
			Offset.ValidateOffset(start, end);
			this.Start = start;
			this.End = end;
		}

		public void Reset()
		{
			this.Start = 0;
			this.End = 0;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Offset);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public bool Equals(Offset offset)
		{
			return !object.ReferenceEquals(offset, null) && (object.ReferenceEquals(this, offset) || (!(base.GetType() != offset.GetType()) && this.Start == offset.Start && this.End == offset.End));
		}

		private static void ValidateOffset(int start, int end)
		{
			if (start > end)
			{
				throw new ArgumentException(string.Format("The start and end values provided do not form a valid offset {0}:{1}", start, end));
			}
		}
	}
}
