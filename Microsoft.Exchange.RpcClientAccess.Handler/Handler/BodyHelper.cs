using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal class BodyHelper
	{
		public BodyHelper(ICoreItem coreItem, ICorePropertyBag corePropertyBag, Encoding string8Encoding, Func<BodyReadConfiguration, Stream> getBodyConversionStreamCallback)
		{
			this.coreItem = coreItem;
			this.corePropertyBag = corePropertyBag;
			this.string8Encoding = string8Encoding;
			this.getBodyConversionStreamCallback = getBodyConversionStreamCallback;
			this.bodyState = BodyHelper.BodyState.None;
			this.alternateBodyState = BodyHelper.BodyState.None;
			this.IsOpeningStream = false;
			this.isRtfInSync = corePropertyBag.GetValueAsNullable<bool>(BodyHelper.RtfInSync);
			this.includeTheBodyPropertyBeingOpeningWhenEvaluatingIfAnyBodyPropertyIsDirty = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).RpcClientAccess.IncludeTheBodyPropertyBeingOpeningWhenEvaluatingIfAnyBodyPropertyIsDirty.Enabled;
		}

		public bool IsBodyChanged
		{
			get
			{
				return this.isBodyChanged;
			}
		}

		public static bool IsBodyProperty(PropertyDefinition propertyDefinition)
		{
			foreach (StorePropertyDefinition storePropertyDefinition in BodyHelper.bodyProperties)
			{
				if (storePropertyDefinition.Equals(propertyDefinition))
				{
					return true;
				}
			}
			return false;
		}

		public static bool ContainsBodyProperty(PropertyTag[] propertyTags)
		{
			if (propertyTags != null)
			{
				foreach (PropertyTag propertyTag in propertyTags)
				{
					if (propertyTag.IsBodyProperty())
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void RemoveBodyProperties(ref PropertyTag[] propertyTags)
		{
			if (propertyTags != null && BodyHelper.ContainsBodyProperty(propertyTags))
			{
				List<PropertyTag> list = new List<PropertyTag>(propertyTags.Length);
				foreach (PropertyTag propertyTag in propertyTags)
				{
					if (!propertyTag.IsBodyProperty())
					{
						list.Add(propertyTag);
					}
				}
				propertyTags = ((list.Count > 0) ? list.ToArray() : Array<PropertyTag>.Empty);
			}
		}

		public static bool IsFixupNeeded(PropertyTag propertyTag)
		{
			return propertyTag.IsBodyProperty() || propertyTag.PropertyId.IsRtfInSync();
		}

		public bool IsOpeningStream { get; private set; }

		public static StorePropertyDefinition GetBodyPropertyDefinition(PropertyId propertyId)
		{
			if (propertyId == PropertyId.Body)
			{
				return BodyHelper.TextBody;
			}
			if (propertyId == PropertyId.RtfCompressed)
			{
				return BodyHelper.RtfCompressed;
			}
			if (propertyId != PropertyId.Html)
			{
				throw new InvalidOperationException("Property is not a body property.");
			}
			return BodyHelper.HtmlBody;
		}

		public bool IsNativeBodyProperty(PropertyTag propertyTag)
		{
			if (!propertyTag.IsBodyProperty())
			{
				return false;
			}
			switch (this.GetCurrentNativeBody())
			{
			case NativeBodyInfo.PlainTextBody:
				return propertyTag.PropertyId == PropertyId.Body;
			case NativeBodyInfo.RtfCompressedBody:
				return propertyTag.PropertyId == PropertyId.RtfCompressed;
			case NativeBodyInfo.HtmlBody:
				return propertyTag.PropertyId == PropertyId.Html;
			default:
				return false;
			}
		}

		public void SetProperty(PropertyDefinition propertyDefinition, object value)
		{
			if (!this.CanWrite(propertyDefinition))
			{
				return;
			}
			if (propertyDefinition == BodyHelper.RtfInSync && value is bool)
			{
				this.isRtfInSync = new bool?((bool)value);
			}
			this.OnBeforeWrite(ref propertyDefinition);
			this.corePropertyBag.SetProperty(propertyDefinition, value);
		}

		public void DeleteProperty(PropertyDefinition propertyDefinition)
		{
			if (BodyHelper.IsBodyProperty(propertyDefinition) || propertyDefinition == BodyHelper.RtfInSync)
			{
				this.bodyState = BodyHelper.BodyState.None;
				this.isBodyChanged = true;
			}
			if ((propertyDefinition == BodyHelper.RtfCompressed && this.alternateBodyState == BodyHelper.BodyState.RtfCompressed) || (propertyDefinition == BodyHelper.HtmlBody && this.alternateBodyState == BodyHelper.BodyState.Html))
			{
				propertyDefinition = BodyHelper.AlternateBody;
				this.alternateBodyState = BodyHelper.BodyState.None;
			}
			this.corePropertyBag.Delete(propertyDefinition);
		}

		public Stream OpenStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			if (openMode == PropertyOpenMode.Create)
			{
				if (!this.CanWrite(propertyDefinition))
				{
					return Stream.Null;
				}
			}
			else if (openMode == PropertyOpenMode.Modify && !this.CanWrite(propertyDefinition))
			{
				openMode = PropertyOpenMode.ReadOnly;
			}
			if (this.coreItem.IsDirty && BodyHelper.IsBodyProperty(propertyDefinition) && this.IsAnyBodyPropertyDirty(propertyDefinition))
			{
				this.IsOpeningStream = true;
				try
				{
					this.coreItem.Flush(SaveMode.FailOnAnyConflict);
					this.corePropertyBag.Load(CoreObjectSchema.AllPropertiesOnStore);
				}
				finally
				{
					this.IsOpeningStream = false;
				}
				this.coreItem.Body.ResetBodyFormat();
			}
			if (openMode != PropertyOpenMode.ReadOnly)
			{
				this.OnBeforeWrite(ref propertyDefinition);
			}
			else if ((propertyDefinition == BodyHelper.RtfCompressed && this.alternateBodyState == BodyHelper.BodyState.RtfCompressed) || (propertyDefinition == BodyHelper.HtmlBody && this.alternateBodyState == BodyHelper.BodyState.Html))
			{
				propertyDefinition = BodyHelper.AlternateBody;
			}
			return this.corePropertyBag.OpenPropertyStream(propertyDefinition, openMode);
		}

		public object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			if ((propertyDefinition == BodyHelper.RtfCompressed && this.alternateBodyState == BodyHelper.BodyState.RtfCompressed) || (propertyDefinition == BodyHelper.HtmlBody && this.alternateBodyState == BodyHelper.BodyState.Html))
			{
				propertyDefinition = BodyHelper.AlternateBody;
			}
			if (propertyDefinition == CoreItemSchema.NativeBodyInfo)
			{
				return (int)this.GetCurrentNativeBody();
			}
			return this.corePropertyBag.TryGetProperty(propertyDefinition);
		}

		public void OnBeforeWrite(PropertyTag propertyTag)
		{
			if (propertyTag.IsHtml())
			{
				this.bodyState = BodyHelper.BodyState.Html;
				this.isBodyChanged = true;
				return;
			}
			if (propertyTag.IsRtfCompressed())
			{
				if (this.bodyState == BodyHelper.BodyState.None || this.bodyState == BodyHelper.BodyState.PlainText)
				{
					this.bodyState = BodyHelper.BodyState.RtfCompressed;
					this.isBodyChanged = true;
					return;
				}
			}
			else if (propertyTag.IsBody() && this.bodyState == BodyHelper.BodyState.None)
			{
				this.bodyState = BodyHelper.BodyState.PlainText;
				this.isBodyChanged = true;
			}
		}

		public void Reset()
		{
			this.bodyState = BodyHelper.BodyState.None;
			this.isBodyChanged = false;
		}

		public void InitiatePropertyEvaluation()
		{
			this.nativeBodyInfo = this.GetCurrentNativeBody();
		}

		public void FixupProperty(ref PropertyValue propertyValue, FixupMapping mapping)
		{
			BodyHelper.BodyPropertyMapper.Fixup(ref propertyValue, this.nativeBodyInfo, mapping);
		}

		public void FixupProperties(PropertyValue[] values, FixupMapping mapping)
		{
			this.InitiatePropertyEvaluation();
			int num;
			if (BodyHelper.IsSingleBodyPropertyRequested(values, out num))
			{
				if (this.DoesABodyPropertyExistOnMessage())
				{
					if (!this.IsNativeBodyProperty(values[num].PropertyTag))
					{
						values[num] = new PropertyValue(new PropertyTag(values[num].PropertyTag.PropertyId, PropertyType.Error), (ErrorCode)2147942414U);
					}
					bool flag = values[num].PropertyTag.PropertyId == PropertyId.RtfCompressed;
					for (int i = 0; i < values.Length; i++)
					{
						if (values[i].PropertyTag.PropertyId.IsRtfInSync())
						{
							if (flag)
							{
								values[i] = BodyHelper.True_RtfInSync;
							}
							else
							{
								values[i] = ((this.nativeBodyInfo == NativeBodyInfo.RtfCompressedBody) ? BodyHelper.True_RtfInSync : BodyHelper.False_RtfInSync);
							}
						}
					}
					return;
				}
			}
			else if (BodyHelper.IsOnlyPlainTextAndRtfPropertyRequested(values) && this.IsNativeBodyProperty(PropertyTag.Html))
			{
				if (this.DoesABodyPropertyExistOnMessage())
				{
					for (int j = 0; j < values.Length; j++)
					{
						PropertyId propertyId = values[j].PropertyTag.PropertyId;
						if (propertyId.IsRtfInSync())
						{
							values[j] = BodyHelper.True_RtfInSync;
						}
						else if (propertyId.IsBody() || propertyId.IsRtfCompressed())
						{
							values[j] = new PropertyValue(new PropertyTag(propertyId, PropertyType.Error), (ErrorCode)2147942414U);
						}
					}
					return;
				}
			}
			else
			{
				for (int k = 0; k < values.Length; k++)
				{
					this.FixupProperty(ref values[k], mapping);
				}
			}
		}

		public bool IsConversionNeeded(PropertyTag propertyTag)
		{
			return propertyTag.IsBodyProperty() && !this.IsBodyPropertyOnMessage(propertyTag) && this.DoesABodyPropertyExistOnMessage() && !this.IsNativeBodyProperty(propertyTag);
		}

		public Stream GetConversionStream(PropertyTag propertyTag)
		{
			if (!this.IsConversionNeeded(propertyTag))
			{
				return null;
			}
			PropertyId propertyId = propertyTag.PropertyId;
			BodyFormat targetFormat;
			if (propertyId != PropertyId.Body)
			{
				if (propertyId != PropertyId.RtfCompressed)
				{
					if (propertyId != PropertyId.Html)
					{
						return null;
					}
					targetFormat = BodyFormat.TextHtml;
				}
				else
				{
					targetFormat = BodyFormat.ApplicationRtf;
				}
			}
			else
			{
				targetFormat = BodyFormat.TextPlain;
			}
			int codePage = this.string8Encoding.CodePage;
			if (propertyTag.PropertyType == PropertyType.Unicode)
			{
				codePage = Encoding.Unicode.CodePage;
			}
			BodyReadConfiguration arg = new BodyReadConfiguration(targetFormat, ConvertUtils.GetCharsetFromCodepage(codePage), true);
			return this.getBodyConversionStreamCallback(arg);
		}

		public void PrepareForSave()
		{
			if ((this.alternateBodyState == BodyHelper.BodyState.Html && this.bodyState == BodyHelper.BodyState.RtfCompressed && this.isRtfInSync != true) || (this.alternateBodyState == BodyHelper.BodyState.RtfCompressed && this.bodyState == BodyHelper.BodyState.Html && this.isRtfInSync == true))
			{
				PropertyDefinition propertyDefinition = (this.alternateBodyState == BodyHelper.BodyState.Html) ? BodyHelper.HtmlBody : BodyHelper.RtfCompressed;
				using (Stream stream = this.corePropertyBag.OpenPropertyStream(BodyHelper.AlternateBody, PropertyOpenMode.ReadOnly))
				{
					using (Stream stream2 = this.corePropertyBag.OpenPropertyStream(propertyDefinition, PropertyOpenMode.Create))
					{
						Util.StreamHandler.CopyStreamData(stream, stream2, null, 0, 65536);
					}
				}
				this.bodyState = this.alternateBodyState;
				this.isBodyChanged = true;
			}
			if (this.alternateBodyState != BodyHelper.BodyState.None)
			{
				this.coreItem.PropertyBag.Delete(BodyHelper.AlternateBody);
				this.alternateBodyState = BodyHelper.BodyState.None;
			}
		}

		public void UpdateBodyPreviewIfNeeded(Body body)
		{
			if (!this.IsBodyChanged)
			{
				return;
			}
			switch (this.GetCurrentNativeBody())
			{
			case NativeBodyInfo.PlainTextBody:
				this.corePropertyBag.Delete(BodyHelper.RtfCompressed);
				this.corePropertyBag.SetProperty(BodyHelper.RtfInSync, BodyHelper.RtfInSync_False);
				this.corePropertyBag.Delete(BodyHelper.HtmlBody);
				break;
			case NativeBodyInfo.RtfCompressedBody:
				this.corePropertyBag.Delete(BodyHelper.TextBody);
				this.corePropertyBag.Delete(BodyHelper.HtmlBody);
				this.corePropertyBag.SetProperty(BodyHelper.RtfInSync, BodyHelper.RtfInSync_True);
				break;
			case NativeBodyInfo.HtmlBody:
				this.corePropertyBag.Delete(BodyHelper.TextBody);
				this.corePropertyBag.Delete(BodyHelper.RtfCompressed);
				this.corePropertyBag.SetProperty(BodyHelper.RtfInSync, BodyHelper.RtfInSync_False);
				break;
			}
			body.NotifyPreviewNeedsUpdated();
		}

		private static bool IsSingleBodyPropertyRequested(PropertyValue[] values, out int propertyIndex)
		{
			propertyIndex = -1;
			int num = 0;
			int num2 = 0;
			while (num2 < values.Length && num < 2)
			{
				if (values[num2].PropertyTag.IsBodyProperty())
				{
					num++;
					propertyIndex = num2;
				}
				num2++;
			}
			return num == 1;
		}

		private static bool IsOnlyPlainTextAndRtfPropertyRequested(PropertyValue[] values)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < values.Length; i++)
			{
				PropertyId propertyId = values[i].PropertyTag.PropertyId;
				if (propertyId.IsHtml())
				{
					return false;
				}
				if (propertyId.IsRtfCompressed())
				{
					flag2 = true;
				}
				else if (propertyId.IsBody())
				{
					flag = true;
				}
			}
			return flag && flag2;
		}

		private void OnBeforeWrite(ref PropertyDefinition propertyDefinition)
		{
			if (propertyDefinition != BodyHelper.HtmlBody)
			{
				if (propertyDefinition == BodyHelper.RtfCompressed)
				{
					if (this.bodyState == BodyHelper.BodyState.Html)
					{
						this.alternateBodyState = BodyHelper.BodyState.RtfCompressed;
						propertyDefinition = BodyHelper.AlternateBody;
					}
					if (this.bodyState == BodyHelper.BodyState.None || this.bodyState == BodyHelper.BodyState.PlainText)
					{
						this.bodyState = BodyHelper.BodyState.RtfCompressed;
						this.isBodyChanged = true;
						return;
					}
				}
				else if (propertyDefinition == BodyHelper.TextBody && this.bodyState == BodyHelper.BodyState.None)
				{
					this.bodyState = BodyHelper.BodyState.PlainText;
					this.isBodyChanged = true;
				}
				return;
			}
			if (this.bodyState == BodyHelper.BodyState.RtfCompressed)
			{
				this.alternateBodyState = BodyHelper.BodyState.Html;
				propertyDefinition = BodyHelper.AlternateBody;
				return;
			}
			this.bodyState = BodyHelper.BodyState.Html;
			this.isBodyChanged = true;
		}

		private bool CanWrite(PropertyDefinition propertyDefinition)
		{
			return (propertyDefinition != BodyHelper.TextBody || (this.bodyState != BodyHelper.BodyState.Html && this.bodyState != BodyHelper.BodyState.RtfCompressed)) && propertyDefinition != BodyHelper.AlternateBody;
		}

		private bool IsBodyPropertyOnMessage(StorePropertyDefinition propertyDefinition)
		{
			object propertyValue = this.corePropertyBag.TryGetProperty(propertyDefinition);
			return !PropertyError.IsPropertyNotFound(propertyValue);
		}

		private bool IsAnyBodyPropertyDirty(PropertyDefinition propertyDefinition)
		{
			foreach (StorePropertyDefinition storePropertyDefinition in BodyHelper.bodyProperties)
			{
				if (this.corePropertyBag.IsPropertyDirty(storePropertyDefinition) && (!storePropertyDefinition.Equals(propertyDefinition) || this.includeTheBodyPropertyBeingOpeningWhenEvaluatingIfAnyBodyPropertyIsDirty))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsBodyPropertyOnMessage(PropertyTag propertyTag)
		{
			return this.IsBodyPropertyOnMessage(BodyHelper.GetBodyPropertyDefinition(propertyTag.PropertyId));
		}

		private bool DoesABodyPropertyExistOnMessage()
		{
			foreach (StorePropertyDefinition propertyDefinition in BodyHelper.bodyProperties)
			{
				if (this.IsBodyPropertyOnMessage(propertyDefinition))
				{
					return true;
				}
			}
			return false;
		}

		private NativeBodyInfo GetCurrentNativeBody()
		{
			switch (this.bodyState)
			{
			case BodyHelper.BodyState.PlainText:
				return NativeBodyInfo.PlainTextBody;
			case BodyHelper.BodyState.RtfCompressed:
				if (this.alternateBodyState == BodyHelper.BodyState.Html && !this.isRtfInSync.GetValueOrDefault(false))
				{
					return NativeBodyInfo.HtmlBody;
				}
				return NativeBodyInfo.RtfCompressedBody;
			case BodyHelper.BodyState.Html:
				if (this.alternateBodyState == BodyHelper.BodyState.RtfCompressed && this.isRtfInSync.GetValueOrDefault(false))
				{
					return NativeBodyInfo.RtfCompressedBody;
				}
				return NativeBodyInfo.HtmlBody;
			default:
			{
				NativeBodyInfo valueOrDefault = this.corePropertyBag.GetValueOrDefault<NativeBodyInfo>(CoreItemSchema.NativeBodyInfo);
				if (valueOrDefault != NativeBodyInfo.Undefined)
				{
					return valueOrDefault;
				}
				if (this.IsBodyPropertyOnMessage(PropertyTag.Html))
				{
					return NativeBodyInfo.HtmlBody;
				}
				if (this.IsBodyPropertyOnMessage(PropertyTag.RtfCompressed))
				{
					return NativeBodyInfo.RtfCompressedBody;
				}
				if (this.IsBodyPropertyOnMessage(PropertyTag.Body))
				{
					return NativeBodyInfo.PlainTextBody;
				}
				return NativeBodyInfo.Undefined;
			}
			}
		}

		public static readonly PropertyTag[] AllBodyPropertiesUnicode = new PropertyTag[]
		{
			PropertyTag.Body,
			PropertyTag.RtfCompressed,
			PropertyTag.RtfInSync,
			PropertyTag.Html
		};

		public static readonly PropertyTag[] AllBodyPropertiesString8 = new PropertyTag[]
		{
			new PropertyTag(PropertyTag.Body.PropertyId, PropertyType.String8),
			PropertyTag.RtfCompressed,
			PropertyTag.RtfInSync,
			PropertyTag.Html
		};

		private static readonly StorePropertyDefinition TextBody = ItemSchema.TextBody;

		private static readonly StorePropertyDefinition HtmlBody = PropertyTagPropertyDefinition.CreateCustom("Html", PropertyTag.Html);

		private static readonly StorePropertyDefinition RtfCompressed = PropertyTagPropertyDefinition.CreateCustom("RtfCompressed", PropertyTag.RtfCompressed);

		private static readonly StorePropertyDefinition AlternateBody = PropertyTagPropertyDefinition.CreateCustom("AlternateBody", PropertyTag.AlternateBestBody);

		private static readonly StorePropertyDefinition RtfInSync = PropertyTagPropertyDefinition.CreateCustom("RtfInSync", PropertyTag.RtfInSync);

		private static readonly object RtfInSync_True = true;

		private static readonly object RtfInSync_False = false;

		private static readonly StorePropertyDefinition[] bodyProperties = new StorePropertyDefinition[]
		{
			BodyHelper.TextBody,
			BodyHelper.RtfCompressed,
			BodyHelper.HtmlBody
		};

		private static readonly PropertyId[] bodyPropertyIds = new PropertyId[]
		{
			PropertyId.Body,
			PropertyId.RtfCompressed,
			PropertyId.Html
		};

		private static readonly PropertyValue True_RtfInSync = new PropertyValue(PropertyTag.RtfInSync, true);

		private static readonly PropertyValue False_RtfInSync = new PropertyValue(PropertyTag.RtfInSync, false);

		private readonly ICoreItem coreItem;

		private readonly ICorePropertyBag corePropertyBag;

		private readonly Encoding string8Encoding;

		private readonly Func<BodyReadConfiguration, Stream> getBodyConversionStreamCallback;

		private readonly bool includeTheBodyPropertyBeingOpeningWhenEvaluatingIfAnyBodyPropertyIsDirty;

		private BodyHelper.BodyState bodyState;

		private BodyHelper.BodyState alternateBodyState;

		private NativeBodyInfo nativeBodyInfo;

		private bool isBodyChanged;

		private bool? isRtfInSync;

		private enum BodyState
		{
			None,
			PlainText,
			RtfCompressed,
			Html
		}

		private static class BodyPropertyMapper
		{
			public static void Fixup(ref PropertyValue propertyValue, NativeBodyInfo nativeBodyInfo, FixupMapping mapping)
			{
				PropertyId propertyId = propertyValue.PropertyTag.PropertyId;
				int num = 0;
				if (BodyHelper.BodyPropertyMapper.TryGetBodyIndex(propertyId, out num) && ((propertyValue.IsError && (ErrorCode)propertyValue.Value == (ErrorCode)2147746063U) || propertyId.IsRtfInSync()))
				{
					int nativeBodyIndex = BodyHelper.BodyPropertyMapper.GetNativeBodyIndex(nativeBodyInfo);
					PropertyValue? propertyValue2 = BodyHelper.BodyPropertyMapper.GetBodyPropertyMap(mapping)[num][nativeBodyIndex];
					if (propertyValue2 != null)
					{
						propertyValue = propertyValue2.Value;
					}
				}
			}

			private static PropertyValue?[][] GetBodyPropertyMap(FixupMapping mapping)
			{
				switch (mapping)
				{
				case FixupMapping.GetProperties:
					return BodyHelper.BodyPropertyMapper.bodyFixupValues_GetProperty;
				case FixupMapping.FastTransfer:
					return BodyHelper.BodyPropertyMapper.bodyFixupValues_FastTransfer;
				case FixupMapping.FastTransferCopyProperties:
					return BodyHelper.BodyPropertyMapper.bodyFixupValues_FastTransferCopyProperties;
				default:
					throw new InvalidOperationException(string.Format("Unknown FixupMapping: {0}", mapping));
				}
			}

			private static bool TryGetBodyIndex(PropertyId propertyId, out int bodyIndex)
			{
				bodyIndex = 0;
				if (propertyId <= PropertyId.Body)
				{
					if (propertyId == PropertyId.RtfInSync)
					{
						bodyIndex = 3;
						return true;
					}
					if (propertyId == PropertyId.Body)
					{
						bodyIndex = 0;
						return true;
					}
				}
				else
				{
					if (propertyId == PropertyId.RtfCompressed)
					{
						bodyIndex = 1;
						return true;
					}
					if (propertyId == PropertyId.Html)
					{
						bodyIndex = 2;
						return true;
					}
				}
				return false;
			}

			private static int GetNativeBodyIndex(NativeBodyInfo nativeBodyInfo)
			{
				switch (nativeBodyInfo)
				{
				case NativeBodyInfo.PlainTextBody:
					return 0;
				case NativeBodyInfo.RtfCompressedBody:
					return 1;
				case NativeBodyInfo.HtmlBody:
					return 2;
				case NativeBodyInfo.ClearSignedBody:
					return 3;
				default:
					return 4;
				}
			}

			private static readonly PropertyValue True_RtfInSync = new PropertyValue(PropertyTag.RtfInSync, true);

			private static readonly PropertyValue False_RtfInSync = new PropertyValue(PropertyTag.RtfInSync, false);

			private static readonly PropertyValue NotEnoughMemory_Rtf = PropertyValue.CreateNotEnoughMemory(PropertyId.RtfCompressed);

			private static readonly PropertyValue NotFound_Rtf = PropertyValue.CreateNotFound(PropertyId.RtfCompressed);

			private static readonly PropertyValue NotEnoughMemory_Body = PropertyValue.CreateNotEnoughMemory(PropertyId.Body);

			private static readonly PropertyValue NotFound_Body = PropertyValue.CreateNotFound(PropertyId.Body);

			private static readonly PropertyValue NotEnoughMemory_Html = PropertyValue.CreateNotEnoughMemory(PropertyId.Html);

			private static readonly PropertyValue NotFound_Html = PropertyValue.CreateNotFound(PropertyId.Html);

			private static readonly PropertyValue? Existing = null;

			private static readonly PropertyValue?[][] bodyFixupValues_GetProperty = new PropertyValue?[][]
			{
				new PropertyValue?[]
				{
					BodyHelper.BodyPropertyMapper.Existing,
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Body),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Body),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Body),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Body)
				},
				new PropertyValue?[]
				{
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Rtf),
					BodyHelper.BodyPropertyMapper.Existing,
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Rtf),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Rtf),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Rtf)
				},
				new PropertyValue?[]
				{
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html),
					BodyHelper.BodyPropertyMapper.Existing,
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html)
				},
				new PropertyValue?[]
				{
					new PropertyValue?(BodyHelper.BodyPropertyMapper.False_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.True_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.False_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.False_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.False_RtfInSync)
				}
			};

			private static readonly PropertyValue?[][] bodyFixupValues_FastTransfer = new PropertyValue?[][]
			{
				new PropertyValue?[]
				{
					BodyHelper.BodyPropertyMapper.Existing,
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Body),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Body),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Body),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Body)
				},
				new PropertyValue?[]
				{
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Rtf),
					BodyHelper.BodyPropertyMapper.Existing,
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Rtf),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Rtf),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Rtf)
				},
				new PropertyValue?[]
				{
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html),
					BodyHelper.BodyPropertyMapper.Existing,
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html)
				},
				new PropertyValue?[]
				{
					new PropertyValue?(BodyHelper.BodyPropertyMapper.True_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.True_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.True_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.False_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.False_RtfInSync)
				}
			};

			private static readonly PropertyValue?[][] bodyFixupValues_FastTransferCopyProperties = new PropertyValue?[][]
			{
				new PropertyValue?[]
				{
					BodyHelper.BodyPropertyMapper.Existing,
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Body),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Body),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Body),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Body)
				},
				new PropertyValue?[]
				{
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Rtf),
					BodyHelper.BodyPropertyMapper.Existing,
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Rtf),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Rtf),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Rtf)
				},
				new PropertyValue?[]
				{
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Html),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotEnoughMemory_Html),
					BodyHelper.BodyPropertyMapper.Existing,
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.NotFound_Html)
				},
				new PropertyValue?[]
				{
					new PropertyValue?(BodyHelper.BodyPropertyMapper.True_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.True_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.True_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.False_RtfInSync),
					new PropertyValue?(BodyHelper.BodyPropertyMapper.False_RtfInSync)
				}
			};
		}
	}
}
