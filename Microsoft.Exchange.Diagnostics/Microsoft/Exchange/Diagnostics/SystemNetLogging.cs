using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class SystemNetLogging : SystemTrace
	{
		private SystemNetLogging() : base("System")
		{
			this.socketsListener = new ExTraceListener(1, "Sockets");
			this.webListener = new ExTraceListener(0, "Web");
			this.httpListenerListener = new ExTraceListener(2, "HttpListener");
			base.Initialize();
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				base.SafeUpdate();
			}
		}

		internal void AddHttpListenerExtendedErrorListener(TraceListener extendedErrorListener)
		{
			lock (this)
			{
				if (this.httpListenerExtendedErrorListener != null)
				{
					throw new InvalidOperationException("Only a single extended error listener allowed");
				}
				this.httpListenerExtendedErrorListener = extendedErrorListener;
			}
			base.SafeUpdate();
		}

		internal void RemoveHttpListenerExtendedErrorListener(TraceListener extendedErrorListener)
		{
			lock (this)
			{
				if (this.httpListenerExtendedErrorListener != null)
				{
					base.ConnectListener(this.httpListenerSource, this.httpListenerExtendedErrorListener, false);
					this.httpListenerExtendedErrorListener = null;
				}
			}
			base.SafeUpdate();
		}

		protected override void Update()
		{
			bool flag = this.httpListenerExtendedErrorListener != null;
			SystemTrace.SetFieldValue(this.fieldEnabled, flag || this.enabled);
			SourceLevels sourceLevels = this.enabled ? base.SourceLevels : SourceLevels.Off;
			this.socketsSource.Switch.Level = sourceLevels;
			this.webSource.Switch.Level = sourceLevels;
			this.httpListenerSource.Switch.Level = ((flag && (sourceLevels == SourceLevels.Off || sourceLevels == SourceLevels.Critical)) ? SourceLevels.Error : sourceLevels);
			base.ConnectListener(this.socketsSource, this.socketsListener, this.enabled);
			base.ConnectListener(this.webSource, this.webListener, this.enabled);
			base.ConnectListener(this.httpListenerSource, this.httpListenerListener, this.enabled);
			if (flag)
			{
				base.ConnectListener(this.httpListenerSource, this.httpListenerExtendedErrorListener, true);
			}
		}

		protected override bool Initialize(Assembly assembly)
		{
			Type type = assembly.GetType("System.Net.Logging");
			if (type == null)
			{
				string failure = "type 'System.Net.Logging' not found in assembly '" + assembly.FullName + "'";
				SystemNetLogging.ReportFailure(1, failure);
				SystemNetLogging.ReportFailure(0, failure);
				return false;
			}
			this.fieldEnabled = type.GetTypeInfo().GetDeclaredField("s_LoggingEnabled");
			if (this.fieldEnabled == null || this.fieldEnabled.IsPublic || !this.fieldEnabled.IsStatic)
			{
				string failure2 = "static field 's_LoggingEnabled' not found in type 'System.Net.Logging'";
				SystemNetLogging.ReportFailure(1, failure2);
				SystemNetLogging.ReportFailure(0, failure2);
				return false;
			}
			SystemTrace.SetFieldValue(this.fieldEnabled, false);
			PropertyInfo declaredProperty = type.GetTypeInfo().GetDeclaredProperty("On");
			if (declaredProperty == null)
			{
				string failure3 = "static property 'On' not found in 'System.Net.Logging' type";
				SystemNetLogging.ReportFailure(1, failure3);
				SystemNetLogging.ReportFailure(0, failure3);
				return false;
			}
			declaredProperty.GetValue(null, null);
			this.socketsSource = SystemNetLogging.GetTraceSource(1, "s_SocketsTraceSource", type);
			this.webSource = SystemNetLogging.GetTraceSource(0, "s_WebTraceSource", type);
			this.httpListenerSource = SystemNetLogging.GetTraceSource(2, "s_HttpListenerTraceSource", type);
			return this.webSource != null || this.socketsSource != null || this.httpListenerSource != null;
		}

		private static TraceSource GetTraceSource(int traceTag, string fieldName, Type type)
		{
			FieldInfo declaredField = type.GetTypeInfo().GetDeclaredField(fieldName);
			if (declaredField == null || declaredField.IsPublic || !declaredField.IsStatic)
			{
				SystemNetLogging.ReportFailure(traceTag, string.Concat(new string[]
				{
					"static field '",
					fieldName,
					"' not found in '",
					type.Name,
					"' type"
				}));
				return null;
			}
			TraceSource traceSource = declaredField.GetValue(null) as TraceSource;
			if (traceSource == null)
			{
				SystemNetLogging.ReportFailure(traceTag, "static field '" + fieldName + "' does not contain object of 'TraceSource' type");
				return null;
			}
			traceSource.Listeners.Remove("Default");
			traceSource.Switch.Level = SourceLevels.Off;
			traceSource.SetMaxDataSize(1073741824);
			return traceSource;
		}

		private static void ReportFailure(int traceTag, string failure)
		{
			ExTraceInternal.Trace<string>(0, TraceType.ErrorTrace, SystemLoggingTags.guid, traceTag, 0L, "Unable to initialize due the following failure: {0}", failure);
		}

		private const int MaxDataSize = 1073741824;

		public static readonly SystemNetLogging Instance = new SystemNetLogging();

		private bool enabled;

		private FieldInfo fieldEnabled;

		private TraceSource socketsSource;

		private TraceSource webSource;

		private TraceSource httpListenerSource;

		private ExTraceListener socketsListener;

		private ExTraceListener webListener;

		private ExTraceListener httpListenerListener;

		private TraceListener httpListenerExtendedErrorListener;
	}
}
