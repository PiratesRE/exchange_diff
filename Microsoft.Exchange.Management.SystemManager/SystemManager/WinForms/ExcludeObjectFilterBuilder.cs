using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ExcludeObjectFilterBuilder : IExchangeCommandFilterBuilder
	{
		public virtual void BuildFilter(out string parameterList, out string filter, out string preArgs, DataRow row)
		{
			ADObjectId adobjectId = null;
			if (!DBNull.Value.Equals(row["ExcludeObject"]))
			{
				adobjectId = (row["ExcludeObject"] as ADObjectId);
			}
			filter = null;
			if (adobjectId != null)
			{
				filter = string.Format(" | Filter-PropertyNotEqualTo -Property 'Identity' -Value '{0}'", adobjectId.ObjectGuid.ToQuotationEscapedString());
			}
			parameterList = null;
			preArgs = null;
		}
	}
}
