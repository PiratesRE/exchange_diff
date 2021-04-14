using System;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("GetUserPhotoType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public sealed class GetUserPhotoRequest : BaseRequest
	{
		public GetUserPhotoRequest()
		{
			this.isPostRequest = true;
		}

		public GetUserPhotoRequest(WebOperationContext webContext, string email, ADObjectId adObjectId, UserPhotoSize size, bool isPreview) : this(new OutgoingWebResponseContextWrapper(webContext.OutgoingResponse), email, size, isPreview)
		{
		}

		public GetUserPhotoRequest(WebOperationContext webContext, string email, UserPhotoSize size, bool isPreview) : this(webContext, email, null, size, isPreview)
		{
		}

		public GetUserPhotoRequest(IOutgoingWebResponseContext outgoingResponse, string email, ADObjectId adObjectId, UserPhotoSize size, bool isPreview)
		{
			this.isPostRequest = false;
			this.OutgoingResponse = outgoingResponse;
			this.Email = email;
			this.AdObjectId = adObjectId;
			this.SizeRequested = size;
			this.isPreview = isPreview;
		}

		public GetUserPhotoRequest(IOutgoingWebResponseContext outgoingResponse, string email, UserPhotoSize size, bool isPreview) : this(outgoingResponse, email, null, size, isPreview)
		{
		}

		public GetUserPhotoRequest(IOutgoingWebResponseContext outgoingResponse, string email, UserPhotoSize size, bool isPreview, bool fallbackToClearImage) : this(outgoingResponse, email, null, size, isPreview)
		{
			this.fallbackToClearImage = fallbackToClearImage;
		}

		internal IOutgoingWebResponseContext OutgoingResponse { get; set; }

		internal string CacheId { get; set; }

		internal ADObjectId AdObjectId { get; set; }

		[XmlElement]
		[DataMember(Name = "Email", IsRequired = true)]
		public string Email { get; set; }

		[XmlElement]
		[DataMember(Name = "SizeRequested", IsRequired = true)]
		public UserPhotoSize SizeRequested { get; set; }

		internal bool IsPreview
		{
			get
			{
				return this.isPreview;
			}
		}

		internal bool FallbackToClearImage
		{
			get
			{
				return this.fallbackToClearImage;
			}
		}

		internal bool IsPostRequest
		{
			get
			{
				return this.isPostRequest;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetUserPhoto(callContext, this, NullTracer.Instance);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		internal override void Validate()
		{
			base.Validate();
			if (string.IsNullOrEmpty(this.Email) || !SmtpAddress.IsValidSmtpAddress(this.Email))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSmtpAddressException(), FaultParty.Sender);
			}
		}

		internal UserPhotoSize GetConvertedSizeRequested()
		{
			return ServicePhotoSizeToStoragePhotoSizeConverter.Convert(this.SizeRequested);
		}

		private readonly bool isPostRequest;

		private readonly bool isPreview;

		private readonly bool fallbackToClearImage;
	}
}
