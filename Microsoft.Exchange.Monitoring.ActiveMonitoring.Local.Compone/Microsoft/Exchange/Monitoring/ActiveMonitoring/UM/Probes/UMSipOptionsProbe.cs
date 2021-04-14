using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM.Probes
{
	internal class UMSipOptionsProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateUMOptionsProbe(UMServiceType umServiceType, string targetResource, string healthSet, string probeName, SipTransportType sipTransportType, string certificateThumbprint, string certificateSubjectName, int sipPort, string umServerAddress, int recurrenceIntervalSeconds, int timeoutSeconds, int maxRetryAttempts, TracingContext traceContext)
		{
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.UnifiedMessagingTracer, traceContext, "UMDiscovery:: DoWork(): Creating {0} for {1}", probeName, targetResource, null, "CreateUMOptionsProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Probes\\UMSipOptionsProbe.cs", 61);
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.TargetResource = targetResource;
			probeDefinition.AssemblyPath = UMMonitoringConstants.AssemblyPath;
			probeDefinition.TypeName = typeof(UMSipOptionsProbe).FullName;
			probeDefinition.Name = probeName;
			probeDefinition.ServiceName = healthSet;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probeDefinition.TimeoutSeconds = timeoutSeconds;
			probeDefinition.MaxRetryAttempts = maxRetryAttempts;
			probeDefinition.Attributes["UMSipTransport"] = sipTransportType.ToString();
			probeDefinition.Attributes["UMCertificateThumbprint"] = certificateThumbprint;
			probeDefinition.Attributes["UMSipListeningPort"] = sipPort.ToString();
			probeDefinition.Attributes["UMServiceType"] = umServiceType.ToString();
			probeDefinition.Attributes["UMServiceAddress"] = umServerAddress;
			probeDefinition.Attributes["UMCertificateSubjectName"] = certificateSubjectName;
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.UnifiedMessagingTracer, traceContext, "UMDiscovery:: DoWork(): Created {0} for {1}", probeName, targetResource, null, "CreateUMOptionsProbe", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Probes\\UMSipOptionsProbe.cs", 84);
			return probeDefinition;
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition definition, Dictionary<string, string> propertyBag)
		{
			UMSipOptionsProbe.CheckPropertiesForUMSipOptionsInvokeNow(propertyBag);
			Dictionary<string, string> dictionary = DefinitionHelperBase.ConvertExtensionAttributesToDictionary(propertyBag["ExtensionAttributes"]);
			definition.Attributes["UMSipTransport"] = dictionary["UMSipTransport"];
			definition.Attributes["UMSipListeningPort"] = dictionary["UMSipListeningPort"];
			definition.Attributes["UMServiceType"] = dictionary["UMServiceType"];
			string value = "localhost";
			if (dictionary.ContainsKey("UMServiceAddress") && !string.IsNullOrEmpty(dictionary["UMServiceAddress"]))
			{
				value = dictionary["UMServiceAddress"];
			}
			string value2 = string.Empty;
			if (dictionary.ContainsKey("UMCertificateSubjectName") && !string.IsNullOrEmpty(dictionary["UMCertificateSubjectName"]))
			{
				value2 = dictionary["UMCertificateSubjectName"];
			}
			string value3 = string.Empty;
			if (dictionary.ContainsKey("UMCertificateThumbprint") && !string.IsNullOrEmpty(dictionary["UMCertificateThumbprint"]))
			{
				value3 = dictionary["UMCertificateThumbprint"];
			}
			definition.Attributes["UMServiceAddress"] = value;
			definition.Attributes["UMCertificateSubjectName"] = value2;
			definition.Attributes["UMCertificateThumbprint"] = value3;
		}

		internal static void CheckPropertiesForUMSipOptionsInvokeNow(Dictionary<string, string> propertyBag)
		{
			if (propertyBag == null || !propertyBag.ContainsKey("ExtensionAttributes") || string.IsNullOrWhiteSpace(propertyBag["ExtensionAttributes"]))
			{
				throw new ArgumentException("Please specify appropriate ExtensionAttributes");
			}
			Dictionary<string, string> dictionary = DefinitionHelperBase.ConvertExtensionAttributesToDictionary(propertyBag["ExtensionAttributes"]);
			if (!dictionary.ContainsKey("UMSipTransport") || !dictionary.ContainsKey("UMSipListeningPort") || !dictionary.ContainsKey("UMServiceType"))
			{
				throw new ArgumentException("Please specify all mandatory parameters for ExtensionAttributes");
			}
			if (dictionary["UMSipTransport"].Equals(SipTransportType.TLS.ToString(), StringComparison.InvariantCultureIgnoreCase) && (!dictionary.ContainsKey("UMCertificateSubjectName") || !dictionary.ContainsKey("UMCertificateThumbprint")))
			{
				throw new ArgumentException("Please specify Certificate Subject Name and CertificateThumbprint when running in TLS mode");
			}
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>
			{
				new PropertyInformation("UMSipTransport", Strings.UMSipTransport, true),
				new PropertyInformation("UMCertificateThumbprint", Strings.UMCertificateThumbprint, false),
				new PropertyInformation("UMSipListeningPort", Strings.UMSipListeningPort, true),
				new PropertyInformation("UMServiceType", Strings.UMServiceType, true),
				new PropertyInformation("UMServiceAddress", Strings.UMServerAddress, false),
				new PropertyInformation("UMCertificateSubjectName", Strings.UMCertificateSubjectName, false)
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			string remoteFQDN = base.Definition.Attributes["UMServiceAddress"];
			string certificateSubjectName = base.Definition.Attributes["UMCertificateSubjectName"];
			if (certificateSubjectName.IndexOf("*", StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				base.Result.StateAttribute1 = "WildCardInCertificateSubjectName";
				throw new Exception("We do not support Wild Card in Certificate Subject Name. Please change the certificate accordingly");
			}
			string text = base.Definition.Attributes["UMCertificateThumbprint"];
			int remoteSipPort = Convert.ToInt32(base.Definition.Attributes["UMSipListeningPort"]);
			SipTransportType sipTransportType = (SipTransportType)Enum.Parse(typeof(SipTransportType), base.Definition.Attributes["UMSipTransport"]);
			X509Certificate2 cert = null;
			if (sipTransportType != SipTransportType.TCP)
			{
				if (string.IsNullOrWhiteSpace(text))
				{
					base.Result.StateAttribute1 = "CertificateNotConfigured";
					throw new Exception("UM Service or UM Call Router Service do not have a certificate configured. Please run enable-exchangecertificate to configure certificate");
				}
				cert = Utils.FindCertificate(base.TraceContext, StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, text);
				if (cert == null)
				{
					base.Result.StateAttribute1 = "CertificateMissing";
					throw new Exception(string.Format("Please check that certificate with thumbprint {0} is present in the certificate store", text));
				}
				if (VoiceObject.CertUsedByTlsCollaborationPlatform != null && !VoiceObject.CertUsedByTlsCollaborationPlatform.Equals(cert))
				{
					base.Result.StateAttribute1 = "OneBoxDifferentCertificateConfiguredOnUMServices";
					throw new Exception("If you are have installed Cafe and Backend on one box, we need UM Call Router Service and UM service to be configured with the same certificate for UM Local Monitoring");
				}
			}
			UMServiceType umServiceType = (UMServiceType)Enum.Parse(typeof(UMServiceType), base.Definition.Attributes["UMServiceType"]);
			Task.Factory.StartNew(delegate()
			{
				this.SendSipOptionPing(remoteFQDN, certificateSubjectName, remoteSipPort, sipTransportType, cert, umServiceType);
			}, cancellationToken, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
		}

		private void SendSipOptionPing(string remoteFQDN, string certificateSubjectName, int remoteSipPort, SipTransportType sipTransport, X509Certificate2 certificate, UMServiceType umServiceType)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.UnifiedMessagingTracer, base.TraceContext, "UMOptionsProbe::SendSipOptionPing Starting UMOptions probe", null, "SendSipOptionPing", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Probes\\UMSipOptionsProbe.cs", 274);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.UnifiedMessagingTracer, base.TraceContext, "Creating VoiceObject remoteFQDN = {0}", remoteFQDN, null, "SendSipOptionPing", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Probes\\UMSipOptionsProbe.cs", 275);
			using (VoiceObject voiceObject = new VoiceObject(base.TraceContext, certificateSubjectName, false, certificate, sipTransport, MediaProtocol.SDPLESS))
			{
				if (!voiceObject.SendSipOptionPing(remoteFQDN, certificateSubjectName, remoteSipPort))
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.UnifiedMessagingTracer, base.TraceContext, "Sip Options Failed", null, "SendSipOptionPing", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Probes\\UMSipOptionsProbe.cs", 283);
					if (voiceObject.VoErrorInformation != null)
					{
						WTFDiagnostics.TraceInformation<int, Exception>(ExTraceGlobals.UnifiedMessagingTracer, base.TraceContext, "Error code = {0}, message = {1}", voiceObject.VoErrorInformation.Code, voiceObject.VoErrorInformation.Exception, null, "SendSipOptionPing", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Probes\\UMSipOptionsProbe.cs", 286);
						if (voiceObject.VoErrorInformation != null && voiceObject.VoErrorInformation.Headers != null)
						{
							foreach (VoiceObject.HeaderInfo headerInfo in voiceObject.VoErrorInformation.Headers)
							{
								if (string.Equals(headerInfo.HeaderName, "ms-diagnostics", StringComparison.InvariantCultureIgnoreCase))
								{
									WTFDiagnostics.TraceInformation(ExTraceGlobals.UnifiedMessagingTracer, base.TraceContext, string.Format("MsDiagnostics is {0}", headerInfo.HeaderValue), null, "SendSipOptionPing", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Probes\\UMSipOptionsProbe.cs", 296);
									base.Result.StateAttribute1 = headerInfo.HeaderValue;
									throw new Exception(string.Format("SipOptions Failed with MsDiagnostics {0}", headerInfo.HeaderValue));
								}
							}
						}
						if (voiceObject.VoErrorInformation.Exception.ToString().IndexOf("15604", StringComparison.InvariantCultureIgnoreCase) < 0)
						{
							base.Result.StateAttribute1 = "ConnectionFailed";
							throw new Exception(string.Format("SipOptions Failed with Error Code = {0} Message = {1}", voiceObject.VoErrorInformation.Code, voiceObject.VoErrorInformation.Exception));
						}
						WTFDiagnostics.TraceInformation(ExTraceGlobals.UnifiedMessagingTracer, base.TraceContext, "Probe failure is due to transient AD error, will not count as failed", null, "SendSipOptionPing", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Probes\\UMSipOptionsProbe.cs", 309);
					}
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.UnifiedMessagingTracer, base.TraceContext, "Sip Options Succeeded with 200 OK", null, "SendSipOptionPing", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Probes\\UMSipOptionsProbe.cs", 325);
				}
			}
		}
	}
}
