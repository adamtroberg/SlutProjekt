using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Bool som används för att bestämma ifall användaren vill ha komplexa rötter eller inte.
        static bool complexActivated = false;

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        /// <summary>
        /// 
        /// </summary>
        public class MainViewModel
        {
            
            public EquationList<Equation> Equations { get; } = new EquationList<Equation>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Eventhandler för när knappen för komplexa tal trycks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ComplexChecked(object sender, RoutedEventArgs e)
        {
            // Knappen disableas och knappen för reella tal enableas, samt text ändras i UIn och en bool för komplexa tal görs till true.
            btnComplex.IsEnabled = false;
            btnReal.IsEnabled = true;
            lblRoots.Content = "Vald talmängd på rötter: Komplexa";
            complexActivated = true;
        }

        /// <summary>
        /// Eventhandler för när knappen för reella tal trycks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RealChecked(object sender, RoutedEventArgs e)
        {
            // Knappen disableas och knappen för komplexa tal enableas, samt text ändras i UIn och en bool för komplexa tal görs till false.
            btnComplex.IsEnabled = true;
            btnReal.IsEnabled = false;
            lblRoots.Content = "Vald talmängd för rötter: Reella";
            complexActivated = false;
        }

        /// <summary>
        /// Eventhandler för när knappen för ABCEkvation trycks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ABCChecked(object sender, RoutedEventArgs e)
        {
            // Knappen disableas och knappen för PQekvationer aktiveras. Samt text och andra saker i UIn ändras.
            btnABC.IsEnabled = false;
            btnPQ.IsEnabled = true;
            txtboxA.Visibility = Visibility.Visible;
            lblA.Visibility = Visibility.Visible;
            lblForm.Content = "Vald form: ax² + bx + c = 0";
            lblB.Content = "Välj värde för b";
            lblC.Content = "Välj värde för c";
        }

        /// <summary>
        /// Eventhandler för när knappen för PQEkvation trycks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PQChecked(object sender, RoutedEventArgs e)
        {
            // Knappen disableas och knappen för ABCekvationer aktiveras. Samt text och andra saker i UIn ändras.
            btnABC.IsEnabled = true;
            btnPQ.IsEnabled = false;
            txtboxA.Visibility = Visibility.Collapsed;
            lblA.Visibility = Visibility.Collapsed;
            lblForm.Content = "Vald form: x² + px + q = 0";
            lblB.Content = "Välj värde för p";
            lblC.Content = "Välj värde för q";
        }

        /// <summary>
        /// Eventhandler för när knappen för att beräkna rötterna trycks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Calculate(object sender, RoutedEventArgs e)
        {
            // Om det är en ekvation på formen ABC.
            if (!btnABC.IsEnabled)
            {
                // Kolla att alla värden som matats in är double.
                if (Helpers.IsDouble(txtboxA.Text) && Helpers.IsDouble(txtboxB.Text) && Helpers.IsDouble(txtboxC.Text))
                {
                    // Skapa en instans av ABCEkvation genom att parsea värdena i UIn.
                    ABCEquation currentEquation = new ABCEquation(Double.Parse(txtboxA.Text), Double.Parse(txtboxB.Text), Double.Parse(txtboxC.Text));
                    // Visa vilken ekvation det är på UIn.
                    lblEquation.Content = currentEquation.EquationString;
                    // Ifall det söks reella rötter.
                    if (complexActivated == false)
                    {
                        // Calla functionen som räknar ut och returnar rötterna.
                        double[] roots = Mathematics.FindRealRoots(currentEquation);
                        // Om den hittade rötter.
                        if (roots != null)
                        {
                            // Ifall det är en "dubbelrot, d.v.s endast en lösning"
                            if (roots.Length == 1)
                            {
                                lblSolvedRoots.Content = $"x = {roots[0]}";
                            }
                            // 2 rötter
                            else 
                            {
                                lblSolvedRoots.Content = $"x₁ = {roots[0]}, x₂ = {roots[1]}";
                            }
                           
                        }
                    }
                    // Ifall det är komplexa rötter som söks.
                    else
                    {
                        // Calla functionen som räknar ut och returnar rötterna.
                        Complex roots = Mathematics.FindComplexRoots(currentEquation);
                        // Om den hittade rötter.
                        if (roots != 0)
                        {
                            // Skriv ut rötterna i UIn. 
                            lblSolvedRoots.Content = $"x₁ = {roots.Real} + {roots.Imaginary}i\nx₂ = {roots.Real} - {roots.Imaginary}i";
                        }
                    }
                    ((MainViewModel)DataContext).Equations.AddEquation(currentEquation);
                }
                else
                {
                    // Felmeddelande ifall det matats in ogiltiga värden.
                    MessageBox.Show("Kan inte beräkna: Värdena måste vara ett reellt tal.\n(OBS: Decimaltal separeras med komma, inte punkt.)");
                }
            }
            // Om det är en ekvation på formen PQ
            else if (!btnPQ.IsEnabled)
            {
                // Kolla att alla värden som matats in är double.
                if (Helpers.IsDouble(txtboxB.Text) && Helpers.IsDouble(txtboxC.Text))
                {
                    // Skapa en instans av PQEkvation genom att parsea värdena i UIn.
                    PQEquation currentEquation = new PQEquation(Double.Parse(txtboxB.Text), Double.Parse(txtboxC.Text));
                    // Visa vilken ekvation det är på UIn.
                    lblEquation.Content = currentEquation.EquationString;
                    // Om det är reella rötter som söks.
                    if (complexActivated == false)
                    {
                        // Calla functionen som räknar ut och returnar rötterna.
                        double[] roots = Mathematics.FindRealRoots(currentEquation);
                        // Om den hittade rötter.
                        if (roots != null)
                        {
                            // Om det är en "dubbelrot"
                            if (roots.Length == 1)
                            {
                                lblSolvedRoots.Content = $"x = {roots[0]}";
                            }
                            // Om det finns två lösningar.
                            else
                            {
                                lblSolvedRoots.Content = $"x₁ = {roots[0]}, x₂ = {roots[1]}";
                            }
                        }

                    }
                    // Om det är komplexa rötter.
                    else
                    {
                        // Calla functionen som hittar rötterna.
                        Complex roots = Mathematics.FindComplexRoots(currentEquation);
                        // Om den hittade rötter.
                        if (roots != 0)
                        {
                            // Skriv ut rötterna i UIn.
                            lblSolvedRoots.Content = $"x₁ = {roots.Real} + {roots.Imaginary}i\nx₂ = {roots.Real} - {roots.Imaginary}i";
                        }
                    }
                    ((MainViewModel)DataContext).Equations.AddEquation(currentEquation);
                }
                else
                {
                    // Felmeddelande Om det som matats in inte var giltiga värden.
                    MessageBox.Show("Kan inte beräkna: Värdena måste vara ett reellt tal.\n(OBS: Decimaltal separeras med komma, inte punkt.)");
                }
            }
            else
            {
                // Felmeddelande om en form på ekvationen inte valts.
                MessageBox.Show("Kan inte beräkna: Välj en form på ekvationen.");
            }

        }

        /// <summary>
        /// Abstrakt klass som är basen till ABC- och PQEkvation.
        /// </summary>
        public abstract class Equation
        {
            // Två av koefficienterna (tredje som behövs för ABC finns i ABC klassen)
            protected double coefficient1;
            protected double coefficient2;

            // Constructor för ekvation.
            public Equation(double coefficient1, double coefficient2)
            {
                this.coefficient1 = coefficient1;
                this.coefficient2 = coefficient2;
            }
            // Setter som använder sig av polymorfi (kan ta emot olika antal parametrar)
            public virtual void SetEquation(double coefficient1, double coefficient2, double coefficient3) { }

            public virtual void SetEquation(double coefficient1, double coefficient2) { }
            
            // Getter för koefficienterna
            public abstract double[] GetCoefficients();
            // Getter för ekvationen.
            public abstract string GetEquation();

            // Attribut som kör GetEquation (behövs för att kunna få EquationString från UI filen.
            public string EquationString => GetEquation();
        }

        /// <summary>
        /// Klass för ABCEkvationer som bygger på basklassen Equation.
        /// </summary>
        public class ABCEquation : Equation
        {
            // Lägger till den tredje koefficienten.
            private double coefficient3;

            // Constructor för ABC Ekvation.
            public ABCEquation(double coefficient1, double coefficient2, double coefficient3) : base(coefficient1, coefficient2)
            {
                this.coefficient3 = coefficient3;
            }
            
            // Override för GetEquation() som lägger till tredje koefficienten.
            public override string GetEquation()
            {
                return $"{coefficient1}x² + {coefficient2}x + {coefficient3} = 0";
            }

            // Setter för koefficienterna.
            public override void SetEquation(double coefficient1, double coefficient2, double coefficient3)
            {

                this.coefficient1 = coefficient1;
                this.coefficient2 = coefficient2;
                this.coefficient3 = coefficient3;
            }

            // Getter för koefficienterna som returnar koefficienterna i en array.
            public override double[] GetCoefficients()
            {
                double[] array = new double[3];
                array[0] = coefficient1;
                array[1] = coefficient2;
                array[2] = coefficient3;
                return array;
            }
        }

        /// <summary>
        /// Klass för PQEquation som bygger på basklassen Equation.
        /// </summary>
        public class PQEquation : Equation
        {
            // Constructor för PQEquation.
            public PQEquation(double coefficient1, double coefficient2) : base(coefficient1, coefficient2)
            {

            }

            // Getter som returnar en string för ekvationen.
            public override string GetEquation()
            {
                return $"x² + {coefficient1}x + {coefficient2} = 0";
            }

            // Setter för koefficienterna.
            public override void SetEquation(double coefficient1, double coefficient2)
            {
                this.coefficient1 = coefficient1;
                this.coefficient2 = coefficient2;
            }

            // Getter för koefficienterna som returnar en array.
            public override double[] GetCoefficients()
            {
                double[] array = new double[2];
                array[0] = coefficient1;
                array[1] = coefficient2;
                return array;
            }
        }

        /// <summary>
        /// Klass där all matematik sker.
        /// </summary>
        public class Mathematics
        {
            /// <summary>
            /// Metod för att hitta reella rötter till en ekvation.
            /// </summary>
            /// <param name="equation"></param>
            /// <returns></returns>
            public static double[] FindRealRoots(Equation equation)
            {
                // Array för rötterna samt bestäm vilken typ av ekvation.
                double[] roots;
                string equationType = equation.GetType().Name;

                // Ifall det är en ekvation på formen ABC.
                if (equationType == "ABCEquation")
                {
                    // Läs av koefficienterna av ABCEquation.
                    ABCEquation abcEquation = equation as ABCEquation;
                    double[] coefficients = abcEquation.GetCoefficients();
                    double a = coefficients[0];
                    double b = coefficients[1];
                    double c = coefficients[2];

                    // Bestäm diskriminanten.
                    double discriminant = b * b - 4 * a * c;

                    // Om diskriminanten är mindre än noll finns inga reella rötter, därför ska det skickas ett felmeddelande.
                    if (discriminant < 0)
                    {
                        MessageBox.Show("ERROR: Det finns inga reella rötter.\nTips: Byt till komplexa rötter.");
                        return null;
                    }

                    // Om diskriminanten är större än noll kommer det att finnas två rötter.
                    else if (discriminant > 0)
                    {
                        roots = new double[2];
                        roots[0] = Math.Round((-b + Math.Sqrt(discriminant)) / (2 * a), 4);
                        roots[1] = Math.Round((-b - Math.Sqrt(discriminant)) / (2 * a), 4);
                        return roots;
                    }

                    // Om diskrimanten är noll kommer det bara att finnas en rot.
                    else if (discriminant == 0)
                    {
                        // Rötterna returnar i en array och beräknas med enkel matematik.
                        roots = new double[1];
                        roots[0] = Math.Round((-b / (2 * a)), 4);
                        return roots;
                    }
                }
                else // Ifall det är en ekvation på formen PQ.
                {
                    // Läs av koefficienterna av PQEquation.
                    PQEquation pqEquation = equation as PQEquation;
                    double[] coefficients = pqEquation.GetCoefficients();
                    double p = coefficients[0];
                    double q = coefficients[1];

                    // Bestäm diskriminanten.
                    double discriminant = p * p * 0.25 - q;

                    // Om diskriminanten är mindre än noll finns inga reella rötter, därför ska det skickas ett felmeddelande.
                    if (discriminant < 0)
                    {
                        MessageBox.Show("ERROR: Det finns inga reella rötter.\nTips: Byt till komplexa rötter.");
                        return null;
                    }

                    // Om diskriminanten är större än noll kommer det att finnas två rötter.
                    else if (discriminant > 0)
                    {
                        // Rötterna returnar i en array och beräknas med enkel matematik.
                        roots = new double[2];
                        roots[0] = Math.Round((-p / 2 + Math.Sqrt(discriminant)), 4);
                        roots[1] = Math.Round((-p / 2 - Math.Sqrt(discriminant)), 4);
                        return roots;
                    }

                    // Om diskriminanten är noll finns det endast en rot.
                    else if (discriminant == 0)
                    {
                        // Roten returnas i en array beräknas med enkel matematik.
                        roots = new double[1];
                        roots[0] = Math.Round((-p / 2), 4);
                        return roots;
                    }
                }
                return null;

            }

            /// <summary>
            /// Metod för att beräkna komplexa rötter.
            /// </summary>
            /// <param name="equation"></param>
            /// <returns></returns>
            public static Complex FindComplexRoots(Equation equation)
            {
                // Skapa ett nytt komplext tal.
                Complex rootsComplex = new Complex();
                // Bestäm vilken typ av ekvation.
                string equationType = equation.GetType().Name;

                // Om det är en ABC Ekvation.
                if (equationType == "ABCEquation")
                {
                    // Ta in alla värden från ekvatoinen.
                    ABCEquation abcEquation = equation as ABCEquation;
                    double[] coefficients = abcEquation.GetCoefficients();
                    double a = coefficients[0];
                    double b = coefficients[1];
                    double c = coefficients[2];

                    // Bestäm värdeti i diskriminaten)
                    double discriminant = b * b - 4 * a * c;

                    // Om diskriminanten är lika med eller större än noll finns det inga komplexa rötter
                    if (discriminant >= 0)
                    {
                        // felmeddelande.
                        MessageBox.Show("ERROR: Det finns inga komplexa rötter.\nTips: Byt till reella rötter.");
                        return 0;
                    }

                    // Om diskriminaten är mindre än noll finns det komplexa rötter.
                    else if (discriminant < 0)
                    {
                        // Reella delen.
                        Complex Re = Math.Round((-b / (2 * a)), 4);
                        // Imaginära delen.
                        Complex Im = Math.Round(Math.Sqrt(Math.Abs(discriminant)), 4) * Complex.ImaginaryOne;

                        // Skapa det komplexa talet och returna.
                        rootsComplex = Complex.Add(Re, Im);
                        return rootsComplex;
                    }
                }
                else // Om det är en PQ Ekvation.
                {
                    // Hämta in alla värdena från PQEkvationen.
                    PQEquation pqEquation = equation as PQEquation;
                    double[] coefficients = pqEquation.GetCoefficients();
                    double p = coefficients[0];
                    double q = coefficients[1];

                    // Bestäm diskriminaten
                    double discriminant = p * p * 0.25 - q;

                    // Om diskriminanten är lika med eller större än noll finns det inga komplexa rötter
                    if (discriminant >= 0)
                    {
                        // Felmeddelande.
                        MessageBox.Show("ERROR: Det finns inga komplexa rötter.\nTips: Byt till reella rötter.");
                        return 0;
                    }

                    // Om diskriminanten är mindre än noll finns det komplexa rötter.
                    else if (discriminant < 0)
                    {
                        // Reella delen.
                        Complex Re = Math.Round(-p / 2, 4);
                        // Imaginära delen.
                        Complex Im = Math.Round(Math.Sqrt(Math.Abs(discriminant)), 4) * Complex.ImaginaryOne;

                        // Skapa komplexa talet och returna det.
                        rootsComplex = Complex.Add(Re, Im);
                        return rootsComplex;
                    }
                }
                return 0;
            }


        }

        /// <summary>
        /// En egen generisk klass för att lagra ekvationer i en lista.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class EquationList<T> where T : Equation
        {
            private List<T> equations;

            // Constructor för listan.
            public EquationList()
            {
                equations = new List<T>();
            }

            // Metod som lägger till ekvation i listan.
            public void AddEquation(T equation)
            {
                equations.Add(equation);
            }

            // Metod som tar bort ekvation från listan.
            public void RemoveEquation(T equation)
            {
                equations.Remove(equation);
            }

            // Metod som clearar listan.
            public void ClearEquations()
            {
                equations.Clear();
            }

            // Metod som returnar alla ekvatoiner i listan.
            public List<T> GetEquations()
            {
                return equations;
            }
        }

        /// <summary>
        /// En klass som innehåller vanliga algoritmer.
        /// </summary>
        public static class Helpers
        {
            /// <summary>
            /// Om inputen är en double returnar metoden true.
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static bool IsDouble(string input)
            {
                // Double för att parsea.
                double output = 0;
                // Försök att parsea inputen.
                if (Double.TryParse(input, out output))
                {
                    // Om det gick return true.
                    return true;
                }
                else
                {
                    // Om det inte gick return false.
                    return false;
                }
            }
        }
    }
}
