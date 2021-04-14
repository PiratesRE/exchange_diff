using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class Restriction : IEquatable<Restriction>
	{
		public static Restriction And(params Restriction[] restrictions)
		{
			return new Restriction.AndRestriction(restrictions);
		}

		public static Restriction Near(int distance, bool ordered, Restriction.AndRestriction restriction)
		{
			return new Restriction.NearRestriction(distance, ordered, restriction);
		}

		public static Restriction BitMask(Restriction.RelBmr relBmr, PropTag tag, int mask)
		{
			return new Restriction.BitMaskRestriction(relBmr, tag, mask);
		}

		public static Restriction BitMaskZero(PropTag tag, int mask)
		{
			return new Restriction.BitMaskRestriction(Restriction.RelBmr.EqualToZero, tag, mask);
		}

		public static Restriction BitMaskNonZero(PropTag tag, int mask)
		{
			return new Restriction.BitMaskRestriction(Restriction.RelBmr.NotEqualToZero, tag, mask);
		}

		public static Restriction Comment(Restriction restriction, params PropValue[] propValues)
		{
			return new Restriction.CommentRestriction(restriction, propValues);
		}

		public static Restriction MemberOf(PropTag tag, object value)
		{
			return new Restriction.PropertyRestriction(Restriction.RelOp.MemberOfDL, tag, value);
		}

		public static Restriction Property(Restriction.RelOp relOp, PropTag tag, bool multiValued, PropValue value)
		{
			return new Restriction.PropertyRestriction(relOp, tag, multiValued, value);
		}

		public static Restriction GE(PropTag tag, object value)
		{
			return new Restriction.PropertyRestriction(Restriction.RelOp.GreaterThanOrEqual, tag, value);
		}

		public static Restriction GT(PropTag tag, object value)
		{
			return new Restriction.PropertyRestriction(Restriction.RelOp.GreaterThan, tag, value);
		}

		public static Restriction LE(PropTag tag, object value)
		{
			return new Restriction.PropertyRestriction(Restriction.RelOp.LessThanOrEqual, tag, value);
		}

		public static Restriction LT(PropTag tag, object value)
		{
			return new Restriction.PropertyRestriction(Restriction.RelOp.LessThan, tag, value);
		}

		public static Restriction NE(PropTag tag, object value)
		{
			return new Restriction.PropertyRestriction(Restriction.RelOp.NotEqual, tag, value);
		}

		public static Restriction RE(PropTag tag, string value)
		{
			return new Restriction.PropertyRestriction(Restriction.RelOp.RegularExpression, tag, value);
		}

		public static Restriction EQ(PropTag tag, object value)
		{
			return new Restriction.PropertyRestriction(Restriction.RelOp.Equal, tag, value);
		}

		public static Restriction CompareProps(Restriction.RelOp relOp, PropTag tag1, PropTag tag2)
		{
			return new Restriction.ComparePropertyRestriction(relOp, tag1, tag2);
		}

		public static Restriction GE(PropTag tag1, PropTag tag2)
		{
			return new Restriction.ComparePropertyRestriction(Restriction.RelOp.GreaterThanOrEqual, tag1, tag2);
		}

		public static Restriction GT(PropTag tag1, PropTag tag2)
		{
			return new Restriction.ComparePropertyRestriction(Restriction.RelOp.GreaterThan, tag1, tag2);
		}

		public static Restriction LE(PropTag tag1, PropTag tag2)
		{
			return new Restriction.ComparePropertyRestriction(Restriction.RelOp.LessThanOrEqual, tag1, tag2);
		}

		public static Restriction LT(PropTag tag1, PropTag tag2)
		{
			return new Restriction.ComparePropertyRestriction(Restriction.RelOp.LessThan, tag1, tag2);
		}

		public static Restriction NE(PropTag tag1, PropTag tag2)
		{
			return new Restriction.ComparePropertyRestriction(Restriction.RelOp.NotEqual, tag1, tag2);
		}

		public static Restriction RE(PropTag tag1, PropTag tag2)
		{
			return new Restriction.ComparePropertyRestriction(Restriction.RelOp.RegularExpression, tag1, tag2);
		}

		public static Restriction EQ(PropTag tag1, PropTag tag2)
		{
			return new Restriction.ComparePropertyRestriction(Restriction.RelOp.Equal, tag1, tag2);
		}

		public static Restriction Content(PropTag tag, object value, ContentFlags flags)
		{
			return new Restriction.ContentRestriction(tag, value, flags);
		}

		public static Restriction Content(PropTag tag, bool multiValued, PropValue value, ContentFlags flags)
		{
			return new Restriction.ContentRestriction(tag, multiValued, value, flags);
		}

		public static Restriction Exist(PropTag tag)
		{
			return new Restriction.ExistRestriction(tag);
		}

		public static Restriction Not(Restriction restriction)
		{
			return new Restriction.NotRestriction(restriction);
		}

		public static Restriction Count(int count, Restriction restriction)
		{
			return new Restriction.CountRestriction(count, restriction);
		}

		public static Restriction Or(params Restriction[] restrictions)
		{
			return new Restriction.OrRestriction(restrictions);
		}

		public static Restriction PropertySize(Restriction.RelOp relOp, PropTag tag, int cb)
		{
			return new Restriction.SizeRestriction(relOp, tag, cb);
		}

		public static Restriction SizeGE(PropTag tag, int cb)
		{
			return new Restriction.SizeRestriction(Restriction.RelOp.GreaterThanOrEqual, tag, cb);
		}

		public static Restriction SizeGT(PropTag tag, int cb)
		{
			return new Restriction.SizeRestriction(Restriction.RelOp.GreaterThan, tag, cb);
		}

		public static Restriction SizeLE(PropTag tag, int cb)
		{
			return new Restriction.SizeRestriction(Restriction.RelOp.LessThanOrEqual, tag, cb);
		}

		public static Restriction SizeLT(PropTag tag, int cb)
		{
			return new Restriction.SizeRestriction(Restriction.RelOp.LessThan, tag, cb);
		}

		public static Restriction SizeNE(PropTag tag, int cb)
		{
			return new Restriction.SizeRestriction(Restriction.RelOp.NotEqual, tag, cb);
		}

		public static Restriction SizeEQ(PropTag tag, int cb)
		{
			return new Restriction.SizeRestriction(Restriction.RelOp.Equal, tag, cb);
		}

		public static Restriction Sub(PropTag tag, Restriction restriction)
		{
			if (tag == PropTag.MessageRecipients)
			{
				return new Restriction.RecipientRestriction(restriction);
			}
			if (tag != PropTag.MessageAttachments)
			{
				return null;
			}
			return new Restriction.AttachmentRestriction(restriction);
		}

		public static Restriction False()
		{
			return new Restriction.FalseRestriction();
		}

		public static Restriction True()
		{
			return new Restriction.TrueRestriction();
		}

		public override bool Equals(object comparand)
		{
			return comparand is Restriction && this.Equals((Restriction)comparand);
		}

		public bool Equals(Restriction comparand)
		{
			return this.IsEqualTo(comparand);
		}

		public static bool Equals(Restriction v1, Restriction v2)
		{
			return v1.Equals(v2);
		}

		public override int GetHashCode()
		{
			int result = 0;
			this.EnumerateRestriction(delegate(Restriction restriction, object ctx)
			{
				result = (int)(result + restriction.Type);
			}, null);
			return result;
		}

		public Restriction.ResType Type
		{
			get
			{
				return this.resType;
			}
		}

		public abstract int GetBytesToMarshal();

		public abstract int GetBytesToMarshalNspi();

		internal unsafe abstract void MarshalToNative(SRestriction* psr, ref byte* pExtra);

		internal unsafe abstract void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra);

		public unsafe void MarshalToNative(SafeHandle handle)
		{
			SRestriction* ptr = (SRestriction*)handle.DangerousGetHandle().ToPointer();
			byte* ptr2 = (byte*)(ptr + (SRestriction.SizeOf + 7 & -8) / sizeof(SRestriction));
			this.MarshalToNative(ptr, ref ptr2);
		}

		public unsafe void MarshalToNativeNspi(SafeHandle handle)
		{
			SNspiRestriction* ptr = (SNspiRestriction*)handle.DangerousGetHandle().ToPointer();
			byte* ptr2 = (byte*)(ptr + (SNspiRestriction.SizeOf + 7 & -8) / sizeof(SNspiRestriction));
			this.MarshalToNative(ptr, ref ptr2);
		}

		internal virtual bool IsEqualTo(Restriction other)
		{
			return this.Type == other.Type;
		}

		private Restriction(Restriction.ResType resType)
		{
			this.resType = resType;
		}

		internal static Restriction Unmarshal(SafeHandle restriction)
		{
			return Restriction.Unmarshal(restriction.DangerousGetHandle());
		}

		public unsafe static Restriction Unmarshal(IntPtr restriction)
		{
			return Restriction.Unmarshal((SRestriction*)restriction.ToPointer());
		}

		internal unsafe static Restriction Unmarshal(SRestriction* psres)
		{
			if (null != psres)
			{
				Restriction.ResType rt = (Restriction.ResType)psres->rt;
				switch (rt)
				{
				case Restriction.ResType.And:
					return new Restriction.AndRestriction(psres);
				case Restriction.ResType.Or:
					return new Restriction.OrRestriction(psres);
				case Restriction.ResType.Not:
					return new Restriction.NotRestriction(psres);
				case Restriction.ResType.Content:
					return new Restriction.ContentRestriction(psres);
				case Restriction.ResType.Property:
					return new Restriction.PropertyRestriction(psres);
				case Restriction.ResType.CompareProps:
					return new Restriction.ComparePropertyRestriction(psres);
				case Restriction.ResType.BitMask:
					return new Restriction.BitMaskRestriction(psres);
				case Restriction.ResType.Size:
					return new Restriction.SizeRestriction(psres);
				case Restriction.ResType.Exist:
					return new Restriction.ExistRestriction(psres);
				case Restriction.ResType.SubRestriction:
				{
					PropTag ulSubObject = (PropTag)psres->union.resSub.ulSubObject;
					if (ulSubObject == PropTag.MessageRecipients)
					{
						return new Restriction.RecipientRestriction(psres);
					}
					if (ulSubObject != PropTag.MessageAttachments)
					{
						return null;
					}
					return new Restriction.AttachmentRestriction(psres);
				}
				case Restriction.ResType.Comment:
					return new Restriction.CommentRestriction(psres);
				case Restriction.ResType.Count:
					return new Restriction.CountRestriction(psres);
				case (Restriction.ResType)12:
					break;
				case Restriction.ResType.Near:
					return new Restriction.NearRestriction(psres);
				default:
					switch (rt)
					{
					case Restriction.ResType.True:
						return new Restriction.TrueRestriction(psres);
					case Restriction.ResType.False:
						return new Restriction.FalseRestriction(psres);
					}
					break;
				}
			}
			return null;
		}

		public unsafe static Restriction UnmarshalNspi(IntPtr restriction)
		{
			return Restriction.UnmarshalNspi((SNspiRestriction*)restriction.ToPointer());
		}

		internal unsafe static Restriction UnmarshalNspi(SNspiRestriction* psres)
		{
			if (null != psres)
			{
				switch (psres->rt)
				{
				case 0:
					return new Restriction.AndRestriction(psres);
				case 1:
					return new Restriction.OrRestriction(psres);
				case 2:
					return new Restriction.NotRestriction(psres);
				case 3:
					return new Restriction.ContentRestriction(psres);
				case 4:
					return new Restriction.PropertyRestriction(psres);
				case 8:
					return new Restriction.ExistRestriction(psres);
				}
			}
			return null;
		}

		public void EnumerateRestriction(Restriction.EnumRestrictionDelegate del, object ctx)
		{
			del(this, ctx);
			this.EnumerateSubRestrictions(del, ctx);
		}

		internal virtual void EnumerateSubRestrictions(Restriction.EnumRestrictionDelegate del, object ctx)
		{
		}

		private Restriction.ResType resType;

		internal enum ResType
		{
			And,
			Or,
			Not,
			Content,
			Property,
			CompareProps,
			BitMask,
			Size,
			Exist,
			SubRestriction,
			Comment,
			Count,
			Near = 13,
			True = 131,
			False
		}

		internal enum RelOp
		{
			LessThan,
			LessThanOrEqual,
			GreaterThan,
			GreaterThanOrEqual,
			Equal,
			NotEqual,
			RegularExpression,
			Include = 16,
			Exclude,
			MemberOfDL = 100
		}

		internal enum RelBmr
		{
			EqualToZero,
			NotEqualToZero
		}

		public delegate void EnumRestrictionDelegate(Restriction restriction, object ctx);

		public class AndOrNotRestriction : Restriction
		{
			internal AndOrNotRestriction(Restriction.ResType resType, params Restriction[] restrictions) : base(resType)
			{
				this.restrictions = restrictions;
			}

			internal unsafe AndOrNotRestriction(SRestriction* psres) : base((Restriction.ResType)psres->rt)
			{
				int num = (this.resType == Restriction.ResType.Not) ? 1 : psres->union.resAnd.cRes;
				this.restrictions = new Restriction[num];
				for (int i = 0; i < num; i++)
				{
					this.restrictions[i] = Restriction.Unmarshal(psres->union.resAnd.lpRes + i);
				}
			}

			internal unsafe AndOrNotRestriction(SNspiRestriction* psres) : base((Restriction.ResType)psres->rt)
			{
				int num = (this.resType == Restriction.ResType.Not) ? 1 : psres->union.resAnd.cRes;
				this.restrictions = new Restriction[num];
				for (int i = 0; i < num; i++)
				{
					this.restrictions[i] = Restriction.UnmarshalNspi(psres->union.resAnd.lpRes + i);
				}
			}

			public override int GetBytesToMarshal()
			{
				int num = SRestriction.SizeOf + 7 & -8;
				foreach (Restriction restriction in this.restrictions)
				{
					num += (restriction.GetBytesToMarshal() + 7 & -8);
				}
				return num;
			}

			public override int GetBytesToMarshalNspi()
			{
				int num = SNspiRestriction.SizeOf + 7 & -8;
				foreach (Restriction restriction in this.restrictions)
				{
					num += (restriction.GetBytesToMarshalNspi() + 7 & -8);
				}
				return num;
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				SRestriction* lpRes = pExtra;
				if (this.restrictions.Length > 0)
				{
					pExtra += (IntPtr)(SRestriction.SizeOf * this.restrictions.Length + 7 & -8);
				}
				psr->rt = (int)this.resType;
				psr->union.resAnd.cRes = this.restrictions.Length;
				if (this.restrictions.Length > 0)
				{
					psr->union.resAnd.lpRes = lpRes;
				}
				else
				{
					psr->union.resAnd.lpRes = null;
				}
				foreach (Restriction restriction in this.restrictions)
				{
					restriction.MarshalToNative(lpRes++, ref pExtra);
				}
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				SNspiRestriction* lpRes = pExtra;
				if (this.restrictions.Length > 0)
				{
					pExtra += (IntPtr)(SNspiRestriction.SizeOf * this.restrictions.Length + 7 & -8);
				}
				psr->rt = (int)this.resType;
				psr->union.resAnd.cRes = this.restrictions.Length;
				if (this.restrictions.Length > 0)
				{
					psr->union.resAnd.lpRes = lpRes;
				}
				else
				{
					psr->union.resAnd.lpRes = null;
				}
				foreach (Restriction restriction in this.restrictions)
				{
					restriction.MarshalToNative(lpRes++, ref pExtra);
				}
			}

			internal override void EnumerateSubRestrictions(Restriction.EnumRestrictionDelegate del, object ctx)
			{
				foreach (Restriction restriction in this.restrictions)
				{
					restriction.EnumerateRestriction(del, ctx);
				}
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction[] array = ((Restriction.AndOrNotRestriction)other).restrictions;
				if (this.restrictions.Length != array.Length)
				{
					return false;
				}
				for (int i = 0; i < this.restrictions.Length; i++)
				{
					if (!this.restrictions[i].IsEqualTo(array[i]))
					{
						return false;
					}
				}
				return true;
			}

			internal Restriction[] restrictions;
		}

		public class AndRestriction : Restriction.AndOrNotRestriction
		{
			public Restriction[] Restrictions
			{
				get
				{
					return this.restrictions;
				}
				set
				{
					this.restrictions = value;
				}
			}

			public AndRestriction(params Restriction[] restrictions) : base(Restriction.ResType.And, restrictions)
			{
			}

			internal unsafe AndRestriction(SRestriction* psres) : base(psres)
			{
			}

			internal unsafe AndRestriction(SNspiRestriction* psres) : base(psres)
			{
			}
		}

		public class OrRestriction : Restriction.AndOrNotRestriction
		{
			public Restriction[] Restrictions
			{
				get
				{
					return this.restrictions;
				}
				set
				{
					this.restrictions = value;
				}
			}

			public OrRestriction(params Restriction[] restrictions) : base(Restriction.ResType.Or, restrictions)
			{
			}

			internal unsafe OrRestriction(SRestriction* psres) : base(psres)
			{
			}

			internal unsafe OrRestriction(SNspiRestriction* psres) : base(psres)
			{
			}
		}

		public class NotRestriction : Restriction.AndOrNotRestriction
		{
			public Restriction Restriction
			{
				get
				{
					return this.restrictions[0];
				}
				set
				{
					this.restrictions = new Restriction[]
					{
						value
					};
				}
			}

			public NotRestriction(Restriction restriction) : base(Restriction.ResType.Not, new Restriction[]
			{
				restriction
			})
			{
			}

			internal unsafe NotRestriction(SRestriction* psres) : base(psres)
			{
			}

			internal unsafe NotRestriction(SNspiRestriction* psres) : base(psres)
			{
			}
		}

		public class NearRestriction : Restriction
		{
			public int Distance
			{
				get
				{
					return this.distance;
				}
				set
				{
					this.distance = value;
				}
			}

			public bool Ordered
			{
				get
				{
					return this.ordered;
				}
				set
				{
					this.ordered = value;
				}
			}

			public Restriction.AndRestriction Restriction
			{
				get
				{
					return this.restriction;
				}
				set
				{
					this.restriction = value;
				}
			}

			public NearRestriction(int distance, bool ordered, Restriction.AndRestriction restriction) : base(Microsoft.Mapi.Restriction.ResType.Near)
			{
				this.Distance = distance;
				this.Ordered = ordered;
				this.Restriction = restriction;
			}

			internal unsafe NearRestriction(SRestriction* psres) : base((Restriction.ResType)psres->rt)
			{
				this.Distance = psres->union.resNear.ulDistance;
				this.Ordered = (psres->union.resNear.ulOrdered == 1);
				this.Restriction = (Microsoft.Mapi.Restriction.Unmarshal(psres->union.resNear.lpRes) as Restriction.AndRestriction);
			}

			public override int GetBytesToMarshal()
			{
				int num = SRestriction.SizeOf + 7 & -8;
				return num + (this.restriction.GetBytesToMarshal() + 7 & -8);
			}

			public override int GetBytesToMarshalNspi()
			{
				throw new NotSupportedException();
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				SRestriction* ptr = pExtra;
				pExtra += (IntPtr)(SRestriction.SizeOf + 7 & -8);
				psr->rt = 13;
				psr->union.resNear.ulDistance = this.distance;
				psr->union.resNear.ulOrdered = (this.ordered ? 1 : 0);
				psr->union.resNear.lpRes = ptr;
				this.restriction.MarshalToNative(ptr, ref pExtra);
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				throw new NotSupportedException();
			}

			internal override void EnumerateSubRestrictions(Restriction.EnumRestrictionDelegate del, object ctx)
			{
				this.restriction.EnumerateRestriction(del, ctx);
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.NearRestriction nearRestriction = other as Restriction.NearRestriction;
				return nearRestriction != null && nearRestriction.Distance == this.Distance && nearRestriction.Ordered == this.Ordered && this.restriction.IsEqualTo(nearRestriction.Restriction);
			}

			private Restriction.AndRestriction restriction;

			private int distance;

			private bool ordered;
		}

		public class CountRestriction : Restriction
		{
			public new int Count
			{
				get
				{
					return this.count;
				}
				set
				{
					this.count = value;
				}
			}

			public Restriction Restriction
			{
				get
				{
					return this.restriction;
				}
				set
				{
					this.restriction = value;
				}
			}

			public CountRestriction(int count, Restriction restriction) : base(Restriction.ResType.Count)
			{
				this.count = count;
				this.restriction = restriction;
			}

			internal unsafe CountRestriction(SRestriction* psres) : base(Restriction.ResType.Count)
			{
				this.count = psres->union.resCount.ulCount;
				this.restriction = Restriction.Unmarshal(psres->union.resCount.lpRes);
			}

			public override int GetBytesToMarshal()
			{
				int num = SRestriction.SizeOf + 7 & -8;
				return num + (this.restriction.GetBytesToMarshal() + 7 & -8);
			}

			public override int GetBytesToMarshalNspi()
			{
				throw new NotSupportedException();
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				SRestriction* ptr = pExtra;
				pExtra += (IntPtr)(SRestriction.SizeOf + 7 & -8);
				psr->rt = 11;
				psr->union.resCount.ulCount = this.count;
				psr->union.resCount.lpRes = ptr;
				this.restriction.MarshalToNative(ptr, ref pExtra);
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				throw new NotSupportedException();
			}

			internal override void EnumerateSubRestrictions(Restriction.EnumRestrictionDelegate del, object ctx)
			{
				this.restriction.EnumerateRestriction(del, ctx);
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.CountRestriction countRestriction = (Restriction.CountRestriction)other;
				return this.Count == countRestriction.Count && this.restriction.IsEqualTo(countRestriction.restriction);
			}

			private int count;

			private Restriction restriction;
		}

		public class PropertyRestriction : Restriction
		{
			public Restriction.RelOp Op
			{
				get
				{
					return this.relOp;
				}
				set
				{
					this.relOp = value;
				}
			}

			public PropTag PropTag
			{
				get
				{
					return this.propTag & (PropTag)4294963199U;
				}
				set
				{
					bool multiValued = this.MultiValued;
					this.propTag = value;
					this.MultiValued = multiValued;
				}
			}

			public bool MultiValued
			{
				get
				{
					return (this.propTag & (PropTag)4096U) == (PropTag)4096U;
				}
				set
				{
					this.propTag = (value ? (this.propTag | (PropTag)4096U) : (this.propTag & (PropTag)4294963199U));
				}
			}

			public PropValue PropValue
			{
				get
				{
					return this.propValue;
				}
				set
				{
					this.propValue = value;
				}
			}

			public PropertyRestriction(Restriction.RelOp relOp, PropTag tag, object value) : this(relOp, tag, false, new PropValue(tag, value))
			{
			}

			public PropertyRestriction(Restriction.RelOp relOp, PropTag tag, bool multiValued, object value) : this(relOp, tag, multiValued, new PropValue(tag, value))
			{
			}

			public PropertyRestriction(Restriction.RelOp relOp, PropTag tag, PropValue value) : this(relOp, tag, false, value)
			{
			}

			public PropertyRestriction(Restriction.RelOp relOp, PropTag tag, bool multiValued, PropValue value) : base(Restriction.ResType.Property)
			{
				this.relOp = relOp;
				this.propTag = tag;
				this.propValue = value;
				this.MultiValued = multiValued;
			}

			internal unsafe PropertyRestriction(SRestriction* psres) : base(Restriction.ResType.Property)
			{
				this.relOp = (Restriction.RelOp)psres->union.resProperty.relop;
				this.propTag = (PropTag)psres->union.resProperty.ulPropTag;
				this.propValue = new PropValue(psres->union.resProperty.lpProp);
			}

			internal unsafe PropertyRestriction(SNspiRestriction* psres) : base(Restriction.ResType.Property)
			{
				this.relOp = (Restriction.RelOp)psres->union.resProperty.relop;
				this.propTag = (PropTag)psres->union.resProperty.ulPropTag;
				this.propValue = new PropValue(psres->union.resProperty.lpProp, true);
			}

			public override int GetBytesToMarshal()
			{
				return (SRestriction.SizeOf + 7 & -8) + (this.propValue.GetBytesToMarshal() + 7 & -8);
			}

			public override int GetBytesToMarshalNspi()
			{
				return (SNspiRestriction.SizeOf + 7 & -8) + (this.propValue.GetBytesToMarshal() + 7 & -8);
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				SPropValue* ptr = pExtra;
				pExtra += (IntPtr)(SPropValue.SizeOf + 7 & -8);
				psr->rt = 4;
				psr->union.resProperty.relop = (int)this.relOp;
				psr->union.resProperty.ulPropTag = (int)this.propTag;
				psr->union.resProperty.lpProp = ptr;
				this.propValue.MarshalToNative(ptr, ref pExtra);
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				SPropValue* ptr = pExtra;
				pExtra += (IntPtr)(SPropValue.SizeOf + 7 & -8);
				psr->rt = 4;
				psr->union.resProperty.relop = (int)this.relOp;
				psr->union.resProperty.ulPropTag = (int)this.propTag;
				psr->union.resProperty.lpProp = ptr;
				this.propValue.MarshalToNative(ptr, ref pExtra);
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.PropertyRestriction propertyRestriction = (Restriction.PropertyRestriction)other;
				return this.Op == propertyRestriction.Op && this.PropTag == propertyRestriction.PropTag && this.PropValue.IsEqualTo(propertyRestriction.PropValue);
			}

			private Restriction.RelOp relOp;

			private PropValue propValue;

			private PropTag propTag;
		}

		public class ContentRestriction : Restriction
		{
			public ContentFlags Flags
			{
				get
				{
					return this.contentFlags;
				}
				set
				{
					this.contentFlags = value;
				}
			}

			public PropTag PropTag
			{
				get
				{
					return this.propTag & (PropTag)4294963199U;
				}
				set
				{
					bool multiValued = this.MultiValued;
					this.propTag = value;
					this.MultiValued = multiValued;
				}
			}

			public PropValue PropValue
			{
				get
				{
					return this.propValue;
				}
				set
				{
					this.propValue = value;
				}
			}

			public bool MultiValued
			{
				get
				{
					return (this.propTag & (PropTag)4096U) == (PropTag)4096U;
				}
				set
				{
					this.propTag = (value ? (this.propTag | (PropTag)4096U) : (this.propTag & (PropTag)4294963199U));
				}
			}

			public ContentRestriction(PropTag tag, object value, ContentFlags flags) : this(tag, false, new PropValue(tag, value), flags)
			{
			}

			public ContentRestriction(PropTag tag, bool multiValued, object value, ContentFlags flags) : this(tag, multiValued, new PropValue(tag, value), flags)
			{
			}

			public ContentRestriction(PropTag tag, bool multiValued, PropValue value, ContentFlags flags) : base(Restriction.ResType.Content)
			{
				this.contentFlags = flags;
				this.propTag = tag;
				this.propValue = value;
				this.MultiValued = multiValued;
			}

			internal unsafe ContentRestriction(SRestriction* psres) : base(Restriction.ResType.Content)
			{
				this.contentFlags = (ContentFlags)psres->union.resContent.ulFuzzyLevel;
				this.propValue = new PropValue(psres->union.resContent.lpProp);
				this.propTag = (PropTag)psres->union.resContent.ulPropTag;
			}

			internal unsafe ContentRestriction(SNspiRestriction* psres) : base(Restriction.ResType.Content)
			{
				this.contentFlags = (ContentFlags)psres->union.resContent.ulFuzzyLevel;
				this.propValue = new PropValue(psres->union.resContent.lpProp, true);
				this.propTag = (PropTag)psres->union.resContent.ulPropTag;
			}

			public override int GetBytesToMarshal()
			{
				return (SRestriction.SizeOf + 7 & -8) + (this.propValue.GetBytesToMarshal() + 7 & -8);
			}

			public override int GetBytesToMarshalNspi()
			{
				return (SNspiRestriction.SizeOf + 7 & -8) + (this.propValue.GetBytesToMarshal() + 7 & -8);
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				SPropValue* ptr = pExtra;
				pExtra += (IntPtr)(SPropValue.SizeOf + 7 & -8);
				psr->rt = 3;
				psr->union.resContent.ulFuzzyLevel = (int)this.contentFlags;
				psr->union.resContent.ulPropTag = (int)this.propTag;
				psr->union.resContent.lpProp = ptr;
				this.propValue.MarshalToNative(ptr, ref pExtra);
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				SPropValue* ptr = pExtra;
				pExtra += (IntPtr)(SPropValue.SizeOf + 7 & -8);
				psr->rt = 3;
				psr->union.resContent.ulFuzzyLevel = (int)this.contentFlags;
				psr->union.resContent.ulPropTag = (int)this.propTag;
				psr->union.resContent.lpProp = ptr;
				this.propValue.MarshalToNative(ptr, ref pExtra);
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.ContentRestriction contentRestriction = (Restriction.ContentRestriction)other;
				return this.Flags == contentRestriction.Flags && this.PropTag == contentRestriction.PropTag && this.MultiValued == contentRestriction.MultiValued && this.PropValue.IsEqualTo(contentRestriction.PropValue);
			}

			private ContentFlags contentFlags;

			private PropValue propValue;

			private PropTag propTag;
		}

		public class BitMaskRestriction : Restriction
		{
			public PropTag Tag
			{
				get
				{
					return this.tag;
				}
				set
				{
					this.tag = value;
				}
			}

			public Restriction.RelBmr Bmr
			{
				get
				{
					return this.relbmr;
				}
				set
				{
					this.relbmr = value;
				}
			}

			public int Mask
			{
				get
				{
					return this.mask;
				}
				set
				{
					this.mask = value;
				}
			}

			public BitMaskRestriction(Restriction.RelBmr relbmr, PropTag tag, int mask) : base(Restriction.ResType.BitMask)
			{
				this.relbmr = relbmr;
				this.tag = tag;
				this.mask = mask;
			}

			internal unsafe BitMaskRestriction(SRestriction* psres) : base(Restriction.ResType.BitMask)
			{
				this.mask = psres->union.resBitMask.ulMask;
				this.tag = (PropTag)psres->union.resBitMask.ulPropTag;
				this.relbmr = (Restriction.RelBmr)psres->union.resBitMask.relBMR;
			}

			public override int GetBytesToMarshal()
			{
				return SRestriction.SizeOf + 7 & -8;
			}

			public override int GetBytesToMarshalNspi()
			{
				throw new NotSupportedException();
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				psr->rt = 6;
				psr->union.resBitMask.relBMR = (int)this.relbmr;
				psr->union.resBitMask.ulPropTag = (int)this.tag;
				psr->union.resBitMask.ulMask = this.mask;
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				throw new NotSupportedException();
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.BitMaskRestriction bitMaskRestriction = (Restriction.BitMaskRestriction)other;
				return this.Bmr == bitMaskRestriction.Bmr && this.Tag == bitMaskRestriction.Tag && this.Mask == bitMaskRestriction.Mask;
			}

			private Restriction.RelBmr relbmr;

			private PropTag tag;

			private int mask;
		}

		public class ComparePropertyRestriction : Restriction
		{
			public PropTag TagLeft
			{
				get
				{
					return this.tagLeft;
				}
				set
				{
					this.tagLeft = value;
				}
			}

			public PropTag TagRight
			{
				get
				{
					return this.tagRight;
				}
				set
				{
					this.tagRight = value;
				}
			}

			public Restriction.RelOp Op
			{
				get
				{
					return this.relOp;
				}
				set
				{
					this.relOp = value;
				}
			}

			public ComparePropertyRestriction(Restriction.RelOp relOp, PropTag tagLeft, PropTag tagRight) : base(Restriction.ResType.CompareProps)
			{
				this.relOp = relOp;
				this.tagLeft = tagLeft;
				this.tagRight = tagRight;
			}

			internal unsafe ComparePropertyRestriction(SRestriction* psres) : base(Restriction.ResType.CompareProps)
			{
				this.relOp = (Restriction.RelOp)psres->union.resCompareProps.relop;
				this.tagLeft = (PropTag)psres->union.resCompareProps.ulPropTag1;
				this.tagRight = (PropTag)psres->union.resCompareProps.ulPropTag2;
			}

			public override int GetBytesToMarshal()
			{
				return SRestriction.SizeOf + 7 & -8;
			}

			public override int GetBytesToMarshalNspi()
			{
				throw new NotSupportedException();
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				psr->rt = 5;
				psr->union.resCompareProps.relop = (int)this.relOp;
				psr->union.resCompareProps.ulPropTag1 = (int)this.tagLeft;
				psr->union.resCompareProps.ulPropTag2 = (int)this.tagRight;
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				throw new NotSupportedException();
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.ComparePropertyRestriction comparePropertyRestriction = (Restriction.ComparePropertyRestriction)other;
				return this.TagLeft == comparePropertyRestriction.TagLeft && this.TagRight == comparePropertyRestriction.TagRight && this.Op == comparePropertyRestriction.Op;
			}

			private Restriction.RelOp relOp;

			private PropTag tagLeft;

			private PropTag tagRight;
		}

		public class ExistRestriction : Restriction
		{
			public PropTag Tag
			{
				get
				{
					return this.tag;
				}
				set
				{
					this.tag = value;
				}
			}

			public ExistRestriction(PropTag tag) : base(Restriction.ResType.Exist)
			{
				this.tag = tag;
			}

			internal unsafe ExistRestriction(SRestriction* psres) : base(Restriction.ResType.Exist)
			{
				this.tag = (PropTag)psres->union.resExist.ulPropTag;
			}

			internal unsafe ExistRestriction(SNspiRestriction* psres) : base(Restriction.ResType.Exist)
			{
				this.tag = (PropTag)psres->union.resExist.ulPropTag;
			}

			public override int GetBytesToMarshal()
			{
				return SRestriction.SizeOf + 7 & -8;
			}

			public override int GetBytesToMarshalNspi()
			{
				return SNspiRestriction.SizeOf + 7 & -8;
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				psr->rt = 8;
				psr->union.resExist.ulPropTag = (int)this.tag;
				psr->union.resExist.ulReserved1 = 0;
				psr->union.resExist.ulReserved2 = 0;
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				psr->rt = 8;
				psr->union.resExist.ulPropTag = (int)this.tag;
				psr->union.resExist.ulReserved1 = 0;
				psr->union.resExist.ulReserved2 = 0;
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.ExistRestriction existRestriction = (Restriction.ExistRestriction)other;
				return this.Tag == existRestriction.Tag;
			}

			private PropTag tag;
		}

		public class SizeRestriction : Restriction
		{
			public Restriction.RelOp Op
			{
				get
				{
					return this.relop;
				}
				set
				{
					this.relop = value;
				}
			}

			public PropTag Tag
			{
				get
				{
					return this.tag;
				}
				set
				{
					this.tag = value;
				}
			}

			public int Size
			{
				get
				{
					return this.size;
				}
				set
				{
					this.size = value;
				}
			}

			public SizeRestriction(Restriction.RelOp relop, PropTag tag, int size) : base(Restriction.ResType.Size)
			{
				this.relop = relop;
				this.tag = tag;
				this.size = size;
			}

			internal unsafe SizeRestriction(SRestriction* psres) : base(Restriction.ResType.Size)
			{
				this.relop = (Restriction.RelOp)psres->union.resSize.relop;
				this.tag = (PropTag)psres->union.resSize.ulPropTag;
				this.size = psres->union.resSize.cb;
			}

			public override int GetBytesToMarshal()
			{
				return SRestriction.SizeOf + 7 & -8;
			}

			public override int GetBytesToMarshalNspi()
			{
				throw new NotSupportedException();
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				psr->rt = 7;
				psr->union.resSize.relop = (int)this.relop;
				psr->union.resSize.ulPropTag = (int)this.tag;
				psr->union.resSize.cb = this.size;
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				throw new NotSupportedException();
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.SizeRestriction sizeRestriction = (Restriction.SizeRestriction)other;
				return this.Tag == sizeRestriction.Tag && this.Op == sizeRestriction.Op && this.Size == sizeRestriction.Size;
			}

			private Restriction.RelOp relop;

			private PropTag tag;

			private int size;
		}

		public class SubRestriction : Restriction
		{
			public Restriction Restriction
			{
				get
				{
					return this.restriction;
				}
				set
				{
					this.restriction = value;
				}
			}

			internal SubRestriction(PropTag tag, Restriction restriction) : base(Restriction.ResType.SubRestriction)
			{
				this.tag = tag;
				this.restriction = restriction;
			}

			internal unsafe SubRestriction(SRestriction* psres) : base(Restriction.ResType.SubRestriction)
			{
				this.tag = (PropTag)psres->union.resSub.ulSubObject;
				this.restriction = Restriction.Unmarshal(psres->union.resSub.lpRes);
			}

			public override int GetBytesToMarshal()
			{
				int num = SRestriction.SizeOf + 7 & -8;
				return num + (this.restriction.GetBytesToMarshal() + 7 & -8);
			}

			public override int GetBytesToMarshalNspi()
			{
				throw new NotSupportedException();
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				SRestriction* ptr = pExtra;
				pExtra += (IntPtr)(SRestriction.SizeOf + 7 & -8);
				psr->rt = 9;
				psr->union.resSub.ulSubObject = (int)this.tag;
				psr->union.resSub.lpRes = ptr;
				this.restriction.MarshalToNative(ptr, ref pExtra);
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				throw new NotSupportedException();
			}

			internal override void EnumerateSubRestrictions(Restriction.EnumRestrictionDelegate del, object ctx)
			{
				this.restriction.EnumerateRestriction(del, ctx);
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.SubRestriction subRestriction = (Restriction.SubRestriction)other;
				return this.tag == subRestriction.tag && this.restriction.IsEqualTo(subRestriction.restriction);
			}

			private PropTag tag;

			private Restriction restriction;
		}

		public class AttachmentRestriction : Restriction.SubRestriction
		{
			public AttachmentRestriction(Restriction restriction) : base(PropTag.MessageAttachments, restriction)
			{
			}

			internal unsafe AttachmentRestriction(SRestriction* psres) : base(psres)
			{
			}
		}

		public class RecipientRestriction : Restriction.SubRestriction
		{
			public RecipientRestriction(Restriction restriction) : base(PropTag.MessageRecipients, restriction)
			{
			}

			internal unsafe RecipientRestriction(SRestriction* psres) : base(psres)
			{
			}
		}

		public class CommentRestriction : Restriction
		{
			public PropValue[] Values
			{
				get
				{
					return this.propValues;
				}
				set
				{
					this.propValues = value;
				}
			}

			public Restriction Restriction
			{
				get
				{
					return this.restriction;
				}
				set
				{
					this.restriction = value;
				}
			}

			public CommentRestriction(Restriction restriction, PropValue[] propValues) : base(Restriction.ResType.Comment)
			{
				this.propValues = propValues;
				this.restriction = restriction;
			}

			internal unsafe CommentRestriction(SRestriction* psres) : base(Restriction.ResType.Comment)
			{
				this.restriction = Restriction.Unmarshal(psres->union.resComment.lpRes);
				this.propValues = new PropValue[psres->union.resComment.cValues];
				for (int i = 0; i < psres->union.resComment.cValues; i++)
				{
					this.propValues[i] = new PropValue(psres->union.resComment.lpProp + i);
				}
			}

			public override int GetBytesToMarshal()
			{
				int num = SRestriction.SizeOf + 7 & -8;
				if (this.propValues != null && this.propValues.Length > 0)
				{
					foreach (PropValue propValue in this.propValues)
					{
						num += (propValue.GetBytesToMarshal() + 7 & -8);
					}
				}
				if (this.restriction != null)
				{
					num += (this.restriction.GetBytesToMarshal() + 7 & -8);
				}
				return num;
			}

			public override int GetBytesToMarshalNspi()
			{
				throw new NotSupportedException();
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				psr->rt = 10;
				if (this.restriction != null)
				{
					SRestriction* ptr = pExtra;
					pExtra += (IntPtr)(SRestriction.SizeOf + 7 & -8);
					psr->union.resComment.lpRes = ptr;
					this.restriction.MarshalToNative(ptr, ref pExtra);
				}
				else
				{
					psr->union.resComment.lpRes = null;
				}
				if (this.propValues != null && this.propValues.Length > 0)
				{
					SPropValue* lpProp = pExtra;
					pExtra += (IntPtr)(Marshal.SizeOf(typeof(SPropValue)) * this.propValues.Length + 7 & -8);
					psr->union.resComment.cValues = this.propValues.Length;
					psr->union.resComment.lpProp = lpProp;
					foreach (PropValue propValue in this.propValues)
					{
						propValue.MarshalToNative(lpProp++, ref pExtra);
					}
					return;
				}
				psr->union.resComment.cValues = 0;
				psr->union.resComment.lpProp = null;
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				throw new NotSupportedException();
			}

			internal override void EnumerateSubRestrictions(Restriction.EnumRestrictionDelegate del, object ctx)
			{
				this.restriction.EnumerateRestriction(del, ctx);
			}

			internal override bool IsEqualTo(Restriction other)
			{
				if (!base.IsEqualTo(other))
				{
					return false;
				}
				Restriction.CommentRestriction commentRestriction = (Restriction.CommentRestriction)other;
				if (this.Values != null != (commentRestriction.Values != null))
				{
					return false;
				}
				if (this.Values != null)
				{
					if (commentRestriction.Values == null || commentRestriction.Values.Length != this.Values.Length)
					{
						return false;
					}
					for (int i = 0; i < this.Values.Length; i++)
					{
						if (!this.Values[i].IsEqualTo(commentRestriction.Values[i]))
						{
							return false;
						}
					}
				}
				else if (commentRestriction.Values != null)
				{
					return false;
				}
				return this.restriction.IsEqualTo(commentRestriction.restriction);
			}

			private PropValue[] propValues;

			private Restriction restriction;
		}

		public class TrueRestriction : Restriction
		{
			public TrueRestriction() : base(Restriction.ResType.True)
			{
			}

			internal unsafe TrueRestriction(SRestriction* psres) : base(Restriction.ResType.True)
			{
			}

			public override int GetBytesToMarshal()
			{
				return SRestriction.SizeOf + 7 & -8;
			}

			public override int GetBytesToMarshalNspi()
			{
				throw new NotSupportedException();
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				psr->rt = 131;
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				throw new NotSupportedException();
			}
		}

		public class FalseRestriction : Restriction
		{
			public FalseRestriction() : base(Restriction.ResType.False)
			{
			}

			internal unsafe FalseRestriction(SRestriction* psres) : base(Restriction.ResType.False)
			{
			}

			public override int GetBytesToMarshal()
			{
				return SRestriction.SizeOf + 7 & -8;
			}

			public override int GetBytesToMarshalNspi()
			{
				throw new NotSupportedException();
			}

			internal unsafe override void MarshalToNative(SRestriction* psr, ref byte* pExtra)
			{
				psr->rt = 132;
			}

			internal unsafe override void MarshalToNative(SNspiRestriction* psr, ref byte* pExtra)
			{
				throw new NotSupportedException();
			}
		}
	}
}
