using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "ServerVersionInfo", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	[Serializable]
	public class ServerVersionInfo
	{
		public ServerVersionInfo()
		{
			this.Version = ExchangeVersion.Exchange2013_SP1.ToString();
		}

		private ServerVersionInfo(FileVersionInfo version) : this()
		{
			this.MajorVersion = version.FileMajorPart;
			this.MinorVersion = version.FileMinorPart;
			this.MajorBuildNumber = version.FileBuildPart;
			this.MinorBuildNumber = version.FilePrivatePart;
		}

		[DataMember(Name = "MajorVersion", IsRequired = false, Order = 1)]
		public int MajorVersion { get; set; }

		[DataMember(Name = "MinorVersion", IsRequired = false, Order = 2)]
		public int MinorVersion { get; set; }

		[DataMember(Name = "MajorBuildNumber", IsRequired = false, Order = 3)]
		public int MajorBuildNumber { get; set; }

		[DataMember(Name = "MinorBuildNumber", IsRequired = false, Order = 4)]
		public int MinorBuildNumber { get; set; }

		[DataMember(Name = "Version", IsRequired = false, Order = 5)]
		public string Version { get; set; }

		private const ExchangeVersion CurrentExchangeVersion = ExchangeVersion.Exchange2013_SP1;

		internal static LazyMember<ServerVersionInfo> CurrentVersion = new LazyMember<ServerVersionInfo>(() => new ServerVersionInfo(Common.ServerVersion));
	}
}
