using System;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public static class PropertyValueHelpers
	{
		public static byte[] FormatSdForTransfer(SecurityDescriptor securityDescriptor)
		{
			if (securityDescriptor.BinaryForm == null)
			{
				return null;
			}
			byte[] array = new byte[PropertyValueHelpers.MapiSdHeader.Length + securityDescriptor.BinaryForm.Length];
			Array.Copy(PropertyValueHelpers.MapiSdHeader, 0, array, 0, PropertyValueHelpers.MapiSdHeader.Length);
			Array.Copy(securityDescriptor.BinaryForm, 0, array, PropertyValueHelpers.MapiSdHeader.Length, securityDescriptor.BinaryForm.Length);
			return array;
		}

		public static byte[] FormatSdForTransfer(ArraySegment<byte> securityDescriptorSegment)
		{
			byte[] array = new byte[PropertyValueHelpers.MapiSdHeader.Length + securityDescriptorSegment.Count];
			Array.Copy(PropertyValueHelpers.MapiSdHeader, 0, array, 0, PropertyValueHelpers.MapiSdHeader.Length);
			Array.Copy(securityDescriptorSegment.Array, securityDescriptorSegment.Offset, array, PropertyValueHelpers.MapiSdHeader.Length, securityDescriptorSegment.Count);
			return array;
		}

		public static SecurityDescriptor UnformatSdFromTransfer(byte[] transferredSDBytes)
		{
			ArraySegment<byte> arraySegment = PropertyValueHelpers.UnformatSdSegmentFromTransfer(transferredSDBytes);
			byte[] array = new byte[arraySegment.Count];
			Array.Copy(arraySegment.Array, arraySegment.Offset, array, 0, arraySegment.Count);
			return new SecurityDescriptor(array);
		}

		public static ArraySegment<byte> UnformatSdSegmentFromTransfer(byte[] transferredSDBytes)
		{
			int num = 0;
			ParseSerialize.GetWord(transferredSDBytes, ref num, transferredSDBytes.Length);
			num += 2;
			ushort word = ParseSerialize.GetWord(transferredSDBytes, ref num, transferredSDBytes.Length);
			num += 2;
			switch (word)
			{
			case 3:
				num += 4;
				break;
			}
			return new ArraySegment<byte>(transferredSDBytes, num, transferredSDBytes.Length - num);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static PropertyValueHelpers()
		{
			byte[] array = new byte[8];
			array[0] = 8;
			array[2] = 3;
			PropertyValueHelpers.MapiSdHeader = array;
		}

		private static readonly byte[] MapiSdHeader;
	}
}
