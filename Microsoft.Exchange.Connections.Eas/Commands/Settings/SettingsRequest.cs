using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.Settings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Settings
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlRoot(ElementName = "Settings", Namespace = "Settings", IsNullable = false)]
	public class SettingsRequest : Settings
	{
		[XmlIgnore]
		internal static readonly SettingsRequest Default = new SettingsRequest
		{
			UserInformation = new UserInformation
			{
				Get = true
			}
		};
	}
}
