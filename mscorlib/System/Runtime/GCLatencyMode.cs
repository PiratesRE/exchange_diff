using System;

namespace System.Runtime
{
	[__DynamicallyInvokable]
	[Serializable]
	public enum GCLatencyMode
	{
		[__DynamicallyInvokable]
		Batch,
		[__DynamicallyInvokable]
		Interactive,
		[__DynamicallyInvokable]
		LowLatency,
		[__DynamicallyInvokable]
		SustainedLowLatency,
		NoGCRegion
	}
}
