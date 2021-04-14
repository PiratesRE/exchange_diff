using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Win32;

namespace Microsoft.Exchange.Security.Dkm.Proxy
{
	internal sealed class DkmProxy
	{
		public DkmProxy(string groupName, string dkmPath = null, string dkmParentContainerDN = null)
		{
			if (DkmProxy.dkmAssembly == null)
			{
				if (!string.IsNullOrEmpty(dkmPath))
				{
					DkmProxy.dkmAssembly = Assembly.LoadFrom(Path.Combine(dkmPath, "Microsoft.Cryptography.DKM.dll"));
				}
				else
				{
					try
					{
						DkmProxy.dkmAssembly = Assembly.LoadFrom("Microsoft.Cryptography.DKM.dll");
					}
					catch (FileNotFoundException)
					{
						DkmProxy.dkmAssembly = Assembly.LoadFrom(Path.Combine(DkmProxy.GetExchangeInstallPath(), string.Format("Bin\\{0}", "Microsoft.Cryptography.DKM.dll")));
					}
				}
			}
			if (DkmProxy.typeGroupKey == null)
			{
				DkmProxy.typeGroupKey = DkmProxy.dkmAssembly.GetType("Microsoft.Incubation.Crypto.GroupKeys.GroupKey");
			}
			if (DkmProxy.typeDkmException == null)
			{
				DkmProxy.typeDkmException = DkmProxy.dkmAssembly.GetType("Microsoft.Incubation.Crypto.GroupKeys.DkmException");
			}
			if (DkmProxy.dkmObjectField == null)
			{
				DkmProxy.dkmObjectField = typeof(DkmProxy).GetField("instanceDkm", BindingFlags.Instance | BindingFlags.NonPublic);
			}
			if (DkmProxy.typeIADRepository == null)
			{
				DkmProxy.typeIADRepository = DkmProxy.dkmAssembly.GetType("Microsoft.Incubation.Crypto.GroupKeys.IADRepository");
			}
			object[] array = new object[2];
			array[0] = groupName;
			object[] args = array;
			this.instanceDkm = Activator.CreateInstance(DkmProxy.typeGroupKey, args);
			PropertyInfo property = DkmProxy.typeGroupKey.GetProperty("Repository", DkmProxy.typeIADRepository);
			this.encryptDelegate = (DkmProxy.EncryptDecryptDelegate)this.GetMethodDelegate(DkmProxy.typeGroupKey, "Protect", new Type[]
			{
				typeof(MemoryStream)
			}, typeof(DkmProxy.EncryptDecryptDelegate));
			this.decryptDelegate = (DkmProxy.EncryptDecryptDelegate)this.GetMethodDelegate(DkmProxy.typeGroupKey, "Unprotect", new Type[]
			{
				typeof(MemoryStream)
			}, typeof(DkmProxy.EncryptDecryptDelegate));
			this.instanceRepository = property.GetValue(this.instanceDkm, null);
			if (!string.IsNullOrEmpty(dkmParentContainerDN))
			{
				this.DkmParentContainerDN = dkmParentContainerDN;
			}
		}

		public string PreferredReplicaName
		{
			set
			{
				PropertyInfo propertyInfoOrFail = this.GetPropertyInfoOrFail(DkmProxy.typeIADRepository, "PreferredReplicaName");
				propertyInfoOrFail.SetValue(this.instanceRepository, value, null);
			}
		}

		public string DkmParentContainerDN
		{
			set
			{
				PropertyInfo propertyInfoOrFail = this.GetPropertyInfoOrFail(DkmProxy.typeIADRepository, "DkmParentContainerDN");
				propertyInfoOrFail.SetValue(this.instanceRepository, value, null);
			}
		}

		public string DkmContainerName
		{
			set
			{
				PropertyInfo propertyInfoOrFail = this.GetPropertyInfoOrFail(DkmProxy.typeIADRepository, "DkmContainerName");
				propertyInfoOrFail.SetValue(this.instanceRepository, value, null);
			}
		}

		public void InitializeDkm()
		{
			MethodInfo methodInfoOrFail = this.GetMethodInfoOrFail(DkmProxy.typeGroupKey, "InitializeDkm", new Type[0]);
			try
			{
				methodInfoOrFail.Invoke(this.instanceDkm, null);
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException.GetType().Name == "ObjectAlreadyExistsException")
				{
					throw new ObjectAlreadyExistsException(ex.InnerException.Message);
				}
				throw;
			}
		}

