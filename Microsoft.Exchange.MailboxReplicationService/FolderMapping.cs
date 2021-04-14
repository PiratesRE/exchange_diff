using System;
using System.Text;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FolderMapping : FolderRecWrapper
	{
		public FolderMapping()
		{
		}

		public FolderMapping(FolderRec folderRec) : base(folderRec)
		{
		}

		public FolderMapping(string folderName, FolderType folderType, string folderClass)
		{
			base.FolderRec.FolderName = folderName;
			base.FolderRec.FolderType = folderType;
			base.FolderRec.FolderClass = folderClass;
			this.WKFType = WellKnownFolderType.None;
			this.Flags = FolderMappingFlags.None;
			this.SourceFolder = null;
			this.TargetFolder = null;
		}

		public WellKnownFolderType WKFType { get; set; }

		public FolderMappingFlags Flags { get; set; }

		public FolderMapping SourceFolder { get; set; }

		public FolderMapping TargetFolder { get; set; }

		public bool IsIncluded
		{
			get
			{
				return (this.Flags & FolderMappingFlags.Exclude) == FolderMappingFlags.None && ((this.Flags & FolderMappingFlags.Include) != FolderMappingFlags.None || ((this.Flags & FolderMappingFlags.InheritedExclude) == FolderMappingFlags.None && (this.Flags & FolderMappingFlags.InheritedInclude) != FolderMappingFlags.None));
			}
		}

		public new string FullFolderName
		{
			get
			{
				if (this.WKFType != WellKnownFolderType.None)
				{
					return string.Format("{0} [{1}]", base.FullFolderName, this.WKFType);
				}
				return base.FullFolderName;
			}
		}

		public bool IsLegacyPublicFolder
		{
			get
			{
				WellKnownFolderType wkftype = this.WKFType;
				switch (wkftype)
				{
				case WellKnownFolderType.FreeBusy:
				case WellKnownFolderType.OfflineAddressBook:
					break;
				default:
					if (wkftype != WellKnownFolderType.FreeBusyData)
					{
						switch (wkftype)
						{
						case WellKnownFolderType.SchemaRoot:
						case WellKnownFolderType.EventsRoot:
							break;
						default:
							return this.FullFolderName.StartsWith("Public Root/NON_IPM_SUBTREE/StoreEvents") || this.FullFolderName.StartsWith("Public Root/NON_IPM_SUBTREE/OWAScratchPad");
						}
					}
					break;
				}
				return true;
			}
		}

		public bool IsSystemPublicFolder
		{
			get
			{
				WellKnownFolderType wkftype = this.WKFType;
				switch (wkftype)
				{
				case WellKnownFolderType.Root:
				case WellKnownFolderType.NonIpmSubtree:
				case WellKnownFolderType.IpmSubtree:
				case WellKnownFolderType.EFormsRegistry:
					break;
				default:
					switch (wkftype)
					{
					case WellKnownFolderType.PublicFolderDumpsterRoot:
					case WellKnownFolderType.PublicFolderTombstonesRoot:
					case WellKnownFolderType.PublicFolderAsyncDeleteState:
					case WellKnownFolderType.PublicFolderInternalSubmission:
						break;
					default:
						return false;
					}
					break;
				}
				return true;
			}
		}

		public override bool AreRulesSupported()
		{
			return base.AreRulesSupported() && !FolderFilterParser.IsDumpster(this.WKFType);
		}

		public void ApplyInheritanceFlags()
		{
			this.Flags &= ~(FolderMappingFlags.InheritedInclude | FolderMappingFlags.InheritedExclude);
			if ((this.Flags & FolderMappingFlags.Inherit) == FolderMappingFlags.None)
			{
				FolderMapping folderMapping = (FolderMapping)base.Parent;
				FolderMappingFlags folderMappingFlags = (folderMapping != null) ? folderMapping.Flags : FolderMappingFlags.None;
				if ((folderMappingFlags & FolderMappingFlags.InheritedInclude) != FolderMappingFlags.None)
				{
					this.Flags |= FolderMappingFlags.InheritedInclude;
				}
				if ((folderMappingFlags & FolderMappingFlags.InheritedExclude) != FolderMappingFlags.None)
				{
					this.Flags |= FolderMappingFlags.InheritedExclude;
				}
				return;
			}
			if ((this.Flags & FolderMappingFlags.Exclude) != FolderMappingFlags.None)
			{
				this.Flags |= FolderMappingFlags.InheritedExclude;
				return;
			}
			this.Flags |= FolderMappingFlags.InheritedInclude;
		}

		public override void EnsureDataLoaded(IFolder folder, FolderRecDataFlags dataToLoad, MailboxCopierBase mbxCtx)
		{
			if (mbxCtx.IsPublicFolderMigration)
			{
				switch (this.WKFType)
				{
				case WellKnownFolderType.Root:
				case WellKnownFolderType.NonIpmSubtree:
				case WellKnownFolderType.IpmSubtree:
				case WellKnownFolderType.EFormsRegistry:
					dataToLoad &= ~(FolderRecDataFlags.SecurityDescriptors | FolderRecDataFlags.FolderAcls | FolderRecDataFlags.ExtendedAclInformation);
					break;
				default:
					if (this.IsLegacyPublicFolder)
					{
						dataToLoad &= ~(FolderRecDataFlags.SecurityDescriptors | FolderRecDataFlags.FolderAcls | FolderRecDataFlags.ExtendedAclInformation);
					}
					break;
				}
			}
			base.EnsureDataLoaded(folder, dataToLoad, mbxCtx);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.FullFolderName);
			if (this.Flags != FolderMappingFlags.None)
			{
				stringBuilder.AppendFormat(" {0}", this.Flags);
			}
			if (this.TargetFolder != null)
			{
				stringBuilder.AppendFormat(" -> {0}", this.TargetFolder.ToString());
			}
			return stringBuilder.ToString();
		}

		protected override void AfterParentChange(FolderRecWrapper previousParent)
		{
			this.ApplyInheritanceFlags();
		}

		private const string StoreEventsPath = "Public Root/NON_IPM_SUBTREE/StoreEvents";

		private const string OWAScratchpadPath = "Public Root/NON_IPM_SUBTREE/OWAScratchPad";
	}
}
