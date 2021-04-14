using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.ServiceHost;

namespace Microsoft.Exchange.Servicelets.AuthAdmin
{
	public class AuthAdmin : Servicelet
	{
		public override void Work()
		{
			using (AuthAdminContext authAdminContext = new AuthAdminContext("AuthAdmin"))
			{
				AnchorApplication anchorApplication = new AnchorApplication(authAdminContext, base.StopEvent);
				anchorApplication.Process();
			}
		}

		internal const string ApplicationName = "AuthAdmin";
	}
}
