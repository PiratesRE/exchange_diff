using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationFeatureNotSupportedException : MigrationPermanentException
	{
		public MigrationFeatureNotSupportedException(string feature) : base(Strings.MigrationFeatureNotSupported(feature))
		{
			this.feature = feature;
		}

		public MigrationFeatureNotSupportedException(string feature, Exception innerException) : base(Strings.MigrationFeatureNotSupported(feature), innerException)
		{
			this.feature = feature;
		}

		protected MigrationFeatureNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.feature = (string)info.GetValue("feature", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("feature", this.feature);
		}

		public string Feature
		{
			get
			{
				return this.feature;
			}
		}

		private readonly string feature;
	}
}
