using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	internal class BodyPreference
	{
		public bool AllOrNone
		{
			get
			{
				return this.allOrNone;
			}
			set
			{
				this.allOrNone = value;
			}
		}

		public long TruncationSize
		{
			get
			{
				return this.truncationSize;
			}
			set
			{
				this.truncationSize = value;
			}
		}

		public virtual BodyType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public int Preview
		{
			get
			{
				return this.preview;
			}
			set
			{
				this.preview = value;
			}
		}

		public BodyPreference Clone()
		{
			return new BodyPreference
			{
				AllOrNone = this.AllOrNone,
				TruncationSize = this.TruncationSize,
				Type = this.Type,
				Preview = this.Preview
			};
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Type: ",
				this.Type,
				", AllOrNone: ",
				this.AllOrNone,
				", TruncationSize: ",
				this.TruncationSize,
				", Preview: ",
				this.Preview
			});
		}

		private bool allOrNone;

		private long truncationSize = -1L;

		private BodyType type;

		private int preview;
	}
}
