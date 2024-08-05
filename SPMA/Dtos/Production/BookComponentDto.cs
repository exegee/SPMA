using SPMA.Dtos.Orders;
using SPMA.Dtos.Warehouse;
using SPMA.Models.Orders;

namespace SPMA.Dtos.Production
{
    public class BookComponentDto
    {
        public int? BookComponentId { get; set; }
        public int? BookId { get; set; }
        public BookDto Book { get; set; }

        public int? ComponentId { get; set; }
        public ComponentDto Component { get; set; }

        // LAST SOURCE TYPE USED
        // 0 - standard,
        // 1 - saw,
        // 2 - plasma IN,
        // 3 - plasma OUT without entrusted material,
        // 4 - plasma OUT with entrusted material,
        // 5 - purchase
        public sbyte LastSourceType { get; set; }
        public string LastSourceTypeComment { get; set; }

        public int Quantity { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }

        // ASSOSIATED OPTIMA WARE
        //public int? WareId { get; set; }
        public WareDto Ware { get; set; }

        public int? WareQuantity { get; set; }
        public decimal? WareLength { get; set; }
        public string WareUnit { get; set; }

        // LAST USED IN PRODUCTION TECHNOLOGY
        public string LastTechnology { get; set; }

        // List of components to add
        public ComponentDto[] Components { get; set; }

        public Order MainOrder { get; set; }

        public OrderBookDto OrderBook { get; set; }
    }
}
