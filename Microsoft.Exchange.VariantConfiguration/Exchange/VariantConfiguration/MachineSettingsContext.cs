using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public class MachineSettingsContext : IConstraintProvider
	{
		protected MachineSettingsContext()
		{
		}

		public static MachineSettingsContext Local
		{
			get
			{
				return MachineSettingsContext.LocalContext;
			}
		}

		public ConstraintCollection Constraints
		{
			get
			{
				return ConstraintCollection.CreateGlobal();
			}
		}

		public string RotationId
		{
			get
			{
				return "Global";
			}
		}

		public string RampId
		{
			get
			{
				return "Global";
			}
		}

		private const string SnapshotId = "Global";

		private static readonly MachineSettingsContext LocalContext = new MachineSettingsContext();
	}
}
