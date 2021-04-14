using System;
using System.Collections;
using System.Reflection;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaEventRegistry
	{
		public void RegisterHandler(Type handlerType)
		{
			ExTraceGlobals.OehCallTracer.TraceDebug(0L, "OwaEventRegistry.RegisterHandler");
			if (handlerType == null)
			{
				throw new ArgumentNullException("handlerType");
			}
			object[] customAttributes = handlerType.GetCustomAttributes(typeof(OwaEventNamespaceAttribute), false);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				throw new OwaException("Internal error: Handler is missing OwaEventNamespaceAttribute attribute");
			}
			OwaEventNamespaceAttribute owaEventNamespaceAttribute = (OwaEventNamespaceAttribute)customAttributes[0];
			owaEventNamespaceAttribute.HandlerType = handlerType;
			ExTraceGlobals.OehDataTracer.TraceDebug<string>(0L, "Handler type: '{0}'", handlerType.ToString());
			foreach (MethodInfo methodInfo in handlerType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
			{
				customAttributes = methodInfo.GetCustomAttributes(typeof(OwaEventAttribute), false);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					OwaEventAttribute owaEventAttribute = (OwaEventAttribute)customAttributes[0];
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (methodInfo.ReturnType == typeof(IAsyncResult))
					{
						if (parameters.Length != 2 || parameters[0].ParameterType != typeof(AsyncCallback) || parameters[1].ParameterType != typeof(object))
						{
							throw new OwaException("Wrong signature for async event handler method.");
						}
						owaEventAttribute.IsAsync = true;
						owaEventAttribute.BeginMethodInfo = methodInfo;
					}
					else if (methodInfo.ReturnType == typeof(void))
					{
						if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IAsyncResult))
						{
							owaEventAttribute.EndMethodInfo = methodInfo;
							owaEventAttribute.IsAsync = true;
						}
						else
						{
							if (parameters.Length != 0)
							{
								throw new OwaException("Wrong signature for event handler method.");
							}
							owaEventAttribute.MethodInfo = methodInfo;
							owaEventAttribute.IsAsync = false;
						}
					}
					if (!owaEventAttribute.IsAsync || !(null != owaEventAttribute.EndMethodInfo))
					{
						this.ScanHandlerAttributes(methodInfo, owaEventAttribute, null);
					}
					owaEventNamespaceAttribute.AddEventInfo(owaEventAttribute);
				}
			}
			foreach (object obj in owaEventNamespaceAttribute.EventInfoTable.Values)
			{
				OwaEventAttribute owaEventAttribute2 = obj as OwaEventAttribute;
				if (owaEventAttribute2.MethodInfo != null)
				{
					if (owaEventAttribute2.BeginMethodInfo != null || owaEventAttribute2.EndMethodInfo != null)
					{
						throw new OwaException("Namespace defines the same event both sync and async");
					}
				}
				else if (owaEventAttribute2.BeginMethodInfo != null)
				{
					if (owaEventAttribute2.EndMethodInfo == null)
					{
						throw new OwaException(string.Format("Begin async method {0} for event {1} is missing its corresponding End method", owaEventAttribute2.BeginMethodInfo.Name, owaEventAttribute2.Name));
					}
				}
				else if (owaEventAttribute2.EndMethodInfo != null && owaEventAttribute2.BeginMethodInfo == null)
				{
					throw new OwaException(string.Format("End async method {0} for event {1} is missing its corresponding Begin method", owaEventAttribute2.EndMethodInfo.Name, owaEventAttribute2.Name));
				}
			}
			this.handlerTable.Add(owaEventNamespaceAttribute.Name, owaEventNamespaceAttribute);
		}

		public OwaEventNamespaceAttribute FindNamespaceInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return (OwaEventNamespaceAttribute)this.handlerTable[name];
		}

		private void ScanHandlerAttributes(MethodInfo method, OwaEventAttribute eventInfo, Type objectIdType)
		{
			object[] customAttributes = method.GetCustomAttributes(typeof(OwaEventVerbAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				OwaEventVerbAttribute owaEventVerbAttribute = (OwaEventVerbAttribute)customAttributes[0];
				eventInfo.AllowedVerbs = owaEventVerbAttribute.Verb;
			}
			else
			{
				eventInfo.AllowedVerbs = OwaEventVerb.Post;
			}
			ExTraceGlobals.OehDataTracer.TraceDebug<string, OwaEventVerb>(0L, "Event handler found. Name: '{0}'. Allowed verbs: '{1}'.", eventInfo.Name, eventInfo.AllowedVerbs);
			ulong num = 0UL;
			int num2 = 0;
			customAttributes = method.GetCustomAttributes(typeof(OwaEventParameterAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				if (customAttributes.Length >= 64)
				{
					throw new OwaException("Event handler declares more parameters than allowed");
				}
				if ((eventInfo.AllowedVerbs & OwaEventVerb.Get) != OwaEventVerb.Unsupported && customAttributes.Length > 16)
				{
					throw new OwaException("Event handler declares more parameters than allowed for a GET request");
				}
				foreach (OwaEventParameterAttribute owaEventParameterAttribute in customAttributes)
				{
					if (!eventInfo.IsInternal && !this.IsAllowedType(owaEventParameterAttribute.Type))
					{
						string message = string.Format("Event handler is using a type that is not supported method: '{0}' param type '{1}'", method.Name, owaEventParameterAttribute.Type);
						throw new OwaException(message);
					}
					if (string.Equals(owaEventParameterAttribute.Name, "ns", StringComparison.Ordinal) || string.Equals(owaEventParameterAttribute.Name, "ev", StringComparison.Ordinal))
					{
						throw new OwaException("Handler is trying to use a reserve name for a parameter");
					}
					owaEventParameterAttribute.ParameterMask = 1UL << num2;
					if (!owaEventParameterAttribute.IsOptional)
					{
						num |= owaEventParameterAttribute.ParameterMask;
					}
					eventInfo.AddParameterInfo(owaEventParameterAttribute);
					num2++;
					ExTraceGlobals.OehDataTracer.TraceDebug(0L, "Event handler parameter found, name: '{0}', type: '{1}', isArray: '{2}', isOptional: '{3}'", new object[]
					{
						owaEventParameterAttribute.Name,
						owaEventParameterAttribute.Type,
						owaEventParameterAttribute.IsArray,
						owaEventParameterAttribute.IsOptional
					});
				}
			}
			eventInfo.RequiredMask = num;
		}

		private bool IsAllowedType(Type type)
		{
			return type == typeof(string) || type == typeof(int) || type == typeof(bool) || type == typeof(double) || type == typeof(long);
		}

		private bool IsAllowedFieldType(Type type)
		{
			return type == typeof(string) || type == typeof(int) || type == typeof(bool) || type == typeof(double) || type == typeof(long);
		}

		private Hashtable handlerTable = new Hashtable(16);
	}
}
