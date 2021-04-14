using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal abstract class NspiStateDispatchTask : NspiDispatchTask
	{
		public NspiStateDispatchTask(CancelableAsyncCallback asyncCallback, object asyncState, ProtocolRequestInfo protocolRequestInfo, NspiContext context, NspiState state) : base(asyncCallback, asyncState, protocolRequestInfo, context)
		{
			this.state = state;
		}

		protected NspiState NspiState
		{
			get
			{
				return this.state;
			}
		}

		protected override void InternalTaskExecute()
		{
			base.InternalTaskExecute();
			if (this.state == null)
			{
				throw new NspiException(NspiStatus.InvalidParameter, "State cannot be null");
			}
			this.TraceNspiState();
		}

		protected void TraceNspiState()
		{
			if (NspiDispatchTask.NspiTracer.IsTraceEnabled(TraceType.DebugTrace) && this.state != null)
			{
				NspiDispatchTask.NspiTracer.TraceDebug<int, int, int>((long)base.ContextHandle, "SortType {0}, ContainerId {1} (0x{1:X}), CurrentRecord {2} (0x{2:X})", this.state.SortType, this.state.ContainerId, this.state.CurrentRecord);
				NspiDispatchTask.NspiTracer.TraceDebug<int, int, int>((long)base.ContextHandle, "  TotalRecords {0} (0x{0:X}), Position {1} (0x{1:X}), Delta {2} (0x{2:X})", this.state.TotalRecords, this.state.Position, this.state.Delta);
				NspiDispatchTask.NspiTracer.TraceDebug<int, int, int>((long)base.ContextHandle, "  CodePage 0x{0:X4}, SortLocale 0x{1:X4}, TemplateLocale 0x{2:X4}", this.state.CodePage, this.state.SortLocale, this.state.TemplateLocale);
			}
		}

		private readonly NspiState state;
	}
}
