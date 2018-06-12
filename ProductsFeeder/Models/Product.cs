namespace ProductsFeeder.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string RealSKU { get; set; }

        [StringLength(255)]
        public string Brand { get; set; }

        [StringLength(255)]
        public string SellerId { get; set; }

        [StringLength(255)]
        public string SKU { get; set; }

        public double? PriceDefault { get; set; }

        public double? CostPrice { get; set; }

        [StringLength(255)]
        public string Active { get; set; }
        
        public DateTime? DateCreated { get; set; }
        public double? Cost { get; set; }
        public string eBayItemID { get; set; }
        public double? LocalizedCustomsDescription { get; set; }
        public int? HarmonizedTariffCode { get; set; }

    }
}
