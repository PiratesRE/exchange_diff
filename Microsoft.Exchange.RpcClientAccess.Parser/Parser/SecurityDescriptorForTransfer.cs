using System;
using System.Security.AccessControl;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal static class SecurityDescriptorForTransfer
	{
		public static byte[] FormatSecurityDescriptor(RawSecurityDescriptor securityDescriptor)
		{
			byte[] array = new byte[SecurityDescriptorForTransfer.MapiSecurityDescriptorHeader.Length + securityDescriptor.BinaryLength];
			Array.Copy(SecurityDescriptorForTransfer.MapiSecurityDescriptorHeader, 0, array, 0, SecurityDescriptorForTransfer.MapiSecurityDescriptorHeader.Length);
			securityDescriptor.GetBinaryForm(array, SecurityDescriptorForTransfer.MapiSecurityDescriptorHeader.Length);
			return array;
		}

		public static RawSecurityDescriptor ExtractSecurityDescriptor(byte[] formattedSecurityDescriptor)
		{
			RawSecurityDescriptor result;
			using (BufferReader bufferReader = new BufferReader(new ArraySegment<byte>(formattedSecurityDescriptor)))
			{
				bufferReader.ReadUInt16();
				ushort num = bufferReader.ReadUInt16();
				if (num != 1)
				{
					if (num != 3)
					{
						throw new BufferParseException(string.Format("Unsupported SD version: {0}", num));
					}
					bufferReader.ReadUInt32();
				}
				RawSecurityDescriptor rawSecurityDescriptor;
				try
				{
					rawSecurityDescriptor = new RawSecurityDescriptor(formattedSecurityDescriptor, (int)bufferReader.Position);
				}
				catch (ArgumentOutOfRangeException ex)
				{
					throw new BufferParseException(string.Format("Failed to parse SecurityDescriptor: {0}", ex.Message));
				}
				catch (ArgumentException ex2)
				{
					throw new BufferParseException(string.Format("Failed to parse SecurityDescriptor: {0}", ex2.Message));
				}
				catch (FormatException ex3)
				{
					throw new BufferParseException(string.Format("Failed to parse SecurityDescriptor: {0}", ex3.Message));
				}
				catch (OverflowException ex4)
				{
					throw new BufferParseException(string.Format("Failed to parse SecurityDescriptor: {0}", ex4.Message));
				}
				catch (InvalidOperationException ex5)
				{
					throw new BufferParseException(string.Format("Failed to parse SecurityDescriptor: {0}", ex5.Message));
				}
				result = rawSecurityDescriptor;
			}
			return result;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static SecurityDescriptorForTransfer()
		{
			byte[] array = new byte[8];
			array[0] = 8;
			array[2] = 3;
			SecurityDescriptorForTransfer.MapiSecurityDescriptorHeader = array;
		}

		private static readonly byte[] MapiSecurityDescriptorHeader;
	}
}
