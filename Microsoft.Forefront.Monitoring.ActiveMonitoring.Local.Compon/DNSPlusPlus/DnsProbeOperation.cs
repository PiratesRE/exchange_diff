using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class DnsProbeOperation
	{
		private DnsProbeOperation(DnsProbeOperation.OperationAttributes attributes, TracingContext traceContext, DnsConfiguration dnsConfig)
		{
			this.traceContext = traceContext;
			if (attributes == null)
			{
				throw new ArgumentNullException("attributes");
			}
			if (dnsConfig == null)
			{
				throw new ArgumentNullException("dnsConfig");
			}
			this.operationAttributes = attributes.GetClone();
			this.dnsConfig = dnsConfig;
		}

		public string ErrorMessage { get; private set; }

		public bool ExitOnFailure
		{
			get
			{
				return this.operationAttributes.ExitOnFailure;
			}
		}

		public int ProbeRetryAttempts
		{
			get
			{
				return this.operationAttributes.ProbeRetryAttempts;
			}
		}

		public static List<DnsProbeOperation> GetOperations(XmlElement operationsNode, DnsConfiguration dnsConfig, TracingContext traceContext)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, traceContext, "DnsProbeOperation: Trying to get list of Operations", null, "GetOperations", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 114);
			if (operationsNode == null)
			{
				throw new ArgumentNullException("operationsNode");
			}
			if (operationsNode.ChildNodes.Count == 0)
			{
				throw new ArgumentException("Atleast one operation needs to be specified", "operationsNode.ChildNodes.Count");
			}
			if (dnsConfig == null)
			{
				throw new ArgumentNullException("dnsConfig");
			}
			List<DnsProbeOperation> list = new List<DnsProbeOperation>();
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, traceContext, "DnsProbeOperation: Parsing operation attributes", null, "GetOperations", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 133);
			DnsProbeOperation.OperationAttributes operationAttributes = new DnsProbeOperation.OperationAttributes();
			operationAttributes.Sla = TimeSpan.FromSeconds((double)Utils.GetInteger(Utils.GetMandatoryXmlAttribute<string>(operationsNode, "SlaSeconds"), "DnsOperations.SlaSeconds", 1, 1));
			operationAttributes.RequestTimeout = TimeSpan.FromSeconds((double)Utils.GetInteger(Utils.GetMandatoryXmlAttribute<string>(operationsNode, "TimeoutSeconds"), "DnsOperations.TimeoutSeconds", 1, 1));
			operationAttributes.SocketRetryAttempts = Utils.GetInteger(Utils.GetMandatoryXmlAttribute<string>(operationsNode, "SocketRetryAttempts"), "DnsOperations.SocketRetryAttempts", 0, 0);
			operationAttributes.ProbeRetryAttempts = Utils.GetInteger(Utils.GetOptionalXmlAttribute<string>(operationsNode, "ProbeRetryAttempts", null), "DnsOperations.ProbeRetryAttempts", 0, 0);
			operationAttributes.ExitOnFailure = Utils.GetOptionalXmlAttribute<bool>(operationsNode, "ExitOnFailure", false);
			if (operationAttributes.RequestTimeout < operationAttributes.Sla)
			{
				throw new DnsMonitorException(string.Format("RequestTimeout should be >= Sla, actual RequestTimeout={0}, Sla={1}", operationAttributes.RequestTimeout, operationAttributes.Sla), null);
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, traceContext, "DnsProbeOperation: Parsing operation nodes", null, "GetOperations", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 147);
			foreach (object obj in operationsNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null)
				{
					operationAttributes.DomainSelectionFilter = Utils.GetEnumValue<DomainSelection>(xmlElement.GetAttribute("DomainSelection"), "DnsOperations/Operation.DomainSelection");
					if (operationAttributes.DomainSelectionFilter == DomainSelection.CustomQuery)
					{
						operationAttributes.DomainKey = Utils.CheckNullOrWhiteSpace(Utils.GetMandatoryXmlAttribute<string>(xmlElement, "DomainKey"), "DomainKey").Trim();
						operationAttributes.ZoneName = Utils.CheckNullOrWhiteSpace(Utils.GetMandatoryXmlAttribute<string>(xmlElement, "ZoneName"), "ZoneName").Trim();
					}
					operationAttributes.QueryType = Utils.GetEnumValue<RecordType>(xmlElement.GetAttribute("QueryType"), "DnsOperations/Operation.QueryType");
					operationAttributes.QueryClass = Utils.GetEnumValue<RecordClass>(xmlElement.GetAttribute("QueryClass"), "DnsOperations/Operation.QueryClass");
					operationAttributes.ExpectedResponseCode = Utils.GetEnumValue<QueryResponseCode>(xmlElement.GetAttribute("ExpectedResponse"), "DnsOperations/Operation.ExpectedResponse");
					DnsProbeOperation.AddOperations(list, dnsConfig, operationAttributes, traceContext);
				}
			}
			return list;
		}

		public override string ToString()
		{
			return string.Format("Op={0}, {1}", this.operationAttributes, this.ErrorMessage);
		}

		public bool Invoke(CancellationToken cancellationToken)
		{
			IPEndPoint server = new IPEndPoint(this.operationAttributes.ServerIPAddress, 53);
			WTFDiagnostics.TraceInformation<DnsProbeOperation>(ExTraceGlobals.DNSTracer, this.traceContext, "DnsProbeOperation: Processing DNS request for operation, {0}", this, null, "Invoke", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 203);
			string errorMessage;
			if (DnsHelper.ProcessDNSRequest(this.operationAttributes.DomainName, this.operationAttributes.QueryType, this.operationAttributes.QueryClass, server, this.operationAttributes.Sla, this.operationAttributes.RequestTimeout, this.operationAttributes.SocketRetryAttempts, this.operationAttributes.ExpectedResponseCode, this.operationAttributes.DomainName.StartsWith(this.dnsConfig.IpV6Prefix), out errorMessage, cancellationToken))
			{
				WTFDiagnostics.TraceInformation<DnsProbeOperation>(ExTraceGlobals.DNSTracer, this.traceContext, "DnsProbeOperation: Operation Passed", this, null, "Invoke", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 217);
				return true;
			}
			this.ErrorMessage = errorMessage;
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DNSTracer, this.traceContext, "DnsProbeOperation: Operation Failed, message={0}", this.ErrorMessage, null, "Invoke", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 223);
			return false;
		}

		public bool CanInvoke()
		{
			bool result = false;
			WTFDiagnostics.TraceInformation<DnsProbeOperation>(ExTraceGlobals.DNSTracer, this.traceContext, "DnsProbeOperation: checking if the operation can be invoked, {0}", this, null, "CanInvoke", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 236);
			if (string.IsNullOrWhiteSpace(this.operationAttributes.DomainName))
			{
				if (this.operationAttributes.DomainSelectionFilter == DomainSelection.InvalidDomainWithFallback)
				{
					this.ErrorMessage = "Cannot execute the operation, as we do not have any domain with fallback";
				}
				else
				{
					if (this.operationAttributes.DomainSelectionFilter != DomainSelection.InvalidDomainWithoutFallback)
					{
						throw new DnsMonitorException("Internal error, the domainName for the query was not generated", null);
					}
					this.ErrorMessage = "Cannot execute the operation, as we do not have any domain without fallback";
				}
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DNSTracer, this.traceContext, "DnsProbeOperation: Operation CanInvoke check failed, message={0}", this.ErrorMessage, null, "CanInvoke", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 253);
			}
			else if (this.operationAttributes.DomainSelectionFilter == DomainSelection.CustomQuery && !this.dnsConfig.IsSupportedZone(this.operationAttributes.ZoneName))
			{
				this.ErrorMessage = string.Format("Cannot execute the operation, as the zone '{0}' is not supported", this.operationAttributes.ZoneName);
			}
			else
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DNSTracer, this.traceContext, "DnsProbeOperation: Operation CanInvoke check passed", null, "CanInvoke", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 262);
				result = true;
			}
			return result;
		}

		private static void AddOperations(List<DnsProbeOperation> operations, DnsConfiguration dnsConfig, DnsProbeOperation.OperationAttributes operationAttributes, TracingContext traceContext)
		{
			List<string> list = null;
			if (operationAttributes.DomainSelectionFilter == DomainSelection.CustomQuery)
			{
				list = new List<string>
				{
					operationAttributes.DomainKey + "." + operationAttributes.ZoneName
				};
			}
			else
			{
				list = dnsConfig.GetDomainsToLookup(operationAttributes.DomainSelectionFilter);
			}
			if (list.Count > 0)
			{
				using (List<IPAddress>.Enumerator enumerator = dnsConfig.DnsServerIps.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IPAddress serverIPAddress = enumerator.Current;
						foreach (string domainName in list)
						{
							operationAttributes.DomainName = domainName;
							operationAttributes.ServerIPAddress = serverIPAddress;
							DnsProbeOperation dnsProbeOperation = new DnsProbeOperation(operationAttributes, traceContext, dnsConfig);
							WTFDiagnostics.TraceInformation<DnsProbeOperation>(ExTraceGlobals.DNSTracer, traceContext, "DnsProbeOperation: Adding operation, {0}", dnsProbeOperation, null, "AddOperations", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 300);
							operations.Add(dnsProbeOperation);
						}
					}
					return;
				}
			}
			operationAttributes.DomainName = null;
			DnsProbeOperation dnsProbeOperation2 = new DnsProbeOperation(operationAttributes, traceContext, dnsConfig);
			WTFDiagnostics.TraceInformation<DnsProbeOperation>(ExTraceGlobals.DNSTracer, traceContext, "DnsProbeOperation: Adding operation, {0}", dnsProbeOperation2, null, "AddOperations", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DNS\\Probes\\DnsProbeOperation.cs", 311);
			operations.Add(dnsProbeOperation2);
		}

		private DnsProbeOperation.OperationAttributes operationAttributes;

		private TracingContext traceContext;

		private DnsConfiguration dnsConfig;

		private class OperationAttributes
		{
			public DomainSelection DomainSelectionFilter { get; set; }

			public string DomainName { get; set; }

			public IPAddress ServerIPAddress { get; set; }

			public RecordType QueryType { get; set; }

			public RecordClass QueryClass { get; set; }

			public TimeSpan Sla { get; set; }

			public TimeSpan RequestTimeout { get; set; }

			public int ProbeRetryAttempts { get; set; }

			public int SocketRetryAttempts { get; set; }

			public bool ExitOnFailure { get; set; }

			public QueryResponseCode ExpectedResponseCode { get; set; }

			public string DomainKey { get; set; }

			public string ZoneName { get; set; }

			public DnsProbeOperation.OperationAttributes GetClone()
			{
				return new DnsProbeOperation.OperationAttributes
				{
					DomainName = this.DomainName,
					DomainSelectionFilter = this.DomainSelectionFilter,
					ExpectedResponseCode = this.ExpectedResponseCode,
					QueryClass = this.QueryClass,
					QueryType = this.QueryType,
					RequestTimeout = this.RequestTimeout,
					ProbeRetryAttempts = this.ProbeRetryAttempts,
					SocketRetryAttempts = this.SocketRetryAttempts,
					ServerIPAddress = this.ServerIPAddress,
					Sla = this.Sla,
					ExitOnFailure = this.ExitOnFailure,
					DomainKey = this.DomainKey,
					ZoneName = this.ZoneName
				};
			}

			public override string ToString()
			{
				return string.Format("F:{0}, Q:{1}, T:{2}, C:{3}", new object[]
				{
					this.DomainSelectionFilter,
					this.DomainName,
					this.QueryType,
					this.QueryClass
				});
			}
		}
	}
}
