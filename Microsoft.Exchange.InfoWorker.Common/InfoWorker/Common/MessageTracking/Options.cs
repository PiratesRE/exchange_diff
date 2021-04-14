using System;
using System.Text;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class Options
	{
		internal bool DiagnosticsEnabled { get; private set; }

		internal bool ExpandTree { get; private set; }

		internal bool SearchAsRecip { get; private set; }

		internal string ServerHint { get; private set; }

		internal Options(bool diagnosticsEnabled, bool expandTree, bool searchAsRecip, string serverHint)
		{
			this.DiagnosticsEnabled = diagnosticsEnabled;
			this.ExpandTree = expandTree;
			this.SearchAsRecip = searchAsRecip;
			this.ServerHint = serverHint;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(20);
			if (this.DiagnosticsEnabled)
			{
				stringBuilder.Append("GetPerfDiagnostics");
			}
			if (this.ExpandTree)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append("ExpandTree");
			}
			if (this.SearchAsRecip)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append("SearchAsRecip");
			}
			if (!string.IsNullOrEmpty(this.ServerHint))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append("ServerHint=");
				stringBuilder.Append(this.ServerHint);
			}
			if (stringBuilder.Length > 0)
			{
				return stringBuilder.ToString();
			}
			return null;
		}

		internal static Options GetOptions(string optionsString)
		{
			if (string.IsNullOrEmpty(optionsString))
			{
				return Options.noOptions;
			}
			string[] array = optionsString.Split(new char[]
			{
				';'
			});
			bool diagnosticsEnabled = false;
			bool expandTree = false;
			bool searchAsRecip = false;
			string text = null;
			foreach (string text2 in array)
			{
				if (string.Equals(text2, "GetPerfDiagnostics"))
				{
					diagnosticsEnabled = true;
				}
				else if (string.Equals(text2, "ExpandTree"))
				{
					expandTree = true;
				}
				else if (string.Equals(text2, "SearchAsRecip"))
				{
					searchAsRecip = true;
				}
				else if (text2.StartsWith("ServerHint="))
				{
					text = text2.Substring("ServerHint=".Length, text2.Length - "ServerHint=".Length);
					if (string.IsNullOrEmpty(text))
					{
						text = null;
					}
				}
			}
			return new Options(diagnosticsEnabled, expandTree, searchAsRecip, text);
		}

		public const string GetPerfDiagnosticsOption = "GetPerfDiagnostics";

		public const string ExpandTreeOption = "ExpandTree";

		public const string SearchAsRecipOption = "SearchAsRecip";

		private const char Separator = ';';

		private const string ServerHintOption = "ServerHint=";

		private static Options noOptions = new Options(false, false, false, null);
	}
}
