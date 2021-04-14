using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADPowerShellVirtualDirectory : ADPowerShellCommonVirtualDirectory
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADPowerShellVirtualDirectory.schema;
			}
		}

		public bool? RequireSSL { get; internal set; }

		private static readonly ADPowerShellVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADPowerShellVirtualDirectorySchema>();
	}
}
