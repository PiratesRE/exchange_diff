using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Management.StoreTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Cmdlet("Get", "PushNotificationSubscription", DefaultParameterSetName = "Default")]
	public sealed class GetPushNotificationSubscription : GetTaskBase<ADRecipient>
	{
		[Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
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

		[Parameter(Mandatory = false, ParameterSetName = "ShowAll")]
		public SwitchParameter ShowAll
		{
			get
			{
				return (SwitchParameter)(base.Fields["ShowAll"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ShowAll"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExpirationTime")]
		public uint ExpirationTimeInHours
		{
			get
			{
				if (base.Fields["ExpirationTime"] != null)
				{
					return (uint)base.Fields["ExpirationTime"];
				}
				return 72U;
			}
			set
			{
				base.Fields["ExpirationTime"] = value;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(ConnectionFailedPermanentException).IsInstanceOfType(exception);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return base.TenantGlobalCatalogSession;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.Mailbox.ToString())));
			ExchangePrincipal principal = ExchangePrincipal.FromADUser(aduser, RemotingOptions.AllowCrossSite);
			using (MailboxSession mailboxSession = StoreTasksHelper.OpenMailboxSession(principal, "Get-PushNotificationSubscription"))
			{
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.PushNotificationRoot);
				if (defaultFolderId != null)
				{
					using (IFolder folder = GetPushNotificationSubscription.xsoFactory.BindToFolder(mailboxSession, defaultFolderId))
					{
						IEnumerable<IStorePropertyBag> enumerable;
						if (this.ShowAll)
						{
							enumerable = new SubscriptionItemEnumerator(folder, this.ResultSize);
						}
						else
						{
							enumerable = new ActiveSubscriptionItemEnumerator(folder, this.ExpirationTimeInHours, this.ResultSize);
						}
						foreach (IStorePropertyBag propertyBag in enumerable)
						{
							this.WriteResult(this.CreatePresentationObject(propertyBag, aduser, mailboxSession));
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		private PushNotificationSubscription CreatePresentationObject(IStorePropertyBag propertyBag, ADUser aduser, MailboxSession mailboxSession)
		{
			VersionedId valueOrDefault = propertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
			string valueOrDefault2 = propertyBag.GetValueOrDefault<string>(PushNotificationSubscriptionItemSchema.SubscriptionId, null);
			string serializedNotificationSubscription = PushNotificationStorage.GetSerializedNotificationSubscription(mailboxSession, propertyBag, GetPushNotificationSubscription.xsoFactory);
			base.WriteVerbose(Strings.WriteVerboseSerializedSubscription(serializedNotificationSubscription));
			return new PushNotificationSubscription(aduser.ObjectId, valueOrDefault, valueOrDefault2, serializedNotificationSubscription);
		}

		private static readonly XSOFactory xsoFactory = new XSOFactory();
	}
}
