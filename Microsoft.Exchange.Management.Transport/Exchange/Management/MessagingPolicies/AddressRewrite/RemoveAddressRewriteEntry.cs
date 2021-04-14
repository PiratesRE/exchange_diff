using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AddressRewrite
{
	[Cmdlet("remove", "addressrewriteentry", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class RemoveAddressRewriteEntry : RemoveSystemConfigurationObjectTask<AddressRewriteEntryIdParameter, AddressRewriteEntry>
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveAddressrewriteentry(this.Identity.ToString());
			}
		}
	}
}
