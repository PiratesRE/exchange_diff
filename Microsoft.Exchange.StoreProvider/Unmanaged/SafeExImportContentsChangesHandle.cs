using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExImportContentsChangesHandle : SafeExInterfaceHandle, IExImportContentsChanges, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExImportContentsChangesHandle()
		{
		}

		internal SafeExImportContentsChangesHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExImportContentsChangesHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExImportContentsChangesHandle>(this);
		}

		public int Config(IStream iStream, int ulFlags)
		{
			return SafeExImportContentsChangesHandle.IExchangeImportContentsChanges_Config(this.handle, iStream, ulFlags);
		}

		public int UpdateState(IStream iStream)
		{
			return SafeExImportContentsChangesHandle.IExchangeImportContentsChanges_UpdateState(this.handle, iStream);
		}

		public unsafe int ImportMessageChange(int cpvalChanges, SPropValue* ppvalChanges, int ulFlags, out SafeExMapiMessageHandle iMessage)
		{
			return SafeExImportContentsChangesHandle.IExchangeImportContentsChanges_ImportMessageChange(this.handle, cpvalChanges, ppvalChanges, ulFlags, out iMessage);
		}

		public unsafe int ImportMessageDeletion(int ulFlags, _SBinaryArray* lpSrcEntryList)
		{
			return SafeExImportContentsChangesHandle.IExchangeImportContentsChanges_ImportMessageDeletion(this.handle, ulFlags, lpSrcEntryList);
		}

		public unsafe int ImportPerUserReadStateChange(int cElements, _ReadState* lpReadState)
		{
			return SafeExImportContentsChangesHandle.IExchangeImportContentsChanges_ImportPerUserReadStateChange(this.handle, cElements, lpReadState);
		}

		public int ImportMessageMove(int cbSourceKeySrcFolder, byte[] pbSourceKeySrcFolder, int cbSourceKeySrcMessage, byte[] pbSourceKeySrcMessage, int cbPCLMessage, byte[] pbPCLMessage, int cbSourceKeyDestMessage, byte[] pbSourceKeyDestMessage, int cbChangeNumDestMessage, byte[] pbChangeNumDestMessage)
		{
			return SafeExImportContentsChangesHandle.IExchangeImportContentsChanges_ImportMessageMove(this.handle, cbSourceKeySrcFolder, pbSourceKeySrcFolder, cbSourceKeySrcMessage, pbSourceKeySrcMessage, cbPCLMessage, pbPCLMessage, cbSourceKeyDestMessage, pbSourceKeyDestMessage, cbChangeNumDestMessage, pbChangeNumDestMessage);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeImportContentsChanges_Config(IntPtr iExchangeImportContentsChanges, IStream iStream, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeImportContentsChanges_UpdateState(IntPtr iExchangeImportContentsChanges, IStream iStream);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeImportContentsChanges_ImportMessageChange(IntPtr iExchangeImportContentsChanges, int cpvalChanges, SPropValue* ppvalChanges, int ulFlags, out SafeExMapiMessageHandle iMessage);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeImportContentsChanges_ImportMessageDeletion(IntPtr iExchangeImportContentsChanges, int ulFlags, _SBinaryArray* lpSrcEntryList);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeImportContentsChanges_ImportPerUserReadStateChange(IntPtr iExchangeImportContentsChanges, int cElements, _ReadState* lpReadState);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeImportContentsChanges_ImportMessageMove(IntPtr iExchangeImportContentsChanges, int cbSourceKeySrcFolder, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbSourceKeySrcFolder, int cbSourceKeySrcMessage, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbSourceKeySrcMessage, int cbPCLMessage, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbPCLMessage, int cbSourceKeyDestMessage, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbSourceKeyDestMessage, int cbChangeNumDestMessage, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbChangeNumDestMessage);
	}
}
