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
    
    public partial class CurrentStock
    {
        public System.Guid id { get; set; }
        public System.Guid StockNameId { get; set; }
        public int Quantity { get; set; }
        public string PackUnit { get; set; }
        public double Price { get; set; }
        public bool Status { get; set; }
        public string Comment { get; set; }
    
        public virtual CompanyStockTag CompanyStockTag { get; set; }
    }
}
