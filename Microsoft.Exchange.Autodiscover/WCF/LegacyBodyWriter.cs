using System;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Autodiscover.Providers;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal class LegacyBodyWriter : BodyWriter
	{
		public LegacyBodyWriter(Message input, HttpContext context) : base(true)
		{
			IPrincipal user = context.User;
			this.parseSuccess = false;
			object obj;
			if (!input.Properties.TryGetValue("RequestData", out obj))
			{
				bool useClientCertificateAuthentication = Common.CheckClientCertificate(context.Request);
				this.requestData = new RequestData(user, useClientCertificateAuthentication, CallerRequestedCapabilities.GetInstance(context));
			}
			else
			{
				this.requestData = (RequestData)obj;
				this.requestData.User = user;
			}
			this.requestData.UserAgent = Common.SafeGetUserAgent(context.Request);
			if (input.Properties.TryGetValue("ParseSuccess", out obj))
			{
				this.parseSuccess = (bool)obj;
			}
			if (input.Properties.TryGetValue("DebugData", out obj))
			{
				this.debugData = (string)obj;
			}
		}

		protected override void OnWriteBodyContents(XmlDictionaryWriter dictionaryXml)
		{
			bool error = true;
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.IndentChars = "  ";
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
			XmlWriter xmlWriter = XmlWriter.Create(dictionaryXml, new XmlWriterSettings());
			string callerInfo = "LegacyBodyWriter.OnWriteBodyContents";
			if (!this.parseSuccess)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string>((long)this.GetHashCode(), "[OnWriteBodyContents()] 'Validation failed. Provider \"{0}\"'", base.GetType().AssemblyQualifiedName);
				Common.GenerateErrorResponse(xmlWriter, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "600", Strings.InvalidRequest.ToString(), this.debugData, this.requestData, base.GetType().AssemblyQualifiedName);
			}
			else
			{
				IStandardBudget standardBudget = null;
				try
				{
					FaultInjection.GenerateFault((FaultInjection.LIDs)2917543229U);
					standardBudget = StandardBudget.Acquire(this.requestData.User.Identity.GetSecurityIdentifier(), BudgetType.Ews, Common.GetSessionSettingsForCallerScope());
					HttpContext.Current.Items["StartBudget"] = standardBudget.ToString();
					standardBudget.CheckOverBudget();
					standardBudget.StartConnection(callerInfo);
					standardBudget.StartLocal(callerInfo, default(TimeSpan));
					string text;
					error = this.GenerateResponse(xmlWriter, standardBudget, out text);
					HttpContext.Current.Items["EndBudget"] = standardBudget.ToString();
					if (!string.IsNullOrEmpty(text))
					{
						HttpContext.Current.Response.Redirect(text, false);
					}
				}
				catch (OverBudgetException)
				{
					this.TraceExceptionAndGenerateErrorResponse(xmlWriter, Strings.ServerBusy.ToString(), "[OnWriteBodyContents()] 'Account has exceeded budget.");
				}
				catch (NonUniqueRecipientException ex)
				{
					this.TraceExceptionAndGenerateErrorResponse(xmlWriter, Strings.ADUnavailable.ToString(), "[OnWriteBodyContents()] " + ex.Message);
				}
				catch (ADTransientException ex2)
				{
					this.TraceExceptionAndGenerateErrorResponse(xmlWriter, Strings.ADUnavailable.ToString(), "[OnWriteBodyContents()] " + ex2.Message);
				}
				finally
				{
					if (standardBudget != null)
					{
						try
						{
							FaultInjection.GenerateFault((FaultInjection.LIDs)4081462589U);
							standardBudget.Dispose();
						}
						catch (ADTransientException arg)
						{
							ExTraceGlobals.FrameworkTracer.TraceError<ADTransientException>((long)this.GetHashCode(), "[OnWriteBodyContents()] Got ADTransientException {0} while disposing budget.", arg);
						}
						standardBudget = null;
					}
				}
			}
			PerformanceCounters.UpdateTotalRequests(error);
			RequestDetailsLoggerBase<RequestDetailsLogger>.Current.ActivityScope.Action = "POX";
		}

		private void TraceExceptionAndGenerateErrorResponse(XmlWriter providerXml, string errorCode, string traceMessage)
		{
			ExTraceGlobals.FrameworkTracer.TraceError<string>((long)this.GetHashCode(), traceMessage + ", Provider \"{0}\"'", base.GetType().AssemblyQualifiedName);
			Common.GenerateErrorResponse(providerXml, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "600", errorCode, string.Empty, this.requestData, base.GetType().AssemblyQualifiedName);
		}

		private bool GenerateResponse(XmlWriter providerXml, IBudget budget, out string redirect)
		{
			bool errorOccurred = false;
			string actualRedirect = null;
			this.requestData.Budget = budget;
			Common.SendWatsonReportOnUnhandledException(delegate
			{
				try
				{
					this.provider = ProvidersTable.LoadProvider(this.requestData);
				}
				catch (ADTransientException)
				{
					ExTraceGlobals.FrameworkTracer.TraceError<string>((long)this.GetHashCode(), "[GenerateResponse()] 'Active directory transient error. Provider \"{0}\"'", this.GetType().AssemblyQualifiedName);
					Common.GenerateErrorResponse(providerXml, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "603", Strings.ActiveDirectoryFailure.ToString(), string.Empty, this.requestData, this.GetType().AssemblyQualifiedName);
					errorOccurred = true;
				}
				if (!errorOccurred)
				{
					if (this.provider == null)
					{
						ExTraceGlobals.FrameworkTracer.TraceError<string>((long)this.GetHashCode(), "[GenerateResponse()] 'Provider \"{0}\" is not available.'", this.GetType().AssemblyQualifiedName);
						Common.GenerateErrorResponse(providerXml, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "601", Strings.ProviderIsNotAvailable.ToString(), string.Empty, this.requestData, this.GetType().AssemblyQualifiedName);
						errorOccurred = true;
						return;
					}
					try
					{
						actualRedirect = this.provider.Get302RedirectUrl();
						if (string.IsNullOrEmpty(actualRedirect))
						{
							this.provider.GenerateResponseXml(providerXml);
						}
						else
						{
							ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "[GenerateResponse()] Will return redirect response to '{0}'", actualRedirect);
						}
					}
					catch (OverBudgetException)
					{
						ExTraceGlobals.FrameworkTracer.TraceError<string>((long)this.GetHashCode(), "[GenerateResponse()] 'Account has exceeded budget. Provider \"{0}\"'", this.GetType().AssemblyQualifiedName);
						Common.GenerateErrorResponse(providerXml, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "600", Strings.ServerBusy.ToString(), string.Empty, this.requestData, this.GetType().AssemblyQualifiedName);
						errorOccurred = true;
					}
					catch (LocalizedException ex)
					{
						ExTraceGlobals.FrameworkTracer.TraceError<string, string>((long)this.GetHashCode(), "[GenerateResponse()] 'LocalizedException' Message=\"{0}\";Assembly=\"{1}\"", ex.Message, this.GetType().AssemblyQualifiedName);
						Common.GenerateErrorResponse(providerXml, "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006", "603", ex.Message, string.Empty, this.requestData, this.GetType().AssemblyQualifiedName);
						errorOccurred = true;
					}
				}
			});
			redirect = actualRedirect;
			return errorOccurred;
		}

		private void ValidationEventHandler(object sender, ValidationEventArgs e)
		{
			ExTraceGlobals.FrameworkTracer.TraceError((long)this.GetHashCode(), "[ValidationEventHandler()] 'ValidationError' Severity=\"{0}\";Message=\"{1}\";LineNumber=\"{2}\";LinePosition=\"{3}\"", new object[]
			{
				e.Severity,
				e.Message,
				e.Exception.LineNumber,
				e.Exception.LinePosition
			});
			Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_WarnCoreValidationError, Common.PeriodicKey, new object[]
			{
				e.Severity.ToString(),
				e.Message,
				e.Exception.LineNumber.ToString(),
				e.Exception.LinePosition.ToString()
			});
		}

		private Provider provider;

		private RequestData requestData;

		private bool parseSuccess;

		private string debugData = string.Empty;
	}
}
