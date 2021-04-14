using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.Exchange.AirSync
{
	internal class DevicePasswordSetting : SettingsBase
	{
		internal DevicePasswordSetting(XmlNode request, XmlNode response, IAirSyncUser user, GlobalInfo globalInfo, ProtocolLogger protocolLogger) : base(request, response, protocolLogger)
		{
			this.user = user;
			if (globalInfo == null)
			{
				throw new ArgumentNullException("globalInfo should not be null here!");
			}
			this.globalInfo = globalInfo;
		}

		public override void Execute()
		{
			using (this.user.Context.Tracker.Start(TimeId.DevicePasswordExecute))
			{
				XmlNode firstChild = base.Request.FirstChild;
				string localName;
				if ((localName = firstChild.LocalName) != null && localName == "Set")
				{
					this.ProcessSet(firstChild);
				}
				this.ReportStatus();
			}
		}

		private void ReportStatus()
		{
			XmlNode xmlNode = base.Response.OwnerDocument.CreateElement("Status", "Settings:");
			int num = (int)this.status;
			xmlNode.InnerText = num.ToString(CultureInfo.InvariantCulture);
			base.Response.AppendChild(xmlNode);
			Command.CurrentCommand.PartialFailure = (this.status != SettingsBase.ErrorCode.Success);
		}

		private void ProcessSet(XmlNode setNode)
		{
			using (this.user.Context.Tracker.Start(TimeId.DevicePasswordProcessSet))
			{
				if (setNode.ChildNodes.Count != 1)
				{
					this.status = SettingsBase.ErrorCode.ProtocolError;
				}
				else
				{
					string innerText = setNode.FirstChild.InnerText;
					if (innerText.Length > 255)
					{
						this.status = SettingsBase.ErrorCode.InvalidArguments;
					}
					else
					{
						PolicyData policyData = ADNotificationManager.GetPolicyData(this.user);
						bool flag = policyData != null && policyData.PasswordRecoveryEnabled;
						if (innerText.Length > 0 && flag)
						{
							this.globalInfo.RecoveryPassword = innerText;
						}
						else
						{
							this.globalInfo.RecoveryPassword = null;
						}
						if (innerText.Length > 0 && !flag)
						{
							this.status = SettingsBase.ErrorCode.DeniedByPolicy;
						}
						else
						{
							this.status = SettingsBase.ErrorCode.Success;
						}
					}
				}
			}
		}

		private SettingsBase.ErrorCode status = SettingsBase.ErrorCode.ProtocolError;

		private GlobalInfo globalInfo;

		private IAirSyncUser user;
	}
}
