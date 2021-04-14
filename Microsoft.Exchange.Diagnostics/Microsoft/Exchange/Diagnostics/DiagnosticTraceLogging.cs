using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	internal sealed class DiagnosticTraceLogging : SystemTrace
	{
		private DiagnosticTraceLogging(string assemblyName, int traceTag, string typeName, string traceSourceName) : base(assemblyName)
		{
			this.traceTag = traceTag;
			this.typeName = typeName;
			this.traceSourceName = traceSourceName;
			this.listener = new ExTraceListener(this.traceTag, this.traceSourceName + "_Trace");
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

		protected override void Update()
		{
			base.ConnectListener(this.source, this.listener, this.enabled);
			SourceLevels sourceLevels = this.enabled ? base.SourceLevels : SourceLevels.Off;
			if (this.propertyLevel != null && this.methodUpdateLevel != null)
			{
				this.propertyLevel.SetValue(null, sourceLevels, null);
				this.methodUpdateLevel.Invoke(null, null);
			}
		}

		protected override bool Initialize(Assembly assembly)
		{
			Type type = assembly.GetType(this.typeName);
			if (type == null)
			{
				this.ReportFailure(string.Concat(new string[]
				{
					"type '",
					this.typeName,
					"' not found in assembly '",
					assembly.FullName,
					"'"
				}));
				return false;
			}
			TypeInfo typeInfo = type.GetTypeInfo();
			MethodInfo declaredMethod = typeInfo.GetDeclaredMethod("InitDiagnosticTraceImpl");
			if (declaredMethod == null || declaredMethod.IsPublic || !declaredMethod.IsStatic)
			{
				this.ReportFailure("static method 'InitDiagnosticTraceImpl' not found in type '" + type.FullName);
				return false;
			}
			declaredMethod.Invoke(null, new object[]
			{
				0,
				this.traceSourceName
			});
			this.methodUpdateLevel = typeInfo.GetDeclaredMethod("UpdateLevel");
			if (this.methodUpdateLevel == null || this.methodUpdateLevel.IsPublic || !this.methodUpdateLevel.IsStatic)
			{
				this.ReportFailure("static method 'UpdateLevel' not found in type '" + type.FullName + "'");
				return false;
			}
			this.propertyLevel = typeInfo.GetDeclaredProperty("Level");
			if (this.propertyLevel == null || this.propertyLevel.GetMethod == null || !this.propertyLevel.GetMethod.IsStatic)
			{
				this.ReportFailure("cannot find static property 'Level' in type '" + type.FullName + "'");
				return false;
			}
			PropertyInfo declaredProperty = typeInfo.GetDeclaredProperty("DiagnosticTrace");
			if (declaredProperty == null)
			{
				this.ReportFailure("static property 'DiagnosticTrace' not found in type '" + type.FullName + "'");
				return false;
			}
			object value = declaredProperty.GetValue(null, null);
			if (value == null)
			{
				this.ReportFailure("static property 'DiagnosticTrace' has null value");
				return false;
			}
			PropertyInfo propertyInfo = null;
			Type type2 = value.GetType();
			while (type2 != null && type2 != typeof(object))
			{
				TypeInfo typeInfo2 = type2.GetTypeInfo();
				propertyInfo = typeInfo2.GetDeclaredProperty("TraceSource");
				if (propertyInfo != null)
				{
					break;
				}
				type2 = typeInfo2.BaseType;
			}
			if (propertyInfo == null || propertyInfo.GetMethod == null)
			{
				this.ReportFailure("readable instance property 'TraceSource' not found in '" + type2.FullName + "' type");
				return false;
			}
			this.source = (propertyInfo.GetValue(value, null) as TraceSource);
			if (this.source == null)
			{
				this.ReportFailure("instance property 'TraceSource' does not return object of 'TraceSource' type");
				return false;
			}
			return true;
		}

		private void ReportFailure(string failure)
		{
			InternalBypassTrace.TracingConfigurationTracer.TraceError(0, (long)this.traceTag, "Unable to initialize due the following failure: {0}", new object[]
			{
				failure
			});
		}

		public static readonly DiagnosticTraceLogging SystemIdentityModel = new DiagnosticTraceLogging("System.IdentityModel", 3, "System.IdentityModel.DiagnosticUtility", "System.IdentityModel");

		public static readonly DiagnosticTraceLogging SystemServiceModel = new DiagnosticTraceLogging("System.ServiceModel", 4, "System.ServiceModel.DiagnosticUtility", "System.ServiceModel");

		private int traceTag;

		private string typeName;

		private string traceSourceName;

		private ExTraceListener listener;

		private MethodInfo methodUpdateLevel;

		private PropertyInfo propertyLevel;

		private bool enabled;

		private TraceSource source;
	}
}
