using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Management.Tasks.MailboxSearch;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Start", "MailboxSearch", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class StartMailboxSearch : ObjectActionTenantADTask<EwsStoreObjectIdParameter, MailboxDiscoverySearch>
	{
		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Resume
		{
			get
			{
				return (SwitchParameter)(base.Fields["Resume"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Resume"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int StatisticsStartIndex
		{
			get
			{
				return (int)(base.Fields["StatisticsStartIndex"] ?? -1);
			}
			set
			{
				base.Fields["StatisticsStartIndex"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageStartMailboxSearch(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = Utils.CreateRecipientSession(base.DomainController, base.SessionSettings);
			this.recipientSession = recipientSession;
			return new DiscoverySearchDataProvider(base.CurrentOrganizationId);
		}

		protected override IConfigurable PrepareDataObject()
		{
			string text = this.Identity.ToString();
			MailboxDataProvider mailboxDataProvider = Utils.GetMailboxDataProvider(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, new Task.TaskErrorLoggingDelegate(base.WriteError));
			SearchObject searchObject;
			if (Utils.IsLegacySearchObjectIdentity(text))
			{
				MailboxDiscoverySearch mailboxDiscoverySearch = ((DiscoverySearchDataProvider)base.DataSession).FindByLegacySearchObjectIdentity(text);
				if (mailboxDiscoverySearch != null)
				{
					return mailboxDiscoverySearch;
				}
				searchObject = (SearchObject)base.GetDataObject<SearchObject>(new SearchObjectIdParameter(text), mailboxDataProvider, this.RootId, base.OptionalIdentityData, new LocalizedString?(Strings.ErrorManagementObjectNotFound(text)), new LocalizedString?(Strings.ErrorManagementObjectAmbiguous(text)));
			}
			else
			{
				searchObject = Utils.GetE14SearchObjectByName(text, mailboxDataProvider);
			}
			if (searchObject == null)
			{
				return base.PrepareDataObject();
			}
			if (!this.Force && !base.ShouldContinue(Strings.EditWillUpgradeSearchObject))
			{
				base.WriteError(new MailboxSearchTaskException(Strings.CannotEditLegacySearchObjectWithoutUpgrade(searchObject.Name)), ErrorCategory.InvalidArgument, text);
			}
			return Utils.UpgradeLegacySearchObject(searchObject, mailboxDataProvider, (DiscoverySearchDataProvider)base.DataSession, new Task.TaskErrorLoggingDelegate(base.WriteError), new Action<LocalizedString>(this.WriteWarning));
		}

		protected override void InternalValidate()
		{
			try
			{
				base.InternalValidate();
			}
			catch (TenantAccessBlockedException exception)
			{
				base.WriteError(exception, (ErrorCategory)1003, null);
				return;
			}
			if (!this.DataObject.StatisticsOnly)
			{
				if (string.IsNullOrEmpty(this.DataObject.Target))
				{
					base.WriteError(new MailboxSearchTaskException(Strings.TargetMailboxRequired), ExchangeErrorCategory.Context, this.DataObject);
					return;
				}
				ExchangePrincipal exchangePrincipal = null;
				try
				{
					this.recipientSession.SessionSettings.IncludeInactiveMailbox = false;
					exchangePrincipal = ExchangePrincipal.FromLegacyDN(this.recipientSession.SessionSettings, this.DataObject.Target, RemotingOptions.AllowCrossSite);
				}
				catch (ObjectNotFoundException)
				{
				}
				finally
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						this.recipientSession.SessionSettings.IncludeInactiveMailbox = false;
					}
				}
				if (exchangePrincipal == null)
				{
					base.WriteError(new ObjectNotFoundException(Strings.ExceptionTargetMailboxNotFound(this.DataObject.Target, this.DataObject.Name)), ErrorCategory.InvalidOperation, null);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.DataObject == null)
			{
				return;
			}
			if (base.ExchangeRunspaceConfig == null)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			Utils.CheckSearchRunningStatus(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError), Strings.CannotStartRunningSearch(this.DataObject.Name));
			this.DataObject.Resume = this.Resume;
			if (this.StatisticsStartIndex != -1)
			{
				this.DataObject.StatisticsStartIndex = this.StatisticsStartIndex;
			}
			ActionRequestType requestType = ActionRequestType.Start;
			if (this.Resume)
			{
				if (this.DataObject.Status == SearchState.Succeeded || this.DataObject.Status == SearchState.EstimateSucceeded)
				{
					this.WriteWarning(Strings.SearchCompletedCannotBeResumed(this.DataObject.Name));
					return;
				}
				if (this.DataObject.Status == SearchState.Stopped || this.DataObject.Status == SearchState.EstimateStopped)
				{
					this.WriteWarning(Strings.SearchStoppedCannotBeResumed(this.DataObject.Name));
					return;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(this.DataObject.Query) && (this.DataObject.Senders == null || this.DataObject.Senders.Count == 0) && (this.DataObject.Recipients == null || this.DataObject.Recipients.Count == 0) && (this.DataObject.MessageTypes == null || this.DataObject.MessageTypes.Count == 0) && this.DataObject.StartDate == null && this.DataObject.EndDate == null && !this.Force && !base.ShouldContinue(Strings.ContinueOnEmptySearchQuery))
				{
					return;
				}
				if (!string.IsNullOrEmpty(this.DataObject.ResultsPath))
				{
					if (!this.DataObject.StatisticsOnly && !this.Force && !base.ShouldContinue(Strings.ContinueOnRemoveSearchResults))
					{
						return;
					}
					requestType = ActionRequestType.Restart;
				}
				Utils.ResetSearchResults(this.DataObject);
			}
			Utils.CheckDiscoveryEnabled(this.recipientSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			this.DataObject.UpdateState(SearchStateTransition.StartSearch);
			((DiscoverySearchDataProvider)base.DataSession).CreateOrUpdate<MailboxDiscoverySearch>(this.DataObject);
			Utils.CreateMailboxDiscoverySearchRequest((DiscoverySearchDataProvider)base.DataSession, this.DataObject.Name, requestType, base.ExchangeRunspaceConfig.GetRbacContext().ToString());
			SearchEventLogger.Instance.LogDiscoverySearchStartRequestedEvent(this.DataObject, base.ExchangeRunspaceConfig.GetRbacContext().ToString());
			TaskLogger.LogExit();
		}

		private const string ParameterResume = "Resume";

		private const string ParameterStatisticsStartIndex = "StatisticsStartIndex";

		private IRecipientSession recipientSession;
	}
}
