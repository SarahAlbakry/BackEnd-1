using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppinMVC6.Models
{
    public class Carts
    {
        public int CarttID { get; set; }
        public string CartName { get; set; }
        public int Quantity { get; set; }
        public float price { get; set; }
        public float Bill { get; set; }
    }
}