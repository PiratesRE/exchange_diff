using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Web;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	[LegacyServiceBehavior]
	public class LegacyAutodiscoverService : ILegacyAutodiscover
	{
		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public Message LegacyGetAction(Message input)
		{
			return Message.CreateMessage(MessageVersion.None, "*");
		}

		[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
		public Message LegacyAction(Message input)
		{
			Message reply = null;
			Common.SendWatsonReportOnUnhandledException(delegate
			{
				RequestData requestData = null;
				object obj = null;
				HttpContext httpContext = HttpContext.Current;
				if (input.Properties.TryGetValue("RequestData", out obj))
				{
					requestData = (RequestData)obj;
				}
				else
				{
					bool useClientCertificateAuthentication = Common.CheckClientCertificate(httpContext.Request);
					requestData = new RequestData(null, useClientCertificateAuthentication, CallerRequestedCapabilities.GetInstance(httpContext));
					input.Properties["RequestData"] = requestData;
					input.Properties["ParseSuccess"] = false;
				}
				string userAgent = Common.SafeGetUserAgent(httpContext.Request);
				ProxyAddress proxyAddress;
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.RedirectOutlookClient.Enabled && AutodiscoverProxy.CanRedirectOutlookClient(userAgent) && !string.IsNullOrEmpty(requestData.EMailAddress) && SmtpProxyAddress.TryDeencapsulate(requestData.EMailAddress, out proxyAddress))
				{
					requestData.EMailAddress = proxyAddress.AddressString;
				}
				LegacyBodyWriter bodyWriter;
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.TrackLatency(ServiceLatencyMetadata.CoreExecutionLatency, delegate()
				{
					if (!HttpContext.Current.Request.IsAuthenticated)
					{
						requestData.Clear();
						input.Properties["ParseSuccess"] = false;
						bodyWriter = new LegacyBodyWriter(input, HttpContext.Current);
						reply = Message.CreateMessage(MessageVersion.None, "*", bodyWriter);
						HttpResponseMessageProperty httpResponseMessageProperty = new HttpResponseMessageProperty();
						httpResponseMessageProperty.StatusCode = HttpStatusCode.Unauthorized;
						reply.Properties.Add(HttpResponseMessageProperty.Name, httpResponseMessageProperty);
						return;
					}
					bodyWriter = new LegacyBodyWriter(input, HttpContext.Current);
					reply = Message.CreateMessage(MessageVersion.None, "*", bodyWriter);
				});
			});
			return reply;
		}
	}
}
