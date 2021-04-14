using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	public sealed class QuarantineConnector
	{
		public QuarantineConnector(QuarantineFlavor flavor, int retentionDays, bool shouldEncrypt)
		{
			this.Flavor = flavor;
			this.ShouldEncrypt = shouldEncrypt;
			this.RetentionDays = retentionDays;
		}

		public QuarantineFlavor Flavor { get; private set; }

		public int RetentionDays { get; private set; }

		public bool ShouldEncrypt { get; private set; }

		public static QuarantineConnector CreateFromMessage(EmailMessage message)
		{
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "CreateFromMessage", new object[0]);
			if (message == null)
			{
				SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Fail, "CreateFromMessage message is null", new object[0]);
				throw new ArgumentNullException("message");
			}
			QuarantineConnector result = QuarantineConnector.CreateFromMessageHeaders(message.MimeDocument.RootPart.Headers);
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "CreateFromMessage: returning Quarantine Connector", new object[0]);
			return result;
		}

		public static QuarantineConnector CreateFromMessageHeaders(HeaderList headers)
		{
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "CreateFromMessageHeaders", new object[0]);
			if (headers == null)
			{
				SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Fail, "CreateFromMessageHeaders message is null", new object[0]);
				throw new ArgumentNullException("headers");
			}
			Header header = headers.FindFirst(QuarantineConnector.XHeaderName);
			if (header == null)
			{
				SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "CreateFromMessageHeaders: header not found", new object[0]);
				return null;
			}
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "CreateFromMessageHeaders: QuaraninteConnector header found: {0}:{1}", new object[]
			{
				header.Name,
				header.Value
			});
			QuarantineConnector result = QuarantineConnector.CreateFromHeader(header);
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "CreateFromMessageHeaders: returning Quarantine Connector", new object[0]);
			return result;
		}

		internal static void StripMessage(EmailMessage message)
		{
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "StripMessage", new object[0]);
			message.MimeDocument.RootPart.Headers.RemoveAll(QuarantineConnector.XHeaderName);
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "StripMessage: Stripped QuarantineConnector headers", new object[0]);
		}

		public void StampMessage(EmailMessage message)
		{
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "StampMessage", new object[0]);
			if (message == null)
			{
				SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Fail, "StampMessage: message is null", new object[0]);
				throw new ArgumentNullException("message");
			}
			QuarantineConnector quarantineConnector = QuarantineConnector.CreateFromMessage(message);
			if (quarantineConnector != null)
			{
				if (quarantineConnector.Flavor < this.Flavor)
				{
					SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "StampMessage: Existing QuarantineConnector header exists with Higher Priority {0} flavor than this {1} flavor, ignoring this stamp", new object[]
					{
						quarantineConnector.Flavor.ToString(),
						this.Flavor.ToString()
					});
					return;
				}
				SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "StampMessage: Existing QuarantineConnector header exists with Lower Priority {0} flavor than this {1} flavor, replacing with this stamp in message", new object[]
				{
					quarantineConnector.Flavor.ToString(),
					this.Flavor.ToString()
				});
				QuarantineConnector.StripMessage(message);
			}
			Header header = this.ToHeader();
			message.MimeDocument.RootPart.Headers.AppendChild(header);
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "StampMessage: Stamp QuarantineConnector header: {0}:{1}", new object[]
			{
				header.Name,
				header.Value
			});
		}

		internal static QuarantineConnector CreateFromHeader(Header header)
		{
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "CreateFromHeader", new object[0]);
			if (header == null)
			{
				SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Fail, "CreateFromHeader: header is null", new object[0]);
				throw new ArgumentNullException("header");
			}
			SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "CreateFromHeader: Processing header: {0]:{1}", new object[]
			{
				header.Name,
				header.Value
			});
			QuarantineConnector result;
			try
			{
				char[] separator = new char[]
				{
					';'
				};
				string[] array = header.Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (string text in array)
				{
					char[] separator2 = new char[]
					{
						':'
					};
					string[] array3 = text.Split(separator2, 2);
					if (array3.Length == 2)
					{
						dictionary[array3[0].Trim()] = array3[1].Trim();
					}
				}
				if (!dictionary.ContainsKey("v"))
				{
					throw new ArgumentException("Missing 'v:'");
				}
				if (dictionary["v"] != "2")
				{
					throw new ArgumentException("Unrecognized quarantine x-header version - 'v:' was not 2");
				}
				if (!dictionary.ContainsKey("f"))
				{
					throw new ArgumentException("Quarantine header missing 'f:'");
				}
				QuarantineFlavor flavor;
				if (!Enum.TryParse<QuarantineFlavor>(dictionary["f"], out flavor))
				{
					throw new ArgumentException("Quarantine header invalid 'f:'");
				}
				if (!dictionary.ContainsKey("r"))
				{
					throw new ArgumentException("Quarantine header missing 'r:'");
				}
				int retentionDays;
				if (!int.TryParse(dictionary["r"], out retentionDays))
				{
					throw new ArgumentException("Quarantine header invalid 'r:'");
				}
				if (!dictionary.ContainsKey("e"))
				{
					throw new ArgumentException("Quarantine header missing 'e:'");
				}
				bool shouldEncrypt;
				if (!bool.TryParse(dictionary["e"], out shouldEncrypt))
				{
					throw new ArgumentException("Quarantine header invalid 'e:'");
				}
				QuarantineConnector quarantineConnector = new QuarantineConnector(flavor, retentionDays, shouldEncrypt);
				SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Pass, "CreateFromHeader: returning Quarantine Connector", new object[0]);
				result = quarantineConnector;
			}
			catch (ArgumentException ex)
			{
				SystemProbe.Trace(QuarantineConnector.ComponentName, SystemProbe.Status.Fail, "CreateFromHeader: Error parsing header: {0}", new object[]
				{
					ex.Message
				});
				throw;
			}
			return result;
		}

		internal Header ToHeader()
		{
			string value = string.Format("v:2;f:{0};r:{1};e:{2}", this.Flavor, this.RetentionDays, this.ShouldEncrypt);
			return new TextHeader(QuarantineConnector.XHeaderName, value);
		}

		internal static readonly string ComponentName = "QuarantineConnector";

		public static readonly string XHeaderName = "X-MS-Exchange-Organization-Hygiene-PutInQuarantine";
	}
}
