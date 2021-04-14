using System;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal class FeaturesStateOverride : IFeaturesStateOverride
	{
		internal FeaturesStateOverride(VariantConfigurationSnapshot snapshot, RecipientTypeDetails recipientTypeDetails)
		{
			this.snapshot = snapshot;
			this.recipientTypeDetails = recipientTypeDetails;
		}

		public bool IsFeatureEnabled(string featureId)
		{
			if (!string.IsNullOrEmpty(featureId) && featureId == this.snapshot.OwaClientServer.ModernGroups.Name)
			{
				if (RequestDispatcherUtilities.GetStringUrlParameter(HttpContext.Current, "sharepointapp") == "true")
				{
					return false;
				}
				if (this.recipientTypeDetails == RecipientTypeDetails.SharedMailbox)
				{
					return false;
				}
			}
			return string.IsNullOrEmpty(featureId) || (!(featureId == this.snapshot.OwaClientServer.O365ParityHeader.Name) && !(featureId == this.snapshot.OwaClientServer.O365Header.Name) && !(featureId == this.snapshot.OwaClientServer.O365G2Header.Name)) || !(RequestDispatcherUtilities.GetStringUrlParameter(HttpContext.Current, "sharepointapp") == "true");
		}

		private readonly VariantConfigurationSnapshot snapshot;

		private readonly RecipientTypeDetails recipientTypeDetails;
	}
}
