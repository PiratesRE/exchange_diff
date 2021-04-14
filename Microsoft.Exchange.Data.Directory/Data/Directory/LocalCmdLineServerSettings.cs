using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class LocalCmdLineServerSettings : RunspaceServerSettings, ICloneable, IEquatable<LocalCmdLineServerSettings>
	{
		private LocalCmdLineServerSettings()
		{
		}

		internal static LocalCmdLineServerSettings CreateLocalCmdLineServerSettings()
		{
			return new LocalCmdLineServerSettings
			{
				ViewEntireForest = true
			};
		}

		internal override bool IsUpdatableByADSession
		{
			get
			{
				return false;
			}
		}

		protected override bool EnforceIsUpdatableByADSession
		{
			get
			{
				return false;
			}
		}

		internal override bool IsFailoverRequired()
		{
			return false;
		}

		internal override bool TryFailover(out ADServerSettings newServerSettings, out string failToFailOverReason, bool forestWideAffinityRequested = false)
		{
			throw new NotSupportedException("TryFailover is not supported by LocalCmdLineServerSettings");
		}

		internal override void MarkDcDown(Fqdn fqdn)
		{
			throw new NotSupportedException("MarkDcDown is not supported by LocalCmdLineServerSettings");
		}

		internal override void MarkDcUp(Fqdn fqdn)
		{
			throw new NotSupportedException("MarkDcUp is not supported by LocalCmdLineServerSettings");
		}

		public override object Clone()
		{
			LocalCmdLineServerSettings localCmdLineServerSettings = new LocalCmdLineServerSettings();
			this.CopyTo(localCmdLineServerSettings);
			return localCmdLineServerSettings;
		}

		public bool Equals(LocalCmdLineServerSettings other)
		{
			return base.Equals(other);
		}
	}
}
