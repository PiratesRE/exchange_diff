using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class HybridConfiguration : ADConfigurationObject
	{
		internal static ADObjectId GetWellKnownLocation(ADObjectId orgContainerId)
		{
			return HybridConfiguration.GetWellKnownParentLocation(orgContainerId).GetDescendantId(HybridConfiguration.parentPath);
		}

		internal static ADObjectId GetWellKnownParentLocation(ADObjectId orgContainerId)
		{
			return orgContainerId.GetDescendantId(HybridConfiguration.parentPath);
		}

		internal static object FeaturesGetter(IPropertyBag propertyBag)
		{
			HybridFeatureFlags hybridFeaturesFlags = (HybridFeatureFlags)propertyBag[HybridConfigurationSchema.Flags];
			return HybridConfiguration.HybridFeaturesFlagsToHybridFeaturesPropertyValue(hybridFeaturesFlags);
		}

		internal static MultiValuedProperty<HybridFeature> HybridFeaturesFlagsToHybridFeaturesPropertyValue(HybridFeatureFlags hybridFeaturesFlags)
		{
			List<HybridFeature> list = new List<HybridFeature>(9);
			if ((hybridFeaturesFlags & HybridFeatureFlags.FreeBusy) == HybridFeatureFlags.FreeBusy)
			{
				list.Add(HybridFeature.FreeBusy);
			}
			if ((hybridFeaturesFlags & HybridFeatureFlags.MoveMailbox) == HybridFeatureFlags.MoveMailbox)
			{
				list.Add(HybridFeature.MoveMailbox);
			}
			if ((hybridFeaturesFlags & HybridFeatureFlags.Mailtips) == HybridFeatureFlags.Mailtips)
			{
				list.Add(HybridFeature.Mailtips);
			}
			if ((hybridFeaturesFlags & HybridFeatureFlags.MessageTracking) == HybridFeatureFlags.MessageTracking)
			{
				list.Add(HybridFeature.MessageTracking);
			}
			if ((hybridFeaturesFlags & HybridFeatureFlags.OwaRedirection) == HybridFeatureFlags.OwaRedirection)
			{
				list.Add(HybridFeature.OwaRedirection);
			}
			if ((hybridFeaturesFlags & HybridFeatureFlags.OnlineArchive) == HybridFeatureFlags.OnlineArchive)
			{
				list.Add(HybridFeature.OnlineArchive);
			}
			if ((hybridFeaturesFlags & HybridFeatureFlags.SecureMail) == HybridFeatureFlags.SecureMail)
			{
				list.Add(HybridFeature.SecureMail);
			}
			if ((hybridFeaturesFlags & HybridFeatureFlags.CentralizedTransportOnPrem) == HybridFeatureFlags.CentralizedTransportOnPrem)
			{
				list.Add(HybridFeature.CentralizedTransport);
			}
			if ((hybridFeaturesFlags & HybridFeatureFlags.Photos) == HybridFeatureFlags.Photos)
			{
				list.Add(HybridFeature.Photos);
			}
			return new MultiValuedProperty<HybridFeature>(list);
		}

		internal static void FeaturesSetter(object value, IPropertyBag propertyBag)
		{
			HybridFeatureFlags hybridFeatureFlags = HybridConfiguration.HybridFeaturePropertyValueToHybridFeatureFlags((MultiValuedProperty<HybridFeature>)value);
			propertyBag[HybridConfigurationSchema.Flags] = (int)hybridFeatureFlags;
		}

		internal static HybridFeatureFlags HybridFeaturePropertyValueToHybridFeatureFlags(MultiValuedProperty<HybridFeature> hybridFeatures)
		{
			HybridFeatureFlags hybridFeatureFlags = HybridFeatureFlags.None;
			if (hybridFeatures != null)
			{
				foreach (HybridFeature hybridFeature in hybridFeatures)
				{
					if (hybridFeature == HybridFeature.FreeBusy)
					{
						hybridFeatureFlags |= HybridFeatureFlags.FreeBusy;
					}
					else if (hybridFeature == HybridFeature.MoveMailbox)
					{
						hybridFeatureFlags |= HybridFeatureFlags.MoveMailbox;
					}
					else if (hybridFeature == HybridFeature.Mailtips)
					{
						hybridFeatureFlags |= HybridFeatureFlags.Mailtips;
					}
					else if (hybridFeature == HybridFeature.MessageTracking)
					{
						hybridFeatureFlags |= HybridFeatureFlags.MessageTracking;
					}
					else if (hybridFeature == HybridFeature.OwaRedirection)
					{
						hybridFeatureFlags |= HybridFeatureFlags.OwaRedirection;
					}
					else if (hybridFeature == HybridFeature.OnlineArchive)
					{
						hybridFeatureFlags |= HybridFeatureFlags.OnlineArchive;
					}
					else if (hybridFeature == HybridFeature.SecureMail)
					{
						hybridFeatureFlags |= HybridFeatureFlags.SecureMail;
					}
					else if (hybridFeature == HybridFeature.CentralizedTransport)
					{
						hybridFeatureFlags |= HybridFeatureFlags.CentralizedTransportOnPrem;
					}
					else
					{
						if (hybridFeature != HybridFeature.Photos)
						{
							throw new ArgumentOutOfRangeException("value");
						}
						hybridFeatureFlags |= HybridFeatureFlags.Photos;
					}
				}
			}
			return hybridFeatureFlags;
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return HybridConfiguration.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return HybridConfiguration.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return HybridConfiguration.parentPath;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public MultiValuedProperty<ADObjectId> ClientAccessServers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[HybridConfigurationSchema.ClientAccessServers];
			}
			set
			{
				this[HybridConfigurationSchema.ClientAccessServers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> EdgeTransportServers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[HybridConfigurationSchema.EdgeTransportServers];
			}
			set
			{
				this[HybridConfigurationSchema.EdgeTransportServers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ReceivingTransportServers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[HybridConfigurationSchema.ReceivingTransportServers];
			}
			set
			{
				this[HybridConfigurationSchema.ReceivingTransportServers] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> SendingTransportServers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[HybridConfigurationSchema.SendingTransportServers];
			}
			set
			{
				this[HybridConfigurationSchema.SendingTransportServers] = value;
			}
		}

		public SmtpDomain OnPremisesSmartHost
		{
			get
			{
				return (SmtpDomain)this[HybridConfigurationSchema.OnPremisesSmartHost];
			}
			set
			{
				this[HybridConfigurationSchema.OnPremisesSmartHost] = value;
			}
		}

		public MultiValuedProperty<AutoDiscoverSmtpDomain> Domains
		{
			get
			{
				return (MultiValuedProperty<AutoDiscoverSmtpDomain>)this[HybridConfigurationSchema.Domains];
			}
			set
			{
				this[HybridConfigurationSchema.Domains] = value;
			}
		}

		public MultiValuedProperty<HybridFeature> Features
		{
			get
			{
				return (MultiValuedProperty<HybridFeature>)this[HybridConfigurationSchema.Features];
			}
			set
			{
				this[HybridConfigurationSchema.Features] = value;
			}
		}

		internal bool FreeBusySharingEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.FreeBusySharingEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.FreeBusySharingEnabled] = value;
			}
		}

		internal bool MoveMailboxEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.MoveMailboxEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.MoveMailboxEnabled] = value;
			}
		}

		internal bool MailtipsEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.MailtipsEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.MailtipsEnabled] = value;
			}
		}

		internal bool MessageTrackingEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.MessageTrackingEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.MessageTrackingEnabled] = value;
			}
		}

		internal bool PhotosEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.PhotosEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.PhotosEnabled] = value;
			}
		}

		internal bool OwaRedirectionEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.OwaRedirectionEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.OwaRedirectionEnabled] = value;
			}
		}

		internal bool OnlineArchiveEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.OnlineArchiveEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.OnlineArchiveEnabled] = value;
			}
		}

		internal bool SecureMailEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.SecureMailEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.SecureMailEnabled] = value;
			}
		}

		internal bool CentralizedTransportOnPremEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.CentralizedTransportOnPremEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.CentralizedTransportOnPremEnabled] = value;
			}
		}

		internal bool CentralizedTransportInCloudEnabled
		{
			get
			{
				return (bool)this[HybridConfigurationSchema.CentralizedTransportInCloudEnabled];
			}
			set
			{
				this[HybridConfigurationSchema.CentralizedTransportInCloudEnabled] = value;
			}
		}

		internal HybridFeatureFlags FeatureFlags
		{
			get
			{
				return (HybridFeatureFlags)this[HybridConfigurationSchema.Flags];
			}
			set
			{
				this[HybridConfigurationSchema.Flags] = (int)value;
			}
		}

		public MultiValuedProperty<IPRange> ExternalIPAddresses
		{
			get
			{
				return (MultiValuedProperty<IPRange>)this[HybridConfigurationSchema.ExternalIPAddresses];
			}
			set
			{
				this[HybridConfigurationSchema.ExternalIPAddresses] = value;
			}
		}

		public SmtpX509Identifier TlsCertificateName
		{
			get
			{
				string text = this[HybridConfigurationSchema.TlsCertificateName] as string;
				if (!string.IsNullOrEmpty(text))
				{
					try
					{
						return SmtpX509Identifier.Parse(text);
					}
					catch
					{
					}
				}
				return null;
			}
			set
			{
				this[HybridConfigurationSchema.TlsCertificateName] = ((value == null) ? null : value.ToString());
			}
		}

		public int ServiceInstance
		{
			get
			{
				return (int)this[HybridConfigurationSchema.ServiceInstance];
			}
			set
			{
				this[HybridConfigurationSchema.ServiceInstance] = value;
			}
		}

		private static HybridConfigurationSchema schema = ObjectSchema.GetInstance<HybridConfigurationSchema>();

		private static string mostDerivedClass = "msExchCoexistenceRelationship";

		private static ADObjectId parentPath = new ADObjectId("CN=Hybrid Configuration");
	}
}
