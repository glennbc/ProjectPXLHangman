using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HideObject(BtnRaad);
            ShowObject(BtnNieuw);
            HideObject(BtnVerberg);
            SetImage();
            SetBck(true);
            spelActief = false;
            MnCPU.IsChecked = true;
            StartNieuwSpel();
        }
        private void SetBck(bool v)
        {
            ImageBrush bckbrush = new ImageBrush();
            Image image = new Image();
            Uri locatie = new Uri($"Assets/Agrd.jpg", UriKind.Relative);
            image.Source = new BitmapImage(locatie);
            
            RotateTransform rotateTransform = new RotateTransform(180);
            image.RenderTransform = rotateTransform;
            bckbrush.ImageSource = image.Source;
            View.Background = bckbrush;
           
            if (!v)
            {
                locatie = new Uri($"Assets/ArgdFout.jpg", UriKind.Relative);
                image.Source = new BitmapImage(locatie);
                image.RenderTransform = rotateTransform;
                bckbrush.ImageSource = image.Source;
                              View.Background = bckbrush;
                
            }
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
        //beperken van aatal letters
        string[]masking = new string [100];
        int userTimer = 10;
        int timerTickCount = 11;
        int tijd = 1;
        bool spelActief = false;
        //True = 1 VS CPU     False = 1 VS 1
        bool spelModus = true;
        DispatcherTimer timer;
        //Aray met woorden voor met CPU
        private string[] galgjeWoorden = new string[]
{
    "grafeem",
    "tjiftjaf",
    "maquette",
    "kitsch",
    "pochet",
    "convocaat",
    "jakkeren",
    "collaps",
    "zuivel",
    "cesium",
    "voyant",
    "spitten",
    "pancake",
    "gietlepel",
    "karwats",
    "dehydreren",
    "viswijf",
    "flater",
    "cretonne",
    "sennhut",
    "tichel",
    "wijten",
    "cadeau",
    "trotyl",
    "chopper",
    "pielen",
    "vigeren",
    "vrijuit",
    "dimorf",
    "kolchoz",
    "janhen",
    "plexus",
    "borium",
    "ontweien",
    "quiche",
    "ijverig",
    "mecenaat",
    "falset",
    "telexen",
    "hieruit",
    "femelaar",
    "cohesie",
    "exogeen",
    "plebejer",
    "opbouw",
    "zodiak",
    "volder",
    "vrezen",
    "convex",
    "verzenden",
    "ijstijd",
    "fetisj",
    "gerekt",
    "necrose",
    "conclaaf",
    "clipper",
    "poppetjes",
    "looikuip",
    "hinten",
    "inbreng",
    "arbitraal",
    "dewijl",
    "kapzaag",
    "welletjes",
    "bissen",
    "catgut",
    "oxymoron",
    "heerschaar",
    "ureter",
    "kijkbuis",
    "dryade",
    "grofweg",
    "laudanum",
    "excitatie",
    "revolte",
    "heugel",
    "geroerd",
    "hierbij",
    "glazig",
    "pussen",
    "liquide",
    "aquarium",
    "formol",
    "kwelder",
    "zwager",
    "vuldop",
    "halfaap",
    "hansop",
    "windvaan",
    "bewogen",
    "vulstuk",
    "efemeer",
    "decisief",
    "omslag",
    "prairie",
    "schuit",
    "weivlies",
    "ontzeggen",
    "schijn",
    "sousafoon"
};
        //Indien men fout heeft geraden
        private void Fout()
        {
            timer.Stop();
            levens--;
            timerTickCount = userTimer;
            LblTimer.Content = timerTickCount;
            PrintLbl();
            TxtInput.Clear();
            timer.Start();
        }
        //Indien men Verloren heeft
        private void Verloren()
        {
            timer.Stop();
            SetImage();
            MessageBox.Show("Je bent opgehangen");
            LblText.Content = "Dank U voor het spelen \n";
            LblText.Content += $"Het woord dat wij zochten was: \n\n{woord}";
            TxtInput.Clear();
            ShowObject(BtnNieuw);
            HideObject(BtnRaad);
            spelActief = false;
        }
        //Indien men gewonnen heeft
        private void Gewonnen()
        {
            timerTickCount = userTimer;
            LblTimer.Content = timerTickCount;
            timer.Stop();
            MessageBox.Show("Correct geraden");
            SetImage();
            LblText.Content = "Dank U voor het spelen\n";
            LblText.Content += $"Het woord dat wij zochten was: \n\n{woord}";
            HideObject(BtnRaad);
            TxtInput.Clear();
            spelActief = false;
        }
        //Het mannetje tonen aan de hand van de levens
        private void SetImage()
        {
            Uri url = new Uri($"Assets/{levens}.jpg",UriKind.Relative);
            //IMG aanmaken als een bitmap
            BitmapImage bitmap = new BitmapImage(url);
            // bitmap toevoegen aan WPF
            Hangman.Source = bitmap;
        }
        //alles resetten voor 1 VS 1
        private void Reset()
        {
            timerTickCount = userTimer;
            TxtInput.Text = "";
            fout = "";
            correct = "";
            levens = 10;
            SetImage();
                }
              
        //Lbl printen
        private void PrintLbl()
        {
            LblText.Content = "Gelieven een woord of een letter in te geven in het gele vak\nDruk dan op raad\n\n";
            LblText.Content += $"U heeft op dit moment {levens} Leven(s)\n\n";
            LblText.Content += $"Correct gerade letters: {correct}\n";
            LblText.Content += $"Fout gerade letters: {fout}";
            Lblmasking.Content = string.Join("", masking);
        }
        //Event als men op Verberg knop klikt
        private void BtnVerberg_Click(object sender, RoutedEventArgs e)
        {
            if (spelModus)
            {
                //Radom getal voor een random woord te vinden
                Random getal = new Random();
                woord = galgjeWoorden[getal.Next(0, galgjeWoorden.Length)];
                masking = new string[woord.Length];
                Reset();
                Masking();
                

            }
            else
            {
               
                woord = TxtInput.Text.ToLower();
                masking = new string[woord.Length];
                Reset();
                Masking();
                
            }
            SetImage();
          
            Reset();
            HideObject(BtnVerberg);
            ShowObject(BtnRaad);
            ShowObject(BtnNieuw);
            StartTimer();
            mntimer.Visibility = Visibility.Hidden;
            spelActief = true;
          
        }
        //Timers voor spel verloop
        private void StartTimer()
        {
            spelActief = true;
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
            }
            timer.Interval = TimeSpan.FromSeconds(tijd);
            timer.Start();
        }
        async void  timer_Tick(object sender, EventArgs e)
        {

            if (timerTickCount == 0)
            {
                SetBck(false);
                MessageBox.Show("Tijd is op!");
                levens--;
                //timer.Stop();

                timerTickCount= userTimer;
                SetBck(true);
            }
            else
            {
                if (timerTickCount == userTimer) 
                {
                    LblTimer.Content = timerTickCount;
                    timer.Stop();
                    await Task.Delay(1000);
                    timer.Start();
                   
                }

                timerTickCount--;
                
            }
            LblTimer.Content = timerTickCount;
            PrintLbl();

        }
        //Event als men op Nieuw spel klikt
        public void BtnNieuw_Click(object sender, RoutedEventArgs e)
        {

            StartNieuwSpel();

        }
        //Een nieuw spel starten en kijken welke modus het is
        private void StartNieuwSpel()
        {
            if (spelActief)
            {
                timer.Stop();
            }
            if (spelModus)
            {
                VsCpu();
                LblText.Content = $"1 VS CPU \n \nKlik op nieuw spel om te starten.\nJe kan ook de spelmodus aan passen in het menu.\n\nKlik op beginnen\nVul daarna een woord of letter in het gele vak en klik dan op raad.";
            }
            else
            {
                Multispeler();
                LblText.Content = $"1 VS 1 \n \nKlik op nieuw spel om te starten.\nJe kan ook de spelmodus aan passen in het menu.\n\nVul een woord in het gele vak en druk op verberg.\nVul daarna een woord of letter in het gele vak en klik dan op raad.";

            }
        }

        private void VsCpu()
        {                               
            ShowObject(BtnVerberg);
            HideObject(BtnRaad);
            BtnVerberg.Content = "Beginnen";
            Lblmasking.Content = "1 VS CPU";
            woord = "";
           
            
            mntimer.Visibility = Visibility.Visible;
          
        }

        private void Multispeler()
        {
            ShowObject(BtnVerberg);
            HideObject(BtnRaad);
            BtnVerberg.Content = "Verberg";
            Lblmasking.Content = "1 VS 1";
            woord = "";
            mntimer.Visibility = Visibility.Visible;
        }

        //Event als men op Raad knop klikt
        private void BtnRaad_Click(object sender, RoutedEventArgs e)
        {
            Raad();
            spelActief = true;
        }

        private void Raad()
        {
            timer.Start();
          
            TxtInput.Focus();
            if (levens > 1)
            {
                if (fout.Contains(TxtInput.Text.ToLower()) || correct.Contains(TxtInput.Text.ToLower()))
                {

                    timerTickCount = userTimer;
                    timer.Stop();
                    MessageBox.Show($"U heeft lettertje {TxtInput.Text} al ingegeven.");
                    TxtInput.Clear();

                }
                else
                {


                    if (TxtInput.Text.Length == 1)
                    {
                        if (woord.Contains(TxtInput.Text.ToLower()))
                        {
                            correct += TxtInput.Text.ToLower();
                            Masking();
                            timerTickCount = userTimer;
                            timer.Stop();
                        }
                        else
                        {
                            fout += TxtInput.Text.ToLower();
                            Fout();
                            Masking();

                        }
                        if (ControleWoord())
                        {
                            Gewonnen();
                            Masking();

                        }
                        else
                        {
                            Masking();
                            PrintLbl();
                            TxtInput.Clear();
                            timerTickCount = userTimer;
                        }
                    }
                    else
                    {
                        if (TxtInput.Text.ToLower() == woord)
                        {
                            Masking();
                            Gewonnen();
                        }
                        else
                        {
                            Masking();
                            Fout();
                            MessageBox.Show("Fout geraden\nHelaas Pindakaas.");
                            timerTickCount = userTimer;
                        }
                    }
                }
            }
            else
            {
                Verloren();
            }
            
        }

        private void Masking()
        {
            SetImage();
            FillMasking();
            for (int i = 0; i < woord.Length; i++)
            {
                if (correct.Contains(woord.Substring(i, 1)))
                {   
                    masking[i] = woord.Substring(i, 1);
                }
                
            }
           
        }
        private void FillMasking()
        {
            for (int i = 0; i < woord.Length; i++)
            {
                masking[i] = "*";
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

        private void menuAfsluiten(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuNieuw(object sender, RoutedEventArgs e)
        {
            StartNieuwSpel();
        }

        private void menuHigh(object sender, RoutedEventArgs e)
        {

        }

        private void menuHint(object sender, RoutedEventArgs e)
        {

        }

        private void menuTimer(object sender, RoutedEventArgs e)
        {
            if (!spelActief)
            {
                mntimer.IsEnabled = true;
                if (!int.TryParse(Microsoft.VisualBasic.Interaction.InputBox("Geef tijd van de timer"), out userTimer))
                {
                    int.TryParse(Microsoft.VisualBasic.Interaction.InputBox("Geef tijd van de timer in gehele getallen"), out userTimer);
                }
                else
                {
                    if (userTimer < 5 || userTimer > 20)
                    {
                        int.TryParse(Microsoft.VisualBasic.Interaction.InputBox("Geef een tijd in groter dan 5 en kleiner dan 20"), out userTimer);
                    }
                    else
                    {
                        timerTickCount = userTimer;
                    }
                }
            }
            else
            {
                mntimer.IsEnabled = false;
            }
           
          
           

        }

        private void MenuCpu(object sender, RoutedEventArgs e)
        {
            MnCPU.IsChecked = true;
            MnEen.IsChecked = false;
            spelModus = true;
            HideObject(BtnRaad);
            ShowObject(BtnNieuw);
            HideObject(BtnVerberg);
            StartNieuwSpel();
        }

        private void MenuEen(object sender, RoutedEventArgs e)
        {
            MnCPU.IsChecked = false;
            MnEen.IsChecked = true;
            spelModus = false;
            ShowObject(BtnVerberg);
            HideObject(BtnRaad);
            Lblmasking.Content = "1 VS 1";
            mntimer.Visibility = Visibility.Visible;
            StartNieuwSpel();

        }

      
        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Raad();
            }
        }

       
    }
}
