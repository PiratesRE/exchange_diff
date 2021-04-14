using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Client
{
	public class InterExchangeWorkloadClient : WorkloadClientBase
	{
		public InterExchangeWorkloadClient()
		{
			this.rpsProviderAssembly = new InterExchangeWorkloadClient.RpsProviderAssembly();
		}

		protected override Task<IEnumerable<ComplianceMessage>> SendMessageAsyncInternal(IEnumerable<ComplianceMessage> messages)
		{
			return Task.Run<IEnumerable<ComplianceMessage>>(delegate()
			{
				StatusPayload statusPayload = new StatusPayload();
				foreach (ComplianceMessage complianceMessage in messages)
				{
					string tenantName = string.Empty;
					OrganizationId organizationId;
					if (OrganizationId.TryCreateFromBytes(complianceMessage.TenantId, Encoding.UTF8, out organizationId) && organizationId.OrganizationalUnit != null)
					{
						tenantName = organizationId.OrganizationalUnit.Name;
						using (IDisposable disposable = (IDisposable)this.GetRpsProvider(tenantName))
						{
							byte[] value = ComplianceSerializer.Serialize<ComplianceMessage>(ComplianceMessage.Description, complianceMessage);
							PSCommand pscommand = new PSCommand();
							pscommand.AddCommand(new Command("Send-ComplianceMessage", false));
							pscommand.AddParameter("SerializedComplianceMessage", value);
							MethodBase method = this.rpsProviderAssembly.RpsProviderType.GetMethod("Execute", new Type[]
							{
								typeof(PSCommand),
								typeof(TimeSpan)
							});
							object obj = disposable;
							object[] array = new object[2];
							array[0] = pscommand;
							IEnumerable<PSObject> enumerable = (IEnumerable<PSObject>)method.Invoke(obj, array);
							if (enumerable != null && enumerable.Count<PSObject>() == 1 && (bool)enumerable.ToArray<PSObject>()[0].BaseObject)
							{
								statusPayload.QueuedMessages.Add(complianceMessage.MessageId);
							}
						}
					}
				}
				return (IEnumerable<ComplianceMessage>)new ComplianceMessage[]
				{
					new ComplianceMessage
					{
						Payload = ComplianceSerializer.Serialize<StatusPayload>(StatusPayload.Description, statusPayload)
					}
				};
			});
		}

		private object GetRpsProvider(string tenantName)
		{
			object obj = this.rpsProviderAssembly.RpsProverFactoryType.GetConstructor(Type.EmptyTypes).Invoke(null);
			return this.rpsProviderAssembly.RpsProverFactoryType.GetMethod("CreateProviderForTenant", new Type[]
			{
				typeof(string),
				typeof(ADObjectId),
				typeof(string)
			}).Invoke(obj, new object[]
			{
				"ComplianceTaskDistributor",
				new ADObjectId("DN=" + tenantName),
				"exo"
			});
		}

		private const string RPSDataProviderCallerId = "ComplianceTaskDistributor";

		private InterExchangeWorkloadClient.RpsProviderAssembly rpsProviderAssembly;

		private class RpsProviderAssembly
		{
			public RpsProviderAssembly()
			{
				Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.PowershellDataProvider");
				this.RpsProverFactoryType = assembly.GetType("Microsoft.Exchange.Hygiene.Migration.PowershellDataProviderFactory");
				this.RpsProviderType = assembly.GetType("Microsoft.Exchange.Data.IRemotePowershellDataProvider");
			}

			public Type RpsProverFactoryType { get; private set; }

			public Type RpsProviderType { get; private set; }

			private const string RemotePowershellProviderAssembly = "Microsoft.Exchange.Hygiene.PowershellDataProvider";
		}
	}
}
