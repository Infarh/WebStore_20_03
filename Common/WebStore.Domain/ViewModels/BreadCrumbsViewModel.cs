namespace WebStore.Domain.ViewModels
{
    public class BreadCrumbsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public BreadCrumbType BreadCrumbType { get; set; }
    }

    public enum BreadCrumbType : byte
    {
        None,
        Section,
        Brand,
        Product
    }
}
