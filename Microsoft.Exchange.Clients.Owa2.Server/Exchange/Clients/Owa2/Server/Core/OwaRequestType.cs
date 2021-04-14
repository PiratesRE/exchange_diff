using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum OwaRequestType
	{
		Invalid,
		Authorize,
		Logoff,
		Aspx,
		EsoRequest,
		Form15,
		Oeh,
		ProxyLogon,
		ProxyPing,
		LanguagePage,
		LanguagePost,
		Attachment,
		WebPart,
		KeepAlive,
		Resource,
		PublishedCalendarView,
		ICalHttpHandler,
		HealthPing,
		ServiceRequest,
		SpeechReco,
		WopiRequest,
		WebReadyRequest,
		RemoteNotificationRequest,
		GroupSubscriptionRequest,
		SuiteServiceProxyPage
	}
}
