using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class SearchSource
	{
		public SearchSource()
		{
			this.ExtendedAttributes = new SearchSource.ExtendedAttributeStore();
		}

		public string ReferenceId { get; set; }

		public string OriginalReferenceId { get; set; }

		public string FolderSpec { get; set; }

		public SourceLocation SourceLocation { get; set; }

		public SourceType SourceType { get; set; }

		public SearchSource.ExtendedAttributeStore ExtendedAttributes { get; set; }

		public SearchRecipient Recipient { get; set; }

		public MailboxInfo MailboxInfo { get; set; }

		public bool CanBeCrossPremise { get; set; }

		public static SourceType GetSourceType(SearchSource source)
		{
			if (string.IsNullOrEmpty(source.ReferenceId))
			{
				return source.SourceType;
			}
			if (source.ReferenceId.StartsWith("/", StringComparison.InvariantCultureIgnoreCase) && (source.ReferenceId.IndexOf("o=", StringComparison.InvariantCultureIgnoreCase) != -1 || source.ReferenceId.IndexOf("ou=", StringComparison.InvariantCultureIgnoreCase) != -1 || source.ReferenceId.IndexOf("cn=", StringComparison.InvariantCultureIgnoreCase) != -1))
			{
				return SourceType.LegacyExchangeDN;
			}
			if (source.ReferenceId.StartsWith("\\", StringComparison.InvariantCultureIgnoreCase))
			{
				return SourceType.PublicFolder;
			}
			return SourceType.Recipient;
		}

		public virtual object GetProperty(PropertyDefinition propertyDefinition)
		{
			if (this.Recipient != null && this.Recipient.ADEntry != null)
			{
				return this.Recipient.ADEntry[propertyDefinition];
			}
			return this.MailboxInfo[propertyDefinition];
		}

		public SearchSource Clone()
		{
			SearchSource searchSource = (SearchSource)base.MemberwiseClone();
			searchSource.ExtendedAttributes = new SearchSource.ExtendedAttributeStore(this.ExtendedAttributes);
			searchSource.MailboxInfo = null;
			return searchSource;
		}

		public string GetPrimarySmtpAddress()
		{
			string result = null;
			if (this.MailboxInfo != null)
			{
				SmtpAddress primarySmtpAddress = this.MailboxInfo.PrimarySmtpAddress;
				result = this.MailboxInfo.PrimarySmtpAddress.ToString();
			}
			return result;
		}

		internal MailboxSearchScope GetScope()
		{
			MailboxSearchScopeType searchScopeType = 0;
			Enum.TryParse<MailboxSearchScopeType>(this.SourceType.ToString(), out searchScopeType);
			MailboxSearchLocation mailboxSearchLocation = 2;
			if (this.SourceLocation == SourceLocation.ArchiveOnly)
			{
				mailboxSearchLocation = 1;
			}
			else if (this.SourceLocation == SourceLocation.PrimaryOnly)
			{
				mailboxSearchLocation = 0;
			}
			MailboxSearchScope mailboxSearchScope = new MailboxSearchScope(this.ReferenceId, mailboxSearchLocation);
			mailboxSearchScope.SearchScopeType = searchScopeType;
			this.TrySaveMailboxInfo();
			foreach (KeyValuePair<string, string> keyValuePair in this.ExtendedAttributes)
			{
				mailboxSearchScope.ExtendedAttributes.Add(new ExtendedAttribute(keyValuePair.Key, keyValuePair.Value));
			}
			return mailboxSearchScope;
		}

		internal bool TrySaveMailboxInfo()
		{
			Recorder.Trace(4L, TraceType.InfoTrace, new object[]
			{
				"SearchSource.TrySaveMailboxInfo Source:",
				this.ReferenceId,
				"SourceType:",
				this.SourceType,
				"Location:",
				this.SourceLocation,
				"MailboxInfo:",
				this.MailboxInfo
			});
			try
			{
				if (this.MailboxInfo != null)
				{
					int num = 0;
					this.ExtendedAttributes[this.GetMailboxInfoKey(num++)] = SearchHelper.ConvertToString(this.OriginalReferenceId, typeof(string));
					this.ExtendedAttributes[this.GetMailboxInfoKey(num++)] = SearchHelper.ConvertToString(this.FolderSpec, typeof(string));
					this.ExtendedAttributes[this.GetMailboxInfoKey(num++)] = SearchHelper.ConvertToString(this.MailboxInfo.Type, typeof(MailboxType));
					for (int i = 0; i < MailboxInfo.PropertyDefinitionCollection.Length; i++)
					{
						if (this.MailboxInfo.PropertyMap.ContainsKey(MailboxInfo.PropertyDefinitionCollection[i]))
						{
							this.ExtendedAttributes[this.GetMailboxInfoKey(num++)] = SearchHelper.ConvertToString(this.MailboxInfo.PropertyMap[MailboxInfo.PropertyDefinitionCollection[i]], MailboxInfo.PropertyDefinitionCollection[i]);
						}
						else
						{
							this.ExtendedAttributes[this.GetMailboxInfoKey(num++)] = string.Empty;
						}
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				Recorder.Trace(4L, TraceType.WarningTrace, new object[]
				{
					"SearchSource.TrySaveMailboxInfo Failed Source:",
					this.ReferenceId,
					"Error:",
					ex
				});
			}
			return false;
		}

		internal bool TryLoadMailboxInfo()
		{
			Recorder.Trace(4L, TraceType.InfoTrace, new object[]
			{
				"SearchSource.TryLoadMailboxInfo Source:",
				this.ReferenceId,
				"SourceType:",
				this.SourceType,
				"Location:",
				this.SourceLocation,
				"ExtendedAttributes:",
				this.ExtendedAttributes
			});
			try
			{
				if (this.MailboxInfo == null)
				{
					int index = 0;
					if (this.ExtendedAttributes.ContainsKey(this.GetMailboxInfoKey(index)))
					{
						this.OriginalReferenceId = (string)SearchHelper.ConvertFromString(this.ExtendedAttributes[this.GetMailboxInfoKey(index++)], typeof(string));
						this.FolderSpec = (string)SearchHelper.ConvertFromString(this.ExtendedAttributes[this.GetMailboxInfoKey(index++)], typeof(string));
						MailboxType type = SearchHelper.ConvertFromString<MailboxType>(this.ExtendedAttributes[this.GetMailboxInfoKey(index++)]);
						Dictionary<PropertyDefinition, object> dictionary = new Dictionary<PropertyDefinition, object>();
						for (int i = 0; i < MailboxInfo.PropertyDefinitionCollection.Length; i++)
						{
							dictionary[MailboxInfo.PropertyDefinitionCollection[i]] = SearchHelper.ConvertFromString(this.ExtendedAttributes[this.GetMailboxInfoKey(index++)], MailboxInfo.PropertyDefinitionCollection[i]);
						}
						this.MailboxInfo = new MailboxInfo(dictionary, type);
						this.MailboxInfo.SourceMailbox = this;
						this.MailboxInfo.Folder = this.FolderSpec;
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Recorder.Trace(4L, TraceType.WarningTrace, new object[]
				{
					"SearchSource.TryLoadMailboxInfo Failed Source:",
					this.ReferenceId,
					"Error:",
					ex
				});
			}
			return false;
		}

		private string GetMailboxInfoKey(int index)
		{
			return string.Format("{0}{1}", "SerializedMailbox", index);
		}

		public const string SerializedMailboxAttribute = "SerializedMailbox";

		internal class ExtendedAttributeStore : Dictionary<string, string>
		{
			public ExtendedAttributeStore()
			{
			}

			public ExtendedAttributeStore(SearchSource.ExtendedAttributeStore existingStore) : base(existingStore)
			{
			}
		}
	}
}
