using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class SharingPolicyAssistor
	{
		private static void UpdateFormattedNameAndDomains(DataRow row)
		{
			row["FormattedName"] = (true.Equals(row["Default"]) ? string.Format(Strings.DefaultSharingPolicyFormatString, row["Name"]) : row["Name"]);
			row["FormattedDomains"] = DDIHelper.JoinList<SharingPolicyDomain>(row["Domains"] as MultiValuedProperty<SharingPolicyDomain>, delegate(SharingPolicyDomain policyDomain)
			{
				if (policyDomain.Domain == "*")
				{
					return Strings.SharingDomainOptionAll;
				}
				return policyDomain.Domain;
			});
		}

		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			SharingPolicy sharingPolicy = store.GetDataObject("SharingPolicy") as SharingPolicy;
			if (dataTable.Rows.Count == 1 && sharingPolicy != null)
			{
				SharingPolicyAssistor.UpdateFormattedNameAndDomains(dataTable.Rows[0]);
			}
		}

		public static void GetListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			foreach (object obj in dataTable.Rows)
			{
				DataRow row = (DataRow)obj;
				SharingPolicyAssistor.UpdateFormattedNameAndDomains(row);
			}
			dataTable.EndLoadData();
		}

		public static void GetDefaultPolicyPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			int count = dataTable.Rows.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				DataRow dataRow = dataTable.Rows[i];
				if (false.Equals(dataRow["Default"]))
				{
					dataTable.Rows.Remove(dataRow);
				}
			}
			if (1 == dataTable.Rows.Count)
			{
				SharingPolicyAssistor.UpdateFormattedNameAndDomains(dataTable.Rows[0]);
			}
			dataTable.EndLoadData();
		}

		private const string ObjectName = "SharingPolicy";

		private const string DefaultColumnName = "Default";

		private const string NameColumnName = "Name";

		private const string DomainsColumnName = "Domains";

		private const string FormattedNameColumnName = "FormattedName";

		private const string FormattedDomainsColumnName = "FormattedDomains";
	}
}
