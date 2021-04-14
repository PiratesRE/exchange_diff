using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	internal class CallStackFrameInfo
	{
		public string FunctionName { get; set; }

		public string PackageName { get; set; }

		public int StartLine { get; set; }

		public int StartColumn { get; set; }

		public int EndLine { get; set; }

		public int EndColumn { get; set; }

		public string FileName { get; set; }

		public string FolderPath { get; set; }

		public CallStackFrameInfo()
		{
		}

		public CallStackFrameInfo(bool browserGeneratedData)
		{
			this.browserGeneratedData = browserGeneratedData;
		}

		public string SanitizedFunctionName
		{
			get
			{
				if (this.sanitizedFunctionName == null)
				{
					string originalFunctionName = this.browserGeneratedData ? CallStackFrameInfo.NormalizeFunctionNameFromBrowser(this.FunctionName) : this.FunctionName;
					this.sanitizedFunctionName = this.SanitizeFunctionName(originalFunctionName);
				}
				return this.sanitizedFunctionName;
			}
		}

		public override string ToString()
		{
			return string.Format("\r\n   at {0}() in {1}{2} : line {3}", new object[]
			{
				this.SanitizedFunctionName,
				this.FolderPath,
				this.FileName,
				this.StartLine
			});
		}

		public string ToDetailedString()
		{
			return string.Format("\r\n   at {0}() in {1}{2} {3}:{4} to {5}:{6}", new object[]
			{
				this.FunctionName,
				this.FolderPath,
				this.FileName,
				this.StartLine,
				this.StartColumn,
				this.EndLine,
				this.EndColumn
			});
		}

		public void UpdateHash(ref int hash)
		{
			hash = WatsonReport.ComputeHash(this.SanitizedFunctionName, hash);
			hash = WatsonReport.ComputeHash(this.FileName, hash);
		}

		private static string NormalizeFunctionNameFromBrowser(string originalFunctionName)
		{
			if (string.IsNullOrEmpty(originalFunctionName) || originalFunctionName[0] == '$' || originalFunctionName.Equals("Anonymous function"))
			{
				return "Anonymous";
			}
			if (originalFunctionName[0] == '_' && originalFunctionName.LastIndexOf(".$", StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				return "Anonymous";
			}
			return originalFunctionName;
		}

		private string SanitizeFunctionName(string originalFunctionName)
		{
			StringBuilder stringBuilder = new StringBuilder(originalFunctionName.Length);
			bool flag = false;
			foreach (char c in originalFunctionName)
			{
				char c2 = c;
				if (c2 != ' ')
				{
					switch (c2)
					{
					case '(':
					case ')':
						break;
					default:
						if (c2 != '.')
						{
							stringBuilder.Append(c);
						}
						else
						{
							flag = true;
							stringBuilder.Append(c);
						}
						break;
					}
				}
				else
				{
					stringBuilder.Append('_');
				}
			}
			if (!flag)
			{
				return string.Format("{0}.{1}", Path.GetFileNameWithoutExtension(this.FileName), stringBuilder);
			}
			return stringBuilder.ToString();
		}

		private const string NormalizedAnonymousFunctionName = "Anonymous";

		private const string AnonymousFunctionNameOnIE = "Anonymous function";

		private const string NormalizedStackFrameFormat = "\r\n   at {0}() in {1}{2} : line {3}";

		private const string NormalizedDetailedStackFrameFormat = "\r\n   at {0}() in {1}{2} {3}:{4} to {5}:{6}";

		private readonly bool browserGeneratedData;

		private string sanitizedFunctionName;
	}
}
