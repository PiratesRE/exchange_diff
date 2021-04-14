using System;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmEvtConfigChanged : AmEvtBase
	{
		internal AmEvtConfigChanged(AmConfigChangedFlags changeFlags, AmConfig previousConfig, AmConfig newConfig)
		{
			this.ChangeFlags = changeFlags;
			this.PreviousConfig = previousConfig;
			this.NewConfig = newConfig;
		}

		internal AmConfigChangedFlags ChangeFlags { get; set; }

		internal AmConfig PreviousConfig { get; set; }

		internal AmConfig NewConfig { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: Params: (ChangeFlags={1}, PrevCfgRole={2}, NewCfgRole={3})", new object[]
			{
				base.GetType().Name,
				this.ChangeFlags,
				this.PreviousConfig.Role,
				this.NewConfig.Role
			});
		}
	}
}
