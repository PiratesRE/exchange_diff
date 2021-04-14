using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using <CppImplementationDetails>;
using <CrtImplementationDetails>;

internal class <Module>
{
	internal unsafe static ushort* GetUnmanagedUni(string pstr)
	{
		return (ushort*)Marshal.StringToHGlobalUni(pstr).ToPointer();
	}

	internal unsafe static void FreeUnmanagedUni(ushort* sz)
	{
		if (sz != null)
		{
			IntPtr hglobal = new IntPtr((void*)sz);
			Marshal.FreeHGlobal(hglobal);
		}
	}

	internal unsafe static byte[] MakeManagedBytes(byte* _pb, int _cb)
	{
		byte[] array = new byte[_cb];
		if (_cb > 0)
		{
			Marshal.Copy((IntPtr)((void*)_pb), array, 0, _cb);
		}
		return array;
	}

	internal static int GetEC(int hr)
	{
		if (-939585531 == hr || -939585532 == hr)
		{
			return Marshal.GetLastWin32Error();
		}
		if (-939587625 == hr)
		{
			return 10;
		}
		if (-939647163 == hr)
		{
			return 11;
		}
		if (21 == hr)
		{
			return 17;
		}
		return (-939587619 == hr) ? 18 : hr;
	}

	[SuppressUnmanagedCodeSecurity]
	[DllImport("Microsoft.Exchange.Cluster.ReplicaSeeder.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public static extern void CleanupLogger();

	[SuppressUnmanagedCodeSecurity]
	[DllImport("Microsoft.Exchange.Cluster.ReplicaSeeder.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public static extern void SetupLogger();

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESESeedForceNewLog(void* hccxBackupContext);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESESeedGetDatabaseInfo(void* hccxBackupContext, ushort* wszDatabase, void* pvBuffer, uint cbBuffer, uint* pcbActual, uint fFlags);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESESeedReadPages(void* hccxBackupContext, ushort* wszDatabase, uint pgnoStart, uint cpg, void* pvBuffer, uint cbBuffer, uint* pcbRead, uint fFlags, uint* pulGenLow, uint* pulGenHigh);

	[SuppressUnmanagedCodeSecurity]
	[DllImport("Microsoft.Exchange.Cluster.ReplicaSeeder.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int EcDoBackupReadFileEx(void* hccxBackupContext, void* hSourceFile, long cbSourceFileOffset, byte* pbOutputBuffer, uint cbSourceFileToRead, uint* pcbSourceFileActuallyRead);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESEBackupCloseFile(void* hccxBackupContext, void* hFile);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESEBackupOpenFile(void* hccxBackupContext, ushort* wszFileName, uint cbReadHintSize, uint cSections, void** rghFile, long* rgliSectionSize);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern void ESEBackupRestoreFreeRegisteredInfo(uint cRegisteredInfo, _ESE_REGISTERED_INFO* aRegisteredInfo);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESEBackupEnd(void* hccxBackupContext);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESEBackupInstanceEnd(void* hccxBackupContext, uint fFlags);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern void ESEBackupFreeInstanceInfo(uint cInstanceInfo, _INSTANCE_BACKUP_INFO* aInstanceInfo);

	[SuppressUnmanagedCodeSecurity]
	[DllImport("Microsoft.Exchange.Cluster.ReplicaSeeder.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int EcDoHrESEBackupSetup(void* hccxBackupContext, long hInstanceId, uint btBackupType, ushort* wszTransferAddrs);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESESeedPrepare(ushort* wszBackupServer, ushort* wszBackupAnnotation, uint fFlags, uint ulTimeoutInMsec, uint* pcInstanceInfo, _INSTANCE_BACKUP_INFO** paInstanceInfo, void** phccxBackupContext);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESEBackupRestoreGetRegisteredEx(ushort* wszServerName, ushort* wszDisplayName, uint fFlags, uint ulTimeoutInMsec, uint* pcRegisteredInfo, _ESE_REGISTERED_INFO** paRegisteredInfo);

	internal unsafe static ushort* GetUnmanagedUni(string pstr)
	{
		return (ushort*)Marshal.StringToHGlobalUni(pstr).ToPointer();
	}

	internal unsafe static void FreeUnmanagedUni(ushort* sz)
	{
		IntPtr hglobal = new IntPtr((void*)sz);
		Marshal.FreeHGlobal(hglobal);
	}

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESELogShipOpenEx(ushort* wszLogShipServer, ushort* wszLogShipAnnotation, ushort* wszSGGuid, ushort* wszClientId, ushort* wszSGBaseName, ushort* wszSGLogFilePath, int fCircularLogging, uint ulFlags, uint ulMsecTimeout, void** phccxLogShipContext);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESELogShipClose(void* hccxLogShipContext);

	[DllImport("esebcli2.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	public unsafe static extern int HrESELogShipSuccessful(void* hccxLogShipContext, int lgenReplayed, int* plgenTruncated, uint grfFlags);

	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool <CrtImplementationDetails>.NativeDll.IsInDllMain()
	{
		return (<Module>.__native_dllmain_reason != uint.MaxValue) ? 1 : 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool <CrtImplementationDetails>.NativeDll.IsInProcessAttach()
	{
		return (<Module>.__native_dllmain_reason == 1U) ? 1 : 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool <CrtImplementationDetails>.NativeDll.IsInProcessDetach()
	{
		return (<Module>.__native_dllmain_reason == 0U) ? 1 : 0;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool <CrtImplementationDetails>.NativeDll.IsSafeForManagedCode()
	{
		if (((<Module>.__native_dllmain_reason != 4294967295U) ? 1 : 0) == 0)
		{
			return 1;
		}
		int num;
		if (((<Module>.__native_dllmain_reason == 1U) ? 1 : 0) == 0 && ((<Module>.__native_dllmain_reason == 0U) ? 1 : 0) == 0)
		{
			num = 1;
		}
		else
		{
			num = 0;
		}
		return (byte)num;
	}

	internal static void <CrtImplementationDetails>.ThrowNestedModuleLoadException(System.Exception innerException, System.Exception nestedException)
	{
		throw new ModuleLoadExceptionHandlerException("A nested exception occurred after the primary exception that caused the C++ module to fail to load.\n", innerException, nestedException);
	}

	internal static void <CrtImplementationDetails>.ThrowModuleLoadException(string errorMessage)
	{
		throw new ModuleLoadException(errorMessage);
	}

	internal static void <CrtImplementationDetails>.ThrowModuleLoadException(string errorMessage, System.Exception innerException)
	{
		throw new ModuleLoadException(errorMessage, innerException);
	}

	internal static void <CrtImplementationDetails>.RegisterModuleUninitializer(EventHandler handler)
	{
		ModuleUninitializer._ModuleUninitializer.AddHandler(handler);
	}

	[SecuritySafeCritical]
	internal unsafe static Guid <CrtImplementationDetails>.FromGUID(_GUID* guid)
	{
		Guid result = new Guid((uint)(*guid), *(guid + 4L), *(guid + 6L), *(guid + 8L), *(guid + 9L), *(guid + 10L), *(guid + 11L), *(guid + 12L), *(guid + 13L), *(guid + 14L), *(guid + 15L));
		return result;
	}

	[SecurityCritical]
	internal unsafe static int __get_default_appdomain(IUnknown** ppUnk)
	{
		ICorRuntimeHost* ptr = null;
		int num;
		try
		{
			Guid riid = <Module>.<CrtImplementationDetails>.FromGUID(ref <Module>._GUID_cb2f6722_ab3a_11d2_9c40_00c04fa30a3e);
			Guid clsid = <Module>.<CrtImplementationDetails>.FromGUID(ref <Module>._GUID_cb2f6723_ab3a_11d2_9c40_00c04fa30a3e);
			ptr = (ICorRuntimeHost*)RuntimeEnvironment.GetRuntimeInterfaceAsIntPtr(clsid, riid).ToPointer();
			goto IL_3E;
		}
		catch (System.Exception e)
		{
			num = Marshal.GetHRForException(e);
		}
		if (num < 0)
		{
			return num;
		}
		IL_3E:
		num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,IUnknown**), ptr, ppUnk, *(*(long*)ptr + 104L));
		ICorRuntimeHost* ptr2 = ptr;
		object obj = calli(System.UInt32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr), ptr2, *(*(long*)ptr2 + 16L));
		return num;
	}

	internal unsafe static void __release_appdomain(IUnknown* ppUnk)
	{
		object obj = calli(System.UInt32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr), ppUnk, *(*(long*)ppUnk + 16L));
	}

	[SecurityCritical]
	internal unsafe static AppDomain <CrtImplementationDetails>.GetDefaultDomain()
	{
		IUnknown* ptr = null;
		int num = <Module>.__get_default_appdomain(&ptr);
		if (num >= 0)
		{
			try
			{
				IntPtr pUnk = new IntPtr((void*)ptr);
				return (AppDomain)Marshal.GetObjectForIUnknown(pUnk);
			}
			finally
			{
				<Module>.__release_appdomain(ptr);
			}
		}
		Marshal.ThrowExceptionForHR(num);
		return null;
	}

	[SecurityCritical]
	internal unsafe static void <CrtImplementationDetails>.DoCallBackInDefaultDomain(method function, void* cookie)
	{
		Guid riid = <Module>.<CrtImplementationDetails>.FromGUID(ref <Module>._GUID_90f1a06c_7712_4762_86b5_7a5eba6bdb02);
		ICLRRuntimeHost* ptr = (ICLRRuntimeHost*)RuntimeEnvironment.GetRuntimeInterfaceAsIntPtr(<Module>.<CrtImplementationDetails>.FromGUID(ref <Module>._GUID_90f1a06e_7712_4762_86b5_7a5eba6bdb02), riid).ToPointer();
		try
		{
			AppDomain appDomain = <Module>.<CrtImplementationDetails>.GetDefaultDomain();
			int num = calli(System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr,System.UInt32 modopt(System.Runtime.CompilerServices.IsLong),System.Int32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl) (System.Void*),System.Void*), ptr, appDomain.Id, function, cookie, *(*(long*)ptr + 64L));
			if (num < 0)
			{
				Marshal.ThrowExceptionForHR(num);
			}
		}
		finally
		{
			ICLRRuntimeHost* ptr2 = ptr;
			object obj = calli(System.UInt32 modopt(System.Runtime.CompilerServices.IsLong) modopt(System.Runtime.CompilerServices.CallConvCdecl)(System.IntPtr), ptr2, *(*(long*)ptr2 + 16L));
		}
	}

	[SecuritySafeCritical]
	internal unsafe static int <CrtImplementationDetails>.DefaultDomain.DoNothing(void* cookie)
	{
		GC.KeepAlive(int.MaxValue);
		return 0;
	}

	[SecuritySafeCritical]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool <CrtImplementationDetails>.DefaultDomain.HasPerProcess()
	{
		if (<Module>.?hasPerProcess@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A == (TriBool.State)2)
		{
			void** ptr = (void**)(&<Module>.?A0xc6f23461.__xc_mp_a);
			if (ref <Module>.?A0xc6f23461.__xc_mp_a < ref <Module>.?A0xc6f23461.__xc_mp_z)
			{
				while (*(long*)ptr == 0L)
				{
					ptr += 8L / (long)sizeof(void*);
					if (ptr >= (void**)(&<Module>.?A0xc6f23461.__xc_mp_z))
					{
						goto IL_35;
					}
				}
				<Module>.?hasPerProcess@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A = (TriBool.State)(-1);
				return 1;
			}
			IL_35:
			<Module>.?hasPerProcess@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A = (TriBool.State)0;
			return 0;
		}
		return (<Module>.?hasPerProcess@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A == (TriBool.State)(-1)) ? 1 : 0;
	}

	[SecuritySafeCritical]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static bool <CrtImplementationDetails>.DefaultDomain.HasNative()
	{
		if (<Module>.?hasNative@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A == (TriBool.State)2)
		{
			void** ptr = (void**)(&<Module>.__xi_a);
			if (ref <Module>.__xi_a < ref <Module>.__xi_z)
			{
				while (*(long*)ptr == 0L)
				{
					ptr += 8L / (long)sizeof(void*);
					if (ptr >= (void**)(&<Module>.__xi_z))
					{
						goto IL_35;
					}
				}
				<Module>.?hasNative@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A = (TriBool.State)(-1);
				return 1;
			}
			IL_35:
			void** ptr2 = (void**)(&<Module>.__xc_a);
			if (ref <Module>.__xc_a < ref <Module>.__xc_z)
			{
				while (*(long*)ptr2 == 0L)
				{
					ptr2 += 8L / (long)sizeof(void*);
					if (ptr2 >= (void**)(&<Module>.__xc_z))
					{
						goto IL_62;
					}
				}
				<Module>.?hasNative@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A = (TriBool.State)(-1);
				return 1;
			}
			IL_62:
			<Module>.?hasNative@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A = (TriBool.State)0;
			return 0;
		}
		return (<Module>.?hasNative@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A == (TriBool.State)(-1)) ? 1 : 0;
	}

	[SecuritySafeCritical]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool <CrtImplementationDetails>.DefaultDomain.NeedsInitialization()
	{
		int num;
		if ((<Module>.<CrtImplementationDetails>.DefaultDomain.HasPerProcess() != null && !<Module>.?InitializedPerProcess@DefaultDomain@<CrtImplementationDetails>@@2_NA) || (<Module>.<CrtImplementationDetails>.DefaultDomain.HasNative() != null && !<Module>.?InitializedNative@DefaultDomain@<CrtImplementationDetails>@@2_NA && <Module>.__native_startup_state == (__enative_startup_state)0))
		{
			num = 1;
		}
		else
		{
			num = 0;
		}
		return (byte)num;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool <CrtImplementationDetails>.DefaultDomain.NeedsUninitialization()
	{
		return <Module>.?Entered@DefaultDomain@<CrtImplementationDetails>@@2_NA;
	}

	[SecurityCritical]
	internal static void <CrtImplementationDetails>.DefaultDomain.Initialize()
	{
		<Module>.<CrtImplementationDetails>.DoCallBackInDefaultDomain(<Module>.__unep@?DoNothing@DefaultDomain@<CrtImplementationDetails>@@$$FCAJPEAX@Z, null);
	}

	internal static void ??__E?Initialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA@@YMXXZ()
	{
		<Module>.?Initialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA = 0;
	}

	internal static void ??__E?Uninitialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA@@YMXXZ()
	{
		<Module>.?Uninitialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA = 0;
	}

	internal static void ??__E?IsDefaultDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2_NA@@YMXXZ()
	{
		<Module>.?IsDefaultDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2_NA = false;
	}

	internal static void ??__E?InitializedVtables@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A@@YMXXZ()
	{
		<Module>.?InitializedVtables@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)0;
	}

	internal static void ??__E?InitializedNative@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A@@YMXXZ()
	{
		<Module>.?InitializedNative@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)0;
	}

	internal static void ??__E?InitializedPerProcess@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A@@YMXXZ()
	{
		<Module>.?InitializedPerProcess@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)0;
	}

	internal static void ??__E?InitializedPerAppDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A@@YMXXZ()
	{
		<Module>.?InitializedPerAppDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)0;
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport.InitializeVtables(LanguageSupport* A_0)
	{
		<Module>.gcroot<System::String\u0020^>.=(A_0, "The C++ module failed to load during vtable initialization.\n");
		<Module>.?InitializedVtables@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)1;
		<Module>._initterm_m((method*)(&<Module>.?A0xc6f23461.__xi_vt_a), (method*)(&<Module>.?A0xc6f23461.__xi_vt_z));
		<Module>.?InitializedVtables@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)2;
	}

	[SecurityCritical]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport.InitializeDefaultAppDomain(LanguageSupport* A_0)
	{
		<Module>.gcroot<System::String\u0020^>.=(A_0, "The C++ module failed to load while attempting to initialize the default appdomain.\n");
		<Module>.<CrtImplementationDetails>.DefaultDomain.Initialize();
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport.InitializeNative(LanguageSupport* A_0)
	{
		<Module>.gcroot<System::String\u0020^>.=(A_0, "The C++ module failed to load during native initialization.\n");
		<Module>.__security_init_cookie();
		<Module>.?InitializedNative@DefaultDomain@<CrtImplementationDetails>@@2_NA = true;
		if (<Module>.<CrtImplementationDetails>.NativeDll.IsSafeForManagedCode() == null)
		{
			<Module>._amsg_exit(33);
		}
		if (<Module>.__native_startup_state == (__enative_startup_state)1)
		{
			<Module>._amsg_exit(33);
		}
		else if (<Module>.__native_startup_state == (__enative_startup_state)0)
		{
			<Module>.?InitializedNative@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)1;
			<Module>.__native_startup_state = (__enative_startup_state)1;
			if (<Module>._initterm_e((method*)(&<Module>.__xi_a), (method*)(&<Module>.__xi_z)) != 0)
			{
				<Module>.<CrtImplementationDetails>.ThrowModuleLoadException(<Module>.gcroot<System::String\u0020^>..PE$AAVString@System@@(A_0));
			}
			<Module>._initterm((method*)(&<Module>.__xc_a), (method*)(&<Module>.__xc_z));
			<Module>.__native_startup_state = (__enative_startup_state)2;
			<Module>.?InitializedNativeFromCCTOR@DefaultDomain@<CrtImplementationDetails>@@2_NA = true;
			<Module>.?InitializedNative@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)2;
		}
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport.InitializePerProcess(LanguageSupport* A_0)
	{
		<Module>.gcroot<System::String\u0020^>.=(A_0, "The C++ module failed to load during process initialization.\n");
		<Module>.?InitializedPerProcess@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)1;
		<Module>._initatexit_m();
		<Module>._initterm_m((method*)(&<Module>.?A0xc6f23461.__xc_mp_a), (method*)(&<Module>.?A0xc6f23461.__xc_mp_z));
		<Module>.?InitializedPerProcess@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)2;
		<Module>.?InitializedPerProcess@DefaultDomain@<CrtImplementationDetails>@@2_NA = true;
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport.InitializePerAppDomain(LanguageSupport* A_0)
	{
		<Module>.gcroot<System::String\u0020^>.=(A_0, "The C++ module failed to load during appdomain initialization.\n");
		<Module>.?InitializedPerAppDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)1;
		<Module>._initatexit_app_domain();
		<Module>._initterm_m((method*)(&<Module>.?A0xc6f23461.__xc_ma_a), (method*)(&<Module>.?A0xc6f23461.__xc_ma_z));
		<Module>.?InitializedPerAppDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A = (Progress.State)2;
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport.InitializeUninitializer(LanguageSupport* A_0)
	{
		<Module>.gcroot<System::String\u0020^>.=(A_0, "The C++ module failed to load during registration for the unload events.\n");
		<Module>.<CrtImplementationDetails>.RegisterModuleUninitializer(new EventHandler(<Module>.<CrtImplementationDetails>.LanguageSupport.DomainUnload));
	}

	[DebuggerStepThrough]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[SecurityCritical]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport._Initialize(LanguageSupport* A_0)
	{
		<Module>.?IsDefaultDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2_NA = AppDomain.CurrentDomain.IsDefaultAppDomain();
		<Module>.?Entered@DefaultDomain@<CrtImplementationDetails>@@2_NA = (<Module>.?IsDefaultDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2_NA || <Module>.?Entered@DefaultDomain@<CrtImplementationDetails>@@2_NA);
		void* ptr = <Module>._getFiberPtrId();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		RuntimeHelpers.PrepareConstrainedRegions();
		try
		{
			while (num2 == 0)
			{
				try
				{
				}
				finally
				{
					void* ptr2 = Interlocked.CompareExchange(ref <Module>.__native_startup_lock, ptr, 0L);
					if (ptr2 == null)
					{
						num2 = 1;
					}
					else if (ptr2 == ptr)
					{
						num = 1;
						num2 = 1;
					}
				}
				if (num2 == 0)
				{
					<Module>.Sleep(1000);
				}
			}
			<Module>.<CrtImplementationDetails>.LanguageSupport.InitializeVtables(A_0);
			if (<Module>.?IsDefaultDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2_NA)
			{
				<Module>.<CrtImplementationDetails>.LanguageSupport.InitializeNative(A_0);
				<Module>.<CrtImplementationDetails>.LanguageSupport.InitializePerProcess(A_0);
			}
			else
			{
				num3 = ((<Module>.<CrtImplementationDetails>.DefaultDomain.NeedsInitialization() != 0) ? 1 : num3);
			}
		}
		finally
		{
			if (num == 0)
			{
				Interlocked.Exchange(ref <Module>.__native_startup_lock, 0L);
			}
		}
		if (num3 != 0)
		{
			<Module>.<CrtImplementationDetails>.LanguageSupport.InitializeDefaultAppDomain(A_0);
		}
		<Module>.<CrtImplementationDetails>.LanguageSupport.InitializePerAppDomain(A_0);
		<Module>.?Initialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA = 1;
		<Module>.<CrtImplementationDetails>.LanguageSupport.InitializeUninitializer(A_0);
	}

	[SecurityCritical]
	internal static void <CrtImplementationDetails>.LanguageSupport.UninitializeAppDomain()
	{
		<Module>._app_exit_callback();
	}

	[SecurityCritical]
	internal unsafe static int <CrtImplementationDetails>.LanguageSupport._UninitializeDefaultDomain(void* cookie)
	{
		<Module>._exit_callback();
		<Module>.?InitializedPerProcess@DefaultDomain@<CrtImplementationDetails>@@2_NA = false;
		if (<Module>.?InitializedNativeFromCCTOR@DefaultDomain@<CrtImplementationDetails>@@2_NA)
		{
			<Module>._cexit();
			<Module>.__native_startup_state = (__enative_startup_state)0;
			<Module>.?InitializedNativeFromCCTOR@DefaultDomain@<CrtImplementationDetails>@@2_NA = false;
		}
		<Module>.?InitializedNative@DefaultDomain@<CrtImplementationDetails>@@2_NA = false;
		return 0;
	}

	[SecurityCritical]
	internal static void <CrtImplementationDetails>.LanguageSupport.UninitializeDefaultDomain()
	{
		if (<Module>.<CrtImplementationDetails>.DefaultDomain.NeedsUninitialization() != null)
		{
			if (AppDomain.CurrentDomain.IsDefaultAppDomain())
			{
				<Module>.<CrtImplementationDetails>.LanguageSupport._UninitializeDefaultDomain(null);
			}
			else
			{
				<Module>.<CrtImplementationDetails>.DoCallBackInDefaultDomain(<Module>.__unep@?_UninitializeDefaultDomain@LanguageSupport@<CrtImplementationDetails>@@$$FCAJPEAX@Z, null);
			}
		}
	}

	[SecurityCritical]
	[PrePrepareMethod]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	internal static void <CrtImplementationDetails>.LanguageSupport.DomainUnload(object source, EventArgs arguments)
	{
		if (<Module>.?Initialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA != 0 && Interlocked.Exchange(ref <Module>.?Uninitialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA, 1) == 0)
		{
			byte b = (Interlocked.Decrement(ref <Module>.?Count@AllDomains@<CrtImplementationDetails>@@2HA) == 0) ? 1 : 0;
			<Module>.<CrtImplementationDetails>.LanguageSupport.UninitializeAppDomain();
			if (b != 0)
			{
				<Module>.<CrtImplementationDetails>.LanguageSupport.UninitializeDefaultDomain();
			}
		}
	}

	[SecurityCritical]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[DebuggerStepThrough]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport.Cleanup(LanguageSupport* A_0, System.Exception innerException)
	{
		try
		{
			bool flag = ((Interlocked.Decrement(ref <Module>.?Count@AllDomains@<CrtImplementationDetails>@@2HA) == 0) ? 1 : 0) != 0;
			<Module>.<CrtImplementationDetails>.LanguageSupport.UninitializeAppDomain();
			if (flag)
			{
				<Module>.<CrtImplementationDetails>.LanguageSupport.UninitializeDefaultDomain();
			}
		}
		catch (System.Exception nestedException)
		{
			<Module>.<CrtImplementationDetails>.ThrowNestedModuleLoadException(innerException, nestedException);
		}
		catch (object obj)
		{
			<Module>.<CrtImplementationDetails>.ThrowNestedModuleLoadException(innerException, null);
		}
	}

	[SecurityCritical]
	internal unsafe static LanguageSupport* <CrtImplementationDetails>.LanguageSupport.{ctor}(LanguageSupport* A_0)
	{
		<Module>.gcroot<System::String\u0020^>.{ctor}(A_0);
		return A_0;
	}

	[SecurityCritical]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport.{dtor}(LanguageSupport* A_0)
	{
		<Module>.gcroot<System::String\u0020^>.{dtor}(A_0);
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	internal unsafe static void <CrtImplementationDetails>.LanguageSupport.Initialize(LanguageSupport* A_0)
	{
		bool flag = false;
		RuntimeHelpers.PrepareConstrainedRegions();
		try
		{
			<Module>.gcroot<System::String\u0020^>.=(A_0, "The C++ module failed to load.\n");
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				Interlocked.Increment(ref <Module>.?Count@AllDomains@<CrtImplementationDetails>@@2HA);
				flag = true;
			}
			<Module>.<CrtImplementationDetails>.LanguageSupport._Initialize(A_0);
		}
		catch (System.Exception innerException)
		{
			if (flag)
			{
				<Module>.<CrtImplementationDetails>.LanguageSupport.Cleanup(A_0, innerException);
			}
			<Module>.<CrtImplementationDetails>.ThrowModuleLoadException(<Module>.gcroot<System::String\u0020^>..PE$AAVString@System@@(A_0), innerException);
		}
		catch (object obj)
		{
			if (flag)
			{
				<Module>.<CrtImplementationDetails>.LanguageSupport.Cleanup(A_0, null);
			}
			<Module>.<CrtImplementationDetails>.ThrowModuleLoadException(<Module>.gcroot<System::String\u0020^>..PE$AAVString@System@@(A_0), null);
		}
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	static unsafe <Module>()
	{
		LanguageSupport languageSupport;
		<Module>.<CrtImplementationDetails>.LanguageSupport.{ctor}(ref languageSupport);
		try
		{
			<Module>.<CrtImplementationDetails>.LanguageSupport.Initialize(ref languageSupport);
		}
		catch
		{
			<Module>.___CxxCallUnwindDtor(ldftn(<CrtImplementationDetails>.LanguageSupport.{dtor}), (void*)(&languageSupport));
			throw;
		}
		<Module>.<CrtImplementationDetails>.LanguageSupport.{dtor}(ref languageSupport);
	}

	[DebuggerStepThrough]
	[SecuritySafeCritical]
	internal unsafe static gcroot<System::String\u0020^>* {ctor}(gcroot<System::String\u0020^>* A_0)
	{
		*A_0 = ((IntPtr)GCHandle.Alloc(null)).ToPointer();
		return A_0;
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal unsafe static void {dtor}(gcroot<System::String\u0020^>* A_0)
	{
		IntPtr value = new IntPtr(*A_0);
		((GCHandle)value).Free();
		*A_0 = 0L;
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal unsafe static gcroot<System::String\u0020^>* =(gcroot<System::String\u0020^>* A_0, string t)
	{
		IntPtr value = new IntPtr(*A_0);
		((GCHandle)value).Target = t;
		return A_0;
	}

	[SecuritySafeCritical]
	internal unsafe static string PE$AAVString@System@@(gcroot<System::String\u0020^>* A_0)
	{
		IntPtr value = new IntPtr(*A_0);
		return ((GCHandle)value).Target;
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal static ValueType <CrtImplementationDetails>.AtExitLock._handle()
	{
		if (<Module>.?_lock@AtExitLock@<CrtImplementationDetails>@@$$Q0PEAXEA != null)
		{
			IntPtr value = new IntPtr(<Module>.?_lock@AtExitLock@<CrtImplementationDetails>@@$$Q0PEAXEA);
			return GCHandle.FromIntPtr(value);
		}
		return null;
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal static void <CrtImplementationDetails>.AtExitLock._lock_Construct(object value)
	{
		<Module>.?_lock@AtExitLock@<CrtImplementationDetails>@@$$Q0PEAXEA = null;
		<Module>.<CrtImplementationDetails>.AtExitLock._lock_Set(value);
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal static void <CrtImplementationDetails>.AtExitLock._lock_Set(object value)
	{
		ValueType valueType = <Module>.<CrtImplementationDetails>.AtExitLock._handle();
		if (valueType == null)
		{
			valueType = GCHandle.Alloc(value);
			<Module>.?_lock@AtExitLock@<CrtImplementationDetails>@@$$Q0PEAXEA = GCHandle.ToIntPtr((GCHandle)valueType).ToPointer();
		}
		else
		{
			((GCHandle)valueType).Target = value;
		}
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal static object <CrtImplementationDetails>.AtExitLock._lock_Get()
	{
		ValueType valueType = <Module>.<CrtImplementationDetails>.AtExitLock._handle();
		if (valueType != null)
		{
			return ((GCHandle)valueType).Target;
		}
		return null;
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal static void <CrtImplementationDetails>.AtExitLock._lock_Destruct()
	{
		ValueType valueType = <Module>.<CrtImplementationDetails>.AtExitLock._handle();
		if (valueType != null)
		{
			((GCHandle)valueType).Free();
			<Module>.?_lock@AtExitLock@<CrtImplementationDetails>@@$$Q0PEAXEA = null;
		}
	}

	[DebuggerStepThrough]
	[SecuritySafeCritical]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool <CrtImplementationDetails>.AtExitLock.IsInitialized()
	{
		return (<Module>.<CrtImplementationDetails>.AtExitLock._lock_Get() != null) ? 1 : 0;
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal static void <CrtImplementationDetails>.AtExitLock.AddRef()
	{
		if (<Module>.<CrtImplementationDetails>.AtExitLock.IsInitialized() == null)
		{
			<Module>.<CrtImplementationDetails>.AtExitLock._lock_Construct(new object());
			<Module>.?_ref_count@AtExitLock@<CrtImplementationDetails>@@$$Q0HA = 0;
		}
		<Module>.?_ref_count@AtExitLock@<CrtImplementationDetails>@@$$Q0HA++;
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal static void <CrtImplementationDetails>.AtExitLock.RemoveRef()
	{
		<Module>.?_ref_count@AtExitLock@<CrtImplementationDetails>@@$$Q0HA += -1;
		if (<Module>.?_ref_count@AtExitLock@<CrtImplementationDetails>@@$$Q0HA == 0)
		{
			<Module>.<CrtImplementationDetails>.AtExitLock._lock_Destruct();
		}
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal static void <CrtImplementationDetails>.AtExitLock.Enter()
	{
		Monitor.Enter(<Module>.<CrtImplementationDetails>.AtExitLock._lock_Get());
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal static void <CrtImplementationDetails>.AtExitLock.Exit()
	{
		Monitor.Exit(<Module>.<CrtImplementationDetails>.AtExitLock._lock_Get());
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool __global_lock()
	{
		bool result = false;
		if (<Module>.<CrtImplementationDetails>.AtExitLock.IsInitialized() != null)
		{
			<Module>.<CrtImplementationDetails>.AtExitLock.Enter();
			result = true;
		}
		return result;
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool __global_unlock()
	{
		bool result = false;
		if (<Module>.<CrtImplementationDetails>.AtExitLock.IsInitialized() != null)
		{
			<Module>.<CrtImplementationDetails>.AtExitLock.Exit();
			result = true;
		}
		return result;
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	[return: MarshalAs(UnmanagedType.U1)]
	internal static bool __alloc_global_lock()
	{
		<Module>.<CrtImplementationDetails>.AtExitLock.AddRef();
		return <Module>.<CrtImplementationDetails>.AtExitLock.IsInitialized();
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal static void __dealloc_global_lock()
	{
		<Module>.<CrtImplementationDetails>.AtExitLock.RemoveRef();
	}

	[SecurityCritical]
	internal unsafe static int _atexit_helper(method func, ulong* __pexit_list_size, method** __ponexitend_e, method** __ponexitbegin_e)
	{
		method system.Void_u0020() = 0L;
		if (func == null)
		{
			return -1;
		}
		if (<Module>.?A0xf28fb846.__global_lock() == 1)
		{
			try
			{
				method* ptr = (method*)<Module>.DecodePointer(*(long*)__ponexitbegin_e);
				method* ptr2 = (method*)<Module>.DecodePointer(*(long*)__ponexitend_e);
				long num = (long)(ptr2 - ptr);
				if (*__pexit_list_size - 1UL < (ulong)num >> 3)
				{
					try
					{
						ulong num2 = *__pexit_list_size * 8UL;
						ulong num3 = (num2 < 4096UL) ? num2 : 4096UL;
						IntPtr cb = new IntPtr((int)(num2 + num3));
						IntPtr pv = new IntPtr((void*)ptr);
						IntPtr intPtr = Marshal.ReAllocHGlobal(pv, cb);
						IntPtr intPtr2 = intPtr;
						ptr2 = (method*)((byte*)intPtr2.ToPointer() + num);
						ptr = (method*)intPtr2.ToPointer();
						ulong num4 = *__pexit_list_size;
						ulong num5 = (512UL < num4) ? 512UL : num4;
						*__pexit_list_size = num4 + num5;
					}
					catch (OutOfMemoryException)
					{
						IntPtr cb2 = new IntPtr((int)(*__pexit_list_size * 8UL + 12UL));
						IntPtr pv2 = new IntPtr((void*)ptr);
						IntPtr intPtr3 = Marshal.ReAllocHGlobal(pv2, cb2);
						IntPtr intPtr4 = intPtr3;
						ptr2 = (intPtr4.ToPointer() - ptr) / (IntPtr)sizeof(method) + ptr2;
						ptr = (method*)intPtr4.ToPointer();
						*__pexit_list_size += 4UL;
					}
				}
				*(long*)ptr2 = func;
				ptr2 += 8L / (long)sizeof(method);
				system.Void_u0020() = func;
				*(long*)__ponexitbegin_e = <Module>.EncodePointer((void*)ptr);
				*(long*)__ponexitend_e = <Module>.EncodePointer((void*)ptr2);
			}
			catch (OutOfMemoryException)
			{
			}
			finally
			{
				<Module>.?A0xf28fb846.__global_unlock();
			}
			if (system.Void_u0020() != null)
			{
				return 0;
			}
		}
		return -1;
	}

	[SecurityCritical]
	internal unsafe static void _exit_callback()
	{
		if (<Module>.?A0xf28fb846.__exit_list_size != 0UL)
		{
			method* ptr = (method*)<Module>.DecodePointer((void*)<Module>.?A0xf28fb846.__onexitbegin_m);
			method* ptr2 = (method*)<Module>.DecodePointer((void*)<Module>.?A0xf28fb846.__onexitend_m);
			if (ptr != -1L && ptr != null && ptr2 != null)
			{
				method* ptr3 = ptr;
				method* ptr4 = ptr2;
				for (;;)
				{
					ptr2 -= 8L / (long)sizeof(method);
					if (ptr2 < ptr)
					{
						break;
					}
					if (*(long*)ptr2 != <Module>.EncodePointer(null))
					{
						void* ptr5 = <Module>.DecodePointer(*(long*)ptr2);
						*(long*)ptr2 = <Module>.EncodePointer(null);
						calli(System.Void(), ptr5);
						method* ptr6 = (method*)<Module>.DecodePointer((void*)<Module>.?A0xf28fb846.__onexitbegin_m);
						method* ptr7 = (method*)<Module>.DecodePointer((void*)<Module>.?A0xf28fb846.__onexitend_m);
						if (ptr3 != ptr6 || ptr4 != ptr7)
						{
							ptr3 = ptr6;
							ptr = ptr6;
							ptr4 = ptr7;
							ptr2 = ptr7;
						}
					}
				}
				IntPtr hglobal = new IntPtr((void*)ptr);
				Marshal.FreeHGlobal(hglobal);
			}
			<Module>.?A0xf28fb846.__dealloc_global_lock();
		}
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal unsafe static int _initatexit_m()
	{
		int result = 0;
		if (<Module>.?A0xf28fb846.__alloc_global_lock() == 1)
		{
			<Module>.?A0xf28fb846.__onexitbegin_m = (method*)<Module>.EncodePointer(Marshal.AllocHGlobal(256).ToPointer());
			<Module>.?A0xf28fb846.__onexitend_m = <Module>.?A0xf28fb846.__onexitbegin_m;
			<Module>.?A0xf28fb846.__exit_list_size = 32UL;
			result = 1;
		}
		return result;
	}

	internal static method _onexit_m(method _Function)
	{
		return (<Module>._atexit_m(_Function) == -1) ? 0L : _Function;
	}

	[SecurityCritical]
	internal unsafe static int _atexit_m(method func)
	{
		return <Module>._atexit_helper(<Module>.EncodePointer(func), &<Module>.?A0xf28fb846.__exit_list_size, &<Module>.?A0xf28fb846.__onexitend_m, &<Module>.?A0xf28fb846.__onexitbegin_m);
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal unsafe static int _initatexit_app_domain()
	{
		if (<Module>.?A0xf28fb846.__alloc_global_lock() == 1)
		{
			<Module>.__onexitbegin_app_domain = (method*)<Module>.EncodePointer(Marshal.AllocHGlobal(256).ToPointer());
			<Module>.__onexitend_app_domain = <Module>.__onexitbegin_app_domain;
			<Module>.__exit_list_size_app_domain = 32UL;
		}
		return 1;
	}

	[SecurityCritical]
	[HandleProcessCorruptedStateExceptions]
	internal unsafe static void _app_exit_callback()
	{
		if (<Module>.__exit_list_size_app_domain != 0UL)
		{
			method* ptr = (method*)<Module>.DecodePointer((void*)<Module>.__onexitbegin_app_domain);
			method* ptr2 = (method*)<Module>.DecodePointer((void*)<Module>.__onexitend_app_domain);
			try
			{
				if (ptr != -1L && ptr != null && ptr2 != null)
				{
					method* ptr3 = ptr;
					method* ptr4 = ptr2;
					for (;;)
					{
						do
						{
							ptr2 -= 8L / (long)sizeof(method);
						}
						while (ptr2 >= ptr && *(long*)ptr2 == <Module>.EncodePointer(null));
						if (ptr2 < ptr)
						{
							break;
						}
						method system.Void_u0020() = <Module>.DecodePointer(*(long*)ptr2);
						*(long*)ptr2 = <Module>.EncodePointer(null);
						calli(System.Void(), system.Void_u0020());
						method* ptr5 = (method*)<Module>.DecodePointer((void*)<Module>.__onexitbegin_app_domain);
						method* ptr6 = (method*)<Module>.DecodePointer((void*)<Module>.__onexitend_app_domain);
						if (ptr3 != ptr5 || ptr4 != ptr6)
						{
							ptr3 = ptr5;
							ptr = ptr5;
							ptr4 = ptr6;
							ptr2 = ptr6;
						}
					}
				}
			}
			finally
			{
				IntPtr hglobal = new IntPtr((void*)ptr);
				Marshal.FreeHGlobal(hglobal);
				<Module>.?A0xf28fb846.__dealloc_global_lock();
			}
		}
	}

	[SecurityCritical]
	internal static method _onexit_m_appdomain(method _Function)
	{
		return (<Module>._atexit_m_appdomain(_Function) == -1) ? 0L : _Function;
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal unsafe static int _atexit_m_appdomain(method func)
	{
		return <Module>._atexit_helper(<Module>.EncodePointer(func), &<Module>.__exit_list_size_app_domain, &<Module>.__onexitend_app_domain, &<Module>.__onexitbegin_app_domain);
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[SecurityCritical]
	[SuppressUnmanagedCodeSecurity]
	[DllImport("KERNEL32.dll")]
	public unsafe static extern void* DecodePointer(void* Ptr);

	[SecurityCritical]
	[SuppressUnmanagedCodeSecurity]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[DllImport("KERNEL32.dll")]
	public unsafe static extern void* EncodePointer(void* Ptr);

	[SecurityCritical]
	[DebuggerStepThrough]
	internal unsafe static int _initterm_e(method* pfbegin, method* pfend)
	{
		int num = 0;
		if (pfbegin < pfend)
		{
			while (num == 0)
			{
				ulong num2 = (ulong)(*(long*)pfbegin);
				if (num2 != 0UL)
				{
					num = calli(System.Int32 modopt(System.Runtime.CompilerServices.CallConvCdecl)(), num2);
				}
				pfbegin += 8L / (long)sizeof(method);
				if (pfbegin >= pfend)
				{
					break;
				}
			}
		}
		return num;
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	internal unsafe static void _initterm(method* pfbegin, method* pfend)
	{
		if (pfbegin < pfend)
		{
			do
			{
				ulong num = (ulong)(*(long*)pfbegin);
				if (num != 0UL)
				{
					calli(System.Void modopt(System.Runtime.CompilerServices.CallConvCdecl)(), num);
				}
				pfbegin += 8L / (long)sizeof(method);
			}
			while (pfbegin < pfend);
		}
	}

	[DebuggerStepThrough]
	internal static ModuleHandle <CrtImplementationDetails>.ThisModule.Handle()
	{
		return typeof(ThisModule).Module.ModuleHandle;
	}

	[DebuggerStepThrough]
	[SecurityCritical]
	[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
	internal unsafe static void _initterm_m(method* pfbegin, method* pfend)
	{
		if (pfbegin < pfend)
		{
			do
			{
				ulong num = (ulong)(*(long*)pfbegin);
				if (num != 0UL)
				{
					object obj = calli(System.Void modopt(System.Runtime.CompilerServices.IsConst)*(), <Module>.<CrtImplementationDetails>.ThisModule.ResolveMethod<void\u0020const\u0020*\u0020__clrcall(void)>(num));
				}
				pfbegin += 8L / (long)sizeof(method);
			}
			while (pfbegin < pfend);
		}
	}

	[SecurityCritical]
	[DebuggerStepThrough]
	internal static method <CrtImplementationDetails>.ThisModule.ResolveMethod<void\u0020const\u0020*\u0020__clrcall(void)>(method methodToken)
	{
		return <Module>.<CrtImplementationDetails>.ThisModule.Handle().ResolveMethodHandle(methodToken).GetFunctionPointer().ToPointer();
	}

	[SecurityCritical]
	[HandleProcessCorruptedStateExceptions]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
	internal unsafe static void ___CxxCallUnwindDtor(method pDtor, void* pThis)
	{
		try
		{
			calli(System.Void(System.Void*), pThis, pDtor);
		}
		catch when (endfilter(<Module>.__FrameUnwindFilter(Marshal.GetExceptionPointers()) != null))
		{
		}
	}

	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[SecurityCritical]
	[HandleProcessCorruptedStateExceptions]
	[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
	internal unsafe static void ___CxxCallUnwindDelDtor(method pDtor, void* pThis)
	{
		try
		{
			calli(System.Void(System.Void*), pThis, pDtor);
		}
		catch when (endfilter(<Module>.__FrameUnwindFilter(Marshal.GetExceptionPointers()) != null))
		{
		}
	}

	[SecurityCritical]
	[HandleProcessCorruptedStateExceptions]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
	internal unsafe static void ___CxxCallUnwindVecDtor(method pVecDtor, void* ptr, ulong size, int count, method pDtor)
	{
		try
		{
			calli(System.Void(System.Void*,System.UInt64,System.Int32,System.Void (System.Void*)), ptr, size, count, pDtor, pVecDtor);
		}
		catch when (endfilter(<Module>.__FrameUnwindFilter(Marshal.GetExceptionPointers()) != null))
		{
		}
	}

	[SuppressUnmanagedCodeSecurity]
	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig)]
	[return: MarshalAs(UnmanagedType.U1)]
	internal unsafe static extern bool IsSpaceEnough(long, long, ushort*);

	[SuppressUnmanagedCodeSecurity]
	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged)]
	internal unsafe static extern int VirtualFree(void*, ulong, uint);

	[SuppressUnmanagedCodeSecurity]
	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged)]
	internal unsafe static extern void* VirtualAlloc(void*, ulong, uint, uint);

	[SuppressUnmanagedCodeSecurity]
	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged)]
	internal unsafe static extern int _wcsicmp(ushort*, ushort*);

	[SuppressUnmanagedCodeSecurity]
	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig)]
	internal unsafe static extern void* _getFiberPtrId();

	[SuppressUnmanagedCodeSecurity]
	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged)]
	internal static extern void _amsg_exit(int);

	[SuppressUnmanagedCodeSecurity]
	[MethodImpl(MethodImplOptions.Unmanaged | MethodImplOptions.PreserveSig)]
	internal static extern void __security_init_cookie();

	[SuppressUnmanagedCodeSecurity]
	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged)]
	internal static extern void Sleep(uint);

	[SuppressUnmanagedCodeSecurity]
	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged)]
	internal static extern void _cexit();

	[SuppressUnmanagedCodeSecurity]
	[DllImport("", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
	[MethodImpl(MethodImplOptions.Unmanaged)]
	internal unsafe static extern int __FrameUnwindFilter(_EXCEPTION_POINTERS*);

	internal static _GUID GUID_PROCESSOR_PERF_DECREASE_TIME;

	internal static _GUID GUID_PROCESSOR_IDLE_TIME_CHECK;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_INCREASE_POLICY;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_AFFINITY_WEIGHTING;

	internal static _GUID GUID_ALLOW_SYSTEM_REQUIRED;

	internal static _GUID FIREWALL_PORT_OPEN_GUID;

	internal static _GUID GUID_ALLOW_AWAYMODE;

	internal static _GUID GUID_ALLOW_DISPLAY_REQUIRED;

	internal static _GUID PPM_IDLESTATES_DATA_GUID;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_DECREASE_THRESHOLD;

	internal static _GUID GUID_PROCESSOR_IDLE_STATE_MAXIMUM;

	internal static _GUID GUID_VIDEO_DIM_TIMEOUT;

	internal static _GUID GUID_PROCESSOR_PERF_INCREASE_TIME;

	internal static _GUID GUID_BATTERY_DISCHARGE_ACTION_2;

	internal static _GUID DOMAIN_JOIN_GUID;

	internal static _GUID NETWORK_MANAGER_FIRST_IP_ADDRESS_ARRIVAL_GUID;

	internal static _GUID GUID_CRITICAL_POWER_TRANSITION;

	internal static _GUID PPM_PERFSTATE_DOMAIN_CHANGE_GUID;

	internal static _GUID GUID_DEVINTERFACE_PARTITION;

	internal static _GUID GUID_IDLE_RESILIENCY_SUBGROUP;

	internal static _GUID GUID_PROCESSOR_PERF_BOOST_POLICY;

	internal static _GUID GUID_PROCESSOR_PARKING_HEADROOM_THRESHOLD;

	internal static _GUID GUID_DEVINTERFACE_WRITEONCEDISK;

	internal static _GUID GUID_VIDEO_ADAPTIVE_DISPLAY_BRIGHTNESS;

	internal static _GUID GUID_CONSOLE_DISPLAY_STATE;

	internal static _GUID GUID_DISK_ADAPTIVE_POWERDOWN;

	internal static _GUID GUID_VIDEO_ADAPTIVE_PERCENT_INCREASE;

	internal static _GUID GUID_DISK_COALESCING_POWERDOWN_TIMEOUT;

	internal static _GUID USER_POLICY_PRESENT_GUID;

	internal static _GUID GUID_SYSTEM_COOLING_POLICY;

	internal static _GUID GUID_VIDEO_ANNOYANCE_TIMEOUT;

	internal static _GUID GUID_DEVINTERFACE_STORAGEPORT;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_OVER_UTILIZATION_WEIGHTING;

	internal static _GUID GUID_IDLE_BACKGROUND_TASK;

	internal static _GUID GUID_VIDEO_CURRENT_MONITOR_BRIGHTNESS;

	internal static _GUID GUID_GLOBAL_USER_PRESENCE;

	internal static _GUID NO_SUBGROUP_GUID;

	internal static _GUID GUID_DEVINTERFACE_TAPE;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_MIN_CORES;

	internal static _GUID GUID_BATTERY_DISCHARGE_LEVEL_3;

	internal static _GUID GUID_DEVINTERFACE_SERENUM_BUS_ENUMERATOR;

	internal static _GUID GUID_BATTERY_DISCHARGE_LEVEL_1;

	internal static _GUID PPM_THERMALCONSTRAINT_GUID;

	internal static _GUID GUID_DEVINTERFACE_HIDDEN_VOLUME;

	internal static _GUID GUID_DEVINTERFACE_SES;

	internal static _GUID IID_IPrintDialogCallback;

	internal static _GUID GUID_MONITOR_POWER_ON;

	internal static _GUID FIREWALL_PORT_CLOSE_GUID;

	internal static _GUID GUID_PROCESSOR_IDLE_DISABLE;

	internal static _GUID GUID_BATTERY_DISCHARGE_FLAGS_2;

	internal static _GUID GUID_PROCESSOR_IDLESTATE_POLICY;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_INCREASE_TIME;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_MAX_CORES;

	internal static _GUID GUID_POWERBUTTON_ACTION;

	internal static _GUID GUID_LOCK_CONSOLE_ON_WAKE;

	internal static _GUID GUID_PROCESSOR_PERF_INCREASE_POLICY;

	internal static _GUID GUID_DEVICE_POWER_POLICY_VIDEO_BRIGHTNESS;

	internal static _GUID GUID_DEVINTERFACE_CDROM;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_AFFINITY_HISTORY_DECREASE_FACTOR;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_INCREASE_THRESHOLD;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_OVER_UTILIZATION_HISTORY_THRESHOLD;

	internal static _GUID NAMED_PIPE_EVENT_GUID;

	internal static _GUID GUID_EXECUTION_REQUIRED_REQUEST_TIMEOUT;

	internal static _GUID GUID_DISK_SUBGROUP;

	internal static _GUID GUID_ACTIVE_POWERSCHEME;

	internal static _GUID PPM_IDLESTATE_CHANGE_GUID;

	internal static _GUID GUID_BATTERY_DISCHARGE_FLAGS_0;

	internal static _GUID GUID_PROCESSOR_PARKING_CORE_OVERRIDE;

	internal static _GUID GUID_DISK_POWERDOWN_TIMEOUT;

	internal static _GUID GUID_ADAPTIVE_POWER_BEHAVIOR_SUBGROUP;

	internal static _GUID GUID_DEVINTERFACE_VOLUME;

	internal static _GUID IID_IPrintDialogServices;

	internal static _GUID GUID_PROCESSOR_PERF_TIME_CHECK;

	internal static _GUID GUID_BATTERY_DISCHARGE_LEVEL_0;

	internal static _GUID ALL_POWERSCHEMES_GUID;

	internal static _GUID NETWORK_MANAGER_LAST_IP_ADDRESS_REMOVAL_GUID;

	internal static _GUID GUID_VIDEO_CONSOLE_LOCK_TIMEOUT;

	internal static _GUID GUID_DEVICE_IDLE_POLICY;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_AFFINITY_HISTORY_THRESHOLD;

	internal static _GUID GUID_SLEEP_SUBGROUP;

	internal static _GUID GUID_DEVINTERFACE_COMPORT;

	internal static _GUID GUID_PCIEXPRESS_SETTINGS_SUBGROUP;

	internal static _GUID RPC_INTERFACE_EVENT_GUID;

	internal static _GUID GUID_STANDBY_TIMEOUT;

	internal static _GUID GUID_MAX_POWER_SAVINGS;

	internal static _GUID GUID_PROCESSOR_SETTINGS_SUBGROUP;

	internal static _GUID GUID_LIDCLOSE_ACTION;

	internal static _GUID GUID_IDLE_RESILIENCY_PERIOD;

	internal static _GUID GUID_BATTERY_DISCHARGE_ACTION_0;

	internal static _GUID GUID_LIDSWITCH_STATE_CHANGE;

	internal static _GUID GUID_MIN_POWER_SAVINGS;

	internal static _GUID GUID_LIDOPEN_POWERSTATE;

	internal static _GUID GUID_USERINTERFACEBUTTON_ACTION;

	internal static _GUID GUID_VIDEO_SUBGROUP;

	internal static _GUID GUID_PROCESSOR_PERF_INCREASE_THRESHOLD;

	internal static _GUID GUID_PROCESSOR_IDLE_ALLOW_SCALING;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_OVER_UTILIZATION_HISTORY_DECREASE_FACTOR;

	internal static _GUID GUID_DEVINTERFACE_CDCHANGER;

	internal static _GUID GUID_PROCESSOR_PERF_DECREASE_THRESHOLD;

	internal static _GUID GUID_PROCESSOR_THROTTLE_MINIMUM;

	internal static _GUID GUID_PROCESSOR_PERF_LATENCY_HINT;

	internal static _GUID GUID_BATTERY_PERCENTAGE_REMAINING;

	internal static _GUID GUID_PROCESSOR_IDLE_PROMOTE_THRESHOLD;

	internal static _GUID GUID_DEVINTERFACE_VMLUN;

	internal static _GUID GUID_PROCESSOR_PARKING_CONCURRENCY_THRESHOLD;

	internal static _GUID PPM_PERFSTATES_DATA_GUID;

	internal static _GUID GUID_HIBERNATE_FASTS4_POLICY;

	internal static int ReplicaSeeder;

	internal static _GUID GUID_UNATTEND_SLEEP_TIMEOUT;

	internal static _GUID GUID_DEVICE_POWER_POLICY_VIDEO_DIM_BRIGHTNESS;

	internal static _GUID GUID_DEVINTERFACE_MEDIUMCHANGER;

	internal static _GUID GUID_SESSION_DISPLAY_STATUS;

	internal static _GUID GUID_ALLOW_RTC_WAKE;

	internal static _GUID GUID_PROCESSOR_PERF_HISTORY;

	internal static _GUID GUID_BATTERY_SUBGROUP;

	internal static _GUID GUID_DEVINTERFACE_FLOPPY;

	internal static _GUID DOMAIN_LEAVE_GUID;

	internal static _GUID GUID_TYPICAL_POWER_SAVINGS;

	internal static _GUID GUID_PROCESSOR_THROTTLE_MAXIMUM;

	internal static _GUID GUID_PROCESSOR_PERFSTATE_POLICY;

	internal static _GUID PPM_PERFSTATE_CHANGE_GUID;

	internal static _GUID GUID_PROCESSOR_PERF_BOOST_MODE;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_DECREASE_POLICY;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_OVER_UTILIZATION_THRESHOLD;

	internal static _GUID GUID_HIBERNATE_TIMEOUT;

	internal static _GUID GUID_DEVINTERFACE_SMARTCARD_READER;

	internal static _GUID GUID_PROCESSOR_DISTRIBUTE_UTILITY;

	internal static _GUID GUID_NON_ADAPTIVE_INPUT_TIMEOUT;

	internal static _GUID GUID_SYSTEM_AWAYMODE;

	internal static _GUID GUID_DISK_BURST_IGNORE_THRESHOLD;

	internal static _GUID GUID_PCIEXPRESS_ASPM_POLICY;

	internal static _GUID GUID_BATTERY_DISCHARGE_FLAGS_3;

	internal static _GUID GUID_PROCESSOR_IDLE_DEMOTE_THRESHOLD;

	internal static _GUID PPM_PERFMON_PERFSTATE_GUID;

	internal static _GUID GUID_ENABLE_SWITCH_FORCED_SHUTDOWN;

	internal static _GUID GUID_PROCESSOR_PERF_DECREASE_POLICY;

	internal static _GUID PPM_IDLE_ACCOUNTING_GUID;

	internal static _GUID GUID_BATTERY_DISCHARGE_ACTION_3;

	internal static _GUID GUID_PROCESSOR_CORE_PARKING_DECREASE_TIME;

	internal static _GUID GUID_ACDC_POWER_SOURCE;

	internal static _GUID GUID_SLEEP_IDLE_THRESHOLD;

	internal static _GUID GUID_POWERSCHEME_PERSONALITY;

	internal static _GUID GUID_BATTERY_DISCHARGE_FLAGS_1;

	internal static _GUID GUID_PROCESSOR_PARKING_PERF_STATE;

	internal static _GUID PPM_IDLE_ACCOUNTING_EX_GUID;

	internal static _GUID GUID_SYSTEM_BUTTON_SUBGROUP;

	internal static _GUID GUID_APPLAUNCH_BUTTON;

	internal static _GUID GUID_PROCESSOR_ALLOW_THROTTLING;

	internal static _GUID MACHINE_POLICY_PRESENT_GUID;

	internal static _GUID GUID_DISK_IDLE_TIMEOUT;

	internal static _GUID GUID_SESSION_USER_PRESENCE;

	internal static _GUID GUID_BACKGROUND_TASK_NOTIFICATION;

	internal static _GUID PPM_THERMAL_POLICY_CHANGE_GUID;

	internal static _GUID CUSTOM_SYSTEM_STATE_CHANGE_EVENT_GUID;

	internal static _GUID GUID_ALLOW_STANDBY_STATES;

	internal static _GUID GUID_DEVINTERFACE_DISK;

	internal static _GUID GUID_BATTERY_DISCHARGE_ACTION_1;

	internal static _GUID GUID_PROCESSOR_THROTTLE_POLICY;

	internal static _GUID GUID_BATTERY_DISCHARGE_LEVEL_2;

	internal static _GUID GUID_SLEEPBUTTON_ACTION;

	internal static _GUID GUID_VIDEO_POWERDOWN_TIMEOUT;

	internal static _GUID GUID_VIDEO_ADAPTIVE_POWERDOWN;

	internal static $ArrayType$$$BY0P@$$CBG ??_C@_1BO@LKIGKJOJ@?$AAC?$AAl?$AAu?$AAs?$AAt?$AAe?$AAr?$AA?4?$AAR?$AAe?$AAp?$AAl?$AAa?$AAy?$AA?$AA@;

	internal static $ArrayType$$$BY0CA@$$CBG ??_C@_1EA@IDJOIADD@?$AAH?$AAr?$AAE?$AAS?$AAE?$AAB?$AAa?$AAc?$AAk?$AAu?$AAp?$AAS?$AAe?$AAt?$AAu?$AAp?$AA?3?$AA?5?$AAn?$AAo?$AA?5?$AAe?$AAd?$AAb?$AA?5?$AAo?$AAn?$AAl?$AAi?$AAn?$AAe?$AA?$AA@;

	internal static $ArrayType$$$BY0BF@$$CBG ??_C@_1CK@OOGKDIDK@?$AA?$CF?$AAs?$AA?$DL?$AA?5?$AAE?$AAr?$AAr?$AAo?$AAr?$AA?5?$AAC?$AAo?$AAd?$AAe?$AA?3?$AA?5?$AA?$CF?$AA?$CD?$AAx?$AA?4?$AA?$AA@;

	internal static $ArrayType$$$BY0BB@$$CBD ??_C@_0BB@DEJIFCPK@HrESEBackupSetup?$AA@;

	internal static $ArrayType$$$BY0EG@$$CBD ??_C@_0EG@JGPBOCNG@f?3?215?400?41497?2sources?2dev?2cluste@;

	internal static $ArrayType$$$BY0BP@$$CBG ??_C@_1DO@BEMMPGGP@?$AA?$CF?$AAh?$AAs?$AA?5?$AA?$EA?$AA?5?$AA?$CF?$AAd?$AA?3?$AA?5?$AA?$CF?$AAh?$AAs?$AA?5?$AAf?$AAa?$AAi?$AAl?$AAe?$AAd?$AA?5?$AAw?$AAi?$AAt?$AAh?$AA?5?$AA?$CF?$AA?$CD?$AAx?$AA?4?$AA?$AA@;

	internal static $ArrayType$$$BY0CG@$$CBG ??_C@_1EM@MIFDHKEH@?$AA?$CF?$AAh?$AAs?$AA?5?$AA?$EA?$AA?5?$AA?$CF?$AAd?$AA?3?$AA?5?$AA?$CF?$AAh?$AAs?$AA?5?$AAc?$AAo?$AAm?$AAp?$AAl?$AAe?$AAt?$AAe?$AAd?$AA?5?$AAs?$AAu?$AAc?$AAc?$AAe?$AAs?$AAs?$AAf@;

	internal static $ArrayType$$$BY0BE@$$CBD ??_C@_0BE@BFBFPMIP@HrESEBackupReadFile?$AA@;

	internal static $ArrayType$$$BY0EA@$$CBG ??_C@_1IA@IGHGLPMJ@?$AAE?$AAr?$AAr?$AAo?$AAr?$AA?3?$AA?5?$AAB?$AAR?$AA_?$AAL?$AAo?$AAg?$AAE?$AAr?$AAr?$AAo?$AAr?$AA?5?$AAf?$AAa?$AAi?$AAl?$AAe?$AAd?$AA?5?$AAb?$AAe?$AAc?$AAa?$AAu?$AAs@;

	internal static $ArrayType$$$BY0BD@$$CBG ??_C@_1CG@CFLDDKCE@?$AA?5?$AAe?$AAr?$AAr?$AAo?$AAr?$AA?5?$AAc?$AAo?$AAd?$AAe?$AA?5?$AA?$DN?$AA?5?$AA?$CF?$AA?$CD?$AAx?$AA?4?$AA?$AA@;

	internal static $ArrayType$$$BY0EF@$$CBG ??_C@_1IK@NCAECPEB@?$AAE?$AAr?$AAr?$AAo?$AAr?$AA?3?$AA?5?$AAB?$AAR?$AA_?$AAL?$AAo?$AAg?$AAE?$AAr?$AAr?$AAo?$AAr?$AA?5?$AAf?$AAa?$AAi?$AAl?$AAe?$AAd?$AA?5?$AAt?$AAo?$AA?5?$AAa?$AAp?$AAp@;

	internal static $ArrayType$$$BY0FA@$$CBG ??_C@_1KA@DOFAHBGC@?$AAE?$AAr?$AAr?$AAo?$AAr?$AA?3?$AA?5?$AAB?$AAR?$AA_?$AAL?$AAo?$AAg?$AAE?$AAr?$AAr?$AAo?$AAr?$AA?5?$AAw?$AAa?$AAs?$AA?5?$AAo?$AAu?$AAt?$AA?5?$AAo?$AAf?$AA?5?$AAb?$AAu@;

	internal static $ArrayType$$$BY02$$CBG ??_C@_15GANGMFKL@?$AA?$CF?$AAs?$AA?$AA@;

	internal static $ArrayType$$$BY0EF@$$CBG ??_C@_1IK@MLLIKKLI@?$AAE?$AAr?$AAr?$AAo?$AAr?$AA?3?$AA?5?$AAB?$AAR?$AA_?$AAL?$AAo?$AAg?$AAE?$AAr?$AAr?$AAo?$AAr?$AA?5?$AAf?$AAa?$AAi?$AAl?$AAe?$AAd?$AA?5?$AAt?$AAo?$AA?5?$AAf?$AAo?$AAr@;

	internal static $ArrayType$$$BY0CH@$$CBG ??_C@_1EO@NIBKGOJF@?$AAR?$AAe?$AAc?$AAe?$AAi?$AAv?$AAe?$AAd?$AA?5?$AAe?$AAr?$AAr?$AAo?$AAr?$AA?3?$AA?5?$AA?$CF?$AA?$CD?$AAx?$AA?0?$AA?5?$AAe?$AAr?$AAr?$AAo?$AAr?$AA?5?$AAc?$AAo?$AAd?$AAe?$AA?3@;

	internal static $ArrayType$$$BY0CG@$$CBG ??_C@_1EM@IBOADBE@?$AAR?$AAe?$AAc?$AAe?$AAi?$AAv?$AAe?$AAd?$AA?5?$AAe?$AAr?$AAr?$AAo?$AAr?$AA?3?$AA?5?$AA?$CF?$AA?$CD?$AAx?$AA?0?$AA?5?$AAe?$AAr?$AAr?$AAo?$AAr?$AA?5?$AAc?$AAo?$AAd?$AAe?$AA?3@;

	internal static int g_fTraceInit;

	internal unsafe static ushort* g_pszComponentName;

	internal static _GUID g_ReplayServiceTraceGuid;

	public static method __m2mep@?GetUnmanagedUni@?A0xe4b0b5ec@@$$FYMPEAGPE$AAVString@System@@@Z;

	public static method __m2mep@?FreeUnmanagedUni@?A0xe4b0b5ec@@$$FYAXPEAG@Z;

	public static method __m2mep@?MakeManagedBytes@?A0xe4b0b5ec@@$$FYMP$01EAEPEAEH@Z;

	public static method __m2mep@?GetEC@ReplicaSeeder@Cluster@Exchange@Microsoft@@$$FYAJJ@Z;

	internal static $ArrayType$$$BY0BM@$$CBG g_wszEndpointAnnotation;

	public static method __m2mep@?GetUnmanagedUni@?A0xbcb4c405@@$$FYMPEAGPE$AAVString@System@@@Z;

	public static method __m2mep@?FreeUnmanagedUni@?A0xbcb4c405@@$$FYAXPEAG@Z;

	internal static __s_GUID _GUID_90f1a06e_7712_4762_86b5_7a5eba6bdb02;

	internal static __s_GUID _GUID_cb2f6722_ab3a_11d2_9c40_00c04fa30a3e;

	internal static $ArrayType$$$BY00Q6MPEBXXZ __xc_mp_z;

	internal static $ArrayType$$$BY00Q6MPEBXXZ __xi_vt_a;

	[FixedAddressValueType]
	internal static Progress.State ?InitializedVtables@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A;

	internal static method ?InitializedVtables$initializer$@CurrentDomain@<CrtImplementationDetails>@@$$Q2P6MXXZEA;

	[FixedAddressValueType]
	internal static bool ?IsDefaultDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2_NA;

	internal static method ?IsDefaultDomain$initializer$@CurrentDomain@<CrtImplementationDetails>@@$$Q2P6MXXZEA;

	internal static $ArrayType$$$BY00Q6MPEBXXZ __xc_ma_a;

	[FixedAddressValueType]
	internal static Progress.State ?InitializedPerAppDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A;

	internal static method ?InitializedPerAppDomain$initializer$@CurrentDomain@<CrtImplementationDetails>@@$$Q2P6MXXZEA;

	internal static $ArrayType$$$BY00Q6MPEBXXZ __xc_ma_z;

	[FixedAddressValueType]
	internal static Progress.State ?InitializedNative@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A;

	internal static method ?InitializedNative$initializer$@CurrentDomain@<CrtImplementationDetails>@@$$Q2P6MXXZEA;

	internal static $ArrayType$$$BY00Q6MPEBXXZ __xi_vt_z;

	internal static __s_GUID _GUID_cb2f6723_ab3a_11d2_9c40_00c04fa30a3e;

	[FixedAddressValueType]
	internal static int ?Uninitialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA;

	internal static method ?Uninitialized$initializer$@CurrentDomain@<CrtImplementationDetails>@@$$Q2P6MXXZEA;

	[FixedAddressValueType]
	internal static int ?Initialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA;

	internal static method ?Initialized$initializer$@CurrentDomain@<CrtImplementationDetails>@@$$Q2P6MXXZEA;

	internal static bool ?InitializedPerProcess@DefaultDomain@<CrtImplementationDetails>@@2_NA;

	[FixedAddressValueType]
	internal static Progress.State ?InitializedPerProcess@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4State@Progress@2@A;

	internal static bool ?Entered@DefaultDomain@<CrtImplementationDetails>@@2_NA;

	internal static bool ?InitializedNative@DefaultDomain@<CrtImplementationDetails>@@2_NA;

	internal static int ?Count@AllDomains@<CrtImplementationDetails>@@2HA;

	internal static uint ?ProcessAttach@NativeDll@<CrtImplementationDetails>@@0IB;

	internal static uint ?ThreadAttach@NativeDll@<CrtImplementationDetails>@@0IB;

	internal static TriBool.State ?hasNative@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A;

	internal static uint ?ProcessDetach@NativeDll@<CrtImplementationDetails>@@0IB;

	internal static uint ?ThreadDetach@NativeDll@<CrtImplementationDetails>@@0IB;

	internal static uint ?ProcessVerifier@NativeDll@<CrtImplementationDetails>@@0IB;

	internal static TriBool.State ?hasPerProcess@DefaultDomain@<CrtImplementationDetails>@@0W4State@TriBool@2@A;

	internal static bool ?InitializedNativeFromCCTOR@DefaultDomain@<CrtImplementationDetails>@@2_NA;

	internal static $ArrayType$$$BY00Q6MPEBXXZ __xc_mp_a;

	internal static __s_GUID _GUID_90f1a06c_7712_4762_86b5_7a5eba6bdb02;

	internal static method ?InitializedPerProcess$initializer$@CurrentDomain@<CrtImplementationDetails>@@$$Q2P6MXXZEA;

	public static method __m2mep@?IsInDllMain@NativeDll@<CrtImplementationDetails>@@$$FSA_NXZ;

	public static method __m2mep@?IsInProcessAttach@NativeDll@<CrtImplementationDetails>@@$$FSA_NXZ;

	public static method __m2mep@?IsInProcessDetach@NativeDll@<CrtImplementationDetails>@@$$FSA_NXZ;

	public static method __m2mep@?IsSafeForManagedCode@NativeDll@<CrtImplementationDetails>@@$$FSA_NXZ;

	public static method __m2mep@?ThrowNestedModuleLoadException@<CrtImplementationDetails>@@$$FYMXPE$AAVException@System@@0@Z;

	public static method __m2mep@?ThrowModuleLoadException@<CrtImplementationDetails>@@$$FYMXPE$AAVString@System@@@Z;

	public static method __m2mep@?ThrowModuleLoadException@<CrtImplementationDetails>@@$$FYMXPE$AAVString@System@@PE$AAVException@3@@Z;

	public static method __m2mep@?RegisterModuleUninitializer@<CrtImplementationDetails>@@$$FYMXPE$AAVEventHandler@System@@@Z;

	public static method __m2mep@?FromGUID@<CrtImplementationDetails>@@$$FYM?AVGuid@System@@AEBU_GUID@@@Z;

	public static method __m2mep@?__get_default_appdomain@@$$FYAJPEAPEAUIUnknown@@@Z;

	public static method __m2mep@?__release_appdomain@@$$FYAXPEAUIUnknown@@@Z;

	public static method __m2mep@?GetDefaultDomain@<CrtImplementationDetails>@@$$FYMPE$AAVAppDomain@System@@XZ;

	public static method __m2mep@?DoCallBackInDefaultDomain@<CrtImplementationDetails>@@$$FYAXP6AJPEAX@Z0@Z;

	public static method __m2mep@?DoNothing@DefaultDomain@<CrtImplementationDetails>@@$$FCAJPEAX@Z;

	public static method __m2mep@?HasPerProcess@DefaultDomain@<CrtImplementationDetails>@@$$FSA_NXZ;

	public static method __m2mep@?HasNative@DefaultDomain@<CrtImplementationDetails>@@$$FSA_NXZ;

	public static method __m2mep@?NeedsInitialization@DefaultDomain@<CrtImplementationDetails>@@$$FSA_NXZ;

	public static method __m2mep@?NeedsUninitialization@DefaultDomain@<CrtImplementationDetails>@@$$FSA_NXZ;

	public static method __m2mep@?Initialize@DefaultDomain@<CrtImplementationDetails>@@$$FSAXXZ;

	public static method __m2mep@?InitializeVtables@LanguageSupport@<CrtImplementationDetails>@@$$FAEAAXXZ;

	public static method __m2mep@?InitializeDefaultAppDomain@LanguageSupport@<CrtImplementationDetails>@@$$FAEAAXXZ;

	public static method __m2mep@?InitializeNative@LanguageSupport@<CrtImplementationDetails>@@$$FAEAAXXZ;

	public static method __m2mep@?InitializePerProcess@LanguageSupport@<CrtImplementationDetails>@@$$FAEAAXXZ;

	public static method __m2mep@?InitializePerAppDomain@LanguageSupport@<CrtImplementationDetails>@@$$FAEAAXXZ;

	public static method __m2mep@?InitializeUninitializer@LanguageSupport@<CrtImplementationDetails>@@$$FAEAAXXZ;

	public static method __m2mep@?_Initialize@LanguageSupport@<CrtImplementationDetails>@@$$FAEAAXXZ;

	public static method __m2mep@?UninitializeAppDomain@LanguageSupport@<CrtImplementationDetails>@@$$FCAXXZ;

	public static method __m2mep@?_UninitializeDefaultDomain@LanguageSupport@<CrtImplementationDetails>@@$$FCAJPEAX@Z;

	public static method __m2mep@?UninitializeDefaultDomain@LanguageSupport@<CrtImplementationDetails>@@$$FCAXXZ;

	public static method __m2mep@?DomainUnload@LanguageSupport@<CrtImplementationDetails>@@$$FCMXPE$AAVObject@System@@PE$AAVEventArgs@4@@Z;

	public static method __m2mep@?Cleanup@LanguageSupport@<CrtImplementationDetails>@@$$FAEAMXPE$AAVException@System@@@Z;

	public static method __m2mep@??0LanguageSupport@<CrtImplementationDetails>@@$$FQEAA@XZ;

	public static method __m2mep@??1LanguageSupport@<CrtImplementationDetails>@@$$FQEAA@XZ;

	public static method __m2mep@?Initialize@LanguageSupport@<CrtImplementationDetails>@@$$FQEAAXXZ;

	public static method cctor@@$$FYMXXZ;

	public static method __m2mep@??0?$gcroot@PE$AAVString@System@@@@$$FQEAA@XZ;

	public static method __m2mep@??1?$gcroot@PE$AAVString@System@@@@$$FQEAA@XZ;

	public static method __m2mep@??4?$gcroot@PE$AAVString@System@@@@$$FQEAMAEAU0@PE$AAVString@System@@@Z;

	public static method __m2mep@??B?$gcroot@PE$AAVString@System@@@@$$FQEBMPE$AAVString@System@@XZ;

	public unsafe static int** __unep@?DoNothing@DefaultDomain@<CrtImplementationDetails>@@$$FCAJPEAX@Z;

	public unsafe static int** __unep@?_UninitializeDefaultDomain@LanguageSupport@<CrtImplementationDetails>@@$$FCAJPEAX@Z;

	[FixedAddressValueType]
	internal static ulong __exit_list_size_app_domain;

	[FixedAddressValueType]
	internal unsafe static method* __onexitbegin_app_domain;

	internal static ulong __exit_list_size;

	[FixedAddressValueType]
	internal unsafe static method* __onexitend_app_domain;

	internal unsafe static method* __onexitbegin_m;

	internal unsafe static method* __onexitend_m;

	[FixedAddressValueType]
	internal static int ?_ref_count@AtExitLock@<CrtImplementationDetails>@@$$Q0HA;

	[FixedAddressValueType]
	internal unsafe static void* ?_lock@AtExitLock@<CrtImplementationDetails>@@$$Q0PEAXEA;

	public static method __m2mep@?_handle@AtExitLock@<CrtImplementationDetails>@@$$FCMPE$AAVGCHandle@InteropServices@Runtime@System@@XZ;

	public static method __m2mep@?_lock_Construct@AtExitLock@<CrtImplementationDetails>@@$$FCMXPE$AAVObject@System@@@Z;

	public static method __m2mep@?_lock_Set@AtExitLock@<CrtImplementationDetails>@@$$FCMXPE$AAVObject@System@@@Z;

	public static method __m2mep@?_lock_Get@AtExitLock@<CrtImplementationDetails>@@$$FCMPE$AAVObject@System@@XZ;

	public static method __m2mep@?_lock_Destruct@AtExitLock@<CrtImplementationDetails>@@$$FCAXXZ;

	public static method __m2mep@?IsInitialized@AtExitLock@<CrtImplementationDetails>@@$$FSA_NXZ;

	public static method __m2mep@?AddRef@AtExitLock@<CrtImplementationDetails>@@$$FSAXXZ;

	public static method __m2mep@?RemoveRef@AtExitLock@<CrtImplementationDetails>@@$$FSAXXZ;

	public static method __m2mep@?Enter@AtExitLock@<CrtImplementationDetails>@@$$FSAXXZ;

	public static method __m2mep@?Exit@AtExitLock@<CrtImplementationDetails>@@$$FSAXXZ;

	public static method __m2mep@?__global_lock@?A0xf28fb846@@$$FYA_NXZ;

	public static method __m2mep@?__global_unlock@?A0xf28fb846@@$$FYA_NXZ;

	public static method __m2mep@?__alloc_global_lock@?A0xf28fb846@@$$FYA_NXZ;

	public static method __m2mep@?__dealloc_global_lock@?A0xf28fb846@@$$FYAXXZ;

	public static method __m2mep@?_atexit_helper@@$$J0YMHP6MXXZPEA_KPEAPEAP6MXXZ2@Z;

	public static method __m2mep@?_exit_callback@@$$J0YMXXZ;

	public static method __m2mep@?_initatexit_m@@$$J0YMHXZ;

	public static method __m2mep@?_onexit_m@@$$J0YMP6MHXZP6MHXZ@Z;

	public static method __m2mep@?_atexit_m@@$$J0YMHP6MXXZ@Z;

	public static method __m2mep@?_initatexit_app_domain@@$$J0YMHXZ;

	public static method __m2mep@?_app_exit_callback@@$$J0YMXXZ;

	public static method __m2mep@?_onexit_m_appdomain@@$$J0YMP6MHXZP6MHXZ@Z;

	public static method __m2mep@?_atexit_m_appdomain@@$$J0YMHP6MXXZ@Z;

	public static method __m2mep@?_initterm_e@@$$FYMHPEAP6AHXZ0@Z;

	public static method __m2mep@?_initterm@@$$FYMXPEAP6AXXZ0@Z;

	public static method __m2mep@?Handle@ThisModule@<CrtImplementationDetails>@@$$FCM?AVModuleHandle@System@@XZ;

	public static method __m2mep@?_initterm_m@@$$FYMXPEBQ6MPEBXXZ0@Z;

	public static method __m2mep@??$ResolveMethod@$$A6MPEBXXZ@ThisModule@<CrtImplementationDetails>@@$$FSMP6MPEBXXZP6MPEBXXZ@Z;

	public static method __m2mep@?___CxxCallUnwindDtor@@$$J0YMXP6MXPEAX@Z0@Z;

	public static method __m2mep@?___CxxCallUnwindDelDtor@@$$J0YMXP6MXPEAX@Z0@Z;

	public static method __m2mep@?___CxxCallUnwindVecDtor@@$$J0YMXP6MXPEAX_KHP6MX0@Z@Z01H2@Z;

	internal static byte g_fTracing;

	internal static $ArrayType$$$BY0A@P6AXXZ __xc_z;

	internal static $ArrayType$$$BY0A@P6AXXZ __xc_a;

	internal static $ArrayType$$$BY0A@P6AHXZ __xi_a;

	internal static volatile __enative_startup_state __native_startup_state;

	internal static $ArrayType$$$BY0A@P6AHXZ __xi_z;

	internal unsafe static volatile void* __native_startup_lock;

	internal static volatile uint __native_dllmain_reason;
}
