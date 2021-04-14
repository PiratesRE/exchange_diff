using System;
using System.IO;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Core;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	internal sealed class PreloadSessionDataHandler : SessionDataHandler
	{
		static PreloadSessionDataHandler()
		{
			OwsLogRegistry.Register("SessionDataPreload_Overall", typeof(SessionDataMetadata), new Type[0]);
		}

		protected override Stream OutputStream
		{
			get
			{
				return this.sessionDataCache.OutputStream;
			}
		}

		protected override Encoding Encoding
		{
			get
			{
				return this.sessionDataCache.Encoding;
			}
		}

		protected override bool BufferOutput { get; set; }

		protected override string ContentType { get; set; }

		protected override bool IsSessionDataPreloadRequest
		{
			get
			{
				return true;
			}
		}

		protected override string LogEventId
		{
			get
			{
				return "SessionDataPreload_Overall";
			}
		}

		protected override void Flush()
		{
		}

		protected override void OnBeforeBeginProcessing(HttpContext context, out bool shouldContinue)
		{
			shouldContinue = true;
			ExAssert.RetailAssert(context != null, "HttpContext is null");
			if (!base.IsSessionDataPreloadEnabled)
			{
				ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), "[SessionDataPreload] SessionDataCache is not supported.");
				shouldContinue = false;
				return;
			}
			this.sessionDataCache = base.RequestContext.UserContext.SessionDataCache;
			ExAssert.RetailAssert(this.sessionDataCache != null, "SessionDataCache is null");
			shouldContinue = this.sessionDataCache.StartBuilding();
			ExTraceGlobals.SessionDataHandlerTracer.TraceDebug<bool>((long)this.GetHashCode(), "[SessionDataPreload] Starting to build SessionDataCache {0}.", shouldContinue);
		}

		protected override void OnBeforeEndProcessing(UserContext userContext)
		{
			if (!base.IsSessionDataPreloadEnabled)
			{
				ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), "[SessionDataPreload] SessionDataCache is not supported.");
				return;
			}
			ExTraceGlobals.SessionDataHandlerTracer.TraceDebug((long)this.GetHashCode(), "[SessionDataPreload] Completing to build SessionDataCache.");
			this.sessionDataCache.CompleteBuilding();
		}

		public const string SessionDataPreLoadOverallEventId = "SessionDataPreload_Overall";

		private SessionDataCache sessionDataCache;
	}
}
