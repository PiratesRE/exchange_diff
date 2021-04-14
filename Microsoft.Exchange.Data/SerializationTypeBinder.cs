using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Compliance.Serialization.Formatters;

namespace Microsoft.Exchange.Data
{
	internal static class SerializationTypeBinder
	{
		public static TypedSerializationFormatter.TypeBinder Instance
		{
			get
			{
				return SerializationTypeBinder.binder.Value;
			}
		}

		public static Exception InitializationException { get; private set; }

		public static Type[] GetLoadedTypes(Assembly assembly)
		{
			Type[] result;
			try
			{
				result = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				Type[] array;
				if (ex.Types != null)
				{
					array = (from x in ex.Types
					where x != null
					select x).ToArray<Type>();
				}
				else
				{
					array = null;
				}
				result = array;
			}
			return result;
		}

		private static TypedSerializationFormatter.TypeBinder CreateBinder()
		{
			TypedSerializationFormatter.TypeBinder result = null;
			try
			{
				int tickCount = Environment.TickCount;
				Type[] serializableTypes = SerializationTypeBinder.GetSerializableTypes();
				Type[] genericTypeDefinitions = SerializationTypeBinder.GetGenericTypeDefinitions(serializableTypes);
				result = new TypedSerializationFormatter.TypeBinder(serializableTypes, SerializationTypeBinder.safeBaseClasses, genericTypeDefinitions, new TypedSerializationFormatter.TypeEncounteredDelegate(TypedSerializationFormatter.WriteTypeEvent), true);
				SerializationTypeBinder.WriteEventLog(EventLogEntryType.Information, "SerializationTypeBinder create TypeBinder succeeded in {0} ms with {1} serializable types and {2} generic types.", new object[]
				{
					Environment.TickCount - tickCount,
					serializableTypes.Length,
					genericTypeDefinitions.Length
				});
			}
			catch (Exception ex)
			{
				SerializationTypeBinder.InitializationException = ex;
				SerializationTypeBinder.WriteEventLog(EventLogEntryType.Error, "SerializationTypeBinder create TypeBinder failed: {0}.", new object[]
				{
					ex.ToString()
				});
			}
			return result;
		}

