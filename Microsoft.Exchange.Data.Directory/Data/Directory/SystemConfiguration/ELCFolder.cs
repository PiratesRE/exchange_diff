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
	public sealed class ELCFolder : ADConfigurationObject
	{
		internal ELCFolder(IConfigurationSession session, string name)
		{
			this.m_Session = session;
			base.SetId(session, name);
		}

		public ELCFolder()
		{
		}

		internal static bool FolderTypeAllowsComments(ElcFolderType folderType)
		{
			return folderType != ElcFolderType.Calendar && folderType != ElcFolderType.Contacts && folderType != ElcFolderType.Notes && folderType != ElcFolderType.Journal && folderType != ElcFolderType.Tasks;
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ELCFolder.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ELCFolder.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return ELCFolder.parentPath;
			}
		}

		public ElcFolderType FolderType
		{
			get
			{
				return (ElcFolderType)this[ELCFolderSchema.FolderType];
			}
			set
			{
				this[ELCFolderSchema.FolderType] = value;
			}
		}

		public ElcFolderCategory Description
		{
			get
			{
				return (ElcFolderCategory)this[ELCFolderSchema.Description];
			}
		}

		[Parameter(Mandatory = false)]
		public string FolderName
		{
			get
			{
				return (string)this[ELCFolderSchema.FolderName];
			}
			set
			{
				this[ELCFolderSchema.FolderName] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> LocalizedFolderName
		{
			get
			{
				return (MultiValuedProperty<string>)this[ELCFolderSchema.LocalizedFolderName];
			}
			set
			{
				this[ELCFolderSchema.LocalizedFolderName] = value;
				this.locNameMap = null;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> StorageQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[ELCFolderSchema.StorageQuota];
			}
			set
			{
				this[ELCFolderSchema.StorageQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return (string)this[ELCFolderSchema.Comment];
			}
			set
			{
				this[ELCFolderSchema.Comment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> LocalizedComment
		{
			get
			{
				return (MultiValuedProperty<string>)this[ELCFolderSchema.LocalizedComment];
			}
			set
			{
				this[ELCFolderSchema.LocalizedComment] = value;
				this.locCommentMap = null;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MustDisplayCommentEnabled
		{
			get
			{
				return (bool)this[ELCFolderSchema.MustDisplayComment];
			}
			set
			{
				this[ELCFolderSchema.MustDisplayComment] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BaseFolderOnly
		{
			get
			{
				return (bool)this[ELCFolderSchema.BaseFolderOnly];
			}
			set
			{
				this[ELCFolderSchema.BaseFolderOnly] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> TemplateIds
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ELCFolderSchema.TemplateIds];
			}
		}

		public MultiValuedProperty<ADObjectId> RetentionPolicyTag
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ELCFolderSchema.RetentionPolicyTag];
			}
		}

		internal ADPagedReader<ElcContentSettings> GetELCContentSettings()
		{
			return base.Session.FindPaged<ElcContentSettings>(base.Id, QueryScope.SubTree, null, null, 0);
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.FolderType == ElcFolderType.ManagedCustomFolder && string.IsNullOrEmpty(this.FolderName))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorELCFolderNotSpecified, ELCFolderSchema.FolderName, this));
			}
			if (!string.IsNullOrEmpty(this.Comment) && base.IsChanged(ELCFolderSchema.Comment) && !ELCFolder.FolderTypeAllowsComments(this.FolderType))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorElcCommentNotAllowed, ELCFolderSchema.Comment, this));
			}
			if (this.MustDisplayCommentEnabled && string.IsNullOrEmpty(this.Comment))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorComment, ELCFolderSchema.Comment, this));
			}
			this.BuildMap(ref this.locNameMap, this.LocalizedFolderName, delegate
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorDuplicateLanguage, ELCFolderSchema.LocalizedFolderName, this));
			}, delegate
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorBadLocalizedFolderName, ELCFolderSchema.LocalizedFolderName, this));
			});
			this.BuildMap(ref this.locCommentMap, this.LocalizedComment, delegate
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorDuplicateLanguage, ELCFolderSchema.LocalizedComment, this));
			}, delegate
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorBadLocalizedComment, ELCFolderSchema.LocalizedComment, this));
			});
		}

		internal string GetLocalizedFolderName(IEnumerable<CultureInfo> cultureList)
		{
			return this.GetLocalizedValue(cultureList, this.FolderName, this.LocalizedFolderName, ref this.locNameMap);
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

		private void BuildMap(ref Dictionary<string, string> map, MultiValuedProperty<string> localizedStrings, ELCFolder.ErrorDelegate duplicateLangError, ELCFolder.ErrorDelegate badSyntaxError)
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
				string[] array = text.Split(ELCFolder.langSep, 2);
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

		private static readonly char[] langSep = new char[]
		{
			':'
		};

		private static ELCFolderSchema schema = ObjectSchema.GetInstance<ELCFolderSchema>();

		private static string mostDerivedClass = "msExchELCFolder";

		internal static string ElcFolderContainerName = "CN=ELC Folders Container";

		private static ADObjectId parentPath = new ADObjectId(ELCFolder.ElcFolderContainerName);

		private Dictionary<string, string> locNameMap;

		private Dictionary<string, string> locCommentMap;

		internal delegate void ErrorDelegate();
	}
}
