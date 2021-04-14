using System;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.GroupMailbox.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class GroupMailboxAccessLayerHelper
	{
		public static void ExecuteOperationWithRetry(IExtensibleLogger logger, string context, Action action, Predicate<Exception> shouldCatchException)
		{
			GroupMailboxAccessLayerHelper.<>c__DisplayClass6 CS$<>8__locals1 = new GroupMailboxAccessLayerHelper.<>c__DisplayClass6();
			CS$<>8__locals1.logger = logger;
			CS$<>8__locals1.context = context;
			CS$<>8__locals1.action = action;
			CS$<>8__locals1.shouldCatchException = shouldCatchException;
			CS$<>8__locals1.retryCount = 0;
			CS$<>8__locals1.attemptOperation = true;
			while (CS$<>8__locals1.attemptOperation)
			{
				ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteOperationWithRetry>b__0)), new FilterDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteOperationWithRetry>b__1)), new CatchDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ExecuteOperationWithRetry>b__2)));
			}
		}

		internal static bool GetDomainControllerAffinityForOrganization(OrganizationId orgId, out ADServerInfo preferredDomainController)
		{
			if (orgId == null || orgId.ConfigurationUnit == null || orgId == OrganizationId.ForestWideOrgId)
			{
				preferredDomainController = null;
				return false;
			}
			ADRunspaceServerSettingsProvider instance = ADRunspaceServerSettingsProvider.GetInstance();
			bool flag;
			preferredDomainController = instance.GetGcFromToken(orgId.PartitionId.ForestFQDN, RunspaceServerSettings.GetTokenForOrganization(orgId), out flag, true);
			return true;
		}

		private static void LogException(IExtensibleLogger logger, string context, Exception exception)
		{
			GroupMailboxAccessLayerHelper.Tracer.TraceError<string, Exception>(0L, "GroupMailboxHelper.LogException: Exception found while executing {0}. Exception: {1}.", context, exception);
			logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.Error>
			{
				{
					MailboxAssociationLogSchema.Error.Context,
					context
				},
				{
					MailboxAssociationLogSchema.Error.Exception,
					exception
				}
			});
		}

		private const int MaximumTransientFailureRetries = 3;

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;
	}
}
