using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	public class RemoveConditionResult
	{
		public RemoveConditionResult()
		{
		}

		public RemoveConditionResult(string cookie, bool removed)
		{
			this.Results = new List<SingleCookieRemoveResult>();
			this.Results.Add(new SingleCookieRemoveResult
			{
				Cookie = cookie,
				Removed = removed
			});
		}

		public RemoveConditionResult(List<string> cookiesRemoved)
		{
			this.Results = new List<SingleCookieRemoveResult>(cookiesRemoved.Count);
			foreach (string cookie in cookiesRemoved)
			{
				this.Results.Add(new SingleCookieRemoveResult
				{
					Cookie = cookie,
					Removed = true
				});
			}
		}

		[XmlArrayItem("Conditional")]
		public List<SingleCookieRemoveResult> Results { get; set; }
	}
}
