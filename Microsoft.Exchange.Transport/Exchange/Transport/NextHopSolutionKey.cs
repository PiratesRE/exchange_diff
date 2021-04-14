using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport
{
	internal struct NextHopSolutionKey : IEquatable<NextHopSolutionKey>
	{
		public NextHopSolutionKey(DeliveryType deliveryType, string nextHopDomain, Guid nextHopConnector)
		{
			this = new NextHopSolutionKey(new NextHopType(deliveryType), nextHopDomain, nextHopConnector);
		}

		public NextHopSolutionKey(NextHopType nextHopType, string nextHopDomain, Guid nextHopConnector)
		{
			this.nextHopType = nextHopType;
			this.nextHopDomain = nextHopDomain.ToLowerInvariant();
			this.nextHopConnector = nextHopConnector;
			this.propertyBag = null;
		}

		public NextHopSolutionKey(DeliveryType deliveryType, string nextHopDomain, Guid nextHopConnector, string nextHopTlsDomain)
		{
			this = new NextHopSolutionKey(new NextHopType(deliveryType), nextHopDomain, nextHopConnector, nextHopTlsDomain);
		}

		public NextHopSolutionKey(NextHopType nextHopType, string nextHopDomain, Guid nextHopConnector, string nextHopTlsDomain)
		{
			this = new NextHopSolutionKey(nextHopType, nextHopDomain, nextHopConnector);
			if (!string.IsNullOrEmpty(nextHopTlsDomain))
			{
				this.SetProperty<string>(NextHopSolutionKeyObjectSchema.TlsDomain, nextHopTlsDomain);
			}
		}

		public NextHopSolutionKey(DeliveryType deliveryType, string nextHopDomain, Guid nextHopConnector, bool isLocalDeliveryGroupRelay)
		{
			this = new NextHopSolutionKey(deliveryType, nextHopDomain, nextHopConnector);
			if (isLocalDeliveryGroupRelay)
			{
				this.SetProperty<bool>(NextHopSolutionKeyObjectSchema.IsLocalDeliveryGroupRelay, isLocalDeliveryGroupRelay);
			}
		}

		public NextHopSolutionKey(DeliveryType deliveryType, string nextHopDomain, Guid nextHopConnector, RequiredTlsAuthLevel? requiredAuthLevel, string nextHopTlsDomain, string overrideSource)
		{
			this = new NextHopSolutionKey(deliveryType, nextHopDomain, nextHopConnector, nextHopTlsDomain);
			if (requiredAuthLevel != null)
			{
				this.SetProperty<RequiredTlsAuthLevel?>(NextHopSolutionKeyObjectSchema.TlsAuthLevel, requiredAuthLevel);
			}
			if (overrideSource != null)
			{
				this.SetProperty<string>(NextHopSolutionKeyObjectSchema.OverrideSource, overrideSource);
			}
		}

		public NextHopSolutionKey(DeliveryType deliveryType, string nextHopDomain, Guid nextHopConnector, RequiredTlsAuthLevel? requiredAuthLevel, string nextHopTlsDomain, RiskLevel riskLevel, int outboundIPPool, string overrideSource)
		{
			this = new NextHopSolutionKey(deliveryType, nextHopDomain, nextHopConnector, requiredAuthLevel, nextHopTlsDomain, overrideSource);
			if (riskLevel != RiskLevel.Normal)
			{
				this.SetProperty<int>(NextHopSolutionKeyObjectSchema.RiskLevel, (int)riskLevel);
			}
			if (outboundIPPool != 0)
			{
				this.SetProperty<int>(NextHopSolutionKeyObjectSchema.OutboundIPPool, outboundIPPool);
			}
		}

		public NextHopSolutionKey(RoutedMessageQueue queue)
		{
			this.nextHopType = queue.Key.NextHopType;
			this.nextHopDomain = queue.Key.NextHopDomain;
			this.nextHopConnector = queue.Key.NextHopConnector;
			this.propertyBag = null;
			if (!string.IsNullOrEmpty(queue.Key.NextHopTlsDomain))
			{
				this.SetProperty<string>(NextHopSolutionKeyObjectSchema.TlsDomain, queue.Key.NextHopTlsDomain);
			}
			if (queue.Key.IsLocalDeliveryGroupRelay)
			{
				this.SetProperty<bool>(NextHopSolutionKeyObjectSchema.IsLocalDeliveryGroupRelay, queue.Key.IsLocalDeliveryGroupRelay);
			}
		}

		public NextHopType NextHopType
		{
			get
			{
				return this.nextHopType;
			}
		}

		public string NextHopDomain
		{
			get
			{
				return this.nextHopDomain;
			}
		}

		public Guid NextHopConnector
		{
			get
			{
				return this.nextHopConnector;
			}
		}

		public string NextHopTlsDomain
		{
			get
			{
				return this.GetProperty<string>(NextHopSolutionKeyObjectSchema.TlsDomain);
			}
		}

		public bool IsLocalDeliveryGroupRelay
		{
			get
			{
				return this.GetProperty<bool>(NextHopSolutionKeyObjectSchema.IsLocalDeliveryGroupRelay);
			}
		}

		public RequiredTlsAuthLevel? TlsAuthLevel
		{
			get
			{
				return this.GetProperty<RequiredTlsAuthLevel?>(NextHopSolutionKeyObjectSchema.TlsAuthLevel);
			}
		}

		public RiskLevel RiskLevel
		{
			get
			{
				return (RiskLevel)this.GetProperty<int>(NextHopSolutionKeyObjectSchema.RiskLevel);
			}
		}

		public int OutboundIPPool
		{
			get
			{
				return this.GetProperty<int>(NextHopSolutionKeyObjectSchema.OutboundIPPool);
			}
		}

		public string OverrideSource
		{
			get
			{
				return this.GetProperty<string>(NextHopSolutionKeyObjectSchema.OverrideSource);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.nextHopType.Equals(NextHopType.Empty) && string.IsNullOrEmpty(this.nextHopDomain) && this.nextHopConnector == Guid.Empty && string.IsNullOrEmpty(this.NextHopTlsDomain);
			}
		}

		public static NextHopSolutionKey CreateShadowNextHopSolutionKey(string serverFqdn, NextHopSolutionKey other)
		{
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			Guid nextHopGuid = other.NextHopType.IsSmtpConnectorDeliveryType ? other.NextHopConnector : Guid.Empty;
			return NextHopSolutionKey.CreateShadowNextHopSolutionKey(serverFqdn, nextHopGuid, other.NextHopTlsDomain);
		}

		public static NextHopSolutionKey CreateShadowNextHopSolutionKey(string serverFqdn, Guid nextHopGuid)
		{
			if (string.IsNullOrEmpty("serverFqdn"))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			return NextHopSolutionKey.CreateShadowNextHopSolutionKey(serverFqdn, nextHopGuid, null);
		}

		public static NextHopSolutionKey CreateShadowNextHopSolutionKey(string serverFqdn, Guid nextHopGuid, string tlsDomain)
		{
			return new NextHopSolutionKey(NextHopType.ShadowRedundancy, serverFqdn, nextHopGuid, tlsDomain);
		}

		public override int GetHashCode()
		{
			return this.NextHopConnector.GetHashCode() ^ this.NextHopDomain.GetHashCode() ^ this.NextHopType.GetHashCode() ^ this.NextHopTlsDomain.GetHashCode() ^ this.IsLocalDeliveryGroupRelay.GetHashCode() ^ this.TlsAuthLevel.GetHashCode() ^ this.RiskLevel.GetHashCode() ^ this.OutboundIPPool.GetHashCode() ^ this.OverrideSource.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is NextHopSolutionKey && this.Equals((NextHopSolutionKey)obj);
		}

		public bool Equals(NextHopSolutionKey other)
		{
			return this.NextHopType.Equals(other.NextHopType) && this.NextHopConnector == other.NextHopConnector && this.NextHopDomain.Equals(other.NextHopDomain, StringComparison.Ordinal) && this.NextHopTlsDomain.Equals(other.NextHopTlsDomain) && this.IsLocalDeliveryGroupRelay == other.IsLocalDeliveryGroupRelay && this.TlsAuthLevel == other.TlsAuthLevel && this.RiskLevel == other.RiskLevel && this.OutboundIPPool == other.OutboundIPPool && this.OverrideSource.Equals(other.OverrideSource, StringComparison.Ordinal);
		}

		public override string ToString()
		{
			return string.Format("{0} ({1}) {2} {3} {4} {5} {6} {7} {8}", new object[]
			{
				this.nextHopType,
				this.nextHopDomain,
				this.nextHopConnector,
				this.NextHopTlsDomain,
				this.IsLocalDeliveryGroupRelay ? "local-relay" : string.Empty,
				this.TlsAuthLevel,
				this.RiskLevel,
				this.OutboundIPPool,
				this.OverrideSource
			});
		}

		public string ToShortString()
		{
			string text = string.Empty;
			if (this.TlsAuthLevel != null)
			{
				text = ((int)this.TlsAuthLevel.Value).ToString(CultureInfo.InvariantCulture);
			}
			string text2 = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				(int)this.NextHopType.NextHopCategory,
				(int)this.NextHopType.DeliveryType
			});
			return string.Format(CultureInfo.InvariantCulture, "({0}):{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8};", new object[]
			{
				this.nextHopDomain,
				this.NextHopTlsDomain,
				text2,
				this.nextHopConnector.ToString().Substring(0, 6),
				this.IsLocalDeliveryGroupRelay ? "1" : "0",
				text,
				(int)this.RiskLevel,
				this.OutboundIPPool,
				this.OverrideSource
			});
		}

		private void SetProperty<T>(ProviderPropertyDefinition property, T value)
		{
			if (this.propertyBag == null)
			{
				this.propertyBag = new NextHopSolutionKeyPropertyBag();
			}
			this.propertyBag[property] = value;
		}

		private T GetProperty<T>(ProviderPropertyDefinition property)
		{
			if (this.propertyBag != null)
			{
				return (T)((object)this.propertyBag[property]);
			}
			return (T)((object)property.DefaultValue);
		}

		private MultiValuedProperty<T> GetMultiValuedProperty<T>(ProviderPropertyDefinition property)
		{
			if (this.propertyBag != null)
			{
				return (MultiValuedProperty<T>)this.propertyBag[property];
			}
			return MultiValuedProperty<T>.Empty;
		}

		public static readonly NextHopSolutionKey Empty = new NextHopSolutionKey(NextHopType.Empty, string.Empty, Guid.Empty);

		public static readonly NextHopSolutionKey Submission = new NextHopSolutionKey(NextHopType.Empty, "Submission", Guid.Empty);

		public static readonly NextHopSolutionKey Unreachable = new NextHopSolutionKey(NextHopType.Unreachable, "Unreachable", Guid.Empty);

		private readonly NextHopType nextHopType;

		private readonly string nextHopDomain;

		private readonly Guid nextHopConnector;

		private NextHopSolutionKeyPropertyBag propertyBag;
	}
}
