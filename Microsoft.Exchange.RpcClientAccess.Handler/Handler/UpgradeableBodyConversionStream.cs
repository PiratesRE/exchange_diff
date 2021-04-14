using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class UpgradeableBodyConversionStream : Stream
	{
		internal UpgradeableBodyConversionStream(Logon logon, StreamSource streamSource, PropertyTag propertyTag, bool isAppend, Encoding string8Encoding, BodyHelper bodyHelper, Func<Logon, StreamSource, PropertyTag, bool, BodyHelper, BodyConversionStream> bodyConversionStreamFactory, Func<Logon, StreamSource, PropertyTag, Encoding, PropertyStream> propertyStreamFactory) : base(logon)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				Util.ThrowOnNullArgument(streamSource, "streamSource");
				if (!propertyTag.IsBodyProperty())
				{
					throw new ArgumentException("PropertyTag must be a body property.");
				}
				this.streamSource = streamSource;
				this.propertyTag = propertyTag;
				this.isAppend = isAppend;
				this.string8Encoding = string8Encoding;
				this.bodyHelper = bodyHelper;
				this.bodyConversionStreamFactory = bodyConversionStreamFactory;
				this.propertyStreamFactory = propertyStreamFactory;
				disposeGuard.Success();
			}
		}

		private BodyConversionStream BodyConversionStream
		{
			get
			{
				if (this.UsePropertyStream)
				{
					throw new InvalidOperationException("Should have detected propertyStream exists and not have called BodyConversionStream");
				}
				if (this.bodyConversionStream == null)
				{
					this.bodyConversionStream = this.bodyConversionStreamFactory(base.LogonObject, this.streamSource, this.propertyTag, this.isAppend, this.bodyHelper);
				}
				return this.bodyConversionStream;
			}
		}

		private PropertyStream PropertyStream
		{
			get
			{
				if (this.propertyStream == null)
				{
					long streamLength = this.BodyConversionStream.StreamLength;
					long currentPosition = this.BodyConversionStream.CurrentPosition;
					using (DisposeGuard disposeGuard = default(DisposeGuard))
					{
						PropertyStream propertyStream = this.propertyStreamFactory(base.LogonObject, this.streamSource, this.propertyTag, this.string8Encoding);
						disposeGuard.Add<PropertyStream>(propertyStream);
						if (streamLength > 0L)
						{
							if (currentPosition > 0L)
							{
								this.BodyConversionStream.Seek(StreamSeekOrigin.Begin, 0L);
							}
							this.BodyConversionStream.CopyToStream(propertyStream, (ulong)streamLength);
							propertyStream.Seek(StreamSeekOrigin.Begin, currentPosition);
						}
						this.propertyStream = propertyStream;
						disposeGuard.Success();
					}
					this.bodyHelper.OnBeforeWrite(this.propertyTag);
					Util.DisposeIfPresent(this.bodyConversionStream);
					this.bodyConversionStream = null;
				}
				return this.propertyStream;
			}
		}

		private bool UsePropertyStream
		{
			get
			{
				return this.propertyStream != null;
			}
		}

		public override void Commit()
		{
			base.CheckDisposed();
			if (this.UsePropertyStream)
			{
				this.PropertyStream.Commit();
			}
		}

		public override uint GetSize()
		{
			base.CheckDisposed();
			if (this.UsePropertyStream)
			{
				return this.PropertyStream.GetSize();
			}
			if (this.bodyConversionStreamSize == null)
			{
				this.bodyConversionStreamSize = new uint?(this.BodyConversionStream.GetSize());
			}
			return this.bodyConversionStreamSize.Value;
		}

		public override ArraySegment<byte> Read(ushort requestedSize)
		{
			base.CheckDisposed();
			if (this.UsePropertyStream)
			{
				return this.PropertyStream.Read(requestedSize);
			}
			return this.BodyConversionStream.Read(requestedSize);
		}

		public override long Seek(StreamSeekOrigin streamSeekOrigin, long offset)
		{
			base.CheckDisposed();
			if (this.UsePropertyStream)
			{
				return this.PropertyStream.Seek(streamSeekOrigin, offset);
			}
			if (streamSeekOrigin == StreamSeekOrigin.Begin && offset == 0L)
			{
				Util.DisposeIfPresent(this.bodyConversionStream);
				this.bodyConversionStream = null;
				return 0L;
			}
			if (offset > 0L && ((streamSeekOrigin == StreamSeekOrigin.Begin && offset > this.BodyConversionStream.StreamLength) || (streamSeekOrigin == StreamSeekOrigin.Current && this.BodyConversionStream.CurrentPosition > this.BodyConversionStream.StreamLength - offset) || streamSeekOrigin == StreamSeekOrigin.End))
			{
				return this.PropertyStream.Seek(streamSeekOrigin, offset);
			}
			return this.BodyConversionStream.Seek(streamSeekOrigin, offset);
		}

		public override void SetSize(ulong size)
		{
			base.CheckDisposed();
			this.PropertyStream.SetSize(size);
		}

		public override ushort Write(ArraySegment<byte> data)
		{
			base.CheckDisposed();
			return this.PropertyStream.Write(data);
		}

		public override ulong CopyToStream(Stream destinationStream, ulong bytesToCopy)
		{
			base.CheckDisposed();
			if (this.UsePropertyStream)
			{
				return this.PropertyStream.CopyToStream(destinationStream, bytesToCopy);
			}
			return this.BodyConversionStream.CopyToStream(destinationStream, bytesToCopy);
		}

		public override void CheckCanWrite()
		{
			base.CheckDisposed();
		}

		internal override void OnAccess()
		{
			if (this.UsePropertyStream)
			{
				this.PropertyStream.OnAccess();
			}
			else if (this.bodyConversionStream != null)
			{
				this.bodyConversionStream.OnAccess();
			}
			base.OnAccess();
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<UpgradeableBodyConversionStream>(this);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.streamSource);
			if (this.bodyConversionStream != null)
			{
				try
				{
					this.bodyConversionStream.Dispose();
				}
				finally
				{
					this.bodyConversionStream = null;
				}
			}
			if (this.propertyStream != null)
			{
				try
				{
					this.propertyStream.Dispose();
				}
				finally
				{
					this.propertyStream = null;
				}
			}
			base.InternalDispose();
		}

		private const ulong DefaultCopyToStreamSegmentSize = 65535UL;

		private readonly StreamSource streamSource;

		private readonly PropertyTag propertyTag;

		private readonly bool isAppend;

		private readonly Encoding string8Encoding;

		private readonly BodyHelper bodyHelper;

		private readonly Func<Logon, StreamSource, PropertyTag, bool, BodyHelper, BodyConversionStream> bodyConversionStreamFactory;

		private readonly Func<Logon, StreamSource, PropertyTag, Encoding, PropertyStream> propertyStreamFactory;

		private BodyConversionStream bodyConversionStream;

		private uint? bodyConversionStreamSize = null;

		private PropertyStream propertyStream;
	}
}
