using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PreferredCultureLocalizer
	{
		internal PreferredCultureLocalizer(IExchangePrincipal userExchangePrincipal)
		{
			SyncUtilities.ThrowIfArgumentNull("userExchangePrincipal", userExchangePrincipal);
			this.InitializeState(userExchangePrincipal.PreferredCultures, PreferredCultureLocalizer.supportedCultures);
		}

		internal PreferredCultureLocalizer(PreferredCultures preferredCultures, List<CultureInfo> supportedCultures)
		{
			SyncUtilities.ThrowIfArgumentNull("preferredCultures", preferredCultures);
			SyncUtilities.ThrowIfArgumentNull("supportedCultures", supportedCultures);
			this.InitializeState(preferredCultures, supportedCultures);
		}

		private PreferredCultureLocalizer()
		{
			this.cultureInfoToApply = null;
		}

		public static PreferredCultureLocalizer DefaultThreadCulture
		{
			get
			{
				return PreferredCultureLocalizer.defaultThreadCulture;
			}
		}

		protected CultureInfo CultureInfoToApply
		{
			get
			{
				return this.cultureInfoToApply;
			}
		}

		public string Apply(LocalizedString localizedString)
		{
			if (this.CultureInfoToApply == null)
			{
				return localizedString.ToString();
			}
			return localizedString.ToString(this.CultureInfoToApply);
		}

		private void InitializeState(IEnumerable<CultureInfo> preferredCultures, List<CultureInfo> supportedCultures)
		{
			if (!preferredCultures.Any<CultureInfo>())
			{
				this.cultureInfoToApply = null;
				return;
			}
			foreach (CultureInfo item in preferredCultures)
			{
				if (supportedCultures.Contains(item))
				{
					this.cultureInfoToApply = item;
					break;
				}
			}
		}

		private static readonly List<CultureInfo> supportedCultures = new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));

		private static readonly PreferredCultureLocalizer defaultThreadCulture = new PreferredCultureLocalizer();

		private CultureInfo cultureInfoToApply;
	}
}
