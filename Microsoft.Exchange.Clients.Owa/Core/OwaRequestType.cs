using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public enum OwaRequestType
	{
		Invalid,
		Authorize,
		Logoff,
		Aspx,
		Form14,
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
		ProxyToEwsEventHandler,
		PublishedCalendarView,
		ICalHttpHandler,
		HealthPing,
		ServiceRequest,
		WebReadyRequest,
		SuiteServiceProxyPage
	}
}
