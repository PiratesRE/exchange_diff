using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class MailboxFilterBuilder : ExcludeObjectFilterBuilder
	{
		public override void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			base.BuildFilter(out parameterList, out filter, out preArgs, row);
			ExchangeObjectVersion exchangeObjectVersion = null;
			if (!DBNull.Value.Equals(row["MinVersion"]))
			{
				exchangeObjectVersion = (row["MinVersion"] as ExchangeObjectVersion);
			}
			if (exchangeObjectVersion != null)
			{
				string text = string.Format(" | Filter-Mailbox -Value {0}", exchangeObjectVersion.ToInt64());
				filter = ((filter == null) ? text : (filter + text));
			}
		}
	}
}
