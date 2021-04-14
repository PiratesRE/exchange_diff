using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal static class DiagnosticQueryStrings
	{
		public static string QueryNull()
		{
			return "Query must not be null or empty";
		}

		public static string ExceededTokenLimit(int tokenLimit)
		{
			return string.Format("Exceeded maximum token limit ({0})", tokenLimit);
		}

		public static string DatabaseNotFound(string databaseName)
		{
			return string.Format("Could not find database with MdbName \"{0}\"", databaseName);
		}

		public static string NoAvailableDatabase()
		{
			return "No databases are available for query";
		}

		public static string DatabaseRequired(string tableName)
		{
			return string.Format("Table \"{0}\" must be prefixed with a database name", tableName);
		}

		public static string DatabaseMismatch()
		{
			return string.Format("Query and Table prefix indicate different databases", new object[0]);
		}

		public static string InvalidFrom()
		{
			return "Invalid From clause";
		}

		public static string TableNotFound(string tableName)
		{
			return string.Format("Table \"{0}\" was not found or is not supported by the current database schema.", tableName);
		}

		public static string ColumnNotFound(string columnName)
		{
			return string.Format("Column \"{0}\" was not recognized as a physical column, property tag, or a property info OR it's not supported by the current database schema.", columnName);
		}

		public static string ColumnTypeMissing(string columnName)
		{
			return string.Format("Column \"{0}\" is missing a valid Type.", columnName);
		}

		public static string ColumnTableMissing(string columnName)
		{
			return string.Format("Column \"{0}\" is missing a valid Table.", columnName);
		}

		public static string UnknownCriterionType()
		{
			return "Unknown type of query criterion";
		}

		public static string UnknownQueryOperator(DiagnosticQueryOperator op)
		{
			return string.Format("Unknown query operator {0}", op);
		}

		public static string UnsupportedQueryValueType(Type type)
		{
			return string.Format("Values of type {0} are not yet supported by the query translator", type.Name);
		}

		public static string FailedToTranslateValue(string value, Type type)
		{
			return string.Format("Couldn't translate value \"{0}\" to type {1}", value, type.Name);
		}

		public static string InvalidTop()
		{
			return "Top requires a positive integer value";
		}

		public static string InvalidCountOrderBy()
		{
			return "Cannot combine a Count query with an Order-by clause";
		}

		public static string DuplicateSortColumn(string columnName)
		{
			return string.Format("Sort Order contains a duplicate column \"{0}\"", columnName);
		}

		public static string DatabaseOffline(string databaseName)
		{
			return string.Format("Database [{0}] is offline and unavailable for query", databaseName);
		}

		public static string UnsupportedQueryOperator(DiagnosticQueryOperator op)
		{
			return string.Format("Query operator {0} is not supported for this criterion", op);
		}

		public static string PartitionedTable(string tableName, IEnumerable<string> columnNames)
		{
			return string.Format("Table {0} is partitioned by columns {1}", tableName, string.Join(", ", columnNames));
		}

		public static string DuplicateSet(string columnName)
		{
			return string.Format("Set list contains a duplicate column \"{0}\"", columnName);
		}

		public static string UnsupportedQueryType()
		{
			return "Unknown or unsupported query type";
		}

		public static string InvalidQueryContext()
		{
			return "Only a Select query may target a Table Function";
		}

		public static string InvalidStoreContext()
		{
			return "The connection provider given is not a context provider.";
		}

		public static string ColumnRequiresValue(string columnName)
		{
			return string.Format("Non-identity Column \"{0}\" must be provided a non-null value", columnName);
		}

		public static string UnimplementedKeyword()
		{
			return "Unimplemented keyword";
		}

		public static string IncorrectParameterCount(string tableName, int expectedCount)
		{
			return string.Format("Table function \"{0}\" expects {1} parameter(s)", tableName, expectedCount);
		}

		public static string InvalidFolderId()
		{
			return "Folder ID format is invalid";
		}

		public static string LikeOperatorNotAllowed(string tableName, string columnName)
		{
			return string.Format("The LIKE comparison operator is not allowed for column \"{0}\" on table \"{1}\"", columnName, tableName);
		}

		public static string RedactedTable(string tableName)
		{
			return string.Format("Table \"{0}\" may contain PII; to query this target requires elevated access", tableName);
		}

		public static string PrivateTable(string tableName)
		{
			return string.Format("Table \"{0}\" contains private content; this target may not be queried", tableName);
		}

		public static string RestrictedColumnValue(string prefix, int length)
		{
			return string.Format("{0} ({1} bytes)", prefix, length.ToString("N0"));
		}

		public static string TruncatedColumnValue(long length)
		{
			return string.Format("TRUNCATED ({0} bytes)", length.ToString("N0"));
		}

		public static string EmptySelectList()
		{
			return "Query must select at least one column";
		}

		public static string ProcessorNotFound(string processor)
		{
			return string.Format("Processor \"{0}\"  was not found", processor);
		}

		public static string ProcessorEmptyArguments()
		{
			return string.Format("Processor usage error: provide one or more column identifiers", new object[0]);
		}

		public static string ProcessorColumnNotFound(string column)
		{
			return string.Format("Unable to locate column {0} in arguments collection", column);
		}

		public static string ProcessorUnsupportedType(string column, string type)
		{
			return string.Format("Column {0} type {1} is not supported by sizing", column, type);
		}

		public static string ProcessorCustomError(string message)
		{
			return string.Format("Processor error: {0}", message);
		}

		public static string InvalidFixedColumnValue(string columnName, int expectedLength, int actualLength)
		{
			return string.Format("The specified value for column \"{0}\" in where clause does not match the required length: expected length = {1}, actual length={2}", columnName, expectedLength, actualLength);
		}

		public static string InvalidEntryIdFormat()
		{
			return "EntryId length is expected to be either 46 bytes (for a folder) or 70 bytes (for a message).";
		}

		public static string InvalidCategorizationInfoFormat()
		{
			return "Categorization info blob format is invalid.";
		}

		public static string UnableToParseEntryId()
		{
			return "Unable to parse EntryId, likely due to an unsupported entry id type.";
		}

		public static string InvalidExchangeIdBinaryFormat()
		{
			return "ExchangeId length is expected to be 8, 9, 22, 24, or 26 bytes.";
		}

		public static string InvalidExchangeIdStringFormat()
		{
			return "ExchangeId string is expected to have the form <ReplId(Hex)>-<GlobCnt> or <ReplIdGuid>[<ReplId(Decimal)>]-<GlobCnt>.";
		}

		public static string InvalidExchangeIdListFormat()
		{
			return "ExchangeIdList length is expected to be greater than 0.";
		}

		public static string UnableToOpenMailbox(int mailboxNumber)
		{
			return string.Format("Unable to open mailbox with MailboxNumber = {0}.", mailboxNumber);
		}

		public static string InvalidRestrictionFormat()
		{
			return "Restriction length is expected to be greater than 0.";
		}

		public static string UnableToLockMailbox(int mailboxNumber)
		{
			return string.Format("Unable to lock mailbox with MailboxNumber = {0}.", mailboxNumber);
		}

		public static string MailboxStateNotFound(int mailboxNumber)
		{
			return string.Format("MailboxState is not found for mailbox with MailboxNumber = {0}.", mailboxNumber);
		}

		public static string ClusterNotInstalled()
		{
			return "The database is hosted on a server where the Failover Cluster Service is not installed.";
		}

		public static string ServerIsNotDAGMember()
		{
			return "The database is hosted on a server that is not a member of a DAG.";
		}

		public static string InvalidAclTableAndSDFormat(string innerMessage)
		{
			return string.Format("Invalid AclTableAndSD property format: {0}", innerMessage);
		}

		public static string FaultExecutingFullTextQuery(Exception ex)
		{
			return string.Format("{0} while executing FullTextQuery: {1}", ex.GetType().Name, ex.Message);
		}
	}
}
