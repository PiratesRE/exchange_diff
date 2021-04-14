using System;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class DetailsTemplatesSerializationService : IDesignerSerializationService
	{
		public DetailsTemplatesSerializationService(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public ICollection Deserialize(object serializationData)
		{
			SerializationStore serializationStore = serializationData as SerializationStore;
			if (serializationStore != null)
			{
				ComponentSerializationService componentSerializationService = this.serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
				return componentSerializationService.Deserialize(serializationStore);
			}
			return new object[0];
		}

		public object Serialize(ICollection objects)
		{
			ComponentSerializationService componentSerializationService = this.serviceProvider.GetService(typeof(ComponentSerializationService)) as ComponentSerializationService;
			SerializationStore result = null;
			using (SerializationStore serializationStore = componentSerializationService.CreateStore())
			{
				foreach (object obj in objects)
				{
					if (obj is Control)
					{
						componentSerializationService.Serialize(serializationStore, obj);
					}
				}
				result = serializationStore;
			}
			return result;
		}

		private IServiceProvider serviceProvider;
	}
}
