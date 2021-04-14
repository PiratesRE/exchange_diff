using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SetUserLocale : ServiceCommand<bool>
	{
		public SetUserLocale(CallContext callContext, string newUserLocale) : this(callContext, newUserLocale, false)
		{
		}

		public SetUserLocale(CallContext callContext, string newUserLocale, bool localizeFolderNames) : base(callContext)
		{
			this.newUserLocale = newUserLocale;
			this.localizeFolderNames = localizeFolderNames;
		}

		public static bool IsSupportedCulture(CultureInfo culture)
		{
			return ClientCultures.SupportedCultureInfos.Contains(culture);
		}

		protected override bool InternalExecute()
		{
			CultureInfo cultureInfo;
			try
			{
				cultureInfo = new CultureInfo(this.newUserLocale);
			}
			catch (CultureNotFoundException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string, string>(0L, "{0} is not a valid culture. Exception Message-{1}", this.newUserLocale, ex.Message);
				return false;
			}
			if (!SetUserLocale.IsSupportedCulture(cultureInfo))
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "SetUserLocale::IsSupportedCulture- {0} is not a supported culture.", this.newUserLocale);
				return false;
			}
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			ExchangePrincipal exchangePrincipal = userContext.ExchangePrincipal;
			PreferredCultures preferredCultures = new PreferredCultures(exchangePrincipal.PreferredCultures);
			preferredCultures.AddSupportedCulture(cultureInfo, new Predicate<CultureInfo>(SetUserLocale.IsSupportedCulture));
			this.SaveCultures(exchangePrincipal.ObjectId, userContext.MailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent), preferredCultures);
			userContext.ExchangePrincipal = exchangePrincipal.WithPreferredCultures(preferredCultures);
			UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.DateFormat);
			UserConfigurationPropertyDefinition propertyDefinition2 = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.TimeFormat);
			new UserOptionsType
			{
				DateFormat = MailboxRegionalConfiguration.GetDefaultDateFormat(cultureInfo),
				TimeFormat = MailboxRegionalConfiguration.GetDefaultTimeFormat(cultureInfo)
			}.Commit(base.CallContext, new UserConfigurationPropertyDefinition[]
			{
				propertyDefinition,
				propertyDefinition2
			});
			if (this.localizeFolderNames && !this.LocalizeFolders(cultureInfo))
			{
				return false;
			}
			CallContext.Current.HttpContext.Response.Cookies.Add(new HttpCookie("mkt", cultureInfo.Name));
			CallContext.Current.HttpContext.Response.Cookies.Add(new HttpCookie("UpdatedUserSettings", 32.ToString()));
			return true;
		}

		private void SaveCultures(ADObjectId adUserObjectId, IRecipientSession adRecipientSession, IEnumerable<CultureInfo> preferredCultures)
		{
			ADUser aduser = adRecipientSession.Read(adUserObjectId) as ADUser;
			if (aduser != null)
			{
				aduser.Languages.Clear();
				foreach (CultureInfo item in preferredCultures)
				{
					aduser.Languages.Add(item);
				}
				adRecipientSession.Save(aduser);
			}
		}

		private bool LocalizeFolders(CultureInfo culture)
		{
			MailboxSession mailboxSession = null;
			bool result = false;
			try
			{
				OwaIdentity logonIdentity = RequestContext.Current.UserContext.LogonIdentity;
				mailboxSession = logonIdentity.CreateMailboxSession(RequestContext.Current.UserContext.ExchangePrincipal, culture);
				Exception[] array;
				mailboxSession.LocalizeDefaultFolders(out array);
				mailboxSession.SetMailboxLocale(culture);
				if (array != null && array.Length > 0)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Exception Message-{0}", array[0].Message);
				}
				else
				{
					result = true;
				}
			}
			finally
			{
				if (mailboxSession != null)
				{
					UserContextUtilities.DisconnectStoreSession(mailboxSession);
					mailboxSession.Dispose();
					mailboxSession = null;
				}
			}
			return result;
		}

		private readonly string newUserLocale;

		private readonly bool localizeFolderNames;
	}
}
