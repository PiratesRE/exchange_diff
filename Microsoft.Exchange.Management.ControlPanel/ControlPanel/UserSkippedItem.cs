using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UserSkippedItem
	{
		public UserSkippedItem(MigrationUserSkippedItem skippedItem)
		{
			this.skippedItem = skippedItem;
		}

		[DataMember]
		public string Date
		{
			get
			{
				if (this.skippedItem.DateReceived != null)
				{
					return this.skippedItem.DateReceived.Value.ToString();
				}
				if (this.skippedItem.DateSent != null)
				{
					return this.skippedItem.DateSent.Value.ToString();
				}
				return string.Empty;
			}
			private set
			{
			}
		}

		[DataMember]
		public string Subject
		{
			get
			{
				return this.skippedItem.Subject;
			}
			private set
			{
			}
		}

		[DataMember]
		public string Kind
		{
			get
			{
				return this.skippedItem.Kind;
			}
			private set
			{
			}
		}

		[DataMember]
		public string FolderName
		{
			get
			{
				return this.skippedItem.FolderName;
			}
			private set
			{
			}
		}

		private MigrationUserSkippedItem skippedItem;
	}
}
