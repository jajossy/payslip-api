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
    
    public partial class CompanyStockTag
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CompanyStockTag()
        {
            this.Audit_CompanyStockTag = new HashSet<Audit_CompanyStockTag>();
            this.OrderItems = new HashSet<OrderItem>();
            this.StockIns = new HashSet<StockIn>();
            this.CurrentStocks = new HashSet<CurrentStock>();
            this.Order_CurrentStock = new HashSet<Order_CurrentStock>();
        }
    
        public System.Guid id { get; set; }
        public string Stockname { get; set; }
        public decimal CompanyPrice { get; set; }
        public System.DateTime Setdate { get; set; }
        public string Comment { get; set; }
        public Nullable<System.Guid> CreatedUser { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Audit_CompanyStockTag> Audit_CompanyStockTag { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockIn> StockIns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CurrentStock> CurrentStocks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_CurrentStock> Order_CurrentStock { get; set; }
    }
}
