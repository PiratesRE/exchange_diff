using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetInsertOperator : InsertOperator
	{
		internal JetInsertOperator(CultureInfo culture, IConnectionProvider connectionProvider, Table table, SimpleQueryOperator simpleQueryOperator, IList<Column> columnsToInsert, IList<object> valuesToInsert, Action<object[]> actionOnInsert, Column[] argumentColumns, Column columnToFetch, bool unversioned, bool ignoreDuplicateKey, bool frequentOperation) : base(culture, connectionProvider, table, simpleQueryOperator, columnsToInsert, valuesToInsert, columnToFetch, frequentOperation)
		{
			this.actionOnInsert = actionOnInsert;
			this.argumentColumns = argumentColumns;
			this.unversioned = unversioned;
			this.ignoreDuplicateKey = ignoreDuplicateKey;
			if (simpleQueryOperator != null && simpleQueryOperator is JetTableOperator && simpleQueryOperator.Table == table)
			{
				this.insertCopy = true;
				int num = 0;
				while (num < table.SpecialCols.NumberOfPartioningColumns && this.insertCopy)
				{
					int i = 0;
					while (i < columnsToInsert.Count)
					{
						if (columnsToInsert[i] == table.Columns[num])
						{
							if (columnsToInsert[i] != simpleQueryOperator.ColumnsToFetch[i])
							{
								this.insertCopy = false;
								break;
							}
							break;
						}
						else
						{
							i++;
						}
					}
					num++;
				}
			}
			if (this.insertCopy)
			{
				this.jetTableOperatorForInsert = (JetTableOperator)simpleQueryOperator;
				return;
			}
			this.jetTableOperatorForInsert = new JetTableOperator(base.Culture, connectionProvider, table, table.PrimaryKeyIndex, null, null, null, null, 0, 0, KeyRange.AllRows, false, true, frequentOperation);
		}

		internal bool InsertCopy
		{
			get
			{
				return this.insertCopy;
			}
		}

		private IJetSimpleQueryOperator JetSimpleQueryOperator
		{
			get
			{
				return (IJetSimpleQueryOperator)base.SimpleQueryOperator;
			}
		}

		public override bool Interrupted
		{
			get
			{
				return base.SimpleQueryOperator != null && this.interruptControl != null && base.SimpleQueryOperator.Interrupted;
			}
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			base.Connection.CountStatement(Connection.OperationType.Insert);
			int num = 0;
			object obj = null;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				using (base.Connection.TrackTimeInDatabase())
				{
					if (base.SimpleQueryOperator != null)
					{
						bool flag;
						if (this.interruptControl == null || !this.JetSimpleQueryOperator.Interrupted)
						{
							int num2;
							flag = this.JetSimpleQueryOperator.MoveFirst(out num2);
						}
						else
						{
							flag = this.JetSimpleQueryOperator.MoveNext();
						}
						object[] array = null;
						while (flag)
						{
							if (this.interruptControl != null && this.JetSimpleQueryOperator.Interrupted)
							{
								if (!this.insertCopy)
								{
									this.jetTableOperatorForInsert.CloseJetCursor();
									break;
								}
								break;
							}
							else
							{
								if (this.interruptControl != null)
								{
									this.interruptControl.RegisterWrite(this.jetTableOperatorForInsert.Table.TableClass);
								}
								bool flag2;
								if (this.insertCopy)
								{
									flag2 = this.jetTableOperatorForInsert.InsertCopy(base.ColumnsToInsert, base.SimpleQueryOperator.ColumnsToFetch, base.ColumnToFetch, this.unversioned, this.ignoreDuplicateKey, out obj);
								}
								else
								{
									if (array == null)
									{
										array = new object[base.ColumnsToInsert.Count];
									}
									for (int i = 0; i < base.ColumnsToInsert.Count; i++)
									{
										Column column = base.SimpleQueryOperator.ColumnsToFetch[i];
										array[i] = this.JetSimpleQueryOperator.GetColumnValue(column);
										if (array[i] is LargeValue)
										{
											if (!(base.SimpleQueryOperator is IColumnStreamAccess) || !(column is PhysicalColumn))
											{
												throw new StoreException((LID)56592U, ErrorCodeValue.NotSupported, "LargeValue value cannot be inserted.");
											}
											array[i] = this.GetStreamableColumnValue((IColumnStreamAccess)base.SimpleQueryOperator, (PhysicalColumn)column);
										}
									}
									flag2 = this.jetTableOperatorForInsert.Insert(base.ColumnsToInsert, array, base.ColumnToFetch, this.unversioned, this.ignoreDuplicateKey, out obj);
								}
								if (flag2)
								{
									if (this.actionOnInsert != null)
									{
										if (this.argumentColumns != null)
										{
											object[] array2 = new object[this.argumentColumns.Length];
											for (int j = 0; j < this.argumentColumns.Length; j++)
											{
												Column column2 = this.argumentColumns[j];
												array2[j] = this.JetSimpleQueryOperator.GetColumnValue(column2);
											}
											this.actionOnInsert(array2);
										}
										else
										{
											this.actionOnInsert(null);
										}
									}
									num++;
								}
								flag = this.JetSimpleQueryOperator.MoveNext();
							}
						}
					}
					else if (this.jetTableOperatorForInsert.Insert(base.ColumnsToInsert, base.ValuesToInsert, base.ColumnToFetch, this.unversioned, this.ignoreDuplicateKey, out obj))
					{
						num++;
					}
				}
			}
			object result = (base.ColumnToFetch != null) ? obj : num;
			base.TraceOperationResult("ExecuteScalar", base.ColumnToFetch, result);
			return result;
		}

		public override bool EnableInterrupts(IInterruptControl interruptControl)
		{
			if (base.SimpleQueryOperator == null || base.ColumnToFetch != null)
			{
				return false;
			}
			if (!base.SimpleQueryOperator.EnableInterrupts(interruptControl))
			{
				return false;
			}
			this.interruptControl = interruptControl;
			return true;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.jetTableOperatorForInsert != null && !this.insertCopy)
				{
					this.jetTableOperatorForInsert.Dispose();
					this.jetTableOperatorForInsert = null;
				}
				base.InternalDispose(calledFromDispose);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetInsertOperator>(this);
		}

		private byte[] GetStreamableColumnValue(IColumnStreamAccess streamAccess, PhysicalColumn column)
		{
			byte[] result;
			using (PhysicalColumnStream physicalColumnStream = new PhysicalColumnStream(streamAccess, column, true))
			{
				byte[] array = new byte[physicalColumnStream.Length];
				physicalColumnStream.Read(array, 0, array.Length);
				result = array;
			}
			return result;
		}

		private readonly Action<object[]> actionOnInsert;

		private readonly Column[] argumentColumns;

		private readonly bool unversioned;

		private readonly bool ignoreDuplicateKey;

		private readonly bool insertCopy;

		private JetTableOperator jetTableOperatorForInsert;

		private IInterruptControl interruptControl;
	}
}
