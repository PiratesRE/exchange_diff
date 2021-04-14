using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class SettingsCommand : Command
	{
		public SettingsCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfSettingsRequests;
		}

		internal override int MinVersion
		{
			get
			{
				return 120;
			}
		}

		protected override string RootNodeName
		{
			get
			{
				return "Settings";
			}
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			Command.ExecutionState result;
			using (base.Context.Tracker.Start(TimeId.SettingsExecuteCommand))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Settings command received. Processing request...");
				this.InitializeResponseXmlDocument();
				this.ReadXmlRequest();
				foreach (SettingsBase settingsBase in this.properties)
				{
					settingsBase.Execute();
				}
				this.FinalizeResponseXmlDocument();
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Settings command finished processing.");
				result = Command.ExecutionState.Complete;
			}
			return result;
		}

		protected override bool HandleQuarantinedState()
		{
			return true;
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			XmlDocument result;
			using (base.Context.Tracker.Start(TimeId.SettingsGetValidationErrorXml))
			{
				if (SettingsCommand.validationErrorXml == null)
				{
					XmlDocument commandXmlStub = base.GetCommandXmlStub();
					XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
					xmlElement.InnerText = XmlConvert.ToString(2);
					commandXmlStub[this.RootNodeName].AppendChild(xmlElement);
					SettingsCommand.validationErrorXml = commandXmlStub;
				}
				result = SettingsCommand.validationErrorXml;
			}
			return result;
		}

		private void ReadXmlRequest()
		{
			using (base.Context.Tracker.Start(TimeId.SettingsReadXmlRequest))
			{
				XmlNode xmlRequest = base.XmlRequest;
				foreach (object obj in xmlRequest.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNode response = base.XmlResponse.CreateElement(xmlNode.LocalName, "Settings:");
					string localName;
					switch (localName = xmlNode.LocalName)
					{
					case "Oof":
					{
						SettingsBase item = new OofSetting(xmlNode, response, base.MailboxSession, base.CurrentAccessState, base.ProtocolLogger);
						this.properties.Add(item);
						break;
					}
					case "DevicePassword":
					{
						SettingsBase item = new DevicePasswordSetting(xmlNode, response, base.User, base.GlobalInfo, base.ProtocolLogger);
						this.properties.Add(item);
						break;
					}
					case "DeviceInformation":
					{
						SettingsBase item = new DeviceInformationSetting(xmlNode, response, this, base.ProtocolLogger);
						this.properties.Add(item);
						break;
					}
					case "UserInformation":
					{
						SettingsBase item = new UserInformationSetting(xmlNode, response, base.User, base.MailboxSession, base.Version, base.ProtocolLogger);
						this.properties.Add(item);
						break;
					}
					case "RightsManagementInformation":
					{
						SettingsBase item = new RightsManagementInformationSetting(xmlNode, response, base.User, base.MailboxSession.PreferedCulture, base.ProtocolLogger, base.MailboxLogger);
						this.properties.Add(item);
						break;
					}
					case "TimeZoneOffsets":
					{
						SettingsBase item = new TimeZoneOffsetSettings(xmlNode, response, base.User, base.ProtocolLogger);
						this.properties.Add(item);
						break;
					}
					}
				}
			}
		}

		private void InitializeResponseXmlDocument()
		{
			using (base.Context.Tracker.Start(TimeId.SettingsInitializeResponseXmlDocument))
			{
				base.XmlResponse = new SafeXmlDocument();
				this.settingsNode = base.XmlResponse.CreateElement("Settings", "Settings:");
				base.XmlResponse.AppendChild(this.settingsNode);
			}
		}

		private void FinalizeResponseXmlDocument()
		{
			using (base.Context.Tracker.Start(TimeId.SettingsFinalizeResponseXmlDocument))
			{
				XmlElement xmlElement = base.XmlResponse.CreateElement("Status", "Settings:");
				xmlElement.InnerText = "1";
				this.settingsNode.AppendChild(xmlElement);
				foreach (SettingsBase settingsBase in this.properties)
				{
					XmlNode response = settingsBase.Response;
					this.settingsNode.AppendChild(response);
				}
			}
		}

		private static XmlDocument validationErrorXml;

		private List<SettingsBase> properties = new List<SettingsBase>();

		private XmlNode settingsNode;
	}
}
