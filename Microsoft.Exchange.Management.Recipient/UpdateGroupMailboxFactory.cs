using System;
using System.Linq;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UpdateGroupMailboxFactory
	{
		public static UpdateGroupMailboxBase CreateUpdateGroupMailbox(IRecipientSession adSession, ADUser group, DatabaseLocationInfo databaseLocationInfo, ADUser executingUser, GroupMailboxConfigurationActionType forceActionMask, ADUser[] addedMembers, ADUser[] removedMembers, int? permissionsVersion)
		{
			ExchangePrincipal exchangePrincipal;
			using (new StopwatchPerformanceTracker("UpdateGroupMailboxFactory.CreateUpdateGroupMailbox.BuildExchangePrincipal", GenericCmdletInfoDataLogger.Instance))
			{
				exchangePrincipal = ExchangePrincipal.FromADUser(group, databaseLocationInfo, RemotingOptions.AllowCrossSite);
			}
			Uri backEndWebServicesUrl;
			using (new StopwatchPerformanceTracker("UpdateGroupMailboxFactory.CreateUpdateGroupMailbox.GetBackEndHttpService", GenericCmdletInfoDataLogger.Instance))
			{
				backEndWebServicesUrl = BackEndLocator.GetBackEndWebServicesUrl(exchangePrincipal.MailboxInfo);
				UpdateGroupMailboxFactory.Tracer.TraceDebug(0L, "UpdateGroupMailboxFactory.CreateUpdateGroupMailbox - BackendUrl=" + backEndWebServicesUrl);
			}
			if (ExEnvironment.IsTest && !VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				UserMailboxLocator[] array;
				if (addedMembers == null)
				{
					array = null;
				}
				else
				{
					array = (from user in addedMembers
					select UserMailboxLocator.Instantiate(adSession, user)).ToArray<UserMailboxLocator>();
				}
				UserMailboxLocator[] addedMembers2 = array;
				UserMailboxLocator[] array2;
				if (removedMembers == null)
				{
					array2 = null;
				}
				else
				{
					array2 = (from user in removedMembers
					select UserMailboxLocator.Instantiate(adSession, user)).ToArray<UserMailboxLocator>();
				}
				UserMailboxLocator[] removedMembers2 = array2;
				return new UpdateGroupMailboxViaXSO(adSession, group, exchangePrincipal, executingUser, forceActionMask, addedMembers2, removedMembers2, permissionsVersion);
			}
			return new UpdateGroupMailboxViaEWS(group, executingUser, backEndWebServicesUrl, forceActionMask, addedMembers, removedMembers, permissionsVersion);
		}

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;
	}
}
