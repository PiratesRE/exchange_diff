using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class RetentionPolicyPropertiesHelper
	{
		public static void GetListPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			List<DataRow> list = new List<DataRow>();
			dataTable.BeginLoadData();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (string.Compare((string)dataRow["Name"], "ArbitrationMailbox", StringComparison.Ordinal) == 0)
				{
					list.Add(dataRow);
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
			dataTable.EndLoadData();
		}

		public static void WebServiceDropDownForRetention(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Text"] = Strings.NoRetentionPolicy;
			dataRow["Value"] = string.Empty;
			dataTable.Rows.InsertAt(dataRow, 0);
			for (int i = dataTable.Rows.Count - 1; i >= 1; i--)
			{
				if (string.Compare((string)dataTable.Rows[i]["Text"], "ArbitrationMailbox", StringComparison.Ordinal) == 0)
				{
					dataTable.Rows.RemoveAt(i);
					return;
				}
			}
		}

		public static void GetForSDOPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)dataRow["RetentionPolicyTagLinks"];
			dataRow["RetentionPolicyTagLinkLabel"] = ((multiValuedProperty.Count > 0) ? Strings.ViewRetentionPolicyTagLinksLabel : Strings.ViewRetentionPolicyEmptyTagLinks);
		}

		private const string ArbitrationPolicy = "ArbitrationMailbox";
	}
}
