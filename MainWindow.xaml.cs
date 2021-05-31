using LiveCharts;
using LiveCharts.Defaults;
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

namespace DifferentialEquations
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ValuesOfXFromYControl.ValueX.Value = 0;
            ValuesOfXFromYControl.ValueY.Value = 1;
            LineSeries.Values = new ChartValues<ObservablePoint>();
        }

        private void UdDegree_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (ValuesOfXFromYControl == null) return;
                ValuesOfXFromYControl.ClearValues();
                
                int degree = (int)UdDegree.Value;
                if (degree > 1) 
                {
                    List<string> namesY = new List<string> { "y'" };
                    for (int i = 1; i < degree - 1; i++)
                    {
                        int prevIndex = namesY.Count - 1;
                        namesY.Add(namesY[prevIndex] + "'");
                    }
                    ValuesOfXFromYControl.AddValues(namesY);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtFindSolution_Click(object sender, RoutedEventArgs e)
        {
            if (TbFunction.Text == string.Empty) MessageBox.Show("Чтобы найти решени, введите функцию", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

            try
            {
                Dictionary<string, Point> values = ValuesOfXFromYControl.GetValues();
                List<Point> resPoints = new List<Point>();
                DifferentialEquation equation = new DifferentialEquation(TbFunction.Text, (double)UdStartInterval.Value, (double)UdEndInterval.Value, (double)UdStep.Value, values);

                switch (CbSelectionMethod.SelectedIndex)
                {
                    case 0:
                        resPoints = equation.EulerMethod();
                        break;
                    case 1:
                        resPoints = equation.EulerMethodRecalculation();
                        break;
                    case 2:
                        resPoints = equation.ItterationEulerMethod(0.01);
                        break;
                    case 3:
                        resPoints = equation.ImprovedEulerMethod();
                        break;
                    case 4:
                        resPoints = equation.RungeKuttaMethod(3);
                        break;
                    case 5:
                        resPoints = equation.RungeKuttaMethod(4);
                        break;
                    case 6:
                        resPoints = equation.AdamsMethod();
                        break;
                    case 7:
                        resPoints = equation.AdamsBushfortMethod();
                        break;
                    case 8:
                        resPoints = equation.AdamsMultonsMethod();
                        break;
                }

                LineSeries.Values.Clear();
                foreach (Point p in resPoints) LineSeries.Values.Add(new ObservablePoint(p.X, p.Y));
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
