//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_product()
        {
            this.tbl_order = new HashSet<tbl_order>();
        }
    
        public int pro_id { get; set; }
        public string pro_name { get; set; }
        public string pro_image { get; set; }
        public string pro_des { get; set; }
        public Nullable<int> pro_price { get; set; }
        public Nullable<int> pro_fk_cat { get; set; }
        public Nullable<int> pro_fk_user { get; set; }
    
        public virtual tbl_category tbl_category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_order> tbl_order { get; set; }
        public virtual tbl_user tbl_user { get; set; }
    }
}
