using System;

namespace Microsoft.Exchange.Management.Analysis.Features
{
	internal class AppliesToModeFeature : Feature
	{
		public AppliesToModeFeature(SetupMode modes) : base(true, true)
		{
			this.Mode = modes;
		}

		public SetupMode Mode { get; private set; }

		public bool Contains(SetupMode mode)
		{
			return (this.Mode & mode) > SetupMode.None;
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.Mode.ToString());
		}
	}
}
