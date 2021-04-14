using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal static class JetRetrieveColumnHelper
	{
		public static long? GetPhysicalColumnValueAsBytes(JetConnection jetConnection, JET_TABLEID tableid, JET_COLUMNID columnid, ArraySegment<byte> userBuffer)
		{
			return JetRetrieveColumnHelper.RetrieveColumnValueToArraySegment(jetConnection, tableid, columnid, userBuffer, null);
		}

		public static long? RetrieveColumnValueToArraySegment(JetConnection jetConnection, JET_TABLEID tableid, JET_COLUMNID columnid, ArraySegment<byte> userBuffer, JET_RETINFO retInfo)
		{
			long? result;
			try
			{
				byte[] array = userBuffer.Array;
				int offset = userBuffer.Offset;
				int count = userBuffer.Count;
				int num;
				using (jetConnection.TrackTimeInDatabase())
				{
					JET_wrn jet_wrn = Api.JetRetrieveColumn(jetConnection.JetSession, tableid, columnid, array, count, offset, out num, RetrieveColumnGrbit.None, retInfo);
					if (JET_wrn.ColumnNull == jet_wrn)
					{
						return null;
					}
					if (JET_wrn.BufferTruncated == jet_wrn)
					{
						num = count;
					}
				}
				result = new long?((long)num);
			}
			catch (EsentErrorException ex)
			{
				jetConnection.OnExceptionCatch(ex);
				throw jetConnection.ProcessJetError((LID)35272U, "JetRetrieveColumnHelper.RetrieveColumnValueToArraySegment", ex);
			}
			return result;
		}

		public static void BuildColumnRetrieveGrbitMap(IEnumerable<PhysicalColumn> columns, Index primaryIndex, Index secondaryIndex, BitArray retrieveFromPrimaryBookmarkMap, BitArray retrieveFromIndexMap)
		{
			foreach (PhysicalColumn physicalColumn in columns)
			{
				if (secondaryIndex.Columns.Contains(physicalColumn))
				{
					retrieveFromIndexMap[physicalColumn.Index] = true;
				}
				else if (primaryIndex.Columns.Contains(physicalColumn))
				{
					retrieveFromPrimaryBookmarkMap[physicalColumn.Index] = true;
				}
			}
		}

		public static RetrieveColumnGrbit GetColumnRetrieveGrbit(PhysicalColumn column, BitArray retrieveFromPrimaryBookmarkMap, BitArray retrieveFromIndexMap)
		{
			if (retrieveFromIndexMap != null && retrieveFromIndexMap[column.Index])
			{
				return RetrieveColumnGrbit.RetrieveFromIndex;
			}
			if (retrieveFromPrimaryBookmarkMap != null && retrieveFromPrimaryBookmarkMap[column.Index])
			{
				return RetrieveColumnGrbit.RetrieveFromPrimaryBookmark;
			}
			return RetrieveColumnGrbit.None;
		}
	}
}
