using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.AgentTasks
{
	[Serializable]
	public class TransportEvent : ConfigurableObject
	{
		internal TransportEvent(string eventTopic, IEnumerable<string> transportAgentIdentities) : base(new SimpleProviderPropertyBag())
		{
			this.Event = eventTopic;
			this.TransportAgents = transportAgentIdentities;
		}

		public string Event
		{
			get
			{
				return (string)this.propertyBag[TransportEventSchema.EventTopic];
			}
			private set
			{
				this.propertyBag[TransportEventSchema.EventTopic] = value;
			}
		}

		public IEnumerable<string> TransportAgents
		{
			get
			{
				return (MultiValuedProperty<string>)this.propertyBag[TransportEventSchema.TransportAgentIdentities];
			}
			private set
			{
				this.propertyBag[TransportEventSchema.TransportAgentIdentities] = new MultiValuedProperty<string>(new List<string>(value));
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TransportEvent.schema;
			}
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<TransportEventSchema>();
	}
}
