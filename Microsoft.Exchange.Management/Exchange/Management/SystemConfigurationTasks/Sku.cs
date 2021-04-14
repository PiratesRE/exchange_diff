using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	internal class Sku
	{
		static Sku()
		{
			Sku.PKConfigurationFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ProductKeyConfig.xml");
			ExchangeBuild exchangeBuild = ExchangeObjectVersion.Exchange2012.ExchangeBuild;
			Sku.ValidServerVersion = new ServerVersion((int)exchangeBuild.Major, (int)exchangeBuild.Minor, (int)exchangeBuild.Build, (int)exchangeBuild.BuildRevision);
		}

		private Sku(string skuCode, ServerEditionType serverEdition, string name, InformationStoreSkuLimits informationStoreSkuLimits)
		{
			this.skuCode = skuCode;
			this.serverEdition = serverEdition;
			this.name = name;
			this.informationStoreSkuLimits = informationStoreSkuLimits;
		}

		public ServerEditionType ServerEdition
		{
			get
			{
				return this.serverEdition;
			}
		}

		public InformationStoreSkuLimits InformationStoreSkuLimits
		{
			get
			{
				return this.informationStoreSkuLimits;
			}
		}

		public static Sku GetSku(string productKey)
		{
			Sku result;
			string text;
			Exception ex;
			Sku.TryGenerateProductID(productKey, out result, out text, out ex);
			return result;
		}

		public bool IsValidVersion(int versionNumber)
		{
			ServerVersion serverVersion = new ServerVersion(versionNumber);
			return Sku.ValidServerVersion.Major == serverVersion.Major;
		}

		public string GenerateProductID(string productKey)
		{
			Sku sku;
			string result;
			Exception ex;
			if (!Sku.TryGenerateProductID(productKey, out sku, out result, out ex))
			{
				throw ex;
			}
			if (string.Compare(sku.skuCode, this.skuCode, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new InvalidProductKeyException();
			}
			return result;
		}

		private static bool TryGenerateProductID(string productKey, out Sku sku, out string pid, out Exception exception)
		{
			Sku[] array = new Sku[]
			{
				Sku.Ent64,
				Sku.Std64,
				Sku.Hybrid
			};
			sku = null;
			pid = null;
			exception = null;
			TaskLogger.Trace("-->GenerateProductId: {0}", new object[]
			{
				productKey
			});
			if (!File.Exists(Sku.PKConfigurationFile))
			{
				exception = new FileNotFoundException(Strings.CannotFindPKConfigFile(Sku.PKConfigurationFile));
				return false;
			}
			Sku.DIGITALPID2 pPID = new Sku.DIGITALPID2();
			Sku.DIGITALPID3 digitalpid = new Sku.DIGITALPID3();
			digitalpid.dwLength = (uint)Marshal.SizeOf(digitalpid);
			Sku.DIGITALPID4 digitalpid2 = new Sku.DIGITALPID4();
			digitalpid2.dwLength = (uint)Marshal.SizeOf(digitalpid2);
			uint num = Sku.PidGenX(productKey, Sku.PKConfigurationFile, "02064", null, pPID, digitalpid, digitalpid2);
			if (num == 0U)
			{
				string text = new string(digitalpid.szSku);
				char[] trimChars = new char[1];
				string strA = text.Trim(trimChars);
				foreach (Sku sku2 in array)
				{
					if (string.Compare(strA, sku2.skuCode, StringComparison.Ordinal) == 0)
					{
						sku = sku2;
						break;
					}
				}
				if (sku != null)
				{
					string text2 = new string(digitalpid.szPid2);
					char[] trimChars2 = new char[1];
					pid = text2.Trim(trimChars2);
					TaskLogger.Trace("<--GenerateProductId: {0}: {1} => valid PID: {2}", new object[]
					{
						sku.name,
						productKey,
						pid
					});
					return true;
				}
				TaskLogger.Trace("<--GenerateProductId: {0} => valid key, but for another SKU.", new object[]
				{
					productKey
				});
				exception = new InvalidProductKeyException();
				return false;
			}
			else
			{
				if (num == 2315321345U)
				{
					TaskLogger.Trace("<--GenerateProductId: {0} => invalid product key config file format. {1}", new object[]
					{
						productKey,
						num
					});
					exception = new InvalidPKConfigFormatException(Sku.PKConfigurationFile);
					return false;
				}
				TaskLogger.Trace("<--GenerateProductId: {0} => invalid product key. {1}", new object[]
				{
					productKey,
					num
				});
				exception = new InvalidProductKeyException();
				return false;
			}
		}

		[DllImport("pidgenX.dll")]
		private static extern uint PidGenX([MarshalAs(UnmanagedType.LPWStr)] string pwszProductKey, [MarshalAs(UnmanagedType.LPWStr)] string pwszConfig, [MarshalAs(UnmanagedType.LPWStr)] string pwszMpc, [MarshalAs(UnmanagedType.LPWStr)] string pwszOemId, [Out] Sku.DIGITALPID2 pPID2, [In] [Out] Sku.DIGITALPID3 pPID3, [In] [Out] Sku.DIGITALPID4 pPID4);

		private const string ProductCode = "02064";

		private const string Std64SkuCode = "X18-49499";

		private const string Ent64SkuCode = "X18-49498";

		private const string HybridSkuCode = "X19-07521";

		private static readonly string PKConfigurationFile;

		private static readonly ServerVersion ValidServerVersion;

		public static readonly Sku Std64 = new Sku("X18-49499", ServerEditionType.Standard, "Standard", InformationStoreSkuLimits.Standard);

		public static readonly Sku Ent64 = new Sku("X18-49498", ServerEditionType.Enterprise, "Enterprise", InformationStoreSkuLimits.Enterprise);

		public static readonly Sku Hybrid = new Sku("X19-07521", ServerEditionType.Coexistence, "Hybrid", InformationStoreSkuLimits.Coexistence);

		private readonly string skuCode;

		private readonly ServerEditionType serverEdition;

		private readonly string name;

		private readonly InformationStoreSkuLimits informationStoreSkuLimits;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
		private class DIGITALPID2
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
			public string szPid2;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		private class DIGITALPID3
		{
			public uint dwLength;

			public short wVersionMajor;

			public short wVersionMinor;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
			public char[] szPid2;

			public uint dwKeyIdx;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public char[] szSku;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] abCdKey;

			public uint dwCloneStatus;

			public uint dwTime;

			public uint dwRandom;

			public uint dwlt;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public uint[] adwLicenseData;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public char[] szOemId;

			public uint dwBundleId;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public char[] aszHardwareIdStatic;

			public uint dwHardwareIdTypeStatic;

			public uint dwBiosChecksumStatic;

			public uint dwVolSerStatic;

			public uint dwTotalRamStatic;

			public uint dwVideoBiosChecksumStatic;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public char[] aszHardwareIdDynamic;

			public uint dwHardwareIdTypeDynamic;

			public uint dwBiosChecksumDynamic;

			public uint dwVolSerDynamic;

			public uint dwTotalRamDynamic;

			public uint dwVideoBiosChecksumDynamic;

			public uint dwCrc32;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
		private class DIGITALPID4
		{
			public uint dwLength;

			public ushort wVersionMajor;

			public ushort wVersionMinor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szPid2Ex;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szSku;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
			public string szOemId;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szEditionId;

			public byte bIsUpgrade;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
			public byte[] abReserved;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			public byte[] abCdKey;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public byte[] abCdKeySHA256Hash;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
			public byte[] abSHA256Hash;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szPartNumber;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szProductKeyType;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szEulaType;
		}
	}
}
