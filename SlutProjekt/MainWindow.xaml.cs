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
using System.Numerics;
using System.Windows.Shapes;

namespace SlutProjekt
{
    /// <summary>
    /// Interaction logic for XAML
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool complexActivated = false;

        public void ComplexChecked(object sender, RoutedEventArgs e)
        {
            btnComplex.IsEnabled = false;
            btnReal.IsEnabled = true;
            lblRoots.Content = "Vald talmängd på rötter: Komplexa";
            complexActivated = true;
        }

        public void RealChecked(object sender, RoutedEventArgs e)
        {
            btnComplex.IsEnabled = true;
            btnReal.IsEnabled = false;
            lblRoots.Content = "Vald talmängd för rötter: Reella";
            complexActivated = false;
        }

        public void ABCChecked(object sender, RoutedEventArgs e)
        {
            btnABC.IsEnabled = false;
            btnPQ.IsEnabled = true;
            txtboxA.Visibility = Visibility.Visible;
            lblA.Visibility = Visibility.Visible;
            lblForm.Content = "Vald form: ax² + bx + c = 0";
            lblB.Content = "Välj värde för b";
            lblC.Content = "Välj värde för c";
        }

        public void PQChecked(object sender, RoutedEventArgs e)
        {
            btnABC.IsEnabled = true;
            btnPQ.IsEnabled = false;
            txtboxA.Visibility = Visibility.Collapsed;
            lblA.Visibility = Visibility.Collapsed;
            lblForm.Content = "Vald form: x² + px + q = 0";
            lblB.Content = "Välj värde för p";
            lblC.Content = "Välj värde för q";
        }

