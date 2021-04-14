using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	public abstract class MessageAction : Task
	{
		protected abstract void RunAction();

		protected abstract LocalizedException GetLocalizedException(Exception ex);

		protected override void InternalProcessRecord()
		{
			try
			{
				int num = 10;
				while (num-- > 0)
				{
					try
					{
						this.RunAction();
						break;
					}
					catch (RpcException ex)
					{
						if ((ex.ErrorCode != 1753 && ex.ErrorCode != 1727) || num == 0)
						{
							throw;
						}
					}
				}
			}
			catch (QueueViewerException ex2)
			{
				base.WriteError(this.GetLocalizedException(ex2) ?? this.GetDefaultException(ex2), ErrorCategory.InvalidOperation, null);
			}
			catch (RpcException ex3)
			{
				base.WriteError(this.GetLocalizedException(ex3) ?? this.GetDefaultException(ex3), ErrorCategory.InvalidOperation, null);
			}
		}

		private Exception GetDefaultException(QueueViewerException ex)
		{
			return ErrorMapper.GetLocalizedException(ex.ErrorCode, null, null);
		}

		private Exception GetDefaultException(RpcException ex)
		{
			return ErrorMapper.GetLocalizedException(ex.ErrorCode, null, null);
		}

		private const int taskExecutionRetryCount = 10;
	}
}
