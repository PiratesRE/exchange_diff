using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.OOF;

namespace Microsoft.Exchange.AirSync
{
	internal class OofSetting : SettingsBase
	{
		public OofSetting(XmlNode request, XmlNode response, MailboxSession mailboxSession, DeviceAccessState currentAccessState, ProtocolLogger protocolLogger) : base(request, response, protocolLogger)
		{
			this.mailboxSession = mailboxSession;
			this.verbResponseNodes = new List<XmlNode>();
			this.currentAccessState = currentAccessState;
		}

		public override void Execute()
		{
			using (Command.CurrentCommand.Context.Tracker.Start(TimeId.OOFSettingsExecute))
			{
				try
				{
					XmlNode firstChild = base.Request.FirstChild;
					base.ProtocolLogger.SetValue(ProtocolLoggerData.OOFVerb, firstChild.LocalName);
					string localName;
					if (this.currentAccessState == DeviceAccessState.Quarantined || this.currentAccessState == DeviceAccessState.DeviceDiscovery)
					{
						this.status = SettingsBase.ErrorCode.DeniedByPolicy;
					}
					else if ((localName = firstChild.LocalName) != null)
					{
						if (!(localName == "Get"))
						{
							if (localName == "Set")
							{
								this.ProcessSet(firstChild);
							}
						}
						else
						{
							this.ProcessGet(firstChild);
						}
					}
					this.ReportStatus();
					foreach (XmlNode newChild in this.verbResponseNodes)
					{
						base.Response.AppendChild(newChild);
					}
				}
				catch (Exception exception)
				{
					if (!this.ProcessException(exception))
					{
						throw;
					}
				}
			}
		}

		private static string EncodeDateTime(DateTime dateTime)
		{
			return dateTime.ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo);
		}

		private static string InternalHtmlToText(string html)
		{
			string result;
			using (Command.CurrentCommand.Context.Tracker.Start(TimeId.OOFSettingsHtmlToText))
			{
				int codePage = 65001;
				StringReader stringReader = null;
				StringWriter stringWriter = null;
				try
				{
					stringReader = new StringReader(html);
					stringWriter = new StringWriter(CultureInfo.InvariantCulture);
					new HtmlToText
					{
						InputEncoding = Charset.GetEncoding(codePage),
						OutputEncoding = Charset.GetEncoding(codePage)
					}.Convert(stringReader, stringWriter);
					result = stringWriter.ToString();
				}
				finally
				{
					if (stringReader != null)
					{
						stringReader.Dispose();
					}
					if (stringWriter != null)
					{
						stringWriter.Dispose();
					}
				}
			}
			return result;
		}

		private static string InternalTextToHtml(string text)
		{
			string result;
			using (Command.CurrentCommand.Context.Tracker.Start(TimeId.OOFSettingsTextToHtml))
			{
				int codePage = 65001;
				StringReader stringReader = null;
				StringWriter stringWriter = null;
				try
				{
					stringReader = new StringReader(text);
					stringWriter = new StringWriter(CultureInfo.InvariantCulture);
					new TextToHtml
					{
						InputEncoding = Charset.GetEncoding(codePage),
						OutputEncoding = Charset.GetEncoding(codePage)
					}.Convert(stringReader, stringWriter);
					result = stringWriter.ToString();
				}
				finally
				{
					if (stringReader != null)
					{
						stringReader.Dispose();
					}
					if (stringWriter != null)
					{
						stringWriter.Dispose();
					}
				}
			}
			return result;
		}

		private void ReportStatus()
		{
			XmlNode xmlNode = base.Response.OwnerDocument.CreateElement("Status", "Settings:");
			int num = (int)this.status;
			xmlNode.InnerText = num.ToString(CultureInfo.InvariantCulture);
			base.Response.AppendChild(xmlNode);
		}

