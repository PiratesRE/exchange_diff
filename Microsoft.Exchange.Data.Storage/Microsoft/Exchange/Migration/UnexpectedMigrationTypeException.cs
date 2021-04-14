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
	internal class UnexpectedMigrationTypeException : MigrationPermanentException
	{
		public UnexpectedMigrationTypeException(string discoveredType, string expectedType) : base(Strings.ErrorUnexpectedMigrationType(discoveredType, expectedType))
		{
			this.discoveredType = discoveredType;
			this.expectedType = expectedType;
		}

		public UnexpectedMigrationTypeException(string discoveredType, string expectedType, Exception innerException) : base(Strings.ErrorUnexpectedMigrationType(discoveredType, expectedType), innerException)
		{
			this.discoveredType = discoveredType;
			this.expectedType = expectedType;
		}

		protected UnexpectedMigrationTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.discoveredType = (string)info.GetValue("discoveredType", typeof(string));
			this.expectedType = (string)info.GetValue("expectedType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("discoveredType", this.discoveredType);
			info.AddValue("expectedType", this.expectedType);
		}

		public string DiscoveredType
		{
			get
			{
				return this.discoveredType;
			}
		}

		public string ExpectedType
		{
			get
			{
				return this.expectedType;
			}
		}

		private readonly string discoveredType;

		private readonly string expectedType;
	}
}
