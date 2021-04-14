using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	[KnownType(typeof(Account))]
	public class Account : OrgPerson
	{
		public Account(Mailbox mailbox) : base(mailbox)
		{
			if (base.IsRoom)
			{
				this.UserNameLabel = OwaOptionStrings.RoomEmailAddressLabel;
				base.EmailAddress = base.PrimaryEmailAddress;
			}
			else
			{
				SmtpAddress windowsLiveID = mailbox.WindowsLiveID;
				if (SmtpAddress.Empty != windowsLiveID)
				{
					base.EmailAddress = windowsLiveID.ToString();
					this.UserName = windowsLiveID.Local;
					this.Domain = windowsLiveID.Domain;
					DomainCacheValue domainCacheValue = DomainCache.Singleton.Get(new SmtpDomainWithSubdomains(this.Domain), mailbox.OrganizationId);
					this.UserNameLabel = ((domainCacheValue != null && domainCacheValue.LiveIdInstanceType == LiveIdInstanceType.Business) ? OwaOptionStrings.UserNameMOSIDLabel : OwaOptionStrings.UserNameWLIDLabel);
				}
				else
				{
					this.UserNameLabel = OwaOptionStrings.UserLogonNameLabel;
					string userPrincipalName = mailbox.UserPrincipalName;
					base.EmailAddress = userPrincipalName;
					int num = userPrincipalName.IndexOf('@');
					if (num > 0)
					{
						this.UserName = userPrincipalName.Substring(0, num);
						this.Domain = userPrincipalName.Substring(num + 1);
					}
					else
					{
						this.UserName = null;
						this.Domain = null;
					}
				}
			}
			this.UserPhotoUrl = string.Format(CultureInfo.InvariantCulture, "~/Download.aspx?Identity={0}&handlerClass=UserPhotoDownloadHandler&preview=false", new object[]
			{
				base.PrimaryEmailAddress
			});
			this.userPhotoPreviewUrl = string.Format(CultureInfo.InvariantCulture, "~/Download.aspx?Identity={0}&handlerClass=UserPhotoDownloadHandler&preview=true", new object[]
			{
				base.PrimaryEmailAddress
			});
		}

		public MailboxStatistics Statistics { get; set; }

		public CalendarConfiguration CalendarConfiguration { get; set; }

		public Mailbox Mailbox
		{
			get
			{
				return (Mailbox)base.MailEnabledOrgPerson;
			}
		}

		public bool IsProfilePage { get; internal set; }

		public CASMailbox CasMailbox { get; set; }

		[DataMember]
		public string UserName { get; private set; }

		[DataMember]
		public string UserPhotoUrl
		{
			get
			{
				return this.userPhotoUrl;
			}
			private set
			{
				this.userPhotoUrl = value;
			}
		}

		[DataMember]
		public string UserPhotoPreviewUrl
		{
			get
			{
				return this.userPhotoPreviewUrl;
			}
			private set
			{
				this.userPhotoPreviewUrl = value;
			}
		}

		[DataMember]
		public string Domain { get; private set; }

		[DataMember]
		public string UserNameLabel { get; private set; }

		[DataMember]
		public Identity MailboxPlan
		{
			get
			{
				if (this.Mailbox == null || this.Mailbox.MailboxPlan == null)
				{
					return null;
				}
				return new Identity(this.Mailbox.MailboxPlan.ObjectGuid.ToString(), MailboxPlanResolverExtensions.ResolveMailboxPlan(this.Mailbox.MailboxPlan).DisplayName);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public Identity RoleAssignmentPolicy
		{
			get
			{
				if (this.Mailbox == null || this.Mailbox.RoleAssignmentPolicy == null)
				{
					return null;
				}
				return new Identity(this.Mailbox.RoleAssignmentPolicy.ObjectGuid.ToString(), this.Mailbox.RoleAssignmentPolicy.Name);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public Identity RetentionPolicy
		{
			get
			{
				if (this.Mailbox == null || this.Mailbox.RetentionPolicy == null)
				{
					return null;
				}
				return new Identity(this.Mailbox.RetentionPolicy.ObjectGuid.ToString(), this.Mailbox.RetentionPolicy.Name);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<string> AllowedSenders { get; set; }

		[DataMember]
		public IEnumerable<string> BlockedSenders { get; set; }

		[DataMember]
		public string AutomaticBooking
		{
			get
			{
				if (this.CalendarConfiguration == null)
				{
					return null;
				}
				if (this.CalendarConfiguration.AutomateProcessing == CalendarProcessingFlags.AutoAccept && this.CalendarConfiguration.AllBookInPolicy && !this.CalendarConfiguration.AllRequestInPolicy)
				{
					return true.ToJsonString(null);
				}
				if (this.CalendarConfiguration.AutomateProcessing == CalendarProcessingFlags.AutoAccept && !this.CalendarConfiguration.AllBookInPolicy && this.CalendarConfiguration.AllRequestInPolicy)
				{
					return false.ToJsonString(null);
				}
				return null;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> ResourceDelegates
		{
			get
			{
				if (this.CalendarConfiguration != null && this.CalendarConfiguration.ResourceDelegates != null)
				{
					return RecipientObjectResolver.Instance.ResolveObjects(this.CalendarConfiguration.ResourceDelegates);
				}
				return null;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public int? ResourceCapacity
		{
			get
			{
				if (this.Mailbox == null)
				{
					return null;
				}
				return this.Mailbox.ResourceCapacity;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<MailboxFeatureInfo> PhoneAndVoiceFeatures
		{
			get
			{
				return from feature in this.GetAllPhoneAndVoiceFeatures()
				where feature.Visible
				select feature;
			}
		}

		private IEnumerable<MailboxFeatureInfo> GetAllPhoneAndVoiceFeatures()
		{
			if (this.CasMailbox != null)
			{
				yield return new EASMailboxFeatureInfo(this.CasMailbox);
			}
			if (this.Mailbox != null && Util.IsMicrosoftHostedOnly)
			{
				yield return new UMMailboxFeatureInfo(this.Mailbox);
			}
			yield break;
		}

		private bool UseDatabaseQuota
		{
			get
			{
				return this.Mailbox.UseDatabaseQuotaDefaults != null && this.Mailbox.UseDatabaseQuotaDefaults.Value;
			}
		}

		private Unlimited<ByteQuantifiedSize> IssueWarningQuota
		{
			get
			{
				if (!this.UseDatabaseQuota)
				{
					return this.Mailbox.IssueWarningQuota;
				}
				return this.Statistics.DatabaseIssueWarningQuota;
			}
		}

		private Unlimited<ByteQuantifiedSize> ActualQuota
		{
			get
			{
				Unlimited<ByteQuantifiedSize> result = this.UseDatabaseQuota ? this.Statistics.DatabaseProhibitSendQuota : this.Mailbox.ProhibitSendQuota;
				if (!result.IsUnlimited)
				{
					return result;
				}
				if (!this.UseDatabaseQuota)
				{
					return this.Mailbox.ProhibitSendReceiveQuota;
				}
				return this.Statistics.DatabaseProhibitSendReceiveQuota;
			}
		}

		private uint UsagePercentage
		{
			get
			{
				if (this.Statistics == null || this.Mailbox == null)
				{
					return 0U;
				}
				double num = this.Statistics.TotalItemSize.Value.ToBytes();
				if ((uint)num == 0U)
				{
					return 0U;
				}
				if (this.ActualQuota.IsUnlimited)
				{
					return 3U;
				}
				return (uint)(num / this.ActualQuota.Value.ToBytes() * 100.0);
			}
		}

		private StatisticsBarState UsageState
		{
			get
			{
				if (this.Mailbox == null || this.Statistics == null)
				{
					return StatisticsBarState.Normal;
				}
				if (this.Statistics.StorageLimitStatus != null)
				{
					if (this.Statistics.StorageLimitStatus == StorageLimitStatus.ProhibitSend || this.Statistics.StorageLimitStatus == StorageLimitStatus.MailboxDisabled)
					{
						return StatisticsBarState.Exceeded;
					}
					if (this.Statistics.StorageLimitStatus == StorageLimitStatus.IssueWarning)
					{
						return StatisticsBarState.Warning;
					}
				}
				else
				{
					ulong num = this.Statistics.TotalItemSize.Value.ToBytes();
					if (!this.ActualQuota.IsUnlimited && num > this.ActualQuota.Value.ToBytes())
					{
						return StatisticsBarState.Exceeded;
					}
					if (!this.IssueWarningQuota.IsUnlimited && num > this.IssueWarningQuota.Value.ToBytes())
					{
						return StatisticsBarState.Warning;
					}
				}
				return StatisticsBarState.Normal;
			}
		}

		private string UsageText
		{
			get
			{
				if (this.Mailbox == null || this.Statistics == null)
				{
					return null;
				}
				return string.Format(OwaOptionStrings.MailboxUsageLegacyText, this.Statistics.TotalItemSize.ToAppropriateUnitFormatString());
			}
		}

		private string AdditionalInfoText
		{
			get
			{
				if (this.Mailbox == null || this.Statistics == null)
				{
					return OwaOptionStrings.MailboxUsageUnavailable;
				}
				if (this.ActualQuota.IsUnlimited)
				{
					return OwaOptionStrings.MailboxUsageUnlimitedText;
				}
				return string.Format((this.UsageState == StatisticsBarState.Exceeded) ? OwaOptionStrings.MailboxUsageExceededText : OwaOptionStrings.MailboxUsageWarningText, this.ActualQuota.ToAppropriateUnitFormatString());
			}
		}

		[DataMember]
		public StatisticsBarData MailboxUsage
		{
			get
			{
				if (this.IsProfilePage)
				{
					return new StatisticsBarData(this.UsagePercentage, this.UsageState, this.UsageText, this.AdditionalInfoText);
				}
				return new StatisticsBarData(this.UsagePercentage, this.UsageState, this.UsageText);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private string userPhotoUrl;

		private string userPhotoPreviewUrl;
	}
}
