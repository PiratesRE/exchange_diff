using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.ContentTypes.Internal
{
	internal class ContentLineReader : IDisposable
	{
		public ContentLineReader(Stream s, Encoding encoding, ComplianceTracker complianceTracker, ValueTypeContainer container)
		{
			this.valueType = container;
			this.parser = new ContentLineParser(s, encoding, complianceTracker);
			this.complianceTracker = complianceTracker;
		}

		public int Depth
		{
			get
			{
				this.CheckDisposed("Depth::get");
				if (ContentLineNodeType.BeforeComponentStart != this.nodeType)
				{
					return this.componentStack.Count;
				}
				return this.componentStack.Count - 1;
			}
		}

		public string ComponentName
		{
			get
			{
				this.CheckDisposed("ComponentName::get");
				return this.componentName;
			}
		}

		public string PropertyName
		{
			get
			{
				this.CheckDisposed("PropertyName::get");
				return this.propertyName;
			}
		}

		public string ParameterName
		{
			get
			{
				this.CheckDisposed("ParameterName::get");
				return this.parameterName;
			}
		}

		public ContentLineNodeType Type
		{
			get
			{
				this.CheckDisposed("Type::get");
				return this.nodeType;
			}
		}

		public ComplianceTracker ComplianceTracker
		{
			get
			{
				this.CheckDisposed("ComplianceTracker::get");
				return this.complianceTracker;
			}
		}

		public ValueTypeContainer ValueType
		{
			get
			{
				this.CheckDisposed("ValueType::get");
				if (!this.valueType.IsInitialized)
				{
					this.valueType.SetPropertyName(this.PropertyName);
				}
				return this.valueType;
			}
		}

		public Encoding CurrentCharsetEncoding
		{
			get
			{
				this.CheckDisposed("CurrentEncoding::get");
				return this.parser.CurrentCharsetEncoding;
			}
		}

		protected virtual void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("ContentLineReader", methodName);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.isDisposed && this.parser != null)
			{
				this.parser.Dispose();
				this.parser = null;
			}
			this.isDisposed = true;
		}

		public bool ReadNextComponent()
		{
			this.CheckDisposed("ReadNextComponent");
			this.DrainValueStream();
			while (this.Read())
			{
				if (this.nodeType == ContentLineNodeType.ComponentStart)
				{
					return true;
				}
			}
			return false;
		}

		public bool ReadFirstChildComponent()
		{
			this.CheckDisposed("ReadFirstChildComponent");
			this.DrainValueStream();
			if (this.nodeType == ContentLineNodeType.ComponentEnd || this.nodeType == ContentLineNodeType.BeforeComponentEnd)
			{
				return false;
			}
			while (this.Read())
			{
				if (this.nodeType == ContentLineNodeType.ComponentStart)
				{
					return true;
				}
				if (this.nodeType == ContentLineNodeType.ComponentEnd)
				{
					return false;
				}
			}
			return false;
		}

		public bool ReadNextSiblingComponent()
		{
			this.CheckDisposed("ReadNextSiblingComponent");
			this.DrainValueStream();
			int depth = this.Depth;
			while ((this.nodeType != ContentLineNodeType.ComponentEnd || this.Depth > depth) && this.Read())
			{
			}
			if (this.nodeType == ContentLineNodeType.ComponentEnd)
			{
				this.Read();
			}
			while (this.nodeType == ContentLineNodeType.Property && this.Read())
			{
			}
			if (this.nodeType == ContentLineNodeType.BeforeComponentStart)
			{
				this.Read();
			}
			return this.nodeType == ContentLineNodeType.ComponentStart;
		}

		private bool Read()
		{
			ContentLineNodeType contentLineNodeType = this.nodeType;
			switch (contentLineNodeType)
			{
			case ContentLineNodeType.DocumentStart:
			case ContentLineNodeType.ComponentStart:
				goto IL_E3;
			case ContentLineNodeType.ComponentEnd:
				if (this.componentStack.Count > 0)
				{
					this.componentName = this.componentStack.Pop();
					goto IL_E3;
				}
				goto IL_E3;
			case ContentLineNodeType.ComponentStart | ContentLineNodeType.ComponentEnd:
			case ContentLineNodeType.ComponentStart | ContentLineNodeType.Parameter:
			case ContentLineNodeType.ComponentEnd | ContentLineNodeType.Parameter:
			case ContentLineNodeType.ComponentStart | ContentLineNodeType.ComponentEnd | ContentLineNodeType.Parameter:
				break;
			case ContentLineNodeType.Parameter:
			case ContentLineNodeType.Property:
				if (this.parser.State == ContentLineParser.States.ParamName || this.nodeType == ContentLineNodeType.Parameter)
				{
					while (this.ReadNextParameter())
					{
					}
				}
				if (this.parser.State == ContentLineParser.States.Value || this.parser.State == ContentLineParser.States.ValueStart)
				{
					this.ReadPropertyValue(false);
				}
				if (this.parser.State == ContentLineParser.States.ValueStartComma || this.parser.State == ContentLineParser.States.ValueStartSemiColon)
				{
					while (this.ReadNextPropertyValue())
					{
					}
					goto IL_E3;
				}
				goto IL_E3;
			default:
				if (contentLineNodeType == ContentLineNodeType.BeforeComponentStart)
				{
					this.nodeType = ContentLineNodeType.ComponentStart;
					return true;
				}
				if (contentLineNodeType == ContentLineNodeType.BeforeComponentEnd)
				{
					this.nodeType = ContentLineNodeType.ComponentEnd;
					return true;
				}
				break;
			}
			return false;
			IL_E3:
			if (this.parser.State == ContentLineParser.States.End)
			{
				if (this.componentStack.Count != 0)
				{
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated | ComplianceStatus.NotAllComponentsClosed, CalendarStrings.NotAllComponentsClosed);
					this.nodeType = ContentLineNodeType.BeforeComponentEnd;
				}
				else
				{
					this.nodeType = ContentLineNodeType.DocumentEnd;
				}
				return false;
			}
			this.nodeType = ContentLineNodeType.Property;
			this.propertyName = this.ReadName();
			this.parameterValueRead = false;
			this.propertyValueRead = false;
			if (this.parser.State == ContentLineParser.States.End)
			{
				if (0 < this.propertyName.Length)
				{
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated | ComplianceStatus.PropertyTruncated, CalendarStrings.PropertyTruncated);
				}
				if (0 < this.componentStack.Count)
				{
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated | ComplianceStatus.NotAllComponentsClosed, CalendarStrings.NotAllComponentsClosed);
					this.nodeType = ContentLineNodeType.BeforeComponentEnd;
				}
				else
				{
					this.nodeType = ContentLineNodeType.DocumentEnd;
				}
				return false;
			}
			if (this.propertyName.Equals("BEGIN", StringComparison.OrdinalIgnoreCase))
			{
				if (this.parser.State == ContentLineParser.States.ParamName)
				{
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.ParametersOnComponentTag, CalendarStrings.ParametersNotPermittedOnComponentTag);
					while (this.ReadNextParameter())
					{
					}
				}
				if (this.parser.State != ContentLineParser.States.End)
				{
					this.componentStack.Push(this.componentName);
					this.componentName = this.ReadPropertyValue(true).Trim();
					if (this.componentName.Length == 0)
					{
						this.complianceTracker.SetComplianceStatus(ComplianceStatus.EmptyComponentName, CalendarStrings.EmptyComponentName);
					}
					this.nodeType = ContentLineNodeType.BeforeComponentStart;
				}
			}
			else if (this.propertyName.Equals("END", StringComparison.OrdinalIgnoreCase))
			{
				if (this.parser.State == ContentLineParser.States.ParamName)
				{
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.ParametersOnComponentTag, CalendarStrings.ParametersNotPermittedOnComponentTag);
					while (this.ReadNextParameter())
					{
					}
				}
				if (this.parser.State != ContentLineParser.States.End)
				{
					string text = this.ReadPropertyValue(true).Trim();
					if (this.componentStack.Count == 0)
					{
						this.complianceTracker.SetComplianceStatus(ComplianceStatus.EndTagWithoutBegin, CalendarStrings.EndTagWithoutBegin);
					}
					if (!text.Equals(this.componentName, StringComparison.OrdinalIgnoreCase))
					{
						if (text.Length == 0)
						{
							this.complianceTracker.SetComplianceStatus(ComplianceStatus.EmptyComponentName, CalendarStrings.EmptyComponentName);
						}
						this.complianceTracker.SetComplianceStatus(ComplianceStatus.ComponentNameMismatch, CalendarStrings.ComponentNameMismatch);
					}
					this.nodeType = ContentLineNodeType.BeforeComponentEnd;
				}
			}
			else if (this.propertyName.Length == 0)
			{
				if (0 < this.componentStack.Count)
				{
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.EmptyPropertyName, CalendarStrings.EmptyPropertyName);
				}
			}
			else if (this.componentStack.Count == 0)
			{
				this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyOutsideOfComponent, CalendarStrings.PropertyOutsideOfComponent);
			}
			return true;
		}

		public bool ReadNextProperty()
		{
			this.CheckDisposed("ReadNextProperty");
			this.DrainValueStream();
			this.parameterValueRead = false;
			this.propertyValueRead = false;
			if (this.parser.State == ContentLineParser.States.End || this.nodeType == ContentLineNodeType.BeforeComponentEnd || this.nodeType == ContentLineNodeType.BeforeComponentStart)
			{
				return false;
			}
			if (!this.Read() || this.nodeType == ContentLineNodeType.BeforeComponentEnd || this.nodeType == ContentLineNodeType.ComponentEnd || this.nodeType != ContentLineNodeType.Property)
			{
				return false;
			}
			this.parameterName = null;
			this.valueType.Reset();
			this.propertyValueSeparator = ContentLineParser.Separators.Comma;
			return true;
		}

		public bool ReadNextParameter()
		{
			this.CheckDisposed("ReadNextParameter");
			if (this.parser.State == ContentLineParser.States.ParamValueStart || this.parser.State == ContentLineParser.States.ParamValueQuoted || this.parser.State == ContentLineParser.States.ParamValueUnquoted)
			{
				while (this.ReadNextParameterValue())
				{
				}
			}
			if (this.parser.State == ContentLineParser.States.ParamName)
			{
				this.nodeType = ContentLineNodeType.Parameter;
				this.parameterName = this.ReadName();
				this.parameterValueSeparator = ContentLineParser.Separators.Comma;
				if (this.parser.State == ContentLineParser.States.UnnamedParamEnd)
				{
					this.unnamedParameterValue = this.parameterName;
					this.parameterName = null;
					int num;
					this.parser.ParseElement(this.charBuffer, 0, 256, out num, false, ContentLineParser.Separators.None);
				}
				else if (this.parameterName.Length == 0)
				{
					this.complianceTracker.SetComplianceStatus(ComplianceStatus.EmptyParameterName, CalendarStrings.EmptyParameterName);
				}
				this.parameterValueRead = false;
				return true;
			}
			return false;
		}

		public bool ReadNextParameterValue()
		{
			this.CheckDisposed("ReadNextParameterValue");
			if (this.parameterValueSeparator == ContentLineParser.Separators.None)
			{
				throw new InvalidOperationException(CalendarStrings.InvalidReaderState);
			}
			if (!this.parameterValueRead && this.parser.State != ContentLineParser.States.ParamValueStart && this.parser.State != ContentLineParser.States.UnnamedParamEnd)
			{
				this.ReadParameterValue(false);
			}
			bool flag = false;
			if (this.parser.State == ContentLineParser.States.UnnamedParamEnd)
			{
				flag = true;
				int num;
				this.parser.ParseElement(this.charBuffer, 0, 256, out num, false, ContentLineParser.Separators.None);
			}
			if (this.parser.State == ContentLineParser.States.ParamValueStart)
			{
				int num2;
				this.parser.ParseElement(this.charBuffer, 0, 256, out num2, false, ContentLineParser.Separators.None);
			}
			flag = (flag || this.parser.State == ContentLineParser.States.ParamValueUnquoted || this.parser.State == ContentLineParser.States.ParamValueQuoted);
			if (flag)
			{
				this.parameterValueRead = false;
				return true;
			}
			this.parameterValueRead = true;
			return false;
		}

		public string ReadParameterValue(bool returnValue)
		{
			this.CheckDisposed("ReadParameterValue");
			if (this.parameterValueRead)
			{
				throw new InvalidOperationException(CalendarStrings.ValueAlreadyRead);
			}
			if (this.unnamedParameterValue != null && this.parameterName == null)
			{
				string result = null;
				if (returnValue)
				{
					result = this.unnamedParameterValue;
				}
				this.unnamedParameterValue = null;
				this.parameterValueRead = true;
				return result;
			}
			if (this.parser.State == ContentLineParser.States.ParamValueStart)
			{
				this.parameterValueSeparator = ContentLineParser.Separators.None;
				int charCount;
				this.parser.ParseElement(this.charBuffer, 0, 256, out charCount, false, ContentLineParser.Separators.None);
			}
			if (this.parser.State == ContentLineParser.States.Value || this.parser.State == ContentLineParser.States.End)
			{
				this.parameterValueRead = true;
				return string.Empty;
			}
			if (this.parser.State != ContentLineParser.States.ParamValueUnquoted && this.parser.State != ContentLineParser.States.ParamValueQuoted)
			{
				throw new InvalidOperationException(CalendarStrings.InvalidReaderState);
			}
			bool flag = string.Compare(this.parameterName, "VALUE", StringComparison.OrdinalIgnoreCase) == 0;
			string text = null;
			if (returnValue || flag)
			{
				this.stringBuilder.Length = 0;
				bool flag2;
				do
				{
					int charCount;
					flag2 = this.parser.ParseElement(this.charBuffer, 0, 256, out charCount, false, this.parameterValueSeparator);
					this.stringBuilder.Append(this.charBuffer, 0, charCount);
				}
				while (flag2);
				text = this.stringBuilder.ToString();
			}
			else
			{
				int charCount;
				while (this.parser.ParseElement(this.charBuffer, 0, 256, out charCount, false, this.parameterValueSeparator))
				{
				}
			}
			if (flag && text.Length > 0)
			{
				this.valueType.SetValueTypeParameter(text);
			}
			this.parameterValueRead = true;
			return text;
		}

		public bool ReadNextPropertyValue()
		{
			this.CheckDisposed("ReadNextPropertyValue");
			if (this.propertyValueSeparator == ContentLineParser.Separators.None)
			{
				throw new InvalidOperationException(CalendarStrings.InvalidReaderState);
			}
			if (this.parser.State == ContentLineParser.States.ParamName || this.nodeType == ContentLineNodeType.Parameter)
			{
				while (this.ReadNextParameter())
				{
				}
			}
			if (!this.propertyValueRead && this.parser.State != ContentLineParser.States.ValueStart)
			{
				this.ReadPropertyValue(false);
			}
			if (this.parser.State == ContentLineParser.States.ValueStart || this.parser.State == ContentLineParser.States.ValueStartComma || this.parser.State == ContentLineParser.States.ValueStartSemiColon)
			{
				int num;
				this.parser.ParseElement(this.charBuffer, 0, 256, out num, false, ContentLineParser.Separators.None);
			}
			this.DrainValueStream();
			this.parameterName = null;
			bool flag = this.parser.State == ContentLineParser.States.Value;
			this.propertyValueRead = !flag;
			return flag;
		}

		private string ReadPropertyValue(bool returnValue)
		{
			ContentLineParser.Separators separators;
			return this.ReadPropertyValue(returnValue, ContentLineParser.Separators.None, true, out separators);
		}

		public string ReadPropertyValue(bool returnValue, ContentLineParser.Separators expectedSeparators, bool useDefaultSeparator, out ContentLineParser.Separators endSeparator)
		{
			this.CheckDisposed("ReadPropertyValue");
			if (this.propertyValueRead)
			{
				throw new InvalidOperationException(CalendarStrings.ValueAlreadyRead);
			}
			endSeparator = ContentLineParser.Separators.None;
			if (this.parser.State == ContentLineParser.States.ParamName || this.nodeType == ContentLineNodeType.Parameter)
			{
				while (this.ReadNextParameter())
				{
				}
			}
			if (!useDefaultSeparator)
			{
				this.propertyValueSeparator = expectedSeparators;
			}
			if (this.parser.State == ContentLineParser.States.ValueStart)
			{
				if (useDefaultSeparator)
				{
					this.propertyValueSeparator = ContentLineParser.Separators.None;
				}
				int charCount;
				this.parser.ParseElement(this.charBuffer, 0, 256, out charCount, false, ContentLineParser.Separators.None);
			}
			this.DrainValueStream();
			if (this.parser.State == ContentLineParser.States.End)
			{
				this.propertyValueRead = true;
				return string.Empty;
			}
			ContentLineParser.Separators separators = this.propertyValueSeparator;
			if (!this.ValueType.CanBeMultivalued)
			{
				separators &= ~ContentLineParser.Separators.Comma;
			}
			if (!this.ValueType.CanBeCompound)
			{
				separators &= ~ContentLineParser.Separators.SemiColon;
			}
			string result = null;
			if (returnValue)
			{
				this.stringBuilder.Length = 0;
				bool flag;
				do
				{
					int charCount;
					flag = this.parser.ParseElement(this.charBuffer, 0, 256, out charCount, this.ValueType.IsTextType, separators);
					this.stringBuilder.Append(this.charBuffer, 0, charCount);
				}
				while (flag);
				result = this.stringBuilder.ToString();
			}
			else
			{
				int charCount;
				while (this.parser.ParseElement(this.charBuffer, 0, 256, out charCount, this.ValueType.IsTextType, separators))
				{
				}
			}
			this.propertyValueRead = true;
			this.parameterName = null;
			if (this.parser.State == ContentLineParser.States.ValueStartComma)
			{
				endSeparator = ContentLineParser.Separators.Comma;
			}
			else if (this.parser.State == ContentLineParser.States.ValueStartSemiColon)
			{
				endSeparator = ContentLineParser.Separators.SemiColon;
			}
			return result;
		}

		public void ApplyValueOverrides(Encoding charset, ByteEncoder decoder)
		{
			this.parser.ApplyValueOverrides(charset, decoder);
		}

		public Stream GetValueReadStream()
		{
			this.CheckDisposed("GetValueReadStream");
			if (this.propertyValueRead)
			{
				throw new InvalidOperationException(CalendarStrings.ValueAlreadyRead);
			}
			if (this.parser.State == ContentLineParser.States.ParamName || this.nodeType == ContentLineNodeType.Parameter)
			{
				while (this.ReadNextParameter())
				{
				}
			}
			if (this.parser.State == ContentLineParser.States.ValueStart)
			{
				int num;
				this.parser.ParseElement(this.charBuffer, 0, 256, out num, false, ContentLineParser.Separators.None);
			}
			this.DrainValueStream();
			this.propertyValueRead = true;
			if (this.parser.State != ContentLineParser.States.Value)
			{
				return new MemoryStream(new byte[0], false);
			}
			this.valueStream = (this.ValueType.IsTextType ? new ContentLineReader.ValueStream(this.parser) : this.parser.GetValueReadStream());
			return this.valueStream;
		}

		public void AssertValidState(ContentLineNodeType nodeType)
		{
			this.CheckDisposed("AssertValidState");
			if ((this.nodeType & nodeType) == ContentLineNodeType.DocumentStart)
			{
				throw new InvalidOperationException(CalendarStrings.InvalidReaderState);
			}
		}

		private void DrainValueStream()
		{
			if (this.valueStream != null)
			{
				this.valueStream.Dispose();
				this.valueStream = null;
			}
		}

		private string ReadName()
		{
			this.stringBuilder.Length = 0;
			bool flag;
			do
			{
				int val;
				flag = this.parser.ParseElement(this.charBuffer, 0, 256, out val, false, ContentLineParser.Separators.None);
				if (this.stringBuilder.Length < 255)
				{
					int charCount = Math.Min(val, 255 - this.stringBuilder.Length);
					this.stringBuilder.Append(this.charBuffer, 0, charCount);
				}
			}
			while (flag);
			return this.stringBuilder.ToString().Trim();
		}

		private const int CharBufferSize = 256;

		private const int MaxNameLength = 255;

		private ContentLineParser parser;

		private ComplianceTracker complianceTracker;

		private ContentLineNodeType nodeType;

		private string parameterName;

		private string unnamedParameterValue;

		private string propertyName;

		private string componentName;

		private Stack<string> componentStack = new Stack<string>();

		private Stream valueStream;

		private char[] charBuffer = new char[256];

		private ValueTypeContainer valueType;

		private ContentLineParser.Separators propertyValueSeparator;

		private ContentLineParser.Separators parameterValueSeparator;

		private StringBuilder stringBuilder = new StringBuilder();

		private bool parameterValueRead;

		private bool propertyValueRead;

		private bool isDisposed;

		private class ValueStream : Stream
		{
			public ValueStream(ContentLineParser reader)
			{
				this.parser = reader;
				this.encoder = reader.CurrentCharsetEncoding.GetEncoder();
				this.buffer = new byte[reader.CurrentCharsetEncoding.GetMaxByteCount(this.charBuffer.Length)];
			}

			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override long Length
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public override long Position
			{
				get
				{
					this.CheckDisposed("Position::get");
					return (long)this.position;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			private void CheckDisposed(string methodName)
			{
				if (this.isClosed)
				{
					throw new ObjectDisposedException("ValueStream", methodName);
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && !this.isClosed)
				{
					byte[] array = new byte[1024];
					while (this.Read(array, 0, array.Length) > 0)
					{
					}
				}
				this.isClosed = true;
			}

			public override void Flush()
			{
				throw new NotSupportedException();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				this.CheckDisposed("Read");
				int num = 0;
				if (this.eof)
				{
					return 0;
				}
				if (buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				if (offset < 0 || offset > buffer.Length)
				{
					throw new ArgumentOutOfRangeException("offset", CalendarStrings.OffsetOutOfRange);
				}
				if (count < 0)
				{
					throw new ArgumentOutOfRangeException("count", CalendarStrings.CountLessThanZero);
				}
				if (this.bufferSize != 0)
				{
					int num2 = Math.Min(count, this.bufferSize);
					Buffer.BlockCopy(this.buffer, this.bufferOffset, buffer, offset, num2);
					count -= num2;
					offset += num2;
					this.bufferSize -= num2;
					this.bufferOffset += num2;
					num += num2;
				}
				while (count > 0)
				{
					if (this.bufferSize == 0 && !this.eof)
					{
						this.ReadBuffer();
					}
					if (this.bufferSize == 0)
					{
						break;
					}
					int num2 = Math.Min(count, this.bufferSize);
					Buffer.BlockCopy(this.buffer, this.bufferOffset, buffer, offset, num2);
					this.bufferOffset += num2;
					this.bufferSize -= num2;
					offset += num2;
					count -= num2;
					num += num2;
				}
				this.position += num;
				return num;
			}

			public override int ReadByte()
			{
				this.CheckDisposed("Read");
				if (this.eof)
				{
					return -1;
				}
				int result;
				if (this.bufferSize != 0)
				{
					result = (int)this.buffer[this.bufferOffset++];
					this.bufferSize--;
					this.position++;
				}
				else
				{
					if (this.bufferSize == 0 && !this.eof)
					{
						this.ReadBuffer();
					}
					if (this.bufferSize == 0)
					{
						return -1;
					}
					result = (int)this.buffer[this.bufferOffset++];
					this.bufferSize--;
					this.position++;
				}
				return result;
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override void WriteByte(byte value)
			{
				throw new NotSupportedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			private void ReadBuffer()
			{
				this.bufferOffset = 0;
				this.bufferSize = 0;
				while (!this.eof && this.bufferSize == 0)
				{
					int charCount;
					this.eof = !this.parser.ParseElement(this.charBuffer, 0, 256, out charCount, true, ContentLineParser.Separators.None);
					this.bufferSize = this.encoder.GetBytes(this.charBuffer, 0, charCount, this.buffer, 0, this.eof);
				}
			}

			private ContentLineParser parser;

			private bool eof;

			private bool isClosed;

			private int position;

			private byte[] buffer;

			private int bufferOffset;

			private int bufferSize;

			private Encoder encoder;

			private char[] charBuffer = new char[256];
		}
	}
}
