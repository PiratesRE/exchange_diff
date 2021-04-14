using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class AnonymousUserContext
	{
		private AnonymousUserContext(HttpContext context)
		{
			this.PublishingUrl = (PublishingUrl)context.Items["AnonymousUserContextPublishedUrl"];
			ExAssert.RetailAssert(this.PublishingUrl != null, "Missing Published Url");
			this.ExchangePrincipal = (ExchangePrincipal)context.Items["AnonymousUserContextExchangePrincipalKey"];
			ExAssert.RetailAssert(this.ExchangePrincipal != null, "Missing Exchange Principal");
			this.TimeZone = (ExTimeZone)context.Items["AnonymousUserContextTimeZoneKey"];
			ExAssert.RetailAssert(this.TimeZone != null, "Missing Timezone");
			this.PublishedCalendarName = (string)context.Items["AnonymousUserContextPublishedCalendarNameKey"];
			ExAssert.RetailAssert(this.PublishedCalendarName != null, "Missing Published Calendar Name");
			this.SharingDetail = (DetailLevelEnumType)context.Items["AnonymousUserContextSharingDetailsKey"];
			ExAssert.RetailAssert(this.SharingDetail.ToString().Length > 0, "Missing SharingDetail");
			this.PublishedCalendarId = (StoreObjectId)context.Items["AnonymousUserContextPublishedCalendarIdKey"];
			ExAssert.RetailAssert(this.PublishedCalendarId != null, "Missing PublishedCalendarId");
			this.UserAgent = OwaUserAgentUtilities.CreateUserAgentAnonymous(context);
			ExAssert.RetailAssert(this.PublishedCalendarId != null, "Missing UserAgent");
		}

		internal static AnonymousUserContext Current
		{
			get
			{
				AnonymousUserContext anonymousUserContext = (AnonymousUserContext)HttpContext.Current.Items["AnonymousUserContext"];
				if (anonymousUserContext != null)
				{
					return anonymousUserContext;
				}
				lock (AnonymousUserContext.syncObject)
				{
					anonymousUserContext = (((AnonymousUserContext)HttpContext.Current.Items["AnonymousUserContext"]) ?? new AnonymousUserContext(HttpContext.Current));
					HttpContext.Current.Items["AnonymousUserContext"] = anonymousUserContext;
				}
				return anonymousUserContext;
			}
		}

		internal PublishingUrl PublishingUrl { get; private set; }

		internal StoreObjectId PublishedCalendarId { get; private set; }

		internal DetailLevelEnumType SharingDetail { get; private set; }

		internal string PublishedCalendarName { get; private set; }

		internal ExchangePrincipal ExchangePrincipal { get; private set; }

		internal ExTimeZone TimeZone { get; private set; }

		internal UserAgent UserAgent { get; private set; }

		internal CultureInfo Culture
		{
			get
			{
				return Thread.CurrentThread.CurrentCulture;
			}
		}

		private const string AnonymousUserContextKey = "AnonymousUserContext";

		private const string AnonymousUserContextExchangePrincipalKey = "AnonymousUserContextExchangePrincipalKey";

		private const string AnonymousUserContextSharingDetailsKey = "AnonymousUserContextSharingDetailsKey";

		private const string AnonymousUserContextPublishedUrl = "AnonymousUserContextPublishedUrl";

		private const string AnonymousUserContextTimeZoneKey = "AnonymousUserContextTimeZoneKey";

		private const string AnonymousUserContextPublishedCalendarNameKey = "AnonymousUserContextPublishedCalendarNameKey";

		private const string AnonymousUserContextPublishedCalendarIdKey = "AnonymousUserContextPublishedCalendarIdKey";

		private static object syncObject = new object();
	}
}
