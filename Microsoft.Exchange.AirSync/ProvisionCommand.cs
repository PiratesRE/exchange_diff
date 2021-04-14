using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class ProvisionCommand : Command, IProvisionCommandHost
	{
		public ProvisionCommand()
		{
			base.PerfCounter = AirSyncCounters.NumberOfProvisionRequests;
		}

		internal override bool RequiresPolicyCheck
		{
			get
			{
				return false;
			}
		}

		internal override bool ShouldOpenGlobalSyncState
		{
			get
			{
				return true;
			}
		}

		protected override string RootNodeName
		{
			get
			{
				return "Provision";
			}
		}

		internal override XmlDocument GetValidationErrorXml()
		{
			if (ProvisionCommand.validationErrorXml == null)
			{
				XmlDocument commandXmlStub = base.GetCommandXmlStub();
				XmlElement xmlElement = commandXmlStub.CreateElement("Status", this.RootNodeNamespace);
				xmlElement.InnerText = XmlConvert.ToString(2);
				commandXmlStub[this.RootNodeName].AppendChild(xmlElement);
				ProvisionCommand.validationErrorXml = commandXmlStub;
			}
			return ProvisionCommand.validationErrorXml;
		}

		internal override Command.ExecutionState ExecuteCommand()
		{
			base.XmlResponse = base.GetCommandXmlStub();
			XmlNode provisionResponseNode = base.XmlResponse[this.RootNodeName];
			switch (ProvisionCommandPhaseBase.DetermineCallPhase(base.XmlRequest))
			{
			case ProvisionCommandPhaseBase.ProvisionPhase.PhaseOne:
			{
				ProvisionCommandPhaseOne provisionCommandPhaseOne = new ProvisionCommandPhaseOne(this);
				provisionCommandPhaseOne.Process(provisionResponseNode);
				break;
			}
			case ProvisionCommandPhaseBase.ProvisionPhase.PhaseTwo:
			{
				ProvisionCommandPhaseTwo provisionCommandPhaseTwo = new ProvisionCommandPhaseTwo(this);
				provisionCommandPhaseTwo.Process(provisionResponseNode);
				break;
			}
			default:
				throw new AirSyncPermanentException(StatusCode.Sync_ProtocolVersionMismatch, false)
				{
					ErrorStringForProtocolLogger = "Provision_InvalidCallType"
				};
			}
			return Command.ExecutionState.Complete;
		}

		protected override bool HandleQuarantinedState()
		{
			return true;
		}

		XmlNode IProvisionCommandHost.XmlRequest
		{
			get
			{
				return base.XmlRequest;
			}
		}

		XmlDocument IProvisionCommandHost.XmlResponse
		{
			get
			{
				return base.XmlResponse;
			}
		}

		ProtocolLogger IProvisionCommandHost.ProtocolLogger
		{
			get
			{
				return base.ProtocolLogger;
			}
		}

		uint? IProvisionCommandHost.HeaderPolicyKey
		{
			get
			{
				return base.Context.Request.PolicyKey;
			}
		}

		int IProvisionCommandHost.ProtocolVersion
		{
			get
			{
				return base.Version;
			}
		}

		IPolicyData IProvisionCommandHost.PolicyData
		{
			get
			{
				return ADNotificationManager.GetPolicyData(base.User);
			}
		}

		IGlobalInfo IProvisionCommandHost.GlobalInfo
		{
			get
			{
				return base.GlobalInfo;
			}
		}

		void IProvisionCommandHost.SetErrorResponse(HttpStatusCode httpStatusCode, StatusCode easStatusCode)
		{
			base.Context.Response.SetErrorResponse(httpStatusCode, easStatusCode);
		}

		void IProvisionCommandHost.SendRemoteWipeConfirmationMessage(ExDateTime wipeAckTime)
		{
			ProvisionCommand.SendRemoteWipeConfirmationMessage(base.GlobalInfo.RemoteWipeConfirmationAddresses, wipeAckTime, base.MailboxSession, base.Context.Request.DeviceIdentity, this);
		}

		void IProvisionCommandHost.ResetMobileServiceSelector()
		{
			DeviceInfo.ResetMobileServiceSelector(base.MailboxSession, base.SyncStateStorage);
		}

		void IProvisionCommandHost.ProcessDeviceInformationSettings(XmlNode inboundDeviceInformationNode, XmlNode provisionResponseNode)
		{
			if (inboundDeviceInformationNode != null)
			{
				XmlNode xmlNode = base.XmlResponse.CreateElement(inboundDeviceInformationNode.LocalName, "Settings:");
				DeviceInformationSetting deviceInformationSetting = new DeviceInformationSetting(inboundDeviceInformationNode, xmlNode, this, base.ProtocolLogger);
				deviceInformationSetting.Execute();
				if (string.IsNullOrEmpty(base.GlobalInfo.DeviceModel))
				{
					throw new AirSyncPermanentException(StatusCode.DeviceInformationRequired, false)
					{
						ErrorStringForProtocolLogger = "DeviceModelMissingInDeviceInformation"
					};
				}
				provisionResponseNode.AppendChild(xmlNode);
			}
		}

		public static void SendRemoteWipeConfirmationMessage(string[] addresses, ExDateTime wipeAckTime, MailboxSession mailboxSession, DeviceIdentity deviceIdentity, object traceObject)
		{
			bool flag = false;
			MessageItem messageItem = null;
			CultureInfo preferedCulture = mailboxSession.PreferedCulture;
			try
			{
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts);
				messageItem = MessageItem.Create(mailboxSession, defaultFolderId);
				messageItem.ClassName = "IPM.Note.Exchange.ActiveSync.RemoteWipeConfirmation";
				messageItem.Subject = Strings.RemoteWipeConfirmationMessageSubject.ToString(preferedCulture);
				string format = (addresses == null) ? Strings.RemoteWipeConfirmationMessageBody1Owa.ToString(preferedCulture) : Strings.RemoteWipeConfirmationMessageBody1Task.ToString(preferedCulture);
				string text = AirSyncUtility.HtmlEncode(string.Format(CultureInfo.InvariantCulture, format, new object[]
				{
					wipeAckTime
				}), false);
				string text2 = AirSyncUtility.HtmlEncode(string.Format(CultureInfo.InvariantCulture, Strings.DeviceType.ToString(preferedCulture), new object[]
				{
					deviceIdentity.DeviceType
				}), false);
				string text3 = AirSyncUtility.HtmlEncode(string.Format(CultureInfo.InvariantCulture, Strings.DeviceId.ToString(preferedCulture), new object[]
				{
					deviceIdentity.DeviceId
				}), false);
				using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextHtml))
				{
					textWriter.Write("\r\n            <html>\r\n                <style>\r\n                    {0}\r\n                </style>\r\n                <body>\r\n                    <h1>{1}</h1>\r\n                    <br><br>\r\n                    <p>\r\n                        {2}\r\n                        <br><br>\r\n                        {3}\r\n                        <br>\r\n                        {4}\r\n                        <br><br>\r\n                        <font color=\"red\">\r\n                        {5}\r\n                        </font>\r\n                        <br><br>\r\n                        {6}\r\n                        <br><br>\r\n                    </p>\r\n                </body>\r\n            </html>\r\n            ", new object[]
					{
						"\r\n            body\r\n            {\r\n                font-family: Tahoma;\r\n                background-color: rgb(255,255,255);\r\n                color: #000000;\r\n                font-size:x-small;\r\n                width: 600px\r\n            }\r\n            p\r\n            {\r\n                margin:0in;\r\n            }\r\n            h1\r\n            {\r\n                font-family: Arial;\r\n                color: #000066;\r\n                margin: 0in;\r\n                font-size: medium; font-weight:bold\r\n            }\r\n            ",
						AirSyncUtility.HtmlEncode(Strings.RemoteWipeConfirmationMessageHeader.ToString(preferedCulture), false),
						text,
						text2,
						text3,
						AirSyncUtility.HtmlEncode(Strings.RemoteWipeConfirmationMessageBody2.ToString(preferedCulture), false),
						AirSyncUtility.HtmlEncode(Strings.RemoteWipeConfirmationMessageBody3.ToString(preferedCulture), false)
					});
				}
				messageItem.From = null;
				Participant participant = new Participant(mailboxSession.MailboxOwner.MailboxInfo.DisplayName, mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), "SMTP");
				messageItem.Recipients.Add(participant, RecipientItemType.To);
				if (addresses != null)
				{
					foreach (string emailAddress in addresses)
					{
						Participant participant2 = new Participant(null, emailAddress, "SMTP");
						messageItem.Recipients.Add(participant2, RecipientItemType.Bcc);
					}
				}
				messageItem.Send();
				flag = true;
			}
			finally
			{
				if (messageItem != null)
				{
					if (!flag)
					{
						ProvisionCommand.DeleteMessage(messageItem, mailboxSession, traceObject);
					}
					messageItem.Dispose();
				}
			}
		}

		private static void DeleteMessage(MessageItem message, MailboxSession mailboxSession, object traceObject)
		{
			message.Load();
			if (message.Id != null)
			{
				AggregateOperationResult aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.HardDelete, new StoreId[]
				{
					message.Id.ObjectId
				});
				if (OperationResult.Succeeded != aggregateOperationResult.OperationResult)
				{
					AirSyncDiagnostics.TraceDebug<MessageItem>(ExTraceGlobals.RequestsTracer, traceObject, "Failed to delete {0}", message);
				}
			}
		}

		private const string RemoteWipeMessageBody = "\r\n            <html>\r\n                <style>\r\n                    {0}\r\n                </style>\r\n                <body>\r\n                    <h1>{1}</h1>\r\n                    <br><br>\r\n                    <p>\r\n                        {2}\r\n                        <br><br>\r\n                        {3}\r\n                        <br>\r\n                        {4}\r\n                        <br><br>\r\n                        <font color=\"red\">\r\n                        {5}\r\n                        </font>\r\n                        <br><br>\r\n                        {6}\r\n                        <br><br>\r\n                    </p>\r\n                </body>\r\n            </html>\r\n            ";

		private const string RemoteWipeMessageStyle = "\r\n            body\r\n            {\r\n                font-family: Tahoma;\r\n                background-color: rgb(255,255,255);\r\n                color: #000000;\r\n                font-size:x-small;\r\n                width: 600px\r\n            }\r\n            p\r\n            {\r\n                margin:0in;\r\n            }\r\n            h1\r\n            {\r\n                font-family: Arial;\r\n                color: #000066;\r\n                margin: 0in;\r\n                font-size: medium; font-weight:bold\r\n            }\r\n            ";

		private static XmlDocument validationErrorXml;

		internal enum PolicyStatusCode
		{
			NotPresent,
			Success,
			NoPolicy,
			UnknownPolicyType,
			PolicyDataIsCorrupt,
			PolicyKeyMismatch
		}

		internal enum ProvisionStatusCode
		{
			NotPresent,
			Success,
			ProtocolError,
			GeneralServerError
		}
	}
}
