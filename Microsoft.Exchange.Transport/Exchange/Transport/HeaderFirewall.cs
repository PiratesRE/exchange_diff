using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport
{
	internal static class HeaderFirewall
	{
		static HeaderFirewall()
		{
			HeaderFirewall.InitializeCrossPremiseHeaderLists();
		}

		public static bool PreserveOutgoingcrossPremisesHeaders(Trace tracer, HeaderList rootPartHeaders)
		{
			return HeaderFirewall.TransformCrossPremisesHeaders(tracer, rootPartHeaders, true, false);
		}

		public static bool PromoteIncomingCrossPremisesHeaders(Trace tracer, HeaderList rootPartHeaders)
		{
			return HeaderFirewall.TransformCrossPremisesHeaders(tracer, rootPartHeaders, false, true);
		}

		public static bool FilterCrossPremisesHeaders(HeaderList headers)
		{
			List<Header> list = new List<Header>();
			foreach (Header header in headers)
			{
				if (header.Name.StartsWith("X-MS-Exchange-CrossPremises-", StringComparison.OrdinalIgnoreCase))
				{
					list.Add(header);
				}
			}
			foreach (Header oldChild in list)
			{
				headers.RemoveChild(oldChild);
			}
			return list.Count != 0;
		}

		public static bool CrossPremisesHeadersPresent(HeaderList headers)
		{
			foreach (Header header in headers)
			{
				if (header.Name.StartsWith("X-MS-Exchange-CrossPremises-", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Filter(HeaderList headers, RestrictedHeaderSet blocked)
		{
			if (blocked == RestrictedHeaderSet.None)
			{
				return false;
			}
			List<Header> list = new List<Header>();
			foreach (Header header in headers)
			{
				if (HeaderFirewall.IsHeaderBlocked(header, blocked))
				{
					list.Add(header);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				headers.RemoveChild(list[i]);
			}
			return list.Count > 0;
		}

		public static bool ContainsBlockedHeaders(HeaderList headers, RestrictedHeaderSet blocked)
		{
			if (blocked == RestrictedHeaderSet.None)
			{
				return false;
			}
			foreach (Header header in headers)
			{
				if (HeaderFirewall.IsHeaderBlocked(header, blocked))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsHeaderBlocked(Header header, RestrictedHeaderSet blocked)
		{
			string name = header.Name;
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			if ((blocked & RestrictedHeaderSet.Organization) != RestrictedHeaderSet.None && name.StartsWith("X-MS-Exchange-Organization-", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if ((blocked & RestrictedHeaderSet.Forest) != RestrictedHeaderSet.None && name.StartsWith("X-MS-Exchange-Forest-", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if ((blocked & RestrictedHeaderSet.MTA) != RestrictedHeaderSet.None)
			{
				for (int i = 0; i < HeaderFirewall.MtaOnlyHeaders.Length; i++)
				{
					if (name.Equals(HeaderFirewall.MtaOnlyHeaders[i], StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
				for (int j = 0; j < HeaderFirewall.MtaOnlyNamespaces.Length; j++)
				{
					if (name.StartsWith(HeaderFirewall.MtaOnlyNamespaces[j], StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void InitializeCrossPremiseHeaderLists()
		{
			HeaderFirewall.orgHeadersToPreserve = new Dictionary<string, Func<Header, bool>>(StringComparer.OrdinalIgnoreCase);
			HeaderFirewall.orgHeaderPrefixesToPreserve = new List<string>();
			HeaderFirewall.crossPremisesHeadersToPromote = new Dictionary<string, Func<Header, bool>>(StringComparer.OrdinalIgnoreCase);
			HeaderFirewall.crossPremisesHeaderPrefixesToPromote = new List<string>();
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-PCL");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-SCL");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Antispam-Report");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Original-SCL");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Original-Sender");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-PRD");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Quarantine");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-SenderIdResult");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Antispam-AsyncContext");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Antispam-ScanContext", null, new Func<Header, bool>(HeaderFirewall.AntiSpamContextPromoteHandler));
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Antispam-IPv6Check");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Hygiene-ReleasedFromQuarantine");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Approval-Allowed-Actions");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Approval-Requestor");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Approval-Initiator");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Approval-AttachToApprovalRequest");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Approval-Approved");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Moderation-Data");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Bypass-Child-Moderation");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Moderation-SavedArrivalTime");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-AuthAs");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-AuthSource");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-AuthDomain");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-AuthMechanism");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-History");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-DeliveryFolder");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Dsn-Version");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-StorageQuota");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Mapi-Admin-Submission");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Recipient-P2-Type");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Journal-Report");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Journaled-To-Recipients");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Journaling-Remote-Accounts");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Do-Not-Journal");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Processed-By-Journaling");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-BCC");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-AutoForwarded");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Original-Received-Time");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Disclaimer-Hash");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Classification");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-MessageSource");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-OriginalClientIPAddress");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-OutboundConnector");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-AVStamp-Service");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Text-Messaging-Mapi-Class");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Text-Messaging-Originator");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Text-Messaging-Count-Of-Settings-Segments");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Text-Messaging-Timestamp");
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists("X-MS-Exchange-Organization-Text-Messaging-Notification-PreferredCulture");
			HeaderFirewall.AddHeaderPrefixToCrossPremisesHeaderPrefixLists("X-MS-Exchange-Organization-Text-Messaging-Settings-Segment-");
		}

		private static void AddHeaderToCrossPremisesHeaderLists(string orgHeaderName)
		{
			HeaderFirewall.AddHeaderToCrossPremisesHeaderLists(orgHeaderName, null, null);
		}

		private static void AddHeaderToCrossPremisesHeaderLists(string orgHeaderName, Func<Header, bool> preserveDelegate, Func<Header, bool> promoteDelegate)
		{
			HeaderFirewall.orgHeadersToPreserve.Add(orgHeaderName, preserveDelegate);
			string oldValue = orgHeaderName.Substring(0, "X-MS-Exchange-Organization-".Length);
			string key = orgHeaderName.Replace(oldValue, "X-MS-Exchange-CrossPremises-");
			HeaderFirewall.crossPremisesHeadersToPromote.Add(key, promoteDelegate);
		}

		private static void AddHeaderPrefixToCrossPremisesHeaderPrefixLists(string orgHeaderPrefix)
		{
			HeaderFirewall.orgHeaderPrefixesToPreserve.Add(orgHeaderPrefix);
			string oldValue = orgHeaderPrefix.Substring(0, "X-MS-Exchange-Organization-".Length);
			string item = orgHeaderPrefix.Replace(oldValue, "X-MS-Exchange-CrossPremises-");
			HeaderFirewall.crossPremisesHeaderPrefixesToPromote.Add(item);
		}

		private static bool TransformCrossPremisesHeaders(Trace tracer, HeaderList rootPartHeaders, bool outgoing, bool removeExisting)
		{
			string text;
			string newValue;
			if (outgoing)
			{
				text = "X-MS-Exchange-Organization-";
				newValue = "X-MS-Exchange-CrossPremises-";
			}
			else
			{
				text = "X-MS-Exchange-CrossPremises-";
				newValue = "X-MS-Exchange-Organization-";
			}
			List<Header> list = new List<Header>();
			foreach (Header header in rootPartHeaders)
			{
				Func<Header, bool> func;
				if (HeaderFirewall.ShouldTransformCrossPremisesHeader(tracer, header, outgoing, out func))
				{
					string oldValue = header.Name.Substring(0, text.Length);
					string name = header.Name.Replace(oldValue, newValue);
					Header header2 = Header.Create(name);
					header.CopyTo(header2);
					if (func == null || func(header2))
					{
						list.Add(header2);
					}
				}
			}
			foreach (Header header3 in list)
			{
				if (removeExisting)
				{
					rootPartHeaders.RemoveAll(header3.Name);
				}
				rootPartHeaders.AppendChild(header3);
			}
			return list.Count != 0;
		}

		private static bool ShouldTransformCrossPremisesHeader(Trace tracer, Header header, bool outgoing, out Func<Header, bool> transformedHeaderHandler)
		{
			transformedHeaderHandler = null;
			CrossPremisesHeaderTransformation crossPremisesHeaderTransformation = HeaderFirewall.crossPremisesHeaderTransformationMode;
			Dictionary<string, Func<Header, bool>> dictionary;
			List<string> prefixList;
			string value;
			if (outgoing)
			{
				dictionary = HeaderFirewall.orgHeadersToPreserve;
				prefixList = HeaderFirewall.orgHeaderPrefixesToPreserve;
				value = "X-MS-Exchange-Organization-";
			}
			else
			{
				dictionary = HeaderFirewall.crossPremisesHeadersToPromote;
				prefixList = HeaderFirewall.crossPremisesHeaderPrefixesToPromote;
				value = "X-MS-Exchange-CrossPremises-";
			}
			bool result = false;
			if (dictionary.TryGetValue(header.Name, out transformedHeaderHandler))
			{
				tracer.TraceDebug<string, string, CrossPremisesHeaderTransformation>(0L, "{0} header {1} because filtering mode={2} and it is in the transformation list", outgoing ? "Preserving organization" : "Promoting cross premises", header.Name, crossPremisesHeaderTransformation);
				result = true;
			}
			else if (crossPremisesHeaderTransformation == CrossPremisesHeaderTransformation.Lenient && header.Name.StartsWith(value, StringComparison.OrdinalIgnoreCase))
			{
				tracer.TraceDebug<string, string, CrossPremisesHeaderTransformation>(0L, "{0} header {1} because filtering mode={2}", outgoing ? "Preserving organization" : "Promoting cross premises", header.Name, crossPremisesHeaderTransformation);
				result = true;
			}
			else if (HeaderFirewall.HeaderStartsWithPrefix(header.Name, prefixList))
			{
				tracer.TraceDebug<string, string, CrossPremisesHeaderTransformation>(0L, "{0} header {1} because filtering mode={2} and it is in the transformation prefix list", outgoing ? "Preserving organization" : "Promoting cross premises", header.Name, crossPremisesHeaderTransformation);
				result = true;
			}
			return result;
		}

		private static bool HeaderStartsWithPrefix(string headerName, List<string> prefixList)
		{
			foreach (string value in prefixList)
			{
				if (headerName.StartsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsCrossPremisesHeader(string headerName)
		{
			return !string.IsNullOrEmpty(headerName) && headerName.StartsWith("X-MS-Exchange-CrossPremises-", StringComparison.OrdinalIgnoreCase);
		}

		private static bool AntiSpamContextPromoteHandler(Header header)
		{
			string text = header.Value;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (text.IndexOf(MimeConstant.AntispamScanContextXPremTagNameWithSeparator, StringComparison.OrdinalIgnoreCase) == -1)
			{
				if (text[text.Length - 1] != ';')
				{
					text += ';';
				}
				header.Value = string.Concat(new object[]
				{
					text,
					MimeConstant.AntispamScanContextXPremTagNameWithSeparator,
					"true",
					';'
				});
			}
			return true;
		}

		public const string OrganizationNamespace = "X-MS-Exchange-Organization-";

		public const string ForestNamespace = "X-MS-Exchange-Forest-";

		public const string CrossPremisesNamespace = "X-MS-Exchange-CrossPremises-";

		public const string DiagnosticHeadersFilteredHeader = "X-CrossPremisesHeadersFilteredBySendConnector";

		public static readonly string[] MtaOnlyHeaders = new string[]
		{
			"Received"
		};

		public static readonly string[] MtaOnlyNamespaces = new string[]
		{
			"Resent-"
		};

		public static readonly string ComputerName = ComputerInformation.DnsPhysicalFullyQualifiedDomainName;

		private static Dictionary<string, Func<Header, bool>> orgHeadersToPreserve;

		private static Dictionary<string, Func<Header, bool>> crossPremisesHeadersToPromote;

		private static List<string> orgHeaderPrefixesToPreserve;

		private static List<string> crossPremisesHeaderPrefixesToPromote;

		private static readonly CrossPremisesHeaderTransformation crossPremisesHeaderTransformationMode = VariantConfiguration.InvariantNoFlightingSnapshot.Transport.StringentHeaderTransformationMode.Enabled ? CrossPremisesHeaderTransformation.Stringent : CrossPremisesHeaderTransformation.Lenient;

		public class OutputFilter : MimeOutputFilter
		{
			public OutputFilter(RestrictedHeaderSet blocked) : this(blocked, false, null)
			{
			}

			public OutputFilter(RestrictedHeaderSet blocked, bool downconvert) : this(blocked, downconvert, null)
			{
			}

			public OutputFilter(RestrictedHeaderSet blocked, bool downconvert, Header diagHeader)
			{
				this.blocked = blocked;
				this.diagHeader = diagHeader;
				this.isdiagHeaderAdded = false;
				if (downconvert)
				{
					this.eightToSevenBitFilter = new EightToSevenBitConverter.OutputFilter();
				}
			}

			public override bool FilterPart(MimePart part, Stream stream)
			{
				if (this.partCount < 2)
				{
					this.partCount++;
				}
				return false;
			}

			public override bool FilterPartBody(MimePart part, Stream stream)
			{
				return this.eightToSevenBitFilter != null && this.eightToSevenBitFilter.FilterPartBody(part, stream);
			}

			public override bool FilterHeaderList(HeaderList headers, Stream stream)
			{
				if (this.eightToSevenBitFilter == null)
				{
					return false;
				}
				if (this.partCount > 1)
				{
					return this.eightToSevenBitFilter.FilterHeaderList(headers, stream);
				}
				HeaderList headerList = (HeaderList)headers.Clone();
				bool flag = false;
				if (this.diagHeader != null)
				{
					flag = HeaderFirewall.FilterCrossPremisesHeaders(headerList);
					if (!this.isdiagHeaderAdded)
					{
						headerList.AppendChild(this.diagHeader);
						this.isdiagHeaderAdded = true;
					}
				}
				flag = (HeaderFirewall.Filter(headerList, this.blocked) || flag);
				if (this.eightToSevenBitFilter.FilterHeaderList(headerList, stream))
				{
					return true;
				}
				if (!flag)
				{
					return false;
				}
				headerList.WriteTo(stream);
				stream.Write(HeaderFirewall.OutputFilter.CrLf, 0, HeaderFirewall.OutputFilter.CrLf.Length);
				return true;
			}

			public override bool FilterHeader(Header header, Stream stream)
			{
				if (this.partCount > 1)
				{
					return false;
				}
				if (this.eightToSevenBitFilter != null)
				{
					return false;
				}
				bool flag = false;
				if (this.diagHeader != null)
				{
					flag = HeaderFirewall.IsCrossPremisesHeader(header.Name);
					if (!this.isdiagHeaderAdded)
					{
						this.diagHeader.WriteTo(stream);
						this.isdiagHeaderAdded = true;
					}
				}
				return HeaderFirewall.IsHeaderBlocked(header, this.blocked) || flag;
			}

			private static readonly byte[] CrLf = new byte[]
			{
				13,
				10
			};

			private RestrictedHeaderSet blocked;

			private int partCount;

			private EightToSevenBitConverter.OutputFilter eightToSevenBitFilter;

			private bool isdiagHeaderAdded;

			private Header diagHeader;
		}

		internal static class TextMessagingHeaders
		{
			public const string XheaderTextMessagingMapiClass = "X-MS-Exchange-Organization-Text-Messaging-Mapi-Class";

			public const string XheaderTextMessagingOriginator = "X-MS-Exchange-Organization-Text-Messaging-Originator";

			public const string XheaderTextMessagingCountOfSettingsSegments = "X-MS-Exchange-Organization-Text-Messaging-Count-Of-Settings-Segments";

			public const string XheaderTextMessagingSettingsSegmentPrefix = "X-MS-Exchange-Organization-Text-Messaging-Settings-Segment-";

			public const string XheaderTextMessagingTimestamp = "X-MS-Exchange-Organization-Text-Messaging-Timestamp";

			public const string XheaderTextMessagingNotificationPreferredCulture = "X-MS-Exchange-Organization-Text-Messaging-Notification-PreferredCulture";
		}
	}
}
