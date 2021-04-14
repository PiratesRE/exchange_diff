using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class AuditingLog
	{
		public static void PreGetObjectAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			AuditLogIdentity auditLogIdentity = new AuditLogIdentity((Identity)inputrow["Identity"]);
			inputrow["StartDate"] = auditLogIdentity.StartDate;
			inputrow["EndDate"] = auditLogIdentity.EndDate;
			inputrow["Cmdlets"] = auditLogIdentity.Cmdlet;
			inputrow["ObjectIds"] = auditLogIdentity.ObjectId;
			store.SetModifiedColumns(new List<string>(new string[]
			{
				"StartDate",
				"EndDate",
				"Cmdlets",
				"ObjectIds"
			}));
		}

		public static void PostGetObjectAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow;
			if (dataTable.Rows.Count > 1)
			{
				AuditLogIdentity changeId = new AuditLogIdentity((Identity)inputrow["Identity"]);
				List<DataRow> list = new List<DataRow>();
				foreach (object obj in dataTable.Rows)
				{
					DataRow item = (DataRow)obj;
					list.Add(item);
				}
				dataRow = list.FirstOrDefault((DataRow r) => AuditingLog.GetDetailRow(r, changeId));
				Array.ForEach<DataRow>(list.Except(new DataRow[]
				{
					dataRow
				}).ToArray<DataRow>(), delegate(DataRow r)
				{
					dataTable.Rows.Remove(r);
				});
			}
			else
			{
				dataRow = dataTable.Rows[0];
			}
			if (DBNull.Value != dataRow["CmdletParameters"])
			{
				MultiValuedProperty<AdminAuditLogCmdletParameter> source = (MultiValuedProperty<AdminAuditLogCmdletParameter>)dataRow["CmdletParameters"];
				dataRow["ParameterValues"] = string.Join(", ", (from p in source
				select string.Format("{0}: {1}", p.Name, p.Value)).ToArray<string>());
				store.SetModifiedColumns(new List<string>(new string[]
				{
					"ParameterValues"
				}));
			}
		}

		private static bool GetDetailRow(DataRow r, AuditLogIdentity changeId)
		{
			DateTime d = DateTime.MinValue;
			if (r["RunDate"] != DBNull.Value)
			{
				d = (DateTime)r["RunDate"];
			}
			return d == DateTime.ParseExact(changeId.StartDate, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture) && r["ObjectModified"].ToStringWithNull() == changeId.ObjectId && r["CmdletName"].ToStringWithNull() == changeId.Cmdlet;
		}

		public static void PostGetListAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				string startDate = ((DateTime)dataRow["RunDate"]).ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
				string endDate = ((DateTime)dataRow["RunDate"]).ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
				string objectId = dataRow["ObjectModified"].ToStringWithNull();
				string cmdlet = dataRow["CmdletName"].ToStringWithNull();
				dataRow["Identity"] = AuditingLog.CreateIdentity(objectId, cmdlet, startDate, endDate);
			}
		}

		private static Identity CreateIdentity(string objectId, string cmdlet, string startDate, string endDate)
		{
			startDate = (startDate.IsNullOrBlank() ? "NoStart" : startDate);
			endDate = (endDate.IsNullOrBlank() ? "NoEnd" : endDate);
			string text = string.Format("{0}\n{1}\n{2}\n{3}", new object[]
			{
				objectId,
				startDate,
				endDate,
				cmdlet
			});
			return new Identity(text, text);
		}

		public static void PreGetListAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			List<string> list = new List<string>();
			if (inputrow["StartDate"] != DBNull.Value)
			{
				inputrow["StartDate"] = AuditHelper.GetDateForAuditReportsFilter((string)inputrow["StartDate"], false);
				list.Add("StartDate");
			}
			if (inputrow["EndDate"] != DBNull.Value)
			{
				inputrow["EndDate"] = AuditHelper.GetDateForAuditReportsFilter((string)inputrow["EndDate"], true);
				list.Add("EndDate");
			}
			store.SetModifiedColumns(list);
		}
	}
}
