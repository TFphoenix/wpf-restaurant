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
    
    public partial class menu_dish
    {
        public int id { get; set; }
        public int menu_id { get; set; }
        public int dish_id { get; set; }
        public int quantity { get; set; }
    
        public virtual dish dish { get; set; }
        public virtual menu menu { get; set; }
    }
}