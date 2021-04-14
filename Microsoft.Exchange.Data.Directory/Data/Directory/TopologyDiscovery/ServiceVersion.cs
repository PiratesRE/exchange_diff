using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[DataContract]
	internal class ServiceVersion : IExtensibleDataObject
	{
		public ServiceVersion(int major, int minor, int build, int revision)
		{
			this.Major = major;
			this.Minor = minor;
			this.Build = build;
			this.Revision = revision;
		}

		public static ServiceVersion Current
		{
			get
			{
				if (ServiceVersion.current == null)
				{
					ServiceVersion.current = new ServiceVersion(ServerVersion.InstalledVersion.Major, ServerVersion.InstalledVersion.Minor, ServerVersion.InstalledVersion.Build, ServerVersion.InstalledVersion.Revision);
				}
				return ServiceVersion.current;
			}
		}

		[DataMember(IsRequired = true)]
		public int Major { get; set; }

		[DataMember(IsRequired = true)]
		public int Minor { get; set; }

		[DataMember(IsRequired = true)]
		public int Build { get; set; }

		[DataMember(IsRequired = true)]
		public int Revision { get; set; }

		public ExtensionDataObject ExtensionData { get; set; }

		public ExchangeBuild ToExchangeBuild()
		{
			return new ExchangeBuild((byte)this.Major, (byte)this.Minor, (ushort)((byte)this.Build), (ushort)((byte)this.Revision));
		}

		public override string ToString()
		{
			return string.Format("{0}.{1} (Build {2}.{3})", new object[]
			{
				this.Major,
				this.Minor,
				this.Build,
				this.Revision
			});
		}

		private static ServiceVersion current;
	}
}
