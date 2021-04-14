using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MailboxReplicationService;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class MrsTracer
	{
		static MrsTracer()
		{
			MrsTracer.instanceCommon = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationCommonTracer, "Common");
			MrsTracer.instanceResourceHealth = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationResourceHealthTracer, "ResourceHealth");
		}

		public static MrsTracer.MrsTracerInstance Service
		{
			get
			{
				return MrsTracer.instanceService;
			}
		}

		public static MrsTracer.MrsTracerInstance Provider
		{
			get
			{
				return MrsTracer.instanceProvider;
			}
		}

		public static MrsTracer.MrsTracerInstance Authorization
		{
			get
			{
				return MrsTracer.instanceAuthorization;
			}
		}

		public static MrsTracer.MrsTracerInstance ProxyService
		{
			get
			{
				return MrsTracer.instanceProxyService;
			}
		}

		public static MrsTracer.MrsTracerInstance ProxyClient
		{
			get
			{
				return MrsTracer.instanceProxyClient;
			}
		}

		public static MrsTracer.MrsTracerInstance Cmdlet
		{
			get
			{
				return MrsTracer.instanceCmdlet;
			}
		}

		public static MrsTracer.MrsTracerInstance UpdateMovedMailbox
		{
			get
			{
				return MrsTracer.instanceUpdateMovedMailbox;
			}
		}

		public static MrsTracer.MrsTracerInstance Throttling
		{
			get
			{
				return MrsTracer.instanceThrottling;
			}
		}

		public static MrsTracer.MrsTracerInstance Common
		{
			get
			{
				return MrsTracer.instanceCommon;
			}
		}

		public static MrsTracer.MrsTracerInstance ResourceHealth
		{
			get
			{
				return MrsTracer.instanceResourceHealth;
			}
		}

		public static int ActivityID
		{
			get
			{
				return MrsTracer.activityID;
			}
			set
			{
				MrsTracer.activityID = value;
			}
		}

		public const string MrsDbgCategory = "Microsoft.Exchange.MailboxReplicationService";

		[ThreadStatic]
		private static int activityID;

		private static MrsTracer.MrsTracerInstance instanceService = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationServiceTracer, "Service");

		private static MrsTracer.MrsTracerInstance instanceProvider = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationServiceProviderTracer, "Provider");

		private static MrsTracer.MrsTracerInstance instanceAuthorization = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationAuthorizationTracer, "Authorization");

		private static MrsTracer.MrsTracerInstance instanceProxyService = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationProxyServiceTracer, "ProxyService");

		private static MrsTracer.MrsTracerInstance instanceProxyClient = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationProxyClientTracer, "ProxyClient");

		private static MrsTracer.MrsTracerInstance instanceUpdateMovedMailbox = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationUpdateMovedMailboxTracer, "UpdateMovedMailbox");

		private static MrsTracer.MrsTracerInstance instanceThrottling = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationServiceThrottlingTracer, "Throttling");

		private static MrsTracer.MrsTracerInstance instanceCommon;

		private static MrsTracer.MrsTracerInstance instanceCmdlet = new MrsTracer.MrsTracerInstance(ExTraceGlobals.MailboxReplicationCmdletTracer, "Cmdlet");

		private static MrsTracer.MrsTracerInstance instanceResourceHealth;

		public class MrsTracerInstance
		{
			public MrsTracerInstance(Microsoft.Exchange.Diagnostics.Trace traceObj, string name)
			{
				this.Tracer = traceObj;
				this.Name = name;
			}

			public Microsoft.Exchange.Diagnostics.Trace Tracer { get; private set; }

			public string Name { get; private set; }

			public void Debug(string formatString, params object[] args)
			{
				this.TraceMessage(TraceType.DebugTrace, new Action<long, string, object[]>(this.Tracer.TraceDebug), formatString, args);
			}

			public void Warning(string formatString, params object[] args)
			{
				this.TraceMessage(TraceType.WarningTrace, new Action<long, string, object[]>(this.Tracer.TraceWarning), formatString, args);
			}

			public void Error(string formatString, params object[] args)
			{
				this.TraceMessage(TraceType.ErrorTrace, new Action<long, string, object[]>(this.Tracer.TraceError), formatString, args);
			}

			public void Function(string formatString, params object[] args)
			{
				this.TraceMessage(TraceType.FunctionTrace, new Action<long, string, object[]>(this.Tracer.TraceFunction), formatString, args);
			}

			public bool IsEnabled(TraceType traceType)
			{
				return this.Tracer.IsTraceEnabled(traceType) || this.IsTraceLoggingEnabled(traceType);
			}

			private bool IsTraceLoggingEnabled(TraceType traceType)
			{
				string config = ConfigBase<MRSConfigSchema>.GetConfig<string>("TraceLogLevels");
				if (!CommonUtils.IsValueInWildcardedList(traceType.ToString(), config))
				{
					return false;
				}
				string config2 = ConfigBase<MRSConfigSchema>.GetConfig<string>("TraceLogTracers");
				return CommonUtils.IsValueInWildcardedList(this.Name, config2);
			}

			private void TraceMessage(TraceType traceType, Action<long, string, object[]> traceFunction, string formatString, object[] args)
			{
				traceFunction((long)MrsTracer.ActivityID, formatString, args);
				this.TraceToDebugger(traceType, formatString, args);
				if (this.IsTraceLoggingEnabled(traceType))
				{
					TraceLog.Write(this.Name, traceType, string.Format(formatString, args));
				}
			}

			private void TraceToDebugger(TraceType traceType, string formatString, object[] args)
			{
				if (!Debugger.IsAttached)
				{
					return;
				}
				ExDateTime now = ExDateTime.Now;
				string text;
				switch (traceType)
				{
				case TraceType.DebugTrace:
					text = "D";
					goto IL_5E;
				case TraceType.WarningTrace:
					text = "W";
					goto IL_5E;
				case TraceType.ErrorTrace:
					text = "E";
					goto IL_5E;
				case TraceType.FunctionTrace:
					text = "F";
					goto IL_5E;
				}
				text = "x";
				IL_5E:
				string message = string.Format("[{0:X8}] {1:D2}:{2:D2}:{3:D2}.{4:D3} {5} ", new object[]
				{
					MrsTracer.ActivityID,
					now.Hour,
					now.Minute,
					now.Second,
					now.Millisecond,
					text
				}) + string.Format(formatString, args) + "\n";
				lock (MrsTracer.MrsTracerInstance.locker)
				{
					Debugger.Log((int)traceType, "Microsoft.Exchange.MailboxReplicationService", message);
				}
			}

			private static object locker = new object();
		}
	}
}
