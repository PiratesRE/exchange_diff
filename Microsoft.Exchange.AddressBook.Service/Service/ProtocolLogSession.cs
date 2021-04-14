using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Nspi.Rfri;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class ProtocolLogSession
	{
		internal ProtocolLogSession(LogRowFormatter row)
		{
			this.row = row;
			this[ProtocolLog.Field.SequenceNumber] = 0;
		}

		internal object this[ProtocolLog.Field field]
		{
			get
			{
				if (this.row != null)
				{
					return this.row[(int)field];
				}
				return null;
			}
			set
			{
				if (this.row != null)
				{
					this.row[(int)field] = value;
				}
			}
		}

		internal void Append(string operation, NspiStatus status, int queuedTime, int processingTime)
		{
			if (ProtocolLog.Enabled)
			{
				this.Append(operation, (status == NspiStatus.Success || status == NspiStatus.UnbindSuccess) ? null : string.Format(NumberFormatInfo.InvariantInfo, "{0:X}", new object[]
				{
					(int)status
				}), queuedTime, processingTime);
			}
		}

		internal void Append(string operation, RfriStatus status, int queuedTime, int processingTime)
		{
			if (ProtocolLog.Enabled)
			{
				this.Append(operation, (status == RfriStatus.Success) ? null : string.Format(NumberFormatInfo.InvariantInfo, "{0:X}", new object[]
				{
					(int)status
				}), queuedTime, processingTime);
			}
		}

		internal void AppendProtocolFailure(string operation, string operationSpecific, string failure)
		{
			if (ProtocolLog.Enabled)
			{
				this[ProtocolLog.Field.OperationSpecific] = operationSpecific;
				this[ProtocolLog.Field.Failures] = failure;
				this.Append(operation, string.Empty, 0, 0);
			}
		}

		private void Append(string operation, string status, int queuedTime, int processingTime)
		{
			if (ProtocolLog.Enabled)
			{
				this[ProtocolLog.Field.Operation] = operation;
				this[ProtocolLog.Field.RpcStatus] = status;
				this[ProtocolLog.Field.ProcessingTime] = processingTime;
				this[ProtocolLog.Field.Delay] = ((queuedTime == 0) ? null : queuedTime);
				ProtocolLog.Append(this.row);
				this[ProtocolLog.Field.SequenceNumber] = (int)this[ProtocolLog.Field.SequenceNumber] + 1;
				this[ProtocolLog.Field.OperationSpecific] = null;
				this[ProtocolLog.Field.Failures] = null;
				this[ProtocolLog.Field.Authentication] = null;
			}
		}

		private readonly LogRowFormatter row;
	}
}
