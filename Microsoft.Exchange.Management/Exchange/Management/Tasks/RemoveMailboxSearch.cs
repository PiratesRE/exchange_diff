using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.InfoWorker.Common.SearchService;
using Microsoft.Exchange.Management.Tasks.MailboxSearch;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.MailboxSearch;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Remove", "MailboxSearch", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMailboxSearch : RemoveTenantADTaskBase<EwsStoreObjectIdParameter, MailboxDiscoverySearch>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string name;
				bool flag;
				if (base.DataObject == null)
				{
					name = this.e14SearchObject.Name;
					flag = this.e14SearchObject.EstimateOnly;
				}
				else
				{
					name = base.DataObject.Name;
					flag = base.DataObject.StatisticsOnly;
				}
				if (flag)
				{
					return Strings.RemoveEstimateMailboxSearchConfirmation(name);
				}
				return Strings.RemoveMailboxSearchConfirmation(name);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = Utils.CreateRecipientSession(base.DomainController, base.SessionSettings);
			this.recipientSession = recipientSession;
			return new DiscoverySearchDataProvider(base.CurrentOrganizationId);
		}

		protected override void InternalValidate()
		{
			try
			{
				string text = this.Identity.ToString();
				this.e14DataProvider = Utils.GetMailboxDataProvider(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, new Task.TaskErrorLoggingDelegate(base.WriteError));
				if (Utils.IsLegacySearchObjectIdentity(text))
				{
					this.e14SearchObject = (SearchObject)base.GetDataObject<SearchObject>(new SearchObjectIdParameter(text), this.e14DataProvider, this.RootId, base.OptionalIdentityData, new LocalizedString?(Strings.ErrorManagementObjectNotFound(text)), new LocalizedString?(Strings.ErrorManagementObjectAmbiguous(text)));
				}
				else
				{
					this.e14SearchObject = Utils.GetE14SearchObjectByName(text, this.e14DataProvider);
				}
				if (this.e14SearchObject == null)
				{
					base.InternalValidate();
				}
			}
			catch (NoInternalEwsAvailableException exception)
			{
				base.WriteError(exception, ErrorCategory.ResourceUnavailable, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (base.ExchangeRunspaceConfig == null)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
				return;
			}
			if (base.DataObject == null)
			{
				if (this.e14SearchObject != null)
				{
					if (this.e14SearchObject.SearchStatus != null)
					{
						if (!string.IsNullOrEmpty(this.e14SearchObject.SearchStatus.ResultsPath) && this.e14SearchObject.TargetMailbox != null)
						{
							ADUser aduser = (ADUser)this.recipientSession.Read(this.e14SearchObject.TargetMailbox);
							if (aduser == null)
							{
								goto IL_357;
							}
							string serverFqdn = ExchangePrincipal.FromADUser(this.recipientSession.SessionSettings, aduser, RemotingOptions.AllowCrossSite).MailboxInfo.Location.ServerFqdn;
							if (!string.IsNullOrEmpty(serverFqdn))
							{
								goto IL_357;
							}
							SearchId searchId = new SearchId(this.e14DataProvider.ADUser.Id.DistinguishedName, this.e14DataProvider.ADUser.Id.ObjectGuid, this.e14SearchObject.Id.Guid.ToString());
							try
							{
								using (MailboxSearchClient client = new MailboxSearchClient(serverFqdn))
								{
									Utils.RpcCallWithRetry(delegate()
									{
										client.Remove(searchId, true);
									});
								}
								goto IL_357;
							}
							catch (SearchServerException ex)
							{
								if (ex.ErrorCode == -2147220980)
								{
									base.WriteError(new MailboxSearchIsInProgressException(this.e14SearchObject.Name), ErrorCategory.InvalidOperation, base.DataObject);
								}
								else
								{
									base.WriteError(ex, ErrorCategory.InvalidOperation, null);
								}
								goto IL_357;
							}
							catch (RpcConnectionException ex2)
							{
								base.WriteError(new TaskException(Strings.MailboxSearchServiceUnavailable(serverFqdn, ex2.ErrorCode), ex2), ErrorCategory.InvalidOperation, null);
								goto IL_357;
							}
							catch (RpcException ex3)
							{
								base.WriteError(new TaskException(Strings.MailboxSearchRpcCallFailed(serverFqdn, ex3.ErrorCode), ex3), ErrorCategory.InvalidOperation, null);
								goto IL_357;
							}
						}
						this.e14DataProvider.Delete(this.e14SearchObject.SearchStatus);
					}
					IL_357:
					this.e14DataProvider.Delete(this.e14SearchObject);
				}
				return;
			}
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.InPlaceHolds, base.DataObject.InPlaceHoldIdentity);
			ADRawEntry[] array = this.recipientSession.Find(null, QueryScope.SubTree, filter, null, 1);
			if (array != null && array.Length > 0)
			{
				base.WriteError(new TaskInvalidOperationException(Strings.ErrorCannotRemoveMailboxSearchWithMailboxOnHold(base.DataObject.Name)), ExchangeErrorCategory.Client, base.DataObject);
			}
			ExchangePrincipal exchangePrincipal = null;
			if (base.DataObject.Target != null)
			{
				try
				{
					exchangePrincipal = ExchangePrincipal.FromLegacyDN(this.recipientSession.SessionSettings, base.DataObject.Target, RemotingOptions.AllowCrossSite);
				}
				catch (ObjectNotFoundException)
				{
				}
				if (exchangePrincipal == null)
				{
					this.WriteWarning(Strings.ExceptionTargetMailboxNotFound(base.DataObject.Target, base.DataObject.Name));
				}
			}
			if (exchangePrincipal != null)
			{
				Utils.CheckSearchRunningStatus(base.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError), Strings.MailboxSearchIsInProgress(base.DataObject.Name));
				base.DataObject.UpdateState(SearchStateTransition.DeleteSearch);
				((DiscoverySearchDataProvider)base.DataSession).CreateOrUpdate<MailboxDiscoverySearch>(base.DataObject);
				Utils.CreateMailboxDiscoverySearchRequest((DiscoverySearchDataProvider)base.DataSession, base.DataObject.Name, ActionRequestType.Delete, base.ExchangeRunspaceConfig.GetRbacContext().ToString());
				SearchEventLogger.Instance.LogDiscoverySearchRemoveRequestedEvent(base.DataObject, base.ExchangeRunspaceConfig.GetRbacContext().ToString());
				return;
			}
			base.InternalProcessRecord();
		}

		private IRecipientSession recipientSession;

		private MailboxDataProvider e14DataProvider;

		private SearchObject e14SearchObject;
	}
}
