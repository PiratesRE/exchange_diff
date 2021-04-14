using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Get", "FolderRestriction")]
	public sealed class GetFolderRestriction : GetFolderObjectBase<RestrictionRow>
	{
		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new GetTaskBaseModuleFactory();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			IEnumerable<RestrictionRow> restrictions = this.GetRestrictions();
			foreach (RestrictionRow dataObject in restrictions)
			{
				this.WriteResult(dataObject);
			}
			TaskLogger.LogExit();
		}

		private IEnumerable<RestrictionRow> GetRestrictions()
		{
			PropTag[] props = new PropTag[]
			{
				PropTag.DisplayName,
				PropTag.LCIDRestriction,
				PropTag.ContentCount,
				PropTag.ContentUnread,
				PropTag.ViewAccessTime,
				PropTag.ViewRestriction,
				PropTag.EntryId
			};
			PropValue[][] restrictiontable = null;
			base.MapiSession.InvokeWithWrappedException(delegate()
			{
				restrictiontable = this.MapiSession.Administration.GetRestrictionTable(this.TargetDatabase.IsPublicFolderDatabase ? MdbFlags.Public : MdbFlags.Private, this.TargetDatabase.Guid, this.MailboxGuid, (byte[])this.FolderEntryId, props);
			}, Strings.ErrorGetRestrictionTableForFolderFailed(base.TargetDatabase.Name, base.FolderEntryId.ToString()), null);
			foreach (PropValue[] pva in restrictiontable)
			{
				RestrictionRow row = new RestrictionRow(base.FolderEntryId);
				if (PropTag.DisplayName == pva[0].PropTag)
				{
					row.DisplayName = pva[0].GetString();
				}
				if (PropTag.LCIDRestriction == pva[1].PropTag)
				{
					row.CultureInfo = new CultureInfo(pva[1].GetInt());
				}
				if (PropTag.ContentCount == pva[2].PropTag)
				{
					row.ContentCount = pva[2].GetLong();
				}
				if (PropTag.ContentUnread == pva[3].PropTag)
				{
					row.ContentUnread = pva[3].GetLong();
				}
				if (PropTag.ViewAccessTime == pva[4].PropTag)
				{
					row.ViewAccessTime = new DateTime?(pva[4].GetDateTime());
				}
				if (PropTag.ViewRestriction == pva[5].PropTag)
				{
					row.Restriction = GetFolderRestriction.DumpRestriction((Restriction)pva[5].Value);
				}
				if (PropTag.EntryId == pva[6].PropTag)
				{
					row.MapiEntryID = new MapiEntryId(pva[6].GetBytes());
				}
				yield return row;
			}
			yield break;
		}

		private static string DumpRestriction(Restriction restriction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			GetFolderRestriction.DumpRestriction(stringBuilder, restriction);
			return stringBuilder.ToString();
		}

		private static void DumpRestriction(StringBuilder sb, Restriction restriction)
		{
			if (restriction == null)
			{
				return;
			}
			sb.Append("(");
			Restriction.ResType type = restriction.Type;
			switch (type)
			{
			case Restriction.ResType.And:
			{
				Restriction.AndRestriction andRestriction = (Restriction.AndRestriction)restriction;
				sb.Append("AND ");
				foreach (Restriction restriction2 in andRestriction.Restrictions)
				{
					GetFolderRestriction.DumpRestriction(sb, restriction2);
				}
				break;
			}
			case Restriction.ResType.Or:
			{
				Restriction.OrRestriction orRestriction = (Restriction.OrRestriction)restriction;
				sb.Append("OR ");
				foreach (Restriction restriction3 in orRestriction.Restrictions)
				{
					GetFolderRestriction.DumpRestriction(sb, restriction3);
				}
				break;
			}
			case Restriction.ResType.Not:
			{
				Restriction.NotRestriction notRestriction = (Restriction.NotRestriction)restriction;
				sb.Append("NOT ");
				GetFolderRestriction.DumpRestriction(sb, notRestriction.Restriction);
				break;
			}
			case Restriction.ResType.Content:
			{
				Restriction.ContentRestriction contentRestriction = (Restriction.ContentRestriction)restriction;
				sb.AppendFormat("CONTENT flags:{0} mv:{1} propTag:{2:X} propValue:(", contentRestriction.Flags, contentRestriction.MultiValued, contentRestriction.PropTag);
				GetFolderRestriction.DumpPropValue(sb, contentRestriction.PropValue);
				sb.Append(")");
				break;
			}
			case Restriction.ResType.Property:
			{
				Restriction.PropertyRestriction propertyRestriction = (Restriction.PropertyRestriction)restriction;
				sb.AppendFormat("PROPERTY Op:{0}, propTag:{1:X} propValue:", propertyRestriction.Op, propertyRestriction.PropTag);
				GetFolderRestriction.DumpPropValue(sb, propertyRestriction.PropValue);
				break;
			}
			case Restriction.ResType.CompareProps:
			{
				Restriction.ComparePropertyRestriction comparePropertyRestriction = (Restriction.ComparePropertyRestriction)restriction;
				sb.AppendFormat("COMPPROPS Op:{0} propTag1:{1:X} propTag2:{2:X}", comparePropertyRestriction.Op, comparePropertyRestriction.TagLeft, comparePropertyRestriction.TagRight);
				break;
			}
			case Restriction.ResType.BitMask:
			{
				Restriction.BitMaskRestriction bitMaskRestriction = (Restriction.BitMaskRestriction)restriction;
				sb.AppendFormat("BITMASK Bmr:{0} Mask:{1:X} propTag:{2:X}", bitMaskRestriction.Bmr, bitMaskRestriction.Mask, bitMaskRestriction.Tag);
				break;
			}
			case Restriction.ResType.Size:
			{
				Restriction.SizeRestriction sizeRestriction = (Restriction.SizeRestriction)restriction;
				sb.AppendFormat("SIZE propTag:{0:X} Op:{1} size:{2}", sizeRestriction.Tag, sizeRestriction.Op, sizeRestriction.Size);
				break;
			}
			case Restriction.ResType.Exist:
			{
				Restriction.ExistRestriction existRestriction = (Restriction.ExistRestriction)restriction;
				sb.AppendFormat("EXIST propTag:{0:X}", existRestriction.Tag);
				break;
			}
			case Restriction.ResType.SubRestriction:
			{
				Restriction.SubRestriction subRestriction = (Restriction.SubRestriction)restriction;
				sb.Append("SUBRESTRICTION ");
				GetFolderRestriction.DumpRestriction(sb, subRestriction.Restriction);
				break;
			}
			case Restriction.ResType.Comment:
			{
				Restriction.CommentRestriction commentRestriction = (Restriction.CommentRestriction)restriction;
				sb.AppendFormat("COMMENT values:{0} ", commentRestriction.Values.Length);
				foreach (PropValue v in commentRestriction.Values)
				{
					GetFolderRestriction.DumpPropValue(sb, v);
				}
				GetFolderRestriction.DumpRestriction(sb, commentRestriction.Restriction);
				break;
			}
			case Restriction.ResType.Count:
			{
				Restriction.CountRestriction countRestriction = (Restriction.CountRestriction)restriction;
				sb.AppendFormat("COUNT {0} ", countRestriction.Count);
				GetFolderRestriction.DumpRestriction(sb, countRestriction.Restriction);
				break;
			}
			default:
				switch (type)
				{
				case Restriction.ResType.True:
					sb.Append("TRUE");
					break;
				case Restriction.ResType.False:
					sb.Append("FALSE");
					break;
				default:
					sb.AppendFormat("<UNKNOWN RESTYPE:{0}>", restriction.Type);
					break;
				}
				break;
			}
			sb.Append(")");
		}

		internal static void DumpPropValue(StringBuilder sb, PropValue v)
		{
			if (PropType.MultiValueFlag == (v.PropType & PropType.MultiValueFlag))
			{
				sb.AppendFormat(" propTag:{0:X} propType:{1} propValue:", v.PropTag, v.PropType);
				PropType propType = v.PropType;
				if (propType <= PropType.StringArray)
				{
					switch (propType)
					{
					case PropType.ShortArray:
						GetFolderRestriction.DumpArray<short>(sb, (short[])v.RawValue);
						goto IL_1D5;
					case PropType.IntArray:
						GetFolderRestriction.DumpArray<int>(sb, (int[])v.RawValue);
						goto IL_1D5;
					case PropType.FloatArray:
						GetFolderRestriction.DumpArray<float>(sb, (float[])v.RawValue);
						goto IL_1D5;
					case PropType.DoubleArray:
						GetFolderRestriction.DumpArray<double>(sb, (double[])v.RawValue);
						goto IL_1D5;
					case PropType.CurrencyArray:
						break;
					case PropType.AppTimeArray:
						GetFolderRestriction.DumpArray<double>(sb, (double[])v.RawValue);
						goto IL_1D5;
					case (PropType)4104:
					case (PropType)4105:
					case (PropType)4106:
					case (PropType)4107:
					case (PropType)4108:
						goto IL_19C;
					case PropType.ObjectArray:
						sb.Append("<object>");
						goto IL_1D5;
					default:
						if (propType != PropType.LongArray)
						{
							switch (propType)
							{
							case PropType.AnsiStringArray:
							case PropType.StringArray:
								GetFolderRestriction.DumpArray<string>(sb, (string[])v.RawValue);
								goto IL_1D5;
							default:
								goto IL_19C;
							}
						}
						break;
					}
				}
				else if (propType != PropType.SysTimeArray)
				{
					if (propType == PropType.GuidArray)
					{
						GetFolderRestriction.DumpArray<Guid>(sb, (Guid[])v.RawValue);
						goto IL_1D5;
					}
					if (propType != PropType.BinaryArray)
					{
						goto IL_19C;
					}
					GetFolderRestriction.DumpArray<byte>(sb, (byte[])v.RawValue);
					goto IL_1D5;
				}
				GetFolderRestriction.DumpArray<long>(sb, (long[])v.RawValue);
				goto IL_1D5;
				IL_19C:
				sb.Append("<unknown propType>");
			}
			else
			{
				sb.AppendFormat("propTag:{0:X} propType:{1} propValue:{2}", v.PropTag, v.PropType, v.RawValue);
			}
			IL_1D5:
			sb.Append(" ");
		}

		internal static void DumpArray<T>(StringBuilder sb, T[] array)
		{
			sb.Append("[");
			bool flag = true;
			foreach (T t in array)
			{
				if (!flag)
				{
					sb.Append(", ");
				}
				sb.AppendFormat("{0}", t);
				flag = false;
			}
			sb.Append("]");
		}
	}
}
