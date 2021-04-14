using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExImportHierarchyChangesHandle : SafeExInterfaceHandle, IExImportHierarchyChanges, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExImportHierarchyChangesHandle()
		{
		}

		internal SafeExImportHierarchyChangesHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExImportHierarchyChangesHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExImportHierarchyChangesHandle>(this);
		}

		public int Config(IStream iStream, int ulFlags)
		{
			return SafeExImportHierarchyChangesHandle.IExchangeImportHierarchyChanges_Config(this.handle, iStream, ulFlags);
		}

		public int UpdateState(IStream iStream)
		{
			return SafeExImportHierarchyChangesHandle.IExchangeImportHierarchyChanges_UpdateState(this.handle, iStream);
		}

		public unsafe int ImportFolderChange(int cpvalChanges, SPropValue* ppvalChanges)
		{
			return SafeExImportHierarchyChangesHandle.IExchangeImportHierarchyChanges_ImportFolderChange(this.handle, cpvalChanges, ppvalChanges);
		}

		public unsafe int ImportFolderDeletion(int ulFlags, _SBinaryArray* lpSrcEntryList)
		{
			return SafeExImportHierarchyChangesHandle.IExchangeImportHierarchyChanges_ImportFolderDeletion(this.handle, ulFlags, lpSrcEntryList);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeImportHierarchyChanges_Config(IntPtr iExchangeImportHierarchyChanges, IStream iStream, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeImportHierarchyChanges_UpdateState(IntPtr iExchangeImportHierarchyChanges, IStream iStream);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeImportHierarchyChanges_ImportFolderChange(IntPtr iExchangeImportHierarchyChanges, int cpvalChanges, SPropValue* ppvalChanges);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeImportHierarchyChanges_ImportFolderDeletion(IntPtr iExchangeImportHierarchyChanges, int ulFlags, _SBinaryArray* lpSrcEntryList);
	}
}
