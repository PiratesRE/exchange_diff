using System;
using System.Reflection;
using Microsoft.Exchange.Data.ApplicationLogic.CommonHandlers;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	public class ExchangeDiagnosticsHelper
	{
		internal static void RegisterDiagnosticsComponents()
		{
			ExchangeDiagnosticsSection config = ExchangeDiagnosticsSection.GetConfig();
			ExTraceGlobals.CommonTracer.TraceInformation<int>(0, 0L, "ExchangeDiagnosticsHelper::RegisterDiagnosticsComponents called. No of Component:{0}", config.DiagnosticComponents.Count);
			foreach (object obj in config.DiagnosticComponents)
			{
				DiagnosticsComponent diagnosticsComponent = (DiagnosticsComponent)obj;
				ExTraceGlobals.CommonTracer.TraceDebug(0L, "ExchangeDiagnosticsHelper::RegisterDiagnosticsComponents ComponentName:{0}, Type:{1}, MethodName:{2}, Argument:{3}", new object[]
				{
					diagnosticsComponent.Name,
					diagnosticsComponent.Implementation,
					diagnosticsComponent.MethodName,
					diagnosticsComponent.Argument
				});
				IDiagnosable instance = ExchangeDiagnosticsHelper.GetInstance(diagnosticsComponent);
				if (diagnosticsComponent.Data != null)
				{
					IDiagnosableExtraData diagnosableExtraData = instance as IDiagnosableExtraData;
					if (diagnosableExtraData != null)
					{
						diagnosableExtraData.SetData(diagnosticsComponent.Data);
					}
				}
				ExchangeDiagnosticsHelper.RegisterDiagnosticsComponents(instance);
			}
		}

		internal static void RegisterDiagnosticsComponents(IDiagnosable instance)
		{
			if (instance != null)
			{
				try
				{
					ProcessAccessManager.RegisterComponent(instance);
				}
				catch (RpcException ex)
				{
					FaultDiagnosticsComponent faultDiagnosticsComponent = new FaultDiagnosticsComponent();
					faultDiagnosticsComponent.SetComponentNameAndMessage(instance.GetDiagnosticComponentName(), -998, ex.ToString());
					ExTraceGlobals.CommonTracer.TraceError<string>(0L, "ExchangeDiagnosticsHelper::RegisterDiagnosticsComponents Exception:{0}", ex.ToString());
					try
					{
						ProcessAccessManager.RegisterComponent(faultDiagnosticsComponent);
					}
					catch (RpcException ex2)
					{
						ExTraceGlobals.CommonTracer.TraceError<string>(0L, "ExchangeDiagnosticsHelper::RegisterDiagnosticsComponents Exception while registering FaultDiagnosticsComponent. {0}", ex2.ToString());
					}
				}
			}
		}

		internal static void UnRegisterDiagnosticsComponents()
		{
			ExchangeDiagnosticsSection config = ExchangeDiagnosticsSection.GetConfig();
			ExTraceGlobals.CommonTracer.TraceInformation<int>(0, 0L, "ExchangeDiagnosticsHelper::UnRegisterDiagnosticsComponents called. No of Component:{0}", config.DiagnosticComponents.Count);
			foreach (object obj in config.DiagnosticComponents)
			{
				DiagnosticsComponent diagnosticsComponent = (DiagnosticsComponent)obj;
				IDiagnosable instance = ExchangeDiagnosticsHelper.GetInstance(diagnosticsComponent);
				ExTraceGlobals.CommonTracer.TraceDebug(0L, "ExchangeDiagnosticsHelper::UnRegisterDiagnosticsComponents ComponentName:{0}, Type:{1}, MethodName:{2}, Argument:{3}", new object[]
				{
					diagnosticsComponent.Name,
					diagnosticsComponent.Implementation,
					diagnosticsComponent.MethodName,
					diagnosticsComponent.Argument
				});
				ExchangeDiagnosticsHelper.UnRegisterDiagnosticsComponents(instance);
			}
		}

		internal static void UnRegisterDiagnosticsComponents(IDiagnosable component)
		{
			if (component != null)
			{
				try
				{
					ProcessAccessManager.UnregisterComponent(component);
				}
				catch (RpcException ex)
				{
					ExTraceGlobals.CommonTracer.TraceError<string>(0L, "ExchangeDiagnosticsHelper::UnRegisterDiagnosticsComponents. Exception:{0}", ex.ToString());
				}
			}
		}

		private static IDiagnosable GetInstance(DiagnosticsComponent component)
		{
			Type type = Type.GetType(component.Implementation, false);
			IDiagnosable result = null;
			if (type != null)
			{
				MethodInfo method = type.GetMethod(component.MethodName, BindingFlags.Static | BindingFlags.Public);
				if (!string.IsNullOrEmpty(component.Argument))
				{
					result = (method.Invoke(null, new object[]
					{
						component.Argument
					}) as IDiagnosable);
				}
				else
				{
					result = (method.Invoke(null, null) as IDiagnosable);
				}
				ExTraceGlobals.CommonTracer.TraceDebug<string>(0L, "ExchangeDiagnosticsHelper::GetInstance - instance of {0} created successfully", component.Implementation);
			}
			else
			{
				ExTraceGlobals.CommonTracer.TraceDebug<string>(0L, "ExchangeDiagnosticsHelper::GetInstance - could not find {0}", component.Implementation);
			}
			return result;
		}
	}
}
