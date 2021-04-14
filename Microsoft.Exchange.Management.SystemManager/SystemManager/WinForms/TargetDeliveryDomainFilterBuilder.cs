using System;
using System.Data;
using System.Text;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class TargetDeliveryDomainFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (row.Table.Columns.Contains("TargetDeliveryDomainOnly") && true.Equals(row["TargetDeliveryDomainOnly"]))
			{
				stringBuilder.Append(" | Filter-PropertyEqualTo -Property 'TargetDeliveryDomain' -Value true");
			}
			else
			{
				stringBuilder.Append(" | Filter-PropertyStringNotContains -Property 'DomainName' -SearchText '*'");
			}
			filter = stringBuilder.ToString();
			parameterList = null;
			preArgs = null;
		}
	}
}
