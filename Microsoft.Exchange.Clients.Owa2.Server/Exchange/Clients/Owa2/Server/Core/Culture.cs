using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public static class Culture
	{
		public static List<CultureInfo> SupportedUMCultures
		{
			get
			{
				if (Culture.cacheUMCultures)
				{
					if (Culture.supportedUMDatacenterCultures == null)
					{
						lock (Culture.lockObject)
						{
							if (Culture.supportedUMDatacenterCultures == null)
							{
								Culture.supportedUMDatacenterCultures = new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.UnifiedMessaging));
							}
						}
					}
					return Culture.supportedUMDatacenterCultures;
				}
				return new List<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.UnifiedMessaging));
			}
		}

		public static bool IsCultureSpeechEnabled(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("Culture cannot be null");
			}
			return Culture.SupportedUMCultures.Contains(culture) && LocConfig.Instance[culture].MowaSpeech.EnableMowaVoice;
		}

		public static CultureInfo GetUserCulture()
		{
			return CultureInfo.CurrentUICulture;
		}

		internal static void InternalSetThreadPreferredCulture()
		{
			Culture.InternalSetThreadPreferredCulture(CultureInfo.GetCultureInfo("en-US"));
		}

		internal static void InternalSetThreadPreferredCulture(IExchangePrincipal exchangePrincipal)
		{
			Culture.InternalSetThreadPreferredCulture(Culture.GetPreferredCultureInfo(exchangePrincipal));
		}

		internal static void InternalSetThreadPreferredCulture(CultureInfo culture)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "Culture.InternalSetThreadCulture, LCID={0}", culture.LCID);
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		internal static void InternalSetAsyncThreadCulture(CultureInfo culture)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "Culture.InternalSetAsyncThreadCulture, LCID={0}", culture.LCID);
			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		internal static CultureInfo GetPreferredCultureInfo(IExchangePrincipal exchangePrincipal)
		{
			return ClientCultures.GetPreferredCultureInfo(exchangePrincipal.PreferredCultures);
		}

		internal static CultureInfo GetPreferredCultureInfo(ADUser adUser)
		{
			return ClientCultures.GetPreferredCultureInfo(adUser.Languages);
		}

		internal static void SetPreferredCulture(IExchangePrincipal exchangePrincipal, IEnumerable<CultureInfo> preferredCultures, IRecipientSession recipientSession)
		{
			ADUser aduser = recipientSession.Read(exchangePrincipal.ObjectId) as ADUser;
			if (aduser != null)
			{
				aduser.Languages.Clear();
				foreach (CultureInfo item in preferredCultures)
				{
					aduser.Languages.Add(item);
				}
				recipientSession.Save(aduser);
			}
		}

		private static List<CultureInfo> supportedUMDatacenterCultures = null;

		private static bool cacheUMCultures = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.CacheUMCultures.Enabled;

		private static object lockObject = new object();
	}
}
