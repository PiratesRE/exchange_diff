using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class RetentionPolicyTag : ADConfigurationObject
	{
		internal RetentionPolicyTag(IConfigurationSession session, string name)
		{
			this.m_Session = session;
			base.SetId(session, name);
		}

		public RetentionPolicyTag()
		{
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return RetentionPolicyTag.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RetentionPolicyTag.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return RetentionPolicyTag.parentPath;
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		public ElcFolderType Type
		{
			get
			{
				return (ElcFolderType)this[RetentionPolicyTagSchema.Type];
			}
			set
			{
				this[RetentionPolicyTagSchema.Type] = value;
			}
		}

		public bool IsDefaultAutoGroupPolicyTag
		{
			get
			{
				return (bool)this[RetentionPolicyTagSchema.IsDefaultAutoGroupPolicyTag];
			}
			set
			{
				this[RetentionPolicyTagSchema.IsDefaultAutoGroupPolicyTag] = value;
			}
		}

		public bool IsDefaultModeratedRecipientsPolicyTag
		{
			get
			{
				return (bool)this[RetentionPolicyTagSchema.IsDefaultModeratedRecipientsPolicyTag];
			}
			set
			{
				this[RetentionPolicyTagSchema.IsDefaultModeratedRecipientsPolicyTag] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SystemTag
		{
			get
			{
				return ((int)this[RetentionPolicyTagSchema.PolicyTagFlags] & 1) != 0;
			}
			set
			{
				ElcTagType elcTagType = (ElcTagType)((int)this[RetentionPolicyTagSchema.PolicyTagFlags]);
				if (value)
				{
					elcTagType |= ElcTagType.SystemTag;
				}
				else
				{
					elcTagType &= ~ElcTagType.SystemTag;
				}
				this[RetentionPolicyTagSchema.PolicyTagFlags] = (int)elcTagType;
			}
		}

		internal bool IsPrimary
		{
			get
			{
				return ((int)this[RetentionPolicyTagSchema.PolicyTagFlags] & 4) != 0;
			}
			set
			{
				ElcTagType elcTagType = (ElcTagType)((int)this[RetentionPolicyTagSchema.PolicyTagFlags]);
				if (value)
				{
					elcTagType |= ElcTagType.PrimaryDefault;
				}
				else
				{
					elcTagType &= ~ElcTagType.PrimaryDefault;
				}
				this[RetentionPolicyTagSchema.PolicyTagFlags] = (int)elcTagType;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> LocalizedRetentionPolicyTagName
		{
			get
			{
				return (MultiValuedProperty<string>)this[RetentionPolicyTagSchema.LocalizedRetentionPolicyTagName];
			}
			set
			{
				this[RetentionPolicyTagSchema.LocalizedRetentionPolicyTagName] = value;
				this.locNameMap = null;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return (string)this[RetentionPolicyTagSchema.Comment];
			}
			set
			{
				this[RetentionPolicyTagSchema.Comment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid RetentionId
		{
			get
			{
				if ((Guid)this[RetentionPolicyTagSchema.RetentionId] == Guid.Empty)
				{
					return base.Guid;
				}
				return (Guid)this[RetentionPolicyTagSchema.RetentionId];
			}
			set
			{
				this[RetentionPolicyTagSchema.RetentionId] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> LocalizedComment
		{
			get
			{
				return (MultiValuedProperty<string>)this[RetentionPolicyTagSchema.LocalizedComment];
			}
			set
			{
				this[RetentionPolicyTagSchema.LocalizedComment] = value;
				this.locCommentMap = null;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MustDisplayCommentEnabled
		{
			get
			{
				return ((ElcTagType)this[RetentionPolicyTagSchema.PolicyTagFlags] & ElcTagType.MustDisplayComment) != ElcTagType.None;
			}
			set
			{
				ElcTagType elcTagType = (ElcTagType)this[RetentionPolicyTagSchema.PolicyTagFlags];
				if (value)
				{
					elcTagType |= ElcTagType.MustDisplayComment;
				}
				else
				{
					elcTagType &= ~ElcTagType.MustDisplayComment;
				}
				if (elcTagType != (ElcTagType)this[RetentionPolicyTagSchema.PolicyTagFlags])
				{
					this[RetentionPolicyTagSchema.PolicyTagFlags] = (int)elcTagType;
				}
			}
		}

		internal static bool FolderTypeAllowsComments(ElcFolderType folderType)
		{
			return folderType != ElcFolderType.Calendar && folderType != ElcFolderType.Contacts && folderType != ElcFolderType.Notes && folderType != ElcFolderType.Journal && folderType != ElcFolderType.Tasks;
		}

		internal ADPagedReader<ElcContentSettings> GetELCContentSettings()
		{
			return base.Session.FindPaged<ElcContentSettings>((ADObjectId)this.Identity, QueryScope.SubTree, null, null, 0);
		}

		public ADObjectId LegacyManagedFolder
		{
			get
			{
				return (ADObjectId)this[RetentionPolicyTagSchema.LegacyManagedFolder];
			}
			set
			{
				this[RetentionPolicyTagSchema.LegacyManagedFolder] = value;
			}
		}

		internal EnhancedTimeSpan? TimeSpanForRetention
		{
			get
			{
				return this.GetTimeSpanForRetention();
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			this.BuildMap(ref this.locNameMap, this.LocalizedRetentionPolicyTagName, delegate
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorDuplicateLanguage, RetentionPolicyTagSchema.LocalizedRetentionPolicyTagName, this));
			}, delegate
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorBadLocalizedFolderName, RetentionPolicyTagSchema.LocalizedRetentionPolicyTagName, this));
			});
			this.BuildMap(ref this.locCommentMap, this.LocalizedComment, delegate
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorDuplicateLanguage, RetentionPolicyTagSchema.LocalizedComment, this));
			}, delegate
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorBadLocalizedComment, RetentionPolicyTagSchema.LocalizedComment, this));
			});
		}

		internal string GetLocalizedFolderName(IEnumerable<CultureInfo> cultureList)
		{
			return this.GetLocalizedValue(cultureList, base.Name, this.LocalizedRetentionPolicyTagName, ref this.locNameMap);
		}

		internal string GetLocalizedFolderComment(IEnumerable<CultureInfo> cultureList)
		{
			return this.GetLocalizedValue(cultureList, this.Comment, this.LocalizedComment, ref this.locCommentMap);
		}

		private string GetLocalizedValue(IEnumerable<CultureInfo> cultureList, string defaultString, MultiValuedProperty<string> localizedStrings, ref Dictionary<string, string> locMap)
		{
			if (localizedStrings == null || localizedStrings.Count == 0 || cultureList == null)
			{
				return defaultString;
			}
			if (locMap == null || localizedStrings.Changed)
			{
				this.BuildMap(ref locMap, localizedStrings, null, null);
			}
			using (IEnumerator<CultureInfo> enumerator = cultureList.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					CultureInfo cultureInfo = enumerator.Current;
					CultureInfo cultureInfo2 = null;
					string result = null;
					if (cultureInfo != null)
					{
						if (locMap.TryGetValue(cultureInfo.TwoLetterISOLanguageName, out result))
						{
							return result;
						}
						if (locMap.TryGetValue(cultureInfo.EnglishName, out result))
						{
							return result;
						}
						cultureInfo2 = cultureInfo.Parent;
					}
					if (cultureInfo2 != null)
					{
						if (locMap.TryGetValue(cultureInfo2.TwoLetterISOLanguageName, out result))
						{
							return result;
						}
						if (locMap.TryGetValue(cultureInfo2.EnglishName, out result))
						{
							return result;
						}
					}
				}
			}
			return defaultString;
		}

		private void BuildMap(ref Dictionary<string, string> map, MultiValuedProperty<string> localizedStrings, RetentionPolicyTag.ErrorDelegate duplicateLangError, RetentionPolicyTag.ErrorDelegate badSyntaxError)
		{
			if (map == null)
			{
				map = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
			}
			else
			{
				map.Clear();
			}
			if (localizedStrings == null || localizedStrings.Count == 0)
			{
				return;
			}
			foreach (string text in localizedStrings)
			{
				string[] array = text.Split(RetentionPolicyTag.langSep, 2);
				if (array == null || array.Length != 2)
				{
					if (badSyntaxError != null)
					{
						badSyntaxError();
					}
				}
				else if (map.ContainsKey(array[0]))
				{
					if (duplicateLangError != null)
					{
						duplicateLangError();
					}
				}
				else
				{
					map.Add(array[0], array[1]);
				}
			}
		}

		internal EnhancedTimeSpan? GetTimeSpanForRetention()
		{
			ADPagedReader<ElcContentSettings> elccontentSettings = this.GetELCContentSettings();
			EnhancedTimeSpan? result = null;
			if (elccontentSettings != null)
			{
				foreach (ElcContentSettings elcContentSettings in elccontentSettings)
				{
					if (elcContentSettings.RetentionEnabled && elcContentSettings.AgeLimitForRetention != null && (elcContentSettings.RetentionAction == RetentionActionType.DeleteAndAllowRecovery || elcContentSettings.RetentionAction == RetentionActionType.PermanentlyDelete || elcContentSettings.RetentionAction == RetentionActionType.MoveToDeletedItems))
					{
						if (result != null)
						{
							return null;
						}
						result = elcContentSettings.AgeLimitForRetention;
					}
				}
			}
			if (result == null)
			{
				result = new EnhancedTimeSpan?(TimeSpan.MaxValue);
			}
			return result;
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return RetentionPolicy.RetentionPolicyVersion;
			}
		}

		internal override void Initialize()
		{
			if (base.ExchangeVersion == RetentionPolicy.E14RetentionPolicyMajorVersion)
			{
				this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, RetentionPolicy.E14RetentionPolicyFullVersion);
			}
		}

		internal override QueryFilter VersioningFilter
		{
			get
			{
				ExchangeObjectVersion e14RetentionPolicyMajorVersion = RetentionPolicy.E14RetentionPolicyMajorVersion;
				ExchangeObjectVersion nextMajorVersion = e14RetentionPolicyMajorVersion.NextMajorVersion;
				return new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.ExchangeVersion, e14RetentionPolicyMajorVersion),
					new ComparisonFilter(ComparisonOperator.LessThan, ADObjectSchema.ExchangeVersion, nextMajorVersion)
				});
			}
		}

		private static readonly char[] langSep = new char[]
		{
			':'
		};

		private static RetentionPolicyTagSchema schema = ObjectSchema.GetInstance<RetentionPolicyTagSchema>();

		private static string mostDerivedClass = "msExchELCFolder";

		private static ADObjectId parentPath = new ADObjectId("CN=Retention Policy Tag Container");

		private Dictionary<string, string> locNameMap;

		private Dictionary<string, string> locCommentMap;

		internal delegate void ErrorDelegate();
	}
}
