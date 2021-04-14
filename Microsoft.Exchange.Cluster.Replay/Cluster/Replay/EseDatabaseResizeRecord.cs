using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class EseDatabaseResizeRecord : EseLogRecord
	{
		internal static string Identifier
		{
			get
			{
				return "LRRI";
			}
		}

		public override string LogRecType
		{
			get
			{
				return EseDatabaseResizeRecord.Identifier;
			}
		}

		protected override Regex Regex()
		{
			return EseDatabaseResizeRecord.regex;
		}

		internal EseDatabaseResizeRecord(string input)
		{
			Match match = base.Match(input);
			this.m_checksum = ulong.Parse(match.Groups["Checksum"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_databaseId = int.Parse(match.Groups["DatabaseId"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			string text = match.Groups["Operation"].ToString();
			if (string.Compare(text, "extenddb", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.m_operation = DatabaseResizeOperation.Extend;
			}
			else if (string.Compare(text, "shrinkdb", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.m_operation = DatabaseResizeOperation.Shrink;
			}
			else
			{
				ExDiagnostics.FailFast(string.Format(CultureInfo.CurrentCulture, "resize operation field {0} failed to match {1} or {2}. input is {3}, regex is {4}", new object[]
				{
					text,
					"extenddb",
					"shrinkdb",
					input,
					EseDatabaseResizeRecord.regex.ToString()
				}), true);
			}
			base.SetLogPosAndSize(match);
		}

		public ulong Checksum
		{
			get
			{
				return this.m_checksum;
			}
		}

		public int DatabaseId
		{
			get
			{
				return this.m_databaseId;
			}
		}

		public DatabaseResizeOperation Operation
		{
			get
			{
				return this.m_operation;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Checksum={0},DatabaseId={1:x},ResizeOperation={2}", new object[]
			{
				this.m_checksum,
				this.m_databaseId,
				this.m_operation
			});
		}

		private const string ChecksumGroup = "Checksum";

		private const string DatabaseIdGroup = "DatabaseId";

		private const string OperationGroup = "Operation";

		private const string ExtendOperation = "extenddb";

		private const string ShrinkOperation = "shrinkdb";

		private readonly ulong m_checksum;

		private readonly int m_databaseId;

		private readonly DatabaseResizeOperation m_operation;

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}\\s*,\\s*(?<{1}>[^,]+?)\\s*,\\s*(?<{2}>[0-9A-F]+)\\s*,\\s*(?<{3}>[0-9A-F]+)\\s*,\\s*(?i:(?<{4}>({5}|{6})))\\s*,\\s*(?<{7}>.+?)\\s*$", new object[]
		{
			EseDatabaseResizeRecord.Identifier,
			"LogPos",
			"LogRecSize",
			"Checksum",
			"Operation",
			"extenddb",
			"shrinkdb",
			"DatabaseId"
		}), RegexOptions.CultureInvariant);
	}
}
