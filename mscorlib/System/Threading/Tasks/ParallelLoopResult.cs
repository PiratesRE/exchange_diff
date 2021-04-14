using System;

namespace System.Threading.Tasks
{
	[__DynamicallyInvokable]
	public struct ParallelLoopResult
	{
		[__DynamicallyInvokable]
		public bool IsCompleted
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_completed;
			}
		}

		[__DynamicallyInvokable]
		public long? LowestBreakIteration
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_lowestBreakIteration;
			}
		}

		internal bool m_completed;

		internal long? m_lowestBreakIteration;
	}
}
