using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.TCEAdapterGen;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using Microsoft.Win32;

namespace System.Runtime.InteropServices
{
	[Guid("F1C3BF79-C3E4-11d3-88E7-00902754C43A")]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public sealed class TypeLibConverter : ITypeLibConverter
	{
		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public AssemblyBuilder ConvertTypeLibToAssembly([MarshalAs(UnmanagedType.Interface)] object typeLib, string asmFileName, int flags, ITypeLibImporterNotifySink notifySink, byte[] publicKey, StrongNameKeyPair keyPair, bool unsafeInterfaces)
		{
			return this.ConvertTypeLibToAssembly(typeLib, asmFileName, unsafeInterfaces ? TypeLibImporterFlags.UnsafeInterfaces : TypeLibImporterFlags.None, notifySink, publicKey, keyPair, null, null);
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public AssemblyBuilder ConvertTypeLibToAssembly([MarshalAs(UnmanagedType.Interface)] object typeLib, string asmFileName, TypeLibImporterFlags flags, ITypeLibImporterNotifySink notifySink, byte[] publicKey, StrongNameKeyPair keyPair, string asmNamespace, Version asmVersion)
		{
			if (typeLib == null)
			{
				throw new ArgumentNullException("typeLib");
			}
			if (asmFileName == null)
			{
				throw new ArgumentNullException("asmFileName");
			}
			if (notifySink == null)
			{
				throw new ArgumentNullException("notifySink");
			}
			if (string.Empty.Equals(asmFileName))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_InvalidFileName"), "asmFileName");
			}
			if (asmFileName.Length > 260)
			{
				throw new ArgumentException(Environment.GetResourceString("IO.PathTooLong"), asmFileName);
			}
			if ((flags & TypeLibImporterFlags.PrimaryInteropAssembly) != TypeLibImporterFlags.None && publicKey == null && keyPair == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_PIAMustBeStrongNamed"));
			}
			ArrayList arrayList = null;
			AssemblyNameFlags asmNameFlags = AssemblyNameFlags.None;
			AssemblyName assemblyNameFromTypelib = TypeLibConverter.GetAssemblyNameFromTypelib(typeLib, asmFileName, publicKey, keyPair, asmVersion, asmNameFlags);
			AssemblyBuilder assemblyBuilder = TypeLibConverter.CreateAssemblyForTypeLib(typeLib, asmFileName, assemblyNameFromTypelib, (flags & TypeLibImporterFlags.PrimaryInteropAssembly) > TypeLibImporterFlags.None, (flags & TypeLibImporterFlags.ReflectionOnlyLoading) > TypeLibImporterFlags.None, (flags & TypeLibImporterFlags.NoDefineVersionResource) > TypeLibImporterFlags.None);
			string fileName = Path.GetFileName(asmFileName);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(fileName, fileName);
			if (asmNamespace == null)
			{
				asmNamespace = assemblyNameFromTypelib.Name;
			}
			TypeLibConverter.TypeResolveHandler typeResolveHandler = new TypeLibConverter.TypeResolveHandler(moduleBuilder, notifySink);
			AppDomain domain = Thread.GetDomain();
			ResolveEventHandler value = new ResolveEventHandler(typeResolveHandler.ResolveEvent);
			ResolveEventHandler value2 = new ResolveEventHandler(typeResolveHandler.ResolveAsmEvent);
			ResolveEventHandler value3 = new ResolveEventHandler(typeResolveHandler.ResolveROAsmEvent);
			domain.TypeResolve += value;
			domain.AssemblyResolve += value2;
			domain.ReflectionOnlyAssemblyResolve += value3;
			TypeLibConverter.nConvertTypeLibToMetadata(typeLib, assemblyBuilder.InternalAssembly, moduleBuilder.InternalModule, asmNamespace, flags, typeResolveHandler, out arrayList);
			TypeLibConverter.UpdateComTypesInAssembly(assemblyBuilder, moduleBuilder);
			if (arrayList.Count > 0)
			{
				new TCEAdapterGenerator().Process(moduleBuilder, arrayList);
			}
			domain.TypeResolve -= value;
			domain.AssemblyResolve -= value2;
			domain.ReflectionOnlyAssemblyResolve -= value3;
			return assemblyBuilder;
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[return: MarshalAs(UnmanagedType.Interface)]
		public object ConvertAssemblyToTypeLib(Assembly assembly, string strTypeLibName, TypeLibExporterFlags flags, ITypeLibExporterNotifySink notifySink)
		{
			AssemblyBuilder assemblyBuilder = assembly as AssemblyBuilder;
			RuntimeAssembly assembly2;
			if (assemblyBuilder != null)
			{
				assembly2 = assemblyBuilder.InternalAssembly;
			}
			else
			{
				assembly2 = (assembly as RuntimeAssembly);
			}
			return TypeLibConverter.nConvertAssemblyToTypeLib(assembly2, strTypeLibName, flags, notifySink);
		}

		public bool GetPrimaryInteropAssembly(Guid g, int major, int minor, int lcid, out string asmName, out string asmCodeBase)
		{
			string name = "{" + g.ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			string name2 = major.ToString("x", CultureInfo.InvariantCulture) + "." + minor.ToString("x", CultureInfo.InvariantCulture);
			asmName = null;
			asmCodeBase = null;
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("TypeLib", false))
			{
				if (registryKey != null)
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(name))
					{
						if (registryKey2 != null)
						{
							using (RegistryKey registryKey3 = registryKey2.OpenSubKey(name2, false))
							{
								if (registryKey3 != null)
								{
									asmName = (string)registryKey3.GetValue("PrimaryInteropAssemblyName");
									asmCodeBase = (string)registryKey3.GetValue("PrimaryInteropAssemblyCodeBase");
								}
							}
						}
					}
				}
			}
			return asmName != null;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static AssemblyBuilder CreateAssemblyForTypeLib(object typeLib, string asmFileName, AssemblyName asmName, bool bPrimaryInteropAssembly, bool bReflectionOnly, bool bNoDefineVersionResource)
		{
			AppDomain domain = Thread.GetDomain();
			string text = null;
			if (asmFileName != null)
			{
				text = Path.GetDirectoryName(asmFileName);
				if (string.IsNullOrEmpty(text))
				{
					text = null;
				}
			}
			AssemblyBuilderAccess access;
			if (bReflectionOnly)
			{
				access = AssemblyBuilderAccess.ReflectionOnly;
			}
			else
			{
				access = AssemblyBuilderAccess.RunAndSave;
			}
			List<CustomAttributeBuilder> list = new List<CustomAttributeBuilder>();
			ConstructorInfo constructor = typeof(SecurityRulesAttribute).GetConstructor(new Type[]
			{
				typeof(SecurityRuleSet)
			});
			CustomAttributeBuilder item = new CustomAttributeBuilder(constructor, new object[]
			{
				SecurityRuleSet.Level2
			});
			list.Add(item);
			AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(asmName, access, text, false, list);
			TypeLibConverter.SetGuidAttributeOnAssembly(assemblyBuilder, typeLib);
			TypeLibConverter.SetImportedFromTypeLibAttrOnAssembly(assemblyBuilder, typeLib);
			if (bNoDefineVersionResource)
			{
				TypeLibConverter.SetTypeLibVersionAttribute(assemblyBuilder, typeLib);
			}
			else
			{
				TypeLibConverter.SetVersionInformation(assemblyBuilder, typeLib, asmName);
			}
			if (bPrimaryInteropAssembly)
			{
				TypeLibConverter.SetPIAAttributeOnAssembly(assemblyBuilder, typeLib);
			}
			return assemblyBuilder;
		}

