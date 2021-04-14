using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class EseDatabaseFileRecord : EseLogRecord
	{
		internal static string Identifier
		{
			get
			{
				return "LRDI";
			}
		}

		public override string LogRecType
		{
			get
			{
				return EseDatabaseFileRecord.Identifier;
			}
		}

		protected override Regex Regex()
		{
			return EseDatabaseFileRecord.regex;
		}

		internal EseDatabaseFileRecord(string input)
		{
			Match match = base.Match(input);
			this.m_checksum = ulong.Parse(match.Groups["Checksum"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_databaseId = int.Parse(match.Groups["DatabaseId"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_database = match.Groups["Database"].ToString();
			string text = match.Groups["Operation"].ToString();
			if (string.Compare(text, "createdb", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.m_operation = DatabaseOperation.Create;
			}
			else if (string.Compare(text, "attachdb", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.m_operation = DatabaseOperation.Attach;
			}
			else if (string.Compare(text, "detachdb", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.m_operation = DatabaseOperation.Detach;
			}
			else
			{
				ExDiagnostics.FailFast(string.Format(CultureInfo.CurrentCulture, "operation field {0} failed to match {1}, {2} or {3}. input is {4}, regex is {5}", new object[]
				{
					text,
					"createdb",
					"attachdb",
					"detachdb",
					input,
					EseDatabaseFileRecord.regex.ToString()
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

		public string Database
		{
			get
			{
				return this.m_database;
			}
		}

		public DatabaseOperation Operation
		{
			get
			{
				return this.m_operation;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Checksum={0},DatabaseId={1:x},Database={2},Operation={3}", new object[]
			{
				this.m_checksum,
				this.m_databaseId,
				this.m_database,
				this.m_operation
			});
		}

		private const string ChecksumGroup = "Checksum";

		private const string DatabaseIdGroup = "DatabaseId";

		private const string DatabaseGroup = "Database";

		private const string OperationGroup = "Operation";

		private const string CreateOperation = "createdb";

		private const string AttachOperation = "attachdb";

		private const string DetachOperation = "detachdb";

		private readonly ulong m_checksum;

		private readonly int m_databaseId;

		private readonly string m_database;

		private readonly DatabaseOperation m_operation;

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}\\s*,\\s*(?<{1}>[^,]+?)\\s*,\\s*(?<{2}>[0-9A-F]+)\\s*,\\s*(?<{3}>[0-9A-F]+)\\s*,\\s*(?i:(?<{4}>({5}|{6}|{7})))\\s*,\\s*(?<{8}>[0-9A-F]+)\\s*,\\s*(?<{9}>.+?)\\s*$", new object[]
		{
			EseDatabaseFileRecord.Identifier,
			"LogPos",
			"LogRecSize",
			"Checksum",
			"Operation",
			"createdb",
			"attachdb",
			"detachdb",
			"DatabaseId",
			"Database"
		}), RegexOptions.CultureInvariant);
	}
}
