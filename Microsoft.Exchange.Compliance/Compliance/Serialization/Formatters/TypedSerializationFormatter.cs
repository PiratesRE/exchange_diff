using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Compliance.Serialization.Formatters
{
	internal class TypedSerializationFormatter
	{
		public static void WriteTypeEvent(Type type, bool allowed)
		{
			if (allowed)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder("The following types were discovered in your serialized stream.\r\n\r\n");
			if (type == null)
			{
				stringBuilder.AppendLine("There were no Types discovered for this serialization object");
			}
			else
			{
				stringBuilder.AppendLine(type.FullName);
			}
			stringBuilder.AppendLine("\r\n\r\nStackTrace for the current call");
			stringBuilder.AppendLine(new StackTrace(true).ToString());
			using (EventLog eventLog = new EventLog("Application"))
			{
				eventLog.Source = "MSExchange Common";
				eventLog.WriteEntry(stringBuilder.ToString(), EventLogEntryType.Information, 8675);
			}
		}

		protected const string Prologue = "The following types were discovered in your serialized stream.\r\n\r\n";

		protected const string NoTypes = "There were no Types discovered for this serialization object";

		protected const string StackIndicator = "\r\n\r\nStackTrace for the current call";

		protected const string EventLog = "Application";

		protected const string EventSource = "MSExchange Common";

		private const int EventId = 8675;

		public delegate void TypeEncounteredDelegate(Type type, bool allowed);

		internal sealed class TypeBinder : SerializationBinder
		{
			public TypeBinder(TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered, bool strict)
			{
				this.typeEncounteredCallback = typeEncountered;
				this.strict = strict;
				this.expected = TypedSerializationFormatter.TypeBinder.GenerateExpected(null);
			}

			public TypeBinder(Dictionary<Type, string> expectedTypes, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered)
			{
				this.expected = expectedTypes;
				this.typeEncounteredCallback = typeEncountered;
			}

			public TypeBinder(Type[] expectedTypes, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered, bool strict)
			{
				this.expected = TypedSerializationFormatter.TypeBinder.GenerateExpected(expectedTypes);
				this.typeEncounteredCallback = typeEncountered;
				this.strict = strict;
			}

			public TypeBinder(Type[] expectedTypes, Type[] baseClasses, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered, bool strict) : this(expectedTypes, baseClasses, null, typeEncountered, strict)
			{
			}

			public TypeBinder(Type[] expectedTypes, Type[] baseClasses, Type[] genericTypes, TypedSerializationFormatter.TypeEncounteredDelegate typeEncountered, bool strict)
			{
				if (baseClasses != null && baseClasses.Length > 0)
				{
					this.baseClassSet = baseClasses;
				}
				if (genericTypes != null && genericTypes.Length > 0)
				{
					this.genericTypeSet = new HashSet<Type>();
					foreach (Type type in genericTypes)
					{
						if (type.IsGenericTypeDefinition && !this.genericTypeSet.Contains(type))
						{
							this.genericTypeSet.Add(type);
						}
					}
				}
				this.expected = TypedSerializationFormatter.TypeBinder.GenerateExpected(expectedTypes);
				this.typeEncounteredCallback = typeEncountered;
				this.strict = strict;
			}

			public bool IsInitialized
			{
				get
				{
					return this.expected != null && this.expected.Count != 0;
				}
			}

			public static Dictionary<Type, string> GenerateExpected(Type[] data)
			{
				Dictionary<Type, string> dictionary = new Dictionary<Type, string>(TypedSerializationFormatter.TypeBinder.Whitelist.Count);
				foreach (Type type in TypedSerializationFormatter.TypeBinder.Whitelist)
				{
					dictionary.Add(type, type.Name);
				}
				if (data != null && data.Length > 0)
				{
					foreach (Type type2 in data)
					{
						if (!dictionary.ContainsKey(type2))
						{
							dictionary.Add(type2, type2.Name);
						}
					}
				}
				return dictionary;
			}

			public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
			{
				base.BindToName(serializedType, out assemblyName, out typeName);
			}

			public override Type BindToType(string assemblyName, string typeName)
			{
				if (string.IsNullOrEmpty(typeName))
				{
					throw new BlockedTypeException("Null");
				}
				Type type = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
				if (type == null)
				{
					string text = assemblyName.Split(new char[]
					{
						','
					})[0];
					type = Type.GetType(string.Format("{0}, {1}", typeName, text));
					if (type == null)
					{
						Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
						foreach (Assembly assembly in assemblies)
						{
							if (text == assembly.FullName.Split(new char[]
							{
								','
							})[0])
							{
								type = assembly.GetType(typeName);
								break;
							}
						}
					}
				}
				if (type == null)
				{
					throw new BlockedTypeException(typeName + "," + assemblyName);
				}
				this.CheckValidity(type);
				return type;
			}

			private void CheckValidity(Type unknown)
			{
				if (unknown != null)
				{
					if (this.strict)
					{
						if (this.expected.ContainsKey(unknown))
						{
							return;
						}
					}
					else if (!this.strict && this.expected.ContainsValue(unknown.Name))
					{
						return;
					}
					if (unknown.IsConstructedGenericType && this.genericTypeSet != null && this.genericTypeSet.Contains(unknown.GetGenericTypeDefinition()))
					{
						return;
					}
					if (this.baseClassSet != null)
					{
						foreach (Type type in this.baseClassSet)
						{
							if (!(type.Name == "Object") && unknown.IsSubclassOf(type))
							{
								return;
							}
						}
					}
				}
				if (this.typeEncounteredCallback != null)
				{
					this.typeEncounteredCallback(unknown, false);
				}
				throw new BlockedTypeException((unknown != null) ? unknown.ToString() : "Null");
			}

			private const string Null = "Null";

			private const string ObjectName = "Object";

			private const string TypeFormat = "{0}, {1}";

			private static readonly List<Type> Whitelist = new List<Type>(new Type[]
			{
				typeof(string),
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(double),
				typeof(float),
				typeof(bool),
				typeof(short),
				typeof(ushort),
				typeof(byte),
				typeof(char)
			});

			private readonly bool strict = true;

			private Dictionary<Type, string> expected;

			private Type[] baseClassSet;

			private HashSet<Type> genericTypeSet;

			private TypedSerializationFormatter.TypeEncounteredDelegate typeEncounteredCallback;
		}
	}
}
