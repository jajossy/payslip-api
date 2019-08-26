//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BaseWebApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class StockIn
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockIn()
        {
            this.Audit_StockIn = new HashSet<Audit_StockIn>();
        }
    
        public System.Guid id { get; set; }
        public System.Guid CompanyProductNameId { get; set; }
        public System.Guid SupplierId { get; set; }
        public string SupplierProductName { get; set; }
        public Nullable<decimal> SuppliedPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantitySupplied { get; set; }
        public System.DateTime DateSupplied { get; set; }
        public string PackUnit { get; set; }
        public string BatchNo { get; set; }
        public Nullable<System.Guid> CreatedUser { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<bool> Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Audit_StockIn> Audit_StockIn { get; set; }
        public virtual CompanyStockTag CompanyStockTag { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
