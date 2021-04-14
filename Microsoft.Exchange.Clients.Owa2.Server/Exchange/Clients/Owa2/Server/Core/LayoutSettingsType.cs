using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class LayoutSettingsType
	{
		[DataMember]
		public bool ShowInferenceUiElements
		{
			get
			{
				return this.showInferenceUiElements;
			}
			set
			{
				this.showInferenceUiElements = value;
			}
		}

		[DataMember]
		public bool ShowSenderOnTopInListView
		{
			get
			{
				return this.showSenderOnTopInListView;
			}
			set
			{
				this.showSenderOnTopInListView = value;
			}
		}

		[DataMember]
		public bool ShowPreviewTextInListView
		{
			get
			{
				return this.showPreviewTextInListView;
			}
			set
			{
				this.showPreviewTextInListView = value;
			}
		}

		[DataMember(Name = "ConversationSortOrder")]
		public string ConversationSortOrderString
		{
			get
			{
				return this.ConversationSortOrder.ToString();
			}
			set
			{
				this.ConversationSortOrder = (ConversationSortOrder)Enum.Parse(typeof(ConversationSortOrder), value);
			}
		}

		public ConversationSortOrder ConversationSortOrder
		{
			get
			{
				return this.conversationOrder;
			}
			set
			{
				this.conversationOrder = value;
			}
		}

		[DataMember]
		public bool HideDeletedItems
		{
			get
			{
				return this.hideDeletedItems;
			}
			set
			{
				this.hideDeletedItems = value;
			}
		}

		[DataMember]
		public int GlobalReadingPanePosition
		{
			get
			{
				return this.globalReadingPanePosition;
			}
			set
			{
				this.globalReadingPanePosition = value;
			}
		}

		[DataMember]
		public bool ShowFirstMessageOnSignIn
		{
			get
			{
				return this.showFirstMessageOnSignIn;
			}
			set
			{
				this.showFirstMessageOnSignIn = value;
			}
		}

		private bool showInferenceUiElements;

		private bool showSenderOnTopInListView;

		private bool showPreviewTextInListView;

		private ConversationSortOrder conversationOrder;

		private bool hideDeletedItems;

		private int globalReadingPanePosition;

		private bool showFirstMessageOnSignIn;
	}
}
