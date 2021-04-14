using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FxProxyWrapper : IExchangeFastTransferEx, IMAPIProp
	{
		internal FxProxyWrapper(IMapiFxCollector fxCollector)
		{
			this.fxCollector = fxCollector;
			this.lastException = null;
		}

		public static MapiProp CreateFxProxyWrapper(IMapiFxProxy iFxProxy, MapiStore mapiStore, out FxProxyWrapper wrapper)
		{
			IMapiFxCollector mapiFxCollector = new FxCollectorSerializer(iFxProxy);
			Guid objectType = mapiFxCollector.GetObjectType();
			if (objectType == InterfaceIds.IMAPIFolderGuid)
			{
				wrapper = new FxProxyFolderWrapper(mapiFxCollector);
				return new MapiFolder(null, (IMAPIFolder)wrapper, mapiStore);
			}
			if (objectType == InterfaceIds.IMsgStoreGuid)
			{
				wrapper = new FxProxyStoreWrapper(mapiFxCollector);
				return new MapiStore(null, (IMsgStore)wrapper, mapiStore.ExRpcConnection, mapiStore.ApplicationId);
			}
			if (objectType == InterfaceIds.IMessageGuid)
			{
				wrapper = new FxProxyMessageWrapper(mapiFxCollector);
				return new MapiMessage(null, (IMessage)wrapper, mapiStore);
			}
			if (objectType == InterfaceIds.IAttachGuid)
			{
				wrapper = new FxProxyAttachWrapper(mapiFxCollector);
				return new MapiAttach(null, (IAttach)wrapper, mapiStore);
			}
			wrapper = null;
			throw MapiExceptionHelper.ArgumentException("iFxProxy", string.Format("Unsupported iFxProxy objectType {0}", objectType));
		}

		public Exception LastException
		{
			get
			{
				return this.lastException;
			}
		}

		public int Config(int ulFlags, int ulTransferMethod)
		{
			if (this.lastException != null)
			{
				return -2147467259;
			}
			int result = 0;
			try
			{
				this.fxCollector.Config(ulFlags, ulTransferMethod);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public int TransferBuffer(int cbData, byte[] data, out int cbProcessed)
		{
			int result = 0;
			cbProcessed = 0;
			if (this.lastException != null)
			{
				return -2147467259;
			}
			try
			{
				this.fxCollector.TransferBuffer(data);
				cbProcessed = cbData;
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public bool IsInterfaceOk(int ulTransferMethod, ref Guid refiid, IntPtr lpPropTagArray, int ulFlags)
		{
			if (this.lastException != null)
			{
				return false;
			}
			bool result;
			try
			{
				this.fxCollector.IsInterfaceOk(ulTransferMethod, refiid, ulFlags);
				result = true;
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = false;
			}
			return result;
		}

		public int GetObjectType(out Guid objType)
		{
			int result = 0;
			objType = Guid.Empty;
			if (this.lastException != null)
			{
				return -2147467259;
			}
			try
			{
				objType = this.fxCollector.GetObjectType();
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public unsafe int GetServerVersion(int cbBufSize, byte* buffer, out int cbBuffer)
		{
			int result = 0;
			cbBuffer = 0;
			if (this.lastException != null)
			{
				return -2147467259;
			}
			try
			{
				byte[] serverVersion = this.fxCollector.GetServerVersion();
				cbBuffer = serverVersion.Length;
				if (cbBuffer > cbBufSize)
				{
					return -2147024774;
				}
				Marshal.Copy(serverVersion, 0, (IntPtr)((void*)buffer), cbBuffer);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public unsafe int TellPartnerVersion(int cbBuffer, byte* buffer)
		{
			if (this.lastException != null)
			{
				return -2147467259;
			}
			int result = 0;
			try
			{
				byte[] array = new byte[cbBuffer];
				Marshal.Copy((IntPtr)((void*)buffer), array, 0, cbBuffer);
				this.fxCollector.TellPartnerVersion(array);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public int GetLastLowLevelError(out int lowLevelError)
		{
			lowLevelError = -2147467259;
			return 0;
		}

		public bool IsPrivateLogon()
		{
			if (this.lastException != null)
			{
				return false;
			}
			bool result = false;
			try
			{
				result = this.fxCollector.IsPrivateLogon();
			}
			catch (Exception ex)
			{
				this.lastException = ex;
			}
			return result;
		}

		public int StartMdbEventsImport()
		{
			if (this.lastException != null)
			{
				return -2147467259;
			}
			int result = 0;
			try
			{
				this.fxCollector.StartMdbEventsImport();
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public int FinishMdbEventsImport(bool success)
		{
			if (this.lastException != null)
			{
				return -2147467259;
			}
			int result = 0;
			try
			{
				this.fxCollector.FinishMdbEventsImport(success);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public int AddMdbEvents(int cbRequest, byte[] pbRequest)
		{
			if (this.lastException != null)
			{
				return -2147467259;
			}
			int result = 0;
			try
			{
				this.fxCollector.AddMdbEvents(pbRequest);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public int SetWatermarks(int cWMs, MDBEVENTWMRAW[] WMs)
		{
			if (this.lastException != null)
			{
				return -2147467259;
			}
			int result = 0;
			try
			{
				this.fxCollector.SetWatermarks(WMs);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public int SetReceiveFolder(int cbEntryId, byte[] entryId, string messageClass)
		{
			if (this.lastException != null)
			{
				return -2147467259;
			}
			int result = 0;
			try
			{
				this.fxCollector.SetReceiveFolder(entryId, messageClass);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public unsafe int SetPerUser(ref MapiLtidNative ltid, Guid* pguidReplica, int lib, byte[] pb, int cb, bool fLast)
		{
			if (this.lastException != null)
			{
				return -2147467259;
			}
			int result = 0;
			try
			{
				Guid pguidReplica2 = (pguidReplica == null) ? Guid.Empty : (*pguidReplica);
				this.fxCollector.SetPerUser(ltid, pguidReplica2, lib, pb, fLast);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public unsafe int SetProps(int cValues, SPropValue* lpPropArray)
		{
			if (this.lastException != null)
			{
				return -2147467259;
			}
			int result = 0;
			try
			{
				PropValue[] array = new PropValue[cValues];
				for (int i = 0; i < cValues; i++)
				{
					array[i] = new PropValue(lpPropArray + i);
				}
				this.fxCollector.SetProps(array);
			}
			catch (Exception ex)
			{
				this.lastException = ex;
				result = -2147467259;
			}
			return result;
		}

		public unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError)
		{
			lpMapiError = (IntPtr)((UIntPtr)0);
			return -2147221246;
		}

		public int SaveChanges(int ulFlags)
		{
			return -2147221246;
		}

		public int GetProps(PropTag[] lpPropTagArray, int ulFlags, out int lpcValues, out SafeExLinkedMemoryHandle lppPropArray)
		{
			lpcValues = 0;
			lppPropArray = null;
			return -2147221246;
		}

		public int GetPropList(int ulFlags, out SafeExLinkedMemoryHandle propList)
		{
			propList = null;
			return -2147221246;
		}

		public int OpenProperty(int propTag, Guid lpInterface, int interfaceOptions, int ulFlags, out object obj)
		{
			if (propTag == 1714356237 && lpInterface == InterfaceIds.IExchangeFastTransferEx)
			{
				obj = this;
				return 0;
			}
			obj = null;
			return -2147467262;
		}

		public unsafe int SetProps(int cValues, SPropValue* lpPropArray, out SafeExLinkedMemoryHandle lppProblems)
		{
			lppProblems = null;
			return -2147221246;
		}

		public int DeleteProps(PropTag[] lpPropTagArray, out SafeExLinkedMemoryHandle lppProblems)
		{
			lppProblems = null;
			return -2147221246;
		}

		public int CopyTo(int ciidExclude, Guid[] rgiidExclude, PropTag[] lpExcludeProps, IntPtr ulUiParam, IntPtr lpProgress, Guid lpInterface, IMAPIProp lpDestObj, int ulFlags, out SafeExLinkedMemoryHandle lppProblems)
		{
			lppProblems = null;
			return -2147221246;
		}

		public int CopyProps(PropTag[] lpIncludeProps, IntPtr ulUIParam, IntPtr lpProgress, Guid lpInterface, IMAPIProp lpDestObj, int ulFlags, out SafeExLinkedMemoryHandle lppProblems)
		{
			lppProblems = null;
			return -2147221246;
		}

		public unsafe int GetNamesFromIDs(int** lppPropTagArray, Guid* lpGuid, int ulFlags, ref int cPropNames, out SafeExLinkedMemoryHandle lppNames)
		{
			lppNames = null;
			return -2147221246;
		}

		public unsafe int GetIDsFromNames(int cPropNames, SNameId** lppPropNames, int ulFlags, out SafeExLinkedMemoryHandle lpPropIDs)
		{
			lpPropIDs = null;
			return -2147221246;
		}

		private IMapiFxCollector fxCollector;

		private Exception lastException;
	}
}
