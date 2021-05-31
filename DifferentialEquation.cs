using MathNet.Symbolics;
using Mathos.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DifferentialEquations
{
    class DifferentialEquation
    {
        private double a, b, h;
        private Dictionary<string, Point> conditions;
        private Dictionary<string, FloatingPoint> variables;
        private string function;

        public DifferentialEquation(string function, double a, double b, double h, Dictionary<string, Point> conditions) 
        {
            if (h < 0) throw new Exception("Шаг меньше нуля.");
            if (a > b) throw new Exception("Левая граница области построения больше правой.");
            
            variables = new Dictionary<string, FloatingPoint>
            {
                {"x", conditions["y"].X },
                {"y", conditions["y"].Y }
            };

            foreach (string key in conditions.Keys) 
            {
                if (key == "y") continue;
                variables.Add(key, conditions[key].Y);
            }

            this.conditions = conditions;
            this.function = function;
            this.a = a;
            this.b = b;
            this.h = h;
        }

        private double GetFunctionValue(double x, double y) 
        {
            MathParser mp = new MathParser();

            variables["x"] = x;
            variables["y"] = y;

            foreach (string key in variables.Keys)
            {
                mp.LocalVariables.Add(key, variables[key].RealValue);
            }

            double res = mp.Parse(function);

            return res;
        }

        private List<Point> Yk(int k, List<Point> points)
        {
            List<Point> newPoints = new List<Point> { conditions["y"] };
            if (k == 0) return EulerMethod();
            else
            {
                for (double x = a; x <= b; a += h)
                {
                    double y = points[points.Count - 1].Y;
                    y += (h * (GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y) + GetFunctionValue(x, Yk(k - 1, points)[points.Count].Y)) / 2);
                    newPoints.Add(new Point(x, y));
                }
            }

            return newPoints;
        }

        public List<Point> EulerMethod() 
        {
            List<Point> points = new List<Point> { conditions["y"] };
            for (double x = a; x <= b; x += h)
            {
                double y = points[points.Count - 1].Y + h * GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                points.Add(new Point(x, y));
            }

            return points;
        }

        public List<Point> EulerMethodRecalculation()
        {
            List<Point> points = new List<Point> { conditions["y"] };
            List<Point> approximateValues = EulerMethod();

            for (double x = a; x <= b; x += h)
            {
                double y = points[points.Count - 1].Y;
                y += (h * (GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y) + GetFunctionValue(x, approximateValues[points.Count].Y))) / 2;
                points.Add(new Point(x, y));
            }

            return points;
        }

        public List<Point> ItterationEulerMethod(double e)
        {
            List<Point> points = new List<Point> { conditions["y"] };
            int k = 0;

            do
            {
                double y = points[points.Count - 1].Y;
                y += (h * (GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y) + GetFunctionValue(points[points.Count - 1].X + h, Yk(k, points)[points.Count].Y))) / 2;
                points.Add(new Point(points[points.Count - 1].X + h, y));
            } while ((points[points.Count - 1].Y - points[points.Count - 2].Y) > e);

            return points;
        }

        public List<Point> ImprovedEulerMethod()
        {
            List<Point> points = new List<Point> { conditions["y"] };

            List<Point> halfIndexPoints = new List<Point> { conditions["y"] };
            for (double x = a; x <= b; x += h)
            {
                double y = halfIndexPoints[halfIndexPoints.Count - 1].Y + h / 2 * GetFunctionValue(halfIndexPoints[halfIndexPoints.Count - 1].X, halfIndexPoints[halfIndexPoints.Count - 1].Y);
                halfIndexPoints.Add(new Point(x, y));
            }

            for (double x = a; x <= b; x += h)
            {
                double y = points[points.Count - 1].Y + h * GetFunctionValue(halfIndexPoints[points.Count - 1].X, halfIndexPoints[points.Count - 1].Y);
                points.Add(new Point(x, y));
            }

            return points;
        }

        public List<Point> RungeKuttaMethod(int degree)
        {
            if (degree < 3 || degree > 4) throw new Exception("Порядок метода Рунге-Кутты должен быть равен 3 или 4");

            List<Point> points = new List<Point> { conditions["y"] };

            double[] aArray, cArray, bArray;
            aArray = cArray = bArray = null;
            switch (degree)
            {
                case 3:
                    {
                        aArray = new double[] { 0, (double)1 / 3, (double)2 / 3 };
                        bArray = new double[] { 0, (double)1 / 3, (double)2 / 3 };
                        cArray = new double[] { (double)1 / 4, 0, (double)3 / 4 };
                    }
                    break;
                case 4:
                    {
                        aArray = new double[] { 0, (double)1 / 2, (double)1 / 2, 1 };
                        bArray = new double[] { 0, (double)1 / 2, (double)1 / 2, 1 };
                        cArray = new double[] { (double)1 / 6, (double)1 / 3, (double)1 / 3, (double)1 / 6 };
                    }
                    break;
            }

            double delY = 0;

            for (double x = a; x <= b; x += h)
            {
                double[] kArray = new double[degree];
                for (int i = 0; i < kArray.Length; i++)
                {
                    if (i == 0) kArray[i] = h * GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                    else
                    {
                        kArray[i] = h * GetFunctionValue(x + aArray[i] * h, points[points.Count - 1].Y + h * bArray[i] * kArray[i - 1]);
                    }
                }

                for (int i = 0; i < kArray.Length; i++)
                {
                    delY += kArray[i] * cArray[i];
                }
                double y = points[points.Count - 1].Y + delY;
                points.Add(new Point(x, y));
            }

            return points;
        }

        public List<Point> AdamsMethod()
        {
            List<Point> points = new List<Point> { conditions["y"] };

            for (double x = a; x <= b; x += h)
            {
                double y = points[points.Count - 1].Y;
                double fi = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                double delfi = fi - points[points.Count - 1].Y;
                double delfi2 = delfi - 2 * fi + points[points.Count - 1].Y;
                double delfi3 = delfi2 - 3 * delfi + 3 * fi - points[points.Count - 1].Y;
                y += h * fi + Math.Pow(h, 2) / 2 * delfi + (5 * Math.Pow(h, 3)) / 12 * delfi2 + (3 * Math.Pow(h, 4)) / 8 * delfi3;

                points.Add(new Point(x, y));
            }

            return points;
        }

        public List<Point> AdamsBushfortMethod()
        {
            List<Point> points = new List<Point> { conditions["y"] };

            for (double x = a; x <= b; x += h)
            {
                double f1 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                double y = points[points.Count - 1].Y + h * f1;
                points.Add(new Point(x, y));
                if (x + h > b) break;

                double f2 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                y = points[points.Count - 1].Y + h * ((3 * f2 / 2) - (f1 / 2));
                points.Add(new Point(x, y));
                if (x + 2 * h > b) break;

                double f3 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                y = points[points.Count - 1].Y + h * ((23 * f3 / 12) - (4 * f2 / 3) + (5 * f1 / 12));
                points.Add(new Point(x, y));
                if (x + 3 * h > b) break;

                double f4 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                y = points[points.Count - 1].Y + h * (55 * f4 / 24 - 59 * f3 / 24 + 37 * f2 / 24 - 3 * f1 / 8);
                points.Add(new Point(x, y));
                if (x + 4 * h > b) break;

                double f5 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                y = points[points.Count - 1].Y + h * (1901 * f5 / 720 - 1387 * f4 / 360 + 109 * f3 / 30 - 637 * f2 / 360 + 251 * f1 / 720);
                points.Add(new Point(x, y));
                x += 4 * h;
            }

            return points;
        }

        public List<Point> AdamsMultonsMethod()
        {
            List<Point> points = new List<Point> { conditions["y"] };

            for (double x = a; x <= b; x += h)
            {
                double f1 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                double y = points[points.Count - 1].Y + h * f1;
                points.Add(new Point(x, y));
                if (x + h > b) break;

                double f2 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                y = points[points.Count - 1].Y + h * (f2 + f1) / 2;
                points.Add(new Point(x, y));
                if (x + 2 * h > b) break;

                double f3 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                y = points[points.Count - 1].Y + h * ((5 * f3 / 12) + (2 * f2 / 3) - (f1 / 12));
                points.Add(new Point(x, y));
                if (x + 3 * h > b) break;

                double f4 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                y = points[points.Count - 1].Y + h * (3 * f4 / 8 + 19 * f3 / 24 - 5 * f2 / 24 + f1 / 24);
                points.Add(new Point(x, y));
                if (x + 4 * h > b) break;

                double f5 = GetFunctionValue(points[points.Count - 1].X, points[points.Count - 1].Y);
                y = points[points.Count - 1].Y + h * (251 * f5 / 720 + 646 * f4 / 720 - 264 * f3 / 720 + 104 * f2 / 720 - 19 * f1 / 720);
                points.Add(new Point(x, y));
                x += 4 * h;
            }

            return points;
        }

    }
}
