using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	public sealed class JavaScriptSymbolComparer<T> : IComparer<T> where T : IJavaScriptSymbol
	{
		private JavaScriptSymbolComparer()
		{
		}

		public static JavaScriptSymbolComparer<T> Instance
		{
			get
			{
				return JavaScriptSymbolComparer<T>.instance;
			}
		}

		public int Compare(T x, T y)
		{
			if (x.ScriptEndLine > y.ScriptEndLine)
			{
				return 1;
			}
			if (x.ScriptEndLine < y.ScriptEndLine)
			{
				return -1;
			}
			if (x.ScriptEndColumn > y.ScriptEndColumn)
			{
				return 1;
			}
			if (x.ScriptEndColumn < y.ScriptEndColumn)
			{
				return -1;
			}
			if (x.ScriptStartLine > y.ScriptStartLine)
			{
				return -1;
			}
			if (x.ScriptStartLine < y.ScriptStartLine)
			{
				return 1;
			}
			if (x.ScriptStartColumn > y.ScriptStartColumn)
			{
				return -1;
			}
			if (x.ScriptStartColumn < y.ScriptStartColumn)
			{
				return 1;
			}
			return 0;
		}

		private const int Less = -1;

		private const int Greater = 1;

		private const int Equal = 0;

		private static readonly JavaScriptSymbolComparer<T> instance = new JavaScriptSymbolComparer<T>();
	}
}
