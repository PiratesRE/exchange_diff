using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmUnhandledExceptionHandler
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AmSystemManagerTracer;
			}
		}

		internal void Add(IUnhandledExceptionHandler handler)
		{
			lock (this.m_locker)
			{
				this.m_handlers.Add(handler);
				if (!this.m_isHandlerSet)
				{
					AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;
					AmUnhandledExceptionHandler.Tracer.TraceDebug((long)this.GetHashCode(), "AmUnhandledExceptionHandler: Unhandled exception handler is set.");
					this.m_isHandlerSet = true;
				}
			}
		}

		internal void Remove(IUnhandledExceptionHandler handler)
		{
			lock (this.m_locker)
			{
				if (handler != null)
				{
					if (this.m_handlers.Count > 0)
					{
						this.m_handlers.Remove(handler);
					}
				}
				else
				{
					this.m_handlers.Clear();
				}
				if (this.m_handlers.Count == 0 && this.m_isHandlerSet)
				{
					AppDomain.CurrentDomain.UnhandledException -= this.OnUnhandledException;
					AmUnhandledExceptionHandler.Tracer.TraceDebug((long)this.GetHashCode(), "AmUnhandledExceptionHandler: Unhandled exception handler is removed.");
					this.m_isHandlerSet = false;
				}
			}
		}

		internal void Cleanup()
		{
			this.Remove(null);
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			lock (this.m_locker)
			{
				string text = (e != null && e.ExceptionObject != null) ? e.ExceptionObject.ToString() : "<unknown>";
				ReplayCrimsonEvents.OperationGeneratedUnhandledException.Log<string>(text);
				AmUnhandledExceptionHandler.Tracer.TraceError<string>((long)this.GetHashCode(), "Best effort critical cleanup before watson dump is triggered (error: {0})", text);
				foreach (IUnhandledExceptionHandler unhandledExceptionHandler in this.m_handlers)
				{
					if (unhandledExceptionHandler != null)
					{
						unhandledExceptionHandler.OnUnhandledException();
					}
				}
			}
		}

		private List<IUnhandledExceptionHandler> m_handlers = new List<IUnhandledExceptionHandler>();

		private object m_locker = new object();

		private bool m_isHandlerSet;
	}
}
