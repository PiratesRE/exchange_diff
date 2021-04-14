using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim
{
	[Serializable]
	public class PimSubscriptionProxy : IConfigurable
	{
		public PimSubscriptionProxy() : this(new PimAggregationSubscription())
		{
		}

		internal PimSubscriptionProxy(PimAggregationSubscription subscription)
		{
			if (subscription == null)
			{
				throw new ArgumentNullException("subscription");
			}
			this.subscription = subscription;
			this.objectState = (subscription.IsNew ? ObjectState.New : ObjectState.Unchanged);
		}

		public Guid SubscriptionGuid
		{
			get
			{
				return this.subscription.SubscriptionGuid;
			}
		}

		public string Name
		{
			get
			{
				return this.RedactIfNeeded(this.subscription.Name, false);
			}
			internal set
			{
				this.subscription.Name = value;
			}
		}

		public SubscriptionCreationType CreationType
		{
			get
			{
				return this.subscription.CreationType;
			}
			set
			{
				this.subscription.CreationType = value;
			}
		}

		public AggregationType AggregationType
		{
			get
			{
				return this.subscription.AggregationType;
			}
			set
			{
				this.subscription.AggregationType = value;
			}
		}

		public SyncPhase SyncPhase
		{
			get
			{
				return this.subscription.SyncPhase;
			}
		}

		public long Version
		{
			get
			{
				return this.subscription.Version;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.RedactIfNeeded(this.subscription.UserDisplayName, false);
			}
			set
			{
				this.subscription.UserDisplayName = value;
			}
		}

		public SmtpAddress EmailAddress
		{
			get
			{
				return this.RedactIfNeeded(this.subscription.UserEmailAddress);
			}
			set
			{
				this.subscription.UserEmailAddress = value;
			}
		}

		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return this.subscription.SubscriptionType;
			}
		}

		public AggregationStatus Status
		{
			get
			{
				return this.subscription.Status;
			}
		}

		public DetailedAggregationStatus DetailedAggregationStatus
		{
			get
			{
				return this.subscription.DetailedAggregationStatus;
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return this.subscription.LastModifiedTime;
			}
		}

		public DateTime? LastSyncTime
		{
			get
			{
				return this.subscription.LastSyncTime;
			}
		}

		public DateTime? LastSuccessfulSync
		{
			get
			{
				if (this.subscription.LastSuccessfulSyncTime != null && this.subscription.LastSuccessfulSyncTime.Value == SyncUtilities.ZeroTime)
				{
					return null;
				}
				return this.subscription.LastSuccessfulSyncTime;
			}
		}

		public bool IsWarningStatus
		{
			get
			{
				return this.Status == AggregationStatus.Delayed || this.Status == AggregationStatus.InvalidVersion;
			}
		}

		public bool IsErrorStatus
		{
			get
			{
				return this.Status == AggregationStatus.Disabled || this.Status == AggregationStatus.Poisonous;
			}
		}

		public bool IsSuccessStatus
		{
			get
			{
				return this.Status == AggregationStatus.Succeeded || this.Status == AggregationStatus.InProgress;
			}
		}

		public string StatusDescription
		{
			get
			{
				switch (this.Status)
				{
				case AggregationStatus.Succeeded:
					return Strings.SuccessStatus;
				case AggregationStatus.InProgress:
					return Strings.InProgressStatus;
				case AggregationStatus.Delayed:
					switch (this.DetailedAggregationStatus)
					{
					case DetailedAggregationStatus.AuthenticationError:
						return Strings.AuthenticationErrorWithDelayedStatus;
					case DetailedAggregationStatus.ConnectionError:
						return Strings.ConnectionErrorWithDelayedStatus;
					case DetailedAggregationStatus.CommunicationError:
						return Strings.CommunicationErrorWithDelayedStatus;
					case DetailedAggregationStatus.RemoteMailboxQuotaWarning:
						return Strings.RemoteMailboxQuotaWarningWithDelayedStatus;
					case DetailedAggregationStatus.LabsMailboxQuotaWarning:
						return Strings.LabsMailboxQuotaWarningWithDelayedStatus;
					case DetailedAggregationStatus.MaxedOutSyncRelationshipsError:
						return Strings.MaxedOutSyncRelationshipsErrorWithDelayedStatus;
					case DetailedAggregationStatus.LeaveOnServerNotSupported:
						return Strings.LeaveOnServerNotSupportedStatus;
					case DetailedAggregationStatus.RemoteAccountDoesNotExist:
						return Strings.RemoteAccountDoesNotExistStatus;
					case DetailedAggregationStatus.RemoteServerIsSlow:
					case DetailedAggregationStatus.RemoteServerIsBackedOff:
						return Strings.RemoteServerIsSlowStatus;
					case DetailedAggregationStatus.TooManyFolders:
						return Strings.TooManyFoldersStatus;
					case DetailedAggregationStatus.ProviderException:
						return Strings.ProviderExceptionStatus;
					}
					return Strings.DelayedStatus;
				case AggregationStatus.Disabled:
					switch (this.DetailedAggregationStatus)
					{
					case DetailedAggregationStatus.AuthenticationError:
						return Strings.AuthenticationErrorWithDisabledStatus;
					case DetailedAggregationStatus.ConnectionError:
						return Strings.ConnectionErrorWithDisabledStatus;
					case DetailedAggregationStatus.CommunicationError:
						return Strings.CommunicationErrorWithDisabledStatus;
					case DetailedAggregationStatus.RemoteMailboxQuotaWarning:
						return Strings.RemoteMailboxQuotaWarningWithDisabledStatus;
					case DetailedAggregationStatus.LabsMailboxQuotaWarning:
						return Strings.LabsMailboxQuotaWarningWithDisabledStatus;
					case DetailedAggregationStatus.MaxedOutSyncRelationshipsError:
						return Strings.MaxedOutSyncRelationshipsErrorWithDisabledStatus;
					case DetailedAggregationStatus.LeaveOnServerNotSupported:
						return Strings.LeaveOnServerNotSupportedStatus;
					case DetailedAggregationStatus.RemoteAccountDoesNotExist:
						return Strings.RemoteAccountDoesNotExistStatus;
					case DetailedAggregationStatus.RemoteServerIsSlow:
					case DetailedAggregationStatus.RemoteServerIsBackedOff:
					case DetailedAggregationStatus.RemoteServerIsPoisonous:
						return Strings.RemoteServerIsSlowStatus;
					case DetailedAggregationStatus.TooManyFolders:
						return Strings.TooManyFoldersStatus;
					case DetailedAggregationStatus.SyncStateSizeError:
						return Strings.SyncStateSizeErrorStatus;
					case DetailedAggregationStatus.ConfigurationError:
						return Strings.ConfigurationErrorStatus;
					case DetailedAggregationStatus.RemoveSubscription:
						return Strings.RemoveSubscriptionStatus;
					}
					return Strings.DisabledStatus;
				case AggregationStatus.Poisonous:
					return Strings.PoisonStatus;
				case AggregationStatus.InvalidVersion:
					return Strings.InvalidVersionStatus;
				default:
					return LocalizedString.Empty;
				}
			}
		}

		public string DetailedStatus
		{
			get
			{
				switch (this.Status)
				{
				case AggregationStatus.Succeeded:
					return Strings.SuccessDetailedStatus;
				case AggregationStatus.InProgress:
					return Strings.InProgressDetailedStatus;
				case AggregationStatus.Delayed:
					switch (this.DetailedAggregationStatus)
					{
					case DetailedAggregationStatus.AuthenticationError:
						return Strings.AuthenticationErrorWithDelayedDetailedStatus;
					case DetailedAggregationStatus.ConnectionError:
						return this.GetConnectionErrorDetailedStatus();
					case DetailedAggregationStatus.CommunicationError:
						return Strings.CommunicationErrorWithDelayedDetailedStatus;
					case DetailedAggregationStatus.RemoteMailboxQuotaWarning:
						return Strings.RemoteMailboxQuotaWarningWithDelayedDetailedStatus;
					case DetailedAggregationStatus.LabsMailboxQuotaWarning:
						return Strings.LabsMailboxQuotaWarningWithDelayedDetailedStatus;
					case DetailedAggregationStatus.MaxedOutSyncRelationshipsError:
						return Strings.MaxedOutSyncRelationshipsErrorWithDelayedDetailedStatus;
					case DetailedAggregationStatus.LeaveOnServerNotSupported:
						return Strings.LeaveOnServerNotSupportedDetailedStatus;
					case DetailedAggregationStatus.RemoteAccountDoesNotExist:
						return Strings.RemoteAccountDoesNotExistDetailedStatus;
					case DetailedAggregationStatus.RemoteServerIsSlow:
					case DetailedAggregationStatus.RemoteServerIsBackedOff:
						return Strings.RemoteServerIsSlowDelayedDetailedStatus;
					case DetailedAggregationStatus.TooManyFolders:
						return Strings.TooManyFoldersDetailedStatus;
					}
					return this.GetDefaultDelayedStatus();
				case AggregationStatus.Disabled:
					switch (this.DetailedAggregationStatus)
					{
					case DetailedAggregationStatus.AuthenticationError:
						return Strings.AuthenticationErrorWithDisabledDetailedStatus;
					case DetailedAggregationStatus.ConnectionError:
						return this.GetConnectionErrorDetailedStatus();
					case DetailedAggregationStatus.CommunicationError:
						return Strings.CommunicationErrorWithDisabledDetailedStatus;
					case DetailedAggregationStatus.RemoteMailboxQuotaWarning:
						return Strings.RemoteMailboxQuotaWarningWithDisabledDetailedStatus;
					case DetailedAggregationStatus.LabsMailboxQuotaWarning:
						return Strings.LabsMailboxQuotaWarningWithDisabledDetailedStatus;
					case DetailedAggregationStatus.MaxedOutSyncRelationshipsError:
						return Strings.MaxedOutSyncRelationshipsErrorWithDisabledDetailedStatus;
					case DetailedAggregationStatus.LeaveOnServerNotSupported:
						return Strings.LeaveOnServerNotSupportedDetailedStatus;
					case DetailedAggregationStatus.RemoteAccountDoesNotExist:
						return Strings.RemoteAccountDoesNotExistDetailedStatus;
					case DetailedAggregationStatus.RemoteServerIsSlow:
					case DetailedAggregationStatus.RemoteServerIsBackedOff:
					case DetailedAggregationStatus.RemoteServerIsPoisonous:
						return Strings.RemoteServerIsSlowDisabledDetailedStatus;
					case DetailedAggregationStatus.TooManyFolders:
						return Strings.TooManyFoldersDetailedStatus;
					case DetailedAggregationStatus.SyncStateSizeError:
						return Strings.SyncStateSizeErrorDetailedStatus;
					case DetailedAggregationStatus.ProviderException:
						return Strings.ProviderExceptionDetailedStatus;
					}
					return Strings.DisabledDetailedStatus;
				case AggregationStatus.Poisonous:
					return Strings.PoisonDetailedStatus;
				case AggregationStatus.InvalidVersion:
					return Strings.InvalidVersionDetailedStatus;
				default:
					return LocalizedString.Empty;
				}
			}
		}

		public SendAsState SendAsState
		{
			get
			{
				return this.subscription.SendAsState;
			}
		}

		public VerificationEmailState VerificationEmailState
		{
			get
			{
				return this.subscription.VerificationEmailState;
			}
		}

		public string VerificationEmailMessageId
		{
			get
			{
				return this.subscription.VerificationEmailMessageId;
			}
		}

		public DateTime? VerificationEmailTimeStamp
		{
			get
			{
				return this.subscription.VerificationEmailTimeStamp;
			}
		}

		public ObjectId Identity
		{
			get
			{
				return this.subscription.SubscriptionIdentity;
			}
		}

		public virtual bool IsValid
		{
			get
			{
				return this.Validate().Length == 0 && this.subscription.IsValid;
			}
		}

		public virtual ObjectState ObjectState
		{
			get
			{
				return this.objectState;
			}
			set
			{
				this.objectState = value;
			}
		}

		public string Diagnostics
		{
			get
			{
				if (!this.debugOn)
				{
					return string.Empty;
				}
				return this.RedactIfNeeded(this.subscription.Diagnostics, true);
			}
		}

		public string PoisonCallstack
		{
			get
			{
				if (!this.debugOn)
				{
					return string.Empty;
				}
				return this.subscription.PoisonCallstack;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this.subscription.CreationTime;
			}
		}

		public DateTime AdjustedLastSuccessfulSyncTime
		{
			get
			{
				return this.subscription.AdjustedLastSuccessfulSyncTime;
			}
		}

		public string FoldersToExclude
		{
			get
			{
				return this.RedactIfNeeded(this.subscription.FoldersToExclude, true);
			}
		}

		public string OutageDetectionDiagnostics
		{
			get
			{
				return this.RedactIfNeeded(this.subscription.OutageDetectionDiagnostics, true);
			}
		}

		public long SubscriptionVersion
		{
			get
			{
				return this.subscription.Version;
			}
		}

		public Report Report
		{
			get
			{
				if (!this.NeedSuppressingPiiData)
				{
					return this.report;
				}
				return null;
			}
			set
			{
				this.report = value;
			}
		}

		public long TotalItemsSynced
		{
			get
			{
				return this.subscription.ItemsSynced;
			}
		}

		public long TotalItemsSkipped
		{
			get
			{
				return this.subscription.ItemsSkipped;
			}
		}

		public long? TotalItemsInSourceMailbox
		{
			get
			{
				return this.subscription.TotalItemsInSourceMailbox;
			}
		}

		public string TotalSizeOfSourceMailbox
		{
			get
			{
				if (this.subscription.TotalSizeOfSourceMailbox != null)
				{
					return new ByteQuantifiedSize(Convert.ToUInt64(this.subscription.TotalSizeOfSourceMailbox.Value)).ToString();
				}
				return null;
			}
		}

		public bool NeedSuppressingPiiData
		{
			get
			{
				return this.needSuppressingPiiData;
			}
			set
			{
				this.needSuppressingPiiData = value;
			}
		}

		internal PimAggregationSubscription Subscription
		{
			get
			{
				return this.subscription;
			}
			set
			{
				this.subscription = value;
			}
		}

		internal bool SendAsCheckNeeded
		{
			get
			{
				return this.sendAsCheckNeeded;
			}
			set
			{
				this.sendAsCheckNeeded = value;
			}
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return string.Empty;
		}

		public virtual ValidationError[] Validate()
		{
			ICollection<ValidationError> collection = PimSubscriptionValidator.Validate(this);
			ValidationError[] array = new ValidationError[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}

		public virtual void CopyChangesFrom(IConfigurable source)
		{
			PimSubscriptionProxy pimSubscriptionProxy = source as PimSubscriptionProxy;
			this.subscription = pimSubscriptionProxy.subscription;
			this.objectState = pimSubscriptionProxy.ObjectState;
		}

		public virtual void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		protected string RedactIfNeeded(string value, bool omit = false)
		{
			if (!this.NeedSuppressingPiiData)
			{
				return value;
			}
			if (!omit)
			{
				return SuppressingPiiData.Redact(value);
			}
			string text;
			string text2;
			return SuppressingPiiData.RedactWithoutHashing(value, out text, out text2);
		}

		protected SmtpAddress RedactIfNeeded(SmtpAddress value)
		{
			if (this.NeedSuppressingPiiData)
			{
				string text;
				string text2;
				return SuppressingPiiData.Redact(value, out text, out text2);
			}
			return value;
		}

		internal void SetDebug(bool debugOn)
		{
			this.debugOn = debugOn;
		}

		private string GetConnectionErrorDetailedStatus()
		{
			int num;
			int num2;
			SyncUtilities.GetHoursAndDaysWithoutSuccessfulSync(this.subscription, false, out num, out num2);
			return SyncUtilities.SelectTimeBasedString(num, num2, Strings.ConnectionErrorDetailedStatusDay(num), Strings.ConnectionErrorDetailedStatusDays(num), Strings.ConnectionErrorDetailedStatusHour(num2), Strings.ConnectionErrorDetailedStatusHours(num2), Strings.ConnectionErrorDetailedStatus);
		}

		private string GetDefaultDelayedStatus()
		{
			int num;
			int num2;
			SyncUtilities.GetHoursAndDaysWithoutSuccessfulSync(this.subscription, false, out num, out num2);
			return SyncUtilities.SelectTimeBasedString(num, num2, Strings.DelayedDetailedStatusDay(num), Strings.DelayedDetailedStatusDays(num), Strings.DelayedDetailedStatusHour(num2), Strings.DelayedDetailedStatusHours(num2), Strings.DelayedDetailedStatus);
		}

		private ObjectState objectState;

		private PimAggregationSubscription subscription;

		private bool debugOn;

		private bool sendAsCheckNeeded;

		private Report report;

		[NonSerialized]
		private bool needSuppressingPiiData;
	}
}
