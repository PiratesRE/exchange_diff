using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal abstract class AjaxMinSymbolParser<T> : ClientWatsonSymbolParser<T> where T : IJavaScriptSymbol
	{
		private int AjaxMinFieldCount
		{
			get
			{
				if (this.ajaxMinFieldCount < 0)
				{
					this.ajaxMinFieldCount = Enum.GetNames(typeof(AjaxMinSymbolParser<T>.AjaxMinSymbolData)).Length;
				}
				return this.ajaxMinFieldCount;
			}
		}

		public override bool TryParseSymbolData(string value, ClientWatsonFunctionNamePool functionNamePool, out T javaScriptSymbol)
		{
			javaScriptSymbol = default(T);
			if (value == null)
			{
				return false;
			}
			string[] array = value.Split(new char[]
			{
				','
			}, StringSplitOptions.None);
			if (array.Length != this.AjaxMinFieldCount)
			{
				return false;
			}
			bool result;
			try
			{
				string item = array[11];
				if (this.symbolTypesToSkip.Contains(item))
				{
					result = false;
				}
				else
				{
					javaScriptSymbol = this.ParseSymbolData(array, functionNamePool);
					result = true;
				}
			}
			catch (ArgumentNullException)
			{
				result = false;
			}
			catch (OverflowException)
			{
				result = false;
			}
			catch (FormatException)
			{
				result = false;
			}
			return result;
		}

		protected abstract T ParseSymbolData(string[] columns, ClientWatsonFunctionNamePool functionNamePool);

		public const string AjaxMinMapFileSuffix = "*_minify.xml";

		public const string AjaxMinMapFilePattern = "**_minify.xml";

		private readonly HashSet<string> symbolTypesToSkip = new HashSet<string>
		{
			"ThisLiteral",
			"Member",
			"Lookup",
			"ConstantWrapper"
		};

		private int ajaxMinFieldCount = -1;

		protected enum AjaxMinSymbolData
		{
			ScriptStartLine,
			ScriptStartColumn,
			ScriptEndLine,
			ScriptEndColumn,
			SourceStartPosition,
			SourceEndPosition,
			SourceStartLine,
			SourceStartColumn,
			SourceEndLine,
			SourceEndColumn,
			SourceFileId,
			SymbolType,
			ParentFunction
		}
	}
}
