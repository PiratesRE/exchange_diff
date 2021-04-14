using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Net
{
	[SuppressUnmanagedCodeSecurity]
	[ComVisible(false)]
	internal static class DnsNativeMethods
	{
		[DllImport("dnsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DnsNameCompare_W")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DnsNameCompare([In] string dnsName1, [In] string dnsName2);

		[DllImport("dnsapi.dll", CharSet = CharSet.Unicode)]
		public static extern DnsNativeMethods.WinDnsStatus DnsQueryConfig(DnsNativeMethods.DnsConfigType config, uint dnsFlag, [In] string adapterName, IntPtr reserved, [Out] byte[] configBuffer, [In] [Out] ref uint bufferLength);

		public static byte[] DnsQuestionToBuffer(bool udpRequest, string questionName, DnsRecordType recordType, ushort queryIdentifier, int recursionDesired)
		{
			byte[] array = new byte[1472];
			uint num = (uint)array.Length;
			if (!DnsNativeMethods.DnsWriteQuestionToBuffer(array, ref num, questionName, recordType, queryIdentifier, recursionDesired))
			{
				if ((ulong)num < (ulong)((long)array.Length))
				{
					return null;
				}
				array = new byte[num];
				if (!DnsNativeMethods.DnsWriteQuestionToBuffer(array, ref num, questionName, recordType, queryIdentifier, recursionDesired))
				{
					return null;
				}
			}
			int num2 = (int)num;
			byte[] array2;
			if (udpRequest)
			{
				array2 = new byte[num2];
				Buffer.BlockCopy(array, 0, array2, 0, num2);
			}
			else
			{
				array2 = new byte[num2 + 2];
				Buffer.BlockCopy(array, 0, array2, 2, num2);
				short num3 = IPAddress.HostToNetworkOrder((short)num);
				array2[0] = (byte)num3;
				array2[1] = (byte)(num3 >> 8);
			}
			return array2;
		}

		[DllImport("dnsapi.dll", CharSet = CharSet.Unicode)]
		public static extern void DnsRecordListFree([In] IntPtr ptrRecords, [In] FreeType freeType);

		[DllImport("dnsapi.dll", EntryPoint = "DnsExtractRecordsFromMessage_W")]
		public unsafe static extern DnsNativeMethods.WinDnsStatus DnsExtractRecordsFromMessage(byte* buffer, ushort messageLength, out IntPtr dnsRecords);

		[DllImport("dnsapi.dll", EntryPoint = "DnsWriteQuestionToBuffer_UTF8")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DnsWriteQuestionToBuffer([Out] byte[] dnsBuffer, [In] [Out] ref uint bufferSize, string hostName, DnsRecordType questionType, ushort xid, int recursionDesired);

		[DllImport("dnsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DnsValidateName_W")]
		internal static extern int ValidateName([In] string name, int format);

		private const string DNSAPI = "dnsapi.dll";

		public enum DnsConfigType
		{
			PrimaryDomainName,
			DnsServerList = 6,
			PrimaryHostNameRegistrationEnabled = 9,
			AdapterHostNameRegistrationEnabled,
			AddressRegistrationMaxCount,
			HostName,
			FullHostName = 15
		}

		public enum WinDnsStatus : uint
		{
			Success,
			ErrorInvalidName = 123U,
			InfoNoRecords = 9501U,
			ErrorBadPacket,
			ErrorNoPacket,
			ErrorRCode,
			ErrorServerFailure = 9002U,
			ErrorUnsecurePacket = 9505U,
			ErrorRCodeNameError = 9003U
		}
	}
}
