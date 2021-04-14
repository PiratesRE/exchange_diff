using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Diagnostics.Components.Tasks;

namespace Microsoft.Exchange.Configuration.PswsProxy
{
	internal class PSObjectSerializer
	{
		static PSObjectSerializer()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (assembly.GetName().Name == "System.Management.Automation")
				{
					PSObjectSerializer.mgrAutomationAssembly = assembly;
				}
			}
			if (PSObjectSerializer.mgrAutomationAssembly == null)
			{
				throw new PswsProxySerializationException(Strings.PswsManagementAutomationAssemblyLoadError);
			}
			PSObjectSerializer.serializerType = PSObjectSerializer.mgrAutomationAssembly.GetType("System.Management.Automation.Serializer", true, false);
			PSObjectSerializer.serializerSerializeMethod = PSObjectSerializer.serializerType.GetMethod("Serialize", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
			{
				typeof(object)
			}, null);
			PSObjectSerializer.serializerTypeTableProperty = PSObjectSerializer.serializerType.GetProperty("TypeTable", BindingFlags.Instance | BindingFlags.NonPublic);
			PSObjectSerializer.serializerDoneMethod = PSObjectSerializer.serializerType.GetMethod("Done", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
			PSObjectSerializer.deserializerType = PSObjectSerializer.mgrAutomationAssembly.GetType("System.Management.Automation.Deserializer", true, false);
			PSObjectSerializer.deserializerDeserializeMethod = PSObjectSerializer.deserializerType.GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
			PSObjectSerializer.deserializerTypeTableProperty = PSObjectSerializer.deserializerType.GetProperty("TypeTable", BindingFlags.Instance | BindingFlags.NonPublic);
			PSObjectSerializer.deserializerDoneMethod = PSObjectSerializer.deserializerType.GetMethod("Done", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null);
			object obj = Enum.Parse(PSObjectSerializer.mgrAutomationAssembly.GetType("System.Management.Automation.SerializationOptions", true, false), "RemotingOptions");
			Assembly assembly2 = PSObjectSerializer.mgrAutomationAssembly;
			string typeName = "System.Management.Automation.SerializationContext";
			bool ignoreCase = true;
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance;
			Binder binder = null;
			object[] array2 = new object[3];
			array2[0] = 1;
			array2[1] = obj;
			PSObjectSerializer.serializationContext = assembly2.CreateInstance(typeName, ignoreCase, bindingAttr, binder, array2, null, null);
			object obj2 = Enum.Parse(PSObjectSerializer.mgrAutomationAssembly.GetType("System.Management.Automation.DeserializationOptions", true, false), "RemotingOptions");
			Assembly assembly3 = PSObjectSerializer.mgrAutomationAssembly;
			string typeName2 = "System.Management.Automation.DeserializationContext";
			bool ignoreCase2 = true;
			BindingFlags bindingAttr2 = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance;
			Binder binder2 = null;
			object[] array3 = new object[2];
			array3[0] = obj2;
			PSObjectSerializer.deserializationContext = assembly3.CreateInstance(typeName2, ignoreCase2, bindingAttr2, binder2, array3, null, null);
		}

		public PSObjectSerializer() : this(null)
		{
		}

		public PSObjectSerializer(TypeTable typeTable)
		{
			this.TypeTable = typeTable;
		}

		internal TypeTable TypeTable { get; private set; }

