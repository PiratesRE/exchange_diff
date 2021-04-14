using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class WindowsTokenAccessor : CommonAccessTokenAccessor
	{
		private WindowsTokenAccessor(CommonAccessToken token) : base(token)
		{
		}

		public override AccessTokenType TokenType
		{
			get
			{
				return AccessTokenType.Windows;
			}
		}

		public static WindowsTokenAccessor Create(WindowsIdentity windowsIdentity)
		{
			if (windowsIdentity == null)
			{
				throw new ArgumentNullException("windowsIdentity");
			}
			CommonAccessToken token = new CommonAccessToken(windowsIdentity);
			return new WindowsTokenAccessor(token);
		}

		public static WindowsTokenAccessor Attach(CommonAccessToken token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			return new WindowsTokenAccessor(token);
		}

		public WindowsAccessToken WindowsAccessToken
		{
			get
			{
				return base.Token.WindowsAccessToken;
			}
		}
	}
}
