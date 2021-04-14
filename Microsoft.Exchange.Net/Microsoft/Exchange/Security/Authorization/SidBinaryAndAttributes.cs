using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Microsoft.Exchange.Security.Authorization
{
	public class SidBinaryAndAttributes
	{
		public SecurityIdentifier SecurityIdentifier
		{
			get
			{
				return this.securityIdentifier;
			}
		}

		[CLSCompliant(false)]
		public uint Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		private SidBinaryAndAttributes()
		{
		}

		[CLSCompliant(false)]
		public SidBinaryAndAttributes(SecurityIdentifier identifier, uint attribute)
		{
			this.securityIdentifier = identifier;
			this.attributes = attribute;
		}

		internal static SidBinaryAndAttributes Read(IntPtr pointer, SecurityIdentifier localMachineAuthoritySid, ref int offset)
		{
			IntPtr binaryForm = Marshal.ReadIntPtr(pointer, offset);
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(binaryForm);
			offset += Marshal.SizeOf(typeof(IntPtr));
			uint num = (uint)Marshal.ReadInt32(pointer, offset);
			offset += Marshal.SizeOf(typeof(IntPtr));
			if (!securityIdentifier.IsEqualDomainSid(localMachineAuthoritySid) && (num & 32U) != 32U)
			{
				return new SidBinaryAndAttributes(securityIdentifier, num);
			}
			return null;
		}

		private readonly SecurityIdentifier securityIdentifier;

		private readonly uint attributes;
	}
}
