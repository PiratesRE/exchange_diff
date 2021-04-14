using System;
using System.Security.Principal;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.PushNotifications.Server.Wcf
{
	internal static class Extensions
	{
		public static IPrincipal GetPrincipal(this OperationContext operationContext)
		{
			Extensions.PrincipalWrapper principalWrapper = operationContext.Extensions.Find<Extensions.PrincipalWrapper>();
			if (principalWrapper != null)
			{
				return principalWrapper.Principal;
			}
			return null;
		}

		public static OAuthIdentity GetOAuthIdentity(this OperationContext operationContext)
		{
			GenericPrincipal genericPrincipal = operationContext.GetPrincipal() as GenericPrincipal;
			if (genericPrincipal != null)
			{
				return genericPrincipal.Identity as OAuthIdentity;
			}
			return null;
		}

		public static WindowsIdentity GetWindowsIdentity(this OperationContext operationContext)
		{
			WindowsPrincipal windowsPrincipal = operationContext.GetPrincipal() as WindowsPrincipal;
			if (windowsPrincipal != null)
			{
				return windowsPrincipal.Identity as WindowsIdentity;
			}
			return null;
		}

		public static void SetPrincipal(this OperationContext operationContext, IPrincipal principal)
		{
			ArgumentValidator.ThrowIfNull("principal", principal);
			operationContext.Extensions.Add(new Extensions.PrincipalWrapper(principal));
		}

		private class PrincipalWrapper : IExtension<OperationContext>
		{
			public PrincipalWrapper(IPrincipal principal)
			{
				ArgumentValidator.ThrowIfNull("principal", principal);
				this.Principal = principal;
			}

			public IPrincipal Principal { get; private set; }

			public void Attach(OperationContext owner)
			{
			}

			public void Detach(OperationContext owner)
			{
			}
		}
	}
}
