using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADComputerWrapper : ADObjectWrapperBase, IADComputer, IADObjectCommon
	{
		private ADComputerWrapper(ADComputer adComputer) : base(adComputer)
		{
			this.DnsHostName = adComputer.DnsHostName;
		}

		public static ADComputerWrapper CreateWrapper(ADComputer adComputer)
		{
			if (adComputer == null)
			{
				return null;
			}
			return new ADComputerWrapper(adComputer);
		}

		public string DnsHostName { get; private set; }
	}
}