		private static Type[] GetSerializableTypes()
		{
			HashSet<Type> hashSet = new HashSet<Type>();
			foreach (string text in SerializationTypeBinder.builtinTypes)
			{
				try
				{
					Type type = Type.GetType(text);
					if (type != null)
					{
						hashSet.Add(type);
					}
				}
				catch (Exception ex)
				{
					SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder initialize builtinType on type {0} failed: {1}.", new object[]
					{
						text,
						ex.ToString()
					});
				}
			}
			try
			{
				foreach (Type type2 in (from x in AppDomain.CurrentDomain.GetAssemblies()
				where x.FullName.StartsWith("Microsoft.Exchange.")
				select x).SelectMany((Assembly x) => from y in SerializationTypeBinder.GetLoadedTypes(x)
				where y.IsSerializable
				select y))
				{
					SerializationTypeBinder.ExpandSerializableTypes(hashSet, type2);
				}
			}
			catch (Exception ex2)
			{
				SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder initialize type in current appdomain failed: {0}.", new object[]
				{
					ex2.ToString()
				});
			}
			return hashSet.ToArray<Type>();
		}

		private static Type[] GetGenericTypeDefinitions(Type[] serializableTypes)
		{
			HashSet<Type> hashSet = new HashSet<Type>();
			foreach (string typeName in SerializationTypeBinder.builtinGenericTypes)
			{
				Type type = Type.GetType(typeName);
				if (type != null)
				{
					hashSet.Add(Type.GetType(typeName));
				}
			}
			foreach (Type type2 in serializableTypes)
			{
				try
				{
					if (type2.IsGenericTypeDefinition && !hashSet.Contains(type2))
					{
						hashSet.Add(type2);
					}
					else if (type2.IsConstructedGenericType)
					{
						Type genericTypeDefinition = type2.GetGenericTypeDefinition();
						if (genericTypeDefinition != null && !hashSet.Contains(genericTypeDefinition))
						{
							hashSet.Add(genericTypeDefinition);
						}
					}
				}
				catch (Exception ex)
				{
					SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder GetGenericTypeDefinitions on type {0} failed: {1}.", new object[]
					{
						type2.FullName,
						ex.ToString()
					});
				}
			}
			return hashSet.ToArray<Type>();
		}

		private static void ExpandSerializableTypes(HashSet<Type> serializableTypes, Type type)
		{
			if (serializableTypes.Contains(type))
			{
				return;
			}
			serializableTypes.Add(type);
			try
			{
				if (type.IsArray)
				{
					SerializationTypeBinder.ExpandSerializableTypes(serializableTypes, type.GetElementType());
				}
			}
			catch (Exception ex)
			{
				SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder load array element type on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex.ToString()
				});
			}
			try
			{
				if (type.IsConstructedGenericType)
				{
					foreach (Type type2 in type.GetGenericArguments())
					{
						SerializationTypeBinder.ExpandSerializableTypes(serializableTypes, type2);
					}
				}
			}
			catch (Exception ex2)
			{
				SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder load generic type definition on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex2.ToString()
				});
			}
			try
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (fields != null && fields.Length > 0)
				{
					foreach (FieldInfo fieldInfo in from x in fields
					where !x.IsNotSerialized && !serializableTypes.Contains(x.FieldType) && x.FieldType.IsSerializable
					select x)
					{
						try
						{
							SerializationTypeBinder.ExpandSerializableTypes(serializableTypes, fieldInfo.FieldType);
						}
						catch (Exception ex3)
						{
							SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder load field {0} on type {1} failed: {2}.", new object[]
							{
								fieldInfo.Name,
								type.FullName,
								ex3.ToString()
							});
						}
					}
				}
			}
			catch (Exception ex4)
			{
				SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder load fields on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex4.ToString()
				});
			}
			try
			{
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (properties != null && properties.Length > 0)
				{
					foreach (PropertyInfo propertyInfo in properties)
					{
						try
						{
							SerializationTypeBinder.ExpandSerializableTypes(serializableTypes, propertyInfo.PropertyType);
						}
						catch (Exception ex5)
						{
							SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder load property {0} on type {1} failed: {2}.", new object[]
							{
								propertyInfo.Name,
								type.FullName,
								ex5.ToString()
							});
						}
					}
				}
			}
			catch (Exception ex6)
			{
				SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder load properties on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex6.ToString()
				});
			}
			try
			{
				if (type.BaseType != null && type.BaseType != typeof(object))
				{
					SerializationTypeBinder.ExpandSerializableTypes(serializableTypes, type.BaseType);
				}
			}
			catch (Exception ex7)
			{
				SerializationTypeBinder.WriteEventLog(EventLogEntryType.Warning, "SerializationTypeBinder load base type on type {0} failed: {1}.", new object[]
				{
					type.FullName,
					ex7.ToString()
				});
			}
		}

		public static void WriteEventLog(EventLogEntryType eventType, string format, params object[] args)
		{
			using (EventLog eventLog = new EventLog("Application"))
			{
				eventLog.Source = "MSExchange Common";
				string arg = string.Empty;
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					arg = string.Format("{0} {1}", currentProcess.Id, currentProcess.ProcessName);
				}
				string message = string.Format("{0}: {1}", arg, string.Format(format, args));
				eventLog.WriteEntry(message, eventType, 8676);
			}
		}

		private const string EventLog = "Application";

		private const string EventSource = "MSExchange Common";

		private const int EventId = 8676;

		private static readonly string[] builtinTypes = new string[]
		{
			"System.UnitySerializationHolder",
			"System.CultureAwareComparer",
			"System.DelegateSerializationHolder",
			"System.OrdinalComparer",
			"System.DelegateSerializationHolder+DelegateEntry",
			"System.Globalization.GregorianCalendarTypes",
			"System.Reflection.MemberInfoSerializationHolder",
			"System.Text.CodePageEncoding",
			"System.Text.MLangCodePageEncoding",
			"System.Collections.ListDictionaryInternal"
		};

		private static readonly string[] builtinGenericTypes = new string[]
		{
			"System.Collections.Generic.ObjectEqualityComparer`1",
			"System.Collections.Generic.EnumEqualityComparer`1",
			"System.Collections.Generic.GenericEqualityComparer`1"
		};

		private static readonly Type[] safeBaseClasses = new Type[]
		{
			typeof(ConfigurableObject),
			typeof(PropertyBag),
			typeof(PropertyDefinition),
			typeof(ObjectId),
			typeof(MultiValuedPropertyBase),
			typeof(Calendar),
			typeof(Enum),
			typeof(Encoding),
			typeof(EncoderFallback),
			typeof(DecoderFallback)
		};

		private static Lazy<TypedSerializationFormatter.TypeBinder> binder = new Lazy<TypedSerializationFormatter.TypeBinder>(new Func<TypedSerializationFormatter.TypeBinder>(SerializationTypeBinder.CreateBinder), true);
	}
}
