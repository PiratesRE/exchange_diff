using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.SenderId
{
	internal static class Util
	{
		public static class PerformanceCounters
		{
			public static void MessageValidatedWithResult(SenderIdResult result)
			{
				if (result == null)
				{
					throw new ArgumentNullException("result");
				}
				SenderIdPerfCounters.TotalMessagesValidated.Increment();
				switch (result.Status)
				{
				case SenderIdStatus.Pass:
					SenderIdPerfCounters.TotalMessagesWithPassResult.Increment();
					return;
				case SenderIdStatus.Neutral:
					SenderIdPerfCounters.TotalMessagesWithNeutralResult.Increment();
					return;
				case SenderIdStatus.SoftFail:
					SenderIdPerfCounters.TotalMessagesWithSoftFailResult.Increment();
					return;
				case SenderIdStatus.Fail:
					switch (result.FailReason)
					{
					case SenderIdFailReason.NotPermitted:
						SenderIdPerfCounters.TotalMessagesWithFailNotPermittedResult.Increment();
						return;
					case SenderIdFailReason.MalformedDomain:
						SenderIdPerfCounters.TotalMessagesWithFailMalformedDomainResult.Increment();
						return;
					case SenderIdFailReason.DomainDoesNotExist:
						SenderIdPerfCounters.TotalMessagesWithFailNonExistentDomainResult.Increment();
						return;
					default:
						throw new InvalidOperationException("Invalid FailReason");
					}
					break;
				case SenderIdStatus.None:
					SenderIdPerfCounters.TotalMessagesWithNoneResult.Increment();
					return;
				case SenderIdStatus.TempError:
					SenderIdPerfCounters.TotalMessagesWithTempErrorResult.Increment();
					return;
				case SenderIdStatus.PermError:
					SenderIdPerfCounters.TotalMessagesWithPermErrorResult.Increment();
					return;
				default:
					return;
				}
			}

			public static void DnsQueryPerformed()
			{
				SenderIdPerfCounters.TotalDnsQueries.Increment();
			}

			public static void NoPRA()
			{
				SenderIdPerfCounters.TotalMessagesWithNoPRA.Increment();
			}

			public static void MissingOriginatingIP()
			{
				SenderIdPerfCounters.TotalMessagesMissingOriginatingIP.Increment();
			}

			public static void MessageBypassedValidation()
			{
				SenderIdPerfCounters.TotalMessagesThatBypassedValidation.Increment();
			}

			public static void RemoveCounters()
			{
				SenderIdPerfCounters.TotalMessagesValidated.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithPassResult.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithNeutralResult.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithSoftFailResult.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithFailNotPermittedResult.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithFailMalformedDomainResult.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithFailNonExistentDomainResult.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithNoneResult.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithTempErrorResult.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithPermErrorResult.RawValue = 0L;
				SenderIdPerfCounters.TotalDnsQueries.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesWithNoPRA.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesMissingOriginatingIP.RawValue = 0L;
				SenderIdPerfCounters.TotalMessagesThatBypassedValidation.RawValue = 0L;
			}
		}

		public static class AsyncDns
		{
			public static void SetDns(Dns dnsObject)
			{
				if (dnsObject == null)
				{
					throw new ArgumentNullException("dnsObject");
				}
				Util.AsyncDns.dns = dnsObject;
			}

			public static bool IsAcceptable(DnsStatus status)
			{
				return status == DnsStatus.Success || status == DnsStatus.InfoDomainNonexistent || status == DnsStatus.InfoNoRecords || status == DnsStatus.InfoTruncated;
			}

			public static bool IsDnsServerListEmpty()
			{
				return Util.AsyncDns.dns.ServerList == null || Util.AsyncDns.dns.ServerList.Count == 0;
			}

			public static IAsyncResult BeginTxtRecordQuery(string domain, AsyncCallback asyncCallback, object asyncState)
			{
				Util.PerformanceCounters.DnsQueryPerformed();
				return Util.AsyncDns.dns.BeginRetrieveTextRecords(domain, asyncCallback, asyncState);
			}

			public static DnsStatus EndTxtRecordQuery(IAsyncResult ar, out string[] text)
			{
				return Dns.EndRetrieveTextRecords(ar, out text);
			}

			public static IAsyncResult BeginARecordQuery(string domain, AddressFamily addressFamily, AsyncCallback asyncCallback, object asyncState)
			{
				Util.PerformanceCounters.DnsQueryPerformed();
				return Util.AsyncDns.dns.BeginResolveToAddresses(domain, addressFamily, asyncCallback, asyncState);
			}

			public static DnsStatus EndARecordQuery(IAsyncResult ar, out IPAddress[] addresses)
			{
				return Dns.EndResolveToAddresses(ar, out addresses);
			}

			public static IAsyncResult BeginMxRecordQuery(string domain, AddressFamily addressFamily, AsyncCallback asyncCallback, object asyncState)
			{
				Util.PerformanceCounters.DnsQueryPerformed();
				return Util.AsyncDns.dns.BeginResolveToMailServers(domain, false, addressFamily, asyncCallback, asyncState);
			}

			public static DnsStatus EndMxRecordQuery(IAsyncResult ar, out TargetHost[] hosts)
			{
				return Dns.EndResolveToMailServers(ar, out hosts);
			}

			public static IAsyncResult BeginPtrRecordQuery(IPAddress ipAddress, AsyncCallback asyncCallback, object asyncState)
			{
				Util.PerformanceCounters.DnsQueryPerformed();
				return Util.AsyncDns.dns.BeginResolveToNames(ipAddress, asyncCallback, asyncState);
			}

			public static DnsStatus EndPtrRecordQuery(IAsyncResult ar, out string[] domains)
			{
				return Dns.EndResolveToNames(ar, out domains);
			}

			public static bool IsValidName(string name)
			{
				if (string.IsNullOrEmpty(name) || !Dns.IsValidQuestion(name))
				{
					return false;
				}
				DNSNameStatus dnsnameStatus = Dns.ValidateName(DNSNameFormat.Domain, name);
				return (dnsnameStatus == DNSNameStatus.Valid || dnsnameStatus == DNSNameStatus.NonRFCName) && !string.Equals(name.Trim(), ".", StringComparison.OrdinalIgnoreCase);
			}

			private static Dns dns = TransportFacades.Dns;
		}
	}
}
