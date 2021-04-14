using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class MessageTraceService
	{
		public static void PreGetObjectAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			string text = string.Empty;
			if (inputrow["Identity"] is Identity)
			{
				Identity identity = (Identity)inputrow["Identity"];
				text = identity.RawIdentity;
			}
			else
			{
				text = (string)inputrow["Identity"];
			}
			string[] array = text.Split(new char[]
			{
				' '
			});
			inputrow["MessageTraceId"] = array[0];
			inputrow["RecipientAddress"] = array[1];
			store.SetModifiedColumns(new List<string>(new string[]
			{
				"MessageTraceId",
				"RecipientAddress"
			}));
		}

		public static void PostGetObjectAction(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			DataRow dataRow = table.Rows[0];
			dataRow["SizeInKB"] = string.Format(Strings.MTRTDetailsMessageSizeInKB, Convert.ToInt32(dataRow["Size"]) / 1024);
		}

		public static void PostGetListAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				string text = (string)dataRow["RecipientAddress"];
				string arg = ((Guid)dataRow["MessageTraceId"]).ToString();
				string rawIdentity = string.Format("{0} {1}", arg, text);
				dataRow["Identity"] = new Identity(rawIdentity, text);
			}
		}

		public static void PreGetListAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			ExDateTime exDateTime = ExDateTime.Now;
			ExDateTime exDateTime2 = ExDateTime.Now;
			if (!DDIHelper.IsEmptyValue(inputrow["DateRangeType"]))
			{
				string a;
				if ((a = inputrow["DateRangeType"].ToString()) != null)
				{
					if (a == "24h")
					{
						exDateTime = ExDateTime.Now.AddHours(-24.0);
						goto IL_13F;
					}
					if (a == "7d")
					{
						exDateTime = ExDateTime.Now.AddDays(-7.0);
						goto IL_13F;
					}
					if (a == "custom")
					{
						ExTimeZone utcTimeZone = ExTimeZone.UtcTimeZone;
						if (!DDIHelper.IsEmptyValue(inputrow["TimeZone"]))
						{
							ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(inputrow["TimeZone"].ToString(), out utcTimeZone);
						}
						if (!DDIHelper.IsEmptyValue(inputrow["StartDate"]))
						{
							exDateTime = ExDateTime.Parse(utcTimeZone, inputrow["StartDate"].ToString());
						}
						if (!DDIHelper.IsEmptyValue(inputrow["EndDate"]))
						{
							exDateTime2 = ExDateTime.Parse(utcTimeZone, inputrow["EndDate"].ToString());
							goto IL_13F;
						}
						goto IL_13F;
					}
				}
				exDateTime = ExDateTime.Now.AddHours(-48.0);
				IL_13F:
				inputrow["StartDate"] = (DateTime)exDateTime.ToUtc();
				inputrow["EndDate"] = (DateTime)exDateTime2.ToUtc();
			}
			else
			{
				inputrow["StartDate"] = DBNull.Value;
				inputrow["EndDate"] = DBNull.Value;
			}
			store.ModifiedColumns.Add("StartDate");
			store.ModifiedColumns.Add("EndDate");
		}

		public static string FormatDateTime(object value)
		{
			return Convert.ToDateTime(value).ToString("G");
		}

		public static ExDateTime GetDefaultStartDate()
		{
			return ExDateTime.Now.AddDays(-7.0).ToUtc();
		}

		public static ExDateTime GetDefaultEndDate()
		{
			return ExDateTime.Now.ToUtc();
		}
	}
}
