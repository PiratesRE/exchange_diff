using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal static class MigMonUtilities
	{
		public static DateTime ConverttoDateTime(string s)
		{
			DateTime result;
			if (string.IsNullOrWhiteSpace(s) || !DateTime.TryParse(s, out result))
			{
				result = default(DateTime);
			}
			else
			{
				result = result.ToUniversalTime();
			}
			if (result.CompareTo(MigMonUtilities.sqlDateTimeMinValue) <= 0)
			{
				return MigMonUtilities.sqlDateTimeMinValue;
			}
			if (result.CompareTo(MigMonUtilities.sqlDateTimeMaxValue) >= 0)
			{
				return MigMonUtilities.sqlDateTimeMaxValue;
			}
			return result;
		}

		public static SqlDateTime ConvertToSqlDateTime(string s)
		{
			return new SqlDateTime(MigMonUtilities.ConverttoDateTime(s));
		}

		public static Guid ConvertToGuid(string s)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				return Guid.Empty;
			}
			Guid result;
			if (Guid.TryParse(s, out result))
			{
				return result;
			}
			return Guid.Empty;
		}

		public static string Encrypt(string originalString)
		{
			string result;
			using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
			{
				MemoryStream memoryStream = new MemoryStream();
				byte[] bytes = Encoding.ASCII.GetBytes("ABE94726E3321250");
				CryptoStream cryptoStream = new CryptoStream(memoryStream, aesCryptoServiceProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
				StreamWriter streamWriter = new StreamWriter(cryptoStream);
				streamWriter.Write(originalString);
				streamWriter.Flush();
				cryptoStream.FlushFinalBlock();
				streamWriter.Flush();
				result = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
			}
			return result;
		}

		public static string Decrypt(string cryptedString)
		{
			string result;
			using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
			{
				MemoryStream stream = new MemoryStream(Convert.FromBase64String(cryptedString));
				byte[] bytes = Encoding.ASCII.GetBytes("ABE94726E3321250");
				CryptoStream stream2 = new CryptoStream(stream, aesCryptoServiceProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
				StreamReader streamReader = new StreamReader(stream2);
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		public static int? GetLocalServerId()
		{
			if (!MigrationMonitor.KnownStringIdMap[KnownStringType.LocalServerName].ContainsKey(MigrationMonitor.ComputerName))
			{
				MigrationMonitor.KnownStringIdMap[KnownStringType.LocalServerName].Add(MigrationMonitor.ComputerName, new int?(MigrationMonitor.SqlHelper.GetLoggingServerId()));
			}
			return MigrationMonitor.KnownStringIdMap[KnownStringType.LocalServerName][MigrationMonitor.ComputerName];
		}

		public static int? GetValueFromIdMap(string stringValue, KnownStringType stringType, string sqlLookupName)
		{
			if (stringType <= KnownStringType.EndpointGuid)
			{
				if (stringType == KnownStringType.TenantName)
				{
					return MigMonUtilities.GetTenantNameId(stringValue);
				}
				if (stringType == KnownStringType.EndpointGuid)
				{
					return MigMonUtilities.GetEndpointId(stringValue);
				}
			}
			else if (stringType != KnownStringType.WatsonHash)
			{
				if (stringType == KnownStringType.LocalServerName)
				{
					return MigMonUtilities.GetLocalServerId();
				}
			}
			else
			{
				if (!MigrationMonitor.KnownStringIdMap[stringType].ContainsKey(stringValue))
				{
					return null;
				}
				return MigrationMonitor.KnownStringIdMap[stringType][stringValue];
			}
			if (!MigrationMonitor.KnownStringIdMap[stringType].ContainsKey(stringValue))
			{
				MigrationMonitor.KnownStringIdMap[stringType].Add(stringValue, MigrationMonitor.SqlHelper.GetIdForKnownString(stringValue, sqlLookupName));
			}
			return MigrationMonitor.KnownStringIdMap[stringType][stringValue];
		}

		public static int? GetTenantNameId(string tenantName)
		{
			if (!MigrationMonitor.KnownStringIdMap[KnownStringType.TenantName].ContainsKey(tenantName))
			{
				MigrationMonitor.KnownStringIdMap[KnownStringType.TenantName].Add(tenantName, new int?(MigrationMonitor.SqlHelper.GetIdForKnownTenantName(tenantName)));
			}
			return MigrationMonitor.KnownStringIdMap[KnownStringType.TenantName][tenantName];
		}

		public static int? GetEndpointId(string endpointGuid)
		{
			if (!MigrationMonitor.KnownStringIdMap[KnownStringType.EndpointGuid].ContainsKey(endpointGuid))
			{
				MigrationMonitor.KnownStringIdMap[KnownStringType.EndpointGuid].Add(endpointGuid, new int?(MigrationMonitor.SqlHelper.GetIdForKnownEndpoint(endpointGuid)));
			}
			return MigrationMonitor.KnownStringIdMap[KnownStringType.EndpointGuid][endpointGuid];
		}

		public static int? GetWatsonHashId(string watsonHash, string stackTrace, string service)
		{
			string service2 = MigMonUtilities.TruncateMessage(service, 32);
			if (!MigrationMonitor.KnownStringIdMap[KnownStringType.WatsonHash].ContainsKey(watsonHash))
			{
				if (string.IsNullOrWhiteSpace(stackTrace))
				{
					return null;
				}
				MigrationMonitor.KnownStringIdMap[KnownStringType.WatsonHash].Add(watsonHash, new int?(MigrationMonitor.SqlHelper.GetIdForKnownWatsonHash(watsonHash, stackTrace, service2)));
			}
			return MigrationMonitor.KnownStringIdMap[KnownStringType.WatsonHash][watsonHash];
		}

		public static string GetColumnStringValue(CsvRow logRow, string columnName)
		{
			if (logRow.ColumnMap.Contains(columnName))
			{
				return logRow[columnName];
			}
			return string.Empty;
		}

		public static T GetColumnValue<T>(CsvRow logRow, string columnName)
		{
			if (!logRow.ColumnMap.Contains(columnName))
			{
				return default(T);
			}
			if (string.IsNullOrWhiteSpace(logRow[columnName]))
			{
				return default(T);
			}
			if (typeof(T) == typeof(Guid))
			{
				Guid guid = MigMonUtilities.ConvertToGuid(logRow[columnName]);
				return (T)((object)Convert.ChangeType(guid, typeof(T)));
			}
			if (typeof(T) == typeof(string))
			{
				return (T)((object)Convert.ChangeType(MigMonUtilities.GetColumnStringValue(logRow, columnName), typeof(T)));
			}
			if (typeof(T) == typeof(SqlDateTime))
			{
				SqlDateTime sqlDateTime = MigMonUtilities.ConvertToSqlDateTime(logRow[columnName]);
				return (T)((object)Convert.ChangeType(sqlDateTime, typeof(T)));
			}
			if (typeof(T) == typeof(DateTime))
			{
				DateTime dateTime = MigMonUtilities.ConverttoDateTime(logRow[columnName]);
				return (T)((object)Convert.ChangeType(dateTime, typeof(T)));
			}
			if (!(typeof(T) == typeof(int)) && !(typeof(T) == typeof(bool)) && !(typeof(T) == typeof(long)) && !(typeof(T) == typeof(double)))
			{
				if (!(typeof(T) == typeof(float)))
				{
					goto IL_20B;
				}
			}
			try
			{
				return (T)((object)Convert.ChangeType(logRow[columnName], typeof(T)));
			}
			catch (Exception ex)
			{
				if (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
				{
					return default(T);
				}
				throw;
			}
			IL_20B:
			throw new ArgumentException("Only support string, int, bool, SqlDateTime, DateTime, long, double, float and Guid");
		}

		public static ColumnDefinition<int> GetLookupColumnDefinition(List<ColumnDefinition<int>> columnsList, KnownStringType stringType)
		{
			if (!columnsList.Any<ColumnDefinition<int>>())
			{
				return new ColumnDefinition<int>();
			}
			return columnsList.FirstOrDefault((ColumnDefinition<int> t) => t.KnownStringType == stringType);
		}

		public static string TruncateMessage(string message, int length = 500)
		{
			if (string.IsNullOrEmpty(message) || message.Length <= length)
			{
				return message;
			}
			return message.Substring(0, length - 3) + "...";
		}

		private const string CryptoKey = "ABE94726E3321250";

		private const int MaxMessageLength = 500;

		private const int MaxErrorLength = 4000;

		private static readonly DateTime sqlDateTimeMinValue = SqlDateTime.MinValue.Value;

		private static readonly DateTime sqlDateTimeMaxValue = SqlDateTime.MaxValue.Value;
	}
}
