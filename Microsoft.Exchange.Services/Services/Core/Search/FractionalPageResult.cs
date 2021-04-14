using System;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class FractionalPageResult : BasePageResult
	{
		public FractionalPageResult(BaseQueryView view, int numerator, int denominator) : base(view)
		{
			this.numerator = numerator;
			this.denominator = denominator;
		}

		public int NumeratorOffset
		{
			get
			{
				return this.numerator;
			}
		}

		public int AbsoluteDenominator
		{
			get
			{
				return this.denominator;
			}
		}

		private int numerator;

		private int denominator;
	}
}
