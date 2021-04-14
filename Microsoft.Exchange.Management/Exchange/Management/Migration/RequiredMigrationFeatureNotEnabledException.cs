using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequiredMigrationFeatureNotEnabledException : LocalizedException
	{
		public RequiredMigrationFeatureNotEnabledException(MigrationFeature feature) : base(Strings.ErrorRequiredMigrationFeatureNotEnabled(feature))
		{
			this.feature = feature;
		}

		public RequiredMigrationFeatureNotEnabledException(MigrationFeature feature, Exception innerException) : base(Strings.ErrorRequiredMigrationFeatureNotEnabled(feature), innerException)
		{
			this.feature = feature;
		}

		protected RequiredMigrationFeatureNotEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.feature = (MigrationFeature)info.GetValue("feature", typeof(MigrationFeature));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("feature", this.feature);
		}

		public MigrationFeature Feature
		{
			get
			{
				return this.feature;
			}
		}

		private readonly MigrationFeature feature;
	}
}
