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
	internal class MigrationVersionMismatchException : MigrationTransientException
	{
		public MigrationVersionMismatchException(long version, long expectedVersion) : base(Strings.MigrationVersionMismatch(version, expectedVersion))
		{
			this.version = version;
			this.expectedVersion = expectedVersion;
		}

		public MigrationVersionMismatchException(long version, long expectedVersion, Exception innerException) : base(Strings.MigrationVersionMismatch(version, expectedVersion), innerException)
		{
			this.version = version;
			this.expectedVersion = expectedVersion;
		}

		protected MigrationVersionMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.version = (long)info.GetValue("version", typeof(long));
			this.expectedVersion = (long)info.GetValue("expectedVersion", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("version", this.version);
			info.AddValue("expectedVersion", this.expectedVersion);
		}

		public long Version
		{
			get
			{
				return this.version;
			}
		}

		public long ExpectedVersion
		{
			get
			{
				return this.expectedVersion;
			}
		}

		private readonly long version;

		private readonly long expectedVersion;
	}
}
