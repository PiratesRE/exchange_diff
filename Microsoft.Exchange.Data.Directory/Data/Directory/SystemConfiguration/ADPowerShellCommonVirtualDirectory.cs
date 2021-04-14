using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADPowerShellCommonVirtualDirectory : ADExchangeServiceVirtualDirectory
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADPowerShellCommonVirtualDirectory.MostDerivedClass;
			}
		}

		public bool? CertificateAuthentication { get; internal set; }

		public PowerShellVirtualDirectoryType VirtualDirectoryType
		{
			get
			{
				return (PowerShellVirtualDirectoryType)this[ADPowerShellCommonVirtualDirectorySchema.VirtualDirectoryType];
			}
			internal set
			{
				this[ADPowerShellCommonVirtualDirectorySchema.VirtualDirectoryType] = value;
			}
		}

		public static readonly string MostDerivedClass = "msExchPowerShellVirtualDirectory";
	}
}
