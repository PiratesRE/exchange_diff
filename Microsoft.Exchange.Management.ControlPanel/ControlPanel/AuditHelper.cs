using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class AuditHelper
	{
		public static string[] GetIdentitiesFromSmtpAddresses(SmtpAddress[] addresses)
		{
			List<string> list = new List<string>();
			foreach (RecipientObjectResolverRow recipientObjectResolverRow in RecipientObjectResolver.Instance.ResolveSmtpAddress(addresses))
			{
				list.Add(recipientObjectResolverRow.ADRawEntry.Id.ToString());
			}
			return list.ToArray();
		}

		public static Identity CreateIdentity(string mailbox, string startDate, string endDate)
		{
			startDate = (startDate.IsNullOrBlank() ? "NoStart" : startDate);
			endDate = (endDate.IsNullOrBlank() ? "NoEnd" : endDate);
			string text = string.Format("{0};{1};{2}", mailbox, startDate, endDate);
			return new Identity(text, text);
		}

		public static Identity CreateNonOwnerAccessIdentity(string mailbox, string startDate, string endDate, string logonTypes)
		{
			startDate = (startDate.IsNullOrBlank() ? "NoStart" : startDate);
			endDate = (endDate.IsNullOrBlank() ? "NoEnd" : endDate);
			string text = string.Format("{0};{1};{2};{3}", new object[]
			{
				mailbox,
				startDate,
				endDate,
				logonTypes
			});
			return new Identity(text, text);
		}

		public static string GetLaterDate(string date1, string date2)
		{
			DateTime t;
			if (!DateTime.TryParse(date1, out t))
			{
				throw new ArgumentException("date1: {0}", date1);
			}
			DateTime t2;
			if (!DateTime.TryParse(date2, out t2))
			{
				throw new ArgumentException("date2: {0}", date2);
			}
			if (!(t > t2))
			{
				return date2;
			}
			return date1;
		}

		public static string MakeUserFriendly(string objectName)
		{
			if (objectName != null && objectName.Contains("/"))
			{
				int num = objectName.LastIndexOf("/") + 1;
				if (num < objectName.Length)
				{
					objectName = objectName.Substring(num, objectName.Length - num);
				}
				else
				{
					objectName = string.Empty;
				}
			}
			return objectName;
		}

		public static string GetDateForAuditReportsFilter(string value, bool isEndDate)
		{
			if (!value.IsNullOrBlank())
			{
				ExDateTime? exDateTime;
				if (value.Length == "yyyy/MM/dd".Length)
				{
					exDateTime = value.ToEcpExDateTime("yyyy/MM/dd");
				}
				else if (value.Length == "yyyy/MM/dd HH:mm:ss".Length)
				{
					exDateTime = value.ToEcpExDateTime("yyyy/MM/dd HH:mm:ss");
				}
				else
				{
					exDateTime = value.ToEcpExDateTime("yyyy-MM-ddTHH:mm:ss.fffffffzzz");
				}
				if (exDateTime != null)
				{
					DateTime dateTime = new DateTime(exDateTime.Value.Year, exDateTime.Value.Month, exDateTime.Value.Day, exDateTime.Value.Hour, exDateTime.Value.Minute, exDateTime.Value.Second, exDateTime.Value.Millisecond, DateTimeKind.Local);
					if (isEndDate)
					{
						return dateTime.AddDays(1.0).ToString("o", CultureInfo.InvariantCulture.DateTimeFormat);
					}
					return dateTime.ToString("o", CultureInfo.InvariantCulture.DateTimeFormat);
				}
			}
			return null;
		}

		internal const string NoStart = "NoStart";

		internal const string NoEnd = "NoEnd";

		internal const string ReadScope = "@R:Organization";
	}
}
