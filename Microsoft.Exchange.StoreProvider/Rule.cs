using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Rule : IComparable
	{
		public int CompareTo(object obj)
		{
			Rule rule = obj as Rule;
			if (rule == null)
			{
				throw MapiExceptionHelper.ArgumentException("obj", "argument is null or it is not a Rule");
			}
			return this._ExecutionSequence - rule._ExecutionSequence;
		}

		public long ID
		{
			get
			{
				return this._ID;
			}
		}

		public byte[] IDx
		{
			get
			{
				return this._IDx;
			}
		}

		public int ExecutionSequence
		{
			get
			{
				return this._ExecutionSequence;
			}
			set
			{
				this._ExecutionSequence = value;
			}
		}

		public int Level
		{
			get
			{
				return this._Level;
			}
			set
			{
				this._Level = value;
			}
		}

		public RuleStateFlags StateFlags
		{
			get
			{
				return this._StateFlags;
			}
			set
			{
				this._StateFlags = value;
			}
		}

		public uint UserFlags
		{
			get
			{
				return this._UserFlags;
			}
			set
			{
				this._UserFlags = value;
			}
		}

		public Restriction Condition
		{
			get
			{
				return this._Condition;
			}
			set
			{
				this._Condition = value;
			}
		}

		public RuleAction[] Actions
		{
			get
			{
				return this._Actions;
			}
			set
			{
				this._Actions = value;
			}
		}

		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
			}
		}

		public string Provider
		{
			get
			{
				return this._Provider;
			}
			set
			{
				this._Provider = value;
			}
		}

		public byte[] ProviderData
		{
			get
			{
				return this._ProviderData;
			}
			set
			{
				this._ProviderData = value;
			}
		}

		public bool IsExtended
		{
			get
			{
				return this._IsExtended;
			}
			set
			{
				this._IsExtended = value;
			}
		}

		public PropValue[] ExtraProperties
		{
			get
			{
				return this._ExtraProperties;
			}
			set
			{
				this._ExtraProperties = value;
			}
		}

		public Rule()
		{
		}

		public RuleOperation Operation { get; set; }

		internal static PropTag[] GetUnmarshalColumns()
		{
			return Rule.UnmarshalColumns;
		}

		internal static PropTag[] GetUnmarshalExColumns()
		{
			return Rule.UnmarshalExColumns;
		}

		internal static Rule CreateRuleFromProperties(MapiFolder folder, Rule existingRule, ICollection<PropValue> properties)
		{
			Rule rule = existingRule ?? new Rule();
			List<PropValue> list = new List<PropValue>(properties.Count);
			foreach (PropValue item in properties)
			{
				PropTag propTag = item.PropTag;
				if (propTag <= PropTag.RuleCondition)
				{
					if (propTag <= PropTag.RuleSequence)
					{
						if (propTag != PropTag.RuleID)
						{
							if (propTag == PropTag.RuleSequence)
							{
								rule.ExecutionSequence = item.GetInt();
								continue;
							}
						}
						else
						{
							if (existingRule == null)
							{
								rule._ID = item.GetLong();
								rule._IDx = folder.MapiStore.CreateEntryId(folder.GetProp(PropTag.Fid).GetLong(), rule._ID);
								continue;
							}
							continue;
						}
					}
					else
					{
						if (propTag == PropTag.RuleState)
						{
							rule.StateFlags = (RuleStateFlags)item.GetInt();
							continue;
						}
						if (propTag == PropTag.RuleUserFlags)
						{
							rule.UserFlags = (uint)item.GetInt();
							continue;
						}
						if (propTag == PropTag.RuleCondition)
						{
							rule.Condition = (Restriction)item.Value;
							continue;
						}
					}
				}
				else if (propTag <= PropTag.RuleProvider)
				{
					if (propTag == PropTag.RuleActions)
					{
						rule.Actions = (RuleAction[])item.Value;
						continue;
					}
					if (propTag == PropTag.RuleProvider)
					{
						rule.Provider = item.GetString();
						continue;
					}
				}
				else
				{
					if (propTag == PropTag.RuleName)
					{
						rule.Name = item.GetString();
						continue;
					}
					if (propTag == PropTag.RuleLevel)
					{
						rule.Level = item.GetInt();
						continue;
					}
					if (propTag == PropTag.RuleProviderData)
					{
						rule.ProviderData = item.GetBytes();
						continue;
					}
				}
				list.Add(item);
			}
			rule.ExtraProperties = list.ToArray();
			return rule;
		}

		internal static bool IsPublicFolderRule(ICollection<PropValue> properties)
		{
			foreach (PropValue propValue in properties)
			{
				if (propValue.PropTag == Rule.PR_RULE_NAME)
				{
					return false;
				}
			}
			return true;
		}

		internal ICollection<PropValue> ToProperties(bool modifyExisting, bool classicFormat)
		{
			if (this._Actions == null)
			{
				this._Actions = Array<RuleAction>.Empty;
			}
			if (this._Actions.Length == 0 && (this._StateFlags & RuleStateFlags.ExitAfterExecution) == (RuleStateFlags)0)
			{
				throw MapiExceptionHelper.DataIntegrityException("Corrupt Rule - No Actions specified");
			}
			if (this._Name == null)
			{
				this._Name = string.Empty;
			}
			if (this._Provider == null || this._Provider.Length == 0)
			{
				this._Provider = string.Empty;
			}
			PropTag[] array = classicFormat ? Rule.UnmarshalColumns : Rule.UnmarshalExColumns;
			List<PropValue> list = new List<PropValue>(11);
			if (!modifyExisting)
			{
				this._ID = 0L;
				this._IDx = null;
			}
			else if (classicFormat)
			{
				list.Add(new PropValue(array[0], this._ID));
			}
			list.Add(new PropValue(array[1], this._ExecutionSequence));
			list.Add(new PropValue(array[2], this._Level));
			list.Add(new PropValue(array[3], this._Name));
			list.Add(new PropValue(array[4], this._Provider));
			if (this._ProviderData != null && this._ProviderData.Length > 0)
			{
				list.Add(new PropValue(array[5], this._ProviderData));
			}
			list.Add(new PropValue(array[6], (int)this._StateFlags));
			list.Add(new PropValue(array[7], (int)this._UserFlags));
			if (this._IsExtended)
			{
				list.Add(new PropValue(PropTag.MessageClass, "IPM.ExtendedRule.Message"));
			}
			else if (!classicFormat)
			{
				list.Add(new PropValue(PropTag.MessageClass, "IPM.Rule.Version2.Message"));
			}
			if (classicFormat)
			{
				list.Add(new PropValue(array[9], this.Condition));
				list.Add(new PropValue(array[8], this.Actions));
			}
			else if (this.ExtraProperties != null && this.ExtraProperties.Length > 0)
			{
				if (!this._IsExtended)
				{
					throw new MapiExceptionRuleFormat("ExtraProperties may not be set on non-extended rules");
				}
				for (int i = 0; i < this.ExtraProperties.Length; i++)
				{
					for (int j = 0; j < Rule.UnmarshalExColumns.Length; j++)
					{
						if (this.ExtraProperties[i].PropTag == Rule.UnmarshalExColumns[j])
						{
							throw new MapiExceptionRuleFormat("ExtraProperties may not contain standard rule properties");
						}
					}
				}
				list.AddRange(this.ExtraProperties);
			}
			return list;
		}

		private static byte[] GetPropertyAsStream(MapiMessage msg, PropTag tag)
		{
			BufferPool bufferPool;
			byte[] buffer = BufferPools.GetBuffer(98304, out bufferPool);
			byte[] result;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (MapiStream mapiStream = msg.OpenStream(tag, OpenPropertyFlags.DeferredErrors))
					{
						int num;
						do
						{
							num = mapiStream.Read(buffer, 0, buffer.Length);
							if (num > 0)
							{
								memoryStream.Write(buffer, 0, num);
							}
						}
						while (num == buffer.Length);
					}
					result = memoryStream.ToArray();
				}
			}
			catch (MapiExceptionNotFound)
			{
				result = null;
			}
			finally
			{
				if (bufferPool != null && buffer != null)
				{
					bufferPool.Release(buffer);
				}
			}
			return result;
		}

		private bool ReadColumn<TReturn, TParam>(Rule.RuleColumn ruleColumn, PropValue[] cols, Rule.PropertyTransform<TReturn, TParam> transform, ref TReturn destination, bool classicFormat, TReturn defaultValue)
		{
			if (!cols[(int)ruleColumn].IsError())
			{
				if (!classicFormat)
				{
					PropType propType;
					if (ruleColumn < (Rule.RuleColumn)Rule.UnmarshalExColumns.Length)
					{
						propType = Rule.UnmarshalExColumns[(int)ruleColumn].ValueType();
					}
					else
					{
						propType = (defaultValue as PropValue?).Value.PropTag.ValueType();
					}
					PropType propType2 = propType;
					if (propType2 != PropType.String)
					{
						if (propType2 == PropType.Binary)
						{
							if (cols[(int)ruleColumn].GetBytes().Length == 510)
							{
								return true;
							}
						}
					}
					else if (cols[(int)ruleColumn].GetString().Length == 255)
					{
						return true;
					}
				}
				if (transform != null)
				{
					destination = transform((TParam)((object)cols[(int)ruleColumn].Value));
				}
				else
				{
					destination = (TReturn)((object)cols[(int)ruleColumn].Value);
				}
				return false;
			}
			int errorValue = cols[(int)ruleColumn].GetErrorValue();
			if (!classicFormat && errorValue == -2147024882)
			{
				return true;
			}
			destination = defaultValue;
			return false;
		}

		internal Rule(PropValue[] cols, PropTag[] extraProps, MapiFolder mapiFolder, bool classicFormat)
		{
			bool[] array = new bool[Rule.UnmarshalExColumns.Length + ((extraProps != null) ? extraProps.Length : 0)];
			this.ReadColumn<uint, int>(Rule.RuleColumn.UserFlags, cols, (int param) => (uint)param, ref this._UserFlags, classicFormat, 0U);
			this.ReadColumn<int, int>(Rule.RuleColumn.Sequence, cols, null, ref this._ExecutionSequence, classicFormat, 0);
			this.ReadColumn<long, long>(Rule.RuleColumn.ShortID, cols, null, ref this._ID, classicFormat, 0L);
			this.ReadColumn<int, int>(Rule.RuleColumn.Level, cols, null, ref this._Level, classicFormat, 0);
			this.ReadColumn<RuleStateFlags, int>(Rule.RuleColumn.State, cols, null, ref this._StateFlags, classicFormat, RuleStateFlags.Error);
			array[3] = this.ReadColumn<string, string>(Rule.RuleColumn.Name, cols, null, ref this._Name, classicFormat, string.Empty);
			array[4] = this.ReadColumn<string, string>(Rule.RuleColumn.Provider, cols, null, ref this._Provider, classicFormat, string.Empty);
			array[5] = this.ReadColumn<byte[], byte[]>(Rule.RuleColumn.ProviderData, cols, null, ref this._ProviderData, classicFormat, null);
			if (classicFormat)
			{
				this.ReadColumn<RuleAction[], RuleAction[]>(Rule.RuleColumn.Actions, cols, null, ref this._Actions, classicFormat, Array<RuleAction>.Empty);
				this.ReadColumn<Restriction, Restriction>(Rule.RuleColumn.Condition, cols, null, ref this._Condition, classicFormat, null);
			}
			if (!classicFormat)
			{
				this.ReadColumn<bool, string>(Rule.RuleColumn.MessageClass, cols, (string param) => string.Compare(param, "IPM.ExtendedRule.Message", StringComparison.OrdinalIgnoreCase) == 0, ref this._IsExtended, classicFormat, !classicFormat);
				this.ReadColumn<byte[], byte[]>(Rule.RuleColumn.LongID, cols, null, ref this._IDx, classicFormat, null);
				array[8] = this.ReadColumn<RuleAction[], byte[]>(Rule.RuleColumn.Actions, cols, new Rule.PropertyTransform<RuleAction[], byte[]>(mapiFolder.DeserializeActions), ref this._Actions, classicFormat, Array<RuleAction>.Empty);
				array[9] = this.ReadColumn<Restriction, byte[]>(Rule.RuleColumn.Condition, cols, new Rule.PropertyTransform<Restriction, byte[]>(mapiFolder.DeserializeRestriction), ref this._Condition, classicFormat, null);
				if (extraProps != null && extraProps.Length > 0 && this._IsExtended)
				{
					this.ExtraProperties = new PropValue[extraProps.Length];
					int i;
					for (i = 0; i < extraProps.Length; i++)
					{
						array[i + Rule.UnmarshalExColumns.Length] = this.ReadColumn<PropValue, object>(i + (Rule.RuleColumn)Rule.UnmarshalExColumns.Length, cols, (object o) => new PropValue(extraProps[i], o), ref this.ExtraProperties[i], false, cols[i + Rule.UnmarshalExColumns.Length]);
					}
				}
				else
				{
					this.ExtraProperties = Array<PropValue>.Empty;
				}
				bool flag = false;
				for (int l = 0; l < array.Length; l++)
				{
					if (array[l])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					using (MapiMessage mapiMessage = (MapiMessage)mapiFolder.OpenEntry(this._IDx, OpenEntryFlags.DeferredErrors))
					{
						for (int j = 0; j < Rule.UnmarshalExColumns.Length; j++)
						{
							if (array[j])
							{
								byte[] propertyAsStream = Rule.GetPropertyAsStream(mapiMessage, Rule.UnmarshalExColumns[j]);
								switch (j)
								{
								case 3:
									this._Name = Encoding.Unicode.GetString(propertyAsStream, 0, propertyAsStream.Length);
									break;
								case 4:
									this._Provider = Encoding.Unicode.GetString(propertyAsStream, 0, propertyAsStream.Length);
									break;
								case 5:
									this._ProviderData = propertyAsStream;
									break;
								case 8:
									this._Actions = mapiFolder.DeserializeActions(propertyAsStream);
									break;
								case 9:
									this._Condition = mapiFolder.DeserializeRestriction(propertyAsStream);
									break;
								}
							}
						}
						if (extraProps != null)
						{
							for (int k = 0; k < extraProps.Length; k++)
							{
								if (array[k + Rule.UnmarshalExColumns.Length])
								{
									byte[] propertyAsStream2 = Rule.GetPropertyAsStream(mapiMessage, extraProps[k]);
									PropType propType = extraProps[k].ValueType();
									if (propType != PropType.String)
									{
										if (propType != PropType.Binary)
										{
											throw new NotSupportedException();
										}
										this.ExtraProperties[k] = new PropValue(extraProps[k], propertyAsStream2);
									}
									else
									{
										this.ExtraProperties[k] = new PropValue(extraProps[k], Encoding.Unicode.GetString(propertyAsStream2, 0, propertyAsStream2.Length));
									}
								}
							}
						}
					}
				}
			}
			if (this._IsExtended)
			{
				this.ScrubMoveCopyActions();
			}
		}

		private void ScrubMoveCopyActions()
		{
			for (int i = 0; i < this._Actions.Length; i++)
			{
				switch (this._Actions[i].ActionType)
				{
				case RuleAction.Type.OP_MOVE:
				{
					byte[] folderEntryID = ((RuleAction.MoveCopy)this._Actions[i]).FolderEntryID;
					uint userFlags = this._Actions[i].UserFlags;
					this._Actions[i] = new RuleAction.InMailboxMove(folderEntryID);
					this._Actions[i].UserFlags = userFlags;
					break;
				}
				case RuleAction.Type.OP_COPY:
				{
					byte[] folderEntryID = ((RuleAction.MoveCopy)this._Actions[i]).FolderEntryID;
					uint userFlags = this._Actions[i].UserFlags;
					this._Actions[i] = new RuleAction.InMailboxCopy(folderEntryID);
					this._Actions[i].UserFlags = userFlags;
					break;
				}
				}
			}
		}

		private static void SerializeNullableBlob(BinarySerializer serializer, byte[] value)
		{
			serializer.Write((value != null) ? 1 : 0);
			if (value != null)
			{
				serializer.Write(value);
			}
		}

		private static void SerializeNullableString(BinarySerializer serializer, string value)
		{
			serializer.Write((value != null) ? 1 : 0);
			if (value != null)
			{
				serializer.Write(value);
			}
		}

		private static void SerializeNullablePropValues(BinarySerializer serializer, PropValue[] values)
		{
			serializer.Write((values != null) ? 1 : 0);
			if (values != null)
			{
				serializer.Write(values);
			}
		}

		private static byte[] DeserializeNullableBlob(BinaryDeserializer deserializer)
		{
			if (deserializer.ReadInt() == 0)
			{
				return null;
			}
			return deserializer.ReadBytes();
		}

		private static string DeserializeNullableString(BinaryDeserializer deserializer)
		{
			if (deserializer.ReadInt() == 0)
			{
				return null;
			}
			return deserializer.ReadString();
		}

		private static PropValue[] DeserializeNullablePropValues(BinaryDeserializer deserializer)
		{
			if (deserializer.ReadInt() == 0)
			{
				return null;
			}
			return deserializer.ReadPropValues();
		}

		internal void SerializeRule(BinarySerializer serializer, MapiFolder folder)
		{
			Rule.SerializeNullableBlob(serializer, this._IDx);
			serializer.Write((ulong)this._ID);
			serializer.Write(this._ExecutionSequence);
			serializer.Write(this._Level);
			serializer.Write((int)this._StateFlags);
			serializer.Write((int)this._UserFlags);
			Rule.SerializeNullableBlob(serializer, folder.SerializeRestriction(this._Condition));
			Rule.SerializeNullableBlob(serializer, folder.SerializeActions(this._Actions));
			Rule.SerializeNullableString(serializer, this._Name);
			Rule.SerializeNullableString(serializer, this._Provider);
			Rule.SerializeNullableBlob(serializer, this._ProviderData);
			serializer.Write(this._IsExtended ? 1 : 0);
			Rule.SerializeNullablePropValues(serializer, this._ExtraProperties);
		}

		internal static Rule DeserializeRule(BinaryDeserializer deserializer, MapiFolder folder)
		{
			return new Rule
			{
				_IDx = Rule.DeserializeNullableBlob(deserializer),
				_ID = (long)deserializer.ReadUInt64(),
				_ExecutionSequence = deserializer.ReadInt(),
				_Level = deserializer.ReadInt(),
				_StateFlags = (RuleStateFlags)deserializer.ReadInt(),
				_UserFlags = (uint)deserializer.ReadInt(),
				_Condition = folder.DeserializeRestriction(Rule.DeserializeNullableBlob(deserializer)),
				_Actions = folder.DeserializeActions(Rule.DeserializeNullableBlob(deserializer)),
				_Name = Rule.DeserializeNullableString(deserializer),
				_Provider = Rule.DeserializeNullableString(deserializer),
				_ProviderData = Rule.DeserializeNullableBlob(deserializer),
				_IsExtended = (deserializer.ReadInt() != 0),
				_ExtraProperties = Rule.DeserializeNullablePropValues(deserializer)
			};
		}

		internal const string ClassicRuleMessageClass = "IPM.Rule.Message";

		internal const string MiddleTierRuleMessageClass = "IPM.Rule.Version2.Message";

		internal const string ExtendedRuleMessageClass = "IPM.ExtendedRule.Message";

		private const int pidSpecialMin = 26224;

		private const int pidStoreNonTransMin = 3648;

		private const int pidExchangeNonXmitReservedMin = 26080;

		internal static Restriction ClassicRule = new Restriction.ContentRestriction(PropTag.MessageClass, "IPM.Rule.Message", ContentFlags.IgnoreCase);

		internal static Restriction MiddleTierRule = new Restriction.ContentRestriction(PropTag.MessageClass, "IPM.Rule.Version2.Message", ContentFlags.IgnoreCase);

		internal static Restriction ExtendedRule = new Restriction.ContentRestriction(PropTag.MessageClass, "IPM.ExtendedRule.Message", ContentFlags.IgnoreCase);

		internal static Restriction NewRuleMessages = Restriction.Or(new Restriction[]
		{
			Rule.MiddleTierRule,
			Rule.ExtendedRule
		});

		internal static Restriction AllRuleMessages = Restriction.Or(new Restriction[]
		{
			Rule.MiddleTierRule,
			Rule.ExtendedRule,
			Rule.ClassicRule
		});

		internal byte[] _IDx;

		internal long _ID;

		private int _ExecutionSequence;

		private int _Level;

		private RuleStateFlags _StateFlags;

		private uint _UserFlags;

		private Restriction _Condition;

		private RuleAction[] _Actions;

		private string _Name;

		private string _Provider;

		private byte[] _ProviderData;

		private bool _IsExtended;

		private PropValue[] _ExtraProperties;

		internal static readonly PropTag PR_RULE_ID = PropTagHelper.PropTagFromIdAndType(26228, PropType.Long);

		internal static readonly PropTag PR_RULE_SEQUENCE = PropTagHelper.PropTagFromIdAndType(26230, PropType.Int);

		internal static readonly PropTag PR_RULE_STATE = PropTagHelper.PropTagFromIdAndType(26231, PropType.Int);

		internal static readonly PropTag PR_RULE_USER_FLAGS = PropTagHelper.PropTagFromIdAndType(26232, PropType.Int);

		internal static readonly PropTag PR_RULE_CONDITION = PropTagHelper.PropTagFromIdAndType(26233, PropType.Restriction);

		internal static readonly PropTag PR_RULE_ACTIONS = PropTagHelper.PropTagFromIdAndType(26240, PropType.Actions);

		internal static readonly PropTag PR_RULE_PROVIDER = PropTagHelper.PropTagFromIdAndType(26241, PropType.String);

		internal static readonly PropTag PR_RULE_NAME = PropTagHelper.PropTagFromIdAndType(26242, PropType.String);

		internal static readonly PropTag PR_RULE_LEVEL = PropTagHelper.PropTagFromIdAndType(26243, PropType.Int);

		internal static readonly PropTag PR_RULE_PROVIDER_DATA = PropTagHelper.PropTagFromIdAndType(26244, PropType.Binary);

		public static readonly PropTag PR_EX_RULE_ACTIONS = PropTagHelper.PropTagFromIdAndType(3737, PropType.Binary);

		internal static readonly PropTag PR_EX_RULE_CONDITION = PropTagHelper.PropTagFromIdAndType(3738, PropType.Binary);

		internal static readonly PropTag PR_EX_RULE_ID = PropTag.Mid;

		internal static readonly PropTag PR_EX_RULE_IDx = PropTag.EntryId;

		internal static readonly PropTag PR_EX_RULE_SEQUENCE = PropTagHelper.PropTagFromIdAndType(26099, PropType.Int);

		internal static readonly PropTag PR_EX_RULE_STATE = PropTagHelper.PropTagFromIdAndType(26089, PropType.Int);

		internal static readonly PropTag PR_EX_RULE_USER_FLAGS = PropTagHelper.PropTagFromIdAndType(26090, PropType.Int);

		internal static readonly PropTag PR_EX_RULE_PROVIDER = PropTagHelper.PropTagFromIdAndType(26091, PropType.String);

		internal static readonly PropTag PR_EX_RULE_NAME = PropTagHelper.PropTagFromIdAndType(26092, PropType.String);

		internal static readonly PropTag PR_EX_RULE_LEVEL = PropTagHelper.PropTagFromIdAndType(26093, PropType.Int);

		internal static readonly PropTag PR_EX_RULE_PROVIDER_DATA = PropTagHelper.PropTagFromIdAndType(26094, PropType.Binary);

		private static readonly PropTag[] UnmarshalColumns = new PropTag[]
		{
			Rule.PR_RULE_ID,
			Rule.PR_RULE_SEQUENCE,
			Rule.PR_RULE_LEVEL,
			Rule.PR_RULE_NAME,
			Rule.PR_RULE_PROVIDER,
			Rule.PR_RULE_PROVIDER_DATA,
			Rule.PR_RULE_STATE,
			Rule.PR_RULE_USER_FLAGS,
			Rule.PR_RULE_ACTIONS,
			Rule.PR_RULE_CONDITION
		};

		internal static readonly PropTag[] UnmarshalExColumns = new PropTag[]
		{
			Rule.PR_EX_RULE_ID,
			Rule.PR_EX_RULE_SEQUENCE,
			Rule.PR_EX_RULE_LEVEL,
			Rule.PR_EX_RULE_NAME,
			Rule.PR_EX_RULE_PROVIDER,
			Rule.PR_EX_RULE_PROVIDER_DATA,
			Rule.PR_EX_RULE_STATE,
			Rule.PR_EX_RULE_USER_FLAGS,
			Rule.PR_EX_RULE_ACTIONS,
			Rule.PR_EX_RULE_CONDITION,
			Rule.PR_EX_RULE_IDx,
			PropTag.MessageClass
		};

		internal static readonly ICollection<PropTag> RuleMsgPreDeleteProps = new List<PropTag>(new PropTag[]
		{
			Rule.PR_EX_RULE_CONDITION,
			Rule.PR_EX_RULE_ACTIONS,
			Rule.PR_EX_RULE_PROVIDER_DATA
		});

		internal static readonly PropTag[] ExRuleGetProps = new PropTag[]
		{
			Rule.PR_EX_RULE_CONDITION,
			Rule.PR_EX_RULE_ACTIONS
		};

		internal enum RuleColumn
		{
			ShortID,
			Sequence,
			Level,
			Name,
			Provider,
			ProviderData,
			State,
			UserFlags,
			Actions,
			Condition,
			LongID,
			MessageClass
		}

		private delegate TReturn PropertyTransform<TReturn, TParam>(TParam propValue);
	}
}
