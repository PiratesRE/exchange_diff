using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;
using Microsoft.Exchange.Net.ExSmtpClient;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Test", "Message", SupportsShouldProcess = true)]
	public sealed class TestMessage : Task
	{
		[Parameter(Mandatory = false, ParameterSetName = "InboxRules")]
		[Parameter(Mandatory = true, ParameterSetName = "Arbitration")]
		[Parameter(Mandatory = false, ParameterSetName = "TransportRules")]
		public RecipientIdParameter SendReportTo
		{
			get
			{
				return (RecipientIdParameter)base.Fields["SendReportTo"];
			}
			set
			{
				base.Fields["SendReportTo"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "InboxRules")]
		[Parameter(Mandatory = false, ParameterSetName = "TransportRules")]
		public byte[] MessageFileData
		{
			get
			{
				return (byte[])base.Fields["MessageFileData"];
			}
			set
			{
				base.Fields["MessageFileData"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "InboxRules")]
		[Parameter(Mandatory = true, ParameterSetName = "Arbitration")]
		[Parameter(Mandatory = false, ParameterSetName = "TransportRules")]
		public SmtpAddress Sender
		{
			get
			{
				return (SmtpAddress)base.Fields["Sender"];
			}
			set
			{
				base.Fields["Sender"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "TransportRules")]
		[Parameter(Mandatory = true, ParameterSetName = "Arbitration")]
		[Parameter(Mandatory = true, ParameterSetName = "InboxRules")]
		public ProxyAddressCollection Recipients
		{
			get
			{
				return (ProxyAddressCollection)base.Fields["Recipients"];
			}
			set
			{
				base.Fields["Recipients"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TransportRules")]
		[Parameter(Mandatory = false, ParameterSetName = "InboxRules")]
		public SwitchParameter DeliverMessage
		{
			get
			{
				return (SwitchParameter)(base.Fields["DeliverMessage"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DeliverMessage"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TransportRules")]
		[Parameter(Mandatory = false, ParameterSetName = "InboxRules")]
		public string Options
		{
			get
			{
				return (string)base.Fields["Options"];
			}
			set
			{
				base.Fields["Options"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Arbitration")]
		public SwitchParameter Arbitration
		{
			get
			{
				return (SwitchParameter)(base.Fields["Arbitration"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Arbitration"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "InboxRules")]
		public SwitchParameter InboxRules
		{
			get
			{
				return (SwitchParameter)(base.Fields["InboxRules"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["InboxRules"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "TransportRules")]
		public SwitchParameter TransportRules
		{
			get
			{
				return (SwitchParameter)(base.Fields["TransportRules"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["TransportRules"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestMessage;
			}
		}

		protected override void InternalValidate()
		{
			if (!this.WasSpecifiedByUser("MessageFileData") && !this.WasSpecifiedByUser("Sender"))
			{
				base.WriteError(new LocalizedException(Strings.MessageFileOrSenderMustBeSpecified), ErrorCategory.InvalidArgument, null);
				return;
			}
			if (this.WasSpecifiedByUser("MessageFileData") && this.MessageFileData == null)
			{
				base.WriteError(new LocalizedException(Strings.MessageFileDataSpecifiedAsNull), ErrorCategory.InvalidArgument, null);
				return;
			}
			this.GenerateMessage();
			this.EnsureSenderSpecified();
		}

		protected override void InternalProcessRecord()
		{
			int num = ServicePrincipalName.RegisterServiceClass("SmtpSvc");
			if (num == 0)
			{
				base.WriteVerbose(Strings.SpnRegistrationSucceeded);
			}
			else
			{
				this.WriteWarning(Strings.SpnRegistrationFailed(num));
			}
			List<string> recipients = new List<string>(this.Recipients.Count);
			foreach (ProxyAddress proxyAddress in this.Recipients)
			{
				recipients.Add(proxyAddress.AddressString);
			}
			using (ServerPickerManager serverPickerManager = new ServerPickerManager("Test-Message cmdlet", ServerRole.HubTransport, ExTraceGlobals.BridgeheadPickerTracer))
			{
				PickerServerList pickerServerList = serverPickerManager.GetPickerServerList();
				try
				{
					PickerServer pickerServer = null;
					bool flag = false;
					while (!flag)
					{
						PickerServer hub = pickerServerList.PickNextUsingRoundRobinPreferringLocal();
						if (hub == null)
						{
							this.WriteWarning(Strings.NoHubsAvailable);
							break;
						}
						if (pickerServer == null)
						{
							pickerServer = hub;
						}
						else if (hub == pickerServer)
						{
							this.WriteWarning(Strings.NoHubsAvailable);
							break;
						}
						base.WriteVerbose(Strings.TryingToSubmitTestmessage(hub.MachineName));
						int[] source = new int[]
						{
							25,
							2525
						};
						if (source.Any((int port) => this.TrySendMessage(recipients, hub, port)))
						{
							flag = true;
						}
					}
				}
				finally
				{
					pickerServerList.Release();
				}
			}
		}

		private bool TrySendMessage(IEnumerable<string> recipients, PickerServer hub, int portNumber)
		{
			bool result;
			using (SmtpClient smtpClient = new SmtpClient(hub.FQDN, portNumber, new TestMessage.SmtpClientDebugOutput(this)))
			{
				smtpClient.AuthCredentials(CredentialCache.DefaultNetworkCredentials);
				using (MemoryStream messageMemoryStream = this.GetMessageMemoryStream())
				{
					smtpClient.DataStream = messageMemoryStream;
					smtpClient.From = this.Sender.ToString();
					smtpClient.To = recipients.ToArray<string>();
					try
					{
						smtpClient.Submit();
					}
					catch (Exception ex)
					{
						base.WriteWarning(ex.Message);
						return false;
					}
					result = true;
				}
			}
			return result;
		}

		internal void GenerateMessage()
		{
			if (this.WasSpecifiedByUser("MessageFileData"))
			{
				string defaultDomain = this.GetDefaultDomain();
				string mimeError;
				if ((defaultDomain == null || !this.TryGenerateMessageFromMsgFileData(defaultDomain)) && !this.TryGenerateMessageFromEmlFileData(out mimeError))
				{
					base.WriteError(new LocalizedException(Strings.InvalidTestMessageFileData(mimeError)), ErrorCategory.InvalidArgument, null);
					return;
				}
			}
			else
			{
				this.GenerateDefaultMessage();
			}
			this.ApplyTestMessageHeaders();
		}

		internal MemoryStream GetMessageMemoryStream()
		{
			MemoryStream memoryStream = new MemoryStream();
			this.message.MimeDocument.WriteTo(memoryStream);
			memoryStream.Position = 0L;
			return memoryStream;
		}

		private void GenerateDefaultMessage()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.ASCII);
				this.AddToFromHeader(streamWriter);
				this.AddDefaultBody(streamWriter);
				streamWriter.Flush();
				memoryStream.Flush();
				memoryStream.Position = 0L;
				this.message = EmailMessage.Create(memoryStream);
			}
		}

		private void AddToFromHeader(StreamWriter writer)
		{
			writer.WriteLine("From: " + this.Sender.ToString());
			writer.Write("To: ");
			bool flag = true;
			foreach (ProxyAddress proxyAddress in this.Recipients)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					writer.Write(";");
				}
				writer.Write(proxyAddress.AddressString);
			}
			writer.WriteLine();
		}

		private void AddDefaultBody(StreamWriter writer)
		{
			writer.Write("Subject: ");
			writer.WriteLine(Strings.TestMessageDefaultSubject);
			writer.WriteLine();
			writer.WriteLine(Strings.TestMessageDefaultBody);
		}

		private IRecipientSession GetRecipientSession()
		{
			ADObjectId adobjectId;
			OrganizationId organizationId = TaskHelper.ResolveCurrentUserOrganization(out adobjectId);
			if (organizationId == null)
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerId(null, null), organizationId, organizationId, false);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 474, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Support\\DiagnosticTasks\\TestMessage.cs");
		}

		private IConfigurationSession GetConfigurationSession()
		{
			ADObjectId adobjectId;
			OrganizationId organizationId = TaskHelper.ResolveCurrentUserOrganization(out adobjectId);
			if (organizationId == null)
			{
				organizationId = OrganizationId.ForestWideOrgId;
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerId(null, null), organizationId, organizationId, false);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 503, "GetConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Support\\DiagnosticTasks\\TestMessage.cs");
		}

		private string GetDefaultDomain()
		{
			if (base.MyInvocation.MyCommand == null)
			{
				return null;
			}
			string text = null;
			if (this.Recipients != null)
			{
				foreach (ProxyAddress proxyAddress in this.Recipients)
				{
					if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp)
					{
						SmtpAddress smtpAddress = new SmtpAddress(proxyAddress.AddressString);
						text = smtpAddress.Domain;
						break;
					}
				}
			}
			if (text != null)
			{
				base.WriteVerbose(Strings.UsingDefaultDomainFromRecipient(text));
				return text;
			}
			IConfigurationSession configurationSession = this.GetConfigurationSession();
			AcceptedDomain defaultAcceptedDomain = configurationSession.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain != null)
			{
				text = defaultAcceptedDomain.DomainName.Domain;
				if (!string.IsNullOrEmpty(text))
				{
					base.WriteVerbose(Strings.UsingDefaultDomainFromAD(text));
					return text;
				}
			}
			base.WriteVerbose(Strings.UnableToDiscoverDefaultDomain);
			return null;
		}

		private bool TryGenerateMessageFromMsgFileData(string defaultDomain)
		{
			bool result;
			using (Stream stream = new MemoryStream(this.MessageFileData))
			{
				using (MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties))
				{
					using (Stream stream2 = new MemoryStream())
					{
						try
						{
							InboundConversionOptions inboundConversionOptions = new InboundConversionOptions(defaultDomain);
							inboundConversionOptions.UserADSession = this.GetRecipientSession();
							OutboundConversionOptions outboundConversionOptions = new OutboundConversionOptions(defaultDomain);
							outboundConversionOptions.UserADSession = inboundConversionOptions.UserADSession;
							ItemConversion.ConvertMsgStorageToItem(stream, messageItem, inboundConversionOptions);
							if (this.WasSpecifiedByUser("Sender"))
							{
								SmtpAddress sender = this.Sender;
								Participant sender2 = new Participant(string.Empty, (string)this.Sender, "SMTP");
								messageItem.Sender = sender2;
							}
							ItemConversion.ConvertItemToSummaryTnef(messageItem, stream2, outboundConversionOptions);
							stream2.Position = 0L;
							this.message = EmailMessage.Create(stream2);
							result = true;
						}
						catch (CorruptDataException ex)
						{
							base.WriteVerbose(Strings.UnableToCreateFromMsg(ex.Message));
							result = false;
						}
						catch (ConversionFailedException ex2)
						{
							base.WriteVerbose(Strings.UnableToCreateFromMsg(ex2.Message));
							result = false;
						}
						catch (PropertyErrorException ex3)
						{
							base.WriteVerbose(Strings.UnableToCreateFromMsg(ex3.Message));
							result = false;
						}
						catch (StoragePermanentException ex4)
						{
							base.WriteVerbose(Strings.UnableToCreateFromMsg(ex4.Message));
							result = false;
						}
						catch (StorageTransientException ex5)
						{
							base.WriteVerbose(Strings.UnableToCreateFromMsg(ex5.Message));
							result = false;
						}
					}
				}
			}
			return result;
		}

		private bool TryGenerateMessageFromEmlFileData(out string error)
		{
			error = string.Empty;
			MimeDocument mimeDocument = new MimeDocument();
			using (Stream stream = new MemoryStream(this.MessageFileData))
			{
				try
				{
					mimeDocument.ComplianceMode = MimeComplianceMode.Strict;
					mimeDocument.Load(stream, CachingMode.Copy);
					this.message = EmailMessage.Create(mimeDocument);
				}
				catch (ArgumentNullException)
				{
					return false;
				}
				catch (ArgumentException)
				{
					return false;
				}
				catch (InvalidOperationException)
				{
					return false;
				}
				catch (NotSupportedException)
				{
					return false;
				}
				catch (MimeException ex)
				{
					error = ex.Message;
					return false;
				}
			}
			if (mimeDocument.ComplianceStatus != MimeComplianceStatus.Compliant)
			{
				error = Strings.MimeDoesNotComplyWithStandards;
				return false;
			}
			return true;
		}

		private void EnsureSenderSpecified()
		{
			if (!this.WasSpecifiedByUser("Sender"))
			{
				EmailRecipient from = this.message.From;
				if (from == null || from.SmtpAddress == null || !SmtpAddress.IsValidSmtpAddress(from.SmtpAddress))
				{
					base.WriteError(new LocalizedException(Strings.SenderNotSpecifiedAndNotPresentInMessage), ErrorCategory.InvalidArgument, null);
					return;
				}
				this.Sender = (SmtpAddress)from.SmtpAddress;
			}
		}

		private void ApplyTestMessageHeaders()
		{
			HeaderList headers = this.message.RootPart.Headers;
			string value = string.Empty;
			AsciiTextHeader newChild;
			if (this.WasSpecifiedByUser("Options"))
			{
				newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Test-Message-Options", this.Options);
				headers.PrependChild(newChild);
			}
			if (this.WasSpecifiedByUser("SendReportTo"))
			{
				value = this.SendReportTo.ToString();
				newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Test-Message-Send-Report-To", value);
				headers.PrependChild(newChild);
			}
			if (this.WasSpecifiedByUser("Arbitration"))
			{
				newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Test-Message-Log-For", this.arbitrationLogHeaderValue);
			}
			else if (this.WasSpecifiedByUser("TransportRules"))
			{
				newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Test-Message-Log-For", this.transportRulesLogHeaderValue);
			}
			else
			{
				newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Test-Message-Log-For", this.inboxRulesLogHeaderValue);
			}
			headers.PrependChild(newChild);
			value = "Supress";
			if (this.WasSpecifiedByUser("DeliverMessage") || this.WasSpecifiedByUser("Arbitration"))
			{
				value = "Deliver";
			}
			newChild = new AsciiTextHeader("X-MS-Exchange-Organization-Test-Message", value);
			headers.PrependChild(newChild);
		}

		private bool WasSpecifiedByUser(string key)
		{
			return base.Fields.IsChanged(key) || base.Fields.IsModified(key);
		}

		private const string SendReportToKey = "SendReportTo";

		private const string MessageFileDataKey = "MessageFileData";

		private const string SenderKey = "Sender";

		private const string RecipientsKey = "Recipients";

		private const string DeliverMessageKey = "DeliverMessage";

		private const string OptionsKey = "Options";

		private const string ArbitrationKey = "Arbitration";

		private const string InboxRulesKey = "InboxRules";

		private const string TransportRulesKey = "TransportRules";

		private const string InboxRulesParameterSetName = "InboxRules";

		private const string ArbitrationParameterSetName = "Arbitration";

		private const string TransportRulesParameterSetName = "TransportRules";

		private readonly string inboxRulesLogHeaderValue = Enum.GetName(typeof(LogTypesEnum), LogTypesEnum.InboxRules);

		private readonly string arbitrationLogHeaderValue = Enum.GetName(typeof(LogTypesEnum), LogTypesEnum.Arbitration);

		private readonly string transportRulesLogHeaderValue = Enum.GetName(typeof(LogTypesEnum), LogTypesEnum.TransportRules);

		private EmailMessage message;

		private class SmtpClientDebugOutput : ISmtpClientDebugOutput
		{
			public SmtpClientDebugOutput(TestMessage context)
			{
				this.context = context;
			}

			public void Output(Trace tracer, object context, string message, params object[] args)
			{
				if (message != null)
				{
					tracer.TraceDebug((long)((context != null) ? context.GetHashCode() : 0), message, args);
					this.context.WriteDebug(string.Format(message, args));
				}
			}

			private TestMessage context;
		}
	}
}
