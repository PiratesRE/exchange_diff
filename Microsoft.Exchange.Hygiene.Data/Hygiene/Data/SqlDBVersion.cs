using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class SqlDBVersion : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.VersionId.ToString());
			}
		}

		public Guid VersionId
		{
			get
			{
				return (Guid)this[SqlDBVersion.VersionIdProp];
			}
			set
			{
				this[SqlDBVersion.VersionIdProp] = value;
			}
		}

		public string VersionName
		{
			get
			{
				return (string)this[SqlDBVersion.VersionStringProp];
			}
			set
			{
				this[SqlDBVersion.VersionStringProp] = value;
			}
		}

		public long VersionNumber
		{
			get
			{
				return (long)this[SqlDBVersion.VersionNumberProp];
			}
			set
			{
				this[SqlDBVersion.VersionNumberProp] = value;
			}
		}

		public static readonly HygienePropertyDefinition VersionIdProp = new HygienePropertyDefinition("VersionId", typeof(Guid));

		public static readonly HygienePropertyDefinition VersionStringProp = new HygienePropertyDefinition("VersionString", typeof(string));

		public static readonly HygienePropertyDefinition VersionNumberProp = new HygienePropertyDefinition("VersionNumber", typeof(long), long.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
