using System;
using System.IO;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authorization
{
	public sealed class WindowsAccessToken : SecurityAccessToken
	{
		internal WindowsAccessToken(string logonName, string authenticationType, ClientSecurityContext clientSecurityContext, CommonAccessToken commonAccessToken)
		{
			this.LogonName = logonName;
			this.AuthenticationType = authenticationType;
			this.commonAccessToken = commonAccessToken;
			clientSecurityContext.SetSecurityAccessToken(this);
		}

		internal WindowsAccessToken(CommonAccessToken commonAccessToken)
		{
			this.commonAccessToken = commonAccessToken;
		}

		public string LogonName { get; private set; }

		public string AuthenticationType { get; private set; }

		internal CommonAccessToken CommonAccessToken
		{
			get
			{
				return this.commonAccessToken;
			}
		}

		internal byte[] GetSerializedToken()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.SerializeToken(binaryWriter);
					binaryWriter.Flush();
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		internal void DeserializeFromToken(byte[] token)
		{
			using (MemoryStream memoryStream = new MemoryStream(token))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					this.commonAccessToken.ReadAndValidateFieldType(binaryReader, 'A', AuthorizationStrings.AuthenticationTypeIsMissing);
					this.AuthenticationType = this.commonAccessToken.BinaryRead<string>(new Func<string>(binaryReader.ReadString), AuthorizationStrings.AuthenticationTypeIsMissing);
					this.commonAccessToken.ReadAndValidateFieldType(binaryReader, 'L', AuthorizationStrings.LogonNameIsMissing);
					this.LogonName = this.commonAccessToken.BinaryRead<string>(new Func<string>(binaryReader.ReadString), AuthorizationStrings.LogonNameIsMissing);
					this.commonAccessToken.ReadAndValidateFieldType(binaryReader, 'U', AuthorizationStrings.MissingUserSid);
					base.UserSid = this.commonAccessToken.BinaryRead<string>(new Func<string>(binaryReader.ReadString), AuthorizationStrings.MissingUserSid);
					if (string.IsNullOrEmpty(base.UserSid))
					{
						throw new CommonAccessTokenException((int)this.commonAccessToken.Version, AuthorizationStrings.MissingUserSid);
					}
					this.ReadGroupsAndExtensionData(binaryReader);
				}
			}
		}

		private static void WriteGroups(BinaryWriter writer, SidStringAndAttributes[] groups, char fieldType)
		{
			if (groups != null)
			{
				writer.Write(fieldType);
				writer.Write(groups.Length);
				for (int i = 0; i < groups.Length; i++)
				{
					WindowsAccessToken.WriteSid(writer, groups[i].SecurityIdentifier, groups[i].Attributes);
				}
			}
		}

		private static void WriteSid(BinaryWriter writer, string sid, uint attributes)
		{
			writer.Write(attributes);
			writer.Write(sid);
		}

		private void SerializeToken(BinaryWriter writer)
		{
			writer.Write('A');
			writer.Write(this.AuthenticationType);
			writer.Write('L');
			writer.Write(this.LogonName);
			writer.Write('U');
			writer.Write(base.UserSid);
			WindowsAccessToken.WriteGroups(writer, base.GroupSids, 'G');
			WindowsAccessToken.WriteGroups(writer, base.RestrictedGroupSids, 'R');
			this.commonAccessToken.WriteExtensionData(writer);
		}

		private void ReadGroupsAndExtensionData(BinaryReader reader)
		{
			int num = 0;
			while (reader.PeekChar() >= 0)
			{
				char c = reader.ReadChar();
				if (c == 'G')
				{
					int num2 = this.commonAccessToken.BinaryRead<int>(new Func<int>(reader.ReadInt32), AuthorizationStrings.InvalidGroupLength);
					num += num2;
					if (num > 3000)
					{
						throw new CommonAccessTokenException((int)this.commonAccessToken.Version, AuthorizationStrings.TooManySidNodes(this.LogonName, 3000));
					}
					base.GroupSids = new SidStringAndAttributes[num2];
					for (int i = 0; i < num2; i++)
					{
						uint num3 = this.commonAccessToken.BinaryRead<uint>(new Func<uint>(reader.ReadUInt32), AuthorizationStrings.InvalidGroupAttributes);
						if (num3 == 0U)
						{
							throw new CommonAccessTokenException((int)this.commonAccessToken.Version, AuthorizationStrings.InvalidGroupAttributesValue);
						}
						string identifier = this.commonAccessToken.BinaryRead<string>(new Func<string>(reader.ReadString), AuthorizationStrings.InvalidGroupSidValue);
						base.GroupSids[i] = new SidStringAndAttributes(identifier, num3);
					}
				}
				else if (c == 'R')
				{
					int num2 = this.commonAccessToken.BinaryRead<int>(new Func<int>(reader.ReadInt32), AuthorizationStrings.InvalidRestrictedGroupLength);
					num += num2;
					if (num > 3000)
					{
						throw new CommonAccessTokenException((int)this.commonAccessToken.Version, AuthorizationStrings.TooManySidNodes(this.LogonName, 3000));
					}
					base.RestrictedGroupSids = new SidStringAndAttributes[num2];
					for (int j = 0; j < num2; j++)
					{
						uint num4 = this.commonAccessToken.BinaryRead<uint>(new Func<uint>(reader.ReadUInt32), AuthorizationStrings.InvalidRestrictedGroupAttributes);
						if (num4 == 0U)
						{
							throw new CommonAccessTokenException((int)this.commonAccessToken.Version, AuthorizationStrings.InvalidRestrictedGroupAttributesValue);
						}
						string identifier2 = this.commonAccessToken.BinaryRead<string>(new Func<string>(reader.ReadString), AuthorizationStrings.InvalidRestrictedGroupSidValue);
						base.RestrictedGroupSids[j] = new SidStringAndAttributes(identifier2, num4);
					}
				}
				else
				{
					if (c != 'E')
					{
						throw new CommonAccessTokenException((int)this.commonAccessToken.Version, AuthorizationStrings.InvalidFieldType);
					}
					this.commonAccessToken.ReadExtensionData(reader);
				}
			}
		}

		private const int MaximumSidsPerContext = 3000;

		private CommonAccessToken commonAccessToken;
	}
}
