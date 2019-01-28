using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker workerMenu = new BackgroundWorker();
        //private BackgroundWorker workerSprawdzic = new BackgroundWorker();

        static SpeechSynthesizer ss;
        static SpeechRecognitionEngine sre;
        static bool doneGlowneMenu = false;
        //private bool doneSprawdzic = false;
        string idKlienta = "";
        int cyfryIdKlienta = 0;

        bool flagaMenu = true;
        private bool flagaUmowic = false;
        private bool flagaUmowicTyp = false;
        private bool flagaUmowicPlacowka = false;
        private bool flagaUmowicData = false;
        private bool flagaUmowicTermin = false;
        bool flagaSprawdzic = false;
        private bool flagaOdwolac = false;
        private bool flagaOdwolacDecyzja = false;
        Grammar grammar;
        private string decyzja;

        List<Wizyta> listaWizyt;
        List<Wizyta> listaGodzinDoWyboru;
        private string typLekarza;
        private string placowka;
        private string data;
        private string miesiac = "";
        private string dzien = "";
        private Timer time;
        private string wybor;

        public MainWindow()
        {
            listaWizyt = new List<Wizyta>();
            listaGodzinDoWyboru = new List<Wizyta>();

            workerMenu.DoWork += worker_DoWork;
            workerMenu.RunWorkerAsync();

            SizeChanged += (o, e) =>
            {
                var r = SystemParameters.WorkArea;
                Left = r.Right - ActualWidth;
                Top = r.Bottom - ActualHeight;
            };
            InitializeComponent();

            //DataTable tabela = ClsDB.Get_DataTable("SELECT * FROM WIZYTA");
            //foreach (DataRow row in tabela.Rows)
            //{
            //    // ... Write value of first field as integer.
            //    Console.WriteLine(row.Field<int>(0) + " " + row.Field<string>(1) + " " + row.Field<int>(2) + " " + row.Field<DateTime>(3).ToString("dd/MM/yyyy"));
            //}

            //UserBox.Text = "adrian";
            //UserBox.Focus();

        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Rate = 2;

            SetSystemBoxFocus();
            SetSystemBox("Witamy w serwisie e-Przychodnia");
            
            ss.Speak("Witamy w serwisie e przychodnia");
            SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
            ss.Speak("Powiedz co chcesz zrobić");
            ss.Speak("umuwić wizytę");
            ss.Speak("odwołać wizytę");
            ss.Speak("czy sprawdzić swoje wizyty");
            SetUserBoxFocus();
            SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");

            CultureInfo ci = new CultureInfo("pl-PL"); //ustawienie języka
            sre = new SpeechRecognitionEngine(ci); //powołanie engine rozpoznawania
            sre.SetInputToDefaultAudioDevice(); //ustawienie domyślnego urządzenia wejściowego
            sre.SpeechRecognized += Sre_SpeechRecognized;


            grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
            grammar.Enabled = true;
            sre.LoadGrammar(grammar);

            sre.RecognizeAsync(RecognizeMode.Multiple);
            while (doneGlowneMenu == false) {; }
            Console.WriteLine("Skonczylem - glowne menu");
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;

            if (confidence >= 0.6)
            {
                //MENU GLOWNE
                if (flagaMenu)
                {
                    grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                    grammar.Enabled = true;
                    sre.UnloadAllGrammars();
                    sre.LoadGrammar(grammar);

                    string first = Convert.ToString(e.Result.Semantics["first"].Value);
                    if (first.Equals("sprawdzić"))
                    {
                        flagaMenu = false;
                        flagaSprawdzic = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Podaj swój numer klienta");
                        ss.Speak("Podaj swoj numer klienta");
                        SetUserBoxFocus();
                        SetUserBox("Podawaj numer klienta po jednej cyfrze. \nnp. jeden, dwa, trzy, cztery, pięć\n\nAby powrócić do głównego menu powiedz STOP");

                        grammar = new Grammar("Grammars\\GrammarSprawdzic.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);
                    }
                    else if (first.Equals("odwołać"))
                    {
                        flagaMenu = false;
                        flagaOdwolac = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Podaj swój numer klienta");
                        ss.Speak("Podaj swoj numer klienta");
                        SetUserBoxFocus();
                        SetUserBox("Podawaj numer klienta po jednej cyfrze. \nnp. jeden, dwa, trzy, cztery, pięć\n\nAby powrócić do głównego menu powiedz STOP");

                        grammar = new Grammar("Grammars\\GrammarSprawdzic.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);
                    }
                    else if (first.Equals("umówić"))
                    {
                        flagaMenu = false;
                        flagaUmowic = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Podaj swój numer klienta");
                        ss.Speak("Podaj swoj numer klienta");
                        SetUserBoxFocus();
                        SetUserBox("Podawaj numer klienta po jednej cyfrze. \nnp. jeden, dwa, trzy, cztery, pięć\n\nAby powrócić do głównego menu powiedz STOP");

                        grammar = new Grammar("Grammars\\GrammarSprawdzic.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);
                    }
                    else if (first.Equals("koniec"))
                    {
                        doneGlowneMenu = true;

                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            this.Close();
                        }));
                    }
                }
                //SPRAWDZANIE
                else if (flagaSprawdzic)
                {
                    idKlienta += Convert.ToString(e.Result.Semantics["first"].Value);
                    if (Convert.ToString(e.Result.Semantics["first"].Value).Equals("STOP"))
                    {
                        grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);

                        
                        idKlienta = "";
                        cyfryIdKlienta = 0;
                        flagaSprawdzic = false;
                        flagaMenu = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                        ss.Speak("Powiedz co chcesz zrobić");
                        ss.Speak("umuwić wizytę");
                        ss.Speak("odwołać wizytę");
                        ss.Speak("czy sprawdzić swoje wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");

                    }
                    else
                    {
                        cyfryIdKlienta++;
                        SetUserBox("Numer klienta: " + idKlienta);
                        //SetSystemBoxFocus();
                    }

                    if (cyfryIdKlienta == 5)
                    {
                        int numerWizyty = 0;
                        DataTable tabela = ClsDB.Get_DataTable("SELECT * FROM WIZYTA WHERE NumerKlienta = " + idKlienta);
                        if (tabela.Rows.Count > 0)
                        {
                            foreach (DataRow row in tabela.Rows)
                            {
                                SetSystemBoxFocus();
                                SetUserBox("");
                                numerWizyty++;
                                SetSystemBox("Wizyta numer " + numerWizyty + "\n" +
                                            "Lekarz " + row.Field<string>(1) + "\n" +
                                            "Termin " + row.Field<DateTime>(3).ToString("dd/MM/yyyy") + "\n" +
                                            "Godzina " + row.Field<TimeSpan>(4).ToString(@"hh\:mm") + "\n" +
                                            "Placówka " + row.Field<string>(5) + "\n" +
                                            "Doktor " + row.Field<string>(6));
                                ss.Speak("Wizyta nr: " + numerWizyty);
                                ss.Speak("Lekarz: " + row.Field<string>(1));
                                ss.Speak("Termin " + row.Field<DateTime>(3).ToString("dd/MM/yyyy"));
                                ss.Speak(row.Field<TimeSpan>(4).ToString(@"hh\:mm"));
                                ss.Speak("Placówka: " + row.Field<string>(5));
                                ss.Speak("Doktor: " + row.Field<string>(6));
                            }
                        }
                        else
                        {
                            SetSystemBoxFocus();
                            SetUserBox("");
                            SetSystemBox("Brak wizyt\nPodpowiedź: Wizyty możesz umówić z głównego menu");
                            ss.Speak("Nie masz umuwionych wizyt. Wizyty możesz umuwić z głuwnego meni");
                        }
                        

                        idKlienta = "";
                        cyfryIdKlienta = 0;
                        flagaSprawdzic = false;
                        flagaMenu = true;

                        grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                        ss.Speak("Powiedz co chcesz zrobić");
                        ss.Speak("umuwić wizytę");
                        ss.Speak("odwołać wizytę");
                        ss.Speak("czy sprawdzić swoje wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");

                    }

                }
                //ODWOLYWANIE - NUMER KLIENTA I WYSWIETLANIE WIZYT
                else if (flagaOdwolac)
                {
                    idKlienta += Convert.ToString(e.Result.Semantics["first"].Value);
                    if (Convert.ToString(e.Result.Semantics["first"].Value).Equals("STOP"))
                    {
                        grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);


                        idKlienta = "";
                        cyfryIdKlienta = 0;
                        flagaOdwolac = false;
                        flagaMenu = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                        ss.Speak("Powiedz co chcesz zrobić");
                        ss.Speak("umuwić wizytę");
                        ss.Speak("odwołać wizytę");
                        ss.Speak("czy sprawdzić swoje wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");

                    }
                    else
                    {
                        cyfryIdKlienta++;
                        SetUserBox("Numer klienta: " + idKlienta);
                        //SetSystemBoxFocus();
                    }

                    if (cyfryIdKlienta == 5)
                    {
                        int numerWizyty = 0;
                        DataTable tabela = ClsDB.Get_DataTable("SELECT * FROM WIZYTA WHERE NumerKlienta = " + idKlienta);

                        if (tabela.Rows.Count > 0)
                        {
                            SetSystemBoxFocus();
                            SetUserBox("");
                            SetSystemBox("Podaj numer wizyty którą chcesz odwołać");
                            ss.Speak("Podaj numer wizyty którą chcesz odwołać");

                            foreach (DataRow row in tabela.Rows)
                            {
                                SetSystemBoxFocus();
                                SetUserBox("");
                                numerWizyty++;
                                SetSystemBox("Podaj numer wizyty którą chcesz odwołać " + "\n" +
                                            "Wizyta " + numerWizyty + "\n" +
                                            "Masz umówioną wizytę u " + row.Field<string>(1) + "\n" +
                                            "Termin " + row.Field<DateTime>(3).ToString("dd/MM/yyyy") + "\n" +
                                            "Godzina " + row.Field<TimeSpan>(4).ToString(@"hh\:mm") + "\n" +
                                            "Placówka " + row.Field<string>(5) + "\n" +
                                            "Lekarz " + row.Field<string>(6));
                                ss.Speak("Wizyta " + numerWizyty);
                                ss.Speak("Lekarz " + row.Field<string>(1));
                                ss.Speak("Termin " + row.Field<DateTime>(3).ToString("dd/MM/yyyy"));
                                ss.Speak(row.Field<TimeSpan>(4).ToString(@"hh\:mm"));
                                ss.Speak("Placówka " + row.Field<string>(5));
                                ss.Speak("Doktor " + row.Field<string>(6));

                                listaWizyt.Add(new Wizyta(numerWizyty, row.Field<int>(0)));

                            }

                            SetUserBoxFocus();
                            SetUserBox("Możesz powiedzieć np. jeden\n\nAby powrócić do głównego menu powiedz STOP");

                            idKlienta = "";
                            cyfryIdKlienta = 0;
                            flagaOdwolac = false;
                            flagaOdwolacDecyzja = true;

                            grammar = new Grammar("Grammars\\GrammarOdwolac.xml", "rootRule");
                            grammar.Enabled = true;
                            sre.UnloadAllGrammars();
                            sre.LoadGrammar(grammar);
                        }
                        else
                        {
                            SetSystemBoxFocus();
                            SetUserBox("");
                            SetSystemBox("Brak wizyt\nPodpowiedź: Wizyty możesz umówić z głównego menu");
                            ss.Speak("Nie masz umuwionych wizyt. Wizyty możesz umuwić z głuwnego meni");

                            idKlienta = "";
                            cyfryIdKlienta = 0;
                            flagaOdwolac = false;
                            flagaMenu = true;

                            SetSystemBoxFocus();
                            SetUserBox("");
                            SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                            ss.Speak("Powiedz co chcesz zrobić");
                            ss.Speak("umuwić wizytę");
                            ss.Speak("odwołać wizytę");
                            ss.Speak("czy sprawdzić swoje wizyty");
                            SetUserBoxFocus();
                            SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");
                        }
                        //ss.Speak("Aby powrócić do głównego meni powiedz STOP");

                        

                        
                    }
                }
                //ODWOLYWANIE - WYBOR WIZYTY DO ODWOLANIA
                else if (flagaOdwolacDecyzja)
                {
                    decyzja = Convert.ToString(e.Result.Semantics["first"].Value);
                    int decyzjaWizyta = 0;
                    if (decyzja.Equals("STOP"))
                    {
                        grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);

                        flagaOdwolacDecyzja = false;
                        flagaMenu = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                        ss.Speak("Powiedz co chcesz zrobić");
                        ss.Speak("umuwić wizytę");
                        ss.Speak("odwołać wizytę");
                        ss.Speak("czy sprawdzić swoje wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");
                    }
                    else if (Int32.TryParse(decyzja, out decyzjaWizyta))
                    {
                        bool flagaCzyUsunalem = false;
                        foreach (Wizyta item in listaWizyt)
                        {
                            if (item.numerPrzypisany == decyzjaWizyta)
                            {
                                ClsDB.Execute_SQL("DELETE FROM Wizyta WHERE NumerWizyty = " + item.NumerWizyty);
                                flagaCzyUsunalem = true;
                                break;
                            }
                        }
                        if (!flagaCzyUsunalem)
                        {
                            SetSystemBoxFocus();
                            SetUserBox("");
                            SetSystemBox("Niepoprawny numer wizyty.");
                            ss.Speak("Niepoprawny numer wizyty");
                            SetUserBoxFocus();
                            SetUserBox("Możesz powiedzieć np. jeden\n\nAby powrócić do głównego menu powiedz STOP");
                        }
                        else
                        {
                            SetSystemBoxFocus();
                            SetUserBox("");
                            SetSystemBox("Usunąłem wizytę nr " + decyzjaWizyta);
                            ss.Speak("Usunąłem wizytę numer " + decyzjaWizyta);
                            flagaOdwolacDecyzja = false;
                            flagaMenu = true;

                            grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                            grammar.Enabled = true;
                            sre.UnloadAllGrammars();
                            sre.LoadGrammar(grammar);

                            SetSystemBoxFocus();
                            SetUserBox("");
                            SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                            ss.Speak("Powiedz co chcesz zrobić");
                            ss.Speak("umuwić wizytę");
                            ss.Speak("odwołać wizytę");
                            ss.Speak("czy sprawdzić swoje wizyty");
                            SetUserBoxFocus();
                            SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");
                        }
                    }
                }
                //UMWAIWANIE - NUMER KLIENTA
                else if (flagaUmowic)
                {
                    idKlienta += Convert.ToString(e.Result.Semantics["first"].Value);
                    
                    if (Convert.ToString(e.Result.Semantics["first"].Value).Equals("STOP"))
                    {
                        grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);


                        idKlienta = "";
                        cyfryIdKlienta = 0;
                        flagaUmowic = false;
                        flagaMenu = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                        ss.Speak("Powiedz co chcesz zrobić");
                        ss.Speak("umuwić wizytę");
                        ss.Speak("odwołać wizytę");
                        ss.Speak("czy sprawdzić swoje wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");

                    }
                    else
                    {
                        cyfryIdKlienta++;
                        SetUserBox("Numer klienta: " + idKlienta);
                        //SetSystemBoxFocus();
                    }

                    if (cyfryIdKlienta == 5)
                    {
                        //nie czyscic idKlienta!!!
                        //idKlienta = "";
                        cyfryIdKlienta = 0;
                        flagaUmowic = false;
                        flagaUmowicTyp = true;
                        grammar = new Grammar("Grammars\\GrammarUmowicTyp.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Podaj typ lekarza");
                        ss.Speak("Podaj typ lekarza");
                        SetUserBoxFocus();
                        SetUserBox("Dostępni specjaliści: \nInternista, Okulista, Stomatolog, Dermatolog, Ortopeda\n\nAby powrócić do głównego menu powiedz STOP");
                    }
                }
                //UMAWIANIE - TYP LEKARZA
                else if (flagaUmowicTyp)
                {
                    typLekarza = Convert.ToString(e.Result.Semantics["first"].Value);
                    if (typLekarza.Equals("STOP"))
                    {
                        grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);

                        typLekarza = "";
                        flagaUmowicTyp = false;
                        flagaMenu = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                        ss.Speak("Powiedz co chcesz zrobić");
                        ss.Speak("umuwić wizytę");
                        ss.Speak("odwołać wizytę");
                        ss.Speak("czy sprawdzić swoje wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");

                    }
                    else
                    {
                        flagaUmowicTyp = false;
                        flagaUmowicPlacowka = true;
                        grammar = new Grammar("Grammars\\GrammarUmowicPlacowka.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Podaj nazwę placówki");
                        ss.Speak("Podaj placowkę");
                        SetUserBoxFocus();
                        SetUserBox("Dostępne placówki: \nWołoska, Grochowska, Puławska, Żołnierska\n\nAby powrócić do głównego menu powiedz STOP");
                    }
                    
                }
                //UMAWIANIE - PLACOWKA
                else if (flagaUmowicPlacowka)
                {
                    placowka = Convert.ToString(e.Result.Semantics["first"].Value);
                    if (placowka.Equals("STOP"))
                    {
                        grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);
                        placowka = "";
                        flagaUmowicPlacowka = false;
                        flagaMenu = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                        ss.Speak("Powiedz co chcesz zrobić");
                        ss.Speak("umuwić wizytę");
                        ss.Speak("odwołać wizytę");
                        ss.Speak("czy sprawdzić swoje wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");

                    }
                    else
                    {
                        

                        flagaUmowicPlacowka = false;
                        flagaUmowicData = true;
                        grammar = new Grammar("Grammars\\GrammarUmowicData.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Podaj datę wizyty");
                        ss.Speak("Podaj datę wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Podaj datę w formacie dzień/miesiąc\nnp. Pierwszy stycznia\n\nAby powrócić do głównego menu powiedz STOP");
                    }
                    
                }
                //UMAWIANIE - DATA
                else if (flagaUmowicData)
                {
                    data = Convert.ToString(e.Result.Semantics["first"].Value);

                    if (data.Equals("STOP"))
                    {
                        grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);

                        if (time != null)
                        {
                            time.Enabled = false;
                        }
                        
                        flagaUmowicData = false;
                        flagaMenu = true;
                        miesiac = "";
                        dzien = "";
                        listaGodzinDoWyboru.Clear();
                        listaWizyt.Clear();

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                        ss.Speak("Powiedz co chcesz zrobić");
                        ss.Speak("umuwić wizytę");
                        ss.Speak("odwołać wizytę");
                        ss.Speak("czy sprawdzić swoje wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");
                        
                    }
                    else
                    {
                        SetUserBoxFocus();
                        

                        if (miesiac.Equals("") && (data.Equals("styczeń") || data.Equals("luty") || data.Equals("marzec") || data.Equals("kwiecień") || data.Equals("maj") || data.Equals("czerwiec") || data.Equals("lipiec") || data.Equals("sierpień") || data.Equals("wrzesień") || data.Equals("październik") || data.Equals("listopad") || data.Equals("grudzień")))
                        {
                            miesiac = data;
                            Console.WriteLine("Miesiac: " + miesiac);
                        }
                        else if (dzien.Equals(""))
                        {
                            dzien = data;
                            Console.WriteLine("Dzien: " + dzien);
                        }
                        SetUserBox("Wizyta w dniu: " + dzien + " / " + miesiac);

                        if ((miesiac.Equals("") && !dzien.Equals("")) || (!miesiac.Equals("") && dzien.Equals("")))
                        {
                            time = new Timer(10000);
                            time.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                            time.Start();
                        }
                        else if (!miesiac.Equals("") && !dzien.Equals(""))
                        {
                            time.Enabled = false;

                            flagaUmowicData = false;
                            flagaUmowicTermin = true;

                            grammar = new Grammar("Grammars\\GrammarOdwolac.xml", "rootRule");
                            grammar.Enabled = true;
                            sre.UnloadAllGrammars();
                            sre.LoadGrammar(grammar);

                            DataTable table = ClsDB.Get_DataTable("SELECT * FROM Wizyta WHERE TERMIN = " + "'" + DateTime.Now.Year + "-" + NameToNumber(miesiac) + "-" + dzien + "'" + " AND TypLekarza = " + "'" + typLekarza + "'" + "AND Placowka= " + "'" + placowka + "'");

                            List<int> listaGodzin = new List<int>();

                            foreach (DataRow row in table.Rows)
                            {
                                listaGodzin.Add(row.Field<TimeSpan>(4).Hours);

                            }
                            int j = 0;
                            for (int i = 9; i < 18; i++)
                            {
                                if (!listaGodzin.Contains(i))
                                {
                                    j++;
                                    listaGodzinDoWyboru.Add(new Wizyta(j, i));
                                }
                            }

                            SetSystemBoxFocus();
                            SetUserBox("");
                            SetSystemBox("Podaj numer terminu na który chcesz się umówić");
                            foreach (Wizyta item in listaGodzinDoWyboru)
                            {
                                AddSystemBox("\nTermin numer " + item.numerPrzypisany);
                                AddSystemBox("Godzina " + item.NumerWizyty);
                            }
                            ss.Speak("Podaj numer terminu na który chcesz się umuwić");
                            SetUserBoxFocus();
                            SetUserBox("Możesz powiedzieć np. jeden\n\nAby powrócić do głównego menu powiedz STOP");
                        }
                    }
                    

                }
                //UMAWIANIE - TERMIN
                else if (flagaUmowicTermin)
                {
                    wybor = Convert.ToString(e.Result.Semantics["first"].Value);
                    if (wybor.Equals("STOP"))
                    {
                        grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                        grammar.Enabled = true;
                        sre.UnloadAllGrammars();
                        sre.LoadGrammar(grammar);

                        idKlienta = "";
                        typLekarza = "";
                        miesiac = "";
                        dzien = "";
                        listaGodzinDoWyboru.Clear();
                        listaWizyt.Clear();
                        placowka = "";

                        flagaUmowicTermin = false;
                        flagaMenu = true;

                        SetSystemBoxFocus();
                        SetUserBox("");
                        SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                        ss.Speak("Powiedz co chcesz zrobić");
                        ss.Speak("umuwić wizytę");
                        ss.Speak("odwołać wizytę");
                        ss.Speak("czy sprawdzić swoje wizyty");
                        SetUserBoxFocus();
                        SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");
                    }
                    else
                    {
                        bool czyWystapil = false;
                        foreach (Wizyta item in listaGodzinDoWyboru)
                        {
                            if (item.numerPrzypisany == Int32.Parse(wybor))
                            {
                                string termin = DateTime.Now.Year + "-" + NameToNumber(miesiac) + "-" + dzien;
                                string godzina = item.NumerWizyty + ":" + "00" + ":" + "00";
                                DataTable imieNazwisko = ClsDB.Get_DataTable("SELECT Imie, Nazwisko,Placowka,Typ FROM Lekarze WHERE Placowka='" + placowka + "' AND Typ='" + typLekarza + "'");
                                string imie = (imieNazwisko.Rows[0]).Field<string>(0).ToString();
                                string nazwisko = (imieNazwisko.Rows[0]).Field<string>(1).ToString();

                                //ClsDB.Execute_SQL("INSERT INTO Wizyta (TypLekarza,NumerKlienta,Termin,Godzina,Placowka,Lekarz) VALUES('" + typLekarza + "'," + Int32.Parse(idKlienta) + ",'" + termin + "','" + godzina + "','" + placowka + "','" + imie + " " + nazwisko + "')");

                                using (SqlCommand cmd = new SqlCommand("INSERT INTO Wizyta (TypLekarza,NumerKlienta,Termin,Godzina,Placowka,Lekarz)" + "values (@typ, @numerKlienta,@termin,@godzina,@placowka,@lekarz)", ClsDB.Get_DB_Connection()))
                                {
                                    cmd.Parameters.AddWithValue("@typ", typLekarza);
                                    cmd.Parameters.AddWithValue("@numerKlienta", Int32.Parse(idKlienta));
                                    cmd.Parameters.AddWithValue("@termin", termin);
                                    cmd.Parameters.AddWithValue("@godzina", godzina);
                                    cmd.Parameters.AddWithValue("@placowka", placowka);
                                    cmd.Parameters.AddWithValue("@lekarz", imie + " " + nazwisko);
                                    cmd.ExecuteNonQuery();
                                    ClsDB.Close_DB_Connection();
                                }


                                idKlienta = "";
                                typLekarza = "";
                                miesiac = "";
                                dzien = "";
                                listaGodzinDoWyboru.Clear();
                                listaWizyt.Clear();
                                placowka = "";

                                flagaUmowicTermin = false;
                                flagaMenu = true;

                                grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
                                grammar.Enabled = true;
                                sre.UnloadAllGrammars();
                                sre.LoadGrammar(grammar);

                                SetSystemBoxFocus();
                                SetUserBox("");
                                SetSystemBox("Powiedz co chcesz zrobić: \n -umówić wizytę\n -odwołać wizytę\n -sprawdzić wizyty");
                                ss.Speak("Powiedz co chcesz zrobić");
                                ss.Speak("umuwić wizytę");
                                ss.Speak("odwołać wizytę");
                                ss.Speak("czy sprawdzić swoje wizyty");
                                SetUserBoxFocus();
                                SetUserBox("Możesz powiedzieć: \numówić, odwołać, sprawdzić");
                                czyWystapil = true;
                                break;
                            }

                        }
                        if (!czyWystapil)
                        {
                            ss.Speak("Niepoprawny termin");
                            ss.Speak("Proszę wybrać termin wyświetlony na ekranie");
                        }
                    }
                    

                }
            }
            else
            {
                ss.Speak("Powtórz");
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (miesiac.Equals(""))
                ss.Speak("Proszę podać miesiąc");
            else if (dzien.Equals(""))
                ss.Speak("Proszę podać dzień");
        }

        private string NameToNumber(String miesiac)
        {
            if (miesiac.Equals("styczeń"))
            {
                return "01";
            }
            else if (miesiac.Equals("luty"))
            {
                return "02";
            }
            else if (miesiac.Equals("marzec"))
            {
                return "03";
            }
            else if (miesiac.Equals("kwiecień"))
            {
                return "04";
            }
            else if (miesiac.Equals("maj"))
            {
                return "05";
            }
            else if (miesiac.Equals("czerwiec"))
            {
                return "06";
            }
            else if (miesiac.Equals("lipiec"))
            {
                return "07";
            }
            else if (miesiac.Equals("sierpień"))
            {
                return "08";
            }
            else if (miesiac.Equals("wrzesień"))
            {
                return "09";
            }
            else if (miesiac.Equals("październik"))
            {
                return "10";
            }
            else if (miesiac.Equals("listopad"))
            {
                return "11";
            }
            else if (miesiac.Equals("grudzień"))
            {
                return "12";
            }
            return "";
        }

        private void SetUserBox(string text)
        {
            UserBox.Dispatcher.Invoke(new Action(() =>
            {
                UserBox.Text = text;
            }));
        }

        private void SetSystemBox(string text)
        {
            SystemBox.Dispatcher.Invoke(new Action(() =>
            {
                SystemBox.Text = text;
            }));
        }

        private void AddSystemBox(string text)
        {
            SystemBox.Dispatcher.Invoke(new Action(() =>
            {
                SystemBox.Text = SystemBox.Text + "\n" + text;
            }));
        }

        private void SetSystemBoxFocus()
        {
            SystemBox.Dispatcher.Invoke(new Action(() =>
            {
                SystemBox.Focus();
                SystemBox.Effect = (DropShadowEffect)MainGrid.Resources["glowEffect"];
                UserBox.Effect = null;
            }));
        }

        private void SetUserBoxFocus()
        {
            UserBox.Dispatcher.Invoke(new Action(() =>
            {
                UserBox.Focus();
                UserBox.Effect = (DropShadowEffect)MainGrid.Resources["glowEffect"];
                SystemBox.Effect = null;
            }));
        }

        private void Exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void ExitEnter(object sender, MouseEventArgs e)
        {
            ButtonExit.Fill = (ImageBrush)MainGrid.Resources["closeActive"];

        }

        private void ExitLeave(object sender, MouseEventArgs e)
        {
            ButtonExit.Fill = (ImageBrush)MainGrid.Resources["close"];
        }

        private void ExitDown(object sender, MouseButtonEventArgs e)
        {
            ButtonExit.Fill = (ImageBrush)MainGrid.Resources["closeDown"];
        }
    }
}
