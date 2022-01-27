namespace AupInfo.Core
{
    public class ExEditFigure
    {
        public string Name { get; }
        public string Kind { get; }

        public ExEditFigure(string name, string kind)
        {
            Name = name;
            Kind = kind;
        }

        public override bool Equals(object? obj)
        {
            return obj is ExEditFigure fig
                && Name == fig.Name
                && Kind == fig.Kind;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Kind.GetHashCode();
        }
    }
}
