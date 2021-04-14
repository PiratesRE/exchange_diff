using System;

namespace Microsoft.Exchange.Services.Core.Conversations.ResponseBuilders
{
	internal abstract class ConversationResponseBuilderBase<T>
	{
		private protected T Response { protected get; private set; }

		public T Build()
		{
			this.Response = this.BuildSkeleton();
			this.BuildConversationProperties();
			this.BuildNodes();
			return this.Response;
		}

		protected abstract T BuildSkeleton();

		protected abstract void BuildConversationProperties();

		protected abstract void BuildNodes();
	}
}
