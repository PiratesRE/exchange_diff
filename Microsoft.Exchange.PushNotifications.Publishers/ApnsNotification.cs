using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsNotification : PushNotification
	{
		public ApnsNotification(string appId, OrganizationId tenantId, string deviceToken, int badge, DateTime lastSubscriptionUpdate) : this(appId, tenantId, deviceToken, new ApnsPayload(new ApnsPayloadBasicData(new int?(badge), null, null, 0), null, null), 0, lastSubscriptionUpdate)
		{
		}

		public ApnsNotification(string appId, OrganizationId tenantId, string deviceToken, ApnsPayload payload, int expiration, DateTime lastSubscriptionUpdate) : base(appId, tenantId)
		{
			this.DeviceToken = deviceToken;
			this.Payload = payload;
			this.Expiration = expiration;
			this.LastSubscriptionUpdate = lastSubscriptionUpdate;
			if (payload != null)
			{
				payload.NotificationId = base.Identifier;
				base.IsBackgroundSyncAvailable = !string.IsNullOrEmpty(payload.BackgroundSyncType);
			}
		}

		public string DeviceToken { get; private set; }

		public ApnsPayload Payload { get; private set; }

		public int Expiration { get; private set; }

		public DateTime LastSubscriptionUpdate { get; private set; }

		public override string RecipientId
		{
			get
			{
				return this.DeviceToken;
			}
		}

		public ExDateTime SentTime { get; set; }

		public byte[] ConvertToApnsBinaryFormat()
		{
			base.Validate();
			return this.toApnsBinaryFormat;
		}

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if ((ExDateTime)this.LastSubscriptionUpdate > ExDateTime.UtcNow)
			{
				errors.Add(Strings.InvalidLastSubscriptionUpdate(this.LastSubscriptionUpdate.ToNullableString(null)));
			}
			int num = 0;
			byte[] array = null;
			byte[] array2 = null;
			byte[] array3 = null;
			byte[] array4 = null;
			byte[] array5 = null;
			byte b = 1;
			num++;
			byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(base.SequenceNumber));
			num += bytes.Length;
			if (this.Expiration < 0)
			{
				ExTraceGlobals.NotificationFormatTracer.TraceError((long)this.GetHashCode(), "[ApnsNotification::RunValidationCheck] Expiration should be set to a positive integer by now");
				errors.Add(Strings.InvalidExpiration(this.Expiration));
			}
			else
			{
				array = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(this.Expiration));
				num += array.Length;
			}
			if (string.IsNullOrEmpty(this.DeviceToken) || this.DeviceToken.Length / 2 != 32)
			{
				ExTraceGlobals.NotificationFormatTracer.TraceError<string>((long)this.GetHashCode(), "[ApnsNotification::RunValidationCheck] DeviceToken is not right: '{0}'", this.DeviceToken.ToNullableString());
				errors.Add(Strings.InvalidDeviceToken(this.DeviceToken.ToNullableString()));
			}
			else
			{
				try
				{
					array2 = HexConverter.HexStringToByteArray(this.DeviceToken);
					array3 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt16(array2.Length)));
					num += array2.Length + array3.Length;
				}
				catch (FormatException)
				{
					ExTraceGlobals.NotificationFormatTracer.TraceError<string>((long)this.GetHashCode(), "[ApnsNotification::RunValidationCheck] DeviceToken is not right: '{0}'", this.DeviceToken);
					errors.Add(Strings.InvalidDeviceToken(this.DeviceToken));
				}
			}
			if (this.Payload == null || this.Payload.Aps == null)
			{
				ExTraceGlobals.NotificationFormatTracer.TraceError((long)this.GetHashCode(), "[ApnsNotification::RunValidationCheck] Payload and Payload.Aps should be set by now.");
				errors.Add(Strings.InvalidPayload);
			}
			else
			{
				try
				{
					string text = this.Payload.ToJson();
					array4 = Encoding.UTF8.GetBytes(text);
					if (array4.Length > 256)
					{
						ExTraceGlobals.NotificationFormatTracer.TraceError<string>((long)this.GetHashCode(), "[ApnsNotification::RunValidationCheck] Payload is too long: '{0}'", text);
						errors.Add(Strings.InvalidPayloadLength(array4.Length, text));
					}
					else
					{
						array5 = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Convert.ToInt16(array4.Length)));
						num += array4.Length + array5.Length;
					}
				}
				catch (SerializationException exception)
				{
					string text2 = exception.ToTraceString();
					ExTraceGlobals.NotificationFormatTracer.TraceError<string>((long)this.GetHashCode(), "[ApnsNotification::RunValidationCheck] Unable to convert the payload to JSON: '{0}'.", text2);
					errors.Add(Strings.InvalidPayloadFormat(text2));
				}
			}
			if (errors.Count == 0)
			{
				this.toApnsBinaryFormat = new byte[num];
				this.toApnsBinaryFormat[0] = b;
				int num2 = 1;
				foreach (byte[] array7 in new byte[][]
				{
					bytes,
					array,
					array3,
					array2,
					array5,
					array4
				})
				{
					Buffer.BlockCopy(array7, 0, this.toApnsBinaryFormat, num2, array7.Length);
					num2 += array7.Length;
				}
			}
		}

		protected override string InternalToFullString()
		{
			return string.Format("{0}; token:{1}; payload:{2}; exp:{3}; lastSubscriptionUpdate:{4}", new object[]
			{
				base.InternalToFullString(),
				this.DeviceToken.ToNullableString(),
				this.Payload.ToNullableString(null),
				this.Expiration,
				this.LastSubscriptionUpdate.ToNullableString(null)
			});
		}

		public const int DeviceTokenBinaryLength = 32;

		private const int PayloadBinaryLengthMax = 256;

		private byte[] toApnsBinaryFormat;

		private enum CommandType : byte
		{
			SimpleFormat,
			EnhancedFormat
		}
	}
}
