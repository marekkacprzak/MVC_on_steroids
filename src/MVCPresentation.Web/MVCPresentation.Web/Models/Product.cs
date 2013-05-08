namespace MVCPresentation.Web.Models
{
    public class Product : IEntity
    {
        public virtual long Id { get; set; }

        public virtual string Name { get; set; }

        public virtual ProductType ProductType { get; set; }
    }
}