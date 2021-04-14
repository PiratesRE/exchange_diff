using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Cluster.Replay.IO;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class EseLogEnumerator : DirectoryEnumerator
	{
		public EseLogEnumerator(DirectoryInfo path, string logPrefix, string logSuffix) : base(path, false, false)
		{
			this.m_prefix = logPrefix;
			this.m_suffix = logSuffix;
		}

		public long FindLowestGeneration()
		{
			if (RegistryParameters.FilesystemMaintainsOrder)
			{
				return this.FindLowestGenerationFast();
			}
			return this.FindLowestGenerationSlow();
		}

		public long FindLowestGenerationFast()
		{
			long result = 0L;
			string filter = this.m_prefix + EseLogEnumerator.hexFieldPattern + this.m_suffix;
			string pattern = string.Format("^{0}([0-9A-F]{{8}}){1}$", this.m_prefix, this.m_suffix);
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
			base.ReturnBaseNames = true;
			foreach (string input in base.EnumerateFiles(filter, null))
			{
				Match match = regex.Match(input);
				if (match.Success)
				{
					Group group = match.Groups[1];
					string s = group.ToString();
					result = (long)ulong.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
					break;
				}
			}
			return result;
		}

		public long FindLowestGenerationSlow()
		{
			string filter = string.Empty;
			long? num = null;
			for (int i = 7; i >= 0; i--)
			{
				for (int j = 1; j <= 15; j++)
				{
					filter = this.BuildFilter(i, j);
					num = base.EnumerateFiles(filter, null).Min(delegate(string file)
					{
						long value;
						if (string.IsNullOrEmpty(file) || !EseHelper.GetGenerationNumberFromFilename(file, this.m_prefix, out value))
						{
							return null;
						}
						return new long?(value);
					});
					if (num != null)
					{
						return num.Value;
					}
				}
			}
			return 0L;
		}

		public string FindHighestGenerationLogFile()
		{
			string filter = string.Empty;
			string result = null;
			long? num = null;
			for (int i = 0; i < 8; i++)
			{
				for (int j = 15; j > 0; j--)
				{
					filter = this.BuildFilter(i, j);
					num = base.EnumerateFiles(filter, null).Max(delegate(string file)
					{
						long value;
						if (string.IsNullOrEmpty(file) || !EseHelper.GetGenerationNumberFromFilename(file, this.m_prefix, out value))
						{
							return null;
						}
						return new long?(value);
					});
					if (num != null)
					{
						return EseHelper.MakeLogfileName(this.m_prefix, this.m_suffix, num.Value);
					}
				}
			}
			return result;
		}

		public IEnumerable<string> GetLogFiles(bool includeE00)
		{
			string filter = string.Empty;
			for (int digitIndex = 7; digitIndex >= 0; digitIndex--)
			{
				for (int digit = 1; digit <= 15; digit++)
				{
					filter = this.BuildFilter(digitIndex, digit);
					IOrderedEnumerable<string> orderedFiles = from file in base.EnumerateFiles(filter, null)
					where this.IsValidEseLogFileName(file)
					orderby this.GetGenerationFromFileName(file)
					select file;
					foreach (string fileName in orderedFiles)
					{
						yield return fileName;
					}
				}
			}
			if (includeE00)
			{
				filter = this.BuildE00Filter();
				string fileName2;
				if (base.GetNextFile(filter, out fileName2))
				{
					yield return fileName2;
				}
			}
			yield break;
		}

		protected override LocalizedString GetIOExceptionMessage(string directoryName, string apiName, string ioErrorMessage, int win32ErrorCode)
		{
			return ReplayStrings.EseLogEnumeratorIOError(apiName, ioErrorMessage, win32ErrorCode, directoryName);
		}

		private string BuildE00Filter()
		{
			return this.m_prefix + this.m_suffix;
		}

		private string BuildFilter(int firstDigit)
		{
			return this.BuildFilter(0, firstDigit);
		}

		private string BuildFilter(int digitIndex, int digit)
		{
			return string.Concat(new object[]
			{
				this.m_prefix,
				new string('0', digitIndex),
				digit.ToString("X"),
				'*',
				this.m_suffix
			});
		}

		private bool IsValidEseLogFileName(string fileName)
		{
			long num;
			return EseHelper.GetGenerationNumberFromFilename(fileName, this.m_prefix, out num);
		}

		private long GetGenerationFromFileName(string fileName)
		{
			long result;
			EseHelper.GetGenerationNumberFromFilename(fileName, this.m_prefix, out result);
			return result;
		}

		private const int EseFileNameHexDigits = 8;

		private readonly string m_prefix;

		private readonly string m_suffix;

		private static readonly string hexFieldPattern = new string('?', 8);
	}
}
