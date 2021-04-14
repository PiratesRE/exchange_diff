using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class TopologyServiceServerSettings : ADServerSettings, ICloneable, IEquatable<TopologyServiceServerSettings>
	{
		private TopologyServiceServerSettings()
		{
		}

		internal static TopologyServiceServerSettings CreateTopologyServiceServerSettings()
		{
			return new TopologyServiceServerSettings();
		}

		protected override bool EnforceIsUpdatableByADSession
		{
			get
			{
				return true;
			}
		}

		internal override ADObjectId RecipientViewRoot
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotSupportedException("RecipientViewRoot setter is not supported on TopologyServiceServerSettings");
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
				throw new NotSupportedException("ViewEntireForest setter is not supported on TopologyServiceServerSettings");
			}
		}

		public override object Clone()
		{
			TopologyServiceServerSettings topologyServiceServerSettings = new TopologyServiceServerSettings();
			this.CopyTo(topologyServiceServerSettings);
			return topologyServiceServerSettings;
		}

		public bool Equals(TopologyServiceServerSettings other)
		{
			return base.Equals(other);
		}
	}
}
