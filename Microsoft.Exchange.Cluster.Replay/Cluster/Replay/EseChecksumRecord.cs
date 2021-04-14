using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class EseChecksumRecord : EseLogRecord
	{
		internal static string Identifier
		{
			get
			{
				return "LRCI";
			}
		}

		public override string LogRecType
		{
			get
			{
				return EseChecksumRecord.Identifier;
			}
		}

		protected override Regex Regex()
		{
			return EseChecksumRecord.regex;
		}

		internal EseChecksumRecord(string input)
		{
			Match match = base.Match(input);
			this.m_checksum = ulong.Parse(match.Groups["checksum"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			base.SetLogPosAndSize(match);
		}

		public ulong Checksum
		{
			get
			{
				return this.m_checksum;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Checksum={0:X}", new object[]
			{
				this.m_checksum
			});
		}

		private const string ChecksumGroup = "checksum";

		private readonly ulong m_checksum;

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}\\s*,\\s*(?<{1}>[^,]+?)\\s*,\\s*(?<{2}>[0-9A-F]+)\\s*,\\s*(?<{3}>[0-9A-F]+)\\s*$", new object[]
		{
			EseChecksumRecord.Identifier,
			"LogPos",
			"LogRecSize",
			"checksum"
		}), RegexOptions.CultureInvariant);
	}
}
