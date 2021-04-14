using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal class DrmEmailMessage : EncryptedEmailMessage
	{
		public DrmEmailMessage()
		{
			DrmEmailMessage.Tracer.TraceDebug((long)this.GetHashCode(), "Creating DrmEmailMessage to load message");
		}

		public DrmEmailMessage(Stream bodyStream, Stream htmlBodyStream, BodyFormat bodyFormat, int cpId) : base(bodyStream)
		{
			if (htmlBodyStream == null && bodyFormat != BodyFormat.Html)
			{
				throw new ArgumentNullException("htmlBodyStream");
			}
			DrmEmailMessage.Tracer.TraceDebug<BodyFormat, int>((long)this.GetHashCode(), "Creating DrmEmailMessage to save message. BodyFormat:{0} CodePageId:{1}", bodyFormat, cpId);
			this.htmlBodyStream = htmlBodyStream;
			this.bodyFormat = bodyFormat;
			this.cpId = cpId;
		}

		public BodyFormat BodyFormat
		{
			get
			{
				return this.bodyFormat;
			}
		}

		public int CodePage
		{
			get
			{
				return this.cpId;
			}
		}

		public Stream HtmlBodyStream
		{
			get
			{
				return this.htmlBodyStream;
			}
		}

		public List<DrmEmailAttachment> Attachments
		{
			get
			{
				return this.attachments;
			}
		}

		public override void Save(IStorage rootStorage, EncryptedEmailMessageBinding messageBinding)
		{
			if (rootStorage == null)
			{
				throw new ArgumentNullException("rootStorage");
			}
			if (messageBinding == null)
			{
				throw new ArgumentNullException("messageBinding");
			}
			if (base.BodyStream == null)
			{
				throw new InvalidOperationException("DrmMailMessage object is not loaded");
			}
			StreamOverIStream streamOverIStream = new StreamOverIStream(null);
			BufferedStream output = new BufferedStream(streamOverIStream);
			BinaryWriter binaryWriter = new BinaryWriter(output);
			Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream = null;
			try
			{
				stream = rootStorage.CreateStream("RpmsgStorageInfo", 4114, 0, 0);
				streamOverIStream.ReplaceIStream(stream);
				binaryWriter.Write(366883359U);
				binaryWriter.Write(2);
				binaryWriter.Write(2);
				binaryWriter.Write(0);
				binaryWriter.Flush();
				stream.Commit(STGC.STGC_DEFAULT);
			}
			finally
			{
				if (stream != null)
				{
					Marshal.ReleaseComObject(stream);
					stream = null;
				}
			}
			if (this.Attachments.Count > 0)
			{
				IStorage storage = null;
				try
				{
					storage = rootStorage.CreateStorage("Attachment List", 4114, 0, 0);
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < this.Attachments.Count; i++)
					{
						string text = string.Format(CultureInfo.InvariantCulture, "MailAttachment {0}", new object[]
						{
							i.ToString(NumberFormatInfo.InvariantInfo)
						});
						IStorage storage2 = null;
						try
						{
							storage2 = storage.CreateStorage(text, 4114, 0, 0);
							this.Attachments[i].Save(i, storage2, messageBinding, this.bodyFormat == BodyFormat.Rtf);
							storage2.Commit(STGC.STGC_DEFAULT);
						}
						finally
						{
							if (storage2 != null)
							{
								Marshal.ReleaseComObject(storage2);
								storage2 = null;
							}
						}
						stringBuilder.Append(text);
						stringBuilder.Append("|");
					}
					Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream2 = null;
					try
					{
						stream2 = storage.CreateStream("Attachment Info", 4114, 0, 0);
						streamOverIStream.ReplaceIStream(stream2);
						if (this.BodyFormat == BodyFormat.Rtf)
						{
							binaryWriter.Write(-1);
						}
						else
						{
							binaryWriter.Write(this.Attachments.Count);
						}
						DrmEmailUtils.WriteByteLengthUnicodeString(binaryWriter, stringBuilder.ToString());
						if (this.BodyFormat == BodyFormat.Rtf)
						{
							binaryWriter.Write(this.Attachments.Count);
							for (int j = 0; j < this.Attachments.Count; j++)
							{
								DrmEmailAttachment drmEmailAttachment = this.Attachments[j];
								binaryWriter.Write(drmEmailAttachment.CharacterPosition);
								binaryWriter.Write((uint)drmEmailAttachment.AttachmentType);
								binaryWriter.Write(drmEmailAttachment.DvAspect);
								binaryWriter.Write(drmEmailAttachment.SizeX);
								binaryWriter.Write(drmEmailAttachment.SizeY);
							}
						}
						binaryWriter.Flush();
						stream2.Commit(STGC.STGC_DEFAULT);
					}
					finally
					{
						if (stream2 != null)
						{
							Marshal.ReleaseComObject(stream2);
							stream2 = null;
						}
					}
					storage.Commit(STGC.STGC_DEFAULT);
				}
				finally
				{
					if (storage != null)
					{
						Marshal.ReleaseComObject(storage);
						storage = null;
					}
				}
			}
			Stream bodyStream = base.BodyStream;
			if (this.BodyFormat == BodyFormat.Rtf)
			{
				bodyStream = this.HtmlBodyStream;
			}
			Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream3 = null;
			try
			{
				stream3 = rootStorage.CreateStream("BodyPT-HTML", 4114, 0, 0);
				DrmEmailUtils.CopyStream(bodyStream, stream3);
				stream3.Commit(STGC.STGC_DEFAULT);
			}
			finally
			{
				if (stream3 != null)
				{
					Marshal.ReleaseComObject(stream3);
					stream3 = null;
				}
			}
			if (this.BodyFormat == BodyFormat.Rtf)
			{
				Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream4 = null;
				try
				{
					stream4 = rootStorage.CreateStream("BodyRTF", 4114, 0, 0);
					DrmEmailUtils.CopyStream(base.BodyStream, stream4);
					stream4.Commit(STGC.STGC_DEFAULT);
					goto IL_36B;
				}
				finally
				{
					if (stream4 != null)
					{
						Marshal.ReleaseComObject(stream4);
						stream4 = null;
					}
				}
			}
			if (this.BodyFormat == BodyFormat.PlainText)
			{
				Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream5 = null;
				try
				{
					stream5 = rootStorage.CreateStream("BodyPTAsHTML", 4114, 0, 0);
					DrmEmailUtils.CopyStream(this.HtmlBodyStream, stream5);
					stream5.Commit(STGC.STGC_DEFAULT);
				}
				finally
				{
					if (stream5 != null)
					{
						Marshal.ReleaseComObject(stream5);
						stream5 = null;
					}
				}
			}
			IL_36B:
			Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream6 = null;
			try
			{
				stream6 = rootStorage.CreateStream("OutlookBodyStreamInfo", 4114, 0, 0);
				streamOverIStream.ReplaceIStream(stream6);
				binaryWriter.Write((ushort)this.BodyFormat);
				binaryWriter.Write((uint)this.CodePage);
				binaryWriter.Flush();
				stream6.Commit(STGC.STGC_DEFAULT);
			}
			finally
			{
				if (stream6 != null)
				{
					Marshal.ReleaseComObject(stream6);
					stream6 = null;
				}
			}
		}

		public override void Close()
		{
			if (this.htmlBodyStream != null && !this.htmlBodyStream.Equals(base.BodyStream))
			{
				this.htmlBodyStream.Close();
			}
			this.htmlBodyStream = null;
			if (this.Attachments.Count > 0)
			{
				foreach (DrmEmailAttachment drmEmailAttachment in this.Attachments)
				{
					drmEmailAttachment.Close();
				}
			}
			base.Close();
		}

		public override void Load(IStorage rootStorage, CreateStreamCallbackDelegate createBodyStreamCallback, CreateStreamCallbackDelegate createAttachmentStreamCallback)
		{
			if (rootStorage == null)
			{
				throw new ArgumentNullException("rootStorage");
			}
			if (createBodyStreamCallback == null)
			{
				throw new ArgumentNullException("createBodyStreamCallback");
			}
			if (createAttachmentStreamCallback == null)
			{
				throw new ArgumentNullException("createAttachmentStreamCallback");
			}
			if (base.BodyStream != null)
			{
				throw new InvalidOperationException("DrmEmailMessage object is already loaded");
			}
			StreamOverIStream streamOverIStream = new StreamOverIStream(null);
			BinaryReader binaryReader = new BinaryReader(streamOverIStream);
			DrmEmailAttachment[] array = null;
			Stream stream = null;
			bool flag = false;
			Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream2 = null;
			try
			{
				try
				{
					stream2 = DrmEmailUtils.EnsureStream(rootStorage, "RpmsgStorageInfo");
					streamOverIStream.ReplaceIStream(stream2);
					if ((long)binaryReader.ReadInt32() != 366883359L || binaryReader.ReadInt32() != 2 || binaryReader.ReadInt32() != 2 || binaryReader.ReadInt32() != 0)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("RpmsgStorageInfoNotValid"));
					}
				}
				catch (EndOfStreamException innerException)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("RpmsgStorageInfoReachedEOS"), innerException);
				}
				finally
				{
					if (stream2 != null)
					{
						Marshal.ReleaseComObject(stream2);
						stream2 = null;
					}
				}
				Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream3 = null;
				BodyFormat bodyFormat;
				int arg;
				try
				{
					stream3 = DrmEmailUtils.EnsureStream(rootStorage, "OutlookBodyStreamInfo");
					streamOverIStream.ReplaceIStream(stream3);
					int num = (int)binaryReader.ReadUInt16();
					if (num != 2 && num != 1 && num != 3)
					{
						throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("InvalidBodyFormat"));
					}
					bodyFormat = (BodyFormat)num;
					arg = binaryReader.ReadInt32();
				}
				catch (EndOfStreamException innerException2)
				{
					throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("InvalidOutlookBodyStreamInfo"), innerException2);
				}
				finally
				{
					if (stream3 != null)
					{
						Marshal.ReleaseComObject(stream3);
						stream3 = null;
					}
				}
				DrmEmailMessage.Tracer.TraceDebug<BodyFormat, int>((long)this.GetHashCode(), "Loaded DrmEmailMessage. BodyFormat:{0} CodePageId:{1}", bodyFormat, arg);
				stream = createBodyStreamCallback(new DrmEmailBodyInfo(bodyFormat, arg));
				if (bodyFormat == BodyFormat.PlainText || bodyFormat == BodyFormat.Html)
				{
					Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream4 = null;
					try
					{
						stream4 = DrmEmailUtils.EnsureStream(rootStorage, "BodyPT-HTML");
						DrmEmailUtils.CopyStream(stream4, stream);
						goto IL_1DF;
					}
					finally
					{
						if (stream4 != null)
						{
							Marshal.ReleaseComObject(stream4);
							stream4 = null;
						}
					}
				}
				Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream5 = null;
				try
				{
					stream5 = DrmEmailUtils.EnsureStream(rootStorage, "BodyRTF");
					DrmEmailUtils.CopyStream(stream5, stream);
				}
				finally
				{
					if (stream5 != null)
					{
						Marshal.ReleaseComObject(stream5);
						stream5 = null;
					}
				}
				IL_1DF:
				IStorage storage = null;
				try
				{
					storage = DrmEmailUtils.EnsureStorage(rootStorage, "Attachment List", false);
					if (storage != null)
					{
						string[] array2 = null;
						uint[,] array3 = null;
						Microsoft.Exchange.Security.RightsManagement.StructuredStorage.IStream stream6 = null;
						try
						{
							stream6 = DrmEmailUtils.EnsureStream(storage, "Attachment Info");
							streamOverIStream.ReplaceIStream(stream6);
							int num2 = binaryReader.ReadInt32();
							if (bodyFormat != BodyFormat.Rtf && num2 <= 0)
							{
								throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachmentCountInvalid"));
							}
							if (bodyFormat == BodyFormat.Rtf && num2 != -1)
							{
								throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("RtfBodyAttachmentCountInvalid"));
							}
							string text = DrmEmailUtils.ReadByteLengthUnicodeString(binaryReader);
							array2 = text.Split(new char[]
							{
								'|'
							}, StringSplitOptions.RemoveEmptyEntries);
							if (bodyFormat != BodyFormat.Rtf && array2.Length != num2)
							{
								throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachmentNameCountMismatch"));
							}
							if (bodyFormat == BodyFormat.Rtf)
							{
								num2 = array2.Length;
							}
							DrmEmailMessage.Tracer.TraceDebug<int, string>((long)this.GetHashCode(), "Loaded attachment list info. Count:{0} AttachNames:{1}", num2, text);
							array = new DrmEmailAttachment[num2];
							Dictionary<string, DrmEmailAttachment> dictionary = new Dictionary<string, DrmEmailAttachment>();
							foreach (string text2 in array2)
							{
								if (string.IsNullOrEmpty(text2))
								{
									throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachmentNameInvalid"));
								}
								if (dictionary.ContainsKey(text2))
								{
									throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachmentNameDuplicate"));
								}
								dictionary[text2] = null;
							}
							if (bodyFormat == BodyFormat.Rtf)
							{
								int num3 = binaryReader.ReadInt32();
								if (num2 != num3)
								{
									throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("AttachmentCountRtfCountMismatch"));
								}
								array3 = new uint[num2, 5];
								for (int j = 0; j < num2; j++)
								{
									array3[j, 0] = binaryReader.ReadUInt32();
									array3[j, 1] = binaryReader.ReadUInt32();
									array3[j, 2] = binaryReader.ReadUInt32();
									array3[j, 3] = binaryReader.ReadUInt32();
									array3[j, 4] = binaryReader.ReadUInt32();
									AttachmentType attachmentType = DrmEmailMessage.ConvertMSOOBJFToAttachmentType(array3[j, 1]);
									if (attachmentType == AttachmentType.None)
									{
										throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("RtfBodyAttachmentTypeInvalid"));
									}
									array3[j, 1] = (uint)attachmentType;
									DVASPECT dvaspect = (attachmentType == AttachmentType.OleObject) ? DVASPECT.DVASPECT_CONTENT : DVASPECT.DVASPECT_ICON;
									if (dvaspect != (DVASPECT)array3[j, 2])
									{
										throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("RtfBodyAttachmentDvAspectIsInvalid"));
									}
									if (DrmEmailMessage.Tracer.IsTraceEnabled(TraceType.DebugTrace))
									{
										string message = string.Format(CultureInfo.InvariantCulture, "Loaded rtf body attachment info. Index:{0} CP:{1} AT:{2} DVA:{3} X:{4} Y:{5}", new object[]
										{
											j,
											array3[j, 0],
											attachmentType,
											dvaspect,
											array3[j, 3],
											array3[j, 4]
										});
										DrmEmailMessage.Tracer.TraceDebug((long)this.GetHashCode(), message);
									}
								}
							}
						}
						catch (EndOfStreamException innerException3)
						{
							throw new InvalidRpmsgFormatException(DrmStrings.InvalidRpmsgFormat("InvalidAttachmentListInfo"), innerException3);
						}
						finally
						{
							if (stream6 != null)
							{
								Marshal.ReleaseComObject(stream6);
								stream6 = null;
							}
						}
						for (int k = 0; k < array2.Length; k++)
						{
							DrmEmailMessage.Tracer.TraceDebug<int, string>((long)this.GetHashCode(), "Loading attachment storage. Index:{0} Name:{1}", k, array2[k]);
							IStorage storage2 = null;
							try
							{
								storage2 = DrmEmailUtils.EnsureStorage(storage, array2[k]);
								DrmEmailAttachment drmEmailAttachment = DrmEmailAttachment.Load(storage2, (array3 != null) ? array3[k, 0] : 0U, (AttachmentType)((array3 != null) ? array3[k, 1] : 0U), (array3 != null) ? array3[k, 2] : 4U, (array3 != null) ? array3[k, 3] : 0U, (array3 != null) ? array3[k, 4] : 0U, createAttachmentStreamCallback);
								array[k] = drmEmailAttachment;
							}
							finally
							{
								if (storage2 != null)
								{
									Marshal.ReleaseComObject(storage2);
									storage2 = null;
								}
							}
						}
					}
				}
				finally
				{
					if (storage != null)
					{
						Marshal.ReleaseComObject(storage);
						storage = null;
					}
				}
				base.BodyStream = stream;
				this.htmlBodyStream = stream;
				this.bodyFormat = bodyFormat;
				if (array != null)
				{
					this.Attachments.AddRange(array);
				}
				DrmEmailMessage.Tracer.TraceDebug((long)this.GetHashCode(), "Successfully loaded the email message.");
				flag = true;
				stream = null;
				array = null;
			}
			finally
			{
				if (!flag)
				{
					if (stream != null)
					{
						stream.Close();
					}
					if (array != null)
					{
						foreach (DrmEmailAttachment drmEmailAttachment2 in array)
						{
							if (drmEmailAttachment2 != null)
							{
								drmEmailAttachment2.Close();
							}
						}
					}
				}
			}
		}

		private static AttachmentType ConvertMSOOBJFToAttachmentType(uint msoObjf)
		{
			AttachmentType result = AttachmentType.None;
			if ((msoObjf & 4U) == 4U)
			{
				result = AttachmentType.ByValue;
			}
			else if ((msoObjf & 8U) == 8U)
			{
				result = AttachmentType.EmbeddedMessage;
			}
			else if ((msoObjf & 1U) == 1U)
			{
				result = AttachmentType.OleObject;
			}
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private BodyFormat bodyFormat;

		private int cpId;

		private Stream htmlBodyStream;

		private List<DrmEmailAttachment> attachments = new List<DrmEmailAttachment>();
	}
}
