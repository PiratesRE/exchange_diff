using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaServiceMethodDispatcher
	{
		internal OwaServiceMethodDispatcher(IOwaServiceMessageInspector inspector)
		{
			this.inspector = inspector;
		}

		private static Dictionary<string, object> ConvertRequestToParameterDictionary(ServiceMethodInfo methodInfo, IEnumerable<ParameterInfo> parameters, Stream requestStream)
		{
			Type wrappedRequestType = methodInfo.WrappedRequestType;
			if (wrappedRequestType != null)
			{
				OwaServiceMethodDispatcher.CreateJsonSerializer(wrappedRequestType);
				object wrapperObject = OwaServiceMethodDispatcher.ReadJsonObject(wrappedRequestType, requestStream);
				return OwaServiceMethodDispatcher.ConvertWrappedObjectToParameterDictionary(parameters, methodInfo.WrappedRequestTypeParameterMap, wrapperObject);
			}
			IEnumerable<Type> enumerable;
			if (parameters == null)
			{
				enumerable = null;
			}
			else
			{
				enumerable = parameters.ToList<ParameterInfo>().ConvertAll<Type>((ParameterInfo p) => p.ParameterType);
			}
			IEnumerable<Type> knownTypes = enumerable;
			DataContractJsonSerializer dataContractJsonSerializer = OwaServiceMethodDispatcher.CreateSimpleDictionaryJsonSerializer(knownTypes);
			return (Dictionary<string, object>)dataContractJsonSerializer.ReadObject(requestStream);
		}

		private static Dictionary<string, object> ConvertWrappedObjectToParameterDictionary(IEnumerable<ParameterInfo> parameters, Dictionary<string, string> parameterMap, object wrapperObject)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Type type = wrapperObject.GetType();
			foreach (ParameterInfo parameterInfo in parameters)
			{
				string name = parameterInfo.Name;
				parameterMap.TryGetValue(parameterInfo.Name, out name);
				object obj = null;
				PropertyInfo property = type.GetProperty(name);
				if (property != null)
				{
					obj = property.GetValue(wrapperObject);
				}
				if (obj != null)
				{
					dictionary.Add(parameterInfo.Name, obj);
				}
			}
			return dictionary;
		}

		private static void WriteResponse(ServiceMethodInfo methodInfo, HttpResponse httpResponse, object response)
		{
			if (response != null)
			{
				if (methodInfo.IsStreamedResponse)
				{
					httpResponse.Buffer = false;
					Stream stream = response as Stream;
					stream.CopyTo(httpResponse.OutputStream);
					httpResponse.OutputStream.Flush();
					return;
				}
				if (methodInfo.IsWrappedResponse)
				{
					Dictionary<string, object> graph = new Dictionary<string, object>
					{
						{
							methodInfo.Name + "Result",
							response
						}
					};
					DataContractJsonSerializer dataContractJsonSerializer = OwaServiceMethodDispatcher.CreateSimpleDictionaryJsonSerializer(new Type[]
					{
						response.GetType()
					});
					dataContractJsonSerializer.WriteObject(httpResponse.OutputStream, graph);
					return;
				}
				DataContractJsonSerializer dataContractJsonSerializer2 = OwaServiceMethodDispatcher.CreateJsonSerializer(methodInfo.ResponseType);
				dataContractJsonSerializer2.WriteObject(httpResponse.OutputStream, response);
			}
		}

		private static object ConvertStringToParameterValue(string strValue, ParameterInfo parameter)
		{
			try
			{
				if (parameter.ParameterType == typeof(string))
				{
					return strValue;
				}
				if (parameter.ParameterType.IsEnum)
				{
					return Enum.Parse(parameter.ParameterType, strValue);
				}
				if (parameter.ParameterType.IsValueType)
				{
					return Convert.ChangeType(strValue, parameter.ParameterType);
				}
				string s = "\"" + strValue + "\"";
				using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
				{
					return OwaServiceMethodDispatcher.ReadJsonObject(parameter.ParameterType, memoryStream);
				}
			}
			catch (InvalidCastException arg)
			{
				ExTraceGlobals.CoreTracer.TraceError<string, InvalidCastException>(0L, "Cast error occurred while converting string value to parameter {0}. Exception: {1}", parameter.Name, arg);
			}
			catch (FormatException arg2)
			{
				ExTraceGlobals.CoreTracer.TraceError<string, FormatException>(0L, "Format error occurred while converting string value to parameter {0}. Exception: {1}", parameter.Name, arg2);
			}
			catch (OverflowException arg3)
			{
				ExTraceGlobals.CoreTracer.TraceError<string, OverflowException>(0L, "Overflow error occurred while converting string value to parameter {0}. Exception: {1}", parameter.Name, arg3);
			}
			return null;
		}

		private static object ReadJsonObject(Type objectType, Stream stream)
		{
			object result;
			try
			{
				DataContractJsonSerializer dataContractJsonSerializer = OwaServiceMethodDispatcher.CreateJsonSerializer(objectType);
				result = dataContractJsonSerializer.ReadObject(stream);
			}
			catch (SerializationException ex)
			{
				string arg = OwaServiceMethodDispatcher.TryGetJsonContentFromStream(stream, 2048);
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("OwaServiceMethodDispatcher", null, "ReadJsonObject", string.Format("Type: {0} Exception: {1}, JSON: {2}", objectType.Name, ex.Message, arg)));
				throw new OwaSerializationException(string.Format("Cannot deserialize object of type {0}", objectType.Name), ex);
			}
			return result;
		}

		private static string TryGetJsonContentFromStream(Stream stream, int maxContentLength)
		{
			string result = string.Empty;
			try
			{
				if (stream.CanSeek)
				{
					stream.Position = 0L;
					using (StreamReader streamReader = new StreamReader(stream))
					{
						char[] array = new char[maxContentLength];
						int length = streamReader.Read(array, 0, array.Length);
						result = new string(array, 0, length);
					}
				}
			}
			catch (Exception arg)
			{
				ExTraceGlobals.CoreTracer.TraceError<Exception>(0L, "Could not retrieve JSON content from stream for diagnostics. Exception: {0}", arg);
			}
			return result;
		}

		private static object InvokeMethod(HttpRequest request, MethodInfo methodInfo, object obj, params object[] parameters)
		{
			object result;
			try
			{
				result = methodInfo.Invoke(obj, parameters);
			}
			catch (ArgumentException ex)
			{
				string arg = OwaServiceMethodDispatcher.TryGetJsonContentFromStream(request.InputStream, 2048);
				OwaServerTraceLogger.AppendToLog(new TraceLogEvent("OwaServiceMethodDispatcher", null, "InvokeMethod", string.Format("Method: {0} Exception: {1}, JSON: {2}", methodInfo.Name, ex.Message, arg)));
				throw new OwaMethodArgumentException(string.Format("Invalid argument used to call method {0}", methodInfo.Name), ex);
			}
			return result;
		}

		private static object[] CreateMethodArgumentsFromUri(ServiceMethodInfo methodInfo, HttpRequest httpRequest)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (methodInfo.UriTemplate != null)
			{
				Uri baseAddress = new Uri(httpRequest.Url, httpRequest.Path);
				Uri url = httpRequest.Url;
				UriTemplateMatch uriTemplateMatch = methodInfo.UriTemplate.Match(baseAddress, url);
				foreach (string text in uriTemplateMatch.BoundVariables.AllKeys)
				{
					dictionary.Add(text, uriTemplateMatch.BoundVariables[text]);
				}
			}
			else
			{
				foreach (string text2 in httpRequest.QueryString.AllKeys)
				{
					dictionary.Add(text2, httpRequest.QueryString[text2]);
				}
			}
			MethodInfo methodInfo2 = methodInfo.IsAsyncPattern ? methodInfo.BeginMethod : methodInfo.SyncMethod;
			ParameterInfo[] parameters = methodInfo2.GetParameters();
			int num = parameters.Length;
			if (methodInfo.IsAsyncPattern)
			{
				num -= 2;
			}
			object[] array = (num > 0) ? new object[num] : null;
			for (int k = 0; k < num; k++)
			{
				object obj = null;
				if (dictionary != null)
				{
					ParameterInfo parameterInfo = parameters[k];
					string text3 = null;
					if (dictionary.TryGetValue(parameterInfo.Name, out text3) && !string.IsNullOrEmpty(text3))
					{
						obj = OwaServiceMethodDispatcher.ConvertStringToParameterValue(text3, parameterInfo);
					}
				}
				array[k] = obj;
			}
			return array;
		}

		private static object[] CreateMethodArgumentsFromWrappedRequest(ServiceMethodInfo methodInfo, HttpRequest httpRequest)
		{
			ParameterInfo[] parameters = methodInfo.SyncMethod.GetParameters();
			Dictionary<string, object> dictionary = OwaServiceMethodDispatcher.ConvertRequestToParameterDictionary(methodInfo, parameters, httpRequest.InputStream);
			object[] array = (parameters.Length > 0) ? new object[parameters.Length] : null;
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				object obj = null;
				dictionary.TryGetValue(parameterInfo.Name, out obj);
				if (obj is object[] && parameterInfo.ParameterType.IsArray)
				{
					obj = OwaServiceMethodDispatcher.ConvertObjectArrayToTypedArray(obj, parameterInfo.ParameterType);
				}
				array[i] = obj;
			}
			return array;
		}

		private static object ConvertObjectArrayToTypedArray(object value, Type arrayType)
		{
			object[] array = value as object[];
			Type elementType = arrayType.GetElementType();
			if (array != null)
			{
				Array array2 = Array.CreateInstance(elementType, array.Length);
				array.CopyTo(array2, 0);
				value = array2;
			}
			return value;
		}

		private static object[] CreateMethodArgumentsFromRequest(ServiceMethodInfo methodInfo, HttpRequest httpRequest)
		{
			object[] result = null;
			if (methodInfo.IsWrappedRequest)
			{
				result = OwaServiceMethodDispatcher.CreateMethodArgumentsFromWrappedRequest(methodInfo, httpRequest);
			}
			else if (methodInfo.RequestType != null)
			{
				object obj = OwaServiceMethodDispatcher.ReadJsonObject(methodInfo.RequestType, httpRequest.InputStream);
				result = new object[]
				{
					obj
				};
			}
			return result;
		}

		private static DataContractJsonSerializer CreateJsonSerializer(Type objectType)
		{
			DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
			{
				MaxItemsInObjectGraph = int.MaxValue
			};
			return new DataContractJsonSerializer(objectType, settings);
		}

		private static DataContractJsonSerializer CreateSimpleDictionaryJsonSerializer(IEnumerable<Type> knownTypes)
		{
			DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings
			{
				MaxItemsInObjectGraph = int.MaxValue,
				UseSimpleDictionaryFormat = true,
				KnownTypes = knownTypes
			};
			return new DataContractJsonSerializer(typeof(Dictionary<string, object>), settings);
		}

		private static void DisposeObjects(params object[] objs)
		{
			foreach (object obj in objs)
			{
				IDisposable disposable = obj as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private void InternalInvokeMethod(ServiceMethodInfo methodInfo, object service, HttpRequest httpRequest, HttpResponse httpResponse, object[] arguments)
		{
			object request = (arguments != null) ? arguments[0] : null;
			this.inspector.AfterReceiveRequest(httpRequest, methodInfo.Name, request);
			if (methodInfo.ShouldAutoDisposeRequest && arguments != null)
			{
				this.delayedDisposalRequestObjects = arguments;
			}
			object response = null;
			using (CpuTracker.StartCpuTracking("EXEC"))
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					response = this.InvokeSyncMethod(httpRequest, methodInfo, service, arguments);
				}, new Func<Exception, bool>(this.CanIgnoreExceptionForWatsonReport));
			}
			if (methodInfo.ShouldAutoDisposeResponse && response != null)
			{
				this.delayedDisposalResponseObject = response;
			}
			using (CpuTracker.StartCpuTracking("WRITE"))
			{
				OwaServiceMethodDispatcher.WriteResponse(methodInfo, httpResponse, response);
			}
		}

		private object InvokeSyncMethod(HttpRequest request, ServiceMethodInfo methodInfo, object service, object[] arguments)
		{
			if (methodInfo.IsAsyncAwait)
			{
				object obj = OwaServiceMethodDispatcher.InvokeMethod(request, methodInfo.SyncMethod, service, arguments);
				return methodInfo.GenericAsyncTaskMethod.Invoke(null, new object[]
				{
					obj
				});
			}
			return OwaServiceMethodDispatcher.InvokeMethod(request, methodInfo.SyncMethod, service, arguments);
		}

		private bool CanIgnoreExceptionForWatsonReport(Exception exception)
		{
			if (OwaDiagnostics.CanIgnoreExceptionForWatsonReport(exception))
			{
				return true;
			}
			TargetInvocationException ex = exception as TargetInvocationException;
			return ex != null && ex.InnerException != null && OwaDiagnostics.CanIgnoreExceptionForWatsonReport(ex.InnerException);
		}

		private IAsyncResult InternalInvokeBeginMethod(ServiceMethodInfo methodInfo, object service, HttpRequest httpRequest, AsyncCallback asyncCallback, object[] arguments)
		{
			int num = (arguments != null) ? arguments.Length : 0;
			object request = (num > 0) ? arguments[0] : null;
			this.inspector.AfterReceiveRequest(httpRequest, methodInfo.Name, request);
			if (methodInfo.ShouldAutoDisposeRequest && arguments != null)
			{
				this.delayedDisposalRequestObjects = arguments;
			}
			IAsyncResult result = null;
			using (CpuTracker.StartCpuTracking("BEGIN"))
			{
				object[] invokeArgs = this.ConstructAsyncInvokeArguments(arguments, asyncCallback);
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					result = (IAsyncResult)OwaServiceMethodDispatcher.InvokeMethod(httpRequest, methodInfo.BeginMethod, service, invokeArgs);
				}, new Func<Exception, bool>(this.CanIgnoreExceptionForWatsonReport));
			}
			return result;
		}

		private object[] ConstructAsyncInvokeArguments(object[] arguments, AsyncCallback asyncCallback)
		{
			object[] array2;
			if (arguments == null || arguments.Length == 0)
			{
				object[] array = new object[2];
				array[0] = asyncCallback;
				array2 = array;
			}
			else if (arguments.Length == 1)
			{
				object[] array3 = new object[3];
				array3[0] = arguments[0];
				array3[1] = asyncCallback;
				array2 = array3;
			}
			else
			{
				array2 = new object[arguments.Length + 2];
				arguments.CopyTo(array2, 0);
				array2[arguments.Length] = asyncCallback;
				array2[arguments.Length + 1] = null;
			}
			return array2;
		}

		internal void InvokeMethod(ServiceMethodInfo methodInfo, object service, HttpRequest httpRequest, HttpResponse httpResponse)
		{
			object[] arguments = OwaServiceMethodDispatcher.CreateMethodArgumentsFromRequest(methodInfo, httpRequest);
			this.InternalInvokeMethod(methodInfo, service, httpRequest, httpResponse, arguments);
		}

		internal void InvokeGetMethod(ServiceMethodInfo methodInfo, object service, HttpRequest httpRequest, HttpResponse httpResponse)
		{
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "OwaServiceMethodDispatcher.InvokeGetMethod");
			object[] arguments = OwaServiceMethodDispatcher.CreateMethodArgumentsFromUri(methodInfo, httpRequest);
			this.InternalInvokeMethod(methodInfo, service, httpRequest, httpResponse, arguments);
		}

		internal IAsyncResult InvokeBeginMethod(ServiceMethodInfo methodInfo, object service, HttpRequest httpRequest, AsyncCallback asyncCallback)
		{
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "OwaServiceMethodDispatcher.InvokeBeginMethod");
			object[] arguments = OwaServiceMethodDispatcher.CreateMethodArgumentsFromRequest(methodInfo, httpRequest);
			return this.InternalInvokeBeginMethod(methodInfo, service, httpRequest, asyncCallback, arguments);
		}

		internal IAsyncResult InvokeBeginGetMethod(ServiceMethodInfo methodInfo, object service, HttpRequest httpRequest, AsyncCallback asyncCallback)
		{
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "OwaServiceMethodDispatcher.InvokeBeginGetMethod");
			object[] arguments = OwaServiceMethodDispatcher.CreateMethodArgumentsFromUri(methodInfo, httpRequest);
			return this.InternalInvokeBeginMethod(methodInfo, service, httpRequest, asyncCallback, arguments);
		}

		internal void InvokeEndMethod(ServiceMethodInfo methodInfo, object service, IAsyncResult result, HttpResponse httpResponse)
		{
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "OwaServiceMethodDispatcher.InvokeEndMethod");
			object response = null;
			using (CpuTracker.StartCpuTracking("END"))
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					response = methodInfo.EndMethod.Invoke(service, new object[]
					{
						result
					});
				}, new Func<Exception, bool>(this.CanIgnoreExceptionForWatsonReport));
			}
			if (methodInfo.ShouldAutoDisposeResponse && response != null)
			{
				this.delayedDisposalResponseObject = response;
			}
			this.inspector.BeforeSendReply(httpResponse, methodInfo.Name, response);
			using (CpuTracker.StartCpuTracking("WRITE"))
			{
				OwaServiceMethodDispatcher.WriteResponse(methodInfo, httpResponse, response);
			}
		}

		internal void DisposeParameters()
		{
			if (this.delayedDisposalRequestObjects != null)
			{
				OwaServiceMethodDispatcher.DisposeObjects(this.delayedDisposalRequestObjects);
				this.delayedDisposalRequestObjects = null;
			}
			if (this.delayedDisposalResponseObject != null)
			{
				OwaServiceMethodDispatcher.DisposeObjects(new object[]
				{
					this.delayedDisposalResponseObject
				});
				this.delayedDisposalResponseObject = null;
			}
		}

		private const int MaxJsonLoggingSize = 2048;

		private IOwaServiceMessageInspector inspector;

		private object[] delayedDisposalRequestObjects;

		private object delayedDisposalResponseObject;
	}
}
