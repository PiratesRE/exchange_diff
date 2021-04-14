using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADWebServicesVirtualDirectory : ADExchangeServiceVirtualDirectory
	{
		public bool? CertificateAuthentication { get; internal set; }

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADWebServicesVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADWebServicesVirtualDirectory.MostDerivedClass;
			}
		}

		public Uri InternalNLBBypassUrl
		{
			get
			{
				return (Uri)this[ADWebServicesVirtualDirectorySchema.InternalNLBBypassUrl];
			}
			internal set
			{
				this[ADWebServicesVirtualDirectorySchema.InternalNLBBypassUrl] = value;
			}
		}

		public GzipLevel GzipLevel
		{
			get
			{
				return (GzipLevel)this[ADWebServicesVirtualDirectorySchema.ADGzipLevel];
			}
			internal set
			{
				this[ADWebServicesVirtualDirectorySchema.ADGzipLevel] = value;
			}
		}

		public bool MRSProxyEnabled
		{
			get
			{
				return (bool)this[ADWebServicesVirtualDirectorySchema.MRSProxyEnabled];
			}
			internal set
			{
				this[ADWebServicesVirtualDirectorySchema.MRSProxyEnabled] = value;
			}
		}

		private static readonly ADWebServicesVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADWebServicesVirtualDirectorySchema>();

		public static readonly string MostDerivedClass = "msExchWebServicesVirtualDirectory";
	}
}
