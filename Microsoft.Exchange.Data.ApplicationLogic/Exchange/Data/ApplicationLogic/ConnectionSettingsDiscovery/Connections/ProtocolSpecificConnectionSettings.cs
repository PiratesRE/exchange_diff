using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using Microsoft.Exchange.Connections.Common;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections
{
	internal abstract class ProtocolSpecificConnectionSettings
	{
		protected ProtocolSpecificConnectionSettings(ConnectionSettingsType settingsType)
		{
			this.LogonResult = OperationStatusCode.None;
			this.ConnectionType = settingsType;
		}

		public ConnectionSettingsType ConnectionType { get; private set; }

		public OperationStatusCode LogonResult { get; private set; }

		public bool TestUserCanLogon(SmtpAddress email, ref string userName, SecureString password)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password", "The password argument cannot be null.");
			}
			if (!email.IsValidAddress)
			{
				throw new ArgumentException("The email argument must have a valid value.", "email");
			}
			foreach (string text in this.FindUserNamesIfNecessary(userName, email))
			{
				OperationStatusCode logonResult = this.TestUserCanLogonWithCurrentSettings(email, text, password);
				this.LogonResult = logonResult;
				if (this.LogonResult == OperationStatusCode.Success)
				{
					userName = text;
					break;
				}
				if (this.LogonResult != OperationStatusCode.ErrorInvalidCredentials)
				{
					break;
				}
			}
			return this.LogonResult == OperationStatusCode.Success;
		}

		public virtual string ToMultiLineString(string lineSeparator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("connection settings:{1}Type={0},{1}", this.ConnectionType, lineSeparator);
			stringBuilder.AppendFormat("Last logon test result={0},{1}", this.LogonResult, lineSeparator);
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return this.ToMultiLineString(" ");
		}

		protected abstract OperationStatusCode TestUserCanLogonWithCurrentSettings(SmtpAddress email, string userName, SecureString password);

		protected virtual IEnumerable<string> FindUserNamesIfNecessary(string userName, SmtpAddress email)
		{
			if (string.IsNullOrEmpty(userName))
			{
				yield return (string)email;
				yield return email.Local;
			}
			else
			{
				yield return userName;
			}
			yield break;
		}
	}
}
