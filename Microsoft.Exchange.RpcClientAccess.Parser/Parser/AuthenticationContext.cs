using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class AuthenticationContext
	{
		public AuthenticationContext(string connectAs, SecurityIdentifier userSid, int primaryGroupIndex, SidBinaryAndAttributes[] regularGroups, SidBinaryAndAttributes[] restrictedGroups)
		{
			if (userSid == null)
			{
				throw new ArgumentNullException("userSid");
			}
			this.connectAs = (connectAs ?? string.Empty);
			this.userSid = userSid;
			this.primaryGroupIndex = primaryGroupIndex;
			this.regularGroups = (regularGroups ?? Array<SidBinaryAndAttributes>.Empty);
			this.restrictedGroups = (restrictedGroups ?? Array<SidBinaryAndAttributes>.Empty);
			if (primaryGroupIndex < -1 || primaryGroupIndex >= this.regularGroups.Length)
			{
				throw new ArgumentException("primaryGroupIndex");
			}
		}

		public string ConnectAs
		{
			get
			{
				return this.connectAs;
			}
		}

		public SecurityIdentifier UserSid
		{
			get
			{
				return this.userSid;
			}
		}

		public int PrimaryGroupIndex
		{
			get
			{
				return this.primaryGroupIndex;
			}
		}

		public SidBinaryAndAttributes[] RegularGroups
		{
			get
			{
				return this.regularGroups;
			}
		}

		public SidBinaryAndAttributes[] RestrictedGroups
		{
			get
			{
				return this.restrictedGroups;
			}
		}

		public static AuthenticationContext Parse(Reader reader)
		{
			long position = reader.Position;
			uint num = (uint)reader.ReadByte();
			if (num != 0U)
			{
				throw new BufferParseException("Unsupported AuthenticationContext flags");
			}
			uint num2 = reader.ReadUInt32();
			if (num2 == 0U)
			{
				throw new BufferParseException("Empty AuthenticationContext buffer.");
			}
			if ((ulong)num2 > (ulong)(reader.Length - position) || num2 > 65535U)
			{
				throw new BufferParseException("Invalid size encoded in AuthenticationContext buffer.");
			}
			string text = reader.ReadAsciiString(StringFlags.IncludeNull | StringFlags.Sized16 | StringFlags.FailOnError);
			SecurityIdentifier securityIdentifier = reader.ReadSecurityIdentifier();
			SidBinaryAndAttributes[] array = AuthenticationContext.ParseGroups(reader);
			short num3 = (short)reader.ReadUInt16();
			if (num3 < -1 || (int)num3 >= array.Length)
			{
				throw new BufferParseException("Invalid primary group index encoded in AuthenticationContext buffer.");
			}
			SidBinaryAndAttributes[] array2 = AuthenticationContext.ParseGroups(reader);
			long position2 = reader.Position;
			if (position2 - position != (long)((ulong)num2))
			{
				throw new BufferParseException("AuthenticationContext buffer had unexpected size.");
			}
			return new AuthenticationContext(text, securityIdentifier, (int)num3, array, array2);
		}

		public void Serialize(Writer writer)
		{
			long position = writer.Position;
			writer.WriteByte(0);
			long position2 = writer.Position;
			writer.WriteUInt32(0U);
			writer.WriteAsciiString(this.ConnectAs, StringFlags.IncludeNull | StringFlags.Sized16);
			writer.WriteSecurityIdentifier(this.UserSid);
			AuthenticationContext.SerializeGroups(writer, this.RegularGroups);
			writer.WriteUInt16((ushort)this.PrimaryGroupIndex);
			AuthenticationContext.SerializeGroups(writer, this.RestrictedGroups);
			long position3 = writer.Position;
			long num = position3 - position;
			writer.Position = position2;
			writer.WriteUInt32((uint)num);
			writer.Position = position3;
		}

		public override string ToString()
		{
			return string.Format("AuthenticationContext: [connectAs={0}],[userSid={1}]", this.ConnectAs, this.UserSid);
		}

		private static SidBinaryAndAttributes[] ParseGroups(Reader reader)
		{
			ushort num = reader.ReadUInt16();
			SidBinaryAndAttributes[] array = new SidBinaryAndAttributes[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				SecurityIdentifier identifier = reader.ReadSecurityIdentifier();
				GroupAttributes attribute = (GroupAttributes)reader.ReadUInt32();
				array[i] = new SidBinaryAndAttributes(identifier, (uint)attribute);
			}
			return array;
		}

		private static void SerializeGroups(Writer writer, SidBinaryAndAttributes[] group)
		{
			ushort num = (ushort)((group == null) ? 0 : group.Length);
			writer.WriteUInt16(num);
			if (num > 0)
			{
				foreach (SidBinaryAndAttributes sidBinaryAndAttributes in group)
				{
					writer.WriteSecurityIdentifier(sidBinaryAndAttributes.SecurityIdentifier);
					writer.WriteUInt32(sidBinaryAndAttributes.Attributes);
				}
			}
		}

		private const byte Flags = 0;

		internal const int MaxSize = 65535;

		private readonly string connectAs;

		private readonly SecurityIdentifier userSid;

		private readonly int primaryGroupIndex;

		private readonly SidBinaryAndAttributes[] regularGroups;

		private readonly SidBinaryAndAttributes[] restrictedGroups;
	}
}
