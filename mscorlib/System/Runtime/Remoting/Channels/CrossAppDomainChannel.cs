using System;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace System.Runtime.Remoting.Channels
{
	[Serializable]
	internal class CrossAppDomainChannel : IChannel, IChannelSender, IChannelReceiver
	{
		private static CrossAppDomainChannel gAppDomainChannel
		{
			get
			{
				return Thread.GetDomain().RemotingData.ChannelServicesData.xadmessageSink;
			}
			set
			{
				Thread.GetDomain().RemotingData.ChannelServicesData.xadmessageSink = value;
			}
		}

		internal static CrossAppDomainChannel AppDomainChannel
		{
			get
			{
				if (CrossAppDomainChannel.gAppDomainChannel == null)
				{
					CrossAppDomainChannel gAppDomainChannel = new CrossAppDomainChannel();
					object obj = CrossAppDomainChannel.staticSyncObject;
					lock (obj)
					{
						if (CrossAppDomainChannel.gAppDomainChannel == null)
						{
							CrossAppDomainChannel.gAppDomainChannel = gAppDomainChannel;
						}
					}
				}
				return CrossAppDomainChannel.gAppDomainChannel;
			}
		}

		[SecurityCritical]
		internal static void RegisterChannel()
		{
			CrossAppDomainChannel appDomainChannel = CrossAppDomainChannel.AppDomainChannel;
			ChannelServices.RegisterChannelInternal(appDomainChannel, false);
		}

		public virtual string ChannelName
		{
			[SecurityCritical]
			get
			{
				return "XAPPDMN";
			}
		}

		public virtual string ChannelURI
		{
			get
			{
				return "XAPPDMN_URI";
			}
		}

		public virtual int ChannelPriority
		{
			[SecurityCritical]
			get
			{
				return 100;
			}
		}

		[SecurityCritical]
		public string Parse(string url, out string objectURI)
		{
			objectURI = url;
			return null;
		}

		public virtual object ChannelData
		{
			[SecurityCritical]
			get
			{
				return new CrossAppDomainData(Context.DefaultContext.InternalContextID, Thread.GetDomain().GetId(), Identity.ProcessGuid);
			}
		}

		[SecurityCritical]
		public virtual IMessageSink CreateMessageSink(string url, object data, out string objectURI)
		{
			objectURI = null;
			IMessageSink result = null;
			if (url != null && data == null)
			{
				if (url.StartsWith("XAPPDMN", StringComparison.Ordinal))
				{
					throw new RemotingException(Environment.GetResourceString("Remoting_AppDomains_NYI"));
				}
			}
			else
			{
				CrossAppDomainData crossAppDomainData = data as CrossAppDomainData;
				if (crossAppDomainData != null && crossAppDomainData.ProcessGuid.Equals(Identity.ProcessGuid))
				{
					result = CrossAppDomainSink.FindOrCreateSink(crossAppDomainData);
				}
			}
			return result;
		}

		[SecurityCritical]
		public virtual string[] GetUrlsForUri(string objectURI)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
		}

		[SecurityCritical]
		public virtual void StartListening(object data)
		{
		}

		[SecurityCritical]
		public virtual void StopListening(object data)
		{
		}

		private const string _channelName = "XAPPDMN";

		private const string _channelURI = "XAPPDMN_URI";

		private static object staticSyncObject = new object();

		private static PermissionSet s_fullTrust = new PermissionSet(PermissionState.Unrestricted);
	}
}
