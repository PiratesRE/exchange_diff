using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RopHandler : BaseObject, IRopHandler, IDisposable
	{
		internal RopHandler(ConnectionHandler connectionHandler) : this(connectionHandler, new DatabaseLocationProvider())
		{
		}

		internal RopHandler(ConnectionHandler connectionHandler, IDatabaseLocationProvider databaseLocationProvider)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.connectionHandler = connectionHandler;
				this.databaseLocationProvider = databaseLocationProvider;
				if (!StoreSession.UseRPCContextPool)
				{
					StoreSession.UseRPCContextPool = true;
				}
				if (!StoreSession.UseRPCContextPoolResiliency)
				{
					StoreSession.UseRPCContextPoolResiliency = true;
				}
				disposeGuard.Success();
			}
		}

		internal IConnection Connection
		{
			get
			{
				return this.connectionHandler.Connection;
			}
		}

		internal NotificationHandler NotificationHandler
		{
			get
			{
				return (NotificationHandler)this.connectionHandler.NotificationHandler;
			}
		}

		public RopResult Abort(IServerObject serverObject, AbortResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 18610, "Abort");
		}

		public RopResult AbortSubmit(IServerObject serverObject, StoreId folderId, StoreId messageId, AbortSubmitResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(folderId, "folderId");
				Util.ThrowOnNullArgument(messageId, "messageId");
				Logon logon = RopHandler.Downcast<Logon>(serverObject);
				logon.AbortSubmit(folderId, messageId);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult AddressTypes(IServerObject serverObject, AddressTypesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				return resultFactory.CreateSuccessfulResult(privateLogon.GetSupportedAddressTypes());
			}, resultFactory);
		}

		public RopResult CloneStream(IServerObject serverObject, CloneStreamResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 18640, "CloneStream");
		}

		public RopResult CollapseRow(IServerObject serverObject, StoreId categoryId, CollapseRowResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				int collapsedRowCount = view.CollapseRow(categoryId);
				return resultFactory.CreateSuccessfulResult(collapsedRowCount);
			}, resultFactory);
		}

		public RopResult CommitStream(IServerObject serverObject, CommitStreamResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Stream stream = RopHandler.Downcast<Stream>(serverObject);
				stream.Commit();
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult CopyFolder(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, bool recurse, StoreId folderId, string folderName, CopyFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(sourceServerObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(sourceServerObject);
				Folder folder2 = RopHandler.Downcast<Folder>(destinationServerObject);
				if (folder.Session is PublicFolderSession || folder2.Session is PublicFolderSession)
				{
					return this.RopNotImplemented(resultFactory, 40210, "For public folders, CopyFolder operation is currently not supported");
				}
				folder.MoveCopyFolder(folder2, folderName, recurse, folderId, reportProgress, true, reportProgress ? resultFactory.CreateProgressToken() : null);
				return RopHandler.CreateProgressRopResult(folder, reportProgress, resultFactory);
			}, resultFactory);
		}

		public RopResult CopyProperties(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] propertyTags, CopyPropertiesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(sourceServerObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(sourceServerObject);
				PropertyServerObject destinationPropertyServerObject = RopHandler.Downcast<PropertyServerObject>(destinationServerObject);
				PropertyProblem[] propertyProblems = propertyServerObject.CopyProperties(destinationPropertyServerObject, reportProgress, copyPropertiesFlags, propertyTags);
				return resultFactory.CreateSuccessfulResult(propertyProblems);
			}, resultFactory);
		}

		public RopResult CopyTo(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludedPropertyTags, CopyToResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(sourceServerObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(sourceServerObject);
				PropertyServerObject destinationPropertyServerObject = RopHandler.Downcast<PropertyServerObject>(destinationServerObject);
				PropertyProblem[] propertyProblems = propertyServerObject.CopyTo(destinationPropertyServerObject, reportProgress, copySubObjects, copyPropertiesFlags, excludedPropertyTags);
				return resultFactory.CreateSuccessfulResult(propertyProblems);
			}, resultFactory);
		}

		public RopResult CopyToExtended(IServerObject sourceServerObject, IServerObject destinationServerObject, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludePropertyTags, CopyToExtendedResultFactory resultFactory)
		{
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		public RopResult CopyToStream(IServerObject sourceServerObject, IServerObject destinationServerObject, ulong bytesToCopy, CopyToStreamResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(sourceServerObject, delegate
			{
				Stream stream = RopHandler.Downcast<Stream>(sourceServerObject);
				Stream destinationStream = RopHandler.Downcast<Stream>(destinationServerObject);
				ulong num = stream.CopyToStream(destinationStream, bytesToCopy);
				return resultFactory.CreateSuccessfulResult(num, num);
			}, resultFactory);
		}

		public RopResult CreateAttachment(IServerObject serverObject, CreateAttachmentResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					Attachment attachment = message.CreateAttachment();
					disposeGuard.Add<Attachment>(attachment);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(attachment, (uint)attachment.CoreAttachment.AttachmentNumber);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult CreateBookmark(IServerObject serverObject, CreateBookmarkResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				FolderBasedView folderBasedView = RopHandler.Downcast<FolderBasedView>(serverObject);
				return resultFactory.CreateSuccessfulResult(folderBasedView.CreateBookmark());
			}, resultFactory);
		}

		public RopResult CreateFolder(IServerObject serverObject, FolderType folderType, CreateFolderFlags flags, string displayName, string folderComment, StoreLongTermId? longTermId, CreateFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					ReplicaServerInfo? replicaInfo;
					bool existed;
					bool hasRules;
					StoreId folderId;
					Folder folder2 = folder.CreateFolder(folderType, flags, displayName, folderComment, longTermId, out replicaInfo, out existed, out hasRules, out folderId);
					disposeGuard.Add<Folder>(folder2);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(folder2, folderId, existed, hasRules, replicaInfo);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult CreateMessage(IServerObject serverObject, ushort codePageId, StoreId folderId, bool createAssociated, CreateMessageResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(serverObject, "serverObject");
				Util.ThrowOnNullArgument(folderId, "folderId");
				SessionServerObject sessionServerObject = RopHandler.Downcast<SessionServerObject>(serverObject);
				StoreObjectId parentFolderId = sessionServerObject.Session.IdConverter.CreateFolderId(folderId);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					CoreItem coreItem = CoreItem.Create(sessionServerObject.Session, parentFolderId, createAssociated ? CreateMessageType.Associated : CreateMessageType.Normal);
					disposeGuard.Add<CoreItem>(coreItem);
					Encoding string8Encoding;
					if (!String8Encodings.TryGetEncoding((int)codePageId, serverObject.String8Encoding, out string8Encoding))
					{
						string message = string.Format("Cannot resolve code page: {0}", codePageId);
						throw new RopExecutionException(message, ErrorCode.UnknownCodepage);
					}
					Message message2 = new Message(coreItem, RopHandler.Downcast<PropertyServerObject>(serverObject).LogonObject, string8Encoding);
					disposeGuard.Add<Message>(message2);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(message2, null);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult CreateMessageExtended(IServerObject serverObject, ushort codePageId, StoreId folderId, CreateMessageExtendedFlags createFlags, CreateMessageExtendedResultFactory resultFactory)
		{
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		public RopResult DeleteAttachment(IServerObject serverObject, uint attachmentNumber, DeleteAttachmentResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				if (message.DeleteAttachment(attachmentNumber))
				{
					return resultFactory.CreateSuccessfulResult();
				}
				return resultFactory.CreateFailedResult((ErrorCode)2147746063U);
			}, resultFactory);
		}

		public RopResult DeleteFolder(IServerObject serverObject, DeleteFolderFlags deleteFolderFlags, StoreId folderId, DeleteFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				bool isPartiallyCompleted;
				folder.DeleteFolder(deleteFolderFlags, folderId, out isPartiallyCompleted);
				return resultFactory.CreateSuccessfulResult(isPartiallyCompleted);
			}, resultFactory);
		}

		public RopResult DeleteMessages(IServerObject serverObject, bool reportProgress, bool isOkToSendNonReadNotification, StoreId[] messageIds, DeleteMessagesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				folder.DeleteMessages(messageIds, DeleteItemFlags.SoftDelete, isOkToSendNonReadNotification, reportProgress, reportProgress ? resultFactory.CreateProgressToken() : null);
				return RopHandler.CreateProgressRopResult(folder, reportProgress, resultFactory);
			}, resultFactory);
		}

		public RopResult DeleteProperties(IServerObject serverObject, PropertyTag[] propertyTags, DeletePropertiesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				return resultFactory.CreateSuccessfulResult(propertyServerObject.DeleteProperties(propertyTags, true));
			}, resultFactory);
		}

		public RopResult DeletePropertiesNoReplicate(IServerObject serverObject, PropertyTag[] propertyTags, DeletePropertiesNoReplicateResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				return resultFactory.CreateSuccessfulResult(propertyServerObject.DeleteProperties(propertyTags, false));
			}, resultFactory);
		}

		public RopResult EchoString(string inParameter, EchoStringResultFactory resultFactory)
		{
			base.CheckDisposed();
			Util.ThrowOnNullArgument(resultFactory, "resultFactory");
			return resultFactory.CreateSuccessfulResult(string.Empty, inParameter);
		}

		public RopResult EchoInt(int inParameter, EchoIntResultFactory resultFactory)
		{
			base.CheckDisposed();
			Util.ThrowOnNullArgument(resultFactory, "resultFactory");
			return resultFactory.CreateSuccessfulResult(0, inParameter);
		}

		public RopResult EchoBinary(byte[] inParameter, EchoBinaryResultFactory resultFactory)
		{
			base.CheckDisposed();
			Util.ThrowOnNullArgument(resultFactory, "resultFactory");
			return resultFactory.CreateSuccessfulResult(0, inParameter);
		}

		public RopResult EmptyFolder(IServerObject serverObject, bool reportProgress, EmptyFolderFlags emptyFolderFlags, EmptyFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				if (folder.Session is PublicFolderSession)
				{
					throw new RopExecutionException("For public folders, EmptyFolder operation is not allowed", (ErrorCode)2147746050U);
				}
				folder.EmptyFolder(reportProgress, reportProgress ? resultFactory.CreateProgressToken() : null, emptyFolderFlags, false);
				return RopHandler.CreateProgressRopResult(folder, reportProgress, resultFactory);
			}, resultFactory);
		}

		public RopResult ExpandRow(IServerObject serverObject, short maxRows, StoreId categoryId, ExpandRowResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				RopResult result;
				using (RowCollector rowCollector = resultFactory.CreateRowCollector())
				{
					int expandedRowCount = view.ExpandRow(maxRows, categoryId, rowCollector);
					result = resultFactory.CreateSuccessfulResult(expandedRowCount, rowCollector);
				}
				return result;
			}, resultFactory);
		}

		public RopResult FastTransferDestinationCopyOperationConfigure(IServerObject serverObject, FastTransferCopyOperation copyOperation, FastTransferCopyPropertiesFlag flags, FastTransferDestinationCopyOperationConfigureResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					FastTransferUpload fastTransferUpload = propertyServerObject.FastTransferDestinationCopyOperationConfigure(copyOperation, flags);
					disposeGuard.Add<FastTransferUpload>(fastTransferUpload);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferUpload);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult FastTransferDestinationPutBuffer(IServerObject serverObject, ArraySegment<byte>[] dataChunks, FastTransferDestinationPutBufferResultFactory resultFactory)
		{
			FastTransferUpload fxUploadContext = null;
			return this.GenericRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(dataChunks, "dataChunks");
				FastTransferUpload fastTransferUpload = RopHandler.Downcast<FastTransferUpload>(serverObject);
				fxUploadContext = fastTransferUpload;
				ushort usedBufferSize = 0;
				foreach (ArraySegment<byte> buffer in dataChunks)
				{
					fastTransferUpload.PutNextBuffer(buffer);
					usedBufferSize = (ushort)buffer.Count;
				}
				return resultFactory.CreateSuccessfulResult((ushort)fastTransferUpload.Progress, (ushort)fastTransferUpload.Steps, fastTransferUpload.IsMovingMailbox, usedBufferSize);
			}, resultFactory, delegate(IResultFactory inResultFactory, ErrorCode errorCode, Exception exception)
			{
				if (fxUploadContext == null)
				{
					return RopHandler.CreateFailedResultWithException(resultFactory.CreateStandardFailedResult(errorCode), exception);
				}
				ushort usedBufferSize = 0;
				return RopHandler.CreateFailedResultWithException(resultFactory.CreateFailedResult(errorCode, (ushort)fxUploadContext.Progress, (ushort)fxUploadContext.Steps, fxUploadContext.IsMovingMailbox, usedBufferSize), exception);
			});
		}

		public RopResult FastTransferDestinationPutBufferExtended(IServerObject serverObject, ArraySegment<byte>[] dataChunks, FastTransferDestinationPutBufferExtendedResultFactory resultFactory)
		{
			FastTransferUpload fxUploadContext = null;
			return this.GenericRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(dataChunks, "dataChunks");
				FastTransferUpload fastTransferUpload = RopHandler.Downcast<FastTransferUpload>(serverObject);
				fxUploadContext = fastTransferUpload;
				ushort usedBufferSize = 0;
				foreach (ArraySegment<byte> buffer in dataChunks)
				{
					fastTransferUpload.PutNextBuffer(buffer);
					usedBufferSize = (ushort)buffer.Count;
				}
				return resultFactory.CreateSuccessfulResult(fastTransferUpload.Progress, fastTransferUpload.Steps, fastTransferUpload.IsMovingMailbox, usedBufferSize);
			}, resultFactory, delegate(IResultFactory inResultFactory, ErrorCode errorCode, Exception exception)
			{
				if (fxUploadContext == null)
				{
					return RopHandler.CreateFailedResultWithException(resultFactory.CreateStandardFailedResult(errorCode), exception);
				}
				ushort usedBufferSize = 0;
				return RopHandler.CreateFailedResultWithException(resultFactory.CreateFailedResult(errorCode, fxUploadContext.Progress, fxUploadContext.Steps, fxUploadContext.IsMovingMailbox, usedBufferSize), exception);
			});
		}

		public RopResult FastTransferGetIncrementalState(IServerObject serverObject, FastTransferGetIncrementalStateResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					IFastTransferProcessor<FastTransferDownloadContext> fastTransferProcessor = RopHandler.Downcast<IIcsStateCheckpoint>(serverObject).CreateIcsStateCheckpointFastTransferObject();
					disposeGuard.Add<IFastTransferProcessor<FastTransferDownloadContext>>(fastTransferProcessor);
					FastTransferDownload fastTransferDownload = new FastTransferDownload(FastTransferSendOption.UseMAPI, fastTransferProcessor, 1U, PropertyFilterFactory.IncludeAllFactory, RopHandler.GetLogonObject(serverObject));
					disposeGuard.Add<FastTransferDownload>(fastTransferDownload);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownload);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult FastTransferSourceCopyFolder(IServerObject serverObject, FastTransferCopyFolderFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					FastTransferDownload fastTransferDownload = folder.FastTransferSourceCopyFolder(flags, sendOptions);
					disposeGuard.Add<FastTransferDownload>(fastTransferDownload);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownload);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult FastTransferSourceCopyMessages(IServerObject serverObject, StoreId[] messageIds, FastTransferCopyMessagesFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyMessagesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					FastTransferDownload fastTransferDownload = folder.FastTransferSourceCopyMessages(messageIds, flags, sendOptions);
					disposeGuard.Add<FastTransferDownload>(fastTransferDownload);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownload);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult FastTransferSourceCopyProperties(IServerObject serverObject, byte level, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] propertyTags, FastTransferSourceCopyPropertiesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					FastTransferDownload fastTransferDownload = propertyServerObject.FastTransferSourceCopyProperties(level, flags, sendOptions, propertyTags);
					disposeGuard.Add<FastTransferDownload>(fastTransferDownload);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownload);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult FastTransferSourceCopyTo(IServerObject serverObject, byte level, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludedPropertyTags, FastTransferSourceCopyToResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					FastTransferDownload fastTransferDownload = propertyServerObject.FastTransferSourceCopyTo(level, flags, sendOptions, excludedPropertyTags);
					disposeGuard.Add<FastTransferDownload>(fastTransferDownload);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownload);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult FastTransferSourceGetBuffer(IServerObject serverObject, ushort bufferSize, FastTransferSourceGetBufferResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Logon logonObject = RopHandler.GetLogonObject(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					if (serverObject is IcsDownloadPassThru)
					{
						IcsDownloadPassThru icsDownloadPassThru = RopHandler.Downcast<IcsDownloadPassThru>(serverObject);
						if (!icsDownloadPassThru.IsAvailableInCache((int)bufferSize))
						{
							disposeGuard.Add<IDisposable>(logonObject.GetFastTransferActivityLock(RopId.FastTransferSourceGetBuffer));
						}
						ArraySegment<byte> outputBuffer = resultFactory.GetOutputBuffer();
						uint num = 0U;
						uint num2 = 0U;
						FastTransferState state = FastTransferState.Partial;
						int nextBuffer = icsDownloadPassThru.GetNextBuffer(outputBuffer, out num, out num2, out state);
						result = resultFactory.CreateSuccessfulResult(state, (ushort)num2, (ushort)num, false, nextBuffer);
					}
					else
					{
						FastTransferDownload fastTransferDownload = RopHandler.Downcast<FastTransferDownload>(serverObject);
						disposeGuard.Add<IDisposable>(logonObject.GetFastTransferActivityLock(RopId.FastTransferSourceGetBuffer));
						ArraySegment<byte> outputBuffer2 = resultFactory.GetOutputBuffer();
						int nextBuffer2 = fastTransferDownload.GetNextBuffer(outputBuffer2);
						result = resultFactory.CreateSuccessfulResult(fastTransferDownload.State, (ushort)fastTransferDownload.Progress, (ushort)fastTransferDownload.Steps, fastTransferDownload.IsMovingMailbox, nextBuffer2);
					}
				}
				return result;
			}, resultFactory);
		}

		public RopResult FastTransferSourceGetBufferExtended(IServerObject serverObject, ushort bufferSize, FastTransferSourceGetBufferExtendedResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Logon logonObject = RopHandler.GetLogonObject(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					if (serverObject is IcsDownloadPassThru)
					{
						IcsDownloadPassThru icsDownloadPassThru = RopHandler.Downcast<IcsDownloadPassThru>(serverObject);
						if (!icsDownloadPassThru.IsAvailableInCache((int)bufferSize))
						{
							disposeGuard.Add<IDisposable>(logonObject.GetFastTransferActivityLock(RopId.FastTransferSourceGetBufferExtended));
						}
						ArraySegment<byte> outputBuffer = resultFactory.GetOutputBuffer();
						uint totalStepCount = 0U;
						uint progressCount = 0U;
						FastTransferState state = FastTransferState.Partial;
						int nextBuffer = icsDownloadPassThru.GetNextBuffer(outputBuffer, out totalStepCount, out progressCount, out state);
						result = resultFactory.CreateSuccessfulResult(state, progressCount, totalStepCount, false, nextBuffer);
					}
					else
					{
						FastTransferDownload fastTransferDownload = RopHandler.Downcast<FastTransferDownload>(serverObject);
						disposeGuard.Add<IDisposable>(logonObject.GetFastTransferActivityLock(RopId.FastTransferSourceGetBufferExtended));
						ArraySegment<byte> outputBuffer2 = resultFactory.GetOutputBuffer();
						int nextBuffer2 = fastTransferDownload.GetNextBuffer(outputBuffer2);
						result = resultFactory.CreateSuccessfulResult(fastTransferDownload.State, fastTransferDownload.Progress, fastTransferDownload.Steps, fastTransferDownload.IsMovingMailbox, nextBuffer2);
					}
				}
				return result;
			}, resultFactory);
		}

		public RopResult FindRow(IServerObject serverObject, FindRowFlags flags, Restriction restriction, BookmarkOrigin bookmarkOrigin, byte[] bookmark, FindRowResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				RopResult result;
				using (RowCollector rowCollector = resultFactory.CreateRowCollector())
				{
					bool flag = view.TryFindRow(flags, restriction, bookmarkOrigin, bookmark, rowCollector);
					if (flag)
					{
						result = resultFactory.CreateSuccessfulResult(false, rowCollector);
					}
					else
					{
						result = resultFactory.CreateFailedResult((ErrorCode)2147746063U);
					}
				}
				return result;
			}, resultFactory);
		}

		public RopResult FlushRecipients(IServerObject serverObject, PropertyTag[] extraPropertyTags, RecipientRow[] recipientRows, FlushRecipientsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				message.ModifyRecipients(extraPropertyTags, recipientRows, delegate(RecipientTranslationException ex)
				{
					RopHandlerHelper.TraceRopResult(RopId.FlushRecipients, ex, ErrorCode.None);
				});
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult FreeBookmark(IServerObject serverObject, byte[] bookmark, FreeBookmarkResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(bookmark, "bookmark");
				FolderBasedView folderBasedView = RopHandler.Downcast<FolderBasedView>(serverObject);
				folderBasedView.FreeBookmark(bookmark);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult GetPermissionsTable(IServerObject serverObject, TableFlags tableFlags, GetPermissionsTableResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					PermissionsView permissionsView = folder.CreatePermissionsView(folder.LogonObject, tableFlags, this.NotificationHandler, resultFactory.ServerObjectHandle);
					disposeGuard.Add<PermissionsView>(permissionsView);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(permissionsView);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult GetPerUserGuid(IServerObject serverObject, StoreLongTermId publicFolderLongTermId, GetPerUserGuidResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				Guid perUserGuid = privateLogon.GetPerUserGuid(publicFolderLongTermId);
				return resultFactory.CreateSuccessfulResult(perUserGuid);
			}, resultFactory);
		}

		public RopResult GetPerUserLongTermIds(IServerObject serverObject, Guid databaseGuid, GetPerUserLongTermIdsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				StoreLongTermId[] perUserLongTermIds = privateLogon.GetPerUserLongTermIds(databaseGuid);
				return resultFactory.CreateSuccessfulResult(perUserLongTermIds);
			}, resultFactory);
		}

		public RopResult GetAllPerUserLongTermIds(IServerObject serverObject, StoreLongTermId startId, GetAllPerUserLongTermIdsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				RopResult result;
				using (PerUserDataCollector perUserDataCollector = resultFactory.CreatePerUserDataCollector())
				{
					bool allPerUserLongTermIds = privateLogon.GetAllPerUserLongTermIds(startId, perUserDataCollector);
					result = resultFactory.CreateSuccessfulResult(perUserDataCollector, allPerUserLongTermIds);
				}
				return result;
			}, resultFactory);
		}

		public RopResult GetAttachmentTable(IServerObject serverObject, TableFlags tableFlags, GetAttachmentTableResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					AttachmentView attachmentTable = message.GetAttachmentTable(message.LogonObject, tableFlags, this.NotificationHandler, resultFactory.ServerObjectHandle);
					disposeGuard.Add<AttachmentView>(attachmentTable);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(attachmentTable);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult GetCollapseState(IServerObject serverObject, StoreId rowId, uint rowInstanceNumber, GetCollapseStateResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				ContentsView contentsView = RopHandler.Downcast<ContentsView>(serverObject);
				return resultFactory.CreateSuccessfulResult(contentsView.GetCollapseState(rowId, rowInstanceNumber));
			}, resultFactory);
		}

		public RopResult GetContentsTable(IServerObject serverObject, TableFlags tableFlags, GetContentsTableResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				int rowCount = 0;
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					ContentsView contentsView = folder.CreateContentsView(folder.LogonObject, tableFlags, this.NotificationHandler, resultFactory.ServerObjectHandle, out rowCount);
					disposeGuard.Add<ContentsView>(contentsView);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(contentsView, rowCount);
					disposeGuard.Success();
					if (Utils.IsTeamMailbox(folder.Session) && Configuration.ServiceConfiguration.TMPublishEnabled)
					{
						((MailboxSession)folder.Session).TryToSyncSiteMailboxNow();
					}
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult GetContentsTableExtended(IServerObject serverObject, ExtendedTableFlags extendedTableFlagsEx, GetContentsTableExtendedResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 54480, "GetContentsTableExtended");
		}

		public RopResult GetEffectiveRights(IServerObject serverObject, byte[] addressBookId, StoreId folderId, GetEffectiveRightsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Logon logon = RopHandler.Downcast<Logon>(serverObject);
				return resultFactory.CreateSuccessfulResult(logon.GetEffectiveRights(addressBookId, folderId));
			}, resultFactory);
		}

		public RopResult GetHierarchyTable(IServerObject serverObject, TableFlags tableFlags, GetHierarchyTableResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				int rowCount = 0;
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					HierarchyView hierarchyView = folder.CreateHierarchyView(folder.LogonObject, tableFlags, this.NotificationHandler, resultFactory.ServerObjectHandle, out rowCount);
					disposeGuard.Add<HierarchyView>(hierarchyView);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(hierarchyView, rowCount);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult GetIdsFromNames(IServerObject serverObject, GetIdsFromNamesFlags flags, NamedProperty[] namedProperties, GetIdsFromNamesResultFactory resultFactory)
		{
			Util.ThrowOnNullArgument(serverObject, "serverObject");
			Util.ThrowOnNullArgument(namedProperties, "namedProperties");
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				GetIdsFromNamesFlags getIdsFromNamesFlags = GetIdsFromNamesFlags.Create;
				int num = 0;
				if ((byte)(flags & ~getIdsFromNamesFlags) != 0)
				{
					return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
				}
				foreach (NamedProperty namedProperty in namedProperties)
				{
					NamedPropertyKind kind = namedProperty.Kind;
					switch (kind)
					{
					case NamedPropertyKind.Id:
						if (!namedProperty.IsMapiNamespace)
						{
							num++;
						}
						break;
					case NamedPropertyKind.String:
						if (namedProperty.IsMapiNamespace)
						{
							return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
						}
						num++;
						break;
					default:
						if (kind != NamedPropertyKind.Null)
						{
							throw new InvalidOperationException(string.Format("NamedProperty contains invalid kind [NamedPropertyKind = {0}].", namedProperty.Kind));
						}
						break;
					}
				}
				SessionServerObject sessionServerObject = RopHandler.Downcast<SessionServerObject>(serverObject);
				if (namedProperties.Length == 0)
				{
					return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
				}
				ushort[] array = null;
				if (num > 0)
				{
					List<NamedPropertyDefinition.NamedPropertyKey> list = new List<NamedPropertyDefinition.NamedPropertyKey>(num);
					for (int j = 0; j < namedProperties.Length; j++)
					{
						NamedPropertyDefinition.NamedPropertyKey namedPropertyKey = namedProperties[j].ToNamedPropertyKey();
						if (namedPropertyKey != null)
						{
							list.Add(namedPropertyKey);
						}
					}
					array = NamedPropConverter.GetIdsFromNames(sessionServerObject.Session, (byte)(flags & GetIdsFromNamesFlags.Create) == 2, list);
					if (array == null || array.Length != num)
					{
						throw new InvalidOperationException("GetIdsFromNames didn't return the correct number of mappings.");
					}
				}
				PropertyId[] array2 = new PropertyId[namedProperties.Length];
				int k = 0;
				int num2 = 0;
				while (k < namedProperties.Length)
				{
					NamedPropertyKind kind2 = namedProperties[k].Kind;
					switch (kind2)
					{
					case NamedPropertyKind.Id:
						if (namedProperties[k].IsMapiNamespace)
						{
							array2[k] = (PropertyId)namedProperties[k].Id;
						}
						else
						{
							array2[k] = (PropertyId)array[num2++];
						}
						break;
					case NamedPropertyKind.String:
						array2[k] = (PropertyId)array[num2++];
						break;
					default:
						if (kind2 == NamedPropertyKind.Null)
						{
							array2[k] = PropertyId.Invalid;
						}
						break;
					}
					k++;
				}
				return resultFactory.CreateSuccessfulResult(array2);
			}, resultFactory);
		}

		public RopResult GetLocalReplicationIds(IServerObject serverObject, uint idCount, GetLocalReplicationIdsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Logon logon = RopHandler.Downcast<Logon>(serverObject);
				Guid guid;
				byte[] globCount;
				((CoreMailboxObject)logon.Session.Mailbox.CoreObject).GetLocalReplicationIds(idCount, out guid, out globCount);
				return resultFactory.CreateSuccessfulResult(new StoreLongTermId(guid, globCount));
			}, resultFactory);
		}

		public RopResult GetNamesFromIDs(IServerObject serverObject, PropertyId[] propertyIds, GetNamesFromIDsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				SessionServerObject sessionServerObject = RopHandler.Downcast<SessionServerObject>(serverObject);
				if (propertyIds == null)
				{
					return resultFactory.CreateSuccessfulResult(Array<NamedProperty>.Empty);
				}
				NamedProperty[] array = new NamedProperty[propertyIds.Length];
				List<ushort> list = new List<ushort>(propertyIds.Length);
				foreach (PropertyId propertyId in propertyIds)
				{
					if (PropertyTag.IsNamedPropertyId(propertyId))
					{
						list.Add((ushort)propertyId);
					}
				}
				NamedPropertyDefinition.NamedPropertyKey[] namesFromIds = NamedPropConverter.GetNamesFromIds(sessionServerObject.Session, list);
				int j = 0;
				int num = 0;
				while (j < propertyIds.Length)
				{
					if (PropertyTag.IsNamedPropertyId(propertyIds[j]))
					{
						array[j] = (namesFromIds[num++].ToNamedProperty() ?? new NamedProperty());
					}
					else
					{
						array[j] = NamedProperty.CreateMAPINamedProperty(propertyIds[j]);
					}
					j++;
				}
				return resultFactory.CreateSuccessfulResult(array);
			}, resultFactory);
		}

		public RopResult GetOptionsData(IServerObject serverObject, string addressType, bool wantWin32, GetOptionsDataResultFactory resultFactory)
		{
			Util.ThrowOnNullArgument(serverObject, "serverObject");
			Util.ThrowOnNullArgument(addressType, "addressType");
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				if (!wantWin32)
				{
					throw new RopExecutionException("wantWin32 must be true since we do not support 16 bit client any more.", (ErrorCode)2147746050U);
				}
				RoutingTypeOptionsData optionsData = privateLogon.MailboxSession.GetOptionsData(addressType);
				string helpFileName;
				byte[] helpFileData;
				if (optionsData.HelpFileName != null && optionsData.HelpFileData != null)
				{
					helpFileName = Encoding.ASCII.GetString(optionsData.HelpFileName);
					helpFileData = optionsData.HelpFileData;
				}
				else
				{
					helpFileName = null;
					helpFileData = Array<byte>.Empty;
				}
				OptionsInfo optionsInfo = new OptionsInfo(optionsData.MessageData, optionsData.RecipientData, helpFileName);
				return resultFactory.CreateSuccessfulResult(optionsInfo.GetBytes(), helpFileData, helpFileName);
			}, resultFactory);
		}

		public RopResult GetOwningServers(IServerObject serverObject, StoreId folderId, GetOwningServersResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PublicLogon publicLogon = RopHandler.Downcast<PublicLogon>(serverObject);
				ReplicaServerInfo value = Microsoft.Exchange.RpcClientAccess.Handler.PublicLogon.GetReplicaServerInfo(publicLogon.PublicFolderSession, folderId, false).Value;
				return resultFactory.CreateSuccessfulResult(value);
			}, resultFactory);
		}

		public RopResult GetMessageStatus(IServerObject serverObject, StoreId messageId, GetMessageStatusResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				MessageStatusFlags messageStatus = folder.GetMessageStatus(messageId);
				return resultFactory.CreateSuccessfulResult(messageStatus);
			}, resultFactory);
		}

		public RopResult GetPropertiesAll(IServerObject serverObject, ushort streamLimit, GetPropertiesFlags flags, GetPropertiesAllResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				PropertyValue[] propertiesAll = propertyServerObject.GetPropertiesAll(streamLimit, flags);
				return resultFactory.CreateSuccessfulResult(propertiesAll);
			}, resultFactory);
		}

		public RopResult GetPropertiesSpecific(IServerObject serverObject, ushort streamLimit, GetPropertiesFlags flags, PropertyTag[] propertyTags, GetPropertiesSpecificResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				PropertyValue[] propertiesSpecific = propertyServerObject.GetPropertiesSpecific(streamLimit, flags, propertyTags);
				return resultFactory.CreateSuccessfulResult(propertyTags, propertiesSpecific);
			}, resultFactory);
		}

		public RopResult GetPropertyList(IServerObject serverObject, GetPropertyListResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				PropertyTag[] propertyList = propertyServerObject.GetPropertyList();
				return resultFactory.CreateSuccessfulResult(propertyList);
			}, resultFactory);
		}

		public RopResult GetReceiveFolder(IServerObject serverObject, string messageClass, GetReceiveFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(messageClass, "messageClass");
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				string messageClass2;
				StoreId receiveFolderId = privateLogon.GetReceiveFolderId(messageClass, out messageClass2);
				return resultFactory.CreateSuccessfulResult(receiveFolderId, messageClass2);
			}, resultFactory);
		}

		public RopResult GetReceiveFolderTable(IServerObject serverObject, GetReceiveFolderTableResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				PropertyValue[][] receiveFolderInfo = privateLogon.GetReceiveFolderInfo();
				return resultFactory.CreateSuccessfulResult(receiveFolderInfo);
			}, resultFactory);
		}

		public RopResult GetRulesTable(IServerObject serverObject, TableFlags tableFlags, GetRulesTableResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				if (folder.Session is PublicFolderSession && !folder.CoreFolder.IsContentAvailable())
				{
					throw new RopExecutionException("For public folders, GetRulesTable operation is only allowed against content session", ErrorCode.NoReplicaHere);
				}
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					RulesView rulesView = folder.CreateRulesView(folder.LogonObject, tableFlags, this.NotificationHandler, resultFactory.ServerObjectHandle);
					disposeGuard.Add<RulesView>(rulesView);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(rulesView);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult GetSearchCriteria(IServerObject serverObject, GetSearchCriteriaFlags flags, GetSearchCriteriaResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				Restriction restriction;
				StoreId[] folderIds;
				SearchState searchState;
				folder.GetSearchCriteria(flags, out restriction, out folderIds, out searchState);
				return resultFactory.CreateSuccessfulResult(restriction, folderIds, searchState);
			}, resultFactory);
		}

		public RopResult GetStatus(IServerObject serverObject, GetStatusResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 12439, "GetStatus");
		}

		public RopResult GetStoreState(IServerObject serverObject, GetStoreStateResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 18670, "GetStoreState");
		}

		public RopResult GetStreamSize(IServerObject serverObject, GetStreamSizeResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Stream stream = RopHandler.Downcast<Stream>(serverObject);
				return resultFactory.CreateSuccessfulResult(stream.GetSize());
			}, resultFactory);
		}

		public RopResult HardDeleteMessages(IServerObject serverObject, bool reportProgress, bool isOkToSendNonReadNotification, StoreId[] messageIds, HardDeleteMessagesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				folder.DeleteMessages(messageIds, DeleteItemFlags.HardDelete, isOkToSendNonReadNotification, reportProgress, reportProgress ? resultFactory.CreateProgressToken() : null);
				return RopHandler.CreateProgressRopResult(folder, reportProgress, resultFactory);
			}, resultFactory);
		}

		public RopResult HardEmptyFolder(IServerObject serverObject, bool reportProgress, EmptyFolderFlags emptyFolderFlags, HardEmptyFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				if (folder.Session is PublicFolderSession)
				{
					throw new RopExecutionException("For public folders, HardEmptyFolder operation is not allowed", (ErrorCode)2147746050U);
				}
				folder.EmptyFolder(reportProgress, reportProgress ? resultFactory.CreateProgressToken() : null, emptyFolderFlags, true);
				return RopHandler.CreateProgressRopResult(folder, reportProgress, resultFactory);
			}, resultFactory);
		}

		public RopResult IdFromLongTermId(IServerObject serverObject, StoreLongTermId longTermId, IdFromLongTermIdResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				SessionServerObject sessionServerObject = RopHandler.Downcast<SessionServerObject>(serverObject);
				long idFromLongTermId = sessionServerObject.Session.IdConverter.GetIdFromLongTermId(longTermId.ToBytes());
				return resultFactory.CreateSuccessfulResult(new StoreId(idFromLongTermId));
			}, resultFactory);
		}

		public RopResult ImportDelete(IServerObject serverObject, ImportDeleteFlags importDeleteFlags, PropertyValue[] deleteChanges, ImportDeleteResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				IcsUpload icsUpload = RopHandler.Downcast<IcsUpload>(serverObject);
				ErrorCode errorCode = icsUpload.ImportDelete(importDeleteFlags, deleteChanges);
				if (errorCode == ErrorCode.None)
				{
					return resultFactory.CreateSuccessfulResult();
				}
				return resultFactory.CreateFailedResult(errorCode);
			}, resultFactory);
		}

		public RopResult ImportHierarchyChange(IServerObject serverObject, PropertyValue[] hierarchyPropertyValues, PropertyValue[] folderPropertyValues, ImportHierarchyChangeResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				IcsFolderUpload icsFolderUpload = RopHandler.Downcast<IcsFolderUpload>(serverObject);
				StoreId folderId = icsFolderUpload.ImportFolderChange(hierarchyPropertyValues, folderPropertyValues);
				return resultFactory.CreateSuccessfulResult(folderId);
			}, resultFactory);
		}

		public RopResult ImportMessageChange(IServerObject serverObject, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangeResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				IcsMessageUpload icsMessageUpload = RopHandler.Downcast<IcsMessageUpload>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					Message message = null;
					StoreId messageId;
					ErrorCode errorCode = icsMessageUpload.ImportMessageChange(importMessageChangeFlags, propertyValues, out messageId, out message);
					disposeGuard.Add<Message>(message);
					if (errorCode != ErrorCode.None)
					{
						result = resultFactory.CreateFailedResult(errorCode);
					}
					else
					{
						RopResult ropResult = resultFactory.CreateSuccessfulResult(message, messageId);
						disposeGuard.Success();
						result = ropResult;
					}
				}
				return result;
			}, resultFactory);
		}

		public RopResult ImportMessageChangePartial(IServerObject serverObject, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangePartialResultFactory resultFactory)
		{
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		public RopResult ImportMessageMove(IServerObject serverObject, byte[] sourceFolder, byte[] sourceMessage, byte[] predecessorChangeList, byte[] destinationMessage, byte[] destinationChangeNumber, ImportMessageMoveResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				IcsMessageUpload icsMessageUpload = RopHandler.Downcast<IcsMessageUpload>(serverObject);
				StoreId messageId;
				ErrorCode errorCode = icsMessageUpload.ImportMessageMove(sourceFolder, sourceMessage, predecessorChangeList, destinationMessage, destinationChangeNumber, out messageId);
				if (errorCode != ErrorCode.None)
				{
					return resultFactory.CreateFailedResult(errorCode);
				}
				return resultFactory.CreateSuccessfulResult(messageId);
			}, resultFactory);
		}

		public RopResult ImportReads(IServerObject serverObject, MessageReadState[] messageReadStates, ImportReadsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				IcsMessageUpload icsMessageUpload = RopHandler.Downcast<IcsMessageUpload>(serverObject);
				icsMessageUpload.ImportReads(messageReadStates);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult IncrementalConfig(IServerObject serverObject, IncrementalConfigOption configOptions, FastTransferSendOption sendOptions, SyncFlag syncFlags, Restriction restriction, SyncExtraFlag extraFlags, PropertyTag[] propertyTags, StoreId[] messageIds, IncrementalConfigResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					if (configOptions == IncrementalConfigOption.Contents && (ushort)(syncFlags & SyncFlag.BestBody) != 0 && (ushort)(syncFlags & SyncFlag.OnlySpecifiedProps) == 0)
					{
						BodyHelper.RemoveBodyProperties(ref propertyTags);
					}
					IcsDownloadPassThru icsDownloadPassThru = folder.CreateIcsDownloadPassThru(configOptions, sendOptions, syncFlags, restriction, extraFlags, propertyTags, messageIds);
					disposeGuard.Add<IcsDownloadPassThru>(icsDownloadPassThru);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(icsDownloadPassThru);
					disposeGuard.Success();
					if (Utils.IsTeamMailbox(folder.Session) && Configuration.ServiceConfiguration.TMPublishEnabled)
					{
						((MailboxSession)folder.Session).TryToSyncSiteMailboxNow();
					}
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult LockRegionStream(IServerObject serverObject, ulong offset, ulong regionLength, LockTypeFlag lockType, LockRegionStreamResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 18675, "LockRegionStream");
		}

		public RopResult Logon(LogonFlags logonFlags, OpenFlags openFlags, StoreState storeState, LogonExtendedRequestFlags extendedFlags, MailboxId? mailboxId, LocaleInfo? localeInfo, string applicationId, AuthenticationContext authenticationContext, byte[] tenantHint, LogonResultFactory resultFactory)
		{
			RopResult ropResult = this.StandardRopHandlerMethod(null, delegate
			{
				if (authenticationContext != null && (byte)(logonFlags & LogonFlags.Private) == 1)
				{
					throw new RopExecutionException("Private Logons done using Constrained delegation are not supported.", (ErrorCode)2147746050U);
				}
				if (authenticationContext != null && (this.Connection.ConnectionFlags & ConnectionFlags.UseDelegatedAuthPrivilege) != ConnectionFlags.UseDelegatedAuthPrivilege)
				{
					throw new RopExecutionException("authenticationContext should not be specified when Constrainted Delegation is not requested on the Connection.", (ErrorCode)2147746065U);
				}
				if (authenticationContext == null && (this.Connection.ConnectionFlags & ConnectionFlags.UseDelegatedAuthPrivilege) == ConnectionFlags.UseDelegatedAuthPrivilege)
				{
					throw new RopExecutionException("authenticationContext needs to be specified when Constrainted Delegation is requested on the Connection.", (ErrorCode)2147746065U);
				}
				if (tenantHint != null)
				{
					throw new RopExecutionException("Client should not pass TenantHint.", (ErrorCode)2147746050U);
				}
				if ((extendedFlags & LogonExtendedRequestFlags.UnifiedLogon) == LogonExtendedRequestFlags.UnifiedLogon)
				{
					throw new RopExecutionException("Client should not attempt create unified logon.", (ErrorCode)2147746050U);
				}
				if (mailboxId != null && mailboxId.Value.IsLegacyDn && this.connectionHandler.Connection.IsWebService)
				{
					try
					{
						string mailboxLegacyDn = LegacyDnHelper.ConvertToLegacyDn(mailboxId.Value.LegacyDn, this.connectionHandler.Connection.OrganizationId, false);
						mailboxId = new MailboxId?(new MailboxId(mailboxLegacyDn));
					}
					catch (ObjectNotFoundException)
					{
						throw new RopExecutionException(string.Format("Unable to convert mailboxId to LegacyDN '{0}'", mailboxId.Value.LegacyDn), ErrorCode.UnknownUser);
					}
				}
				Microsoft.Exchange.RpcClientAccess.Handler.Logon.ValidateLogonSettings(logonFlags, openFlags, mailboxId);
				if (string.IsNullOrEmpty(applicationId) && this.Connection.ClientInformation.Mode != ClientMode.ExchangeServer)
				{
					applicationId = "Client=MSExchangeRPC";
				}
				if ((byte)(logonFlags & LogonFlags.Private) == 1)
				{
					return this.PrivateLogon(logonFlags, openFlags, storeState, extendedFlags, mailboxId, localeInfo, applicationId, authenticationContext, resultFactory);
				}
				return this.PublicLogon(logonFlags, openFlags, storeState, extendedFlags, mailboxId, localeInfo, applicationId, authenticationContext, resultFactory);
			}, resultFactory);
			if (Configuration.ServiceConfiguration.EnableSmartConnectionTearDown && ropResult.ErrorCode != ErrorCode.None && ropResult.ErrorCode != ErrorCode.WrongServer && ropResult.ErrorCode != ErrorCode.UnknownUser && ropResult.ErrorCode != ErrorCode.LoginPerm && mailboxId != null && mailboxId.Value.IsLegacyDn && LegacyDN.StringComparer.Equals(mailboxId.Value.LegacyDn, this.Connection.ActAsLegacyDN))
			{
				this.connectionHandler.Connection.BackoffConnect(new RopExecutionException("The previously reported logon failure for a connection indicates the reason.", ropResult.ErrorCode));
				throw new SessionDeadException("The primary owner logon has failed. Dropping a connection.", ropResult.Exception);
			}
			SuccessfulLogonResult successfulLogonResult = ropResult as SuccessfulLogonResult;
			if (successfulLogonResult != null)
			{
				Logon logon = (Logon)successfulLogonResult.ReturnObject;
				ProtocolLog.LogLogonSuccess((int)logon.LogonId);
				this.connectionHandler.Connection.TargetServer = Fqdn.Parse(logon.Session.ServerFullyQualifiedDomainName);
			}
			return ropResult;
		}

		public RopResult LongTermIdFromId(IServerObject serverObject, StoreId storeId, LongTermIdFromIdResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				SessionServerObject sessionServerObject = RopHandler.Downcast<SessionServerObject>(serverObject);
				byte[] longTermIdFromId = sessionServerObject.Session.IdConverter.GetLongTermIdFromId(storeId);
				StoreLongTermId longTermId;
				try
				{
					longTermId = StoreLongTermId.Parse(longTermIdFromId);
				}
				catch (BufferParseException innerException)
				{
					throw new RopExecutionException(string.Format("Found invalid long term id {0}", new ArrayTracer<byte>(longTermIdFromId)), (ErrorCode)2147746063U, innerException);
				}
				return resultFactory.CreateSuccessfulResult(longTermId);
			}, resultFactory);
		}

		public RopResult ModifyPermissions(IServerObject serverObject, ModifyPermissionsFlags modifyPermissionsFlags, ModifyTableRow[] permissions, ModifyPermissionsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				folder.ModifyPermissions(modifyPermissionsFlags, permissions);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult ModifyRules(IServerObject serverObject, ModifyRulesFlags modifyRulesFlags, ModifyTableRow[] rulesData, ModifyRulesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				folder.ModifyRules(modifyRulesFlags, rulesData);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult MoveCopyMessages(IServerObject sourceServerObject, IServerObject destinationServerObject, StoreId[] messageIds, bool reportProgress, bool isCopy, MoveCopyMessagesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(sourceServerObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(sourceServerObject);
				Folder destinationFolder = RopHandler.Downcast<Folder>(destinationServerObject);
				folder.MoveCopyMessages(destinationFolder, messageIds, reportProgress, reportProgress ? resultFactory.CreateProgressToken() : null, isCopy);
				return RopHandler.CreateProgressRopResult(folder, reportProgress, resultFactory);
			}, resultFactory);
		}

		public RopResult MoveCopyMessagesExtended(IServerObject sourceServerObject, IServerObject destinationServerObject, StoreId[] messageIds, bool reportProgress, bool isCopy, PropertyValue[] propertyValues, MoveCopyMessagesExtendedResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 87600, "MoveCopyMessagesExtended");
		}

		public RopResult MoveCopyMessagesExtendedWithEntryIds(IServerObject sourceServerObject, IServerObject destinationServerObject, StoreId[] messageIds, bool reportProgress, bool isCopy, PropertyValue[] propertyValues, MoveCopyMessagesExtendedWithEntryIdsResultFactory resultFactory)
		{
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U, false);
		}

		public RopResult MoveFolder(IServerObject sourceServerObject, IServerObject destinationServerObject, bool reportProgress, StoreId folderId, string folderName, MoveFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(sourceServerObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(sourceServerObject);
				Folder folder2 = RopHandler.Downcast<Folder>(destinationServerObject);
				if (folder.Session is PublicFolderSession ^ folder2.Session is PublicFolderSession)
				{
					throw new RopExecutionException("Cannot move folders between Public and Private Mailboxes", (ErrorCode)2147746050U);
				}
				folder.MoveCopyFolder(folder2, folderName, true, folderId, reportProgress, false, reportProgress ? resultFactory.CreateProgressToken() : null);
				return RopHandler.CreateProgressRopResult(folder, reportProgress, resultFactory);
			}, resultFactory);
		}

		public RopResult RemoveAllRecipients(IServerObject serverObject, RemoveAllRecipientsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				message.RemoveAllRecipients();
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult OpenAttachment(IServerObject serverObject, OpenMode openMode, uint attachmentNumber, OpenAttachmentResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					Attachment attachment = null;
					if (message.TryOpenAttachment(openMode, attachmentNumber, out attachment))
					{
						disposeGuard.Add<Attachment>(attachment);
						RopResult ropResult = resultFactory.CreateSuccessfulResult(attachment);
						disposeGuard.Success();
						result = ropResult;
					}
					else
					{
						result = resultFactory.CreateFailedResult((ErrorCode)2147746063U);
					}
				}
				return result;
			}, resultFactory);
		}

		public RopResult OpenCollector(IServerObject serverObject, bool wantMessageCollector, OpenCollectorResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					IcsUpload icsUpload = folder.OpenCollector(wantMessageCollector);
					disposeGuard.Add<IcsUpload>(icsUpload);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(icsUpload);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult OpenEmbeddedMessage(IServerObject serverObject, ushort codePageId, OpenMode openMode, OpenEmbeddedMessageResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Attachment attachment = RopHandler.Downcast<Attachment>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					Encoding string8Encoding;
					if (!String8Encodings.TryGetEncoding((int)codePageId, serverObject.String8Encoding, out string8Encoding))
					{
						string message = string.Format("Cannot resolve code page: {0}", codePageId);
						throw new RopExecutionException(message, ErrorCode.UnknownCodepage);
					}
					EmbeddedMessage embeddedMessage = attachment.OpenEmbeddedMessage(openMode, string8Encoding);
					disposeGuard.Add<EmbeddedMessage>(embeddedMessage);
					MessageHeader messageHeader = Message.GetMessageHeader(embeddedMessage.CoreItem);
					using (RecipientCollector recipientCollector = resultFactory.CreateRecipientCollector(messageHeader, embeddedMessage.ExtraRecipientPropertyTags, String8Encodings.TemporaryDefault))
					{
						embeddedMessage.AddRecipients(recipientCollector);
						RopResult ropResult = resultFactory.CreateSuccessfulResult(embeddedMessage, default(StoreId), recipientCollector);
						disposeGuard.Success();
						result = ropResult;
					}
				}
				return result;
			}, resultFactory);
		}

		public RopResult OpenFolder(IServerObject serverObject, StoreId folderId, OpenMode openMode, OpenFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				SessionServerObject sessionServerObject = RopHandler.Downcast<SessionServerObject>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					bool hasRules = false;
					ReplicaServerInfo? replicaInfo = null;
					Folder folder = sessionServerObject.OpenFolder(folderId, openMode, out hasRules, out replicaInfo);
					disposeGuard.Add<Folder>(folder);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(folder, hasRules, replicaInfo);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult OpenMessage(IServerObject serverObject, ushort codePageId, StoreId folderId, OpenMode openMode, StoreId messageId, OpenMessageResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				SessionServerObject sessionServerObject = RopHandler.Downcast<SessionServerObject>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					RecipientCollector recipientCollector = null;
					Message message = sessionServerObject.OpenMessage(codePageId, folderId, openMode, messageId, new Func<MessageHeader, PropertyTag[], Encoding, RecipientCollector>(resultFactory.CreateRecipientCollector), out recipientCollector);
					disposeGuard.Add<Message>(message);
					using (recipientCollector)
					{
						RopResult ropResult = resultFactory.CreateSuccessfulResult(message, recipientCollector);
						disposeGuard.Success();
						result = ropResult;
					}
				}
				return result;
			}, resultFactory);
		}

		public RopResult OpenStream(IServerObject serverObject, PropertyTag propertyTag, OpenMode openMode, OpenStreamResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					uint streamSize;
					Stream stream = propertyServerObject.OpenStream(propertyTag, openMode, out streamSize);
					disposeGuard.Add<Stream>(stream);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(stream, streamSize);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult PrereadMessages(IServerObject serverObject, StoreIdPair[] messages, PrereadMessagesResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 250386, "PrereadMessages");
		}

		public RopResult Progress(IServerObject serverObject, bool wantCancel, ProgressResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				ServerObject serverObject2 = serverObject as ServerObject;
				if (serverObject2 == null)
				{
					throw new RopExecutionException(string.Format("Type {0} cannot be obtained from an instance of {1}", typeof(ServerObject).Name, serverObject.GetType().Name), (ErrorCode)2147746050U);
				}
				Logon logonObject = serverObject2.LogonObject;
				if (logonObject.AsyncOperationExecutor == null)
				{
					return resultFactory.CreateStandardFailedResult((ErrorCode)2147746050U);
				}
				if (wantCancel)
				{
					logonObject.AsyncOperationExecutor.EndOperation();
				}
				object arg;
				ProgressInfo progressInfo;
				try
				{
					logonObject.AsyncOperationExecutor.GetProgressInfo(out arg, out progressInfo);
				}
				catch
				{
					logonObject.RemoveAsyncExecutor();
					throw;
				}
				if (progressInfo.IsCompleted)
				{
					logonObject.RemoveAsyncExecutor();
					return progressInfo.CreateCompleteResultForProgress(arg, resultFactory);
				}
				return resultFactory.CreateSuccessfulResult(logonObject.LogonId, progressInfo.CompletedTaskCount, progressInfo.TotalTaskCount);
			}, resultFactory);
		}

		public RopResult PublicFolderIsGhosted(IServerObject serverObject, StoreId folderId, PublicFolderIsGhostedResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				if (serverObject is PrivateLogon)
				{
					return resultFactory.CreateSuccessfulResult(null);
				}
				PublicLogon publicLogon = RopHandler.Downcast<PublicLogon>(serverObject);
				return resultFactory.CreateSuccessfulResult(Microsoft.Exchange.RpcClientAccess.Handler.PublicLogon.GetReplicaServerInfo(publicLogon.PublicFolderSession, folderId, true));
			}, resultFactory);
		}

		public RopResult QueryColumnsAll(IServerObject serverObject, QueryColumnsAllResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				return view.ProtectAgainstNoColumnsSet((View localView) => resultFactory.CreateSuccessfulResult(localView.QueryColumnsAll()));
			}, resultFactory);
		}

		public RopResult QueryNamedProperties(IServerObject serverObject, QueryNamedPropertyFlags queryFlags, Guid? propertyGuid, QueryNamedPropertiesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				PropertyId[] propertyIds;
				NamedProperty[] namedProperties;
				propertyServerObject.GetNamedProperties(propertyGuid, (byte)(queryFlags & QueryNamedPropertyFlags.NoStrings) == 1, (byte)(queryFlags & QueryNamedPropertyFlags.NoIds) == 2, out propertyIds, out namedProperties);
				return resultFactory.CreateSuccessfulResult(propertyIds, namedProperties);
			}, resultFactory);
		}

		public RopResult QueryPosition(IServerObject serverObject, QueryPositionResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				return view.ProtectAgainstNoColumnsSet((View localView) => checked(resultFactory.CreateSuccessfulResult((uint)localView.GetPosition(), (uint)localView.GetRowCount())));
			}, resultFactory);
		}

		public RopResult QueryRows(IServerObject serverObject, QueryRowsFlags flags, bool useForwardDirection, ushort rowCount, QueryRowsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				RopResult result;
				using (RowCollector rowCollector = resultFactory.CreateRowCollector())
				{
					bool flag = view.CollectRows(rowCollector, flags, useForwardDirection, rowCount);
					result = resultFactory.CreateSuccessfulResult(flag ? BookmarkOrigin.Current : BookmarkOrigin.End, rowCollector);
				}
				return result;
			}, resultFactory);
		}

		public RopResult ReadPerUserInformation(IServerObject serverObject, StoreLongTermId longTermId, bool wantIfChanged, uint dataOffset, ushort maxDataSize, ReadPerUserInformationResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Logon logon = RopHandler.Downcast<Logon>(serverObject);
				bool hasFinished;
				byte[] data = logon.Session.ReadPerUserInformation(longTermId.ToBytes(true), wantIfChanged, dataOffset, maxDataSize, out hasFinished);
				return resultFactory.CreateSuccessfulResult(hasFinished, data);
			}, resultFactory);
		}

		public RopResult ReadRecipients(IServerObject serverObject, uint recipientRowId, PropertyTag[] extraUnicodePropertyTags, ReadRecipientsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				RopResult result;
				using (RecipientCollector recipientCollector = message.ReadRecipients(recipientRowId, extraUnicodePropertyTags, new Func<PropertyTag[], RecipientCollector>(resultFactory.CreateRecipientCollector)))
				{
					if (!recipientCollector.IsEmpty)
					{
						result = resultFactory.CreateSuccessfulResult(recipientCollector);
					}
					else
					{
						result = resultFactory.CreateFailedResult((ErrorCode)2147746063U);
					}
				}
				return result;
			}, resultFactory);
		}

		public RopResult ReadStream(IServerObject serverObject, ushort byteCount, ReadStreamResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Stream stream = RopHandler.Downcast<Stream>(serverObject);
				return resultFactory.CreateSuccessfulResult(stream.Read(byteCount));
			}, resultFactory);
		}

		public RopResult RegisterNotification(IServerObject serverObject, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, StoreId folderId, StoreId messageId, RegisterNotificationResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Logon logon = RopHandler.Downcast<Logon>(serverObject);
				RopResult result;
				using (DisposeGuard disposeGuard = default(DisposeGuard))
				{
					NotificationRegistration notificationRegistration = new NotificationRegistration(this.NotificationHandler, logon, flags, eventFlags, wantGlobalScope, folderId, messageId, resultFactory.ServerObjectHandle);
					disposeGuard.Add<NotificationRegistration>(notificationRegistration);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(notificationRegistration);
					disposeGuard.Success();
					result = ropResult;
				}
				return result;
			}, resultFactory);
		}

		public RopResult RegisterSynchronizationNotifications(IServerObject serverObject, StoreId[] folderIds, uint[] changeNumbers, RegisterSynchronizationNotificationsResultFactory resultFactory)
		{
			if (folderIds != null && changeNumbers != null && folderIds.Length != changeNumbers.Length)
			{
				throw new RopExecutionException("The list of folder Ids and the list of change numbers are expected to contain the same number of items.", (ErrorCode)2147942487U);
			}
			return this.RopNotImplemented(resultFactory, 18653, "RegisterSynchronizationNotifications");
		}

		public void Release(IServerObject serverObject)
		{
			base.CheckDisposed();
			Util.ThrowOnNullArgument(serverObject, "serverObject");
			Logon logonWithAsyncOperation = null;
			this.connectionHandler.ForAnyLogon(delegate(Logon logonX)
			{
				if (logonX.AsyncOperationExecutor != null)
				{
					logonWithAsyncOperation = logonX;
					return true;
				}
				return false;
			});
			if (logonWithAsyncOperation != null && logonWithAsyncOperation.AsyncOperationExecutor != null)
			{
				logonWithAsyncOperation.RemoveAsyncExecutor();
			}
			Logon logon = serverObject as Logon;
			if (logon != null)
			{
				logon.LogLogoff();
				this.connectionHandler.RemoveLogon(logon);
			}
			ErrorCode errorCode;
			Exception exception;
			ErrorCode errorCode2;
			if (!ExceptionTranslator.TryExecuteCatchAndTranslateExceptions<ErrorCode>(delegate()
			{
				((IDisposable)serverObject).Dispose();
				return ErrorCode.None;
			}, (ErrorCode ec) => ec, true, out errorCode, out exception, out errorCode2))
			{
				RopHandlerHelper.TraceRopResult(RopId.Release, exception, errorCode2);
			}
		}

		public RopResult ReloadCachedInformation(IServerObject serverObject, PropertyTag[] extraUnicodePropertyTags, ReloadCachedInformationResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				RopResult result;
				using (RecipientCollector recipientCollector = message.ReloadCachedInformation(extraUnicodePropertyTags, new Func<MessageHeader, PropertyTag[], Encoding, RecipientCollector>(resultFactory.CreateRecipientCollector)))
				{
					result = resultFactory.CreateSuccessfulResult(recipientCollector);
				}
				return result;
			}, resultFactory);
		}

		public RopResult ResetTable(IServerObject serverObject, ResetTableResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				view.Reset();
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult Restrict(IServerObject serverObject, RestrictFlags flags, Restriction restriction, RestrictResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				view.Restrict(flags, restriction);
				return resultFactory.CreateSuccessfulResult(TableStatus.Complete);
			}, resultFactory);
		}

		public RopResult SaveChangesAttachment(IServerObject serverObject, SaveChangesMode saveChangesMode, SaveChangesAttachmentResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Attachment attachment = RopHandler.Downcast<Attachment>(serverObject);
				attachment.SaveChanges(saveChangesMode);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult SaveChangesMessage(IServerObject serverObject, SaveChangesMode saveChangesMode, SaveChangesMessageResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				return resultFactory.CreateSuccessfulResult(message.SaveChanges(saveChangesMode));
			}, resultFactory);
		}

		public RopResult SeekRow(IServerObject serverObject, BookmarkOrigin bookmarkOrigin, int rowCount, bool wantMoveCount, SeekRowResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				return view.ProtectAgainstNoColumnsSet(delegate(View localView)
				{
					int num = localView.SeekRow(bookmarkOrigin, rowCount);
					return resultFactory.CreateSuccessfulResult(num != rowCount, num);
				});
			}, resultFactory);
		}

		public RopResult SeekRowApproximate(IServerObject serverObject, uint numerator, uint denominator, SeekRowApproximateResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				return view.ProtectAgainstNoColumnsSet(delegate(View localView)
				{
					localView.SeekRowApproximate(numerator, denominator);
					return resultFactory.CreateSuccessfulResult();
				});
			}, resultFactory);
		}

		public RopResult SeekRowBookmark(IServerObject serverObject, byte[] bookmark, int rowCount, bool wantMoveCount, SeekRowBookmarkResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(bookmark, "bookmark");
				FolderBasedView folderBasedView = RopHandler.Downcast<FolderBasedView>(serverObject);
				bool soughtLessThanRequested;
				bool positionChanged;
				int rowsSought = folderBasedView.SeekRowBookmark(bookmark, rowCount, wantMoveCount, out soughtLessThanRequested, out positionChanged);
				return resultFactory.CreateSuccessfulResult(positionChanged, soughtLessThanRequested, rowsSought);
			}, resultFactory);
		}

		public RopResult SeekStream(IServerObject serverObject, StreamSeekOrigin streamSeekOrigin, long offset, SeekStreamResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Stream stream = RopHandler.Downcast<Stream>(serverObject);
				long num;
				try
				{
					num = stream.Seek(streamSeekOrigin, offset);
				}
				catch (StoragePermanentException)
				{
					num = -1L;
				}
				catch (IOException)
				{
					num = -1L;
				}
				if (num < 0L || num > 2147483647L)
				{
					return resultFactory.CreateFailedResult((ErrorCode)2147680281U);
				}
				return resultFactory.CreateSuccessfulResult((ulong)num);
			}, resultFactory);
		}

		public RopResult SetCollapseState(IServerObject serverObject, byte[] collapseState, SetCollapseStateResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(collapseState, "collapseState");
				ContentsView contentsView = RopHandler.Downcast<ContentsView>(serverObject);
				return resultFactory.CreateSuccessfulResult(contentsView.SetCollapseState(collapseState));
			}, resultFactory);
		}

		public RopResult SetColumns(IServerObject serverObject, SetColumnsFlags flags, PropertyTag[] propertyTags, SetColumnsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				View view = RopHandler.Downcast<View>(serverObject);
				view.SetColumns(flags, propertyTags);
				return resultFactory.CreateSuccessfulResult(TableStatus.Complete);
			}, resultFactory);
		}

		public RopResult SetLocalReplicaMidsetDeleted(IServerObject serverObject, LongTermIdRange[] longTermIdRanges, SetLocalReplicaMidsetDeletedResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				folder.SetLocalReplicaMidsetDeleted(longTermIdRanges);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult SetMessageFlags(IServerObject serverObject, StoreId messageId, MessageFlags flags, MessageFlags flagsMask, SetMessageFlagsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				folder.SetMessageFlags(messageId, flags, flagsMask);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult SetMessageStatus(IServerObject serverObject, StoreId messageId, MessageStatusFlags status, MessageStatusFlags statusMask, SetMessageStatusResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				MessageStatusFlags oldStatus = folder.SetMessageStatus(messageId, status, statusMask);
				return resultFactory.CreateSuccessfulResult(oldStatus);
			}, resultFactory);
		}

		public RopResult SetProperties(IServerObject serverObject, PropertyValue[] propertyValues, SetPropertiesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				return resultFactory.CreateSuccessfulResult(propertyServerObject.SetProperties(propertyValues, true));
			}, resultFactory);
		}

		public RopResult SetPropertiesNoReplicate(IServerObject serverObject, PropertyValue[] propertyValues, SetPropertiesNoReplicateResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PropertyServerObject propertyServerObject = RopHandler.Downcast<PropertyServerObject>(serverObject);
				return resultFactory.CreateSuccessfulResult(propertyServerObject.SetProperties(propertyValues, false));
			}, resultFactory);
		}

		public RopResult SetReadFlag(IServerObject serverObject, SetReadFlagFlags flags, SetReadFlagResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				return resultFactory.CreateSuccessfulResult(message.SetReadFlag(flags));
			}, resultFactory);
		}

		public RopResult SetReadFlags(IServerObject serverObject, bool reportProgress, SetReadFlagFlags flags, StoreId[] messageIds, SetReadFlagsResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				if (!folder.LogonObject.SupportProgressForSetReadFlags && reportProgress)
				{
					reportProgress = false;
				}
				TestInterceptor.Intercept(TestInterceptorLocation.RopHandler_SetReadFlags, new object[]
				{
					reportProgress
				});
				folder.SetReadFlags(reportProgress, reportProgress ? resultFactory.CreateProgressToken() : null, flags, messageIds);
				return RopHandler.CreateProgressRopResult(folder, reportProgress, resultFactory);
			}, resultFactory);
		}

		public RopResult SetReceiveFolder(IServerObject serverObject, StoreId folderId, string messageClass, SetReceiveFolderResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(messageClass, "messageClass");
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				privateLogon.SetReceiveFolder(messageClass, folderId);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult SetSearchCriteria(IServerObject serverObject, Restriction restriction, StoreId[] folderIds, SetSearchCriteriaFlags setSearchCriteriaFlags, SetSearchCriteriaResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Folder folder = RopHandler.Downcast<Folder>(serverObject);
				folder.SetSearchCriteria(restriction, folderIds, setSearchCriteriaFlags);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult SetSizeStream(IServerObject serverObject, ulong streamSize, SetSizeStreamResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Stream stream = RopHandler.Downcast<Stream>(serverObject);
				stream.SetSize(streamSize);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult SetSpooler(IServerObject serverObject, SetSpoolerResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				privateLogon.SetSpooler();
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult SetSynchronizationNotificationGuid(IServerObject serverObject, Guid notificationGuid, SetSynchronizationNotificationGuidResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 18654, "SetSynchronizationNotificationGuid");
		}

		public RopResult SetTransport(IServerObject serverObject, SetTransportResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				StoreId transportQueueId = privateLogon.GetTransportQueueId();
				return resultFactory.CreateSuccessfulResult(transportQueueId);
			}, resultFactory);
		}

		public RopResult SortTable(IServerObject serverObject, SortTableFlags flags, ushort categoryCount, ushort expandedCount, SortOrder[] sortOrders, SortTableResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(serverObject, "serverObject");
				Util.ThrowOnNullArgument(sortOrders, "sortOrders");
				View view = RopHandler.Downcast<View>(serverObject);
				view.Sort(flags, categoryCount, expandedCount, sortOrders);
				return resultFactory.CreateSuccessfulResult(TableStatus.Complete);
			}, resultFactory);
		}

		public RopResult SpoolerLockMessage(IServerObject serverObject, StoreId messageId, LockState lockState, SpoolerLockMessageResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				privateLogon.SpoolerLockMessage(messageId, lockState);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult SpoolerRules(IServerObject serverObject, StoreId folderId, SpoolerRulesResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 12456, "SpoolerRules");
		}

		public RopResult SubmitMessage(IServerObject serverObject, SubmitMessageFlags submitFlags, SubmitMessageResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				message.SubmitMessage(submitFlags);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult SynchronizationOpenAdvisor(IServerObject serverObject, SynchronizationOpenAdvisorResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 18652, "SynchronizationOpenAdvisor");
		}

		public RopResult TellVersion(IServerObject serverObject, ushort productVersion, ushort buildMajorVersion, ushort buildMinorVersion, TellVersionResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, () => resultFactory.CreateSuccessfulResult(), resultFactory);
		}

		public RopResult TransportDeliverMessage(IServerObject serverObject, TransportRecipientType recipientType, TransportDeliverMessageResultFactory resultFactory)
		{
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		public RopResult TransportDeliverMessage2(IServerObject serverObject, TransportRecipientType recipientType, TransportDeliverMessage2ResultFactory resultFactory)
		{
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		public RopResult TransportDoneWithMessage(IServerObject serverObject, TransportDoneWithMessageResultFactory resultFactory)
		{
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		public RopResult TransportDuplicateDeliveryCheck(IServerObject serverObject, byte flags, ExDateTime submitTime, string internetMessageId, TransportDuplicateDeliveryCheckResultFactory resultFactory)
		{
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		public RopResult TransportNewMail(IServerObject serverObject, StoreId folderId, StoreId messageId, string messageClass, MessageFlags messageFlags, TransportNewMailResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				privateLogon.TransportNewMail(folderId, messageId, messageClass, messageFlags);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult TransportSend(IServerObject serverObject, TransportSendResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Message message = RopHandler.Downcast<Message>(serverObject);
				PropertyValue[] propertyValues = message.TransportSend();
				return resultFactory.CreateSuccessfulResult(propertyValues);
			}, resultFactory);
		}

		public RopResult UnlockRegionStream(IServerObject serverObject, ulong offset, ulong regionLength, LockTypeFlag lockType, UnlockRegionStreamResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 18676, "UnlockRegionStream");
		}

		public RopResult UpdateDeferredActionMessages(IServerObject serverObject, byte[] serverEntryId, byte[] clientEntryId, UpdateDeferredActionMessagesResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				PrivateLogon privateLogon = RopHandler.Downcast<PrivateLogon>(serverObject);
				privateLogon.UpdateDeferredActionMessages(serverEntryId, clientEntryId);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult UploadStateStreamBegin(IServerObject serverObject, PropertyTag propertyTag, uint size, UploadStateStreamBeginResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				IcsStateHelper icsStateHelper = RopHandler.Downcast<IcsStateHelper>(serverObject);
				icsStateHelper.BeginUploadProperty(propertyTag, size);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult UploadStateStreamContinue(IServerObject serverObject, ArraySegment<byte> data, UploadStateStreamContinueResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				IcsStateHelper icsStateHelper = RopHandler.Downcast<IcsStateHelper>(serverObject);
				icsStateHelper.UploadPropertyData(data);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult UploadStateStreamEnd(IServerObject serverObject, UploadStateStreamEndResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				IcsStateHelper icsStateHelper = RopHandler.Downcast<IcsStateHelper>(serverObject);
				icsStateHelper.EndUploadProperty();
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult WriteCommitStream(IServerObject serverObject, byte[] data, WriteCommitStreamResultFactory resultFactory)
		{
			return this.RopNotImplemented(resultFactory, 12463, "WriteCommitStream");
		}

		public RopResult WritePerUserInformation(IServerObject serverObject, StoreLongTermId longTermId, bool hasFinished, uint dataOffset, byte[] data, Guid? replicaGuid, WritePerUserInformationResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(data, "data");
				Logon logon = RopHandler.Downcast<Logon>(serverObject);
				if (dataOffset == 0U && logon is PrivateLogon)
				{
					if (replicaGuid == null || replicaGuid.Value == Guid.Empty)
					{
						throw new RopExecutionException("The replicaGuid is required for the first write operation on a private logon.", (ErrorCode)2147942487U);
					}
				}
				else if (replicaGuid != null)
				{
					throw new RopExecutionException("The replicaGuid was expected to be null.", (ErrorCode)2147942487U);
				}
				logon.Session.WritePerUserInformation(longTermId.ToBytes(true), hasFinished, dataOffset, data, replicaGuid);
				return resultFactory.CreateSuccessfulResult();
			}, resultFactory);
		}

		public RopResult WriteStream(IServerObject serverObject, ArraySegment<byte> data, WriteStreamResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(data, "data");
				Stream stream = RopHandler.Downcast<Stream>(serverObject);
				ushort byteCount = stream.Write(data);
				return resultFactory.CreateSuccessfulResult(byteCount);
			}, resultFactory);
		}

		public RopResult WriteStreamExtended(IServerObject serverObject, ArraySegment<byte>[] dataChunks, WriteStreamExtendedResultFactory resultFactory)
		{
			return this.StandardRopHandlerMethod(serverObject, delegate
			{
				Util.ThrowOnNullArgument(dataChunks, "dataChunks");
				Stream stream = RopHandler.Downcast<Stream>(serverObject);
				uint num = 0U;
				foreach (ArraySegment<byte> arraySegment in dataChunks)
				{
					Util.ThrowOnNullArgument(arraySegment, "dataChunk");
					num += (uint)stream.Write(arraySegment);
				}
				return resultFactory.CreateSuccessfulResult(num);
			}, resultFactory);
		}

		protected override void InternalDispose()
		{
			this.LogBudgetStatistics();
			base.InternalDispose();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RopHandler>(this);
		}

		internal static T Downcast<T>(IServerObject serverObject) where T : class
		{
			Util.ThrowOnNullArgument(serverObject, "serverObject");
			T t = serverObject as T;
			if (t == null)
			{
				IServiceProvider<T> serviceProvider = serverObject as IServiceProvider<T>;
				if (serviceProvider != null)
				{
					t = serviceProvider.Get();
				}
			}
			if (t == null)
			{
				throw new RopExecutionException(string.Format("Type {0} cannot be obtained from an instance of {1}", typeof(T).Name, serverObject.GetType().Name), (ErrorCode)2147746050U);
			}
			ServerObject serverObject2 = t as ServerObject;
			if (serverObject2 != null)
			{
				serverObject2.OnAccess();
			}
			return t;
		}

		private static Logon GetLogonObject(IServerObject serverObject)
		{
			Util.ThrowOnNullArgument(serverObject, "serverObject");
			return ((ServerObject)serverObject).LogonObject;
		}

		internal static void CheckEnum<T>(T enumValue) where T : struct
		{
			if (!EnumValidator<T>.IsValidValue(enumValue))
			{
				throw new RopExecutionException(string.Format("Invalid {0} value: {1}", typeof(T), enumValue), (ErrorCode)2147942487U);
			}
		}

		internal static bool IsSearchFolder(VersionedId folderId)
		{
			return folderId.ObjectId.ObjectType == StoreObjectType.SearchFolder || folderId.ObjectId.ObjectType == StoreObjectType.OutlookSearchFolder;
		}

		private static RopResult CreateProgressRopResult(ServerObject serverObject, bool reportProgress, IProgressResultFactory resultFactory)
		{
			object arg;
			ProgressInfo progressInfo;
			serverObject.LogonObject.AsyncOperationExecutor.GetProgressInfo(out arg, out progressInfo);
			if (progressInfo.IsCompleted)
			{
				serverObject.LogonObject.RemoveAsyncExecutor();
				return progressInfo.CreateCompleteResult(arg, resultFactory);
			}
			return resultFactory.CreateProgressResult(progressInfo.CompletedTaskCount, progressInfo.TotalTaskCount);
		}

		private static bool AllowXTCAccess(string applicationId)
		{
			foreach (string value in RopHandler.allowedXTCApplications)
			{
				if (!string.IsNullOrEmpty(applicationId) && applicationId.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static ClientSecurityContext CreateClientSecurityContextFromAuthenicationContext(AuthenticationContext authenticationContext)
		{
			Util.ThrowOnNullArgument(authenticationContext, "authenticationContext");
			ClientSecurityContext clientSecurityContext = ClientSecurityContextFactory.Create(authenticationContext, new Action<Exception>(RopHandler.OnException));
			if (clientSecurityContext == null)
			{
				throw new RopExecutionException("Unable to create clientsecuritycontext from the authenticationContext for delegate resource access.", (ErrorCode)2147942405U);
			}
			return clientSecurityContext;
		}

		private static void OnException(Exception exception)
		{
			if (ExTraceGlobals.LogonTracer.IsTraceEnabled(TraceType.WarningTrace))
			{
				ExTraceGlobals.LogonTracer.TraceWarning<Exception>(0, Activity.TraceId, "Could not create ClientSecurityContext from the authenticationContext. Exception {0}", exception);
			}
		}

		private static RopResult CreateStandardFailedResult(IResultFactory resultFactory, ErrorCode errorCode, Exception exception)
		{
			return RopHandler.CreateFailedResultWithException(resultFactory.CreateStandardFailedResult(errorCode), exception);
		}

		private static RopResult CreateFailedResultWithException(RopResult result, Exception exception)
		{
			if (result != null)
			{
				result.Exception = exception;
			}
			return result;
		}

		private RopResult StandardRopHandlerMethod(IServerObject serverObject, Func<RopResult> methodBody, IResultFactory resultFactory)
		{
			return this.GenericRopHandlerMethod(serverObject, methodBody, resultFactory, new Func<IResultFactory, ErrorCode, Exception, RopResult>(RopHandler.CreateStandardFailedResult));
		}

		private void TrackBudgetStatistics(IStandardBudget budget)
		{
			StandardBudgetWrapper standardBudgetWrapper = budget as StandardBudgetWrapper;
			if (standardBudgetWrapper != null)
			{
				StandardBudget innerBudget = standardBudgetWrapper.GetInnerBudget();
				if (innerBudget != null)
				{
					float balance = innerBudget.CasTokenBucket.GetBalance();
					if (balance < this.lowestBudgetBalance)
					{
						this.lowestBudgetBalance = balance;
					}
				}
			}
		}

		private void LogBudgetStatistics()
		{
			ProtocolLog.LogThrottlingStatistics(this.lowestBudgetBalance, this.overBudgetCount);
		}

		private void LogOverBudget(IBudget budget, OverBudgetException overBudgetException)
		{
			this.overBudgetCount += 1U;
			ProtocolLog.LogThrottlingOverBudget(overBudgetException.PolicyPart, overBudgetException.BackoffTime);
			ExTraceGlobals.ClientThrottledTracer.TraceWarning(0, Activity.TraceId, "User {0}: {1}", new object[]
			{
				this.Connection.ActAsLegacyDN,
				"Over budget for: {0}  Backoff time: {1}",
				overBudgetException.PolicyPart,
				overBudgetException.BackoffTime
			});
		}

		private RopResult GenericRopHandlerMethod(IServerObject serverObject, Func<RopResult> methodBody, IResultFactory resultFactory, Func<IResultFactory, ErrorCode, Exception, RopResult> createFailedResult)
		{
			base.CheckDisposed();
			Util.ThrowOnNullArgument(resultFactory, "resultFactory");
			Util.ThrowOnNullArgument(createFailedResult, "createFailedResult");
			Activity.Guard guard = null;
			RopResult result;
			try
			{
				if (Activity.Current == null && Activity.AllowImplicit)
				{
					guard = new Activity.Guard();
					guard.AssociateWithCurrentThread(RopHandler.implicitActivity, true);
				}
				IStandardBudget budget = Activity.Current.Budget;
				if (budget != null)
				{
					this.TrackBudgetStatistics(budget);
					OverBudgetException ex = null;
					if (Activity.Current.Budget.TryCheckOverBudget(out ex))
					{
						this.LogOverBudget(budget, ex);
						if (serverObject == null)
						{
							throw new ClientBackoffException("Client is over budget");
						}
						throw new ClientBackoffException("Client is over budget", RopHandler.GetLogonObject(serverObject).LogonId, (uint)ex.BackoffTime);
					}
				}
				Logon logon = null;
				ServerObject serverObject2 = serverObject as ServerObject;
				if (serverObject2 != null)
				{
					logon = serverObject2.LogonObject;
				}
				bool connectionHasActiveAsyncOperation = false;
				bool isCurrentLogonTheOneDoingAsyncOperation = false;
				this.connectionHandler.ForAnyLogon(delegate(Logon logonX)
				{
					if (logonX.HasActiveAsyncOperation)
					{
						connectionHasActiveAsyncOperation = true;
						isCurrentLogonTheOneDoingAsyncOperation = (logon != null && logon == logonX);
						return true;
					}
					return false;
				});
				if (connectionHasActiveAsyncOperation && !isCurrentLogonTheOneDoingAsyncOperation)
				{
					if (logon != null)
					{
						throw new ClientBackoffException("Async operation is being executed on another logon.", logon.LogonId, 1000U);
					}
					throw new ClientBackoffException("Async operation is being executed on another logon.");
				}
				else
				{
					if (isCurrentLogonTheOneDoingAsyncOperation && !(resultFactory is ProgressResultFactory))
					{
						logon.RemoveAsyncExecutor();
					}
					if (logon != null)
					{
						IAsyncOperationExecutor asyncOperationExecutor = logon.AsyncOperationExecutor;
					}
					result = RopHandlerHelper.CallHandler(this, methodBody, resultFactory, createFailedResult);
				}
			}
			finally
			{
				if (guard != null)
				{
					guard.Dispose();
				}
			}
			return result;
		}

		internal static Guid? FindLocalRecoveryDatabase()
		{
			List<Guid> list = new List<Guid>();
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 5523, "FindLocalRecoveryDatabase", "f:\\15.00.1497\\sources\\dev\\mapimt\\src\\Handler\\RopHandler.cs");
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, DatabaseSchema.Recovery, true);
			IList<MailboxDatabase> list2 = topologyConfigurationSession.Find<MailboxDatabase>(topologyConfigurationSession.GetOrgContainerId(), QueryScope.SubTree, filter, null, 0);
			if (list2 != null && list2.Count > 0)
			{
				ActiveManager cachingActiveManagerInstance = ActiveManager.GetCachingActiveManagerInstance();
				GetServerForDatabaseFlags gsfdFlags = GetServerForDatabaseFlags.IgnoreAdSiteBoundary | GetServerForDatabaseFlags.ReadThrough | GetServerForDatabaseFlags.BasicQuery;
				string text = Configuration.ServiceConfiguration.ThisServerFqdn.ToString();
				foreach (MailboxDatabase mailboxDatabase in list2)
				{
					DatabaseLocationInfo serverForDatabase = cachingActiveManagerInstance.GetServerForDatabase(mailboxDatabase.Id.ObjectGuid, gsfdFlags);
					if (serverForDatabase != null && serverForDatabase.ServerFqdn != null && serverForDatabase.ServerFqdn.Equals(text, StringComparison.OrdinalIgnoreCase))
					{
						list.Add(mailboxDatabase.Id.ObjectGuid);
					}
				}
				if (list.Count > 0)
				{
					List<Guid> onlineDatabase = MailboxAdminHelper.GetOnlineDatabase(text, list);
					return new Guid?((onlineDatabase.Count > 0) ? onlineDatabase[0] : list[0]);
				}
			}
			return null;
		}

		private RopResult PrivateLogon(LogonFlags logonFlags, OpenFlags openFlags, StoreState storeState, LogonExtendedRequestFlags extendedFlags, MailboxId? mailboxId, LocaleInfo? localeInfo, string applicationId, AuthenticationContext authenticationContext, LogonResultFactory resultFactory)
		{
			LogonFlags logonFlags2 = logonFlags & (LogonFlags.Private | LogonFlags.Undercover | LogonFlags.Ghosted);
			bool flag = this.TryUpdateWithRecoveryDatabase(openFlags, ref mailboxId);
			Microsoft.Exchange.RpcClientAccess.Handler.PrivateLogon.ValidatePrivateLogonSettings(logonFlags, openFlags, extendedFlags, mailboxId);
			RopResult result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ExchangePrincipal exchangeMailboxPrincipal = Microsoft.Exchange.RpcClientAccess.Handler.Logon.FindExchangePrincipal(this.Connection, mailboxId.Value);
				ProtocolLogLogonType protocolLogLogonType = this.DetermineLogonTypeForLogging(openFlags, flag, mailboxId, exchangeMailboxPrincipal);
				ProtocolLog.LogLogonPending(protocolLogLogonType, exchangeMailboxPrincipal, applicationId);
				if (authenticationContext != null && (exchangeMailboxPrincipal == null || !exchangeMailboxPrincipal.MailboxInfo.IsArchive))
				{
					throw new RopExecutionException("Private Logons done using Constrained delegation is only supported for main archive.", (ErrorCode)2147746050U);
				}
				if (mailboxId.Value.IsLegacyDn)
				{
					string legacyDn = mailboxId.Value.LegacyDn;
					if (!exchangeMailboxPrincipal.MailboxInfo.IsArchive && !LegacyDN.StringComparer.Equals(legacyDn, exchangeMailboxPrincipal.LegacyDn) && LegacyDN.StringComparer.Equals(exchangeMailboxPrincipal.LegacyDn, this.Connection.ActAsLegacyDN))
					{
						throw new RopExecutionException(string.Format("Cannot connect to a target mailbox using an outdated LegDn if the updated LegDn was used as the ConnectAs entity. ConnectAs LegDn used: {0}, Logon LegDn used: {1}.", this.Connection.ActAsLegacyDN, legacyDn), ErrorCode.UnknownUser);
					}
				}
				if (exchangeMailboxPrincipal.MailboxInfo.IsRemote)
				{
					throw new RopExecutionException("Exchange Rpc Client Access does not support remote mailbox connections.", ErrorCode.UnknownUser);
				}
				if (exchangeMailboxPrincipal.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox && this.Connection.ClientInformation.Version < Configuration.ServiceConfiguration.TMRequiredMAPIClientVersion)
				{
					throw new RopExecutionException(string.Format("Exchange Rpc Client Access does not support client with version {0} to log onto TeamMailbox", this.Connection.ClientInformation.Version.ToString()), (ErrorCode)2147746050U);
				}
				if (this.connectionHandler.Connection.ProtocolSequence == NetworkProtocol.Http.ProtocolName)
				{
					bool shouldCreateRedirectResult = false;
					if (exchangeMailboxPrincipal.MailboxInfo.Location.ServerVersion < Server.E15MinVersion)
					{
						throw new RopExecutionException("Attempting to connect to an older (pre-E15) target mailbox version. Returning ecUnknownUser.", ErrorCode.UnknownUser);
					}
					if (this.Connection.RpcServerTarget != null && this.Connection.RpcServerTarget.IndexOf("@") > 0)
					{
						string strB = ExchangeRpcClientAccess.CreatePersonalizedServer(exchangeMailboxPrincipal.MailboxInfo.MailboxGuid, exchangeMailboxPrincipal.MailboxInfo.PrimarySmtpAddress.Domain);
						if (string.Compare(this.Connection.RpcServerTarget, strB, true, CultureInfo.InvariantCulture) != 0)
						{
							shouldCreateRedirectResult = true;
						}
					}
					if (!shouldCreateRedirectResult)
					{
						this.connectionHandler.ForAnyLogon(delegate(Logon logonX)
						{
							PrivateLogon privateLogon2 = logonX as PrivateLogon;
							if (privateLogon2 != null)
							{
								if (string.Compare(privateLogon2.MailboxSession.MailboxOwner.LegacyDn, exchangeMailboxPrincipal.LegacyDn, true) != 0)
								{
									shouldCreateRedirectResult = true;
								}
								return true;
							}
							return false;
						});
					}
					if (shouldCreateRedirectResult)
					{
						return resultFactory.CreateRedirectResult(Microsoft.Exchange.RpcClientAccess.Handler.Logon.CreatePersonalizedServerRedirectLegacyDN(exchangeMailboxPrincipal).ToString(), logonFlags2);
					}
				}
				ClientSecurityContext clientSecurityContext = null;
				MailboxSession mailboxSession;
				try
				{
					PrivateLogon linkedLogon = null;
					if (authenticationContext == null)
					{
						if (Configuration.ServiceConfiguration.ShareConnections && !flag)
						{
							this.connectionHandler.ForAnyLogon(delegate(Logon logonX)
							{
								PrivateLogon privateLogon2 = logonX as PrivateLogon;
								if (privateLogon2 != null && string.Compare(privateLogon2.MailboxSession.MailboxOwner.LegacyDn, exchangeMailboxPrincipal.LegacyDn, true, CultureInfo.InvariantCulture) == 0)
								{
									linkedLogon = privateLogon2;
									return true;
								}
								return false;
							});
						}
					}
					else
					{
						if (this.Connection.IsFederatedSystemAttendant)
						{
							throw new RopExecutionException("Unable to logone using federated system attendant with delegated authentication context.", (ErrorCode)2147746050U);
						}
						clientSecurityContext = RopHandler.CreateClientSecurityContextFromAuthenicationContext(authenticationContext);
						disposeGuard.Add<ClientSecurityContext>(clientSecurityContext);
					}
					mailboxSession = Microsoft.Exchange.RpcClientAccess.Handler.PrivateLogon.CreateMailboxSession(linkedLogon, this.Connection, openFlags, extendedFlags, exchangeMailboxPrincipal, applicationId, clientSecurityContext);
					disposeGuard.Add<MailboxSession>(mailboxSession);
				}
				catch (NotSupportedWithServerVersionException)
				{
					return resultFactory.CreateRedirectResult(exchangeMailboxPrincipal.MailboxInfo.Location.RpcClientAccessServerLegacyDn, logonFlags2);
				}
				mailboxSession.AccountingObject = Activity.Current.Budget;
				mailboxSession.DisablePrivateItemsFilter();
				this.AddUserInformationToSession(mailboxSession);
				if (this.Connection.IsWebService && !RopHandler.AllowXTCAccess(applicationId))
				{
					throw new RopExecutionException("XROP is limited to only a few on-premise applications", (ErrorCode)2147746065U);
				}
				PrivateLogon privateLogon = new PrivateLogon(mailboxSession, clientSecurityContext, this.connectionHandler, this.NotificationHandler, openFlags, resultFactory.LogonId, protocolLogLogonType);
				disposeGuard.Add<PrivateLogon>(privateLogon);
				if ((openFlags & OpenFlags.RemoteTransport) == OpenFlags.RemoteTransport)
				{
					privateLogon.GetTransportQueueId();
				}
				LogonResponseFlags logonResponseFlags = LogonResponseFlags.None;
				if (mailboxSession.IsMailboxLocalized)
				{
					logonResponseFlags = LogonResponseFlags.IsMailboxLocalized;
				}
				if (mailboxSession.CanActAsOwner)
				{
					logonResponseFlags |= LogonResponseFlags.IsMailboxOwner;
				}
				if (mailboxSession.CanSendAs)
				{
					logonResponseFlags |= LogonResponseFlags.HasSendAsRights;
				}
				if (mailboxSession.Mailbox.TryGetProperty(MailboxSchema.MailboxOofState).Equals(true))
				{
					logonResponseFlags |= LogonResponseFlags.IsOOF;
				}
				KeyValuePair<ushort, Guid> mdbIdMapping = mailboxSession.IdConverter.GetMdbIdMapping();
				ulong routingConfigurationTimestamp = (ulong)ExDateTime.UtcNow.ToBinary();
				if (privateLogon.Connection.ClientInformation.Mode == ClientMode.ExchangeServer && mailboxSession.IsE15Session)
				{
					logonFlags2 |= LogonFlags.NoRules;
				}
				RopResult ropResult = resultFactory.CreateSuccessfulPrivateResult(privateLogon, logonFlags2, privateLogon.GetDefaultFolderIds(), LogonExtendedResponseFlags.None, null, logonResponseFlags, mailboxSession.Mailbox.InstanceGuid, new ReplId(mdbIdMapping.Key), mdbIdMapping.Value, ExDateTime.UtcNow, routingConfigurationTimestamp, storeState);
				this.connectionHandler.AddLogon(privateLogon);
				disposeGuard.Success();
				result = ropResult;
			}
			return result;
		}

		private ProtocolLogLogonType DetermineLogonTypeForLogging(OpenFlags openFlags, bool loggingOnToRecoveryDatabase, MailboxId? mailboxId, ExchangePrincipal exchangeMailboxPrincipal)
		{
			if (loggingOnToRecoveryDatabase)
			{
				return ProtocolLogLogonType.Recovery;
			}
			if ((openFlags & OpenFlags.UseAdminPrivilege) == OpenFlags.UseAdminPrivilege)
			{
				return ProtocolLogLogonType.Admin;
			}
			if (mailboxId.Value.IsLegacyDn && LegacyDN.StringComparer.Equals(mailboxId.Value.LegacyDn, this.Connection.ActAsLegacyDN))
			{
				return ProtocolLogLogonType.Owner;
			}
			if (exchangeMailboxPrincipal.MailboxInfo.IsArchive)
			{
				return ProtocolLogLogonType.Archive;
			}
			return ProtocolLogLogonType.Delegate;
		}

		private bool TryUpdateWithRecoveryDatabase(OpenFlags openFlags, ref MailboxId? mailboxId)
		{
			if ((openFlags & OpenFlags.RestoreDatabase) != OpenFlags.RestoreDatabase || mailboxId == null)
			{
				return false;
			}
			if ((openFlags & OpenFlags.UseAdminPrivilege) != OpenFlags.UseAdminPrivilege)
			{
				throw new RopExecutionException("Logons to recovery databases must be accompanied by ConnectionFlags.AdminPrivilege", ErrorCode.LoginPerm);
			}
			if (!Configuration.ServiceConfiguration.CanServiceRecoveryDatabaseLogons)
			{
				throw new RopExecutionException("Logons to recovery databases are only supported on Mailbox role", (ErrorCode)2147746050U);
			}
			Guid mailboxGuid;
			if (mailboxId.Value.IsLegacyDn)
			{
				mailboxGuid = Microsoft.Exchange.RpcClientAccess.Handler.Logon.FindExchangePrincipal(this.Connection, mailboxId.Value).MailboxInfo.MailboxGuid;
			}
			else
			{
				mailboxGuid = mailboxId.Value.MailboxGuid;
			}
			Guid? guid = RopHandler.FindLocalRecoveryDatabase();
			if (guid == null)
			{
				throw new RopExecutionException("There's no recovery database on the local server", (ErrorCode)2147746065U);
			}
			mailboxId = new MailboxId?(new MailboxId(mailboxGuid, guid.Value));
			return true;
		}

		private void AddUserInformationToSession(StoreSession session)
		{
			ClientInfo clientInformation = this.connectionHandler.Connection.ClientInformation;
			session.ClientMachineName = clientInformation.MachineName;
			session.ClientProcessName = clientInformation.ProcessName;
			session.ClientVersion = (long)clientInformation.Version.Value;
			session.SetClientIPEndpoints(clientInformation.IpAddress, this.connectionHandler.Connection.ServerIpAddress);
			if (clientInformation.ClientSessionInfo != null)
			{
				try
				{
					session.SetRemoteClientSessionInfo(clientInformation.ClientSessionInfo);
				}
				catch (ClientSessionInfoParseException ex)
				{
					throw new BufferParseException(ex.Message);
				}
			}
		}

		private RopResult PublicLogon(LogonFlags logonFlags, OpenFlags openFlags, StoreState storeState, LogonExtendedRequestFlags extendedFlags, MailboxId? mailboxId, LocaleInfo? localeInfo, string applicationId, AuthenticationContext authenticationContext, LogonResultFactory resultFactory)
		{
			LogonFlags logonFlags2 = logonFlags & (LogonFlags.Private | LogonFlags.Undercover | LogonFlags.Ghosted);
			Microsoft.Exchange.RpcClientAccess.Handler.PublicLogon.ValidatePublicLogonSettings(logonFlags, openFlags, extendedFlags, mailboxId, authenticationContext);
			DatabaseLocationInfo databaseLocationInfo = null;
			ExchangePrincipal publicFolderMailboxPrincipal = Microsoft.Exchange.RpcClientAccess.Handler.PublicLogon.GetPublicFolderMailboxPrincipal(this.Connection.MiniRecipient, this.Connection.RpcServerTarget, mailboxId, logonFlags, openFlags);
			if (publicFolderMailboxPrincipal == null)
			{
				try
				{
					ExchangePrincipal exchangePrincipal = this.Connection.FindExchangePrincipalByLegacyDN(this.Connection.ActAsLegacyDN);
					if (exchangePrincipal.MailboxInfo.Location.HomePublicFolderDatabaseGuid != Guid.Empty)
					{
						databaseLocationInfo = this.databaseLocationProvider.GetLocationInfo(exchangePrincipal.MailboxInfo.Location.HomePublicFolderDatabaseGuid, false, true);
					}
				}
				catch (UserHasNoMailboxException)
				{
				}
				if (databaseLocationInfo == null)
				{
					throw new RopExecutionException(string.Format("No hierarchy has been provisioned for user in the organization.", new object[0]), (ErrorCode)2147746065U);
				}
			}
			else
			{
				ProtocolLog.LogLogonPending(ProtocolLogLogonType.Public, publicFolderMailboxPrincipal, applicationId);
			}
			string redirectionServerLegacyDN = Microsoft.Exchange.RpcClientAccess.Handler.PublicLogon.GetRedirectionServerLegacyDN(this.Connection, publicFolderMailboxPrincipal, databaseLocationInfo, openFlags);
			if (redirectionServerLegacyDN != null)
			{
				return resultFactory.CreateRedirectResult(redirectionServerLegacyDN, logonFlags2);
			}
			RopResult result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ClientSecurityContext clientSecurityContext = null;
				PublicFolderSession publicFolderSession;
				try
				{
					if (authenticationContext != null)
					{
						if (this.Connection.IsFederatedSystemAttendant)
						{
							throw new RopExecutionException("Unable to logon using federated system attendant with delegated authentication context.", (ErrorCode)2147746050U);
						}
						clientSecurityContext = RopHandler.CreateClientSecurityContextFromAuthenicationContext(authenticationContext);
						disposeGuard.Add<ClientSecurityContext>(clientSecurityContext);
					}
					publicFolderSession = Microsoft.Exchange.RpcClientAccess.Handler.PublicLogon.CreatePublicFolderSession(this.Connection, openFlags, extendedFlags, publicFolderMailboxPrincipal, clientSecurityContext, applicationId);
					disposeGuard.Add<PublicFolderSession>(publicFolderSession);
					publicFolderSession.AccountingObject = Activity.Current.Budget;
				}
				catch (NotSupportedWithServerVersionException)
				{
					return resultFactory.CreateRedirectResult(publicFolderMailboxPrincipal.MailboxInfo.Location.RpcClientAccessServerLegacyDn, logonFlags2);
				}
				this.AddUserInformationToSession(publicFolderSession);
				KeyValuePair<ushort, Guid> mdbIdMapping = publicFolderSession.IdConverter.GetMdbIdMapping();
				PublicLogon publicLogon = new PublicLogon(publicFolderSession, clientSecurityContext, this.connectionHandler, this.NotificationHandler, openFlags, resultFactory.LogonId, localeInfo);
				disposeGuard.Add<PublicLogon>(publicLogon);
				Guid empty = Guid.Empty;
				RopResult ropResult = resultFactory.CreateSuccessfulPublicResult(publicLogon, logonFlags2, publicLogon.GetDefaultFolderIds(), LogonExtendedResponseFlags.None, null, new ReplId(mdbIdMapping.Key), mdbIdMapping.Value, empty);
				this.connectionHandler.AddLogon(publicLogon);
				disposeGuard.Success();
				result = ropResult;
			}
			return result;
		}

		private RopResult RopNotImplemented(ResultFactory resultFactory, int bugNumber, string message)
		{
			base.CheckDisposed();
			Util.ThrowOnNullArgument(resultFactory, "resultFactory");
			return RopHandlerHelper.CreateAndTraceFailedRopResult(resultFactory, new Func<IResultFactory, ErrorCode, Exception, RopResult>(RopHandler.CreateStandardFailedResult), Feature.NotImplemented(bugNumber, message), (ErrorCode)2147749887U);
		}

		private static readonly Activity implicitActivity = Activity.Create(0L);

		private static readonly string[] allowedXTCApplications = new string[]
		{
			"Client=OWA",
			"Client=TBA;Action=MoveToArchive",
			"Client=TimeBased;Action=MoveToArchive",
			"Client=Management;Action=E-Discovery",
			"ArchiveLogon",
			"Client=Monitoring;Action=Test-ArchiveConnectivity",
			"Client=MSExchangeRPC"
		};

		private readonly ConnectionHandler connectionHandler;

		private readonly IDatabaseLocationProvider databaseLocationProvider;

		private float lowestBudgetBalance = float.MaxValue;

		private uint overBudgetCount;
	}
}
