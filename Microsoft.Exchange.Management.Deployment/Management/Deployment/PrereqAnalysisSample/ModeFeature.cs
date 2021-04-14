using System;
using Microsoft.Exchange.Management.Deployment.Analysis;

namespace Microsoft.Exchange.Management.Deployment.PrereqAnalysisSample
{
	internal sealed class ModeFeature : Feature
	{
		public ModeFeature(SetupMode modes)
		{
			this.mode = modes;
		}

		public SetupMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		public bool Contains(SetupMode mode)
		{
			return (this.mode & mode) > SetupMode.None;
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", base.ToString(), this.mode.ToString());
		}

		private readonly SetupMode mode;
	}
}
