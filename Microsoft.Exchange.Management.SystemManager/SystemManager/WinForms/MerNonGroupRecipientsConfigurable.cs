using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class MerNonGroupRecipientsConfigurable : MerMailEnableRecipientConfigurable
	{
		public MerNonGroupRecipientsConfigurable() : this(true)
		{
		}

		public MerNonGroupRecipientsConfigurable(bool allowedPublicFolder) : base(false, true, allowedPublicFolder, Strings.RecipientUserOrContact)
		{
		}

		public override ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			ResultsLoaderProfile resultsLoaderProfile = base.BuildResultsLoaderProfile();
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.MerNonGroupRecipientsPicker";
			return resultsLoaderProfile;
		}
	}
}
