using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADServer : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADServer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADServer.mostDerivedClass;
			}
		}

		public string DnsHostName
		{
			get
			{
				return (string)this[ADServerSchema.DnsHostName];
			}
			internal set
			{
				this[ADServerSchema.DnsHostName] = value;
			}
		}

		public ADObjectId Site
		{
			get
			{
				if (base.Id != null)
				{
					return base.Id.Parent.Parent;
				}
				return null;
			}
		}

		public bool IsInLocalSite
		{
			get
			{
				if (this.isInLocalSite == null)
				{
					string siteName = NativeHelpers.GetSiteName(false);
					this.isInLocalSite = new bool?(!string.IsNullOrEmpty(siteName) && this.Site != null && this.Site.Name.Equals(siteName, StringComparison.OrdinalIgnoreCase));
				}
				return this.isInLocalSite.Value;
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return (SecurityIdentifier)this[ADServerSchema.Sid];
			}
		}

		public ADObjectId ServerReference
		{
			get
			{
				return (ADObjectId)this[ADServerSchema.ServerReference];
			}
		}

		public ADObjectId DomainId
		{
			get
			{
				if (this.ServerReference != null)
				{
					return this.ServerReference.DomainId;
				}
				return null;
			}
		}

		internal bool IsAvailable()
		{
			ExTraceGlobals.ADTopologyTracer.TraceDebug<string>((long)this.GetHashCode(), "Checking {0} for availability", this.DnsHostName);
			if (base.Session == null)
			{
				return false;
			}
			bool result;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DnsHostName, true, ConsistencyMode.FullyConsistent, base.Session.NetworkCredential, ADSessionSettings.FromRootOrgScopeSet(), 147, "IsAvailable", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADServer.cs");
				topologyConfigurationSession.UseConfigNC = false;
				topologyConfigurationSession.UseGlobalCatalog = false;
				if (topologyConfigurationSession.FindComputerByHostName(this.DnsHostName) == null)
				{
					ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)this.GetHashCode(), "FindComputerByHostName returned null for dns {0}", this.DnsHostName);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (SuitabilityException ex)
			{
				ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)this.GetHashCode(), "Server unavailable because of a caught exception: {0}", ex.Message);
				result = false;
			}
			catch (ADTransientException ex2)
			{
				ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)this.GetHashCode(), "Server unavailable because of a caught exception: {0}", ex2.Message);
				result = false;
			}
			catch (DataValidationException ex3)
			{
				ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)this.GetHashCode(), "Server unavailable because of a caught exception: {0}", ex3.Message);
				result = false;
			}
			catch (ADExternalException ex4)
			{
				ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)this.GetHashCode(), "Server unavailable because of a caught exception: {0}", ex4.Message);
				result = false;
			}
			catch (ADServerNotSuitableException ex5)
			{
				ExTraceGlobals.ADTopologyTracer.TraceError<string>((long)this.GetHashCode(), "Server unavailable because of a caught exception: {0}", ex5.Message);
				result = false;
			}
			return result;
		}

		private static ADServerSchema schema = ObjectSchema.GetInstance<ADServerSchema>();

		private static string mostDerivedClass = "server";

		private bool? isInLocalSite = null;
	}
}
