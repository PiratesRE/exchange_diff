using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Microsoft.Exchange.Security.Authorization
{
	[Serializable]
	public class SidStringAndAttributes
	{
		public string SecurityIdentifier
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

		private SidStringAndAttributes()
		{
		}

		[CLSCompliant(false)]
		public SidStringAndAttributes(string identifier, uint attribute)
		{
			this.securityIdentifier = identifier;
			this.attributes = attribute;
		}

		internal static SidStringAndAttributes Read(IntPtr pointer, SecurityIdentifier localMachineAuthoritySid, ref int offset)
		{
			IntPtr binaryForm = Marshal.ReadIntPtr(pointer, offset);
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(binaryForm);
			offset += Marshal.SizeOf(typeof(IntPtr));
			uint attribute = (uint)Marshal.ReadInt32(pointer, offset);
			offset += Marshal.SizeOf(typeof(IntPtr));
			if (securityIdentifier.IsEqualDomainSid(localMachineAuthoritySid))
			{
				return null;
			}
			string text = securityIdentifier.ToString();
			if (!text.StartsWith("S-1-16", StringComparison.OrdinalIgnoreCase))
			{
				return new SidStringAndAttributes(securityIdentifier.ToString(), attribute);
			}
			return null;
		}

		private const string IntegritySidPrefix = "S-1-16";

		private string securityIdentifier;

		private uint attributes;
	}
}
