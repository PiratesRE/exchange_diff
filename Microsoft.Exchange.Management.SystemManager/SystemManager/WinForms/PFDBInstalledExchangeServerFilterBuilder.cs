using System;
using System.Data;
using System.Text;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class PFDBInstalledExchangeServerFilterBuilder : IExchangeCommandFilterBuilder
	{
		public void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			long num = 0L;
			if (!DBNull.Value.Equals(row["MinVersion"]))
			{
				num = (long)row["MinVersion"];
			}
			string text = null;
			if (!DBNull.Value.Equals(row["ExcludeServer"]))
			{
				text = (string)row["ExcludeServer"];
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(" | Filter-PublicFolderInstalledExchangeServer -minVersion {0}", num);
			if (text != null)
			{
				stringBuilder.AppendFormat(" -excludeServer '{0}'", text.ToQuotationEscapedString());
			}
			filter = stringBuilder.ToString();
			preArgs = null;
			parameterList = null;
		}
	}
}
