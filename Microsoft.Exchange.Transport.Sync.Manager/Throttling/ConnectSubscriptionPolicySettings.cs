using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConnectSubscriptionPolicySettings
	{
		internal bool IsFacebookDisabled
		{
			get
			{
				return !this.facebookEnabled;
			}
		}

		internal bool IsLinkedInDisabled
		{
			get
			{
				return !this.linkedInEnabled;
			}
		}

		internal static ConnectSubscriptionPolicySettings GetFallbackInstance()
		{
			return new ConnectSubscriptionPolicySettings();
		}

		private ConnectSubscriptionPolicySettings()
		{
			this.facebookEnabled = true;
			this.linkedInEnabled = true;
		}

		internal ConnectSubscriptionPolicySettings(OwaSegmentationSettings owaSegmentationSettings)
		{
			this.facebookEnabled = owaSegmentationSettings[OwaMailboxPolicySchema.FacebookEnabled];
			this.linkedInEnabled = owaSegmentationSettings[OwaMailboxPolicySchema.LinkedInEnabled];
		}

		private readonly bool facebookEnabled;

		private readonly bool linkedInEnabled;
	}
}
