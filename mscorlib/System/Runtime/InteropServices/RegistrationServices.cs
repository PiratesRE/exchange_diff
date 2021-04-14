using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using Microsoft.Win32;

namespace System.Runtime.InteropServices
{
	[Guid("475E398F-8AFA-43a7-A3BE-F4EF8D6787C9")]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public class RegistrationServices : IRegistrationServices
	{
		[SecurityCritical]
		public virtual bool RegisterAssembly(Assembly assembly, AssemblyRegistrationFlags flags)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (assembly.ReflectionOnly)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_AsmLoadedForReflectionOnly"));
			}
			RuntimeAssembly runtimeAssembly = assembly as RuntimeAssembly;
			if (runtimeAssembly == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeAssembly"));
			}
			string fullName = assembly.FullName;
			if (fullName == null)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NoAsmName"));
			}
			string text = null;
			if ((flags & AssemblyRegistrationFlags.SetCodeBase) != AssemblyRegistrationFlags.None)
			{
				text = runtimeAssembly.GetCodeBase(false);
				if (text == null)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NoAsmCodeBase"));
				}
			}
			Type[] registrableTypesInAssembly = this.GetRegistrableTypesInAssembly(assembly);
			int num = registrableTypesInAssembly.Length;
			string strAsmVersion = runtimeAssembly.GetVersion().ToString();
			string imageRuntimeVersion = assembly.ImageRuntimeVersion;
			for (int i = 0; i < num; i++)
			{
				if (this.IsRegisteredAsValueType(registrableTypesInAssembly[i]))
				{
					this.RegisterValueType(registrableTypesInAssembly[i], fullName, strAsmVersion, text, imageRuntimeVersion);
				}
				else if (this.TypeRepresentsComType(registrableTypesInAssembly[i]))
				{
					this.RegisterComImportedType(registrableTypesInAssembly[i], fullName, strAsmVersion, text, imageRuntimeVersion);
				}
				else
				{
					this.RegisterManagedType(registrableTypesInAssembly[i], fullName, strAsmVersion, text, imageRuntimeVersion);
				}
				this.CallUserDefinedRegistrationMethod(registrableTypesInAssembly[i], true);
			}
			object[] customAttributes = assembly.GetCustomAttributes(typeof(PrimaryInteropAssemblyAttribute), false);
			int num2 = customAttributes.Length;
			for (int j = 0; j < num2; j++)
			{
				this.RegisterPrimaryInteropAssembly(runtimeAssembly, text, (PrimaryInteropAssemblyAttribute)customAttributes[j]);
			}
			return registrableTypesInAssembly.Length != 0 || num2 > 0;
		}

		[SecurityCritical]
		public virtual bool UnregisterAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (assembly.ReflectionOnly)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_AsmLoadedForReflectionOnly"));
			}
			RuntimeAssembly runtimeAssembly = assembly as RuntimeAssembly;
			if (runtimeAssembly == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeAssembly"));
			}
			bool flag = true;
			Type[] registrableTypesInAssembly = this.GetRegistrableTypesInAssembly(assembly);
			int num = registrableTypesInAssembly.Length;
			string strAsmVersion = runtimeAssembly.GetVersion().ToString();
			for (int i = 0; i < num; i++)
			{
				this.CallUserDefinedRegistrationMethod(registrableTypesInAssembly[i], false);
				if (this.IsRegisteredAsValueType(registrableTypesInAssembly[i]))
				{
					if (!this.UnregisterValueType(registrableTypesInAssembly[i], strAsmVersion))
					{
						flag = false;
					}
				}
				else if (this.TypeRepresentsComType(registrableTypesInAssembly[i]))
				{
					if (!this.UnregisterComImportedType(registrableTypesInAssembly[i], strAsmVersion))
					{
						flag = false;
					}
				}
				else if (!this.UnregisterManagedType(registrableTypesInAssembly[i], strAsmVersion))
				{
					flag = false;
				}
			}
			object[] customAttributes = assembly.GetCustomAttributes(typeof(PrimaryInteropAssemblyAttribute), false);
			int num2 = customAttributes.Length;
			if (flag)
			{
				for (int j = 0; j < num2; j++)
				{
					this.UnregisterPrimaryInteropAssembly(assembly, (PrimaryInteropAssemblyAttribute)customAttributes[j]);
				}
			}
			return registrableTypesInAssembly.Length != 0 || num2 > 0;
		}

		[SecurityCritical]
		public virtual Type[] GetRegistrableTypesInAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			if (!(assembly is RuntimeAssembly))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeAssembly"), "assembly");
			}
			Type[] exportedTypes = assembly.GetExportedTypes();
			int num = exportedTypes.Length;
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < num; i++)
			{
				Type type = exportedTypes[i];
				if (this.TypeRequiresRegistration(type))
				{
					arrayList.Add(type);
				}
			}
			Type[] array = new Type[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		[SecurityCritical]
		public virtual string GetProgIdForType(Type type)
		{
			return Marshal.GenerateProgIdForType(type);
		}

		[SecurityCritical]
		public virtual void RegisterTypeForComClients(Type type, ref Guid g)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type as RuntimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
			}
			if (!this.TypeRequiresRegistration(type))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_TypeMustBeComCreatable"), "type");
			}
			RegistrationServices.RegisterTypeForComClientsNative(type, ref g);
		}

		public virtual Guid GetManagedCategoryGuid()
		{
			return RegistrationServices.s_ManagedCategoryGuid;
		}

		[SecurityCritical]
		public virtual bool TypeRequiresRegistration(Type type)
		{
			return RegistrationServices.TypeRequiresRegistrationHelper(type);
		}

		[SecuritySafeCritical]
		public virtual bool TypeRepresentsComType(Type type)
		{
			if (!type.IsCOMObject)
			{
				return false;
			}
			if (type.IsImport)
			{
				return true;
			}
			Type baseComImportType = this.GetBaseComImportType(type);
			return Marshal.GenerateGuidForType(type) == Marshal.GenerateGuidForType(baseComImportType);
		}

		[SecurityCritical]
		[ComVisible(false)]
		public virtual int RegisterTypeForComClients(Type type, RegistrationClassContext classContext, RegistrationConnectionType flags)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type as RuntimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "type");
			}
			if (!this.TypeRequiresRegistration(type))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_TypeMustBeComCreatable"), "type");
			}
			return RegistrationServices.RegisterTypeForComClientsExNative(type, classContext, flags);
		}

		[SecurityCritical]
		[ComVisible(false)]
		public virtual void UnregisterTypeForComClients(int cookie)
		{
			RegistrationServices.CoRevokeClassObject(cookie);
		}

		[SecurityCritical]
		internal static bool TypeRequiresRegistrationHelper(Type type)
		{
			return (type.IsClass || type.IsValueType) && !type.IsAbstract && (type.IsValueType || !(type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null) == null)) && Marshal.IsTypeVisibleFromCom(type);
		}

		[SecurityCritical]
		private void RegisterValueType(Type type, string strAsmName, string strAsmVersion, string strAsmCodeBase, string strRuntimeVersion)
		{
			string subkey = "{" + Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey("Record"))
			{
				using (RegistryKey registryKey2 = registryKey.CreateSubKey(subkey))
				{
					using (RegistryKey registryKey3 = registryKey2.CreateSubKey(strAsmVersion))
					{
						registryKey3.SetValue("Class", type.FullName);
						registryKey3.SetValue("Assembly", strAsmName);
						registryKey3.SetValue("RuntimeVersion", strRuntimeVersion);
						if (strAsmCodeBase != null)
						{
							registryKey3.SetValue("CodeBase", strAsmCodeBase);
						}
					}
				}
			}
		}

		[SecurityCritical]
		private void RegisterManagedType(Type type, string strAsmName, string strAsmVersion, string strAsmCodeBase, string strRuntimeVersion)
		{
			string value = type.FullName ?? "";
			string text = "{" + Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			string progIdForType = this.GetProgIdForType(type);
			if (progIdForType != string.Empty)
			{
				using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(progIdForType))
				{
					registryKey.SetValue("", value);
					using (RegistryKey registryKey2 = registryKey.CreateSubKey("CLSID"))
					{
						registryKey2.SetValue("", text);
					}
				}
			}
			using (RegistryKey registryKey3 = Registry.ClassesRoot.CreateSubKey("CLSID"))
			{
				using (RegistryKey registryKey4 = registryKey3.CreateSubKey(text))
				{
					registryKey4.SetValue("", value);
					using (RegistryKey registryKey5 = registryKey4.CreateSubKey("InprocServer32"))
					{
						registryKey5.SetValue("", "mscoree.dll");
						registryKey5.SetValue("ThreadingModel", "Both");
						registryKey5.SetValue("Class", type.FullName);
						registryKey5.SetValue("Assembly", strAsmName);
						registryKey5.SetValue("RuntimeVersion", strRuntimeVersion);
						if (strAsmCodeBase != null)
						{
							registryKey5.SetValue("CodeBase", strAsmCodeBase);
						}
						using (RegistryKey registryKey6 = registryKey5.CreateSubKey(strAsmVersion))
						{
							registryKey6.SetValue("Class", type.FullName);
							registryKey6.SetValue("Assembly", strAsmName);
							registryKey6.SetValue("RuntimeVersion", strRuntimeVersion);
							if (strAsmCodeBase != null)
							{
								registryKey6.SetValue("CodeBase", strAsmCodeBase);
							}
						}
						if (progIdForType != string.Empty)
						{
							using (RegistryKey registryKey7 = registryKey4.CreateSubKey("ProgId"))
							{
								registryKey7.SetValue("", progIdForType);
							}
						}
					}
					using (RegistryKey registryKey8 = registryKey4.CreateSubKey("Implemented Categories"))
					{
						using (registryKey8.CreateSubKey("{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}"))
						{
						}
					}
				}
			}
			this.EnsureManagedCategoryExists();
		}

		[SecurityCritical]
		private void RegisterComImportedType(Type type, string strAsmName, string strAsmVersion, string strAsmCodeBase, string strRuntimeVersion)
		{
			string subkey = "{" + Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey("CLSID"))
			{
				using (RegistryKey registryKey2 = registryKey.CreateSubKey(subkey))
				{
					using (RegistryKey registryKey3 = registryKey2.CreateSubKey("InprocServer32"))
					{
						registryKey3.SetValue("Class", type.FullName);
						registryKey3.SetValue("Assembly", strAsmName);
						registryKey3.SetValue("RuntimeVersion", strRuntimeVersion);
						if (strAsmCodeBase != null)
						{
							registryKey3.SetValue("CodeBase", strAsmCodeBase);
						}
						using (RegistryKey registryKey4 = registryKey3.CreateSubKey(strAsmVersion))
						{
							registryKey4.SetValue("Class", type.FullName);
							registryKey4.SetValue("Assembly", strAsmName);
							registryKey4.SetValue("RuntimeVersion", strRuntimeVersion);
							if (strAsmCodeBase != null)
							{
								registryKey4.SetValue("CodeBase", strAsmCodeBase);
							}
						}
					}
				}
			}
		}

		[SecurityCritical]
		private bool UnregisterValueType(Type type, string strAsmVersion)
		{
			bool result = true;
			string text = "{" + Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("Record", true))
			{
				if (registryKey != null)
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
					{
						if (registryKey2 != null)
						{
							using (RegistryKey registryKey3 = registryKey2.OpenSubKey(strAsmVersion, true))
							{
								if (registryKey3 != null)
								{
									registryKey3.DeleteValue("Assembly", false);
									registryKey3.DeleteValue("Class", false);
									registryKey3.DeleteValue("CodeBase", false);
									registryKey3.DeleteValue("RuntimeVersion", false);
									if (registryKey3.SubKeyCount == 0 && registryKey3.ValueCount == 0)
									{
										registryKey2.DeleteSubKey(strAsmVersion);
									}
								}
							}
							if (registryKey2.SubKeyCount != 0)
							{
								result = false;
							}
							if (registryKey2.SubKeyCount == 0 && registryKey2.ValueCount == 0)
							{
								registryKey.DeleteSubKey(text);
							}
						}
					}
					if (registryKey.SubKeyCount == 0 && registryKey.ValueCount == 0)
					{
						Registry.ClassesRoot.DeleteSubKey("Record");
					}
				}
			}
			return result;
		}

		[SecurityCritical]
		private bool UnregisterManagedType(Type type, string strAsmVersion)
		{
			bool flag = true;
			string text = "{" + Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			string progIdForType = this.GetProgIdForType(type);
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("CLSID", true))
			{
				if (registryKey != null)
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
					{
						if (registryKey2 != null)
						{
							using (RegistryKey registryKey3 = registryKey2.OpenSubKey("InprocServer32", true))
							{
								if (registryKey3 != null)
								{
									using (RegistryKey registryKey4 = registryKey3.OpenSubKey(strAsmVersion, true))
									{
										if (registryKey4 != null)
										{
											registryKey4.DeleteValue("Assembly", false);
											registryKey4.DeleteValue("Class", false);
											registryKey4.DeleteValue("RuntimeVersion", false);
											registryKey4.DeleteValue("CodeBase", false);
											if (registryKey4.SubKeyCount == 0 && registryKey4.ValueCount == 0)
											{
												registryKey3.DeleteSubKey(strAsmVersion);
											}
										}
									}
									if (registryKey3.SubKeyCount != 0)
									{
										flag = false;
									}
									if (flag)
									{
										registryKey3.DeleteValue("", false);
										registryKey3.DeleteValue("ThreadingModel", false);
									}
									registryKey3.DeleteValue("Assembly", false);
									registryKey3.DeleteValue("Class", false);
									registryKey3.DeleteValue("RuntimeVersion", false);
									registryKey3.DeleteValue("CodeBase", false);
									if (registryKey3.SubKeyCount == 0 && registryKey3.ValueCount == 0)
									{
										registryKey2.DeleteSubKey("InprocServer32");
									}
								}
							}
							if (flag)
							{
								registryKey2.DeleteValue("", false);
								if (progIdForType != string.Empty)
								{
									using (RegistryKey registryKey5 = registryKey2.OpenSubKey("ProgId", true))
									{
										if (registryKey5 != null)
										{
											registryKey5.DeleteValue("", false);
											if (registryKey5.SubKeyCount == 0 && registryKey5.ValueCount == 0)
											{
												registryKey2.DeleteSubKey("ProgId");
											}
										}
									}
								}
								using (RegistryKey registryKey6 = registryKey2.OpenSubKey("Implemented Categories", true))
								{
									if (registryKey6 != null)
									{
										using (RegistryKey registryKey7 = registryKey6.OpenSubKey("{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}", true))
										{
											if (registryKey7 != null && registryKey7.SubKeyCount == 0 && registryKey7.ValueCount == 0)
											{
												registryKey6.DeleteSubKey("{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}");
											}
										}
										if (registryKey6.SubKeyCount == 0 && registryKey6.ValueCount == 0)
										{
											registryKey2.DeleteSubKey("Implemented Categories");
										}
									}
								}
							}
							if (registryKey2.SubKeyCount == 0 && registryKey2.ValueCount == 0)
							{
								registryKey.DeleteSubKey(text);
							}
						}
					}
					if (registryKey.SubKeyCount == 0 && registryKey.ValueCount == 0)
					{
						Registry.ClassesRoot.DeleteSubKey("CLSID");
					}
				}
				if (flag && progIdForType != string.Empty)
				{
					using (RegistryKey registryKey8 = Registry.ClassesRoot.OpenSubKey(progIdForType, true))
					{
						if (registryKey8 != null)
						{
							registryKey8.DeleteValue("", false);
							using (RegistryKey registryKey9 = registryKey8.OpenSubKey("CLSID", true))
							{
								if (registryKey9 != null)
								{
									registryKey9.DeleteValue("", false);
									if (registryKey9.SubKeyCount == 0 && registryKey9.ValueCount == 0)
									{
										registryKey8.DeleteSubKey("CLSID");
									}
								}
							}
							if (registryKey8.SubKeyCount == 0 && registryKey8.ValueCount == 0)
							{
								Registry.ClassesRoot.DeleteSubKey(progIdForType);
							}
						}
					}
				}
			}
			return flag;
		}

		[SecurityCritical]
		private bool UnregisterComImportedType(Type type, string strAsmVersion)
		{
			bool result = true;
			string text = "{" + Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("CLSID", true))
			{
				if (registryKey != null)
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
					{
						if (registryKey2 != null)
						{
							using (RegistryKey registryKey3 = registryKey2.OpenSubKey("InprocServer32", true))
							{
								if (registryKey3 != null)
								{
									registryKey3.DeleteValue("Assembly", false);
									registryKey3.DeleteValue("Class", false);
									registryKey3.DeleteValue("RuntimeVersion", false);
									registryKey3.DeleteValue("CodeBase", false);
									using (RegistryKey registryKey4 = registryKey3.OpenSubKey(strAsmVersion, true))
									{
										if (registryKey4 != null)
										{
											registryKey4.DeleteValue("Assembly", false);
											registryKey4.DeleteValue("Class", false);
											registryKey4.DeleteValue("RuntimeVersion", false);
											registryKey4.DeleteValue("CodeBase", false);
											if (registryKey4.SubKeyCount == 0 && registryKey4.ValueCount == 0)
											{
												registryKey3.DeleteSubKey(strAsmVersion);
											}
										}
									}
									if (registryKey3.SubKeyCount != 0)
									{
										result = false;
									}
									if (registryKey3.SubKeyCount == 0 && registryKey3.ValueCount == 0)
									{
										registryKey2.DeleteSubKey("InprocServer32");
									}
								}
							}
							if (registryKey2.SubKeyCount == 0 && registryKey2.ValueCount == 0)
							{
								registryKey.DeleteSubKey(text);
							}
						}
					}
					if (registryKey.SubKeyCount == 0 && registryKey.ValueCount == 0)
					{
						Registry.ClassesRoot.DeleteSubKey("CLSID");
					}
				}
			}
			return result;
		}

		[SecurityCritical]
		private void RegisterPrimaryInteropAssembly(RuntimeAssembly assembly, string strAsmCodeBase, PrimaryInteropAssemblyAttribute attr)
		{
			if (assembly.GetPublicKey().Length == 0)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_PIAMustBeStrongNamed"));
			}
			string subkey = "{" + Marshal.GetTypeLibGuidForAssembly(assembly).ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			string subkey2 = attr.MajorVersion.ToString("x", CultureInfo.InvariantCulture) + "." + attr.MinorVersion.ToString("x", CultureInfo.InvariantCulture);
			using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey("TypeLib"))
			{
				using (RegistryKey registryKey2 = registryKey.CreateSubKey(subkey))
				{
					using (RegistryKey registryKey3 = registryKey2.CreateSubKey(subkey2))
					{
						registryKey3.SetValue("PrimaryInteropAssemblyName", assembly.FullName);
						if (strAsmCodeBase != null)
						{
							registryKey3.SetValue("PrimaryInteropAssemblyCodeBase", strAsmCodeBase);
						}
					}
				}
			}
		}

		[SecurityCritical]
		private void UnregisterPrimaryInteropAssembly(Assembly assembly, PrimaryInteropAssemblyAttribute attr)
		{
			string text = "{" + Marshal.GetTypeLibGuidForAssembly(assembly).ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
			string text2 = attr.MajorVersion.ToString("x", CultureInfo.InvariantCulture) + "." + attr.MinorVersion.ToString("x", CultureInfo.InvariantCulture);
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("TypeLib", true))
			{
				if (registryKey != null)
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey(text, true))
					{
						if (registryKey2 != null)
						{
							using (RegistryKey registryKey3 = registryKey2.OpenSubKey(text2, true))
							{
								if (registryKey3 != null)
								{
									registryKey3.DeleteValue("PrimaryInteropAssemblyName", false);
									registryKey3.DeleteValue("PrimaryInteropAssemblyCodeBase", false);
									if (registryKey3.SubKeyCount == 0 && registryKey3.ValueCount == 0)
									{
										registryKey2.DeleteSubKey(text2);
									}
								}
							}
							if (registryKey2.SubKeyCount == 0 && registryKey2.ValueCount == 0)
							{
								registryKey.DeleteSubKey(text);
							}
						}
					}
					if (registryKey.SubKeyCount == 0 && registryKey.ValueCount == 0)
					{
						Registry.ClassesRoot.DeleteSubKey("TypeLib");
					}
				}
			}
		}

		private void EnsureManagedCategoryExists()
		{
			if (!RegistrationServices.ManagedCategoryExists())
			{
				using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey("Component Categories"))
				{
					using (RegistryKey registryKey2 = registryKey.CreateSubKey("{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}"))
					{
						registryKey2.SetValue("0", ".NET Category");
					}
				}
			}
		}

		private static bool ManagedCategoryExists()
		{
			using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("Component Categories", RegistryKeyPermissionCheck.ReadSubTree))
			{
				if (registryKey == null)
				{
					return false;
				}
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}", RegistryKeyPermissionCheck.ReadSubTree))
				{
					if (registryKey2 == null)
					{
						return false;
					}
					object value = registryKey2.GetValue("0");
					if (value == null || value.GetType() != typeof(string))
					{
						return false;
					}
					string a = (string)value;
					if (a != ".NET Category")
					{
						return false;
					}
				}
			}
			return true;
		}

		[SecurityCritical]
		private void CallUserDefinedRegistrationMethod(Type type, bool bRegister)
		{
			bool flag = false;
			Type typeFromHandle;
			if (bRegister)
			{
				typeFromHandle = typeof(ComRegisterFunctionAttribute);
			}
			else
			{
				typeFromHandle = typeof(ComUnregisterFunctionAttribute);
			}
			Type type2 = type;
			while (!flag && type2 != null)
			{
				MethodInfo[] methods = type2.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				int num = methods.Length;
				for (int i = 0; i < num; i++)
				{
					MethodInfo methodInfo = methods[i];
					if (methodInfo.GetCustomAttributes(typeFromHandle, true).Length != 0)
					{
						if (!methodInfo.IsStatic)
						{
							if (bRegister)
							{
								throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NonStaticComRegFunction", new object[]
								{
									methodInfo.Name,
									type2.Name
								}));
							}
							throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NonStaticComUnRegFunction", new object[]
							{
								methodInfo.Name,
								type2.Name
							}));
						}
						else
						{
							ParameterInfo[] parameters = methodInfo.GetParameters();
							if (methodInfo.ReturnType != typeof(void) || parameters == null || parameters.Length != 1 || (parameters[0].ParameterType != typeof(string) && parameters[0].ParameterType != typeof(Type)))
							{
								if (bRegister)
								{
									throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_InvalidComRegFunctionSig", new object[]
									{
										methodInfo.Name,
										type2.Name
									}));
								}
								throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_InvalidComUnRegFunctionSig", new object[]
								{
									methodInfo.Name,
									type2.Name
								}));
							}
							else if (flag)
							{
								if (bRegister)
								{
									throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_MultipleComRegFunctions", new object[]
									{
										type2.Name
									}));
								}
								throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_MultipleComUnRegFunctions", new object[]
								{
									type2.Name
								}));
							}
							else
							{
								object[] array = new object[1];
								if (parameters[0].ParameterType == typeof(string))
								{
									array[0] = "HKEY_CLASSES_ROOT\\CLSID\\{" + Marshal.GenerateGuidForType(type).ToString().ToUpper(CultureInfo.InvariantCulture) + "}";
								}
								else
								{
									array[0] = type;
								}
								methodInfo.Invoke(null, array);
								flag = true;
							}
						}
					}
				}
				type2 = type2.BaseType;
			}
		}

		private Type GetBaseComImportType(Type type)
		{
			while (type != null && !type.IsImport)
			{
				type = type.BaseType;
			}
			return type;
		}

		private bool IsRegisteredAsValueType(Type type)
		{
			return type.IsValueType;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterTypeForComClientsNative(Type type, ref Guid g);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RegisterTypeForComClientsExNative(Type t, RegistrationClassContext clsContext, RegistrationConnectionType flags);

		[DllImport("ole32.dll", CharSet = CharSet.Auto, PreserveSig = false)]
		private static extern void CoRevokeClassObject(int cookie);

		private const string strManagedCategoryGuid = "{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}";

		private const string strDocStringPrefix = "";

		private const string strManagedTypeThreadingModel = "Both";

		private const string strComponentCategorySubKey = "Component Categories";

		private const string strManagedCategoryDescription = ".NET Category";

		private const string strImplementedCategoriesSubKey = "Implemented Categories";

		private const string strMsCorEEFileName = "mscoree.dll";

		private const string strRecordRootName = "Record";

		private const string strClsIdRootName = "CLSID";

		private const string strTlbRootName = "TypeLib";

		private static Guid s_ManagedCategoryGuid = new Guid("{62C8FE65-4EBB-45e7-B440-6E39B2CDBF29}");
	}
}
