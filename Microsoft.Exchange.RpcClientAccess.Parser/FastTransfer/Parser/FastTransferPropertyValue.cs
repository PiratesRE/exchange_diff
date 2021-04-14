using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class FastTransferPropertyValue
	{
		internal static FastTransferStateMachine Serialize(FastTransferDownloadContext context, PropertyValue propertyValue)
		{
			return new FastTransferStateMachine(FastTransferPropertyValue.DownloadImpl.Serialize(context, propertyValue));
		}

		internal static FastTransferStateMachine Serialize(FastTransferDownloadContext context, IPropertyBag propertyBag, AnnotatedPropertyValue annotatedPropertyValue)
		{
			return new FastTransferStateMachine(FastTransferPropertyValue.DownloadImpl.Serialize(context, propertyBag, annotatedPropertyValue));
		}

		internal static FastTransferStateMachine DeserializeInto(FastTransferUploadContext context, IPropertyBag propertyBag)
		{
			return new FastTransferStateMachine(FastTransferPropertyValue.UploadImpl.Deserialize(context, propertyBag));
		}

		internal static FastTransferStateMachine SkipPropertyIfExists(FastTransferUploadContext context, PropertyTag propertyTag)
		{
			return new FastTransferStateMachine(FastTransferPropertyValue.UploadImpl.SkipPropertyIfExists(context, propertyTag));
		}

		internal static void CheckVariableSizePropertyType(PropertyType propertyType)
		{
			if (propertyType <= PropertyType.Unicode)
			{
				if (propertyType == PropertyType.Object)
				{
					return;
				}
				switch (propertyType)
				{
				case PropertyType.String8:
				case PropertyType.Unicode:
					return;
				}
			}
			else if (propertyType == PropertyType.ServerId || propertyType == PropertyType.Binary)
			{
				return;
			}
			throw new NotSupportedException();
		}

		private static int GetNullTerminatorSize(FastTransferDownloadContext context, PropertyType propertyType)
		{
			if (!PropertyTag.IsStringPropertyType(propertyType))
			{
				return 0;
			}
			bool flag = (context != null) ? context.UseCpidOrUnicode : (propertyType == PropertyType.Unicode);
			if (flag)
			{
				return 2;
			}
			return 1;
		}

		private const int UnicodeNullTerminatorSize = 2;

		private const int String8NullTerminatorSize = 1;

		internal static class DownloadImpl
		{
			public static IEnumerator<FastTransferStateMachine?> Serialize(FastTransferDownloadContext context, PropertyValue propertyValue)
			{
				if (propertyValue.IsError || propertyValue.PropertyTag.IsNamedProperty)
				{
					throw new ArgumentException("Only ptag properties with valid values can be written out without a property bag");
				}
				AnnotatedPropertyValue annotatedPropertyValue = new AnnotatedPropertyValue(propertyValue.PropertyTag, propertyValue, null);
				return FastTransferPropertyValue.DownloadImpl.Serialize(context, null, annotatedPropertyValue);
			}

			public static IEnumerator<FastTransferStateMachine?> Serialize(FastTransferDownloadContext context, IPropertyBag propertyBag, AnnotatedPropertyValue annotatedPropertyValue)
			{
				return context.StateMachineFactory.Serialize(context, propertyBag, annotatedPropertyValue);
			}

			internal static IEnumerator<FastTransferStateMachine?> Serialize_CreateDisplayClass(object instance, FastTransferDownloadContext context, IPropertyBag propertyBag, AnnotatedPropertyValue annotatedPropertyValue)
			{
				PropertyValue propertyValue = annotatedPropertyValue.PropertyValue;
				NamedProperty namedProperty = annotatedPropertyValue.NamedProperty;
				PropertyTag propertyTagToSerialize = annotatedPropertyValue.PropertyTag;
				if (propertyTagToSerialize.PropertyType == PropertyType.Unspecified)
				{
					throw new NotSupportedException(string.Format("PT_Unspecified property type download is not supported. Property = {0}.", propertyTagToSerialize));
				}
				if (!propertyValue.IsError)
				{
					FastTransferPropertyValue.DownloadImpl.WritePropertyTag(context, propertyValue.PropertyTag, namedProperty);
					yield return null;
					if (propertyValue.PropertyTag.IsMultiValuedProperty)
					{
						Array multiValues = (Array)propertyValue.Value;
						context.DataInterface.WriteLength((uint)multiValues.Length);
						yield return null;
						foreach (object value in multiValues)
						{
							yield return new FastTransferStateMachine?(new FastTransferStateMachine(FastTransferPropertyValue.DownloadImpl.SerializeElementProperty(context, new PropertyValue(new PropertyTag(propertyValue.PropertyTag.PropertyId, propertyValue.PropertyTag.ElementPropertyType), value))));
						}
					}
					else
					{
						yield return new FastTransferStateMachine?(new FastTransferStateMachine(FastTransferPropertyValue.DownloadImpl.SerializeElementProperty(context, propertyValue)));
					}
				}
				else if (context.SendPropertyErrors)
				{
					FastTransferPropertyValue.DownloadImpl.WritePropertyTag(context, propertyValue.PropertyTag, namedProperty);
					yield return new FastTransferStateMachine?(new FastTransferStateMachine(FastTransferPropertyValue.DownloadImpl.SerializeElementProperty(context, propertyValue)));
				}
				else
				{
					ErrorCode errorCode = (ErrorCode)propertyValue.Value;
					if (errorCode == (ErrorCode)2147942414U || (errorCode == (ErrorCode)2147746075U && propertyTagToSerialize.PropertyType == PropertyType.Object))
					{
						FastTransferPropertyValue.CheckVariableSizePropertyType(propertyTagToSerialize.PropertyType);
						FastTransferPropertyValue.DownloadImpl.WritePropertyTag(context, propertyTagToSerialize, namedProperty);
						Stream propertyReadStream = FastTransferPropertyValue.DownloadImpl.ConvertPropertyStreamIfNeeded(context, propertyTagToSerialize, propertyBag.GetPropertyStream(propertyTagToSerialize));
						yield return new FastTransferStateMachine?(new FastTransferStateMachine(FastTransferPropertyValue.DownloadImpl.SerializeVariableSizeProperty(context, propertyTagToSerialize, propertyReadStream)));
					}
					else if ((errorCode != (ErrorCode)2147746564U || propertyTagToSerialize.PropertyType != PropertyType.Binary) && (errorCode != (ErrorCode)2147746075U || propertyTagToSerialize.PropertyType != PropertyType.SysTime) && !FastTransferPropertyValue.DownloadImpl.ShouldIgnoreError(context, errorCode))
					{
						throw new RopExecutionException(string.Format("Attempted to download a property with an error value. Property = {0}. Value = {1}.", propertyTagToSerialize, propertyValue), errorCode);
					}
				}
				yield break;
			}

			private static Stream ConvertPropertyStreamIfNeeded(FastTransferDownloadContext context, PropertyTag propertyTag, Stream innerStream)
			{
				if (!context.UseCpidOrUnicode && propertyTag.IsStringProperty)
				{
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						disposeGuard.Add<Stream>(innerStream);
						EncodedStream encodedStream = new EncodedStream(innerStream, context.String8Encoding, context.ResourceTracker);
						disposeGuard.Add<EncodedStream>(encodedStream);
						disposeGuard.Success();
						return encodedStream;
					}
					return innerStream;
				}
				return innerStream;
			}

			private static IEnumerator<FastTransferStateMachine?> SerializeElementProperty(FastTransferDownloadContext context, PropertyValue propertyValue)
			{
				if (propertyValue.PropertyTag.EstimatedValueSize > 0)
				{
					return FastTransferPropertyValue.DownloadImpl.SerializeFixedSizeProperty(context, propertyValue);
				}
				if (propertyValue.PropertyTag.EstimatedValueSize == 0)
				{
					Stream propertyReadStream = FastTransferPropertyValue.DownloadImpl.ConvertPropertyStreamIfNeeded(context, propertyValue.PropertyTag, MemoryPropertyBag.WrapPropertyReadStream(propertyValue));
					return FastTransferPropertyValue.DownloadImpl.SerializeVariableSizeProperty(context, propertyValue.PropertyTag, propertyReadStream);
				}
				throw new NotSupportedException(string.Format("Property type is not supported. PropertyTag = {0}.", propertyValue.PropertyTag));
			}

			private static IEnumerator<FastTransferStateMachine?> SerializeFixedSizeProperty(FastTransferDownloadContext context, PropertyValue propertyValue)
			{
				return context.StateMachineFactory.SerializeFixedSize(context, propertyValue);
			}

			internal static IEnumerator<FastTransferStateMachine?> SerializeFixedSizeProperty_CreateDisplayClass(object instance, FastTransferDownloadContext context, PropertyValue propertyValue)
			{
				context.DataInterface.SerializeFixedSizeValue(propertyValue);
				yield break;
			}

			private static IEnumerator<FastTransferStateMachine?> SerializeVariableSizeProperty(FastTransferDownloadContext context, PropertyTag propertyTag, Stream propertyReadStream)
			{
				return context.StateMachineFactory.SerializeVariableSize(context, propertyTag, propertyReadStream);
			}

			internal static IEnumerator<FastTransferStateMachine?> SerializeVariableSizeProperty_CreateDisplayClass(object instance, FastTransferDownloadContext context, PropertyTag propertyTag, Stream propertyReadStream)
			{
				if (propertyReadStream == null)
				{
					Feature.Stubbed(91071, "Ignore large non-streamable store-computed properties");
					propertyReadStream = new MemoryStream();
				}
				int nullTerminatorSize;
				try
				{
					nullTerminatorSize = FastTransferPropertyValue.GetNullTerminatorSize(context, propertyTag.PropertyType);
					context.DataInterface.WriteLength((uint)(propertyReadStream.Length + (long)nullTerminatorSize));
					yield return null;
					while (propertyReadStream.Position < propertyReadStream.Length)
					{
						int bytesToRead = (int)(propertyReadStream.Length - propertyReadStream.Position);
						context.DataInterface.CopyFrom(propertyReadStream, bytesToRead);
						yield return null;
					}
				}
				finally
				{
					propertyReadStream.Dispose();
				}
				int nullTerminatorBytesWriten = 0;
				while (nullTerminatorBytesWriten < nullTerminatorSize)
				{
					int bytesToRead2 = Math.Min(context.DataInterface.AvailableBufferSize, nullTerminatorSize - nullTerminatorBytesWriten);
					context.DataInterface.SerializeVariableSizeValue(FastTransferPropertyValue.DownloadImpl.NullTerminator, 0, bytesToRead2);
					nullTerminatorBytesWriten += bytesToRead2;
					yield return null;
				}
				yield break;
			}

			private static void WritePropertyTag(FastTransferDownloadContext context, PropertyTag propertyTag, NamedProperty namedProperty)
			{
				context.DataInterface.WritePropertyInfo(context, FastTransferPropertyValue.DownloadImpl.ChangeToWrongTypes(propertyTag), namedProperty);
			}

			private static bool ShouldIgnoreError(FastTransferDownloadContext context, ErrorCode errorCode)
			{
				if (errorCode == (ErrorCode)2147746063U || errorCode == (ErrorCode)2147942405U)
				{
					return true;
				}
				if (errorCode != (ErrorCode)2147942414U)
				{
					return false;
				}
				ExAssert.RetailAssert(false, "NotEnoughMemory should've been handled through streaming in Serialize()");
				return true;
			}

			private static PropertyTag ChangeToWrongTypes(PropertyTag propertyTag)
			{
				if (propertyTag.PropertyId == FastTransferIcsState.IdsetGiven.PropertyId)
				{
					return new PropertyTag(FastTransferIcsState.IdsetGiven.PropertyId, PropertyType.Int32);
				}
				return propertyTag;
			}

			private static readonly byte[] NullTerminator = new byte[2];
		}

		internal static class UploadImpl
		{
			public static IEnumerator<FastTransferStateMachine?> Deserialize(FastTransferUploadContext context, IPropertyBag propertyBag)
			{
				return context.StateMachineFactory.Deserialize(context, propertyBag);
			}

			internal static IEnumerator<FastTransferStateMachine?> Deserialize_CreateDisplayClass(object unused, FastTransferUploadContext context, IPropertyBag propertyBag)
			{
				NamedProperty namedProperty;
				int codePage;
				PropertyTag propertyTag = FastTransferPropertyValue.UploadImpl.ChangeFromWrongTypes(context.DataInterface.ReadPropertyInfo(out namedProperty, out codePage));
				if (namedProperty != null)
				{
					if (propertyBag is SingleMemberPropertyBag || propertyBag is SingleMemberMultiValuePropertyBag)
					{
						throw new RopExecutionException(string.Format("Unable to upload named property to property bag without session {0}", namedProperty), ErrorCode.FxUnexpectedMarker);
					}
					if (!propertyBag.Session.TryResolveFromNamedProperty(namedProperty, ref propertyTag))
					{
						throw new RopExecutionException(string.Format("Unable to resolve named property {0}", namedProperty), (ErrorCode)2147942487U);
					}
				}
				yield return null;
				if (propertyTag.IsMultiValuedProperty)
				{
					int elementCount = context.DataInterface.ReadLength(32768);
					if (elementCount > 0)
					{
						yield return null;
					}
					PropertyTag multiValuePropertyTag = propertyTag;
					if (propertyTag.ElementPropertyType == PropertyType.String8)
					{
						multiValuePropertyTag = propertyTag.ChangeElementPropertyType(PropertyType.Unicode);
					}
					SingleMemberMultiValuePropertyBag multiValuePropertyBag = new SingleMemberMultiValuePropertyBag(multiValuePropertyTag, elementCount, propertyBag);
					for (int elementIndex = 0; elementIndex < elementCount; elementIndex++)
					{
						multiValuePropertyBag.ElementIndex = elementIndex;
						yield return new FastTransferStateMachine?(new FastTransferStateMachine(FastTransferPropertyValue.UploadImpl.DeserializeElementPropertyValue(context, new PropertyTag(propertyTag.PropertyId, propertyTag.ElementPropertyType), codePage, multiValuePropertyBag)));
					}
				}
				else
				{
					yield return new FastTransferStateMachine?(new FastTransferStateMachine(FastTransferPropertyValue.UploadImpl.DeserializeElementPropertyValue(context, propertyTag, codePage, propertyBag)));
				}
				yield break;
			}

			private static IEnumerator<FastTransferStateMachine?> DeserializeElementPropertyValue(FastTransferUploadContext context, PropertyTag propertyTag, int codePage, IPropertyBag propertyBag)
			{
				if (propertyTag.EstimatedValueSize > 0)
				{
					PropertyValue property = context.DataInterface.ReadAndParseFixedSizeValue(propertyTag);
					propertyBag.SetProperty(property);
					return null;
				}
				if (propertyTag.EstimatedValueSize == 0)
				{
					return context.StateMachineFactory.DeserializeVariableSizeProperty(context, propertyBag, propertyTag, codePage);
				}
				throw new BufferParseException(string.Format("Property type is not supported. PropertyTag = {0}.", propertyTag));
			}

			internal static IEnumerator<FastTransferStateMachine?> DeserializeVariableSizeProperty_CreateDisplayClass(object unused, FastTransferUploadContext context, IPropertyBag propertyBag, PropertyTag propertyTag, int codePage)
			{
				FastTransferPropertyValue.CheckVariableSizePropertyType(propertyTag.PropertyType);
				int bytesToConsume = context.DataInterface.ReadLength(1073741824);
				if (bytesToConsume > 0)
				{
					yield return null;
				}
				int bytesToTrim = FastTransferPropertyValue.GetNullTerminatorSize(null, propertyTag.PropertyType);
				PropertyTag streamPropertyTag = propertyTag;
				bool convertingToUnicode = false;
				if (propertyTag.ElementPropertyType == PropertyType.String8)
				{
					streamPropertyTag = propertyTag.ChangeElementPropertyType(PropertyType.Unicode);
					convertingToUnicode = true;
				}
				EncodedStream encodedStream = null;
				Stream propertyWriteStream = null;
				context.AllowEndOfBufferActions(false);
				try
				{
					if (bytesToConsume > 0)
					{
						Encoding encoding = null;
						Stream stream = null;
						bool attemptedToOpenStream = false;
						byte[] last2Bytes = null;
						if (streamPropertyTag != propertyTag)
						{
							if (codePage == 4095)
							{
								encoding = context.String8Encoding;
							}
							else if ((ushort)codePage == 1201)
							{
								encoding = String8Encodings.ReducedUnicode;
							}
							else if (!String8Encodings.TryGetEncoding(codePage, out encoding))
							{
								throw new BufferParseException(string.Format("Failed to get encoding for code page {0}", codePage));
							}
						}
						else if (propertyTag.PropertyType == PropertyType.Unicode)
						{
							encoding = Encoding.Unicode;
						}
						int bytesRemaining = bytesToConsume;
						while (bytesRemaining > 0)
						{
							ArraySegment<byte> chunk = context.DataInterface.ReadVariableSizeValue(bytesRemaining);
							if (chunk.Count == 0)
							{
								throw new BufferParseException("Failed to read the remainder of variable-size property bytes");
							}
							bytesRemaining -= chunk.Count;
							int bytesToWrite = chunk.Count;
							if (bytesRemaining == 0 && !attemptedToOpenStream && propertyTag.PropertyType != PropertyType.Object)
							{
								if (encoding == null)
								{
									byte[] array = new byte[chunk.Count];
									Array.Copy(chunk.Array, chunk.Offset, array, 0, chunk.Count);
									propertyBag.SetProperty(new PropertyValue(propertyTag, array));
								}
								else
								{
									if (chunk.Array[chunk.Offset + chunk.Count - 1] == 0 && (bytesToTrim == 1 || chunk.Array[chunk.Offset + chunk.Count - 2] == 0))
									{
										bytesToWrite -= bytesToTrim;
									}
									propertyBag.SetProperty(new PropertyValue(streamPropertyTag, encoding.GetString(chunk.Array, chunk.Offset, bytesToWrite)));
								}
							}
							else
							{
								if (!attemptedToOpenStream)
								{
									propertyWriteStream = propertyBag.SetPropertyStream(streamPropertyTag, (long)(convertingToUnicode ? (bytesToConsume * 2) : bytesToConsume));
									attemptedToOpenStream = true;
									stream = propertyWriteStream;
									if (propertyWriteStream != null && streamPropertyTag != propertyTag)
									{
										encodedStream = new EncodedStream(propertyWriteStream, encoding, context.ResourceTracker);
										stream = encodedStream;
									}
								}
								if (stream != null)
								{
									if (bytesToTrim > 0 && bytesToTrim <= bytesToConsume)
									{
										if (bytesToTrim == 2 && bytesRemaining == 1)
										{
											last2Bytes = new byte[2];
											last2Bytes[0] = chunk.Array[chunk.Offset + chunk.Count - 1];
											bytesToWrite--;
										}
										else if (bytesRemaining == 0)
										{
											if (last2Bytes != null)
											{
												last2Bytes[1] = chunk.Array[chunk.Offset];
												if (!Array.TrueForAll<byte>(last2Bytes, (byte b) => b == 0))
												{
													stream.Write(last2Bytes, 0, 2);
												}
												bytesToWrite = 0;
											}
											else if (chunk.Array[chunk.Offset + chunk.Count - 1] == 0 && (bytesToTrim == 1 || chunk.Array[chunk.Offset + chunk.Count - 2] == 0))
											{
												bytesToWrite -= bytesToTrim;
											}
										}
									}
									if (bytesToWrite > 0)
									{
										stream.Write(chunk.Array, chunk.Offset, bytesToWrite);
									}
								}
							}
							if (bytesRemaining > 0)
							{
								yield return null;
							}
						}
						if (stream != null)
						{
							stream.Flush();
						}
					}
					else if (PropertyTag.IsStringPropertyType(propertyTag.PropertyType))
					{
						propertyBag.SetProperty(new PropertyValue(streamPropertyTag, string.Empty));
					}
					else if (propertyTag.PropertyType != PropertyType.Object)
					{
						propertyBag.SetProperty(new PropertyValue(propertyTag, Array<byte>.Empty));
					}
					else
					{
						propertyWriteStream = propertyBag.SetPropertyStream(streamPropertyTag, 0L);
						propertyWriteStream.Flush();
					}
					context.AllowEndOfBufferActions(true);
					yield return null;
				}
				finally
				{
					if (encodedStream != null)
					{
						encodedStream.Dispose();
					}
					if (propertyWriteStream != null)
					{
						propertyWriteStream.Dispose();
					}
					context.AllowEndOfBufferActions(true);
				}
				yield break;
			}

			private static PropertyTag ChangeFromWrongTypes(PropertyTag propertyTag)
			{
				if (propertyTag.PropertyId == FastTransferIcsState.IdsetGiven.PropertyId)
				{
					return FastTransferIcsState.IdsetGiven;
				}
				return propertyTag;
			}

			public static IEnumerator<FastTransferStateMachine?> SkipPropertyIfExists(FastTransferUploadContext context, PropertyTag propertyTagToSkip)
			{
				return context.StateMachineFactory.SkipPropertyIfExists(context, propertyTagToSkip);
			}

			internal static IEnumerator<FastTransferStateMachine?> SkipPropertyIfExists_CreateDisplayClass(object unused, FastTransferUploadContext context, PropertyTag propertyTagToSkip)
			{
				if (!context.NoMoreData)
				{
					PropertyTag propertyTag;
					if (!context.DataInterface.TryPeekMarker(out propertyTag))
					{
						if (propertyTag == propertyTagToSkip)
						{
							SingleMemberPropertyBag propertyBag = new SingleMemberPropertyBag(propertyTagToSkip);
							yield return new FastTransferStateMachine?(FastTransferPropertyValue.DeserializeInto(context, propertyBag));
						}
					}
					else if (propertyTag == propertyTagToSkip)
					{
						context.DataInterface.ReadMarker(propertyTagToSkip);
					}
				}
				yield break;
			}
		}
	}
}
