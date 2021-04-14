using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Hybrid
{
	[Serializable]
	internal class TooManyResultsException : LocalizedException
	{
		public TooManyResultsException(string identity, LocalizedString localizedMessage, Exception innerException, IEnumerable<string> matches) : base(localizedMessage, innerException)
		{
			this.identity = identity;
			this.matches = new List<string>(matches.Count<string>());
			this.matches.AddRange(matches);
		}

		public string[] Matches
		{
			get
			{
				return this.matches.ToArray();
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.Append(HybridStrings.ErrorTooManyMatchingResults(this.identity));
			stringBuilder.Append(":\r\n");
			foreach (string value in this.matches)
			{
				stringBuilder.AppendLine(value);
			}
			return stringBuilder.ToString();
		}

		private List<string> matches;

		private readonly string identity;
	}
}
