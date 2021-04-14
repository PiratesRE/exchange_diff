using System;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa2.Server.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IUserContext : IMailboxContext, IDisposable
	{
		long SignIntoIMTime { get; set; }

		InstantMessagingTypeOptions InstantMessageType { get; }

		string SipUri { get; }

		long LastUserRequestTime { get; }

		CultureInfo UserCulture { get; set; }

		PlayOnPhoneNotificationManager PlayOnPhoneNotificationManager { get; }

		InstantMessageManager InstantMessageManager { get; }

		BposNavBarInfoAssetReader BposNavBarInfoAssetReader { get; }

		BposShellInfoAssetReader BposShellInfoAssetReader { get; }

		bool IsInstantMessageEnabled { get; }

		FeaturesManager FeaturesManager { get; }

		string CurrentOwaVersion { get; }

		void UpdateLastUserRequestTime();
	}
}
