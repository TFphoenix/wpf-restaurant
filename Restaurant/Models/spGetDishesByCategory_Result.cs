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
    
    public partial class spGetDishesByCategory_Result
    {
        public int id { get; set; }
        public string name { get; set; }
        public string category_code { get; set; }
        public string photos_source { get; set; }
        public bool is_menu { get; set; }
        public string measurement_unit { get; set; }
        public double price { get; set; }
        public int portion_quantity { get; set; }
        public int total_quantity { get; set; }
    }
}