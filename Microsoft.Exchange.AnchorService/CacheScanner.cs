using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.AnchorService
{
	internal class CacheScanner : CacheProcessorBase
	{
		internal CacheScanner(AnchorContext context, WaitHandle stopEvent) : base(context, stopEvent)
		{
			this.nextProcessTime = ExDateTime.MinValue;
		}

		internal override string Name
		{
			get
			{
				return "CacheScanner";
			}
		}

		public override XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(parameters);
			diagnosticInfo.Add(new XElement("nextProcessTime", this.nextProcessTime));
			diagnosticInfo.Add(new XElement("diagnosticMessage", this.diagnosticMessage));
			return diagnosticInfo;
		}

		internal override bool ShouldProcess()
		{
			return ExDateTime.UtcNow >= this.nextProcessTime;
		}

		internal override bool Process(JobCache data)
		{
			AnchorUtil.ThrowOnNullArgument(data, "data");
			if (base.LastRunTime == null)
			{
				TimeSpan config = base.Context.Config.GetConfig<TimeSpan>("ScannerInitialTimeDelay");
				if (!config.Equals(TimeSpan.Zero))
				{
					base.Context.Logger.Log(MigrationEventType.Information, "Sleeping initial delay of {0} on start", new object[]
					{
						config
					});
					base.StopEvent.WaitOne(config, false);
				}
			}
			this.nextProcessTime = ExDateTime.UtcNow.Add(base.Context.Config.GetConfig<TimeSpan>("ScannerTimeDelay"));
			if (base.Context.Config.GetConfig<bool>("ScannerClearCacheOnRefresh"))
			{
				data.Clear();
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			HashSet<ADObjectId> hashSet = new HashSet<ADObjectId>();
			foreach (ADUser aduser in this.GetLocalMailboxUsers())
			{
				hashSet.Add(aduser.ObjectId);
				bool flag = data.Contains(aduser.ObjectId);
				if (data.TryAdd(aduser, false))
				{
					if (flag)
					{
						num2++;
					}
					else
					{
						num++;
					}
				}
				else if (flag)
				{
					num3++;
				}
			}
			foreach (CacheEntryBase cacheEntryBase in data.Get())
			{
				if (!hashSet.Contains(cacheEntryBase.ObjectId))
				{
					if (data.TryUpdate(cacheEntryBase, false))
					{
						num2++;
					}
					else
					{
						num3++;
					}
				}
			}
			this.diagnosticMessage = string.Format(" Modified {0}, Added {1}, removed {2} cache entries", num2, num, num3);
			if (num > 0 || num3 > 0)
			{
				base.Context.Logger.Log(MigrationEventType.Information, this.diagnosticMessage, new object[0]);
			}
			else
			{
				base.Context.Logger.Log(MigrationEventType.Verbose, this.diagnosticMessage, new object[0]);
			}
			return false;
		}

		protected virtual IEnumerable<ADUser> GetLocalMailboxUsers()
		{
			foreach (Guid databaseGuid in this.GetLocalActiveDatabases())
			{
				foreach (ADUser user in OrganizationMailbox.FindByDatabaseId(base.Context.ActiveCapability, new ADObjectId(databaseGuid)))
				{
					yield return user;
				}
			}
			yield break;
		}

		protected ICollection<Guid> GetLocalActiveDatabases()
		{
			List<Guid> list = new List<Guid>();
			try
			{
				using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=AnchorService", LocalServer.GetServer().Fqdn, null, null, null))
				{
					MdbStatus[] array = exRpcAdmin.ListMdbStatus(true);
					if (array != null)
					{
						foreach (MdbStatus mdbStatus in array)
						{
							if ((mdbStatus.Status & MdbStatusFlags.Online) != MdbStatusFlags.Offline && (mdbStatus.Status & MdbStatusFlags.InRecoverySG) == MdbStatusFlags.Offline)
							{
								list.Add(mdbStatus.MdbGuid);
							}
						}
					}
				}
			}
			catch (MapiRetryableException ex)
			{
				base.Context.Logger.Log(MigrationEventType.Warning, "error when looking for local databases {0}", new object[]
				{
					ex
				});
				throw new MigrationLocalDatabasesNotFoundException(ex);
			}
			catch (MapiPermanentException ex2)
			{
				base.Context.Logger.Log(MigrationEventType.Error, "error when looking for local databases {0}", new object[]
				{
					ex2
				});
				throw new MigrationLocalDatabasesNotFoundException(ex2);
			}
			return list;
		}

		private ExDateTime nextProcessTime;

		private string diagnosticMessage;
	}
}
