using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Exchange.Migration.Logging;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Nspi.Client;
using Microsoft.Exchange.Nspi.Rfri;
using Microsoft.Exchange.Nspi.Rfri.Client;
using Microsoft.Exchange.Rpc;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationNspiClient : IMigrationNspiClient
	{
		public MigrationNspiClient(ReportData reportData = null)
		{
			int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationSourceNspiHttpPort");
			int config2 = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationSourceRfrHttpPort");
			this.nspiProtocolSequence = "ncacn_http:" + config.ToString();
			this.nspiRfrProtocolSequence = "ncacn_http:" + config2.ToString();
			this.reportData = reportData;
		}

		public IList<PropRow> QueryRows(ExchangeOutlookAnywhereEndpoint endpoint, int? batchSize, int? startIndex, long[] longPropTags)
		{
			MigrationUtil.ThrowOnNullArgument(endpoint, "endpoint");
			MigrationUtil.AssertOrThrow(batchSize != null, "should always specify a batch size", new object[0]);
			MigrationUtil.AssertOrThrow(startIndex != null, "should always specify a start index", new object[0]);
			PropRowSet rowset = null;
			this.RunNspiOperation("QueryRows", null, endpoint, delegate(string debugContext, NspiClient client)
			{
				client.Stat.CurrentRecord = 0;
				client.Stat.Delta = startIndex.Value;
				client.Stat.ContainerId = 0;
				this.RunNspiClientCommand(debugContext, () => client.QueryRows(NspiQueryRowsFlags.None, null, batchSize.Value, MigrationNspiClient.ConvertPropTags(longPropTags), out rowset));
				MigrationLogger.Log(MigrationEventType.Information, "NSPI::query rows found {0} records", new object[]
				{
					client.Stat.TotalRecords
				});
			});
			if (rowset == null || rowset.Rows == null || rowset.Rows.Count == 0)
			{
				return null;
			}
			return rowset.Rows;
		}

		public IList<PropRow> GetGroupMembers(ExchangeOutlookAnywhereEndpoint endpoint, string groupSmtpAddress)
		{
			MigrationUtil.ThrowOnNullArgument(endpoint, "endpoint");
			MigrationUtil.ThrowOnNullOrEmptyArgument(groupSmtpAddress, "groupSmtpAddress");
			PropRowSet rowset = null;
			Restriction restriction = Restriction.EQ(PropTag.SmtpAddress, groupSmtpAddress);
			PropTag[] requiredPropTags = new PropTag[]
			{
				PropTag.SmtpAddress
			};
			this.RunNspiOperation("GetGroupMembers", null, endpoint, delegate(string debugContext, NspiClient client)
			{
				int[] mids = null;
				PropRowSet groupRowSet;
				this.RunNspiClientCommand("initial getMatches " + debugContext, () => client.GetMatches(restriction, null, 1, null, out mids, out groupRowSet));
				if (mids == null || mids.Length != 1 || mids[0] == 0)
				{
					throw new MigrationTransientException(Strings.MigrationExchangeRpcConnectionFailure, "Nspi::GetMatches failed to retrieve requested DG!");
				}
				this.RunNspiClientCommand("getMatches " + debugContext, delegate
				{
					client.Stat.CurrentRecord = mids[0];
					client.Stat.SortType = 1000;
					client.Stat.ContainerId = -2146893811;
					return client.GetMatches(null, null, 5000, requiredPropTags, out mids, out rowset);
				});
				if (mids != null && rowset != null && mids.Length > rowset.Rows.Count)
				{
					client.Stat.CurrentRecord = 0;
					client.Stat.SortType = 0;
					client.Stat.ContainerId = 0;
					int num = rowset.Rows.Count;
					int[] array = null;
					while (mids.Length > num)
					{
						num += MigrationNspiClient.GetBatchOfMIds(mids, num, ref array);
						PropRowSet rowsetNew = null;
						this.RunNspiClientCommand("queryRows for members of DG " + debugContext, () => client.QueryRows(NspiQueryRowsFlags.None, mids, mids.Length, requiredPropTags, out rowsetNew));
						if (rowsetNew != null)
						{
							foreach (PropRow row in rowsetNew.Rows)
							{
								rowset.Add(row);
							}
						}
					}
				}
				MigrationLogger.Log(MigrationEventType.Information, "NSPI::get group members found {0} records", new object[]
				{
					(rowset != null) ? rowset.Rows.Count : 0
				});
			});
			if (rowset == null || rowset.Rows == null || rowset.Rows.Count == 0)
			{
				return null;
			}
			return rowset.Rows;
		}

		public string GetNewDSA(ExchangeOutlookAnywhereEndpoint endpoint)
		{
			return this.GetNewDSA(endpoint, string.Empty);
		}

		public PropRow GetRecipient(ExchangeOutlookAnywhereEndpoint endpoint, string recipientSmtpAddress, long[] longPropTags)
		{
			MigrationUtil.ThrowOnNullArgument(endpoint, "endpoint");
			MigrationUtil.ThrowOnNullOrEmptyArgument(recipientSmtpAddress, "recipientSmtpAddress");
			Restriction restriction = Restriction.EQ(PropTag.SmtpAddress, recipientSmtpAddress);
			PropRowSet rowset = null;
			this.RunNspiOperation("GetRecipient", null, endpoint, delegate(string debugContext, NspiClient client)
			{
				int[] mids;
				this.RunNspiClientCommand("getMatches " + debugContext, () => client.GetMatches(restriction, null, 10, MigrationNspiClient.ConvertPropTags(longPropTags), out mids, out rowset));
				MigrationLogger.Log(MigrationEventType.Verbose, "NSPI::get recipient found {0} records", new object[]
				{
					(rowset != null) ? rowset.Rows.Count : 0
				});
			});
			if (rowset == null || rowset.Rows == null || rowset.Rows.Count == 0)
			{
				return null;
			}
			if (rowset.Rows.Count > 1)
			{
				throw new SourceEmailAddressNotUniquePermanentException(recipientSmtpAddress);
			}
			return rowset.Rows[0];
		}

		public void SetRecipient(ExchangeOutlookAnywhereEndpoint endpoint, string recipientSmtpAddress, string recipientLegDN, string[] propTagValues, long[] longPropTags)
		{
			MigrationUtil.ThrowOnNullArgument(endpoint, "endpoint");
			MigrationUtil.ThrowOnNullOrEmptyArgument(recipientSmtpAddress, "recipientSmtpAddress");
			MigrationUtil.ThrowOnNullOrEmptyArgument(recipientLegDN, "recipientLegDN");
			string newDSA = this.GetNewDSA(endpoint, recipientLegDN);
			PropRow row = new PropRow();
			PropTag[] propTags = MigrationNspiClient.ConvertPropTags(longPropTags);
			for (int i = 0; i < propTags.Length; i++)
			{
				row.Add(new PropValue(propTags[i], propTagValues[i]));
			}
			Restriction restriction = Restriction.EQ(PropTag.SmtpAddress, recipientSmtpAddress);
			this.RunNspiOperation("SetRecipient", newDSA, endpoint, delegate(string debugContext, NspiClient client)
			{
				int[] mids = null;
				PropRowSet rowset;
				this.RunNspiClientCommand("getMatches " + debugContext, () => client.GetMatches(restriction, null, 1, null, out mids, out rowset));
				if (mids == null || mids.Length != 1 || mids[0] == 0)
				{
					throw new MigrationTransientException(Strings.MigrationExchangeRpcConnectionFailure, string.Format("Nspi::GetMatches failed to retrieve requested user {0}", recipientSmtpAddress));
				}
				MigrationLogger.Log(MigrationEventType.Verbose, "NSPI::get recipient successfully found {0}.  now modifying...", new object[]
				{
					recipientSmtpAddress
				});
				client.Stat.CurrentRecord = mids[0];
				this.RunNspiClientCommand("ModProps " + debugContext, () => client.ModProps(propTags, row));
				MigrationLogger.Log(MigrationEventType.Verbose, "NSPI::set recipient successfully modified {0} ", new object[]
				{
					recipientSmtpAddress
				});
			});
		}

		private static int GetBatchOfMIds(int[] source, int fromIndex, ref int[] target)
		{
			int num = source.Length - fromIndex;
			int num2 = (num > 1000) ? 1000 : num;
			if (target == null || target.Length != num2)
			{
				target = new int[num2];
			}
			Array.Copy(source, fromIndex, target, 0, num2);
			return num2;
		}

		private static PropTag[] ConvertPropTags(long[] source)
		{
			if (source == null)
			{
				return null;
			}
			PropTag[] array = new PropTag[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				array[i] = (PropTag)source[i];
			}
			return array;
		}

		private string GetNewDSA(ExchangeOutlookAnywhereEndpoint endpoint, string user)
		{
			MigrationUtil.ThrowOnNullArgument(endpoint, "endpoint");
			string[] array = new string[]
			{
				endpoint.ExchangeServer,
				endpoint.NspiServer
			};
			MigrationTransientException ex = null;
			foreach (string nspiServer in array)
			{
				try
				{
					return this.GetNewDSA(nspiServer, endpoint, user);
				}
				catch (MigrationTransientException ex2)
				{
					if (ex == null)
					{
						ex = ex2;
					}
				}
			}
			throw ex;
		}

		private string GetNewDSA(string nspiServer, ExchangeOutlookAnywhereEndpoint endpoint, string user)
		{
			MigrationUtil.ThrowOnNullArgument(endpoint, "endpoint");
			HTTPAuthentication auth = (endpoint.AuthenticationMethod == AuthenticationMethod.Ntlm) ? HTTPAuthentication.Ntlm : HTTPAuthentication.Basic;
			string debugContext = string.Format("server='{0}', domain='{1}', user='{2}', auth='{3}', email='{4}'", new object[]
			{
				nspiServer,
				endpoint.NetworkCredentials.Domain,
				endpoint.NetworkCredentials.UserName,
				auth,
				user
			});
			string server = null;
			this.RunOperation(debugContext, delegate
			{
				using (RfriClient client = new RfriClient(nspiServer, endpoint.RpcProxyServer, this.nspiRfrProtocolSequence, endpoint.NetworkCredentials, auth, AuthenticationService.Negotiate))
				{
					this.RunNspiRfrClientCommand(string.Format("command='GetNewDSA' {0}", debugContext), () => client.GetNewDSA(user ?? string.Empty, out server));
				}
			});
			return server;
		}

		private void RunNspiClientCommand(string debugContext, Func<NspiStatus> nspiCommand)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			NspiStatus nspiStatus = nspiCommand();
			stopwatch.Stop();
			MigrationLogger.Log(MigrationEventType.Verbose, "NSPI::{0} took {1} ms", new object[]
			{
				debugContext,
				stopwatch.ElapsedMilliseconds
			});
			if (nspiStatus != NspiStatus.Success)
			{
				MigrationLogger.Log(MigrationEventType.Error, "NSPI::{0} failed with status {1}", new object[]
				{
					debugContext,
					nspiStatus
				});
				if (this.reportData != null)
				{
					this.reportData.Append(Strings.MigrationReportNspiFailed(debugContext, nspiStatus.ToString()));
				}
				throw new MigrationTransientException(Strings.MigrationExchangeRpcConnectionFailure, debugContext);
			}
		}

		private void RunNspiRfrClientCommand(string debugContext, Func<RfriStatus> rfriCommand)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			RfriStatus rfriStatus = rfriCommand();
			stopwatch.Stop();
			MigrationLogger.Log(MigrationEventType.Verbose, "NSPIRFR::{0} took {1} ms", new object[]
			{
				debugContext,
				stopwatch.ElapsedMilliseconds
			});
			if (rfriStatus != RfriStatus.Success)
			{
				MigrationLogger.Log(MigrationEventType.Error, "NSPIRFR::{0} failed with status {1}", new object[]
				{
					debugContext,
					rfriStatus
				});
				if (this.reportData != null)
				{
					this.reportData.Append(Strings.MigrationReportNspiRfrFailed(debugContext, rfriStatus.ToString()));
				}
				throw new MigrationTransientException(Strings.MigrationExchangeRpcConnectionFailure, debugContext);
			}
		}

		private void RunOperation(string debugContext, Action command)
		{
			try
			{
				command();
			}
			catch (MigrationDataCorruptionException ex)
			{
				MigrationLogger.Log(MigrationEventType.Error, ex, "{0} has thrown corruptException.", new object[]
				{
					debugContext
				});
				throw new MigrationTransientException(Strings.MigrationExchangeRpcConnectionFailure, ex);
			}
			catch (RpcException ex2)
			{
				MigrationLogger.Log(MigrationEventType.Error, ex2, "{0} has thrown rpcException.", new object[]
				{
					debugContext
				});
				if (this.reportData != null)
				{
					this.reportData.Append(Strings.MigrationReportNspiFailed(debugContext, string.Empty), ex2, ReportEntryFlags.Failure | ReportEntryFlags.Target);
				}
				if (ex2.ErrorCode == 5)
				{
					throw new FailedToDiscoverCredentialTransientException(ex2);
				}
				throw new FailedToDiscoverRpcEndpointTransientException(ex2);
			}
		}

		private void RunNspiOperation(string commandName, string nspiServer, ExchangeOutlookAnywhereEndpoint endpoint, Action<string, NspiClient> nspiCommand)
		{
			HTTPAuthentication auth = (endpoint.AuthenticationMethod == AuthenticationMethod.Ntlm) ? HTTPAuthentication.Ntlm : HTTPAuthentication.Basic;
			string adserver = (!string.IsNullOrEmpty(nspiServer)) ? nspiServer : endpoint.NspiServer;
			string debugContext = string.Format("server='{0}', domain='{1}', user='{2}', auth='{3}'", new object[]
			{
				adserver,
				endpoint.NetworkCredentials.Domain,
				endpoint.NetworkCredentials.UserName,
				auth
			});
			this.RunOperation(debugContext, delegate
			{
				using (NspiClient client = new NspiClient(adserver, endpoint.RpcProxyServer, endpoint.NetworkCredentials, this.nspiProtocolSequence, auth, AuthenticationService.Negotiate))
				{
					this.RunNspiClientCommand(string.Format("command='Bind' {0}", debugContext), () => client.Bind(NspiBindFlags.None));
					nspiCommand(string.Format("command='{0}' {1}", commandName, debugContext), client);
				}
			});
		}

		private const int RootContainerId = 0;

		private readonly string nspiProtocolSequence;

		private readonly string nspiRfrProtocolSequence;

		private readonly ReportData reportData;
	}
}
