using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParameterNotSupportedForMigrationTypeException : LocalizedException
	{
		public ParameterNotSupportedForMigrationTypeException(string parameterName, string migrationType) : base(Strings.ErrorParameterNotSupportedForMigrationType(parameterName, migrationType))
		{
			this.parameterName = parameterName;
			this.migrationType = migrationType;
		}

		public ParameterNotSupportedForMigrationTypeException(string parameterName, string migrationType, Exception innerException) : base(Strings.ErrorParameterNotSupportedForMigrationType(parameterName, migrationType), innerException)
		{
			this.parameterName = parameterName;
			this.migrationType = migrationType;
		}

		protected ParameterNotSupportedForMigrationTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parameterName = (string)info.GetValue("parameterName", typeof(string));
			this.migrationType = (string)info.GetValue("migrationType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parameterName", this.parameterName);
			info.AddValue("migrationType", this.migrationType);
		}

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}

		public string MigrationType
		{
			get
			{
				return this.migrationType;
			}
		}

		private readonly string parameterName;

		private readonly string migrationType;
	}
}
