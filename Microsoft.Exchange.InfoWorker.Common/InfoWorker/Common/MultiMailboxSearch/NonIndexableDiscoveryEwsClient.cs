using System;
using System.Net.Security;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal class NonIndexableDiscoveryEwsClient : INonIndexableDiscoveryEwsClient
	{
		public NonIndexableDiscoveryEwsClient(GroupId groupId, MailboxInfo[] mailboxes, ExTimeZone timeZone, CallerInfo caller)
		{
			Util.ThrowOnNull(groupId, "groupId");
			Util.ThrowOnNull(mailboxes, "mailboxes");
			Util.ThrowOnNull(timeZone, "timeZone");
			Util.ThrowOnNull(caller, "caller");
			this.groupId = groupId;
			this.mailboxes = mailboxes;
			this.callerInfo = caller;
			CertificateValidationManager.RegisterCallback(base.GetType().FullName, new RemoteCertificateValidationCallback(CertificateValidation.CertificateErrorHandler));
			this.service = new ExchangeService(4, NonIndexableDiscoveryEwsClient.GetTimeZoneInfoFromExTimeZone(timeZone));
			this.service.Url = this.groupId.Uri;
			this.service.HttpHeaders[CertificateValidationManager.ComponentIdHeaderName] = base.GetType().FullName;
			if (this.groupId.GroupType != GroupType.CrossPremise)
			{
				this.service.UserAgent = WellKnownUserAgent.GetEwsNegoAuthUserAgent(base.GetType().FullName);
			}
			this.service.ClientRequestId = this.callerInfo.QueryCorrelationId.ToString("N");
			this.Authenticate();
		}

		public IAsyncResult BeginGetNonIndexableItemStatistics(AsyncCallback callback, object state, GetNonIndexableItemStatisticsParameters parameters)
		{
			return this.service.BeginGetNonIndexableItemStatistics(callback, state, parameters);
		}

		public GetNonIndexableItemStatisticsResponse EndGetNonIndexableItemStatistics(IAsyncResult result)
		{
			return this.service.EndGetNonIndexableItemStatistics(result);
		}

		public IAsyncResult BeginGetNonIndexableItemDetails(AsyncCallback callback, object state, GetNonIndexableItemDetailsParameters parameters)
		{
			return this.service.BeginGetNonIndexableItemDetails(callback, state, parameters);
		}

		public GetNonIndexableItemDetailsResponse EndGetNonIndexableItemDetails(IAsyncResult result)
		{
			return this.service.EndGetNonIndexableItemDetails(result);
		}

		private static TimeZoneInfo GetTimeZoneInfoFromExTimeZone(ExTimeZone timeZone)
		{
			return TimeZoneInfo.Utc;
		}

		private void Authenticate()
		{
			switch (this.groupId.GroupType)
			{
			case GroupType.CrossServer:
				this.service.OnSerializeCustomSoapHeaders += new CustomXmlSerializationDelegate(this.OnSerializeCustomSoapHeaders);
				return;
			case GroupType.CrossPremise:
				this.service.ManagementRoles = new ManagementRoles(null, DiscoveryEwsClient.MailboxSearchApplicationRole);
				this.service.Credentials = new OAuthCredentials(OAuthCredentials.GetOAuthCredentialsForAppToken(this.callerInfo.OrganizationId, this.mailboxes[0].GetDomain()));
				return;
			default:
				return;
			}
		}

		private void OnSerializeCustomSoapHeaders(XmlWriter writer)
		{
			Util.SerializeIdentityCustomSoapHeaders(NonIndexableDiscoveryEwsClient.securityContextSerializer, writer, this.callerInfo.PrimarySmtpAddress);
		}

		private static XmlSerializer securityContextSerializer = new XmlSerializer(typeof(OpenAsAdminOrSystemServiceType));

		private readonly ExchangeService service;

		private readonly CallerInfo callerInfo;

		private readonly MailboxInfo[] mailboxes;

		private readonly GroupId groupId;
	}
}
