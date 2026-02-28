using System.Windows;

namespace Layla.Desktop.Models
{
    public class GraphNode
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public Point Center { get; set; }
        public double Radius { get; set; } = 40;
    }

    public class GraphEdge
    {
        public GraphNode Source { get; set; } = null!;
        public GraphNode Target { get; set; } = null!;
        public string Label { get; set; } = string.Empty;
    }
}
