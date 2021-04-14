using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class RecipientDataSourceService : DataSourceService
	{
		protected new PowerShellResults<O> GetObject<O>(string getCmdlet, Identity identity)
		{
			return this.GetObject<O>(getCmdlet, identity, true);
		}

		protected PowerShellResults<O> GetObject<O>(string getCmdlet, Identity identity, bool readFromDomainController)
		{
			if (readFromDomainController)
			{
				return base.GetObject<O>(new PSCommand().AddCommand(getCmdlet).AddParameter("ReadFromDomainController"), identity);
			}
			return base.GetObject<O>(new PSCommand().AddCommand(getCmdlet), identity);
		}
	}
}
