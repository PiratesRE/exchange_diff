using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal abstract class BulkOperation : DisposableBase
	{
		public BulkOperation(int chunkSize)
		{
			this.chunkSize = chunkSize;
		}

		public int ChunkSize
		{
			get
			{
				return this.chunkSize;
			}
		}

		public abstract bool DoChunk(MapiContext context, out bool progress, out bool incomplete, out ErrorCode error);

		public bool DoChunk(MapiContext context, out bool progress, out bool incomplete, out ErrorCode error)
		{
			ErrorCode errorCode;
			bool result = this.DoChunk(context, out progress, out incomplete, out errorCode);
			error = (ErrorCode)errorCode;
			return result;
		}

		public bool DoChunk(MapiContext context)
		{
			bool flag2;
			bool flag3;
			ErrorCode errorCode;
			bool flag = this.DoChunk(context, out flag2, out flag3, out errorCode);
			if (flag && errorCode != ErrorCode.NoError)
			{
				throw new StoreException((LID)65336U, errorCode);
			}
			return flag;
		}

		public ErrorCode DoAll(MapiContext context, bool splitTransaction, out bool progress, out bool incomplete)
		{
			progress = false;
			incomplete = false;
			bool flag;
			bool flag2;
			ErrorCode result;
			while (!this.DoChunk(context, out flag, out flag2, out result))
			{
				progress = (progress || flag);
				incomplete = (incomplete || flag2);
				if (splitTransaction)
				{
					context.Commit();
				}
			}
			progress = (progress || flag);
			incomplete = (incomplete || flag2);
			return result;
		}

		public void DoAll(MapiContext context, bool splitTransaction)
		{
			while (!this.DoChunk(context))
			{
				if (splitTransaction)
				{
					context.Commit();
				}
			}
		}

		internal static bool MoveMessages(MapiContext context, MapiFolder source, MapiFolder destination, IList<ExchangeId> mids, Properties propsToSet, BulkErrorAction notFoundAction, BulkErrorAction softErrorAction, IList<ExchangeId> outputMids, IList<ExchangeId> outputCns, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			progressCount = 0;
			bool result = true;
			for (int i = 0; i < mids.Count; i++)
			{
				ExchangeId item;
				ExchangeId item2;
				error = source.MoveMessageTo(context, destination, mids[i], propsToSet, out item, out item2);
				if (error != ErrorCode.NoError)
				{
					if (!BulkOperation.ContinueOnError(ref error, ref incomplete, notFoundAction, softErrorAction))
					{
						result = false;
						break;
					}
				}
				else
				{
					progressCount++;
					if (outputMids != null)
					{
						outputMids.Add(item);
					}
					if (outputCns != null)
					{
						outputCns.Add(item2);
					}
				}
			}
			return result;
		}

		internal static bool CopyMessages(MapiContext context, MapiFolder source, MapiFolder destination, IList<ExchangeId> mids, Properties propsToSet, BulkErrorAction notFoundAction, BulkErrorAction softErrorAction, IList<ExchangeId> outputMids, IList<ExchangeId> outputCns, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			progressCount = 0;
			bool result = true;
			for (int i = 0; i < mids.Count; i++)
			{
				ExchangeId item;
				ExchangeId item2;
				error = source.CopyMessageTo(context, destination, mids[i], propsToSet, out item, out item2);
				if (error != ErrorCode.NoError)
				{
					if (!BulkOperation.ContinueOnError(ref error, ref incomplete, notFoundAction, softErrorAction))
					{
						result = false;
						break;
					}
				}
				else
				{
					progressCount++;
					if (outputMids != null)
					{
						outputMids.Add(item);
					}
					if (outputCns != null)
					{
						outputCns.Add(item2);
					}
				}
			}
			return result;
		}

		internal static bool DeleteMessages(MapiContext context, MapiFolder source, bool sendNRN, bool deleteSubmitted, IList<ExchangeId> mids, BulkErrorAction notFoundAction, BulkErrorAction softErrorAction, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			progressCount = 0;
			bool result = true;
			int count = mids.Count;
			for (int i = 0; i < count; i++)
			{
				if (mids[i].IsValid)
				{
					error = source.DeleteMessage(context, sendNRN, deleteSubmitted, mids[i]);
					if (error != ErrorCode.NoError)
					{
						if (!BulkOperation.ContinueOnError(ref error, ref incomplete, notFoundAction, softErrorAction))
						{
							incomplete = true;
							result = false;
							break;
						}
					}
					else
					{
						progressCount++;
					}
				}
			}
			return result;
		}

		internal static bool SetReadFlags(MapiContext context, MapiFolder source, SetReadFlagFlags flags, IList<ExchangeId> mids, BulkErrorAction notFoundAction, BulkErrorAction softErrorAction, IList<ExchangeId> readCns, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			progressCount = 0;
			bool result = true;
			for (int i = 0; i < mids.Count; i++)
			{
				bool flag;
				ExchangeId item;
				error = source.SetMessageReadFlag(context, flags, mids[i], out flag, out item);
				if (error != ErrorCode.NoError)
				{
					if (!BulkOperation.ContinueOnError(ref error, ref incomplete, notFoundAction, softErrorAction))
					{
						result = false;
						break;
					}
				}
				else if (flag)
				{
					progressCount++;
					if (readCns != null)
					{
						readCns.Add(item);
					}
				}
			}
			return result;
		}

		protected static bool ContinueOnError(ref ErrorCode error, ref bool incomplete, BulkErrorAction notFoundAction, BulkErrorAction softErrorAction)
		{
			switch ((error == ErrorCodeValue.NotFound) ? notFoundAction : softErrorAction)
			{
			case BulkErrorAction.Skip:
				error = ErrorCode.NoError;
				return true;
			case BulkErrorAction.Incomplete:
				incomplete = true;
				error = ErrorCode.NoError;
				return true;
			case BulkErrorAction.Error:
				return false;
			default:
				throw new StoreException((LID)40760U, error);
			}
		}

		protected static void InvalidateFolderIndicesIfNeeded(MapiContext context, Folder folder)
		{
			bool flag = folder.GetMessageCount(context) < (long)MessageViewTable.SmallFolderThreshold;
			bool flag2 = folder.GetHiddenItemCount(context) < (long)MessageViewTable.SmallFolderThreshold;
			if (flag || flag2)
			{
				folder.InvalidateIndexes(context, flag, flag2);
			}
		}

		internal const int ReasonableBulkOperationChunkSize = 100;

		internal static readonly StorePropTag[] ColumnsToFetchMid = new StorePropTag[]
		{
			PropTag.Message.MidBin
		};

		private readonly int chunkSize;
	}
}
