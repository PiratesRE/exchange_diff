using System;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaAnonymousApplication : BaseApplication
	{
		public override int MaxBreadcrumbs
		{
			get
			{
				return 0;
			}
		}

		public override bool LogVerboseNotifications
		{
			get
			{
				return false;
			}
		}

		public override int ActivityBasedPresenceDuration
		{
			get
			{
				return 0;
			}
		}

		public override HttpClientCredentialType ServiceAuthenticationType
		{
			get
			{
				return HttpClientCredentialType.None;
			}
		}

		public override TroubleshootingContext TroubleshootingContext
		{
			get
			{
				return this.troubleshootingContext;
			}
		}

		public override bool LogErrorDetails
		{
			get
			{
				return false;
			}
		}

		public override bool LogErrorTraces
		{
			get
			{
				return false;
			}
		}

		internal override void UpdateErrorTracingConfiguration()
		{
		}

		internal override void Initialize()
		{
		}

		protected override void InternalDispose()
		{
		}

		protected override void ExecuteApplicationSpecificStart()
		{
			ErrorHandlerUtilities.RegisterForUnhandledExceptions();
		}

		private TroubleshootingContext troubleshootingContext = new TroubleshootingContext("OwaAnonymousServer");
	}
}
