﻿using System;
using System.Collections.Generic;
using System.Data;
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
        #region Variabelen
        // Globale variabelen
        public String woord = "";
        String fout = "";
        String correct = "";
        int levens = 10;
        string[]masking = new string [100];
        int userTimer = 10;
        int timerTickCount = 11;
        int tijd = 1;
        //Highscore list
        List<HigScore> Punten = new List<HigScore>();
        //Kan de speler in de highscore komen?
        bool TopList = true;
        // is het spel actief
        bool spelActief = false;
        //True = 1 VS CPU     False = 1 VS 1
        bool spelModus = true;
        //Timer aanmaken
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
        #endregion
        #region Start van de app
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // default uitzicht creeeren als app laad
            HideObject(BtnRaad);
            ShowObject(BtnNieuw);
            HideObject(BtnVerberg);
            SetImage();
            SetBck(true);
            //Het spel is nog niet bezif
            spelActief = false;
            //Standaard speel je tegen de CPU
            MnCPU.IsChecked = true;
            //Spel starten voor CPU gebruiker moet eerst op verberg/beginnen klikken
            StartNieuwSpel();
        }
        #endregion
        #region achtergrond zetten standaard en als fout is.
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
        #endregion
        #region  Buttons hidden of viable zetten
        private void HideObject(Button button)
        {
            button.Visibility = Visibility.Hidden;
        }
        private void ShowObject(Button button)
        {
            button.Visibility = Visibility.Visible;
        }
        #endregion
        #region Fout geraden
        private void Fout()
        {
            
            timer.Stop();
            levens--;
            //timer resetten voor nieuwe letter te raden
            timerTickCount = userTimer;
            LblTimer.Content = timerTickCount;
            //Update van de label met de foute letter erin te zetten
            PrintLbl();
            //Veld leeg maken voor nieuwe letter te kunnen ingeven
            TxtInput.Clear();
            //Timer weer starten voor opnieuw te raden
            timer.Start();
        }
        #endregion
        #region Indien men Verloren heeft
        private void Verloren()
        {
            timer.Stop();
            SetImageDOOD();
            MessageBox.Show("Je bent opgehangen");
            LblText.Content = "Dank U voor het spelen \n";
            LblText.Content += $"Het woord dat wij zochten was: \n\n{woord}";
            TxtInput.Clear();
            ShowObject(BtnNieuw);
            HideObject(BtnRaad);
            spelActief = false;
        }

       
        #endregion
        #region Indien men gewonnen heeft
        private void Gewonnen()
        {
            timerTickCount = userTimer;
            LblTimer.Content = timerTickCount;
            timer.Stop();
            SetImage();
            LblText.Content = "Dank U voor het spelen\n";
            LblText.Content += $"Het woord dat wij zochten was: \n\n{woord}";
            HideObject(BtnRaad);
            TxtInput.Clear();
            BehaaldHS();
            spelActief = false;
        }
        #endregion
        #region Highscore
        private void BehaaldHS()
        {
            //Als je in de higscore mag komen
            if (TopList)
            {
                //Naam vragen aan de speler en deze toevoegen aan de List Punten
                string naam = Microsoft.VisualBasic.Interaction.InputBox("WOW goed gespeeld geef je naam in voor de highscore");
                Punten.Add(new HigScore { tijd = DateTime.Now.ToString("HH:mm:ss"), naam = naam, score = levens });
            }

       

        }
        #endregion
        #region Tekenen mannetje aan de hand van de levens
        private void SetImage()
        {
            Uri url = new Uri($"Assets/{levens}.png",UriKind.Relative);
            //IMG aanmaken als een bitmap
            BitmapImage bitmap = new BitmapImage(url);
            // bitmap toevoegen aan WPF
            Hangman.Source = bitmap;
        }
        private void SetImageDOOD()
        {
            Uri url = new Uri($"Assets/0.png", UriKind.Relative);
            //IMG aanmaken als een bitmap
            BitmapImage bitmap = new BitmapImage(url);
            // bitmap toevoegen aan WPF
            Hangman.Source = bitmap;
        }
        #endregion
        #region reset
        private void Reset()
        {
            timerTickCount = userTimer;
            TxtInput.Text = "";
            fout = "";
            correct = "";
            levens = 10;
            SetImage();
                }
        #endregion
        #region Lbl printen
        private void PrintLbl()
        {
            LblText.Content = "Gelieven een woord of een letter in te geven in het gele vak\nDruk dan op raad\n\n";
            LblText.Content += $"U heeft op dit moment {levens} Leven(s)\n\n";
            LblText.Content += $"Correct gerade letters: {correct}\n";
            LblText.Content += $"Fout gerade letters: {fout}";
            Lblmasking.Content = string.Join("", masking);
        }
        #endregion
        #region Event als men op Verberg/begin knop klikt
        private void BtnVerberg_Click(object sender, RoutedEventArgs e)
        {
            // speler mag in highscore komen
            TopList = true;
            // kijken 1VS1 of CPU vs 1
            if (spelModus)
            {
                //Radom getal voor een random woord te vinden voor 1 VS CPU
                Random getal = new Random();
                woord = galgjeWoorden[getal.Next(0, galgjeWoorden.Length)];
                masking = new string[woord.Length];
                Reset();
                Masking();
                

            }
            else
            {
               // 1 VS 1
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
            MnHint.IsEnabled = true;
            mntimer.IsEnabled = false;
            spelActief = true;
          
        }
        #endregion
        #region Timers voor spel verloop
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
        async void timer_Tick(object sender, EventArgs e)
        {

            if (timerTickCount == 0)
            {
                SetBck(false);
                MessageBox.Show("Tijd is op!");
                levens--;
                //timer.Stop();

                timerTickCount = userTimer;
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
        #endregion

        #region Nieuw spel 
        public void BtnNieuw_Click(object sender, RoutedEventArgs e)
        {

            StartNieuwSpel();
            levens = 10;
            SetImage();

        }
        private void StartNieuwSpel()
        {
            MnHint.IsEnabled = false;
            mntimer.IsEnabled = true;
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




        }

        private void Multispeler()
        {
            ShowObject(BtnVerberg);
            HideObject(BtnRaad);
            BtnVerberg.Content = "Verberg";
            Lblmasking.Content = "1 VS 1";
            woord = "";

        }

        #endregion


        #region Events als men op Raad knop klikt
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
        #endregion
        #region Het woord maskeren
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
        #endregion
        #region Kijken als de ingevoerde letters overeen komen met het woor dat we zoeken
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
        #endregion
        #region Menu Items
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
            var ordered = Punten.OrderByDescending(x => x.score);
            string print = "";
            foreach (var score in ordered)
            {
                print += $"{score.naam.ToUpper()} - {score.score} Levens ({score.tijd})\n";
            }
            MessageBox.Show($"Dit is monmenteel de highscore:\n\n" +
               $"{print}");

        }

        class HigScore
        {
            public string tijd { get; set; }
            public string naam { get; set; }
            public int score { get; set; }
        }

        private void menuHint(object sender, RoutedEventArgs e)
        {
         
               
          //een radom char generegren in een do while zolang dat de letters niet in het woord en niet al fout geraden is
                Random rnd = new Random();
                char hint = ' ';
                do
                {
                    hint = (char)rnd.Next('a', 'z');
                } while (woord.Contains(hint) || fout.Contains(hint));
                MessageBox.Show($"Hier is je hint:\n{hint}\n\n Nu geraak je niet meer in de higscore");
            //Speler komt niet in Highscore
            TopList = false;
           
           
        }

        private void menuTimer(object sender, RoutedEventArgs e)
        {
           
             
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
      

            StartNieuwSpel();

        }
      
        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (spelActief)
            {
                if (e.Key == Key.Enter)
                {
                    Raad();
                }
            }  
        }
        #endregion
    }
}
