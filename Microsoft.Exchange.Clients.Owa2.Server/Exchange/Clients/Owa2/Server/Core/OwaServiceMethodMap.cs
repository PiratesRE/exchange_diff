using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaServiceMethodMap
	{
		internal OwaServiceMethodMap(Type contractType)
		{
			this.methodMap = OwaServiceMethodMap.Load(contractType);
			this.supportedMethods = OwaServiceMethodMap.LoadMethodSetFromWebConfig("OWAHttpHandlerMethods");
			this.unsupportedMethods = OwaServiceMethodMap.LoadMethodSetFromWebConfig("OWAHttpHandlerUnsupportedMethods");
			this.supportAllMethods = BaseApplication.GetAppSetting<bool>("OWAHttpHandlerSupportAllMethods", false);
		}

		internal bool TryGetMethodInfo(string methodName, out ServiceMethodInfo methodInfo)
		{
			if ((this.supportAllMethods || this.supportedMethods.Contains(methodName)) && !this.unsupportedMethods.Contains(methodName))
			{
				return this.methodMap.TryGetValue(methodName, out methodInfo);
			}
			methodInfo = null;
			return false;
		}

		internal static TResult HandleAsync<TResult>(Task<TResult> task)
		{
			return task.Result;
		}

		private static Dictionary<string, ServiceMethodInfo> Load(Type contractType)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Dictionary<string, List<Attribute>> dictionary = OwaServiceMethodMap.CollectCustomAttributes(contractType);
			MethodInfo[] methods = contractType.GetMethods();
			IEnumerable<MethodInfo> source = from method in methods
			where method.Name.StartsWith("End")
			select method;
			Dictionary<string, MethodInfo> endMethodMap = source.ToDictionary((MethodInfo methodInfo) => methodInfo.Name);
			Dictionary<string, ServiceMethodInfo> dictionary2 = new Dictionary<string, ServiceMethodInfo>();
			foreach (MethodInfo methodInfo2 in methods)
			{
				List<Attribute> attributes;
				if (dictionary.TryGetValue(methodInfo2.Name, out attributes))
				{
					OwaServiceMethodMap.ProcessMethod(methodInfo2, endMethodMap, dictionary2, attributes);
				}
			}
			stopwatch.Stop();
			ExTraceGlobals.CoreTracer.TraceDebug<long, string>(0L, "OwaServiceMethodMap.Load took {0} milliseconds to load methods for contract type {1}", stopwatch.ElapsedMilliseconds, contractType.Name);
			return dictionary2;
		}

		private static HashSet<string> LoadMethodSetFromWebConfig(string settingKey)
		{
			string appSetting = BaseApplication.GetAppSetting<string>(settingKey, string.Empty);
			HashSet<string> hashSet = new HashSet<string>();
			if (!string.IsNullOrWhiteSpace(appSetting))
			{
				foreach (string text in appSetting.Split(new char[]
				{
					','
				}))
				{
					if (!string.IsNullOrWhiteSpace(text))
					{
						hashSet.Add(text.Trim());
					}
				}
			}
			return hashSet;
		}

		private static Dictionary<string, List<Attribute>> CollectCustomAttributes(Type contractType)
		{
			Dictionary<string, List<Attribute>> dictionary = new Dictionary<string, List<Attribute>>();
			foreach (Type type in contractType.GetInterfaces())
			{
				OwaServiceMethodMap.CollectCustomAttributesForType(dictionary, type);
			}
			OwaServiceMethodMap.CollectCustomAttributesForType(dictionary, contractType);
			return dictionary;
		}

		private static void CollectCustomAttributesForType(Dictionary<string, List<Attribute>> dict, Type type)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
			foreach (MethodInfo methodInfo in type.GetMethods(bindingAttr))
			{
				foreach (Attribute item in methodInfo.GetCustomAttributes())
				{
					List<Attribute> list;
					if (dict.TryGetValue(methodInfo.Name, out list))
					{
						list.Add(item);
					}
					else
					{
						list = new List<Attribute>();
						list.Add(item);
						dict.Add(methodInfo.Name, list);
					}
				}
			}
		}

		private static TAttribute GetCustomAttribute<TAttribute>(List<Attribute> attributes) where TAttribute : Attribute
		{
			if (attributes == null)
			{
				return default(TAttribute);
			}
			return attributes.OfType<TAttribute>().FirstOrDefault<TAttribute>();
		}

		private static void ProcessMethod(MethodInfo methodInfo, Dictionary<string, MethodInfo> endMethodMap, Dictionary<string, ServiceMethodInfo> methodTable, List<Attribute> attributes)
		{
			OperationContractAttribute customAttribute = OwaServiceMethodMap.GetCustomAttribute<OperationContractAttribute>(attributes);
			if (customAttribute == null)
			{
				return;
			}
			WebInvokeAttribute customAttribute2 = OwaServiceMethodMap.GetCustomAttribute<WebInvokeAttribute>(attributes);
			JsonRequestFormatAttribute customAttribute3 = OwaServiceMethodMap.GetCustomAttribute<JsonRequestFormatAttribute>(attributes);
			WebGetAttribute customAttribute4 = OwaServiceMethodMap.GetCustomAttribute<WebGetAttribute>(attributes);
			OperationBehaviorAttribute customAttribute5 = OwaServiceMethodMap.GetCustomAttribute<OperationBehaviorAttribute>(attributes);
			JsonResponseOptionsAttribute customAttribute6 = OwaServiceMethodMap.GetCustomAttribute<JsonResponseOptionsAttribute>(attributes);
			JsonRequestWrapperTypeAttribute customAttribute7 = OwaServiceMethodMap.GetCustomAttribute<JsonRequestWrapperTypeAttribute>(attributes);
			AsyncStateMachineAttribute customAttribute8 = OwaServiceMethodMap.GetCustomAttribute<AsyncStateMachineAttribute>(attributes);
			bool flag = customAttribute != null && customAttribute.AsyncPattern;
			bool flag2 = customAttribute8 != null;
			bool flag3 = customAttribute5 == null || customAttribute5.AutoDisposeParameters;
			bool isResponseCacheable = customAttribute6 != null && customAttribute6.IsCacheable;
			WebMessageBodyStyle webMessageBodyStyle = (customAttribute2 != null) ? customAttribute2.BodyStyle : WebMessageBodyStyle.Bare;
			if (customAttribute2 != null)
			{
				WebMessageFormat requestFormat = customAttribute2.RequestFormat;
			}
			if (customAttribute2 != null)
			{
				WebMessageFormat responseFormat = customAttribute2.ResponseFormat;
			}
			JsonRequestFormat jsonRequestFormat = (customAttribute3 != null) ? customAttribute3.Format : JsonRequestFormat.Custom;
			bool isHttpGet = (customAttribute2 != null) ? customAttribute2.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase) : (customAttribute4 != null);
			string text = (customAttribute2 != null) ? customAttribute2.UriTemplate : ((customAttribute4 != null) ? customAttribute4.UriTemplate : null);
			UriTemplate uriTemplate = (!string.IsNullOrEmpty(text)) ? new UriTemplate(text) : null;
			bool flag4 = webMessageBodyStyle == WebMessageBodyStyle.WrappedRequest || webMessageBodyStyle == WebMessageBodyStyle.Wrapped;
			bool isWrappedResponse = webMessageBodyStyle == WebMessageBodyStyle.WrappedResponse || webMessageBodyStyle == WebMessageBodyStyle.Wrapped;
			WebMessageFormat webMethodRequestFormat = (customAttribute2 != null && customAttribute2.IsRequestFormatSetExplicitly) ? customAttribute2.RequestFormat : WebMessageFormat.Json;
			WebMessageFormat webMethodResponseFormat = (customAttribute2 != null && customAttribute2.IsResponseFormatSetExplicitly) ? customAttribute2.ResponseFormat : WebMessageFormat.Json;
			Type type = (customAttribute7 != null) ? customAttribute7.Type : null;
			string text2 = methodInfo.Name;
			MethodInfo beginMethod = null;
			MethodInfo methodInfo2 = null;
			MethodInfo syncMethod = null;
			MethodInfo genericAsyncTaskMethod = null;
			Type type2 = null;
			Type type3;
			if (text2.StartsWith("Begin", StringComparison.InvariantCultureIgnoreCase) && flag)
			{
				type3 = ((methodInfo.GetParameters().Length > 0) ? methodInfo.GetParameters()[0].ParameterType : null);
				beginMethod = methodInfo;
				text2 = text2.Substring("Begin".Length);
				string key = "End" + text2;
				if (endMethodMap.TryGetValue(key, out methodInfo2))
				{
					type2 = methodInfo2.ReturnType;
				}
			}
			else
			{
				syncMethod = methodInfo;
				type3 = ((methodInfo.GetParameters().Length > 0) ? methodInfo.GetParameters()[0].ParameterType : null);
				type2 = methodInfo.ReturnType;
				if (flag2 && type2 != null && type2.GenericTypeArguments != null && type2.GenericTypeArguments.Length > 0)
				{
					genericAsyncTaskMethod = OwaServiceMethodMap.handleAsyncMethodInfo.MakeGenericMethod(type2.GenericTypeArguments);
					type2 = type2.GenericTypeArguments[0];
				}
			}
			bool isStreamedResponse = OwaServiceMethodMap.IsStreamResponse(type2);
			bool shouldAutoDisposeResponse = flag3 && OwaServiceMethodMap.ImplementsInterface<IDisposable>(type2);
			bool shouldAutoDisposeRequest = flag3 && OwaServiceMethodMap.ImplementsInterface<IDisposable>(type3);
			if (flag4 && type == null)
			{
				string wrappedRequestTypeName = OwaServiceMethodMap.GetWrappedRequestTypeName(text2);
				type = OwaServiceMethodMap.thisAssembly.GetType(wrappedRequestTypeName, false);
			}
			ServiceMethodInfo value = new ServiceMethodInfo
			{
				BeginMethod = beginMethod,
				EndMethod = methodInfo2,
				GenericAsyncTaskMethod = genericAsyncTaskMethod,
				IsAsyncAwait = flag2,
				IsAsyncPattern = flag,
				IsHttpGet = isHttpGet,
				IsResponseCacheable = isResponseCacheable,
				IsStreamedResponse = isStreamedResponse,
				IsWrappedRequest = flag4,
				IsWrappedResponse = isWrappedResponse,
				JsonRequestFormat = jsonRequestFormat,
				Name = text2,
				RequestType = type3,
				ResponseType = type2,
				ShouldAutoDisposeRequest = shouldAutoDisposeRequest,
				ShouldAutoDisposeResponse = shouldAutoDisposeResponse,
				SyncMethod = syncMethod,
				UriTemplate = uriTemplate,
				WebMethodRequestFormat = webMethodRequestFormat,
				WebMethodResponseFormat = webMethodResponseFormat,
				WrappedRequestType = type,
				WrappedRequestTypeParameterMap = OwaServiceMethodMap.BuildParameterMap(type)
			};
			methodTable.Add(text2, value);
		}

		private static string GetWrappedRequestTypeName(string methodName)
		{
			return string.Format("{0}.{1}RequestWrapper", "Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers", methodName);
		}

		private static bool ImplementsInterface<TInterface>(Type type)
		{
			return type != null && type.GetInterfaces().Any((Type itype) => itype == typeof(TInterface));
		}

		private static bool IsStreamResponse(Type responseType)
		{
			return responseType != null && (responseType == typeof(Stream) || responseType.IsSubclassOf(typeof(Stream)));
		}

		private static Dictionary<string, string> BuildParameterMap(Type type)
		{
			if (type == null)
			{
				return null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (PropertyInfo propertyInfo in type.GetProperties())
			{
				string name = propertyInfo.Name;
				DataMemberAttribute customAttribute = propertyInfo.GetCustomAttribute<DataMemberAttribute>();
				if (customAttribute != null && !string.IsNullOrEmpty(customAttribute.Name))
				{
					name = customAttribute.Name;
				}
				dictionary.Add(name, propertyInfo.Name);
			}
			return dictionary;
		}

		private const string supportedMethodsKey = "OWAHttpHandlerMethods";

		private const string unsupportedMethodsKey = "OWAHttpHandlerUnsupportedMethods";

		private const string supportAllMethodsKey = "OWAHttpHandlerSupportAllMethods";

		private const string AsyncMethodBeginPrefix = "Begin";

		private const string AsyncMethodEndPrefix = "End";

		private const string WrappedRequestNamespace = "Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers";

		private static MethodInfo handleAsyncMethodInfo = typeof(OwaServiceMethodMap).GetMethod("HandleAsync", BindingFlags.Static | BindingFlags.NonPublic);

		private static Assembly thisAssembly = typeof(OwaServiceMethodMap).Assembly;

		private HashSet<string> unsupportedMethods;

		private readonly bool supportAllMethods;

		private HashSet<string> supportedMethods;

		private Dictionary<string, ServiceMethodInfo> methodMap;
	}
}
