using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Properties.XSO;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EmailProperty : ContactProperty<string>
	{
		public EmailProperty(IXSOPropertyManager propertyManager, PropertyDefinition writePropertyDefinition, PropertyDefinition readPropertyDefinition) : base(propertyManager, new PropertyDefinition[]
		{
			writePropertyDefinition,
			readPropertyDefinition
		})
		{
			this.writePropertyDefinition = writePropertyDefinition;
			this.readPropertyDefinition = readPropertyDefinition;
		}

		public override string ReadProperty(Item item)
		{
			SyncUtilities.ThrowIfArgumentNull("item", item);
			return SyncUtilities.SafeGetProperty<string>(item, this.readPropertyDefinition);
		}

		public override void WriteProperty(Item item, string desiredValue)
		{
			SyncUtilities.ThrowIfArgumentNull("item", item);
			if (desiredValue != null)
			{
				Participant participant = null;
				if (!base.IsItemNew(item))
				{
					participant = SyncUtilities.SafeGetProperty<Participant>(item, this.writePropertyDefinition);
				}
				Participant value;
				if (participant == null)
				{
					value = new Participant(null, desiredValue, "SMTP");
				}
				else
				{
					value = new Participant(participant.DisplayName, desiredValue, participant.RoutingType);
				}
				item[this.writePropertyDefinition] = value;
			}
		}

		private readonly PropertyDefinition writePropertyDefinition;

		private readonly PropertyDefinition readPropertyDefinition;
	}
}
