using SPMA.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SPMA.Models.Production
{
    public class Component
    {
        public int ComponentId { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string MaterialType { get; set; }
        [Column(TypeName = "Money")]
        public decimal? Cost { get; set; }
        public float? Weight { get; set; }
        public string Description { get; set; }

        // COMPONENT TYPE
        // 0 - Part,
        // 1 - Assembly,
        // 2 - Book
        public sbyte ComponentType { get; set; }

        // LAST SOURCE TYPE USED
        // 0 - standard,
        // 1 - saw,
        // 2 - plasma IN,
        // 3 - plasma OUT without entrusted material,
        // 4 - plasma OUT with entrusted material,
        // 5 - purchase
        public sbyte LastSourceType { get; set; }

        public string Comment { get; set; }
        public string Author { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ASSOSIATED OPTIMA WARE
        public Ware Ware { get; set; }

        public int? WareQuantity { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? WareLength { get; set; }
        public string WareUnit { get; set; }

        // LAST USED IN PRODUCTION TECHNOLOGY
        public string LastTechnology { get; set; }

        // ADDITIONAL OPTIONS
        public bool IsAdditionallyPurchasable { get; set; } = false;

        // VIRTUAL MEMBERS
        [IgnoreDataMember]
        public virtual ICollection<BookComponent> BookComponents { get; set; }
        public virtual ICollection<InProduction> InProduction { get; set; }
    }
}
