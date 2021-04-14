using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DDIService
{
	public class HybridConfigurationServiceCodeBehind
	{
		public static void SetHybridConfigurationEnabled(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			HybridConfiguration hybridConfiguration = store.GetDataObject("HybridConfiguration") as HybridConfiguration;
			bool flag = hybridConfiguration != null && !DDIHelper.IsLegacyObject(hybridConfiguration);
			dataTable.Rows[0]["HybridConfigurationEnabled"] = flag;
			if (flag)
			{
				dataTable.Rows[0]["IsHostedOnGallatin"] = (hybridConfiguration.ServiceInstance == 1);
			}
		}

		public static void ExtractTenantDomainInfo(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			IEnumerable<object> enumerable = store.GetDataObject("AcceptedDomain") as IEnumerable<object>;
			List<string> list = new List<string>();
			bool flag = false;
			if (enumerable != null)
			{
				foreach (object obj in enumerable)
				{
					AcceptedDomain acceptedDomain = (AcceptedDomain)obj;
					if (!flag && acceptedDomain.IsCoexistenceDomain)
					{
						flag = true;
					}
					if (!acceptedDomain.IsCoexistenceDomain && acceptedDomain.DomainName.SmtpDomain != null)
					{
						list.Add(acceptedDomain.DomainName.SmtpDomain.ToString());
					}
				}
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["HasCoexistenceDomain"] = flag;
			dataRow["DomainNames"] = string.Join(",", list.ToArray());
			dataTable.Rows.Add(dataRow);
		}
	}
}
