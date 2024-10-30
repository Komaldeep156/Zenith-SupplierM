using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenith.Repository.DomainModels
{
    public class PaymentTerms:BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CreditTermsId { get; set; }
        public int CreditLimit { get; set; }
        public Guid CreditLimitCurrencyId { get; set; }
        public string DocumentPathURL { get; set; }

        [ForeignKey("CreditTermsId")]
        public virtual DropdownValues DropdownValues_CreditTerms { get; set; }
        [ForeignKey("CreditLimitCurrencyId")]
        public virtual DropdownValues DropdownValues_CreditLimitCurrency { get; set; }

    }
}
