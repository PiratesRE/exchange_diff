using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PolicyTagList : Dictionary<Guid, PolicyTag>
	{
		internal PolicyTagList()
		{
		}

		private static string GetStringFromLocalizedStringPair(string cultureName, string localizedStringPair)
		{
			int num = localizedStringPair.IndexOf(":");
			if (num < 0 || num == localizedStringPair.Length - 1)
			{
				return null;
			}
			if (string.Compare(cultureName, localizedStringPair.Substring(0, num), StringComparison.OrdinalIgnoreCase) == 0)
			{
				return localizedStringPair.Substring(num + 1);
			}
			return null;
		}

		internal static PolicyTagList GetPolicyTagListFromMailboxSession(RetentionActionType type, MailboxSession session)
		{
			StoreId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.Inbox);
			IReadableUserConfiguration readableUserConfiguration = null;
			try
			{
				readableUserConfiguration = session.UserConfigurationManager.GetReadOnlyFolderConfiguration("MRM", UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary, defaultFolderId);
				if (readableUserConfiguration != null)
				{
					using (Stream xmlStream = readableUserConfiguration.GetXmlStream())
					{
						string sessionCultureName = null;
						if (session.Capabilities.CanHaveCulture)
						{
							sessionCultureName = session.PreferedCulture.Name;
						}
						return PolicyTagList.GetPolicyTakListFromXmlStream(type, xmlStream, sessionCultureName);
					}
				}
			}
			catch (ObjectNotFoundException)
			{
			}
			catch (CorruptDataException)
			{
			}
			finally
			{
				if (readableUserConfiguration != null)
				{
					readableUserConfiguration.Dispose();
				}
			}
			return null;
		}

		internal static PolicyTagList GetPolicyTakListFromXmlStream(RetentionActionType type, Stream xmlStream, string sessionCultureName)
		{
			PolicyTagList policyTagList = new PolicyTagList();
			if (xmlStream.Length == 0L)
			{
				return policyTagList;
			}
			using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(xmlStream))
			{
				try
				{
					xmlTextReader.MoveToContent();
					while ((xmlTextReader.NodeType != XmlNodeType.Element || (string.CompareOrdinal(xmlTextReader.Name, "PolicyTag") != 0 && string.CompareOrdinal(xmlTextReader.Name, "ArchiveTag") != 0 && string.CompareOrdinal(xmlTextReader.Name, "DefaultArchiveTag") != 0)) && xmlTextReader.Read())
					{
					}
					for (;;)
					{
						bool flag = string.CompareOrdinal(xmlTextReader.Name, "PolicyTag") == 0;
						bool flag2 = string.CompareOrdinal(xmlTextReader.Name, "ArchiveTag") == 0 || string.CompareOrdinal(xmlTextReader.Name, "DefaultArchiveTag") == 0;
						if (!flag && !flag2)
						{
							break;
						}
						PolicyTag policyTag = new PolicyTag();
						policyTag.IsArchive = flag2;
						if (xmlTextReader.MoveToAttribute("Guid"))
						{
							policyTag.PolicyGuid = new Guid(xmlTextReader.Value);
						}
						if (xmlTextReader.MoveToAttribute("Name"))
						{
							policyTag.Name = xmlTextReader.Value;
						}
						if (xmlTextReader.MoveToAttribute("Comment"))
						{
							policyTag.Description = xmlTextReader.Value;
						}
						if (xmlTextReader.MoveToAttribute("Type"))
						{
							string value;
							switch (value = xmlTextReader.Value)
							{
							case "Calendar":
								policyTag.Type = ElcFolderType.Calendar;
								goto IL_341;
							case "Contacts":
								policyTag.Type = ElcFolderType.Contacts;
								goto IL_341;
							case "DeletedItems":
								policyTag.Type = ElcFolderType.DeletedItems;
								goto IL_341;
							case "Drafts":
								policyTag.Type = ElcFolderType.Drafts;
								goto IL_341;
							case "Inbox":
								policyTag.Type = ElcFolderType.Inbox;
								goto IL_341;
							case "JunkEmail":
								policyTag.Type = ElcFolderType.JunkEmail;
								goto IL_341;
							case "Journal":
								policyTag.Type = ElcFolderType.Journal;
								goto IL_341;
							case "Notes":
								policyTag.Type = ElcFolderType.Notes;
								goto IL_341;
							case "Outbox":
								policyTag.Type = ElcFolderType.Outbox;
								goto IL_341;
							case "SentItems":
								policyTag.Type = ElcFolderType.SentItems;
								goto IL_341;
							case "Tasks":
								policyTag.Type = ElcFolderType.Tasks;
								goto IL_341;
							case "All":
								policyTag.Type = ElcFolderType.All;
								goto IL_341;
							case "ManagedCustomFolder":
								policyTag.Type = ElcFolderType.ManagedCustomFolder;
								goto IL_341;
							case "RssSubscriptions":
								policyTag.Type = ElcFolderType.RssSubscriptions;
								goto IL_341;
							case "SyncIssues":
								policyTag.Type = ElcFolderType.SyncIssues;
								goto IL_341;
							case "ConversationHistory":
								policyTag.Type = ElcFolderType.ConversationHistory;
								goto IL_341;
							}
							policyTag.Type = ElcFolderType.Personal;
						}
						IL_341:
						if (xmlTextReader.MoveToAttribute("IsVisible"))
						{
							policyTag.IsVisible = bool.Parse(xmlTextReader.Value);
						}
						if (xmlTextReader.MoveToAttribute("OptedInto"))
						{
							policyTag.OptedInto = bool.Parse(xmlTextReader.Value);
						}
						while (string.CompareOrdinal(xmlTextReader.Name, "ContentSettings") != 0 && string.CompareOrdinal(xmlTextReader.Name, "PolicyTag") != 0 && string.CompareOrdinal(xmlTextReader.Name, "ArchiveTag") != 0)
						{
							if (string.CompareOrdinal(xmlTextReader.Name, "DefaultArchiveTag") == 0)
							{
								break;
							}
							if (!xmlTextReader.Read())
							{
								break;
							}
							if (!string.IsNullOrEmpty(sessionCultureName))
							{
								if (string.CompareOrdinal(xmlTextReader.Name, "LocalizedName") == 0)
								{
									xmlTextReader.Read();
									bool flag3 = false;
									while (string.CompareOrdinal(xmlTextReader.Name, "LocalizedName") != 0)
									{
										if (!flag3 && !string.IsNullOrEmpty(xmlTextReader.Value))
										{
											string stringFromLocalizedStringPair = PolicyTagList.GetStringFromLocalizedStringPair(sessionCultureName, xmlTextReader.Value);
											if (stringFromLocalizedStringPair != null)
											{
												policyTag.Name = stringFromLocalizedStringPair;
												flag3 = true;
											}
										}
										if (!xmlTextReader.Read())
										{
											break;
										}
									}
								}
								if (string.CompareOrdinal(xmlTextReader.Name, "LocalizedComment") == 0)
								{
									xmlTextReader.Read();
									bool flag4 = false;
									while (string.CompareOrdinal(xmlTextReader.Name, "LocalizedComment") != 0)
									{
										if (!flag4 && !string.IsNullOrEmpty(xmlTextReader.Value))
										{
											string stringFromLocalizedStringPair2 = PolicyTagList.GetStringFromLocalizedStringPair(sessionCultureName, xmlTextReader.Value);
											if (stringFromLocalizedStringPair2 != null)
											{
												policyTag.Description = stringFromLocalizedStringPair2;
												flag4 = true;
											}
										}
										if (!xmlTextReader.Read())
										{
											break;
										}
									}
								}
							}
						}
						while (string.CompareOrdinal(xmlTextReader.Name, "ContentSettings") == 0)
						{
							if (xmlTextReader.MoveToAttribute("ExpiryAgeLimit"))
							{
								policyTag.TimeSpanForRetention = EnhancedTimeSpan.FromDays(double.Parse(xmlTextReader.Value));
							}
							if (xmlTextReader.MoveToAttribute("RetentionAction"))
							{
								policyTag.RetentionAction = (RetentionActionType)Enum.Parse(typeof(RetentionActionType), xmlTextReader.Value, true);
							}
							xmlTextReader.Read();
						}
						if (type == (RetentionActionType)0 || (type == RetentionActionType.MoveToArchive && flag2) || (type != (RetentionActionType)0 && type != RetentionActionType.MoveToArchive && flag))
						{
							policyTagList[policyTag.PolicyGuid] = policyTag;
						}
						if ((string.CompareOrdinal(xmlTextReader.Name, "PolicyTag") == 0 || string.CompareOrdinal(xmlTextReader.Name, "ArchiveTag") == 0 || string.CompareOrdinal(xmlTextReader.Name, "DefaultArchiveTag") == 0) && xmlTextReader.NodeType == XmlNodeType.EndElement)
						{
							xmlTextReader.Read();
						}
					}
				}
				catch (XmlException ex)
				{
				}
				catch (ArgumentException ex2)
				{
				}
				catch (FormatException ex3)
				{
				}
			}
			return policyTagList;
		}

		internal const string ElcTagConfigurationXSOClass = "MRM";

		internal const UserConfigurationTypes ElcConfigurationTypes = UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary;

		private const string Info = "Info";

		private const string Version = "version";

		private const string VersionValue = "Exchange.14";

		private const string Data = "Data";

		private const string RetentionHoldTag = "RetentionHold";

		private const string Enabled = "Enabled";

		private const string RetentionComment = "RetentionComment";

		private const string RetentionURL = "RetentionUrl";

		private const string PolicyTag = "PolicyTag";

		private const string ArchiveTag = "ArchiveTag";

		private const string DefaultArchiveTag = "DefaultArchiveTag";

		private const string Guid = "Guid";

		private const string Name = "Name";

		private const string LocalizedNameElement = "LocalizedName";

		private const string Comment = "Comment";

		private const string LocalizedCommentElement = "LocalizedComment";

		private const string Type = "Type";

		private const string MustDisplayComment = "MustDisplayComment";

		private const string IsVisibleElement = "IsVisible";

		private const string OptedIntoElement = "OptedInto";

		private const string ContentSettingsElement = "ContentSettings";

		private const string ExpiryAgeLimit = "ExpiryAgeLimit";

		private const string MessageClass = "MessageClass";

		private const string RetentionAction = "RetentionAction";

		private const char Delimiter = ',';
	}
}
