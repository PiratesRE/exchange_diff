using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Services.Common;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class FreeBusyApplication : Application
	{
		public FreeBusyApplication(ClientContext clientContext, FreeBusyViewOptions freeBusyView, bool defaultFreeBusyOnly, QueryType queryType) : base(true)
		{
			this.freeBusyView = freeBusyView;
			this.clientContext = clientContext;
			this.defaultFreeBusyOnly = defaultFreeBusyOnly;
			this.queryType = queryType;
		}

		public static FreeBusyQuery[] ConvertBaseToFreeBusyQuery(BaseQuery[] baseQueries)
		{
			FreeBusyQuery[] array = new FreeBusyQuery[baseQueries.Length];
			for (int i = 0; i < baseQueries.Length; i++)
			{
				array[i] = (FreeBusyQuery)baseQueries[i];
			}
			return array;
		}

		public override IService CreateService(WebServiceUri webServiceUri, TargetServerVersion targetVersion, RequestType requestType)
		{
			Service service = new Service(webServiceUri);
			if (targetVersion >= TargetServerVersion.E14R3OrLater || targetVersion == TargetServerVersion.Unknown)
			{
				this.SetTimeZoneDefinitionHeader(service);
			}
			return service;
		}

		public override IAsyncResult BeginProxyWebRequest(IService service, MailboxData[] mailboxArray, AsyncCallback callback, object asyncState)
		{
			return service.BeginGetUserAvailability(new GetUserAvailabilityRequest
			{
				MailboxDataArray = mailboxArray,
				FreeBusyViewOptions = this.freeBusyView,
				TimeZone = new SerializableTimeZone(this.clientContext.TimeZone)
			}, callback, null);
		}

		public override void EndProxyWebRequest(ProxyWebRequest proxyWebRequest, QueryList queryList, IService service, IAsyncResult asyncResult)
		{
			GetUserAvailabilityResponse getUserAvailabilityResponse = service.EndGetUserAvailability(asyncResult);
			FreeBusyResponse[] array = null;
			if (getUserAvailabilityResponse != null)
			{
				array = getUserAvailabilityResponse.FreeBusyResponseArray;
			}
			if (array == null)
			{
				Application.ProxyWebRequestTracer.TraceError((long)proxyWebRequest.GetHashCode(), "{0}: Proxy web request returned NULL FreeBusyResponseArray.", new object[]
				{
					TraceContext.Get()
				});
			}
			for (int i = 0; i < queryList.Count; i++)
			{
				FreeBusyQuery freeBusyQuery = (FreeBusyQuery)queryList[i];
				FreeBusyResponse freeBusyResponse = null;
				if (array != null && i < array.Length)
				{
					freeBusyResponse = array[i];
					if (freeBusyResponse == null)
					{
						Application.ProxyWebRequestTracer.TraceDebug<object, EmailAddress>((long)proxyWebRequest.GetHashCode(), "{0}: Proxy web request returned NULL FreeBusyResponse for mailbox {1}.", TraceContext.Get(), freeBusyQuery.Email);
					}
				}
				FreeBusyQueryResult resultOnFirstCall;
				if (freeBusyResponse == null)
				{
					resultOnFirstCall = new FreeBusyQueryResult(new ProxyNoResultException(Strings.descProxyNoResultError(freeBusyQuery.Email.Address, service.Url), 60732U));
				}
				else
				{
					resultOnFirstCall = FreeBusyApplication.CopyViewAndResponseToResult(service.Url, freeBusyResponse.FreeBusyView, freeBusyResponse.ResponseMessage, freeBusyQuery.Email);
				}
				freeBusyQuery.SetResultOnFirstCall(resultOnFirstCall);
			}
		}

		public override string GetParameterDataString()
		{
			return string.Format("Parameters: windowStart = {0}, windowEnd = {1}, MergedFBInterval = {2}, RequestedView = {3}", new object[]
			{
				this.freeBusyView.TimeWindow.StartTime,
				this.freeBusyView.TimeWindow.EndTime,
				this.freeBusyView.MergedFreeBusyIntervalInMinutes,
				this.freeBusyView.RequestedView
			});
		}

		public override LocalQuery CreateLocalQuery(ClientContext clientContext, DateTime requestCompletionDeadline)
		{
			return new CalendarQuery(clientContext, this.freeBusyView, this.defaultFreeBusyOnly, requestCompletionDeadline);
		}

		public override BaseQueryResult CreateQueryResult(LocalizedException exception)
		{
			return new FreeBusyQueryResult(exception);
		}

		public override BaseQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			return FreeBusyQuery.CreateFromUnknown(recipientData, exception);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData)
		{
			return FreeBusyQuery.CreateFromIndividual(recipientData);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			return FreeBusyQuery.CreateFromIndividual(recipientData, exception);
		}

		public override AvailabilityException CreateExceptionForUnsupportedVersion(RecipientData recipient, int serverVersion)
		{
			return new E14orHigherProxyServerNotFound((recipient != null) ? recipient.EmailAddress : null, Globals.E14Version);
		}

		public override BaseQuery CreateFromGroup(RecipientData recipientData, BaseQuery[] groupMembers, bool groupCapped)
		{
			bool flag = (this.queryType & QueryType.FreeBusy) != (QueryType)0;
			bool flag2 = (this.queryType & QueryType.MeetingSuggestions) != (QueryType)0 && !groupCapped;
			FreeBusyQuery[] groupMembersForFreeBusy = null;
			FreeBusyQuery[] groupMembersForSuggestions = null;
			if (flag)
			{
				if (groupCapped)
				{
					Application.ProxyWebRequestTracer.TraceDebug<object, RecipientData>((long)this.GetHashCode(), "{0}:Not generating requests for members of group {1} because the group is larger than the allowed group size.", TraceContext.Get(), recipientData);
					return FreeBusyQuery.CreateFromGroup(recipientData, new FreeBusyDLLimitReachedException(Configuration.MaximumGroupMemberCount));
				}
				groupMembersForFreeBusy = FreeBusyApplication.ConvertBaseToFreeBusyQuery(groupMembers);
			}
			if (flag2)
			{
				groupMembersForSuggestions = FreeBusyApplication.ConvertBaseToFreeBusyQuery(groupMembers);
			}
			return FreeBusyQuery.CreateFromGroup(recipientData, groupMembersForFreeBusy, groupMembersForSuggestions);
		}

		public override AsyncRequestWithQueryList CreateCrossForestAsyncRequestWithAutoDiscover(ClientContext clientContext, RequestLogger requestLogger, QueryList queryList, TargetForestConfiguration targetForestConfiguration)
		{
			return new ProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, targetForestConfiguration, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestXmlByUser.Create));
		}

		public override AsyncRequestWithQueryList CreateExternalAsyncRequestWithAutoDiscover(InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, ExternalAuthenticationRequest autoDiscoverExternalAuthenticationRequest, ExternalAuthenticationRequest webProxyExternalAuthenticationRequest, Uri autoDiscoverUrl, SmtpAddress sharingKey)
		{
			return new ExternalProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, autoDiscoverExternalAuthenticationRequest, webProxyExternalAuthenticationRequest, autoDiscoverUrl, sharingKey, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestByUser.Create));
		}

		public override AsyncRequestWithQueryList CreateExternalByOAuthAsyncRequestWithAutoDiscover(InternalClientContext clientContext, RequestLogger requestLogger, QueryList queryList, Uri autoDiscoverUrl)
		{
			return new ExternalByOAuthProxyWebRequestWithAutoDiscover(this, clientContext, requestLogger, queryList, autoDiscoverUrl, new CreateAutoDiscoverRequestDelegate(AutoDiscoverRequestByUser.Create));
		}

		public override bool EnabledInRelationship(OrganizationRelationship organizationRelationship)
		{
			return organizationRelationship.FreeBusyAccessEnabled;
		}

		public override Offer OfferForExternalSharing
		{
			get
			{
				return Offer.SharingCalendarFreeBusy;
			}
		}

		public override ThreadCounter Worker
		{
			get
			{
				return FreeBusyApplication.FreeBusyWorker;
			}
		}

		public override ThreadCounter IOCompletion
		{
			get
			{
				return FreeBusyApplication.FreeBusyIOCompletion;
			}
		}

		public override int MinimumRequiredVersion
		{
			get
			{
				return 0;
			}
		}

		public override LocalizedString Name
		{
			get
			{
				return Strings.FreeBusyApplicationName;
			}
		}

		private static FreeBusyQueryResult CopyViewAndResponseToResult(string source, FreeBusyView view, ResponseMessage responseMessage, EmailAddress emailAddress)
		{
			if (responseMessage == null || view == null)
			{
				Application.ProxyWebRequestTracer.TraceError<object, string, EmailAddress>(0L, "{0}: Proxy web request failed to return a {1} for mailbox {2}.", TraceContext.Get(), (responseMessage == null) ? "response message" : "view", emailAddress);
				return new FreeBusyQueryResult(new ProxyNoResultException(Strings.descProxyNoResultError(emailAddress.Address, source), 36156U));
			}
			if (responseMessage.ResponseClass != ResponseClassType.Success)
			{
				int errorCode = 0;
				string str = string.Empty;
				string serverName = string.Empty;
				string str2 = string.Empty;
				if (responseMessage.MessageXml != null && responseMessage.MessageXml.HasChildNodes)
				{
					foreach (object obj in responseMessage.MessageXml.ChildNodes)
					{
						XmlNode xmlNode = (XmlNode)obj;
						if (xmlNode.Name == "ExceptionCode")
						{
							errorCode = int.Parse(xmlNode.InnerText);
						}
						else if (xmlNode.Name == "ExceptionMessage")
						{
							str = xmlNode.InnerText;
						}
						else if (xmlNode.Name == "ExceptionServerName")
						{
							serverName = xmlNode.InnerText;
						}
						else if (xmlNode.Name == "ExceptionType")
						{
							str2 = xmlNode.InnerText;
						}
					}
				}
				return new FreeBusyQueryResult(new ProxyQueryFailureException(serverName, new LocalizedString(str2 + ":" + str), (ErrorConstants)errorCode, responseMessage, source));
			}
			return new FreeBusyQueryResult(view.FreeBusyViewType, view.CalendarEventArray, view.MergedFreeBusy, view.WorkingHours);
		}

		private void SetTimeZoneDefinitionHeader(Service service)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("TimeZoneDefinitionType", "http://schemas.microsoft.com/exchange/services/2006/types");
			TimeZoneDefinition timeZoneDefinition = new TimeZoneDefinition(this.clientContext.TimeZone);
			timeZoneDefinition.Render(xmlElement, "ext", "http://schemas.microsoft.com/exchange/services/2006/types", "TimeZoneDefinition", true, this.clientContext.ClientCulture ?? CultureInfo.InvariantCulture);
			service.TimeZoneDefinitionContextValue = new TimeZoneContext();
			service.TimeZoneDefinitionContextValue.TimeZoneDefinitionValue = (xmlElement.FirstChild as XmlElement);
		}

		private const string ErrorsNamespacePrefix = "Errors";

		private const string TimeZoneDefinitionType = "TimeZoneDefinitionType";

		private const string ProxyTypePrefix = "ext";

		private const string TimeZoneDefinitionElement = "TimeZoneDefinition";

		public static readonly ThreadCounter FreeBusyWorker = new ThreadCounter();

		public static readonly ThreadCounter FreeBusyIOCompletion = new ThreadCounter();

		private ClientContext clientContext;

		private FreeBusyViewOptions freeBusyView;

		private bool defaultFreeBusyOnly;

		private QueryType queryType;
	}
}
