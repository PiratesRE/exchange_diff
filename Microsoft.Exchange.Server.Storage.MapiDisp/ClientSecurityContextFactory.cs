using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	internal static class ClientSecurityContextFactory
	{
		public static ClientSecurityContext Create(Context operationContext, byte[] serialization)
		{
			return ClientSecurityContextFactory.Create(serialization, delegate(Exception ex)
			{
				ClientSecurityContextFactory.OnException(ex, operationContext);
			});
		}

		public static ClientSecurityContext Create(Context operationContext, AuthenticationContext authenticationContext)
		{
			if (authenticationContext.RegularGroups.Length == 0)
			{
				DiagnosticContext.TraceLocation((LID)48424U);
				return null;
			}
			if (authenticationContext.PrimaryGroupIndex < 0 && !authenticationContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid))
			{
				DiagnosticContext.TraceLocation((LID)64808U);
				return null;
			}
			return ClientSecurityContextFactory.Create(authenticationContext, delegate(Exception ex)
			{
				ClientSecurityContextFactory.OnException(ex, operationContext);
			});
		}

		private static void OnException(Exception exception, Context operationContext)
		{
			operationContext.OnExceptionCatch(exception);
			if (exception is BufferParseException)
			{
				ClientSecurityContextFactory.RecordError((LID)49896U, exception);
				return;
			}
			if (exception is AuthzException)
			{
				ClientSecurityContextFactory.RecordError((LID)58088U, exception);
				return;
			}
			if (exception is InvalidSidException)
			{
				ClientSecurityContextFactory.RecordError((LID)33512U, exception);
				return;
			}
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "Unexpected exception");
		}

		private static void RecordError(LID lid, Exception exception)
		{
			DiagnosticContext.TraceLocation(lid);
			if (ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				ExTraceGlobals.RpcDetailTracer.TraceError<Exception>((long)((ulong)lid.Value), "Caught exception {0} while creating a ClientSecurityContext from an AuthenticationContext", exception);
			}
		}
	}
}
