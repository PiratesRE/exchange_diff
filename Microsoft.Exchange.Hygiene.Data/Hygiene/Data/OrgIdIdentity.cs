using System;
using System.IO;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	[Serializable]
	public sealed class OrgIdIdentity : GenericIdentity
	{
		public OrgIdIdentity(string userPrincipal) : this(userPrincipal, string.Empty)
		{
		}

		public OrgIdIdentity(string userPrincipal, string authType) : base(userPrincipal, authType)
		{
		}

		public ADObjectId UserId { get; internal set; }

		public ADObjectId TenantId { get; internal set; }

		public static OrgIdIdentity Deserialize(string token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			OrgIdIdentity result;
			using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(token)))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					string authType = binaryReader.ReadString();
					string userPrincipal = binaryReader.ReadString();
					string distinguishedName = binaryReader.ReadString();
					string input = binaryReader.ReadString();
					string distinguishedName2 = binaryReader.ReadString();
					string input2 = binaryReader.ReadString();
					result = new OrgIdIdentity(userPrincipal, authType)
					{
						UserId = new ADObjectId(distinguishedName, Guid.Parse(input)),
						TenantId = new ADObjectId(distinguishedName2, Guid.Parse(input2))
					};
				}
			}
			return result;
		}

		public string Serialize()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(this.AuthenticationType);
					binaryWriter.Write(this.Name);
					binaryWriter.Write(this.UserId.DistinguishedName);
					binaryWriter.Write(this.UserId.ObjectGuid.ToString());
					binaryWriter.Write(this.TenantId.DistinguishedName);
					binaryWriter.Write(this.TenantId.ObjectGuid.ToString());
					binaryWriter.Flush();
				}
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}
	}
}
