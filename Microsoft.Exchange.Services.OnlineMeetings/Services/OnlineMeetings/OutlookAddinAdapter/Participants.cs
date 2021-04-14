using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.OutlookAddinAdapter
{
	[XmlType("Participants")]
	[DataContract(Name = "Participants")]
	[KnownType(typeof(Participants))]
	public class Participants
	{
		[XmlArray("Attendees")]
		[XmlArrayItem("User")]
		[DataMember(Name = "Attendees", EmitDefaultValue = true)]
		public User[] Attendees { get; set; }

		[XmlArray("Presenters")]
		[XmlArrayItem("User")]
		[DataMember(Name = "Presenters", EmitDefaultValue = true)]
		public User[] Presenters { get; set; }

		internal static Participants ConvertFrom(IEnumerable<string> attendees, IEnumerable<string> leaders)
		{
			Participants participants = new Participants();
			Collection<User> collection = new Collection<User>();
			foreach (string attendee in attendees)
			{
				collection.Add(Participants.CreateUserFromSmtpAddress(attendee));
			}
			participants.Attendees = collection.ToArray<User>();
			Collection<User> collection2 = new Collection<User>();
			foreach (string attendee2 in leaders)
			{
				collection2.Add(Participants.CreateUserFromSmtpAddress(attendee2));
			}
			participants.Presenters = collection2.ToArray<User>();
			return participants;
		}

		private static User CreateUserFromSmtpAddress(string attendee)
		{
			return new User
			{
				Name = attendee,
				SmtpAddress = attendee,
				SipAddress = attendee
			};
		}
	}
}
