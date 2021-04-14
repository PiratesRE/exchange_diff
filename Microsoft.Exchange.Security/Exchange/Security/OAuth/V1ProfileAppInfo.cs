using System;
using System.IdentityModel.Tokens;
using System.Linq;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class V1ProfileAppInfo
	{
		public V1ProfileAppInfo(OAuthTokenHandler.V1ProfileTokenHandlerBase handler, JwtSecurityToken token)
		{
			JwtPayload payload = token.Payload;
			this.AppId = handler.GetClaimValue(token, Constants.ClaimTypes.AppId);
			this.Scope = OAuthCommon.TryGetClaimValue(token.Payload, Constants.ClaimTypes.Scp);
			object obj;
			if (token.Payload.TryGetValue(Constants.ClaimTypes.Roles, out obj))
			{
				object[] array = obj as object[];
				string text = obj as string;
				if (array != null)
				{
					if (array.All((object x) => x.GetType() == typeof(string)))
					{
						this.Role = string.Join(" ", array);
						return;
					}
				}
				if (text != null && !text.Contains(" "))
				{
					this.Role = text;
					return;
				}
				handler.Throw(OAuthErrors.InvalidClaimValueFound, new object[]
				{
					obj.SerializeToJson(),
					Constants.ClaimTypes.Roles
				}, null, null);
			}
		}

		public V1ProfileAppInfo(string appId, string scp, string rol = null)
		{
			this.AppId = appId;
			this.Scope = scp;
			this.Role = rol;
		}

		public string AppId { get; private set; }

		public string Scope { get; set; }

		public string Role { get; set; }

		public bool ContainsFullAccessRole
		{
			get
			{
				return !string.IsNullOrEmpty(this.Role) && this.Role.Split(new char[]
				{
					' '
				}, StringSplitOptions.RemoveEmptyEntries).Contains(Constants.ClaimValues.FullAccess);
			}
		}

		public override string ToString()
		{
			return this.AppId;
		}
	}
}
