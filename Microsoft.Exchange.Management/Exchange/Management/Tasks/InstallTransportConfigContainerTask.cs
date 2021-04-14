using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "TransportConfigContainer")]
	public sealed class InstallTransportConfigContainerTask : InstallContainerTaskBase<TransportConfigContainer>
	{
		protected override void InternalProcessRecord()
		{
			bool flag = base.DataSession.Read<TransportConfigContainer>(this.DataObject.Id) == null;
			MessageDeliveryGlobalSettings[] array = null;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(base.CurrentOrganizationId), 48, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallTransportSettingsContainerTask.cs");
			if (flag)
			{
				base.InternalProcessRecord();
				array = tenantOrTopologyConfigurationSession.Find<MessageDeliveryGlobalSettings>(base.CurrentOrgContainerId, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, MessageDeliveryGlobalSettings.DefaultName), null, 1);
			}
			if (base.Organization == null)
			{
				if (flag)
				{
					if (array != null && array.Length == 1)
					{
						this.ApplyTiGlobalSettingsToE12(array[0], tenantOrTopologyConfigurationSession);
						return;
					}
					this.ApplyDefaultTransportSettings(tenantOrTopologyConfigurationSession);
					return;
				}
				else
				{
					this.ApplyE12SettingsToTiGlobalSettings(tenantOrTopologyConfigurationSession);
				}
			}
		}

		private void ApplyDefaultTransportSettings(IConfigurationSession session)
		{
			TransportConfigContainer[] array = session.Find<TransportConfigContainer>(null, QueryScope.SubTree, null, null, 1);
			if (array == null || array.Length == 0)
			{
				return;
			}
			array[0].MaxRecipientEnvelopeLimit = 500;
			array[0].MaxReceiveSize = ByteQuantifiedSize.FromKB(10240UL);
			array[0].MaxSendSize = ByteQuantifiedSize.FromKB(10240UL);
			session.Save(array[0]);
		}

		private void ApplyTiGlobalSettingsToE12(MessageDeliveryGlobalSettings settings, IConfigurationSession session)
		{
			TransportConfigContainer[] array = session.Find<TransportConfigContainer>(null, QueryScope.SubTree, null, null, 1);
			if (array == null || array.Length == 0)
			{
				return;
			}
			array[0].MaxRecipientEnvelopeLimit = settings.MaxRecipientEnvelopeLimit;
			array[0].MaxReceiveSize = settings.MaxReceiveSize;
			array[0].MaxSendSize = settings.MaxSendSize;
			session.Save(array[0]);
		}

		private void ApplyE12SettingsToTiGlobalSettings(IConfigurationSession session)
		{
			TransportConfigContainer[] array = session.Find<TransportConfigContainer>(null, QueryScope.SubTree, null, null, 1);
			if (array == null || array.Length == 0)
			{
				return;
			}
			MessageDeliveryGlobalSettings[] array2 = session.Find<MessageDeliveryGlobalSettings>(session.GetOrgContainerId(), QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, MessageDeliveryGlobalSettings.DefaultName), null, 1);
			if (array2 == null || array2.Length == 0)
			{
				return;
			}
			InstallTransportConfigContainerTask.SizeRestriction sizeRestriction = new InstallTransportConfigContainerTask.SizeRestriction(array[0].MaxRecipientEnvelopeLimit, array[0].MaxSendSize, array[0].MaxReceiveSize);
			InstallTransportConfigContainerTask.SizeRestriction sizeRestriction2 = new InstallTransportConfigContainerTask.SizeRestriction(array2[0].MaxRecipientEnvelopeLimit, array2[0].MaxSendSize, array2[0].MaxReceiveSize);
			InstallTransportConfigContainerTask.SizeRestriction sizeRestriction3 = InstallTransportConfigContainerTask.SizeRestriction.Min(sizeRestriction, sizeRestriction2);
			if (!sizeRestriction3.Equals(sizeRestriction))
			{
				array[0].MaxRecipientEnvelopeLimit = sizeRestriction3.MaxRecipientEnvelopeLimit;
				array[0].MaxReceiveSize = sizeRestriction3.MaxReceiveSize;
				array[0].MaxSendSize = sizeRestriction3.MaxSendSize;
				session.Save(array[0]);
			}
			if (!sizeRestriction3.Equals(sizeRestriction2))
			{
				array2[0].MaxRecipientEnvelopeLimit = sizeRestriction3.MaxRecipientEnvelopeLimit;
				array2[0].MaxReceiveSize = sizeRestriction3.MaxReceiveSize;
				array2[0].MaxSendSize = sizeRestriction3.MaxSendSize;
				session.Save(array2[0]);
			}
		}

		private const int DefaultMaxRecipientEnvelopeLimit = 500;

		private const int DefaultMaxReceiveSize = 10240;

		private const int DefaultMaxSendSize = 10240;

		private struct SizeRestriction : IEquatable<InstallTransportConfigContainerTask.SizeRestriction>
		{
			public SizeRestriction(Unlimited<int> maxRecipientEnvelopeLimit, Unlimited<ByteQuantifiedSize> maxSendSize, Unlimited<ByteQuantifiedSize> maxReceiveSize)
			{
				this.maxRecipientEnvelopeLimit = maxRecipientEnvelopeLimit;
				this.maxSendSize = maxSendSize;
				this.maxReceiveSize = maxReceiveSize;
			}

			public Unlimited<int> MaxRecipientEnvelopeLimit
			{
				get
				{
					return this.maxRecipientEnvelopeLimit;
				}
			}

			public Unlimited<ByteQuantifiedSize> MaxSendSize
			{
				get
				{
					return this.maxSendSize;
				}
			}

			public Unlimited<ByteQuantifiedSize> MaxReceiveSize
			{
				get
				{
					return this.maxReceiveSize;
				}
			}

			public static InstallTransportConfigContainerTask.SizeRestriction Min(InstallTransportConfigContainerTask.SizeRestriction left, InstallTransportConfigContainerTask.SizeRestriction right)
			{
				return new InstallTransportConfigContainerTask.SizeRestriction(InstallTransportConfigContainerTask.SizeRestriction.Min<int>(left.MaxRecipientEnvelopeLimit, right.MaxRecipientEnvelopeLimit), InstallTransportConfigContainerTask.SizeRestriction.Min<ByteQuantifiedSize>(left.MaxSendSize, right.MaxSendSize), InstallTransportConfigContainerTask.SizeRestriction.Min<ByteQuantifiedSize>(left.MaxReceiveSize, right.MaxReceiveSize));
			}

			public override bool Equals(object other)
			{
				if (other is InstallTransportConfigContainerTask.SizeRestriction)
				{
					InstallTransportConfigContainerTask.SizeRestriction other2 = (InstallTransportConfigContainerTask.SizeRestriction)other;
					return this.Equals(other2);
				}
				return false;
			}

			public bool Equals(InstallTransportConfigContainerTask.SizeRestriction other)
			{
				return this.MaxRecipientEnvelopeLimit == other.MaxRecipientEnvelopeLimit && this.MaxSendSize == other.MaxSendSize && this.MaxReceiveSize == other.MaxReceiveSize;
			}

			public override int GetHashCode()
			{
				return this.MaxRecipientEnvelopeLimit.GetHashCode() ^ this.MaxSendSize.GetHashCode() ^ this.MaxReceiveSize.GetHashCode();
			}

			private static Unlimited<T> Min<T>(Unlimited<T> left, Unlimited<T> right) where T : struct, IComparable
			{
				if (left.CompareTo(right) < 0)
				{
					return left;
				}
				return right;
			}

			private Unlimited<int> maxRecipientEnvelopeLimit;

			private Unlimited<ByteQuantifiedSize> maxSendSize;

			private Unlimited<ByteQuantifiedSize> maxReceiveSize;
		}
	}
}
