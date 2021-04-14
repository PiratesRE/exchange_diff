using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.AirSync
{
	internal class RightsManagementInformationSetting : SettingsBase
	{
		public RightsManagementInformationSetting(XmlNode request, XmlNode response, IAirSyncUser user, CultureInfo cultureInfo, ProtocolLogger protocolLogger, MailboxLogger mailboxLogger) : base(request, response, protocolLogger)
		{
			this.user = user;
			this.cultureInfo = cultureInfo;
			this.mailboxLogger = mailboxLogger;
		}

		public override void Execute()
		{
			using (this.user.Context.Tracker.Start(TimeId.RMSExecute))
			{
				XmlNode firstChild = base.Request.FirstChild;
				string localName;
				if ((localName = firstChild.LocalName) != null && localName == "Get")
				{
					this.ProcessGet();
				}
			}
		}

		private void ProcessGet()
		{
			using (this.user.Context.Tracker.Start(TimeId.RMSProcessGet))
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Processing RightsManagementInformationSetting - Get");
				XmlNode xmlNode = base.Response.OwnerDocument.CreateElement("Get", "Settings:");
				XmlNode xmlNode2 = base.Response.OwnerDocument.CreateElement("RightsManagementTemplates", "RightsManagement:");
				try
				{
					if (this.user.IrmEnabled)
					{
						List<RmsTemplate> list = new List<RmsTemplate>(RmsTemplateReaderCache.GetRmsTemplates(this.user.OrganizationId));
						IComparer<RmsTemplate> comparer = new RightsManagementInformationSetting.RmsTemplateNameComparer(this.cultureInfo);
						list.Sort(comparer);
						int count = list.Count;
						int maxRmsTemplates = GlobalSettings.MaxRmsTemplates;
						if (count > maxRmsTemplates)
						{
							list.RemoveRange(maxRmsTemplates, count - maxRmsTemplates);
						}
						using (List<RmsTemplate>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								RmsTemplate rmsTemplate = enumerator.Current;
								AirSyncDiagnostics.TraceInfo<Guid>(ExTraceGlobals.RequestsTracer, this, "Found RMS template {0}", rmsTemplate.Id);
								XmlNode xmlNode3 = base.Response.OwnerDocument.CreateElement("RightsManagementTemplate", "RightsManagement:");
								XmlNode xmlNode4 = base.Response.OwnerDocument.CreateElement("TemplateID", "RightsManagement:");
								xmlNode4.InnerText = rmsTemplate.Id.ToString();
								xmlNode3.AppendChild(xmlNode4);
								XmlNode xmlNode5 = base.Response.OwnerDocument.CreateElement("TemplateName", "RightsManagement:");
								xmlNode5.InnerText = rmsTemplate.GetName(this.cultureInfo);
								xmlNode3.AppendChild(xmlNode5);
								XmlNode xmlNode6 = base.Response.OwnerDocument.CreateElement("TemplateDescription", "RightsManagement:");
								xmlNode6.InnerText = rmsTemplate.GetDescription(this.cultureInfo);
								xmlNode3.AppendChild(xmlNode6);
								xmlNode2.AppendChild(xmlNode3);
							}
							goto IL_205;
						}
					}
					AirSyncDiagnostics.TraceError<string>(ExTraceGlobals.RequestsTracer, this, "IRM feature disabled for user {0}", this.user.DisplayName);
					this.status = StatusCode.IRM_FeatureDisabled;
					IL_205:;
				}
				catch (AirSyncPermanentException ex)
				{
					AirSyncDiagnostics.TraceError<AirSyncPermanentException>(ExTraceGlobals.RequestsTracer, this, "AirSyncPermanentException encountered while processing RightsManagementInformationSetting->Get {0}", ex);
					if (base.ProtocolLogger != null && !string.IsNullOrEmpty(ex.ErrorStringForProtocolLogger))
					{
						base.ProtocolLogger.SetValueIfNotSet(ProtocolLoggerData.Error, ex.ErrorStringForProtocolLogger);
					}
					this.ProcessException(ex);
					this.status = ex.AirSyncStatusCode;
				}
				XmlNode xmlNode7 = base.Response.OwnerDocument.CreateElement("Status", "Settings:");
				XmlNode xmlNode8 = xmlNode7;
				int num = (int)this.status;
				xmlNode8.InnerText = num.ToString(CultureInfo.InvariantCulture);
				base.Response.AppendChild(xmlNode7);
				xmlNode.AppendChild(xmlNode2);
				base.Response.AppendChild(xmlNode);
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, this, "Done processing RightsManagementInformationSetting - Get.");
			}
		}

		private void ProcessException(Exception exception)
		{
			using (this.user.Context.Tracker.Start(TimeId.RMSProcessException))
			{
				Command.CurrentCommand.PartialFailure = true;
				if (this.mailboxLogger != null)
				{
					RightsManagementException ex = exception as RightsManagementException;
					if (ex == null)
					{
						ex = (exception.InnerException as RightsManagementException);
					}
					if (ex != null)
					{
						this.mailboxLogger.SetData(MailboxLogDataName.IRM_FailureCode, ex.FailureCode);
					}
					this.mailboxLogger.SetData(MailboxLogDataName.IRM_Exception, new AirSyncUtility.ExceptionToStringHelper(exception));
				}
			}
		}

		private StatusCode status = StatusCode.Success;

		private IAirSyncUser user;

		private CultureInfo cultureInfo;

		private MailboxLogger mailboxLogger;

		private class RmsTemplateNameComparer : IComparer<RmsTemplate>
		{
			internal RmsTemplateNameComparer(CultureInfo locale)
			{
				this.locale = locale;
			}

			public int Compare(RmsTemplate template1, RmsTemplate template2)
			{
				if (template1 == template2)
				{
					return 0;
				}
				if (template1 == RmsTemplate.DoNotForward)
				{
					return -1;
				}
				if (template2 == RmsTemplate.DoNotForward)
				{
					return 1;
				}
				if (template1 == RmsTemplate.InternetConfidential)
				{
					return -1;
				}
				if (template2 == RmsTemplate.InternetConfidential)
				{
					return 1;
				}
				return string.Compare(template1.GetName(this.locale), template2.GetName(this.locale), true, this.locale);
			}

			private CultureInfo locale;
		}
	}
}