		[SecurityCritical]
		internal static AssemblyName GetAssemblyNameFromTypelib(object typeLib, string asmFileName, byte[] publicKey, StrongNameKeyPair keyPair, Version asmVersion, AssemblyNameFlags asmNameFlags)
		{
			string text = null;
			string text2 = null;
			int num = 0;
			string text3 = null;
			ITypeLib typeLib2 = (ITypeLib)typeLib;
			typeLib2.GetDocumentation(-1, out text, out text2, out num, out text3);
			if (asmFileName == null)
			{
				asmFileName = text;
			}
			else
			{
				string fileName = Path.GetFileName(asmFileName);
				string extension = Path.GetExtension(asmFileName);
				if (!".dll".Equals(extension, StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_InvalidFileExtension"));
				}
				asmFileName = fileName.Substring(0, fileName.Length - ".dll".Length);
			}
			if (asmVersion == null)
			{
				int major;
				int minor;
				Marshal.GetTypeLibVersion(typeLib2, out major, out minor);
				asmVersion = new Version(major, minor, 0, 0);
			}
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Init(asmFileName, publicKey, null, asmVersion, null, AssemblyHashAlgorithm.None, AssemblyVersionCompatibility.SameMachine, null, asmNameFlags, keyPair);
			return assemblyName;
		}

		private static void UpdateComTypesInAssembly(AssemblyBuilder asmBldr, ModuleBuilder modBldr)
		{
			AssemblyBuilderData assemblyData = asmBldr.m_assemblyData;
			Type[] types = modBldr.GetTypes();
			int num = types.Length;
			for (int i = 0; i < num; i++)
			{
				assemblyData.AddPublicComType(types[i]);
			}
		}

		[SecurityCritical]
		private static void SetGuidAttributeOnAssembly(AssemblyBuilder asmBldr, object typeLib)
		{
			Type[] types = new Type[]
			{
				typeof(string)
			};
			ConstructorInfo constructor = typeof(GuidAttribute).GetConstructor(types);
			object[] constructorArgs = new object[]
			{
				Marshal.GetTypeLibGuid((ITypeLib)typeLib).ToString()
			};
			CustomAttributeBuilder customAttribute = new CustomAttributeBuilder(constructor, constructorArgs);
			asmBldr.SetCustomAttribute(customAttribute);
		}

		[SecurityCritical]
		private static void SetImportedFromTypeLibAttrOnAssembly(AssemblyBuilder asmBldr, object typeLib)
		{
			Type[] types = new Type[]
			{
				typeof(string)
			};
			ConstructorInfo constructor = typeof(ImportedFromTypeLibAttribute).GetConstructor(types);
			string typeLibName = Marshal.GetTypeLibName((ITypeLib)typeLib);
			object[] constructorArgs = new object[]
			{
				typeLibName
			};
			CustomAttributeBuilder customAttribute = new CustomAttributeBuilder(constructor, constructorArgs);
			asmBldr.SetCustomAttribute(customAttribute);
		}

		[SecurityCritical]
		private static void SetTypeLibVersionAttribute(AssemblyBuilder asmBldr, object typeLib)
		{
			Type[] types = new Type[]
			{
				typeof(int),
				typeof(int)
			};
			ConstructorInfo constructor = typeof(TypeLibVersionAttribute).GetConstructor(types);
			int num;
			int num2;
			Marshal.GetTypeLibVersion((ITypeLib)typeLib, out num, out num2);
			object[] constructorArgs = new object[]
			{
				num,
				num2
			};
			CustomAttributeBuilder customAttribute = new CustomAttributeBuilder(constructor, constructorArgs);
			asmBldr.SetCustomAttribute(customAttribute);
		}

		[SecurityCritical]
		private static void SetVersionInformation(AssemblyBuilder asmBldr, object typeLib, AssemblyName asmName)
		{
			string arg = null;
			string text = null;
			int num = 0;
			string text2 = null;
			ITypeLib typeLib2 = (ITypeLib)typeLib;
			typeLib2.GetDocumentation(-1, out arg, out text, out num, out text2);
			string product = string.Format(CultureInfo.InvariantCulture, Environment.GetResourceString("TypeLibConverter_ImportedTypeLibProductName"), arg);
			asmBldr.DefineVersionInfoResource(product, asmName.Version.ToString(), null, null, null);
			TypeLibConverter.SetTypeLibVersionAttribute(asmBldr, typeLib);
		}

		[SecurityCritical]
		private static void SetPIAAttributeOnAssembly(AssemblyBuilder asmBldr, object typeLib)
		{
			IntPtr zero = IntPtr.Zero;
			ITypeLib typeLib2 = (ITypeLib)typeLib;
			int num = 0;
			int num2 = 0;
			Type[] types = new Type[]
			{
				typeof(int),
				typeof(int)
			};
			ConstructorInfo constructor = typeof(PrimaryInteropAssemblyAttribute).GetConstructor(types);
			try
			{
				typeLib2.GetLibAttr(out zero);
				TYPELIBATTR typelibattr = (TYPELIBATTR)Marshal.PtrToStructure(zero, typeof(TYPELIBATTR));
				num = (int)typelibattr.wMajorVerNum;
				num2 = (int)typelibattr.wMinorVerNum;
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					typeLib2.ReleaseTLibAttr(zero);
				}
			}
			object[] constructorArgs = new object[]
			{
				num,
				num2
			};
			CustomAttributeBuilder customAttribute = new CustomAttributeBuilder(constructor, constructorArgs);
			asmBldr.SetCustomAttribute(customAttribute);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void nConvertTypeLibToMetadata(object typeLib, RuntimeAssembly asmBldr, RuntimeModule modBldr, string nameSpace, TypeLibImporterFlags flags, ITypeLibImporterNotifySink notifySink, out ArrayList eventItfInfoList);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object nConvertAssemblyToTypeLib(RuntimeAssembly assembly, string strTypeLibName, TypeLibExporterFlags flags, ITypeLibExporterNotifySink notifySink);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void LoadInMemoryTypeByName(RuntimeModule module, string className);

		private const string s_strTypeLibAssemblyTitlePrefix = "TypeLib ";

		private const string s_strTypeLibAssemblyDescPrefix = "Assembly generated from typelib ";

		private const int MAX_NAMESPACE_LENGTH = 1024;

		private class TypeResolveHandler : ITypeLibImporterNotifySink
		{
			public TypeResolveHandler(ModuleBuilder mod, ITypeLibImporterNotifySink userSink)
			{
				this.m_Module = mod;
				this.m_UserSink = userSink;
			}

			public void ReportEvent(ImporterEventKind eventKind, int eventCode, string eventMsg)
			{
				this.m_UserSink.ReportEvent(eventKind, eventCode, eventMsg);
			}

			public Assembly ResolveRef(object typeLib)
			{
				Assembly assembly = this.m_UserSink.ResolveRef(typeLib);
				if (assembly == null)
				{
					throw new ArgumentNullException();
				}
				RuntimeAssembly runtimeAssembly = assembly as RuntimeAssembly;
				if (runtimeAssembly == null)
				{
					AssemblyBuilder assemblyBuilder = assembly as AssemblyBuilder;
					if (assemblyBuilder != null)
					{
						runtimeAssembly = assemblyBuilder.InternalAssembly;
					}
				}
				if (runtimeAssembly == null)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeAssembly"));
				}
				this.m_AsmList.Add(runtimeAssembly);
				return runtimeAssembly;
			}

			[SecurityCritical]
			public Assembly ResolveEvent(object sender, ResolveEventArgs args)
			{
				try
				{
					TypeLibConverter.LoadInMemoryTypeByName(this.m_Module.GetNativeHandle(), args.Name);
					return this.m_Module.Assembly;
				}
				catch (TypeLoadException ex)
				{
					if (ex.ResourceId != -2146233054)
					{
						throw;
					}
				}
				foreach (RuntimeAssembly runtimeAssembly in this.m_AsmList)
				{
					try
					{
						runtimeAssembly.GetType(args.Name, true, false);
						return runtimeAssembly;
					}
					catch (TypeLoadException ex2)
					{
						if (ex2._HResult != -2146233054)
						{
							throw;
						}
					}
				}
				return null;
			}

			public Assembly ResolveAsmEvent(object sender, ResolveEventArgs args)
			{
				foreach (RuntimeAssembly runtimeAssembly in this.m_AsmList)
				{
					if (string.Compare(runtimeAssembly.FullName, args.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return runtimeAssembly;
					}
				}
				return null;
			}

			public Assembly ResolveROAsmEvent(object sender, ResolveEventArgs args)
			{
				foreach (RuntimeAssembly runtimeAssembly in this.m_AsmList)
				{
					if (string.Compare(runtimeAssembly.FullName, args.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return runtimeAssembly;
					}
				}
				string assemblyString = AppDomain.CurrentDomain.ApplyPolicy(args.Name);
				return Assembly.ReflectionOnlyLoad(assemblyString);
			}

			private ModuleBuilder m_Module;

			private ITypeLibImporterNotifySink m_UserSink;

			private List<RuntimeAssembly> m_AsmList = new List<RuntimeAssembly>();
		}
	}
}
