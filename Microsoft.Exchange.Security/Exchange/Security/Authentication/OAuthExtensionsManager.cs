using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class OAuthExtensionsManager
	{
		public OAuthExtensionsManager()
		{
			this.oAuthExtensionHandlers = new List<IOAuthExtensionAuthenticationHandler>();
		}

		public void AppendHandlerToChain(IOAuthExtensionAuthenticationHandler handler)
		{
			if (!this.oAuthExtensionHandlers.Contains(handler))
			{
				this.oAuthExtensionHandlers.Add(handler);
			}
		}

		public void RemoveHandlerFromChain(IOAuthExtensionAuthenticationHandler handler)
		{
			if (this.oAuthExtensionHandlers.Contains(handler))
			{
				this.oAuthExtensionHandlers.Remove(handler);
			}
		}

		public bool TryHandleRequestPreAuthentication(OAuthExtensionContext context, out bool isAuthenticationNeeded)
		{
			bool flag = false;
			isAuthenticationNeeded = true;
			foreach (IOAuthExtensionAuthenticationHandler ioauthExtensionAuthenticationHandler in this.oAuthExtensionHandlers)
			{
				flag = ioauthExtensionAuthenticationHandler.TryHandleRequestPreAuthentication(context, out isAuthenticationNeeded);
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		public bool TryGetBearerToken(OAuthExtensionContext context, out string token)
		{
			bool flag = false;
			token = null;
			foreach (IOAuthExtensionAuthenticationHandler ioauthExtensionAuthenticationHandler in this.oAuthExtensionHandlers)
			{
				flag = ioauthExtensionAuthenticationHandler.TryGetBearerToken(context, out token);
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		public bool TryHandleRequestPostAuthentication(OAuthExtensionContext context)
		{
			bool flag = false;
			foreach (IOAuthExtensionAuthenticationHandler ioauthExtensionAuthenticationHandler in this.oAuthExtensionHandlers)
			{
				flag = ioauthExtensionAuthenticationHandler.TryHandleRequestPostAuthentication(context);
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		private readonly List<IOAuthExtensionAuthenticationHandler> oAuthExtensionHandlers;
	}
}
