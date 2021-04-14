using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal sealed class FindTrendingConversationCommand : SingleStepServiceCommand<FindTrendingConversationRequest, ConversationType[]>
	{
		public FindTrendingConversationCommand(CallContext callContext, FindTrendingConversationRequest request) : base(callContext, request)
		{
			this.request = request;
			OwsLogRegistry.Register(base.GetType().Name, typeof(FindTrendingConversationMetadata), new Type[0]);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			int? totalConversationsInView = null;
			if (base.Result.Value != null)
			{
				totalConversationsInView = new int?(base.Result.Value.Length);
			}
			return new FindConversationResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value, null, totalConversationsInView, null, false);
		}

		internal override ServiceResult<ConversationType[]> Execute()
		{
			ServiceCommandBase.ThrowIfNull(this.request.ParentFolderId, "parentFolderId", "FindTrendingConversationCommand::Execute");
			IdAndSession session = null;
			try
			{
				session = base.IdConverter.ConvertFolderIdToIdAndSession(this.request.ParentFolderId.BaseFolderId, IdConverter.ConvertOption.IgnoreChangeKey);
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new ParentFolderNotFoundException(innerException);
			}
			FindTrendingConversation findTrendingConversation = new FindTrendingConversation(session, this.request.PageSize);
			return findTrendingConversation.Execute();
		}

		private readonly FindTrendingConversationRequest request;
	}
}
