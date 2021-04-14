using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ParameterNotSupportedForMigrationException : LocalizedException
	{
		public ParameterNotSupportedForMigrationException(string parameterName) : base(Strings.ErrorParameterNotSupportedForMigration(parameterName))
		{
			this.parameterName = parameterName;
		}

		public ParameterNotSupportedForMigrationException(string parameterName, Exception innerException) : base(Strings.ErrorParameterNotSupportedForMigration(parameterName), innerException)
		{
			this.parameterName = parameterName;
		}

		protected ParameterNotSupportedForMigrationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.parameterName = (string)info.GetValue("parameterName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("parameterName", this.parameterName);
		}

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}

		private readonly string parameterName;
	}
}
