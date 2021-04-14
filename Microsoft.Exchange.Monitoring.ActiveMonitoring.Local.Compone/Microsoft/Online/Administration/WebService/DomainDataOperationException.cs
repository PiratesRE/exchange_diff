using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[KnownType(typeof(SetUnverifiedDomainAsDefaultException))]
	[KnownType(typeof(DomainOverlappingOperationException))]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DomainDataOperationException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[KnownType(typeof(DomainUnexpectedAuthenticationException))]
	[KnownType(typeof(DomainCapabilitySetException))]
	[KnownType(typeof(DomainLiveNamespaceExistsException))]
	[KnownType(typeof(DomainNotRootException))]
	[KnownType(typeof(DomainLiveNamespaceAuthenticationException))]
	[KnownType(typeof(DomainLiveNamespaceUriConflictException))]
	[KnownType(typeof(DomainNameForbiddenWordException))]
	[KnownType(typeof(DomainPendingDeletionException))]
	[KnownType(typeof(DomainVerificationMissingCnameException))]
	[KnownType(typeof(DomainVerificationWrongCnameTargetException))]
	[KnownType(typeof(DomainVerificationMissingDnsRecordException))]
	[KnownType(typeof(DomainLiveNamespaceUnmanagedException))]
	[KnownType(typeof(DomainCapabilityUnsetException))]
	[KnownType(typeof(DefaultDomainDeletionException))]
	[KnownType(typeof(InitialDomainDeletionException))]
	[KnownType(typeof(DomainHasChildDomainException))]
	[KnownType(typeof(DomainNotEmptyException))]
	[KnownType(typeof(DefaultDomainUnsetException))]
	[KnownType(typeof(DefaultDomainInvalidAuthenticationException))]
	[KnownType(typeof(InitialDomainUpdateException))]
	[KnownType(typeof(DomainCapabilityUnavailableException))]
	public class DomainDataOperationException : DataOperationException
	{
		[DataMember]
		public string DomainName
		{
			get
			{
				return this.DomainNameField;
			}
			set
			{
				this.DomainNameField = value;
			}
		}

		private string DomainNameField;
	}
}
