using System;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CharsetDetectionOptions
	{
		public CharsetDetectionOptions()
		{
		}

		public CharsetDetectionOptions(CharsetDetectionOptions options)
		{
			this.preferredInternetCodePageForShiftJis = options.preferredInternetCodePageForShiftJis;
			this.requiredCoverage = options.requiredCoverage;
			this.preferredCharset = options.preferredCharset;
		}

		public int PreferredInternetCodePageForShiftJis
		{
			get
			{
				return this.preferredInternetCodePageForShiftJis;
			}
			set
			{
				switch (value)
				{
				case 50220:
				case 50221:
					this.preferredInternetCodePageForShiftJis = value;
					return;
				}
				this.preferredInternetCodePageForShiftJis = 50222;
			}
		}

		public Charset PreferredCharset
		{
			get
			{
				return this.preferredCharset;
			}
			set
			{
				if (!value.IsAvailable)
				{
					throw new ArgumentException();
				}
				this.preferredCharset = value;
			}
		}

		public int RequiredCoverage
		{
			get
			{
				return this.requiredCoverage;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					this.requiredCoverage = value;
					return;
				}
				throw new ArgumentOutOfRangeException();
			}
		}

		public override string ToString()
		{
			return string.Format("CharsetDetectionOptions:\r\n- preferredInternetCodePageForShiftJis: {0}\r\n- requiredCoverage:  {1}\r\n- preferredCharset:  {2}\r\n", this.preferredInternetCodePageForShiftJis, this.requiredCoverage, (this.preferredCharset == null) ? "null" : this.preferredCharset.Name);
		}

		private int preferredInternetCodePageForShiftJis = 50222;

		private int requiredCoverage = 100;

		private Charset preferredCharset;
	}
}
