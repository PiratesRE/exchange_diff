using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters
{
	internal class ParticipantRoutingTypeConverter : IParticipantRoutingTypeConverter
	{
		public ParticipantRoutingTypeConverter(IStoreSession session)
		{
			this.session = session.AssertNotNull("session");
		}

		public ConvertValue<Participant[], Participant[]> ConvertToEntity
		{
			get
			{
				return (Participant[] value) => this.Convert(value, "SMTP");
			}
		}

		public ConvertValue<Participant[], Participant[]> ConvertToStorage
		{
			get
			{
				return (Participant[] value) => this.Convert(value, "EX");
			}
		}

		protected virtual IStoreSession Session
		{
			get
			{
				return this.session;
			}
		}

		public static Participant[] ConvertToSmtp(Participant[] participants, IMailboxSession mailboxSession)
		{
			Dictionary<string, Participant> dictionary = new Dictionary<string, Participant>();
			List<Participant> list = new List<Participant>();
			foreach (Participant participant in participants)
			{
				if (!dictionary.ContainsKey(participant.EmailAddress))
				{
					if (string.Equals(participant.RoutingType, "SMTP", StringComparison.OrdinalIgnoreCase))
					{
						dictionary.Add(participant.EmailAddress, participant);
					}
					else
					{
						string valueOrDefault = participant.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress);
						if (!string.IsNullOrEmpty(valueOrDefault))
						{
							Participant value = new Participant(participant.DisplayName, valueOrDefault, "SMTP", participant.Origin, new KeyValuePair<PropertyDefinition, object>[0]);
							dictionary.Add(participant.EmailAddress, value);
						}
						else
						{
							list.Add(participant);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				Participant[] array = list.ToArray();
				Participant[] array2 = ParticipantRoutingTypeConverter.ResolveParticipantsFromAD(array, "SMTP", mailboxSession);
				for (int j = 0; j < array.Length; j++)
				{
					if (!dictionary.ContainsKey(array[j].EmailAddress))
					{
						dictionary.Add(array[j].EmailAddress, array2[j] ?? array[j]);
					}
				}
			}
			Participant[] array3 = new Participant[participants.Length];
			for (int k = 0; k < participants.Length; k++)
			{
				array3[k] = dictionary[participants[k].EmailAddress];
			}
			return array3;
		}

		protected virtual Participant[] Convert(Participant[] value, string destinationRoutingType)
		{
			IMailboxSession mailboxSession = this.Session as IMailboxSession;
			if (mailboxSession == null)
			{
				ExTraceGlobals.ConvertersTracer.TraceDebug<string>(0L, "Provided session ({0}) is not supported for participant conversion.", this.Session.GetType().Name);
				return value;
			}
			if (string.Equals(destinationRoutingType, "SMTP", StringComparison.OrdinalIgnoreCase))
			{
				return ParticipantRoutingTypeConverter.ConvertToSmtp(value, mailboxSession);
			}
			Participant[] array = ParticipantRoutingTypeConverter.ResolveParticipantsFromAD(value, destinationRoutingType, mailboxSession);
			Participant[] array2 = new Participant[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (array[i] ?? value[i]);
			}
			return array2;
		}

		private static Participant[] ResolveParticipantsFromAD(Participant[] participants, string destinationRoutingType, IMailboxSession mailboxSession)
		{
			ADObjectId searchRoot = null;
			IExchangePrincipal mailboxOwner = mailboxSession.MailboxOwner;
			if (mailboxOwner.MailboxInfo.Configuration.AddressBookPolicy != null)
			{
				searchRoot = DirectoryHelper.GetGlobalAddressListFromAddressBookPolicy(mailboxOwner.MailboxInfo.Configuration.AddressBookPolicy, mailboxSession.GetADConfigurationSession(true, ConsistencyMode.IgnoreInvalid));
			}
			Participant[] array = Participant.TryConvertTo(participants, destinationRoutingType, mailboxOwner, searchRoot);
			if (array == null)
			{
				MailboxSession mailboxSession2 = mailboxSession as MailboxSession;
				array = Participant.TryConvertTo(participants, destinationRoutingType, mailboxSession2);
			}
			if (array == null)
			{
				ExTraceGlobals.ConvertersTracer.TraceDebug<string>(0L, "Provided session ({0}) does not support participant conversion.", mailboxSession.GetType().Name);
				return participants;
			}
			return array;
		}

		public const string EntitiesRouting = "SMTP";

		public const string StorageRouting = "EX";

		private readonly IStoreSession session;

		public class PassThru : IParticipantRoutingTypeConverter
		{
			private PassThru()
			{
			}

			public static ParticipantRoutingTypeConverter.PassThru SingletonInstance
			{
				get
				{
					return ParticipantRoutingTypeConverter.PassThru.Instance;
				}
			}

			public ConvertValue<Participant[], Participant[]> ConvertToEntity
			{
				get
				{
					return (Participant[] value) => value;
				}
			}

			public ConvertValue<Participant[], Participant[]> ConvertToStorage
			{
				get
				{
					return (Participant[] value) => value;
				}
			}

			private static readonly ParticipantRoutingTypeConverter.PassThru Instance = new ParticipantRoutingTypeConverter.PassThru();
		}
	}
}
