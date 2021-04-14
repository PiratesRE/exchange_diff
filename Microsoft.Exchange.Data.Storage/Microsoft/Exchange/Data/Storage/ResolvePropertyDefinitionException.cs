using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ResolvePropertyDefinitionException : StoragePermanentException
	{
		public ResolvePropertyDefinitionException(uint unresolvablePropertyTag, LocalizedString message) : base(message, null)
		{
			this.unresolvablePropertyTag = unresolvablePropertyTag;
		}

		protected ResolvePropertyDefinitionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.unresolvablePropertyTag = (uint)info.GetValue("unresolvablePropertyTag", typeof(uint));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("unresolvablePropertyTag", this.unresolvablePropertyTag);
		}

		public uint UnresolvablePropertyTag
		{
			get
			{
				return this.unresolvablePropertyTag;
			}
		}

		private const string UnresolvablePropertyTagLabel = "unresolvablePropertyTag";

		private readonly uint unresolvablePropertyTag;
	}
}
