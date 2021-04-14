using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Xml;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveSync.Probes
{
	public static class ActiveSyncProbeUtil
	{
		static ActiveSyncProbeUtil()
		{
			ActiveSyncProbeUtil.KnownErrors["AccessDenied"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["AccountDisabled"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["CorruptSyncState"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["DeviceIdMissingOrInvalid"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["InvalidSyncState"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["InvalidWBXML"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["RequestOverMaxDelay"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["SchemaConversionFailure"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["SyncStateExisted"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["SyncStateNotFound"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["CafeTimeOutContactingBackEnd"] = ActiveSyncProbeUtil.EasFailingComponent.EAS;
			ActiveSyncProbeUtil.KnownErrors["WrongServerException"] = ActiveSyncProbeUtil.EasFailingComponent.Cafe;
			ActiveSyncProbeUtil.KnownErrors["CafeFailure"] = ActiveSyncProbeUtil.EasFailingComponent.Cafe;
			ActiveSyncProbeUtil.KnownErrors["CafeActiveDirectoryFailure"] = ActiveSyncProbeUtil.EasFailingComponent.Cafe;
			ActiveSyncProbeUtil.KnownErrors["CafeHighAvailabilityFailure"] = ActiveSyncProbeUtil.EasFailingComponent.Cafe;
			ActiveSyncProbeUtil.KnownErrors["ConnectFailure"] = ActiveSyncProbeUtil.EasFailingComponent.Network;
			ActiveSyncProbeUtil.KnownErrors["KeepAliveFailure"] = ActiveSyncProbeUtil.EasFailingComponent.Network;
			ActiveSyncProbeUtil.KnownErrors["ReceiveFailure"] = ActiveSyncProbeUtil.EasFailingComponent.Network;
			ActiveSyncProbeUtil.KnownErrors["SendFailure"] = ActiveSyncProbeUtil.EasFailingComponent.Network;
			ActiveSyncProbeUtil.KnownErrors["SocketException"] = ActiveSyncProbeUtil.EasFailingComponent.Network;
			ActiveSyncProbeUtil.KnownErrors["MisconfiguredDevice"] = ActiveSyncProbeUtil.EasFailingComponent.Network;
			ActiveSyncProbeUtil.KnownErrors["NameResolution"] = ActiveSyncProbeUtil.EasFailingComponent.Network;
			ActiveSyncProbeUtil.KnownErrors["SocketException"] = ActiveSyncProbeUtil.EasFailingComponent.Network;
			ActiveSyncProbeUtil.KnownErrors["MailboxCrossSiteFailureException"] = ActiveSyncProbeUtil.EasFailingComponent.Mailbox;
			ActiveSyncProbeUtil.KnownErrors["MailboxOffline"] = ActiveSyncProbeUtil.EasFailingComponent.Mailbox;
			ActiveSyncProbeUtil.KnownErrors["ConnectionFailedTransient"] = ActiveSyncProbeUtil.EasFailingComponent.Mailbox;
			ActiveSyncProbeUtil.KnownErrors["StorageTransientMapiExceptionRpcServerTooBusy"] = ActiveSyncProbeUtil.EasFailingComponent.Mailbox;
			ActiveSyncProbeUtil.KnownErrors["BackingOffMailboxServer"] = ActiveSyncProbeUtil.EasFailingComponent.Mailbox;
			ActiveSyncProbeUtil.KnownErrors["401"] = ActiveSyncProbeUtil.EasFailingComponent.Auth;
			ActiveSyncProbeUtil.KnownErrors["456"] = ActiveSyncProbeUtil.EasFailingComponent.Auth;
			ActiveSyncProbeUtil.KnownErrors["MServe+proxy"] = ActiveSyncProbeUtil.EasFailingComponent.Auth;
			ActiveSyncProbeUtil.KnownErrors["CommonAccessToken Validation Failure"] = ActiveSyncProbeUtil.EasFailingComponent.Auth;
			ActiveSyncProbeUtil.KnownErrors["CommonAccessToken Creation Failure"] = ActiveSyncProbeUtil.EasFailingComponent.Auth;
			ActiveSyncProbeUtil.KnownErrors["ADOperationException"] = ActiveSyncProbeUtil.EasFailingComponent.Auth;
			ActiveSyncProbeUtil.KnownErrors["LiveIdFailure"] = ActiveSyncProbeUtil.EasFailingComponent.Auth;
			ActiveSyncProbeUtil.KnownErrors["OverBudget"] = ActiveSyncProbeUtil.EasFailingComponent.WLM;
			ActiveSyncProbeUtil.KnownErrors["WLMTimeout"] = ActiveSyncProbeUtil.EasFailingComponent.WLM;
			ActiveSyncProbeUtil.KnownErrors["TrySubmitNewTaskFailure"] = ActiveSyncProbeUtil.EasFailingComponent.WLM;
			ActiveSyncProbeUtil.KnownErrors["ResourceUnhealthy"] = ActiveSyncProbeUtil.EasFailingComponent.WLM;
			ActiveSyncProbeUtil.KnownErrors["MonitoringAccountFailure"] = ActiveSyncProbeUtil.EasFailingComponent.Monitoring;
		}

		public static HttpWebRequest CreateEmptyGetCommand(string targetUrl)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.Method = "GET";
			CertificateValidationManager.SetComponentId(httpWebRequest, "ActiveSyncAMProbe");
			return httpWebRequest;
		}

		public static HttpWebRequest CreateOptionsCommand(string targetUrl, bool isMbxProbe, bool isDcProbe, string account, string password, string hostOverride)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(targetUrl);
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.Method = "OPTIONS";
			httpWebRequest.UserAgent = "TestActiveSyncConnectivity";
			CertificateValidationManager.SetComponentId(httpWebRequest, "ActiveSyncAMProbe");
			ActiveSyncProbeUtil.AddAuthHeader(httpWebRequest, isMbxProbe, isDcProbe, account, password);
			if (!string.IsNullOrEmpty(hostOverride))
			{
				httpWebRequest.Host = hostOverride;
			}
			return httpWebRequest;
		}

		public static HttpWebRequest CreateSettingsCommand(string targetUrl, bool isMbxProbe, bool isDcProbe, string account, string password, string body, string protocolVersion, string hostOverride, int cafeOutBoundRequestTimeOut = 0)
		{
			string requestUriString = string.Format("{0}?Cmd=Settings&User={1}&DeviceId={2}&DeviceType={3}", new object[]
			{
				targetUrl,
				account,
				"EASProbeDeviceId" + protocolVersion.Replace(".", string.Empty),
				"EASProbeDeviceType"
			});
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/vnd.ms-sync.wbxml";
			httpWebRequest.ContentLength = 11L;
			httpWebRequest.UserAgent = "TestActiveSyncConnectivity";
			httpWebRequest.Headers.Add("MS-ASProtocolVersion", protocolVersion);
			if (cafeOutBoundRequestTimeOut > 0)
			{
				httpWebRequest.Headers.Add(WellKnownHeader.FrontEndToBackEndTimeout, cafeOutBoundRequestTimeOut.ToString());
			}
			CertificateValidationManager.SetComponentId(httpWebRequest, "ActiveSyncAMProbe");
			ActiveSyncProbeUtil.AddAuthHeader(httpWebRequest, isMbxProbe, isDcProbe, account, password);
			if (isMbxProbe)
			{
				httpWebRequest.Headers.Add("msExchProxyUri", targetUrl);
			}
			if (!string.IsNullOrEmpty(hostOverride))
			{
				httpWebRequest.Host = hostOverride;
			}
			Stream stream = ActiveSyncProbeUtil.WbxmlEncodeRequestBody(body);
			stream.Seek(0L, SeekOrigin.Begin);
			StreamReader streamReader = new StreamReader(stream);
			streamReader.ReadToEnd();
			Stream stream2 = httpWebRequest.EndGetRequestStream(httpWebRequest.BeginGetRequestStream(null, null));
			stream.Seek(0L, SeekOrigin.Begin);
			stream.CopyTo(stream2, 11);
			stream2.Close();
			return httpWebRequest;
		}

		private static MemoryStream WbxmlEncodeRequestBody(string encodeBody)
		{
			MemoryStream memoryStream = new MemoryStream();
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.LoadXml(encodeBody);
			WbxmlWriter wbxmlWriter = new WbxmlWriter(memoryStream);
			wbxmlWriter.WriteXmlDocument(safeXmlDocument);
			return memoryStream;
		}

		public static XmlDocument WbxmlDecodeResponseBody(Stream bodyStream)
		{
			MemoryStream memoryStream = new MemoryStream();
			bodyStream.CopyTo(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			XmlDocument result;
			using (WbxmlReader wbxmlReader = new WbxmlReader(memoryStream))
			{
				result = wbxmlReader.ReadXmlDocument();
			}
			return result;
		}

		private static void AddAuthHeader(HttpWebRequest webRequest, bool isMbxProbe, bool isDcProbe, string account, string password)
		{
			if (isMbxProbe)
			{
				ActiveSyncProbeUtil.MbxAuthHeader(webRequest, account, isDcProbe);
				return;
			}
			string value = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(account + ":" + password));
			webRequest.Headers.Add("Authorization", value);
		}

		private static void MbxAuthHeader(HttpWebRequest webRequest, string account, bool isDcProbe)
		{
			webRequest.Headers.Add("X-IsFromCafe", "1");
			webRequest.ServicePoint.Expect100Continue = false;
			webRequest.UnsafeAuthenticatedConnectionSharing = true;
			webRequest.PreAuthenticate = true;
			webRequest.UseDefaultCredentials = true;
			if (isDcProbe)
			{
				CommonAccessToken commonAccessToken = CommonAccessTokenHelper.CreateLiveIdBasic(account);
				string value = commonAccessToken.ExtensionData["UserSid"] + "," + account;
				webRequest.Headers.Add("X-EAS-Proxy", value);
				webRequest.Headers.Add("X-CommonAccessToken", commonAccessToken.Serialize());
				return;
			}
			using (WindowsIdentity windowsIdentity = new WindowsIdentity(account))
			{
				string value2 = windowsIdentity.User.ToString() + "," + windowsIdentity.User.ToString();
				webRequest.Headers.Add("X-EAS-Proxy", value2);
				CommonAccessToken commonAccessToken2 = CommonAccessTokenHelper.CreateWindows(windowsIdentity);
				webRequest.Headers.Add("X-CommonAccessToken", commonAccessToken2.Serialize());
			}
		}

		public static int RemainingTimeout(DateTime probeStartTime, int probeTimeout)
		{
			int num = (int)(DateTime.UtcNow - probeStartTime).TotalMilliseconds;
			if (probeTimeout - num <= 0)
			{
				return -1;
			}
			return probeTimeout - num;
		}

		public static int GetCafeBackEndTimeout(DateTime probeTimeout)
		{
			int num = (int)(probeTimeout - DateTime.UtcNow).TotalSeconds;
			if (num < 6)
			{
				return num;
			}
			return num - 5;
		}

		public static string[] ReturnComponentExceptions(ActiveSyncProbeUtil.EasFailingComponent targetComponent)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, ActiveSyncProbeUtil.EasFailingComponent> keyValuePair in ActiveSyncProbeUtil.KnownErrors)
			{
				if (keyValuePair.Value == targetComponent)
				{
					list.Add(keyValuePair.Key);
				}
			}
			return list.ToArray();
		}

		public static string PopulateSiteName(string mailboxServer)
		{
			string[] array = mailboxServer.ToLowerInvariant().Split(ActiveSyncProbeUtil.mailboxSplitString, StringSplitOptions.None);
			if (array.Length > 1)
			{
				return array[0];
			}
			return null;
		}

		public const string ComponentId = "ActiveSyncAMProbe";

		public const string SettingsBody = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Settings xmlns=\"Settings:\"><UserInformation><Get/></UserInformation></Settings>";

		private const string SettingsUriValues = "{0}?Cmd=Settings&User={1}&DeviceId={2}&DeviceType={3}";

		private const string ContentHeader = "application/vnd.ms-sync.wbxml";

		private static readonly string[] mailboxSplitString = new string[]
		{
			"mb"
		};

		public static Dictionary<string, ActiveSyncProbeUtil.EasFailingComponent> KnownErrors = new Dictionary<string, ActiveSyncProbeUtil.EasFailingComponent>();

		public enum EasFailingComponent
		{
			Auth = 1,
			Cafe,
			Network,
			WLM,
			Mailbox,
			Monitoring,
			EAS,
			Unknown
		}
	}
}
