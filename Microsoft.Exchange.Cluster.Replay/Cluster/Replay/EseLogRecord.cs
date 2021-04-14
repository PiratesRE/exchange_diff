using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class EseLogRecord
	{
		protected abstract Regex Regex();

		protected Match Match(string input)
		{
			Regex regex = this.Regex();
			Match match = regex.Match(input);
			if (!match.Success)
			{
				EseLogRecord.ThrowParseError(input, regex);
			}
			return match;
		}

		private static void ThrowParseError(string input, Regex regex)
		{
			throw new EseutilParseErrorException(input, regex.ToString());
		}

		public static EseLogRecord Parse(string input)
		{
			Match match = EseLogRecord.regex.Match(input);
			if (!match.Success)
			{
				EseLogRecord.ThrowParseError(input, EseLogRecord.regex);
			}
			string text = match.Groups["Identifier"].ToString();
			if (text == EseLogHeaderRecord.Identifier)
			{
				return new EseLogHeaderRecord(input);
			}
			if (text == EseAttachInfoRecord.Identifier)
			{
				return new EseAttachInfoRecord(input);
			}
			if (text == EseDatabaseFileRecord.Identifier)
			{
				return new EseDatabaseFileRecord(input);
			}
			if (text == EseChecksumRecord.Identifier)
			{
				return new EseChecksumRecord(input);
			}
			if (text == EsePageRecord.Identifier)
			{
				return new EsePageRecord(input);
			}
			if (text == EseMiscRecord.Identifier)
			{
				return new EseMiscRecord(input);
			}
			if (text == EseEofRecord.Identifier)
			{
				return new EseEofRecord(input);
			}
			if (text == EseDatabaseResizeRecord.Identifier)
			{
				return new EseDatabaseResizeRecord(input);
			}
			if (text == EseDatabaseTrimRecord.Identifier)
			{
				return new EseDatabaseTrimRecord(input);
			}
			ExDiagnostics.FailFast(string.Format(CultureInfo.CurrentCulture, "identifier field {0} failed to match. input is {1}, regex is {2}", new object[]
			{
				text,
				input,
				EseLogRecord.regex.ToString()
			}), true);
			return null;
		}

		public EseLogPos LogPos { get; protected set; }

		public int LogRecSize { get; protected set; }

		protected void SetLogPosAndSize(Match m)
		{
			this.LogPos = EseLogPos.Parse(m.Groups["LogPos"].ToString());
			this.LogRecSize = int.Parse(m.Groups["LogRecSize"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		}

		public abstract string LogRecType { get; }

		private const string IdentifierGroup = "Identifier";

		protected const string LogPosGroup = "LogPos";

		protected const string LogRecSizeGroup = "LogRecSize";

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^(?<{0}>{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9})\\s*", new object[]
		{
			"Identifier",
			EseLogHeaderRecord.Identifier,
			EseAttachInfoRecord.Identifier,
			EseDatabaseFileRecord.Identifier,
			EseChecksumRecord.Identifier,
			EsePageRecord.Identifier,
			EseMiscRecord.Identifier,
			EseEofRecord.Identifier,
			EseDatabaseResizeRecord.Identifier,
			EseDatabaseTrimRecord.Identifier
		}), RegexOptions.CultureInvariant);
	}
}
