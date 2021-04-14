using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class TraceTagInfo
	{
		public string PrettyName
		{
			get
			{
				return this.prettyName;
			}
		}

		public int NumericValue
		{
			get
			{
				return this.numericValue;
			}
		}

		public TraceTagInfo(int numericValue, string prettyName)
		{
			this.numericValue = numericValue;
			this.prettyName = prettyName;
		}

		private string prettyName;

		private int numericValue;
	}
}
