using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Protocols.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreIntegrityCheck;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	internal sealed class RopHandler : RopHandlerBase
	{
		internal MapiSession Session
		{
			get
			{
				base.CheckDisposed();
				return this.mapiSession;
			}
		}

		internal RopHandler(MapiSession mapiSession)
		{
			this.mapiSession = mapiSession;
		}

		protected override RopResult AbortSubmit(MapiContext context, MapiBase serverObject, ExchangeId folderId, ExchangeId messageId, AbortSubmitResultFactory resultFactory)
		{
			MapiLogon logon = serverObject.Logon;
			try
			{
				using (MapiFolder mapiFolder = MapiFolder.OpenFolder(context, logon, folderId))
				{
					if (mapiFolder == null)
					{
						DiagnosticContext.TraceLocation((LID)60808U);
						return resultFactory.CreateFailedResult((ErrorCode)2147747329U);
					}
					using (MapiMessage mapiMessage = new MapiMessage())
					{
						ErrorCode errorCode = mapiMessage.ConfigureMessage(context, logon, folderId, messageId, MessageConfigureFlags.None, logon.Session.CodePage);
						if (errorCode == ErrorCode.NoError)
						{
							errorCode = mapiMessage.AbortSubmit(context);
						}
						if (errorCode != ErrorCode.NoError)
						{
							DiagnosticContext.TraceLocation((LID)36232U);
							return resultFactory.CreateFailedResult((ErrorCode)errorCode);
						}
					}
				}
			}
			finally
			{
				if (logon.SpoolerRights)
				{
					logon.SpoolerLockMessage(context, messageId, LockState.Unlocked);
				}
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult AddressTypes(MapiContext context, MapiBase serverObject, AddressTypesResultFactory resultFactory)
		{
			return resultFactory.CreateSuccessfulResult(serverObject.Logon.GetSupportedAddressTypes());
		}

		protected override RopResult CloneStream(MapiContext context, MapiStream stream, CloneStreamResultFactory resultFactory)
		{
			MapiStream mapiStream = null;
			RopResult result;
			try
			{
				ErrorCode errorCode = stream.Clone(context, out mapiStream);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)60288U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiStream);
					mapiStream = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiStream != null)
				{
					mapiStream.Dispose();
				}
			}
			return result;
		}

		protected override RopResult CollapseRow(MapiContext context, MapiViewTableBase viewTable, ExchangeId categoryId, CollapseRowResultFactory resultFactory)
		{
			int collapsedRowCount = viewTable.CollapseRow(context, categoryId);
			return resultFactory.CreateSuccessfulResult(collapsedRowCount);
		}

		protected override RopResult CommitStream(MapiContext context, MapiStream stream, CommitStreamResultFactory resultFactory)
		{
			stream.Commit(context);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult CopyFolder(MapiContext context, MapiFolder sourceParentFolder, MapiFolder destinationParentFolder, bool reportProgress, bool recurse, ExchangeId sourceSubFolderId, string folderName, out bool partiallyCompleted, CopyFolderResultFactory resultFactory)
		{
			partiallyCompleted = false;
			if (folderName == null)
			{
				throw new ArgumentNullException("folderName");
			}
			bool flag = false;
			if (!object.ReferenceEquals(sourceParentFolder.Logon, destinationParentFolder.Logon))
			{
				DiagnosticContext.TraceLocation((LID)52616U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U, false);
			}
			MapiLogon logon = sourceParentFolder.Logon;
			using (MapiFolder mapiFolder = MapiFolder.OpenFolder(context, logon, sourceSubFolderId))
			{
				if (mapiFolder == null)
				{
					DiagnosticContext.TraceLocation((LID)46472U);
					return resultFactory.CreateFailedResult((ErrorCode)2147746063U, false);
				}
				if (string.IsNullOrEmpty(folderName))
				{
					folderName = mapiFolder.GetDisplayName(context);
				}
				using (CopyFolderOperation copyFolderOperation = new CopyFolderOperation(mapiFolder, destinationParentFolder, folderName, recurse))
				{
					bool flag2;
					bool flag3;
					ErrorCode errorCode;
					while (!copyFolderOperation.DoChunk(context, out flag2, out flag3, out errorCode))
					{
						partiallyCompleted = (partiallyCompleted || flag2);
						flag = (flag || flag3);
						errorCode = context.PulseMailboxOperation();
						if (errorCode != ErrorCode.NoError)
						{
							DiagnosticContext.TraceLocation((LID)62856U);
							return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
						}
					}
					partiallyCompleted = (partiallyCompleted || flag2);
					flag = (flag || flag3);
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)38280U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
					}
				}
			}
			return resultFactory.CreateSuccessfulResult(flag);
		}

		protected override RopResult CopyProperties(MapiContext context, MapiPropBagBase sourcePropertyBag, MapiPropBagBase destinationPropertyBag, bool reportProgress, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] propertyTags, CopyPropertiesResultFactory resultFactory)
		{
			if (propertyTags == null)
			{
				throw new ArgumentNullException("propertyTags");
			}
			bool flag = false;
			bool flag2 = false;
			if ((byte)(copyPropertiesFlags & ~(CopyPropertiesFlags.Move | CopyPropertiesFlags.NoReplace)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)54664U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (sourcePropertyBag.MapiObjectType != destinationPropertyBag.MapiObjectType && (sourcePropertyBag.MapiObjectType != MapiObjectType.Message || destinationPropertyBag.MapiObjectType != MapiObjectType.EmbeddedMessage) && (sourcePropertyBag.MapiObjectType != MapiObjectType.EmbeddedMessage || destinationPropertyBag.MapiObjectType != MapiObjectType.Message))
			{
				DiagnosticContext.TraceLocation((LID)42376U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (!object.ReferenceEquals(sourcePropertyBag.Logon, destinationPropertyBag.Logon))
			{
				DiagnosticContext.TraceLocation((LID)58760U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			bool replaceIfExists = (byte)(copyPropertiesFlags & CopyPropertiesFlags.NoReplace) == 0;
			List<MapiPropertyProblem> problems = null;
			StorePropTag[] propTags = LegacyHelper.ConvertFromLegacyPropTags(propertyTags, Helper.GetPropTagObjectType(sourcePropertyBag.MapiObjectType), sourcePropertyBag.Logon.MapiMailbox, true);
			if (sourcePropertyBag.MapiObjectType == MapiObjectType.Folder)
			{
				if ((byte)(copyPropertiesFlags & CopyPropertiesFlags.Move) == 1)
				{
					ErrorCode errorCode = ErrorCode.CreateWithLid((LID)10U, ErrorCodeValue.NotSupported);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				using (FolderCopyPropsOperation folderCopyPropsOperation = new FolderCopyPropsOperation((MapiFolder)sourcePropertyBag, (MapiFolder)destinationPropertyBag, propTags, replaceIfExists))
				{
					ErrorCode errorCode;
					bool flag3;
					bool flag4;
					while (!folderCopyPropsOperation.DoChunk(context, out flag3, out flag4, out errorCode))
					{
						flag = (flag || flag3);
						flag2 = (flag2 || flag4);
						errorCode = context.PulseMailboxOperation();
						if (errorCode != ErrorCode.NoError)
						{
							DiagnosticContext.TraceLocation((LID)34184U);
							return resultFactory.CreateFailedResult((ErrorCode)errorCode);
						}
					}
					bool flag5 = flag || flag3;
					bool flag6 = flag2 || flag4;
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)50568U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode);
					}
					problems = folderCopyPropsOperation.PropertyProblems;
					goto IL_1F0;
				}
			}
			sourcePropertyBag.CopyProps(context, destinationPropertyBag, propTags, replaceIfExists, ref problems);
			IL_1F0:
			return resultFactory.CreateSuccessfulResult(RcaTypeHelpers.PropertyProblemFromMapiPropertyPropblem(problems, propertyTags));
		}

		protected override RopResult CopyTo(MapiContext context, MapiPropBagBase sourcePropertyBag, MapiPropBagBase destinationPropertyBag, bool reportProgress, bool copySubObjects, CopyPropertiesFlags copyPropertiesFlags, PropertyTag[] excludedPropertyTags, CopyToResultFactory resultFactory)
		{
			if (excludedPropertyTags == null)
			{
				throw new ArgumentNullException("excludedPropertyTags");
			}
			CopyToFlags copyToFlags = CopyToFlags.None;
			bool flag = false;
			bool flag2 = false;
			if ((byte)(copyPropertiesFlags & ~(CopyPropertiesFlags.Move | CopyPropertiesFlags.NoReplace)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)47496U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (sourcePropertyBag.MapiObjectType != destinationPropertyBag.MapiObjectType && (sourcePropertyBag.MapiObjectType != MapiObjectType.Message || destinationPropertyBag.MapiObjectType != MapiObjectType.EmbeddedMessage) && (sourcePropertyBag.MapiObjectType != MapiObjectType.EmbeddedMessage || destinationPropertyBag.MapiObjectType != MapiObjectType.Message))
			{
				DiagnosticContext.TraceLocation((LID)43580U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (!object.ReferenceEquals(sourcePropertyBag.Logon, destinationPropertyBag.Logon))
			{
				DiagnosticContext.TraceLocation((LID)63880U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if ((byte)(copyPropertiesFlags & CopyPropertiesFlags.Move) != 0)
			{
				copyToFlags |= CopyToFlags.MoveProperties;
			}
			if ((byte)(copyPropertiesFlags & CopyPropertiesFlags.NoReplace) != 0)
			{
				copyToFlags |= CopyToFlags.DoNotReplaceProperties;
			}
			if (sourcePropertyBag.MapiObjectType == MapiObjectType.Message || sourcePropertyBag.MapiObjectType == MapiObjectType.EmbeddedMessage)
			{
				if (copySubObjects)
				{
					copyToFlags |= (CopyToFlags.CopyRecipients | CopyToFlags.CopyAttachments);
				}
			}
			else if (sourcePropertyBag.MapiObjectType == MapiObjectType.Folder)
			{
				if (copySubObjects && (byte)(copyPropertiesFlags & CopyPropertiesFlags.NoReplace) == 0)
				{
					copyToFlags |= (CopyToFlags.CopyHierarchy | CopyToFlags.CopyContent | CopyToFlags.CopyHiddenItems);
				}
			}
			else
			{
				if (sourcePropertyBag.MapiObjectType != MapiObjectType.Attachment)
				{
					DiagnosticContext.TraceLocation((LID)55688U);
					return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
				}
				copyToFlags |= CopyToFlags.CopyEmbeddedMessage;
			}
			List<MapiPropertyProblem> problems = null;
			StorePropTag[] propTagsExclude = LegacyHelper.ConvertFromLegacyPropTags(excludedPropertyTags, Helper.GetPropTagObjectType(sourcePropertyBag.MapiObjectType), sourcePropertyBag.Logon.MapiMailbox, true);
			if (sourcePropertyBag.MapiObjectType == MapiObjectType.Folder)
			{
				using (FolderCopyToOperation folderCopyToOperation = new FolderCopyToOperation((MapiFolder)sourcePropertyBag, (MapiFolder)destinationPropertyBag, propTagsExclude, copyToFlags))
				{
					bool flag3;
					bool flag4;
					ErrorCode errorCode;
					while (!folderCopyToOperation.DoChunk(context, out flag3, out flag4, out errorCode))
					{
						flag = (flag || flag3);
						flag2 = (flag2 || flag4);
						errorCode = context.PulseMailboxOperation();
						if (errorCode != ErrorCode.NoError)
						{
							DiagnosticContext.TraceLocation((LID)43400U);
							return resultFactory.CreateFailedResult((ErrorCode)errorCode);
						}
					}
					bool flag5 = flag || flag3;
					bool flag6 = flag2 || flag4;
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)59784U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode);
					}
					problems = folderCopyToOperation.PropertyProblems;
					goto IL_23C;
				}
			}
			sourcePropertyBag.CopyTo(context, destinationPropertyBag, propTagsExclude, copyToFlags, ref problems);
			IL_23C:
			return resultFactory.CreateSuccessfulResult(RcaTypeHelpers.PropertyProblemFromMapiPropertyPropblem(problems, null));
		}

		protected override RopResult CreateAttachment(MapiContext context, MapiMessage message, CreateAttachmentResultFactory resultFactory)
		{
			MapiAttachment mapiAttachment = null;
			RopResult result;
			try
			{
				ErrorCode errorCode = message.CreateAttachment(context, out mapiAttachment);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)35208U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					uint attachmentNumber = (uint)mapiAttachment.GetAttachmentNumber();
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiAttachment, attachmentNumber);
					mapiAttachment = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiAttachment != null)
				{
					mapiAttachment.Dispose();
				}
			}
			return result;
		}

		protected override RopResult CreateBookmark(MapiContext context, MapiViewTableBase view, CreateBookmarkResultFactory resultFactory)
		{
			if (view.MapiObjectType != MapiObjectType.MessageView && view.MapiObjectType != MapiObjectType.FolderView)
			{
				DiagnosticContext.TraceLocation((LID)51592U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			byte[] bookmark = view.CreateBookmark();
			return resultFactory.CreateSuccessfulResult(bookmark);
		}

		protected override RopResult CreateFolder(MapiContext context, MapiFolder parentFolder, FolderType folderType, CreateFolderFlags flags, string displayName, string folderComment, StoreLongTermId? longTermId, CreateFolderResultFactory resultFactory)
		{
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			if (folderComment == null)
			{
				throw new ArgumentNullException("folderComment");
			}
			if (FolderType.Normal != folderType && FolderType.Search != folderType)
			{
				DiagnosticContext.TraceLocation((LID)45448U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!EnumValidator.IsValidValue<CreateFolderFlags>(flags))
			{
				DiagnosticContext.TraceLocation((LID)61832U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (string.Empty == displayName)
			{
				DiagnosticContext.TraceLocation((LID)37256U);
				return resultFactory.CreateFailedResult(ErrorCode.BadFolderName);
			}
			if ((flags & CreateFolderFlags.InstantSearch) != CreateFolderFlags.None && (folderType != FolderType.Search || (flags & ~(CreateFolderFlags.InstantSearch | CreateFolderFlags.OptimizedConversationSearch)) != CreateFolderFlags.None))
			{
				DiagnosticContext.TraceLocation((LID)47600U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if ((flags & CreateFolderFlags.CreatePublicFolderDumpster) != CreateFolderFlags.None && (folderType == FolderType.Search || !parentFolder.Logon.MapiMailbox.IsPublicFolderMailbox || context.ClientType != ClientType.Migration))
			{
				DiagnosticContext.TraceLocation((LID)41020U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (parentFolder.IsInstantSearch)
			{
				DiagnosticContext.TraceLocation((LID)63984U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (longTermId != null && !parentFolder.Logon.IsMoveUser && !parentFolder.Logon.MapiMailbox.IsPublicFolderMailbox)
			{
				DiagnosticContext.TraceLocation((LID)53640U);
				return resultFactory.CreateFailedResult((ErrorCode)2147749887U);
			}
			bool flag = parentFolder.Logon.AllowsDuplicateFolderNames && CreateFolderFlags.None == (flags & CreateFolderFlags.OpenIfExists);
			MapiFolder mapiFolder = null;
			RopResult result;
			try
			{
				MapiLogon logon = parentFolder.Logon;
				bool existed = false;
				ExchangeId exchangeId = ExchangeId.Null;
				if (!flag)
				{
					exchangeId = Folder.FindFolderIdByName(context, parentFolder.Fid, displayName, logon.StoreMailbox);
				}
				if (exchangeId.IsValid)
				{
					existed = true;
					if ((flags & CreateFolderFlags.OpenIfExists) == CreateFolderFlags.None)
					{
						if (!parentFolder.Logon.AllowsDuplicateFolderNames)
						{
							DiagnosticContext.TraceLong((LID)41352U, (ulong)DateTime.UtcNow.Ticks);
							DiagnosticContext.TraceDword((LID)55033U, (uint)logon.InTransitStatus);
							return resultFactory.CreateFailedResult((ErrorCode)2147747332U);
						}
						if ((flags & CreateFolderFlags.InstantSearch) != CreateFolderFlags.None)
						{
							DiagnosticContext.TraceLocation((LID)55792U);
							return resultFactory.CreateFailedResult((ErrorCode)2147747332U);
						}
					}
					else
					{
						mapiFolder = MapiFolder.OpenFolder(context, logon, exchangeId);
					}
				}
				if ((flags & CreateFolderFlags.CreatePublicFolderDumpster) != CreateFolderFlags.None && logon.MapiMailbox.SharedState.MailboxType != MailboxInfo.MailboxType.PublicFolderPrimary)
				{
					DiagnosticContext.TraceLocation((LID)53308U);
					result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
				}
				else
				{
					if (mapiFolder == null)
					{
						FolderConfigureFlags folderConfigureFlags = FolderConfigureFlags.None;
						if (FolderType.Search == folderType)
						{
							folderConfigureFlags |= FolderConfigureFlags.CreateSearchFolder;
							if ((flags & CreateFolderFlags.InstantSearch) != CreateFolderFlags.None)
							{
								folderConfigureFlags |= FolderConfigureFlags.InstantSearch;
								if ((flags & CreateFolderFlags.OptimizedConversationSearch) != CreateFolderFlags.None)
								{
									folderConfigureFlags |= FolderConfigureFlags.OptimizedConversationSearch;
								}
							}
						}
						if ((flags & CreateFolderFlags.InternalAccess) != CreateFolderFlags.None)
						{
							folderConfigureFlags |= FolderConfigureFlags.InternalAccess;
						}
						ExchangeId exchangeId2 = ExchangeId.Zero;
						if (longTermId != null)
						{
							ulong itemNbr = ExchangeIdHelpers.GlobcntFromByteArray(longTermId.Value.GlobCount, 0U);
							exchangeId2 = ExchangeId.Create(context, logon.MapiMailbox.StoreMailbox.ReplidGuidMap, longTermId.Value.Guid, itemNbr);
						}
						ErrorCode errorCode = MapiFolder.CreateFolder(context, logon, ref exchangeId2, false, folderConfigureFlags, parentFolder, MapiFolder.ManageAssociatedDumpsterFolder(logon, (flags & CreateFolderFlags.CreatePublicFolderDumpster) != CreateFolderFlags.None), out mapiFolder);
						if (errorCode != ErrorCode.NoError)
						{
							DiagnosticContext.TraceLocation((LID)35904U);
							return resultFactory.CreateFailedResult((ErrorCode)errorCode);
						}
						mapiFolder.StoreFolder.SetName(context, displayName);
						mapiFolder.StoreFolder.SetComment(context, folderComment);
						exchangeId = mapiFolder.Fid;
					}
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiFolder, RcaTypeHelpers.ExchangeIdToStoreId(exchangeId), existed, false, null);
					mapiFolder = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiFolder != null)
				{
					mapiFolder.Dispose();
				}
			}
			return result;
		}

		protected override RopResult CreateMessage(MapiContext context, MapiBase serverObject, ushort codePageId, ExchangeId folderId, bool createAssociated, CreateMessageResultFactory resultFactory)
		{
			MapiLogon mapiLogon = serverObject as MapiLogon;
			if (mapiLogon == null)
			{
				MapiFolder mapiFolder = serverObject as MapiFolder;
				if (mapiFolder != null)
				{
					mapiLogon = mapiFolder.Logon;
				}
			}
			if (mapiLogon == null)
			{
				DiagnosticContext.TraceLocation((LID)57736U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (codePageId != 4095 && codePageId != 0)
			{
				DiagnosticContext.TraceLocation((LID)33160U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			MapiMessage mapiMessage = null;
			CodePage codePage = mapiLogon.Session.CodePage;
			MessageConfigureFlags messageConfigureFlags = MessageConfigureFlags.CreateNewMessage;
			messageConfigureFlags |= (createAssociated ? MessageConfigureFlags.IsAssociated : MessageConfigureFlags.None);
			RopResult result;
			try
			{
				mapiMessage = new MapiMessage();
				ErrorCode errorCode = mapiMessage.ConfigureMessage(context, mapiLogon, folderId, ExchangeId.Zero, messageConfigureFlags, codePage);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)49544U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiMessage, null);
					mapiMessage = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiMessage != null)
				{
					mapiMessage.Dispose();
				}
			}
			return result;
		}

		protected override RopResult CreateMessageExtended(MapiContext context, MapiBase serverObject, ushort codePageId, ExchangeId folderId, CreateMessageExtendedFlags createFlags, CreateMessageExtendedResultFactory resultFactory)
		{
			MapiLogon mapiLogon = serverObject as MapiLogon;
			if (mapiLogon == null)
			{
				MapiFolder mapiFolder = serverObject as MapiFolder;
				if (mapiFolder != null)
				{
					mapiLogon = mapiFolder.Logon;
				}
			}
			if (mapiLogon == null)
			{
				DiagnosticContext.TraceLocation((LID)49032U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (codePageId != 4095 && codePageId != 0)
			{
				DiagnosticContext.TraceLocation((LID)65416U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			MapiMessage mapiMessage = null;
			CodePage codePage = mapiLogon.Session.CodePage;
			MessageConfigureFlags messageConfigureFlags = MessageConfigureFlags.CreateNewMessage;
			if ((createFlags & CreateMessageExtendedFlags.ClientAssociated) == CreateMessageExtendedFlags.ClientAssociated)
			{
				messageConfigureFlags |= MessageConfigureFlags.IsAssociated;
			}
			if (mapiLogon.ExchangeTransportServiceRights && (createFlags & CreateMessageExtendedFlags.ContentAggregation) == CreateMessageExtendedFlags.ContentAggregation)
			{
				messageConfigureFlags |= MessageConfigureFlags.IsContentAggregation;
			}
			RopResult result;
			try
			{
				mapiMessage = new MapiMessage();
				ErrorCode errorCode = mapiMessage.ConfigureMessage(context, mapiLogon, folderId, ExchangeId.Zero, messageConfigureFlags, codePage);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)40840U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiMessage, null);
					mapiMessage = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiMessage != null)
				{
					mapiMessage.Dispose();
				}
			}
			return result;
		}

		protected override RopResult DeleteAttachment(MapiContext context, MapiMessage message, uint attachmentNumber, DeleteAttachmentResultFactory resultFactory)
		{
			ErrorCode errorCode = message.DeleteAttachment(context, (int)attachmentNumber);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)57224U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult DeleteFolder(MapiContext context, MapiFolder folder, DeleteFolderFlags deleteFolderFlags, ExchangeId folderId, DeleteFolderResultFactory resultFactory)
		{
			bool flag = false;
			bool flag2 = false;
			if (folder == null)
			{
				DiagnosticContext.TraceLocation((LID)44936U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if ((byte)(deleteFolderFlags & ~(DeleteFolderFlags.DeleteMessages | DeleteFolderFlags.DeleteFolders | DeleteFolderFlags.HardDelete)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)61320U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			MapiLogon logon = folder.Logon;
			using (MapiFolder mapiFolder = MapiFolder.OpenFolder(context, logon, folderId))
			{
				if (mapiFolder == null || mapiFolder.GetParentFid(context) != folder.Fid)
				{
					DiagnosticContext.TraceLocation((LID)53128U);
					return resultFactory.CreateFailedResult((ErrorCode)2147746063U);
				}
				using (DeleteFolderOperation deleteFolderOperation = new DeleteFolderOperation(mapiFolder, 0 != (byte)(deleteFolderFlags & DeleteFolderFlags.DeleteFolders), 0 != (byte)(deleteFolderFlags & DeleteFolderFlags.DeleteMessages), MapiFolder.ManageAssociatedDumpsterFolder(logon, false), false))
				{
					bool flag3;
					bool flag4;
					ErrorCode errorCode;
					while (!deleteFolderOperation.DoChunk(context, out flag3, out flag4, out errorCode))
					{
						flag = (flag || flag3);
						flag2 = (flag2 || flag4);
						errorCode = context.PulseMailboxOperation();
						if (errorCode != ErrorCode.NoError)
						{
							DiagnosticContext.TraceLocation((LID)46984U);
							return resultFactory.CreateFailedResult((ErrorCode)errorCode);
						}
					}
					bool flag5 = flag || flag3;
					flag2 = (flag2 || flag4);
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)633680U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode);
					}
				}
			}
			return resultFactory.CreateSuccessfulResult(flag2);
		}

		protected override RopResult DeleteMessages(MapiContext context, MapiFolder folder, bool reportProgress, bool sendNonReadNotification, ExchangeId[] messageIds, out bool partiallyCompleted, DeleteMessagesResultFactory resultFactory)
		{
			partiallyCompleted = false;
			bool flag = false;
			if (messageIds == null || messageIds.Length == 0)
			{
				DiagnosticContext.TraceLocation((LID)38792U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U, false);
			}
			using (DeleteMessagesOperation deleteMessagesOperation = new DeleteMessagesOperation(folder, messageIds, sendNonReadNotification))
			{
				bool flag2;
				bool flag3;
				ErrorCode errorCode;
				while (!deleteMessagesOperation.DoChunk(context, out flag2, out flag3, out errorCode))
				{
					partiallyCompleted = (partiallyCompleted || flag2 || flag3);
					flag = (flag || flag3);
					errorCode = context.PulseMailboxOperation();
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)55176U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
					}
				}
				partiallyCompleted = (partiallyCompleted || flag2 || flag3);
				flag = (flag || flag3);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)45928U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
				}
			}
			return resultFactory.CreateSuccessfulResult(flag);
		}

		protected override RopResult DeleteProperties(MapiContext context, MapiPropBagBase propertyBag, PropertyTag[] propertyTags, DeletePropertiesResultFactory resultFactory)
		{
			if (propertyTags == null)
			{
				throw new ArgumentNullException("propertyTags");
			}
			List<MapiPropertyProblem> problems = null;
			MapiLogon logon = propertyBag.Logon;
			StorePropTag[] array = LegacyHelper.ConvertFromLegacyPropTags(propertyTags, Helper.GetPropTagObjectType(propertyBag.MapiObjectType), logon.MapiMailbox, true);
			if (array.Length > 0)
			{
				propertyBag.DeleteProps(context, array, ref problems);
				if (propertyBag.MapiObjectType != MapiObjectType.Folder && propertyBag.MapiObjectType != MapiObjectType.Logon)
				{
					problems = null;
				}
			}
			return resultFactory.CreateSuccessfulResult(RcaTypeHelpers.PropertyProblemFromMapiPropertyPropblem(problems, propertyTags));
		}

		protected override RopResult DeletePropertiesNoReplicate(MapiContext context, MapiPropBagBase propertyBag, PropertyTag[] propertyTags, DeletePropertiesNoReplicateResultFactory resultFactory)
		{
			List<MapiPropertyProblem> problems = null;
			MapiLogon logon = propertyBag.Logon;
			StorePropTag[] array = LegacyHelper.ConvertFromLegacyPropTags(propertyTags, Helper.GetPropTagObjectType(propertyBag.MapiObjectType), logon.MapiMailbox, true);
			if (array.Length > 0)
			{
				try
				{
					propertyBag.NoReplicateOperationInProgress = true;
					propertyBag.DeleteProps(context, array, ref problems);
				}
				finally
				{
					propertyBag.NoReplicateOperationInProgress = false;
				}
				if (propertyBag.MapiObjectType != MapiObjectType.Folder && propertyBag.MapiObjectType != MapiObjectType.Logon)
				{
					problems = null;
				}
			}
			return resultFactory.CreateSuccessfulResult(RcaTypeHelpers.PropertyProblemFromMapiPropertyPropblem(problems, propertyTags));
		}

		protected override RopResult EmptyFolder(MapiContext context, MapiFolder folder, bool reportProgress, EmptyFolderFlags emptyFolderFlags, out bool partiallyCompleted, EmptyFolderResultFactory resultFactory)
		{
			partiallyCompleted = false;
			bool flag = false;
			if ((byte)(emptyFolderFlags & ~(EmptyFolderFlags.Associated | EmptyFolderFlags.Force)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)59272U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U, false);
			}
			using (EmptyFolderOperation emptyFolderOperation = new EmptyFolderOperation(folder, (byte)(emptyFolderFlags & EmptyFolderFlags.Associated) != 0, MapiFolder.ManageAssociatedDumpsterFolder(folder.Logon, false), (byte)(emptyFolderFlags & EmptyFolderFlags.Force) != 0))
			{
				bool flag2;
				bool flag3;
				ErrorCode errorCode;
				while (!emptyFolderOperation.DoChunk(context, out flag2, out flag3, out errorCode))
				{
					partiallyCompleted = (partiallyCompleted || flag2);
					flag = (flag || flag3);
					errorCode = context.PulseMailboxOperation();
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)34696U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
					}
				}
				partiallyCompleted = (partiallyCompleted || flag2);
				flag = (flag || flag3);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)51080U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
				}
			}
			return resultFactory.CreateSuccessfulResult(flag);
		}

		protected override RopResult ExpandRow(MapiContext context, MapiViewTableBase viewTable, short maxRows, ExchangeId categoryId, ExpandRowResultFactory resultFactory)
		{
			if (maxRows != 0)
			{
				DiagnosticContext.TraceLocation((LID)50672U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			int expandedRowCount = viewTable.ExpandRow(context, categoryId);
			RopResult result;
			using (RowCollector rowCollector = resultFactory.CreateRowCollector())
			{
				result = resultFactory.CreateSuccessfulResult(expandedRowCount, rowCollector);
			}
			return result;
		}

		protected override RopResult FastTransferDestinationCopyOperationConfigure(MapiContext context, MapiPropBagBase propertyBag, FastTransferCopyOperation copyOperation, FastTransferCopyPropertiesFlag flags, FastTransferDestinationCopyOperationConfigureResultFactory resultFactory)
		{
			if ((byte)(flags & ~(FastTransferCopyPropertiesFlag.Move | FastTransferCopyPropertiesFlag.FastTrasferStream | FastTransferCopyPropertiesFlag.CopyMailboxPerUserData | FastTransferCopyPropertiesFlag.CopyFolderPerUserData)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)48008U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			MapiLogon logon = propertyBag.Logon;
			if ((byte)(flags & FastTransferCopyPropertiesFlag.FastTrasferStream) == 0)
			{
				Folder folderForQuotaCheck = null;
				if (propertyBag is MapiFolder)
				{
					folderForQuotaCheck = ((MapiFolder)propertyBag).StoreFolder;
				}
				else if (propertyBag is MapiMessage || propertyBag is MapiAttachment)
				{
					MapiPropBagBase mapiPropBagBase = propertyBag;
					while (!(mapiPropBagBase.ParentObject is MapiLogon))
					{
						mapiPropBagBase = mapiPropBagBase.ParentObject;
					}
					folderForQuotaCheck = ((TopMessage)((MapiMessage)mapiPropBagBase).StoreMessage).ParentFolder;
				}
				FastTransferUploadContext fastTransferUploadContext = new FastTransferUploadContext();
				FastTransferUploadContext capturedContext = fastTransferUploadContext;
				FastTransferCopyFlag capturedFlags = (FastTransferCopyFlag)flags;
				MapiPropBagBase capturedPropertyBag = propertyBag;
				try
				{
					if (propertyBag.MapiObjectType == MapiObjectType.Message || propertyBag.MapiObjectType == MapiObjectType.EmbeddedMessage)
					{
						if (copyOperation == FastTransferCopyOperation.CopyTo)
						{
							fastTransferUploadContext.Configure(logon, delegate(MapiContext operationContext)
							{
								FastTransferMessage message = new FastTransferMessage(capturedContext, (MapiMessage)capturedPropertyBag, true, capturedFlags);
								return new FastTransferMessageCopyTo(false, message, true);
							}, null, folderForQuotaCheck);
						}
						else
						{
							if (copyOperation != FastTransferCopyOperation.CopyProperties)
							{
								DiagnosticContext.TraceLocation((LID)64392U);
								return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
							}
							fastTransferUploadContext.Configure(logon, delegate(MapiContext operationContext)
							{
								FastTransferMessage message = new FastTransferMessage(capturedContext, (MapiMessage)capturedPropertyBag, true, capturedFlags);
								return new FastTransferMessageCopyTo(false, message, true);
							}, null, folderForQuotaCheck);
						}
					}
					else if (propertyBag.MapiObjectType == MapiObjectType.Folder)
					{
						if ((capturedFlags & FastTransferCopyFlag.CopyMailboxPerUserData) != FastTransferCopyFlag.None || (capturedFlags & FastTransferCopyFlag.CopyFolderPerUserData) != FastTransferCopyFlag.None)
						{
							if (!logon.IsMoveUser)
							{
								DiagnosticContext.TraceLocation((LID)42908U);
								return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
							}
							fastTransferUploadContext.Configure(logon, delegate(MapiContext operationContext)
							{
								ExchangeId folderId = ExchangeId.Zero;
								if ((capturedFlags & FastTransferCopyFlag.CopyFolderPerUserData) != FastTransferCopyFlag.None)
								{
									folderId = ((MapiFolder)capturedPropertyBag).Fid;
								}
								PerUserTableIterator messageIteratorClient = new PerUserTableIterator(capturedContext, folderId);
								return new FastTransferMessageIterator(messageIteratorClient, true);
							}, null, folderForQuotaCheck);
						}
						else if (copyOperation == FastTransferCopyOperation.CopyTo)
						{
							fastTransferUploadContext.Configure(logon, delegate(MapiContext operationContext)
							{
								FastTransferFolder folder = new FastTransferFolder(capturedContext, (MapiFolder)capturedPropertyBag, true, capturedFlags);
								return FastTransferFolderCopyTo.CreateUploadStateMachine(folder);
							}, delegate(MapiContext operationContext)
							{
								((MapiFolder)capturedPropertyBag).StoreFolder.Flush(operationContext);
								return false;
							}, folderForQuotaCheck);
						}
						else if (copyOperation == FastTransferCopyOperation.CopyProperties)
						{
							fastTransferUploadContext.Configure(logon, delegate(MapiContext operationContext)
							{
								FastTransferFolder folder = new FastTransferFolder(capturedContext, (MapiFolder)capturedPropertyBag, true, FastTransferCopyFlag.None);
								return FastTransferFolderCopyTo.CreateUploadStateMachine(folder);
							}, delegate(MapiContext operationContext)
							{
								((MapiFolder)capturedPropertyBag).StoreFolder.Flush(operationContext);
								return false;
							}, folderForQuotaCheck);
						}
						else if (copyOperation == FastTransferCopyOperation.CopyMessages)
						{
							fastTransferUploadContext.Configure(logon, delegate(MapiContext operationContext)
							{
								FastTransferFolder messageIteratorClient = new FastTransferFolder(capturedContext, (MapiFolder)capturedPropertyBag, true, capturedFlags);
								return new FastTransferMessageIterator(messageIteratorClient, true);
							}, null, folderForQuotaCheck);
						}
						else
						{
							if (copyOperation != FastTransferCopyOperation.CopyFolder)
							{
								DiagnosticContext.TraceLocation((LID)39816U);
								return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
							}
							fastTransferUploadContext.Configure(logon, delegate(MapiContext operationContext)
							{
								FastTransferFolder folder = new FastTransferFolder(capturedContext, (MapiFolder)capturedPropertyBag, true, capturedFlags);
								return FastTransferCopyFolder.CreateUploadStateMachine(folder);
							}, null, folderForQuotaCheck);
						}
					}
					else
					{
						if (propertyBag.MapiObjectType != MapiObjectType.Attachment)
						{
							DiagnosticContext.TraceLocation((LID)43912U);
							return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
						}
						if (copyOperation == FastTransferCopyOperation.CopyTo)
						{
							fastTransferUploadContext.Configure(logon, delegate(MapiContext operationContext)
							{
								FastTransferAttachment attachment = new FastTransferAttachment(capturedContext, (MapiAttachment)capturedPropertyBag, true, capturedFlags);
								return FastTransferAttachmentCopyTo.CreateUploadStateMachine(attachment);
							}, null, folderForQuotaCheck);
						}
						else
						{
							if (copyOperation != FastTransferCopyOperation.CopyProperties)
							{
								DiagnosticContext.TraceLocation((LID)56200U);
								return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
							}
							fastTransferUploadContext.Configure(logon, delegate(MapiContext operationContext)
							{
								FastTransferAttachment attachment = new FastTransferAttachment(capturedContext, (MapiAttachment)capturedPropertyBag, true, capturedFlags);
								return FastTransferAttachmentCopyTo.CreateUploadStateMachine(attachment);
							}, null, folderForQuotaCheck);
						}
					}
					RopResult result = resultFactory.CreateSuccessfulResult(fastTransferUploadContext);
					fastTransferUploadContext = null;
					return result;
				}
				finally
				{
					if (fastTransferUploadContext != null)
					{
						fastTransferUploadContext.Dispose();
					}
				}
			}
			if (propertyBag.MapiObjectType == MapiObjectType.Logon)
			{
				FastTransferStream fastTransferStream = null;
				try
				{
					fastTransferStream = FastTransferStream.CreateNew(context, logon, FastTransferStreamMode.Upload);
					RopResult result2 = resultFactory.CreateSuccessfulResult(fastTransferStream);
					fastTransferStream = null;
					return result2;
				}
				finally
				{
					if (fastTransferStream != null)
					{
						fastTransferStream.Dispose();
					}
				}
			}
			DiagnosticContext.TraceLocation((LID)44368U);
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		protected override RopResult FastTransferDestinationPutBuffer(MapiContext context, MapiBase uploadContext, ArraySegment<byte>[] dataChunks, out ushort outputProgressCount, out ushort outputTotalStepCount, out bool outputMoveUserOperation, out ushort outputUsedBufferSize, FastTransferDestinationPutBufferResultFactory resultFactory)
		{
			outputProgressCount = 0;
			outputTotalStepCount = 0;
			outputMoveUserOperation = false;
			outputUsedBufferSize = 0;
			if (dataChunks == null)
			{
				throw new ArgumentNullException("dataChunks");
			}
			FastTransferUploadContext fastTransferUploadContext = uploadContext as FastTransferUploadContext;
			QuotaType quotaType = QuotaType.StorageShutoff;
			if (fastTransferUploadContext != null && fastTransferUploadContext.FolderForQuotaCheck != null)
			{
				quotaType = (fastTransferUploadContext.FolderForQuotaCheck.IsDumpsterMarkedFolder(context) ? QuotaType.DumpsterShutoff : QuotaType.StorageShutoff);
				Quota.Enforce((LID)42252U, context, fastTransferUploadContext.FolderForQuotaCheck, QuotaType.StorageOverQuotaLimit, false);
			}
			Quota.Enforce((LID)46348U, context, uploadContext.Logon.StoreMailbox, quotaType, false);
			if (fastTransferUploadContext != null)
			{
				foreach (ArraySegment<byte> buffer in dataChunks)
				{
					fastTransferUploadContext.PutNextBuffer(context, buffer);
					outputUsedBufferSize = (ushort)buffer.Count;
				}
				fastTransferUploadContext.Flush(context);
				return resultFactory.CreateSuccessfulResult(0, 1, fastTransferUploadContext.IsMovingMailbox, outputUsedBufferSize);
			}
			FastTransferStream fastTransferStream = uploadContext as FastTransferStream;
			if (fastTransferStream != null)
			{
				foreach (ArraySegment<byte> buffer2 in dataChunks)
				{
					fastTransferStream.PutNextBuffer(context, buffer2);
					outputUsedBufferSize = (ushort)buffer2.Count;
				}
				fastTransferStream.Flush(context);
				return resultFactory.CreateSuccessfulResult(0, 1, false, outputUsedBufferSize);
			}
			DiagnosticContext.TraceLocation((LID)60752U);
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		protected override RopResult FastTransferDestinationPutBufferExtended(MapiContext context, MapiBase uploadContext, ArraySegment<byte>[] dataChunks, out uint outputProgressCount, out uint outputTotalStepCount, out bool outputMoveUserOperation, out ushort outputUsedBufferSize, FastTransferDestinationPutBufferExtendedResultFactory resultFactory)
		{
			outputProgressCount = 0U;
			outputTotalStepCount = 0U;
			outputMoveUserOperation = false;
			outputUsedBufferSize = 0;
			if (dataChunks == null)
			{
				throw new ArgumentNullException("dataChunks");
			}
			FastTransferUploadContext fastTransferUploadContext = uploadContext as FastTransferUploadContext;
			QuotaType quotaType = QuotaType.StorageShutoff;
			if (fastTransferUploadContext != null && fastTransferUploadContext.FolderForQuotaCheck != null)
			{
				quotaType = (fastTransferUploadContext.FolderForQuotaCheck.IsDumpsterMarkedFolder(context) ? QuotaType.DumpsterShutoff : QuotaType.StorageShutoff);
				Quota.Enforce((LID)58636U, context, fastTransferUploadContext.FolderForQuotaCheck, QuotaType.StorageOverQuotaLimit, false);
			}
			Quota.Enforce((LID)62732U, context, uploadContext.Logon.StoreMailbox, quotaType, false);
			if (fastTransferUploadContext != null)
			{
				foreach (ArraySegment<byte> buffer in dataChunks)
				{
					fastTransferUploadContext.PutNextBuffer(context, buffer);
					outputUsedBufferSize = (ushort)buffer.Count;
				}
				fastTransferUploadContext.Flush(context);
				return resultFactory.CreateSuccessfulResult(0U, 1U, fastTransferUploadContext.IsMovingMailbox, outputUsedBufferSize);
			}
			FastTransferStream fastTransferStream = uploadContext as FastTransferStream;
			if (fastTransferStream != null)
			{
				foreach (ArraySegment<byte> buffer2 in dataChunks)
				{
					fastTransferStream.PutNextBuffer(context, buffer2);
					outputUsedBufferSize = (ushort)buffer2.Count;
				}
				fastTransferStream.Flush(context);
				return resultFactory.CreateSuccessfulResult(0U, 1U, false, outputUsedBufferSize);
			}
			DiagnosticContext.TraceLocation((LID)36176U);
			return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
		}

		protected override RopResult FastTransferGetIncrementalState(MapiContext context, MapiBase serverObject, FastTransferGetIncrementalStateResultFactory resultFactory)
		{
			FastTransferDownloadContext fastTransferDownloadContext = null;
			RopResult result;
			try
			{
				IcsDownloadContext icsDownloadContext = serverObject as IcsDownloadContext;
				ErrorCode errorCode;
				if (icsDownloadContext != null)
				{
					errorCode = icsDownloadContext.OpenIcsStateDownloadContext(out fastTransferDownloadContext);
				}
				else
				{
					IcsUploadContext icsUploadContext = serverObject as IcsUploadContext;
					if (icsUploadContext != null)
					{
						errorCode = icsUploadContext.OpenIcsStateDownloadContext(out fastTransferDownloadContext);
					}
					else
					{
						errorCode = ErrorCode.CreateNotSupported((LID)64664U);
					}
				}
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)60296U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownloadContext);
					fastTransferDownloadContext = null;
					result = ropResult;
				}
			}
			finally
			{
				if (fastTransferDownloadContext != null)
				{
					fastTransferDownloadContext.Dispose();
				}
			}
			return result;
		}

		protected override RopResult FastTransferSourceCopyFolder(MapiContext context, MapiFolder folder, FastTransferCopyFolderFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyFolderResultFactory resultFactory)
		{
			if ((byte)(flags & ~(FastTransferCopyFolderFlag.Move | FastTransferCopyFolderFlag.CopySubFolders)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)35720U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!EnumValidator.IsValidValue<FastTransferSendOption>(sendOptions))
			{
				DiagnosticContext.TraceLocation((LID)52104U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			MapiLogon logon = folder.Logon;
			FastTransferDownloadContext fastTransferDownloadContext = null;
			RopResult result;
			try
			{
				fastTransferDownloadContext = new FastTransferDownloadContext(Array<PropertyTag>.Empty);
				FastTransferDownloadContext capturedContext = fastTransferDownloadContext;
				FastTransferCopyFolderFlag capturedFlags = flags;
				MapiFolder capturedFolder = folder;
				fastTransferDownloadContext.Configure(logon, sendOptions, delegate(MapiContext operationContext)
				{
					FastTransferFolder folder2 = new FastTransferFolder(capturedContext, capturedFolder, true, new HashSet<StorePropTag>
					{
						PropTag.Folder.DisplayName
					}, true, (FastTransferCopyFlag)capturedFlags);
					FastTransferFolderContentBase.IncludeSubObject includeSubObject = FastTransferFolderContentBase.IncludeSubObject.Messages | FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages;
					if ((byte)(capturedFlags & FastTransferCopyFolderFlag.CopySubFolders) != 0 || (byte)(capturedFlags & FastTransferCopyFolderFlag.Move) != 0)
					{
						includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Subfolders;
					}
					return FastTransferCopyFolder.CreateDownloadStateMachine(folder2, includeSubObject);
				});
				RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownloadContext);
				fastTransferDownloadContext = null;
				result = ropResult;
			}
			finally
			{
				if (fastTransferDownloadContext != null)
				{
					fastTransferDownloadContext.Dispose();
				}
			}
			return result;
		}

		protected override RopResult FastTransferSourceCopyMessages(MapiContext context, MapiFolder folder, ExchangeId[] messageIds, FastTransferCopyMessagesFlag flags, FastTransferSendOption sendOptions, FastTransferSourceCopyMessagesResultFactory resultFactory)
		{
			if ((byte)(flags & ~(FastTransferCopyMessagesFlag.Move | FastTransferCopyMessagesFlag.BestBody | FastTransferCopyMessagesFlag.SendEntryId)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)45960U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			FastTransferDownloadContext fastTransferDownloadContext = null;
			RopResult result;
			try
			{
				MapiLogon logon = folder.Logon;
				fastTransferDownloadContext = new FastTransferDownloadContext(Array<PropertyTag>.Empty);
				FastTransferDownloadContext capturedContext = fastTransferDownloadContext;
				FastTransferCopyMessagesFlag capturedFlags = flags;
				MapiFolder capturedFolder = folder;
				fastTransferDownloadContext.Configure(logon, sendOptions, delegate(MapiContext operationContext)
				{
					FastTransferFolder messageIterator = new FastTransferFolder(capturedContext, capturedFolder, (FastTransferCopyFlag)capturedFlags, messageIds);
					return new FastTransferMessageIterator(messageIterator, capturedFlags, true);
				});
				RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownloadContext);
				fastTransferDownloadContext = null;
				result = ropResult;
			}
			finally
			{
				if (fastTransferDownloadContext != null)
				{
					fastTransferDownloadContext.Dispose();
				}
			}
			return result;
		}

		protected override RopResult FastTransferSourceCopyProperties(MapiContext context, MapiPropBagBase propertyBag, byte level, FastTransferCopyPropertiesFlag flags, FastTransferSendOption sendOptions, PropertyTag[] propertyTags, FastTransferSourceCopyPropertiesResultFactory resultFactory)
		{
			if ((byte)(flags & ~FastTransferCopyPropertiesFlag.Move) != 0)
			{
				DiagnosticContext.TraceLocation((LID)37768U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			MapiLogon logon = propertyBag.Logon;
			FastTransferDownloadContext fastTransferDownloadContext = null;
			RopResult result;
			try
			{
				StorePropTag[] collection = LegacyHelper.ConvertFromLegacyPropTags(propertyTags, Helper.GetPropTagObjectType(propertyBag.MapiObjectType), logon.MapiMailbox, true);
				fastTransferDownloadContext = new FastTransferDownloadContext(Array<PropertyTag>.Empty);
				FastTransferDownloadContext capturedContext = fastTransferDownloadContext;
				MapiPropBagBase capturedPropertyBag = propertyBag;
				HashSet<StorePropTag> capturedIncludeSet = new HashSet<StorePropTag>(collection);
				byte capturedLevel = level;
				if (propertyBag.MapiObjectType == MapiObjectType.Message || propertyBag.MapiObjectType == MapiObjectType.EmbeddedMessage)
				{
					bool capturedExcludeAttachments = level > 0;
					bool capturedExcludeRecipients = level > 0;
					if (level == 0)
					{
						capturedExcludeAttachments = !capturedIncludeSet.Contains(PropTag.Message.MessageAttachments);
						capturedExcludeRecipients = !capturedIncludeSet.Contains(PropTag.Message.MessageRecipients);
					}
					fastTransferDownloadContext.Configure(logon, sendOptions, delegate(MapiContext operationContext)
					{
						FastTransferMessage message = new FastTransferMessage(capturedContext, (MapiMessage)capturedPropertyBag, false, capturedIncludeSet, capturedExcludeAttachments, capturedExcludeRecipients, true, FastTransferCopyFlag.None);
						return new FastTransferMessageCopyTo(capturedExcludeAttachments && capturedExcludeRecipients, message, true);
					});
				}
				else if (propertyBag.MapiObjectType == MapiObjectType.Folder)
				{
					fastTransferDownloadContext.Configure(logon, sendOptions, delegate(MapiContext operationContext)
					{
						FastTransferFolder folder = new FastTransferFolder(capturedContext, (MapiFolder)capturedPropertyBag, false, capturedIncludeSet, true, FastTransferCopyFlag.None);
						FastTransferFolderContentBase.IncludeSubObject includeSubObject = FastTransferFolderContentBase.IncludeSubObject.None;
						if (capturedLevel == 0)
						{
							if (capturedIncludeSet.Contains(PropTag.Folder.ContainerContents))
							{
								includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Messages;
							}
							if (capturedIncludeSet.Contains(PropTag.Folder.FolderAssociatedContents))
							{
								includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages;
							}
							if (capturedIncludeSet.Contains(PropTag.Folder.ContainerHierarchy))
							{
								includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Subfolders;
							}
						}
						return FastTransferFolderCopyTo.CreateDownloadStateMachine(folder, includeSubObject);
					});
				}
				else
				{
					if (propertyBag.MapiObjectType != MapiObjectType.Attachment)
					{
						DiagnosticContext.TraceLocation((LID)62344U);
						return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
					}
					fastTransferDownloadContext.Configure(logon, sendOptions, delegate(MapiContext operationContext)
					{
						FastTransferAttachment attachment = new FastTransferAttachment(capturedContext, (MapiAttachment)capturedPropertyBag, false, capturedIncludeSet, true, FastTransferCopyFlag.None);
						return FastTransferAttachmentCopyTo.CreateDownloadStateMachine(attachment);
					});
				}
				RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownloadContext);
				fastTransferDownloadContext = null;
				result = ropResult;
			}
			finally
			{
				if (fastTransferDownloadContext != null)
				{
					fastTransferDownloadContext.Dispose();
				}
			}
			return result;
		}

		protected override RopResult FastTransferSourceCopyTo(MapiContext context, MapiPropBagBase propertyBag, byte level, FastTransferCopyFlag flags, FastTransferSendOption sendOptions, PropertyTag[] excludedPropertyTags, FastTransferSourceCopyToResultFactory resultFactory)
		{
			if (excludedPropertyTags == null)
			{
				throw new ArgumentNullException("excludedPropertyTags");
			}
			if ((flags & ~(FastTransferCopyFlag.CopyMailboxPerUserData | FastTransferCopyFlag.CopyFolderPerUserData | FastTransferCopyFlag.MoveUser | FastTransferCopyFlag.ForceUnicode | FastTransferCopyFlag.FastTrasferStream | FastTransferCopyFlag.BestBody | FastTransferCopyFlag.Unicode)) != FastTransferCopyFlag.None)
			{
				DiagnosticContext.TraceLocation((LID)54152U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!EnumValidator.IsValidValue<FastTransferSendOption>(sendOptions))
			{
				DiagnosticContext.TraceLocation((LID)41864U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			FastTransferDownloadContext fastTransferDownloadContext = null;
			RopResult result;
			try
			{
				MapiLogon logon = propertyBag.Logon;
				if ((flags & FastTransferCopyFlag.FastTrasferStream) == FastTransferCopyFlag.None)
				{
					StorePropTag[] collection = LegacyHelper.ConvertFromLegacyPropTags(excludedPropertyTags, Helper.GetPropTagObjectType(propertyBag.MapiObjectType), logon.MapiMailbox, true);
					fastTransferDownloadContext = new FastTransferDownloadContext(excludedPropertyTags);
					FastTransferDownloadContext capturedContext = fastTransferDownloadContext;
					FastTransferCopyFlag capturedFlags = flags;
					MapiPropBagBase capturedPropertyBag = propertyBag;
					HashSet<StorePropTag> capturedExcludeSet = new HashSet<StorePropTag>(collection);
					byte capturedLevel = level;
					if (propertyBag.MapiObjectType == MapiObjectType.Message || propertyBag.MapiObjectType == MapiObjectType.EmbeddedMessage)
					{
						bool capturedExcludeAttachments = level > 0;
						bool capturedExcludeRecipients = level > 0;
						if (level == 0)
						{
							capturedExcludeAttachments = capturedExcludeSet.Contains(PropTag.Message.MessageAttachments);
							capturedExcludeRecipients = capturedExcludeSet.Contains(PropTag.Message.MessageRecipients);
						}
						fastTransferDownloadContext.Configure(logon, sendOptions, delegate(MapiContext operationContext)
						{
							FastTransferMessage message = new FastTransferMessage(capturedContext, (MapiMessage)capturedPropertyBag, true, capturedExcludeSet, capturedExcludeAttachments, capturedExcludeRecipients, true, capturedFlags);
							return new FastTransferMessageCopyTo(capturedExcludeAttachments && capturedExcludeRecipients, message, true);
						});
					}
					else if (propertyBag.MapiObjectType == MapiObjectType.Folder)
					{
						if ((capturedFlags & FastTransferCopyFlag.CopyMailboxPerUserData) != FastTransferCopyFlag.None || (capturedFlags & FastTransferCopyFlag.CopyFolderPerUserData) != FastTransferCopyFlag.None)
						{
							if (!logon.IsMoveUser)
							{
								DiagnosticContext.TraceLocation((LID)59292U);
								return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
							}
							fastTransferDownloadContext.Configure(logon, sendOptions, delegate(MapiContext operationContext)
							{
								ExchangeId folderId = ExchangeId.Zero;
								if ((capturedFlags & FastTransferCopyFlag.CopyFolderPerUserData) != FastTransferCopyFlag.None)
								{
									folderId = ((MapiFolder)capturedPropertyBag).Fid;
								}
								PerUserTableIterator messageIterator = new PerUserTableIterator(capturedContext, folderId);
								return new FastTransferMessageIterator(messageIterator, FastTransferCopyMessagesFlag.None, true);
							});
						}
						else
						{
							fastTransferDownloadContext.Configure(logon, sendOptions, delegate(MapiContext operationContext)
							{
								FastTransferFolder folder = new FastTransferFolder(capturedContext, (MapiFolder)capturedPropertyBag, true, capturedExcludeSet, true, capturedFlags);
								FastTransferFolderContentBase.IncludeSubObject includeSubObject = FastTransferFolderContentBase.IncludeSubObject.None;
								if (capturedLevel == 0)
								{
									if (!capturedExcludeSet.Contains(PropTag.Folder.ContainerContents))
									{
										includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Messages;
									}
									if (!capturedExcludeSet.Contains(PropTag.Folder.FolderAssociatedContents))
									{
										includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.AssociatedMessages;
									}
									if (!capturedExcludeSet.Contains(PropTag.Folder.ContainerHierarchy))
									{
										includeSubObject |= FastTransferFolderContentBase.IncludeSubObject.Subfolders;
									}
								}
								return FastTransferFolderCopyTo.CreateDownloadStateMachine(folder, includeSubObject);
							});
						}
					}
					else
					{
						if (propertyBag.MapiObjectType != MapiObjectType.Attachment)
						{
							DiagnosticContext.TraceLocation((LID)58248U);
							return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
						}
						fastTransferDownloadContext.Configure(logon, sendOptions, delegate(MapiContext operationContext)
						{
							FastTransferAttachment attachment = new FastTransferAttachment(capturedContext, (MapiAttachment)capturedPropertyBag, true, capturedExcludeSet, true, capturedFlags);
							return FastTransferAttachmentCopyTo.CreateDownloadStateMachine(attachment);
						});
					}
					RopResult ropResult = resultFactory.CreateSuccessfulResult(fastTransferDownloadContext);
					fastTransferDownloadContext = null;
					result = ropResult;
				}
				else
				{
					if (propertyBag.MapiObjectType == MapiObjectType.Logon)
					{
						FastTransferStream fastTransferStream = null;
						try
						{
							fastTransferStream = FastTransferStream.CreateNew(context, logon, FastTransferStreamMode.Download);
							RopResult result2 = resultFactory.CreateSuccessfulResult(fastTransferStream);
							fastTransferStream = null;
							return result2;
						}
						finally
						{
							if (fastTransferStream != null)
							{
								fastTransferStream.Dispose();
							}
						}
					}
					DiagnosticContext.TraceLocation((LID)52560U);
					result = resultFactory.CreateFailedResult((ErrorCode)2147746050U);
				}
			}
			finally
			{
				if (fastTransferDownloadContext != null)
				{
					fastTransferDownloadContext.Dispose();
				}
			}
			return result;
		}

		protected override RopResult FastTransferSourceGetBuffer(MapiContext context, MapiBase downloadContext, ushort bufferSize, FastTransferSourceGetBufferResultFactory resultFactory)
		{
			ArraySegment<byte> outputBuffer = resultFactory.GetOutputBuffer();
			FastTransferDownloadContext fastTransferDownloadContext = downloadContext as FastTransferDownloadContext;
			if (fastTransferDownloadContext != null)
			{
				ErrorCode errorCode = this.ChunkedPrepareIfNecessary(context, fastTransferDownloadContext.PrepareIndexes(context));
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)54300U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				int nextBuffer = fastTransferDownloadContext.GetNextBuffer(context, outputBuffer);
				return resultFactory.CreateSuccessfulResult(fastTransferDownloadContext.State, 1, 1, fastTransferDownloadContext.IsMovingMailbox, nextBuffer);
			}
			else
			{
				FastTransferStream fastTransferStream = downloadContext as FastTransferStream;
				if (fastTransferStream != null)
				{
					int nextBuffer = fastTransferStream.GetNextBuffer(context, outputBuffer);
					return resultFactory.CreateSuccessfulResult(fastTransferStream.State, 1, 1, false, nextBuffer);
				}
				DiagnosticContext.TraceLocation((LID)46416U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
		}

		protected override RopResult FastTransferSourceGetBufferExtended(MapiContext context, MapiBase downloadContext, ushort bufferSize, FastTransferSourceGetBufferExtendedResultFactory resultFactory)
		{
			ArraySegment<byte> outputBuffer = resultFactory.GetOutputBuffer();
			FastTransferDownloadContext fastTransferDownloadContext = downloadContext as FastTransferDownloadContext;
			if (fastTransferDownloadContext != null)
			{
				ErrorCode errorCode = this.ChunkedPrepareIfNecessary(context, fastTransferDownloadContext.PrepareIndexes(context));
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)42012U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				int nextBuffer = fastTransferDownloadContext.GetNextBuffer(context, outputBuffer);
				return resultFactory.CreateSuccessfulResult(fastTransferDownloadContext.State, 1U, 1U, fastTransferDownloadContext.IsMovingMailbox, nextBuffer);
			}
			else
			{
				FastTransferStream fastTransferStream = downloadContext as FastTransferStream;
				if (fastTransferStream != null)
				{
					int nextBuffer = fastTransferStream.GetNextBuffer(context, outputBuffer);
					return resultFactory.CreateSuccessfulResult(fastTransferStream.State, 1U, 1U, false, nextBuffer);
				}
				DiagnosticContext.TraceLocation((LID)62800U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
		}

		protected override RopResult FindRow(MapiContext context, MapiViewTableBase view, FindRowFlags flags, Restriction restriction, BookmarkOrigin bookmarkOrigin, byte[] bookmark, FindRowResultFactory resultFactory)
		{
			if (restriction == null || bookmarkOrigin > BookmarkOrigin.Custom || bookmarkOrigin == BookmarkOrigin.Custom != (bookmark != null))
			{
				DiagnosticContext.TraceLocation((LID)33672U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			MapiLogon logon = view.Logon;
			Restriction restriction2 = RcaTypeHelpers.RestrictionToRestriction(context, restriction, logon, view.MapiObjectType);
			Stopwatch stopwatch = (view.CorrelationId != null) ? Stopwatch.StartNew() : null;
			bool positionChanged;
			Properties properties;
			try
			{
				ErrorCode errorCode = this.ChunkedPrepareIfNecessary(context, view.PrepareIndexes(context, restriction2));
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)63800U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				if (!view.FindRow(context, (ViewSeekOrigin)bookmarkOrigin, bookmark, 0 != (byte)(flags & FindRowFlags.Backward), restriction2, out positionChanged, out properties))
				{
					DiagnosticContext.TraceLocation((LID)50056U);
					return resultFactory.CreateFailedResult((ErrorCode)2147746063U);
				}
			}
			finally
			{
				if (view.CorrelationId != null)
				{
					stopwatch.Stop();
					FullTextIndexLogger.LogViewOperation(context.DatabaseGuid, context.Diagnostics.MailboxNumber, (int)context.ClientType, view.CorrelationId.Value, FullTextIndexLogger.ViewOperationType.FindRow, stopwatch.ToTimeSpan(), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Read), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Read), view.IsOptimizedInstantSearch);
				}
			}
			RopResult result;
			using (RowCollector rowCollector = resultFactory.CreateRowCollector())
			{
				PropertyTag[] columns;
				LegacyHelper.ConvertToLegacyPropTags(view.ViewColumns, out columns);
				rowCollector.SetColumns(columns);
				MapiViewTableBase.TruncateLongValues(context, view, properties);
				bool flag;
				PropertyValue[] rowValues = RcaTypeHelpers.MassageOutgoingProperties(properties, out flag);
				rowCollector.TryAddRow(rowValues);
				result = resultFactory.CreateSuccessfulResult(positionChanged, rowCollector);
			}
			return result;
		}

		protected override RopResult FlushRecipients(MapiContext context, MapiMessage message, PropertyTag[] extraPropertyTags, RecipientRow[] recipientRows, FlushRecipientsResultFactory resultFactory)
		{
			if (recipientRows == null)
			{
				throw new ArgumentNullException("recipientRows");
			}
			MapiLogon logon = message.Logon;
			StorePropTag[] propTags = LegacyHelper.ConvertFromLegacyPropTags(extraPropertyTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient, logon.MapiMailbox, true);
			message.SetRecipientPropListExtra(0U, propTags);
			MapiPersonCollection recipients = message.GetRecipients();
			foreach (RecipientRow recipientRow in recipientRows)
			{
				if (!recipientRow.IsEmpty)
				{
					Properties? properties = RopHandler.PropertiesFromRecipientRow(recipientRow, message);
					if (properties != null)
					{
						MapiPerson item = recipients.GetItem((int)recipientRow.RecipientRowId, true);
						if (!item.IsDeleted)
						{
							List<MapiPropertyProblem> list = null;
							item.SetProps(context, properties.Value, ref list);
						}
					}
				}
				else
				{
					recipients.RemoveItem((int)recipientRow.RecipientRowId);
				}
			}
			recipients.SaveChanges(context);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult FreeBookmark(MapiContext context, MapiViewTableBase view, byte[] bookmark, FreeBookmarkResultFactory resultFactory)
		{
			if (bookmark == null)
			{
				throw new ArgumentNullException("bookmark");
			}
			if (view.MapiObjectType != MapiObjectType.MessageView && view.MapiObjectType != MapiObjectType.FolderView)
			{
				DiagnosticContext.TraceLocation((LID)48264U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			view.FreeBookmark(bookmark);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult GetAllPerUserLongTermIds(MapiContext context, MapiLogon logon, StoreLongTermId startId, GetAllPerUserLongTermIdsResultFactory resultFactory)
		{
			RopResult result;
			using (PerUserDataCollector perUserDataCollector = resultFactory.CreatePerUserDataCollector())
			{
				bool allPerUserLongTermIds = logon.GetAllPerUserLongTermIds(context, startId, new Func<PerUserData, bool>(perUserDataCollector.TryAddPerUserData));
				result = resultFactory.CreateSuccessfulResult(perUserDataCollector, allPerUserLongTermIds);
			}
			return result;
		}

		protected override RopResult GetAttachmentTable(MapiContext context, MapiMessage message, TableFlags tableFlags, GetAttachmentTableResultFactory resultFactory)
		{
			if ((byte)(tableFlags & (TableFlags.RetrieveFromIndex | TableFlags.Associated | TableFlags.Depth | TableFlags.SoftDeletes)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)64648U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			MapiViewAttachment mapiViewAttachment = null;
			RopResult result;
			try
			{
				MapiLogon logon = message.Logon;
				mapiViewAttachment = new MapiViewAttachment();
				mapiViewAttachment.Configure(context, logon, message);
				RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiViewAttachment);
				mapiViewAttachment = null;
				result = ropResult;
			}
			finally
			{
				if (mapiViewAttachment != null)
				{
					mapiViewAttachment.Dispose();
				}
			}
			return result;
		}

		protected override RopResult GetCollapseState(MapiContext context, MapiViewTableBase serverObject, ExchangeId rowId, uint rowInstanceNumber, GetCollapseStateResultFactory resultFactory)
		{
			byte[] collapseState = serverObject.GetCollapseState(context, rowId, (int)rowInstanceNumber);
			return resultFactory.CreateSuccessfulResult(collapseState);
		}

		protected override RopResult GetContentsTable(MapiContext context, MapiFolder folder, TableFlags tableFlags, GetContentsTableResultFactory resultFactory)
		{
			if ((byte)(tableFlags & TableFlags.SoftDeletes) != 0)
			{
				DiagnosticContext.TraceLocation((LID)40072U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			bool flag = 2 == (byte)(tableFlags & TableFlags.Associated);
			bool flag2 = 16 == (byte)(tableFlags & TableFlags.NoNotifications);
			ViewMessageConfigureFlags viewMessageConfigureFlags = ViewMessageConfigureFlags.None;
			if (flag)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.ViewFAI;
			}
			if (flag2)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.NoNotifications;
			}
			if ((byte)(tableFlags & TableFlags.Depth) != 0)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.Conversation;
			}
			if ((byte)(tableFlags & TableFlags.SuppressNotifications) != 0)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.ConversationMembers;
			}
			if ((byte)(tableFlags & TableFlags.RetrieveFromIndex) != 0)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.RetrieveFromIndexOnly;
			}
			MapiViewMessage mapiViewMessage = null;
			RopResult result;
			try
			{
				MapiLogon logon = folder.Logon;
				mapiViewMessage = new MapiViewMessage();
				mapiViewMessage.Configure(context, logon, folder, viewMessageConfigureFlags, resultFactory.ServerObjectHandle.HandleValue);
				int rowCount = -1;
				if ((byte)(tableFlags & TableFlags.Depth) == 0 || !folder.IsSearchFolder())
				{
					Stopwatch stopwatch = (mapiViewMessage.CorrelationId != null) ? Stopwatch.StartNew() : null;
					try
					{
						rowCount = mapiViewMessage.GetRowCount(context);
					}
					finally
					{
						if (mapiViewMessage.CorrelationId != null)
						{
							stopwatch.Stop();
							FullTextIndexLogger.LogViewOperation(context.DatabaseGuid, context.Diagnostics.MailboxNumber, (int)context.ClientType, mapiViewMessage.CorrelationId.Value, FullTextIndexLogger.ViewOperationType.GetContentsTable, stopwatch.ToTimeSpan(), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Read), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Read), mapiViewMessage.IsOptimizedInstantSearch);
						}
					}
				}
				RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiViewMessage, rowCount);
				mapiViewMessage = null;
				result = ropResult;
			}
			finally
			{
				if (mapiViewMessage != null)
				{
					mapiViewMessage.Dispose();
				}
			}
			return result;
		}

		protected override RopResult GetContentsTableExtended(MapiContext context, MapiFolder folder, ExtendedTableFlags extendedTableFlags, GetContentsTableExtendedResultFactory resultFactory)
		{
			if ((extendedTableFlags & ~(ExtendedTableFlags.RetrieveFromIndex | ExtendedTableFlags.Associated | ExtendedTableFlags.Depth | ExtendedTableFlags.DeferredErrors | ExtendedTableFlags.NoNotifications | ExtendedTableFlags.MapiUnicode | ExtendedTableFlags.SuppressNotifications | ExtendedTableFlags.DocumentIdView | ExtendedTableFlags.ExpandedConversations | ExtendedTableFlags.PrereadExtendedProperties)) != ExtendedTableFlags.None)
			{
				DiagnosticContext.TraceDword((LID)55504U, (uint)extendedTableFlags);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			bool flag = ExtendedTableFlags.Associated == (extendedTableFlags & ExtendedTableFlags.Associated);
			bool flag2 = ExtendedTableFlags.NoNotifications == (extendedTableFlags & ExtendedTableFlags.NoNotifications);
			ViewMessageConfigureFlags viewMessageConfigureFlags = ViewMessageConfigureFlags.None;
			if (flag)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.ViewFAI;
			}
			if (flag2)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.NoNotifications;
			}
			if ((extendedTableFlags & ExtendedTableFlags.Depth) != ExtendedTableFlags.None)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.Conversation;
			}
			if ((extendedTableFlags & ExtendedTableFlags.SuppressNotifications) != ExtendedTableFlags.None)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.ConversationMembers;
			}
			if ((extendedTableFlags & ExtendedTableFlags.ExpandedConversations) != ExtendedTableFlags.None)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.ExpandedConversations;
			}
			if ((extendedTableFlags & ExtendedTableFlags.DocumentIdView) != ExtendedTableFlags.None)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.DocumentIdView;
				if ((extendedTableFlags & ExtendedTableFlags.PrereadExtendedProperties) != ExtendedTableFlags.None)
				{
					viewMessageConfigureFlags |= ViewMessageConfigureFlags.PrereadExtendedProperties;
				}
			}
			if ((extendedTableFlags & ExtendedTableFlags.RetrieveFromIndex) != ExtendedTableFlags.None)
			{
				viewMessageConfigureFlags |= ViewMessageConfigureFlags.RetrieveFromIndexOnly;
			}
			MapiViewMessage mapiViewMessage = null;
			RopResult result;
			try
			{
				MapiLogon logon = folder.Logon;
				mapiViewMessage = new MapiViewMessage();
				mapiViewMessage.Configure(context, logon, folder, viewMessageConfigureFlags, resultFactory.ServerObjectHandle.HandleValue);
				int rowCount = -1;
				if ((extendedTableFlags & ExtendedTableFlags.Depth) == ExtendedTableFlags.None || !folder.IsSearchFolder())
				{
					Stopwatch stopwatch = (mapiViewMessage.CorrelationId != null) ? Stopwatch.StartNew() : null;
					try
					{
						rowCount = mapiViewMessage.GetRowCount(context);
					}
					finally
					{
						if (mapiViewMessage.CorrelationId != null)
						{
							stopwatch.Stop();
							FullTextIndexLogger.LogViewOperation(context.DatabaseGuid, context.Diagnostics.MailboxNumber, (int)context.ClientType, mapiViewMessage.CorrelationId.Value, FullTextIndexLogger.ViewOperationType.GetContentsTableEx, stopwatch.ToTimeSpan(), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Read), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Read), mapiViewMessage.IsOptimizedInstantSearch);
						}
					}
				}
				RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiViewMessage, rowCount);
				mapiViewMessage = null;
				result = ropResult;
			}
			finally
			{
				if (mapiViewMessage != null)
				{
					mapiViewMessage.Dispose();
				}
			}
			return result;
		}

		protected override RopResult GetHierarchyTable(MapiContext context, MapiFolder folder, TableFlags tableFlags, GetHierarchyTableResultFactory resultFactory)
		{
			if ((byte)(tableFlags & (TableFlags.RetrieveFromIndex | TableFlags.SoftDeletes)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)56456U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			MapiViewFolder mapiViewFolder = null;
			RopResult result;
			try
			{
				MapiLogon logon = folder.Logon;
				mapiViewFolder = new MapiViewFolder();
				mapiViewFolder.Configure(context, logon, folder, (FolderViewTable.ConfigureFlags)tableFlags, resultFactory.ServerObjectHandle.HandleValue);
				int rowCount;
				if ((tableFlags & TableFlags.Depth) == TableFlags.Depth && ConfigurationSchema.AllowRecursiveFolderHierarchyRowCountApproximation.Value)
				{
					FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, logon.StoreMailbox, folder.Fid.ToExchangeShortId(), FolderInformationType.Basic);
					if (folderHierarchy.HierarchyRoots != null && folderHierarchy.HierarchyRoots.Count != 0)
					{
						rowCount = folderHierarchy.HierarchyRoots[0].AllDescendantFolderIds().Count<ExchangeShortId>() - 1;
					}
					else
					{
						rowCount = 0;
					}
				}
				else
				{
					rowCount = mapiViewFolder.GetRowCount(context);
				}
				RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiViewFolder, rowCount);
				mapiViewFolder = null;
				result = ropResult;
			}
			finally
			{
				if (mapiViewFolder != null)
				{
					mapiViewFolder.Dispose();
				}
			}
			return result;
		}

		protected override RopResult GetIdsFromNames(MapiContext context, MapiBase serverObject, GetIdsFromNamesFlags flags, NamedProperty[] namedProperties, GetIdsFromNamesResultFactory resultFactory)
		{
			if (namedProperties == null)
			{
				DiagnosticContext.TraceLocation((LID)44168U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (namedProperties.Length == 0)
			{
				DiagnosticContext.TraceLocation((LID)60552U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (flags != GetIdsFromNamesFlags.Create && flags != GetIdsFromNamesFlags.None)
			{
				DiagnosticContext.TraceLocation((LID)35976U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			for (int i = 0; i < namedProperties.Length; i++)
			{
				if (namedProperties[i].Kind == NamedPropertyKind.String && !StorePropName.IsValidName(namedProperties[i].Guid, namedProperties[i].Name))
				{
					throw new BufferParseException("Invalid Characters " + namedProperties[i].Name);
				}
			}
			bool create = 0 != (byte)(flags & GetIdsFromNamesFlags.Create);
			MapiLogon logon = serverObject.Logon;
			PropertyId[] numbersFromNames = LegacyHelper.GetNumbersFromNames(context, create, namedProperties, logon);
			return resultFactory.CreateSuccessfulResult(numbersFromNames);
		}

		protected override RopResult GetLocalReplicationIds(MapiContext context, MapiLogon logon, uint countOfIds, GetLocalReplicationIdsResultFactory resultFactory)
		{
			if (countOfIds > 1048575U)
			{
				DiagnosticContext.TraceLocation((LID)52360U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			ExchangeId localRepids = logon.StoreMailbox.GetLocalRepids(context, countOfIds);
			return resultFactory.CreateSuccessfulResult(new StoreLongTermId(localRepids.Guid, localRepids.Globcnt));
		}

		protected override RopResult GetMessageStatus(MapiContext context, MapiFolder folder, ExchangeId messageId, GetMessageStatusResultFactory resultFactory)
		{
			int messageStatus = folder.GetMessageStatus(context, messageId);
			return resultFactory.CreateSuccessfulResult((MessageStatusFlags)messageStatus);
		}

		protected override RopResult GetNamesFromIDs(MapiContext context, MapiBase serverObject, PropertyId[] propertyIds, GetNamesFromIDsResultFactory resultFactory)
		{
			MapiLogon logon = serverObject.Logon;
			NamedProperty[] namesFromPropertyIDs = RcaTypeHelpers.GetNamesFromPropertyIDs(context, propertyIds, logon.MapiMailbox);
			return resultFactory.CreateSuccessfulResult(namesFromPropertyIDs);
		}

		protected override RopResult GetPerUserGuid(MapiContext context, MapiLogon logon, StoreLongTermId publicFolderLongTermId, GetPerUserGuidResultFactory resultFactory)
		{
			byte[] foreignFolderId = publicFolderLongTermId.ToBytes(false);
			PerUser perUser = PerUser.LoadForeign(context, logon.StoreMailbox, foreignFolderId);
			if (perUser != null)
			{
				return resultFactory.CreateSuccessfulResult(perUser.Guid);
			}
			DiagnosticContext.TraceLocation((LID)30232U);
			return resultFactory.CreateFailedResult((ErrorCode)2147746063U);
		}

		protected override RopResult GetPerUserLongTermIds(MapiContext context, MapiLogon logon, Guid databaseGuid, GetPerUserLongTermIdsResultFactory resultFactory)
		{
			return resultFactory.CreateSuccessfulResult(logon.GetPerUserLongTermIds(context, databaseGuid).ToArray());
		}

		protected override RopResult GetPropertiesAll(MapiContext context, MapiPropBagBase propertyBag, ushort streamLimit, GetPropertiesFlags flags, GetPropertiesAllResultFactory resultFactory)
		{
			if ((flags & ~GetPropertiesFlags.Unicode) != GetPropertiesFlags.None)
			{
				DiagnosticContext.TraceLocation((LID)46216U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			PropertyValue[] array = Array<PropertyValue>.Empty;
			List<Property> allProperties = propertyBag.GetAllProperties(context, GetPropListFlags.None, true);
			if (allProperties != null && allProperties.Count > 0)
			{
				array = new PropertyValue[allProperties.Count];
				for (int i = 0; i < allProperties.Count; i++)
				{
					array[i] = RcaTypeHelpers.MassageOutgoingProperty(allProperties[i], false);
				}
			}
			return resultFactory.CreateSuccessfulResult(array);
		}

		protected override RopResult GetPropertiesSpecific(MapiContext context, MapiPropBagBase propertyBag, ushort streamLimit, GetPropertiesFlags flags, PropertyTag[] propertyTags, GetPropertiesSpecificResultFactory resultFactory)
		{
			if (propertyTags == null)
			{
				throw new ArgumentNullException("propertyTags");
			}
			if ((flags & ~GetPropertiesFlags.Unicode) != GetPropertiesFlags.None || propertyTags == null || propertyTags.Length == 0)
			{
				DiagnosticContext.TraceLocation((LID)62600U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			MapiLogon logon = propertyBag.Logon;
			StorePropTag[] array = LegacyHelper.ConvertFromLegacyPropTags(propertyTags, Helper.GetPropTagObjectType(propertyBag.MapiObjectType), logon.MapiMailbox, true);
			foreach (StorePropTag storePropTag in array)
			{
				if (storePropTag.PropType == PropertyType.Invalid)
				{
					DiagnosticContext.TraceLocation((LID)38024U);
					return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
				}
			}
			MapiPropBagBase.PropertyReader propsReader = propertyBag.GetPropsReader(context, array);
			PropertyValue[] array3 = Array<PropertyValue>.Empty;
			if (propsReader.PropertyCount > 0)
			{
				array3 = new PropertyValue[propsReader.PropertyCount];
				int num = 0;
				Property prop;
				while (propsReader.ReadNext(context, out prop))
				{
					array3[num] = RcaTypeHelpers.MassageOutgoingProperty(prop, false);
					num++;
				}
			}
			return resultFactory.CreateSuccessfulResult(propertyTags, array3);
		}

		protected override RopResult GetPropertyList(MapiContext context, MapiPropBagBase propertyBag, GetPropertyListResultFactory resultFactory)
		{
			List<Property> allProperties = propertyBag.GetAllProperties(context, GetPropListFlags.None, false);
			PropertyTag[] array = Array<PropertyTag>.Empty;
			if (allProperties != null && allProperties.Count > 0)
			{
				array = new PropertyTag[allProperties.Count];
				for (int i = 0; i < allProperties.Count; i++)
				{
					array[i] = new PropertyTag(allProperties[i].Tag.ExternalTag);
				}
			}
			return resultFactory.CreateSuccessfulResult(array);
		}

		protected override RopResult GetReceiveFolder(MapiContext context, MapiLogon logon, string messageClass, GetReceiveFolderResultFactory resultFactory)
		{
			if (messageClass == null)
			{
				throw new ArgumentNullException("messageClass");
			}
			if (!MessageClassHelper.IsValidMessageClass(messageClass) || messageClass.Length > 255)
			{
				DiagnosticContext.TraceLocation((LID)54408U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			ReceiveFolder receiveFolder = logon.GetReceiveFolder(context, messageClass);
			if (receiveFolder == null)
			{
				DiagnosticContext.TraceLocation((LID)42120U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746063U);
			}
			return resultFactory.CreateSuccessfulResult(RcaTypeHelpers.ExchangeIdToStoreId(receiveFolder.FolderId), receiveFolder.MessageClass);
		}

		protected override RopResult GetReceiveFolderTable(MapiContext context, MapiBase serverObject, GetReceiveFolderTableResultFactory resultFactory)
		{
			IList<ReceiveFolder> receiveFolders = serverObject.Logon.GetReceiveFolders(context);
			PropertyValue[][] array = new PropertyValue[receiveFolders.Count][];
			for (int i = 0; i < receiveFolders.Count; i++)
			{
				array[i] = new PropertyValue[]
				{
					new PropertyValue(PropertyTag.Fid, receiveFolders[i].FolderId.ToLong()),
					new PropertyValue(PropertyTag.MessageClass, receiveFolders[i].MessageClass),
					new PropertyValue(PropertyTag.LastModificationTime, (ExDateTime)receiveFolders[i].LastModificationTime)
				};
			}
			return resultFactory.CreateSuccessfulResult(array);
		}

		protected override RopResult GetSearchCriteria(MapiContext context, MapiFolder folder, GetSearchCriteriaFlags flags, GetSearchCriteriaResultFactory resultFactory)
		{
			byte[] buffer = null;
			IList<ExchangeId> exchangeIds = null;
			SearchState searchState = SearchState.None;
			ErrorCode searchCriteria = folder.GetSearchCriteria(context, flags, out buffer, out exchangeIds, out searchState);
			if (searchCriteria != ErrorCode.NoError)
			{
				DiagnosticContext.TraceStoreError((LID)35544U, (uint)searchCriteria);
				return resultFactory.CreateFailedResult((ErrorCode)searchCriteria);
			}
			StoreId[] folderIds = ((byte)(flags & GetSearchCriteriaFlags.FolderIds) != 0) ? RcaTypeHelpers.ExchangeIdsToStoreIds(exchangeIds) : null;
			if ((byte)(flags & GetSearchCriteriaFlags.Restriction) != 0)
			{
				using (Reader reader = Reader.CreateBufferReader(buffer))
				{
					return resultFactory.CreateSuccessfulResult(Restriction.Parse(reader, WireFormatStyle.Rop), folderIds, searchState);
				}
			}
			return resultFactory.CreateSuccessfulResult(null, folderIds, searchState);
		}

		protected override RopResult GetStreamSize(MapiContext context, MapiStream stream, GetStreamSizeResultFactory resultFactory)
		{
			long size = stream.GetSize(context);
			return resultFactory.CreateSuccessfulResult((uint)size);
		}

		protected override RopResult HardDeleteMessages(MapiContext context, MapiFolder folder, bool reportProgress, bool sendNonReadNotification, ExchangeId[] messageIds, out bool partiallyCompleted, HardDeleteMessagesResultFactory resultFactory)
		{
			partiallyCompleted = false;
			bool flag = false;
			if (messageIds == null || messageIds.Length == 0)
			{
				DiagnosticContext.TraceLocation((LID)58504U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U, false);
			}
			using (DeleteMessagesOperation deleteMessagesOperation = new DeleteMessagesOperation(folder, messageIds, sendNonReadNotification))
			{
				bool flag2;
				bool flag3;
				ErrorCode errorCode;
				while (!deleteMessagesOperation.DoChunk(context, out flag2, out flag3, out errorCode))
				{
					partiallyCompleted = (partiallyCompleted || flag2 || flag3);
					flag = (flag || flag3);
					errorCode = context.PulseMailboxOperation();
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)33928U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
					}
				}
				partiallyCompleted = (partiallyCompleted || flag2 || flag3);
				flag = (flag || flag3);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)50312U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
				}
			}
			return resultFactory.CreateSuccessfulResult(flag);
		}

		protected override RopResult HardEmptyFolder(MapiContext context, MapiFolder folder, bool reportProgress, EmptyFolderFlags emptyFolderFlags, out bool partiallyCompleted, HardEmptyFolderResultFactory resultFactory)
		{
			partiallyCompleted = false;
			bool flag = false;
			if ((byte)(emptyFolderFlags & ~(EmptyFolderFlags.Associated | EmptyFolderFlags.Force)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)47240U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U, false);
			}
			using (EmptyFolderOperation emptyFolderOperation = new EmptyFolderOperation(folder, (byte)(emptyFolderFlags & EmptyFolderFlags.Associated) != 0, MapiFolder.ManageAssociatedDumpsterFolder(folder.Logon, false), (byte)(emptyFolderFlags & EmptyFolderFlags.Force) != 0))
			{
				bool flag2;
				bool flag3;
				ErrorCode errorCode;
				while (!emptyFolderOperation.DoChunk(context, out flag2, out flag3, out errorCode))
				{
					partiallyCompleted = (partiallyCompleted || flag2);
					flag = (flag || flag3);
					errorCode = context.PulseMailboxOperation();
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)63624U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
					}
				}
				partiallyCompleted = (partiallyCompleted || flag2);
				flag = (flag || flag3);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)39048U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
				}
			}
			return resultFactory.CreateSuccessfulResult(flag);
		}

		protected override RopResult IdFromLongTermId(MapiContext context, MapiBase serverObject, StoreLongTermId longTermId, IdFromLongTermIdResultFactory resultFactory)
		{
			if (longTermId.Guid == Guid.Empty || longTermId.GlobCount == null)
			{
				DiagnosticContext.TraceLocation((LID)55432U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			MapiLogon logon = serverObject.Logon;
			ushort replidFromGuid = logon.GetReplidFromGuid(context, longTermId.Guid);
			ulong counter = ExchangeIdHelpers.GlobcntFromByteArray(longTermId.GlobCount, 0U);
			return resultFactory.CreateSuccessfulResult(new StoreId(ExchangeIdHelpers.ToLong(replidFromGuid, counter)));
		}

		protected override RopResult ImportDelete(MapiContext context, IcsUploadContext uploadContext, ImportDeleteFlags importDeleteFlags, PropertyValue[] deleteChanges, ImportDeleteResultFactory resultFactory)
		{
			if (deleteChanges == null)
			{
				throw new ArgumentNullException("deleteChanges");
			}
			if ((byte)(importDeleteFlags & ImportDeleteFlags.Hierarchy) != 0 && !(uploadContext is IcsHierarchyUploadContext))
			{
				DiagnosticContext.TraceLocation((LID)43144U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if ((byte)(importDeleteFlags & ImportDeleteFlags.Hierarchy) == 0 && !(uploadContext is IcsContentUploadContext))
			{
				DiagnosticContext.TraceLocation((LID)59528U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (deleteChanges.Length != 1 || deleteChanges[0].PropertyTag.PropertyId != PropertyId.Null || deleteChanges[0].PropertyTag.PropertyType != PropertyType.MultiValueBinary)
			{
				DiagnosticContext.TraceLocation((LID)34952U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			byte[][] array = deleteChanges[0].Value as byte[][];
			if (array != null && array.Length > 0)
			{
				ErrorCode errorCode = uploadContext.ImportDelete(context, array);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)51336U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult ImportHierarchyChange(MapiContext context, IcsHierarchyUploadContext uploadContext, PropertyValue[] hierarchyPropertyValues, PropertyValue[] folderPropertyValues, ImportHierarchyChangeResultFactory resultFactory)
		{
			if (hierarchyPropertyValues == null)
			{
				throw new ArgumentNullException("hierarchyPropertyValues");
			}
			if (folderPropertyValues == null)
			{
				throw new ArgumentNullException("folderPropertyValues");
			}
			Properties properties = new Properties(hierarchyPropertyValues.Length);
			RopHandler.AddProperties(hierarchyPropertyValues, MapiObjectType.Folder, uploadContext.Logon.MapiMailbox, properties, null);
			Properties properties2 = new Properties(folderPropertyValues.Length);
			RopHandler.AddProperties(folderPropertyValues, MapiObjectType.Folder, uploadContext.Logon.MapiMailbox, properties2, null);
			ExchangeId exchangeId;
			ErrorCode errorCode = uploadContext.ImportFolderChange(context, properties, properties2, out exchangeId);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)46600U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult(RcaTypeHelpers.ExchangeIdToStoreId(exchangeId));
		}

		protected override RopResult ImportMessageChange(MapiContext context, IcsContentUploadContext uploadContext, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangeResultFactory resultFactory)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			if ((byte)(importMessageChangeFlags & ~(ImportMessageChangeFlags.Associated | ImportMessageChangeFlags.FailOnConflict)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)45192U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!(uploadContext.ParentObject is MapiFolder))
			{
				DiagnosticContext.TraceLocation((LID)42504U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746056U);
			}
			MapiMessage mapiMessage = null;
			RopResult result;
			try
			{
				Properties properties = new Properties(propertyValues.Length);
				RopHandler.AddProperties(propertyValues, MapiObjectType.Message, uploadContext.Logon.MapiMailbox, properties, null);
				ExchangeId exchangeId;
				ErrorCode errorCode = uploadContext.ImportMessageChange(context, properties, importMessageChangeFlags, false, out mapiMessage, out exchangeId);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)54792U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiMessage, RcaTypeHelpers.ExchangeIdToStoreId(exchangeId));
					mapiMessage = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiMessage != null)
				{
					mapiMessage.Dispose();
				}
			}
			return result;
		}

		protected override RopResult ImportMessageChangePartial(MapiContext context, IcsContentUploadContext uploadContext, ImportMessageChangeFlags importMessageChangeFlags, PropertyValue[] propertyValues, ImportMessageChangePartialResultFactory resultFactory)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			if ((byte)(importMessageChangeFlags & ~(ImportMessageChangeFlags.Associated | ImportMessageChangeFlags.FailOnConflict)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)50696U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!(uploadContext.ParentObject is MapiFolder))
			{
				DiagnosticContext.TraceLocation((LID)47624U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746056U);
			}
			MapiMessage mapiMessage = null;
			RopResult result;
			try
			{
				Properties properties = new Properties(propertyValues.Length);
				RopHandler.AddProperties(propertyValues, MapiObjectType.Message, uploadContext.Logon.MapiMailbox, properties, null);
				ExchangeId exchangeId;
				ErrorCode errorCode = uploadContext.ImportMessageChange(context, properties, importMessageChangeFlags, true, out mapiMessage, out exchangeId);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)34312U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiMessage, RcaTypeHelpers.ExchangeIdToStoreId(exchangeId));
					mapiMessage = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiMessage != null)
				{
					mapiMessage.Dispose();
				}
			}
			return result;
		}

		protected override RopResult ImportMessageMove(MapiContext context, IcsContentUploadContext uploadContext, byte[] sourceFolder, byte[] sourceMessage, byte[] predecessorChangeList, byte[] destinationMessage, byte[] destinationChangeNumber, ImportMessageMoveResultFactory resultFactory)
		{
			if (sourceFolder == null)
			{
				throw new ArgumentNullException("sourceFolder");
			}
			if (sourceMessage == null)
			{
				throw new ArgumentNullException("sourceMessage");
			}
			if (predecessorChangeList == null)
			{
				throw new ArgumentNullException("predecessorChangeList");
			}
			if (destinationMessage == null)
			{
				throw new ArgumentNullException("destinationMessage");
			}
			if (destinationChangeNumber == null)
			{
				throw new ArgumentNullException("destinationChangeNumber");
			}
			ErrorCode errorCode = uploadContext.ImportMessageMove(context, sourceFolder, sourceMessage, predecessorChangeList, destinationMessage, destinationChangeNumber);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)64008U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult(StoreId.Empty);
		}

		protected override RopResult ImportReads(MapiContext context, IcsContentUploadContext uploadContext, MessageReadState[] messageReadStates, ImportReadsResultFactory resultFactory)
		{
			if (messageReadStates == null)
			{
				throw new ArgumentNullException("messageReadStates");
			}
			if (messageReadStates != null && messageReadStates.Length != 0)
			{
				ErrorCode errorCode = uploadContext.ImportReads(context, messageReadStates);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)39432U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult IncrementalConfig(MapiContext context, MapiFolder folder, IncrementalConfigOption configOptions, FastTransferSendOption sendOptions, SyncFlag syncFlags, Restriction restriction, SyncExtraFlag extraFlags, PropertyTag[] propertyTags, ExchangeId[] messageIds, IncrementalConfigResultFactory resultFactory)
		{
			bool flag = configOptions == IncrementalConfigOption.Hierarchy;
			bool flag2 = configOptions == IncrementalConfigOption.Contents;
			if (!flag && !flag2)
			{
				DiagnosticContext.TraceLocation((LID)55816U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (flag && ((byte)(sendOptions & ~(FastTransferSendOption.Unicode | FastTransferSendOption.RecoverMode | FastTransferSendOption.ForceUnicode | FastTransferSendOption.SendPropErrors)) != 0 || (ushort)(syncFlags & ~(SyncFlag.Unicode | SyncFlag.NoDeletions | SyncFlag.NoConflicts | SyncFlag.OnlySpecifiedProps | SyncFlag.NoForeignKeys | SyncFlag.CatchUp)) != 0 || (extraFlags & (SyncExtraFlag.MessageSize | SyncExtraFlag.OrderByDeliveryTime | SyncExtraFlag.NoChanges)) != SyncExtraFlag.None))
			{
				DiagnosticContext.TraceLocation((LID)43528U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (flag2 && ((byte)(sendOptions & ~(FastTransferSendOption.Unicode | FastTransferSendOption.RecoverMode | FastTransferSendOption.ForceUnicode | FastTransferSendOption.PartialItem | FastTransferSendOption.SendPropErrors)) != 0 || (ushort)0 != 0))
			{
				DiagnosticContext.TraceLocation((LID)59912U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if ((flag || (ushort)(syncFlags & SyncFlag.Conversations) != 0) && (restriction != null || (messageIds != null && messageIds.Length > 0)))
			{
				DiagnosticContext.TraceLocation((LID)51720U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if ((ushort)(syncFlags & SyncFlag.Conversations) != 0 && ((extraFlags & SyncExtraFlag.ManifestMode) == SyncExtraFlag.None || (extraFlags & SyncExtraFlag.MessageSize) != SyncExtraFlag.None || (byte)(sendOptions & FastTransferSendOption.PartialItem) != 0 || (ushort)(syncFlags & (SyncFlag.ReadState | SyncFlag.Associated)) != 0))
			{
				DiagnosticContext.TraceLocation((LID)36437U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			MapiObjectType mapiObjectType = flag ? MapiObjectType.Folder : MapiObjectType.Message;
			MapiLogon logon = folder.Logon;
			IcsDownloadContext icsDownloadContext = null;
			RopResult result;
			try
			{
				Restriction restriction2 = RcaTypeHelpers.RestrictionToRestriction(context, restriction, logon, mapiObjectType);
				StorePropTag[] propertyTags2 = LegacyHelper.ConvertFromLegacyPropTags(propertyTags, Helper.GetPropTagObjectType(mapiObjectType), logon.MapiMailbox, true);
				ErrorCode errorCode;
				if (flag)
				{
					IcsHierarchyDownloadContext icsHierarchyDownloadContext = new IcsHierarchyDownloadContext();
					icsDownloadContext = icsHierarchyDownloadContext;
					errorCode = icsHierarchyDownloadContext.Configure(logon, new HierarchySynchronizationScope(folder), sendOptions, syncFlags, extraFlags, propertyTags2);
				}
				else
				{
					IcsContentDownloadContext icsContentDownloadContext = new IcsContentDownloadContext();
					icsDownloadContext = icsContentDownloadContext;
					IContentSynchronizationScope contentSynchronizationScope = null;
					try
					{
						if ((ushort)(syncFlags & SyncFlag.Conversations) != 0)
						{
							contentSynchronizationScope = new ConversationSynchronizationScope(folder, syncFlags, extraFlags, propertyTags2, icsContentDownloadContext);
						}
						else
						{
							contentSynchronizationScope = new ContentSynchronizationScope(context, folder, restriction2, syncFlags, extraFlags, propertyTags2, icsContentDownloadContext);
						}
						errorCode = icsContentDownloadContext.Configure(logon, contentSynchronizationScope, sendOptions, syncFlags, extraFlags, propertyTags2, messageIds);
						contentSynchronizationScope = null;
					}
					finally
					{
						if (contentSynchronizationScope != null)
						{
							contentSynchronizationScope.Dispose();
						}
					}
				}
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)45576U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(icsDownloadContext);
					icsDownloadContext = null;
					result = ropResult;
				}
			}
			finally
			{
				if (icsDownloadContext != null)
				{
					icsDownloadContext.Dispose();
				}
			}
			return result;
		}

		protected override RopResult LockRegionStream(MapiContext context, MapiStream stream, ulong offset, ulong regionLength, LockTypeFlag lockType, LockRegionStreamResultFactory resultFactory)
		{
			ErrorCode errorCode = stream.LockRegion(context, offset, regionLength, lockType != LockTypeFlag.None);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)61960U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult Logon(MapiContext context, LogonFlags logonFlags, OpenFlags openFlags, StoreState storeState, LogonExtendedRequestFlags extendedFlags, MailboxId? mailboxId, LocaleInfo? localeInfo, string applicationId, AuthenticationContext authenticationContext, byte[] tenantHintBlob, LogonResultFactory resultFactory)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			RopResult result = null;
			Guid mailboxGuid = Guid.Empty;
			Guid guid = Guid.Empty;
			MapiLogon mapiLogon = null;
			MailboxInfo mailboxInfo = null;
			AddressInfo addressInfo = null;
			int? num = null;
			try
			{
				this.Session.UpdateApplicationId(context, applicationId);
				this.ValidateLogonParameters(context, logonFlags, openFlags, extendedFlags, mailboxId, applicationId, authenticationContext);
				mailboxGuid = mailboxId.Value.MailboxGuid;
				guid = mailboxId.Value.DatabaseGuid;
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.FaultInjectionTracer.IsTraceEnabled(TraceType.FaultInjection))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.FaultInjectionTracer.TraceTest<Guid>(3133549885U, ref mailboxGuid);
				}
				bool flag = true;
				context.Initialize(guid, mailboxGuid, flag, true);
				errorCode = context.StartMailboxOperation(MailboxCreation.DontAllow, true, true);
				if (errorCode != ErrorCode.NoError && errorCode != ErrorCodeValue.NotFound)
				{
					throw new StoreException((LID)54056U, errorCode);
				}
				bool commit = false;
				MapiMailbox mapiMailbox = null;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = true;
				try
				{
					bool flag7 = false;
					bool flag8 = false;
					TenantHint tenantHint = TenantHint.Empty;
					DateTime dateTime = DateTime.UtcNow;
					if (context.IsMailboxOperationStarted)
					{
						num = new int?(context.LockedMailboxState.MailboxNumber);
						context.SkipDatabaseLogsFlush = RopHandlerBase.SkipDatabaseLogFlush(RopId.Logon, null);
						if (context.Database.IsReadOnly)
						{
							RopHandlerBase.CheckClientTypeIsAllowedOnReadOnlyDatabase(context, RopId.Logon, RopHandler.ClientTypesAllowedToLogonToReadOnlyDatabase);
						}
						else
						{
							flag4 = (dateTime - context.LockedMailboxState.LastMailboxMaintenanceTime >= Mailbox.MaintenanceRunInterval);
							flag5 = (dateTime - context.LockedMailboxState.LastQuotaCheckTime >= MapiMailboxShape.QuotaWarningCheckInterval);
						}
						if (flag)
						{
							flag = (!flag4 && !flag5 && !context.LockedMailboxState.IsDisabled);
						}
						tenantHint = context.LockedMailboxState.TenantHint;
						context.EndMailboxOperation(false, false);
					}
					context.Initialize(guid, mailboxGuid, flag, true);
					TenantHint tenantHint2;
					if (tenantHint.IsEmpty && tenantHintBlob != null && tenantHintBlob.Length != 0)
					{
						tenantHint2 = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.ResolveTenantHint(context, tenantHintBlob);
					}
					else
					{
						tenantHint2 = tenantHint;
					}
					if (RopHandler.FinalizeSessionConfigurationIfNeeded(context, tenantHint2, this.Session, authenticationContext, OpenFlags.None != (openFlags & OpenFlags.UseAdminPrivilege), guid))
					{
						context.Initialize(guid, mailboxGuid, flag, true);
					}
					DatabaseInfo databaseInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(context, guid);
					if (OpenFlags.None != (openFlags & OpenFlags.RestoreDatabase) != databaseInfo.IsRecoveryDatabase)
					{
						throw new StoreException((LID)45768U, ErrorCodeValue.RecoveryMDBMismatch);
					}
					bool flag9 = context.ClientType == ClientType.Migration && ((OpenFlags)1073741824U & openFlags) == (OpenFlags)1073741824U;
					bool flag10 = this.Session.UsingAdminPrivilege && OpenFlags.None != (openFlags & OpenFlags.UseAdminPrivilege);
					bool flag11 = databaseInfo.IsRecoveryDatabase && OpenFlags.None != (openFlags & OpenFlags.RestoreDatabase);
					bool flag12 = flag10 && (flag9 || flag11);
					if (!flag12)
					{
						Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByMailboxGuid(context, tenantHint2, mailboxGuid, GetAddressInfoFlags.None);
					}
					try
					{
						GetMailboxInfoFlags flags = GetMailboxInfoFlags.None;
						if (RopHandlerBase.SkipHomeMdbValidation(context, (OpenStoreFlags)openFlags))
						{
							flags = GetMailboxInfoFlags.IgnoreHomeMdb;
						}
						mailboxInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetMailboxInfo(context, tenantHint2, mailboxGuid, flags);
						if (mailboxInfo.IsSystemAttendantRecipient)
						{
							throw new ExExceptionNoSupport((LID)34856U, "Logon to the SystemAttendant mailbox is not supported");
						}
						addressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfoByMailboxGuid(context, tenantHint2, mailboxGuid, GetAddressInfoFlags.None);
						flag8 = true;
						flag7 = (mailboxInfo.MdbGuid == guid);
						((MapiExecutionDiagnostics)context.Diagnostics).MapiExMonLogger.AccessedMailboxLegacyDn = addressInfo.LegacyExchangeDN;
					}
					catch (MailboxNotFoundException exception)
					{
						context.OnExceptionCatch(exception);
						flag8 = false;
						flag7 = false;
						((MapiExecutionDiagnostics)context.Diagnostics).MapiExMonLogger.AccessedMailboxLegacyDn = this.Session.AddressInfoUser.LegacyExchangeDN;
					}
					if (!flag7 && (!flag8 || !mailboxInfo.IsArchiveMailbox))
					{
						if ((openFlags & OpenFlags.OverrideHomeMdb) == OpenFlags.None)
						{
							throw new StoreException((LID)62152U, ErrorCodeValue.WrongServer);
						}
						if ((openFlags & OpenFlags.UseAdminPrivilege) == OpenFlags.None)
						{
							throw new StoreException((LID)37576U, ErrorCodeValue.Unconfigured);
						}
						if (!SecurityHelper.CheckAdministrativeRights(this.Session.CurrentSecurityContext, databaseInfo.NTSecurityDescriptor))
						{
							throw new ExExceptionLogonFailed((LID)53960U, "No admin rights.");
						}
					}
					context.Initialize(guid, mailboxGuid, flag, true);
					MailboxCreation mailboxCreation = (flag || !flag8) ? MailboxCreation.DontAllow : MailboxCreation.Allow(mailboxInfo.PartitionGuid);
					errorCode = context.StartMailboxOperation(mailboxCreation, true, false);
					if (flag && errorCode == ErrorCodeValue.NotFound)
					{
						flag = false;
						mailboxCreation = (flag8 ? MailboxCreation.Allow(mailboxInfo.PartitionGuid) : MailboxCreation.DontAllow);
						context.Initialize(guid, mailboxGuid, flag, true);
						errorCode = context.StartMailboxOperation(mailboxCreation, true, false);
					}
					if (errorCode != ErrorCode.NoError)
					{
						throw new StoreException((LID)38120U, errorCode);
					}
					Storage.FindDatabase(guid);
					num = new int?(context.LockedMailboxState.MailboxNumber);
					if (context.LockedMailboxState.IsNew && !tenantHint2.IsEmpty)
					{
						context.LockedMailboxState.TenantHint = tenantHint2;
					}
					else
					{
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(context.LockedMailboxState.IsNew || !context.LockedMailboxState.TenantHint.IsEmpty, "If this is not 'new' mailbox, then TenantHint should not be empty.");
					}
					if (!context.LockedMailboxState.IsAccessible && !context.LockedMailboxState.IsNew)
					{
						if (context.LockedMailboxState.IsRemoved)
						{
							using (Mailbox mailbox = Mailbox.OpenMailbox(context, context.LockedMailboxState))
							{
								if (PropertyBagHelpers.TestPropertyFlags(context, mailbox, PropTag.Mailbox.MailboxFlags, 256, 256))
								{
									throw new StoreException((LID)37672U, ErrorCodeValue.MailboxInTransit);
								}
							}
						}
						throw new ExExceptionLogonFailed((LID)36920U, string.Format("Invalid mailbox state: {0}", context.LockedMailboxState.Status));
					}
					if (context.LockedMailboxState.IsNew)
					{
						flag3 = true;
						if (!flag7 || databaseInfo.IsRecoveryDatabase)
						{
							throw new ExExceptionLogonFailed((LID)53304U, "Can create the mailbox only in the active home database.");
						}
						if (!flag8)
						{
							throw new MailboxNotFoundException((LID)41016U, mailboxGuid);
						}
						using (context.GrantMailboxFullRights())
						{
							MapiMailbox.CreateMailbox(context, context.LockedMailboxState, addressInfo, mailboxInfo, databaseInfo, context.LockedMailboxState.TenantHint, context.LockedMailboxState.MailboxInstanceGuid, Guid.NewGuid());
							goto IL_74A;
						}
					}
					if (context.LockedMailboxState.IsUserAccessible)
					{
						if (!flag8)
						{
							if (!databaseInfo.IsRecoveryDatabase)
							{
								throw new MailboxNotFoundException((LID)57184U, mailboxGuid);
							}
							flag6 = false;
							addressInfo = this.Session.AddressInfoUser;
							mailboxInfo = RopHandler.GenerateDisconnectedMailboxInfo(addressInfo, context.LockedMailboxState);
						}
						if (!RopHandlerBase.ValidateMailboxType(context.LockedMailboxState, mailboxInfo))
						{
							throw new StoreException((LID)52172U, ErrorCodeValue.UnexpectedMailboxState);
						}
					}
					else
					{
						if (!context.LockedMailboxState.IsDisconnected)
						{
							throw new StoreException((LID)48696U, ErrorCodeValue.UnknownMailbox);
						}
						if (flag12)
						{
							flag6 = false;
							addressInfo = this.Session.AddressInfoUser;
							mailboxInfo = RopHandler.GenerateDisconnectedMailboxInfo(addressInfo, context.LockedMailboxState);
						}
						else
						{
							if (context.LockedMailboxState.IsSoftDeleted)
							{
								throw new StoreException((LID)57400U, ErrorCodeValue.MailboxInTransit);
							}
							if (!context.LockedMailboxState.IsDisabled)
							{
								throw new StoreException((LID)49208U, ErrorCodeValue.InvalidParameter);
							}
							if (!flag8)
							{
								throw new StoreException((LID)32824U, ErrorCodeValue.DisabledMailbox);
							}
						}
					}
					IL_74A:
					mapiMailbox = this.Session.GetMapiMailbox(context.LockedMailboxState.MailboxNumber);
					if (mapiMailbox == null)
					{
						mapiMailbox = MapiMailbox.OpenMailbox(context, context.LockedMailboxState, mailboxInfo.Type, mailboxInfo.Lcid);
						flag2 = true;
						if (context.LockedMailboxState.IsDisabled && flag8 && !flag)
						{
							if (this.Session.UsingDelegatedAuth)
							{
								RopHandler.ReconnectMailboxToDirectoryObject(context, mapiMailbox, mailboxInfo);
							}
							flag6 = false;
						}
					}
					else
					{
						mapiMailbox.Connect(context);
					}
					if (flag)
					{
						flag4 = false;
						flag5 = false;
					}
					if (mapiMailbox.StoreMailbox.GetCreatedByMove(context))
					{
						flag4 = false;
						flag5 = false;
					}
					if (flag4 && !flag3)
					{
						mapiMailbox.StoreMailbox.Save(context);
						mapiMailbox.StoreMailbox.CheckMailboxVersionAndUpgrade(context);
					}
					mapiLogon = new MapiLogon();
					bool unifiedLogon = (extendedFlags & LogonExtendedRequestFlags.UnifiedLogon) == LogonExtendedRequestFlags.UnifiedLogon;
					mapiLogon.ConfigureLogon(context, this.Session, addressInfo, mailboxInfo, databaseInfo, ref mapiMailbox, (OpenStoreFlags)openFlags, unifiedLogon, (int)resultFactory.LogonId);
					context.SetMapiLogon(mapiLogon);
					((MapiExecutionDiagnostics)context.Diagnostics).MapiObject = mapiLogon;
					mapiLogon.PerformTransportLogonQuotaCheck(context);
					if (flag5)
					{
						mapiLogon.PerformLogonQuotaCheck(context);
					}
					if (flag4)
					{
						if (flag6)
						{
							mapiLogon.MapiMailbox.StoreMailbox.SetDirectoryPersonalInfoOnMailbox(context, mailboxInfo);
						}
						mapiLogon.MapiMailbox.StoreMailbox.UpdateTableSizeStatistics(context);
						mapiLogon.WriteMailboxInfoTrace(context);
						if (flag3)
						{
							dateTime = DateTime.MinValue;
						}
						mapiLogon.MapiMailbox.StoreMailbox.SetLastMailboxMaintenanceTime(context, dateTime);
						if (context.LockedMailboxState.IsUserAccessible)
						{
							this.CheckAndRunISIntegScheduled(context, mapiLogon.MapiMailbox.StoreMailbox, mailboxGuid, dateTime, context.LockedMailboxState.UtcNow);
						}
					}
					mapiLogon.UpdateLastLogonTime(context);
					LogonResponseFlags logonResponseFlags = LogonResponseFlags.HasSendAsRights;
					if (mapiLogon.MapiMailbox.GetLocalized(context))
					{
						logonResponseFlags |= LogonResponseFlags.IsMailboxLocalized;
					}
					if (mapiLogon.StoreMailbox.GetOofState(context))
					{
						logonResponseFlags |= LogonResponseFlags.IsOOF;
					}
					if (context.HasMailboxFullRights)
					{
						logonResponseFlags |= LogonResponseFlags.IsMailboxOwner;
					}
					ulong routingConfigurationTimestamp = 0UL;
					storeState = StoreState.None;
					result = resultFactory.CreateSuccessfulPrivateResult(mapiLogon, LogonFlags.Private | LogonFlags.NoRules, (mailboxInfo.Type == MailboxInfo.MailboxType.Private) ? new StoreId[]
					{
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidRoot),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidDAF),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidSpoolerQ),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidIPMsubtree),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidInbox),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidOutbox),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidSentmail),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidWastebasket),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidCommonViews),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidSchedule),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidFinder),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidViews),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidShortcuts)
					} : new StoreId[]
					{
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidRoot),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidPublicFolderIpmSubTree),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidPublicFolderNonIpmSubTree),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidPublicFolderEFormsRegistry),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidPublicFolderDumpsterRoot),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidPublicFolderTombstonesRoot),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidPublicFolderAsyncDeleteState),
						RcaTypeHelpers.ExchangeIdToStoreId(mapiLogon.FidC.FidPublicFolderInternalSubmission),
						StoreId.Empty,
						StoreId.Empty,
						StoreId.Empty,
						StoreId.Empty,
						StoreId.Empty
					}, LogonExtendedResponseFlags.None, null, logonResponseFlags, mapiLogon.MailboxInstanceGuid, new ReplId(mapiLogon.GetReplidFromGuid(context, mapiLogon.StoreMailbox.GetMappingSignatureGuid(context))), mapiLogon.StoreMailbox.GetMappingSignatureGuid(context), ExDateTime.UtcNow, routingConfigurationTimestamp, storeState);
					this.InvokeOnBeforeOperationCompleteCallbackIfNecessary(RopId.Logon);
					MapiBase objectToDisposeOnAbort = mapiLogon;
					context.RegisterStateAction(null, delegate(Context ctx)
					{
						objectToDisposeOnAbort.Dispose();
					});
					mapiLogon = null;
					commit = true;
				}
				finally
				{
					if (context.Diagnostics.MailboxNumber < 0 && num != null && num.Value > 0)
					{
						context.Diagnostics.MailboxNumber = num.Value;
					}
					if (mapiMailbox != null)
					{
						if (flag2)
						{
							mapiMailbox.Dispose();
							flag2 = false;
						}
						mapiMailbox = null;
					}
					if (mapiLogon != null)
					{
						context.SetMapiLogon(null);
						mapiLogon.Dispose();
						mapiLogon = null;
					}
					if (context.IsMailboxOperationStarted)
					{
						if (flag3)
						{
							MailboxState capturedMailboxState = context.LockedMailboxState;
							context.RegisterStateAction(null, delegate(Context ctx)
							{
								MailboxStateCache.DeleteMailboxState(ctx, capturedMailboxState);
							});
						}
						context.EndMailboxOperation(commit, false);
					}
				}
			}
			catch (DatabaseNotFoundException exception2)
			{
				context.OnExceptionCatch(exception2);
				errorCode = ErrorCode.CreateWrongServer((LID)37272U);
			}
			catch (ExExceptionObjectDeleted exception3)
			{
				context.OnExceptionCatch(exception3);
				errorCode = ErrorCode.CreateWrongServer((LID)41368U);
			}
			finally
			{
				context.SkipDatabaseLogsFlush = false;
			}
			if (errorCode == ErrorCodeValue.WrongServer)
			{
				return resultFactory.CreateRedirectResult(string.Empty, LogonFlags.Private | LogonFlags.NoRules);
			}
			if (errorCode != ErrorCode.NoError)
			{
				result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return result;
		}

		protected override RopResult LongTermIdFromId(MapiContext context, MapiBase serverObject, ExchangeId storeId, LongTermIdFromIdResultFactory resultFactory)
		{
			MapiLogon logon = serverObject.Logon;
			Guid guidFromReplid = logon.GetGuidFromReplid(context, storeId.Replid);
			StoreLongTermId longTermId = new StoreLongTermId(guidFromReplid, storeId.Globcnt);
			return resultFactory.CreateSuccessfulResult(longTermId);
		}

		protected override RopResult MoveCopyMessages(MapiContext context, MapiFolder sourceFolder, MapiFolder destinationFolder, ExchangeId[] messageIds, bool reportProgress, bool copyMessages, out bool partiallyCompleted, MoveCopyMessagesResultFactory resultFactory)
		{
			partiallyCompleted = false;
			bool flag = false;
			if (!object.ReferenceEquals(sourceFolder.Logon, destinationFolder.Logon))
			{
				DiagnosticContext.TraceLocation((LID)61576U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U, false);
			}
			if (messageIds != null && messageIds.Length > 0)
			{
				using (MoveCopyMessagesOperation moveCopyMessagesOperation = new MoveCopyMessagesOperation(copyMessages, sourceFolder, destinationFolder, messageIds, Properties.Empty, null, null))
				{
					bool flag2;
					bool flag3;
					ErrorCode errorCode;
					while (!moveCopyMessagesOperation.DoChunk(context, out flag2, out flag3, out errorCode))
					{
						partiallyCompleted = (partiallyCompleted || flag2);
						flag = (flag || flag3);
						errorCode = context.PulseMailboxOperation();
						if (errorCode != ErrorCode.NoError)
						{
							DiagnosticContext.TraceLocation((LID)37384U);
							return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
						}
					}
					partiallyCompleted = (partiallyCompleted || flag2);
					flag = (flag || flag3);
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)37000U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
					}
				}
			}
			return resultFactory.CreateSuccessfulResult(flag);
		}

		protected override RopResult MoveCopyMessagesExtended(MapiContext context, MapiFolder sourceFolder, MapiFolder destinationFolder, ExchangeId[] messageIds, bool reportProgress, bool copyMessages, PropertyValue[] propertyValues, out bool partiallyCompleted, MoveCopyMessagesExtendedResultFactory resultFactory)
		{
			partiallyCompleted = false;
			bool flag = false;
			if (!object.ReferenceEquals(sourceFolder.Logon, destinationFolder.Logon))
			{
				DiagnosticContext.TraceLocation((LID)64904U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U, false);
			}
			if (messageIds != null && messageIds.Length > 0)
			{
				MapiLogon logon = sourceFolder.Logon;
				Properties empty = Properties.Empty;
				if (propertyValues != null && propertyValues.Length != 0)
				{
					empty = new Properties(propertyValues.Length);
					RopHandler.AddProperties(propertyValues, MapiObjectType.Message, logon.MapiMailbox, empty, null);
				}
				using (MoveCopyMessagesOperation moveCopyMessagesOperation = new MoveCopyMessagesOperation(copyMessages, sourceFolder, destinationFolder, messageIds, empty, null, null))
				{
					bool flag2;
					bool flag3;
					ErrorCode errorCode;
					while (!moveCopyMessagesOperation.DoChunk(context, out flag2, out flag3, out errorCode))
					{
						partiallyCompleted = (partiallyCompleted || flag2);
						flag = (flag || flag3);
						errorCode = context.PulseMailboxOperation();
						if (errorCode != ErrorCode.NoError)
						{
							DiagnosticContext.TraceLocation((LID)56712U);
							return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
						}
					}
					partiallyCompleted = (partiallyCompleted || flag2);
					flag = (flag || flag3);
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)44424U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
					}
				}
			}
			return resultFactory.CreateSuccessfulResult(flag);
		}

		protected override RopResult MoveCopyMessagesExtendedWithEntryIds(MapiContext context, MapiFolder sourceFolder, MapiFolder destinationFolder, ExchangeId[] messageIds, bool reportProgress, bool copyMessages, PropertyValue[] propertyValues, out bool partiallyCompleted, MoveCopyMessagesExtendedWithEntryIdsResultFactory resultFactory)
		{
			partiallyCompleted = false;
			bool flag = false;
			if (!object.ReferenceEquals(sourceFolder.Logon, destinationFolder.Logon))
			{
				DiagnosticContext.TraceLocation((LID)57864U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U, false);
			}
			StoreId[] messageIds2 = null;
			ulong[] changeNumbers = null;
			if (messageIds != null && messageIds.Length > 0)
			{
				List<ExchangeId> list = new List<ExchangeId>(messageIds.Length);
				List<ExchangeId> list2 = new List<ExchangeId>(messageIds.Length);
				MapiLogon logon = sourceFolder.Logon;
				Properties empty = Properties.Empty;
				if (propertyValues != null && propertyValues.Length != 0)
				{
					empty = new Properties(propertyValues.Length);
					RopHandler.AddProperties(propertyValues, MapiObjectType.Message, logon.MapiMailbox, empty, null);
				}
				using (MoveCopyMessagesOperation moveCopyMessagesOperation = new MoveCopyMessagesOperation(copyMessages, sourceFolder, destinationFolder, messageIds, empty, list, list2))
				{
					bool flag2;
					bool flag3;
					ErrorCode errorCode;
					while (!moveCopyMessagesOperation.DoChunk(context, out flag2, out flag3, out errorCode))
					{
						partiallyCompleted = (partiallyCompleted || flag2);
						flag = (flag || flag3);
						errorCode = context.PulseMailboxOperation();
						if (errorCode != ErrorCode.NoError)
						{
							DiagnosticContext.TraceLocation((LID)49672U);
							return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
						}
					}
					partiallyCompleted = (partiallyCompleted || flag2);
					flag = (flag || flag3);
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)48520U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
					}
				}
				if (list.Count != 0)
				{
					messageIds2 = RcaTypeHelpers.ExchangeIdsToStoreIds(list);
					changeNumbers = RcaTypeHelpers.ExchangeIdsToArrayOfUInt64(list2);
				}
			}
			return resultFactory.CreateSuccessfulResult(flag, messageIds2, changeNumbers);
		}

		protected override RopResult MoveFolder(MapiContext context, MapiFolder sourceParentFolder, MapiFolder destinationParentFolder, bool reportProgress, ExchangeId folderId, string folderName, out bool partiallyCompleted, MoveFolderResultFactory resultFactory)
		{
			partiallyCompleted = false;
			if (folderName == null)
			{
				throw new ArgumentNullException("folderName");
			}
			if (!object.ReferenceEquals(sourceParentFolder.Logon, destinationParentFolder.Logon))
			{
				DiagnosticContext.TraceLocation((LID)53384U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U, false);
			}
			MapiLogon logon = sourceParentFolder.Logon;
			using (MapiFolder mapiFolder = MapiFolder.OpenFolder(context, logon, folderId))
			{
				if (mapiFolder == null)
				{
					DiagnosticContext.TraceLocation((LID)41096U);
					return resultFactory.CreateFailedResult((ErrorCode)2147746063U, false);
				}
				if (mapiFolder.GetParentFid(context) == destinationParentFolder.Fid && string.IsNullOrEmpty(folderName))
				{
					return resultFactory.CreateSuccessfulResult(false);
				}
				ErrorCode errorCode = mapiFolder.Move(context, destinationParentFolder, folderName, false);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)57480U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode, false);
				}
			}
			return resultFactory.CreateSuccessfulResult(false);
		}

		protected override RopResult OpenAttachment(MapiContext context, MapiMessage message, OpenMode openMode, uint attachmentNumber, OpenAttachmentResultFactory resultFactory)
		{
			if ((byte)(openMode & ~(OpenMode.ReadWrite | OpenMode.Create)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)32904U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			AttachmentConfigureFlags attachmentConfigureFlags = AttachmentConfigureFlags.None;
			if (openMode == OpenMode.ReadOnly)
			{
				attachmentConfigureFlags |= AttachmentConfigureFlags.RequestReadOnly;
			}
			else if (openMode == OpenMode.ReadWrite)
			{
				attachmentConfigureFlags |= AttachmentConfigureFlags.RequestWrite;
			}
			MapiAttachment mapiAttachment = null;
			RopResult result;
			try
			{
				ErrorCode errorCode = message.OpenAttachment(context, (int)attachmentNumber, attachmentConfigureFlags, out mapiAttachment);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)49288U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiAttachment);
					mapiAttachment = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiAttachment != null)
				{
					mapiAttachment.Dispose();
				}
			}
			return result;
		}

		protected override RopResult OpenCollector(MapiContext context, MapiFolder folder, bool wantMessageCollector, OpenCollectorResultFactory resultFactory)
		{
			IcsUploadContext icsUploadContext = null;
			RopResult result;
			try
			{
				if (wantMessageCollector)
				{
					icsUploadContext = new IcsContentUploadContext();
				}
				else
				{
					icsUploadContext = new IcsHierarchyUploadContext();
				}
				ErrorCode errorCode = icsUploadContext.Configure(context, folder);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)48776U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(icsUploadContext);
					icsUploadContext = null;
					result = ropResult;
				}
			}
			finally
			{
				if (icsUploadContext != null)
				{
					icsUploadContext.Dispose();
				}
			}
			return result;
		}

		protected override RopResult OpenEmbeddedMessage(MapiContext context, MapiAttachment attachment, ushort codePageId, OpenMode openMode, OpenEmbeddedMessageResultFactory resultFactory)
		{
			if ((byte)(openMode & ~(OpenMode.ReadWrite | OpenMode.Create)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)65160U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			CodePage codePage = (CodePage)codePageId;
			if (CodePage.None == codePage)
			{
				codePage = attachment.Logon.CodePage;
			}
			MessageConfigureFlags messageConfigureFlags = MessageConfigureFlags.None;
			if (openMode == OpenMode.ReadOnly)
			{
				messageConfigureFlags = MessageConfigureFlags.RequestReadOnly;
			}
			else if (openMode == OpenMode.Create)
			{
				messageConfigureFlags |= MessageConfigureFlags.CreateNewMessage;
			}
			else if (openMode == OpenMode.ReadWrite)
			{
				messageConfigureFlags |= MessageConfigureFlags.RequestWrite;
			}
			MapiMessage mapiMessage = null;
			RopResult result;
			try
			{
				MapiLogon logon = attachment.Logon;
				ErrorCode errorCode = attachment.OpenEmbeddedMessage(context, messageConfigureFlags, codePage, out mapiMessage);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)40584U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					CodePage codePageProperties = logon.Session.CanConvertCodePage ? CodePage.Unicode : codePage;
					using (RecipientCollector recipientCollector = RopHandler.CreateMessageHeader(context, mapiMessage, resultFactory, codePageProperties, null))
					{
						RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiMessage, RcaTypeHelpers.ExchangeIdToStoreId(mapiMessage.Mid), recipientCollector);
						mapiMessage = null;
						result = ropResult;
					}
				}
			}
			finally
			{
				if (mapiMessage != null)
				{
					mapiMessage.Dispose();
				}
			}
			return result;
		}

		protected override RopResult OpenFolder(MapiContext context, MapiBase serverObject, ExchangeId folderId, OpenMode openMode, OpenFolderResultFactory resultFactory)
		{
			if (serverObject.MapiObjectType != MapiObjectType.Logon && serverObject.MapiObjectType != MapiObjectType.Folder)
			{
				DiagnosticContext.TraceLocation((LID)56968U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			MapiLogon logon = serverObject.Logon;
			MapiFolder mapiFolder = null;
			RopResult result;
			try
			{
				mapiFolder = MapiFolder.OpenFolder(context, logon, folderId);
				if (mapiFolder == null)
				{
					DiagnosticContext.TraceLocation((LID)44680U);
					result = resultFactory.CreateFailedResult((ErrorCode)2147746063U);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiFolder, false, null);
					mapiFolder = null;
					result = ropResult;
				}
			}
			catch (ExExceptionAccessDenied exception)
			{
				context.OnExceptionCatch(exception);
				DiagnosticContext.TraceLocation((LID)61064U);
				result = resultFactory.CreateFailedResult((ErrorCode)2147746063U);
			}
			finally
			{
				if (mapiFolder != null)
				{
					mapiFolder.Dispose();
				}
			}
			return result;
		}

		protected override RopResult OpenMessage(MapiContext context, MapiBase serverObject, ushort codePageId, ExchangeId folderId, OpenMode openMode, ExchangeId messageId, OpenMessageResultFactory resultFactory)
		{
			MapiLogon mapiLogon = serverObject as MapiLogon;
			OpenMode openMode2 = openMode & ~(OpenMode.OpenSoftDeleted | OpenMode.NoBlock);
			if (mapiLogon == null)
			{
				MapiFolder mapiFolder = serverObject as MapiFolder;
				if (mapiFolder != null)
				{
					mapiLogon = mapiFolder.Logon;
				}
			}
			if (mapiLogon == null)
			{
				DiagnosticContext.TraceLocation((LID)36488U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746056U);
			}
			if (!EnumValidator.IsValidValue<CodePage>((CodePage)codePageId))
			{
				DiagnosticContext.TraceLocation((LID)52872U);
				return resultFactory.CreateFailedResult(ErrorCode.UnknownCodepage);
			}
			if (codePageId != 4095 && codePageId != 0)
			{
				DiagnosticContext.TraceLocation((LID)46728U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (openMode2 != OpenMode.ReadOnly && openMode2 != OpenMode.ReadWrite && openMode2 != OpenMode.BestAccess)
			{
				DiagnosticContext.TraceLocation((LID)63112U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			MessageConfigureFlags messageConfigureFlags = MessageConfigureFlags.None;
			if (openMode2 == OpenMode.ReadOnly)
			{
				messageConfigureFlags |= MessageConfigureFlags.RequestReadOnly;
			}
			else if (openMode2 == OpenMode.ReadWrite)
			{
				messageConfigureFlags |= MessageConfigureFlags.RequestWrite;
			}
			CodePage codePage = mapiLogon.Session.CodePage;
			MapiMessage mapiMessage = null;
			RopResult result;
			try
			{
				mapiMessage = new MapiMessage();
				ErrorCode errorCode = mapiMessage.ConfigureMessage(context, mapiLogon, folderId, messageId, messageConfigureFlags, codePage);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)38536U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					CodePage codePageProperties = mapiLogon.Session.CanConvertCodePage ? CodePage.Unicode : codePage;
					using (RecipientCollector recipientCollector = RopHandler.CreateMessageHeader(context, mapiMessage, resultFactory, codePageProperties, null))
					{
						RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiMessage, recipientCollector);
						mapiMessage = null;
						result = ropResult;
					}
				}
			}
			finally
			{
				if (mapiMessage != null)
				{
					mapiMessage.Dispose();
				}
			}
			return result;
		}

		protected override RopResult OpenStream(MapiContext context, MapiPropBagBase propertyBag, PropertyTag propertyTag, OpenMode openMode, OpenStreamResultFactory resultFactory)
		{
			if (propertyBag is MapiLogon)
			{
				DiagnosticContext.TraceLocation((LID)54920U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			StreamFlags streamFlags = (StreamFlags)0;
			bool flag = false;
			if (4 == (byte)(OpenMode.OpenSoftDeleted & openMode))
			{
				streamFlags |= StreamFlags.AllowAppend;
				flag = true;
			}
			openMode &= ~OpenMode.OpenSoftDeleted;
			if (flag && OpenMode.Create != openMode && OpenMode.ReadWrite != openMode)
			{
				DiagnosticContext.TraceLocation((LID)42632U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942405U);
			}
			if (OpenMode.Create == openMode)
			{
				streamFlags |= (StreamFlags.AllowCreate | StreamFlags.AllowRead | StreamFlags.AllowWrite);
			}
			else if (openMode == OpenMode.ReadOnly)
			{
				streamFlags |= StreamFlags.AllowRead;
			}
			else if (OpenMode.ReadWrite == openMode)
			{
				streamFlags |= (StreamFlags.AllowRead | StreamFlags.AllowWrite);
			}
			else
			{
				if (OpenMode.BestAccess != openMode)
				{
					DiagnosticContext.TraceLocation((LID)59016U);
					return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
				}
				streamFlags |= (StreamFlags.AllowRead | StreamFlags.AllowWrite);
			}
			if (propertyBag is MapiFolder && propertyTag.PropertyType != PropertyType.Binary)
			{
				DiagnosticContext.TraceLocation((LID)34440U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			MapiStream mapiStream = null;
			RopResult result;
			try
			{
				MapiLogon logon = propertyBag.Logon;
				StorePropTag propTag = LegacyHelper.ConvertFromLegacyPropTag(propertyTag, Helper.GetPropTagObjectType(propertyBag.MapiObjectType), logon.MapiMailbox, true);
				CodePage codePage = logon.CodePage;
				if (PropertyType.Unicode == propertyTag.PropertyType)
				{
					codePage = CodePage.Unicode;
				}
				else if (propertyBag is MapiMessage)
				{
					codePage = ((MapiMessage)propertyBag).CodePage;
				}
				ErrorCode errorCode = propertyBag.OpenStream(context, streamFlags, propTag, codePage, out mapiStream);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)41392U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					uint streamSize = (uint)mapiStream.GetSize(context);
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiStream, streamSize);
					mapiStream = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiStream != null)
				{
					mapiStream.Dispose();
				}
			}
			return result;
		}

		protected override RopResult PrereadMessages(MapiContext context, MapiLogon logon, StoreIdPair[] messages, PrereadMessagesResultFactory resultFactory)
		{
			if (logon == null)
			{
				DiagnosticContext.TraceLocation((LID)50896U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746056U);
			}
			if (messages != null)
			{
				List<FidMid> list = new List<FidMid>(messages.Length);
				for (int i = 0; i < messages.Length; i++)
				{
					ExchangeId folderId = RcaTypeHelpers.StoreIdToExchangeId(messages[i].First, logon.MapiMailbox.StoreMailbox);
					ExchangeId messageId = RcaTypeHelpers.StoreIdToExchangeId(messages[i].Second, logon.MapiMailbox.StoreMailbox);
					list.Add(new FidMid(folderId, messageId));
				}
				TopMessage.PreReadMessages(context, logon.MapiMailbox.StoreMailbox, list);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult QueryColumnsAll(MapiContext context, MapiViewTableBase view, QueryColumnsAllResultFactory resultFactory)
		{
			if (view == null)
			{
				return resultFactory.CreateSuccessfulResult(Array<PropertyTag>.Empty);
			}
			IList<StorePropTag> propTags = view.QueryColumnsAll();
			PropertyTag[] array;
			LegacyHelper.ConvertToLegacyPropTags(propTags, out array);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsStringProperty && array[i].PropertyType == PropertyType.Unicode)
				{
					array[i] = array[i].ChangeElementPropertyType(PropertyType.String8);
				}
			}
			return resultFactory.CreateSuccessfulResult(array);
		}

		protected override RopResult QueryNamedProperties(MapiContext context, MapiPropBagBase propertyBag, QueryNamedPropertyFlags queryFlags, Guid? propertyGuid, QueryNamedPropertiesResultFactory resultFactory)
		{
			MapiLogon logon = propertyBag.Logon;
			IList<ushort> list = LegacyHelper.QueryNamedProps(context, (MapiQryNamedPropsFlags)queryFlags, propertyGuid, logon.MapiMailbox);
			PropertyId[] array = new PropertyId[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = (PropertyId)list[i];
			}
			NamedProperty[] namesFromPropertyIDs = RcaTypeHelpers.GetNamesFromPropertyIDs(context, array, logon.MapiMailbox);
			return resultFactory.CreateSuccessfulResult(array, namesFromPropertyIDs);
		}

		protected override RopResult QueryPosition(MapiContext context, MapiViewTableBase view, QueryPositionResultFactory resultFactory)
		{
			int numerator = 0;
			int denominator = 0;
			Stopwatch stopwatch = (view.CorrelationId != null) ? Stopwatch.StartNew() : null;
			try
			{
				if (view.MayNeedIndexForQueryPosition(context))
				{
					ErrorCode errorCode = this.ChunkedPrepareIfNecessary(context, view.PrepareIndexes(context, null));
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)37728U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode);
					}
				}
				view.QueryPosition(context, ref numerator, ref denominator);
			}
			finally
			{
				if (view.CorrelationId != null)
				{
					stopwatch.Stop();
					FullTextIndexLogger.LogViewOperation(context.DatabaseGuid, context.Diagnostics.MailboxNumber, (int)context.ClientType, view.CorrelationId.Value, FullTextIndexLogger.ViewOperationType.QueryPosition, stopwatch.ToTimeSpan(), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Read), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Read), view.IsOptimizedInstantSearch);
				}
			}
			return resultFactory.CreateSuccessfulResult((uint)numerator, (uint)denominator);
		}

		protected override RopResult QueryRows(MapiContext context, MapiViewTableBase view, QueryRowsFlags flags, bool useForwardDirection, ushort rowCount, QueryRowsResultFactory resultFactory)
		{
			if ((byte)(flags & ~(QueryRowsFlags.DoNotAdvance | QueryRowsFlags.SendMax | QueryRowsFlags.ChainAlways)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)50824U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if (!useForwardDirection)
			{
				flags |= QueryRowsFlags.Backwards;
			}
			RopResult result;
			using (RowCollector rowCollector = resultFactory.CreateRowCollector())
			{
				PropertyTag[] columns;
				LegacyHelper.ConvertToLegacyPropTags(view.ViewColumns, out columns);
				rowCollector.SetColumns(columns);
				bool flag = false;
				Stopwatch stopwatch = (view.CorrelationId != null) ? Stopwatch.StartNew() : null;
				try
				{
					ErrorCode errorCode = this.ChunkedPrepareIfNecessary(context, view.PrepareIndexes(context, null));
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)54112U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode);
					}
					using (MapiViewTableBase.RowReader rowReader = view.QueryRows(context, (int)rowCount, flags))
					{
						MapiViewTableBase.RowColumnReader rowColumnReader;
						while (rowReader.ReadNextRow(context, out rowColumnReader))
						{
							PropertyValue[] array = null;
							int num = 0;
							Property prop;
							while (rowColumnReader.ReadNextColumn(context, out prop))
							{
								if (array == null)
								{
									array = new PropertyValue[rowColumnReader.ColumnCount];
								}
								MapiViewTableBase.TruncateLongValue(context, view, num, ref prop);
								array[num] = RcaTypeHelpers.MassageOutgoingProperty(prop, false);
								num++;
							}
							bool flag2 = false;
							try
							{
								flag2 = rowCollector.TryAddRow((array != null) ? array : Array<PropertyValue>.Empty);
							}
							finally
							{
								if (!flag2)
								{
									rowReader.UnreadLast();
								}
							}
							if (!flag2)
							{
								flag = true;
								break;
							}
						}
					}
				}
				finally
				{
					if (view.CorrelationId != null)
					{
						stopwatch.Stop();
						FullTextIndexLogger.LogViewOperation(context.DatabaseGuid, context.Diagnostics.MailboxNumber, (int)context.ClientType, view.CorrelationId.Value, FullTextIndexLogger.ViewOperationType.QueryRows, stopwatch.ToTimeSpan(), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Read), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Read), view.IsOptimizedInstantSearch);
					}
				}
				BookmarkOrigin bookmarkOrigin = BookmarkOrigin.Current;
				if ((byte)(flags & QueryRowsFlags.DoNotAdvance) == 0 && rowCount > rowCollector.RowCount && !flag)
				{
					bookmarkOrigin = (useForwardDirection ? BookmarkOrigin.End : BookmarkOrigin.Beginning);
				}
				result = resultFactory.CreateSuccessfulResult(bookmarkOrigin, rowCollector);
			}
			return result;
		}

		protected override RopResult ReadPerUserInformation(MapiContext context, MapiLogon logon, StoreLongTermId folderId, bool wantIfChanged, uint dataOffset, ushort maxDataSize, ReadPerUserInformationResultFactory resultFactory)
		{
			bool hasFinished;
			byte[] data;
			ErrorCode errorCode = logon.ReadPerUserInformation(context, folderId, dataOffset, maxDataSize, out hasFinished, out data);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)30176U);
				return resultFactory.CreateFailedResult((ErrorCode)((int)errorCode));
			}
			return resultFactory.CreateSuccessfulResult(hasFinished, data);
		}

		protected override RopResult ReadRecipients(MapiContext context, MapiMessage message, uint recipientRowId, PropertyTag[] extraUnicodePropertyTags, ReadRecipientsResultFactory resultFactory)
		{
			if (extraUnicodePropertyTags == null)
			{
				throw new ArgumentNullException("extraUnicodePropertyTags");
			}
			IList<StorePropTag> recipientPropListExtra = message.GetRecipientPropListExtra();
			PropertyTag[] extraPropertyTags;
			LegacyHelper.ConvertToLegacyPropTags(recipientPropListExtra, out extraPropertyTags);
			MapiLogon logon = message.Logon;
			IList<StorePropTag> extraUnicodeTags = LegacyHelper.ConvertFromLegacyPropTags(extraUnicodePropertyTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient, logon.MapiMailbox, true);
			RopResult result;
			using (RecipientCollector recipientCollector = resultFactory.CreateRecipientCollector(extraPropertyTags))
			{
				bool flag = false;
				foreach (RecipientRow row in RopHandler.GetRecipientRows(context, message, recipientRowId, (ushort)message.CodePage, recipientPropListExtra, extraUnicodeTags))
				{
					if (!recipientCollector.TryAddRecipientRow(row))
					{
						if (!flag)
						{
							throw new BufferTooSmallException();
						}
						break;
					}
					else
					{
						flag = true;
					}
				}
				if (!flag)
				{
					DiagnosticContext.TraceLocation((LID)47752U);
					result = resultFactory.CreateFailedResult((ErrorCode)2147746063U);
				}
				else
				{
					result = resultFactory.CreateSuccessfulResult(recipientCollector);
				}
			}
			return result;
		}

		protected override RopResult ReadStream(MapiContext context, MapiStream stream, ushort byteCount, ReadStreamResultFactory resultFactory)
		{
			int count = 0;
			byte[] array = stream.Read(context, (int)byteCount, out count);
			return resultFactory.CreateSuccessfulResult(new ArraySegment<byte>(array, 0, count));
		}

		protected override RopResult RegisterNotification(MapiContext context, MapiLogon logon, NotificationFlags flags, NotificationEventFlags eventFlags, bool wantGlobalScope, ExchangeId folderId, ExchangeId messageId, RegisterNotificationResultFactory resultFactory)
		{
			EventType eventType = (EventType)((ushort)(flags & (NotificationFlags.CriticalError | NotificationFlags.NewMail | NotificationFlags.ObjectCreated | NotificationFlags.ObjectDeleted | NotificationFlags.ObjectModified | NotificationFlags.ObjectMoved | NotificationFlags.ObjectCopied | NotificationFlags.SearchComplete | NotificationFlags.TableModified | NotificationFlags.Ics)));
			if ((ushort)(flags & NotificationFlags.Extended) != 0)
			{
				eventType |= EventType.Extended;
			}
			MapiNotify mapiNotify = null;
			RopResult result;
			try
			{
				mapiNotify = new MapiNotify();
				ErrorCode errorCode = mapiNotify.Configure(context, logon, eventType, wantGlobalScope, folderId, messageId, resultFactory.ServerObjectHandle.HandleValue);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)64136U);
					result = resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
				else
				{
					RopResult ropResult = resultFactory.CreateSuccessfulResult(mapiNotify);
					mapiNotify = null;
					result = ropResult;
				}
			}
			finally
			{
				if (mapiNotify != null)
				{
					mapiNotify.Dispose();
				}
			}
			return result;
		}

		protected override void Release(MapiContext context, MapiBase serverObject)
		{
			serverObject.OnRelease(context);
		}

		protected override RopResult ReloadCachedInformation(MapiContext context, MapiMessage message, PropertyTag[] extraUnicodePropertyTags, ReloadCachedInformationResultFactory resultFactory)
		{
			if (extraUnicodePropertyTags == null)
			{
				throw new ArgumentNullException("extraUnicodePropertyTags");
			}
			RopResult result;
			using (RecipientCollector recipientCollector = RopHandler.CreateMessageHeader(context, message, resultFactory, message.CodePage, extraUnicodePropertyTags))
			{
				result = resultFactory.CreateSuccessfulResult(recipientCollector);
			}
			return result;
		}

		protected override RopResult RemoveAllRecipients(MapiContext context, MapiMessage message, RemoveAllRecipientsResultFactory resultFactory)
		{
			MapiPersonCollection recipients = message.GetRecipients();
			recipients.DeleteAll();
			recipients.SaveChanges(context);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult ResetTable(MapiContext context, MapiViewTableBase view, ResetTableResultFactory resultFactory)
		{
			view.ResetTable();
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult Restrict(MapiContext context, MapiViewTableBase view, RestrictFlags flags, Restriction restriction, RestrictResultFactory resultFactory)
		{
			MapiLogon logon = view.Logon;
			Restriction restriction2 = RcaTypeHelpers.RestrictionToRestriction(context, restriction, logon, view.MapiObjectType);
			view.Restrict(context, (int)flags, restriction2);
			return resultFactory.CreateSuccessfulResult(TableStatus.Complete);
		}

		protected override RopResult SaveChangesAttachment(MapiContext context, MapiAttachment attachment, SaveChangesMode saveChangesMode, SaveChangesAttachmentResultFactory resultFactory)
		{
			if ((byte)(saveChangesMode & (SaveChangesMode.TransportDelivery | SaveChangesMode.IMAPChange | SaveChangesMode.ForceNotificationPublish)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)39560U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if ((byte)(saveChangesMode & SaveChangesMode.KeepOpenReadOnly) != 0 && (byte)(saveChangesMode & SaveChangesMode.KeepOpenReadWrite) != 0)
			{
				DiagnosticContext.TraceLocation((LID)55944U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			ErrorCode errorCode = this.ChunkedPrepareIfNecessary(context, attachment.PrepareToSaveChanges(context));
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)51228U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			attachment.SaveChanges(context);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult SaveChangesMessage(MapiContext context, MapiMessage message, SaveChangesMode saveChangesMode, SaveChangesMessageResultFactory resultFactory)
		{
			if ((byte)0 != 0)
			{
				DiagnosticContext.TraceLocation((LID)43656U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if ((byte)(saveChangesMode & SaveChangesMode.KeepOpenReadOnly) != 0 && (byte)(saveChangesMode & SaveChangesMode.KeepOpenReadWrite) != 0)
			{
				DiagnosticContext.TraceLocation((LID)60040U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			if ((byte)(saveChangesMode & SaveChangesMode.TransportDelivery) != 0 && !message.Logon.ExchangeTransportServiceRights)
			{
				DiagnosticContext.TraceLocation((LID)35464U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			MapiSaveMessageChangesFlags mapiSaveMessageChangesFlags = MapiSaveMessageChangesFlags.None;
			if ((byte)(saveChangesMode & SaveChangesMode.IMAPChange) == 64)
			{
				mapiSaveMessageChangesFlags |= MapiSaveMessageChangesFlags.IMAPIDChange;
			}
			if ((byte)(saveChangesMode & SaveChangesMode.ForceSave) == 4)
			{
				mapiSaveMessageChangesFlags |= MapiSaveMessageChangesFlags.ForceSave;
			}
			if ((byte)(saveChangesMode & SaveChangesMode.SkipQuotaCheck) == 16)
			{
				mapiSaveMessageChangesFlags |= MapiSaveMessageChangesFlags.SkipQuotaCheck;
			}
			context.ForceNotificationPublish = ((byte)(saveChangesMode & SaveChangesMode.ForceNotificationPublish) == 128);
			ErrorCode errorCode;
			if (ConfigurationSchema.FixMessageCreatorSidOnMailboxMove.Value && message.Logon.IsMoveDestination && !message.IsEmbedded && ((TopMessage)message.StoreMessage).GetCreatorSecurityIdentifier(context) == null)
			{
				string text = null;
				string a = null;
				string text2 = null;
				byte[] array = (byte[])message.GetOnePropValue(context, PropTag.Message.CreatorEntryId);
				Eidt eidt;
				MapiAPIFlags mapiAPIFlags;
				if (array != null && (AddressBookEID.IsAddressBookEntryId(context, array, out eidt, out text) || (AddressBookEID.IsOneOffEntryId(context, array, out mapiAPIFlags, ref a, ref text, ref text2) && a == "EX")) && !string.IsNullOrEmpty(text))
				{
					SecurityIdentifier securityIdentifier = null;
					context.EndMailboxOperation(false, true);
					try
					{
						AddressInfo addressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfo(context, message.Logon.MapiMailbox.SharedState.TenantHint, text, false);
						securityIdentifier = SecurityHelper.ComputeObjectSID(addressInfo.UserSid, addressInfo.MasterAccountSid);
					}
					catch (UserNotFoundException exception)
					{
						context.OnExceptionCatch(exception);
					}
					catch (NonUniqueRecipientException exception2)
					{
						context.OnExceptionCatch(exception2);
					}
					errorCode = context.StartMailboxOperation();
					if (errorCode != ErrorCode.NoError)
					{
						return resultFactory.CreateFailedResult((ErrorCode)errorCode);
					}
					if (securityIdentifier != null)
					{
						((TopMessage)message.StoreMessage).SetCreatorSecurityIdentifier(context, securityIdentifier);
					}
				}
			}
			errorCode = this.ChunkedPrepareIfNecessary(context, message.PrepareToSaveChanges(context));
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)45084U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			ExchangeId exchangeId;
			errorCode = message.SaveChanges(context, mapiSaveMessageChangesFlags, out exchangeId);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)51848U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult(RcaTypeHelpers.ExchangeIdToStoreId(exchangeId));
		}

		protected override RopResult SeekRow(MapiContext context, MapiViewTableBase view, BookmarkOrigin bookmarkOrigin, int rowCount, bool wantMoveCount, SeekRowResultFactory resultFactory)
		{
			if (bookmarkOrigin != BookmarkOrigin.Beginning && bookmarkOrigin != BookmarkOrigin.Current && bookmarkOrigin != BookmarkOrigin.End)
			{
				DiagnosticContext.TraceLocation((LID)45704U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			Stopwatch stopwatch = (view.CorrelationId != null) ? Stopwatch.StartNew() : null;
			bool soughtLessThanRequested;
			int rowsSought;
			try
			{
				if (bookmarkOrigin != BookmarkOrigin.Beginning || rowCount > 0)
				{
					ErrorCode errorCode = this.ChunkedPrepareIfNecessary(context, view.PrepareIndexes(context, null));
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)41824U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode);
					}
				}
				bool flag;
				view.SeekRow(context, (ViewSeekOrigin)bookmarkOrigin, null, rowCount, wantMoveCount, out soughtLessThanRequested, out rowsSought, false, out flag);
			}
			finally
			{
				if (view.CorrelationId != null)
				{
					stopwatch.Stop();
					FullTextIndexLogger.LogViewOperation(context.DatabaseGuid, context.Diagnostics.MailboxNumber, (int)context.ClientType, view.CorrelationId.Value, FullTextIndexLogger.ViewOperationType.SeekRow, stopwatch.ToTimeSpan(), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.LazyIndex, RowStatsCounterType.Read), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Seek), context.Diagnostics.RowStatistics.GetCounter(RowStatsTableClassIndex.Message, RowStatsCounterType.Read), view.IsOptimizedInstantSearch);
				}
			}
			if (!wantMoveCount)
			{
				soughtLessThanRequested = false;
				rowsSought = 0;
			}
			return resultFactory.CreateSuccessfulResult(soughtLessThanRequested, rowsSought);
		}

		protected override RopResult SeekRowApproximate(MapiContext context, MapiViewTableBase view, uint numerator, uint denominator, SeekRowApproximateResultFactory resultFactory)
		{
			if (numerator != 0U)
			{
				ErrorCode errorCode = this.ChunkedPrepareIfNecessary(context, view.PrepareIndexes(context, null));
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)62492U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
			}
			view.SeekRowApprox(context, (int)numerator, (int)denominator);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult SeekRowBookmark(MapiContext context, MapiViewTableBase view, byte[] bookmark, int rowCount, bool wantMoveCount, SeekRowBookmarkResultFactory resultFactory)
		{
			if (bookmark == null)
			{
				throw new ArgumentNullException("bookmark");
			}
			if (view.MapiObjectType != MapiObjectType.MessageView && view.MapiObjectType != MapiObjectType.FolderView)
			{
				DiagnosticContext.TraceLocation((LID)48552U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			ErrorCode errorCode = this.ChunkedPrepareIfNecessary(context, view.PrepareIndexes(context, null));
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)37916U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			bool soughtLessThanRequested;
			int rowsSought;
			bool positionChanged;
			view.SeekRow(context, ViewSeekOrigin.Bookmark, bookmark, rowCount, wantMoveCount, out soughtLessThanRequested, out rowsSought, true, out positionChanged);
			if (!wantMoveCount)
			{
				soughtLessThanRequested = false;
				rowsSought = 0;
			}
			return resultFactory.CreateSuccessfulResult(positionChanged, soughtLessThanRequested, rowsSought);
		}

		protected override RopResult SeekStream(MapiContext context, MapiStream stream, StreamSeekOrigin streamSeekOrigin, long offset, SeekStreamResultFactory resultFactory)
		{
			SeekOrigin origin;
			switch (streamSeekOrigin)
			{
			case StreamSeekOrigin.Begin:
				origin = SeekOrigin.Begin;
				break;
			case StreamSeekOrigin.Current:
				origin = SeekOrigin.Current;
				break;
			case StreamSeekOrigin.End:
				origin = SeekOrigin.End;
				break;
			default:
				DiagnosticContext.TraceLocation((LID)53896U);
				return resultFactory.CreateFailedResult((ErrorCode)2147680343U);
			}
			long resultOffset = stream.Seek(context, offset, origin);
			return resultFactory.CreateSuccessfulResult((ulong)resultOffset);
		}

		protected override RopResult SetCollapseState(MapiContext context, MapiViewTableBase serverObject, byte[] collapseState, SetCollapseStateResultFactory resultFactory)
		{
			byte[] bookmark = serverObject.SetCollapseState(context, collapseState);
			return resultFactory.CreateSuccessfulResult(bookmark);
		}

		protected override RopResult SetColumns(MapiContext context, MapiViewTableBase view, SetColumnsFlags flags, PropertyTag[] propertyTags, SetColumnsResultFactory resultFactory)
		{
			if (propertyTags == null)
			{
				throw new ArgumentNullException("propertyTags");
			}
			if ((byte)(flags & ~SetColumnsFlags.Asynchronous) != 0)
			{
				DiagnosticContext.TraceLocation((LID)64936U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (propertyTags.Length == 0)
			{
				DiagnosticContext.TraceLocation((LID)40360U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			int i = 0;
			while (i < propertyTags.Length)
			{
				PropertyTag propertyTag = propertyTags[i];
				PropertyType propertyType = propertyTag.PropertyType;
				if (propertyType <= PropertyType.Actions)
				{
					if (propertyType <= PropertyType.Unicode)
					{
						switch (propertyType)
						{
						case PropertyType.Int16:
						case PropertyType.Int32:
						case PropertyType.Float:
						case PropertyType.Double:
						case PropertyType.Currency:
						case PropertyType.AppTime:
						case PropertyType.Bool:
						case PropertyType.Object:
							break;
						case (PropertyType)8:
						case (PropertyType)9:
						case PropertyType.Error:
						case (PropertyType)12:
							goto IL_19A;
						default:
							if (propertyType != PropertyType.Int64)
							{
								switch (propertyType)
								{
								case PropertyType.String8:
								case PropertyType.Unicode:
									break;
								default:
									goto IL_19A;
								}
							}
							break;
						}
					}
					else if (propertyType != PropertyType.SysTime && propertyType != PropertyType.Guid)
					{
						switch (propertyType)
						{
						case PropertyType.ServerId:
						case PropertyType.Restriction:
						case PropertyType.Actions:
							break;
						case (PropertyType)252:
							goto IL_19A;
						default:
							goto IL_19A;
						}
					}
				}
				else if (propertyType <= PropertyType.MultiValueInt64)
				{
					if (propertyType != PropertyType.Binary)
					{
						switch (propertyType)
						{
						case PropertyType.MultiValueInt16:
						case PropertyType.MultiValueInt32:
						case PropertyType.MultiValueFloat:
						case PropertyType.MultiValueDouble:
						case PropertyType.MultiValueCurrency:
						case PropertyType.MultiValueAppTime:
							break;
						default:
							if (propertyType != PropertyType.MultiValueInt64)
							{
								goto IL_19A;
							}
							break;
						}
					}
				}
				else if (propertyType <= PropertyType.MultiValueSysTime)
				{
					switch (propertyType)
					{
					case PropertyType.MultiValueString8:
					case PropertyType.MultiValueUnicode:
						break;
					default:
						if (propertyType != PropertyType.MultiValueSysTime)
						{
							goto IL_19A;
						}
						break;
					}
				}
				else if (propertyType != PropertyType.MultiValueGuid && propertyType != PropertyType.MultiValueBinary)
				{
					goto IL_19A;
				}
				i++;
				continue;
				IL_19A:
				DiagnosticContext.TraceLocation((LID)41608U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			MapiLogon logon = view.Logon;
			StorePropTag[] columns = LegacyHelper.ConvertFromLegacyPropTags(propertyTags, Helper.GetPropTagObjectType(view.MapiObjectType), logon.MapiMailbox, false);
			MapiViewSetColumnsFlag flags2 = MapiViewSetColumnsFlag.None;
			if (this.SkipColumnPropertiesPromotionValidation(context))
			{
				flags2 = MapiViewSetColumnsFlag.NoColumnValidation;
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.TraceDebug(0L, "Properties promotion validation skipped.");
				}
			}
			view.SetColumns(context, columns, flags2);
			return resultFactory.CreateSuccessfulResult(TableStatus.Complete);
		}

		protected override RopResult SetMessageFlags(MapiContext context, MapiFolder folder, ExchangeId messageId, MessageFlags flags, MessageFlags flagsMask, SetMessageFlagsResultFactory resultFactory)
		{
			folder.SetMessageFlags(context, messageId, (int)flags, (int)flagsMask);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult SetMessageStatus(MapiContext context, MapiFolder folder, ExchangeId messageId, MessageStatusFlags status, MessageStatusFlags statusMask, SetMessageStatusResultFactory resultFactory)
		{
			int oldStatus = 0;
			MapiLogon logon = folder.Logon;
			folder.SetMessageStatus(context, messageId, (int)status, (int)statusMask, out oldStatus);
			return resultFactory.CreateSuccessfulResult((MessageStatusFlags)oldStatus);
		}

		protected override RopResult SetProperties(MapiContext context, MapiPropBagBase propertyBag, PropertyValue[] propertyValues, SetPropertiesResultFactory resultFactory)
		{
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			List<MapiPropertyProblem> problems = null;
			Properties properties = new Properties(propertyValues.Length);
			RopHandler.AddProperties(propertyValues, propertyBag.MapiObjectType, propertyBag.Logon.MapiMailbox, properties, null);
			propertyBag.SetProps(context, properties, ref problems);
			PropertyProblem[] propertyProblems = Array<PropertyProblem>.Empty;
			if (!(propertyBag is MapiMessage) && !(propertyBag is MapiAttachment))
			{
				propertyProblems = RcaTypeHelpers.PropertyProblemFromMapiPropertyPropblemAndValues(problems, propertyValues);
			}
			return resultFactory.CreateSuccessfulResult(propertyProblems);
		}

		protected override RopResult SetPropertiesNoReplicate(MapiContext context, MapiPropBagBase propertyBag, PropertyValue[] propertyValues, SetPropertiesNoReplicateResultFactory resultFactory)
		{
			List<MapiPropertyProblem> problems = null;
			Properties properties = new Properties(propertyValues.Length);
			RopHandler.AddProperties(propertyValues, propertyBag.MapiObjectType, propertyBag.Logon.MapiMailbox, properties, null);
			try
			{
				propertyBag.NoReplicateOperationInProgress = true;
				propertyBag.SetProps(context, properties, ref problems);
			}
			finally
			{
				propertyBag.NoReplicateOperationInProgress = false;
			}
			PropertyProblem[] propertyProblems = Array<PropertyProblem>.Empty;
			if (!(propertyBag is MapiMessage) && !(propertyBag is MapiAttachment))
			{
				propertyProblems = RcaTypeHelpers.PropertyProblemFromMapiPropertyPropblemAndValues(problems, propertyValues);
			}
			return resultFactory.CreateSuccessfulResult(propertyProblems);
		}

		protected override RopResult SetReadFlag(MapiContext context, MapiMessage message, SetReadFlagFlags flags, SetReadFlagResultFactory resultFactory)
		{
			if ((byte)(flags & ~(SetReadFlagFlags.SuppressReceipt | SetReadFlagFlags.FolderMessageDialog | SetReadFlagFlags.ClearReadFlag | SetReadFlagFlags.DeferredErrors | SetReadFlagFlags.GenerateReceiptOnly | SetReadFlagFlags.ClearReadNotificationPending | SetReadFlagFlags.ClearNonReadNotificationPending)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)57992U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			bool hasChanged;
			message.SetReadFlag(context, flags, out hasChanged);
			return resultFactory.CreateSuccessfulResult(hasChanged);
		}

		protected override RopResult SetReadFlags(MapiContext context, MapiFolder folder, bool reportProgress, SetReadFlagFlags flags, ExchangeId[] messageIds, out bool partiallyCompleted, SetReadFlagsResultFactory resultFactory)
		{
			partiallyCompleted = false;
			bool flag = false;
			if ((byte)(flags & ~(SetReadFlagFlags.SuppressReceipt | SetReadFlagFlags.FolderMessageDialog | SetReadFlagFlags.ClearReadFlag | SetReadFlagFlags.DeferredErrors | SetReadFlagFlags.GenerateReceiptOnly | SetReadFlagFlags.ClearReadNotificationPending | SetReadFlagFlags.ClearNonReadNotificationPending)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)33416U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U, false);
			}
			using (SetReadFlagsOperation setReadFlagsOperation = new SetReadFlagsOperation(folder, messageIds, flags))
			{
				bool flag2;
				bool flag3;
				ErrorCode errorCode;
				while (!setReadFlagsOperation.DoChunk(context, out flag2, out flag3, out errorCode))
				{
					partiallyCompleted = (partiallyCompleted || flag2);
					flag = (flag || flag3);
					errorCode = context.PulseMailboxOperation();
					if (errorCode != ErrorCode.NoError)
					{
						DiagnosticContext.TraceLocation((LID)39484U);
						return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
					}
				}
				partiallyCompleted = (partiallyCompleted || flag2);
				flag = (flag || flag3);
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)49800U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode, partiallyCompleted);
				}
			}
			return resultFactory.CreateSuccessfulResult(flag);
		}

		protected override RopResult SetReceiveFolder(MapiContext context, MapiBase serverObject, ExchangeId folderId, string messageClass, SetReceiveFolderResultFactory resultFactory)
		{
			if (messageClass == null)
			{
				throw new ArgumentNullException("messageClass");
			}
			if (!MessageClassHelper.IsValidMessageClass(messageClass) || messageClass.Length > 255)
			{
				DiagnosticContext.TraceLocation((LID)64872U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (messageClass.Equals("IPM", StringComparison.OrdinalIgnoreCase) || messageClass.Equals("Report.IPM", StringComparison.OrdinalIgnoreCase))
			{
				DiagnosticContext.TraceLocation((LID)40296U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942405U);
			}
			if (folderId.IsNullOrZero && string.IsNullOrEmpty(messageClass))
			{
				DiagnosticContext.TraceLocation((LID)56680U);
				return resultFactory.CreateFailedResult((ErrorCode)2147500037U);
			}
			serverObject.Logon.SetReceiveFolder(context, messageClass, folderId);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult SetSearchCriteria(MapiContext context, MapiFolder folder, Restriction restriction, ExchangeId[] folderIds, SetSearchCriteriaFlags flags, SetSearchCriteriaResultFactory resultFactory)
		{
			byte[] legacyRestriction = null;
			if (folder.IsInstantSearch)
			{
				Guid correlationId = CorrelationIdHelper.GetCorrelationId(context.Diagnostics.MailboxNumber, folder.Fid.ToLong());
				FullTextIndexLogger.LogViewSetSearchCriteria(context.DatabaseGuid, context.Diagnostics.MailboxNumber, (int)context.ClientType, correlationId);
			}
			if (restriction != null)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (Writer writer = new StreamWriter(memoryStream))
					{
						restriction.Serialize(writer, CTSGlobals.AsciiEncoding, WireFormatStyle.Rop);
						legacyRestriction = memoryStream.ToArray();
					}
				}
			}
			ErrorCode errorCode = folder.SetSearchCriteria(context, legacyRestriction, folderIds, flags);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)44392U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult SetSizeStream(MapiContext context, MapiStream stream, ulong streamSize, SetSizeStreamResultFactory resultFactory)
		{
			if (streamSize > 2147483647UL)
			{
				DiagnosticContext.TraceLocation((LID)60776U);
				return resultFactory.CreateFailedResult(ErrorCode.MaxSubmissionExceeded);
			}
			stream.SetSize(context, (long)streamSize);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult SetSpooler(MapiContext context, MapiLogon logon, SetSpoolerResultFactory resultFactory)
		{
			ErrorCode errorCode = logon.SetSpooler();
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)36200U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult SetTransport(MapiContext context, MapiLogon logon, SetTransportResultFactory resultFactory)
		{
			ExchangeId exchangeId;
			logon.SetTransportProvider(out exchangeId);
			return resultFactory.CreateSuccessfulResult(RcaTypeHelpers.ExchangeIdToStoreId(exchangeId));
		}

		protected override RopResult SortTable(MapiContext context, MapiViewTableBase view, SortTableFlags flags, ushort categoryCount, ushort expandedCount, Microsoft.Exchange.RpcClientAccess.Parser.SortOrder[] sortOrders, SortTableResultFactory resultFactory)
		{
			if (sortOrders == null)
			{
				throw new ArgumentNullException("sortOrders");
			}
			view.Sort(context, sortOrders, flags, (uint)categoryCount, (uint)expandedCount);
			return resultFactory.CreateSuccessfulResult(TableStatus.Complete);
		}

		protected override RopResult SpoolerLockMessage(MapiContext context, MapiLogon logon, ExchangeId messageId, LockState lockState, SpoolerLockMessageResultFactory resultFactory)
		{
			if (lockState != LockState.Locked && lockState != LockState.Unlocked && lockState != LockState.Finished)
			{
				DiagnosticContext.TraceLocation((LID)52584U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!logon.SpoolerRights)
			{
				DiagnosticContext.TraceLocation((LID)46440U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			ErrorCode first = logon.SpoolerLockMessage(context, messageId, lockState);
			if (first != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)62824U);
				return resultFactory.CreateFailedResult((ErrorCode)2147747329U);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult SubmitMessage(MapiContext context, MapiMessage message, SubmitMessageFlags submitFlags, SubmitMessageResultFactory resultFactory)
		{
			if ((byte)(submitFlags & ~(SubmitMessageFlags.Preprocess | SubmitMessageFlags.NeedsSpooler | SubmitMessageFlags.IgnoreSendAsRight)) != 0)
			{
				DiagnosticContext.TraceLocation((LID)38248U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			RopFlags ropFlags = RopFlags.ReadOnly;
			if ((byte)(submitFlags & SubmitMessageFlags.Preprocess) != 0)
			{
				ropFlags |= RopFlags.Private;
			}
			if ((byte)(submitFlags & SubmitMessageFlags.NeedsSpooler) != 0)
			{
				ropFlags |= RopFlags.NeedsSpooler;
			}
			if ((byte)(submitFlags & SubmitMessageFlags.IgnoreSendAsRight) != 0)
			{
				ropFlags |= RopFlags.IgnoreSendAsRight;
			}
			byte[] sentRepresentingEntryId;
			bool flag;
			string text;
			message.GetInformationForSetSender(context, out sentRepresentingEntryId, out flag, out text);
			AddressInfo addressInfo = null;
			ErrorCode errorCode = ErrorCode.NoError;
			SubmitMessageRightsCheckFlags submitMessageRightsCheckFlags = SubmitMessageRightsCheckFlags.None;
			if (message.Logon.IsPrimaryOwner && string.Compare(text, message.Logon.Session.AddressInfoUser.LegacyExchangeDN, StringComparison.OrdinalIgnoreCase) == 0)
			{
				DiagnosticContext.TraceLocation((LID)33596U);
				addressInfo = message.Logon.Session.AddressInfoUser;
				if (addressInfo.IsDistributionList)
				{
					submitMessageRightsCheckFlags |= SubmitMessageRightsCheckFlags.SendingAsDL;
				}
				submitMessageRightsCheckFlags |= SubmitMessageRightsCheckFlags.SendAsRights;
			}
			else if (!flag)
			{
				DiagnosticContext.TraceLocation((LID)44092U);
				bool ndrMessage = message.IsNDR(context);
				bool quotaMessage = message.IsQuotaMessage(context);
				context.EndMailboxOperation(true, true);
				try
				{
					addressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfo(context, message.Logon.MapiMailbox.SharedState.TenantHint, text, false);
					Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.RefreshMailboxInfo(context, addressInfo.ObjectId);
					addressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfo(context, message.Logon.MapiMailbox.SharedState.TenantHint, text, true);
				}
				catch (UserNotFoundException exception)
				{
					context.OnExceptionCatch(exception);
					DiagnosticContext.TraceDwordAndString((LID)36423U, 0U, text);
					return resultFactory.CreateFailedResult((ErrorCode)1244U);
				}
				RopHandler.CheckSendAsSOBORights(context, message.Logon, addressInfo, ndrMessage, quotaMessage, out submitMessageRightsCheckFlags);
				errorCode = context.StartMailboxOperation();
				if (errorCode != ErrorCode.NoError)
				{
					return resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
			}
			errorCode = message.SubmitMessage(context, ropFlags, sentRepresentingEntryId, addressInfo, submitMessageRightsCheckFlags);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)54632U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult TellVersion(MapiContext context, FastTransferContext fxContext, ushort productVersion, ushort buildMajorVersion, ushort buildMinorVersion, TellVersionResultFactory resultFactory)
		{
			MapiVersion mapiVersion;
			try
			{
				mapiVersion = new MapiVersion(productVersion, buildMajorVersion, buildMinorVersion);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				context.OnExceptionCatch(ex);
				throw new ExExceptionInvalidParameter((LID)56888U, "RopHandler.TellVersion: other server version values are invalid.", ex);
			}
			fxContext.OtherSideVersion = new Microsoft.Exchange.Protocols.MAPI.Version(mapiVersion.Value);
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult TransportDeliverMessage(MapiContext context, MapiMessage message, TransportRecipientType transportRecipientType, TransportDeliverMessageResultFactory resultFactory)
		{
			ExchangeId exchangeId;
			ErrorCode errorCode = message.Deliver(context, (RecipientType)transportRecipientType, out exchangeId);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)42344U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult TransportDeliverMessage2(MapiContext context, MapiMessage message, TransportRecipientType transportRecipientType, TransportDeliverMessage2ResultFactory resultFactory)
		{
			ExchangeId exchangeId;
			ErrorCode errorCode = message.Deliver(context, (RecipientType)transportRecipientType, out exchangeId);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)58728U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult(RcaTypeHelpers.ExchangeIdToStoreId(exchangeId));
		}

		protected override RopResult TransportDoneWithMessage(MapiContext context, MapiMessage message, TransportDoneWithMessageResultFactory resultFactory)
		{
			ErrorCode errorCode = message.TransportDoneWithMessage(context);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)34152U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult TransportDuplicateDeliveryCheck(MapiContext context, MapiMessage message, byte flags, ExDateTime submitTime, string internetMessageId, TransportDuplicateDeliveryCheckResultFactory resultFactory)
		{
			ErrorCode errorCode = message.DuplicateDeliveryCheck(context, (DateTime)submitTime, internetMessageId);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)48720U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult TransportNewMail(MapiContext context, MapiLogon logon, ExchangeId folderId, ExchangeId messageId, string messageClass, MessageFlags messageFlags, TransportNewMailResultFactory resultFactory)
		{
			if (messageClass == null)
			{
				throw new ArgumentNullException("messageClass");
			}
			if (!folderId.IsValid)
			{
				DiagnosticContext.TraceLocation((LID)34784U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!messageId.IsValid)
			{
				DiagnosticContext.TraceLocation((LID)59360U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!MessageClassHelper.IsValidMessageClass(messageClass) || messageClass.Length > 255)
			{
				DiagnosticContext.TraceLocation((LID)50536U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!EnumValidator.IsValidValue<MessageFlags>(messageFlags))
			{
				DiagnosticContext.TraceLocation((LID)47464U);
				return resultFactory.CreateFailedResult((ErrorCode)2147942487U);
			}
			if (!logon.SpoolerRights)
			{
				DiagnosticContext.TraceLocation((LID)63848U);
				return resultFactory.CreateFailedResult((ErrorCode)2147746050U);
			}
			MapiFolder.GhostedFolderCheck(context, logon, folderId, (LID)30336U);
			using (TopMessage topMessage = TopMessage.OpenMessage(context, logon.StoreMailbox, folderId, messageId))
			{
				if (topMessage == null)
				{
					DiagnosticContext.TraceLocation((LID)39272U);
					return resultFactory.CreateFailedResult((ErrorCode)2147746063U);
				}
				NewMailNotificationEvent nev = NotificationEvents.CreateNewMailEvent(context, topMessage, messageClass, (MessageFlags)messageFlags);
				context.RiseNotificationEvent(nev);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult TransportSend(MapiContext context, MapiMessage message, TransportSendResultFactory resultFactory)
		{
			byte[] sentRepresentingEntryId;
			bool flag;
			string text;
			message.GetInformationForSetSender(context, out sentRepresentingEntryId, out flag, out text);
			AddressInfo addressInfo = null;
			ErrorCode errorCode = ErrorCode.NoError;
			SubmitMessageRightsCheckFlags submitMessageRightsCheckFlags = SubmitMessageRightsCheckFlags.None;
			if (message.Logon.IsPrimaryOwner && string.Compare(text, message.Logon.Session.AddressInfoUser.LegacyExchangeDN, StringComparison.OrdinalIgnoreCase) == 0)
			{
				DiagnosticContext.TraceLocation((LID)64572U);
				addressInfo = message.Logon.Session.AddressInfoUser;
				if (addressInfo.IsDistributionList)
				{
					submitMessageRightsCheckFlags |= SubmitMessageRightsCheckFlags.SendingAsDL;
				}
				submitMessageRightsCheckFlags |= SubmitMessageRightsCheckFlags.SendAsRights;
			}
			else if (!flag)
			{
				DiagnosticContext.TraceLocation((LID)39996U);
				bool ndrMessage = message.IsNDR(context);
				bool quotaMessage = message.IsQuotaMessage(context);
				context.EndMailboxOperation(true, true);
				try
				{
					addressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfo(context, message.Logon.MapiMailbox.SharedState.TenantHint, text, true);
				}
				catch (UserNotFoundException exception)
				{
					context.OnExceptionCatch(exception);
					DiagnosticContext.TraceDwordAndString((LID)55933U, 0U, text);
					return resultFactory.CreateFailedResult((ErrorCode)1244U);
				}
				RopHandler.CheckSendAsSOBORights(context, message.Logon, addressInfo, ndrMessage, quotaMessage, out submitMessageRightsCheckFlags);
				errorCode = context.StartMailboxOperation();
				if (errorCode != ErrorCode.NoError)
				{
					DiagnosticContext.TraceLocation((LID)55656U);
					return resultFactory.CreateFailedResult((ErrorCode)errorCode);
				}
			}
			message.InternalSetPropsShouldNotFail(context, RopHandler.deleteAfterSubmit);
			errorCode = message.SubmitMessage(context, RopFlags.ReadOnly, sentRepresentingEntryId, addressInfo, submitMessageRightsCheckFlags);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)43368U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			MapiPropBagBase.PropertyReader propsReader = message.GetPropsReader(context, RopHandler.transportProviderSubmitProperties);
			PropertyValue[] array = new PropertyValue[propsReader.PropertyCount + 1];
			int num = 0;
			Property prop;
			while (propsReader.ReadNext(context, out prop))
			{
				array[num] = RcaTypeHelpers.MassageOutgoingProperty(prop, false);
				num++;
			}
			array[num] = RcaTypeHelpers.MassageOutgoingProperty(new Property(PropTag.Message.ProviderSubmitTime, message.StoreMessage.Mailbox.UtcNow), false);
			return resultFactory.CreateSuccessfulResult(array);
		}

		protected override RopResult UnlockRegionStream(MapiContext context, MapiStream stream, ulong offset, ulong regionLength, LockTypeFlag lockType, UnlockRegionStreamResultFactory resultFactory)
		{
			ErrorCode errorCode = stream.UnlockRegion(context, offset, regionLength, lockType != LockTypeFlag.None);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)59752U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult UploadStateStreamBegin(MapiContext context, MapiBase serverObject, PropertyTag propertyTag, uint size, UploadStateStreamBeginResultFactory resultFactory)
		{
			MapiLogon logon = serverObject.Logon;
			StorePropTag propTag = LegacyHelper.ConvertFromLegacyPropTag(propertyTag, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.IcsState, logon.MapiMailbox, true);
			IcsDownloadContext icsDownloadContext = serverObject as IcsDownloadContext;
			ErrorCode errorCode;
			if (icsDownloadContext != null)
			{
				errorCode = icsDownloadContext.BeginUploadStateProperty(propTag, size);
			}
			else
			{
				IcsUploadContext icsUploadContext = serverObject as IcsUploadContext;
				if (icsUploadContext != null)
				{
					errorCode = icsUploadContext.BeginUploadStateProperty(propTag, size);
				}
				else
				{
					errorCode = ErrorCode.CreateNotSupported((LID)48280U);
				}
			}
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)35176U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult UploadStateStreamContinue(MapiContext context, MapiBase serverObject, ArraySegment<byte> data, UploadStateStreamContinueResultFactory resultFactory)
		{
			IcsDownloadContext icsDownloadContext = serverObject as IcsDownloadContext;
			ErrorCode errorCode;
			if (icsDownloadContext != null)
			{
				errorCode = icsDownloadContext.ContinueUploadStateProperty(data);
			}
			else
			{
				IcsUploadContext icsUploadContext = serverObject as IcsUploadContext;
				if (icsUploadContext != null)
				{
					errorCode = icsUploadContext.ContinueUploadStateProperty(data);
				}
				else
				{
					errorCode = ErrorCode.CreateNotSupported((LID)50072U);
				}
			}
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)51560U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult UploadStateStreamEnd(MapiContext context, MapiBase serverObject, UploadStateStreamEndResultFactory resultFactory)
		{
			IcsDownloadContext icsDownloadContext = serverObject as IcsDownloadContext;
			ErrorCode errorCode;
			if (icsDownloadContext != null)
			{
				errorCode = icsDownloadContext.EndUploadStateProperty();
			}
			else
			{
				IcsUploadContext icsUploadContext = serverObject as IcsUploadContext;
				if (icsUploadContext != null)
				{
					errorCode = icsUploadContext.EndUploadStateProperty();
				}
				else
				{
					errorCode = ErrorCode.CreateNotSupported((LID)33688U);
				}
			}
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)45416U);
				return resultFactory.CreateFailedResult((ErrorCode)errorCode);
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult WritePerUserInformation(MapiContext context, MapiLogon logon, StoreLongTermId folderId, bool hasFinished, uint dataOffset, byte[] data, Guid? replicaGuid, WritePerUserInformationResultFactory resultFactory)
		{
			if (dataOffset == 0U)
			{
				if (logon.MapiMailbox.IsPublicFolderMailbox)
				{
					if (replicaGuid == null || replicaGuid.Value != Guid.Empty)
					{
						throw new StoreException((LID)49052U, ErrorCodeValue.InvalidParameter);
					}
					replicaGuid = null;
				}
				else if (replicaGuid == null || replicaGuid.Value == Guid.Empty)
				{
					throw new StoreException((LID)51484U, ErrorCodeValue.InvalidParameter);
				}
			}
			else if (replicaGuid != null)
			{
				throw new StoreException((LID)65436U, ErrorCodeValue.InvalidParameter);
			}
			ErrorCode errorCode = logon.WritePerUserInformation(context, folderId, dataOffset, hasFinished, data, replicaGuid);
			if (errorCode != ErrorCode.NoError)
			{
				DiagnosticContext.TraceLocation((LID)30532U);
				return resultFactory.CreateFailedResult((ErrorCode)((int)errorCode));
			}
			return resultFactory.CreateSuccessfulResult();
		}

		protected override RopResult WriteStream(MapiContext context, MapiStream stream, ArraySegment<byte> data, out ushort outputByteCount, WriteStreamResultFactory resultFactory)
		{
			outputByteCount = 0;
			if (data.Array == null)
			{
				throw new ArgumentNullException("data");
			}
			ushort byteCount = (ushort)stream.Write(context, data.Array, data.Offset, data.Count);
			return resultFactory.CreateSuccessfulResult(byteCount);
		}

		protected override RopResult WriteStreamExtended(MapiContext context, MapiStream stream, ArraySegment<byte>[] dataChunks, out uint outputByteCount, WriteStreamExtendedResultFactory resultFactory)
		{
			outputByteCount = 0U;
			if (dataChunks == null)
			{
				throw new ArgumentNullException("dataChunks");
			}
			uint num = 0U;
			foreach (ArraySegment<byte> arraySegment in dataChunks)
			{
				if (arraySegment.Array == null)
				{
					throw new ArgumentNullException("dataChunk");
				}
				num += (uint)stream.Write(context, arraySegment.Array, arraySegment.Offset, arraySegment.Count);
			}
			return resultFactory.CreateSuccessfulResult(num);
		}

		internal static IDisposable SetConcurrencyTestingApplicationIdHook(string hook)
		{
			return RopHandler.hookableConcurrencyTestingApplicationId.SetTestHook(hook);
		}

		internal static IDisposable SetConcurrencyTestingRopIdHook(RopId hook)
		{
			return RopHandler.hookableConcurrencyTestingRopId.SetTestHook(hook);
		}

		internal static IDisposable SetConcurrencyTestingInvokeWithLockTypeHook(LockManager.LockType hook)
		{
			return RopHandler.hookableConcurrencyTestingInvokeWithLockType.SetTestHook(hook);
		}

		internal static IDisposable SetConcurrencyTestingOnBeforeOperationCompleteHook(Action hook)
		{
			return RopHandler.hookableConcurrencyTestingOnBeforeOperationCompleteHook.SetTestHook(hook);
		}

		private static MailboxInfo GenerateDisconnectedMailboxInfo(AddressInfo addressInfo, MailboxState mailboxState)
		{
			QuotaInfo unlimited = QuotaInfo.Unlimited;
			return new MailboxInfo(mailboxState.DatabaseGuid, mailboxState.MailboxGuid, (mailboxState.UnifiedState != null) ? new Guid?(mailboxState.UnifiedState.UnifiedMailboxGuid) : null, false, false, false, false, false, mailboxState.MailboxType, MailboxInfo.MailboxTypeDetail.None, addressInfo.ObjectId, addressInfo.LegacyExchangeDN, addressInfo.DisplayName, addressInfo.SimpleDisplayName, addressInfo.DistinguishedName, false, false, addressInfo.MasterAccountSid, addressInfo.UserSid, addressInfo.UserSidHistory, addressInfo.OSSecurityDescriptor, 0, 0, new UnlimitedBytes(0L), new UnlimitedBytes(0L), QuotaStyle.UseSpecificValues, unlimited, UnlimitedBytes.UnlimitedValue, UnlimitedBytes.UnlimitedValue, UnlimitedBytes.UnlimitedValue, Guid.Empty)
			{
				IsDisconnected = true
			};
		}

		private static IEnumerable<RecipientRow> GetRecipientRows(MapiContext context, MapiMessage message, uint recipientRowId, ushort codePageId, IList<StorePropTag> extraTags, IList<StorePropTag> extraUnicodeTags)
		{
			MapiPersonCollection personsCollection = message.GetRecipients();
			IList<MapiPerson> recipients = personsCollection;
			if (recipients != null)
			{
				for (int i = 0; i < recipients.Count; i++)
				{
					MapiPerson person = recipients[i];
					if (person.GetRowId() >= (int)recipientRowId && !person.IsDeleted)
					{
						yield return RopHandler.RecipientRowFromMapiPerson(context, person, codePageId, extraTags, extraUnicodeTags);
					}
				}
			}
			yield break;
		}

		private static Properties? PropertiesFromRecipientRow(RecipientRow recipientRow, MapiMessage message)
		{
			int recipientPropListCount = message.GetRecipientPropListCount(0U);
			Properties properties = new Properties(recipientPropListCount);
			properties.Add(PropTag.Recipient.RecipientType, SerializedValue.GetBoxedInt32((int)RopHandler.RecipientTypeToStoreRecipientType(recipientRow.RecipientType)));
			bool flag = (ushort)(recipientRow.Flags & RecipientFlags.SendNoRichInformation) != 0;
			properties.Add(PropTag.Recipient.SendRichInfo, (!flag).GetBoxed());
			properties.Add(PropTag.Recipient.InstanceKey, BitConverter.GetBytes(recipientRow.RecipientRowId));
			if (recipientRow.DisplayName != null)
			{
				properties.Add(PropTag.Recipient.DisplayName, recipientRow.DisplayName);
				if ((ushort)(recipientRow.Flags & RecipientFlags.TransmitSameAsDisplayName) != 0)
				{
					properties.Add(PropTag.Recipient.TransmitableDisplayName, recipientRow.DisplayName);
				}
			}
			if (recipientRow.SimpleDisplayName != null)
			{
				properties.Add(PropTag.Recipient.SimpleDisplayName, recipientRow.SimpleDisplayName);
			}
			if (recipientRow.TransmittableDisplayName != null)
			{
				properties.Add(PropTag.Recipient.TransmitableDisplayName, recipientRow.TransmittableDisplayName);
			}
			RecipientAddress.ExchangeRecipientAddress exchangeRecipientAddress = recipientRow.RecipientAddress as RecipientAddress.ExchangeRecipientAddress;
			RecipientAddress.DistributionListRecipientAddress distributionListRecipientAddress = recipientRow.RecipientAddress as RecipientAddress.DistributionListRecipientAddress;
			RecipientAddress.OtherRecipientAddress otherRecipientAddress = recipientRow.RecipientAddress as RecipientAddress.OtherRecipientAddress;
			RecipientAddress.EmptyRecipientAddress emptyRecipientAddress = recipientRow.RecipientAddress as RecipientAddress.EmptyRecipientAddress;
			string text = null;
			string text2 = null;
			if (exchangeRecipientAddress != null)
			{
				text = "EX";
				RecipientDisplayType recipientDisplayType = exchangeRecipientAddress.RecipientDisplayType;
				switch (recipientDisplayType)
				{
				case RecipientDisplayType.DistributionList:
				case RecipientDisplayType.Agent:
					break;
				case RecipientDisplayType.Forum:
					goto IL_159;
				default:
					if (recipientDisplayType != RecipientDisplayType.RemoteMailUser)
					{
						goto IL_159;
					}
					break;
				}
				properties.Add(PropTag.Recipient.DisplayType, SerializedValue.GetBoxedInt32((int)exchangeRecipientAddress.RecipientDisplayType));
				goto IL_16B;
				IL_159:
				properties.Add(PropTag.Recipient.DisplayType, SerializedValue.GetBoxedInt32(0));
				IL_16B:
				if (!exchangeRecipientAddress.TryGetFullAddress(string.Empty, out text2))
				{
					if (exchangeRecipientAddress.AddressPrefixLengthUsed > 0)
					{
						throw new ExExceptionInvalidRecipients((LID)63032U, "Invalid recipient address prefix.");
					}
					text2 = null;
				}
			}
			else if (distributionListRecipientAddress != null)
			{
				text = ((distributionListRecipientAddress.RecipientAddressType == RecipientAddressType.MapiPrivateDistributionList) ? "MAPIPDL" : "DOSPDL");
				properties.Add(PropTag.Recipient.EntryId, distributionListRecipientAddress.EntryId);
				properties.Add(PropTag.Recipient.SearchKey, distributionListRecipientAddress.SearchKey);
			}
			else if (otherRecipientAddress != null)
			{
				if (!string.IsNullOrEmpty(otherRecipientAddress.AddressType) && !string.IsNullOrEmpty(recipientRow.EmailAddress))
				{
					text = otherRecipientAddress.AddressType;
					text2 = recipientRow.EmailAddress;
				}
			}
			else if (emptyRecipientAddress != null)
			{
				switch (emptyRecipientAddress.RecipientAddressType)
				{
				case RecipientAddressType.None:
					goto IL_285;
				case RecipientAddressType.MicrosoftMail:
					text = "MS";
					goto IL_285;
				case RecipientAddressType.Smtp:
					text = "SMTP";
					goto IL_285;
				case RecipientAddressType.Fax:
					text = "FAX";
					goto IL_285;
				case RecipientAddressType.ProfessionalOfficeSystem:
					text = "PROFS";
					goto IL_285;
				}
				return null;
				IL_285:
				if (emptyRecipientAddress.RecipientAddressType == RecipientAddressType.None)
				{
					if (recipientRow.EmailAddress != null)
					{
						text2 = recipientRow.EmailAddress;
					}
				}
				else if (!string.IsNullOrEmpty(recipientRow.EmailAddress))
				{
					text2 = recipientRow.EmailAddress;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				properties.Add(PropTag.Recipient.AddressType, text);
			}
			if (text2 != null)
			{
				properties.Add(PropTag.Recipient.EmailAddress, text2);
			}
			bool flag2 = 0 != (ushort)(recipientRow.Flags & RecipientFlags.Responsibility);
			if (!flag2)
			{
				flag2 = message.Logon.CanHandleAddressType(text);
			}
			properties.Add(PropTag.Recipient.Responsibility, flag2.GetBoxed());
			RopHandler.AddProperties(recipientRow.ExtraPropertyValues, MapiObjectType.Person, message.Logon.MapiMailbox, properties, RopHandler.recipientExtraExcludeProperties);
			RopHandler.AddProperties(recipientRow.ExtraUnicodePropertyValues, MapiObjectType.Person, message.Logon.MapiMailbox, properties, RopHandler.recipientExtraExcludeProperties);
			return new Properties?(properties);
		}

		private static void AddProperties(PropertyValue[] propertyValues, MapiObjectType mapiObjectType, MapiMailbox mapiMailbox, Properties properties, HashSet<PropertyTag> excludeProps)
		{
			Microsoft.Exchange.Server.Storage.PropTags.ObjectType propTagObjectType = Helper.GetPropTagObjectType(mapiObjectType);
			for (int i = 0; i < propertyValues.Length; i++)
			{
				if (!propertyValues[i].IsError && (excludeProps == null || !excludeProps.Contains(propertyValues[i].PropertyTag)))
				{
					StorePropTag tag = LegacyHelper.ConvertFromLegacyPropTag(propertyValues[i].PropertyTag, propTagObjectType, mapiMailbox, true);
					object value = propertyValues[i].Value;
					RcaTypeHelpers.MassageIncomingPropertyValue(propertyValues[i].PropertyTag, ref value);
					properties.Add(tag, value);
				}
			}
		}

		private static RecipientRow RecipientRowFromMapiPerson(MapiContext context, MapiPerson person, ushort codePageId, IList<StorePropTag> extraTags, IList<StorePropTag> extraUnicodeTags)
		{
			uint rowId = (uint)person.GetRowId();
			RecipientType recipientType = RopHandler.RecipientTypeFromStoreRecipientType((RecipientType)person.GetRecipientType());
			RecipientFlags recipientFlags = RecipientFlags.None;
			RecipientAddress recipientAddress = RopHandler.GetRecipientAddress(person);
			string emailAddress = null;
			string displayName = person.GetDisplayName();
			string simpleDisplayName = person.GetSimpleDisplayName();
			string text = person.GetTransmitableDisplayName();
			if (!(recipientAddress is RecipientAddress.ExchangeRecipientAddress))
			{
				emailAddress = person.GetEmailAddress();
			}
			if (person.GetResponsibility())
			{
				recipientFlags |= RecipientFlags.Responsibility;
			}
			if (!person.GetSendRichInfo())
			{
				recipientFlags |= RecipientFlags.SendNoRichInformation;
			}
			if (displayName == text)
			{
				recipientFlags |= RecipientFlags.TransmitSameAsDisplayName;
				text = null;
			}
			PropertyValue[] array = Array<PropertyValue>.Empty;
			PropertyValue[] array2 = Array<PropertyValue>.Empty;
			if (extraTags != null && 0 < extraTags.Count)
			{
				MapiPropBagBase.PropertyReader propsReader = person.GetPropsReader(context, extraTags);
				array = new PropertyValue[propsReader.PropertyCount];
				int num = 0;
				Property prop;
				while (propsReader.ReadNext(context, out prop))
				{
					array[num] = RcaTypeHelpers.MassageOutgoingProperty(prop, false);
					num++;
				}
			}
			if (extraUnicodeTags != null && 0 < extraUnicodeTags.Count)
			{
				MapiPropBagBase.PropertyReader propsReader2 = person.GetPropsReader(context, extraUnicodeTags);
				array2 = new PropertyValue[propsReader2.PropertyCount];
				int num2 = 0;
				Property prop2;
				while (propsReader2.ReadNext(context, out prop2))
				{
					array2[num2] = RcaTypeHelpers.MassageOutgoingProperty(prop2, false);
					num2++;
				}
			}
			return new RecipientRow(rowId, recipientType, new ushort?(codePageId), recipientAddress, recipientFlags, true, emailAddress, displayName, simpleDisplayName, text, array, array2);
		}

		private static RecipientAddress GetRecipientAddress(MapiPerson person)
		{
			RecipientAddress result = null;
			string addrType = person.GetAddrType();
			RecipientAddressType recipientAddressType = RopHandler.GetRecipientAddressType(addrType);
			RecipientAddressType recipientAddressType2 = recipientAddressType;
			switch (recipientAddressType2)
			{
			case RecipientAddressType.None:
			case RecipientAddressType.MicrosoftMail:
			case RecipientAddressType.Smtp:
			case RecipientAddressType.Fax:
			case RecipientAddressType.ProfessionalOfficeSystem:
				result = new RecipientAddress.EmptyRecipientAddress(recipientAddressType);
				break;
			case RecipientAddressType.Exchange:
			{
				RecipientDisplayType recipientDisplayType = (RecipientDisplayType)person.GetDisplayType();
				string empty = string.Empty;
				string text = person.GetEmailAddress();
				if (text == null)
				{
					text = string.Empty;
				}
				result = new RecipientAddress.ExchangeRecipientAddress(recipientAddressType, recipientDisplayType, empty, text);
				break;
			}
			case RecipientAddressType.MapiPrivateDistributionList:
			case RecipientAddressType.DosPrivateDistributionList:
			{
				byte[] entryId = person.GetEntryId();
				byte[] searchKey = person.GetSearchKey();
				if (entryId == null)
				{
					throw new InvalidRecipientsException((LID)64056U, "Recipient entry id is null.");
				}
				if (searchKey == null)
				{
					throw new InvalidRecipientsException((LID)54840U, "Recipient search key is null.");
				}
				result = new RecipientAddress.DistributionListRecipientAddress(recipientAddressType, entryId, searchKey);
				break;
			}
			default:
				if (recipientAddressType2 == RecipientAddressType.Other)
				{
					result = new RecipientAddress.OtherRecipientAddress(recipientAddressType, addrType);
				}
				break;
			}
			return result;
		}

		private static RecipientAddressType GetRecipientAddressType(string addressType)
		{
			switch (addressType)
			{
			case "MAPIPDL":
				return RecipientAddressType.MapiPrivateDistributionList;
			case "DOSPDL":
				return RecipientAddressType.DosPrivateDistributionList;
			case "EX":
				return RecipientAddressType.Exchange;
			case "MS":
				return RecipientAddressType.MicrosoftMail;
			case "SMTP":
				return RecipientAddressType.Smtp;
			case "FAX":
				return RecipientAddressType.Fax;
			case "PROFS":
				return RecipientAddressType.ProfessionalOfficeSystem;
			case null:
				break;
			default:
				return RecipientAddressType.Other;
				break;
			}
			return RecipientAddressType.None;
		}

		private static RecipientType RecipientTypeToStoreRecipientType(RecipientType recipientType)
		{
			switch (recipientType)
			{
			case RecipientType.To:
				return RecipientType.To;
			case RecipientType.Cc:
				return RecipientType.Cc;
			case RecipientType.Bcc:
				return RecipientType.Bcc;
			default:
				if (recipientType == RecipientType.P1)
				{
					return RecipientType.P1;
				}
				switch (recipientType)
				{
				case RecipientType.SubmittedTo:
					return RecipientType.SubmittedTo;
				case RecipientType.SubmittedCc:
					return RecipientType.SubmittedCc;
				case RecipientType.SubmittedBcc:
					return RecipientType.SubmittedBcc;
				default:
					throw new ExExceptionInvalidRecipients((LID)42552U, string.Format("Invalid recipient type: {0}.", recipientType));
				}
				break;
			}
		}

		private static RecipientType RecipientTypeFromStoreRecipientType(RecipientType recipientType)
		{
			switch (recipientType)
			{
			case RecipientType.SubmittedTo:
				return RecipientType.SubmittedTo;
			case RecipientType.SubmittedCc:
				return RecipientType.SubmittedCc;
			case RecipientType.SubmittedBcc:
				return RecipientType.SubmittedBcc;
			default:
				switch (recipientType)
				{
				case RecipientType.Orig:
					return (RecipientType)0;
				case RecipientType.To:
					return RecipientType.To;
				case RecipientType.Cc:
					return RecipientType.Cc;
				case RecipientType.Bcc:
					return RecipientType.Bcc;
				default:
					if (recipientType != RecipientType.P1)
					{
						throw new ExExceptionInvalidRecipients((LID)58936U, string.Format("Invalid recipient type: {0}.", recipientType));
					}
					return RecipientType.P1;
				}
				break;
			}
		}

		private static RecipientCollector CreateMessageHeader(MapiContext context, MapiMessage message, MessageHeaderResultFactory resultFactory, CodePage codePageProperties, PropertyTag[] extraUnicodePropertyTags)
		{
			bool flag = false;
			RecipientCollector recipientCollector = null;
			bool? flag2 = (bool?)message.GetOnePropValue(context, PropTag.Message.HasNamedProperties);
			string subjectPrefix = message.GetOnePropValue(context, PropTag.Message.SubjectPrefix) as string;
			string normalizedSubject = message.GetOnePropValue(context, PropTag.Message.NormalizedSubject) as string;
			MessageHeader messageHeader = new MessageHeader(flag2 != null && flag2.Value, true, subjectPrefix, normalizedSubject, (ushort)message.GetRecipients().GetAliveCount());
			IList<StorePropTag> recipientPropListExtra = message.GetRecipientPropListExtra();
			PropertyTag[] extraPropertyTags;
			LegacyHelper.ConvertToLegacyPropTags(recipientPropListExtra, out extraPropertyTags);
			IList<StorePropTag> extraUnicodeTags = null;
			if (extraUnicodePropertyTags != null)
			{
				extraUnicodeTags = LegacyHelper.ConvertFromLegacyPropTags(extraUnicodePropertyTags, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Recipient, message.Logon.MapiMailbox, true);
			}
			RecipientCollector result;
			try
			{
				recipientCollector = resultFactory.CreateRecipientCollector(messageHeader, extraPropertyTags, CTSGlobals.AsciiEncoding);
				foreach (RecipientRow row in RopHandler.GetRecipientRows(context, message, 0U, (ushort)codePageProperties, recipientPropListExtra, extraUnicodeTags))
				{
					if (!recipientCollector.TryAddRecipientRow(row))
					{
						break;
					}
				}
				flag = true;
				result = recipientCollector;
			}
			finally
			{
				if (recipientCollector != null && !flag)
				{
					recipientCollector.Dispose();
					recipientCollector = null;
				}
			}
			return result;
		}

		private static bool FinalizeSessionConfigurationIfNeeded(Context context, TenantHint tenantHint, MapiSession session, AuthenticationContext authenticationContext, bool claimAdminPrivilegeOnTargetMailboxDatabase, Guid targetMailboxDatabaseGuid)
		{
			session.UsingLogonAdminPrivilege = false;
			if (authenticationContext != null)
			{
				DiagnosticContext.TraceLocation((LID)47536U);
				RopHandler.TrySetAuthenticationContext(context, tenantHint, session, authenticationContext, claimAdminPrivilegeOnTargetMailboxDatabase, targetMailboxDatabaseGuid);
				session.CanAcceptROPs = true;
				return true;
			}
			if (session.UsingDelegatedAuth)
			{
				if (!session.CanAcceptROPs)
				{
					throw new ExExceptionLogonFailed((LID)38911U, "Delegate session without delegate auth context");
				}
				DiagnosticContext.TraceLocation((LID)39344U);
				return false;
			}
			else
			{
				if (session.AddressInfoUser == null && !string.IsNullOrEmpty(session.UserDN))
				{
					DiagnosticContext.TraceLocation((LID)50608U);
					AddressInfo addressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfo(context, tenantHint, session.UserDN, false);
					session.SetAddressInfo(addressInfo);
					return true;
				}
				return false;
			}
		}

		internal static void CheckSendAsSOBORights(MapiContext context, MapiLogon logon, AddressInfo addressInfoForAuthorization, bool ndrMessage, bool quotaMessage, out SubmitMessageRightsCheckFlags submitMessageRightsCheckFlags)
		{
			Microsoft.Exchange.Diagnostics.Trace submitMessageTracer = Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi.ExTraceGlobals.SubmitMessageTracer;
			submitMessageRightsCheckFlags = SubmitMessageRightsCheckFlags.SendingAsDL;
			if (logon.SystemRights)
			{
				if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					submitMessageTracer.TraceDebug(0L, "System rights or System submit; skipping SendAs access check");
				}
				submitMessageRightsCheckFlags |= SubmitMessageRightsCheckFlags.SendAsRights;
				return;
			}
			if (logon.Session.CurrentSecurityContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid))
			{
				if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					submitMessageTracer.TraceDebug(0L, "Current session owner is LocalSystem; granting SendAs rights");
				}
				submitMessageRightsCheckFlags |= SubmitMessageRightsCheckFlags.SendAsRights;
				return;
			}
			bool flag = SecurityHelper.CheckSendAsRights(logon.Session.CurrentSecurityContext, addressInfoForAuthorization);
			if (flag)
			{
				submitMessageRightsCheckFlags |= SubmitMessageRightsCheckFlags.SendAsRights;
				if (addressInfoForAuthorization.IsDistributionList)
				{
					submitMessageRightsCheckFlags |= SubmitMessageRightsCheckFlags.SendingAsDL;
					return;
				}
				submitMessageRightsCheckFlags &= ~SubmitMessageRightsCheckFlags.SendingAsDL;
				return;
			}
			else
			{
				if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					submitMessageTracer.TraceDebug(0L, "SendAs access check has failed; fall through for SOBO checks");
					DiagnosticContext.TraceLocation((LID)52807U);
				}
				bool flag2 = RopHandler.IsValidDelegate(context, logon.MapiMailbox.SharedState.TenantHint, logon.LoggedOnUserAddressInfo, addressInfoForAuthorization);
				if (flag2)
				{
					submitMessageRightsCheckFlags |= SubmitMessageRightsCheckFlags.SOBORights;
					return;
				}
				if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					submitMessageTracer.TraceDebug(0L, "SOBO check has failed");
					DiagnosticContext.TraceLocation((LID)46663U);
				}
				return;
			}
		}

		private static bool IsValidDelegate(MapiContext context, TenantHint tenantHint, AddressInfo addressInfo, AddressInfo addressInfoOther)
		{
			Microsoft.Exchange.Diagnostics.Trace submitMessageTracer = Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi.ExTraceGlobals.SubmitMessageTracer;
			if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				submitMessageTracer.TraceDebug<string>(0L, "Checking public delegates for [{0}]", addressInfo.DistinguishedName);
			}
			if (addressInfoOther.PublicDelegates.Count == 0)
			{
				if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					submitMessageTracer.TraceDebug(0L, "No public delegates found. returning false.");
				}
				return false;
			}
			foreach (AddressInfo.PublicDelegate publicDelegate in addressInfoOther.PublicDelegates)
			{
				if (!publicDelegate.IsDistributionList)
				{
					if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						submitMessageTracer.TraceDebug<string>(0L, "Checking match for public delegate [{0}]", publicDelegate.DistinguishedName);
					}
					if (string.Equals(publicDelegate.DistinguishedName, addressInfo.DistinguishedName, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				submitMessageTracer.TraceDebug<string>(0L, "Checking public delegate DLs for [{0}]", addressInfo.DistinguishedName);
			}
			foreach (AddressInfo.PublicDelegate publicDelegate2 in addressInfoOther.PublicDelegates)
			{
				if (publicDelegate2.IsDistributionList)
				{
					if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						submitMessageTracer.TraceDebug<string>(0L, "Checking IsMemberOf for public delegate [{0}]", publicDelegate2.DistinguishedName);
					}
					if (Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.IsMemberOfDistributionList(context, tenantHint, addressInfo, publicDelegate2.ObjectId))
					{
						return true;
					}
				}
			}
			if (submitMessageTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				submitMessageTracer.TraceDebug(0L, "No match found. returning false.");
			}
			return false;
		}

		private static void TrySetAuthenticationContext(Context context, TenantHint tenantHint, MapiSession session, AuthenticationContext authenticationContext, bool claimAdminPrivilegeOnTargetMailboxDatabase, Guid targetMailboxDatabaseGuid)
		{
			if (string.IsNullOrEmpty(authenticationContext.ConnectAs))
			{
				throw new ExExceptionLogonFailed((LID)34360U, "authenticationContext.ConnectAs is NULL or empty");
			}
			AddressInfo addressInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetAddressInfo(context, tenantHint, authenticationContext.ConnectAs, false);
			ClientSecurityContext clientSecurityContext = null;
			try
			{
				clientSecurityContext = ClientSecurityContextFactory.Create(context, authenticationContext);
				if (clientSecurityContext == null)
				{
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.TraceError(0L, "Failed to build ClientSecurityContext from AuthenticationContext.");
					}
					throw new ExExceptionAccessDenied((LID)43752U, "Failed to build ClientSecurityContext from AuthenticationContext.");
				}
				Func<MailboxInfo> mailboxInfoGetter;
				Func<MailboxInfo, DatabaseInfo> databaseInfoGetter;
				if (claimAdminPrivilegeOnTargetMailboxDatabase)
				{
					mailboxInfoGetter = (() => null);
					databaseInfoGetter = ((MailboxInfo mailboxInfo) => Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(context, targetMailboxDatabaseGuid));
				}
				else if (addressInfo.HasMailbox)
				{
					mailboxInfoGetter = (() => Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetMailboxInfo(context, tenantHint, authenticationContext.ConnectAs));
					databaseInfoGetter = delegate(MailboxInfo mailboxInfo)
					{
						if (mailboxInfo.IsSystemAttendantRecipient)
						{
							return null;
						}
						return Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetDatabaseInfo(context, mailboxInfo.MdbGuid);
					};
				}
				else
				{
					mailboxInfoGetter = (() => null);
					databaseInfoGetter = ((MailboxInfo mailboxInfo) => null);
				}
				if (!MapiSession.CheckCreateSessionRightsOnLogon(addressInfo, mailboxInfoGetter, databaseInfoGetter, Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetServerInfo(context).NTSecurityDescriptor, clientSecurityContext, claimAdminPrivilegeOnTargetMailboxDatabase))
				{
					if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.TraceError(0L, "AuthenticationContext has no rights on this session.");
					}
					throw new ExExceptionLogonFailed((LID)51944U, "AuthenticationContext has no rights on this session.");
				}
				session.SetDelegatedAuthInfo(addressInfo, ref clientSecurityContext);
				session.UsingLogonAdminPrivilege = claimAdminPrivilegeOnTargetMailboxDatabase;
				clientSecurityContext = null;
			}
			catch (Win32Exception ex)
			{
				context.OnExceptionCatch(ex);
				DiagnosticContext.TraceLocation((LID)60136U);
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.TraceError<Win32Exception>(0L, "Caught exception {0} while attempting to add SIDs to AuthzContext", ex);
				}
			}
			catch (AuthzException ex2)
			{
				context.OnExceptionCatch(ex2);
				DiagnosticContext.TraceLocation((LID)35560U);
				if (Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.TraceError<AuthzException>(0L, "Caught exception {0} while trying to use a serialized context", ex2);
				}
			}
			finally
			{
				if (clientSecurityContext != null)
				{
					clientSecurityContext.Dispose();
				}
			}
		}

		private static void ReconnectMailboxToDirectoryObject(MapiContext context, MapiMailbox mapiMailbox, MailboxInfo mailboxInfo)
		{
			MailboxCleanup.ReconnectMailboxToADObject(context, mapiMailbox.StoreMailbox, mailboxInfo);
		}

		private void ValidateLogonParameters(MapiContext context, LogonFlags logonFlags, OpenFlags openFlags, LogonExtendedRequestFlags extendedFlags, MailboxId? mailboxId, string applicationId, AuthenticationContext authenticationContext)
		{
			OpenFlags openFlags2 = openFlags & (OpenFlags.DeliverNormalMessage | OpenFlags.DeliverSpecialMessage | OpenFlags.DeliverQuotaMessage);
			if ((openFlags2 & openFlags2 - 1U) != OpenFlags.None)
			{
				throw new StoreException((LID)29776U, ErrorCodeValue.InvalidParameter);
			}
			if (openFlags2 != OpenFlags.None && !this.Session.UsingTransportPrivilege)
			{
				throw new StoreException((LID)29800U, ErrorCodeValue.InvalidParameter);
			}
			if ((this.Session.ApplicationId != null && this.Session.ApplicationId.EndsWith("E-Discovery")) || (applicationId != null && applicationId.EndsWith("E-Discovery")))
			{
				throw new ExExceptionLogonFailed((LID)51256U, "E-Discovery is not welcome because it kills us!");
			}
			if (context.ClientType == ClientType.Migration && !MapiDispHelper.IsSupportedMrsVersion(this.Session.ClientVersion.Value))
			{
				throw new ExExceptionAccessDenied((LID)45112U, "Not supported MRS version.");
			}
			if ((byte)(logonFlags & LogonFlags.Private) != 1)
			{
				throw new ExExceptionLogonFailed((LID)55295U, "Only private logons are supported");
			}
			if ((openFlags & OpenFlags.CliWithPerMdbFix) != OpenFlags.CliWithPerMdbFix)
			{
				throw new ExExceptionLogonFailed((LID)43007U, "Client must support per-mdb mapping");
			}
			if ((byte)(logonFlags & LogonFlags.MbxGuids) == 0)
			{
				throw new ExExceptionNoSupport((LID)60392U, "logonFlags doesn't indicate LogonFlags.MbxGuids");
			}
			if ((openFlags & OpenFlags.MailboxGuid) == OpenFlags.None)
			{
				throw new ExExceptionNoSupport((LID)35816U, "openFlags doesn't indicate OpenFlags.MailboxGuid");
			}
			if (mailboxId == null)
			{
				throw new ExExceptionLogonFailed((LID)62728U, "Client must supply the ID of the mailbox to logon to.");
			}
			MailboxId value = mailboxId.Value;
			if (value.IsLegacyDn)
			{
				throw new ExExceptionNoSupport((LID)52200U, "mailboxId contains LegacyDN");
			}
			if (value.MailboxGuid == Guid.Empty)
			{
				throw new ExExceptionInvalidParameter((LID)51168U, "mailboxId.MailboxGuid is an empty Guid");
			}
			if (value.DatabaseGuid == Guid.Empty)
			{
				throw new ExExceptionInvalidParameter((LID)48096U, "mailboxId.DatabaseGuid is an empty Guid");
			}
			if (authenticationContext != null)
			{
				if (!this.Session.UsingDelegatedAuth)
				{
					throw new ExExceptionLogonFailed((LID)53247U, "Session is not configured to use delegated auth");
				}
				if (this.Session.LogonCount > 0)
				{
					throw new ExExceptionLogonFailed((LID)47103U, "Delegated authentication is allowed only on first logon.");
				}
			}
		}

		private void InvokeOnBeforeOperationCompleteCallbackIfNecessary(RopId ropId)
		{
			if (this.Session.ApplicationId == RopHandler.hookableConcurrencyTestingApplicationId.Value && ropId == RopHandler.hookableConcurrencyTestingRopId.Value && RopHandler.hookableConcurrencyTestingOnBeforeOperationCompleteHook.Value != null && LockManager.IsLockHeld(RopHandler.hookableConcurrencyTestingInvokeWithLockType.Value))
			{
				RopHandler.hookableConcurrencyTestingOnBeforeOperationCompleteHook.Value();
			}
		}

		private bool SkipColumnPropertiesPromotionValidation(MapiContext context)
		{
			MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)context.Diagnostics;
			return !string.IsNullOrEmpty(mapiExecutionDiagnostics.ClientActionString) && RopHandler.actionsNoPropertiesPromotionValidation.Contains(mapiExecutionDiagnostics.ClientActionString);
		}

		private ErrorCode ChunkedPrepareIfNecessary(MapiContext context, IChunked prepare)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			if (prepare != null)
			{
				try
				{
					bool flag = false;
					using (context.DisableNotificationPumping())
					{
						while (!prepare.DoChunk(context))
						{
							flag = true;
							Action actionOutsideMailboxLock;
							if (!prepare.MustYield)
							{
								actionOutsideMailboxLock = null;
							}
							else
							{
								actionOutsideMailboxLock = delegate()
								{
									Thread.Sleep(15);
								};
							}
							errorCode = context.PulseMailboxOperation(actionOutsideMailboxLock);
							if (errorCode != ErrorCode.NoError)
							{
								break;
							}
						}
					}
					if (flag && errorCode == ErrorCode.NoError)
					{
						errorCode = context.PulseMailboxOperation(null);
					}
				}
				finally
				{
					prepare.Dispose(context);
				}
			}
			return errorCode;
		}

		private void CheckAndRunISIntegScheduled(MapiContext context, Mailbox storeMailbox, Guid mailboxGuid, DateTime valueCurrentTime, DateTime deterministicNow)
		{
			if (!ConfigurationSchema.ScheduledISIntegEnabled.Value)
			{
				return;
			}
			DateTime? isintegScheduledLast = storeMailbox.GetISIntegScheduledLast(context);
			if (isintegScheduledLast == null && valueCurrentTime != DateTime.MinValue)
			{
				Random random = new Random();
				int num = random.Next(ConfigurationSchema.ScheduledISIntegExecutePeriod.Value.Days);
				storeMailbox.SetISIntegScheduledLast(context, deterministicNow - TimeSpan.FromDays((double)num), null, null);
				return;
			}
			if (isintegScheduledLast != null)
			{
				DateTime t = isintegScheduledLast.Value + ConfigurationSchema.ScheduledISIntegExecutePeriod.Value;
				if (valueCurrentTime < t)
				{
					return;
				}
			}
			InMemoryJobStorage inMemoryJobStorage = InMemoryJobStorage.Instance(context.Database);
			IEnumerable<IntegrityCheckJob> jobsByMailboxGuid = inMemoryJobStorage.GetJobsByMailboxGuid(mailboxGuid);
			if (jobsByMailboxGuid != null)
			{
				foreach (IntegrityCheckJob integrityCheckJob in jobsByMailboxGuid)
				{
					if (integrityCheckJob.TaskId == TaskId.ScheduledCheck && integrityCheckJob.Source == JobSource.Maintenance && integrityCheckJob.State == JobState.Pending)
					{
						return;
					}
				}
			}
			Properties[] array = null;
			Guid a = JobBuilder.BuildAndSchedule(context, mailboxGuid, IntegrityCheckRequestFlags.Maintenance, new TaskId[]
			{
				TaskId.ScheduledCheck
			}, null, ref array);
			if (a == Guid.Empty && Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp.ExTraceGlobals.RpcDetailTracer.TraceDebug(0L, "ISIntegScheduled was not scheduled. Job queue might be full.");
			}
		}

		private const int MaximumMessageClassLength = 255;

		private const OpenMode SupportedOpenAttachmentFlags = OpenMode.BestAccess;

		private const OpenMode SupportedOpenEmbeddedMessageFlags = OpenMode.BestAccess;

		private const SaveChangesMode SupportedSaveChangesAttachmentFlags = ~(SaveChangesMode.TransportDelivery | SaveChangesMode.IMAPChange | SaveChangesMode.ForceNotificationPublish);

		private const SaveChangesMode SupportedSaveChangesMessageFlags = SaveChangesMode.KeepOpenReadOnly | SaveChangesMode.KeepOpenReadWrite | SaveChangesMode.ForceSave | SaveChangesMode.DelayedCall | SaveChangesMode.SkipQuotaCheck | SaveChangesMode.TransportDelivery | SaveChangesMode.IMAPChange | SaveChangesMode.ForceNotificationPublish;

		private const SetReadFlagFlags SupportedSetReadFlagFlags = SetReadFlagFlags.SuppressReceipt | SetReadFlagFlags.FolderMessageDialog | SetReadFlagFlags.ClearReadFlag | SetReadFlagFlags.DeferredErrors | SetReadFlagFlags.GenerateReceiptOnly | SetReadFlagFlags.ClearReadNotificationPending | SetReadFlagFlags.ClearNonReadNotificationPending;

		private const CopyPropertiesFlags SupportedCopyPropertiesFlags = CopyPropertiesFlags.Move | CopyPropertiesFlags.NoReplace;

		private const CopyPropertiesFlags SupportedCopyToFlags = CopyPropertiesFlags.Move | CopyPropertiesFlags.NoReplace;

		private const EmptyFolderFlags SupportedEmptyFolderFlags = EmptyFolderFlags.Associated | EmptyFolderFlags.Force;

		private const DeleteFolderFlags SupportedDeleteFolderFlags = DeleteFolderFlags.DeleteMessages | DeleteFolderFlags.DeleteFolders | DeleteFolderFlags.HardDelete;

		private const EmptyFolderFlags SupportedHardEmptyFolderFlags = EmptyFolderFlags.Associated | EmptyFolderFlags.Force;

		private const SubmitMessageFlags SupportedSubmitMessageFlags = SubmitMessageFlags.Preprocess | SubmitMessageFlags.NeedsSpooler | SubmitMessageFlags.IgnoreSendAsRight;

		private const GetPropertiesFlags SupportedGetPropertiesSpecificFlags = GetPropertiesFlags.Unicode;

		private const GetPropertiesFlags SupportedGetPropertiesAllFlags = GetPropertiesFlags.Unicode;

		private const FastTransferCopyPropertiesFlag SupportedFastTransferDestinationCopyFlags = FastTransferCopyPropertiesFlag.Move | FastTransferCopyPropertiesFlag.FastTrasferStream | FastTransferCopyPropertiesFlag.CopyMailboxPerUserData | FastTransferCopyPropertiesFlag.CopyFolderPerUserData;

		private const FastTransferCopyFolderFlag SupportedFastTransferSourceCopyFolderFlags = FastTransferCopyFolderFlag.Move | FastTransferCopyFolderFlag.CopySubFolders;

		private const FastTransferCopyMessagesFlag SupportedFastTransferSourceCopyMessagesFlags = FastTransferCopyMessagesFlag.Move | FastTransferCopyMessagesFlag.BestBody | FastTransferCopyMessagesFlag.SendEntryId;

		private const FastTransferCopyPropertiesFlag SupportedFastTransferSourcePropertiesFlag = FastTransferCopyPropertiesFlag.Move;

		private const FastTransferCopyFlag SupportedFastTransferSourceCopyToFlag = FastTransferCopyFlag.CopyMailboxPerUserData | FastTransferCopyFlag.CopyFolderPerUserData | FastTransferCopyFlag.MoveUser | FastTransferCopyFlag.ForceUnicode | FastTransferCopyFlag.FastTrasferStream | FastTransferCopyFlag.BestBody | FastTransferCopyFlag.Unicode;

		private const FastTransferSendOption SupportedFastTransferSendOptionsForContentsSync = FastTransferSendOption.Unicode | FastTransferSendOption.RecoverMode | FastTransferSendOption.ForceUnicode | FastTransferSendOption.PartialItem | FastTransferSendOption.SendPropErrors;

		private const FastTransferSendOption SupportedFastTransferSendOptionsForHierarchySync = FastTransferSendOption.Unicode | FastTransferSendOption.RecoverMode | FastTransferSendOption.ForceUnicode | FastTransferSendOption.SendPropErrors;

		private const SyncFlag SupportedSyncFlagsForContentsSync = SyncFlag.Unicode | SyncFlag.NoDeletions | SyncFlag.NoSoftDeletions | SyncFlag.ReadState | SyncFlag.Associated | SyncFlag.Normal | SyncFlag.NoConflicts | SyncFlag.OnlySpecifiedProps | SyncFlag.NoForeignKeys | SyncFlag.LimitedIMessage | SyncFlag.CatchUp | SyncFlag.Conversations | SyncFlag.MessageSelective | SyncFlag.BestBody | SyncFlag.IgnoreSpecifiedOnAssociated | SyncFlag.ProgressMode;

		private const SyncFlag SupportedSyncFlagsForHierarchySync = SyncFlag.Unicode | SyncFlag.NoDeletions | SyncFlag.NoConflicts | SyncFlag.OnlySpecifiedProps | SyncFlag.NoForeignKeys | SyncFlag.CatchUp;

		private const SyncExtraFlag UnsupportedSyncExtraFlagsForHierarchySync = SyncExtraFlag.MessageSize | SyncExtraFlag.OrderByDeliveryTime | SyncExtraFlag.NoChanges;

		private const QueryRowsFlags SupportedQueryRowsFlags = QueryRowsFlags.DoNotAdvance | QueryRowsFlags.SendMax | QueryRowsFlags.ChainAlways;

		private const SortTableFlags SupportedSortTableFlags = SortTableFlags.NoWait;

		private const NotificationFlags StandardEventTypesMask = NotificationFlags.CriticalError | NotificationFlags.NewMail | NotificationFlags.ObjectCreated | NotificationFlags.ObjectDeleted | NotificationFlags.ObjectModified | NotificationFlags.ObjectMoved | NotificationFlags.ObjectCopied | NotificationFlags.SearchComplete | NotificationFlags.TableModified | NotificationFlags.Ics;

		private const TableFlags SupportedGetContentsTableFlags = ~TableFlags.SoftDeletes;

		private const ExtendedTableFlags SupportedGetContentsTableExFlags = ExtendedTableFlags.RetrieveFromIndex | ExtendedTableFlags.Associated | ExtendedTableFlags.Depth | ExtendedTableFlags.DeferredErrors | ExtendedTableFlags.NoNotifications | ExtendedTableFlags.MapiUnicode | ExtendedTableFlags.SuppressNotifications | ExtendedTableFlags.DocumentIdView | ExtendedTableFlags.ExpandedConversations | ExtendedTableFlags.PrereadExtendedProperties;

		private const TableFlags SupportedGetHierarchyTableFlags = ~(TableFlags.RetrieveFromIndex | TableFlags.SoftDeletes);

		private const OpenMode RequestedOpenModeMask = OpenMode.BestAccess;

		private const ImportMessageChangeFlags SupportedImportMessageChangeFlags = ImportMessageChangeFlags.Associated | ImportMessageChangeFlags.FailOnConflict;

		private const SetColumnsFlags SupportedSetColumnsFlags = SetColumnsFlags.Asynchronous;

		private static StorePropTag[] transportProviderSubmitProperties = new StorePropTag[]
		{
			PropTag.Message.MessageSubmissionId,
			PropTag.Message.SenderName,
			PropTag.Message.SenderEntryId,
			PropTag.Message.SenderSearchKey,
			PropTag.Message.SentRepresentingName,
			PropTag.Message.SentRepresentingEntryId,
			PropTag.Message.SentRepresentingSearchKey
		};

		private static HashSet<PropertyTag> recipientExtraExcludeProperties = new HashSet<PropertyTag>
		{
			PropertyTag.AddressType,
			PropertyTag.EmailAddress,
			PropertyTag.EntryId,
			PropertyTag.RecipientType,
			PropertyTag.DisplayName,
			PropertyTag.RowId,
			PropertyTag.InstanceKey,
			PropertyTag.SearchKey,
			PropertyTag.TransmittableDisplayName,
			PropertyTag.Responsibility,
			PropertyTag.SendRichInfo
		};

		private static Properties deleteAfterSubmit = new Properties(new Property[]
		{
			new Property(PropTag.Message.DeleteAfterSubmit, true)
		});

		private static HashSet<string> actionsNoPropertiesPromotionValidation = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"SyncPeople",
			"FindItem",
			"CreateItem",
			"SubscribeToPushNotification",
			"SyncFolderItems"
		};

		private static readonly ClientType[] ClientTypesAllowedToLogonToReadOnlyDatabase = new ClientType[]
		{
			ClientType.ContentIndexing,
			ClientType.ContentIndexingMoveDestination,
			ClientType.StoreActiveMonitoring
		};

		private static Hookable<string> hookableConcurrencyTestingApplicationId = Hookable<string>.Create(true, null);

		private static Hookable<RopId> hookableConcurrencyTestingRopId = Hookable<RopId>.Create(true, RopId.None);

		private static Hookable<LockManager.LockType> hookableConcurrencyTestingInvokeWithLockType = Hookable<LockManager.LockType>.Create(true, (LockManager.LockType)0);

		private static Hookable<Action> hookableConcurrencyTestingOnBeforeOperationCompleteHook = Hookable<Action>.Create(true, null);

		private MapiSession mapiSession;
	}
}
