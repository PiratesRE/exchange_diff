using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureHubPayloadWriter
	{
		public void AddAuthorizationRule(AzureSasKey sasKey)
		{
			ArgumentValidator.ThrowIfNull("sasKey", sasKey);
			ArgumentValidator.ThrowIfNull("sasKey.Claims", sasKey.Claims);
			this.sasBuilder.AppendFormat("<AuthorizationRule i:type=\"SharedAccessAuthorizationRule\">\r\n            <ClaimType>SharedAccessKey</ClaimType>\r\n            <ClaimValue>None</ClaimValue>\r\n            <Rights>\r\n                {0}\r\n            </Rights>\r\n            <KeyName>{1}</KeyName>\r\n            <PrimaryKey>{2}</PrimaryKey>\r\n        </AuthorizationRule>", this.GetAccessRightXML(sasKey.Claims), sasKey.KeyName, sasKey.KeyValue.AsUnsecureString());
		}

		public override string ToString()
		{
			return string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<entry xmlns=\"http://www.w3.org/2005/Atom\">\r\n    <content type=\"application/xml\">\r\n        <NotificationHubDescription xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n        <RegistrationTtl>P7D</RegistrationTtl>\r\n        <AuthorizationRules>\r\n        {0}\r\n        </AuthorizationRules>\r\n        </NotificationHubDescription>\r\n    </content>\r\n</entry>", this.sasBuilder.ToString());
		}

		private string GetAccessRightXML(AzureSasKey.ClaimType claims)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if ((claims & AzureSasKey.ClaimType.Listen) == AzureSasKey.ClaimType.Listen)
			{
				stringBuilder.AppendFormat("<AccessRights>{0}</AccessRights>", AzureSasKey.ClaimType.Listen.ToString());
			}
			if ((claims & AzureSasKey.ClaimType.Send) == AzureSasKey.ClaimType.Send)
			{
				stringBuilder.AppendFormat("<AccessRights>{0}</AccessRights>", AzureSasKey.ClaimType.Send.ToString());
			}
			if ((claims & AzureSasKey.ClaimType.Manage) == AzureSasKey.ClaimType.Manage)
			{
				stringBuilder.AppendFormat("<AccessRights>{0}</AccessRights>", AzureSasKey.ClaimType.Manage.ToString());
			}
			return stringBuilder.ToString();
		}

		public const string CreateHubXMLTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<entry xmlns=\"http://www.w3.org/2005/Atom\">\r\n    <content type=\"application/xml\">\r\n        <NotificationHubDescription xmlns=\"http://schemas.microsoft.com/netservices/2010/10/servicebus/connect\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n        <RegistrationTtl>P7D</RegistrationTtl>\r\n        <AuthorizationRules>\r\n        {0}\r\n        </AuthorizationRules>\r\n        </NotificationHubDescription>\r\n    </content>\r\n</entry>";

		public const string AuthorizationRuleXMLTemplate = "<AuthorizationRule i:type=\"SharedAccessAuthorizationRule\">\r\n            <ClaimType>SharedAccessKey</ClaimType>\r\n            <ClaimValue>None</ClaimValue>\r\n            <Rights>\r\n                {0}\r\n            </Rights>\r\n            <KeyName>{1}</KeyName>\r\n            <PrimaryKey>{2}</PrimaryKey>\r\n        </AuthorizationRule>";

		private const string AccessRightsXMLTemplate = "<AccessRights>{0}</AccessRights>";

		private readonly StringBuilder sasBuilder = new StringBuilder();
	}
}
