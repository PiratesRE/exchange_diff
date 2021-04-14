using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class StatementParser
	{
		public StatementParser(string promptName, CultureInfo culture, string locString)
		{
			this.locString = locString.Trim();
			this.promptName = promptName;
			this.culture = culture;
			this.match = Regex.Match(locString, StatementParser.FormatParameterRegex);
		}

		internal string SubPromptFileName { get; set; }

		public StatementChunk NextChunk()
		{
			StatementChunk result = null;
			string text = string.Empty;
			if (this.match.Success)
			{
				text = this.locString.Substring(this.startIndex, this.match.Index - this.startIndex).Trim();
				if (StatementParser.IsSpokenText(text))
				{
					result = this.CreateTextOrFileChunk(text);
					this.startIndex = this.match.Index;
					this.fileChunkNum++;
				}
				else
				{
					result = this.CreateVariableChunk(int.Parse(this.match.Groups[StatementParser.FormatParameterGroup].Value, CultureInfo.InvariantCulture));
					this.startIndex = this.match.Index + this.match.Length;
					this.match = this.match.NextMatch();
				}
			}
			else if (this.startIndex < this.locString.Length)
			{
				text = this.locString.Substring(this.startIndex, this.locString.Length - this.startIndex).Trim();
				this.startIndex = this.locString.Length;
				if (StatementParser.IsSpokenText(text))
				{
					result = this.CreateTextOrFileChunk(text);
				}
			}
			return result;
		}

		private StatementChunk CreateVariableChunk(int varNum)
		{
			return new StatementChunk
			{
				Type = ChunkType.Variable,
				Value = varNum
			};
		}

		private StatementChunk CreateTextOrFileChunk(string chunkValue)
		{
			string path = this.promptName + "." + this.fileChunkNum.ToString(this.culture) + ".wav";
			this.SubPromptFileName = Path.Combine(Util.WavPathFromCulture(this.culture), path);
			StatementChunk result;
			if (GlobalActivityManager.ConfigClass.RecordingFileNameCache.Contains(this.SubPromptFileName))
			{
				result = new StatementChunk
				{
					Type = ChunkType.File,
					Value = chunkValue
				};
			}
			else
			{
				if (!GlobCfg.AllowTemporaryTTS)
				{
					throw new FileNotFoundException(this.SubPromptFileName);
				}
				result = new StatementChunk
				{
					Type = ChunkType.Text,
					Value = chunkValue
				};
			}
			return result;
		}

		protected static bool IsSpokenText(string chunk)
		{
			if (chunk == null || chunk.Length < 1)
			{
				return false;
			}
			foreach (char c in chunk)
			{
				if (char.IsLetterOrDigit(c))
				{
					return true;
				}
			}
			return false;
		}

		private static readonly string FormatParameterGroup = "paramNumber";

		private static readonly string FormatParameterRegex = "\\{(?<" + StatementParser.FormatParameterGroup + ">\\d+)\\}";

		private string promptName;

		private CultureInfo culture;

		private string locString;

		private Match match;

		private int startIndex;

		private int fileChunkNum = 1;
	}
}
