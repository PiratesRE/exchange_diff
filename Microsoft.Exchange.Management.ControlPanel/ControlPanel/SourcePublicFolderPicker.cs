using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Management.DDIService;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class SourcePublicFolderPicker
	{
		public static string CurrentPath
		{
			get
			{
				return SourcePublicFolderPicker.currentPath;
			}
			set
			{
				SourcePublicFolderPicker.currentPath = value;
			}
		}

		public static void GetListPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			List<string> list = new List<string>();
			string value = "\\";
			if (inputRow["SearchText"] != DBNull.Value && !string.IsNullOrWhiteSpace((string)inputRow["SearchText"]))
			{
				value = (((string)inputRow["SearchText"]).StartsWith("\\") ? ((string)inputRow["SearchText"]) : ((string)inputRow["SearchText"]).Insert(0, "\\"));
				inputRow["SearchText"] = value;
				list.Add("SearchText");
			}
			SourcePublicFolderPicker.CurrentPath = value;
			if (list.Count > 0)
			{
				store.SetModifiedColumns(list);
			}
		}

		public static void GetListPostAction(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			List<DataRow> list = new List<DataRow>();
			dataTable.BeginLoadData();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (string.Compare((string)dataRow["FolderPath"], "\\", StringComparison.Ordinal) == 0)
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

		internal const string SearchTextColumnName = "SearchText";

		private static string currentPath = "\\";
	}
}
