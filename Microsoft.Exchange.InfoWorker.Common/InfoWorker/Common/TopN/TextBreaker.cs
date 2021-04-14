using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.TopN
{
	internal class TextBreaker
	{
		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "TextBreaker for locale id " + this.localeId + ". ";
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

		internal TextBreaker(int localeId)
		{
			this.localeId = localeId;
		}

		internal List<string> BreakText(string text)
		{
			List<string> list = new List<string>(10);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			int num = 0;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				bool flag = this.IsSeparatorChar(c);
				bool flag2 = i == text.Length - 1;
				if (flag || flag2)
				{
					int num2 = 0;
					if (flag)
					{
						num2 = i - num;
					}
					else if (i != 8191)
					{
						num2 = i - num + 1;
					}
					if (num2 > 0)
					{
						string text2 = text.Substring(num, num2);
						list.Add(text2.ToLower());
					}
					num = i + 1;
				}
			}
			return list;
		}

		internal bool IsSeparatorChar(char c)
		{
			return char.IsSeparator(c) || char.IsSymbol(c) || char.IsPunctuation(c) || char.IsControl(c) || char.IsWhiteSpace(c);
		}

		internal const int MaxTextSize = 8192;

		private int localeId;

		private string toString;

		protected static readonly Trace Tracer = ExTraceGlobals.TopNTracer;
	}
}
