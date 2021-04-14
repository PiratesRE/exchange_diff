using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Net.MonitoringWebClient.Owa.Parsers;

namespace Microsoft.Exchange.Net.MonitoringWebClient.Owa
{
	internal class OwaExceptionAnalyzer : BaseExceptionAnalyzer
	{
		static OwaExceptionAnalyzer()
		{
			Dictionary<FailureReason, FailingComponent> dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.MissingKeyword, FailingComponent.Owa);
			dictionary.Add(FailureReason.NameResolution, FailingComponent.Networking);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.Networking);
			dictionary.Add(FailureReason.OwaActiveDirectoryErrorPage, FailingComponent.ActiveDirectory);
			dictionary.Add(FailureReason.OwaErrorPage, FailingComponent.Owa);
			dictionary.Add(FailureReason.OwaMServErrorPage, FailingComponent.MServ);
			dictionary.Add(FailureReason.OwaMailboxErrorPage, FailingComponent.Mailbox);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.OwaUnknownRootCause);
			dictionary.Add(FailureReason.ConnectionTimeout, FailingComponent.Networking);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.Owa);
			dictionary.Add(FailureReason.ScenarioTimeout, FailingComponent.OwaUnknownRootCause);
			dictionary.Add(FailureReason.PassiveDatabase, FailingComponent.Mailbox);
			dictionary.Add(FailureReason.OwaFindPlacesError, FailingComponent.Places);
			dictionary.Add(FailureReason.SslNegotiation, FailingComponent.Networking);
			dictionary.Add(FailureReason.BrokenAffinity, FailingComponent.Networking);
			dictionary.Add(FailureReason.CafeFailure, FailingComponent.Owa);
			dictionary.Add(FailureReason.CafeActiveDirectoryFailure, FailingComponent.ActiveDirectory);
			dictionary.Add(FailureReason.CafeHighAvailabilityFailure, FailingComponent.Mailbox);
			dictionary.Add(FailureReason.CafeToMailboxNetworkingFailure, FailingComponent.Networking);
			dictionary.Add(FailureReason.CafeTimeoutContactingBackend, FailingComponent.OwaUnknownRootCause);
			OwaExceptionAnalyzer.FailureMatrix.Add(RequestTarget.Owa, dictionary);
			dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.MissingKeyword, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.NameResolution, FailingComponent.Networking);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.ConnectionTimeout, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.ScenarioTimeout, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.SslNegotiation, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.BadUserNameOrPassword, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.AccountLocked, FailingComponent.LiveIdConsumer);
			dictionary.Add(FailureReason.LogonError, FailingComponent.LiveIdConsumer);
			OwaExceptionAnalyzer.FailureMatrix.Add(RequestTarget.LiveIdConsumer, dictionary);
			dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.MissingKeyword, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.NameResolution, FailingComponent.Networking);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.ConnectionTimeout, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.ScenarioTimeout, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.SslNegotiation, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.BadUserNameOrPassword, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.AccountLocked, FailingComponent.LiveIdBusiness);
			dictionary.Add(FailureReason.LogonError, FailingComponent.LiveIdBusiness);
			OwaExceptionAnalyzer.FailureMatrix.Add(RequestTarget.LiveIdBusiness, dictionary);
			dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.MissingKeyword, FailingComponent.Akamai);
			dictionary.Add(FailureReason.NameResolution, FailingComponent.Akamai);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.Akamai);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.Akamai);
			dictionary.Add(FailureReason.ConnectionTimeout, FailingComponent.Akamai);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.Akamai);
			dictionary.Add(FailureReason.ScenarioTimeout, FailingComponent.Akamai);
			dictionary.Add(FailureReason.SslNegotiation, FailingComponent.Akamai);
			OwaExceptionAnalyzer.FailureMatrix.Add(RequestTarget.Akamai, dictionary);
			dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.MissingKeyword, FailingComponent.Hotmail);
			dictionary.Add(FailureReason.NameResolution, FailingComponent.Hotmail);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.Hotmail);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.Hotmail);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.Hotmail);
			dictionary.Add(FailureReason.ScenarioTimeout, FailingComponent.Hotmail);
			dictionary.Add(FailureReason.SslNegotiation, FailingComponent.Hotmail);
			OwaExceptionAnalyzer.FailureMatrix.Add(RequestTarget.Hotmail, dictionary);
			dictionary = new Dictionary<FailureReason, FailingComponent>();
			dictionary.Add(FailureReason.MissingKeyword, FailingComponent.Adfs);
			dictionary.Add(FailureReason.NameResolution, FailingComponent.Adfs);
			dictionary.Add(FailureReason.NetworkConnection, FailingComponent.Adfs);
			dictionary.Add(FailureReason.RequestTimeout, FailingComponent.Adfs);
			dictionary.Add(FailureReason.UnexpectedHttpResponseCode, FailingComponent.Adfs);
			dictionary.Add(FailureReason.ScenarioTimeout, FailingComponent.Adfs);
			dictionary.Add(FailureReason.SslNegotiation, FailingComponent.Adfs);
			OwaExceptionAnalyzer.FailureMatrix.Add(RequestTarget.Adfs, dictionary);
		}

		public OwaExceptionAnalyzer(Dictionary<string, RequestTarget> hostNameSourceMapping) : base(hostNameSourceMapping)
		{
		}

		public override HttpWebResponseWrapperException VerifyResponse(HttpWebRequestWrapper request, HttpWebResponseWrapper response, CafeErrorPageValidationRules cafeErrorPageValidationRules)
		{
			OwaErrorPage owaErrorPage;
			if (OwaErrorPage.TryParse(response, out owaErrorPage))
			{
				return new OwaErrorPageException(MonitoringWebClientStrings.OwaErrorPage, request, response, owaErrorPage);
			}
			HttpWebResponseWrapperException ex = base.VerifyResponse(request, response, cafeErrorPageValidationRules);
			if (ex != null)
			{
				return ex;
			}
			return null;
		}

		protected override FailureReason GetFailureReason(Exception exception, HttpWebRequestWrapper request, HttpWebResponseWrapper response)
		{
			if (this.IsFindPlacesFailure(request))
			{
				return FailureReason.OwaFindPlacesError;
			}
			if (exception is OwaErrorPageException)
			{
				return (exception as OwaErrorPageException).OwaErrorPage.FailureReason;
			}
			return base.GetFailureReason(exception, request, response);
		}

		protected override Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>> ComponentMatrix
		{
			get
			{
				return OwaExceptionAnalyzer.FailureMatrix;
			}
		}

		protected override FailingComponent DefaultComponent
		{
			get
			{
				return FailingComponent.Owa;
			}
		}

		protected override FailingComponent GetFailingComponentForScenarioTimeout(RequestTarget failureSource, FailureReason failureReason, ResponseTrackerItem item)
		{
			FailingComponent failingComponent = base.GetFailingComponent(failureSource, failureReason);
			if (failingComponent != FailingComponent.Owa)
			{
				return failingComponent;
			}
			TimeSpan t = item.CasLatency ?? TimeSpan.FromTicks(0L);
			TimeSpan timeSpan = item.RpcLatency ?? TimeSpan.FromTicks(0L);
			TimeSpan timeSpan2 = item.LdapLatency ?? TimeSpan.FromTicks(0L);
			TimeSpan latency = item.MservLatency ?? TimeSpan.FromTicks(0L);
			if (t.Ticks <= 0L)
			{
				t = item.ResponseLatency;
			}
			TimeSpan latency2 = t - timeSpan - timeSpan2;
			var array = new <>f__AnonymousType3<FailingComponent, string, TimeSpan>[]
			{
				new
				{
					Component = FailingComponent.Owa,
					Server = item.RespondingServer,
					Latency = latency2
				},
				new
				{
					Component = FailingComponent.Mailbox,
					Server = item.MailboxServer,
					Latency = timeSpan
				},
				new
				{
					Component = FailingComponent.ActiveDirectory,
					Server = item.DomainController,
					Latency = timeSpan2
				},
				new
				{
					Component = FailingComponent.MServ,
					Server = item.RespondingServer,
					Latency = latency
				}
			};
			var <>f__AnonymousType = array[0];
			var array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				var <>f__AnonymousType2 = array2[i];
				if (<>f__AnonymousType2.Latency > <>f__AnonymousType.Latency)
				{
					<>f__AnonymousType = <>f__AnonymousType2;
				}
			}
			item.FailingServer = <>f__AnonymousType.Server;
			return <>f__AnonymousType.Component;
		}

		private bool IsFindPlacesFailure(HttpWebRequestWrapper request)
		{
			if (request == null)
			{
				return false;
			}
			WebHeaderCollection headers = request.Headers;
			string text = headers.Get("action");
			return text != null && string.Equals(text, "FindPlaces", StringComparison.OrdinalIgnoreCase);
		}

		private static readonly Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>> FailureMatrix = new Dictionary<RequestTarget, Dictionary<FailureReason, FailingComponent>>();
	}
}
