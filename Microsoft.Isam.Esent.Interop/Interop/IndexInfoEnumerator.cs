using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Isam.Esent.Interop
{
	internal abstract class IndexInfoEnumerator : TableEnumerator<IndexInfo>
	{
		protected IndexInfoEnumerator(JET_SESID sesid) : base(sesid)
		{
		}

		protected JET_INDEXLIST Indexlist { get; set; }

		protected override IndexInfo GetCurrent()
		{
			return this.GetIndexInfoFromIndexlist(base.Sesid, this.Indexlist);
		}

		protected abstract void GetIndexInfo(JET_SESID sesid, string indexname, out string result, JET_IdxInfo infoLevel);

		private static IndexSegment[] GetIndexSegmentsFromIndexlist(JET_SESID sesid, JET_INDEXLIST indexlist)
		{
			int value = Api.RetrieveColumnAsInt32(sesid, indexlist.tableid, indexlist.columnidcColumn).Value;
			Encoding encoding = EsentVersion.SupportsVistaFeatures ? Encoding.Unicode : LibraryHelpers.EncodingASCII;
			IndexSegment[] array = new IndexSegment[value];
			for (int i = 0; i < value; i++)
			{
				string text = Api.RetrieveColumnAsString(sesid, indexlist.tableid, indexlist.columnidcolumnname, encoding, RetrieveColumnGrbit.None);
				text = StringCache.TryToIntern(text);
				JET_coltyp value2 = (JET_coltyp)Api.RetrieveColumnAsInt32(sesid, indexlist.tableid, indexlist.columnidcoltyp).Value;
				IndexKeyGrbit value3 = (IndexKeyGrbit)Api.RetrieveColumnAsInt32(sesid, indexlist.tableid, indexlist.columnidgrbitColumn).Value;
				bool isAscending = IndexKeyGrbit.Ascending == value3;
				JET_CP value4 = (JET_CP)Api.RetrieveColumnAsInt16(sesid, indexlist.tableid, indexlist.columnidCp).Value;
				bool isASCII = JET_CP.ASCII == value4;
				array[i] = new IndexSegment(text, value2, isAscending, isASCII);
				if (i < value - 1)
				{
					Api.JetMove(sesid, indexlist.tableid, JET_Move.Next, MoveGrbit.None);
				}
			}
			return array;
		}

		private IndexInfo GetIndexInfoFromIndexlist(JET_SESID sesid, JET_INDEXLIST indexlist)
		{
			Encoding encoding = EsentVersion.SupportsVistaFeatures ? Encoding.Unicode : LibraryHelpers.EncodingASCII;
			string text = Api.RetrieveColumnAsString(sesid, indexlist.tableid, indexlist.columnidindexname, encoding, RetrieveColumnGrbit.None);
			text = StringCache.TryToIntern(text);
			CultureInfo cultureInfo;
			if (EsentVersion.SupportsWindows8Features)
			{
				string name;
				this.GetIndexInfo(sesid, text, out name, (JET_IdxInfo)14);
				cultureInfo = new CultureInfo(name);
			}
			else
			{
				int value = (int)Api.RetrieveColumnAsInt16(sesid, indexlist.tableid, indexlist.columnidLangid).Value;
				cultureInfo = LibraryHelpers.CreateCultureInfoByLcid(value);
			}
			uint value2 = Api.RetrieveColumnAsUInt32(sesid, indexlist.tableid, indexlist.columnidLCMapFlags).Value;
			CompareOptions compareOptions = Conversions.CompareOptionsFromLCMapFlags(value2);
			uint value3 = Api.RetrieveColumnAsUInt32(sesid, indexlist.tableid, indexlist.columnidgrbitIndex).Value;
			int value4 = Api.RetrieveColumnAsInt32(sesid, indexlist.tableid, indexlist.columnidcKey).Value;
			int value5 = Api.RetrieveColumnAsInt32(sesid, indexlist.tableid, indexlist.columnidcEntry).Value;
			int value6 = Api.RetrieveColumnAsInt32(sesid, indexlist.tableid, indexlist.columnidcPage).Value;
			IndexSegment[] indexSegmentsFromIndexlist = IndexInfoEnumerator.GetIndexSegmentsFromIndexlist(sesid, indexlist);
			return new IndexInfo(text, cultureInfo, compareOptions, indexSegmentsFromIndexlist, (CreateIndexGrbit)value3, value4, value5, value6);
		}
	}
}
