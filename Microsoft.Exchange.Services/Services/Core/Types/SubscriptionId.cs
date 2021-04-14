using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class SubscriptionId
	{
		public SubscriptionId(Guid subscriptionGuid) : this(subscriptionGuid, LocalServer.GetServer().Fqdn)
		{
		}

		public SubscriptionId(Guid subscriptionGuid, string serverFQDN)
		{
			this.mailboxGuid = Guid.Empty;
			base..ctor();
			this.serverFQDN = serverFQDN;
			this.subscriptionGuid = subscriptionGuid;
			this.timeCreated = ExDateTime.UtcNow;
		}

		public SubscriptionId(Guid subscriptionGuid, Guid mailboxGuid) : this(subscriptionGuid)
		{
			this.mailboxGuid = mailboxGuid;
		}

		private SubscriptionId(Guid subscriptionGuid, string serverFQDN, ExDateTime timeCreated, Guid mailboxGuid)
		{
			this.mailboxGuid = Guid.Empty;
			base..ctor();
			this.subscriptionGuid = subscriptionGuid;
			this.serverFQDN = serverFQDN;
			this.timeCreated = timeCreated;
			this.mailboxGuid = mailboxGuid;
		}

		public static SubscriptionId Parse(string idAndCASString)
		{
			SubscriptionId result;
			try
			{
				byte[] buffer = Convert.FromBase64String(idAndCASString);
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						ushort num = binaryReader.ReadUInt16();
						if (num > 2048)
						{
							ExTraceGlobals.SubscriptionsTracer.TraceDebug(0L, "[SubscriptionId::Parse] FQDN length in id exceeded limits.");
							throw new InvalidSubscriptionException();
						}
						if (memoryStream.Length != (long)((int)num + SubscriptionId.GuidSize + 2 + 4 + 8) && memoryStream.Length != (long)((int)num + SubscriptionId.GuidSize + SubscriptionId.GuidSize + 2 + 4 + 4 + 8))
						{
							ExTraceGlobals.SubscriptionsTracer.TraceDebug(0L, "[SubscriptionId::Parse] Id buffer had an unexcepted length.");
							throw new InvalidSubscriptionException();
						}
						byte[] array = binaryReader.ReadBytes((int)num);
						string @string = CTSGlobals.AsciiEncoding.GetString(array, 0, array.Length);
						int count = binaryReader.ReadInt32();
						byte[] b = binaryReader.ReadBytes(count);
						ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, binaryReader.ReadInt64());
						Guid empty = Guid.Empty;
						if (binaryReader.PeekChar() != -1)
						{
							int count2 = binaryReader.ReadInt32();
							byte[] b2 = binaryReader.ReadBytes(count2);
							empty = new Guid(b2);
						}
						result = new SubscriptionId(new Guid(b), @string, exDateTime, empty);
					}
				}
			}
			catch (EndOfStreamException innerException)
			{
				throw new InvalidSubscriptionException(innerException);
			}
			catch (ArgumentOutOfRangeException innerException2)
			{
				throw new InvalidSubscriptionException(innerException2);
			}
			catch (FormatException innerException3)
			{
				ExTraceGlobals.SubscriptionsTracer.TraceDebug(0L, "[SubscriptionId::Parse] subscription id was not a valid guid or was not valid base64");
				throw new InvalidSubscriptionException(innerException3);
			}
			return result;
		}

		public Guid Id
		{
			get
			{
				return this.subscriptionGuid;
			}
		}

		public string ServerFQDN
		{
			get
			{
				return this.serverFQDN;
			}
		}

		public ExDateTime TimeCreated
		{
			get
			{
				return this.timeCreated;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public override string ToString()
		{
			byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(this.serverFQDN.ToLower());
			byte[] array = this.subscriptionGuid.ToByteArray();
			long utcTicks = this.timeCreated.UtcTicks;
			byte[] array2 = this.mailboxGuid.ToByteArray();
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((ushort)bytes.Length);
					binaryWriter.Write(bytes);
					binaryWriter.Write(array.Length);
					binaryWriter.Write(array);
					binaryWriter.Write(utcTicks);
					binaryWriter.Write(array2.Length);
					binaryWriter.Write(array2);
					binaryWriter.Flush();
					result = Convert.ToBase64String(memoryStream.ToArray(), 0, (int)memoryStream.Length);
				}
			}
			return result;
		}

		private const short MaxFqdnLength = 2048;

		private static readonly int GuidSize = Marshal.SizeOf(typeof(Guid));

		private string serverFQDN;

		private Guid subscriptionGuid;

		private ExDateTime timeCreated;

		private Guid mailboxGuid;
	}
}
