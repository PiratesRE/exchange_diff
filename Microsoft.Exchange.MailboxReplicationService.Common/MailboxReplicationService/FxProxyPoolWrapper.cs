using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolWrapper : WrapperBase<IFxProxyPool>, IFxProxyPool, IDisposable
	{
		public FxProxyPoolWrapper(IFxProxyPool proxy, CommonUtils.CreateContextDelegate createContext) : base(proxy, createContext)
		{
		}

		IFolderProxy IFxProxyPool.CreateFolder(FolderRec folder)
		{
			IFolderProxy result = null;
			base.CreateContext("IFxProxyPool.CreateFolder", new DataContext[]
			{
				new FolderRecDataContext(folder)
			}).Execute(delegate
			{
				result = this.WrappedObject.CreateFolder(folder);
			}, true);
			return new FxProxyPoolWrapper.FolderProxyWrapper(result, base.CreateContext);
		}

		IFolderProxy IFxProxyPool.GetFolderProxy(byte[] folderId)
		{
			IFolderProxy result = null;
			base.CreateContext("IFxProxyPool.GetFolderProxy", new DataContext[]
			{
				new EntryIDsDataContext(folderId)
			}).Execute(delegate
			{
				result = this.WrappedObject.GetFolderProxy(folderId);
			}, true);
			if (result == null)
			{
				return null;
			}
			return new FxProxyPoolWrapper.FolderProxyWrapper(result, base.CreateContext);
		}

		EntryIdMap<byte[]> IFxProxyPool.GetFolderData()
		{
			EntryIdMap<byte[]> result = null;
			base.CreateContext("IFxProxyPool.GetFolderData", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetFolderData();
			}, true);
			return result;
		}

		void IFxProxyPool.Flush()
		{
			base.CreateContext("IFxProxyPool.Flush", new DataContext[0]).Execute(delegate
			{
				base.WrappedObject.Flush();
			}, true);
		}

		void IFxProxyPool.SetItemProperties(ItemPropertiesBase props)
		{
			base.CreateContext("IFxProxyPool.SetItemProperties", new DataContext[]
			{
				new ItemPropertiesDataContext(props)
			}).Execute(delegate
			{
				this.WrappedObject.SetItemProperties(props);
			}, true);
		}

		List<byte[]> IFxProxyPool.GetUploadedMessageIDs()
		{
			List<byte[]> result = null;
			base.CreateContext("IFxProxyPool.GetUploadedMessageIDs", new DataContext[0]).Execute(delegate
			{
				result = this.WrappedObject.GetUploadedMessageIDs();
			}, true);
			return result;
		}

		private class MapiFxProxyExWrapper : FxProxyWrapper, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public MapiFxProxyExWrapper(IMapiFxProxyEx proxy, CommonUtils.CreateContextDelegate createContext) : base(proxy, createContext)
			{
			}

			void IMapiFxProxyEx.SetProps(PropValueData[] pvda)
			{
				base.CreateContext("IMapiFxProxyEx.SetProps", new DataContext[]
				{
					new PropValuesDataContext(pvda)
				}).Execute(delegate
				{
					((IMapiFxProxyEx)this.WrappedObject).SetProps(pvda);
				}, true);
			}

			void IMapiFxProxyEx.SetItemProperties(ItemPropertiesBase props)
			{
				base.CreateContext("IMapiFxProxyEx.SetItemProperties", new DataContext[]
				{
					new ItemPropertiesDataContext(props)
				}).Execute(delegate
				{
					((IMapiFxProxyEx)this.WrappedObject).SetItemProperties(props);
				}, true);
			}
		}

		private class MessageProxyWrapper : FxProxyPoolWrapper.MapiFxProxyExWrapper, IMessageProxy, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public MessageProxyWrapper(IMessageProxy proxy, CommonUtils.CreateContextDelegate createContext) : base(proxy, createContext)
			{
			}

			void IMessageProxy.SaveChanges()
			{
				base.CreateContext("IMessageProxy.SaveChanges", new DataContext[0]).Execute(delegate
				{
					((IMessageProxy)base.WrappedObject).SaveChanges();
				}, true);
			}

			void IMessageProxy.WriteToMime(byte[] buffer)
			{
				base.CreateContext("IMessageProxy.WriteToMime", new DataContext[0]).Execute(delegate
				{
					((IMessageProxy)this.WrappedObject).WriteToMime(buffer);
				}, true);
			}
		}

		private class FolderProxyWrapper : FxProxyPoolWrapper.MapiFxProxyExWrapper, IFolderProxy, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public FolderProxyWrapper(IFolderProxy proxy, CommonUtils.CreateContextDelegate createContext) : base(proxy, createContext)
			{
			}

			IMessageProxy IFolderProxy.OpenMessage(byte[] entryId)
			{
				IMessageProxy result = null;
				base.CreateContext("IFolderProxy.OpenMessage", new DataContext[]
				{
					new EntryIDsDataContext(entryId)
				}).Execute(delegate
				{
					result = ((IFolderProxy)this.WrappedObject).OpenMessage(entryId);
				}, true);
				if (result == null)
				{
					return null;
				}
				return new FxProxyPoolWrapper.MessageProxyWrapper(result, base.CreateContext);
			}

			IMessageProxy IFolderProxy.CreateMessage(bool isAssociated)
			{
				IMessageProxy result = null;
				base.CreateContext("IFolderProxy.CreateMessage", new DataContext[]
				{
					new SimpleValueDataContext("IsAssociated", isAssociated)
				}).Execute(delegate
				{
					result = ((IFolderProxy)this.WrappedObject).CreateMessage(isAssociated);
				}, true);
				if (result == null)
				{
					return null;
				}
				return new FxProxyPoolWrapper.MessageProxyWrapper(result, base.CreateContext);
			}

			void IFolderProxy.DeleteMessage(byte[] entryId)
			{
				base.CreateContext("IFolderProxy.DeleteMessage", new DataContext[]
				{
					new EntryIDsDataContext(entryId)
				}).Execute(delegate
				{
					((IFolderProxy)this.WrappedObject).DeleteMessage(entryId);
				}, true);
			}
		}
	}
}
