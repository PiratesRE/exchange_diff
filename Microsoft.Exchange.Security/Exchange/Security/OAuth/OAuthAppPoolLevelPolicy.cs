using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class OAuthAppPoolLevelPolicy
	{
		private OAuthAppPoolLevelPolicy() : this(new StringAppSettingsEntry("OAuthHttpModule.Profiles", string.Empty, ExTraceGlobals.OAuthTracer).Value, new StringAppSettingsEntry("OAuthHttpModule.V1AppScopes", Constants.ClaimValues.UserImpersonation, ExTraceGlobals.OAuthTracer).Value, new StringAppSettingsEntry("OAuthHttpModule.V1AppRoles", string.Empty, ExTraceGlobals.OAuthTracer).Value)
		{
		}

		internal OAuthAppPoolLevelPolicy(string profiles, string scopes, string roles)
		{
			this.allowedProfiles = (from t in profiles.Split(new char[]
			{
				OAuthAppPoolLevelPolicy.seperator
			})
			where Constants.TokenCategories.All.Contains(t)
			select t).ToArray<string>();
			this.allowAnyV1AppScope = string.Equals(scopes, OAuthAppPoolLevelPolicy.allowAnything);
			if (!this.allowAnyV1AppScope)
			{
				this.allowedV1AppScopes = scopes.Split(new char[]
				{
					OAuthAppPoolLevelPolicy.seperator
				});
			}
			this.allowAnyV1AppRole = string.Equals(roles, OAuthAppPoolLevelPolicy.allowAnything);
			if (!this.allowAnyV1AppRole)
			{
				this.allowedV1AppRoles = roles.Split(new char[]
				{
					OAuthAppPoolLevelPolicy.seperator
				});
			}
		}

		public static OAuthAppPoolLevelPolicy Instance
		{
			get
			{
				return OAuthAppPoolLevelPolicy.instance;
			}
		}

		public bool IsAllowedProfiles(string givenProfile)
		{
			return this.allowedProfiles.Contains(givenProfile, StringComparer.OrdinalIgnoreCase);
		}

		public List<string> GetV1AppScope(string[] givenScope)
		{
			return this.GetFilteredPermissions(givenScope, this.allowAnyV1AppScope, this.allowedV1AppScopes);
		}

		public List<string> GetV1AppRole(string[] givenRole)
		{
			return this.GetFilteredPermissions(givenRole, this.allowAnyV1AppRole, this.allowedV1AppRoles);
		}

		private List<string> GetFilteredPermissions(string[] permissions, bool allowAny, string[] allowedValues)
		{
			List<string> list = new List<string>();
			if (allowAny)
			{
				list.AddRange(permissions);
			}
			else
			{
				foreach (string text in permissions)
				{
					if (allowedValues.Contains(text, StringComparer.OrdinalIgnoreCase))
					{
						list.Add(text);
					}
				}
			}
			list.Sort();
			return list;
		}

		private static readonly char seperator = '|';

		private static readonly char[] claimValueSeperators = new char[]
		{
			' '
		};

		private static readonly string allowAnything = "*";

		private static OAuthAppPoolLevelPolicy instance = new OAuthAppPoolLevelPolicy();

		private readonly string[] allowedProfiles;

		private readonly bool allowAnyV1AppScope;

		private readonly string[] allowedV1AppScopes;

		private readonly bool allowAnyV1AppRole;

		private readonly string[] allowedV1AppRoles;
	}
}
