using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Util;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace System.Reflection
{
	[Serializable]
	internal class RuntimeAssembly : Assembly, ICustomQueryInterface
	{
		[SecurityCritical]
		CustomQueryInterfaceResult ICustomQueryInterface.GetInterface([In] ref Guid iid, out IntPtr ppv)
		{
			if (iid == typeof(NativeMethods.IDispatch).GUID)
			{
				ppv = Marshal.GetComInterfaceForObject(this, typeof(_Assembly));
				return CustomQueryInterfaceResult.Handled;
			}
			ppv = IntPtr.Zero;
			return CustomQueryInterfaceResult.NotHandled;
		}

		internal RuntimeAssembly()
		{
			throw new NotSupportedException();
		}

		[method: SecurityCritical]
		private event ModuleResolveEventHandler _ModuleResolve;

		internal int InvocableAttributeCtorToken
		{
			get
			{
				int num = (int)(this.Flags & RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_TOKEN_MASK);
				return num | 100663296;
			}
		}

		private RuntimeAssembly.ASSEMBLY_FLAGS Flags
		{
			[SecuritySafeCritical]
			get
			{
				if ((this.m_flags & RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_INITIALIZED) == RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_UNKNOWN)
				{
					RuntimeAssembly.ASSEMBLY_FLAGS assembly_FLAGS = RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_UNKNOWN;
					if (RuntimeAssembly.IsFrameworkAssembly(this.GetName()))
					{
						assembly_FLAGS |= (RuntimeAssembly.ASSEMBLY_FLAGS)100663296U;
						foreach (string strB in RuntimeAssembly.s_unsafeFrameworkAssemblyNames)
						{
							if (string.Compare(this.GetSimpleName(), strB, StringComparison.OrdinalIgnoreCase) == 0)
							{
								assembly_FLAGS &= (RuntimeAssembly.ASSEMBLY_FLAGS)4227858431U;
								break;
							}
						}
						Type type = this.GetType("__DynamicallyInvokableAttribute", false);
						if (type != null)
						{
							ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
							int metadataToken = constructor.MetadataToken;
							assembly_FLAGS |= (RuntimeAssembly.ASSEMBLY_FLAGS)(metadataToken & 16777215);
						}
					}
					else if (this.IsDesignerBindingContext())
					{
						assembly_FLAGS = RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_SAFE_REFLECTION;
					}
					this.m_flags = (assembly_FLAGS | RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_INITIALIZED);
				}
				return this.m_flags;
			}
		}

		internal object SyncRoot
		{
			get
			{
				if (this.m_syncRoot == null)
				{
					Interlocked.CompareExchange<object>(ref this.m_syncRoot, new object(), null);
				}
				return this.m_syncRoot;
			}
		}

		public override event ModuleResolveEventHandler ModuleResolve
		{
			[SecurityCritical]
			add
			{
				this._ModuleResolve += value;
			}
			[SecurityCritical]
			remove
			{
				this._ModuleResolve -= value;
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetCodeBase(RuntimeAssembly assembly, bool copiedName, StringHandleOnStack retString);

		[SecurityCritical]
		internal string GetCodeBase(bool copiedName)
		{
			string result = null;
			RuntimeAssembly.GetCodeBase(this.GetNativeHandle(), copiedName, JitHelpers.GetStringHandleOnStack(ref result));
			return result;
		}

		public override string CodeBase
		{
			[SecuritySafeCritical]
			get
			{
				string codeBase = this.GetCodeBase(false);
				this.VerifyCodeBaseDiscovery(codeBase);
				return codeBase;
			}
		}

		internal RuntimeAssembly GetNativeHandle()
		{
			return this;
		}

		[SecuritySafeCritical]
		public override AssemblyName GetName(bool copiedName)
		{
			AssemblyName assemblyName = new AssemblyName();
			string codeBase = this.GetCodeBase(copiedName);
			this.VerifyCodeBaseDiscovery(codeBase);
			assemblyName.Init(this.GetSimpleName(), this.GetPublicKey(), null, this.GetVersion(), this.GetLocale(), this.GetHashAlgorithm(), AssemblyVersionCompatibility.SameMachine, codeBase, this.GetFlags() | AssemblyNameFlags.PublicKey, null);
			Module manifestModule = this.ManifestModule;
			if (manifestModule != null && manifestModule.MDStreamVersion > 65536)
			{
				PortableExecutableKinds pek;
				ImageFileMachine ifm;
				this.ManifestModule.GetPEKind(out pek, out ifm);
				assemblyName.SetProcArchIndex(pek, ifm);
			}
			return assemblyName;
		}

		[SecurityCritical]
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		private string GetNameForConditionalAptca()
		{
			AssemblyName name = this.GetName();
			return name.GetNameWithPublicKey();
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetFullName(RuntimeAssembly assembly, StringHandleOnStack retString);

		public override string FullName
		{
			[SecuritySafeCritical]
			get
			{
				if (this.m_fullname == null)
				{
					string value = null;
					RuntimeAssembly.GetFullName(this.GetNativeHandle(), JitHelpers.GetStringHandleOnStack(ref value));
					Interlocked.CompareExchange<string>(ref this.m_fullname, value, null);
				}
				return this.m_fullname;
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetEntryPoint(RuntimeAssembly assembly, ObjectHandleOnStack retMethod);

		public override MethodInfo EntryPoint
		{
			[SecuritySafeCritical]
			get
			{
				IRuntimeMethodInfo runtimeMethodInfo = null;
				RuntimeAssembly.GetEntryPoint(this.GetNativeHandle(), JitHelpers.GetObjectHandleOnStack<IRuntimeMethodInfo>(ref runtimeMethodInfo));
				if (runtimeMethodInfo == null)
				{
					return null;
				}
				return (MethodInfo)RuntimeType.GetMethodBase(runtimeMethodInfo);
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetType(RuntimeAssembly assembly, string name, bool throwOnError, bool ignoreCase, ObjectHandleOnStack type);

		[SecuritySafeCritical]
		public override Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			RuntimeType result = null;
			RuntimeAssembly.GetType(this.GetNativeHandle(), name, throwOnError, ignoreCase, JitHelpers.GetObjectHandleOnStack<RuntimeType>(ref result));
			return result;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		internal static extern void GetForwardedTypes(RuntimeAssembly assembly, ObjectHandleOnStack retTypes);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetExportedTypes(RuntimeAssembly assembly, ObjectHandleOnStack retTypes);

		[SecuritySafeCritical]
		public override Type[] GetExportedTypes()
		{
			Type[] result = null;
			RuntimeAssembly.GetExportedTypes(this.GetNativeHandle(), JitHelpers.GetObjectHandleOnStack<Type[]>(ref result));
			return result;
		}

		public override IEnumerable<TypeInfo> DefinedTypes
		{
			[SecuritySafeCritical]
			get
			{
				List<RuntimeType> list = new List<RuntimeType>();
				RuntimeModule[] modulesInternal = this.GetModulesInternal(true, false);
				for (int i = 0; i < modulesInternal.Length; i++)
				{
					list.AddRange(modulesInternal[i].GetDefinedTypes());
				}
				return list.ToArray();
			}
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public override Stream GetManifestResourceStream(Type type, string name)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return this.GetManifestResourceStream(type, name, false, ref stackCrawlMark);
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public override Stream GetManifestResourceStream(string name)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return this.GetManifestResourceStream(name, ref stackCrawlMark, false);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetEvidence(RuntimeAssembly assembly, ObjectHandleOnStack retEvidence);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern SecurityRuleSet GetSecurityRuleSet(RuntimeAssembly assembly);

		public override Evidence Evidence
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true)]
			get
			{
				Evidence evidenceNoDemand = this.EvidenceNoDemand;
				return evidenceNoDemand.Clone();
			}
		}

		internal Evidence EvidenceNoDemand
		{
			[SecurityCritical]
			get
			{
				Evidence result = null;
				RuntimeAssembly.GetEvidence(this.GetNativeHandle(), JitHelpers.GetObjectHandleOnStack<Evidence>(ref result));
				return result;
			}
		}

		public override PermissionSet PermissionSet
		{
			[SecurityCritical]
			get
			{
				PermissionSet permissionSet = null;
				PermissionSet permissionSet2 = null;
				this.GetGrantSet(out permissionSet, out permissionSet2);
				if (permissionSet != null)
				{
					return permissionSet.Copy();
				}
				return new PermissionSet(PermissionState.Unrestricted);
			}
		}

		public override SecurityRuleSet SecurityRuleSet
		{
			[SecuritySafeCritical]
			get
			{
				return RuntimeAssembly.GetSecurityRuleSet(this.GetNativeHandle());
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetUnitySerializationInfo(info, 6, this.FullName, this);
		}

		public override Module ManifestModule
		{
			get
			{
				return RuntimeAssembly.GetManifestModule(this.GetNativeHandle());
			}
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return CustomAttribute.GetCustomAttributes(this, typeof(object) as RuntimeType);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			RuntimeType runtimeType = attributeType.UnderlyingSystemType as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "attributeType");
			}
			return CustomAttribute.GetCustomAttributes(this, runtimeType);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			RuntimeType runtimeType = attributeType.UnderlyingSystemType as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "caType");
			}
			return CustomAttribute.IsDefined(this, runtimeType);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributesInternal(this);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static RuntimeAssembly InternalLoadFrom(string assemblyFile, Evidence securityEvidence, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm, bool forIntrospection, bool suppressSecurityChecks, ref StackCrawlMark stackMark)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			if (securityEvidence != null && !AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
			{
				throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyImplicit"));
			}
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.CodeBase = assemblyFile;
			assemblyName.SetHashControl(hashValue, hashAlgorithm);
			return RuntimeAssembly.InternalLoadAssemblyName(assemblyName, securityEvidence, null, ref stackMark, true, forIntrospection, suppressSecurityChecks);
		}

		[SecurityCritical]
		internal static RuntimeAssembly InternalLoad(string assemblyString, Evidence assemblySecurity, ref StackCrawlMark stackMark, bool forIntrospection)
		{
			return RuntimeAssembly.InternalLoad(assemblyString, assemblySecurity, ref stackMark, IntPtr.Zero, forIntrospection);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static RuntimeAssembly InternalLoad(string assemblyString, Evidence assemblySecurity, ref StackCrawlMark stackMark, IntPtr pPrivHostBinder, bool forIntrospection)
		{
			RuntimeAssembly runtimeAssembly;
			AssemblyName assemblyRef = RuntimeAssembly.CreateAssemblyName(assemblyString, forIntrospection, out runtimeAssembly);
			if (runtimeAssembly != null)
			{
				return runtimeAssembly;
			}
			return RuntimeAssembly.InternalLoadAssemblyName(assemblyRef, assemblySecurity, null, ref stackMark, pPrivHostBinder, true, forIntrospection, false);
		}

		[SecurityCritical]
		internal static AssemblyName CreateAssemblyName(string assemblyString, bool forIntrospection, out RuntimeAssembly assemblyFromResolveEvent)
		{
			if (assemblyString == null)
			{
				throw new ArgumentNullException("assemblyString");
			}
			if (assemblyString.Length == 0 || assemblyString[0] == '\0')
			{
				throw new ArgumentException(Environment.GetResourceString("Format_StringZeroLength"));
			}
			if (forIntrospection)
			{
				AppDomain.CheckReflectionOnlyLoadSupported();
			}
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = assemblyString;
			assemblyName.nInit(out assemblyFromResolveEvent, forIntrospection, true);
			return assemblyName;
		}

		[SecurityCritical]
		internal static RuntimeAssembly InternalLoadAssemblyName(AssemblyName assemblyRef, Evidence assemblySecurity, RuntimeAssembly reqAssembly, ref StackCrawlMark stackMark, bool throwOnFileNotFound, bool forIntrospection, bool suppressSecurityChecks)
		{
			return RuntimeAssembly.InternalLoadAssemblyName(assemblyRef, assemblySecurity, reqAssembly, ref stackMark, IntPtr.Zero, true, forIntrospection, suppressSecurityChecks);
		}

		[SecurityCritical]
		internal static RuntimeAssembly InternalLoadAssemblyName(AssemblyName assemblyRef, Evidence assemblySecurity, RuntimeAssembly reqAssembly, ref StackCrawlMark stackMark, IntPtr pPrivHostBinder, bool throwOnFileNotFound, bool forIntrospection, bool suppressSecurityChecks)
		{
			if (assemblyRef == null)
			{
				throw new ArgumentNullException("assemblyRef");
			}
			if (assemblyRef.CodeBase != null)
			{
				AppDomain.CheckLoadFromSupported();
			}
			assemblyRef = (AssemblyName)assemblyRef.Clone();
			if (assemblySecurity != null)
			{
				if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
				{
					throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyImplicit"));
				}
				if (!suppressSecurityChecks)
				{
					new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
				}
			}
			string text = RuntimeAssembly.VerifyCodeBase(assemblyRef.CodeBase);
			if (text != null && !suppressSecurityChecks)
			{
				if (string.Compare(text, 0, "file:", 0, 5, StringComparison.OrdinalIgnoreCase) != 0)
				{
					IPermission permission = RuntimeAssembly.CreateWebPermission(assemblyRef.EscapedCodeBase);
					permission.Demand();
				}
				else
				{
					URLString urlstring = new URLString(text, true);
					new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, urlstring.GetFileName()).Demand();
				}
			}
			return RuntimeAssembly.nLoad(assemblyRef, text, assemblySecurity, reqAssembly, ref stackMark, pPrivHostBinder, throwOnFileNotFound, forIntrospection, suppressSecurityChecks);
		}

		[SecuritySafeCritical]
		internal bool IsFrameworkAssembly()
		{
			RuntimeAssembly.ASSEMBLY_FLAGS flags = this.Flags;
			return (flags & RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_FRAMEWORK) > RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_UNKNOWN;
		}

		internal bool IsSafeForReflection()
		{
			RuntimeAssembly.ASSEMBLY_FLAGS flags = this.Flags;
			return (flags & RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_SAFE_REFLECTION) > RuntimeAssembly.ASSEMBLY_FLAGS.ASSEMBLY_FLAGS_UNKNOWN;
		}

		[SecuritySafeCritical]
		private bool IsDesignerBindingContext()
		{
			return RuntimeAssembly.nIsDesignerBindingContext(this);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern bool nIsDesignerBindingContext(RuntimeAssembly assembly);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RuntimeAssembly _nLoad(AssemblyName fileName, string codeBase, Evidence assemblySecurity, RuntimeAssembly locationHint, ref StackCrawlMark stackMark, IntPtr pPrivHostBinder, bool throwOnFileNotFound, bool forIntrospection, bool suppressSecurityChecks);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsFrameworkAssembly(AssemblyName assemblyName);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsNewPortableAssembly(AssemblyName assemblyName);

		[SecurityCritical]
		private static RuntimeAssembly nLoad(AssemblyName fileName, string codeBase, Evidence assemblySecurity, RuntimeAssembly locationHint, ref StackCrawlMark stackMark, IntPtr pPrivHostBinder, bool throwOnFileNotFound, bool forIntrospection, bool suppressSecurityChecks)
		{
			return RuntimeAssembly._nLoad(fileName, codeBase, assemblySecurity, locationHint, ref stackMark, pPrivHostBinder, throwOnFileNotFound, forIntrospection, suppressSecurityChecks);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static RuntimeAssembly LoadWithPartialNameHack(string partialName, bool cropPublicKey)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			AssemblyName assemblyName = new AssemblyName(partialName);
			if (!RuntimeAssembly.IsSimplyNamed(assemblyName))
			{
				if (cropPublicKey)
				{
					assemblyName.SetPublicKey(null);
					assemblyName.SetPublicKeyToken(null);
				}
				if (RuntimeAssembly.IsFrameworkAssembly(assemblyName) || !AppDomain.IsAppXModel())
				{
					AssemblyName assemblyName2 = RuntimeAssembly.EnumerateCache(assemblyName);
					if (assemblyName2 != null)
					{
						return RuntimeAssembly.InternalLoadAssemblyName(assemblyName2, null, null, ref stackCrawlMark, true, false, false);
					}
					return null;
				}
			}
			if (AppDomain.IsAppXModel())
			{
				assemblyName.Version = null;
				return RuntimeAssembly.nLoad(assemblyName, null, null, null, ref stackCrawlMark, IntPtr.Zero, false, false, false);
			}
			return null;
		}

		[SecurityCritical]
		internal static RuntimeAssembly LoadWithPartialNameInternal(string partialName, Evidence securityEvidence, ref StackCrawlMark stackMark)
		{
			AssemblyName an = new AssemblyName(partialName);
			return RuntimeAssembly.LoadWithPartialNameInternal(an, securityEvidence, ref stackMark);
		}

		[SecurityCritical]
		internal static RuntimeAssembly LoadWithPartialNameInternal(AssemblyName an, Evidence securityEvidence, ref StackCrawlMark stackMark)
		{
			if (securityEvidence != null)
			{
				if (!AppDomain.CurrentDomain.IsLegacyCasPolicyEnabled)
				{
					throw new NotSupportedException(Environment.GetResourceString("NotSupported_RequiresCasPolicyImplicit"));
				}
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
			}
			AppDomain.CheckLoadWithPartialNameSupported(stackMark);
			RuntimeAssembly result = null;
			try
			{
				result = RuntimeAssembly.nLoad(an, null, securityEvidence, null, ref stackMark, IntPtr.Zero, true, false, false);
			}
			catch (Exception ex)
			{
				if (ex.IsTransient)
				{
					throw ex;
				}
				if (RuntimeAssembly.IsUserError(ex))
				{
					throw;
				}
				if (RuntimeAssembly.IsFrameworkAssembly(an) || !AppDomain.IsAppXModel())
				{
					if (RuntimeAssembly.IsSimplyNamed(an))
					{
						return null;
					}
					AssemblyName assemblyName = RuntimeAssembly.EnumerateCache(an);
					if (assemblyName != null)
					{
						result = RuntimeAssembly.InternalLoadAssemblyName(assemblyName, securityEvidence, null, ref stackMark, true, false, false);
					}
				}
				else
				{
					an.Version = null;
					result = RuntimeAssembly.nLoad(an, null, securityEvidence, null, ref stackMark, IntPtr.Zero, false, false, false);
				}
			}
			return result;
		}

		[SecuritySafeCritical]
		private static bool IsUserError(Exception e)
		{
			return e.HResult == -2146234280;
		}

		private static bool IsSimplyNamed(AssemblyName partialName)
		{
			byte[] array = partialName.GetPublicKeyToken();
			if (array != null && array.Length == 0)
			{
				return true;
			}
			array = partialName.GetPublicKey();
			return array != null && array.Length == 0;
		}

		[SecurityCritical]
		private static AssemblyName EnumerateCache(AssemblyName partialName)
		{
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
			partialName.Version = null;
			ArrayList arrayList = new ArrayList();
			Fusion.ReadCache(arrayList, partialName.FullName, 2U);
			IEnumerator enumerator = arrayList.GetEnumerator();
			AssemblyName assemblyName = null;
			CultureInfo cultureInfo = partialName.CultureInfo;
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				AssemblyName assemblyName2 = new AssemblyName((string)obj);
				if (RuntimeAssembly.CulturesEqual(cultureInfo, assemblyName2.CultureInfo))
				{
					if (assemblyName == null)
					{
						assemblyName = assemblyName2;
					}
					else if (assemblyName2.Version > assemblyName.Version)
					{
						assemblyName = assemblyName2;
					}
				}
			}
			return assemblyName;
		}

		private static bool CulturesEqual(CultureInfo refCI, CultureInfo defCI)
		{
			bool flag = defCI.Equals(CultureInfo.InvariantCulture);
			if (refCI == null || refCI.Equals(CultureInfo.InvariantCulture))
			{
				return flag;
			}
			return !flag && defCI.Equals(refCI);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsReflectionOnly(RuntimeAssembly assembly);

		[ComVisible(false)]
		public override bool ReflectionOnly
		{
			[SecuritySafeCritical]
			get
			{
				return RuntimeAssembly.IsReflectionOnly(this.GetNativeHandle());
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void LoadModule(RuntimeAssembly assembly, string moduleName, byte[] rawModule, int cbModule, byte[] rawSymbolStore, int cbSymbolStore, ObjectHandleOnStack retModule);

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, ControlEvidence = true)]
		public override Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
		{
			RuntimeModule result = null;
			RuntimeAssembly.LoadModule(this.GetNativeHandle(), moduleName, rawModule, (rawModule != null) ? rawModule.Length : 0, rawSymbolStore, (rawSymbolStore != null) ? rawSymbolStore.Length : 0, JitHelpers.GetObjectHandleOnStack<RuntimeModule>(ref result));
			return result;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetModule(RuntimeAssembly assembly, string name, ObjectHandleOnStack retModule);

		[SecuritySafeCritical]
		public override Module GetModule(string name)
		{
			Module result = null;
			RuntimeAssembly.GetModule(this.GetNativeHandle(), name, JitHelpers.GetObjectHandleOnStack<Module>(ref result));
			return result;
		}

		[SecuritySafeCritical]
		public override FileStream GetFile(string name)
		{
			RuntimeModule runtimeModule = (RuntimeModule)this.GetModule(name);
			if (runtimeModule == null)
			{
				return null;
			}
			return new FileStream(runtimeModule.GetFullyQualifiedName(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false);
		}

		[SecuritySafeCritical]
		public override FileStream[] GetFiles(bool getResourceModules)
		{
			Module[] modules = this.GetModules(getResourceModules);
			int num = modules.Length;
			FileStream[] array = new FileStream[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new FileStream(((RuntimeModule)modules[i]).GetFullyQualifiedName(), FileMode.Open, FileAccess.Read, FileShare.Read, 4096, false);
			}
			return array;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetManifestResourceNames(RuntimeAssembly assembly);

		[SecuritySafeCritical]
		public override string[] GetManifestResourceNames()
		{
			return RuntimeAssembly.GetManifestResourceNames(this.GetNativeHandle());
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetExecutingAssembly(StackCrawlMarkHandle stackMark, ObjectHandleOnStack retAssembly);

		[SecurityCritical]
		internal static RuntimeAssembly GetExecutingAssembly(ref StackCrawlMark stackMark)
		{
			RuntimeAssembly result = null;
			RuntimeAssembly.GetExecutingAssembly(JitHelpers.GetStackCrawlMarkHandle(ref stackMark), JitHelpers.GetObjectHandleOnStack<RuntimeAssembly>(ref result));
			return result;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AssemblyName[] GetReferencedAssemblies(RuntimeAssembly assembly);

		[SecuritySafeCritical]
		public override AssemblyName[] GetReferencedAssemblies()
		{
			return RuntimeAssembly.GetReferencedAssemblies(this.GetNativeHandle());
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern int GetManifestResourceInfo(RuntimeAssembly assembly, string resourceName, ObjectHandleOnStack assemblyRef, StringHandleOnStack retFileName, StackCrawlMarkHandle stackMark);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
		{
			RuntimeAssembly containingAssembly = null;
			string containingFileName = null;
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			int manifestResourceInfo = RuntimeAssembly.GetManifestResourceInfo(this.GetNativeHandle(), resourceName, JitHelpers.GetObjectHandleOnStack<RuntimeAssembly>(ref containingAssembly), JitHelpers.GetStringHandleOnStack(ref containingFileName), JitHelpers.GetStackCrawlMarkHandle(ref stackCrawlMark));
			if (manifestResourceInfo == -1)
			{
				return null;
			}
			return new ManifestResourceInfo(containingAssembly, containingFileName, (ResourceLocation)manifestResourceInfo);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetLocation(RuntimeAssembly assembly, StringHandleOnStack retString);

		public override string Location
		{
			[SecuritySafeCritical]
			get
			{
				string text = null;
				RuntimeAssembly.GetLocation(this.GetNativeHandle(), JitHelpers.GetStringHandleOnStack(ref text));
				if (text != null)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, text).Demand();
				}
				return text;
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetImageRuntimeVersion(RuntimeAssembly assembly, StringHandleOnStack retString);

		[ComVisible(false)]
		public override string ImageRuntimeVersion
		{
			[SecuritySafeCritical]
			get
			{
				string result = null;
				RuntimeAssembly.GetImageRuntimeVersion(this.GetNativeHandle(), JitHelpers.GetStringHandleOnStack(ref result));
				return result;
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsGlobalAssemblyCache(RuntimeAssembly assembly);

		public override bool GlobalAssemblyCache
		{
			[SecuritySafeCritical]
			get
			{
				return RuntimeAssembly.IsGlobalAssemblyCache(this.GetNativeHandle());
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern long GetHostContext(RuntimeAssembly assembly);

		public override long HostContext
		{
			[SecuritySafeCritical]
			get
			{
				return RuntimeAssembly.GetHostContext(this.GetNativeHandle());
			}
		}

		private static string VerifyCodeBase(string codebase)
		{
			if (codebase == null)
			{
				return null;
			}
			int length = codebase.Length;
			if (length == 0)
			{
				return null;
			}
			int num = codebase.IndexOf(':');
			if (num != -1 && num + 2 < length && (codebase[num + 1] == '/' || codebase[num + 1] == '\\') && (codebase[num + 2] == '/' || codebase[num + 2] == '\\'))
			{
				return codebase;
			}
			if (length > 2 && codebase[0] == '\\' && codebase[1] == '\\')
			{
				return "file://" + codebase;
			}
			return "file:///" + Path.GetFullPathInternal(codebase);
		}

		[SecurityCritical]
		internal Stream GetManifestResourceStream(Type type, string name, bool skipSecurityCheck, ref StackCrawlMark stackMark)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (type == null)
			{
				if (name == null)
				{
					throw new ArgumentNullException("type");
				}
			}
			else
			{
				string @namespace = type.Namespace;
				if (@namespace != null)
				{
					stringBuilder.Append(@namespace);
					if (name != null)
					{
						stringBuilder.Append(Type.Delimiter);
					}
				}
			}
			if (name != null)
			{
				stringBuilder.Append(name);
			}
			return this.GetManifestResourceStream(stringBuilder.ToString(), ref stackMark, skipSecurityCheck);
		}

		internal bool IsStrongNameVerified
		{
			[SecurityCritical]
			get
			{
				return RuntimeAssembly.GetIsStrongNameVerified(this.GetNativeHandle());
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern bool GetIsStrongNameVerified(RuntimeAssembly assembly);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private unsafe static extern byte* GetResource(RuntimeAssembly assembly, string resourceName, out ulong length, StackCrawlMarkHandle stackMark, bool skipSecurityCheck);

		[SecurityCritical]
		internal unsafe Stream GetManifestResourceStream(string name, ref StackCrawlMark stackMark, bool skipSecurityCheck)
		{
			ulong num = 0UL;
			byte* resource = RuntimeAssembly.GetResource(this.GetNativeHandle(), name, out num, JitHelpers.GetStackCrawlMarkHandle(ref stackMark), skipSecurityCheck);
			if (resource == null)
			{
				return null;
			}
			if (num > 9223372036854775807UL)
			{
				throw new NotImplementedException(Environment.GetResourceString("NotImplemented_ResourcesLongerThan2^63"));
			}
			return new UnmanagedMemoryStream(resource, (long)num, (long)num, FileAccess.Read, true);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetVersion(RuntimeAssembly assembly, out int majVer, out int minVer, out int buildNum, out int revNum);

		[SecurityCritical]
		internal Version GetVersion()
		{
			int major;
			int minor;
			int build;
			int revision;
			RuntimeAssembly.GetVersion(this.GetNativeHandle(), out major, out minor, out build, out revision);
			return new Version(major, minor, build, revision);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetLocale(RuntimeAssembly assembly, StringHandleOnStack retString);

		[SecurityCritical]
		internal CultureInfo GetLocale()
		{
			string text = null;
			RuntimeAssembly.GetLocale(this.GetNativeHandle(), JitHelpers.GetStringHandleOnStack(ref text));
			if (text == null)
			{
				return CultureInfo.InvariantCulture;
			}
			return new CultureInfo(text);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool FCallIsDynamic(RuntimeAssembly assembly);

		public override bool IsDynamic
		{
			[SecuritySafeCritical]
			get
			{
				return RuntimeAssembly.FCallIsDynamic(this.GetNativeHandle());
			}
		}

		[SecurityCritical]
		private void VerifyCodeBaseDiscovery(string codeBase)
		{
			if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				return;
			}
			if (codeBase != null && string.Compare(codeBase, 0, "file:", 0, 5, StringComparison.OrdinalIgnoreCase) == 0)
			{
				URLString urlstring = new URLString(codeBase, true);
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, urlstring.GetFileName()).Demand();
			}
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetSimpleName(RuntimeAssembly assembly, StringHandleOnStack retSimpleName);

		[SecuritySafeCritical]
		internal string GetSimpleName()
		{
			string result = null;
			RuntimeAssembly.GetSimpleName(this.GetNativeHandle(), JitHelpers.GetStringHandleOnStack(ref result));
			return result;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern AssemblyHashAlgorithm GetHashAlgorithm(RuntimeAssembly assembly);

		[SecurityCritical]
		private AssemblyHashAlgorithm GetHashAlgorithm()
		{
			return RuntimeAssembly.GetHashAlgorithm(this.GetNativeHandle());
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern AssemblyNameFlags GetFlags(RuntimeAssembly assembly);

		[SecurityCritical]
		private AssemblyNameFlags GetFlags()
		{
			return RuntimeAssembly.GetFlags(this.GetNativeHandle());
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetRawBytes(RuntimeAssembly assembly, ObjectHandleOnStack retRawBytes);

		[SecuritySafeCritical]
		internal byte[] GetRawBytes()
		{
			byte[] result = null;
			RuntimeAssembly.GetRawBytes(this.GetNativeHandle(), JitHelpers.GetObjectHandleOnStack<byte[]>(ref result));
			return result;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetPublicKey(RuntimeAssembly assembly, ObjectHandleOnStack retPublicKey);

		[SecurityCritical]
		internal byte[] GetPublicKey()
		{
			byte[] result = null;
			RuntimeAssembly.GetPublicKey(this.GetNativeHandle(), JitHelpers.GetObjectHandleOnStack<byte[]>(ref result));
			return result;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetGrantSet(RuntimeAssembly assembly, ObjectHandleOnStack granted, ObjectHandleOnStack denied);

		[SecurityCritical]
		internal void GetGrantSet(out PermissionSet newGrant, out PermissionSet newDenied)
		{
			PermissionSet permissionSet = null;
			PermissionSet permissionSet2 = null;
			RuntimeAssembly.GetGrantSet(this.GetNativeHandle(), JitHelpers.GetObjectHandleOnStack<PermissionSet>(ref permissionSet), JitHelpers.GetObjectHandleOnStack<PermissionSet>(ref permissionSet2));
			newGrant = permissionSet;
			newDenied = permissionSet2;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsAllSecurityCritical(RuntimeAssembly assembly);

		[SecuritySafeCritical]
		internal bool IsAllSecurityCritical()
		{
			return RuntimeAssembly.IsAllSecurityCritical(this.GetNativeHandle());
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsAllSecuritySafeCritical(RuntimeAssembly assembly);

		[SecuritySafeCritical]
		internal bool IsAllSecuritySafeCritical()
		{
			return RuntimeAssembly.IsAllSecuritySafeCritical(this.GetNativeHandle());
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsAllPublicAreaSecuritySafeCritical(RuntimeAssembly assembly);

		[SecuritySafeCritical]
		internal bool IsAllPublicAreaSecuritySafeCritical()
		{
			return RuntimeAssembly.IsAllPublicAreaSecuritySafeCritical(this.GetNativeHandle());
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsAllSecurityTransparent(RuntimeAssembly assembly);

		[SecuritySafeCritical]
		internal bool IsAllSecurityTransparent()
		{
			return RuntimeAssembly.IsAllSecurityTransparent(this.GetNativeHandle());
		}

		[SecurityCritical]
		private static void DemandPermission(string codeBase, bool havePath, int demandFlag)
		{
			FileIOPermissionAccess access = FileIOPermissionAccess.PathDiscovery;
			switch (demandFlag)
			{
			case 1:
				access = FileIOPermissionAccess.Read;
				break;
			case 2:
				access = (FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery);
				break;
			case 3:
			{
				IPermission permission = RuntimeAssembly.CreateWebPermission(AssemblyName.EscapeCodeBase(codeBase));
				permission.Demand();
				return;
			}
			}
			if (!havePath)
			{
				URLString urlstring = new URLString(codeBase, true);
				codeBase = urlstring.GetFileName();
			}
			codeBase = Path.GetFullPathInternal(codeBase);
			new FileIOPermission(access, codeBase).Demand();
		}

		private static IPermission CreateWebPermission(string codeBase)
		{
			Assembly assembly = Assembly.Load("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			Type type = assembly.GetType("System.Net.NetworkAccess", true);
			IPermission permission = null;
			if (type.IsEnum && type.IsVisible)
			{
				object[] array = new object[2];
				array[0] = (Enum)Enum.Parse(type, "Connect", true);
				if (array[0] != null)
				{
					array[1] = codeBase;
					type = assembly.GetType("System.Net.WebPermission", true);
					if (type.IsVisible)
					{
						permission = (IPermission)Activator.CreateInstance(type, array);
					}
				}
			}
			if (permission == null)
			{
				throw new InvalidOperationException();
			}
			return permission;
		}

		[SecurityCritical]
		private RuntimeModule OnModuleResolveEvent(string moduleName)
		{
			ModuleResolveEventHandler moduleResolve = this._ModuleResolve;
			if (moduleResolve == null)
			{
				return null;
			}
			Delegate[] invocationList = moduleResolve.GetInvocationList();
			int num = invocationList.Length;
			for (int i = 0; i < num; i++)
			{
				RuntimeModule runtimeModule = (RuntimeModule)((ModuleResolveEventHandler)invocationList[i])(this, new ResolveEventArgs(moduleName, this));
				if (runtimeModule != null)
				{
					return runtimeModule;
				}
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override Assembly GetSatelliteAssembly(CultureInfo culture)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return this.InternalGetSatelliteAssembly(culture, null, ref stackCrawlMark);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return this.InternalGetSatelliteAssembly(culture, version, ref stackCrawlMark);
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal Assembly InternalGetSatelliteAssembly(CultureInfo culture, Version version, ref StackCrawlMark stackMark)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			string name = this.GetSimpleName() + ".resources";
			return this.InternalGetSatelliteAssembly(name, culture, version, true, ref stackMark);
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UseRelativeBindForSatellites();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal RuntimeAssembly InternalGetSatelliteAssembly(string name, CultureInfo culture, Version version, bool throwOnFileNotFound, ref StackCrawlMark stackMark)
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.SetPublicKey(this.GetPublicKey());
			assemblyName.Flags = (this.GetFlags() | AssemblyNameFlags.PublicKey);
			if (version == null)
			{
				assemblyName.Version = this.GetVersion();
			}
			else
			{
				assemblyName.Version = version;
			}
			assemblyName.CultureInfo = culture;
			assemblyName.Name = name;
			RuntimeAssembly runtimeAssembly = null;
			bool flag = AppDomain.IsAppXDesignMode();
			bool flag2 = false;
			if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				flag2 = (this.IsFrameworkAssembly() || RuntimeAssembly.UseRelativeBindForSatellites());
			}
			if (flag || flag2)
			{
				if (this.GlobalAssemblyCache)
				{
					ArrayList arrayList = new ArrayList();
					bool flag3 = false;
					try
					{
						Fusion.ReadCache(arrayList, assemblyName.FullName, 2U);
					}
					catch (Exception ex)
					{
						if (ex.IsTransient)
						{
							throw;
						}
						if (!AppDomain.IsAppXModel())
						{
							flag3 = true;
						}
					}
					if (arrayList.Count > 0 || flag3)
					{
						runtimeAssembly = RuntimeAssembly.nLoad(assemblyName, null, null, this, ref stackMark, IntPtr.Zero, throwOnFileNotFound, false, false);
					}
				}
				else
				{
					string codeBase = this.CodeBase;
					if (codeBase != null && string.Compare(codeBase, 0, "file:", 0, 5, StringComparison.OrdinalIgnoreCase) == 0)
					{
						runtimeAssembly = this.InternalProbeForSatelliteAssemblyNextToParentAssembly(assemblyName, name, codeBase, culture, throwOnFileNotFound, flag, ref stackMark);
						if (runtimeAssembly != null && !RuntimeAssembly.IsSimplyNamed(assemblyName))
						{
							AssemblyName name2 = runtimeAssembly.GetName();
							if (!AssemblyName.ReferenceMatchesDefinitionInternal(assemblyName, name2, false))
							{
								runtimeAssembly = null;
							}
						}
					}
					else if (!flag)
					{
						runtimeAssembly = RuntimeAssembly.nLoad(assemblyName, null, null, this, ref stackMark, IntPtr.Zero, throwOnFileNotFound, false, false);
					}
				}
			}
			else
			{
				runtimeAssembly = RuntimeAssembly.nLoad(assemblyName, null, null, this, ref stackMark, IntPtr.Zero, throwOnFileNotFound, false, false);
			}
			if (runtimeAssembly == this || (runtimeAssembly == null && throwOnFileNotFound))
			{
				throw new FileNotFoundException(string.Format(culture, Environment.GetResourceString("IO.FileNotFound_FileName"), assemblyName.Name));
			}
			return runtimeAssembly;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private RuntimeAssembly InternalProbeForSatelliteAssemblyNextToParentAssembly(AssemblyName an, string name, string codeBase, CultureInfo culture, bool throwOnFileNotFound, bool useLoadFile, ref StackCrawlMark stackMark)
		{
			RuntimeAssembly runtimeAssembly = null;
			string text = null;
			if (useLoadFile)
			{
				text = this.Location;
			}
			FileNotFoundException ex = null;
			StringBuilder stringBuilder = new StringBuilder(useLoadFile ? text : codeBase, 0, useLoadFile ? (text.LastIndexOf('\\') + 1) : (codeBase.LastIndexOf('/') + 1), 260);
			stringBuilder.Append(an.CultureInfo.Name);
			stringBuilder.Append(useLoadFile ? '\\' : '/');
			stringBuilder.Append(name);
			stringBuilder.Append(".DLL");
			string text2 = stringBuilder.ToString();
			AssemblyName assemblyName = null;
			if (!useLoadFile)
			{
				assemblyName = new AssemblyName();
				assemblyName.CodeBase = text2;
			}
			try
			{
				try
				{
					runtimeAssembly = (useLoadFile ? RuntimeAssembly.nLoadFile(text2, null) : RuntimeAssembly.nLoad(assemblyName, text2, null, this, ref stackMark, IntPtr.Zero, throwOnFileNotFound, false, false));
				}
				catch (FileNotFoundException)
				{
					ex = new FileNotFoundException(string.Format(culture, Environment.GetResourceString("IO.FileNotFound_FileName"), text2), text2);
					runtimeAssembly = null;
				}
				if (runtimeAssembly == null)
				{
					stringBuilder.Remove(stringBuilder.Length - 4, 4);
					stringBuilder.Append(".EXE");
					text2 = stringBuilder.ToString();
					if (!useLoadFile)
					{
						assemblyName.CodeBase = text2;
					}
					try
					{
						runtimeAssembly = (useLoadFile ? RuntimeAssembly.nLoadFile(text2, null) : RuntimeAssembly.nLoad(assemblyName, text2, null, this, ref stackMark, IntPtr.Zero, false, false, false));
					}
					catch (FileNotFoundException)
					{
						runtimeAssembly = null;
					}
					if (runtimeAssembly == null && throwOnFileNotFound)
					{
						throw ex;
					}
				}
			}
			catch (DirectoryNotFoundException)
			{
				if (throwOnFileNotFound)
				{
					throw;
				}
				runtimeAssembly = null;
			}
			return runtimeAssembly;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern RuntimeAssembly nLoadFile(string path, Evidence evidence);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern RuntimeAssembly nLoadImage(byte[] rawAssembly, byte[] rawSymbolStore, Evidence evidence, ref StackCrawlMark stackMark, bool fIntrospection, bool fSkipIntegrityCheck, SecurityContextSource securityContextSource);

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetModules(RuntimeAssembly assembly, bool loadIfNotFound, bool getResourceModules, ObjectHandleOnStack retModuleHandles);

		[SecuritySafeCritical]
		private RuntimeModule[] GetModulesInternal(bool loadIfNotFound, bool getResourceModules)
		{
			RuntimeModule[] result = null;
			RuntimeAssembly.GetModules(this.GetNativeHandle(), loadIfNotFound, getResourceModules, JitHelpers.GetObjectHandleOnStack<RuntimeModule[]>(ref result));
			return result;
		}

		public override Module[] GetModules(bool getResourceModules)
		{
			return this.GetModulesInternal(true, getResourceModules);
		}

		public override Module[] GetLoadedModules(bool getResourceModules)
		{
			return this.GetModulesInternal(false, getResourceModules);
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern RuntimeModule GetManifestModule(RuntimeAssembly assembly);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool AptcaCheck(RuntimeAssembly targetAssembly, RuntimeAssembly sourceAssembly);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetToken(RuntimeAssembly assembly);

		private const uint COR_E_LOADING_REFERENCE_ASSEMBLY = 2148733016U;

		private string m_fullname;

		private object m_syncRoot;

		private IntPtr m_assembly;

		private RuntimeAssembly.ASSEMBLY_FLAGS m_flags;

		private const string s_localFilePrefix = "file:";

		private static string[] s_unsafeFrameworkAssemblyNames = new string[]
		{
			"System.Reflection.Context",
			"Microsoft.VisualBasic"
		};

		private enum ASSEMBLY_FLAGS : uint
		{
			ASSEMBLY_FLAGS_UNKNOWN,
			ASSEMBLY_FLAGS_INITIALIZED = 16777216U,
			ASSEMBLY_FLAGS_FRAMEWORK = 33554432U,
			ASSEMBLY_FLAGS_SAFE_REFLECTION = 67108864U,
			ASSEMBLY_FLAGS_TOKEN_MASK = 16777215U
		}
	}
}
