using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class NotInBagPropertyErrorException : ExInvalidOperationException
	{
		public NotInBagPropertyErrorException(PropertyDefinition propertyDefinition) : base(new LocalizedString(propertyDefinition.ToString()))
		{
			this.propertyDefinition = propertyDefinition;
		}

		protected NotInBagPropertyErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyDefinition = (PropertyDefinition)info.GetValue("propertyDefinition", typeof(PropertyDefinition));
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyDefinition", this.propertyDefinition);
		}

		private const string PropertyDefinitionLabel = "propertyDefinition";

		private readonly PropertyDefinition propertyDefinition;
	}
}
