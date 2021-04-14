using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MessagePlayerContext
	{
		internal StoreObjectId Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		internal LinkedListNode<MessagePartManager.MessagePart> CurrentTextPart
		{
			get
			{
				return this.currentTextPart;
			}
			set
			{
				this.currentTextPart = value;
			}
		}

		internal LinkedListNode<MessagePartManager.MessagePart> CurrentWavePart
		{
			get
			{
				return this.currentWavePart;
			}
			set
			{
				this.currentWavePart = value;
			}
		}

		internal char[] Remnant
		{
			get
			{
				return this.remnant;
			}
			set
			{
				this.remnant = value;
			}
		}

		internal PlaybackMode Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}

		internal long SeekPosition
		{
			get
			{
				return this.seekPosition;
			}
			set
			{
				this.seekPosition = value;
			}
		}

		internal PlaybackContent ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				this.contentType = value;
			}
		}

		internal CultureInfo Language
		{
			get
			{
				return this.language;
			}
			set
			{
				this.language = value;
			}
		}

		internal void Reset(StoreObjectId id)
		{
			this.Id = id;
			this.CurrentTextPart = null;
			this.CurrentWavePart = null;
			this.Remnant = new char[0];
			this.SeekPosition = 0L;
			this.Mode = PlaybackMode.None;
			this.ContentType = PlaybackContent.Unknown;
			this.Language = null;
		}

		private StoreObjectId id;

		private LinkedListNode<MessagePartManager.MessagePart> currentTextPart;

		private LinkedListNode<MessagePartManager.MessagePart> currentWavePart;

		private char[] remnant;

		private long seekPosition;

		private PlaybackMode mode;

		private PlaybackContent contentType;

		private CultureInfo language;
	}
}
