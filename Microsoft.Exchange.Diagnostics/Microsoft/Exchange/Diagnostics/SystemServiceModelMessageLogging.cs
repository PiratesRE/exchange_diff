using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class SystemServiceModelMessageLogging : SystemTrace
	{
		private SystemServiceModelMessageLogging() : base("System.ServiceModel")
		{
			this.listener = new ExTraceListener(5, "System.ServiceModel_MessageLogging");
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

		public bool LogMalformedMessages
		{
			get
			{
				return this.logMalformedMessages;
			}
			set
			{
				this.logMalformedMessages = value;
				base.SafeUpdate();
			}
		}

		public bool LogMessagesAtServiceLevel
		{
			get
			{
				return this.logMessagesAtServiceLevel;
			}
			set
			{
				this.logMessagesAtServiceLevel = value;
				base.SafeUpdate();
			}
		}

		public bool LogMessagesAtTransportLevel
		{
			get
			{
				return this.logMessagesAtTransportLevel;
			}
			set
			{
				this.logMessagesAtTransportLevel = value;
				base.SafeUpdate();
			}
		}

		public bool LogMessageBody
		{
			get
			{
				return this.logMessageBody;
			}
			set
			{
				this.logMessageBody = value;
				base.SafeUpdate();
			}
		}

		protected override void Update()
		{
			bool flag = this.Enabled && (this.logMalformedMessages || this.logMessagesAtServiceLevel || this.logMessagesAtTransportLevel || this.logMessageBody);
			base.ConnectListener(this.source, this.listener, flag);
			this.source.Switch.Level = (flag ? base.SourceLevels : SourceLevels.Off);
			SystemTrace.SetPropertyValue(this.propertyLogMalformedMessages, this.logMalformedMessages);
			SystemTrace.SetPropertyValue(this.propertyLogMessagesAtServiceLevel, this.logMessagesAtServiceLevel);
			SystemTrace.SetPropertyValue(this.propertyLogMessagesAtTransportLevel, this.logMessagesAtTransportLevel);
			SystemTrace.SetPropertyValue(this.propertyLogMessageBody, this.logMessageBody);
		}

		protected override bool Initialize(Assembly assembly)
		{
			Type type = assembly.GetType("System.ServiceModel.Diagnostics.MessageLogger");
			if (type == null)
			{
				SystemServiceModelMessageLogging.ReportFailure("type 'System.ServiceModel.Diagnostics.MessageLogger' not found in assembly " + assembly.FullName);
				return false;
			}
			FieldInfo declaredField = type.GetTypeInfo().GetDeclaredField("messageTraceSource");
			if (declaredField == null || declaredField.IsPublic || !declaredField.IsStatic)
			{
				SystemServiceModelMessageLogging.ReportFailure("field 'messageTraceSource' not found in type " + type.Name);
				return false;
			}
			Assembly assembly2 = SystemTrace.GetAssembly("SMDiagnostics");
			if (assembly2 == null)
			{
				SystemServiceModelMessageLogging.ReportFailure("assembly 'SMDiagnostics' not found");
				return false;
			}
			Type type2 = assembly2.GetType("System.ServiceModel.Diagnostics.PiiTraceSource");
			if (type2 == null)
			{
				SystemServiceModelMessageLogging.ReportFailure("type 'System.ServiceModel.Diagnostics.PiiTraceSource' not found in assembly " + assembly2.FullName);
				return false;
			}
			ConstructorInfo constructorInfo = null;
			foreach (ConstructorInfo constructorInfo2 in type2.GetTypeInfo().DeclaredConstructors)
			{
				if (!constructorInfo2.IsPublic)
				{
					ParameterInfo[] parameters = constructorInfo2.GetParameters();
					if (parameters != null && parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(string))
					{
						constructorInfo = constructorInfo2;
						break;
					}
				}
			}
			if (constructorInfo == null)
			{
				SystemServiceModelMessageLogging.ReportFailure("instance constructor 'System.ServiceModel.Diagnostics.PiiTraceSource(string,string)' not found");
				return false;
			}
			object obj = constructorInfo.Invoke(new object[]
			{
				"System.ServiceModel.MessageLogging",
				"System.ServiceModel 4.0.0.0"
			});
			this.source = (obj as TraceSource);
			if (this.source == null)
			{
				SystemServiceModelMessageLogging.ReportFailure("instance constructor 'System.ServiceModel.Diagnostics.PiiTraceSource(string,string)' did not return TraceSource object");
				return false;
			}
			this.source.Switch.Level = SourceLevels.Off;
			this.source.Listeners.Remove("Default");
			declaredField.SetValue(null, this.source);
			TypeInfo typeInfo = type.GetTypeInfo();
			this.propertyLogMalformedMessages = typeInfo.GetDeclaredProperty("LogMalformedMessages");
			this.propertyLogMessagesAtServiceLevel = typeInfo.GetDeclaredProperty("LogMessagesAtServiceLevel");
			this.propertyLogMessagesAtTransportLevel = typeInfo.GetDeclaredProperty("LogMessagesAtTransportLevel");
			this.propertyLogMessageBody = typeInfo.GetDeclaredProperty("LogMessageBody");
			SystemServiceModelMessageLogging.SetMinimum(type, "MaxMessageSize", 262144);
			SystemServiceModelMessageLogging.SetMinimum(type, "MaxNumberOfMessagesToLog", 10000);
			return true;
		}

		private static void SetMinimum(Type type, string propertyName, int defaultValue)
		{
			PropertyInfo declaredProperty = type.GetTypeInfo().GetDeclaredProperty(propertyName);
			if (declaredProperty != null)
			{
				int num = (int)declaredProperty.GetValue(declaredProperty, null);
				if (num < defaultValue)
				{
					declaredProperty.SetValue(declaredProperty, defaultValue, null);
				}
			}
		}

		private static void ReportFailure(string failure)
		{
			ExTraceInternal.Trace<string>(0, TraceType.ErrorTrace, SystemLoggingTags.guid, 5, 0L, "Unable to initialize due the following failure: {0}", failure);
		}

		public static readonly SystemServiceModelMessageLogging Instance = new SystemServiceModelMessageLogging();

		private PropertyInfo propertyLogMalformedMessages;

		private PropertyInfo propertyLogMessagesAtServiceLevel;

		private PropertyInfo propertyLogMessagesAtTransportLevel;

		private PropertyInfo propertyLogMessageBody;

		private TraceSource source;

		private ExTraceListener listener;

		private bool enabled;

		private bool logMalformedMessages;

		private bool logMessagesAtServiceLevel;

		private bool logMessagesAtTransportLevel;

		private bool logMessageBody;
	}
}
