using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class WnsNotification : PushNotification
	{
		static WnsNotification()
		{
			List<string> list = new List<string>
			{
				"localhost",
				"127.0.0.1"
			};
			try
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
				list.Add(hostEntry.HostName);
				foreach (IPAddress ipaddress in hostEntry.AddressList)
				{
					list.Add(ipaddress.ToString());
				}
			}
			catch (SocketException)
			{
			}
			WnsNotification.LocalHostIds = list.ToArray();
		}

		public WnsNotification(string appId, OrganizationId tenantId, string deviceUri) : base(appId, tenantId)
		{
			this.DeviceUri = deviceUri;
		}

		public string DeviceUri { get; private set; }

		public override string RecipientId
		{
			get
			{
				return this.DeviceUri;
			}
		}

		internal WnsRequest CreateWnsRequest()
		{
			base.Validate();
			WnsRequest wnsRequest = new WnsRequest();
			wnsRequest.Uri = this.uriCache;
			wnsRequest.RequestStream = new MemoryStream(this.payloadBytesCache);
			this.PrepareWnsRequest(wnsRequest);
			return wnsRequest;
		}

		protected abstract void PrepareWnsRequest(WnsRequest wnsRequest);

		protected abstract string GetSerializedPayload(List<LocalizedString> errors);

		protected override string InternalToFullString()
		{
			return string.Format("{0}; uri:{1}", base.InternalToFullString(), this.DeviceUri.ToNullableString());
		}

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (string.IsNullOrWhiteSpace(this.DeviceUri))
			{
				errors.Add(Strings.InvalidWnsDeviceUri(this.DeviceUri.ToNullableString(), string.Empty));
			}
			else
			{
				try
				{
					this.uriCache = new Uri(this.DeviceUri, UriKind.Absolute);
					if (!this.uriCache.DnsSafeHost.EndsWith("notify.windows.com", StringComparison.OrdinalIgnoreCase) && !WnsNotification.LocalHostIds.Contains(this.uriCache.DnsSafeHost, StringComparer.OrdinalIgnoreCase))
					{
						errors.Add(Strings.InvalidWnsDeviceUri(this.DeviceUri, string.Empty));
					}
				}
				catch (UriFormatException exception)
				{
					errors.Add(Strings.InvalidWnsDeviceUri(this.DeviceUri, new LocalizedString(exception.ToTraceString())));
				}
			}
			string serializedPayload = this.GetSerializedPayload(errors);
			byte[] bytes = Encoding.UTF8.GetBytes(serializedPayload);
			if (bytes.Length > 5000)
			{
				errors.Add(Strings.InvalidWnsPayloadLength(5000, serializedPayload));
				return;
			}
			this.payloadBytesCache = bytes;
		}

		protected void ValidateTimeToLive(int? timeToLive, List<LocalizedString> errors)
		{
			if (timeToLive != null && timeToLive.Value < 60)
			{
				errors.Add(Strings.InvalidWnsTimeToLive(timeToLive.Value));
			}
		}

		protected void ValidateTemplate(WnsBinding binding, List<LocalizedString> errors)
		{
			int num = (binding.Texts != null) ? binding.Texts.Length : 0;
			int num2 = (binding.Images != null) ? binding.Images.Length : 0;
			if (binding.TemplateDescription.MaxNumOfTexts < num || binding.TemplateDescription.MaxNumOfImages < num2)
			{
				errors.Add(Strings.InvalidWnsTemplate(binding.ToString()));
			}
		}

		private const string WnsDeviceUriDomain = "notify.windows.com";

		private const int MaxPayloadSize = 5000;

		private const int MinTimeToLive = 60;

		private static readonly string[] LocalHostIds;

		private Uri uriCache;

		private byte[] payloadBytesCache;
	}
}
