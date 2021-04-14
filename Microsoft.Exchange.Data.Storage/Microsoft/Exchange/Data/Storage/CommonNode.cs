using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CommonNode
	{
		public CommonNode(string serverIdIn, string versionIdIn, byte sentToClientIn, byte itemIsEmailIn, byte readIn, byte itemIsCalendarIn, ExDateTime endTimeIn)
		{
			this.ServerId = serverIdIn;
			this.VersionId = versionIdIn;
			this.SentToClient = (sentToClientIn == 1);
			this.IsEmail = (itemIsEmailIn == 1);
			this.Read = (readIn == 43);
			this.IsCalendar = (itemIsCalendarIn == 1);
			this.endTime = endTimeIn;
		}

		public ExDateTime EndTime
		{
			get
			{
				return this.endTime;
			}
			set
			{
				this.endTime = value;
			}
		}

		public bool IsCalendar
		{
			get
			{
				return this.itemIsCalendar;
			}
			set
			{
				this.itemIsCalendar = value;
			}
		}

		public bool IsEmail
		{
			get
			{
				return this.itemIsEmail;
			}
			set
			{
				this.itemIsEmail = value;
			}
		}

		public bool Read
		{
			get
			{
				return this.read;
			}
			set
			{
				this.read = value;
			}
		}

		public bool SentToClient
		{
			get
			{
				return this.sentToClient;
			}
			set
			{
				this.sentToClient = value;
			}
		}

		public string ServerId
		{
			get
			{
				return this.serverId;
			}
			set
			{
				this.serverId = value;
			}
		}

		public string VersionId
		{
			get
			{
				return this.versionId;
			}
			set
			{
				this.versionId = value;
			}
		}

		private ExDateTime endTime = ExDateTime.MinValue;

		private bool itemIsCalendar;

		private bool itemIsEmail;

		private bool read;

		private bool sentToClient;

		private string serverId;

		private string versionId;
	}
}
