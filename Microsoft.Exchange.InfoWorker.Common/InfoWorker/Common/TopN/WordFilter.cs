using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.TopN
{
	internal class WordFilter
	{
		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "WordFilter for locale id " + this.localeId + ". ";
			}
			return this.toString;
		}

		internal int LocaleId
		{
			get
			{
				return this.localeId;
			}
		}

		internal WordFilter(int localeId)
		{
			this.localeId = localeId;
		}

		internal List<string> Filter(List<string> wordList)
		{
			List<string> list = new List<string>(10);
			foreach (string text in wordList)
			{
				if (!string.IsNullOrEmpty(text) && text.Length <= 20)
				{
					bool flag = false;
					bool flag2 = false;
					foreach (char c in text)
					{
						if (!char.IsLetter(c))
						{
							flag = true;
							break;
						}
						if (!flag2 && (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'y'))
						{
							flag2 = true;
						}
					}
					if (!flag && flag2)
					{
						list.Add(text);
					}
				}
			}
			return list;
		}

		private int localeId;

		private string toString;

		protected static readonly Trace Tracer = ExTraceGlobals.TopNTracer;
	}
}
