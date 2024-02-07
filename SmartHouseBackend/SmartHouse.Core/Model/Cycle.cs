namespace SmartHouse.Core.Model
{
    public enum CycleName
    {
        Cotton,
        EcoWash,
        QuickWash,
        Synthetics,
        Delicate,
        Wool

    }
    public class Cycle
    {
        public Guid Id { get; set; }

        public CycleName Name { get; set; }
        public int Temperature { get; set; }
        public double Duration { get; set; }

    }
}
