using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace System
{
	internal sealed class TypeNameParser : IDisposable
	{
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void _CreateTypeNameParser(string typeName, ObjectHandleOnStack retHandle, bool throwOnError);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void _GetNames(SafeTypeNameParserHandle pTypeNameParser, ObjectHandleOnStack retArray);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void _GetTypeArguments(SafeTypeNameParserHandle pTypeNameParser, ObjectHandleOnStack retArray);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void _GetModifiers(SafeTypeNameParserHandle pTypeNameParser, ObjectHandleOnStack retArray);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void _GetAssemblyName(SafeTypeNameParserHandle pTypeNameParser, StringHandleOnStack retString);

		[SecuritySafeCritical]
		internal static Type GetType(string typeName, Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError, bool ignoreCase, ref StackCrawlMark stackMark)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (typeName.Length > 0 && typeName[0] == '\0')
			{
				throw new ArgumentException(Environment.GetResourceString("Format_StringZeroLength"));
			}
			Type result = null;
			SafeTypeNameParserHandle safeTypeNameParserHandle = TypeNameParser.CreateTypeNameParser(typeName, throwOnError);
			if (safeTypeNameParserHandle != null)
			{
				using (TypeNameParser typeNameParser = new TypeNameParser(safeTypeNameParserHandle))
				{
					result = typeNameParser.ConstructType(assemblyResolver, typeResolver, throwOnError, ignoreCase, ref stackMark);
				}
			}
			return result;
		}

		[SecuritySafeCritical]
		private TypeNameParser(SafeTypeNameParserHandle handle)
		{
			this.m_NativeParser = handle;
		}

		[SecuritySafeCritical]
		public void Dispose()
		{
			this.m_NativeParser.Dispose();
		}

		[SecuritySafeCritical]
		private unsafe Type ConstructType(Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError, bool ignoreCase, ref StackCrawlMark stackMark)
		{
			Assembly assembly = null;
			string assemblyName = this.GetAssemblyName();
			if (assemblyName.Length > 0)
			{
				assembly = TypeNameParser.ResolveAssembly(assemblyName, assemblyResolver, throwOnError, ref stackMark);
				if (assembly == null)
				{
					return null;
				}
			}
			string[] names = this.GetNames();
			if (names == null)
			{
				if (throwOnError)
				{
					throw new TypeLoadException(Environment.GetResourceString("Arg_TypeLoadNullStr"));
				}
				return null;
			}
			else
			{
				Type type = TypeNameParser.ResolveType(assembly, names, typeResolver, throwOnError, ignoreCase, ref stackMark);
				if (type == null)
				{
					return null;
				}
				SafeTypeNameParserHandle[] typeArguments = this.GetTypeArguments();
				Type[] array = null;
				if (typeArguments != null)
				{
					array = new Type[typeArguments.Length];
					for (int i = 0; i < typeArguments.Length; i++)
					{
						using (TypeNameParser typeNameParser = new TypeNameParser(typeArguments[i]))
						{
							array[i] = typeNameParser.ConstructType(assemblyResolver, typeResolver, throwOnError, ignoreCase, ref stackMark);
						}
						if (array[i] == null)
						{
							return null;
						}
					}
				}
				int[] modifiers = this.GetModifiers();
				fixed (int* ptr = modifiers)
				{
					IntPtr pModifiers = new IntPtr((void*)ptr);
					return RuntimeTypeHandle.GetTypeHelper(type, array, pModifiers, (modifiers == null) ? 0 : modifiers.Length);
				}
			}
		}

		[SecuritySafeCritical]
		private static Assembly ResolveAssembly(string asmName, Func<AssemblyName, Assembly> assemblyResolver, bool throwOnError, ref StackCrawlMark stackMark)
		{
			Assembly assembly = null;
			if (assemblyResolver == null)
			{
				if (throwOnError)
				{
					return RuntimeAssembly.InternalLoad(asmName, null, ref stackMark, false);
				}
				try
				{
					return RuntimeAssembly.InternalLoad(asmName, null, ref stackMark, false);
				}
				catch (FileNotFoundException)
				{
					return null;
				}
			}
			assembly = assemblyResolver(new AssemblyName(asmName));
			if (assembly == null && throwOnError)
			{
				throw new FileNotFoundException(Environment.GetResourceString("FileNotFound_ResolveAssembly", new object[]
				{
					asmName
				}));
			}
			return assembly;
		}

		private static Type ResolveType(Assembly assembly, string[] names, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError, bool ignoreCase, ref StackCrawlMark stackMark)
		{
			string text = TypeNameParser.EscapeTypeName(names[0]);
			Type type;
			if (typeResolver != null)
			{
				type = typeResolver(assembly, text, ignoreCase);
				if (type == null && throwOnError)
				{
					string message = (assembly == null) ? Environment.GetResourceString("TypeLoad_ResolveType", new object[]
					{
						text
					}) : Environment.GetResourceString("TypeLoad_ResolveTypeFromAssembly", new object[]
					{
						text,
						assembly.FullName
					});
					throw new TypeLoadException(message);
				}
			}
			else if (assembly == null)
			{
				type = RuntimeType.GetType(text, throwOnError, ignoreCase, false, ref stackMark);
			}
			else
			{
				type = assembly.GetType(text, throwOnError, ignoreCase);
			}
			if (type != null)
			{
				BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
				if (ignoreCase)
				{
					bindingFlags |= BindingFlags.IgnoreCase;
				}
				int i = 1;
				while (i < names.Length)
				{
					type = type.GetNestedType(names[i], bindingFlags);
					if (type == null)
					{
						if (throwOnError)
						{
							throw new TypeLoadException(Environment.GetResourceString("TypeLoad_ResolveNestedType", new object[]
							{
								names[i],
								names[i - 1]
							}));
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			return type;
		}

		private static string EscapeTypeName(string name)
		{
			if (name.IndexOfAny(TypeNameParser.SPECIAL_CHARS) < 0)
			{
				return name;
			}
			StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
			foreach (char value in name)
			{
				if (Array.IndexOf<char>(TypeNameParser.SPECIAL_CHARS, value) >= 0)
				{
					stringBuilder.Append('\\');
				}
				stringBuilder.Append(value);
			}
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}

		[SecuritySafeCritical]
		private static SafeTypeNameParserHandle CreateTypeNameParser(string typeName, bool throwOnError)
		{
			SafeTypeNameParserHandle result = null;
			TypeNameParser._CreateTypeNameParser(typeName, JitHelpers.GetObjectHandleOnStack<SafeTypeNameParserHandle>(ref result), throwOnError);
			return result;
		}

		[SecuritySafeCritical]
		private string[] GetNames()
		{
			string[] result = null;
			TypeNameParser._GetNames(this.m_NativeParser, JitHelpers.GetObjectHandleOnStack<string[]>(ref result));
			return result;
		}

		[SecuritySafeCritical]
		private SafeTypeNameParserHandle[] GetTypeArguments()
		{
			SafeTypeNameParserHandle[] result = null;
			TypeNameParser._GetTypeArguments(this.m_NativeParser, JitHelpers.GetObjectHandleOnStack<SafeTypeNameParserHandle[]>(ref result));
			return result;
		}

		[SecuritySafeCritical]
		private int[] GetModifiers()
		{
			int[] result = null;
			TypeNameParser._GetModifiers(this.m_NativeParser, JitHelpers.GetObjectHandleOnStack<int[]>(ref result));
			return result;
		}

		[SecuritySafeCritical]
		private string GetAssemblyName()
		{
			string result = null;
			TypeNameParser._GetAssemblyName(this.m_NativeParser, JitHelpers.GetStringHandleOnStack(ref result));
			return result;
		}

		[SecurityCritical]
		private SafeTypeNameParserHandle m_NativeParser;

		private static readonly char[] SPECIAL_CHARS = new char[]
		{
			',',
			'[',
			']',
			'&',
			'*',
			'+',
			'\\'
		};
	}
}
