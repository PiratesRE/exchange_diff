using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ProgressResultFactory : StandardResultFactory
	{
		internal ProgressResultFactory() : base(RopId.Progress)
		{
		}

		public static RopResult Parse(Reader reader)
		{
			RopId ropId = (RopId)reader.PeekByte(0L);
			ErrorCode errorCode = (ErrorCode)reader.PeekUInt32(2L);
			if (ropId != RopId.Progress)
			{
				RopId ropId2 = ropId;
				if (ropId2 <= RopId.EmptyFolder)
				{
					if (ropId2 == RopId.DeleteMessages)
					{
						return new DeleteMessagesResult(reader);
					}
					switch (ropId2)
					{
					case RopId.MoveCopyMessages:
						return new MoveCopyMessagesResult(reader);
					case RopId.AbortSubmit:
					case RopId.QueryColumnsAll:
					case RopId.Abort:
						break;
					case RopId.MoveFolder:
						return new MoveFolderResult(reader);
					case RopId.CopyFolder:
						return new CopyFolderResult(reader);
					case RopId.CopyTo:
						if (errorCode == ErrorCode.None)
						{
							return new SuccessfulCopyToResult(reader);
						}
						return new FailedCopyToResult(reader);
					default:
						if (ropId2 == RopId.EmptyFolder)
						{
							return new EmptyFolderResult(reader);
						}
						break;
					}
				}
				else if (ropId2 <= RopId.HardEmptyFolder)
				{
					switch (ropId2)
					{
					case RopId.SetReadFlags:
						return new SetReadFlagsResult(reader);
					case RopId.CopyProperties:
						if (errorCode == ErrorCode.None)
						{
							return new SuccessfulCopyPropertiesResult(reader);
						}
						return new FailedCopyPropertiesResult(reader);
					default:
						switch (ropId2)
						{
						case RopId.HardDeleteMessages:
							return new HardDeleteMessagesResult(reader);
						case RopId.HardEmptyFolder:
							return new HardEmptyFolderResult(reader);
						}
						break;
					}
				}
				else
				{
					if (ropId2 == RopId.MoveCopyMessagesExtended)
					{
						return new MoveCopyMessagesExtendedResult(reader);
					}
					if (ropId2 == RopId.MoveCopyMessagesExtendedWithEntryIds)
					{
						return new MoveCopyMessagesExtendedWithEntryIdsResult(reader);
					}
				}
				throw new BufferParseException(string.Format("Unexpected result RopId: {0}", ropId));
			}
			if (errorCode == ErrorCode.None)
			{
				return new SuccessfulProgressResult(reader);
			}
			return StandardRopResult.ParseFailResult(reader);
		}

		public RopResult CreateSuccessfulResult(byte logonId, uint completedTaskCount, uint totalTaskCount)
		{
			return new SuccessfulProgressResult(logonId, completedTaskCount, totalTaskCount);
		}

		public RopResult CreateSuccessfulCopyFolderResult(object progressToken, bool isPartiallyCompleted)
		{
			CopyFolderResultFactory copyFolderResultFactory = new CopyFolderResultFactory(progressToken);
			return copyFolderResultFactory.CreateSuccessfulResult(isPartiallyCompleted);
		}

		public RopResult CreateFailedCopyFolderResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			CopyFolderResultFactory copyFolderResultFactory = new CopyFolderResultFactory(progressToken);
			return copyFolderResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulCopyPropertiesResult(object progressToken, PropertyProblem[] propertyProblems)
		{
			CopyPropertiesResultFactory copyPropertiesResultFactory = new CopyPropertiesResultFactory(progressToken);
			return copyPropertiesResultFactory.CreateSuccessfulResult(propertyProblems);
		}

		public RopResult CreateFailedCopyPropertiesResult(object progressToken, ErrorCode errorCode)
		{
			CopyPropertiesResultFactory copyPropertiesResultFactory = new CopyPropertiesResultFactory(progressToken);
			return copyPropertiesResultFactory.CreateFailedResult(errorCode);
		}

		public RopResult CreateSuccessfulCopyToResult(object progressToken, PropertyProblem[] propertyProblems)
		{
			CopyToResultFactory copyToResultFactory = new CopyToResultFactory(progressToken);
			return copyToResultFactory.CreateSuccessfulResult(propertyProblems);
		}

		public RopResult CreateFailedCopyToResult(object progressToken, ErrorCode errorCode)
		{
			CopyToResultFactory copyToResultFactory = new CopyToResultFactory(progressToken);
			return copyToResultFactory.CreateFailedResult(errorCode);
		}

		public RopResult CreateSuccessfulDeleteMessagesResult(object progressToken, bool isPartiallyCompleted)
		{
			DeleteMessagesResultFactory deleteMessagesResultFactory = new DeleteMessagesResultFactory(progressToken);
			return deleteMessagesResultFactory.CreateSuccessfulResult(isPartiallyCompleted);
		}

		public RopResult CreateFailedDeleteMessagesResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			DeleteMessagesResultFactory deleteMessagesResultFactory = new DeleteMessagesResultFactory(progressToken);
			return deleteMessagesResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulEmptyFolderResult(object progressToken, bool isPartiallyCompleted)
		{
			EmptyFolderResultFactory emptyFolderResultFactory = new EmptyFolderResultFactory(progressToken);
			return emptyFolderResultFactory.CreateSuccessfulResult(isPartiallyCompleted);
		}

		public RopResult CreateFailedEmptyFolderResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			EmptyFolderResultFactory emptyFolderResultFactory = new EmptyFolderResultFactory(progressToken);
			return emptyFolderResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulHardDeleteMessagesResult(object progressToken, bool isPartiallyCompleted)
		{
			HardDeleteMessagesResultFactory hardDeleteMessagesResultFactory = new HardDeleteMessagesResultFactory(progressToken);
			return hardDeleteMessagesResultFactory.CreateSuccessfulResult(isPartiallyCompleted);
		}

		public RopResult CreateFailedHardDeleteMessagesResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			HardDeleteMessagesResultFactory hardDeleteMessagesResultFactory = new HardDeleteMessagesResultFactory(progressToken);
			return hardDeleteMessagesResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulHardEmptyFolderResult(object progressToken, bool isPartiallyCompleted)
		{
			HardEmptyFolderResultFactory hardEmptyFolderResultFactory = new HardEmptyFolderResultFactory(progressToken);
			return hardEmptyFolderResultFactory.CreateSuccessfulResult(isPartiallyCompleted);
		}

		public RopResult CreateFailedHardEmptyFolderResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			HardEmptyFolderResultFactory hardEmptyFolderResultFactory = new HardEmptyFolderResultFactory(progressToken);
			return hardEmptyFolderResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulMoveCopyMessagesResult(object progressToken, bool isPartiallyCompleted)
		{
			MoveCopyMessagesResultFactory moveCopyMessagesResultFactory = new MoveCopyMessagesResultFactory(progressToken);
			return moveCopyMessagesResultFactory.CreateSuccessfulResult(isPartiallyCompleted);
		}

		public RopResult CreateFailedMoveCopyMessagesResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			MoveCopyMessagesResultFactory moveCopyMessagesResultFactory = new MoveCopyMessagesResultFactory(progressToken);
			return moveCopyMessagesResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulMoveCopyMessagesExtendedResult(object progressToken, bool isPartiallyCompleted)
		{
			MoveCopyMessagesExtendedResultFactory moveCopyMessagesExtendedResultFactory = new MoveCopyMessagesExtendedResultFactory(progressToken);
			return moveCopyMessagesExtendedResultFactory.CreateSuccessfulResult(isPartiallyCompleted);
		}

		public RopResult CreateFailedMoveCopyMessagesExtendedResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			MoveCopyMessagesExtendedResultFactory moveCopyMessagesExtendedResultFactory = new MoveCopyMessagesExtendedResultFactory(progressToken);
			return moveCopyMessagesExtendedResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulMoveCopyMessagesExtendedWithEntryIdsResult(object progressToken, bool isPartiallyCompleted, StoreId[] messageIds, ulong[] changeNumbers)
		{
			MoveCopyMessagesExtendedWithEntryIdsResultFactory moveCopyMessagesExtendedWithEntryIdsResultFactory = new MoveCopyMessagesExtendedWithEntryIdsResultFactory(progressToken);
			return moveCopyMessagesExtendedWithEntryIdsResultFactory.CreateSuccessfulResult(isPartiallyCompleted, messageIds, changeNumbers);
		}

		public RopResult CreateFailedMoveCopyMessagesExtendedWithEntryIdsResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			MoveCopyMessagesExtendedWithEntryIdsResultFactory moveCopyMessagesExtendedWithEntryIdsResultFactory = new MoveCopyMessagesExtendedWithEntryIdsResultFactory(progressToken);
			return moveCopyMessagesExtendedWithEntryIdsResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulMoveFolderResult(object progressToken, bool isPartiallyCompleted)
		{
			MoveFolderResultFactory moveFolderResultFactory = new MoveFolderResultFactory(progressToken);
			return moveFolderResultFactory.CreateSuccessfulResult(isPartiallyCompleted);
		}

		public RopResult CreateFailedMoveFolderResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			MoveFolderResultFactory moveFolderResultFactory = new MoveFolderResultFactory(progressToken);
			return moveFolderResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}

		public RopResult CreateSuccessfulSetReadFlagsResult(object progressToken, bool isPartiallyCompleted)
		{
			SetReadFlagsResultFactory setReadFlagsResultFactory = new SetReadFlagsResultFactory(progressToken);
			return setReadFlagsResultFactory.CreateSuccessfulResult(isPartiallyCompleted);
		}

		public RopResult CreateFailedSetReadFlagsResult(object progressToken, ErrorCode errorCode, bool isPartiallyCompleted)
		{
			SetReadFlagsResultFactory setReadFlagsResultFactory = new SetReadFlagsResultFactory(progressToken);
			return setReadFlagsResultFactory.CreateFailedResult(errorCode, isPartiallyCompleted);
		}
	}
}
