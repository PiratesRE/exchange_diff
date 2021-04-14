using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangeJobItemSubscriptionSettings : JobItemSubscriptionSettingsBase
	{
		public string MailboxDN { get; private set; }

		public string ExchangeServerDN { get; private set; }

		public string ExchangeServer { get; private set; }

		public string RPCProxyServer { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return ExchangeJobItemSubscriptionSettings.ExchangeJobItemSubscriptionSettingsPropertyDefinitions;
			}
		}

		protected override bool IsEmpty
		{
			get
			{
				return base.IsEmpty && string.IsNullOrEmpty(this.MailboxDN) && string.IsNullOrEmpty(this.ExchangeServerDN) && string.IsNullOrEmpty(this.ExchangeServer) && string.IsNullOrEmpty(this.RPCProxyServer);
			}
		}

		public override JobItemSubscriptionSettingsBase Clone()
		{
			return new ExchangeJobItemSubscriptionSettings
			{
				MailboxDN = this.MailboxDN,
				ExchangeServer = this.ExchangeServer,
				ExchangeServerDN = this.ExchangeServerDN,
				RPCProxyServer = this.RPCProxyServer,
				LastModifiedTime = base.LastModifiedTime
			};
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			MigrationHelper.WriteOrDeleteNullableProperty<string>(message, MigrationBatchMessageSchema.MigrationJobItemExchangeRemoteMailboxLegacyDN, this.MailboxDN);
			MigrationHelper.WriteOrDeleteNullableProperty<string>(message, MigrationBatchMessageSchema.MigrationJobItemExchangeRemoteServerLegacyDN, this.ExchangeServerDN);
			MigrationHelper.WriteOrDeleteNullableProperty<string>(message, MigrationBatchMessageSchema.MigrationJobItemExchangeRemoteServerHostName, this.ExchangeServer);
			MigrationHelper.WriteOrDeleteNullableProperty<string>(message, MigrationBatchMessageSchema.MigrationJobItemExchangeRPCProxyServerHostName, this.RPCProxyServer);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.MailboxDN = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemExchangeRemoteMailboxLegacyDN, null);
			this.ExchangeServerDN = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemExchangeRemoteServerLegacyDN, null);
			this.ExchangeServer = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemExchangeRemoteServerHostName, null);
			this.RPCProxyServer = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemExchangeRPCProxyServerHostName, null);
			return base.ReadFromMessageItem(message);
		}

		public override void UpdateFromDataRow(IMigrationDataRow request)
		{
			if (!(request is ExchangeMigrationDataRow))
			{
				throw new ArgumentException("expected an ExchangeMigrationDataRow", "request");
			}
		}

		internal static ExchangeJobItemSubscriptionSettings CreateFromAutodiscoverResponse(AutodiscoverClientResponse autodResponse)
		{
			ExchangeJobItemSubscriptionSettings exchangeJobItemSubscriptionSettings = new ExchangeJobItemSubscriptionSettings();
			if (autodResponse != null)
			{
				exchangeJobItemSubscriptionSettings.MailboxDN = autodResponse.MailboxDN;
				exchangeJobItemSubscriptionSettings.ExchangeServerDN = autodResponse.ExchangeServerDN;
				exchangeJobItemSubscriptionSettings.ExchangeServer = autodResponse.ExchangeServer;
				exchangeJobItemSubscriptionSettings.RPCProxyServer = autodResponse.RPCProxyServer;
			}
			exchangeJobItemSubscriptionSettings.LastModifiedTime = ExDateTime.UtcNow;
			return exchangeJobItemSubscriptionSettings;
		}

		internal static ExchangeJobItemSubscriptionSettings CreateFromProperties(string mailboxDN, string exchangeServerDN, string exchangeServer, string rpcProxyServer)
		{
			return new ExchangeJobItemSubscriptionSettings
			{
				MailboxDN = mailboxDN,
				ExchangeServerDN = exchangeServerDN,
				ExchangeServer = exchangeServer,
				RPCProxyServer = rpcProxyServer
			};
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			parent.Add(new object[]
			{
				new XElement("MailboxDN", this.MailboxDN),
				new XElement("ExchangeServerDN", this.ExchangeServerDN),
				new XElement("ExchangeServer", this.ExchangeServer),
				new XElement("RPCProxyServer", this.RPCProxyServer)
			});
		}

		public static readonly PropertyDefinition[] ExchangeJobItemSubscriptionSettingsPropertyDefinitions = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
		{
			new StorePropertyDefinition[]
			{
				MigrationBatchMessageSchema.MigrationJobItemExchangeRemoteMailboxLegacyDN,
				MigrationBatchMessageSchema.MigrationJobItemExchangeRemoteServerLegacyDN,
				MigrationBatchMessageSchema.MigrationJobItemExchangeRemoteServerHostName,
				MigrationBatchMessageSchema.MigrationJobItemExchangeRPCProxyServerHostName
			},
			SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions
		});
	}
}
