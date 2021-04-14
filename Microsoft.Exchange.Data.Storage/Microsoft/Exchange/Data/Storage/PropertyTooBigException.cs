using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class PropertyTooBigException : CorruptDataException
	{
		public PropertyTooBigException(PropertyDefinition propertyDefinition) : base(ServerStrings.ExPropertyRequiresStreaming(propertyDefinition))
		{
			this.propertyDefinition = propertyDefinition;
		}

		protected PropertyTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyDefinition = (PropertyDefinition)info.GetValue("propertyDefinition", typeof(PropertyDefinition));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyDefinition", this.propertyDefinition);
		}

		public PropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
		}

		private const string PropertyDefinitionLabel = "propertyDefinition";

		private PropertyDefinition propertyDefinition;
	}
}
