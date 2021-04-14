using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Management.StoreTasks;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Cmdlet("Remove", "PushNotificationSubscription", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "RemoveAll")]
	public sealed class RemovePushNotificationSubscription : RemoveTaskBase<MailboxIdParameter, ADUser>
	{
		[Parameter(Position = 0, Mandatory = true, ParameterSetName = "RemoveStorage")]
		[ValidateNotNullOrEmpty]
		[Parameter(Position = 0, Mandatory = true, ParameterSetName = "RemoveAll")]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RemoveStorage")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveAll")]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? false);
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RemoveStorage")]
		public SwitchParameter RemoveStorage
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveStorage"] ?? false);
			}
			set
			{
				base.Fields["RemoveStorage"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "IndividualRemove", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public PushNotificationStoreId SubscriptionStoreId
		{
			get
			{
				return (PushNotificationStoreId)base.Fields["SubscriptionStoreId"];
			}
			set
			{
				base.Fields["SubscriptionStoreId"] = value;
			}
		}

		public override MailboxIdParameter Identity { get; set; }

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(ConnectionFailedPermanentException).IsInstanceOfType(exception);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.SubscriptionStoreId != null)
				{
					return Strings.ConfirmationMessageRemoveSinglePushNotificationSubscription(this.SubscriptionStoreId.ToString());
				}
				return Strings.ConfirmationMessageRemovePushNotificationSubscription(this.Mailbox.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return base.TenantGlobalCatalogSession;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.Mailbox != null)
			{
				this.Identity = this.Mailbox;
			}
			else if (this.SubscriptionStoreId != null)
			{
				this.Identity = new MailboxIdParameter(this.SubscriptionStoreId.MailboxOwnerId.ToString());
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ExchangePrincipal principal = ExchangePrincipal.FromADUser(base.DataObject, RemotingOptions.AllowCrossSite);
			using (MailboxSession mailboxSession = StoreTasksHelper.OpenMailboxSession(principal, "Remove-PushNotificationSubscription"))
			{
				using (IPushNotificationStorage pushNotificationStorage = PushNotificationStorage.Find(mailboxSession))
				{
					if (pushNotificationStorage != null)
					{
						if (this.SubscriptionStoreId != null)
						{
							pushNotificationStorage.DeleteSubscription(this.SubscriptionStoreId.StoreObjectIdValue);
						}
						else if (this.Force || base.ShouldContinue(Strings.ConfirmRemoveUserPushNotificationSubscriptions(this.Mailbox.ToString())))
						{
							if (base.ParameterSetName.Equals("RemoveStorage"))
							{
								PushNotificationStorage.DeleteStorage(mailboxSession);
							}
							else
							{
								pushNotificationStorage.DeleteAllSubscriptions();
							}
						}
					}
				}
			}
			TaskLogger.LogExit();
		}
	}
}
