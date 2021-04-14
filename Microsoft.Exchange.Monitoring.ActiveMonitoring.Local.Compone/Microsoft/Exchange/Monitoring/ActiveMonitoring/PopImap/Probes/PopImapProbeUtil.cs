using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PopImap.Probes
{
	public static class PopImapProbeUtil
	{
		static PopImapProbeUtil()
		{
			PopImapProbeUtil.KnownErrors["LoginDenied"] = PopImapProbeUtil.PopImapFailingComponent.Auth;
			PopImapProbeUtil.KnownErrors["WrongServerException/MapiExceptionWrongServer"] = PopImapProbeUtil.PopImapFailingComponent.Cafe;
			PopImapProbeUtil.KnownErrors["WrongServerException/MapiExceptionMdbOffline"] = PopImapProbeUtil.PopImapFailingComponent.Cafe;
			PopImapProbeUtil.KnownErrors["MailboxOfflineException/MapiExceptionMdbOffline"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["MailboxCrossSiteFailoverException/MapiExceptionMdbOffline"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["ConnectionFailedTransientException/MapiExceptionLogonFailed"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["MailboxCrossSiteFailoverException/MapiExceptionLogonFailed"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["ConnectionFailedTransientException/MapiExceptionNetworkError"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["MailboxOfflineException"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["MailboxCrossSiteFailoverException"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["ConnectionFailedTransientException"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["MailboxCrossSiteFailoverException"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["ConnectionFailedTransientException/MapiExceptionNetworkError"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["StorageTransientException/MapiExceptionTimeout"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["ObjectNotFoundException/MapiExceptionNotFound"] = PopImapProbeUtil.PopImapFailingComponent.Store;
			PopImapProbeUtil.KnownErrors["WrongServerException/MapiExceptionMailboxInTransit"] = PopImapProbeUtil.PopImapFailingComponent.Monitoring;
			PopImapProbeUtil.KnownErrors["OverBudgetException"] = PopImapProbeUtil.PopImapFailingComponent.WLM;
			PopImapProbeUtil.KnownErrors["A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond"] = PopImapProbeUtil.PopImapFailingComponent.Networking;
			PopImapProbeUtil.KnownErrors["ProxyFailed"] = PopImapProbeUtil.PopImapFailingComponent.Networking;
			PopImapProbeUtil.KnownErrors["NetworkConnectionFailed"] = PopImapProbeUtil.PopImapFailingComponent.Networking;
			PopImapProbeUtil.KnownErrors["ServerNotFoundException"] = PopImapProbeUtil.PopImapFailingComponent.Deployment;
			PopImapProbeUtil.KnownErrors["ServerNotInSiteException"] = PopImapProbeUtil.PopImapFailingComponent.Deployment;
			PopImapProbeUtil.KnownErrors["ProxyNotAuthenticated"] = PopImapProbeUtil.PopImapFailingComponent.Deployment;
			PopImapProbeUtil.KnownErrors["ObjectExistedException"] = PopImapProbeUtil.PopImapFailingComponent.PopImap;
			PopImapProbeUtil.KnownErrors["ProxyTimeout"] = PopImapProbeUtil.PopImapFailingComponent.PopImap;
			PopImapProbeUtil.KnownErrors["EndReadFailure"] = PopImapProbeUtil.PopImapFailingComponent.PopImap;
		}

		internal static void CreateImapStartTls(PopImapProbeStateObject probeTrackingObject)
		{
			probeTrackingObject.Command = "z STARTTLS";
			probeTrackingObject.ExpectedTag = "z";
			probeTrackingObject.MultiLine = false;
		}

		internal static void CreateImapCapabilities(PopImapProbeStateObject probeTrackingObject)
		{
			probeTrackingObject.Command = "z capability";
			probeTrackingObject.ExpectedTag = "z";
			probeTrackingObject.MultiLine = false;
		}

		internal static void CreateImapLogin(PopImapProbeStateObject probeTrackingObject)
		{
			probeTrackingObject.Command = string.Format("z login {0} {1}", probeTrackingObject.UserAccount, probeTrackingObject.UserPassword);
			probeTrackingObject.ExpectedTag = "z";
			probeTrackingObject.MultiLine = false;
		}

		internal static void CreatePopStartTls(PopImapProbeStateObject probeTrackingObject)
		{
			probeTrackingObject.Command = "STLS";
			probeTrackingObject.ExpectedTag = null;
			probeTrackingObject.MultiLine = false;
		}

		internal static void CreatePopCapabilities(PopImapProbeStateObject probeTrackingObject)
		{
			probeTrackingObject.Command = "CAPA";
			probeTrackingObject.ExpectedTag = null;
			probeTrackingObject.MultiLine = true;
		}

		internal static void CreatePopUserCommand(PopImapProbeStateObject probeTrackingObject)
		{
			probeTrackingObject.Command = string.Format("user {0}", probeTrackingObject.UserAccount);
			probeTrackingObject.ExpectedTag = null;
			probeTrackingObject.MultiLine = false;
		}

		internal static void CreatePopPassCommand(PopImapProbeStateObject probeTrackingObject)
		{
			probeTrackingObject.Command = string.Format("pass {0}", probeTrackingObject.UserPassword);
			probeTrackingObject.ExpectedTag = null;
			probeTrackingObject.MultiLine = false;
		}

		internal static PopImapProbeStateObject CreateImapSSLStateObject(IPEndPoint targetAddress, ProbeResult result)
		{
			Imap4Connection imap4Connection;
			PopImapProbeStateObject popImapProbeStateObject;
			try
			{
				imap4Connection = new Imap4Connection(targetAddress);
			}
			catch (SocketException connectionException)
			{
				popImapProbeStateObject = new PopImapProbeStateObject(null, result, ProbeState.Capability1);
				popImapProbeStateObject.ConnectionException = connectionException;
				return popImapProbeStateObject;
			}
			popImapProbeStateObject = new PopImapProbeStateObject(imap4Connection, result, ProbeState.Capability1);
			try
			{
				imap4Connection.NegotiateSSL();
				TcpResponse response = imap4Connection.GetResponse();
				popImapProbeStateObject.Result.StateAttribute3 = PopImapProbeUtil.DecodeServerName(response.ResponseMessage);
				popImapProbeStateObject.TcpResponses.Add(response);
			}
			catch (Exception connectionException2)
			{
				popImapProbeStateObject.ConnectionException = connectionException2;
			}
			return popImapProbeStateObject;
		}

		internal static PopImapProbeStateObject CreateImapTLSStateObject(IPEndPoint targetAddress, ProbeResult result)
		{
			Imap4Connection protocolConnection;
			try
			{
				protocolConnection = new Imap4Connection(targetAddress);
			}
			catch (SocketException connectionException)
			{
				return new PopImapProbeStateObject(null, result, ProbeState.Capability1)
				{
					ConnectionException = connectionException
				};
			}
			return new PopImapProbeStateObject(protocolConnection, result, ProbeState.Capability1);
		}

		internal static PopImapProbeStateObject CreatePopSSLStateObject(IPEndPoint targetAddress, ProbeResult result)
		{
			Pop3Connection pop3Connection;
			PopImapProbeStateObject popImapProbeStateObject;
			try
			{
				pop3Connection = new Pop3Connection(targetAddress);
			}
			catch (SocketException connectionException)
			{
				popImapProbeStateObject = new PopImapProbeStateObject(null, result, ProbeState.Capability1);
				popImapProbeStateObject.ConnectionException = connectionException;
				return popImapProbeStateObject;
			}
			popImapProbeStateObject = new PopImapProbeStateObject(pop3Connection, result, ProbeState.Capability1);
			try
			{
				pop3Connection.NegotiateSSL();
				TcpResponse response = pop3Connection.GetResponse();
				popImapProbeStateObject.Result.StateAttribute3 = PopImapProbeUtil.DecodeServerName(response.ResponseMessage);
				popImapProbeStateObject.TcpResponses.Add(response);
			}
			catch (Exception connectionException2)
			{
				popImapProbeStateObject.ConnectionException = connectionException2;
			}
			return popImapProbeStateObject;
		}

		internal static PopImapProbeStateObject CreatePopTLSStateObject(IPEndPoint targetAddress, ProbeResult result)
		{
			Pop3Connection protocolConnection;
			try
			{
				protocolConnection = new Pop3Connection(targetAddress);
			}
			catch (SocketException connectionException)
			{
				return new PopImapProbeStateObject(null, result, ProbeState.Capability1)
				{
					ConnectionException = connectionException
				};
			}
			return new PopImapProbeStateObject(protocolConnection, result, ProbeState.Capability1);
		}

		internal static string DecodeServerName(string banner)
		{
			string text = " ";
			string text2 = string.Empty;
			if (!string.IsNullOrEmpty(banner))
			{
				string[] array = banner.Split(new string[]
				{
					text
				}, StringSplitOptions.None);
				string text3 = array[array.Length - 1];
				if (text3.StartsWith("[") && text3.EndsWith("]"))
				{
					text3 = text3.Substring(1, text3.Length - 2);
					try
					{
						text2 = Encoding.Unicode.GetString(Convert.FromBase64String(text3));
					}
					catch (FormatException)
					{
					}
				}
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = "Unable to retrieve server name";
			}
			return text2;
		}

		public static Dictionary<string, PopImapProbeUtil.PopImapFailingComponent> KnownErrors = new Dictionary<string, PopImapProbeUtil.PopImapFailingComponent>();

		public enum PopImapFailingComponent
		{
			Auth = 1,
			Cafe,
			Networking,
			Monitoring,
			WLM,
			Store,
			HA,
			Deployment,
			PopImap,
			Unknown
		}
	}
}
