using System;
using System.Collections;
using System.Text;
using Microsoft.Exchange.Clients.Owa.Core.Controls;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class InfobarRenderingHelper
	{
		public string FileNameStringForLevelOneAndBlock
		{
			get
			{
				return this.fileNameStringForLevelOneAndBlock;
			}
		}

		public bool HasLevelOneAndBlock
		{
			get
			{
				return this.hasLevelOneAndBlock;
			}
		}

		public bool HasWebReadyFirst
		{
			get
			{
				return this.hasWebReadyFirst;
			}
		}

		public bool HasLevelOne
		{
			get
			{
				return this.hasLevelOne;
			}
		}

		public bool HasLevelTwo
		{
			get
			{
				return this.hasLevelTwo;
			}
		}

		public bool HasLevelThree
		{
			get
			{
				return this.hasLevelThree;
			}
		}

		public InfobarRenderingHelper(ArrayList attachmentList)
		{
			this.CreateAttachmentInfobarHelper(attachmentList);
		}

		private void CreateAttachmentInfobarHelper(ArrayList attachmentList)
		{
			if (attachmentList == null || attachmentList.Count <= 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			UserContext userContext = UserContextManager.GetUserContext();
			AttachmentPolicy attachmentPolicy = userContext.AttachmentPolicy;
			foreach (object obj in attachmentList)
			{
				AttachmentWellInfo attachmentWellInfo = (AttachmentWellInfo)obj;
				bool flag = AttachmentUtility.IsWebReadyDocument(attachmentWellInfo.FileExtension, attachmentWellInfo.MimeType);
				if (flag && (attachmentWellInfo.AttachmentLevel == AttachmentPolicy.Level.Block || attachmentPolicy.ForceWebReadyDocumentViewingFirst))
				{
					this.hasWebReadyFirst = true;
				}
				if (attachmentWellInfo.AttachmentLevel == AttachmentPolicy.Level.Block)
				{
					this.hasLevelOne = true;
					if (!flag)
					{
						this.hasLevelOneAndBlock = true;
						num++;
						if (num == 16)
						{
							stringBuilder.Append(",...");
						}
						else if (num <= 15)
						{
							if (num != 1)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.Append(attachmentWellInfo.AttachmentName);
						}
					}
				}
				else if (attachmentWellInfo.AttachmentLevel == AttachmentPolicy.Level.ForceSave)
				{
					this.hasLevelTwo = true;
				}
				else if (attachmentWellInfo.AttachmentLevel == AttachmentPolicy.Level.Allow)
				{
					this.hasLevelThree = true;
				}
			}
			if (stringBuilder.Length > 0)
			{
				this.fileNameStringForLevelOneAndBlock = stringBuilder.ToString();
			}
		}

		private string fileNameStringForLevelOneAndBlock = string.Empty;

		private bool hasLevelOneAndBlock;

		private bool hasWebReadyFirst;

		private bool hasLevelOne;

		private bool hasLevelTwo;

		private bool hasLevelThree;
	}
}
