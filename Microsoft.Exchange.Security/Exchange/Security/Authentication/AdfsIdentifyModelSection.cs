using System;
using System.Xml;
using Microsoft.IdentityModel.Configuration;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class AdfsIdentifyModelSection : MicrosoftIdentityModelSection
	{
		internal void Deserialize(XmlReader reader)
		{
			this.DeserializeElement(reader, false);
		}
	}
}
