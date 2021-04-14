using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class EsePageRecord : EseLogRecord
	{
		internal static string Identifier
		{
			get
			{
				return "LRPI";
			}
		}

		public override string LogRecType
		{
			get
			{
				return EsePageRecord.Identifier;
			}
		}

		protected override Regex Regex()
		{
			return EsePageRecord.regex;
		}

		internal EsePageRecord(string input)
		{
			Match match = base.Match(input);
			this.m_operation = match.Groups["operation"].ToString();
			this.m_checksum = ulong.Parse(match.Groups["checksum"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_page = long.Parse(match.Groups["page"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_objectId = long.Parse(match.Groups["objectId"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_databaseId = long.Parse(match.Groups["databaseId"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_dbtimeBefore = ulong.Parse(match.Groups["dbtimeBefore"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_dbtimeAfter = ulong.Parse(match.Groups["dbtimeAfter"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			base.SetLogPosAndSize(match);
		}

		public ulong Checksum
		{
			get
			{
				return this.m_checksum;
			}
		}

		public long DatabaseId
		{
			get
			{
				return this.m_databaseId;
			}
		}

		public string Operation
		{
			get
			{
				return this.m_operation;
			}
		}

		public long PageNumber
		{
			get
			{
				return this.m_page;
			}
		}

		public long ObjectId
		{
			get
			{
				return this.m_objectId;
			}
		}

		public ulong DbtimeBefore
		{
			get
			{
				return this.m_dbtimeBefore;
			}
		}

		public ulong DbtimeAfter
		{
			get
			{
				return this.m_dbtimeAfter;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Checksum={0:x},DatabaseId={1:X},Operation={2},Page={3:X},ObjectId={4:X},DbtimeBefore={5:X},DbtimeAfter={6:X}", new object[]
			{
				this.m_checksum,
				this.m_databaseId,
				this.m_operation,
				this.m_page,
				this.m_objectId,
				this.m_dbtimeBefore,
				this.m_dbtimeAfter
			});
		}

		private const string ChecksumGroup = "checksum";

		private const string OperationGroup = "operation";

		private const string PageGroup = "page";

		private const string ObjectIdGroup = "objectId";

		private const string DatabaseIdGroup = "databaseId";

		private const string DbtimeBeforeGroup = "dbtimeBefore";

		private const string DbtimeAfterGroup = "dbtimeAfter";

		private readonly ulong m_checksum;

		private readonly long m_databaseId;

		private readonly string m_operation;

		private readonly long m_page;

		private readonly long m_objectId;

		private readonly ulong m_dbtimeBefore;

		private readonly ulong m_dbtimeAfter;

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}\\s*,\\s*(?<{1}>[^,]+?)\\s*,\\s*(?<{2}>[0-9A-F]+)\\s*,\\s*(?<{3}>[0-9A-F]+)\\s*,\\s*(?<{4}>[^,]+?)\\s*,\\s*(?<{5}>[0-9A-F]+)\\s*,\\s*(?<{6}>[0-9A-F]+)\\s*,\\s*(?<{7}>[0-9A-F]+)\\s*,\\s*(?<{8}>[0-9A-F]+)\\s*,\\s*(?<{9}>[0-9A-F]+)\\s*$", new object[]
		{
			EsePageRecord.Identifier,
			"LogPos",
			"LogRecSize",
			"checksum",
			"operation",
			"page",
			"objectId",
			"databaseId",
			"dbtimeBefore",
			"dbtimeAfter"
		}), RegexOptions.CultureInvariant);
	}
}
