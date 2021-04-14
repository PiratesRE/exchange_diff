using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.Psws;
using Microsoft.Exchange.Management.FfoQuarantine;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class QuarantineCodeBehind
	{
		public static void IdentityToStringAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			List<string> list = new List<string>();
			Identity identity = inputrow["Identity"] as Identity;
			inputrow["Identity"] = identity.RawIdentity.Replace(" ", "+");
			list.Add("Identity");
			store.SetModifiedColumns(list);
		}

		public static void GetObjectPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			dataRow["Identity"] = new Identity(dataRow["Identity"].ToString().Replace(" ", "+"));
		}

		public static string ConvertSizeForDisplay(int size)
		{
			return new ByteQuantifiedSize((ulong)Convert.ToUInt32(size)).ToAppropriateUnitFormatString();
		}

		public static bool IsSpamMessage(object value)
		{
			if (value != null)
			{
				QuarantineMessageType quarantineMessageType = (QuarantineMessageType)value;
				return quarantineMessageType.Value == QuarantineMessageTypeEnum.Spam;
			}
			return false;
		}

		public static string ConvertEnumToLocalizedString(object value)
		{
			return DDIUtil.EnumToLocalizedString(((QuarantineMessageType)value).Value);
		}

		public static string ToUserDateTime(object value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			string result;
			try
			{
				ExDateTime dateTimeValue = new ExDateTime(EcpDateTimeHelper.GetCurrentUserTimeZone(), (DateTime)value);
				result = dateTimeValue.ToUserDateTimeString();
			}
			catch
			{
				result = string.Empty;
			}
			return result;
		}

		public static void GetRecipientsPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			dataRow["Identity"] = new Identity(dataRow["Identity"].ToString().Replace(" ", "+"));
		}

		public static string[] GetNotYetReleasedUsers(object quarantinedUsers, object releasedUsers)
		{
			IEnumerable<string> first;
			if (quarantinedUsers == null)
			{
				first = new List<string>();
			}
			else
			{
				first = (List<string>)quarantinedUsers;
			}
			IEnumerable<string> second;
			if (releasedUsers == null)
			{
				second = new List<string>();
			}
			else
			{
				second = (List<string>)releasedUsers;
			}
			return first.Except(second, StringComparer.OrdinalIgnoreCase).ToArray<string>();
		}

		public static void GetListPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				dataRow["Identity"] = new Identity(dataRow["Identity"].ToString().Replace(" ", "+"));
			}
		}

		public static void GetListPreAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			ExDateTime now = ExDateTime.Now;
			ExDateTime value = ExDateTime.Create(EcpDateTimeHelper.GetCurrentUserTimeZone(), (DateTime)now.ToUserExDateTime()).First<ExDateTime>();
			TimeSpan value2 = now.Subtract(value);
			if (!DDIHelper.IsEmptyValue(inputrow["ReceivedFilter"]))
			{
				string text = inputrow["ReceivedFilter"].ToStringWithNull();
				string a;
				ExDateTime exDateTime;
				ExDateTime exDateTime2;
				if ((a = text) != null)
				{
					if (a == "2d")
					{
						exDateTime = value.Date.AddDays(-2.0);
						exDateTime2 = value.Date;
						goto IL_13A;
					}
					if (a == "7d")
					{
						exDateTime = value.Date.AddDays(-7.0);
						exDateTime2 = value.Date;
						goto IL_13A;
					}
					if (a == "custom")
					{
						exDateTime = ExDateTime.ParseExact(inputrow["ReceivedStartDateFilter"].ToString(), "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
						exDateTime2 = ExDateTime.ParseExact(inputrow["ReceivedEndDateFilter"].ToString(), "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
						goto IL_13A;
					}
				}
				exDateTime = value.Date;
				exDateTime2 = value.Date.AddDays(-1.0);
				IL_13A:
				inputrow["ReceivedStartDateFilter"] = exDateTime.Add(value2);
				inputrow["ReceivedEndDateFilter"] = exDateTime2.Add(value2);
			}
			else
			{
				inputrow["ReceivedStartDateFilter"] = DBNull.Value;
				inputrow["ReceivedEndDateFilter"] = DBNull.Value;
			}
			store.ModifiedColumns.Add("ReceivedStartDateFilter");
			store.ModifiedColumns.Add("ReceivedEndDateFilter");
			if (!DDIHelper.IsEmptyValue(inputrow["ExpiresFilter"]))
			{
				string text2 = inputrow["ExpiresFilter"].ToStringWithNull();
				string a2;
				ExDateTime exDateTime3;
				ExDateTime exDateTime4;
				if ((a2 = text2) != null)
				{
					if (a2 == "2d")
					{
						exDateTime3 = value.Date;
						exDateTime4 = value.Date.AddDays(2.0);
						goto IL_2C5;
					}
					if (a2 == "7d")
					{
						exDateTime3 = value.Date;
						exDateTime4 = value.Date.AddDays(7.0);
						goto IL_2C5;
					}
					if (a2 == "custom")
					{
						exDateTime3 = ExDateTime.ParseExact(inputrow["ExpiresStartDateFilter"].ToString(), "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
						exDateTime4 = ExDateTime.ParseExact(inputrow["ExpiresEndDateFilter"].ToString(), "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
						goto IL_2C5;
					}
				}
				exDateTime3 = value.Date;
				exDateTime4 = value.Date.AddDays(1.0);
				IL_2C5:
				inputrow["ExpiresStartDateFilter"] = exDateTime3.Add(value2);
				inputrow["ExpiresEndDateFilter"] = exDateTime4.Add(value2);
			}
			else
			{
				inputrow["ExpiresStartDateFilter"] = DBNull.Value;
				inputrow["ExpiresEndDateFilter"] = DBNull.Value;
			}
			store.ModifiedColumns.Add("ExpiresStartDateFilter");
			store.ModifiedColumns.Add("ExpiresEndDateFilter");
		}

		public static string GetExecutingUserPrimarySmtpAddress()
		{
			return (string)HttpContext.Current.Items[TokenIssuer.ItemTagUpn];
		}
	}
}
