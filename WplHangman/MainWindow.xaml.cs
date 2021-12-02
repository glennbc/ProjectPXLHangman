using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WplHangman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HideObject(BtnRaad);
            HideObject(BtnNieuw);
           
        }


        // Buttons hidden of viable zetten
        private void HideObject(Button button)
        {
            button.Visibility = Visibility.Hidden;
        }
        private void ShowObject(Button button)
        {
            button.Visibility = Visibility.Visible;
        }


        // Globale variabelen
        public String woord = "";
        String fout = "";
        String correct = "";
        int levens = 10;


        //Indien men fout heeft geraden
        private void Fout()
        {
            
            levens--;
            PrintLbl();
            TxtInput.Clear();
            SetImage();
        }

        //Indien men Verloren heeft
        private void Verloren()
        {
            SetImage();
            MessageBox.Show("Je bent opgehangen");
            LblText.Content = "Dank U voor het spelen \n";
            LblText.Content += $"Het woord dat wij zochten was: \n\n{woord}";
            TxtInput.Clear();
        }

        //Indien men gewonnen heeft
        private void Gewonnen()
        {
            MessageBox.Show("Correct geraden");
            SetImage();
            LblText.Content = "Dank U voor het spelen\n";
            LblText.Content += $"Het woord dat wij zochten was: \n\n{woord}";
            HideObject(BtnRaad);
            TxtInput.Clear();
        }

        private void SetImage()
        {


            Uri url = new Uri($"{levens}.png",UriKind.Relative);
            //IMG aanmaken als een bitmap
            BitmapImage bitmap = new BitmapImage(url);
            // bitmap toevoegen aan WPF
          


        }

        //alles resetten
        private void Reset()
        {
            TxtInput.Text = "";
            fout = "";
            correct = "";
            levens = 10;
            LblText.Content = "Gelieven een woord in te geven:";
        }

        //Lbl printen
        private void PrintLbl()
        {
            LblText.Content = "";
            LblText.Content += $"U heeft op dit moment {levens} Leven(s)\n\n";
            LblText.Content += $"Correct gerade letters: {correct}\n";
            LblText.Content += $"Fout gerade letters: {fout}";
        }



        //Event als men op Verberg knop klikt
        private void BtnVerberg_Click(object sender, RoutedEventArgs e)
        {
            SetImage();
            woord = TxtInput.Text.ToLower();
            Reset();
            HideObject(BtnVerberg);
            ShowObject(BtnRaad);
            ShowObject(BtnNieuw);
        }

        //Event als men op Nieuw spel klikt
        public void BtnNieuw_Click(object sender, RoutedEventArgs e)
        {
            ShowObject(BtnVerberg);
            HideObject(BtnRaad);
            Reset();

        }



        //Event als men op Raad knop klikt
        private void BtnRaad_Click(object sender, RoutedEventArgs e)
        {
            SetImage();
            TxtInput.Focus();

            if (levens > 1)
            {
                if (TxtInput.Text.Length == 1)
                {
                    if (woord.Contains(TxtInput.Text.ToLower()))
                    {
                        correct += TxtInput.Text.ToLower();
                    }
                    else
                    {
                        fout += TxtInput.Text.ToLower();
                        Fout();
                        
                    }
                    if (ControleWoord() )
                    {
                        Gewonnen();
                    }
                    else
                    {
                        PrintLbl();
                        TxtInput.Clear();                    
                    }
                }
                else
                {
                    if (TxtInput.Text.ToLower() == woord)
                    {
                        Gewonnen();
                    }
                    else
                    {
                        Fout();
                        MessageBox.Show("Fout geraden\nHelaas Pindakaas.");                       
                    }
                }
            }
            else
            {
                Verloren();
            }


        }



        //Kijken als de ingevoerde letters overeen komen met het woor dat we zoeken
        private bool ControleWoord()
        {
           int ControleWoord = 0;
            for (int i = 0; i < woord.Length; i++)
            {
                if (correct.Contains(woord.Substring(i,1)))
                {    //Letter per letter kijken als deze in correct zit

                    ControleWoord++;                            //Als de letter overeenkomt dan tel ik hier 1 bij 
                                                                // bv glenn --> als ik n ingeef is dit Controlewoord +2
                                                                // De som gaat dat automatisch overeenkomen met lengte van het te raden woord
                }

            }

            return ControleWoord == woord.Length;               //als het correct geraden is dan geef ik een true terug
        }
    }
}
