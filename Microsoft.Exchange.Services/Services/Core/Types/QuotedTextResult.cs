using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class QuotedTextResult
	{
		public string NewMsg { get; set; }

		public string QuotedText { get; set; }

		public List<QuotedPart> QuotedParts
		{
			get
			{
				return this.quotedParts;
			}
		}

		protected List<QuotedPart> quotedParts = new List<QuotedPart>();
	}
}
