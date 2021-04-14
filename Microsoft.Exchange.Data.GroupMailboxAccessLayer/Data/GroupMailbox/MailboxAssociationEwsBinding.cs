using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxAssociationEwsBinding : PrivateExchangeServiceBinding
	{
		internal MailboxAssociationEwsBinding(ADUser user, OpenAsAdminOrSystemServiceBudgetTypeType budgetType) : base("GroupMailboxAccessLayer", new RemoteCertificateValidationCallback(CommonCertificateValidationCallbacks.InternalServerToServer))
		{
			ArgumentValidator.ThrowIfNull("user", user);
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(user, null);
			MailboxAssociationEwsBinding.Tracer.TraceDebug<bool>((long)this.GetHashCode(), "MailboxAssociationEwsBinding.ctor - ExchangePrincipal.MailboxInfo.Location is null? {0}", exchangePrincipal.MailboxInfo.Location == null);
			Uri ewsEndpoint = null;
			MailboxAssociationEwsBinding.ExecuteEwsOperationWithRetry("GetBackEndWebServicesUrl", delegate
			{
				ewsEndpoint = BackEndLocator.GetBackEndWebServicesUrl(exchangePrincipal.MailboxInfo);
			});
			if (ewsEndpoint == null)
			{
				throw new MailboxNotFoundException(Strings.EwsUrlDiscoveryFailed(user.PrimarySmtpAddress.ToString()));
			}
			base.Url = ewsEndpoint.ToString();
			if (string.IsNullOrEmpty(base.Url))
			{
				throw new MailboxNotFoundException(Strings.EwsUrlDiscoveryFailed(user.PrimarySmtpAddress.ToString()));
			}
			base.HttpHeaders[WellKnownHeader.EWSTargetVersion] = EwsTargetVersion.V2_7;
			base.UserAgent = "GroupMailboxAccessLayer";
			base.Proxy = new WebProxy();
			base.SetClientRequestIdHeaderFromActivityId();
			base.Authenticator = SoapHttpClientAuthenticator.CreateNetworkService();
			base.Authenticator.AdditionalSoapHeaders.Add(new OpenAsAdminOrSystemServiceType
			{
				ConnectingSID = new ConnectingSIDType
				{
					Item = new SmtpAddressType
					{
						Value = user.PrimarySmtpAddress.ToString()
					}
				},
				LogonType = SpecialLogonType.Admin,
				BudgetType = (int)budgetType,
				BudgetTypeSpecified = true
			});
			NetworkServiceImpersonator.Initialize();
		}

		public static void ExecuteEwsOperationWithRetry(string taskName, Action function)
		{
			MailboxAssociationEwsBinding.<>c__DisplayClassc CS$<>8__locals1 = new MailboxAssociationEwsBinding.<>c__DisplayClassc();
			CS$<>8__locals1.taskName = taskName;
			CS$<>8__locals1.function = function;
			CS$<>8__locals1.retryCount = 0;
			CS$<>8__locals1.taskCompleted = false;
			while (!CS$<>8__locals1.taskCompleted)
			{
				ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteEwsOperationWithRetry>b__6)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteEwsOperationWithRetry>b__7)), new CatchDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteEwsOperationWithRetry>b__8)));
			}
		}

		private static bool IsTransientFailure(Exception e)
		{
			return e is TransientException || e is BackEndLocatorException || e is IOException || e is WebException || e is SoapException;
		}

		private const int MaximumTransientFailureRetries = 3;

		private const string ComponentId = "GroupMailboxAccessLayer";

		private static readonly Trace Tracer = ExTraceGlobals.AssociationReplicationTracer;
	}
}
