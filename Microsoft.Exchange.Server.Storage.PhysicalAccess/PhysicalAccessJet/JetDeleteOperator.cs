using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal class JetDeleteOperator : DeleteOperator
	{
		internal JetDeleteOperator(CultureInfo culture, IConnectionProvider connectionProvider, TableOperator tableOperator, bool frequentOperation) : base(culture, connectionProvider, tableOperator, frequentOperation)
		{
		}

		public override bool Interrupted
		{
			get
			{
				return this.interruptControl != null && base.TableOperator.Interrupted;
			}
		}

		public override object ExecuteScalar()
		{
			base.TraceOperation("ExecuteScalar");
			base.Connection.CountStatement(Connection.OperationType.Delete);
			object result;
			using (base.Connection.TrackDbOperationExecution(this))
			{
				using (base.Connection.TrackTimeInDatabase())
				{
					JetTableOperator jetTableOperator = base.TableOperator as JetTableOperator;
					int num = 0;
					if (jetTableOperator.Interrupted || !jetTableOperator.QuickDeleteAllMatchingRows(out num))
					{
						int num2 = 0;
						bool flag;
						if (this.interruptControl == null || !jetTableOperator.Interrupted)
						{
							flag = jetTableOperator.MoveFirst(true, Connection.OperationType.Delete, ref num2);
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
							jetTableOperator.Delete();
							num++;
							flag = jetTableOperator.MoveNext();
						}
					}
					result = num;
				}
			}
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

		protected override void InternalDispose(bool calledFromDispose)
		{
			base.InternalDispose(calledFromDispose);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<JetDeleteOperator>(this);
		}

		private IInterruptControl interruptControl;
	}
}
