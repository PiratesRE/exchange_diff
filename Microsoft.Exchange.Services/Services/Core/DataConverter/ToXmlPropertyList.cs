using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ToXmlPropertyList : ToXmlPropertyListBase
	{
		private ToXmlPropertyList()
		{
		}

		public ToXmlPropertyList(Shape shape, ResponseShape responseShape) : base(shape, responseShape)
		{
		}

		public char[] CharBuffer
		{
			set
			{
				this.charBuffer = value;
			}
		}

		protected override ToXmlCommandSettingsBase GetCommandSettings()
		{
			return new ToXmlCommandSettings();
		}

		protected override ToXmlCommandSettingsBase GetCommandSettings(PropertyPath propertyPath)
		{
			return new ToXmlCommandSettings(propertyPath);
		}

		protected override bool ValidateProperty(PropertyInformation propertyInformation, bool returnErrorForInvalidProperty)
		{
			bool implementsToXmlCommand = propertyInformation.ImplementsToXmlCommand;
			if (!implementsToXmlCommand && returnErrorForInvalidProperty)
			{
				throw new InvalidPropertyForOperationException(propertyInformation.PropertyPath);
			}
			return implementsToXmlCommand;
		}

		internal bool IgnoreCorruptPropertiesWhenRendering
		{
			get
			{
				return this.ignoreCorruptPropertiesWhenRendering;
			}
			set
			{
				this.ignoreCorruptPropertiesWhenRendering = value;
			}
		}

		private void CheckAndAddParticipantToConvert(List<Participant> participantsToConvert, Participant participantToCheckAndAdd)
		{
			bool flag = false;
			for (int i = 0; i < participantsToConvert.Count; i++)
			{
				if (participantsToConvert[i] == participantToCheckAndAdd)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<string, int>((long)this.GetHashCode(), "ToXmlPropertyList.CheckAndAddParticipantToConvert - converting participant with EmailAddress = {0} and HashCode = {1}.", participantToCheckAndAdd.EmailAddress, participantToCheckAndAdd.GetHashCode());
				participantsToConvert.Add(participantToCheckAndAdd);
			}
		}

		private void ConvertAndGetParticipantInformation(List<Participant> participants)
		{
			if (participants.Count == 0)
			{
				return;
			}
			List<Participant> list = new List<Participant>(participants.Count);
			ParticipantInformationDictionary participantInformation = EWSSettings.ParticipantInformation;
			foreach (Participant participant in participants)
			{
				ParticipantInformation participantInformation2 = null;
				if (participant == null)
				{
					ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug((long)this.GetHashCode(), "ToXmlPropertyList.ConvertAndGetParticipantInformation - found null entry");
				}
				else if (participantInformation.ContainsParticipant(participant))
				{
					ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<string, int>((long)this.GetHashCode(), "ToXmlPropertyList.ConvertAndGetParticipantInformation - using already resolved participant for EmailAddress = {0}, HashCode = {1}", participant.EmailAddress, participant.GetHashCode());
				}
				else
				{
					if (string.Equals(participant.RoutingType, "EX", StringComparison.OrdinalIgnoreCase))
					{
						this.CheckAndAddParticipantToConvert(list, participant);
					}
					else
					{
						string valueOrDefault = participant.GetValueOrDefault<string>(ParticipantSchema.SipUri, null);
						participantInformation2 = new ParticipantInformation(participant.DisplayName, participant.RoutingType, participant.EmailAddress, participant.Origin, null, valueOrDefault, new bool?(participant.Submitted));
					}
					if (participantInformation2 != null)
					{
						participantInformation.AddParticipant(participant, participantInformation2);
					}
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			ExTraceGlobals.ParticipantLookupBatchingTracer.TraceDebug<int>((long)this.GetHashCode(), "ToXmlPropertyList.ConvertAndGetParticipantInformation - now calling TryConvertTo on {0} participants", list.Count);
			Participant[] array = MailboxHelper.TryConvertTo(list.ToArray(), "SMTP");
			int num = 0;
			foreach (Participant participant2 in array)
			{
				Participant participant3;
				bool value;
				if (participant2 == null || participant2.EmailAddress == null)
				{
					participant3 = list[num];
					value = true;
				}
				else
				{
					participant3 = participant2;
					value = false;
				}
				string valueOrDefault2 = participant3.GetValueOrDefault<string>(ParticipantSchema.SipUri, null);
				ParticipantInformation participantInformation3 = new ParticipantInformation(participant3.DisplayName, participant3.RoutingType, participant3.EmailAddress, participant3.Origin, new bool?(value), valueOrDefault2, new bool?(participant3.Submitted));
				participantInformation.AddParticipant(list[num], participantInformation3);
				num++;
			}
		}

		public IList<IToXmlCommand> CreatePropertyCommands(IdAndSession idAndSession, StoreObject storeObject, XmlElement serviceItem)
		{
			List<IToXmlCommand> list = new List<IToXmlCommand>();
			foreach (CommandContext commandContext in this.commandContextsOrdered)
			{
				ToXmlCommandSettings toXmlCommandSettings = (ToXmlCommandSettings)commandContext.CommandSettings;
				toXmlCommandSettings.IdAndSession = idAndSession;
				toXmlCommandSettings.StoreObject = storeObject;
				toXmlCommandSettings.ServiceItem = serviceItem;
				toXmlCommandSettings.ResponseShape = this.responseShape;
				list.Add((IToXmlCommand)commandContext.PropertyInformation.CreatePropertyCommand(commandContext));
			}
			return list;
		}

		private void ConvertStoreObjectPropertiesToXml(XmlElement serviceItem, IdAndSession idAndSession, StoreObject storeObject)
		{
			IList<IToXmlCommand> list = this.CreatePropertyCommands(idAndSession, storeObject, serviceItem);
			List<Participant> participants = new List<Participant>();
			foreach (IToXmlCommand toXmlCommand in list)
			{
				IPregatherParticipants pregatherParticipants = toXmlCommand as IPregatherParticipants;
				if (pregatherParticipants != null)
				{
					pregatherParticipants.Pregather(storeObject, participants);
				}
			}
			this.ConvertAndGetParticipantInformation(participants);
			foreach (IToXmlCommand toXmlCommand2 in list)
			{
				IRequireCharBuffer requireCharBuffer = toXmlCommand2 as IRequireCharBuffer;
				if (requireCharBuffer != null)
				{
					requireCharBuffer.CharBuffer = this.charBuffer;
				}
				try
				{
					toXmlCommand2.ToXml();
				}
				catch (PropertyRequestFailedException ex)
				{
					if (!this.IgnoreCorruptPropertiesWhenRendering)
					{
						throw;
					}
					if (ExTraceGlobals.ServiceCommandBaseDataTracer.IsTraceEnabled(TraceType.ErrorTrace))
					{
						ExTraceGlobals.ServiceCommandBaseDataTracer.TraceError<string, string>((long)this.GetHashCode(), "[ToXmlPropertyList::ConvertStoreObjectPropertiesToXml] Encountered PropertyRequestFailedException.  Message: '{0}'. Data: {1} IgnoreCorruptPropertiesWhenRendering is true, so processing will continue.", ex.Message, ((IProvidePropertyPaths)ex).GetPropertyPathsString());
					}
				}
			}
		}

		public XmlElement ConvertStoreObjectPropertiesToXml(IdAndSession idAndSession, StoreObject storeObject, XmlDocument parentDocument)
		{
			XmlElement xmlElement = base.CreateItemXmlElement(parentDocument);
			this.ConvertStoreObjectPropertiesToXml(xmlElement, idAndSession, storeObject);
			return xmlElement;
		}

		public XmlElement ConvertStoreObjectPropertiesToXml(IdAndSession idAndSession, StoreObject storeObject, XmlElement parentElement)
		{
			XmlElement xmlElement = base.CreateItemXmlElement(parentElement);
			this.ConvertStoreObjectPropertiesToXml(xmlElement, idAndSession, storeObject);
			return xmlElement;
		}

		private bool ignoreCorruptPropertiesWhenRendering;

		public static ToXmlPropertyList Empty = new ToXmlPropertyList();

		private char[] charBuffer;
	}
}
