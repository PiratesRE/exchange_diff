using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderRecWrapper
	{
		public FolderRecWrapper()
		{
			this.FolderRec = new FolderRec();
		}

		public FolderRecWrapper(FolderRec folderRec)
		{
			this.FolderRec = new FolderRec(folderRec);
		}

		public FolderRecWrapper(FolderRecWrapper folderRecWrapper)
		{
			this.FolderRec = new FolderRec();
			this.CopyFrom(folderRecWrapper);
		}

		public FolderRec FolderRec { get; private set; }

		public bool IsSpoolerQueue
		{
			get
			{
				return this.FullFolderName == "/Spooler Queue" || this.FullFolderName == "Spooler Queue";
			}
		}

		public bool IsPublicFolderDumpster
		{
			get
			{
				if (this.FolderRec[PropTag.TimeInServer] != null)
				{
					ELCFolderFlags elcfolderFlags = (ELCFolderFlags)this.FolderRec[PropTag.TimeInServer];
					return elcfolderFlags.HasFlag(ELCFolderFlags.DumpsterFolder);
				}
				return false;
			}
		}

		public bool IsInternalAccess
		{
			get
			{
				object obj = this.FolderRec[PropTag.InternalAccess];
				return obj != null && obj is bool && (bool)obj;
			}
		}

		public List<FolderRecWrapper> Children { get; private set; }

		public FolderRecWrapper Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				FolderRecWrapper folderRecWrapper = this.parent;
				if (folderRecWrapper == value)
				{
					return;
				}
				if (folderRecWrapper != null && folderRecWrapper.Children != null)
				{
					folderRecWrapper.Children.Remove(this);
				}
				this.parent = value;
				if (this.parent != null)
				{
					if (this.parent.Children == null)
					{
						this.parent.Children = new List<FolderRecWrapper>(0);
					}
					this.parent.Children.Add(this);
				}
				this.AfterParentChange(folderRecWrapper);
			}
		}

		public FolderRecWrapper PublicFolderDumpster
		{
			get
			{
				return this.publicFolderDumpster;
			}
			set
			{
				this.publicFolderDumpster = value;
			}
		}

		public byte[] EntryId
		{
			get
			{
				return this.FolderRec.EntryId;
			}
		}

		public string FolderName
		{
			get
			{
				return this.FolderRec.FolderName;
			}
		}

		public FolderType FolderType
		{
			get
			{
				return this.FolderRec.FolderType;
			}
		}

		public string FolderClass
		{
			get
			{
				return this.FolderRec.FolderClass;
			}
		}

		public byte[] ParentId
		{
			get
			{
				if (this.Parent != null)
				{
					return this.Parent.EntryId;
				}
				return this.FolderRec.ParentId;
			}
		}

		public string FullFolderName
		{
			get
			{
				if (this.Parent == null)
				{
					return this.FolderName;
				}
				return this.Parent.FullFolderName + "/" + this.FolderName;
			}
		}

		public RuleData[] Rules { get; private set; }

		public RestrictionData SearchFolderRestriction { get; private set; }

		public byte[][] SearchFolderScopeIDs { get; private set; }

		public SearchState SearchFolderState { get; private set; }

		public RawSecurityDescriptor FolderNTSD { get; private set; }

		public RawSecurityDescriptor FolderFreeBusyNTSD { get; private set; }

		public PropValueData[][] FolderACL { get; private set; }

		public PropValueData[][] FolderFreeBusyACL { get; private set; }

		public FolderRecDataFlags LoadedData { get; private set; }

		public FolderRecDataFlags MappedData { get; private set; }

		public static List<FolderRecWrapper> WrapList<TFolderRec>(List<FolderRec> input) where TFolderRec : FolderRecWrapper, new()
		{
			List<FolderRecWrapper> list = new List<FolderRecWrapper>(input.Count);
			foreach (FolderRec sourceRec in input)
			{
				TFolderRec tfolderRec = Activator.CreateInstance<TFolderRec>();
				tfolderRec.FolderRec.CopyFrom(sourceRec);
				list.Add(tfolderRec);
			}
			return list;
		}

		public bool FieldIsLoaded(FolderRecDataFlags field)
		{
			return (this.LoadedData & field) != FolderRecDataFlags.None;
		}

		public bool FieldIsMapped(FolderRecDataFlags field)
		{
			return (this.MappedData & field) != FolderRecDataFlags.None;
		}

		public virtual void EnsureDataLoaded(IFolder folder, FolderRecDataFlags dataToLoad, MailboxCopierBase mbxCtx)
		{
			FolderRecDataFlags folderRecDataFlags = dataToLoad & FolderRecDataFlags.ExtendedData;
			if (folderRecDataFlags != FolderRecDataFlags.None && !this.FieldIsLoaded(folderRecDataFlags))
			{
				GetFolderRecFlags getFolderRecFlags = GetFolderRecFlags.None;
				if (this.FolderType == FolderType.Generic)
				{
					if ((dataToLoad & FolderRecDataFlags.PromotedProperties) != FolderRecDataFlags.None && !this.FieldIsLoaded(FolderRecDataFlags.PromotedProperties))
					{
						getFolderRecFlags |= GetFolderRecFlags.PromotedProperties;
					}
					if ((dataToLoad & FolderRecDataFlags.Restrictions) != FolderRecDataFlags.None && !this.FieldIsLoaded(FolderRecDataFlags.Restrictions))
					{
						getFolderRecFlags |= GetFolderRecFlags.Restrictions;
					}
				}
				if ((this.FolderType == FolderType.Generic || this.FolderType == FolderType.Search) && (dataToLoad & FolderRecDataFlags.Views) != FolderRecDataFlags.None && !this.FieldIsLoaded(FolderRecDataFlags.Views))
				{
					getFolderRecFlags |= GetFolderRecFlags.Views;
				}
				if (getFolderRecFlags != GetFolderRecFlags.None)
				{
					FolderRec folderRec = folder.GetFolderRec(null, getFolderRecFlags);
					this.FolderRec.SetPromotedProperties(folderRec.GetPromotedProperties());
					this.FolderRec.Views = folderRec.Views;
					this.FolderRec.ICSViews = folderRec.ICSViews;
					this.FolderRec.Restrictions = folderRec.Restrictions;
				}
				this.LoadedData |= folderRecDataFlags;
			}
			if ((dataToLoad & FolderRecDataFlags.SearchCriteria) != FolderRecDataFlags.None && !this.IsSpoolerQueue && !this.FieldIsLoaded(FolderRecDataFlags.SearchCriteria))
			{
				this.ReadSearchCriteria(folder, new Action<List<BadMessageRec>>(mbxCtx.ReportBadItems), new Func<byte[], IFolder>(mbxCtx.SourceMailboxWrapper.GetFolder));
				this.LoadedData |= FolderRecDataFlags.SearchCriteria;
			}
			if ((dataToLoad & FolderRecDataFlags.Rules) != FolderRecDataFlags.None && !this.FieldIsLoaded(FolderRecDataFlags.Rules))
			{
				this.ReadRules(folder, FolderRecWrapper.extraTags, new Action<List<BadMessageRec>>(mbxCtx.ReportBadItems), new Func<byte[], IFolder>(mbxCtx.SourceMailboxWrapper.GetFolder));
				this.LoadedData |= FolderRecDataFlags.Rules;
				MrsTracer.Service.Debug("Rules loaded: {0}", new object[]
				{
					new RulesDataContext(this.Rules)
				});
			}
			if ((dataToLoad & FolderRecDataFlags.SecurityDescriptors) != FolderRecDataFlags.None && !this.FieldIsLoaded(FolderRecDataFlags.SecurityDescriptors))
			{
				this.FolderNTSD = this.ReadSD(folder, SecurityProp.NTSD, new Action<List<BadMessageRec>>(mbxCtx.ReportBadItems), new Func<byte[], IFolder>(mbxCtx.SourceMailboxWrapper.GetFolder));
				this.FolderFreeBusyNTSD = this.ReadSD(folder, SecurityProp.FreeBusyNTSD, new Action<List<BadMessageRec>>(mbxCtx.ReportBadItems), new Func<byte[], IFolder>(mbxCtx.SourceMailboxWrapper.GetFolder));
				this.LoadedData |= FolderRecDataFlags.SecurityDescriptors;
				MrsTracer.Service.Debug("FolderSDs loaded: NTSD {0}, FreeBusyNTSD {1}", new object[]
				{
					CommonUtils.GetSDDLString(this.FolderNTSD),
					CommonUtils.GetSDDLString(this.FolderFreeBusyNTSD)
				});
			}
			if (dataToLoad.HasFlag(FolderRecDataFlags.FolderAcls) && !this.FieldIsLoaded(FolderRecDataFlags.FolderAcls))
			{
				this.FolderACL = this.ReadACL(folder, SecurityProp.NTSD, new Action<List<BadMessageRec>>(mbxCtx.ReportBadItems), new Func<byte[], IFolder>(mbxCtx.SourceMailboxWrapper.GetFolder));
				this.FolderFreeBusyACL = this.ReadACL(folder, SecurityProp.FreeBusyNTSD, new Action<List<BadMessageRec>>(mbxCtx.ReportBadItems), new Func<byte[], IFolder>(mbxCtx.SourceMailboxWrapper.GetFolder));
				this.LoadedData |= FolderRecDataFlags.FolderAcls;
				MrsTracer.Service.Debug("FolderAcls are loaded: ACL {0}, FreeBusyACL {1}", new object[]
				{
					new PropValuesDataContext(this.FolderACL).ToString(),
					new PropValuesDataContext(this.FolderFreeBusyACL).ToString()
				});
			}
			if (dataToLoad.HasFlag(FolderRecDataFlags.ExtendedAclInformation) && !this.FieldIsLoaded(FolderRecDataFlags.ExtendedAclInformation))
			{
				this.FolderACL = this.ReadExtendedAcl(folder, AclFlags.FolderAcl, new Action<List<BadMessageRec>>(mbxCtx.ReportBadItems), new Func<byte[], IFolder>(mbxCtx.SourceMailboxWrapper.GetFolder));
				this.FolderFreeBusyACL = this.ReadExtendedAcl(folder, AclFlags.FreeBusyAcl, new Action<List<BadMessageRec>>(mbxCtx.ReportBadItems), new Func<byte[], IFolder>(mbxCtx.SourceMailboxWrapper.GetFolder));
				this.LoadedData |= FolderRecDataFlags.ExtendedAclInformation;
				MrsTracer.Service.Debug("FolderExtendedAcls are loaded: Acl {0}, FreeBusyAcl {1}", new object[]
				{
					new PropValuesDataContext(this.FolderACL).ToString(),
					new PropValuesDataContext(this.FolderFreeBusyACL).ToString()
				});
			}
		}

		public void EnumerateMappableData(MailboxCopierBase mbxCopier)
		{
			if (this.FieldIsLoaded(FolderRecDataFlags.PromotedProperties) && !this.FieldIsMapped(FolderRecDataFlags.PromotedProperties))
			{
				mbxCopier.NamedPropTranslator.EnumeratePropTags(this.FolderRec.GetPromotedProperties());
			}
			if (this.FieldIsLoaded(FolderRecDataFlags.Views) && !this.FieldIsMapped(FolderRecDataFlags.Views) && this.FolderRec.Views != null)
			{
				foreach (SortOrderData sortOrder in this.FolderRec.Views)
				{
					mbxCopier.NamedPropTranslator.EnumerateSortOrder(sortOrder);
				}
			}
			if (this.FieldIsLoaded(FolderRecDataFlags.Restrictions) && !this.FieldIsMapped(FolderRecDataFlags.Restrictions) && this.FolderRec.Restrictions != null)
			{
				foreach (RestrictionData rest in this.FolderRec.Restrictions)
				{
					mbxCopier.NamedPropTranslator.EnumerateRestriction(this.FolderRec, BadItemKind.CorruptFolderRestriction, rest);
				}
			}
			if (this.FieldIsLoaded(FolderRecDataFlags.SearchCriteria) && !this.FieldIsMapped(FolderRecDataFlags.SearchCriteria) && this.FolderType == FolderType.Search)
			{
				mbxCopier.NamedPropTranslator.EnumerateRestriction(this.FolderRec, BadItemKind.CorruptSearchFolderCriteria, this.SearchFolderRestriction);
			}
			if (mbxCopier.PrincipalTranslator != null && this.FieldIsLoaded(FolderRecDataFlags.SecurityDescriptors) && !this.FieldIsMapped(FolderRecDataFlags.SecurityDescriptors))
			{
				mbxCopier.PrincipalTranslator.EnumerateSecurityDescriptor(this.FolderNTSD);
				mbxCopier.PrincipalTranslator.EnumerateSecurityDescriptor(this.FolderFreeBusyNTSD);
			}
			if (this.FieldIsLoaded(FolderRecDataFlags.Rules) && !this.FieldIsMapped(FolderRecDataFlags.Rules))
			{
				mbxCopier.NamedPropTranslator.EnumerateRules(this.Rules);
				if (mbxCopier.PrincipalTranslator != null)
				{
					mbxCopier.PrincipalTranslator.EnumerateRules(this.Rules);
				}
			}
			if (mbxCopier.PrincipalTranslator != null && ((this.FieldIsLoaded(FolderRecDataFlags.FolderAcls) && !this.FieldIsMapped(FolderRecDataFlags.FolderAcls)) || (this.FieldIsLoaded(FolderRecDataFlags.ExtendedAclInformation) && !this.FieldIsMapped(FolderRecDataFlags.ExtendedAclInformation))))
			{
				mbxCopier.PrincipalTranslator.EnumerateFolderACL(this.FolderACL);
				mbxCopier.PrincipalTranslator.EnumerateFolderACL(this.FolderFreeBusyACL);
			}
		}

		public void TranslateMappableData(MailboxCopierBase mbxCopier)
		{
			if (this.FieldIsLoaded(FolderRecDataFlags.PromotedProperties) && !this.FieldIsMapped(FolderRecDataFlags.PromotedProperties))
			{
				PropTag[] promotedProperties = this.FolderRec.GetPromotedProperties();
				mbxCopier.NamedPropTranslator.TranslatePropTags(promotedProperties);
				this.FolderRec.SetPromotedProperties(promotedProperties);
				this.MappedData |= FolderRecDataFlags.PromotedProperties;
			}
			if (this.FieldIsLoaded(FolderRecDataFlags.Views) && !this.FieldIsMapped(FolderRecDataFlags.Views) && this.FolderRec.Views != null)
			{
				foreach (SortOrderData so in this.FolderRec.Views)
				{
					mbxCopier.NamedPropTranslator.TranslateSortOrder(so);
				}
				this.MappedData |= FolderRecDataFlags.Views;
			}
			if (this.FieldIsLoaded(FolderRecDataFlags.Restrictions) && !this.FieldIsMapped(FolderRecDataFlags.Restrictions) && this.FolderRec.Restrictions != null)
			{
				foreach (RestrictionData restrictionData in this.FolderRec.Restrictions)
				{
					mbxCopier.NamedPropTranslator.TranslateRestriction(restrictionData);
					if (mbxCopier.FolderIdTranslator != null)
					{
						mbxCopier.FolderIdTranslator.TranslateRestriction(restrictionData);
					}
				}
				this.MappedData |= FolderRecDataFlags.Restrictions;
			}
			if (this.FieldIsLoaded(FolderRecDataFlags.SearchCriteria) && !this.FieldIsMapped(FolderRecDataFlags.SearchCriteria))
			{
				if (this.FolderType == FolderType.Search)
				{
					mbxCopier.NamedPropTranslator.TranslateRestriction(this.SearchFolderRestriction);
					if (mbxCopier.FolderIdTranslator != null)
					{
						mbxCopier.FolderIdTranslator.TranslateRestriction(this.SearchFolderRestriction);
						this.SearchFolderScopeIDs = mbxCopier.FolderIdTranslator.TranslateFolderIds(this.SearchFolderScopeIDs);
					}
				}
				this.MappedData |= FolderRecDataFlags.SearchCriteria;
			}
			if (mbxCopier.PrincipalTranslator != null && this.FieldIsLoaded(FolderRecDataFlags.SecurityDescriptors) && !this.FieldIsMapped(FolderRecDataFlags.SecurityDescriptors))
			{
				mbxCopier.PrincipalTranslator.TranslateSecurityDescriptor(this.FolderNTSD, TranslateSecurityDescriptorFlags.None);
				mbxCopier.PrincipalTranslator.TranslateSecurityDescriptor(this.FolderFreeBusyNTSD, TranslateSecurityDescriptorFlags.None);
				this.MappedData |= FolderRecDataFlags.SecurityDescriptors;
			}
			if (this.Rules != null && this.FieldIsLoaded(FolderRecDataFlags.Rules) && !this.FieldIsMapped(FolderRecDataFlags.Rules))
			{
				mbxCopier.NamedPropTranslator.TranslateRules(this.Rules);
				if (!mbxCopier.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.InMailboxExternalRules) && mbxCopier.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.InMailboxExternalRules))
				{
					this.PatchRulesForDownlevelSources(this.Rules, mbxCopier.GetSourceFolderMap(GetFolderMapFlags.None));
				}
				if (mbxCopier.SourceMailbox.IsCapabilitySupported(MRSProxyCapabilities.InMailboxExternalRules) && !mbxCopier.DestMailbox.IsCapabilitySupported(MRSProxyCapabilities.InMailboxExternalRules))
				{
					this.PatchRulesForDownlevelDestinations(this.Rules);
				}
				if (mbxCopier.FolderIdTranslator != null)
				{
					mbxCopier.FolderIdTranslator.TranslateRules(this.Rules);
				}
				if (mbxCopier.PrincipalTranslator != null)
				{
					mbxCopier.PrincipalTranslator.TranslateRules(this.Rules);
				}
				this.MappedData |= FolderRecDataFlags.Rules;
			}
			if (mbxCopier.PrincipalTranslator != null && ((this.FieldIsLoaded(FolderRecDataFlags.FolderAcls) && !this.FieldIsMapped(FolderRecDataFlags.FolderAcls)) || (this.FieldIsLoaded(FolderRecDataFlags.ExtendedAclInformation) && !this.FieldIsMapped(FolderRecDataFlags.ExtendedAclInformation))))
			{
				mbxCopier.PrincipalTranslator.TranslateFolderACL(this.FolderACL);
				mbxCopier.PrincipalTranslator.TranslateFolderACL(this.FolderFreeBusyACL);
				this.MappedData |= (this.FieldIsLoaded(FolderRecDataFlags.ExtendedAclInformation) ? FolderRecDataFlags.ExtendedAclInformation : FolderRecDataFlags.FolderAcls);
			}
		}

		public virtual void CopyFrom(FolderRecWrapper sourceRec)
		{
			this.FolderRec.CopyFrom(sourceRec.FolderRec);
			this.Rules = sourceRec.Rules;
			this.FolderNTSD = sourceRec.FolderNTSD;
			this.FolderFreeBusyNTSD = sourceRec.FolderFreeBusyNTSD;
			this.FolderACL = sourceRec.FolderACL;
			this.FolderFreeBusyACL = sourceRec.FolderFreeBusyACL;
			this.SearchFolderRestriction = sourceRec.SearchFolderRestriction;
			this.SearchFolderScopeIDs = sourceRec.SearchFolderScopeIDs;
			this.SearchFolderState = sourceRec.SearchFolderState;
			this.LoadedData = sourceRec.LoadedData;
			this.MappedData = sourceRec.MappedData;
		}

		public Guid GetContentMailboxGuid(Func<string, Guid> getContentMailboxGuidDelegate)
		{
			if (this.contentMailboxGuid == Guid.Empty)
			{
				this.contentMailboxGuid = getContentMailboxGuidDelegate(this.FullFolderName);
			}
			return this.contentMailboxGuid;
		}

		public bool IsTargetMailbox(Guid mailboxGuid, Func<string, HashSet<Guid>> getTargetMailboxesDelegate)
		{
			if (this.pfTargetMailboxes == null)
			{
				this.pfTargetMailboxes = (getTargetMailboxesDelegate(this.FullFolderName) ?? FolderRecWrapper.EmptyHashSet);
			}
			return this.pfTargetMailboxes.Contains(mailboxGuid);
		}

		public virtual bool AreRulesSupported()
		{
			return this.FolderType != FolderType.Search;
		}

		public FolderRecWrapper FindChildByName(string subfolderName, CultureInfo cultureInfo)
		{
			if (this.Children != null)
			{
				foreach (FolderRecWrapper folderRecWrapper in this.Children)
				{
					if (cultureInfo != null)
					{
						if (string.Compare(subfolderName, folderRecWrapper.FolderName, cultureInfo, CompareOptions.IgnoreCase) == 0)
						{
							return folderRecWrapper;
						}
					}
					else if (string.Compare(subfolderName, folderRecWrapper.FolderName, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return folderRecWrapper;
					}
				}
			}
			return null;
		}

		public bool IsChildOf(FolderRecWrapper anotherFolder)
		{
			for (FolderRecWrapper folderRecWrapper = this; folderRecWrapper != null; folderRecWrapper = folderRecWrapper.Parent)
			{
				if (folderRecWrapper == anotherFolder)
				{
					return true;
				}
			}
			return false;
		}

		public void WriteRules(IDestinationFolder targetFolder, Action<List<BadMessageRec>> reportBadItemsDelegate)
		{
			if (this.FolderType == FolderType.Search)
			{
				return;
			}
			CommonUtils.ProcessKnownExceptions(delegate
			{
				targetFolder.SetRules(this.Rules);
			}, delegate(Exception ex)
			{
				if (reportBadItemsDelegate != null && CommonUtils.ExceptionIsAny(ex, new WellKnownException[]
				{
					WellKnownException.DataProviderPermanent,
					WellKnownException.MapiNotEnoughMemory
				}))
				{
					List<BadMessageRec> list = new List<BadMessageRec>(1);
					list.Add(BadMessageRec.Folder(this.FolderRec, BadItemKind.CorruptFolderRule, ex));
					reportBadItemsDelegate(list);
					return true;
				}
				return false;
			});
		}

		public override string ToString()
		{
			return this.FullFolderName;
		}

		protected virtual void AfterParentChange(FolderRecWrapper previousParent)
		{
		}

		private T ReadFolderData<T>(Func<T> getDataDelegate, Action<List<BadMessageRec>> reportBadItemsDelegate, Func<byte[], IFolder> openFolderDelegate) where T : class
		{
			T result = default(T);
			CommonUtils.ProcessKnownExceptions(delegate
			{
				CommonUtils.TreatMissingFolderAsTransient(delegate
				{
					result = getDataDelegate();
				}, this.EntryId, openFolderDelegate);
			}, delegate(Exception ex)
			{
				if (reportBadItemsDelegate != null && CommonUtils.ExceptionIsAny(ex, new WellKnownException[]
				{
					WellKnownException.DataProviderPermanent,
					WellKnownException.CorruptData,
					WellKnownException.NonCanonicalACL
				}))
				{
					List<BadMessageRec> list = new List<BadMessageRec>(1);
					list.Add(BadMessageRec.Folder(this.FolderRec, BadItemKind.CorruptFolderACL, ex));
					reportBadItemsDelegate(list);
					return true;
				}
				return false;
			});
			return result;
		}

		private void ReadSearchCriteria(IFolder folder, Action<List<BadMessageRec>> reportBadItemsDelegate, Func<byte[], IFolder> openFolderDelegate)
		{
			if (this.FolderType != FolderType.Search)
			{
				return;
			}
			this.SearchFolderRestriction = this.ReadFolderData<RestrictionData>(delegate
			{
				RestrictionData restrictionData;
				byte[][] array;
				SearchState searchState;
				folder.GetSearchCriteria(out restrictionData, out array, out searchState);
				this.SearchFolderScopeIDs = array;
				this.SearchFolderState = searchState;
				MrsTracer.Service.Debug("Search criteria loaded: {0}, scopes: {1}, state {2}", new object[]
				{
					restrictionData,
					new EntryIDsDataContext(array),
					searchState
				});
				return restrictionData;
			}, reportBadItemsDelegate, openFolderDelegate);
		}

		private void ReadRules(IFolder folder, PropTag[] extraPtags, Action<List<BadMessageRec>> reportBadItemsDelegate, Func<byte[], IFolder> openFolderDelegate)
		{
			this.Rules = Array<RuleData>.Empty;
			if (this.FolderType == FolderType.Search)
			{
				return;
			}
			this.Rules = this.ReadFolderData<RuleData[]>(() => folder.GetRules(extraPtags), reportBadItemsDelegate, openFolderDelegate);
		}

		private RawSecurityDescriptor ReadSD(IFolder folder, SecurityProp secProp, Action<List<BadMessageRec>> reportBadItemsDelegate, Func<byte[], IFolder> openFolderDelegate)
		{
			return this.ReadFolderData<RawSecurityDescriptor>(() => folder.GetSecurityDescriptor(secProp), reportBadItemsDelegate, openFolderDelegate);
		}

		private PropValueData[][] ReadACL(IFolder folder, SecurityProp secProp, Action<List<BadMessageRec>> reportBadItemsDelegate, Func<byte[], IFolder> openFolderDelegate)
		{
			return this.ReadFolderData<PropValueData[][]>(() => folder.GetACL(secProp), reportBadItemsDelegate, openFolderDelegate);
		}

		private PropValueData[][] ReadExtendedAcl(IFolder folder, AclFlags flags, Action<List<BadMessageRec>> reportBadItemsDelegate, Func<byte[], IFolder> openFolderDelegate)
		{
			return this.ReadFolderData<PropValueData[][]>(() => folder.GetExtendedAcl(flags), reportBadItemsDelegate, openFolderDelegate);
		}

		private void PatchRulesForDownlevelSources(RuleData[] rules, FolderMap sourceFolderMap)
		{
			RuleData.ConvertRulesToUplevel(rules, (byte[] entryID) => sourceFolderMap[entryID] != null);
		}

		private void PatchRulesForDownlevelDestinations(RuleData[] rules)
		{
			RuleData.ConvertRulesToDownlevel(rules);
		}

		private static PropTag[] extraTags = new PropTag[]
		{
			PropTag.ReportTime,
			(PropTag)1627389955U,
			(PropTag)1627455491U
		};

		public static readonly HashSet<Guid> EmptyHashSet = new HashSet<Guid>();

		private FolderRecWrapper parent;

		private FolderRecWrapper publicFolderDumpster;

		private Guid contentMailboxGuid;

		private HashSet<Guid> pfTargetMailboxes;
	}
}
