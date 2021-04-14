using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DDIService
{
	public class SharingDomainPickerCodeBehind
	{
		public static void FilterForSharingDomain(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			List<DataRow> list = new List<DataRow>();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if ((AcceptedDomainType)dataRow["DomainType"] == AcceptedDomainType.ExternalRelay || ((string)dataRow["DomainName"]).IndexOf("*") >= 0)
				{
					list.Add(dataRow);
				}
			}
			foreach (DataRow row2 in list)
			{
				dataTable.Rows.Remove(row2);
			}
		}
	}
}
