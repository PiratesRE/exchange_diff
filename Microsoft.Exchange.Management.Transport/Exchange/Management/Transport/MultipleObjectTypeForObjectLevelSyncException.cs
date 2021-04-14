using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MultipleObjectTypeForObjectLevelSyncException : InvalidConfigurationObjectTypeException
	{
		public MultipleObjectTypeForObjectLevelSyncException(string types) : base(Strings.ErrorMultipleObjectTypeForObjectLevelSync(types))
		{
			this.types = types;
		}

		public MultipleObjectTypeForObjectLevelSyncException(string types, Exception innerException) : base(Strings.ErrorMultipleObjectTypeForObjectLevelSync(types), innerException)
		{
			this.types = types;
		}

		protected MultipleObjectTypeForObjectLevelSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.types = (string)info.GetValue("types", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("types", this.types);
		}

		public string Types
		{
			get
			{
				return this.types;
			}
		}

		private readonly string types;
	}
}
