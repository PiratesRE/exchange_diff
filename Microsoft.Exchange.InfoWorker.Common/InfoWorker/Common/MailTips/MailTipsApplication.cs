using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.MailTips;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	internal sealed class MailTipsApplication : Application
	{
		public MailTipsApplication(int traceId, ProxyAddress sendingAs, MailTipTypes mailTipTypes, IBudget callerBudget) : base(false)
		{
			this.traceId = traceId;
			this.sendingAs = sendingAs;
			this.mailTipTypes = mailTipTypes;
			this.callerBudget = callerBudget;
		}

		public override IService CreateService(WebServiceUri webServiceUri, TargetServerVersion targetVersion, RequestType requestType)
		{
			Service service = new Service(webServiceUri);
			service.RequestServerVersionValue = new RequestServerVersion();
			if (targetVersion >= TargetServerVersion.E15)
			{
				service.RequestServerVersionValue.Version = ExchangeVersionType.Exchange2012;
			}
			else
			{
				this.mailTipTypes &= ~MailTipTypes.Scope;
				service.RequestServerVersionValue.Version = ExchangeVersionType.Exchange2010;
			}
			return service;
		}

		public override IAsyncResult BeginProxyWebRequest(IService service, MailboxData[] mailboxArray, AsyncCallback callback, object asyncState)
		{
			MailTipsApplication.GetMailTipsTracer.TraceFunction((long)this.traceId, "Entering MailTipsApplication.BeginProxyWebRequest");
			if (Testability.WebServiceCredentials != null)
			{
				service.Credentials = Testability.WebServiceCredentials;
				ServicePointManager.ServerCertificateValidationCallback = ((object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true);
			}
			GetMailTipsType getMailTipsType = new GetMailTipsType();
			getMailTipsType.SendingAs = new EmailAddressType();
			getMailTipsType.SendingAs.EmailAddress = this.sendingAs.AddressString;
			getMailTipsType.SendingAs.RoutingType = this.sendingAs.Prefix.PrimaryPrefix;
			getMailTipsType.Recipients = new EmailAddressType[mailboxArray.Length];
			for (int i = 0; i < mailboxArray.Length; i++)
			{
				MailboxData mailboxData = mailboxArray[i];
				getMailTipsType.Recipients[i] = new EmailAddressType();
				getMailTipsType.Recipients[i].EmailAddress = mailboxData.Email.Address;
				getMailTipsType.Recipients[i].RoutingType = mailboxData.Email.RoutingType;
			}
			getMailTipsType.MailTipsRequested = (MailTipTypes)this.mailTipTypes;
			return service.BeginGetMailTips(getMailTipsType, callback, asyncState);
		}

		public override void EndProxyWebRequest(ProxyWebRequest proxyWebRequest, QueryList queryList, IService service, IAsyncResult asyncResult)
		{
			MailTipsApplication.GetMailTipsTracer.TraceFunction((long)this.traceId, "Entering MailTipsApplication.EndProxyWebRequest");
			GetMailTipsResponseMessageType getMailTipsResponseMessageType = service.EndGetMailTips(asyncResult);
			int hashCode = proxyWebRequest.GetHashCode();
			if (getMailTipsResponseMessageType == null)
			{
				Application.ProxyWebRequestTracer.TraceError((long)this.traceId, "{0}: Proxy web request returned NULL GetMailTipsResponseMessageType", new object[]
				{
					TraceContext.Get()
				});
				queryList.SetResultInAllQueries(new MailTipsQueryResult(new NoEwsResponseException()));
				base.HandleNullResponse(proxyWebRequest);
				return;
			}
			ResponseCodeType responseCode = getMailTipsResponseMessageType.ResponseCode;
			if (responseCode != ResponseCodeType.NoError)
			{
				Application.ProxyWebRequestTracer.TraceError<object, string>((long)hashCode, "{0}: Proxy web request returned error code {1}", TraceContext.Get(), responseCode.ToString());
				queryList.SetResultInAllQueries(new MailTipsQueryResult(new ErrorEwsResponseException(responseCode)));
				return;
			}
			this.ProcessResponseMessages(hashCode, queryList, getMailTipsResponseMessageType);
		}

		public override string GetParameterDataString()
		{
			return this.traceId.ToString() + " " + this.sendingAs.ToString();
		}

		public override LocalQuery CreateLocalQuery(ClientContext clientContext, DateTime requestCompletionDeadline)
		{
			return new MailTipsLocalQuery(clientContext, requestCompletionDeadline, this.callerBudget);
		}

		public override BaseQueryResult CreateQueryResult(LocalizedException exception)
		{
			return new MailTipsQueryResult(exception);
		}

		public override BaseQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			return MailTipsQuery.CreateFromUnknown(recipientData, exception);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData)
		{
			return MailTipsQuery.CreateFromIndividual(recipientData);
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			return MailTipsQuery.CreateFromIndividual(recipientData, exception);
		}

		public override AvailabilityException CreateExceptionForUnsupportedVersion(RecipientData recipient, int serverVersion)
		{
			return new E14orHigherProxyServerNotFound((recipient != null) ? recipient.EmailAddress : null, Globals.E14Version);
		}

		public override BaseQuery CreateFromGroup(RecipientData recipientData, BaseQuery[] groupMembers, bool groupCapped)
		{
			throw new NotSupportedException();
		}

		public override bool EnabledInRelationship(OrganizationRelationship organizationRelationship)
		{
			return organizationRelationship.MailTipsAccessEnabled;
		}

		public override Offer OfferForExternalSharing
		{
			get
			{
				return Offer.MailTips;
			}
		}

		public override ThreadCounter Worker
		{
			get
			{
				return MailTipsApplication.MailTipsWorker;
			}
		}

		public override ThreadCounter IOCompletion
		{
			get
			{
				return MailTipsApplication.MailTipsIOCompletion;
			}
		}

		public override int MinimumRequiredVersion
		{
			get
			{
				return Globals.E14Version;
			}
		}

		public override int MinimumRequiredVersionForExternalUser
		{
			get
			{
				return Globals.E14SP1Version;
			}
		}

		public override LocalizedString Name
		{
			get
			{
				return Strings.MailtipsApplicationName;
			}
		}

		private void ProcessResponseMessages(int traceId, QueryList queryList, GetMailTipsResponseMessageType response)
		{
			if (response.ResponseMessages == null)
			{
				Application.ProxyWebRequestTracer.TraceError((long)traceId, "{0}: Proxy web request returned NULL GetMailTipsResponseMessageType.ResponseMessages", new object[]
				{
					TraceContext.Get()
				});
				queryList.SetResultInAllQueries(new MailTipsQueryResult(new NoEwsResponseException()));
				return;
			}
			for (int i = 0; i < response.ResponseMessages.Length; i++)
			{
				MailTipsResponseMessageType mailTipsResponseMessageType = response.ResponseMessages[i];
				BaseQuery[] array = queryList.FindByEmailAddress(queryList[i].Email.Address);
				foreach (MailTipsQuery mailTipsQuery in array)
				{
					if (mailTipsResponseMessageType == null)
					{
						Application.ProxyWebRequestTracer.TraceError<object, Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress>((long)traceId, "{0}: Proxy web request returned NULL MailTipsResponseMessageType for mailbox {1}.", TraceContext.Get(), queryList[i].Email);
						mailTipsQuery.SetResultOnFirstCall(new MailTipsQueryResult(new NoEwsResponseException()));
					}
					else if (mailTipsResponseMessageType.ResponseCode != ResponseCodeType.NoError)
					{
						Application.ProxyWebRequestTracer.TraceError<object, Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress, ResponseCodeType>((long)traceId, "{0}: Proxy web request returned error MailTipsResponseMessageType for mailbox {1}. Error coee is {2}.", TraceContext.Get(), queryList[i].Email, mailTipsResponseMessageType.ResponseCode);
						mailTipsQuery.SetResultOnFirstCall(new MailTipsQueryResult(new ErrorEwsResponseException(mailTipsResponseMessageType.ResponseCode)));
					}
					else
					{
						MailTips mailTips = mailTipsResponseMessageType.MailTips;
						if (mailTips == null)
						{
							Application.ProxyWebRequestTracer.TraceDebug<object, Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress>((long)traceId, "{0}: Proxy web request returned NULL MailTips for mailbox {1}.", TraceContext.Get(), queryList[i].Email);
							mailTipsQuery.SetResultOnFirstCall(new MailTipsQueryResult(new NoMailTipsInEwsResponseMessageException()));
						}
						else
						{
							MailTips mailTips2 = MailTipsApplication.ParseWebServiceMailTips(mailTips);
							MailTipsQueryResult resultOnFirstCall = new MailTipsQueryResult(mailTips2);
							mailTipsQuery.SetResultOnFirstCall(resultOnFirstCall);
						}
					}
				}
			}
		}

		private static MailTips ParseWebServiceMailTips(MailTips webServiceMailTips)
		{
			Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress emailAddress = new Microsoft.Exchange.InfoWorker.Common.Availability.EmailAddress(string.Empty, webServiceMailTips.RecipientAddress.EmailAddress, webServiceMailTips.RecipientAddress.RoutingType);
			MailTips mailTips = new MailTips(emailAddress);
			if (webServiceMailTips.CustomMailTip != null)
			{
				mailTips.CustomMailTip = webServiceMailTips.CustomMailTip;
			}
			if (webServiceMailTips.DeliveryRestrictedSpecified)
			{
				mailTips.DeliveryRestricted = webServiceMailTips.DeliveryRestricted;
			}
			if (webServiceMailTips.ExternalMemberCountSpecified)
			{
				mailTips.ExternalMemberCount = webServiceMailTips.ExternalMemberCount;
			}
			if (webServiceMailTips.InvalidRecipientSpecified)
			{
				mailTips.InvalidRecipient = webServiceMailTips.InvalidRecipient;
			}
			if (webServiceMailTips.ScopeSpecified)
			{
				mailTips.Scope = (ScopeTypes)webServiceMailTips.Scope;
			}
			if (webServiceMailTips.IsModeratedSpecified)
			{
				mailTips.IsModerated = webServiceMailTips.IsModerated;
			}
			if (webServiceMailTips.MailboxFullSpecified)
			{
				mailTips.MailboxFull = webServiceMailTips.MailboxFull;
			}
			if (webServiceMailTips.MaxMessageSizeSpecified)
			{
				mailTips.MaxMessageSize = webServiceMailTips.MaxMessageSize;
			}
			if (webServiceMailTips.OutOfOffice != null && webServiceMailTips.OutOfOffice.ReplyBody != null)
			{
				mailTips.OutOfOfficeMessage = webServiceMailTips.OutOfOffice.ReplyBody.Message;
				mailTips.OutOfOfficeMessageLanguage = webServiceMailTips.OutOfOffice.ReplyBody.Lang;
				mailTips.OutOfOfficeDuration = webServiceMailTips.OutOfOffice.Duration;
			}
			if (webServiceMailTips.MailboxFullSpecified)
			{
				mailTips.MailboxFull = webServiceMailTips.MailboxFull;
			}
			if (webServiceMailTips.TotalMemberCountSpecified)
			{
				mailTips.TotalMemberCount = webServiceMailTips.TotalMemberCount;
			}
			return mailTips;
		}

		private static readonly Trace GetMailTipsTracer = ExTraceGlobals.GetMailTipsTracer;

		private int traceId;

		private ProxyAddress sendingAs;

		private MailTipTypes mailTipTypes;

		private IBudget callerBudget;

		public static readonly ThreadCounter MailTipsWorker = new ThreadCounter();

		public static readonly ThreadCounter MailTipsIOCompletion = new ThreadCounter();
	}
}
