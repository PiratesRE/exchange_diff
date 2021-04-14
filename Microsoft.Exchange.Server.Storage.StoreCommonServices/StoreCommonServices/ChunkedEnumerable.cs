using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class ChunkedEnumerable : IChunked
	{
		public ChunkedEnumerable(string workDescription, IEnumerable<bool> steps, ILockName mailboxLockName, TimeSpan minTimeBetweenInterrupts, TimeSpan maxTimeBetweenInterrupts)
		{
			this.workDescription = workDescription;
			this.steps = steps;
			this.mailboxLockName = mailboxLockName;
			this.minMillisecondsBetweenInterrupts = (int)minTimeBetweenInterrupts.TotalMilliseconds;
			this.maxMillisecondsBetweenInterrupts = (int)maxTimeBetweenInterrupts.TotalMilliseconds;
		}

		public bool MustYield
		{
			get
			{
				return false;
			}
		}

		internal static IDisposable SetInterruptAfterEachStepTestHook(bool value)
		{
			return ChunkedEnumerable.interruptAfterEachStepTestHook.SetTestHook(value);
		}

		public bool DoChunk(Context context)
		{
			if (this.steps != null)
			{
				if (this.stepEnumerator == null)
				{
					if (ExTraceGlobals.ChunkingTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.ChunkingTracer.TraceDebug<string>(0L, "Starting chunked {0}", this.workDescription);
					}
					this.stepEnumerator = this.steps.GetEnumerator();
				}
				if (ExTraceGlobals.ChunkingTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ChunkingTracer.TraceDebug<string>(0L, "Starting {0} chunk", this.workDescription);
				}
				int tickCount = Environment.TickCount;
				int num = tickCount + this.maxMillisecondsBetweenInterrupts;
				int num2 = tickCount + this.minMillisecondsBetweenInterrupts;
				while (this.stepEnumerator.MoveNext())
				{
					tickCount = Environment.TickCount;
					if (ChunkedEnumerable.interruptAfterEachStepTestHook.Value || tickCount - num >= 0 || (tickCount - num2 >= 0 && LockManager.HasContention(this.mailboxLockName)))
					{
						return false;
					}
				}
				this.Dispose(context);
				if (ExTraceGlobals.ChunkingTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.ChunkingTracer.TraceDebug<string>(0L, "Finished chunked {0}", this.workDescription);
				}
			}
			return true;
		}

		public void Dispose(Context context)
		{
			if (this.steps != null)
			{
				if (this.stepEnumerator != null)
				{
					this.stepEnumerator.Dispose();
					this.stepEnumerator = null;
				}
				this.steps = null;
			}
		}

		private static readonly Hookable<bool> interruptAfterEachStepTestHook = Hookable<bool>.Create(true, false);

		private readonly string workDescription;

		private readonly ILockName mailboxLockName;

		private readonly int minMillisecondsBetweenInterrupts;

		private readonly int maxMillisecondsBetweenInterrupts;

		private IEnumerable<bool> steps;

		private IEnumerator<bool> stepEnumerator;
	}
}
