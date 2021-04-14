using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class InstantMessageSignIn : InstantMessageCommandBase<int>
	{
		static InstantMessageSignIn()
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessageSignIn.LogMetadata), new Type[0]);
		}

		public InstantMessageSignIn(CallContext callContext, bool signedInManually) : base(callContext)
		{
			this.signedInManually = signedInManually;
		}

		protected override int InternalExecute()
		{
			InstantMessageOperationError instantMessageOperationError = this.SignIn();
			OwaApplication.GetRequestDetailsLogger.Set(InstantMessagingLogMetadata.OperationErrorCode, instantMessageOperationError);
			return (int)instantMessageOperationError;
		}

		private static InstantMessageOperationError InitializeProvider()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			InstantMessageOperationError result;
			try
			{
				result = InstantMessageProvider.Initialize();
			}
			finally
			{
				stopwatch.Stop();
				OwaApplication.GetRequestDetailsLogger.Set(InstantMessageSignIn.LogMetadata.InitializeProvider, stopwatch.ElapsedMilliseconds);
			}
			return result;
		}

		private static UserContext GetUserContext()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			UserContext userContext;
			try
			{
				userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			}
			finally
			{
				stopwatch.Stop();
				OwaApplication.GetRequestDetailsLogger.Set(InstantMessageSignIn.LogMetadata.GetUserContext, stopwatch.ElapsedMilliseconds);
			}
			return userContext;
		}

		private InstantMessageOperationError SignIn()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			InstantMessageOperationError result;
			try
			{
				InstantMessageOperationError instantMessageOperationError = InstantMessageSignIn.InitializeProvider();
				if (instantMessageOperationError != InstantMessageOperationError.Success)
				{
					result = instantMessageOperationError;
				}
				else
				{
					UserContext userContext = InstantMessageSignIn.GetUserContext();
					if (!userContext.IsInstantMessageEnabled)
					{
						result = InstantMessageOperationError.NotEnabled;
					}
					else if (userContext.InstantMessageManager == null)
					{
						result = InstantMessageOperationError.NotConfigured;
					}
					else if (!this.ShouldSignIn(userContext))
					{
						result = InstantMessageOperationError.NotSignedIn;
					}
					else
					{
						InstantMessageOperationError instantMessageOperationError2 = userContext.InstantMessageManager.StartProvider(base.MailboxIdentityMailboxSession);
						result = instantMessageOperationError2;
					}
				}
			}
			finally
			{
				stopwatch.Stop();
				OwaApplication.GetRequestDetailsLogger.Set(InstantMessageSignIn.LogMetadata.Total, stopwatch.ElapsedMilliseconds);
			}
			return result;
		}

		private bool ShouldSignIn(UserContext userContext)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			bool result;
			try
			{
				if (this.signedInManually)
				{
					InstantMessageUtilities.SetSignedOutFlag(base.MailboxIdentityMailboxSession, false);
				}
				else if (InstantMessageUtilities.IsSignedOut(base.MailboxIdentityMailboxSession))
				{
					InstantMessageNotifier notifier = userContext.InstantMessageManager.Notifier;
					if (notifier != null)
					{
						InstantMessagePayloadUtilities.GenerateUnavailablePayload(notifier, null, "Not signed in because IsSignedOutOfIM was true.", InstantMessageServiceError.ClientSignOut, false);
					}
					return false;
				}
				result = true;
			}
			finally
			{
				stopwatch.Stop();
				OwaApplication.GetRequestDetailsLogger.Set(InstantMessageSignIn.LogMetadata.HandleSignOutFlag, stopwatch.ElapsedMilliseconds);
			}
			return result;
		}

		private readonly bool signedInManually;

		public enum LogMetadata
		{
			[DisplayName("IM.SI.UC")]
			GetUserContext,
			[DisplayName("IM.SI.IP")]
			InitializeProvider,
			[DisplayName("IM.SI.SOF")]
			HandleSignOutFlag,
			[DisplayName("IM.SI.CC")]
			CheckConfiguration,
			[DisplayName("IM.SI.FP")]
			FindProvider,
			[DisplayName("IM.SI.DLL")]
			LoadDll,
			[DisplayName("IM.SI.GC")]
			GetCertificate,
			[DisplayName("IM.SI.CEM")]
			CreateEndpointManager,
			[DisplayName("IM.SI.IEM")]
			InitializeEndpointManager,
			[DisplayName("IM.SI.CP")]
			CreateProvider,
			[DisplayName("IM.SI.ES")]
			EstablishSession,
			[DisplayName("IM.SI.GEG")]
			GetExpandedGroups,
			[DisplayName("IM.SI.EG")]
			ExpandGroups,
			[DisplayName("IM.SI.RP")]
			ResetPresence,
			[DisplayName("IM.SI.T")]
			Total
		}
	}
}