        public void Calculate(object sender, RoutedEventArgs e)
        {
            if (!btnABC.IsEnabled)
            {
                if (Helpers.IsDouble(txtboxA.Text) && Helpers.IsDouble(txtboxB.Text) && Helpers.IsDouble(txtboxC.Text))
                {
                    ABCEquation currentEquation = new ABCEquation(Double.Parse(txtboxA.Text), Double.Parse(txtboxB.Text), Double.Parse(txtboxC.Text));
                    lblEquation.Content = Helpers.EquationToString(currentEquation);
                    if (complexActivated == false)
                    {
                        string rootsString = "";
                        double[] roots = Mathematics.FindRealRoots(currentEquation);
                        if (roots != null)
                        {
                            lblSolvedRoots.Content = roots.ToString();
                        }
                        
                    }
                    else
                    {
                        string rootsString = "";
                        Complex[] roots = Mathematics.FindComplexRoots(currentEquation);
                        if (roots != null)
                        {
                            lblSolvedRoots.Content = roots.ToString();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Kan inte beräkna: Värdena måste vara ett reellt tal.\n(OBS: Decimaltal separeras med komma, inte punkt.)");
                }
            }
            else if (!btnPQ.IsEnabled)
            {
                if (Helpers.IsDouble(txtboxB.Text) && Helpers.IsDouble(txtboxC.Text))
                {
                    PQEquation currentEquation = new PQEquation(Double.Parse(txtboxB.Text), Double.Parse(txtboxC.Text));
                    lblEquation.Content = Helpers.EquationToString(currentEquation);
                    if (complexActivated == false)
                    {
                        string rootsString = "";
                        double[] roots = Mathematics.FindRealRoots(currentEquation);
                        if (roots != null)
                        {
                            lblSolvedRoots.Content = roots.ToString();
                        }

                    }
                    else
                    {
                        string rootsString = "";
                        Complex[] roots = Mathematics.FindComplexRoots(currentEquation);
                        if (roots != null)
                        {
                            
                            lblSolvedRoots.Content = rootsString;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Kan inte beräkna: Värdena måste vara ett reellt tal.\n(OBS: Decimaltal separeras med komma, inte punkt.)");
                }
            }
            else
            {
                MessageBox.Show("Kan inte beräkna: Välj en form på ekvationen.");
            }

        }

        public abstract class Equation
        {
            protected double coefficient1;
            protected double coefficient2;

            public Equation(double coefficient1, double coefficient2)
            {
                this.coefficient1 = coefficient1;
                this.coefficient2 = coefficient2;
            }
            public virtual void SetEquation(double coefficient1, double coefficient2, double coefficient3) { }

            public virtual void SetEquation(double coefficient1, double coefficient2) { }

            public abstract double[] GetCoefficients();
            public abstract string GetEquation();
        }

        public class ABCEquation : Equation
        {
            private double coefficient3;

            public ABCEquation(double coefficient1, double coefficient2, double coefficient3) : base(coefficient1, coefficient2)
            {
                this.coefficient3 = coefficient3;
            }

            public override string GetEquation()
            {
                return $"{coefficient1}x² + {coefficient2}x + {coefficient3} = 0";
            }

            public override void SetEquation(double coefficient1, double coefficient2, double coefficient3)
            {

                this.coefficient1 = coefficient1;
                this.coefficient2 = coefficient2;
                this.coefficient3 = coefficient3;
            }

            public override double[] GetCoefficients()
            {
                double[] array = new double[3];
                array[0] = coefficient1;
                array[1] = coefficient2;
                array[2] = coefficient3;
                return array;
            }
        }

        public class PQEquation : Equation
        {
            public PQEquation(double coefficient1, double coefficient2) : base(coefficient1, coefficient2)
            {

            }

            public override string GetEquation()
            {
                return $"x² + {coefficient1}x + {coefficient2} = 0";
            }

            public override void SetEquation(double coefficient1, double coefficient2)
            {
                this.coefficient1 = coefficient1;
                this.coefficient2 = coefficient2;
            }

            public override double[] GetCoefficients()
            {
                double[] array = new double[2];
                array[0] = coefficient1;
                array[1] = coefficient2;
                return array;
            }
        }

        public class Mathematics
        {
            public static double[] FindRealRoots(Equation equation)
            {
                double[] roots;
                string equationType = equation.GetType().Name;

                if (equationType == "ABCEquation")
                {
                    ABCEquation abcEquation = equation as ABCEquation;
                    double[] coefficients = abcEquation.GetCoefficients();
                    double a = coefficients[0];
                    double b = coefficients[1];
                    double c = coefficients[2];

                    double discriminant = b * b - 4 * a * c;

                    if (discriminant < 0)
                    {
                        MessageBox.Show("ERROR: Det finns inga reella rötter.\nTips: Byt till komplexa rötter.");
                        return null;
                    }

                    else if (discriminant > 0)
                    {
                        roots = new double[2];
                        roots[0] = (-b + Math.Sqrt(discriminant)) / (2 * a);
                        roots[1] = (-b - Math.Sqrt(discriminant)) / (2 * a);
                        return roots;
                    }

                    else if (discriminant == 0)
                    {
                        roots = new double[1];
                        roots[0] = (-b / (2 * a));
                        return roots;
                    }


                }
                else
                {

                    PQEquation pqEquation = equation as PQEquation;
                    double[] coefficients = pqEquation.GetCoefficients();
                    double p = coefficients[0];
                    double q = coefficients[1];

                    double discriminant = p * p * 0.25 - q;

                    if (discriminant < 0)
                    {
                        MessageBox.Show("ERROR: Det finns inga reella rötter.\nTips: Byt till komplexa rötter.");
                        return null;
                    }

                    else if (discriminant > 0)
                    {
                        roots = new double[2];
                        roots[0] = (-p / 2 + Math.Sqrt(discriminant));
                        roots[1] = (-p / 2 - Math.Sqrt(discriminant));
                        return roots;
                    }

                    else if (discriminant == 0)
                    {
                        roots = new double[1];
                        roots[0] = (-p / 2);
                        return roots;
                    }
                }
                return null;

            }

            public static Complex[] FindComplexRoots(Equation equation)
            {
                Complex[] rootsComplex = new Complex[2];
                string equationType = equation.GetType().Name;

                if (equationType == "ABCEquation")
                {
                    ABCEquation abcEquation = equation as ABCEquation;
                    double[] coefficients = abcEquation.GetCoefficients();
                    double a = coefficients[0];
                    double b = coefficients[1];
                    double c = coefficients[2];

                    double discriminant = b * b - 4 * a * c;

                    if (discriminant >= 0)
                    {
                        MessageBox.Show("ERROR: Det finns inga komplexa rötter.\nTips: Byt till reella rötter.");
                        return null;
                    }

                    else if (discriminant < 0)
                    {
                        Complex Re = (-b / (2 * a * c));
                        Complex Im = Math.Sqrt(discriminant);

                        rootsComplex[0] = Re + Im * Complex.ImaginaryOne;
                        return rootsComplex;
                    }
                }
                else
                {
                    PQEquation pqEquation = equation as PQEquation;
                    double[] coefficients = pqEquation.GetCoefficients();
                    double p = coefficients[0];
                    double q = coefficients[1];

                    double discriminant = p * p * 0.25 - q;

                    if (discriminant >= 0)
                    {
                        MessageBox.Show("ERROR: Det finns inga komplexa rötter.\nTips: Byt till reella rötter.");
                        return null;
                    }

                    else if (discriminant < 0 && complexActivated)
                    {
                        Complex Re = (-p / 2);
                        Complex Im = Math.Sqrt(discriminant);

                        rootsComplex[0] = Re + Im * Complex.ImaginaryOne;
                        return rootsComplex;
                    }
                }
                return null;
            }


        }
        public class Helpers
        {
            public static bool IsDouble(string input)
            {
                double output = 0;
                if (Double.TryParse(input, out output))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static string EquationToString(Equation equation)
            {
                string output = "";
                if (equation.GetType().Name == "ABCEquation")
                {
                    output = $"{equation.GetCoefficients()[0]}x² + {equation.GetCoefficients()[1]}x + {equation.GetCoefficients()[2]} = 0";
                }
                else if (equation.GetType().Name == "PQEquation")
                {
                    output = $"x² + {equation.GetCoefficients()[0]}x + {equation.GetCoefficients()[1]} = 0";
                }
                return output;
            }
        }
    }
}
