using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotResolvePropertyException : StoragePermanentException
	{
		public CannotResolvePropertyException(string propertySchema) : base(ServerStrings.CannotResolvePropertyException(propertySchema))
		{
			this.propertySchema = propertySchema;
		}

		public CannotResolvePropertyException(string propertySchema, Exception innerException) : base(ServerStrings.CannotResolvePropertyException(propertySchema), innerException)
		{
			this.propertySchema = propertySchema;
		}

		protected CannotResolvePropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertySchema = (string)info.GetValue("propertySchema", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertySchema", this.propertySchema);
		}

		public string PropertySchema
		{
			get
			{
				return this.propertySchema;
			}
		}

		private readonly string propertySchema;
	}
}
