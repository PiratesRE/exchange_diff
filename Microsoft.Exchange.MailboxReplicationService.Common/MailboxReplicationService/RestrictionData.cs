using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[KnownType(typeof(FalseRestrictionData))]
	[KnownType(typeof(PropertyRestrictionData))]
	[KnownType(typeof(BitMaskRestrictionData))]
	[KnownType(typeof(ComparePropertyRestrictionData))]
	[KnownType(typeof(ExistRestrictionData))]
	[KnownType(typeof(CountRestrictionData))]
	[KnownType(typeof(AttachmentRestrictionData))]
	[KnownType(typeof(RecipientRestrictionData))]
	[KnownType(typeof(CommentRestrictionData))]
	[KnownType(typeof(TrueRestrictionData))]
	[KnownType(typeof(NotRestrictionData))]
	[KnownType(typeof(NullRestrictionData))]
	[KnownType(typeof(ContentRestrictionData))]
	[KnownType(typeof(SizeRestrictionData))]
	[KnownType(typeof(NearRestrictionData))]
	[DataContract]
	[KnownType(typeof(AndRestrictionData))]
	[KnownType(typeof(OrRestrictionData))]
	internal abstract class RestrictionData
	{
		public RestrictionData()
		{
		}

		[DataMember(EmitDefaultValue = false)]
		public int LCID { get; set; }

		static RestrictionData()
		{
			foreach (KeyValuePair<int, int> keyValuePair in RestrictionData.ComparisonOperatorToRelOp)
			{
				RestrictionData.RelOpToComparisonOperator.Add(keyValuePair.Value, keyValuePair.Key);
			}
		}

		public override string ToString()
		{
			if (this.LCID != 0)
			{
				return string.Format("Restriction: LCID=0x{0:X}, {1}", this.LCID, this.ToStringInternal());
			}
			return string.Format("Restriction: {0}", this.ToStringInternal());
		}

		internal static RestrictionData GetRestrictionData(Restriction restriction)
		{
			if (restriction is Restriction.AndRestriction)
			{
				return new AndRestrictionData((Restriction.AndRestriction)restriction);
			}
			if (restriction is Restriction.OrRestriction)
			{
				return new OrRestrictionData((Restriction.OrRestriction)restriction);
			}
			if (restriction is Restriction.NotRestriction)
			{
				return new NotRestrictionData((Restriction.NotRestriction)restriction);
			}
			if (restriction is Restriction.CountRestriction)
			{
				return new CountRestrictionData((Restriction.CountRestriction)restriction);
			}
			if (restriction is Restriction.PropertyRestriction)
			{
				return new PropertyRestrictionData((Restriction.PropertyRestriction)restriction);
			}
			if (restriction is Restriction.ContentRestriction)
			{
				return new ContentRestrictionData((Restriction.ContentRestriction)restriction);
			}
			if (restriction is Restriction.BitMaskRestriction)
			{
				return new BitMaskRestrictionData((Restriction.BitMaskRestriction)restriction);
			}
			if (restriction is Restriction.ComparePropertyRestriction)
			{
				return new ComparePropertyRestrictionData((Restriction.ComparePropertyRestriction)restriction);
			}
			if (restriction is Restriction.ExistRestriction)
			{
				return new ExistRestrictionData((Restriction.ExistRestriction)restriction);
			}
			if (restriction is Restriction.SizeRestriction)
			{
				return new SizeRestrictionData((Restriction.SizeRestriction)restriction);
			}
			if (restriction is Restriction.AttachmentRestriction)
			{
				return new AttachmentRestrictionData((Restriction.AttachmentRestriction)restriction);
			}
			if (restriction is Restriction.RecipientRestriction)
			{
				return new RecipientRestrictionData((Restriction.RecipientRestriction)restriction);
			}
			if (restriction is Restriction.CommentRestriction)
			{
				return new CommentRestrictionData((Restriction.CommentRestriction)restriction);
			}
			if (restriction is Restriction.TrueRestriction)
			{
				return new TrueRestrictionData();
			}
			if (restriction is Restriction.FalseRestriction)
			{
				return new FalseRestrictionData();
			}
			if (restriction is Restriction.NearRestriction)
			{
				return new NearRestrictionData((Restriction.NearRestriction)restriction);
			}
			if (restriction == null)
			{
				return new TrueRestrictionData();
			}
			string type = restriction.GetType().ToString();
			throw new UnknownRestrictionTypeException(type);
		}

		internal static RestrictionData GetRestrictionData(StoreSession storeSession, QueryFilter queryFilter)
		{
			if (queryFilter is AndFilter)
			{
				return new AndRestrictionData(storeSession, (AndFilter)queryFilter);
			}
			if (queryFilter is OrFilter)
			{
				return new OrRestrictionData(storeSession, (OrFilter)queryFilter);
			}
			if (queryFilter is NotFilter)
			{
				return new NotRestrictionData(storeSession, (NotFilter)queryFilter);
			}
			if (queryFilter is CountFilter)
			{
				return new CountRestrictionData(storeSession, (CountFilter)queryFilter);
			}
			if (queryFilter is ComparisonFilter)
			{
				return new PropertyRestrictionData(storeSession, (ComparisonFilter)queryFilter);
			}
			if (queryFilter is ContentFilter)
			{
				return new ContentRestrictionData(storeSession, (ContentFilter)queryFilter);
			}
			if (queryFilter is BitMaskFilter)
			{
				return new BitMaskRestrictionData(storeSession, (BitMaskFilter)queryFilter);
			}
			if (queryFilter is PropertyComparisonFilter)
			{
				return new ComparePropertyRestrictionData(storeSession, (PropertyComparisonFilter)queryFilter);
			}
			if (queryFilter is ExistsFilter)
			{
				return new ExistRestrictionData(storeSession, (ExistsFilter)queryFilter);
			}
			if (queryFilter is SizeFilter)
			{
				return new SizeRestrictionData(storeSession, (SizeFilter)queryFilter);
			}
			if (queryFilter is SubFilter && ((SubFilter)queryFilter).SubFilterProperty == SubFilterProperties.Attachments)
			{
				return new AttachmentRestrictionData(storeSession, (SubFilter)queryFilter);
			}
			if (queryFilter is SubFilter && ((SubFilter)queryFilter).SubFilterProperty == SubFilterProperties.Recipients)
			{
				return new RecipientRestrictionData(storeSession, (SubFilter)queryFilter);
			}
			if (queryFilter is CommentFilter)
			{
				return new CommentRestrictionData(storeSession, (CommentFilter)queryFilter);
			}
			if (queryFilter is TrueFilter)
			{
				return new TrueRestrictionData();
			}
			if (queryFilter is FalseFilter)
			{
				return new FalseRestrictionData();
			}
			if (queryFilter is NearFilter)
			{
				return new NearRestrictionData(storeSession, (NearFilter)queryFilter);
			}
			if (queryFilter is NullFilter || queryFilter == null)
			{
				return new TrueRestrictionData();
			}
			string type = queryFilter.GetType().ToString();
			throw new UnknownRestrictionTypeException(type);
		}

		internal abstract Restriction GetRestriction();

		internal abstract QueryFilter GetQueryFilter(StoreSession storeSession);

		internal abstract string ToStringInternal();

		internal void Enumerate(RestrictionData.EnumRestrictionDelegate del)
		{
			del(this);
			HierRestrictionData hierRestrictionData = this as HierRestrictionData;
			if (hierRestrictionData != null)
			{
				foreach (RestrictionData restrictionData in hierRestrictionData.Restrictions)
				{
					if (restrictionData == null)
					{
						throw new CorruptRestrictionDataException();
					}
					restrictionData.Enumerate(del);
				}
			}
		}

		internal virtual void InternalEnumPropTags(CommonUtils.EnumPropTagDelegate del)
		{
		}

		internal virtual void InternalEnumPropValues(CommonUtils.EnumPropValueDelegate del)
		{
		}

		internal void EnumeratePropTags(CommonUtils.EnumPropTagDelegate del)
		{
			this.Enumerate(delegate(RestrictionData r)
			{
				r.InternalEnumPropTags(del);
			});
		}

		internal void EnumeratePropValues(CommonUtils.EnumPropValueDelegate del)
		{
			this.Enumerate(delegate(RestrictionData r)
			{
				r.InternalEnumPropValues(del);
			});
		}

		internal virtual int GetApproximateSize()
		{
			return 4;
		}

		protected int GetPropTagFromDefinition(StoreSession storeSession, PropertyDefinition definition)
		{
			uint[] array = new uint[1];
			PropertyTagCache.Cache.PropertyTagsFromPropertyDefinitions(storeSession, new List<NativeStorePropertyDefinition>
			{
				(NativeStorePropertyDefinition)definition
			}).CopyTo(array, 0);
			return (int)array[0];
		}

		protected int GetRelOpFromComparisonOperator(ComparisonOperator comparisonOperator)
		{
			int result;
			if (!RestrictionData.ComparisonOperatorToRelOp.TryGetValue((int)comparisonOperator, out result))
			{
				MrsTracer.Common.Error("Cannot convert comparison operator '{0}' to relop.", new object[]
				{
					comparisonOperator
				});
				throw new CorruptRestrictionDataException();
			}
			return result;
		}

		protected ComparisonOperator GetComparisonOperatorFromRelOp(int relOp)
		{
			int result;
			if (!RestrictionData.RelOpToComparisonOperator.TryGetValue(relOp, out result))
			{
				MrsTracer.Common.Error("Cannot convert relop '{0}' to comparison operator.", new object[]
				{
					relOp
				});
				throw new CorruptRestrictionDataException();
			}
			return (ComparisonOperator)result;
		}

		protected NativeStorePropertyDefinition GetPropertyDefinitionFromPropTag(StoreSession storeSession, int propTag)
		{
			return MapiUtils.ConvertPropTagsToDefinitions(storeSession, new PropTag[]
			{
				(PropTag)propTag
			})[0];
		}

		protected ContentFlags GetContentFlags(MatchFlags matchFlags, MatchOptions matchOptions)
		{
			ContentFlags contentFlags;
			switch (matchOptions)
			{
			case MatchOptions.SubString:
				contentFlags = ContentFlags.SubString;
				goto IL_34;
			case MatchOptions.Prefix:
				contentFlags = ContentFlags.Prefix;
				goto IL_34;
			case MatchOptions.PrefixOnWords:
				contentFlags = ContentFlags.PrefixOnWords;
				goto IL_34;
			case MatchOptions.ExactPhrase:
				contentFlags = ContentFlags.ExactPhrase;
				goto IL_34;
			}
			contentFlags = ContentFlags.FullString;
			IL_34:
			if ((matchFlags & MatchFlags.IgnoreCase) != MatchFlags.Default)
			{
				contentFlags |= ContentFlags.IgnoreCase;
			}
			if ((matchFlags & MatchFlags.IgnoreNonSpace) != MatchFlags.Default)
			{
				contentFlags |= ContentFlags.IgnoreNonSpace;
			}
			if ((matchFlags & MatchFlags.Loose) != MatchFlags.Default)
			{
				contentFlags |= ContentFlags.Loose;
			}
			return contentFlags;
		}

		protected void GetMatchFlagsAndOptions(ContentFlags contentFlags, out MatchFlags matchFlags, out MatchOptions matchOptions)
		{
			ContentFlags contentFlags2 = contentFlags & (ContentFlags)51;
			switch (contentFlags2)
			{
			case ContentFlags.SubString:
				matchOptions = MatchOptions.SubString;
				break;
			case ContentFlags.Prefix:
				matchOptions = MatchOptions.Prefix;
				break;
			default:
				if (contentFlags2 != ContentFlags.PrefixOnWords)
				{
					if (contentFlags2 != ContentFlags.ExactPhrase)
					{
						matchOptions = MatchOptions.FullString;
					}
					else
					{
						matchOptions = MatchOptions.ExactPhrase;
					}
				}
				else
				{
					matchOptions = MatchOptions.PrefixOnWords;
				}
				break;
			}
			matchFlags = MatchFlags.Default;
			if ((contentFlags & ContentFlags.IgnoreCase) != ContentFlags.FullString)
			{
				matchFlags |= MatchFlags.IgnoreCase;
			}
			if ((contentFlags & ContentFlags.IgnoreNonSpace) != ContentFlags.FullString)
			{
				matchFlags |= MatchFlags.IgnoreNonSpace;
			}
			if ((contentFlags & ContentFlags.Loose) != ContentFlags.FullString)
			{
				matchFlags |= MatchFlags.Loose;
			}
		}

		private static readonly Dictionary<int, int> ComparisonOperatorToRelOp = new Dictionary<int, int>
		{
			{
				0,
				4
			},
			{
				1,
				5
			},
			{
				2,
				0
			},
			{
				3,
				1
			},
			{
				4,
				2
			},
			{
				5,
				3
			},
			{
				6,
				6
			},
			{
				7,
				100
			}
		};

		private static readonly Dictionary<int, int> RelOpToComparisonOperator = new Dictionary<int, int>();

		internal delegate void EnumRestrictionDelegate(RestrictionData rd);
	}
}
