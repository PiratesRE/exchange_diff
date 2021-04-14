using System;
using System.Net;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.SenderId;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class SenderIdValidator
	{
		public SenderIdValidator(SmtpServer server)
		{
			this.server = server;
			this.cachedResults = new CachedSenderIdResults();
		}

		public static RoutingAddress GetPurportedResponsibleAddress(HeaderList headers)
		{
			Header header = SenderIdValidator.SelectPRAHeader(headers);
			return SenderIdValidator.ExtractPRAFromHeader(header);
		}

		public IAsyncResult BeginCheckHost(IPAddress ipAddress, RoutingAddress purportedResponsibleAddress, string helloDomain, bool processExpModifier, AsyncCallback asyncCallback, object asyncState)
		{
			if (!purportedResponsibleAddress.IsValid)
			{
				SenderIdResult result = new SenderIdResult(SenderIdStatus.Fail, SenderIdFailReason.MalformedDomain);
				return new SenderIdAsyncResult(asyncCallback, asyncState, result);
			}
			if (Util.AsyncDns.IsDnsServerListEmpty())
			{
				SenderIdResult result2 = new SenderIdResult(SenderIdStatus.TempError);
				return new SenderIdAsyncResult(asyncCallback, asyncState, result2);
			}
			SenderIdValidationBaseContext senderIdValidationBaseContext = new SenderIdValidationBaseContext(this, ipAddress, purportedResponsibleAddress, helloDomain, this.server);
			SenderIdValidationContext context = senderIdValidationBaseContext.CreateContext(purportedResponsibleAddress.DomainPart, processExpModifier, asyncCallback, asyncState);
			return this.BeginCheckHost(context);
		}

		public SenderIdResult EndCheckHost(IAsyncResult ar)
		{
			SenderIdResult senderIdResult = ((SenderIdAsyncResult)ar).GetResult() as SenderIdResult;
			if (senderIdResult == null)
			{
				throw new ArgumentException("Argument must be obtained by calling BeginCheckHost()", "ar");
			}
			return senderIdResult;
		}

		internal void ValidationCompleted(SenderIdValidationContext context, SenderIdResult result)
		{
			if (!context.BaseContext.UsesUncacheableMacro)
			{
				this.cachedResults.SaveResult(context.PurportedResponsibleDomain, context.BaseContext.IPAddress, result);
			}
		}

		internal IAsyncResult BeginCheckHost(SenderIdValidationContext context, string purportedResponsibleDomain, bool processExpModifier, AsyncCallback asyncCallback, object asyncState)
		{
			SenderIdValidationContext context2 = context.BaseContext.CreateContext(purportedResponsibleDomain, processExpModifier, asyncCallback, asyncState);
			return this.BeginCheckHost(context2);
		}

		internal IAsyncResult BeginCheckHost(SenderIdValidationContext context)
		{
			if (context.IsValid)
			{
				SenderIdResult cachedResult = this.cachedResults.GetCachedResult(context.PurportedResponsibleDomain, context.BaseContext.IPAddress);
				if (cachedResult != null)
				{
					ExTraceGlobals.ValidationTracer.TraceDebug<SenderIdStatus, string, IPAddress>((long)this.GetHashCode(), "Using cached result of {0} for domain={1}, IP={2}", cachedResult.Status, context.PurportedResponsibleDomain, context.BaseContext.IPAddress);
					context.ValidationCompleted(cachedResult);
				}
				else if (!Util.AsyncDns.IsValidName(context.PurportedResponsibleDomain))
				{
					context.ValidationCompleted(SenderIdStatus.PermError);
				}
				else
				{
					Util.AsyncDns.BeginTxtRecordQuery(context.PurportedResponsibleDomain, new AsyncCallback(this.TxtCallback), context);
				}
			}
			else
			{
				context.ValidationCompleted(SenderIdStatus.PermError);
			}
			return context.ClientAR;
		}

		private static Header SelectPRAHeader(HeaderList headers)
		{
			if (headers == null)
			{
				throw new ArgumentNullException("headers");
			}
			Header header = null;
			Header header2 = null;
			Header header3 = null;
			Header header4 = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (Header header5 in headers)
			{
				try
				{
					if (!header5.HasChildren && header5.Value == null)
					{
						continue;
					}
				}
				catch (InvalidCharsetException)
				{
					continue;
				}
				HeaderId headerId = header5.HeaderId;
				switch (headerId)
				{
				case HeaderId.Received:
					goto IL_9B;
				case HeaderId.Date:
				case HeaderId.Subject:
					break;
				case HeaderId.From:
					if (header4 == null)
					{
						header4 = header5;
					}
					else
					{
						flag3 = true;
					}
					break;
				case HeaderId.Sender:
					if (header3 == null)
					{
						header3 = header5;
					}
					else
					{
						flag2 = true;
					}
					break;
				default:
					if (headerId == HeaderId.ReturnPath)
					{
						goto IL_9B;
					}
					switch (headerId)
					{
					case HeaderId.ResentSender:
						header = header5;
						flag = true;
						break;
					case HeaderId.ResentFrom:
						header2 = header5;
						break;
					}
					break;
				}
				IL_BB:
				if (!flag)
				{
					continue;
				}
				break;
				IL_9B:
				if (header2 != null)
				{
					flag = true;
					goto IL_BB;
				}
				goto IL_BB;
			}
			Header result = null;
			if (header != null)
			{
				result = header;
			}
			else if (header2 != null)
			{
				result = header2;
			}
			else if (header3 != null)
			{
				if (!flag2)
				{
					result = header3;
				}
			}
			else if (header4 != null && !flag3)
			{
				result = header4;
			}
			return result;
		}

		private static RoutingAddress ExtractPRAFromHeader(Header header)
		{
			RoutingAddress routingAddress = SenderIdValidator.NoPRA;
			if (header != null)
			{
				foreach (AddressItem addressItem in ((AddressHeader)header))
				{
					MimeRecipient mimeRecipient = addressItem as MimeRecipient;
					if (mimeRecipient != null)
					{
						if (routingAddress != SenderIdValidator.NoPRA)
						{
							return SenderIdValidator.NoPRA;
						}
						routingAddress = (RoutingAddress)mimeRecipient.Email;
					}
					else
					{
						MimeGroup mimeGroup = addressItem as MimeGroup;
						if (mimeGroup != null)
						{
							foreach (MimeRecipient mimeRecipient2 in mimeGroup)
							{
								if (routingAddress != SenderIdValidator.NoPRA)
								{
									return SenderIdValidator.NoPRA;
								}
								routingAddress = (RoutingAddress)mimeRecipient2.Email;
							}
						}
					}
				}
				return routingAddress;
			}
			return routingAddress;
		}

		private static string SelectSpfRecordForPRA(string[] txtRecords, out bool tooManyRecordsFound)
		{
			string text = null;
			bool flag = false;
			tooManyRecordsFound = false;
			for (int i = 0; i < txtRecords.Length; i++)
			{
				string text2 = txtRecords[i].ToLowerInvariant().TrimStart(new char[0]);
				if (!flag && text2.StartsWith("v=spf1", StringComparison.OrdinalIgnoreCase) && (text2.Length == "v=spf1".Length || text2["v=spf1".Length] == ' '))
				{
					if (!string.IsNullOrEmpty(text))
					{
						tooManyRecordsFound = true;
					}
					text = text2;
				}
				if (text2.StartsWith("spf2.", StringComparison.OrdinalIgnoreCase) && text2.Length > "spf2.".Length && char.IsDigit(text2, "spf2.".Length))
				{
					int num = "spf2.".Length + 1;
					while (num < text2.Length && char.IsDigit(text2, num))
					{
						num++;
					}
					if (num < text2.Length && text2[num] == '/')
					{
						string text3 = text2.Substring(num + 1);
						num = text3.IndexOf(' ');
						if (num >= 0)
						{
							text3 = text3.Substring(0, num);
						}
						string[] array = text3.Split(new char[]
						{
							','
						});
						foreach (string text4 in array)
						{
							if (text4.Equals("pra", StringComparison.OrdinalIgnoreCase))
							{
								if (!flag)
								{
									tooManyRecordsFound = false;
									flag = true;
								}
								else if (!string.IsNullOrEmpty(text))
								{
									tooManyRecordsFound = true;
									return text;
								}
								text = text2;
								break;
							}
						}
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			int num2 = text.IndexOf(' ');
			if (num2 == -1)
			{
				return string.Empty;
			}
			return text.Substring(num2);
		}

		private static void ProcessSpfRecord(string spfRecordString, SenderIdValidationContext context)
		{
			SpfTerm spfTerm = SpfParser.ParseSpfRecord(context, spfRecordString);
			if (context.IsValid)
			{
				spfTerm.Process();
				return;
			}
			context.ValidationCompleted(SenderIdStatus.PermError);
		}

		private void TxtCallback(IAsyncResult ar)
		{
			string[] txtRecords;
			DnsStatus dnsStatus = Util.AsyncDns.EndTxtRecordQuery(ar, out txtRecords);
			SenderIdValidationContext senderIdValidationContext = (SenderIdValidationContext)ar.AsyncState;
			switch (dnsStatus)
			{
			case DnsStatus.Success:
			case DnsStatus.InfoNoRecords:
			{
				bool flag;
				string text = SenderIdValidator.SelectSpfRecordForPRA(txtRecords, out flag);
				if (text == null)
				{
					ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "SPF record not found");
					senderIdValidationContext.ValidationCompleted(SenderIdStatus.None);
					return;
				}
				if (flag)
				{
					ExTraceGlobals.ValidationTracer.TraceDebug((long)this.GetHashCode(), "More than one SPF record found, unable to proceed");
					senderIdValidationContext.ValidationCompleted(SenderIdStatus.PermError);
					return;
				}
				SenderIdValidator.ProcessSpfRecord(text, senderIdValidationContext);
				return;
			}
			case DnsStatus.InfoDomainNonexistent:
				senderIdValidationContext.ValidationCompleted(SenderIdStatus.Fail, SenderIdFailReason.DomainDoesNotExist);
				return;
			default:
				senderIdValidationContext.ValidationCompleted(SenderIdStatus.TempError);
				return;
			}
		}

		public static readonly RoutingAddress NoPRA = new RoutingAddress("invalid address");

		private SmtpServer server;

		private CachedSenderIdResults cachedResults;
	}
}
