using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Security.Authentication
{
	internal abstract class CommonAccessTokenAccessor
	{
		protected CommonAccessTokenAccessor()
		{
		}

		protected CommonAccessTokenAccessor(CommonAccessToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			this.CheckTokenType(token);
			this.Token = token;
		}

		public abstract AccessTokenType TokenType { get; }

		protected CommonAccessToken Token { get; set; }

		public virtual CommonAccessToken GetToken()
		{
			return this.Token;
		}

		public static string SerializeOrganizationId(OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			if (organizationId.OrganizationalUnit != null)
			{
				organizationId.EnsureFullyPopulated();
			}
			return Convert.ToBase64String(organizationId.GetBytes(Encoding.UTF8));
		}

		public static OrganizationId DeserializeOrganizationId(string serializedString)
		{
			if (string.IsNullOrEmpty(serializedString))
			{
				throw new ArgumentNullException("serializedString");
			}
			OrganizationId result = null;
			byte[] bytes = Convert.FromBase64String(serializedString);
			if (!OrganizationId.TryCreateFromBytes(bytes, Encoding.UTF8, out result))
			{
				throw new FormatException("Invalid OrganizationId format.");
			}
			return result;
		}

		public static string SerializeGroupMembershipSids(IEnumerable<string> groupSids)
		{
			if (groupSids != null)
			{
				return string.Join(";", groupSids);
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaServer.ShouldSkipAdfsGroupReadOnFrontend.Enabled)
			{
				return string.Empty;
			}
			throw new ArgumentNullException("groupSids");
		}

		public static string SerializeIsPublicSession(bool isPublicSession)
		{
			return isPublicSession.ToString();
		}

		public static IEnumerable<string> DeserializeGroupMembershipSids(string serializedString)
		{
			if (string.IsNullOrEmpty(serializedString))
			{
				throw new ArgumentNullException("serializedString");
			}
			return serializedString.Split(new string[]
			{
				";"
			}, StringSplitOptions.RemoveEmptyEntries);
		}

		public static bool DeserializIsPublicSession(string serializedString)
		{
			if (string.IsNullOrEmpty(serializedString))
			{
				throw new ArgumentNullException("serializedString");
			}
			return bool.Parse(serializedString);
		}

		protected void CheckTokenType(CommonAccessToken token)
		{
			if (!string.Equals(token.TokenType, this.TokenType.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("token");
			}
		}

		protected string SafeGetValue(string key)
		{
			string result = null;
			this.Token.ExtensionData.TryGetValue(key, out result);
			return result;
		}

		protected void SafeSetValue(string key, string value)
		{
			this.Token.ExtensionData[key] = value;
		}

		private const string GroupMembershipSidDelimiter = ";";
	}
}
