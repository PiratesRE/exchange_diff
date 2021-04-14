using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	internal class QueueViewerInputPropertyBag : PropertyBag
	{
		public QueueViewerInputPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
			base.SetField(this.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		public QueueViewerInputPropertyBag() : base(false, 16)
		{
			base.SetField(this.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return QueueViewerInputObjectSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return QueueViewerInputObjectSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return QueueViewerInputObjectSchema.Identity;
			}
		}

		internal override object SerializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (!(propertyDefinition.Type == typeof(IConfigurable)) || input == null)
			{
				return base.SerializeData(propertyDefinition, input);
			}
			if (input is ExtensibleMessageInfo)
			{
				return input as ExtensibleMessageInfo;
			}
			if (input is ExtensibleQueueInfo)
			{
				return input as ExtensibleQueueInfo;
			}
			throw new NotImplementedException();
		}

		internal override object DeserializeData(ProviderPropertyDefinition propertyDefinition, object input)
		{
			if (!(propertyDefinition.Type == typeof(IConfigurable)) || input == null)
			{
				return base.DeserializeData(propertyDefinition, input);
			}
			if (input is ExtensibleMessageInfo)
			{
				return input as ExtensibleMessageInfo;
			}
			if (input is ExtensibleQueueInfo)
			{
				return input as ExtensibleQueueInfo;
			}
			throw new NotImplementedException();
		}
	}
}
