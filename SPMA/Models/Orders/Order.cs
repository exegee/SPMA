using SPMA.Models.Production;
using SPMA.Models.Sales;
using SPMA.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SPMA.Models.Orders
{
    public class Order
    {
        public int OrderId { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int PlannedQty { get; set; }
        public int FinishedQty { get; set; } = 0;
        public int? State { get; set; } = 0;

        // ORDER TYPE
        // 0 - External,
        // 1 - Warranty,
        // 2 - Internal
        public sbyte Type { get; set; }

        public string ClientName { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string ShippingName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingRegion { get; set; }
        public string ShippingCountry { get; set; }
        public int? ShippingTypeId { get; set; }
        public int? AddedById { get; set; }
        public string Comment { get; set; }
        public decimal RwCompletion { get; set; }

        public int? ClientId { get; set; }
        public virtual Client Client { get; set; }

        // VIRTUAL MEMBERS
        //[IgnoreDataMember]
        public virtual ICollection<OrderBook> OrderBooks { get; set; }
        //public virtual ICollection<InProduction> InProduction { get; set; }
    }
}
