using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[Cmdlet("Get", "LocalADSite")]
	public sealed class GetLocalADSite : Task
	{
		protected override void InternalProcessRecord()
		{
			ADSite adsite = null;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 41, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\GetLocalADSite.cs");
				adsite = topologyConfigurationSession.GetLocalSite();
			}
			catch (ADExternalException exception)
			{
				base.WriteError(exception, ErrorCategory.ObjectNotFound, null);
			}
			catch (ADTransientException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ObjectNotFound, null);
			}
			if (adsite == null)
			{
				base.WriteError(new ADExternalException(Strings.LocalSiteNotFound), ErrorCategory.ObjectNotFound, null);
				return;
			}
			base.WriteObject(adsite);
		}
	}
}
