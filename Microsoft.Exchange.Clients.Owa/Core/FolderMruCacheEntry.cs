using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class FolderMruCacheEntry
	{
		public StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		public long LastAccessedDateTimeTicks
		{
			get
			{
				return this.lastAccessedDateTimeTicks;
			}
			set
			{
				this.lastAccessedDateTimeTicks = value;
			}
		}

		public int NumberOfSessionsSinceLastUse
		{
			get
			{
				return this.numberOfSessionsSinceLastUse;
			}
			set
			{
				this.numberOfSessionsSinceLastUse = value;
			}
		}

		public bool UsedInCurrentSession
		{
			get
			{
				return this.usedInCurrentSession;
			}
			set
			{
				this.usedInCurrentSession = value;
			}
		}

		public int UsageCount
		{
			get
			{
				return this.usageCount;
			}
			set
			{
				this.usageCount = value;
			}
		}

		public FolderMruCacheEntry()
		{
		}

		public FolderMruCacheEntry(StoreObjectId folderId)
		{
			if (folderId == null)
			{
				throw new ArgumentException("folderId cannot be null");
			}
			this.folderId = folderId;
		}

		public void RenderEntryXml(XmlTextWriter xmlWriter, string mruEntryName)
		{
			if (xmlWriter == null)
			{
				throw new ArgumentException("xmlWriter cannot be null");
			}
			if (string.IsNullOrEmpty(mruEntryName))
			{
				throw new ArgumentException("mruEntryName cannot be null or empty");
			}
			xmlWriter.WriteStartElement(mruEntryName);
			if (this.folderId != null)
			{
				xmlWriter.WriteAttributeString("folderId", this.folderId.ToBase64String());
			}
			if (this.lastAccessedDateTimeTicks >= 0L)
			{
				xmlWriter.WriteAttributeString("lastAccessedDateTimeTicks", this.lastAccessedDateTimeTicks.ToString());
			}
			if (this.numberOfSessionsSinceLastUse >= 0)
			{
				xmlWriter.WriteAttributeString("numberOfSessionsSinceLastUse", this.numberOfSessionsSinceLastUse.ToString());
			}
			xmlWriter.WriteAttributeString("usedInCurrentSession", this.usedInCurrentSession.ToString());
			if (this.usageCount >= 0)
			{
				xmlWriter.WriteAttributeString("usageCount", this.usageCount.ToString());
			}
			xmlWriter.WriteFullEndElement();
		}

		public void ParseEntry(XmlTextReader reader, UserContext userContext)
		{
			if (reader == null)
			{
				throw new ArgumentException("reader cannot be null");
			}
			if (userContext == null)
			{
				throw new ArgumentException("userContext cannot be null");
			}
			try
			{
				if (reader.HasAttributes)
				{
					int i = 0;
					while (i < reader.AttributeCount)
					{
						reader.MoveToAttribute(i);
						string name;
						if ((name = reader.Name) == null)
						{
							goto IL_EA;
						}
						if (!(name == "folderId"))
						{
							if (!(name == "lastAccessedDateTimeTicks"))
							{
								if (!(name == "numberOfSessionsSinceLastUse"))
								{
									if (!(name == "usedInCurrentSession"))
									{
										if (!(name == "usageCount"))
										{
											goto IL_EA;
										}
										this.usageCount = int.Parse(reader.Value);
									}
									else
									{
										this.usedInCurrentSession = Convert.ToBoolean(reader.Value);
									}
								}
								else
								{
									this.numberOfSessionsSinceLastUse = int.Parse(reader.Value);
								}
							}
							else
							{
								this.lastAccessedDateTimeTicks = long.Parse(reader.Value);
							}
						}
						else
						{
							this.folderId = Utilities.CreateStoreObjectId(userContext.MailboxSession, reader.Value);
						}
						IL_F1:
						i++;
						continue;
						IL_EA:
						this.ThrowParserException(reader);
						goto IL_F1;
					}
					reader.MoveToElement();
				}
			}
			catch (FormatException)
			{
				this.ThrowParserException(reader);
			}
			catch (OverflowException)
			{
				this.ThrowParserException(reader);
			}
			catch (OwaInvalidIdFormatException)
			{
				this.ThrowParserException(reader);
			}
		}

		private void ThrowParserException(XmlTextReader reader)
		{
			string text = null;
			string message = string.Format(CultureInfo.InvariantCulture, "Mru Cache. Invalid request. Line number: {0} Position: {1}.{2}", new object[]
			{
				reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				(text != null) ? (" " + text) : string.Empty
			});
			throw new OwaFolderMruParserException(message, null, this);
		}

		public void UpdateTimeStamp()
		{
			this.lastAccessedDateTimeTicks = DateTime.UtcNow.Ticks;
		}

		public void SetInitialUsage()
		{
			this.usageCount = 6;
		}

		public void DecayUsage(int decay)
		{
			if (this.usageCount != -1)
			{
				this.usageCount -= decay;
				if (this.usageCount < 0)
				{
					this.usageCount = 0;
				}
			}
		}

		private const string FolderIdAttribute = "folderId";

		private const string LastAccessedDateTimeTicksAttribute = "lastAccessedDateTimeTicks";

		private const string NumberOfSessionsSinceLastUseAttribute = "numberOfSessionsSinceLastUse";

		private const string UsedInCurrentSessionAttribute = "usedInCurrentSession";

		private const string UsageCountAttribute = "usageCount";

		private StoreObjectId folderId;

		private long lastAccessedDateTimeTicks;

		private int numberOfSessionsSinceLastUse;

		private bool usedInCurrentSession;

		private int usageCount;
	}
}
