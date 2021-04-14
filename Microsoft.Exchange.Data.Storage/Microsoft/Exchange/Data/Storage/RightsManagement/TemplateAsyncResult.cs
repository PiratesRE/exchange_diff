using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class TemplateAsyncResult : RightsManagementAsyncResult
	{
		public TemplateAsyncResult(RmsClientManagerContext context, Guid templateId, object callerState, AsyncCallback callerCallback) : base(context, callerState, callerCallback)
		{
			this.templateId = templateId;
		}

		public Guid TemplateId
		{
			get
			{
				return this.templateId;
			}
		}

		public TemplateWSManager TemplateManager
		{
			get
			{
				return this.templateManager;
			}
			set
			{
				this.templateManager = value;
			}
		}

		public void ReleaseWsManagers()
		{
			if (this.TemplateManager != null)
			{
				this.TemplateManager.Dispose();
				this.TemplateManager = null;
			}
		}

		private TemplateWSManager templateManager;

		private Guid templateId;
	}
}
