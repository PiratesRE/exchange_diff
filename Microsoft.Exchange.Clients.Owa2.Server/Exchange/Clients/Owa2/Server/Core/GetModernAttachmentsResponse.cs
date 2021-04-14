using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetModernAttachmentsResponse
	{
		[DataMember(Order = 1)]
		public ModernAttachmentGroup[] AttachmentGroups
		{
			get
			{
				if (this.modernAttachmentGroups == null)
				{
					return null;
				}
				return this.modernAttachmentGroups.ToArray();
			}
		}

		public void AddGroup(ModernAttachmentGroup modernAttachmentGroup)
		{
			if (modernAttachmentGroup != null)
			{
				if (this.modernAttachmentGroups == null)
				{
					this.modernAttachmentGroups = new List<ModernAttachmentGroup>(1);
				}
				this.modernAttachmentGroups.Add(modernAttachmentGroup);
			}
		}

		[DataMember(Order = 2)]
		public StructuredErrors[] Errors
		{
			get
			{
				if (this.errors == null)
				{
					return null;
				}
				return this.errors.ToArray();
			}
		}

		public void AddError(StructuredErrors error)
		{
			if (this.errors == null)
			{
				this.errors = new List<StructuredErrors>(1);
			}
			this.errors.Add(error);
		}

		private List<StructuredErrors> errors;

		private List<ModernAttachmentGroup> modernAttachmentGroups;
	}
}
