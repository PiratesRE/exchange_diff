using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetUpdateOperator : UpdateOperator
	{
		internal JetUpdateOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, IList<Column> columnsToUpdate, IList<object> valuesToUpdate, bool frequentOperation) : base(culture, connectionProvider, tableOperator, columnsToUpdate, valuesToUpdate, frequentOperation)
		{
		}

		public override bool Interrupted
		{
			get
			{
				return base.TableOperator.Interrupted;
			}
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			base.Connection.CountStatement(Connection.OperationType.Update);
			int num = 0;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				using (base.Connection.TrackTimeInDatabase())
				{
					JetTableOperator jetTableOperator = base.TableOperator as JetTableOperator;
					int num2 = 0;
					bool flag;
					if (!jetTableOperator.Interrupted)
					{
						flag = jetTableOperator.MoveFirst(true, Connection.OperationType.Update, ref num2);
					}
					else
					{
						flag = jetTableOperator.MoveNext();
					}
					while (flag && (this.interruptControl == null || !jetTableOperator.Interrupted))
					{
						if (this.interruptControl != null)
						{
							this.interruptControl.RegisterWrite(jetTableOperator.Table.TableClass);
						}
						jetTableOperator.Update(base.ColumnsToUpdate, base.ValuesToUpdate);
						num++;
						flag = jetTableOperator.MoveNext();
					}
				}
			}
			object result = num;
			base.TraceOperationResult("ExecuteScalar", null, result);
			return result;
		}

		public override bool EnableInterrupts(IInterruptControl interruptControl)
		{
			if (!base.TableOperator.EnableInterrupts(interruptControl))
			{
				return false;
			}
			this.interruptControl = interruptControl;
			return true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetUpdateOperator>(this);
		}

		private IInterruptControl interruptControl;
	}
}
