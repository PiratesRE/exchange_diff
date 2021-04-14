using System;
using System.Collections;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.DocumentLibrary;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaEventRegistry
	{
		private OwaEventRegistry()
		{
		}

		public static void RegisterHandler(Type handlerType)
		{
			ExTraceGlobals.OehCallTracer.TraceDebug(0L, "OwaEventRegistry.RegisterHandler");
			if (handlerType == null)
			{
				throw new ArgumentNullException("handlerType");
			}
			object[] customAttributes = handlerType.GetCustomAttributes(typeof(OwaEventNamespaceAttribute), false);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				throw new OwaNotSupportedException("Internal error: Handler is missing OwaEventNamespaceAttribute attribute");
			}
			OwaEventNamespaceAttribute owaEventNamespaceAttribute = (OwaEventNamespaceAttribute)customAttributes[0];
			owaEventNamespaceAttribute.HandlerType = handlerType;
			customAttributes = handlerType.GetCustomAttributes(typeof(OwaEventSegmentationAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				OwaEventSegmentationAttribute owaEventSegmentationAttribute = (OwaEventSegmentationAttribute)customAttributes[0];
				owaEventNamespaceAttribute.SegmentationFlags = owaEventSegmentationAttribute.SegmentationFlags;
			}
			else
			{
				owaEventNamespaceAttribute.SegmentationFlags = 0UL;
			}
			Type objectIdType = null;
			customAttributes = handlerType.GetCustomAttributes(typeof(OwaEventObjectIdAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				objectIdType = ((OwaEventObjectIdAttribute)customAttributes[0]).ObjectIdType;
			}
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
							throw new OwaNotSupportedException("Wrong signature for async event handler method.");
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
								throw new OwaNotSupportedException("Wrong signature for event handler method.");
							}
							owaEventAttribute.MethodInfo = methodInfo;
							owaEventAttribute.IsAsync = false;
						}
					}
					if (!owaEventAttribute.IsAsync || !(null != owaEventAttribute.EndMethodInfo))
					{
						OwaEventRegistry.ScanHandlerAttributes(methodInfo, owaEventAttribute, objectIdType);
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
						throw new OwaNotSupportedException("Namespace defines the same event both sync and async");
					}
				}
				else if (owaEventAttribute2.BeginMethodInfo != null)
				{
					if (owaEventAttribute2.EndMethodInfo == null)
					{
						throw new OwaNotSupportedException(string.Format("Begin async method {0} for event {1} is missing its corresponding End method", owaEventAttribute2.BeginMethodInfo.Name, owaEventAttribute2.Name));
					}
				}
				else if (owaEventAttribute2.EndMethodInfo != null && owaEventAttribute2.BeginMethodInfo == null)
				{
					throw new OwaNotSupportedException(string.Format("End async method {0} for event {1} is missing its corresponding Begin method", owaEventAttribute2.EndMethodInfo.Name, owaEventAttribute2.Name));
				}
			}
			OwaEventRegistry.handlerTable.Add(owaEventNamespaceAttribute.Name, owaEventNamespaceAttribute);
		}

		private static void ScanHandlerAttributes(MethodInfo method, OwaEventAttribute eventInfo, Type objectIdType)
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
			customAttributes = method.GetCustomAttributes(typeof(OwaEventSegmentationAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				OwaEventSegmentationAttribute owaEventSegmentationAttribute = (OwaEventSegmentationAttribute)customAttributes[0];
				eventInfo.SegmentationFlags = owaEventSegmentationAttribute.SegmentationFlags;
			}
			else
			{
				eventInfo.SegmentationFlags = 0UL;
			}
			ExTraceGlobals.OehDataTracer.TraceDebug<string, OwaEventVerb>(0L, "Event handler found. Name: '{0}'. Allowed verbs: '{1}'.", eventInfo.Name, eventInfo.AllowedVerbs);
			ulong num = 0UL;
			int num2 = 0;
			if (Globals.CanaryProtectionRequired)
			{
				eventInfo.AddParameterInfo(new OwaEventParameterAttribute("canary", typeof(string)));
			}
			customAttributes = method.GetCustomAttributes(typeof(OwaEventParameterAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				if (customAttributes.Length >= 64)
				{
					throw new OwaNotSupportedException("Event handler declares more parameters than allowed");
				}
				if ((eventInfo.AllowedVerbs & OwaEventVerb.Get) != OwaEventVerb.Unsupported && customAttributes.Length > 16)
				{
					throw new OwaNotSupportedException("Event handler declares more parameters than allowed for a GET request");
				}
				foreach (OwaEventParameterAttribute owaEventParameterAttribute in customAttributes)
				{
					if (objectIdType != null && owaEventParameterAttribute.Type == typeof(ObjectId))
					{
						owaEventParameterAttribute = new OwaEventParameterAttribute(owaEventParameterAttribute.Name, objectIdType, owaEventParameterAttribute.IsArray, owaEventParameterAttribute.IsOptional);
					}
					if (!eventInfo.IsInternal && !OwaEventRegistry.IsAllowedType(owaEventParameterAttribute.Type))
					{
						string message = string.Format("Event handler is using a type that is not supported method: '{0}' param type '{1}'", method.Name, owaEventParameterAttribute.Type);
						throw new OwaNotSupportedException(message);
					}
					if (string.Equals(owaEventParameterAttribute.Name, "ns", StringComparison.Ordinal) || string.Equals(owaEventParameterAttribute.Name, "ev", StringComparison.Ordinal))
					{
						throw new OwaNotSupportedException("Handler is trying to use a reserve name for a parameter");
					}
					if (OwaEventRegistry.structTypeTable[owaEventParameterAttribute.Type] != null)
					{
						owaEventParameterAttribute.IsStruct = true;
					}
					owaEventParameterAttribute.ParameterMask = 1UL << num2;
					if (!owaEventParameterAttribute.IsOptional)
					{
						num |= owaEventParameterAttribute.ParameterMask;
					}
					eventInfo.AddParameterInfo(owaEventParameterAttribute);
					num2++;
					ExTraceGlobals.OehDataTracer.TraceDebug(0L, "Event handler parameter found, name: '{0}', type: '{1}', isArray: '{2}', isOptional: '{3}', isStruct: '{4}'", new object[]
					{
						owaEventParameterAttribute.Name,
						owaEventParameterAttribute.Type,
						owaEventParameterAttribute.IsArray,
						owaEventParameterAttribute.IsOptional,
						owaEventParameterAttribute.IsStruct
					});
				}
			}
			eventInfo.RequiredMask = num;
		}

		public static void RegisterEnum(Type enumType)
		{
			ExTraceGlobals.OehCallTracer.TraceDebug(0L, "OwaEventRegistry.RegisterEnum");
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (!enumType.IsEnum || enumType.BaseType != typeof(Enum))
			{
				throw new OwaNotSupportedException("Type is not an enum");
			}
			if (OwaEventRegistry.enumTable.ContainsKey(enumType))
			{
				return;
			}
			Array values = Enum.GetValues(enumType);
			if (values == null || values.Length == 0)
			{
				throw new OwaNotSupportedException("Enum doesn't have any members");
			}
			OwaEventEnumAttribute owaEventEnumAttribute = new OwaEventEnumAttribute();
			for (int i = 0; i < values.Length; i++)
			{
				object value = values.GetValue(i);
				owaEventEnumAttribute.AddValueInfo((int)value, value);
			}
			OwaEventRegistry.enumTable[enumType] = owaEventEnumAttribute;
		}

		public static void RegisterStruct(Type structType)
		{
			ExTraceGlobals.OehCallTracer.TraceDebug(0L, "OwaEventRegistry.RegisterStruct");
			if (structType == null)
			{
				throw new ArgumentNullException("structType");
			}
			object[] customAttributes = structType.GetCustomAttributes(typeof(OwaEventStructAttribute), false);
			if (customAttributes == null || customAttributes.Length == 0)
			{
				throw new OwaNotSupportedException("Struct type is missing the OwaEventStructAttribute attribute");
			}
			OwaEventStructAttribute owaEventStructAttribute = (OwaEventStructAttribute)customAttributes[0];
			owaEventStructAttribute.StructType = structType;
			ExTraceGlobals.OehDataTracer.TraceDebug<Type>(0L, "Struct type: '{0}'", structType);
			Type type = null;
			customAttributes = structType.GetCustomAttributes(typeof(OwaEventObjectIdAttribute), false);
			if (customAttributes != null && customAttributes.Length > 0)
			{
				type = ((OwaEventObjectIdAttribute)customAttributes[0]).ObjectIdType;
			}
			FieldInfo[] fields = structType.GetFields(BindingFlags.Instance | BindingFlags.Public);
			int num = 0;
			uint num2 = 0U;
			foreach (FieldInfo fieldInfo in fields)
			{
				customAttributes = fieldInfo.GetCustomAttributes(typeof(OwaEventFieldAttribute), false);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					OwaEventFieldAttribute owaEventFieldAttribute = (OwaEventFieldAttribute)customAttributes[0];
					owaEventFieldAttribute.FieldInfo = fieldInfo;
					owaEventFieldAttribute.FieldType = fieldInfo.FieldType;
					if (type != null && owaEventFieldAttribute.FieldType == typeof(ObjectId))
					{
						owaEventFieldAttribute.FieldType = type;
					}
					if (!OwaEventRegistry.IsAllowedFieldType(owaEventFieldAttribute.FieldType))
					{
						throw new OwaNotSupportedException("Field type is not supported.");
					}
					owaEventFieldAttribute.FieldMask = 1U << num;
					owaEventStructAttribute.AllFieldsMask |= owaEventFieldAttribute.FieldMask;
					if (!owaEventFieldAttribute.IsOptional)
					{
						num2 |= owaEventFieldAttribute.FieldMask;
					}
					else if (fieldInfo.FieldType == typeof(ExDateTime))
					{
						owaEventFieldAttribute.DefaultValue = ExDateTime.MinValue;
					}
					if (num >= 32)
					{
						throw new OwaNotSupportedException("Struct declares more fields than allowed");
					}
					owaEventStructAttribute.AddFieldInfo(owaEventFieldAttribute, num);
					num++;
					ExTraceGlobals.OehDataTracer.TraceDebug<string, Type>(0L, "Struct field found, name: '{0}', type: '{1}'", owaEventStructAttribute.Name, owaEventFieldAttribute.FieldType);
				}
			}
			if (num == 0)
			{
				throw new OwaNotSupportedException("Struct must have at least one field");
			}
			owaEventStructAttribute.FieldCount = num;
			owaEventStructAttribute.RequiredMask = num2;
			OwaEventRegistry.structTable.Add(owaEventStructAttribute.Name, owaEventStructAttribute);
			OwaEventRegistry.structTypeTable.Add(owaEventStructAttribute.StructType, owaEventStructAttribute);
		}

		public static OwaEventNamespaceAttribute FindNamespaceInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return (OwaEventNamespaceAttribute)OwaEventRegistry.handlerTable[name];
		}

		public static OwaEventStructAttribute FindStructInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return (OwaEventStructAttribute)OwaEventRegistry.structTable[name];
		}

		public static OwaEventEnumAttribute FindEnumInfo(Type enumType)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			return (OwaEventEnumAttribute)OwaEventRegistry.enumTable[enumType];
		}

		private static bool IsAllowedType(Type type)
		{
			return type == typeof(string) || type == typeof(int) || type == typeof(bool) || type == typeof(double) || type == typeof(ExDateTime) || type == typeof(StoreObjectId) || type == typeof(ADObjectId) || type == typeof(DocumentLibraryObjectId) || type == typeof(OwaStoreObjectId) || OwaEventRegistry.structTypeTable[type] != null || null != OwaEventRegistry.enumTable[type];
		}

		private static bool IsAllowedFieldType(Type type)
		{
			return type == typeof(string) || type == typeof(int) || type == typeof(bool) || type == typeof(double) || type == typeof(ExDateTime) || type == typeof(StoreObjectId) || type == typeof(ADObjectId) || type == typeof(DocumentLibraryObjectId) || type == typeof(OwaStoreObjectId) || null != OwaEventRegistry.enumTable[type];
		}

		private static Hashtable handlerTable = new Hashtable(16);

		private static Hashtable structTable = new Hashtable(4);

		private static Hashtable structTypeTable = new Hashtable(4);

		private static Hashtable enumTable = new Hashtable(4);
	}
}
