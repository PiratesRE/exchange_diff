using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolBudgetWrapper : BudgetWrapperBase<IFxProxyPool>, IFxProxyPool, IDisposable
	{
		public FxProxyPoolBudgetWrapper(IFxProxyPool proxy, bool ownsObject, Func<IDisposable> createCostHandleDelegate, Action<uint> chargeDelegate) : base(proxy, ownsObject, createCostHandleDelegate, chargeDelegate)
		{
		}

		IFolderProxy IFxProxyPool.CreateFolder(FolderRec folder)
		{
			IFolderProxy proxy = null;
			using (base.CreateCostHandle())
			{
				proxy = base.WrappedObject.CreateFolder(folder);
			}
			return new FxProxyPoolBudgetWrapper.FolderProxyBudgetWrapper(proxy, true, base.CreateCostHandle, base.Charge);
		}

		IFolderProxy IFxProxyPool.GetFolderProxy(byte[] folderId)
		{
			IFolderProxy folderProxy = null;
			using (base.CreateCostHandle())
			{
				folderProxy = base.WrappedObject.GetFolderProxy(folderId);
			}
			if (folderProxy == null)
			{
				return null;
			}
			return new FxProxyPoolBudgetWrapper.FolderProxyBudgetWrapper(folderProxy, true, base.CreateCostHandle, base.Charge);
		}

		EntryIdMap<byte[]> IFxProxyPool.GetFolderData()
		{
			EntryIdMap<byte[]> result = null;
			using (base.CreateCostHandle())
			{
				result = base.WrappedObject.GetFolderData();
			}
			return result;
		}

		void IFxProxyPool.Flush()
		{
			using (base.CreateCostHandle())
			{
				base.WrappedObject.Flush();
			}
		}

		void IFxProxyPool.SetItemProperties(ItemPropertiesBase props)
		{
			using (base.CreateCostHandle())
			{
				base.WrappedObject.SetItemProperties(props);
			}
		}

		List<byte[]> IFxProxyPool.GetUploadedMessageIDs()
		{
			List<byte[]> uploadedMessageIDs;
			using (base.CreateCostHandle())
			{
				uploadedMessageIDs = base.WrappedObject.GetUploadedMessageIDs();
			}
			return uploadedMessageIDs;
		}

		private class MapiFxProxyBudgetExWrapper : FxProxyBudgetWrapper, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public MapiFxProxyBudgetExWrapper(IMapiFxProxyEx proxy, bool ownsObject, Func<IDisposable> createCostHandleDelegate, Action<uint> chargeDelegate) : base(proxy, ownsObject, createCostHandleDelegate, chargeDelegate)
			{
			}

			void IMapiFxProxyEx.SetProps(PropValueData[] pvda)
			{
				using (base.CreateCostHandle())
				{
					((IMapiFxProxyEx)base.WrappedObject).SetProps(pvda);
				}
			}

			void IMapiFxProxyEx.SetItemProperties(ItemPropertiesBase props)
			{
				using (base.CreateCostHandle())
				{
					((IMapiFxProxyEx)base.WrappedObject).SetItemProperties(props);
				}
			}
		}

		private class MessageProxyBudgetWrapper : FxProxyPoolBudgetWrapper.MapiFxProxyBudgetExWrapper, IMessageProxy, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public MessageProxyBudgetWrapper(IMessageProxy proxy, bool ownsObject, Func<IDisposable> createCostHandleDelegate, Action<uint> chargeDelegate) : base(proxy, ownsObject, createCostHandleDelegate, chargeDelegate)
			{
			}

			void IMessageProxy.SaveChanges()
			{
				using (base.CreateCostHandle())
				{
					((IMessageProxy)base.WrappedObject).SaveChanges();
				}
			}

			void IMessageProxy.WriteToMime(byte[] buffer)
			{
				using (base.CreateCostHandle())
				{
					((IMessageProxy)base.WrappedObject).WriteToMime(buffer);
				}
				if (buffer != null)
				{
					base.Charge((uint)buffer.Length);
				}
			}
		}

		private class FolderProxyBudgetWrapper : FxProxyPoolBudgetWrapper.MapiFxProxyBudgetExWrapper, IFolderProxy, IMapiFxProxyEx, IMapiFxProxy, IDisposeTrackable, IDisposable
		{
			public FolderProxyBudgetWrapper(IFolderProxy proxy, bool ownsObject, Func<IDisposable> createCostHandleDelegate, Action<uint> chargeDelegate) : base(proxy, ownsObject, createCostHandleDelegate, chargeDelegate)
			{
			}

			IMessageProxy IFolderProxy.OpenMessage(byte[] entryId)
			{
				IMessageProxy messageProxy = null;
				using (base.CreateCostHandle())
				{
					messageProxy = ((IFolderProxy)base.WrappedObject).OpenMessage(entryId);
				}
				if (messageProxy == null)
				{
					return null;
				}
				return new FxProxyPoolBudgetWrapper.MessageProxyBudgetWrapper(messageProxy, true, base.CreateCostHandle, base.Charge);
			}

			IMessageProxy IFolderProxy.CreateMessage(bool isAssociated)
			{
				IMessageProxy messageProxy = null;
				using (base.CreateCostHandle())
				{
					messageProxy = ((IFolderProxy)base.WrappedObject).CreateMessage(isAssociated);
				}
				if (messageProxy == null)
				{
					return null;
				}
				return new FxProxyPoolBudgetWrapper.MessageProxyBudgetWrapper(messageProxy, true, base.CreateCostHandle, base.Charge);
			}

			void IFolderProxy.DeleteMessage(byte[] entryId)
			{
				using (base.CreateCostHandle())
				{
					((IFolderProxy)base.WrappedObject).DeleteMessage(entryId);
				}
			}
		}
	}
}
