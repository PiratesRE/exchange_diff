using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class FastTransferFolderContentBase : FastTransferObject
	{
		protected FastTransferFolderContentBase(IFolder folder, FastTransferFolderContentBase.IncludeSubObject includeSubObject, bool isTopLevel) : base(isTopLevel)
		{
			Util.ThrowOnNullArgument(folder, "folder");
			this.folder = folder;
			this.includeSubObject = includeSubObject;
		}

		protected override void InternalDispose()
		{
			this.folder.Dispose();
			base.InternalDispose();
		}

		protected IFolder Folder
		{
			get
			{
				return this.folder;
			}
		}

		protected FastTransferFolderContentBase.IncludeSubObject Include
		{
			get
			{
				return this.includeSubObject;
			}
		}

		protected IEnumerator<FastTransferStateMachine?> DownloadProperties(FastTransferDownloadContext context, IPropertyFilter filter)
		{
			yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.folder.PropertyBag, filter)));
			yield break;
		}

		protected IEnumerator<FastTransferStateMachine?> DownloadSubObjects(FastTransferDownloadContext context, bool includeFXDelProp)
		{
			bool distributedFolderInfoSent = false;
			bool isContentAvailable = this.Folder.IsContentAvailable;
			FastTransferFolderContentBase.IncludeSubObject[] subObjectOrder = includeFXDelProp ? FastTransferFolderContentBase.AssociatedMessagesFirst : FastTransferFolderContentBase.NormalMessagesFirst;
			foreach (FastTransferFolderContentBase.IncludeSubObject subObject in subObjectOrder)
			{
				bool isNormalMessages = subObject == FastTransferFolderContentBase.IncludeSubObject.Messages;
				if ((this.Include & subObject) == subObject)
				{
					if (includeFXDelProp)
					{
						yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, isNormalMessages ? FastTransferFolderContentBase.FXDelPropContainerContentsPropertyValue : FastTransferFolderContentBase.FXDelPropFolderAssociatedContentsPropertyValue));
						distributedFolderInfoSent = false;
					}
					if (isContentAvailable)
					{
						yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.DownloadMessages(context, isNormalMessages ? FastTransferMessage.MessageType.Normal : FastTransferMessage.MessageType.Associated, isNormalMessages ? this.Folder.GetContents() : this.Folder.GetAssociatedContents())));
					}
					else if (!distributedFolderInfoSent)
					{
						yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.DownloadDistributedFolderInfo(context)));
						distributedFolderInfoSent = true;
					}
				}
			}
			if ((this.Include & FastTransferFolderContentBase.IncludeSubObject.Subfolders) == FastTransferFolderContentBase.IncludeSubObject.Subfolders)
			{
				if (includeFXDelProp)
				{
					yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, FastTransferFolderContentBase.FXDelPropContainerHierarchyPropertyValue));
				}
				yield return new FastTransferStateMachine?(new FastTransferStateMachine(this.DownloadSubfolders(context)));
			}
			yield break;
		}

		private IEnumerator<FastTransferStateMachine?> DownloadMessages(FastTransferDownloadContext context, FastTransferMessage.MessageType messageType, IEnumerable<IMessage> messageEnumerable)
		{
			foreach (IMessage message in messageEnumerable)
			{
				FastTransferMessage fastTransferMessage = this.CreateDownloadFastTransferMessage(message, messageType);
				yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferMessage));
			}
			yield break;
		}

		private FastTransferMessage CreateDownloadFastTransferMessage(IMessage message, FastTransferMessage.MessageType messageType)
		{
			FastTransferMessage result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<IMessage>(message);
				FastTransferMessage fastTransferMessage = new FastTransferMessage(message, messageType, false);
				disposeGuard.Add<FastTransferMessage>(fastTransferMessage);
				disposeGuard.Success();
				result = fastTransferMessage;
			}
			return result;
		}

		private IEnumerator<FastTransferStateMachine?> DownloadSubfolders(FastTransferDownloadContext context)
		{
			foreach (IFolder subFolder in this.folder.GetFolders())
			{
				IFastTransferProcessor<FastTransferDownloadContext> fastTransferFolderContent = this.CreateDownloadFastTransferFolderContent(subFolder);
				yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferFolderContent));
			}
			yield break;
		}

		private IFastTransferProcessor<FastTransferDownloadContext> CreateDownloadFastTransferFolderContent(IFolder subFolder)
		{
			IFastTransferProcessor<FastTransferDownloadContext> result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<IFolder>(subFolder);
				FastTransferFolderContent fastTransferFolderContent = new FastTransferFolderContent(subFolder, FastTransferFolderContentBase.IncludeSubObject.All, false);
				disposeGuard.Add<FastTransferFolderContent>(fastTransferFolderContent);
				FastTransferDownloadDelimitedObject fastTransferDownloadDelimitedObject = new FastTransferDownloadDelimitedObject(fastTransferFolderContent, PropertyTag.StartSubFld, PropertyTag.EndFolder);
				disposeGuard.Add<FastTransferDownloadDelimitedObject>(fastTransferDownloadDelimitedObject);
				disposeGuard.Success();
				result = fastTransferDownloadDelimitedObject;
			}
			return result;
		}

		private IEnumerator<FastTransferStateMachine?> DownloadDistributedFolderInfo(FastTransferDownloadContext context)
		{
			StoreLongTermId folderLongTermId = this.Folder.GetLongTermId();
			ushort localSiteDatabaseCount;
			string[] replicaDatabases = this.Folder.GetReplicaDatabases(out localSiteDatabaseCount);
			FastTransferDistributedFolderInfo distributedFolderInfo = new FastTransferDistributedFolderInfo(0U, 0U, folderLongTermId, (uint)localSiteDatabaseCount, replicaDatabases);
			yield return new FastTransferStateMachine?(FastTransferPropertyValue.Serialize(context, new PropertyValue(PropertyTag.NewFXFolder, BufferWriter.Serialize(delegate(Writer writer)
			{
				distributedFolderInfo.Serialize(writer);
			}))));
			context.DataInterface.ForceBufferFull();
			yield break;
		}

		protected IEnumerator<FastTransferStateMachine?> UploadProperties(FastTransferUploadContext context)
		{
			if (base.IsTopLevel)
			{
				context.SetEndOfBufferAction(delegate
				{
					this.Folder.Save();
				});
			}
			try
			{
				yield return new FastTransferStateMachine?(context.CreateStateMachine(new FastTransferPropList(this.folder.PropertyBag)));
			}
			finally
			{
				if (base.IsTopLevel)
				{
					context.SetEndOfBufferAction(null);
				}
			}
			this.Folder.Save();
			yield break;
		}

		protected IEnumerator<FastTransferStateMachine?> UploadMessages(FastTransferUploadContext context)
		{
			while (!context.NoMoreData)
			{
				PropertyTag propertyTag;
				if (context.DataInterface.TryPeekMarker(out propertyTag))
				{
					if (!(propertyTag == PropertyTag.StartMessage) && !(propertyTag == PropertyTag.StartFAIMsg))
					{
						break;
					}
					FastTransferMessage.MessageType messageType = (propertyTag == PropertyTag.StartFAIMsg) ? FastTransferMessage.MessageType.Associated : FastTransferMessage.MessageType.Normal;
					FastTransferMessage fastTransferMessage = this.CreateUploadFastTransferMessage(messageType);
					yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferMessage));
				}
				else
				{
					if (!propertyTag.IsMetaProperty)
					{
						break;
					}
					yield return new FastTransferStateMachine?(FastTransferPropertyValue.SkipPropertyIfExists(context, propertyTag));
				}
			}
			yield break;
		}

		private FastTransferMessage CreateUploadFastTransferMessage(FastTransferMessage.MessageType messageType)
		{
			FastTransferMessage result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IMessage message = this.folder.CreateMessage(messageType == FastTransferMessage.MessageType.Associated);
				disposeGuard.Add<IMessage>(message);
				FastTransferMessage fastTransferMessage = new FastTransferMessage(message, messageType, false);
				disposeGuard.Add<FastTransferMessage>(fastTransferMessage);
				disposeGuard.Success();
				result = fastTransferMessage;
			}
			return result;
		}

		protected IEnumerator<FastTransferStateMachine?> UploadSubfolders(FastTransferUploadContext context)
		{
			while (!context.NoMoreData)
			{
				PropertyTag propertyTag;
				if (context.DataInterface.TryPeekMarker(out propertyTag))
				{
					if (!(propertyTag == PropertyTag.StartSubFld))
					{
						break;
					}
					IFastTransferProcessor<FastTransferUploadContext> fastTransferFolderContent = this.CreateUploadFastTransferFolderContent();
					yield return new FastTransferStateMachine?(context.CreateStateMachine(fastTransferFolderContent));
				}
				else
				{
					if (!propertyTag.IsMetaProperty)
					{
						break;
					}
					yield return new FastTransferStateMachine?(FastTransferPropertyValue.SkipPropertyIfExists(context, propertyTag));
				}
			}
			yield break;
		}

		private IFastTransferProcessor<FastTransferUploadContext> CreateUploadFastTransferFolderContent()
		{
			IFastTransferProcessor<FastTransferUploadContext> result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				IFolder disposable = this.folder.CreateFolder();
				disposeGuard.Add<IFolder>(disposable);
				FastTransferFolderContent fastTransferFolderContent = new FastTransferFolderContent(disposable, FastTransferFolderContentBase.IncludeSubObject.All, false);
				disposeGuard.Add<FastTransferFolderContent>(fastTransferFolderContent);
				FastTransferUploadDelimitedObject fastTransferUploadDelimitedObject = new FastTransferUploadDelimitedObject(fastTransferFolderContent, PropertyTag.StartSubFld, PropertyTag.EndFolder);
				disposeGuard.Add<FastTransferUploadDelimitedObject>(fastTransferUploadDelimitedObject);
				disposeGuard.Success();
				result = fastTransferUploadDelimitedObject;
			}
			return result;
		}

		private static readonly PropertyValue FXDelPropContainerContentsPropertyValue = new PropertyValue(PropertyTag.FXDelProp, (int)PropertyTag.ContainerContents);

		private static readonly PropertyValue FXDelPropContainerHierarchyPropertyValue = new PropertyValue(PropertyTag.FXDelProp, (int)PropertyTag.ContainerHierarchy);

		private static readonly PropertyValue FXDelPropFolderAssociatedContentsPropertyValue = new PropertyValue(PropertyTag.FXDelProp, (int)PropertyTag.FolderAssociatedContents);

		private static readonly FastTransferFolderContentBase.IncludeSubObject[] AssociatedMessagesFirst = new FastTransferFolderContentBase.IncludeSubObject[]
		{
			FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages,
			FastTransferFolderContentBase.IncludeSubObject.Messages
		};

		private static readonly FastTransferFolderContentBase.IncludeSubObject[] NormalMessagesFirst = new FastTransferFolderContentBase.IncludeSubObject[]
		{
			FastTransferFolderContentBase.IncludeSubObject.Messages,
			FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages
		};

		private readonly IFolder folder;

		private readonly FastTransferFolderContentBase.IncludeSubObject includeSubObject;

		[Flags]
		public enum IncludeSubObject
		{
			None = 0,
			Messages = 1,
			AssociatedMessages = 2,
			Subfolders = 4,
			All = 7
		}
	}
}
