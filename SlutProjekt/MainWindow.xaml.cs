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

namespace SlutProjekt
{
    /// <summary>
    /// Interaction logic for xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool complexActivated = false;

        public void ComplexChecked(object sender, RoutedEventArgs e)
        {
            btnComplex.IsEnabled = false;
            btnReal.IsEnabled = true;
            lblRoots.Content = "Vald talmängd på rötter: Komplexa och Reella";
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

        public abstract class Equation
        {
            protected double coefficient1;
            protected double coefficient2;

            public Equation(double coefficient1, double coefficient2)
            {
                this.coefficient1 = coefficient1;
                this.coefficient2 = coefficient2;
            }

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
                return $"{coefficient1}x^2 + {coefficient2}x + {coefficient3} = 0";
            }
        }

        public class PQEquation : Equation
        {
            public PQEquation(double coefficient1, double coefficient2) : base(coefficient1, coefficient2)
            {

            }

            public override string GetEquation()
            {
                return $"x^2 + {coefficient1}x + {coefficient2} = 0";
            }
        }
        public void Calculate(object sender, RoutedEventArgs e)
        {
            if (!btnABC.IsEnabled)
            {
                if (Helpers.IsDouble(txtboxA.Text) && Helpers.IsDouble(txtboxB.Text) && Helpers.IsDouble(txtboxC.Text))
                {
                    ABCEquation currentEquation = new ABCEquation(Double.Parse(txtboxA.Text), Double.Parse(txtboxB.Text), Double.Parse(txtboxC.Text));
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
        }
    }
}
