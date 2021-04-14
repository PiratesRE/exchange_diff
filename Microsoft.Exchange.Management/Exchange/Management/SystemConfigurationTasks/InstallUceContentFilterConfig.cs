using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "UceContentFilterConfig")]
	public sealed class InstallUceContentFilterConfig : NewFixedNameSystemConfigurationObjectTask<UceContentFilter>
	{
		protected override IConfigurable PrepareDataObject()
		{
			UceContentFilter uceContentFilter = (UceContentFilter)base.PrepareDataObject();
			uceContentFilter.SetId((IConfigurationSession)base.DataSession, "UCE Content Filter");
			return uceContentFilter;
		}

		protected override void InternalProcessRecord()
		{
			UceContentFilter[] array = this.ConfigurationSession.FindAllPaged<UceContentFilter>().ReadAllPages();
			if (array != null && array.Length > 0)
			{
				base.WriteVerbose(Strings.UceContentFilterAlreadyExists(array[0].DistinguishedName));
				return;
			}
			base.InternalProcessRecord();
		}

		private const string CanonicalName = "UCE Content Filter";
	}
}
