using System;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class DefaultOutboundTracer : IOutboundTracer
	{
		protected DefaultOutboundTracer()
		{
		}

		public static IOutboundTracer Instance
		{
			get
			{
				return DefaultOutboundTracer.instance;
			}
		}

		public void LogInformation(int hashCode, string formatString, params object[] args)
		{
			ExTraceGlobals.OAuthTracer.TraceDebug((long)hashCode, formatString, args);
			this.LogInformation2(hashCode, formatString, args);
		}

		protected virtual void LogInformation2(int hashCode, string formatString, params object[] args)
		{
		}

		public void LogWarning(int hashCode, string formatString, params object[] args)
		{
			ExTraceGlobals.OAuthTracer.TraceWarning((long)hashCode, formatString, args);
			this.LogWarning2(hashCode, formatString, args);
		}

		protected virtual void LogWarning2(int hashCode, string formatString, params object[] args)
		{
		}

		public void LogError(int hashCode, string formatString, params object[] args)
		{
			ExTraceGlobals.OAuthTracer.TraceError((long)hashCode, formatString, args);
			this.LogError2(hashCode, formatString, args);
		}

		protected virtual void LogError2(int hashCode, string formatString, params object[] args)
		{
		}

		public void LogToken(int hashCode, string tokenString)
		{
			ExTraceGlobals.OAuthTracer.TraceError<string>((long)hashCode, "the final token is {0}", tokenString);
			this.LogToken2(hashCode, tokenString);
		}

		protected virtual void LogToken2(int hashCode, string tokenString)
		{
		}

		private static IOutboundTracer instance = new DefaultOutboundTracer();
	}
}
