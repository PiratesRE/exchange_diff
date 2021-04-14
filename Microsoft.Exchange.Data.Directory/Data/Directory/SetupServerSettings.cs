using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class SetupServerSettings : ADServerSettings, ICloneable, IEquatable<SetupServerSettings>
	{
		private SetupServerSettings()
		{
		}

		internal static SetupServerSettings CreateSetupServerSettings()
		{
			return new SetupServerSettings();
		}

		protected override bool EnforceIsUpdatableByADSession
		{
			get
			{
				return false;
			}
		}

		internal override void SetPreferredGlobalCatalog(string partitionFqdn, ADServerInfo serverInfo)
		{
			throw new NotSupportedException("SetPreferredGlobalCatalog passing ADServerInfo is not supported on SetupServerSettings");
		}

		internal override void SetConfigurationDomainController(string partitionFqdn, ADServerInfo serverInfo)
		{
			throw new NotSupportedException("SetConfigurationDomainController passing ADServerInfo is not supported on SetupServerSettings");
		}

		internal override void AddPreferredDC(ADServerInfo serverInfo)
		{
			throw new NotSupportedException("AddPreferredDC passing ADServerInfo is not supported on SetupServerSettings");
		}

		internal override ADObjectId RecipientViewRoot
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotSupportedException("RecipientViewRoot setter is not supported on SetupServerSettings");
			}
		}

		internal override bool ViewEntireForest
		{
			get
			{
				return true;
			}
			set
			{
				throw new NotSupportedException("ViewEntireForest setter is not supported on SetupServerSettings");
			}
		}

		internal override bool IsFailoverRequired()
		{
			return false;
		}

		internal override bool TryFailover(out ADServerSettings newServerSettings, out string failToFailOverReason, bool forestWideAffinityRequested = false)
		{
			throw new NotSupportedException("TryFailover setter is not supported on SetupServerSettings");
		}

		internal override void MarkDcDown(Fqdn fqdn)
		{
			throw new NotSupportedException("MarkDcDown setter is not supported on SetupServerSettings");
		}

		internal override void MarkDcUp(Fqdn fqdn)
		{
			throw new NotSupportedException("MarkDcUp setter is not supported on SetupServerSettings");
		}

		public override object Clone()
		{
			SetupServerSettings setupServerSettings = new SetupServerSettings();
			this.CopyTo(setupServerSettings);
			return setupServerSettings;
		}

		public bool Equals(SetupServerSettings other)
		{
			return base.Equals(other);
		}
	}
}
