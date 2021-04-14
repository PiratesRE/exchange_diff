using System;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Exchange.Security.Authorization
{
	[Serializable]
	public class SecurityAccessToken : ISecurityAccessToken
	{
		public SecurityAccessToken()
		{
		}

		public SecurityAccessToken(ISecurityAccessToken securityAccessToken)
		{
			if (securityAccessToken == null)
			{
				throw new ArgumentNullException("securityAccessToken");
			}
			this.UserSid = securityAccessToken.UserSid;
			this.GroupSids = securityAccessToken.GroupSids;
			this.RestrictedGroupSids = securityAccessToken.RestrictedGroupSids;
		}

		public string UserSid { get; set; }

		public SidStringAndAttributes[] GroupSids { get; set; }

		public SidStringAndAttributes[] RestrictedGroupSids { get; set; }

		public override string ToString()
		{
			return this.ToString(false);
		}

		public string ToString(bool resolveIdentities)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.AppendLine("UserSid=" + SecurityAccessToken.SecurityIdentifierToString(this.UserSid, resolveIdentities));
			this.AddToString(stringBuilder, "GroupSid", this.GroupSids, resolveIdentities);
			this.AddToString(stringBuilder, "RestrictedGroupSid", this.RestrictedGroupSids, resolveIdentities);
			return stringBuilder.ToString();
		}

		private static string SecurityIdentifierToString(string sid, bool resolveIdentities)
		{
			if (sid == null)
			{
				return "<null>";
			}
			if (resolveIdentities)
			{
				try
				{
					SecurityIdentifier securityIdentifier = new SecurityIdentifier(sid);
					if (securityIdentifier.IsValidTargetType(typeof(NTAccount)))
					{
						NTAccount ntaccount = (NTAccount)securityIdentifier.Translate(typeof(NTAccount));
						return string.Concat(new object[]
						{
							sid,
							"(",
							ntaccount,
							")"
						});
					}
				}
				catch (ArgumentException)
				{
				}
				catch (IdentityNotMappedException)
				{
				}
				catch (SystemException)
				{
				}
				return sid;
			}
			return sid;
		}

		private void AddToString(StringBuilder token, string name, SidStringAndAttributes[] sidStringAndAttributesArray, bool resolveIdentities)
		{
			if (sidStringAndAttributesArray == null || sidStringAndAttributesArray.Length == 0)
			{
				token.AppendLine("<no " + name + ">");
				return;
			}
			foreach (SidStringAndAttributes sidStringAndAttributes in sidStringAndAttributesArray)
			{
				token.AppendLine(string.Concat(new object[]
				{
					name,
					"=",
					SecurityAccessToken.SecurityIdentifierToString(sidStringAndAttributes.SecurityIdentifier, resolveIdentities),
					", Attributes=",
					sidStringAndAttributes.Attributes
				}));
			}
		}
	}
}
