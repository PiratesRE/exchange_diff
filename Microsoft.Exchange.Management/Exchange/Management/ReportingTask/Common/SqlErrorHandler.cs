using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	internal class SqlErrorHandler
	{
		public static LocalizedException TrasnlateError(SqlException sqlException)
		{
			if (sqlException.Number == 208 || sqlException.Number == 207)
			{
				return new InvalidDataSourceException(sqlException.Number, sqlException);
			}
			if (sqlException.Number == -2)
			{
				return new DataMartTimeoutException(sqlException);
			}
			if (sqlException.Class >= 11 && sqlException.Class <= 16)
			{
				return new InvalidQueryException(sqlException.Number, sqlException);
			}
			if (sqlException.Class >= 17 && sqlException.Class <= 25)
			{
				return new InternalErrorException(sqlException.Number, sqlException);
			}
			return new DatabaseException(sqlException.Number, sqlException.Message, sqlException);
		}

		public static LocalizedException TrasnlateConnectionError(SqlException sqlException)
		{
			return new ConnectionFailedException(sqlException.Number, sqlException);
		}

		public static LocalizedException TrasnlateError(SqlTypeException sqlException)
		{
			return new InvalidDataException(sqlException.Message, sqlException);
		}

		public static bool IsObjectNotFoundError(SqlException sqlException)
		{
			return sqlException.Number == 208;
		}

		private const int ObjectNotFound = 208;

		private const int ColumnNotFound = 207;

		private const int TimeoutExpired = -2;
	}
}
