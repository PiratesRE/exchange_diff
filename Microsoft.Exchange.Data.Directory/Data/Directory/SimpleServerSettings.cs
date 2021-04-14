using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class SimpleServerSettings : ADServerSettings, ICloneable, IEquatable<SimpleServerSettings>
	{
		internal SimpleServerSettings()
		{
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
				throw new NotSupportedException("RecipientViewRoot setter is not supported on SimpleServerSettings");
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
				throw new NotSupportedException("ViewEntireForest setter is not supported on SimpleServerSettings");
			}
		}

		public override object Clone()
		{
			SimpleServerSettings simpleServerSettings = new SimpleServerSettings();
			this.CopyTo(simpleServerSettings);
			return simpleServerSettings;
		}

		public bool Equals(SimpleServerSettings other)
		{
			return base.Equals(other);
		}
	}
}
