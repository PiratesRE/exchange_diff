using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class SubscriptionLoader
	{
		public SubscriptionLoader(ADUser adUser, IBudget requesterBudget)
		{
			this.adUser = adUser;
			this.requesterBudget = requesterBudget;
		}

		public Exception HandledException { get; private set; }

		public bool IsValid
		{
			get
			{
				return this.HandledException == null;
			}
		}

		private SharingSubscriptionData[] Subscriptions
		{
			get
			{
				if (this.subscriptions == null)
				{
					this.subscriptions = this.LoadAllSubscriptions();
				}
				return this.subscriptions;
			}
		}

		public SharingSubscriptionData GetUserSubscription(EmailAddress emailAddress)
		{
			foreach (SharingSubscriptionData sharingSubscriptionData in this.Subscriptions)
			{
				if (StringComparer.InvariantCultureIgnoreCase.Equals(sharingSubscriptionData.SharerIdentity, emailAddress.Address))
				{
					return sharingSubscriptionData;
				}
			}
			return null;
		}

		private SharingSubscriptionData[] LoadAllSubscriptions()
		{
			try
			{
				if (this.requesterBudget != null)
				{
					this.requesterBudget.CheckOverBudget();
				}
				ExchangePrincipal mailboxOwner = ExchangePrincipal.FromADUser(this.adUser, null);
				using (MailboxSession mailboxSession = MailboxSession.OpenAsSystemService(mailboxOwner, CultureInfo.InvariantCulture, "Client=AS"))
				{
					if (this.requesterBudget != null)
					{
						mailboxSession.AccountingObject = this.requesterBudget;
					}
					try
					{
						using (SharingSubscriptionManager sharingSubscriptionManager = new SharingSubscriptionManager(mailboxSession))
						{
							return sharingSubscriptionManager.GetAll();
						}
					}
					catch (ObjectNotFoundException)
					{
					}
				}
			}
			catch (OverBudgetException handledException)
			{
				this.HandledException = handledException;
			}
			catch (ConnectionFailedPermanentException handledException2)
			{
				this.HandledException = handledException2;
			}
			catch (ObjectNotFoundException handledException3)
			{
				this.HandledException = handledException3;
			}
			catch (ConnectionFailedTransientException handledException4)
			{
				this.HandledException = handledException4;
			}
			catch (AccountDisabledException handledException5)
			{
				this.HandledException = handledException5;
			}
			catch (VirusScanInProgressException innerException)
			{
				LocalizedString localizedString = Strings.descVirusScanInProgress(this.adUser.PrimarySmtpAddress.ToString());
				this.HandledException = new LocalizedException(localizedString, innerException);
			}
			catch (VirusDetectedException innerException2)
			{
				LocalizedString localizedString2 = Strings.descVirusDetected(this.adUser.PrimarySmtpAddress.ToString());
				this.HandledException = new LocalizedException(localizedString2, innerException2);
			}
			catch (StoragePermanentException handledException6)
			{
				this.HandledException = handledException6;
			}
			catch (StorageTransientException handledException7)
			{
				this.HandledException = handledException7;
			}
			return new SharingSubscriptionData[0];
		}

		private ADUser adUser;

		private IBudget requesterBudget;

		private SharingSubscriptionData[] subscriptions;
	}
}
