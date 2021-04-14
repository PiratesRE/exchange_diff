using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class AnonymousCalendar : DefaultPageBase
	{
		public ResourceBase[] Resources
		{
			get
			{
				return this.UserDataEmbeddedLinks;
			}
		}

		public override string TenantId
		{
			get
			{
				return "AnonymousCalendar";
			}
		}

		public override string MdbGuid
		{
			get
			{
				return "AnonymousCalendar";
			}
		}

		public override string SlabsManifest
		{
			get
			{
				return SlabManifestCollectionFactory.GetInstance(this.VersionString).GetSlabsJson(SlabManifestType.Anonymous, new string[0], this.UserAgent.Layout);
			}
		}

		public string CurrentUICultureName
		{
			get
			{
				return Thread.CurrentThread.CurrentUICulture.Name;
			}
		}

		public string CurrentResourceLocalizedCultureName
		{
			get
			{
				ResourceBase[] nonThemedUserDataEmbededLinks = UserResourcesFinder.GetNonThemedUserDataEmbededLinks(base.GetBootSlab(), this.VersionString);
				return ((LocalizedStringsScriptResource)nonThemedUserDataEmbededLinks.First((ResourceBase t) => t is LocalizedStringsScriptResource)).GetLocalizedCultureName();
			}
		}

		public string DefaultStylesFolder
		{
			get
			{
				return string.Format(base.BootStylesFolderFormat, base.Theme).Replace("#LCL", DefaultPageBase.ThemeStyleCultureDirectory);
			}
		}

		public string OwaTitle
		{
			get
			{
				return AntiXssEncoder.HtmlEncode(Strings.AnonymousCalendarTitle(AnonymousUserContext.Current.ExchangePrincipal.MailboxInfo.DisplayName), false);
			}
		}

		public override string ServerSettings
		{
			get
			{
				return base.ServerSettings + ",'bootType': 'AnonymousCalendar','disableCalendarDetails': '" + (AnonymousUserContext.Current.SharingDetail == DetailLevelEnumType.AvailabilityOnly).ToString().ToLower() + "'";
			}
		}

		public override string FormatURIForCDN(string relativeUri, bool isBootResourceUri)
		{
			return relativeUri;
		}

		public override string GetCdnEndpointForResources(bool bootResources)
		{
			return string.Empty;
		}

		public override SlabManifestType ManifestType
		{
			get
			{
				return SlabManifestType.Anonymous;
			}
		}

		public override string VersionString
		{
			get
			{
				if (this.buildVersion == null)
				{
					string value = OwaVersionId.Current;
					if (string.IsNullOrEmpty(value))
					{
						ExTraceGlobals.AppcacheManifestHandlerTracer.TraceError(0L, "DefaultPageHandler.VersionString: Could not retrieve OwaVersion from registry.");
						throw new ArgumentException("Could not retrieve OwaVersion from registry.");
					}
					this.buildVersion = value;
				}
				return this.buildVersion;
			}
			set
			{
				this.buildVersion = value;
			}
		}

		public string GetCalendarFolder()
		{
			CalendarGroup[] array = new CalendarGroup[]
			{
				new CalendarGroup()
			};
			array[0].GroupId = Guid.Empty.ToString();
			array[0].GroupType = CalendarGroupType.MyCalendars;
			array[0].GroupName = string.Empty;
			array[0].ItemId = new Microsoft.Exchange.Services.Core.Types.ItemId(array[0].GroupId, array[0].GroupId);
			array[0].Calendars = new CalendarEntry[1];
			LocalCalendarEntry localCalendarEntry = new LocalCalendarEntry();
			localCalendarEntry.CalendarColor = CalendarColor.Auto;
			localCalendarEntry.IsDefaultCalendar = true;
			array[0].Calendars[0] = localCalendarEntry;
			CalendarFolderType calendarFolderType = new CalendarFolderType();
			calendarFolderType.FolderId = IdConverter.GetFolderIdFromStoreId(AnonymousUserContext.Current.PublishedCalendarId, new MailboxId(AnonymousUserContext.Current.ExchangePrincipal.MailboxInfo.MailboxGuid));
			localCalendarEntry.ItemId = new Microsoft.Exchange.Services.Core.Types.ItemId("calendarEntryFor" + calendarFolderType.FolderId.Id, "calendarEntryChangeKeyFor" + calendarFolderType.FolderId.ChangeKey);
			calendarFolderType.DisplayName = AntiXssEncoder.HtmlEncode(AnonymousUserContext.Current.PublishedCalendarName, false);
			calendarFolderType.ChildFolderCount = new int?(0);
			calendarFolderType.ChildFolderCountSpecified = true;
			calendarFolderType.ExtendedProperty = null;
			calendarFolderType.FolderClass = "IPF.Appointment";
			calendarFolderType.EffectiveRights = new EffectiveRightsType();
			calendarFolderType.EffectiveRights.CreateAssociated = false;
			calendarFolderType.EffectiveRights.CreateContents = false;
			calendarFolderType.EffectiveRights.CreateHierarchy = false;
			calendarFolderType.EffectiveRights.Delete = false;
			calendarFolderType.EffectiveRights.Modify = false;
			calendarFolderType.EffectiveRights.Read = true;
			calendarFolderType.EffectiveRights.ViewPrivateItemsSpecified = false;
			localCalendarEntry.CalendarFolderId = calendarFolderType.FolderId;
			GetCalendarFoldersResponse instance = new GetCalendarFoldersResponse(array, new CalendarFolderType[]
			{
				calendarFolderType
			});
			return SessionDataHandler.EmitPayload("calendarFolders", JsonConverter.ToJSON(instance));
		}

		public string GetUserConfiguration()
		{
			OwaUserConfiguration owaUserConfiguration = new OwaUserConfiguration();
			owaUserConfiguration.UserOptions = new UserOptionsType();
			owaUserConfiguration.UserOptions.TimeZone = AnonymousUserContext.Current.TimeZone.AlternativeId;
			owaUserConfiguration.UserOptions.TimeFormat = MailboxRegionalConfiguration.GetDefaultTimeFormat(CultureInfo.CurrentUICulture);
			owaUserConfiguration.UserOptions.DateFormat = MailboxRegionalConfiguration.GetDefaultDateFormat(CultureInfo.CurrentUICulture);
			owaUserConfiguration.UserOptions.WorkingHours = new WorkingHoursType(0, 1440, 127, AnonymousUserContext.Current.TimeZone, AnonymousUserContext.Current.TimeZone);
			owaUserConfiguration.ApplicationSettings = new ApplicationSettingsType();
			owaUserConfiguration.ApplicationSettings.AnalyticsEnabled = false;
			owaUserConfiguration.ApplicationSettings.InferenceEnabled = false;
			owaUserConfiguration.ApplicationSettings.WatsonEnabled = false;
			owaUserConfiguration.ApplicationSettings.DefaultTraceLevel = TraceLevel.Off;
			owaUserConfiguration.ApplicationSettings.InstrumentationSendIntervalSeconds = 0;
			owaUserConfiguration.SessionSettings = new SessionSettingsType();
			owaUserConfiguration.SessionSettings.ArchiveDisplayName = string.Empty;
			owaUserConfiguration.SessionSettings.CanActAsOwner = false;
			owaUserConfiguration.SessionSettings.IsExplicitLogon = false;
			owaUserConfiguration.SessionSettings.IsExplicitLogonOthersMailbox = false;
			owaUserConfiguration.SessionSettings.UserCulture = AnonymousUserContext.Current.Culture.Name;
			owaUserConfiguration.SessionSettings.UserDisplayName = AntiXssEncoder.HtmlEncode(AnonymousUserContext.Current.ExchangePrincipal.MailboxInfo.DisplayName, false);
			owaUserConfiguration.SessionSettings.MaxMessageSizeInKb = 0;
			owaUserConfiguration.SessionSettings.DefaultFolderIds = new FolderId[1];
			owaUserConfiguration.SessionSettings.DefaultFolderIds[0] = IdConverter.GetFolderIdFromStoreId(AnonymousUserContext.Current.PublishedCalendarId, new MailboxId(AnonymousUserContext.Current.ExchangePrincipal.MailboxInfo.MailboxGuid));
			owaUserConfiguration.SessionSettings.DefaultFolderNames = new string[]
			{
				DefaultFolderType.Calendar.ToString()
			};
			owaUserConfiguration.PolicySettings = new PolicySettingsType();
			owaUserConfiguration.PolicySettings.PlacesEnabled = false;
			owaUserConfiguration.PolicySettings.WeatherEnabled = false;
			owaUserConfiguration.ViewStateConfiguration = new OwaViewStateConfiguration();
			owaUserConfiguration.ViewStateConfiguration.CalendarViewTypeDesktop = 4;
			owaUserConfiguration.ViewStateConfiguration.CalendarViewTypeNarrow = 1;
			owaUserConfiguration.ViewStateConfiguration.CalendarViewTypeWide = 4;
			owaUserConfiguration.SegmentationSettings = new SegmentationSettingsType(0UL);
			owaUserConfiguration.SegmentationSettings.Calendar = true;
			return SessionDataHandler.EmitPayload("owaUserConfig", JsonConverter.ToJSON(owaUserConfiguration));
		}

		protected override string GetFeaturesSupportedJsonArray(FlightedFeatureScope scope)
		{
			return "[]";
		}

		protected override bool ShouldSkipThemeFolder()
		{
			return ThemeManagerFactory.GetInstance(this.VersionString).ShouldSkipThemeFolder;
		}

		protected override void OnPreInit(EventArgs e)
		{
			if (!Globals.IsAnonymousCalendarApp)
			{
				HttpUtilities.EndResponse(this.Context, HttpStatusCode.BadRequest);
			}
			base.OnPreInit(e);
		}

		protected override bool GetIsClientAppCacheEnabled(HttpContext context)
		{
			return false;
		}

		protected const string CalendarFolderPayLoadName = "calendarFolders";

		private const int AnonymousDefaultWorkDayStartTime = 0;

		private const int AnonymousDefaultWorkDayEndTime = 1440;

		private const int AnonymousDefaultWorkDays = 127;

		private string buildVersion;
	}
}
