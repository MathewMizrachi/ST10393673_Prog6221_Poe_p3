using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WpfApp1
{
    public static class TextBoxHelper
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(TextBoxHelper), new PropertyMetadata(string.Empty, OnWatermarkChanged));

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.Loaded -= TextBox_Loaded;
                textBox.Loaded += TextBox_Loaded;
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.TextChanged += TextBox_TextChanged;
                UpdateWatermark(textBox);
            }
        }

        private static void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateWatermark(sender as TextBox);
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateWatermark(sender as TextBox);
        }

        private static void UpdateWatermark(TextBox textBox)
        {
            if (textBox == null) return;

            var watermark = GetWatermark(textBox);

            if (string.IsNullOrEmpty(textBox.Text) && !string.IsNullOrEmpty(watermark))
            {
                var placeholder = new TextBlock
                {
                    Text = watermark,
                    Foreground = Brushes.Gray,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    IsHitTestVisible = false
                };

                if (textBox.Template.FindName("PART_Watermark", textBox) == null)
                {
                    textBox.AddHandler(TextBox.LostFocusEvent, new RoutedEventHandler((s, ea) =>
                    {
                        if (textBox.Template.FindName("PART_Watermark", textBox) == null)
                        {
                            placeholder.Name = "PART_Watermark";
                            var grid = textBox.Parent as Grid;
                            if (grid != null)
                            {
                                grid.Children.Add(placeholder);
                            }
                            else
                            {
                                var parent = textBox.Parent as Panel;
                                if (parent != null)
                                {
                                    parent.Children.Add(placeholder);
                                }
                            }
                        }
                    }), true);
                }
            }
            else
            {
                var parent = textBox.Parent as Panel;
                var watermarkElement = textBox.Template.FindName("PART_Watermark", textBox) as UIElement;
                if (parent != null && watermarkElement != null)
                {
                    parent.Children.Remove(watermarkElement);
                }
            }
        }
    }
}
