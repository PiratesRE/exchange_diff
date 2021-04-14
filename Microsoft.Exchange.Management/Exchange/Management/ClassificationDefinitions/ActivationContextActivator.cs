using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	public sealed class ActivationContextActivator : IDisposable
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern ActivationContextActivator.ActivationContextHandle CreateActCtx(ref ActivationContextActivator.ACTCTX ActCtx);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ActivateActCtx(ActivationContextActivator.ActivationContextHandle hActCtx, out ActivationContextActivator.ActivationContextCookie lpCookie);

		[DllImport("Kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool DeactivateActCtx(int dwFlags, IntPtr lpCookie);

		[DllImport("Kernel32.dll", SetLastError = true)]
		private static extern void ReleaseActCtx(IntPtr hActCtx);

		private ActivationContextActivator(ActivationContextActivator.ACTCTX actctx)
		{
			this.m_hActivationContext = ActivationContextActivator.CreateActCtx(ref actctx);
			if (this.m_hActivationContext.IsInvalid || !ActivationContextActivator.ActivateActCtx(this.m_hActivationContext, out this.m_cookie))
			{
				throw new ActivationContextActivatorException();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.m_cookie != null)
				{
					this.m_cookie.Dispose();
				}
				if (this.m_hActivationContext != null)
				{
					this.m_hActivationContext.Dispose();
				}
			}
		}

		public static ActivationContextActivator FromExternalManifest(string source, string assemblyDirectory)
		{
			ActivationContextActivator.ACTCTX actctx = default(ActivationContextActivator.ACTCTX);
			actctx.cbSize = Marshal.SizeOf(actctx);
			actctx.lpSource = source;
			actctx.lpAssemblyDirectory = assemblyDirectory;
			actctx.dwFlags = 36U;
			return ActivationContextActivator.FromActivationContext(actctx);
		}

		public static ActivationContextActivator FromInternalManifest(string source, string assemblyDirectory)
		{
			ActivationContextActivator.ACTCTX actctx = default(ActivationContextActivator.ACTCTX);
			actctx.cbSize = Marshal.SizeOf(actctx);
			actctx.lpSource = source;
			actctx.lpAssemblyDirectory = assemblyDirectory;
			actctx.lpResourceName = 2;
			actctx.dwFlags = 44U;
			return ActivationContextActivator.FromActivationContext(actctx);
		}

		internal static ActivationContextActivator FromActivationContext(ActivationContextActivator.ACTCTX actctx)
		{
			return new ActivationContextActivator(actctx);
		}

		private const uint ACTCTX_FLAG_PROCESSOR_ARCHITECTURE_VALID = 1U;

		private const uint ACTCTX_FLAG_LANGID_VALID = 2U;

		private const uint ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 4U;

		private const uint ACTCTX_FLAG_RESOURCE_NAME_VALID = 8U;

		private const uint ACTCTX_FLAG_SET_PROCESS_DEFAULT = 16U;

		private const uint ACTCTX_FLAG_APPLICATION_NAME_VALID = 32U;

		private const uint ACTCTX_FLAG_HMODULE_VALID = 128U;

		private const ushort ISOLATIONAWARE_MANIFEST_RESOURCE_ID = 2;

		private ActivationContextActivator.ActivationContextHandle m_hActivationContext;

		private ActivationContextActivator.ActivationContextCookie m_cookie;

		internal struct ACTCTX
		{
			public int cbSize;

			public uint dwFlags;

			public string lpSource;

			public ushort wProcessorArchitecture;

			public ushort wLangId;

			public string lpAssemblyDirectory;

			public ushort lpResourceName;

			public string lpApplicationName;
		}

		private sealed class ActivationContextHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			private ActivationContextHandle() : base(true)
			{
			}

			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			protected override bool ReleaseHandle()
			{
				ActivationContextActivator.ReleaseActCtx(this.handle);
				return true;
			}
		}

		private sealed class ActivationContextCookie : SafeHandleZeroOrMinusOneIsInvalid
		{
			private ActivationContextCookie() : base(true)
			{
			}

			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			protected override bool ReleaseHandle()
			{
				return ActivationContextActivator.DeactivateActCtx(0, this.handle);
			}
		}
	}
}
