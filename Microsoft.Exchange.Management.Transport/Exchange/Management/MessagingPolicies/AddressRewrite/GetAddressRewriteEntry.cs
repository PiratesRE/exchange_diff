using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.MessagingPolicies.AddressRewrite
{
	[Cmdlet("get", "addressrewriteentry")]
	public class GetAddressRewriteEntry : GetSystemConfigurationObjectTask<AddressRewriteEntryIdParameter, AddressRewriteEntry>
	{
		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.CreateSession();
			configurationSession.UseConfigNC = false;
			return configurationSession;
		}

		protected override ObjectId RootId
		{
			get
			{
				return Utils.RootId;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}
	}
}
