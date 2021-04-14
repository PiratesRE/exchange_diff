using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolFxCallbackWrapper : DisposableWrapper<IFxProxyPool>, IFxProxyPool, IDisposable
	{
		public FxProxyPoolFxCallbackWrapper(IFxProxyPool destProxies, bool ownsObject, Action<TimeSpan> updateDuration) : base(destProxies, ownsObject)
		{
			this.updateDuration = updateDuration;
		}

		IFolderProxy IFxProxyPool.CreateFolder(FolderRec folder)
		{
			IFolderProxy proxy = base.WrappedObject.CreateFolder(folder);
			return new FxProxyPoolFxCallbackWrapper.FolderProxyFxCallbackWrapper(proxy, true, this.updateDuration);
		}

		IFolderProxy IFxProxyPool.GetFolderProxy(byte[] folderId)
		{
			IFolderProxy folderProxy = base.WrappedObject.GetFolderProxy(folderId);
			if (folderProxy == null)
			{
				return null;
			}
			return new FxProxyPoolFxCallbackWrapper.FolderProxyFxCallbackWrapper(folderProxy, true, this.updateDuration);
		}

		EntryIdMap<byte[]> IFxProxyPool.GetFolderData()
		{
			return base.WrappedObject.GetFolderData();
		}

		void IFxProxyPool.Flush()
		{
			base.WrappedObject.Flush();
		}

		void IFxProxyPool.SetItemProperties(ItemPropertiesBase props)
		{
			base.WrappedObject.SetItemProperties(props);
		}

		List<byte[]> IFxProxyPool.GetUploadedMessageIDs()
		{
			return base.WrappedObject.GetUploadedMessageIDs();
		}

		private Action<TimeSpan> updateDuration;

		private class FolderProxyFxCallbackWrapper : DisposableWrapper<IFolderProxy>, IFolderProxy, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public FolderProxyFxCallbackWrapper(IFolderProxy proxy, bool ownsObject, Action<TimeSpan> updateDuration) : base(proxy, ownsObject)
			{
				this.updateDuration = updateDuration;
			}

			IMessageProxy IFolderProxy.OpenMessage(byte[] entryId)
			{
				return base.WrappedObject.OpenMessage(entryId);
			}

			IMessageProxy IFolderProxy.CreateMessage(bool isAssociated)
			{
				return base.WrappedObject.CreateMessage(isAssociated);
			}

			void IFolderProxy.DeleteMessage(byte[] entryId)
			{
				base.WrappedObject.DeleteMessage(entryId);
			}

			void IMapiFxProxyEx.SetProps(PropValueData[] pvda)
			{
				base.WrappedObject.SetProps(pvda);
			}

			void IMapiFxProxyEx.SetItemProperties(ItemPropertiesBase props)
			{
				base.WrappedObject.SetItemProperties(props);
			}

			byte[] IMapiFxProxy.GetObjectData()
			{
				return base.WrappedObject.GetObjectData();
			}

			void IMapiFxProxy.ProcessRequest(FxOpcodes opCode, byte[] request)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					base.WrappedObject.ProcessRequest(opCode, request);
				}
				finally
				{
					this.updateDuration(stopwatch.Elapsed);
					stopwatch.Stop();
				}
			}

			private Action<TimeSpan> updateDuration;
		}
	}
}
