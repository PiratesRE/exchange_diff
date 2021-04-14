using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpdateGroupMailboxEwsBinding : PrivateExchangeServiceBinding
	{
		internal UpdateGroupMailboxEwsBinding(ADUser group, Uri ewsEndpoint) : base("GroupMailboxCmdlet", new RemoteCertificateValidationCallback(CommonCertificateValidationCallbacks.InternalServerToServer))
		{
			ArgumentValidator.ThrowIfNull("group", group);
			ArgumentValidator.ThrowIfNull("ewsEndpointUrl", ewsEndpoint);
			base.Url = ewsEndpoint.ToString();
			base.HttpHeaders[WellKnownHeader.EWSTargetVersion] = EwsTargetVersion.V2_14;
			base.UserAgent = "GroupMailboxCmdlet";
			base.Proxy = new WebProxy();
			base.SetClientRequestIdHeaderFromActivityId();
			using (new StopwatchPerformanceTracker("UpdateGroupMailboxEwsBinding.CreateNetworkService", GenericCmdletInfoDataLogger.Instance))
			{
				base.Authenticator = SoapHttpClientAuthenticator.CreateNetworkService();
			}
			using (new StopwatchPerformanceTracker("UpdateGroupMailboxEwsBinding.InitNetworkServiceImpersonator", GenericCmdletInfoDataLogger.Instance))
			{
				NetworkServiceImpersonator.Initialize();
			}
		}

		private UpdateGroupMailboxEwsBinding() : base("GroupMailboxCmdlet", new RemoteCertificateValidationCallback(CommonCertificateValidationCallbacks.InternalServerToServer))
		{
		}

		public static void InitializeXmlSerializer()
		{
			using (new UpdateGroupMailboxEwsBinding())
			{
			}
		}

		public static void ExecuteEwsOperationWithRetry(string taskName, Action function)
		{
			UpdateGroupMailboxEwsBinding.<>c__DisplayClass6 CS$<>8__locals1 = new UpdateGroupMailboxEwsBinding.<>c__DisplayClass6();
			CS$<>8__locals1.taskName = taskName;
			CS$<>8__locals1.function = function;
			CS$<>8__locals1.retryCount = 0;
			CS$<>8__locals1.taskCompleted = false;
			while (!CS$<>8__locals1.taskCompleted)
			{
				ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteEwsOperationWithRetry>b__0)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteEwsOperationWithRetry>b__1)), new CatchDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteEwsOperationWithRetry>b__2)));
			}
		}

		public UpdateGroupMailboxResponseType ExecuteUpdateGroupMailboxWithRetry(UpdateGroupMailboxType request)
		{
			UpdateGroupMailboxResponseType response = null;
			UpdateGroupMailboxEwsBinding.ExecuteEwsOperationWithRetry("ExecuteUpdateGroupMailboxWithRetry", delegate
			{
				response = this.UpdateGroupMailbox(request);
			});
			return response;
		}

		private static bool IsTransientFailure(Exception e)
		{
			return e is TransientException || e is BackEndLocatorException || e is IOException || e is WebException || e is SoapException;
		}

		private const int MaximumTransientFailureRetries = 3;

		private const string ComponentId = "GroupMailboxCmdlet";

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;
	}
}
