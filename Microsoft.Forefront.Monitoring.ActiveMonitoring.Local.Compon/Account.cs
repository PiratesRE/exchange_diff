using System;
using System.Xml;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class Account
	{
		public Account(string username, string password)
		{
			this.username = username;
			this.password = password;
		}

		public string Username
		{
			get
			{
				return this.username;
			}
		}

		public string Password
		{
			get
			{
				return this.password;
			}
		}

		public static Account FromXml(XmlNode workContext)
		{
			XmlElement xmlElement = workContext as XmlElement;
			string text = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Username"), "Username");
			string attribute = xmlElement.GetAttribute("Password");
			return new Account(text, attribute);
		}

		private readonly string username;

		private readonly string password;
	}
}
