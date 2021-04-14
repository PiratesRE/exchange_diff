using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	[DataServiceKey("objectId")]
	public class SubscribedSku
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static SubscribedSku CreateSubscribedSku(string objectId, Collection<ServicePlanInfo> servicePlans)
		{
			SubscribedSku subscribedSku = new SubscribedSku();
			subscribedSku.objectId = objectId;
			if (servicePlans == null)
			{
				throw new ArgumentNullException("servicePlans");
			}
			subscribedSku.servicePlans = servicePlans;
			return subscribedSku;
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string capabilityStatus
		{
			get
			{
				return this._capabilityStatus;
			}
			set
			{
				this._capabilityStatus = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public int? consumedUnits
		{
			get
			{
				return this._consumedUnits;
			}
			set
			{
				this._consumedUnits = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string objectId
		{
			get
			{
				return this._objectId;
			}
			set
			{
				this._objectId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public LicenseUnitsDetail prepaidUnits
		{
			get
			{
				if (this._prepaidUnits == null && !this._prepaidUnitsInitialized)
				{
					this._prepaidUnits = new LicenseUnitsDetail();
					this._prepaidUnitsInitialized = true;
				}
				return this._prepaidUnits;
			}
			set
			{
				this._prepaidUnits = value;
				this._prepaidUnitsInitialized = true;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Collection<ServicePlanInfo> servicePlans
		{
			get
			{
				return this._servicePlans;
			}
			set
			{
				this._servicePlans = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public Guid? skuId
		{
			get
			{
				return this._skuId;
			}
			set
			{
				this._skuId = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string skuPartNumber
		{
			get
			{
				return this._skuPartNumber;
			}
			set
			{
				this._skuPartNumber = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _capabilityStatus;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private int? _consumedUnits;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _objectId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private LicenseUnitsDetail _prepaidUnits;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private bool _prepaidUnitsInitialized;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Collection<ServicePlanInfo> _servicePlans = new Collection<ServicePlanInfo>();

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private Guid? _skuId;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _skuPartNumber;
	}
}
