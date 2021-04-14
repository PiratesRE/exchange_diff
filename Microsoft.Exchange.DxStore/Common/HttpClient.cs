using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.DxStore;

namespace Microsoft.Exchange.DxStore.Common
{
	public class HttpClient
	{
		public static async Task SendMessageAsync(string targetServer, string nodeName, string groupName, HttpRequest.PaxosMessage msg)
		{
			try
			{
				string uri = HttpConfiguration.FormClientUriPrefix(targetServer, nodeName, groupName);
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
				req.Method = "PUT";
				req.ContentType = "application/octet-stream";
				MemoryStream ms = DxSerializationUtil.SerializeMessage(msg);
				req.ContentLength = ms.Length;
				ms.Position = 0L;
				Stream outStream = await req.GetRequestStreamAsync();
				using (outStream)
				{
					await ms.CopyToAsync(outStream);
				}
				ExTraceGlobals.PaxosMessageTracer.TraceDebug<long>(0L, "Sent PaxosMessage len={0}", ms.Length);
				using ((HttpWebResponse)(await req.GetResponseAsync()))
				{
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.PaxosMessageTracer.TraceError<string, Exception>(0L, "SendMessageAsync failed to node {0} caught: {1}", nodeName, ex);
				throw new DxStoreInstanceClientException(ex.Message, ex);
			}
		}

		public static async Task<InstanceStatusInfo> GetStatusAsync(string targetServer, string targetNodeName, string groupName, string sendingNodeName)
		{
			InstanceStatusInfo reply;
			try
			{
				string uri = HttpConfiguration.FormClientUriPrefix(targetServer, targetNodeName, groupName);
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
				req.Method = "PUT";
				req.ContentType = "application/octet-stream";
				HttpRequest.GetStatusRequest reqMsg = new HttpRequest.GetStatusRequest(sendingNodeName);
				MemoryStream ms = DxSerializationUtil.SerializeMessage(reqMsg);
				req.ContentLength = ms.Length;
				ms.Position = 0L;
				Stream outStream = await req.GetRequestStreamAsync();
				using (outStream)
				{
					await ms.CopyToAsync(outStream);
				}
				using (HttpWebResponse httpResponse = (HttpWebResponse)(await req.GetResponseAsync()))
				{
					using (Stream responseStream = httpResponse.GetResponseStream())
					{
						HttpReply httpReply = DxSerializationUtil.Deserialize<HttpReply>(responseStream);
						if (httpReply is HttpReply.GetInstanceStatusReply)
						{
							reply = (httpReply as HttpReply.GetInstanceStatusReply).Reply;
						}
						else
						{
							if (httpReply is HttpReply.ExceptionReply)
							{
								Exception exception = (httpReply as HttpReply.ExceptionReply).Exception;
								throw new DxStoreInstanceServerException(exception.Message, exception);
							}
							throw new DxStoreInstanceClientException(string.Format("unexpected reply: {0}", httpReply.GetType().FullName));
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.InstanceClientTracer.TraceError<string, Exception>(0L, "GetStatusAsync to {0} caught: {1}", targetNodeName, ex);
				if (ex is DxStoreInstanceClientException || ex is DxStoreInstanceServerException)
				{
					throw;
				}
				throw new DxStoreInstanceClientException(ex.Message, ex);
			}
			return reply;
		}

		public class TargetInfo
		{
			public TargetInfo(string host, string node, string groupName)
			{
				this.TargetHost = host;
				this.TargetNode = node;
				this.GroupName = groupName;
			}

			public string TargetHost { get; set; }

			public string TargetNode { get; set; }

			public string GroupName { get; set; }

			public static HttpClient.TargetInfo BuildFromNode(string nodeName, InstanceGroupConfig groupConfig)
			{
				if (groupConfig == null)
				{
					return new HttpClient.TargetInfo(nodeName, nodeName, "NoGroupName");
				}
				return new HttpClient.TargetInfo(groupConfig.GetMemberNetworkAddress(nodeName), nodeName, groupConfig.Name);
			}
		}
	}
}
