using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Resolver
{
	internal class ActiveDirectoryTargetResolver : ITargetResolver
	{
		public static TimeSpan DataLookupTime { get; set; } = TimeSpan.FromMinutes(1.0);

		public IEnumerable<ComplianceMessage> Resolve(IEnumerable<ComplianceMessage> sources)
		{
			return this.ResolveServers(this.ResolveTargets(sources));
		}

		private IEnumerable<ComplianceMessage> ResolveServers(IEnumerable<ComplianceMessage> sources)
		{
			MemoryCache cache;
			FaultDefinition faultDefinition;
			if (Registry.Instance.TryGetInstance<MemoryCache>(RegistryComponent.Common, CommonComponent.BestEffortCache, out cache, out faultDefinition, "ResolveServers", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Resolver\\ActiveDirectoryTargetResolver.cs", 84))
			{
				faultDefinition = null;
				cache = null;
			}
			using (IEnumerator<ComplianceMessage> enumerator = sources.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ComplianceMessage source = enumerator.Current;
					if (source.MessageTarget.Database != Guid.Empty && !ExceptionHandler.IsFaulted(source))
					{
						string text = null;
						string databaseKey = string.Format("DATABASELOCATION:{0}", source.MessageTarget.Database);
						if (cache != null)
						{
							text = (cache.Get(databaseKey, null) as string);
							if (!string.IsNullOrEmpty(text))
							{
								string key = string.Format("BADSERVER:{0}", text);
								if (cache.Get(key, null) == null)
								{
									source.MessageTarget.Server = text;
								}
								else
								{
									text = null;
								}
							}
						}
						if (string.IsNullOrEmpty(text))
						{
							if (!ExceptionHandler.DataSource.TryRun(delegate
							{
								using (MailboxServerLocator mailboxServerLocator = MailboxServerLocator.CreateWithResourceForestFqdn(source.MessageTarget.Database, null))
								{
									BackEndServer server = mailboxServerLocator.GetServer();
									source.MessageTarget.Server = server.Fqdn;
									if (cache != null)
									{
										cache.Set(databaseKey, server.Fqdn, DateTimeOffset.Now.AddMinutes(5.0), null);
									}
								}
							}, ActiveDirectoryTargetResolver.DataLookupTime, out faultDefinition, source, null, default(CancellationToken), null, "ResolveServers", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Resolver\\ActiveDirectoryTargetResolver.cs", 115))
							{
								ExceptionHandler.FaultMessage(source, faultDefinition, true);
							}
						}
					}
					yield return source;
				}
			}
			yield break;
		}

		private IEnumerable<ComplianceMessage> ResolveTargets(IEnumerable<ComplianceMessage> sources)
		{
			byte[] lastTenantId = null;
			using (IEnumerator<ComplianceMessage> enumerator = sources.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ComplianceMessage source = enumerator.Current;
					if (source.MessageTarget.TargetType == Target.Type.Driver)
					{
						source.MessageTarget.Database = Guid.Empty;
						source.MessageTarget.Mailbox = Guid.Empty;
						yield return source;
					}
					else if (!ExceptionHandler.IsFaulted(source) && (source.MessageTarget.Database == Guid.Empty || source.MessageTarget.Mailbox == Guid.Empty))
					{
						bool resolved = false;
						IRecipientSession recipientSession = null;
						if (lastTenantId == source.TenantId)
						{
							goto IL_22C;
						}
						FaultDefinition faultDefinition;
						if (ExceptionHandler.DataSource.TryRun(delegate
						{
							recipientSession = this.GetSession(source.TenantId);
						}, ActiveDirectoryTargetResolver.DataLookupTime, out faultDefinition, source, null, default(CancellationToken), null, "ResolveTargets", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Resolver\\ActiveDirectoryTargetResolver.cs", 170))
						{
							goto IL_22C;
						}
						ExceptionHandler.FaultMessage(source, faultDefinition, true);
						IL_51C:
						if (!resolved && ExceptionHandler.IsFaulted(source))
						{
							yield return source;
							continue;
						}
						continue;
						IL_22C:
						if (recipientSession != null)
						{
							ActiveDirectoryTargetResolver.<>c__DisplayClass15 CS$<>8__locals3 = new ActiveDirectoryTargetResolver.<>c__DisplayClass15();
							CS$<>8__locals3.filter = this.GetFilter(source.MessageTarget);
							if (CS$<>8__locals3.filter == null)
							{
								ExceptionHandler.FaultMessage(source, FaultDefinition.FromErrorString("Could not build query filter for target", "ResolveTargets", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Resolver\\ActiveDirectoryTargetResolver.cs", 226), true);
								goto IL_51C;
							}
							ADPagedReader<ADRawEntry> reader = null;
							if (!ExceptionHandler.DataSource.TryRun(delegate
							{
								reader = recipientSession.FindPagedADRawEntry(null, QueryScope.OneLevel, CS$<>8__locals3.filter, null, 100, ActiveDirectoryTargetResolver.properties);
							}, ActiveDirectoryTargetResolver.DataLookupTime, out faultDefinition, source, null, default(CancellationToken), null, "ResolveTargets", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Resolver\\ActiveDirectoryTargetResolver.cs", 191))
							{
								ExceptionHandler.FaultMessage(source, faultDefinition, true);
								goto IL_51C;
							}
							IEnumerator<ADRawEntry> enumerator2 = reader.GetEnumerator();
							for (;;)
							{
								try
								{
									goto IL_4A6;
									IL_352:
									ADRawEntry entry = enumerator2.Current;
									resolved = true;
									Tuple<Guid, ADObjectId> archive = new Tuple<Guid, ADObjectId>((Guid)entry[ADUserSchema.ArchiveGuid], entry[ADUserSchema.ArchiveDatabaseRaw] as ADObjectId);
									Tuple<Guid, ADObjectId> primary = new Tuple<Guid, ADObjectId>((Guid)entry[ADMailboxRecipientSchema.ExchangeGuid], entry[ADMailboxRecipientSchema.Database] as ADObjectId);
									string smtpAddress = entry[ADRecipientSchema.PrimarySmtpAddress] as string;
									using (IEnumerator<ComplianceMessage> enumerator3 = this.GetMessageForMailboxes(source, smtpAddress, new Tuple<Guid, ADObjectId>[]
									{
										primary,
										archive
									}).GetEnumerator())
									{
										goto IL_493;
										IL_43D:
										ComplianceMessage message = enumerator3.Current;
										yield return message;
										if (source.ComplianceMessageType != ComplianceMessageType.StartJob)
										{
											goto IL_493;
										}
									}
								}
								finally
								{
									if (enumerator2 != null)
									{
										enumerator2.Dispose();
									}
								}
								break;
								IL_493:
								IEnumerator<ComplianceMessage> enumerator3;
								if (enumerator3.MoveNext())
								{
									goto IL_43D;
								}
								IL_4A6:
								if (!enumerator2.MoveNext())
								{
									goto Block_13;
								}
								goto IL_352;
							}
							goto IL_51C;
							Block_13:;
						}
						if (!resolved)
						{
							ExceptionHandler.FaultMessage(source, FaultDefinition.FromErrorString("Could not resolve target", "ResolveTargets", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Resolver\\ActiveDirectoryTargetResolver.cs", 233), true);
							goto IL_51C;
						}
						goto IL_51C;
					}
					else
					{
						yield return source;
					}
				}
			}
			yield break;
		}

		private IEnumerable<ComplianceMessage> GetMessageForMailboxes(ComplianceMessage message, string identifier, params Tuple<Guid, ADObjectId>[] mailboxDatabasePairs)
		{
			foreach (Tuple<Guid, ADObjectId> mailboxDatabasePair in mailboxDatabasePairs)
			{
				if (mailboxDatabasePair.Item1 != Guid.Empty && mailboxDatabasePair.Item2 != null && mailboxDatabasePair.Item2.ObjectGuid != Guid.Empty)
				{
					ComplianceMessage newMessage = message.Clone();
					newMessage.MessageId = string.Format("{0}\\{1}", newMessage.MessageId, mailboxDatabasePair.Item1);
					newMessage.MessageTarget = message.MessageTarget.Clone();
					newMessage.MessageTarget.Mailbox = mailboxDatabasePair.Item1;
					newMessage.MessageTarget.Database = mailboxDatabasePair.Item2.ObjectGuid;
					if (!string.IsNullOrEmpty(identifier))
					{
						newMessage.MessageTarget.Identifier = identifier;
					}
					yield return newMessage;
				}
			}
			yield break;
		}

		private IRecipientSession GetSession(byte[] tenantId)
		{
			OrganizationId scopingOrganizationId = null;
			IRecipientSession result = null;
			if (OrganizationId.TryCreateFromBytes(tenantId, Encoding.UTF8, out scopingOrganizationId))
			{
				ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId);
				adsessionSettings.IncludeInactiveMailbox = true;
				result = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, adsessionSettings, 297, "GetSession", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Resolver\\ActiveDirectoryTargetResolver.cs");
			}
			return result;
		}

		private QueryFilter GetFilter(Target target)
		{
			switch (target.TargetType)
			{
			case Target.Type.MailboxSmtpAddress:
				return new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, "SMTP:" + target.Identifier);
			case Target.Type.MailboxGuid:
			{
				Guid guid;
				if (Guid.TryParse(target.Identifier, out guid))
				{
					return new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.ExchangeGuid, guid);
				}
				break;
			}
			case Target.Type.QueryFilter:
			{
				QueryParser.ConvertValueFromStringDelegate convertDelegate = (object value, Type type) => ADValueConvertor.ConvertValueFromString(value as string, type, null);
				return new QueryParser(target.Identifier, ObjectSchema.GetInstance<ADUserSchema>(), QueryParser.Capabilities.All, null, convertDelegate).ParseTree;
			}
			case Target.Type.InactiveMailboxes:
				return new BitMaskAndFilter(ADRecipientSchema.RecipientSoftDeletedStatus, 8UL);
			case Target.Type.TenantMaster:
				return MailboxDataProvider.DiscoverySystemMailboxFilter;
			}
			return null;
		}

		private static ADPropertyDefinition[] properties = new ADPropertyDefinition[]
		{
			ADUserSchema.ArchiveGuid,
			ADUserSchema.ArchiveDatabaseRaw,
			ADMailboxRecipientSchema.ExchangeGuid,
			ADMailboxRecipientSchema.Database,
			ADRecipientSchema.PrimarySmtpAddress
		};
	}
}
