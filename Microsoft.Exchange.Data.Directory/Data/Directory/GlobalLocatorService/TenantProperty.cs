using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class TenantProperty : GlsProperty
	{
		static TenantProperty()
		{
			TenantProperty.properties.Add(TenantProperty.EXOResourceForest.Name, TenantProperty.EXOResourceForest);
			TenantProperty.properties.Add(TenantProperty.EXOAccountForest.Name, TenantProperty.EXOAccountForest);
			TenantProperty.properties.Add(TenantProperty.EXOPrimarySite.Name, TenantProperty.EXOPrimarySite);
			TenantProperty.properties.Add(TenantProperty.EXOSmtpNextHopDomain.Name, TenantProperty.EXOSmtpNextHopDomain);
			TenantProperty.properties.Add(TenantProperty.EXOTenantFlags.Name, TenantProperty.EXOTenantFlags);
			TenantProperty.properties.Add(TenantProperty.EXOTenantContainerCN.Name, TenantProperty.EXOTenantContainerCN);
			TenantProperty.properties.Add(TenantProperty.CustomerType.Name, TenantProperty.CustomerType);
			TenantProperty.properties.Add(TenantProperty.Version.Name, TenantProperty.Version);
			TenantProperty.properties.Add(TenantProperty.Region.Name, TenantProperty.Region);
			TenantProperty.properties.Add(TenantProperty.GlobalResumeCache.Name, TenantProperty.GlobalResumeCache);
		}

		protected TenantProperty(string name, Type dataType) : base(name, dataType, null)
		{
		}

		protected TenantProperty(string name, Type dataType, object defaultValue) : base(name, dataType, defaultValue)
		{
		}

		internal static TenantProperty Get(string name)
		{
			return TenantProperty.properties[name];
		}

		internal static readonly TenantProperty EXOResourceForest = new TenantProperty(GlsProperty.ExoPrefix + ".ResourceForest", typeof(string));

		internal static readonly TenantProperty EXOAccountForest = new TenantProperty(GlsProperty.ExoPrefix + ".AccountForest", typeof(string));

		internal static readonly TenantProperty EXOPrimarySite = new TenantProperty(GlsProperty.ExoPrefix + ".PrimarySite", typeof(string));

		internal static readonly TenantProperty EXOSmtpNextHopDomain = new TenantProperty(GlsProperty.ExoPrefix + ".SmtpNextHopDomain", typeof(string));

		internal static readonly TenantProperty EXOTenantFlags = new TenantProperty(GlsProperty.ExoPrefix + ".TenantFlags", typeof(int));

		internal static readonly TenantProperty EXOTenantContainerCN = new TenantProperty(GlsProperty.ExoPrefix + ".TenantContainerCN", typeof(string));

		internal static readonly TenantProperty CustomerType = new TenantProperty(GlsProperty.FfoPrefix + ".CustomerType", typeof(int), -1);

		internal static readonly TenantProperty Version = new TenantProperty(GlsProperty.FfoPrefix + ".Version", typeof(string));

		internal static readonly TenantProperty Region = new TenantProperty(GlsProperty.FfoPrefix + ".Region", typeof(string));

		internal static readonly TenantProperty GlobalResumeCache = new TenantProperty(GlsProperty.GlobalPrefix + ".ResumeCache", typeof(string));

		private static IDictionary<string, TenantProperty> properties = new Dictionary<string, TenantProperty>();
	}
}
