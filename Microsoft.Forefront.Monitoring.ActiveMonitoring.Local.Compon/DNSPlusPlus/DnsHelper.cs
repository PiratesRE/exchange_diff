using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal static class DnsHelper
	{
		public static bool ProcessDNSRequest(string requestedDomain, RecordType queryType, RecordClass queryClass, IPEndPoint server, TimeSpan sla, TimeSpan requestTimeout, int retry, QueryResponseCode expectedResponseCode, bool hasIpV6Prefix, out string errorMessage, CancellationToken cancellationToken = default(CancellationToken))
		{
			Question question = new Question();
			int num = Interlocked.Increment(ref DnsHelper.requestId);
			ushort num2 = (ushort)((num > 65535) ? (num >> 16) : num);
			byte[] queryBytes = question.GetQueryBytes((int)num2, requestedDomain, queryType, queryClass);
			byte[] array = null;
			Socket socket = null;
			ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), "ProcessDNSRequest Start:requestedDomain={0}, QueryType={1}, QueryClass={2}, IPEndPoint={3}, sla(s)={4}, retry={5}, requestTimeout(s)={6}, ExpectedResponseCode={7}, Requestid={8}", new object[]
			{
				requestedDomain,
				queryType,
				queryClass,
				server.Address.ToString(),
				sla.TotalSeconds,
				retry,
				requestTimeout.TotalSeconds,
				expectedResponseCode,
				num2
			});
			Stopwatch stopwatch = new Stopwatch();
			for (int i = 0; i <= retry; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					throw new OperationCanceledException(cancellationToken);
				}
				try
				{
					stopwatch.Restart();
					ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), "ProcessDNSRequest Creating socket");
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					int optionValue = (int)requestTimeout.TotalMilliseconds;
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, optionValue);
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, optionValue);
					array = new byte[512];
					ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), "ProcessDNSRequest Sending request");
					socket.SendTo(queryBytes, queryBytes.Length, SocketFlags.None, server);
					ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), "ProcessDNSRequest Receiving response");
					socket.Receive(array);
					stopwatch.Stop();
					break;
				}
				catch (SocketException ex)
				{
					if (i == retry)
					{
						ExTraceGlobals.DNSTracer.TraceError<int, SocketError>((long)"DnsHelper".GetHashCode(), "ProcessDNSRequest failed with SocketException after {0} tries, SocketErroCode={1}", i + 1, ex.SocketErrorCode);
						throw new DnsMonitorException("ProcessDNSRequest failed(see innerexception)", ex);
					}
				}
				finally
				{
					if (socket != null)
					{
						socket.Close();
					}
				}
			}
			try
			{
				errorMessage = null;
				ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), "ProcessDNSRequest Validating response");
				Response response;
				if (DnsHelper.IsExpectedResponse(array, queryType, num2, expectedResponseCode, hasIpV6Prefix, out errorMessage, out response) && stopwatch.Elapsed > sla)
				{
					errorMessage = string.Format("Failed to meet sla(ms)={0}, actualTimeTaken(ms)={1}", sla.TotalMilliseconds, stopwatch.ElapsedMilliseconds);
					ExTraceGlobals.DNSTracer.Information<string>((long)"DnsHelper".GetHashCode(), "ProcessDNSRequest {0}", errorMessage);
				}
				string message = string.Format("ProcessDNSRequest Response Details, RequestId={0}, TimeTaken(ms)={1}, AnswerCount={2}, NameserverCount={3}, ResponseCode={4}, Answers={5}, AuthorityRecords={6}", new object[]
				{
					num2,
					stopwatch.ElapsedMilliseconds,
					response.AnswerCount,
					response.NameserverCount,
					response.ResponseCode,
					string.Join<Answer>(";", response.Answers),
					string.Join<Answer>(";", response.NSServers)
				});
				if (errorMessage == null)
				{
					ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), message);
				}
				else
				{
					ExTraceGlobals.DNSTracer.TraceWarning((long)"DnsHelper".GetHashCode(), message);
				}
			}
			catch (FormatException arg)
			{
				errorMessage = string.Format("Format error occured while processing requestId={0}, exception={1}, responseMessageBytes={2}", num2, arg, DnsHelper.DumpBytes(array, 0, array.Length - 1));
				ExTraceGlobals.DNSTracer.TraceWarning((long)"DnsHelper".GetHashCode(), "ProcessDNSRequest format error occured");
			}
			return errorMessage == null;
		}

		public static byte GetByte(byte[] message, int position)
		{
			return message[position];
		}

		public static ushort GetUShort(byte[] message, int position)
		{
			return (ushort)((int)message[position] << 8 | (int)message[position + 1]);
		}

		public static int GetInt(byte[] message, int position)
		{
			return ((int)message[position] << 24) + ((int)message[position + 1] << 16) + ((int)message[position + 2] << 8) + (int)message[position + 3];
		}

		public static string ReadDomain(byte[] message, ref int position)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = position;
			int i;
			while ((i = (int)DnsHelper.GetByte(message, num)) != 0)
			{
				if ((i & 192) == 192)
				{
					int num2 = (i & 63) << 8 | (int)DnsHelper.GetByte(message, num + 1);
					stringBuilder.Append(DnsHelper.ReadDomain(message, ref num2));
				}
				while (i > 0)
				{
					num++;
					stringBuilder.Append((char)DnsHelper.GetByte(message, num));
					i--;
				}
				num++;
				if (DnsHelper.GetByte(message, num) != 0)
				{
					stringBuilder.Append('.');
				}
			}
			num++;
			position = num;
			return stringBuilder.ToString();
		}

		public static string DumpBytes(byte[] message, int startIndex, int endIndex)
		{
			StringBuilder stringBuilder = new StringBuilder(endIndex - startIndex + 1);
			for (int i = startIndex; i <= endIndex; i++)
			{
				stringBuilder.Append(DnsHelper.Ascii[(int)message[i]]);
			}
			return stringBuilder.ToString();
		}

		private static bool IsExpectedResponse(byte[] responseMessage, RecordType queryType, ushort expectedRequestId, QueryResponseCode expectedResponseCode, bool hasIpV6Prefix, out string errorMessage, out Response response)
		{
			errorMessage = null;
			response = new Response(responseMessage);
			ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), "Checking responseId and responseCode");
			if (response.ResponseId != expectedRequestId)
			{
				errorMessage = string.Format("RequestId doesnot match in the response, expected:{0}, actual:{1}", expectedRequestId, response.ResponseId);
				ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), errorMessage);
			}
			else if (response.ResponseCode != expectedResponseCode)
			{
				errorMessage = string.Format("Unexpected response code, expected:{0}, actual:{1}", expectedResponseCode, response.ResponseCode);
				ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), errorMessage);
			}
			else if (response.ResponseCode == QueryResponseCode.Success)
			{
				ExTraceGlobals.DNSTracer.Information<RecordType>((long)"DnsHelper".GetHashCode(), "responseId and responseCode check passed, checking Answercount for queryType={0}", queryType);
				switch (queryType)
				{
				case RecordType.A:
				{
					int num = (from answer in response.Answers
					select answer.RecordType == RecordType.A).Count<bool>();
					if (!hasIpV6Prefix)
					{
						if (num == 0)
						{
							errorMessage = string.Format("Unexpected ARecord count, expected:>0, actual:{0}", num);
						}
					}
					else if (num != 0)
					{
						errorMessage = string.Format("Unexpected ARecord count, expected:0, actual:{0}", num);
					}
					break;
				}
				case RecordType.NS:
					if ((from answer in response.Answers
					select answer.RecordType == RecordType.NS).Count<bool>() == 0)
					{
						errorMessage = "Unexpected NSRecord count, expected:>0, actual:0";
					}
					break;
				default:
					if (queryType != RecordType.SOA)
					{
						if (queryType != RecordType.AAAA)
						{
							if (response.AnswerCount > 0)
							{
								errorMessage = string.Format("Got answers for a non-supported record type(={1})", queryType);
							}
						}
						else
						{
							int num2 = (from answer in response.Answers
							select answer.RecordType == RecordType.AAAA).Count<bool>();
							if (hasIpV6Prefix)
							{
								if (num2 == 0)
								{
									errorMessage = string.Format("Unexpected AAAARecord count, expected:>0, actual:{0}", num2);
								}
							}
							else if (num2 != 0)
							{
								errorMessage = string.Format("Unexpected AAAARecord count, expected:0, actual:{0}", num2);
							}
						}
					}
					else if ((from answer in response.NSServers
					select answer.RecordType == RecordType.SOA).Count<bool>() == 0)
					{
						errorMessage = "Unexpected SOARecord count, expected:>0, actual:0";
					}
					break;
				}
			}
			if (errorMessage == null)
			{
				ExTraceGlobals.DNSTracer.Information((long)"DnsHelper".GetHashCode(), "Response check passed");
				return true;
			}
			ExTraceGlobals.DNSTracer.Information<string>((long)"DnsHelper".GetHashCode(), "Response check failed, message={0}", errorMessage);
			return false;
		}

		private static readonly string[] Ascii = new string[]
		{
			"00",
			"01",
			"02",
			"03",
			"04",
			"05",
			"06",
			"07",
			"08",
			"09",
			"0A",
			"0B",
			"0C",
			"0D",
			"0E",
			"0F",
			"10",
			"11",
			"12",
			"13",
			"14",
			"15",
			"16",
			"17",
			"18",
			"19",
			"1A",
			"1B",
			"1C",
			"1D",
			"1E",
			"1F",
			"20",
			"21",
			"22",
			"23",
			"24",
			"25",
			"26",
			"27",
			"28",
			"29",
			"2A",
			"2B",
			"2C",
			"2D",
			"2E",
			"2F",
			"30",
			"31",
			"32",
			"33",
			"34",
			"35",
			"36",
			"37",
			"38",
			"39",
			"3A",
			"3B",
			"3C",
			"3D",
			"3E",
			"3F",
			"40",
			"41",
			"42",
			"43",
			"44",
			"45",
			"46",
			"47",
			"48",
			"49",
			"4A",
			"4B",
			"4C",
			"4D",
			"4E",
			"4F",
			"50",
			"51",
			"52",
			"53",
			"54",
			"55",
			"56",
			"57",
			"58",
			"59",
			"5A",
			"5B",
			"5C",
			"5D",
			"5E",
			"5F",
			"60",
			"61",
			"62",
			"63",
			"64",
			"65",
			"66",
			"67",
			"68",
			"69",
			"6A",
			"6B",
			"6C",
			"6D",
			"6E",
			"6F",
			"70",
			"71",
			"72",
			"73",
			"74",
			"75",
			"76",
			"77",
			"78",
			"79",
			"7A",
			"7B",
			"7C",
			"7D",
			"7E",
			"7F",
			"80",
			"81",
			"82",
			"83",
			"84",
			"85",
			"86",
			"87",
			"88",
			"89",
			"8A",
			"8B",
			"8C",
			"8D",
			"8E",
			"8F",
			"90",
			"91",
			"92",
			"93",
			"94",
			"95",
			"96",
			"97",
			"98",
			"99",
			"9A",
			"9B",
			"9C",
			"9D",
			"9E",
			"9F",
			"A0",
			"A1",
			"A2",
			"A3",
			"A4",
			"A5",
			"A6",
			"A7",
			"A8",
			"A9",
			"AA",
			"AB",
			"AC",
			"AD",
			"AE",
			"AF",
			"B0",
			"B1",
			"B2",
			"B3",
			"B4",
			"B5",
			"B6",
			"B7",
			"B8",
			"B9",
			"BA",
			"BB",
			"BC",
			"BD",
			"BE",
			"BF",
			"C0",
			"C1",
			"C2",
			"C3",
			"C4",
			"C5",
			"C6",
			"C7",
			"C8",
			"C9",
			"CA",
			"CB",
			"CC",
			"CD",
			"CE",
			"CF",
			"D0",
			"D1",
			"D2",
			"D3",
			"D4",
			"D5",
			"D6",
			"D7",
			"D8",
			"D9",
			"DA",
			"DB",
			"DC",
			"DD",
			"DE",
			"DF",
			"E0",
			"E1",
			"E2",
			"E3",
			"E4",
			"E5",
			"E6",
			"E7",
			"E8",
			"E9",
			"EA",
			"EB",
			"EC",
			"ED",
			"EE",
			"EF",
			"F0",
			"F1",
			"F2",
			"F3",
			"F4",
			"F5",
			"F6",
			"F7",
			"F8",
			"F9",
			"FA",
			"FB",
			"FC",
			"FD",
			"FE",
			"FF"
		};

		private static int requestId = 0;
	}
}
