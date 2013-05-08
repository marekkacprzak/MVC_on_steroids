namespace MVCPresentation.Web.Models
{
    public abstract class Glossary   : IEntity
    {
        public virtual string Name { get; set; }

        public virtual long Id { get; set; }
    }
}