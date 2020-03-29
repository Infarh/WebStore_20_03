using System.Collections.Generic;

namespace WebStore.Domain.Entities
{
    public class ProductFilter
    {
        public int? SectionId { get; set; }

        public int? BrandId { get; set; }

        public IEnumerable<int> Ids { get; set; }
    }
}
