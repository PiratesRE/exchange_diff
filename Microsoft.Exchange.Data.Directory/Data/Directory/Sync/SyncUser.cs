using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal class SyncUser : SyncOrgPerson
	{
		public SyncUser(SyncDirection syncDirection) : base(syncDirection)
		{
		}

		public override SyncObjectSchema Schema
		{
			get
			{
				return SyncUser.schema;
			}
		}

		internal override DirectoryObjectClass ObjectClass
		{
			get
			{
				return DirectoryObjectClass.User;
			}
		}

		protected override DirectoryObject CreateDirectoryObject()
		{
			return new User();
		}

		public SyncProperty<Guid> ArchiveGuid
		{
			get
			{
				return (SyncProperty<Guid>)base[SyncUserSchema.ArchiveGuid];
			}
			set
			{
				base[SyncUserSchema.ArchiveGuid] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> ArchiveName
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncUserSchema.ArchiveName];
			}
			set
			{
				base[SyncUserSchema.ArchiveName] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<AssignedPlanValue>> AssignedPlan
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<AssignedPlanValue>>)base[SyncUserSchema.AssignedPlan];
			}
			set
			{
				base[SyncUserSchema.AssignedPlan] = value;
			}
		}

		public byte[] CloudMsExchBlockedSendersHash
		{
			get
			{
				return (byte[])base[SyncUserSchema.CloudMsExchBlockedSendersHash];
			}
			set
			{
				base[SyncUserSchema.CloudMsExchBlockedSendersHash] = value;
			}
		}

		public byte[] CloudMsExchSafeRecipientsHash
		{
			get
			{
				return (byte[])base[SyncUserSchema.CloudMsExchSafeRecipientsHash];
			}
			set
			{
				base[SyncUserSchema.CloudMsExchSafeRecipientsHash] = value;
			}
		}

		public byte[] CloudMsExchSafeSendersHash
		{
			get
			{
				return (byte[])base[SyncUserSchema.CloudMsExchSafeSendersHash];
			}
			set
			{
				base[SyncUserSchema.CloudMsExchSafeSendersHash] = value;
			}
		}

		public SyncProperty<bool> DeliverToMailboxAndForward
		{
			get
			{
				return (SyncProperty<bool>)base[SyncUserSchema.DeliverToMailboxAndForward];
			}
			set
			{
				base[SyncUserSchema.DeliverToMailboxAndForward] = value;
			}
		}

		public SyncProperty<Guid> ExchangeGuid
		{
			get
			{
				return (SyncProperty<Guid>)base[SyncUserSchema.ExchangeGuid];
			}
			set
			{
				base[SyncUserSchema.ExchangeGuid] = value;
			}
		}

		public SyncProperty<SyncLink> Manager
		{
			get
			{
				return (SyncProperty<SyncLink>)base[SyncUserSchema.Manager];
			}
			set
			{
				base[SyncUserSchema.Manager] = value;
			}
		}

		public SyncProperty<NetID> NetID
		{
			get
			{
				return (SyncProperty<NetID>)base[SyncUserSchema.NetID];
			}
			set
			{
				base[SyncUserSchema.NetID] = value;
			}
		}

		public SyncProperty<byte[]> Picture
		{
			get
			{
				return (SyncProperty<byte[]>)base[SyncUserSchema.Picture];
			}
			set
			{
				base[SyncUserSchema.Picture] = value;
			}
		}

		public int RecipientSoftDeletedStatus
		{
			get
			{
				return (int)base[SyncUserSchema.RecipientSoftDeletedStatus];
			}
		}

		public SyncProperty<DateTime?> WhenSoftDeleted
		{
			get
			{
				return (SyncProperty<DateTime?>)base[SyncUserSchema.WhenSoftDeleted];
			}
			set
			{
				base[SyncUserSchema.WhenSoftDeleted] = value;
			}
		}

		public SyncProperty<DateTime> MSExchUserCreatedTimestamp
		{
			get
			{
				return (SyncProperty<DateTime>)base[SyncUserSchema.MSExchUserCreatedTimestamp];
			}
			set
			{
				base[SyncUserSchema.MSExchUserCreatedTimestamp] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<ProvisionedPlanValue>> ProvisionedPlan
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<ProvisionedPlanValue>>)base[SyncUserSchema.ProvisionedPlan];
			}
			set
			{
				base[SyncUserSchema.ProvisionedPlan] = value;
			}
		}

		public SyncProperty<int?> ResourceCapacity
		{
			get
			{
				return (SyncProperty<int?>)base[SyncUserSchema.ResourceCapacity];
			}
			set
			{
				base[SyncUserSchema.ResourceCapacity] = value;
			}
		}

		public SyncProperty<string> ResourcePropertiesDisplay
		{
			get
			{
				return (SyncProperty<string>)base[SyncUserSchema.ResourcePropertiesDisplay];
			}
			set
			{
				base[SyncUserSchema.ResourcePropertiesDisplay] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> ResourceMetaData
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncUserSchema.ResourceMetaData];
			}
			set
			{
				base[SyncUserSchema.ResourceMetaData] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> ResourceSearchProperties
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncUserSchema.ResourceSearchProperties];
			}
			set
			{
				base[SyncUserSchema.ResourceSearchProperties] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<ServiceInfoValue>> ServiceInfo
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<ServiceInfoValue>>)base[SyncUserSchema.ServiceInfo];
			}
			set
			{
				base[SyncUserSchema.ServiceInfo] = value;
			}
		}

		public SyncProperty<SmtpAddress> WindowsLiveID
		{
			get
			{
				return (SyncProperty<SmtpAddress>)base[SyncUserSchema.WindowsLiveID];
			}
			set
			{
				base[SyncUserSchema.WindowsLiveID] = value;
			}
		}

		public SyncProperty<RemoteRecipientType> RemoteRecipientType
		{
			get
			{
				return (SyncProperty<RemoteRecipientType>)base[SyncUserSchema.RemoteRecipientType];
			}
			set
			{
				base[SyncUserSchema.RemoteRecipientType] = value;
			}
		}

		public SyncProperty<CountryInfo> UsageLocation
		{
			get
			{
				return (SyncProperty<CountryInfo>)base[SyncUserSchema.UsageLocation];
			}
			set
			{
				base[SyncUserSchema.UsageLocation] = value;
			}
		}

		public SyncProperty<Capability> SKUCapability
		{
			get
			{
				return (SyncProperty<Capability>)base[SyncUserSchema.SKUCapability];
			}
		}

		public SyncProperty<MultiValuedProperty<Capability>> AddOnSKUCapability
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<Capability>>)base[SyncUserSchema.AddOnSKUCapability];
			}
		}

		public SyncProperty<AssignedCapabilityStatus?> SKUCapabilityStatus
		{
			get
			{
				return (SyncProperty<AssignedCapabilityStatus?>)base[SyncUserSchema.SKUCapabilityStatus];
			}
		}

		public SyncProperty<bool> SKUAssigned
		{
			get
			{
				return (SyncProperty<bool>)base[SyncUserSchema.SKUAssigned];
			}
		}

		public SyncProperty<DirectoryPropertyReferenceAddressList> SiteMailboxOwners
		{
			get
			{
				return (SyncProperty<DirectoryPropertyReferenceAddressList>)base[SyncUserSchema.SiteMailboxOwners];
			}
			set
			{
				base[SyncUserSchema.SiteMailboxOwners] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<SyncLink>> SiteMailboxUsers
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<SyncLink>>)base[SyncUserSchema.SiteMailboxUsers];
			}
			set
			{
				base[SyncUserSchema.SiteMailboxUsers] = value;
			}
		}

		public SyncProperty<DateTime?> SiteMailboxClosedTime
		{
			get
			{
				return (SyncProperty<DateTime?>)base[SyncUserSchema.SiteMailboxClosedTime];
			}
			set
			{
				base[SyncUserSchema.SiteMailboxClosedTime] = value;
			}
		}

		public SyncProperty<Uri> SharePointUrl
		{
			get
			{
				return (SyncProperty<Uri>)base[SyncUserSchema.SharePointUrl];
			}
			set
			{
				base[SyncUserSchema.SharePointUrl] = value;
			}
		}

		public SyncProperty<MultiValuedProperty<string>> InPlaceHoldsRaw
		{
			get
			{
				return (SyncProperty<MultiValuedProperty<string>>)base[SyncUserSchema.InPlaceHoldsRaw];
			}
			set
			{
				base[SyncUserSchema.InPlaceHoldsRaw] = value;
			}
		}

		public bool HasValidLicense
		{
			get
			{
				return this.AssignedPlan.HasValue && this.AssignedPlan.Value != null && this.AssignedPlan.Value.Count > 0 && this.SKUCapability.HasValue && this.SKUCapability.Value != Capability.None;
			}
		}

		public bool IsLicenseDeletion
		{
			get
			{
				return this.HasValidLicense && this.SKUCapabilityStatus.HasValue && this.SKUCapabilityStatus.Value == AssignedCapabilityStatus.Deleted;
			}
		}

		public bool HasRemoteMailboxRequest
		{
			get
			{
				return this.RemoteRecipientType.HasValue && ((this.RemoteRecipientType.Value & Microsoft.Exchange.Data.Directory.Recipient.RemoteRecipientType.ProvisionMailbox) == Microsoft.Exchange.Data.Directory.Recipient.RemoteRecipientType.ProvisionMailbox || (this.RemoteRecipientType.Value & Microsoft.Exchange.Data.Directory.Recipient.RemoteRecipientType.DeprovisionMailbox) == Microsoft.Exchange.Data.Directory.Recipient.RemoteRecipientType.DeprovisionMailbox);
			}
		}

		public SyncProperty<bool?> AccountEnabled
		{
			get
			{
				return (SyncProperty<bool?>)base[SyncUserSchema.AccountEnabled];
			}
			set
			{
				base[SyncUserSchema.AccountEnabled] = value;
			}
		}

		public SyncProperty<DateTime?> StsRefreshTokensValidFrom
		{
			get
			{
				return (SyncProperty<DateTime?>)base[SyncUserSchema.StsRefreshTokensValidFrom];
			}
			set
			{
				base[SyncUserSchema.AccountEnabled] = value;
			}
		}

		public static Capability GetExchangeCapability(XmlElement xmlCapability)
		{
			Capability result;
			try
			{
				result = (Capability)Enum.Parse(typeof(Capability), CapabilityHelper.TransformCapabilityString(xmlCapability.GetAttribute("MailboxPlan").Trim()), true);
			}
			catch (ArgumentException)
			{
				result = Capability.None;
			}
			return result;
		}

		internal static object SKUCapabilityGetter(IPropertyBag propertyBag)
		{
			Capability capability = Capability.None;
			AssignedPlanValue effectiveRootServicePlan = SyncUser.GetEffectiveRootServicePlan(propertyBag);
			if (effectiveRootServicePlan != null)
			{
				capability = SyncUser.GetExchangeCapability(effectiveRootServicePlan.Capability);
			}
			return capability;
		}

		internal static object AddOnSKUCapabilityGetter(IPropertyBag propertyBag)
		{
			return SyncUser.GetEffectiveAddOnSKUCapabilities(propertyBag);
		}

		internal static object SKUCapabilityStatusGetter(IPropertyBag propertyBag)
		{
			AssignedCapabilityStatus? assignedCapabilityStatus = null;
			AssignedPlanValue effectiveRootServicePlan = SyncUser.GetEffectiveRootServicePlan(propertyBag);
			if (effectiveRootServicePlan != null)
			{
				assignedCapabilityStatus = new AssignedCapabilityStatus?(effectiveRootServicePlan.CapabilityStatus);
			}
			return assignedCapabilityStatus;
		}

		internal static object SKUAssignedGetter(IPropertyBag propertyBag)
		{
			AssignedCapabilityStatus? assignedCapabilityStatus = (AssignedCapabilityStatus?)SyncUser.SKUCapabilityStatusGetter(propertyBag);
			return assignedCapabilityStatus != null && assignedCapabilityStatus == AssignedCapabilityStatus.Enabled;
		}

		protected override SyncPropertyDefinition[] MinimumForwardSyncProperties
		{
			get
			{
				List<SyncPropertyDefinition> list = base.MinimumForwardSyncProperties.ToList<SyncPropertyDefinition>();
				list.AddRange(new SyncPropertyDefinition[]
				{
					SyncUserSchema.WindowsLiveID,
					SyncUserSchema.NetID
				});
				return list.ToArray();
			}
		}

		private static AssignedPlanValue GetEffectiveRootServicePlan(IPropertyBag propertyBag)
		{
			AssignedPlanValue assignedPlanValue = null;
			MultiValuedProperty<AssignedPlanValue> multiValuedProperty = (MultiValuedProperty<AssignedPlanValue>)propertyBag[SyncUserSchema.AssignedPlan];
			if (multiValuedProperty != null && multiValuedProperty.Count != 0)
			{
				IOrderedEnumerable<AssignedPlanValue> source = from ap in multiValuedProperty
				orderby ap.AssignedTimestamp descending
				select ap;
				foreach (AssignedPlanValue assignedPlanValue2 in from ap in source
				where ap.CapabilityStatus != AssignedCapabilityStatus.Deleted
				select ap)
				{
					if (CapabilityHelper.IsRootSKUCapability(SyncUser.GetExchangeCapability(assignedPlanValue2.Capability)))
					{
						assignedPlanValue = assignedPlanValue2;
						break;
					}
				}
				if (assignedPlanValue == null)
				{
					foreach (AssignedPlanValue assignedPlanValue3 in from ap in source
					where ap.CapabilityStatus == AssignedCapabilityStatus.Deleted
					select ap)
					{
						if (CapabilityHelper.IsRootSKUCapability(SyncUser.GetExchangeCapability(assignedPlanValue3.Capability)))
						{
							assignedPlanValue = assignedPlanValue3;
							break;
						}
					}
				}
			}
			return assignedPlanValue;
		}

		private static MultiValuedProperty<Capability> GetEffectiveAddOnSKUCapabilities(IPropertyBag propertyBag)
		{
			MultiValuedProperty<AssignedPlanValue> multiValuedProperty = (MultiValuedProperty<AssignedPlanValue>)propertyBag[SyncUserSchema.AssignedPlan];
			MultiValuedProperty<Capability> multiValuedProperty2 = new MultiValuedProperty<Capability>();
			if (multiValuedProperty != null && multiValuedProperty.Count != 0)
			{
				foreach (AssignedPlanValue assignedPlanValue in multiValuedProperty)
				{
					Capability exchangeCapability = SyncUser.GetExchangeCapability(assignedPlanValue.Capability);
					if (CapabilityHelper.IsAddOnSKUCapability(exchangeCapability) && assignedPlanValue.CapabilityStatus != AssignedCapabilityStatus.Deleted)
					{
						multiValuedProperty2.Add(exchangeCapability);
					}
				}
			}
			return multiValuedProperty2;
		}

		private static readonly SyncUserSchema schema = ObjectSchema.GetInstance<SyncUserSchema>();
	}
}
