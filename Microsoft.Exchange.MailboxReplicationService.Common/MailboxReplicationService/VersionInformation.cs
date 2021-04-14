using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class VersionInformation
	{
		[DataMember(IsRequired = true)]
		public int ProductMajor { get; set; }

		[DataMember(IsRequired = true)]
		public int ProductMinor { get; set; }

		[DataMember(IsRequired = true)]
		public int BuildMajor { get; set; }

		[DataMember(IsRequired = true)]
		public int BuildMinor { get; set; }

		[DataMember(IsRequired = true)]
		public byte[] SupportedCapabilities
		{
			get
			{
				byte[] array = new byte[(this.supportedCapabilities.Length + 7) / 8];
				this.supportedCapabilities.CopyTo(array, 0);
				return array;
			}
			set
			{
				this.supportedCapabilities = new BitArray(value);
			}
		}

		[DataMember]
		public string ComputerName { get; set; }

		static VersionInformation()
		{
			VersionInformation.mrsVersion[0] = true;
			VersionInformation.mrsVersion[1] = true;
			VersionInformation.mrsVersion[2] = true;
			VersionInformation.mrsVersion[3] = true;
			VersionInformation.mrsVersion[4] = true;
			VersionInformation.mrsVersion[5] = true;
			VersionInformation.mrsVersion[6] = true;
			VersionInformation.mrsVersion[7] = true;
			VersionInformation.mrsVersion[8] = true;
			VersionInformation.mrsVersion[9] = true;
			VersionInformation.mrsVersion[10] = true;
			VersionInformation.mrsVersion[11] = true;
			VersionInformation.mrsVersion[12] = true;
			VersionInformation.mrsProxyVersion[0] = true;
			VersionInformation.mrsProxyVersion[1] = true;
			VersionInformation.mrsProxyVersion[2] = true;
			VersionInformation.mrsProxyVersion[3] = true;
			VersionInformation.mrsProxyVersion[4] = true;
			VersionInformation.mrsProxyVersion[5] = true;
			VersionInformation.mrsProxyVersion[6] = true;
			VersionInformation.mrsProxyVersion[7] = true;
			VersionInformation.mrsProxyVersion[8] = true;
			VersionInformation.mrsProxyVersion[9] = true;
			VersionInformation.mrsProxyVersion[10] = true;
			VersionInformation.mrsProxyVersion[11] = true;
			VersionInformation.mrsProxyVersion[12] = true;
			VersionInformation.mrsProxyVersion[13] = true;
			VersionInformation.mrsProxyVersion[14] = true;
			VersionInformation.mrsProxyVersion[15] = true;
			VersionInformation.mrsProxyVersion[16] = true;
			VersionInformation.mrsProxyVersion[17] = true;
			VersionInformation.mrsProxyVersion[18] = true;
			VersionInformation.mrsProxyVersion[24] = true;
			VersionInformation.mrsProxyVersion[25] = true;
			VersionInformation.mrsProxyVersion[27] = true;
			VersionInformation.mrsProxyVersion[28] = false;
			VersionInformation.mrsProxyVersion[30] = true;
			VersionInformation.mrsProxyVersion[31] = true;
			VersionInformation.mrsProxyVersion[32] = true;
			VersionInformation.mrsProxyVersion[33] = true;
			VersionInformation.mrsProxyVersion[34] = true;
			VersionInformation.mrsProxyVersion[35] = true;
			VersionInformation.mrsProxyVersion[36] = true;
			VersionInformation.mrsProxyVersion[37] = true;
			VersionInformation.mrsProxyVersion[38] = true;
			VersionInformation.mrsProxyVersion[39] = true;
			VersionInformation.mrsProxyVersion[40] = true;
			VersionInformation.mrsProxyVersion[41] = true;
			VersionInformation.mrsProxyVersion[42] = true;
			VersionInformation.mrsProxyVersion[43] = true;
			VersionInformation.mrsProxyVersion[44] = true;
			VersionInformation.mrsProxyVersion[45] = true;
			VersionInformation.mrsProxyVersion[46] = true;
			VersionInformation.mrsProxyVersion[47] = true;
			VersionInformation.mrsProxyVersion[48] = true;
			VersionInformation.mrsProxyVersion[49] = true;
			VersionInformation.mrsProxyVersion[50] = true;
			VersionInformation.mrsProxyVersion[51] = true;
			VersionInformation.mrsProxyVersion[52] = true;
			VersionInformation.mrsProxyVersion[53] = true;
			VersionInformation.mrsProxyVersion[54] = true;
			VersionInformation.mrsProxyVersion[55] = false;
			VersionInformation.mrsProxyVersion[56] = true;
			VersionInformation.mrsProxyVersion[57] = true;
			VersionInformation.mrsProxyVersion[58] = true;
			VersionInformation.mrsProxyVersion[59] = true;
			VersionInformation.mrsProxyVersion[60] = true;
			VersionInformation.mrsProxyVersion[82] = true;
		}

		public VersionInformation(int maxSupportedCapabilities)
		{
			this.ProductMajor = VersionInformation.assemblyFileVersion.FileMajorPart;
			this.ProductMinor = VersionInformation.assemblyFileVersion.FileMinorPart;
			this.BuildMajor = VersionInformation.assemblyFileVersion.FileBuildPart;
			this.BuildMinor = VersionInformation.assemblyFileVersion.FilePrivatePart;
			this.supportedCapabilities = new BitArray(maxSupportedCapabilities);
			this.ComputerName = CommonUtils.LocalComputerName;
		}

		public static VersionInformation MRS
		{
			get
			{
				return VersionInformation.mrsVersion;
			}
		}

		public static VersionInformation MRSProxy
		{
			get
			{
				return VersionInformation.mrsProxyVersion;
			}
		}

		public ServerVersion ServerVersion
		{
			get
			{
				return new ServerVersion(this.ProductMajor, this.ProductMinor, this.BuildMajor, this.BuildMinor);
			}
		}

		public bool this[int index]
		{
			get
			{
				return index >= 0 && index < this.supportedCapabilities.Length && this.supportedCapabilities[index];
			}
			set
			{
				this.supportedCapabilities[index] = value;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = this.SupportedCapabilities.Length - 1; i >= 0; i--)
			{
				stringBuilder.AppendFormat("{0:X2}", this.SupportedCapabilities[i]);
			}
			return string.Format("{0}.{1}.{2}.{3} caps:{4}", new object[]
			{
				this.ProductMajor,
				this.ProductMinor,
				this.BuildMajor,
				this.BuildMinor,
				stringBuilder.ToString()
			});
		}

		private BitArray supportedCapabilities;

		private static readonly FileVersionInfo assemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

		private static readonly VersionInformation mrsVersion = new VersionInformation(13);

		private static readonly VersionInformation mrsProxyVersion = new VersionInformation(83);
	}
}
