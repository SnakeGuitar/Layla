using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Layla.Desktop.Models;

namespace Layla.Desktop.Views
{
    public partial class NarrativeGraphView : Page
    {
        private readonly List<GraphNode> _mockNodes = new();
        private readonly List<GraphEdge> _mockEdges = new();

        public NarrativeGraphView()
        {
            InitializeComponent();
            InitializeMockData();
            DrawGraph();
        }

        private void InitializeMockData()
        {
            var dianaNode = new GraphNode
            {
                Id = "diana",
                Name = "Diana",
                Subtitle = "Gray Fox",
                Center = new Point(300, 200),
                Radius = 55
            };

            var menudoNode = new GraphNode
            {
                Id = "menudo",
                Name = "Menudo",
                Subtitle = "Terrier",
                Center = new Point(600, 350),
                Radius = 55
            };

            var loveEdge = new GraphEdge
            {
                Source = menudoNode,
                Target = dianaNode,
                Label = "Falls in love with"
            };

            _mockNodes.Add(dianaNode);
            _mockNodes.Add(menudoNode);
            _mockEdges.Add(loveEdge);
        }

        private void DrawGraph()
        {
            GraphCanvas.Children.Clear();

            foreach (var edge in _mockEdges)
            {
                DrawEdge(edge);
            }

            foreach (var node in _mockNodes)
            {
                DrawNode(node);
            }
        }

        private void DrawEdge(GraphEdge edge)
        {
            var line = new Line
            {
                X1 = edge.Source.Center.X,
                Y1 = edge.Source.Center.Y,
                X2 = edge.Target.Center.X,
                Y2 = edge.Target.Center.Y,
                Stroke = (Brush)FindResource("BorderColor"),
                StrokeThickness = 3
            };
            GraphCanvas.Children.Add(line);

            double midX = (line.X1 + line.X2) / 2;
            double midY = (line.Y1 + line.Y2) / 2;

            var border = new Border
            {
                Background = (Brush)FindResource("ControlBackground"),
                BorderBrush = (Brush)FindResource("BorderColor"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(5),
                Padding = new Thickness(5, 2, 5, 2)
            };

            var textLabel = new TextBlock
            {
                Text = edge.Label,
                Foreground = (Brush)FindResource("PrimaryText"),
                FontSize = 12,
                FontWeight = FontWeights.Medium
            };
            border.Child = textLabel;

            border.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(border, midX - (border.DesiredSize.Width / 2));
            Canvas.SetTop(border, midY - (border.DesiredSize.Height / 2));
            
            GraphCanvas.Children.Add(border);
        }

        private void DrawNode(GraphNode node)
        {
            var ellipse = new Ellipse
            {
                Width = node.Radius * 2,
                Height = node.Radius * 2,
                Fill = (Brush)FindResource("AccentColor"),
                Stroke = (Brush)FindResource("BorderColor"),
                StrokeThickness = 3
            };

            Canvas.SetLeft(ellipse, node.Center.X - node.Radius);
            Canvas.SetTop(ellipse, node.Center.Y - node.Radius);
            GraphCanvas.Children.Add(ellipse);

            var stackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var nameText = new TextBlock
            {
                Text = node.Name,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(nameText);

            var subtitleText = new TextBlock
            {
                Text = node.Subtitle,
                Foreground = (Brush)FindResource("SecondaryText"),
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 2, 0, 0)
            };
            stackPanel.Children.Add(subtitleText);

            stackPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(stackPanel, node.Center.X - (stackPanel.DesiredSize.Width / 2));
            Canvas.SetTop(stackPanel, node.Center.Y - (stackPanel.DesiredSize.Height / 2));
            
            GraphCanvas.Children.Add(stackPanel);
        }
    }
}
