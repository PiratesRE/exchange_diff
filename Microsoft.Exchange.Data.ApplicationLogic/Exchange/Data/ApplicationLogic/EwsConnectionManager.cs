using System;
using System.Net;
using System.Net.Security;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class EwsConnectionManager : IEwsConnectionManager
	{
		public EwsConnectionManager(ExchangePrincipal principal, OpenAsAdminOrSystemServiceBudgetTypeType budgetType, Trace tracer)
		{
			EnumValidator.AssertValid<OpenAsAdminOrSystemServiceBudgetTypeType>(budgetType);
			this.budgetType = budgetType;
			this.currentExchangePrincipal = principal;
			this.Tracer = tracer;
		}

		private Trace Tracer { get; set; }

		public string GetSmtpAddress()
		{
			return this.currentExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
		}

		public string GetPrincipalInfoForTracing()
		{
			return this.currentExchangePrincipal.ToString();
		}

		public void ReloadPrincipal()
		{
			string text = this.currentExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			SmtpAddress smtpAddress = new SmtpAddress(text);
			this.currentExchangePrincipal = ExchangePrincipal.FromProxyAddress(ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(smtpAddress.Domain), text);
		}

		public Uri GetBackEndWebServicesUrl()
		{
			return BackEndLocator.GetBackEndWebServicesUrl(this.currentExchangePrincipal.MailboxInfo);
		}

		public virtual IExchangeService CreateBinding(RemoteCertificateValidationCallback certificateErrorHandler)
		{
			bool flag = true;
			NetworkServiceImpersonator.Initialize();
			if (NetworkServiceImpersonator.Exception != null)
			{
				if (this.IsTraceEnabled(TraceType.ErrorTrace))
				{
					this.Tracer.TraceError<LocalizedException>(0L, "Unable to impersonate network service to call EWS due to exception {0}", NetworkServiceImpersonator.Exception);
				}
				flag = false;
			}
			ExchangeServiceBinding exchangeServiceBinding = new ExchangeServiceBinding(certificateErrorHandler);
			exchangeServiceBinding.UserAgent = WellKnownUserAgent.GetEwsNegoAuthUserAgent("AuditLog");
			exchangeServiceBinding.RequestServerVersionValue = new RequestServerVersion
			{
				Version = ExchangeVersionType.Exchange2013
			};
			if (flag)
			{
				exchangeServiceBinding.Authenticator = SoapHttpClientAuthenticator.CreateNetworkService();
			}
			else
			{
				exchangeServiceBinding.Authenticator = SoapHttpClientAuthenticator.Create(CredentialCache.DefaultCredentials);
			}
			exchangeServiceBinding.Authenticator.AdditionalSoapHeaders.Add(new OpenAsAdminOrSystemServiceType
			{
				ConnectingSID = new ConnectingSIDType
				{
					Item = new PrimarySmtpAddressType
					{
						Value = this.GetSmtpAddress()
					}
				},
				LogonType = SpecialLogonType.SystemService,
				BudgetType = (int)this.budgetType,
				BudgetTypeSpecified = true
			});
			return exchangeServiceBinding;
		}

		private bool IsTraceEnabled(TraceType traceType)
		{
			return this.Tracer != null && this.Tracer.IsTraceEnabled(traceType);
		}

		private readonly OpenAsAdminOrSystemServiceBudgetTypeType budgetType;

		private ExchangePrincipal currentExchangePrincipal;
	}
}
