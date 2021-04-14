using System;
using System.Collections;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SubmissionHelper
	{
		internal SubmissionHelper() : this(null, PhoneNumber.Empty, Guid.Empty, null, null, null, null, Guid.Empty)
		{
		}

		internal SubmissionHelper(string callId, PhoneNumber callerId, Guid recipientObjectGuid, string recipientName, string cultureInfo, string callerAddress, string callerName, Guid tenantGuid) : this(callId, callerId, recipientObjectGuid, recipientName, cultureInfo, callerAddress, callerName, new Hashtable(), null, tenantGuid)
		{
		}

		internal SubmissionHelper(string callId, PhoneNumber callerId, Guid recipientObjectGuid, string recipientName, string cultureInfo, string callerAddress, string callerName, string callerIdDisplayName, Guid tenantGuid) : this(callId, callerId, recipientObjectGuid, recipientName, cultureInfo, callerAddress, callerName, new Hashtable(), callerIdDisplayName, tenantGuid)
		{
		}

		private SubmissionHelper(string callId, PhoneNumber callerId, Guid recipientObjectGuid, string recipientName, string cultureInfo, string callerAddress, string callerName, Hashtable customHeaders, string callerIdDisplayName, Guid tenantGuid)
		{
			this.callId = callId;
			this.callerId = callerId;
			this.callerName = callerName;
			this.customHeaders = customHeaders;
			this.recipientObjectGuid = recipientObjectGuid;
			this.cultureInfo = cultureInfo;
			this.recipientName = recipientName;
			this.callerAddress = callerAddress;
			this.callerIdDisplayName = callerIdDisplayName;
			this.TenantGuid = tenantGuid;
		}

		public Guid TenantGuid { get; set; }

		internal string CallId
		{
			get
			{
				return this.callId;
			}
			set
			{
				this.callId = value;
			}
		}

		internal PhoneNumber CallerId
		{
			get
			{
				return this.callerId;
			}
			set
			{
				this.callerId = value;
			}
		}

		internal string CallerName
		{
			get
			{
				return this.callerName;
			}
			set
			{
				this.callerName = value;
			}
		}

		internal Hashtable CustomHeaders
		{
			get
			{
				return this.customHeaders;
			}
			set
			{
				this.customHeaders = value;
			}
		}

		internal Guid RecipientObjectGuid
		{
			get
			{
				return this.recipientObjectGuid;
			}
			set
			{
				this.recipientObjectGuid = value;
			}
		}

		internal string CultureInfo
		{
			get
			{
				return this.cultureInfo;
			}
			set
			{
				this.cultureInfo = value;
			}
		}

		internal string RecipientName
		{
			get
			{
				return this.recipientName;
			}
			set
			{
				this.recipientName = value;
			}
		}

		internal string CallerAddress
		{
			get
			{
				return this.callerAddress;
			}
			set
			{
				this.callerAddress = value;
			}
		}

		internal string CallerIdDisplayName
		{
			get
			{
				return this.callerIdDisplayName;
			}
			set
			{
				this.callerIdDisplayName = value;
			}
		}

		private string callId;

		private PhoneNumber callerId;

		private string callerIdDisplayName;

		private Guid recipientObjectGuid;

		private string recipientName;

		private string cultureInfo;

		private string callerAddress;

		private string callerName;

		private Hashtable customHeaders;
	}
}
