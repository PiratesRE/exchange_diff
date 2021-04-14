using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class EseMiscRecord : EseLogRecord
	{
		internal static string Identifier
		{
			get
			{
				return "LRMI";
			}
		}

		public override string LogRecType
		{
			get
			{
				return EseMiscRecord.Identifier;
			}
		}

		protected override Regex Regex()
		{
			return EseMiscRecord.regex;
		}

		internal EseMiscRecord(string input)
		{
			Match match = base.Match(input);
			this.m_checksum = ulong.Parse(match.Groups["checksum"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_operation = match.Groups["operation"].ToString();
			base.SetLogPosAndSize(match);
		}

		public ulong Checksum
		{
			get
			{
				return this.m_checksum;
			}
		}

		public string Operation
		{
			get
			{
				return this.m_operation;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Checksum={0:x},Operation={1}", new object[]
			{
				this.m_checksum,
				this.m_operation
			});
		}

		private const string ChecksumGroup = "checksum";

		private const string OperationGroup = "operation";

		private readonly ulong m_checksum;

		private readonly string m_operation;

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}\\s*,\\s*(?<{1}>[^,]+?)\\s*,\\s*(?<{2}>[0-9A-F]+)\\s*,\\s*(?<{3}>[0-9A-F]+)\\s*,\\s*(?<{4}>[^,]+?)\\s*$", new object[]
		{
			EseMiscRecord.Identifier,
			"LogPos",
			"LogRecSize",
			"checksum",
			"operation"
		}), RegexOptions.CultureInvariant);
	}
}
