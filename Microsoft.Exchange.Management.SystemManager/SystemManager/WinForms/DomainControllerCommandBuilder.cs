using System;
using System.Data;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class DomainControllerCommandBuilder : ProcedureBuilder
	{
		internal override MonadCommand InnerBuildProcedureCore(string commandText, DataRow row)
		{
			MonadCommand monadCommand = new LoggableMonadCommand(commandText);
			string value = null;
			if (!DBNull.Value.Equals(row["Forest"]))
			{
				value = (string)row["Forest"];
			}
			string value2 = null;
			if (!DBNull.Value.Equals(row["DomainName"]))
			{
				value2 = (string)row["DomainName"];
			}
			PSCredential pscredential = null;
			if (!DBNull.Value.Equals(row["Credential"]))
			{
				pscredential = (PSCredential)row["Credential"];
			}
			bool flag = false;
			if (!DBNull.Value.Equals(row["IsGlobalCatalog"]))
			{
				flag = (bool)row["IsGlobalCatalog"];
			}
			if (flag)
			{
				monadCommand.Parameters.AddSwitch("GlobalCatalog");
			}
			if (!string.IsNullOrEmpty(value))
			{
				monadCommand.Parameters.AddWithValue("Forest", value);
			}
			if (!string.IsNullOrEmpty(value2))
			{
				monadCommand.Parameters.AddWithValue("DomainName", value2);
			}
			if (pscredential != null)
			{
				monadCommand.Parameters.AddWithValue("Credential", pscredential);
			}
			return monadCommand;
		}
	}
}
