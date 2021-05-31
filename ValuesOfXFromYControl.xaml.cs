using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace DifferentialEquations
{
    /// <summary>
    /// Логика взаимодействия для ValuesOfXFromYControl.xaml
    /// </summary>
    public partial class ValuesOfXFromYControl : UserControl
    {
        public ValuesOfXFromYControl()
        {
            InitializeComponent();
        }

        public Dictionary<string, Point> GetValues() 
        {
            Dictionary<string, Point> dictionary = new Dictionary<string, Point>();
            for (int i = 0; i < MainGrid.Children.Count; i++)
            {
                StackPanel panel = MainGrid.Children[i] as StackPanel;
                string startY = (panel?.Children[0] as Label)?.Content.ToString();
                
                string nameY = startY.Remove(startY.IndexOf('('));
                double? valueX = (panel?.Children[1] as DoubleUpDown)?.Value;
                double? valueY = (panel?.Children[3] as DoubleUpDown)?.Value;

                if (nameY != string.Empty && valueX != null && valueY != null) 
                {
                    dictionary.Add(nameY, new Point((double)valueX, (double)valueY));
                }

            }

            return dictionary;
        }

        public void AddValues(List<string> namesY)
        {
            for (int i = 0; i < namesY.Count; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition());

                StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
                panel.Children.Add(new Label { Content = namesY[i] + "(" });
                panel.Children.Add(new DoubleUpDown());
                panel.Children.Add(new Label { Content = ")=" });
                panel.Children.Add(new DoubleUpDown());

                MainGrid.Children.Add(panel);
                Grid.SetRow(panel, MainGrid.RowDefinitions.Count - 1);
            }
        }

        public void ClearValues() 
        {
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            AddValues(new List<string> { "y" });
        }
    }
}
