using System;
using System.Collections.Generic;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class NamedPropTranslator
	{
		public NamedPropTranslator(Action<List<BadMessageRec>> reportBadItemsDelegate, NamedPropMapper sourceMapper, NamedPropMapper targetMapper)
		{
			this.reportBadItemsDelegate = reportBadItemsDelegate;
			this.sourceMapper = sourceMapper;
			this.targetMapper = targetMapper;
			this.hasUnresolvedMappings = true;
		}

		public void EnumerateRestriction(FolderRec folderRec, BadItemKind badItemKind, RestrictionData rest)
		{
			if (rest != null)
			{
				CommonUtils.ProcessKnownExceptions(delegate
				{
					rest.EnumeratePropTags(new CommonUtils.EnumPropTagDelegate(this.EnumeratePtag));
				}, delegate(Exception ex)
				{
					if (this.reportBadItemsDelegate != null && CommonUtils.ExceptionIsAny(ex, new WellKnownException[]
					{
						WellKnownException.DataProviderPermanent,
						WellKnownException.CorruptData
					}))
					{
						List<BadMessageRec> list = new List<BadMessageRec>(1);
						list.Add(BadMessageRec.Folder(folderRec, badItemKind, ex));
						this.reportBadItemsDelegate(list);
						return true;
					}
					return false;
				});
				this.hasUnresolvedMappings = true;
			}
		}

		public void EnumerateSortOrder(SortOrderData sortOrder)
		{
			if (sortOrder != null)
			{
				sortOrder.Enumerate(delegate(SortOrderMember som)
				{
					int propTag = som.PropTag;
					this.EnumeratePtag(ref propTag);
				});
				this.hasUnresolvedMappings = true;
			}
		}

		public void EnumerateRules(RuleData[] rules)
		{
			if (rules != null)
			{
				foreach (RuleData ruleData in rules)
				{
					ruleData.Enumerate(new CommonUtils.EnumPropTagDelegate(this.EnumeratePtag), null, null);
				}
				this.hasUnresolvedMappings = true;
			}
		}

		public void EnumeratePropTags(PropTag[] ptags)
		{
			if (ptags != null)
			{
				foreach (int num in ptags)
				{
					this.EnumeratePtag(ref num);
				}
				this.hasUnresolvedMappings = true;
			}
		}

		public void TranslateRestriction(RestrictionData rest)
		{
			if (rest != null)
			{
				this.ResolveMappingsIfNeeded();
				rest.EnumeratePropTags(new CommonUtils.EnumPropTagDelegate(this.TranslatePtag));
			}
		}

		public void TranslateSortOrder(SortOrderData so)
		{
			if (so != null)
			{
				this.ResolveMappingsIfNeeded();
				so.Enumerate(delegate(SortOrderMember som)
				{
					int propTag = som.PropTag;
					this.TranslatePtag(ref propTag);
					som.PropTag = propTag;
				});
			}
		}

		public void TranslateRules(RuleData[] rules)
		{
			if (rules != null)
			{
				this.ResolveMappingsIfNeeded();
				foreach (RuleData ruleData in rules)
				{
					ruleData.Enumerate(new CommonUtils.EnumPropTagDelegate(this.TranslatePtag), null, null);
				}
			}
		}

		public void TranslatePropTags(PropTag[] ptags)
		{
			if (ptags != null)
			{
				this.ResolveMappingsIfNeeded();
				for (int i = 0; i < ptags.Length; i++)
				{
					int num = (int)ptags[i];
					this.TranslatePtag(ref num);
					ptags[i] = (PropTag)num;
				}
			}
		}

		public void Clear()
		{
			this.sourceMapper.Clear();
			this.targetMapper.Clear();
		}

		private void ResolveMappingsIfNeeded()
		{
			if (!this.hasUnresolvedMappings)
			{
				return;
			}
			ICollection<NamedPropData> resolvedKeys = this.sourceMapper.ByNamedProp.ResolvedKeys;
			this.targetMapper.ByNamedProp.AddKeys(resolvedKeys);
			this.targetMapper.LookupUnresolvedKeys();
			this.hasUnresolvedMappings = false;
		}

		private void EnumeratePtag(ref int ptag)
		{
			PropTag propTag = (PropTag)ptag;
			if (!propTag.IsNamedProperty())
			{
				return;
			}
			this.sourceMapper.ById.AddKey(propTag.Id());
		}

		private void TranslatePtag(ref int ptag)
		{
			PropTag propTag = (PropTag)ptag;
			if (!propTag.IsNamedProperty())
			{
				return;
			}
			NamedPropMapper.Mapping mapping = this.sourceMapper.ById[propTag.Id()];
			if (mapping == null)
			{
				MrsTracer.Service.Warning("Proptag 0x{0:X} could not be mapped to a namedprop in the source mailbox, not translating.", new object[]
				{
					propTag
				});
				return;
			}
			NamedPropMapper.Mapping mapping2 = this.targetMapper.ByNamedProp[mapping.NPData];
			if (mapping2 != null)
			{
				ptag = (int)PropTagHelper.PropTagFromIdAndType(mapping2.PropId, propTag.ValueType());
				return;
			}
			MrsTracer.Service.Warning("NamedProp {0} (source ptag 0x{1:X}) could not be mapped in the target mailbox, not translating.", new object[]
			{
				mapping.NPData,
				propTag
			});
		}

		private NamedPropMapper sourceMapper;

		private NamedPropMapper targetMapper;

		private bool hasUnresolvedMappings;

		private Action<List<BadMessageRec>> reportBadItemsDelegate;
	}
}
