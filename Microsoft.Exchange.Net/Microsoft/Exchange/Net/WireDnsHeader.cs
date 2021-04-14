using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Net
{
	internal struct WireDnsHeader
	{
		public int XId
		{
			get
			{
				return (int)this.xid;
			}
		}

		public int Questions
		{
			get
			{
				return (int)this.questionCount;
			}
		}

		public int Answers
		{
			get
			{
				return (int)this.answerCount;
			}
		}

		public int AuthorityRecords
		{
			get
			{
				return (int)this.nameServerCount;
			}
		}

		public int AdditionalRecords
		{
			get
			{
				return (int)this.additionalCount;
			}
		}

		public bool IsResponse
		{
			get
			{
				return (byte)(this.byteFlags1 & WireDnsHeader.ByteMask1.IsResponse) == 128;
			}
		}

		public bool IsTruncated
		{
			get
			{
				return (byte)(this.byteFlags1 & WireDnsHeader.ByteMask1.Truncation) == 2;
			}
		}

		public bool IsAuthoritative
		{
			get
			{
				return (byte)(this.byteFlags1 & WireDnsHeader.ByteMask1.Authoritative) == 4;
			}
		}

		public WireDnsHeader.Response ResponseCode
		{
			get
			{
				return (WireDnsHeader.Response)(this.byteFlags2 & WireDnsHeader.ByteMask2.ResponseCode);
			}
		}

		public bool IsRecursionAvailable
		{
			get
			{
				return (byte)(this.byteFlags2 & WireDnsHeader.ByteMask2.RecursionAvailable) == 128;
			}
		}

		public bool WasRecursionDesired
		{
			get
			{
				return (byte)(this.byteFlags1 & WireDnsHeader.ByteMask1.RecursionDesired) == 1;
			}
		}

		public static WireDnsHeader NetworkToHostOrder(byte[] buffer, int offset)
		{
			WireDnsHeader result = default(WireDnsHeader);
			result.xid = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(buffer, offset));
			int num = offset + 2;
			result.byteFlags1 = (WireDnsHeader.ByteMask1)buffer[num];
			num++;
			result.byteFlags2 = (WireDnsHeader.ByteMask2)buffer[num];
			num++;
			result.questionCount = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(buffer, num));
			num += 2;
			result.answerCount = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(buffer, num));
			num += 2;
			result.nameServerCount = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(buffer, num));
			num += 2;
			result.additionalCount = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(buffer, num));
			buffer[offset] = (byte)result.xid;
			buffer[offset + 1] = (byte)(result.xid >> 8);
			buffer[offset + 4] = (byte)result.questionCount;
			buffer[offset + 5] = (byte)(result.questionCount >> 8);
			buffer[offset + 6] = (byte)result.answerCount;
			buffer[offset + 7] = (byte)(result.answerCount >> 8);
			buffer[offset + 8] = (byte)result.nameServerCount;
			buffer[offset + 9] = (byte)(result.nameServerCount >> 8);
			buffer[offset + 10] = (byte)result.additionalCount;
			buffer[offset + 11] = (byte)(result.additionalCount >> 8);
			return result;
		}

		public static readonly int MarshalSize = Marshal.SizeOf(typeof(WireDnsHeader));

		public ushort xid;

		private WireDnsHeader.ByteMask1 byteFlags1;

		private WireDnsHeader.ByteMask2 byteFlags2;

		private ushort questionCount;

		private ushort answerCount;

		private ushort nameServerCount;

		private ushort additionalCount;

		public enum Response : byte
		{
			NoError,
			FormatError,
			ServerFailure,
			NameError,
			NotImplemented,
			Refused
		}

		[Flags]
		private enum ByteMask1 : byte
		{
			RecursionDesired = 1,
			Truncation = 2,
			Authoritative = 4,
			IsResponse = 128
		}

		[Flags]
		private enum ByteMask2 : byte
		{
			ResponseCode = 15,
			Broadcast = 16,
			RecursionAvailable = 128
		}
	}
}
