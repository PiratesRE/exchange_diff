using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class EseAttachInfoRecord : EseLogRecord
	{
		internal static string Identifier
		{
			get
			{
				return "LHAI";
			}
		}

		public override string LogRecType
		{
			get
			{
				return EseAttachInfoRecord.Identifier;
			}
		}

		protected override Regex Regex()
		{
			return EseAttachInfoRecord.regex;
		}

		internal EseAttachInfoRecord(string input)
		{
			Match match = base.Match(input);
			this.m_databaseId = int.Parse(match.Groups["databaseId"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_database = match.Groups["database"].ToString();
		}

		public int DatabaseId
		{
			get
			{
				return this.m_databaseId;
			}
		}

		public string Database
		{
			get
			{
				return this.m_database;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "DatabaseId={0:x},Database={1}", new object[]
			{
				this.m_databaseId,
				this.m_database
			});
		}

		private const string DatabaseIdGroup = "databaseId";

		private const string DatabaseGroup = "database";

		private readonly int m_databaseId;

		private readonly string m_database;

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}\\s*,\\s*(?<{1}>[0-9A-F]+)\\s*,\\s*(?<{2}>.+?)\\s*$", new object[]
		{
			EseAttachInfoRecord.Identifier,
			"databaseId",
			"database"
		}), RegexOptions.CultureInvariant);
	}
}
