using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlCommand : DisposableBase
	{
		public Connection Connection
		{
			get
			{
				return this.connection;
			}
		}

		public StringBuilder Sb
		{
			get
			{
				return this.sb;
			}
		}

		public int Length
		{
			get
			{
				if (this.sb == null)
				{
					return 0;
				}
				return this.sb.Length;
			}
		}

		private SqlCommand(Connection connection, bool createStringBuilder)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.connection = (connection as SqlConnection);
				if (createStringBuilder)
				{
					this.sb = new StringBuilder(200);
				}
				disposeGuard.Success();
			}
		}

		public SqlCommand(Connection connection) : this(connection, true)
		{
		}

		public SqlCommand(Connection connection, Connection.OperationType operationType) : this(connection, true)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.StartNewStatement(operationType);
				disposeGuard.Success();
			}
		}

		public SqlCommand(Connection connection, string statement, Connection.OperationType operationType) : this(connection, false)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.StartNewStatement(operationType);
				this.command = this.connection.CreateSqlCommand();
				this.command.CommandText = statement;
				disposeGuard.Success();
			}
		}

		public void Append(string str)
		{
			this.sb.Append(str);
		}

		public void Append(char c)
		{
			this.sb.Append(c);
		}

		public void AppendStatement(string statement, Connection.OperationType operationType)
		{
			this.StartNewStatement(operationType);
			this.Append(statement);
		}

		public void AppendParameter(SqlDbType sqlDbType, int size, string parameterName)
		{
			if (this.command == null)
			{
				this.command = this.connection.CreateSqlCommand();
			}
			this.command.Parameters.Add(parameterName, sqlDbType, size);
			this.sb.Append(parameterName);
		}

		public void AppendParameter(SqlDbType sqlDbType, string parameterName)
		{
			if (this.command == null)
			{
				this.command = this.connection.CreateSqlCommand();
			}
			this.command.Parameters.Add(parameterName, sqlDbType);
			this.sb.Append(parameterName);
		}

		public void AppendParameter(object parameterValue)
		{
			if (parameterValue == null)
			{
				this.sb.Append("NULL");
				return;
			}
			this.AppendParameter(this.NextParameterName(), null, parameterValue);
		}

		public void AppendParameter(Column column, object parameterValue)
		{
			if (parameterValue == null && column == null)
			{
				this.sb.Append("NULL");
				return;
			}
			this.AppendParameter(this.NextParameterName(), column, parameterValue);
		}

		public void AppendParameter(string parameterName, object parameterValue)
		{
			this.AppendParameter(parameterName, null, parameterValue);
		}

		public void AppendParameter(string parameterName, Column column, object parameterValue)
		{
			if (this.command == null)
			{
				this.command = this.connection.CreateSqlCommand();
			}
			if (parameterValue is Array && !(parameterValue is byte[]))
			{
				parameterValue = SerializedValue.Serialize(parameterValue);
			}
			else
			{
				if (parameterValue is ICustomParameter)
				{
					throw new StoreException((LID)51797U, ErrorCodeValue.NotSupported);
				}
				if (parameterValue is ArraySegment<byte>)
				{
					ArraySegment<byte> arraySegment = (ArraySegment<byte>)parameterValue;
					byte[] array = new byte[arraySegment.Count];
					Array.Copy(arraySegment.Array, arraySegment.Offset, array, 0, arraySegment.Count);
					parameterValue = array;
				}
			}
			SqlParameter sqlParameter = this.command.Parameters.AddWithValue(parameterName, parameterValue);
			bool flag = false;
			if (column != null)
			{
				flag = SqlCommand.SetParameterMetadataFromColumn(sqlParameter, column);
			}
			if (parameterValue != null)
			{
				if (!flag && parameterValue.GetType() == typeof(string))
				{
					string text = parameterValue as string;
					if (text.Length >= 4000)
					{
						sqlParameter.SqlDbType = SqlDbType.NText;
					}
					sqlParameter.Size = (text.Length / 4000 + 1) * 4000;
				}
				else if (!flag && parameterValue is byte[])
				{
					byte[] array2 = parameterValue as byte[];
					sqlParameter.Size = (array2.Length / 4000 + 1) * 4000;
				}
				else if (parameterValue.GetType() == typeof(DateTime))
				{
					DateTime dateTime = (DateTime)parameterValue;
					sqlParameter.SqlDbType = SqlDbType.DateTime2;
				}
			}
			else
			{
				sqlParameter.SqlValue = DBNull.Value;
			}
			this.sb.Append(parameterName);
		}

		private static bool SetParameterMetadataFromColumn(SqlParameter param, Column column)
		{
			SqlDbType sqlDbType;
			if (!SqlCommand.SqlDbTypeFromClrType(column.Type, out sqlDbType))
			{
				return false;
			}
			if (sqlDbType == SqlDbType.NVarChar)
			{
				if (column.MaxLength != 0)
				{
					param.SqlDbType = sqlDbType;
					param.Size = column.MaxLength;
				}
				else
				{
					param.SqlDbType = SqlDbType.NChar;
					param.Size = column.Size;
				}
			}
			else if (sqlDbType == SqlDbType.VarBinary)
			{
				if (column.MaxLength != 0)
				{
					param.SqlDbType = sqlDbType;
					param.Size = column.MaxLength;
				}
				else
				{
					param.SqlDbType = SqlDbType.Binary;
					param.Size = column.Size;
				}
			}
			else
			{
				param.SqlDbType = sqlDbType;
			}
			return true;
		}

		private static bool SqlDbTypeFromClrType(Type clrType, out SqlDbType sqlDbType)
		{
			switch (ValueTypeHelper.GetExtendedTypeCode(clrType))
			{
			case ExtendedTypeCode.Boolean:
				sqlDbType = SqlDbType.Bit;
				return true;
			case ExtendedTypeCode.Int16:
				sqlDbType = SqlDbType.SmallInt;
				return true;
			case ExtendedTypeCode.Int32:
				sqlDbType = SqlDbType.Int;
				return true;
			case ExtendedTypeCode.Int64:
				sqlDbType = SqlDbType.BigInt;
				return true;
			case ExtendedTypeCode.Single:
				sqlDbType = SqlDbType.Real;
				return true;
			case ExtendedTypeCode.Double:
				sqlDbType = SqlDbType.Float;
				return true;
			case ExtendedTypeCode.DateTime:
				sqlDbType = SqlDbType.DateTime2;
				return true;
			case ExtendedTypeCode.Guid:
				sqlDbType = SqlDbType.UniqueIdentifier;
				return true;
			case ExtendedTypeCode.String:
				sqlDbType = SqlDbType.NVarChar;
				return true;
			case ExtendedTypeCode.Binary:
				sqlDbType = SqlDbType.VarBinary;
				return true;
			case ExtendedTypeCode.MVInt16:
			case ExtendedTypeCode.MVInt32:
			case ExtendedTypeCode.MVInt64:
			case ExtendedTypeCode.MVSingle:
			case ExtendedTypeCode.MVDouble:
			case ExtendedTypeCode.MVDateTime:
			case ExtendedTypeCode.MVGuid:
			case ExtendedTypeCode.MVString:
			case ExtendedTypeCode.MVBinary:
				sqlDbType = SqlDbType.VarBinary;
				return true;
			}
			throw new InvalidOperationException(string.Format("Unknown or unexpected type {0}", clrType));
		}

		private static string[] BuildParamNameArray()
		{
			string[] array = new string[200];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = "@P" + i.ToString();
			}
			return array;
		}

		public void AppendColumn(Column column, SqlQueryModel model, ColumnUse use)
		{
			model.AppendColumnToQuery(column, use, this);
		}

		public void AppendFromList(SqlQueryModel model)
		{
			model.AppendFromList(this);
		}

		public void AppendSelectList(IList<Column> columnsToFetch, SqlQueryModel model)
		{
			model.AppendSelectList(columnsToFetch, this);
		}

		public void AppendOrderByList(CultureInfo culture, Microsoft.Exchange.Server.Storage.PhysicalAccess.SortOrder sortOrder, SqlQueryModel model)
		{
			model.AppendOrderByList(culture, sortOrder, false, this);
		}

		public void AppendOrderByList(CultureInfo culture, Microsoft.Exchange.Server.Storage.PhysicalAccess.SortOrder sortOrder, bool reverse, SqlQueryModel model)
		{
			model.AppendOrderByList(culture, sortOrder, reverse, this);
		}

		public void StartNewStatement(Connection.OperationType operationType)
		{
			this.connection.CountStatement(operationType);
			this.numberOfStatements++;
			if (this.maxOperationTypeForBatch < operationType)
			{
				this.maxOperationTypeForBatch = operationType;
			}
			if (this.sb != null && this.sb.Length != 0)
			{
				this.sb.Append(";");
			}
		}

		public Reader ExecuteReader(Connection.TransactionOption transactionOption, int skipTo, SimpleQueryOperator simpleQueryOperator, bool disposeQueryOperator)
		{
			return this.connection.ExecuteReader(this.ToSqlCommand(), this.numberOfStatements, transactionOption, skipTo, simpleQueryOperator, disposeQueryOperator);
		}

		public int ExecuteNonQuery()
		{
			return this.ExecuteNonQuery(Connection.TransactionOption.NeedTransaction);
		}

		public int ExecuteNonQuery(Connection.TransactionOption transactionOption)
		{
			return this.connection.ExecuteNonQuery(this.ToSqlCommand(), this.numberOfStatements, transactionOption);
		}

		public object ExecuteScalar()
		{
			return this.ExecuteScalar(Connection.TransactionOption.DontNeedTransaction);
		}

		public object ExecuteScalar(Connection.TransactionOption transactionOption)
		{
			return this.connection.ExecuteScalar(this.ToSqlCommand(), this.numberOfStatements, transactionOption);
		}

		public ISqlCommand ToSqlCommand()
		{
			if (this.command == null)
			{
				this.command = this.connection.CreateSqlCommand();
			}
			if (this.sb != null)
			{
				this.command.CommandText = this.sb.ToString();
			}
			return this.command;
		}

		public override string ToString()
		{
			return this.sb.ToString();
		}

		private string NextParameterName()
		{
			int num = this.parameterCounter++;
			string result;
			if (num < SqlCommand.ParamNameArray.Length)
			{
				result = SqlCommand.ParamNameArray[num];
			}
			else
			{
				result = "@P" + num.ToString();
			}
			return result;
		}

		internal void AppendQueryHints(bool frequentOperation)
		{
			if (!frequentOperation)
			{
				this.Append(" OPTION (LOOP JOIN, FORCE ORDER, RECOMPILE)");
				return;
			}
			this.Append(" OPTION (LOOP JOIN, FORCE ORDER)");
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SqlCommand>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.command != null)
			{
				this.command.Dispose();
				this.command = null;
			}
			this.connection = null;
		}

		private const int AverageSize = 200;

		private static readonly string[] ParamNameArray = SqlCommand.BuildParamNameArray();

		private SqlConnection connection;

		private StringBuilder sb;

		private ISqlCommand command;

		private int parameterCounter;

		private int numberOfStatements;

		private Connection.OperationType maxOperationTypeForBatch;
	}
}
