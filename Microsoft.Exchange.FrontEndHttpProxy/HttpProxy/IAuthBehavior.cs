using System;
using System.Web;

namespace Microsoft.Exchange.HttpProxy
{
	internal interface IAuthBehavior
	{
		AuthState AuthState { get; }

		bool ShouldDoFullAuthOnUnresolvedAnchorMailbox { get; }

		bool ShouldCopyAuthenticationHeaderToClientResponse { get; }

		void SetState(int serverVersion);

		void ResetState();

		string GetExecutingUserOrganization();

		bool IsFullyAuthenticated();

		AnchorMailbox CreateAuthModuleSpecificAnchorMailbox(IRequestContext requestContext);

		void ContinueOnAuthenticate(HttpApplication app, AsyncCallback callback);

		void SetFailureStatus();
	}
}
