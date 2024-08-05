using SPMA.Models.Warehouse;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SPMA.Models.Production
{
    public class BookComponent
    {
        public int BookComponentId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int ComponentId { get; set; }
        public Component Component { get; set; }

        // LAST SOURCE TYPE USED
        // 0 - standard,
        // 1 - saw,
        // 2 - plasma IN,
        // 3 - plasma OUT without entrusted material,
        // 4 - plasma OUT with entrusted material,
        // 5 - purchase
        public sbyte? LastSourceType { get; set; }
        public string LastSourceTypeComment { get; set; }

        public int Quantity { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }

        // ASSOSIATED OPTIMA WARE
        public Ware Ware { get; set; }

        public int? WareQuantity { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? WareLength { get; set; }
        public string WareUnit { get; set; }

        // LAST USED IN PRODUCTION TECHNOLOGY
        public string LastTechnology { get; set; }

        // Is additionaly purchasable
        public bool IsAdditionallyPurchasable { get; set; }

        // VIRTUAL MEMBERS
        public virtual ICollection<InProduction> InProduction { get; set; }

    }
}
