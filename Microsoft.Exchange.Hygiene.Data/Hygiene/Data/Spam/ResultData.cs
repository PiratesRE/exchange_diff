using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public class ResultData
	{
		public ResultType ResultType { get; set; }

		public int? Score { get; set; }

		public string ResponseString { get; set; }
	}
}
