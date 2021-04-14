using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class MerMailEnabledGroupConfigurable : MerMailEnableRecipientConfigurable
	{
		public MerMailEnabledGroupConfigurable() : base(true, false, false, Strings.MailEnabledGroup)
		{
		}

		public override ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			ResultsLoaderProfile resultsLoaderProfile = base.BuildResultsLoaderProfile();
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.MerMailEnabledGroupPicker";
			return resultsLoaderProfile;
		}
	}
}
