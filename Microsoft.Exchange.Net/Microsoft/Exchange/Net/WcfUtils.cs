using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal static class WcfUtils
	{
		public static void DisposeWcfClientGracefully(ICommunicationObject client, bool skipDispose = false)
		{
			if (client == null)
			{
				return;
			}
			bool flag = false;
			try
			{
				if (client.State != CommunicationState.Faulted)
				{
					client.Close();
					flag = true;
				}
			}
			catch (TimeoutException)
			{
			}
			catch (CommunicationException)
			{
			}
			catch (InvalidOperationException)
			{
			}
			finally
			{
				if (!flag)
				{
					client.Abort();
				}
				if (!skipDispose)
				{
					((IDisposable)client).Dispose();
				}
			}
		}

		public static ChannelFactory<TClient> TryCreateChannelFactoryFromConfig<TClient>(string endpointName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("endpointName", endpointName);
			ClientSection clientSection = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");
			if (clientSection != null)
			{
				bool flag = false;
				foreach (object obj in clientSection.Endpoints)
				{
					ChannelEndpointElement channelEndpointElement = (ChannelEndpointElement)obj;
					if (endpointName.Equals(channelEndpointElement.Name, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					return new ChannelFactory<TClient>(endpointName);
				}
			}
			return null;
		}

		public static LocalizedString FullExceptionMessage(Exception ex)
		{
			return WcfUtils.FullExceptionMessage(ex, false);
		}

		public static LocalizedString FullExceptionMessage(Exception ex, bool includeStack)
		{
			LocalizedString localizedString = LocalizedString.Empty;
			string stackTrace = ex.StackTrace;
			int num = 0;
			while (ex != null && num < 20)
			{
				LocalizedException ex2 = ex as LocalizedException;
				LocalizedString localizedString2 = (ex2 != null) ? ex2.LocalizedString : new LocalizedString(ex.Message);
				localizedString = ((num > 0) ? NetServerException.NestedExceptionMsg(localizedString, localizedString2) : localizedString2);
				num++;
				ex = ex.InnerException;
			}
			if (includeStack)
			{
				localizedString = NetServerException.ExceptionWithStack(localizedString, stackTrace);
			}
			return localizedString;
		}
	}
}
