using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ReplyToProperty : ComplexPropertyBase, IPregatherParticipants, IToXmlCommand, IToServiceObjectCommand, ISetCommand, IAppendUpdateCommand, IDeleteUpdateCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		public ReplyToProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static ReplyToProperty CreateCommand(CommandContext commandContext)
		{
			return new ReplyToProperty(commandContext);
		}

		void IPregatherParticipants.Pregather(StoreObject storeObject, List<Participant> participants)
		{
			MessageItem messageItem = storeObject as MessageItem;
			if (messageItem != null)
			{
				try
				{
					foreach (Participant item in messageItem.ReplyTo)
					{
						participants.Add(item);
					}
				}
				catch (PropertyErrorException innerException)
				{
					throw new PropertyRequestFailedException(CoreResources.IDs.ErrorItemPropertyRequestFailed, this.commandContext.PropertyInformation.PropertyPath, innerException);
				}
			}
		}

		public void ToXml()
		{
			throw new InvalidOperationException("ReplyToProperty.ToXml should not be called");
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			this.SetProperty(serviceObject, storeObject, false);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			this.SetProperty(setPropertyUpdate.ServiceObject, storeObject, false);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			MessageItem messageItem = updateCommandSettings.StoreObject as MessageItem;
			if (messageItem == null)
			{
				throw new InvalidPropertyRequestException(this.commandContext.PropertyInformation.PropertyPath);
			}
			messageItem.ReplyTo.Clear();
		}

		public override void AppendUpdate(AppendPropertyUpdate appendPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			this.SetProperty(appendPropertyUpdate.ServiceObject, storeObject, true);
		}

		private void SetProperty(ServiceObject serviceObject, StoreObject storeObject, bool isAppend)
		{
			MessageItem messageItem = storeObject as MessageItem;
			if (messageItem == null)
			{
				throw new InvalidPropertyRequestException(this.commandContext.PropertyInformation.PropertyPath);
			}
			if (!isAppend)
			{
				messageItem.ReplyTo.Clear();
			}
			EmailAddressWrapper[] valueOrDefault = serviceObject.GetValueOrDefault<EmailAddressWrapper[]>(this.commandContext.PropertyInformation);
			if (valueOrDefault != null)
			{
				foreach (EmailAddressWrapper emailAddressWrapper in valueOrDefault)
				{
					if (emailAddressWrapper != null)
					{
						messageItem.ReplyTo.Add(this.GetParticipantFromAddress(messageItem, emailAddressWrapper));
					}
				}
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			MessageItem messageItem = storeObject as MessageItem;
			if (messageItem != null)
			{
				IList<Participant> replyTo = messageItem.ReplyTo;
				List<EmailAddressWrapper> list = ReplyToProperty.Render(replyTo);
				serviceObject[propertyInformation] = ((list.Count > 0) ? list.ToArray() : null);
			}
		}

		internal static List<EmailAddressWrapper> Render(IList<Participant> replyToProperty)
		{
			return ReplyToProperty.Render(replyToProperty.Cast<IParticipant>().ToList<IParticipant>());
		}

		internal static List<EmailAddressWrapper> Render(IList<IParticipant> replyToProperty)
		{
			List<EmailAddressWrapper> list = new List<EmailAddressWrapper>();
			ParticipantInformationDictionary participantInformation = EWSSettings.ParticipantInformation;
			foreach (IParticipant participant in replyToProperty)
			{
				ParticipantInformation participantInformationOrCreateNew = participantInformation.GetParticipantInformationOrCreateNew(participant);
				list.Add(EmailAddressWrapper.FromParticipantInformation(participantInformationOrCreateNew));
			}
			return list;
		}
	}
}
