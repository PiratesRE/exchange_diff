using System;
using System.Data.SqlTypes;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public interface ISqlDataReader : IDisposable
	{
		int FieldCount { get; }

		bool IsClosed { get; }

		void Close();

		bool GetBoolean(int i);

		long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length);

		long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length);

		DateTime GetDateTime(int i);

		Type GetFieldType(int i);

		Guid GetGuid(int i);

		short GetInt16(int i);

		int GetInt32(int i);

		long GetInt64(int i);

		string GetName(int i);

		int GetOrdinal(string name);

		SqlBinary GetSqlBinary(int i);

		string GetString(int i);

		object GetValue(int i);

		bool IsDBNull(int i);

		bool NextResult();

		bool Read();
	}
}
