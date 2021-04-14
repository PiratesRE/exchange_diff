using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Ews.Probes
{
	public abstract class EWSGenericProbeCommon : EWSCommon
	{
		protected string OperationName { get; set; }

		protected bool IsVerifyCookieAffinity { get; set; }

		protected bool VerifyXropEndPoint { get; set; }

		protected string SecondaryOperationName { get; set; }

		protected override void Configure()
		{
			this.OperationName = this.ReadAttribute("OperationName", "ConvertId");
			this.IsVerifyCookieAffinity = this.ReadAttribute("VerifyCookieAffinity", false);
			this.VerifyXropEndPoint = this.ReadAttribute("VerifyXropEndpoint", false);
			this.SecondaryOperationName = this.ReadAttribute("SecondaryOperationName", "ConvertId");
			base.Configure();
		}

		protected void RunEWSGenericProbe(CancellationToken cancellationToken)
		{
			base.Initialize(ExTraceGlobals.EWSTracer);
			if (15 == base.MailboxVersion && base.IsOutsideInMonitoring && base.CafeVipModeWhenPossible)
			{
				this.componentIdForRequest = "UnifiedNamespaceAMProbe";
				CafeUtils.DoWorkWithUnifiedNamespace(new CafeUtils.DoWork(this.DoWorkInternal), cancellationToken, false, base.Definition, base.Broker, base.UnifiedNamespace);
				return;
			}
			if (base.TrustAnySSLCertificate)
			{
				string componentId = "Ews_AM_Probe";
				RemoteCertificateValidationCallback callback = delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
				{
					if (base.SslValidationDelaySeconds != 0)
					{
						Thread.Sleep(base.SslValidationDelaySeconds * 1000);
					}
					return true;
				};
				CertificateValidationManager.RegisterCallback(componentId, callback);
				this.componentIdForRequest = componentId;
			}
			this.DoWorkInternal(cancellationToken);
		}

		private void ExecuteEWSCall(string endPoint, string operation, bool verifyAffinity, CancellationToken cancellationToken, bool trackLatency = false)
		{
			base.LogTrace(string.Format("Starting EwsGenericProbe with Username: {0} ", base.Definition.Account));
			ExchangeService exchangeService = base.GetExchangeService(base.Definition.Account, base.Definition.AccountPassword, new EWSCommon.TraceListener(base.TraceContext, ExTraceGlobals.EWSTracer), base.ConstructEwsUrl(endPoint), 2);
			if (!string.IsNullOrEmpty(this.componentIdForRequest))
			{
				exchangeService.SetComponentId(this.componentIdForRequest);
			}
			this.PerformEWSOperation(exchangeService, operation, trackLatency, cancellationToken);
			if (verifyAffinity)
			{
				base.DecorateLogSection("COOKIE AFFINITY VERIFICATION");
				this.VerifyCookieAffinity(exchangeService, this.OperationName, cancellationToken);
			}
		}

		private void DoWorkInternal(CancellationToken cancellationToken)
		{
			try
			{
				base.LogTrace(string.Format("Executing EWS Generic probe with {0}", base.EffectiveAuthN));
				base.LatencyMeasurement.Start();
				base.DecorateLogSection("PRIMARY ENDPOINT VERIFICATION");
				if (base.IsOutsideInMonitoring)
				{
					if (base.MailboxVersion == 14)
					{
						this.ExecuteEWSCall(base.Definition.Endpoint, this.SecondaryOperationName, this.IsVerifyCookieAffinity, cancellationToken, false);
					}
					else
					{
						this.ExecuteEWSCall(base.Definition.Endpoint, this.OperationName, this.IsVerifyCookieAffinity, cancellationToken, true);
					}
				}
				else
				{
					this.ExecuteEWSCall(base.Definition.Endpoint, this.OperationName, this.IsVerifyCookieAffinity, cancellationToken, false);
				}
				if (this.VerifyXropEndPoint)
				{
					base.LogTrace(string.Format("Executing EWS Generic probe against xrop endpoint: {0}", this.SecondaryOperationName));
					base.DecorateLogSection("XROP ENDPOINT VERIFICATION");
					this.ExecuteEWSCall(base.Definition.SecondaryEndpoint, this.SecondaryOperationName, false, cancellationToken, false);
				}
				base.LatencyMeasurement.Stop();
			}
			catch (Exception e)
			{
				this.RecordEWSProbeError(e);
			}
			finally
			{
				base.LatencyMeasurement.Stop();
				base.Result.StateAttribute20 = (double)base.LatencyMeasurement.ElapsedMilliseconds;
			}
			this.RecordProbeResult();
			base.WriteVitalInfoToExecutionContext();
			base.LogTrace("EwsGenericProbe succeeded");
		}

		private void RecordProbeResult()
		{
			if (!string.IsNullOrEmpty(this.probeErrorResult))
			{
				base.ThrowError("EWSGenericProbeError", new Exception(this.probeErrorResult), "");
			}
		}

		private void RecordEWSProbeError(Exception e)
		{
			base.LogTrace(string.Format("EWS Generic Probe failed while using {0}", base.EffectiveAuthN));
			this.probeErrorResult += e.ToString();
		}

		private void PerformEWSOperation(ExchangeService service, string operation, bool trackLatency, CancellationToken cancellationToken)
		{
			if (operation != null)
			{
				if (operation == "GetFolder")
				{
					base.RetrySoapActionAndThrow(delegate()
					{
						PropertySet propertySet = new PropertySet(0);
						Folder.Bind(service, 4, propertySet);
					}, operation, service, cancellationToken, trackLatency);
					return;
				}
				if (operation == "SendEmail")
				{
					base.RetrySoapActionAndThrow(delegate()
					{
						string text = "OBD Test EWS MessageSent ";
						SearchFilter searchFilter = new SearchFilter.ContainsSubstring(ItemSchema.Subject, text);
						ItemView itemView = new ItemView(10);
						itemView.PropertySet = new PropertySet(0);
						FindItemsResults<Item> findItemsResults = service.FindItems(4, searchFilter, itemView);
						foreach (Item item in findItemsResults.Items)
						{
							item.Delete(0);
						}
						EmailMessage emailMessage = new EmailMessage(service);
						string subject = text + Guid.NewGuid().ToString();
						emailMessage.Subject = subject;
						emailMessage.Body = text;
						emailMessage.ToRecipients.Add(this.Definition.Account);
						emailMessage.Save(3);
						emailMessage.Load();
						AlternateId alternateId = new AlternateId(1, emailMessage.Id.ToString(), this.Definition.Account);
						AlternateId alternateId2 = service.ConvertId(alternateId, 2) as AlternateId;
						this.Result.StateAttribute22 = alternateId2.UniqueId.ToString();
						this.Result.StateAttribute23 = emailMessage.InternetMessageId;
						emailMessage.Send();
					}, operation, service, cancellationToken, trackLatency);
					return;
				}
				if (!(operation == "ConvertId"))
				{
				}
			}
			base.RetrySoapActionAndThrow(delegate()
			{
				AlternateId alternateId = new AlternateId(3, "00000000EEC1BD786111D011917B00000000000107002FBF98FC7852CF11912A000000000001000002AD4860000052FA68AB0CD61C4681AA421824823451000057E89BBD0000", "sarrusaphone@bombastadron.com");
				service.ConvertId(alternateId, 4);
			}, operation, service, cancellationToken);
		}

		private void VerifyCookieAffinity(ExchangeService service, string operationName, CancellationToken cancellationToken)
		{
			string text = string.Empty;
			if (base.MailboxVersion == 14)
			{
				text = service.HttpResponseHeaders["X-DiagInfo"];
				if (string.IsNullOrEmpty(text))
				{
					this.AddTracesAndThrow("The CAS name we expected to find in the X-DiagInfo header after the first request was either null or empty.", service);
				}
			}
			this.PerformEWSOperation(service, operationName, true, cancellationToken);
			if (base.MailboxVersion == 14)
			{
				string text2 = service.HttpResponseHeaders["X-DiagInfo"];
				if (string.IsNullOrEmpty(text2))
				{
					this.AddTracesAndThrow("The CAS name we expected to find in the X-DiagInfo header after the second request was either null or empty.", service);
				}
				if (!text.Equals(text2))
				{
					this.AddTracesAndThrow("The CAS names from the X-DiagInfoHeader from consecutive requests using the same service object dont match. Affinity may not be working correctly. CAS from 1st request: " + text + ". CAS from second request: " + text2, service);
				}
			}
		}

		private void AddTracesAndThrow(string errorMessage, ExchangeService service)
		{
			base.TraceBuilder.AppendLine(((EWSCommon.TraceListener)service.TraceListener).RequestLog);
			throw new Exception(errorMessage);
		}

		protected const string DefaultOperationName = "ConvertId";

		private const string XDiagInfoHeaderName = "X-DiagInfo";

		private string componentIdForRequest;

		private string probeErrorResult = string.Empty;
	}
}
