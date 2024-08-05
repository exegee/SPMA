using System;

namespace SPMA.Dtos.Production
{
    public class BookDto
    {
        public int? BookId { get; set; }
        public string OfficeNumber { get; set; }
        public string Name { get; set; }
        // 0 - Inventor, 1 - Manual
        public sbyte Type { get; set; }
        public string Comment { get; set; }
        public string Author { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int Quantity { get; set; }
        public int? Position { get; set; }

        public string ComponentNumber { get; set; }
    }
}
