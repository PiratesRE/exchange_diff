using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADDomain : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADDomain.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADDomain.MostDerivedClass;
			}
		}

		internal static object FqdnGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			return NativeHelpers.CanonicalNameFromDistinguishedName(adobjectId.DistinguishedName);
		}

		public string Fqdn
		{
			get
			{
				return (string)this[ADDomainSchema.Fqdn];
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this[ADDomainSchema.Sid];
			}
		}

		internal static object MaximumPasswordAgeGetter(IPropertyBag propertyBag)
		{
			long? num = (long?)propertyBag[ADDomainSchema.MaximumPasswordAgeRaw];
			if (num == null)
			{
				return null;
			}
			if (num.Value == -9223372036854775808L)
			{
				return EnhancedTimeSpan.Zero;
			}
			return new EnhancedTimeSpan?(EnhancedTimeSpan.FromTicks(-num.Value));
		}

		public long? MaximumPasswordAgeRaw
		{
			get
			{
				return (long?)this[ADDomainSchema.MaximumPasswordAgeRaw];
			}
		}

		public EnhancedTimeSpan? MaximumPasswordAge
		{
			get
			{
				return (EnhancedTimeSpan?)this[ADDomainSchema.MaximumPasswordAge];
			}
		}

		internal ReadOnlyCollection<ADServer> FindAllDomainControllers(bool includingRodc)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.Session.DomainController, base.Session.ReadOnly, base.Session.ConsistencyMode, base.Session.NetworkCredential, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(Datacenter.IsMicrosoftHostedOnly(true) ? base.Id.GetPartitionId() : PartitionId.LocalForest), 215, "FindAllDomainControllers", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADDomain.cs");
			topologyConfigurationSession.UseConfigNC = true;
			return topologyConfigurationSession.FindServerWithNtdsdsa(base.DistinguishedName, false, includingRodc);
		}

		internal ReadOnlyCollection<ADServer> FindAllDomainControllers()
		{
			return this.FindAllDomainControllers(true);
		}

		internal IEnumerable<ADServer> FindDomainControllersInSite(ADObjectId siteId)
		{
			foreach (ADServer adServer in this.FindAllDomainControllers())
			{
				if (adServer.Site.Equals(siteId))
				{
					yield return adServer;
				}
			}
			yield break;
		}

		private static ADDomainSchema schema = ObjectSchema.GetInstance<ADDomainSchema>();

		internal static string MostDerivedClass = "domainDNS";
	}
}
