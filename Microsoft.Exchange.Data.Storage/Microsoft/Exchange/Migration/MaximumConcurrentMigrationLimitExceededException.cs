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
	internal class MaximumConcurrentMigrationLimitExceededException : MigrationPermanentException
	{
		public MaximumConcurrentMigrationLimitExceededException(string endpointValue, string limitValue, string migrationType) : base(Strings.ErrorMaximumConcurrentMigrationLimitExceeded(endpointValue, limitValue, migrationType))
		{
			this.endpointValue = endpointValue;
			this.limitValue = limitValue;
			this.migrationType = migrationType;
		}

		public MaximumConcurrentMigrationLimitExceededException(string endpointValue, string limitValue, string migrationType, Exception innerException) : base(Strings.ErrorMaximumConcurrentMigrationLimitExceeded(endpointValue, limitValue, migrationType), innerException)
		{
			this.endpointValue = endpointValue;
			this.limitValue = limitValue;
			this.migrationType = migrationType;
		}

		protected MaximumConcurrentMigrationLimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.endpointValue = (string)info.GetValue("endpointValue", typeof(string));
			this.limitValue = (string)info.GetValue("limitValue", typeof(string));
			this.migrationType = (string)info.GetValue("migrationType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("endpointValue", this.endpointValue);
			info.AddValue("limitValue", this.limitValue);
			info.AddValue("migrationType", this.migrationType);
		}

		public string EndpointValue
		{
			get
			{
				return this.endpointValue;
			}
		}

		public string LimitValue
		{
			get
			{
				return this.limitValue;
			}
		}

		public string MigrationType
		{
			get
			{
				return this.migrationType;
			}
		}

		private readonly string endpointValue;

		private readonly string limitValue;

		private readonly string migrationType;
	}
}
