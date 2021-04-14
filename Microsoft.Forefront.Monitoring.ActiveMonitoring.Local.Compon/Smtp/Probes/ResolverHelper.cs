using System;
using System.Net;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class ResolverHelper
	{
		public static IPAddress ResolveEndPoint(string domain, DnsUtils.DnsRecordType queryType, DelTraceDebug traceDebug, bool simpleResolve)
		{
			if (queryType == DnsUtils.DnsRecordType.DnsTypeA)
			{
				return ResolverHelper.ResolveARecord(domain, traceDebug);
			}
			if (queryType == DnsUtils.DnsRecordType.DnsTypeMX)
			{
				return ResolverHelper.ResolveMxRecord(domain, traceDebug);
			}
			if (queryType != DnsUtils.DnsRecordType.DnsTypeAAAA)
			{
				throw new ArgumentException(string.Format("Unhandled RecordResolveType {0}", queryType));
			}
			return ResolverHelper.ResolveAAAARecord(domain, traceDebug, simpleResolve);
		}

		private static IPAddress ResolveMxRecord(string domain, DelTraceDebug traceDebug)
		{
			int num;
			string[] mxrecords = DnsUtils.GetMXRecords(domain, out num);
			if (mxrecords == null || mxrecords.Length == 0)
			{
				throw new ResolverHelper.UnableToResolveException(domain, DnsUtils.DnsRecordType.DnsTypeMX, (DnsUtils.DnsResponseCode)num);
			}
			traceDebug("Resolved MxRecord for {0} to {1}", new object[]
			{
				domain,
				mxrecords[0]
			});
			string text = mxrecords[0];
			DnsUtils.DnsResponse arecord = DnsUtils.GetARecord(text);
			if (arecord.IPAddress == IPAddress.None)
			{
				throw new ResolverHelper.UnableToResolveException(text, DnsUtils.DnsRecordType.DnsTypeA, arecord.ReturnCode);
			}
			traceDebug("Resolved A for {0} to {1}", new object[]
			{
				text,
				arecord.IPAddress
			});
			return arecord.IPAddress;
		}

		private static IPAddress ResolveARecord(string domain, DelTraceDebug traceDebug)
		{
			DnsUtils.DnsResponse arecord = DnsUtils.GetARecord(domain);
			if (arecord.IPAddress == IPAddress.None)
			{
				throw new ResolverHelper.UnableToResolveException(domain, DnsUtils.DnsRecordType.DnsTypeA, arecord.ReturnCode);
			}
			traceDebug("Resolved A for {0} to {1}", new object[]
			{
				domain,
				arecord.IPAddress
			});
			return arecord.IPAddress;
		}

		private static IPAddress ResolveAAAARecord(string domain, DelTraceDebug traceDebug, bool simpleResolve)
		{
			int num;
			string[] mxrecords = DnsUtils.GetMXRecords(domain, out num);
			string text = domain;
			if (!simpleResolve)
			{
				if (mxrecords == null || mxrecords.Length == 0)
				{
					throw new ResolverHelper.UnableToResolveException(domain, DnsUtils.DnsRecordType.DnsTypeMX, (DnsUtils.DnsResponseCode)num);
				}
				traceDebug("Resolved MxRecord for {0} to {1}", new object[]
				{
					domain,
					mxrecords[0]
				});
				text = mxrecords[0];
			}
			DnsUtils.DnsResponse aaaarecord = DnsUtils.GetAAAARecord(text);
			if (aaaarecord.IPAddress == IPAddress.None)
			{
				throw new ResolverHelper.UnableToResolveException(text, DnsUtils.DnsRecordType.DnsTypeAAAA, aaaarecord.ReturnCode);
			}
			traceDebug("Resolved AAAA for {0} to {1}", new object[]
			{
				text,
				aaaarecord.IPAddress
			});
			return aaaarecord.IPAddress;
		}

		public class UnableToResolveException : Exception
		{
			public UnableToResolveException()
			{
			}

			public UnableToResolveException(string domain, DnsUtils.DnsRecordType queryType, DnsUtils.DnsResponseCode responseCode) : base(string.Format("Unable to resolve {0} records for {1} (ReturnCode = {2}).", queryType.ToString(), domain, responseCode.ToString()))
			{
				this.Domain = domain;
				this.QueryType = queryType;
			}

			public string Domain { get; private set; }

			public DnsUtils.DnsRecordType QueryType { get; private set; }
		}
	}
}
