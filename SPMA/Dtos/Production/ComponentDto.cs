using SPMA.Dtos.Warehouse;
using System;

namespace SPMA.Dtos.Production
{
    public class ComponentDto
    {
        public int ComponentId { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public string MaterialType { get; set; }
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
        public sbyte? LastSourceType { get; set; }

        public string Comment { get; set; }
        public string Author { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // ASSOSIATED OPTIMA WARE
        //public int? WareId { get; set; }
        public WareDto Ware { get; set; }

        public int? WareQuantity { get; set; }
        public decimal? WareLength { get; set; }
        public string WareUnit { get; set; }

        // LAST USED IN PRODUCTION TECHNOLOGY
        public string LastTechnology { get; set; }

        public int Quantity { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }
        public bool ExistInDatabase { get; set; }
        public int SumQuantity { get; set; }
        public bool ToProduction { get; set; } = true;
        public string TreeNumber { get; set; }
        public int SinglePieceQty { get; set; }

        // ADDITIONAL OPTIONS
        public bool IsAdditionallyPurchasable { get; set; }

        public int reservedItemId { get; set; } = -1;
        public int reservedQty { get; set; } = 0;

    }
}
