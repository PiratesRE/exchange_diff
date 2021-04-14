using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class LikersProperty : ComplexPropertyBase, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		public LikersProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static LikersProperty CreateCommand(CommandContext commandContext)
		{
			return new LikersProperty(commandContext);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			MessageItem messageItem = commandSettings.StoreObject as MessageItem;
			if (messageItem == null || messageItem.PropertyBag == null)
			{
				return;
			}
			Likers likers = new Likers(messageItem.PropertyBag);
			commandSettings.ServiceObject.PropertyBag[propertyInformation] = LikersProperty.ToEmailAddress(likers);
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			Likers likers = Likers.CreateInstance(propertyBag);
			if (likers != null)
			{
				serviceObject[this.commandContext.PropertyInformation] = LikersProperty.ToEmailAddress(likers);
			}
		}

		private static EmailAddressWrapper[] ToEmailAddress(IEnumerable<Participant> likers)
		{
			if (likers != null)
			{
				return likers.Select(new Func<Participant, EmailAddressWrapper>(EmailAddressWrapper.FromParticipant)).ToArray<EmailAddressWrapper>();
			}
			return null;
		}
	}
}
