using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public abstract class JET_RECOVERYCONTROL
	{
		public JET_err errDefault { get; internal set; }

		public JET_INSTANCE instance { get; private set; }

		public JET_SNT sntUnion { get; private set; }

		internal void SetFromNativeSnrecoverycontrol(ref NATIVE_RECOVERY_CONTROL native)
		{
			this.errDefault = native.errDefault;
			this.instance = new JET_INSTANCE
			{
				Value = native.instance
			};
			this.sntUnion = native.sntUnion;
		}
	}
}
