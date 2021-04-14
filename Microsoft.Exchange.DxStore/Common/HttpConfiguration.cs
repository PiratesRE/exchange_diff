using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.DxStore.Common
{
	public static class HttpConfiguration
	{
		public static bool UseEncryption { get; set; }

		public static bool IsZeroboxMode { get; set; }

		public static void Configure(InstanceGroupConfig groupConfig)
		{
			if (!HttpConfiguration.isConfigured)
			{
				lock (HttpConfiguration.lockObj)
				{
					if (!HttpConfiguration.isConfigured)
					{
						DxSerializationUtil.UseBinarySerialize = groupConfig.Settings.IsUseBinarySerializerForClientCommunication;
						HttpConfiguration.UseEncryption = groupConfig.Settings.IsUseEncryption;
						HttpConfiguration.IsZeroboxMode = groupConfig.IsZeroboxMode;
						bool flag2 = true;
						if (HttpConfiguration.UseEncryption && flag2)
						{
							ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(HttpConfiguration.IgnoreCertValidate);
						}
						else
						{
							ServicePointManager.ServerCertificateValidationCallback = null;
						}
						HttpConfiguration.isConfigured = true;
					}
				}
			}
		}

		public static string FormClientUriPrefix(string targetServerAddr, string targetNodeId, string groupName)
		{
			if (HttpConfiguration.IsZeroboxMode)
			{
				targetServerAddr = "localhost";
				return string.Format("http{0}://{1}/{2}/{3}/{4}/", new object[]
				{
					HttpConfiguration.UseEncryption ? "s" : string.Empty,
					targetServerAddr,
					"Microsoft.Exchange.DxStore.Http",
					groupName,
					targetNodeId
				});
			}
			return string.Format("http{0}://{1}/{2}/{3}/", new object[]
			{
				HttpConfiguration.UseEncryption ? "s" : string.Empty,
				targetServerAddr,
				"Microsoft.Exchange.DxStore.Http",
				groupName
			});
		}

		public static string FormServerUriPrefix(string self, string groupName)
		{
			string text = "+";
			if (HttpConfiguration.IsZeroboxMode)
			{
				return string.Format("http{0}://{1}/{2}/{3}/{4}/", new object[]
				{
					HttpConfiguration.UseEncryption ? "s" : string.Empty,
					text,
					"Microsoft.Exchange.DxStore.Http",
					groupName,
					self
				});
			}
			return string.Format("http{0}://{1}/{2}/{3}/", new object[]
			{
				HttpConfiguration.UseEncryption ? "s" : string.Empty,
				text,
				"Microsoft.Exchange.DxStore.Http",
				groupName
			});
		}

		private static bool IgnoreCertValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
		{
			bool flag = false;
			if (!HttpConfiguration.invalidCertLogged)
			{
				lock (HttpConfiguration.lockObj)
				{
					if (!HttpConfiguration.invalidCertLogged)
					{
						flag = true;
						HttpConfiguration.invalidCertLogged = true;
					}
				}
			}
			if (flag)
			{
				EventLogger.LogErr("IgnoreCertValidate ignored {0}", new object[]
				{
					cert
				});
			}
			return true;
		}

		public const string UriNameSpace = "Microsoft.Exchange.DxStore.Http";

		private static bool isConfigured;

		private static object lockObj = new object();

		private static bool invalidCertLogged = false;
	}
}
