using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal static class OAuthCredentialsFactory
	{
		public static OAuthCredentials CreateAsApp(InternalClientContext clientContext, RequestLogger requestLogger)
		{
			ArgumentValidator.ThrowIfNull("clientContext", clientContext);
			OrganizationId organizationId = clientContext.OrganizationId;
			string domain = clientContext.ADUser.PrimarySmtpAddress.Domain;
			string text = FaultInjection.TraceTest<string>((FaultInjection.LIDs)2743479613U);
			if (!string.IsNullOrEmpty(text))
			{
				domain = SmtpAddress.Parse(text).Domain;
				organizationId = OrganizationId.FromAcceptedDomain(domain);
			}
			OAuthCredentials oauthCredentialsForAppToken = OAuthCredentials.GetOAuthCredentialsForAppToken(organizationId, domain);
			OAuthCredentialsFactory.SetCredentialsProperties(oauthCredentialsForAppToken, clientContext, requestLogger);
			return oauthCredentialsForAppToken;
		}

		public static OAuthCredentials Create(InternalClientContext clientContext, RequestLogger requestLogger)
		{
			ArgumentValidator.ThrowIfNull("clientContext", clientContext);
			OrganizationId organizationId = clientContext.OrganizationId;
			ADUser aduser = clientContext.ADUser;
			string text = FaultInjection.TraceTest<string>((FaultInjection.LIDs)2743479613U);
			if (!string.IsNullOrEmpty(text))
			{
				SmtpAddress smtpAddress = SmtpAddress.Parse(text);
				IRecipientSession recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromTenantAcceptedDomain(smtpAddress.Domain), 68, "Create", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\RequestDispatch\\OAuthCredentialsFactory.cs");
				aduser = (recipientSession.FindByProxyAddress(ProxyAddress.Parse(text)) as ADUser);
				organizationId = aduser.OrganizationId;
			}
			OAuthCredentials oauthCredentialsForAppActAsToken = OAuthCredentials.GetOAuthCredentialsForAppActAsToken(organizationId, aduser, null);
			OAuthCredentialsFactory.SetCredentialsProperties(oauthCredentialsForAppActAsToken, clientContext, requestLogger);
			return oauthCredentialsForAppActAsToken;
		}

		private static void SetCredentialsProperties(OAuthCredentials creds, InternalClientContext clientContext, RequestLogger requestLogger)
		{
			creds.Tracer = new OAuthCredentialsFactory.OAuthOutboundTracer(requestLogger);
			string messageId = clientContext.MessageId;
			Guid value;
			if (messageId.StartsWith(OAuthCredentialsFactory.messagePrefix) && messageId.Length > OAuthCredentialsFactory.messagePrefixLength && Guid.TryParse(messageId.Substring(OAuthCredentialsFactory.messagePrefixLength), out value))
			{
				creds.ClientRequestId = new Guid?(value);
			}
		}

		private static string messagePrefix = "urn:uuid:";

		private static int messagePrefixLength = OAuthCredentialsFactory.messagePrefix.Length;

		public sealed class OAuthOutboundTracer : DefaultOutboundTracer
		{
			public OAuthOutboundTracer(RequestLogger requestLogger)
			{
				this.logger = requestLogger;
			}

			protected override void LogWarning2(int hashCode, string formatString, params object[] args)
			{
				this.logger.AppendToLog<string>("OAuth", string.Format(formatString, args));
			}

			protected override void LogError2(int hashCode, string formatString, params object[] args)
			{
				this.logger.AppendToLog<string>("OAuth", string.Format(formatString, args));
			}

			private readonly RequestLogger logger;
		}
	}
}
