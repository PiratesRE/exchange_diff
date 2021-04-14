using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Isam.Esent.Interop;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal struct CachedColumnValues
	{
		public CachedColumnValues(JetConnection jetConnection, HashSet<PhysicalColumn> columns, BitArray retrieveFromPrimaryBookmarkMap, BitArray retrieveFromIndexMap)
		{
			this.columnValues = new Microsoft.Isam.Esent.Interop.ColumnValue[columns.Count];
			int num = 0;
			foreach (PhysicalColumn physicalColumn in columns)
			{
				this.columnValues[num++] = JetColumnValueHelper.CreateColumnValue(jetConnection, (JetPhysicalColumn)physicalColumn, retrieveFromPrimaryBookmarkMap, retrieveFromIndexMap);
			}
			Array.Sort<Microsoft.Isam.Esent.Interop.ColumnValue>(this.columnValues, ColumnValueComparer.Instance);
			this.valuesAreRetrieved = false;
		}

		public bool TryGetValue(JetConnection jetConnection, Microsoft.Exchange.Server.Storage.PhysicalAccess.Table table, JET_TABLEID tableid, JET_COLUMNID columnid, out object value)
		{
			Microsoft.Isam.Esent.Interop.ColumnValue columnValue = this.FindColumnValue(columnid);
			if (columnValue == null)
			{
				value = null;
				return false;
			}
			if (!this.valuesAreRetrieved)
			{
				try
				{
					using (jetConnection.TrackTimeInDatabase())
					{
						Api.RetrieveColumns(jetConnection.JetSession, tableid, this.columnValues);
					}
				}
				catch (EsentErrorException ex)
				{
					jetConnection.OnExceptionCatch(ex);
					throw jetConnection.ProcessJetError((LID)64304U, "CachedColumnValues.TryGetValue", ex);
				}
				this.valuesAreRetrieved = true;
				int num = 0;
				for (int i = 0; i < this.columnValues.Length; i++)
				{
					num += this.columnValues[i].Length;
				}
				jetConnection.AddRowStatsCounter(table, RowStatsCounterType.ReadBytes, num);
			}
			value = columnValue.ValueAsObject;
			return true;
		}

		public void Reset()
		{
			this.valuesAreRetrieved = false;
		}

		private Microsoft.Isam.Esent.Interop.ColumnValue FindColumnValue(JET_COLUMNID columnid)
		{
			int i = 0;
			int num = this.columnValues.Length;
			while (i < num)
			{
				int num2 = (i + num) / 2;
				Microsoft.Isam.Esent.Interop.ColumnValue columnValue = this.columnValues[num2];
				if (columnValue.Columnid > columnid)
				{
					num = num2;
				}
				else
				{
					if (!(columnValue.Columnid < columnid))
					{
						return columnValue;
					}
					i = num2 + 1;
				}
			}
			return null;
		}

		private readonly Microsoft.Isam.Esent.Interop.ColumnValue[] columnValues;

		private bool valuesAreRetrieved;
	}
}
