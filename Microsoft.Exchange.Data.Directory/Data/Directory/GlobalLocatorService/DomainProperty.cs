using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class DomainProperty : GlsProperty
	{
		static DomainProperty()
		{
			DomainProperty.properties.Add(DomainProperty.region.Name, DomainProperty.region);
			DomainProperty.properties.Add(DomainProperty.serviceVersion.Name, DomainProperty.serviceVersion);
			DomainProperty.properties.Add(DomainProperty.domainInUse.Name, DomainProperty.domainInUse);
			DomainProperty.properties.Add(DomainProperty.ExoFlags.Name, DomainProperty.ExoFlags);
			DomainProperty.properties.Add(DomainProperty.ExoDomainInUse.Name, DomainProperty.ExoDomainInUse);
			DomainProperty.properties.Add(DomainProperty.ipv6.Name, DomainProperty.ipv6);
		}

		protected DomainProperty(string name, Type dataType) : base(name, dataType, null)
		{
		}

		protected DomainProperty(string name, Type dataType, object defaultValue) : base(name, dataType, defaultValue)
		{
		}

		internal static DomainProperty Get(string name)
		{
			return DomainProperty.properties[name];
		}

		internal static DomainProperty Region
		{
			get
			{
				return DomainProperty.region;
			}
		}

		internal static DomainProperty ServiceVersion
		{
			get
			{
				return DomainProperty.serviceVersion;
			}
		}

		internal static DomainProperty DomainInUse
		{
			get
			{
				return DomainProperty.domainInUse;
			}
		}

		internal static DomainProperty IPv6
		{
			get
			{
				return DomainProperty.ipv6;
			}
		}

		private static readonly DomainProperty region = new DomainProperty(GlsProperty.FfoPrefix + ".Region", typeof(string));

		private static readonly DomainProperty serviceVersion = new DomainProperty(GlsProperty.FfoPrefix + ".Version", typeof(string));

		private static readonly DomainProperty domainInUse = new DomainProperty(GlsProperty.FfoPrefix + ".DomainInUse", typeof(bool));

		private static readonly DomainProperty ipv6 = new DomainProperty(GlsProperty.FfoPrefix + ".IPv6", typeof(int), 0);

		internal static readonly DomainProperty ExoFlags = new DomainProperty(GlsProperty.ExoPrefix + ".Flags", typeof(int));

		internal static readonly DomainProperty ExoDomainInUse = new DomainProperty(GlsProperty.ExoPrefix + ".DomainInUse", typeof(bool));

		private static IDictionary<string, DomainProperty> properties = new Dictionary<string, DomainProperty>();
	}
}
