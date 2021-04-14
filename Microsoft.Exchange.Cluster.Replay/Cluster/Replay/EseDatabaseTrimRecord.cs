using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class EseDatabaseTrimRecord : EseLogRecord
	{
		internal static string Identifier
		{
			get
			{
				return "LRTI";
			}
		}

		public override string LogRecType
		{
			get
			{
				return EseDatabaseTrimRecord.Identifier;
			}
		}

		protected override Regex Regex()
		{
			return EseDatabaseTrimRecord.regex;
		}

		internal EseDatabaseTrimRecord(string input)
		{
			Match match = base.Match(input);
			this.m_checksum = ulong.Parse(match.Groups["Checksum"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_databaseId = int.Parse(match.Groups["DatabaseId"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_operation = match.Groups["Operation"].ToString();
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

		public int DatabaseId
		{
			get
			{
				return this.m_databaseId;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Checksum={0},DatabaseId={1:x},Operation={2}", new object[]
			{
				this.m_checksum,
				this.m_databaseId,
				this.Operation
			});
		}

		private const string ChecksumGroup = "Checksum";

		private const string DatabaseIdGroup = "DatabaseId";

		private const string OperationGroup = "Operation";

		private readonly ulong m_checksum;

		private readonly int m_databaseId;

		private readonly string m_operation;

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}\\s*,\\s*(?<{1}>[^,]+?)\\s*,\\s*(?<{2}>[0-9A-F]+)\\s*,\\s*(?<{3}>[0-9A-F]+)\\s*,\\s*(?<{4}>[^,]+?)\\s*,\\s*(?<{5}>[0-9A-F]+)\\s*$", new object[]
		{
			EseDatabaseTrimRecord.Identifier,
			"LogPos",
			"LogRecSize",
			"Checksum",
			"Operation",
			"DatabaseId"
		}), RegexOptions.CultureInvariant);
	}
}