		private bool ProcessException(Exception exception)
		{
			bool result;
			using (Command.CurrentCommand.Context.Tracker.Start(TimeId.OOFSettingsProcessException))
			{
				Command.CurrentCommand.PartialFailure = true;
				if (exception is InvalidUserOofSettings || exception is InvalidScheduledOofDuration)
				{
					this.status = SettingsBase.ErrorCode.InvalidArguments;
					this.ReportStatus();
					result = true;
				}
				else if (exception is OofRulesSaveException)
				{
					this.status = SettingsBase.ErrorCode.AccessDenied;
					this.ReportStatus();
					result = true;
				}
				else if (exception is FormatException)
				{
					this.status = SettingsBase.ErrorCode.ProtocolError;
					this.ReportStatus();
					result = true;
				}
				else if (exception is ConnectionFailedTransientException)
				{
					this.status = SettingsBase.ErrorCode.ServerUnavailable;
					this.ReportStatus();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private void ProcessGet(XmlNode getNode)
		{
			using (Command.CurrentCommand.Context.Tracker.Start(TimeId.OOFSettingsProcessGet))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Processing Oof - Get");
				XmlNode firstChild = getNode.FirstChild;
				string innerText = firstChild.InnerText;
				bool flag;
				if (string.Equals(innerText, "html", StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
				}
				else
				{
					if (!string.Equals(innerText, "text", StringComparison.OrdinalIgnoreCase))
					{
						this.status = SettingsBase.ErrorCode.ProtocolError;
						return;
					}
					flag = false;
				}
				XmlNode xmlNode = base.Response.OwnerDocument.CreateElement("Get", "Settings:");
				XmlNode xmlNode2 = base.Response.OwnerDocument.CreateElement("OofState", "Settings:");
				XmlNode xmlNode3 = base.Response.OwnerDocument.CreateElement("StartTime", "Settings:");
				XmlNode xmlNode4 = base.Response.OwnerDocument.CreateElement("EndTime", "Settings:");
				XmlNode xmlNode5 = base.Response.OwnerDocument.CreateElement("OofMessage", "Settings:");
				XmlNode newChild = base.Response.OwnerDocument.CreateElement("AppliesToInternal", "Settings:");
				XmlNode xmlNode6 = base.Response.OwnerDocument.CreateElement("ReplyMessage", "Settings:");
				XmlNode xmlNode7 = base.Response.OwnerDocument.CreateElement("Enabled", "Settings:");
				XmlNode xmlNode8 = base.Response.OwnerDocument.CreateElement("BodyType", "Settings:");
				XmlNode xmlNode9 = base.Response.OwnerDocument.CreateElement("OofMessage", "Settings:");
				XmlNode newChild2 = base.Response.OwnerDocument.CreateElement("AppliesToExternalKnown", "Settings:");
				XmlNode xmlNode10 = base.Response.OwnerDocument.CreateElement("ReplyMessage", "Settings:");
				XmlNode xmlNode11 = base.Response.OwnerDocument.CreateElement("Enabled", "Settings:");
				XmlNode xmlNode12 = base.Response.OwnerDocument.CreateElement("BodyType", "Settings:");
				XmlNode xmlNode13 = base.Response.OwnerDocument.CreateElement("OofMessage", "Settings:");
				XmlNode newChild3 = base.Response.OwnerDocument.CreateElement("AppliesToExternalUnknown", "Settings:");
				XmlNode xmlNode14 = base.Response.OwnerDocument.CreateElement("ReplyMessage", "Settings:");
				XmlNode xmlNode15 = base.Response.OwnerDocument.CreateElement("Enabled", "Settings:");
				XmlNode xmlNode16 = base.Response.OwnerDocument.CreateElement("BodyType", "Settings:");
				UserOofSettings userOofSettings = UserOofSettings.GetUserOofSettings(this.mailboxSession);
				xmlNode.AppendChild(xmlNode2);
				xmlNode2.InnerText = ((int)userOofSettings.OofState).ToString(CultureInfo.InvariantCulture);
				if (userOofSettings.Duration != null && !userOofSettings.SetByLegacyClient)
				{
					xmlNode.AppendChild(xmlNode3);
					xmlNode.AppendChild(xmlNode4);
					xmlNode3.InnerText = OofSetting.EncodeDateTime(userOofSettings.Duration.StartTime);
					xmlNode4.InnerText = OofSetting.EncodeDateTime(userOofSettings.Duration.EndTime);
				}
				xmlNode.AppendChild(xmlNode5);
				xmlNode5.AppendChild(newChild);
				xmlNode5.AppendChild(xmlNode7);
				string message = userOofSettings.InternalReply.Message;
				if (!string.IsNullOrEmpty(message))
				{
					xmlNode7.InnerText = "1";
					xmlNode5.AppendChild(xmlNode6);
					xmlNode5.AppendChild(xmlNode8);
					if (flag)
					{
						xmlNode6.InnerText = message;
						xmlNode8.InnerText = "HTML";
					}
					else
					{
						xmlNode6.InnerText = OofSetting.InternalHtmlToText(message);
						xmlNode8.InnerText = "TEXT";
					}
				}
				else
				{
					xmlNode7.InnerText = "0";
				}
				xmlNode.AppendChild(xmlNode9);
				string message2 = userOofSettings.ExternalReply.Message;
				if (!string.IsNullOrEmpty(message2))
				{
					xmlNode9.AppendChild(newChild2);
					xmlNode9.AppendChild(xmlNode11);
					xmlNode9.AppendChild(xmlNode10);
					xmlNode9.AppendChild(xmlNode12);
					if (userOofSettings.ExternalAudience != ExternalAudience.None)
					{
						xmlNode11.InnerText = "1";
					}
					else
					{
						xmlNode11.InnerText = "0";
					}
					if (flag)
					{
						xmlNode10.InnerText = message2;
						xmlNode12.InnerText = "HTML";
					}
					else
					{
						xmlNode10.InnerText = OofSetting.InternalHtmlToText(message2);
						xmlNode12.InnerText = "TEXT";
					}
				}
				else
				{
					xmlNode9.AppendChild(newChild2);
					xmlNode9.AppendChild(xmlNode11);
					xmlNode11.InnerText = "0";
				}
				xmlNode.AppendChild(xmlNode13);
				if (!string.IsNullOrEmpty(message2))
				{
					xmlNode13.AppendChild(newChild3);
					xmlNode13.AppendChild(xmlNode15);
					xmlNode13.AppendChild(xmlNode14);
					xmlNode13.AppendChild(xmlNode16);
					if (userOofSettings.ExternalAudience == ExternalAudience.All)
					{
						xmlNode15.InnerText = "1";
					}
					else
					{
						xmlNode15.InnerText = "0";
					}
					xmlNode14.InnerText = xmlNode10.InnerText;
					xmlNode16.InnerText = xmlNode12.InnerText;
				}
				else
				{
					xmlNode13.AppendChild(newChild3);
					xmlNode13.AppendChild(xmlNode15);
					xmlNode15.InnerText = "0";
				}
				this.verbResponseNodes.Add(xmlNode);
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Done processing Oof - Get.");
			}
		}

		private void ProcessSet(XmlNode setNode)
		{
			using (Command.CurrentCommand.Context.Tracker.Start(TimeId.OOFSettingsProcessSet))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Processing Oof - Set");
				UserOofSettings userOofSettings = UserOofSettings.GetUserOofSettings(this.mailboxSession);
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = false;
				bool flag6 = false;
				bool flag7 = false;
				foreach (object obj in setNode.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (this.status != SettingsBase.ErrorCode.Success)
					{
						break;
					}
					string localName;
					if ((localName = xmlNode.LocalName) != null)
					{
						if (localName == "OofState")
						{
							string innerText = xmlNode.InnerText;
							string a;
							if ((a = innerText) != null)
							{
								if (a == "0")
								{
									userOofSettings.OofState = OofState.Disabled;
									continue;
								}
								if (a == "1")
								{
									userOofSettings.OofState = OofState.Enabled;
									continue;
								}
								if (a == "2")
								{
									userOofSettings.OofState = OofState.Scheduled;
									continue;
								}
							}
							this.status = SettingsBase.ErrorCode.ProtocolError;
							continue;
						}
						if (localName == "StartTime")
						{
							string innerText2 = xmlNode.InnerText;
							if (userOofSettings.Duration == null)
							{
								userOofSettings.Duration = new Duration((DateTime)ExDateTime.UtcNow, (DateTime)ExDateTime.UtcNow);
							}
							userOofSettings.Duration.StartTime = this.DecodeDateTime(innerText2);
							flag = true;
							continue;
						}
						if (localName == "EndTime")
						{
							string innerText3 = xmlNode.InnerText;
							if (userOofSettings.Duration == null)
							{
								userOofSettings.Duration = new Duration((DateTime)ExDateTime.UtcNow, (DateTime)ExDateTime.UtcNow);
							}
							userOofSettings.Duration.EndTime = this.DecodeDateTime(innerText3);
							flag2 = true;
							continue;
						}
						if (localName == "OofMessage")
						{
							OofSetting.OofMessage oofMessage;
							oofMessage.Enabled = false;
							oofMessage.ReplyMessage = null;
							oofMessage.BodyType = null;
							oofMessage.AppliesToInternal = false;
							oofMessage.AppliesToExternalKnown = false;
							oofMessage.AppliesToExternalUnknown = false;
							foreach (object obj2 in xmlNode.ChildNodes)
							{
								XmlNode xmlNode2 = (XmlNode)obj2;
								if (this.status != SettingsBase.ErrorCode.Success)
								{
									break;
								}
								string localName2;
								switch (localName2 = xmlNode2.LocalName)
								{
								case "AppliesToInternal":
									if (flag3)
									{
										this.status = SettingsBase.ErrorCode.ConflictingArguments;
										continue;
									}
									flag3 = true;
									oofMessage.AppliesToInternal = true;
									continue;
								case "AppliesToExternalKnown":
									if (flag4)
									{
										this.status = SettingsBase.ErrorCode.ConflictingArguments;
										continue;
									}
									oofMessage.AppliesToExternalKnown = true;
									continue;
								case "AppliesToExternalUnknown":
									if (flag6)
									{
										this.status = SettingsBase.ErrorCode.ConflictingArguments;
										continue;
									}
									oofMessage.AppliesToExternalUnknown = true;
									continue;
								case "Enabled":
									if (xmlNode2.InnerText.Equals("1", StringComparison.OrdinalIgnoreCase))
									{
										oofMessage.Enabled = true;
										continue;
									}
									continue;
								case "ReplyMessage":
									oofMessage.ReplyMessage = xmlNode2.InnerText;
									continue;
								case "BodyType":
									oofMessage.BodyType = xmlNode2.InnerText;
									continue;
								}
								this.status = SettingsBase.ErrorCode.ProtocolError;
							}
							if (this.status != SettingsBase.ErrorCode.Success)
							{
								continue;
							}
							if (oofMessage.ReplyMessage != null)
							{
								if (oofMessage.BodyType == null)
								{
									this.status = SettingsBase.ErrorCode.ProtocolError;
									continue;
								}
								if (oofMessage.BodyType.Equals("TEXT", StringComparison.OrdinalIgnoreCase))
								{
									oofMessage.ReplyMessage = OofSetting.InternalTextToHtml(oofMessage.ReplyMessage);
									oofMessage.BodyType = "HTML";
								}
								if (!oofMessage.BodyType.Equals("HTML", StringComparison.OrdinalIgnoreCase))
								{
									this.status = SettingsBase.ErrorCode.InvalidArguments;
									continue;
								}
							}
							if (oofMessage.AppliesToInternal)
							{
								if (oofMessage.Enabled)
								{
									userOofSettings.InternalReply.Message = oofMessage.ReplyMessage;
								}
								else
								{
									userOofSettings.InternalReply.Message = string.Empty;
								}
							}
							if (oofMessage.AppliesToExternalKnown && oofMessage.AppliesToExternalUnknown)
							{
								if (flag4 || flag6)
								{
									this.status = SettingsBase.ErrorCode.ConflictingArguments;
									continue;
								}
								if (oofMessage.Enabled)
								{
									userOofSettings.ExternalReply.Message = oofMessage.ReplyMessage;
									userOofSettings.ExternalAudience = ExternalAudience.All;
									flag5 = true;
									flag7 = true;
									flag4 = true;
									flag6 = true;
									continue;
								}
								userOofSettings.ExternalReply.Message = string.Empty;
								userOofSettings.ExternalAudience = ExternalAudience.None;
								flag4 = true;
								flag6 = true;
								continue;
							}
							else if (oofMessage.AppliesToExternalKnown)
							{
								if (oofMessage.Enabled)
								{
									if (flag6)
									{
										if (!string.Equals(oofMessage.ReplyMessage, userOofSettings.ExternalReply.Message, StringComparison.Ordinal))
										{
											this.status = SettingsBase.ErrorCode.ConflictingArguments;
											continue;
										}
										userOofSettings.ExternalAudience = ExternalAudience.All;
									}
									else
									{
										userOofSettings.ExternalReply.Message = oofMessage.ReplyMessage;
										userOofSettings.ExternalAudience = ExternalAudience.Known;
									}
									flag5 = true;
									flag4 = true;
									continue;
								}
								if (flag6 && flag7)
								{
									this.status = SettingsBase.ErrorCode.ConflictingArguments;
									continue;
								}
								userOofSettings.ExternalAudience = ExternalAudience.None;
								flag4 = true;
								continue;
							}
							else
							{
								if (!oofMessage.AppliesToExternalUnknown)
								{
									continue;
								}
								if (oofMessage.Enabled)
								{
									if (flag4)
									{
										if (!string.Equals(oofMessage.ReplyMessage, userOofSettings.ExternalReply.Message, StringComparison.Ordinal))
										{
											this.status = SettingsBase.ErrorCode.ConflictingArguments;
											continue;
										}
										userOofSettings.ExternalAudience = ExternalAudience.All;
									}
									else
									{
										userOofSettings.ExternalReply.Message = oofMessage.ReplyMessage;
										userOofSettings.ExternalAudience = ExternalAudience.All;
									}
									flag7 = true;
									flag6 = true;
									continue;
								}
								if (flag4 && flag5)
								{
									userOofSettings.ExternalAudience = ExternalAudience.Known;
								}
								else
								{
									userOofSettings.ExternalAudience = ExternalAudience.None;
								}
								flag6 = true;
								continue;
							}
						}
					}
					this.status = SettingsBase.ErrorCode.ProtocolError;
				}
				if (this.status == SettingsBase.ErrorCode.Success && (flag ^ flag2))
				{
					this.status = SettingsBase.ErrorCode.ProtocolError;
				}
				if (this.status == SettingsBase.ErrorCode.Success && userOofSettings.OofState == OofState.Scheduled && (userOofSettings.Duration == null || userOofSettings.Duration.EndTime <= userOofSettings.Duration.StartTime || userOofSettings.Duration.EndTime <= DateTime.UtcNow))
				{
					this.status = SettingsBase.ErrorCode.ConflictingArguments;
				}
				if (this.status == SettingsBase.ErrorCode.Success)
				{
					userOofSettings.Save(this.mailboxSession);
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Done processing Oof - Set.");
				}
			}
		}

		private DateTime DecodeDateTime(string dateTimeString)
		{
			DateTime utcNow = DateTime.UtcNow;
			if (!DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd\\THH:mm:ss.fff\\Z", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out utcNow))
			{
				this.status = SettingsBase.ErrorCode.InvalidArguments;
			}
			return DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
		}

		private SettingsBase.ErrorCode status = SettingsBase.ErrorCode.Success;

		private MailboxSession mailboxSession;

		private List<XmlNode> verbResponseNodes;

		private DeviceAccessState currentAccessState;

		private struct OofMessage
		{
			public bool AppliesToInternal;

			public bool AppliesToExternalKnown;

			public bool AppliesToExternalUnknown;

			public bool Enabled;

			public string ReplyMessage;

			public string BodyType;
		}
	}
}
