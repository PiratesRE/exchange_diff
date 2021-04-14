using System;
using System.Xml;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal interface IPAAParser
	{
		void SerializeTo(PersonalAutoAttendant paa, XmlWriter writer);

		PersonalAutoAttendant DeserializeFrom(XmlNode node);
	}
}
