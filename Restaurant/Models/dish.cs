//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Restaurant.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class dish
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dish()
        {
            this.dish_allergen = new HashSet<dish_allergen>();
            this.menu_dish = new HashSet<menu_dish>();
        }
    
        public int id { get; set; }
        public int product_id { get; set; }
        public string measurement_unit { get; set; }
        public double price { get; set; }
        public int portion_quantity { get; set; }
        public int total_quantity { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<dish_allergen> dish_allergen { get; set; }
        public virtual product product { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<menu_dish> menu_dish { get; set; }
    }
}
