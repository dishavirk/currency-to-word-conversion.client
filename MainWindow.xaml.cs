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
using System.ServiceModel;
using System.Diagnostics;
using currency_to_word_conversion.client.ConversionServiceReference;
using System.Globalization;
using System.Text.RegularExpressions;

namespace currency_to_word_conversion.client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConversionServiceReference.ConversionServiceClient conversionServiceClient = new ConversionServiceReference.ConversionServiceClient();
            string inputString = currencyTextBox.Text.Replace(" ", string.Empty);
            bool isValid = validateInput(inputString);

            if (isValid)
            {
                try
                {
                    lbl.Content = "";
                    conversionServiceClient.Open();
                    resultTextBox.Text = conversionServiceClient.convertNumbersToWords(inputString);
                    conversionServiceClient.Close();
                }            
                catch (TimeoutException ex)
                {
                    Console.WriteLine("Service has timed out");
                    Console.WriteLine(ex.Message);
                    conversionServiceClient.Abort();
                }
                catch (FaultException<UnexpectedServiceFault> ex)
                {
                    Console.WriteLine("Service error message: {0}", ex.Detail.Message);
                    Console.WriteLine("Service error description: {0}", ex.Detail.Description);
                    Console.WriteLine("Result: {0}", ex.Detail.Result);
                    conversionServiceClient.Abort();
                }
                catch (FaultException<ValidationFault> ex)
                {
                    Console.WriteLine("Service error message: {0}", ex.Detail.Message);
                    Console.WriteLine("Service error description: {0}", ex.Detail.Description);
                    Console.WriteLine("Result: {0}", ex.Detail.Result);
                    conversionServiceClient.Abort();
                }
                catch (FaultException ex)
                {
                    Console.WriteLine("Service error occurred: {0}", ex.Message);
                    conversionServiceClient.Abort();
                }

                catch (CommunicationException ex)
                {
                    Console.WriteLine("Communications error occurred: {0}", ex.Message);
                    conversionServiceClient.Abort();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error has occurred");
                    Console.WriteLine(ex.Message);
                    conversionServiceClient.Abort();
                }
            }
            else
            {
                lbl.Visibility = Visibility.Visible;
                resultTextBox.Clear();
                lbl.Content = "Please enter a valid input.";
            }         
        }
        public static bool validateInput(string input)
        {
            Regex reg = new Regex("^[0-9]*[,]?[0-9]{0,2}$");

            if (String.IsNullOrEmpty(input) ||
            !reg.IsMatch(input) ||
            input.Equals(",") ||
            input.Split(',')[0].Length > 30 ||
            (input.Split(',').Length > 1 && input.Split(',')[1].Length > 2))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
