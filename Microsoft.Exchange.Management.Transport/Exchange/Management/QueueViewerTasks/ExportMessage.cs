using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Transport.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[Cmdlet("Export", "Message", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ExportMessage : MessageActionWithIdentity
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return QueueViewerStrings.ConfirmationMessageExportMessage(base.Identity.ToString());
			}
		}

		protected override void RunAction()
		{
			using (QueueViewerClient<ExtensibleMessageInfo> queueViewerClient = new QueueViewerClient<ExtensibleMessageInfo>((string)base.Server))
			{
				try
				{
					int num = 0;
					for (;;)
					{
						byte[] array = queueViewerClient.ReadMessageBody(base.Identity, num, 65536);
						if (array == null)
						{
							break;
						}
						num += array.Length;
						BinaryFileDataObject binaryFileDataObject = new BinaryFileDataObject();
						binaryFileDataObject.SetIdentity(base.Identity);
						binaryFileDataObject.FileData = array;
						base.WriteObject(binaryFileDataObject);
					}
				}
				catch (RpcException ex)
				{
					if (ex.ErrorCode == 1753 || ex.ErrorCode == 1727)
					{
						base.WriteError(ErrorMapper.GetLocalizedException(ex.ErrorCode, null, base.Server), (ErrorCategory)1002, null);
					}
					throw;
				}
			}
		}

		private const int BlockSize = 65536;
	}
}