		public void AddGroup()
		{
			MethodInfo methodInfoOrFail = this.GetMethodInfoOrFail(DkmProxy.typeGroupKey, "AddGroup", new Type[0]);
			try
			{
				methodInfoOrFail.Invoke(this.instanceDkm, null);
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException.GetType().Name == "ObjectAlreadyExistsException")
				{
					throw new ObjectAlreadyExistsException(ex.InnerException.Message);
				}
				throw;
			}
		}

		public void AddGroupMemberWithUpdateRights(IdentityReference identity)
		{
			object[] parameters = new object[]
			{
				identity
			};
			MethodInfo methodInfoOrFail = this.GetMethodInfoOrFail(this.instanceRepository.GetType(), "AddGroupMemberWithUpdateRights", new Type[]
			{
				typeof(IdentityReference)
			});
			methodInfoOrFail.Invoke(this.instanceRepository, parameters);
		}

		public void UninitializeDkm()
		{
			MethodInfo methodInfoOrFail = this.GetMethodInfoOrFail(DkmProxy.typeGroupKey, "UninitializeDkm", new Type[0]);
			try
			{
				methodInfoOrFail.Invoke(this.instanceDkm, null);
			}
			catch (TargetInvocationException ex)
			{
				if (ex.InnerException.GetType().Name == "ObjectNotFoundException")
				{
					throw new ObjectNotFoundException(ex.InnerException.Message);
				}
				throw;
			}
		}

		public bool IsDkmException(Exception ex)
		{
			return DkmProxy.typeDkmException.IsAssignableFrom(ex.GetType());
		}

		public void AddGroupOwner(IdentityReference identity)
		{
			object[] parameters = new object[]
			{
				identity
			};
			MethodInfo methodInfoOrFail = this.GetMethodInfoOrFail(this.instanceRepository.GetType(), "AddGroupOwner", new Type[]
			{
				typeof(IdentityReference)
			});
			methodInfoOrFail.Invoke(this.instanceRepository, parameters);
		}

		public void UpdateKey(bool forcedUpdate)
		{
			object[] parameters = new object[]
			{
				forcedUpdate
			};
			MethodInfo methodInfoOrFail = this.GetMethodInfoOrFail(DkmProxy.typeGroupKey, "UpdateKey", new Type[]
			{
				typeof(bool)
			});
			methodInfoOrFail.Invoke(this.instanceDkm, parameters);
		}

		public MemoryStream Protect(MemoryStream inStream)
		{
			return this.encryptDelegate(inStream);
		}

		public MemoryStream Unprotect(MemoryStream inStream)
		{
			return this.decryptDelegate(inStream);
		}

		private static string GetExchangeInstallPath()
		{
			return (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", null);
		}

		private PropertyInfo GetPropertyInfoOrFail(Type type, string propName)
		{
			PropertyInfo property = type.GetProperty(propName);
			if (property == null)
			{
				throw new InvalidOperationException();
			}
			return property;
		}

		private MethodInfo GetMethodInfoOrFail(Type type, string methodName, Type[] argTypes)
		{
			MethodInfo method = type.GetMethod(methodName, argTypes);
			if (method == null)
			{
				throw new InvalidOperationException();
			}
			return method;
		}

		private Delegate GetMethodDelegate(Type methodType, string methodName, Type[] methodParamTypes, Type delegateType)
		{
			MethodInfo method = methodType.GetMethod(methodName, methodParamTypes);
			return Delegate.CreateDelegate(delegateType, this.instanceDkm, method);
		}

		private const string AssemblyName = "Microsoft.Cryptography.DKM.dll";

		private const string DkmTypeFullName = "Microsoft.Incubation.Crypto.GroupKeys.GroupKey";

		private const string DkmExceptionTypeFullName = "Microsoft.Incubation.Crypto.GroupKeys.DkmException";

		private const string DkmRespositoryTypeFullName = "Microsoft.Incubation.Crypto.GroupKeys.IADRepository";

		private static Assembly dkmAssembly;

		private static Type typeGroupKey;

		private static Type typeDkmException;

		private static FieldInfo dkmObjectField;

		private static Type typeIADRepository;

		private readonly object instanceDkm;

		private readonly object instanceRepository;

		private readonly DkmProxy.EncryptDecryptDelegate encryptDelegate;

		private readonly DkmProxy.EncryptDecryptDelegate decryptDelegate;

		private delegate MemoryStream EncryptDecryptDelegate(MemoryStream memstream);
	}
}