		internal string Serialize(PSObject psObject)
		{
			if (psObject == null)
			{
				throw new ArgumentNullException("psObject");
			}
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriterSettings settings = new XmlWriterSettings
			{
				CheckCharacters = false,
				Indent = false,
				CloseOutput = false,
				Encoding = Encoding.UTF8,
				NewLineHandling = NewLineHandling.None,
				OmitXmlDeclaration = true,
				ConformanceLevel = ConformanceLevel.Fragment
			};
			try
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
				{
					object obj = PSObjectSerializer.mgrAutomationAssembly.CreateInstance("System.Management.Automation.Serializer", true, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, new object[]
					{
						xmlWriter,
						PSObjectSerializer.serializationContext
					}, null, null);
					PSObjectSerializer.serializerTypeTableProperty.SetValue(obj, this.TypeTable, null);
					PSObjectSerializer.serializerSerializeMethod.Invoke(obj, new object[]
					{
						psObject
					});
					PSObjectSerializer.serializerDoneMethod.Invoke(obj, null);
					xmlWriter.Flush();
				}
			}
			catch (Exception ex)
			{
				throw new PswsProxySerializationException(Strings.PswsSerializationError(ex.Message), ex);
			}
			ExTraceGlobals.LogTracer.Information<string, string>(0L, "ConvertToExchangeXml: Serialize object {0} successfully with following data:\r\n{1}", psObject.ToString(), stringBuilder.ToString());
			return stringBuilder.ToString();
		}

		internal PSObject Deserialize(string serializedData)
		{
			if (string.IsNullOrEmpty(serializedData))
			{
				throw new ArgumentNullException("serializedData");
			}
			XmlReaderSettings settings = new XmlReaderSettings
			{
				CheckCharacters = false,
				CloseInput = false,
				ConformanceLevel = ConformanceLevel.Document,
				IgnoreComments = true,
				IgnoreProcessingInstructions = true,
				IgnoreWhitespace = false,
				MaxCharactersFromEntities = 1024L,
				Schemas = null,
				ValidationFlags = XmlSchemaValidationFlags.None,
				ValidationType = ValidationType.None,
				XmlResolver = null
			};
			PSObject result;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					StreamWriter streamWriter = new StreamWriter(memoryStream);
					streamWriter.Write(serializedData);
					streamWriter.Flush();
					memoryStream.Seek(0L, SeekOrigin.Begin);
					using (XmlReader xmlReader = XmlReader.Create(memoryStream, settings))
					{
						object obj = PSObjectSerializer.mgrAutomationAssembly.CreateInstance("System.Management.Automation.Deserializer", true, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, new object[]
						{
							xmlReader,
							PSObjectSerializer.deserializationContext
						}, null, null);
						PSObjectSerializer.deserializerTypeTableProperty.SetValue(obj, this.TypeTable, null);
						object obj2 = PSObjectSerializer.deserializerDeserializeMethod.Invoke(obj, null);
						PSObjectSerializer.deserializerDoneMethod.Invoke(obj, null);
						ExTraceGlobals.LogTracer.Information<string, string>(0L, "ObjectTransfer: Deserialize object {0} successfully by using following data:\r\n{1}", obj2.ToString(), serializedData);
						result = PSObject.AsPSObject(obj2);
					}
				}
			}
			catch (Exception ex)
			{
				throw new PswsProxySerializationException(Strings.PswsDeserializationError(ex.Message), ex);
			}
			return result;
		}

		private const string mgrAutomationAssemblyName = "System.Management.Automation";

		private const string serializerTypeName = "System.Management.Automation.Serializer";

		private const string serializerSerializeMethodName = "Serialize";

		private const string serializerTypeTablePropertyName = "TypeTable";

		private const string serializerDoneMethodName = "Done";

		private const string deserializerTypeName = "System.Management.Automation.Deserializer";

		private const string deserializerDeserializeMethodName = "Deserialize";

		private const string deserializerTypeTablePropertyName = "TypeTable";

		private const string deserializerDoneMethodName = "Done";

		private const string serializationContextTypeName = "System.Management.Automation.SerializationContext";

		private const int serializationDepth = 1;

		private const string serializationOptionsTypeName = "System.Management.Automation.SerializationOptions";

		private const string serializationRemotingOptions = "RemotingOptions";

		private const string deserializationContextTypeName = "System.Management.Automation.DeserializationContext";

		private const string deserializationOptionsTypeName = "System.Management.Automation.DeserializationOptions";

		private const string deserializationRemotingOptions = "RemotingOptions";

		private static Assembly mgrAutomationAssembly;

		private static Type serializerType;

		private static MethodInfo serializerSerializeMethod;

		private static PropertyInfo serializerTypeTableProperty;

		private static MethodInfo serializerDoneMethod;

		private static Type deserializerType;

		private static MethodInfo deserializerDeserializeMethod;

		private static PropertyInfo deserializerTypeTableProperty;

		private static MethodInfo deserializerDoneMethod;

		private static object serializationContext;

		private static object deserializationContext;
	}
}
