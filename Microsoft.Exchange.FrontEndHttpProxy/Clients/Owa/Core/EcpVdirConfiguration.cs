using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class EcpVdirConfiguration : VdirConfiguration
	{
		private EcpVdirConfiguration(ADEcpVirtualDirectory ecpVirtualDirectory) : base(ecpVirtualDirectory)
		{
		}

		public new static EcpVdirConfiguration Instance
		{
			get
			{
				return VdirConfiguration.Instance as EcpVdirConfiguration;
			}
		}

		internal static EcpVdirConfiguration CreateInstance(ITopologyConfigurationSession session, ADObjectId virtualDirectoryDN)
		{
			ADEcpVirtualDirectory adecpVirtualDirectory = null;
			ADEcpVirtualDirectory[] array = session.Find<ADEcpVirtualDirectory>(virtualDirectoryDN, QueryScope.Base, null, null, 1);
			if (array != null && array.Length == 1)
			{
				adecpVirtualDirectory = array[0];
			}
			if (adecpVirtualDirectory == null)
			{
				throw new ADNoSuchObjectException(LocalizedString.Empty);
			}
			return new EcpVdirConfiguration(adecpVirtualDirectory);
		}
	}
}
