using System;
using System.Reflection;
using System.Web;

namespace Microsoft.Exchange.RpcHttpModules
{
	internal static class NativeIisInteropBackend
	{
		public static void SetServerVariable(this HttpContextBase httpContext, string name, string value)
		{
			HttpWorkerRequest httpWorkerRequest = (HttpWorkerRequest)((IServiceProvider)httpContext).GetService(typeof(HttpWorkerRequest));
			if (httpWorkerRequest == null)
			{
				return;
			}
			if (NativeIisInteropBackend.setServerVariableMethod == null)
			{
				NativeIisInteropBackend.Initialize(httpWorkerRequest);
			}
			object[] parameters = new object[]
			{
				name,
				value
			};
			NativeIisInteropBackend.setServerVariableMethod.Invoke(httpWorkerRequest, parameters);
		}

		private static void Initialize(HttpWorkerRequest httpWorkerRequest)
		{
			lock (NativeIisInteropBackend.initializationLock)
			{
				BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
				NativeIisInteropBackend.setServerVariableMethod = httpWorkerRequest.GetType().GetMethod(NativeIisInteropBackend.setServerVariableMethodName, bindingAttr);
			}
		}

		private static readonly object initializationLock = new object();

		private static MethodInfo setServerVariableMethod;

		private static string setServerVariableMethodName = "SetServerVariable";
	}
}
