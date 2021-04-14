using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Management.Mobility;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("New", "Subscription", SupportsShouldProcess = true)]
	public sealed class NewSubscription : NewSubscriptionBase<PimSubscriptionProxy>
	{
		[Parameter(Mandatory = true)]
		public SecureString Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Hotmail
		{
			get
			{
				return (SwitchParameter)(base.Fields["Hotmail"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Hotmail"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Imap
		{
			get
			{
				return (SwitchParameter)(base.Fields["Imap"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Imap"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Pop
		{
			get
			{
				return (SwitchParameter)(base.Fields["Pop"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Pop"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.AutoProvisionConfirmation(this.DataObject);
			}
		}

		internal static DiscoverSettingsResult DiscoverSettings(IAutoProvision[] clients, bool testOnlyInsecure, Dictionary<Authority, bool> connectiveAuthority, AutoProvisionProgress provisionProgressCallback, SyncLogSession syncLogSession, out PimSubscriptionProxy subscription)
		{
			subscription = null;
			bool flag = false;
			foreach (IAutoProvision autoProvision in clients)
			{
				DiscoverSettingsResult discoverSettingsResult = autoProvision.DiscoverSetting(syncLogSession, testOnlyInsecure, connectiveAuthority, provisionProgressCallback, out subscription);
				if (discoverSettingsResult == DiscoverSettingsResult.Succeeded)
				{
					return discoverSettingsResult;
				}
				if (discoverSettingsResult == DiscoverSettingsResult.AuthenticationError)
				{
					syncLogSession.LogDebugging((TSLID)1264UL, "Found an authentication error when trying to discover settings.", new object[0]);
					flag = true;
				}
			}
			if (flag)
			{
				return DiscoverSettingsResult.AuthenticationError;
			}
			return DiscoverSettingsResult.SettingsNotFound;
		}

		internal static Dictionary<Authority, bool> CheckConnectivity(IAutoProvision[] clients, AutoProvisionProgress provisionProgressCallback)
		{
			Dictionary<string, IPAddress[]> dictionary = new Dictionary<string, IPAddress[]>(StringComparer.OrdinalIgnoreCase);
			foreach (IAutoProvision autoProvision in clients)
			{
				foreach (string text in autoProvision.Hostnames)
				{
					if (!dictionary.ContainsKey(text))
					{
						if (text.Length > SyncUtilities.MaximumFqdnLength)
						{
							CommonLoggingHelper.SyncLogSession.LogError((TSLID)1265UL, "FQDN: [{0}] with length: {1} was too long.", new object[]
							{
								text,
								text.Length
							});
							dictionary.Add(text, null);
						}
						else
						{
							try
							{
								provisionProgressCallback(Strings.AutoProvisionQueryDNS, new LocalizedString(text));
								CommonLoggingHelper.SyncLogSession.LogInformation((TSLID)1266UL, "Querying DNS: {0}", new object[]
								{
									text
								});
								IPHostEntry hostEntry = Dns.GetHostEntry(text);
								dictionary.Add(text, hostEntry.AddressList);
							}
							catch (SocketException ex)
							{
								CommonLoggingHelper.SyncLogSession.LogError((TSLID)1267UL, "DNS Query failed with error: {0}", new object[]
								{
									ex
								});
								dictionary.Add(text, null);
							}
						}
					}
				}
			}
			Dictionary<Authority, bool> dictionary2 = new Dictionary<Authority, bool>();
			Dictionary<Socket, Authority> dictionary3 = new Dictionary<Socket, Authority>();
			try
			{
				List<IAsyncResult> list = new List<IAsyncResult>();
				List<WaitHandle> list2 = new List<WaitHandle>();
				foreach (IAutoProvision autoProvision2 in clients)
				{
					foreach (string text2 in autoProvision2.Hostnames)
					{
						foreach (int num in autoProvision2.ConnectivePorts)
						{
							Authority authority = new Authority(text2, num);
							if (!dictionary2.ContainsKey(authority))
							{
								dictionary2[authority] = false;
								if (dictionary[text2] == null)
								{
									CommonLoggingHelper.SyncLogSession.LogInformation((TSLID)1268UL, "No valid DNS results exist for fqdn: {0}", new object[]
									{
										text2
									});
								}
								else
								{
									provisionProgressCallback(Strings.AutoProvisionConnectivity, new LocalizedString(authority.ToString()));
									CommonLoggingHelper.SyncLogSession.LogInformation((TSLID)1269UL, "Connecting to {0} ...", new object[]
									{
										authority
									});
									Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
									IAsyncResult asyncResult = null;
									try
									{
										asyncResult = socket.BeginConnect(dictionary[text2], num, null, socket);
									}
									catch (SocketException)
									{
										socket.Close();
										goto IL_284;
									}
									dictionary3.Add(socket, authority);
									list.Add(asyncResult);
									list2.Add(asyncResult.AsyncWaitHandle);
								}
							}
							IL_284:;
						}
					}
				}
				if (list2.Count == 0)
				{
					return dictionary2;
				}
				provisionProgressCallback(Strings.AutoProvisionConnectivity, Strings.AutoProvisionResults);
				WaitHandle.WaitAll(list2.ToArray(), NewSubscription.socketTimeout, false);
				foreach (IAsyncResult asyncResult2 in list)
				{
					if (asyncResult2.IsCompleted)
					{
						Socket socket2 = (Socket)asyncResult2.AsyncState;
						try
						{
							socket2.EndConnect(asyncResult2);
						}
						catch (SocketException)
						{
						}
					}
				}
			}
			finally
			{
				foreach (KeyValuePair<Socket, Authority> keyValuePair in dictionary3)
				{
					Socket key = keyValuePair.Key;
					Authority value = keyValuePair.Value;
					if (key.Connected)
					{
						dictionary2[value] = true;
					}
					CommonLoggingHelper.SyncLogSession.LogInformation((TSLID)1270UL, "Connection to {0} succeeded: {1}.", new object[]
					{
						value,
						key.Connected
					});
					key.Close();
				}
			}
			return dictionary2;
		}

		protected override IConfigurable PrepareDataObject()
		{
			CommonLoggingHelper.SyncLogSession.SetBlackBoxCapacity(NewSubscription.IncreasedBlackBoxCapacity);
			AutoProvisionProgress autoProvisionProgress = delegate(LocalizedString activity, LocalizedString statusDescription)
			{
				if (base.IsVerboseOn)
				{
					base.WriteVerbose(Strings.AutoProvisionDebug(activity, statusDescription));
				}
				ExProgressRecord record = new ExProgressRecord(0, activity, statusDescription);
				base.WriteProgress(record);
				CommonLoggingHelper.SyncLogSession.LogVerbose((TSLID)1271UL, Strings.AutoProvisionDebug(activity, statusDescription), new object[0]);
			};
			IAutoProvision[] array = this.ClientsToUse();
			if (array.Length == 0)
			{
				base.WriteDebugInfoAndError(new LocalizedException(Strings.AutoProvisionNoProtocols), ErrorCategory.InvalidArgument, null);
			}
			Dictionary<Authority, bool> connectiveAuthority = NewSubscription.CheckConnectivity(array, autoProvisionProgress);
			PimSubscriptionProxy pimSubscriptionProxy;
			DiscoverSettingsResult discoverSettingsResult = NewSubscription.DiscoverSettings(array, false, connectiveAuthority, autoProvisionProgress, CommonLoggingHelper.SyncLogSession, out pimSubscriptionProxy);
			if (pimSubscriptionProxy == null && discoverSettingsResult != DiscoverSettingsResult.AuthenticationError && (this.Force == true || base.ShouldContinue(Strings.InsecureConfirmation)))
			{
				CommonLoggingHelper.SyncLogSession.LogVerbose((TSLID)1272UL, "Checking for in-secure option.", new object[0]);
				NewSubscription.DiscoverSettings(array, true, connectiveAuthority, autoProvisionProgress, CommonLoggingHelper.SyncLogSession, out pimSubscriptionProxy);
			}
			if (pimSubscriptionProxy == null)
			{
				base.WriteDebugInfoAndError(new AutoProvisionFailedException(), ErrorCategory.InvalidArgument, null);
			}
			pimSubscriptionProxy.CreationType = SubscriptionCreationType.Auto;
			this.DataObject = pimSubscriptionProxy;
			autoProvisionProgress(Strings.AutoProvisionComplete, Strings.AutoProvisionCreate);
			base.WriteDebugInfo();
			return base.PrepareDataObject();
		}

		private IAutoProvision[] ClientsToUse()
		{
			List<IAutoProvision> list = new List<IAutoProvision>();
			bool flag = false;
			CommonLoggingHelper.SyncLogSession.LogVerbose((TSLID)1273UL, "Protocols specified in the command line: Hotmail:{0}, Imap:{1}, Pop:{2}", new object[]
			{
				this.Hotmail,
				this.Imap,
				this.Pop
			});
			AggregationSubscriptionDataProvider aggregationSubscriptionDataProvider = (AggregationSubscriptionDataProvider)base.DataSession;
			if (this.Hotmail == true)
			{
				list.Add(new DeltaSyncAutoProvision(base.EmailAddress, this.Password));
				flag = true;
			}
			if (this.Imap == true)
			{
				list.Add(new IMAPAutoProvision(base.EmailAddress, this.Password));
				flag = true;
			}
			if (this.Pop == true)
			{
				list.Add(new Pop3AutoProvision(base.EmailAddress, this.Password, aggregationSubscriptionDataProvider.UserLegacyDN));
				flag = true;
			}
			if (flag)
			{
				return list.ToArray();
			}
			CommonLoggingHelper.SyncLogSession.LogVerbose((TSLID)1274UL, "Building protocol list dynamically from our permissions ...", new object[0]);
			if (base.ExchangeRunspaceConfig.CalculateScopeSetForExchangeCmdlet(base.MyInvocation.MyCommand.Name, new string[]
			{
				"Hotmail"
			}, base.CurrentOrganizationId, null) != null)
			{
				CommonLoggingHelper.SyncLogSession.LogVerbose((TSLID)1275UL, "Hotmail Permissions found.", new object[0]);
				list.Add(new DeltaSyncAutoProvision(base.EmailAddress, this.Password));
			}
			if (base.ExchangeRunspaceConfig.CalculateScopeSetForExchangeCmdlet(base.MyInvocation.MyCommand.Name, new string[]
			{
				"Imap"
			}, base.CurrentOrganizationId, null) != null)
			{
				CommonLoggingHelper.SyncLogSession.LogVerbose((TSLID)1276UL, "Imap Permissions found.", new object[0]);
				list.Add(new IMAPAutoProvision(base.EmailAddress, this.Password));
			}
			if (base.ExchangeRunspaceConfig.CalculateScopeSetForExchangeCmdlet(base.MyInvocation.MyCommand.Name, new string[]
			{
				"Pop"
			}, base.CurrentOrganizationId, null) != null)
			{
				CommonLoggingHelper.SyncLogSession.LogVerbose((TSLID)1277UL, "Pop Permissions found.", new object[0]);
				list.Add(new Pop3AutoProvision(base.EmailAddress, this.Password, aggregationSubscriptionDataProvider.UserLegacyDN));
			}
			return list.ToArray();
		}

		private static readonly TimeSpan socketTimeout = TimeSpan.FromSeconds(5.0);

		private static readonly int IncreasedBlackBoxCapacity = 500;
	}
}
