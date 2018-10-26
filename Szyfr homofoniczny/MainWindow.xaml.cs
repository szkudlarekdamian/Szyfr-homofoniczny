using Microsoft.Win32;
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
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace Szyfr_homofoniczny
{
    public partial class MainWindow : Window
    {

        #region Deklaracje zmiennych
        private List<String> alphabet = new List<String>(112);
        private static Random random = new Random();
        private List<KeyValuePair<String, List<String>>> cipherTextBoxHomophones = new List<KeyValuePair<String, List<String>>>();
        private List<KeyValuePair<String, List<String>>> cipherFileDialogHomophones = new List<KeyValuePair<String, List<String>>>();
        private String cipherFileRead = "";
        private String decipherFileRead = "";

        private List<KeyValuePair<String, long>> c2;
        private List<KeyValuePair<String, List<String>>> c3;
        private List<KeyValuePair<String, long>> c4;
        private bool t1b = false;
        

        #endregion

        public MainWindow()
        {
            for (int i = 33; i < 127; i++)
                alphabet.Add(((char)i).ToString());
            alphabet.AddRange(new String[] { "Ą", "ą", "Ć", "ć", "Ę", "ę", "Ł", "ł", "Ń", "ń", "Ó", "ó", "Ś", "ś", "Ź", "ź", "Ż", "ż" });
            Console.WriteLine(alphabet[94] + ((int)alphabet[94].ToCharArray()[0]));
            alphabet = alphabet.OrderBy(c => c).ToList<String>();
            List<String> specialCharacters = new List<String>();
            foreach (var letter in alphabet)
                if (!letter.All(char.IsLetterOrDigit))
                    specialCharacters.Add(letter);
            foreach(var character in specialCharacters)
                alphabet.Remove(character);
            alphabet.AddRange(specialCharacters);

            InitializeComponent();
            //help.Text = File.ReadAllText(@"./help.txt", Encoding.GetEncoding("Windows-1250"));
            help.Text = "Autor: Damian Szkudlarek\nRodzaj szyfru: Szyfr homofoniczny\nFunkcje programu:\n\t-Szyfrowanie tekstu\n\t-Deszyfrowanie tekstu\n\t-Szyfrowanie pliku\n\t-Deszyfrowanie pliku\n\t-Zapisanie zaszyfrowanego tekstu do pliku\n\t-Zapisanie deszyfrowanego tekstu do pliku\n\t-Zapisanie tablicy homofonów do pliku\n\t-Automatyczne uzupełnienie tablicy homofonów\n\t-Ręczne uzupełnienie tablicy homofonów\n\t-Wczytanie tablicy homofonów z pliku\n\n\nZnaki dozwolone:\n\t-Duże litery alfabetu polskiego\n\t-Małe litery alfabetu polskiego\n\t-Cyfry\n\t-Znaki specjalne drukowalne (np. !, @, .)\n\t-Spacja, Znak nowej linii, Powrót karety\n\n\nWażne!\nAby rozpocząć proces szyfrowania/deszyfrowania należy najpierw wprowadzić tekst/plik do programu i uzupełnić tablicę homofonów. Można to zrobić przyciskami u dołu ekranu.\n\n\nUwagi odnośnie programu:\n\t-Szyfrowanie nie działa w czasie rzeczywistym, trzeba każdorazowo wcisnąć odpwoiedni przycisk szyfrujący.\n\t-Karta 'Tablica Homofonów' jest niedostępna jeśli wcześniej nie wpisano tekstu do zaszyfrowania, nie wybrano pliku za szafrowania lub nie wczytano tablicy homofonów z pliku.\n\t-Funkcja 'Zapisz tekst do pliku' w przypadku szyfrowania tekstu dotyczy pola tekstowego nad nią (tekst jawny), a w przypadku szyfrowania pliku dotyczy zaszyfrowanego tekstu (tekst zaszyfrowany).\n\t-Program obsługuje tylko pliki tekstowe.\n\t-Rozmiar czcionki wczytanego pliku można powiększać i pomniejszać przy pomocy ikon pod tekstem.\n\n\n\nPrzykład obsługi programu - szyfrowanie tekstu\nKrok 1.\n\tWprowadzono tekst. (Tekst do zaszyfrowania)\nKrok 2.\n\tAutomatycznie wygenerowano tablicę homofonów. (Automatycznie)\nKrok 3.\n\tZaszyfrowano tekst. (Szyfruj tekst)\nKrok 4. \n\tOdczytano zaszyfrowany tekst. (Tekst do odszyfrowania)\n\n\n\n\nPrzykład obsługi programu - szyfrowanie pliku\nKrok 1.\n\tZaznaczono pole wyboru Plik. (Plik)\nKrok 2.\n\tWybrano plik. (Plik do zaszyfrowania > Wybierz plik)\nKrok 3.\n\tWygenerowano pustą tablicę homofonów. (Ręcznie)\nKrok 4. \n\tUzupełniono bezbłędnie wszystkie pola. (Pola tekstowe)\nKrok 5.\n\tPrzesłano przypisania znaków do homofonów. (Prześlij)\nKrok 6.\n\tZaszyfrowano plik. (Szyfruj plik)\nKrok 7. \n\tZapisano zaszyfrowany tekst do pliku. (Zapisz tekst do pliku)\nKrok 8.\n\tZapisano tablicę do pliku. (Zapisz tablicę do pliku)\n\n\n\n\nPrzykład obsługi programu - odszyfrowanie wcześniej zaszyfrowanego pliku\nKrok 1.\n\tZaznaczono pole wyboru Plik. (Plik)\nKrok 2.\n\tWybrano plik. (Plik do odszyfrowania > Wybierz plik)\nKrok 3.\n\tWczytano tablicę homofonów. (Wczytaj z pliku)\nKrok 4.\n\tOdszyfrowano plik. (Odszyfruj plik)";

        }

        #region Losowanie
        public static List<int> GenerateRandom(int count, int min, int max)
        {
            if (max <= min || count < 0 ||

                    (count > max - min && max - min > 0))
            {

                throw new ArgumentOutOfRangeException("Range " + min + " to " + max +
                        " (" + ((Int64)max - (Int64)min) + " values), or count " + count + " is illegal");
            }


            HashSet<int> candidates = new HashSet<int>();
            
            for (int top = max - count; top < max; top++)
            {
                if (!candidates.Add(random.Next(min, top + 1)))
                {

                    candidates.Add(top);
                }
            }


            List<int> result = candidates.ToList();
            for (int i = result.Count - 1; i > 0; i--)
            {
                int k = random.Next(i + 1);
                int tmp = result[k];
                result[k] = result[i];
                result[i] = tmp;
            }
            return result;
        }
        #endregion

        #region Okienka informacyjne
        private void showAlert(String text)
        {
            string caption = "Uwaga";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBox.Show(text, caption, button, icon);
        }
        #endregion

        #region ZliczanieWystapienLiter
        private List<KeyValuePair<String, long>> mapLetters(String text) {
            Dictionary<string, long> lettersDictionary = new Dictionary<string, long>();
            if (lettersDictionary.Count() != 0) { lettersDictionary.Clear(); }
            var counts = text.GroupBy(c => c).Select(g => new { Char = g.Key, Count = g.Count() });

            foreach (var c in counts)
            {
                if((int)(c.Char) > 31)
                    lettersDictionary.Add(c.Char.ToString(), c.Count);
            }
            List<KeyValuePair<String, long>> myList = lettersDictionary.ToList();
            //myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            var myList2 = myList.OrderBy(c => c.Key);

            List<KeyValuePair<String, long>> myListSorted = new List<KeyValuePair<String, long>>();
            for (int i = 0; i < myList.Count; i++)
            {
                myListSorted.Add(new KeyValuePair<String, long>(myList2.ElementAt(i).Key, myList2.ElementAt(i).Value));
            }

            List<KeyValuePair<String, long>> specialCharacters = new List<KeyValuePair<String, long>>();
            foreach (var letter in myListSorted)
                if (!letter.Key.All(char.IsLetterOrDigit))
                    specialCharacters.Add(letter);
            foreach (var character in specialCharacters)
                myListSorted.Remove(character);
            myListSorted.AddRange(specialCharacters);


            return myListSorted;
        }
        #endregion

        #region PrzypisanieHomofonowDoLiter
        private List<KeyValuePair<String, List<String>>> bindHomophones(List<KeyValuePair<String, long>> myList) {
            List<KeyValuePair<String, List<String>>> returnList = new List<KeyValuePair<String, List<String>>>();
            //int index = random.Next(0, 113);
            List<int> tempowa = GenerateRandom(112, 0, 112);
            int index = 0;
            int lettersCount = myList.Count();
            long sumOfRepetitions = 0;

            List<String> alphabetCopy = new List<String>();
            foreach (String c in alphabet) { alphabetCopy.Add(c); }
            foreach (var c in myList) { sumOfRepetitions += c.Value; }

            long sumOfRepetitionsCopy = sumOfRepetitions;

            foreach (var c in myList)
            {
                long repetitions = c.Value;

                returnList.Add(new KeyValuePair<String, List<String>>(c.Key, new List<String>()));

                //jeśli unikalnych znaków jest mniej niż w alfabecie
                if (lettersCount <= alphabet.Count())
                {
                    //jeśli znaków w zdaniu jest mniej niż w alfabecie
                    if (sumOfRepetitions < alphabet.Count())
                    {
                        if (t3.Text == "" && t1b==true)
                            c4.Add(new KeyValuePair<string, long>(c.Key, repetitions));
                        while (repetitions > 0)
                        {
                            while (alphabet.ElementAt(tempowa.ElementAt(index)) == c.Key || !alphabetCopy.Any(x => x.Equals(alphabet.ElementAt(tempowa.ElementAt(index)))))
                            {
                                if (alphabetCopy.Count() == 0) break;
                                index++;
                                if (index > tempowa.Count() - 1) index = 0;
                            }
                            alphabetCopy.Remove(alphabetCopy.Find(x => x.Equals(alphabet.ElementAt(tempowa.ElementAt(index)))));
                            returnList.Last().Value.Add(alphabet.ElementAt(tempowa.ElementAt(index)));
                            repetitions--;
                        }
                        
                    }
                    //jeśli znaków w zdaniu jest więcej niż w alfabecie 
                    else
                    {
                        float ratio = (float)(repetitions) / (float)(sumOfRepetitions);
                        int allocation = (int)Math.Floor(ratio * alphabet.Count());
                        if (allocation == 0) allocation = 1;
                        if (myList.Count() > 80 && allocation > 1) allocation = 1;
                        else if ((myList.Count() > 60 && myList.Count() < 81) && allocation > 2) allocation = 2;
                        else if ((myList.Count() < 61 && myList.Count() > 39) && allocation > 3) allocation = 3;
                        else if (myList.Count() < 40 && allocation > 7) allocation = 7;

                        if (t3.Text == "" && t1b == true)
                            c4.Add(new KeyValuePair<string, long>(c.Key, allocation));

                        while (allocation > 0)
                        {
                            while ( alphabet.ElementAt(tempowa.ElementAt(index)) == c.Key || !alphabetCopy.Any(x => x.Equals(alphabet.ElementAt(tempowa.ElementAt(index)))))
                            {
                                if (alphabetCopy.Count() == 0) break;                                
                                index++;
                                if (index > tempowa.Count()-1) index = 0;
                            }
                            alphabetCopy.Remove(alphabetCopy.Find(x => x.Equals(alphabet.ElementAt(tempowa.ElementAt(index)))));
                            returnList.Last().Value.Add(alphabet.ElementAt(tempowa.ElementAt(index)));
                            allocation--;
                        }
                    }
                }
                

            }
                        
            return returnList;

        }
        #endregion

        #region Zapis tabeli do pliku i wczytanie tabeli z pliku

        private void saveTableToFile(List<KeyValuePair<String, List<String>>> homophones)
        {
            if (homophones.Count() > 0)
            {
                KeyValuePair<String, List<String>> tempKVP;
                String key;
                List<String> value;
                String valueIndex;

                String returnValue = "";

                foreach (var kvp in homophones)
                {
                    tempKVP = kvp;
                    key = tempKVP.Key;
                    value = tempKVP.Value;
                    returnValue += key;
                    foreach (var vI in value)
                    {
                        valueIndex = vI;
                        returnValue += valueIndex;
                    }
                    returnValue += "\n";
                }

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Pliki tekstowe (.txt)|*.txt";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                    File.WriteAllText(dlg.FileName, returnValue, Encoding.GetEncoding("Windows-1250"));
            }
            else
                showAlert("Przypisania homofonów do liter nie istnieją.\n Wygeneruj je lub wpisz własnoręcznie");
        }

        private void loadTableFromFile(List<KeyValuePair<String, List<String>>> homophones)
        {
            if (homophones.Count() > 0) homophones.Clear();
            Microsoft.Win32.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt";

            String letter = "";
            String keyIndex = "";
            List<String> key = new List<String>();

            if (openFileDialog.ShowDialog() == true)
            {
                var lines = File.ReadLines(openFileDialog.FileName, Encoding.GetEncoding("Windows-1250"));
                foreach (var line in lines) {
                    letter = line[0].ToString();
                    for (int i = 1; i < line.Length; i++)
                        key.Add(line[i].ToString());
                    homophones.Add(new KeyValuePair<string, List<string>>(letter, key));
                    key = new List<String>();
                }
                autoUpdateHomophonesTable(homophones);
                tabItemHomophones.IsEnabled = true;
                showAlert("Wczytano tablicę homofonów.\nPrzejdź do zakładki Tablica Homofonów, aby je zobaczyć.");
            }
        }


        #endregion

        #region FunkcjaSzyfrujaca
        private String cipher(String text,List<KeyValuePair<String, List<String>>> key)
        {
            String returnString = "";
            if (key.Count > 0)
            {
                foreach (var letter in text)
                {
                    if ((int)letter > 31)
                    {
                        var temp = key.Where(x => x.Key == letter.ToString());
                        if (temp.Count() > 0)
                            returnString += temp.First().Value[random.Next(temp.First().Value.Count())];
                    }
                    else returnString += letter;
                }
            }
            return returnString;
        }
        #endregion

        #region EventySzyfrowanie
                
        private void cipherTextBoxButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (!cipherTextBox.Text.Equals("")) {
                if (cipherTextBoxHomophones.Count > 0)
                    decipherTextBox.Text = cipher(cipherTextBox.Text, cipherTextBoxHomophones);
                else
                {
                    showAlert("Przypisania homofonów do liter nie istnieją.\n Wygeneruj je lub wpisz własnoręcznie."); 
                }
            }
            else
                showAlert("Wprowadź tekst.");
        }


        private void cipherTextBoxTextSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cipherTextBox.Text != "")
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Pliki tekstowe (.txt)|*.txt";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                    File.WriteAllText(dlg.FileName, cipherTextBox.Text, Encoding.GetEncoding("Windows-1250"));
            }
            else
                showAlert("Nie ma tekstu do zapisania.\nWpisz tekst do zaszyfrowania.");
        }

        private void cipherTextBoxTableSaveButton_Click(object sender, RoutedEventArgs e)
        {
            saveTableToFile(cipherTextBoxHomophones);
        }

        private void cipherFileDialog_MouseEnter(object sender, MouseEventArgs e)
        {
            cipherFileDialog.Foreground = Brushes.Black;
        }

        private void cipherFileDialog_MouseLeave(object sender, MouseEventArgs e)
        {
            var converter = new BrushConverter();
            cipherFileDialog.Foreground = (Brush)converter.ConvertFromString("#E4DFDA");
        }

        private void cipherFileDialog_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                flowDocumentCipher.Document = new FlowDocument();
                cipherFileName.Text = openFileDialog.FileName;
                cipherFileRead = File.ReadAllText(openFileDialog.FileName, Encoding.GetEncoding("Windows-1250"));

                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(System.IO.File.ReadAllText(openFileDialog.FileName, Encoding.GetEncoding("Windows-1250")));
                FlowDocument document = new FlowDocument(paragraph);
                document.FontSize = 10;
                flowDocumentNormal.Document = document;
            }
        }

        private void cipherFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            if (cipherFileRead != "")
            {
                if (cipherFileDialogHomophones.Count > 0)
                {
                    Paragraph paragraph = new Paragraph();


                    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "tempCipher.txt", cipher(cipherFileRead, cipherFileDialogHomophones), Encoding.GetEncoding("Windows-1250"));
                    paragraph.Inlines.Add(System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "tempCipher.txt", Encoding.GetEncoding("Windows-1250")));
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "tempCipher.txt");

                    FlowDocument document = new FlowDocument(paragraph);
                    document.FontSize = 10;
                    flowDocumentCipher.Document = document;
                }
                else
                {
                    showAlert("Przypisania homofonów do liter nie istnieją.\n Wygeneruj je lub wpisz własnoręcznie.");
                }
            }
            else
                showAlert("Nie wczytano pliku do zaszyfrowania.");
        }
        private void cipherFileDialogTextSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (cipherFileRead != "" && flowDocumentNormal.Document != new FlowDocument())
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Pliki tekstowe (.txt)|*.txt";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                    File.WriteAllText(dlg.FileName, cipher(cipherFileRead, cipherFileDialogHomophones), Encoding.GetEncoding("Windows-1250"));
            }
            else
                showAlert("Nie ma tekstu do zapisania.\nWczytaj plik do szyfrowania.");


        }

        private void cipherFileDialogTableSaveButton_Click(object sender, RoutedEventArgs e)
        {
            saveTableToFile(cipherFileDialogHomophones);
        }

        #endregion

        #region Funkcja Deszyfrujaca
        private string decipher(string text, List<KeyValuePair<string, List<string>>> key)
        {
            String returnString = "";
            if (key.Count > 0)
            {
                foreach (var letter in text)
                {
                    if ((int)letter > 31)
                    {
                        var temp = key.Where(x => x.Value.Any(z => z.Equals(letter.ToString())) == true);
                        if (temp.Count() > 0) returnString += temp.First().Key;
                    }
                    else returnString += letter;
                }
            }
            return returnString;
        }
        #endregion

        #region EventyDeszyfrowanie
        private void decipherFileDialog_MouseEnter(object sender, MouseEventArgs e)
        {
            decipherFileDialog.Foreground = Brushes.Black;
        }

        private void decipherFileDialog_MouseLeave(object sender, MouseEventArgs e)
        {
            var converter = new BrushConverter();
            decipherFileDialog.Foreground = (Brush)converter.ConvertFromString("#79C0BE");
        }

        private void decipherFileDialog_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Filter = "Pliki tekstowe (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                flowDocumentCipheredDecrypted.Document = new FlowDocument();
                decipherFileName.Text = openFileDialog.FileName;
                decipherFileRead = File.ReadAllText(openFileDialog.FileName, Encoding.GetEncoding("Windows-1250"));

                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(decipherFileRead);
                FlowDocument document = new FlowDocument(paragraph);
                document.FontSize = 10;
                flowDocumentCipheredNormal.Document = document;
                
            }
        }


        private void decipherFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            if (decipherFileRead != "")
            {
                if (cipherFileDialogHomophones.Count > 0)
                {
                    Paragraph paragraph = new Paragraph();
                    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "tempDecipher.txt", decipher(decipherFileRead, cipherFileDialogHomophones), Encoding.GetEncoding("Windows-1250"));
                    paragraph.Inlines.Add(System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "tempDecipher.txt", Encoding.GetEncoding("Windows-1250")));
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "tempDecipher.txt");

                    FlowDocument document = new FlowDocument(paragraph);
                    document.FontSize = 10;
                    flowDocumentCipheredDecrypted.Document = document;
                }
                else
                {
                    showAlert("Przypisania homofonów do liter nie istnieją.\n Wygeneruj je lub wpisz własnoręcznie.");
                }
            }
            else
                showAlert("Nie wybrano pliku do deszyfrowania.");
        }
       
        private void decipherTextBoxButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decipherTextBox.Text.Equals(""))
            {
                tabItemHomophones.IsEnabled = true;
                cipherTextBox.Text = decipher(decipherTextBox.Text, cipherTextBoxHomophones);
            }
        }
        
        private void decipherTextBoxTextSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (decipherTextBox.Text != "")
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Pliki tekstowe (.txt)|*.txt";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                    File.WriteAllText(dlg.FileName, decipherTextBox.Text, Encoding.GetEncoding("Windows-1250"));
            }
            else
                showAlert("Nie ma tekstu do zapisania.\nWpisz tekst do odszyfrowania.");
        }
        private void decipherTextBoxTableSaveButton_Click(object sender, RoutedEventArgs e)
        {
            saveTableToFile(cipherTextBoxHomophones);
        }
        private void decipherFileDialogTextSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (decipherFileRead != "" && flowDocumentCipheredNormal.Document.Blocks.Count > 0 )
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Pliki tekstowe (.txt)|*.txt";
                
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                    File.WriteAllText(dlg.FileName, decipher(decipherFileRead, cipherFileDialogHomophones), Encoding.GetEncoding("Windows-1250"));
            }
            else
                showAlert("Nie ma tekstu do zapisania.\nWczytaj plik do odszyfrowania.");
        }
        private void decipherFileDialogTableSaveButton_Click(object sender, RoutedEventArgs e)
        {
            saveTableToFile(cipherFileDialogHomophones);
        }

        #endregion

        #region Aktualizacja Tabeli Homofonów
        private void autoUpdateHomophonesTable(List<KeyValuePair<String, List<String>>> homophones) {
            if (homophones.Count > 0)
            {
                var converterBrush = new BrushConverter();
                if (homophonesStackPanel.Children.Count > 0) homophonesStackPanel.Children.RemoveRange(0, homophonesStackPanel.Children.Count);
                for (var i = 0; i < homophones.Count(); i++)
                {
                    var wrapPanel = new WrapPanel { Name = "wrapPanel", Margin = new Thickness(0, 20, 0, 0) };
                    var index = ((int)homophones[i].Key.ToCharArray()[0]).ToString();
                    var letter = homophones[i].Key;
                    wrapPanel.Name += index;
                    wrapPanel.Children.Add(new Label { Name = "labelRight" + index, Content = letter + "        { ", Foreground = Brushes.AliceBlue, FontSize = 18, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });
                    for (var j = 0; j < homophones[i].Value.Count(); j++)
                    {
                        wrapPanel.Children.Add(new TextBox { Name = "textbox" + j.ToString(), Margin = new Thickness(10, 0, 10, 0), FontSize = 17, Text = homophones[i].Value[j], Width = 25, Height = 25, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, IsReadOnly = true });
                    }
                    wrapPanel.Children.Add(new Label { Name = "labelLeft" + index, Content = " }", Foreground = Brushes.AliceBlue, FontSize = 18, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });

                    homophonesStackPanel.Children.Add(wrapPanel);
                }
            }
        }

        private void manualUpdateHomophonesTable(List<KeyValuePair<String, List<String>>> homophones)
        {
            if (homophones.Count > 0)
            {
                var converterBrush = new BrushConverter();
                if (homophonesStackPanel.Children.Count > 0) homophonesStackPanel.Children.RemoveRange(0, homophonesStackPanel.Children.Count);
                for (var i = 0; i < homophones.Count(); i++)
                {
                    var wrapPanel = new WrapPanel { Name = "wrapPanel", Margin = new Thickness(0, 20, 0, 0) };
                    var index = ((int)homophones[i].Key.ToCharArray()[0]).ToString();
                    var letter = homophones[i].Key;
                    wrapPanel.Name += index;
                    wrapPanel.Children.Add(new Label { Name = "labelRight" + index, Content = letter + "        { ", Foreground = Brushes.AliceBlue, FontSize = 18, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });
                    for (var j = 0; j < homophones[i].Value.Count(); j++)
                    {
                        wrapPanel.Children.Add(new TextBox { Name = "textbox" + j.ToString(),MaxLength=1, Text = homophones[i].Value[j], Margin = new Thickness(10, 0, 10, 0), FontSize = 17, Width = 25, Height = 25, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, IsReadOnly = false });
                    }
                    wrapPanel.Children.Add(new Label { Name = "labelLeft" + index, Content = " }", Foreground = Brushes.AliceBlue, FontSize = 18, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });

                    homophonesStackPanel.Children.Add(wrapPanel);
                }
            }
        }

        #endregion

        #region Przyciski do aktualizacji Tabeli Homofonów
        private void manualHomophoneSend_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;
            List<KeyValuePair<String, List<String>>> homophones = new List<KeyValuePair<String, List<String>>>();
            if (cipherTextBoxHomophones.Count() > 0 && cipherTextBoxRadioButton.IsChecked==true)
                homophones = cipherTextBoxHomophones;
            else if (cipherFileDialogHomophones.Count() > 0 && cipherFileRadioButton.IsChecked == true)
                homophones = cipherFileDialogHomophones;
            else
            {
                showAlert("Nie wszystkie pola zostały wypełnione, lub homofony się powtarzają.\nPamiętaj, że homofon, nie może być taki sam jak znak, który reprezentuje.");
                return;
            }

            foreach (WrapPanel wrap in homophonesStackPanel.Children)
            {

                var letterInCharInt = wrap.Name.Skip(9);
                var letterInStringInt = "";
                foreach (var number in letterInCharInt) {
                    letterInStringInt += number;
                }
                var letter = ((char)(Int32.Parse(letterInStringInt))).ToString();


                foreach (Control child in wrap.Children)
                {
                    if(child.GetType() == typeof(TextBox))
                    {
                        TextBox c = (TextBox)child;
                        IEnumerable<KeyValuePair<String, List<String>>> temp = homophones.Where(keyLetter => keyLetter.Key == letter);
                        temp.First().Value[index] = c.Text;
                        index++;
                    }
                    
                }
                index = 0;
            }
            foreach (var d in homophones)
            {
                Console.Write("{0} => ", d.Key);
                foreach (var x in d.Value)
                {
                    Console.Write("{0}  ", x);
                }
                Console.WriteLine();
            }
            List<String> previousKeys = new List<String>();
            bool didIBreakTheRules = false;
            foreach(var keypair in homophones)
                foreach( var letter in keypair.Value)
                    if (previousKeys.Any(x => x.Equals(letter)) == true || letter==keypair.Key == true || letter == "")
                        didIBreakTheRules = true;
            if (!didIBreakTheRules) {
                manualUpdateHomophonesTable(homophones);
                showAlert("Prawidłowo wypełniono tablicę homofonów.");
            }
            else
            {
                foreach (var keypair in homophones)
                    for (int i = 0; i < keypair.Value.Count; i++)
                        keypair.Value[i] = "";
                showAlert("Nie wszystkie pola zostały wypełnione, lub homofony się powtarzają.\nPamiętaj, że homofon, nie może być taki sam jak znak, który reprezentuje.");

            }
        }



        private void generateHomophonesButton_Click(object sender, RoutedEventArgs e)
        {
            if (cipherFileRadioButton.IsChecked == true)
            {
                if (cipherFileRead != "")
                {
                    var myList = mapLetters(cipherFileRead);
                    cipherFileDialogHomophones = bindHomophones(myList);
                    autoUpdateHomophonesTable(cipherFileDialogHomophones);
                    manualHomophoneSend.Visibility = Visibility.Hidden;
                    tabItemHomophones.IsEnabled = true;
                    showAlert("Wygenerowano losowo homofony.\nPrzejdź do zakładki Tablica Homofonów, aby je zobaczyć.");
                }
                else
                {
                    showAlert("Najpierw wczytaj plik.");
                }
            }
            else if(cipherTextBoxRadioButton.IsChecked == true)
            {
                if (cipherTextBox.Text != "") 
                {
                    var myList = mapLetters(cipherTextBox.Text);
                    cipherTextBoxHomophones = bindHomophones(myList);
                    autoUpdateHomophonesTable(cipherTextBoxHomophones);
                    manualHomophoneSend.Visibility = Visibility.Hidden;
                    tabItemHomophones.IsEnabled = true;
                    showAlert("Wygenerowano losowo homofony.\nPrzejdź do zakładki Tablica Homofonów, aby je zobaczyć.");
                }
                else
                {
                    showAlert("Najpierw wpisz tekst.");
                }
            }
        }

        private void typeHomophonesButton_Click(object sender, RoutedEventArgs e)
        {
            if (cipherFileRadioButton.IsChecked == true)
            {
                if (cipherFileRead != "")
                {
                    var myList = mapLetters(cipherFileRead);
                    cipherFileDialogHomophones = bindHomophones(myList);
                    foreach (var keypair in cipherFileDialogHomophones)
                        for (int i = 0; i < keypair.Value.Count; i++)
                            keypair.Value[i] = "";
                    manualUpdateHomophonesTable(cipherFileDialogHomophones);
                    manualHomophoneSend.Visibility = Visibility.Visible;
                    tabItemHomophones.IsEnabled = true;
                    tabControl.SelectedIndex = 1;
                }
                else
                {
                    showAlert("Najpierw wczytaj plik.");
                }
            }
            else if (cipherTextBoxRadioButton.IsChecked == true)
            {
                if (cipherTextBox.Text != "") //LUB JESLI WCZYTANO TABLICĘ Z PLIKU
                {
                    var myList = mapLetters(cipherTextBox.Text);
                    cipherTextBoxHomophones = bindHomophones(myList);
                    foreach (var keypair in cipherTextBoxHomophones)
                        for (int i = 0; i < keypair.Value.Count; i++)
                            keypair.Value[i] = "";

                    manualUpdateHomophonesTable(cipherTextBoxHomophones);
                    tabItemHomophones.IsEnabled = true;
                    manualHomophoneSend.Visibility = Visibility.Visible;
                    tabControl.SelectedIndex = 1;
                }
                else
                {
                    showAlert("Najpierw wpisz tekst.");
                }
            }
        }

        private void loadHomophonesButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (cipherFileRadioButton.IsChecked == true)
                loadTableFromFile(cipherFileDialogHomophones);
            else if (cipherTextBoxRadioButton.IsChecked == true)
                loadTableFromFile(cipherTextBoxHomophones);
        }

        #endregion

        #region EventyZmianaRadioPlikTekst
        private void cipherTextBoxRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!(cipherFileStackPanel is null))
            {
                cipherFileStackPanel.Visibility = Visibility.Collapsed;
                decipherFileStackPanel.Visibility = Visibility.Collapsed;
                cipherTextBoxStackPanel.Visibility = Visibility.Visible;
                decipherTextBoxStackPanel.Visibility = Visibility.Visible;


                if (cipherTextBoxHomophones.Count > 0)
                {
                    tabItemHomophones.IsEnabled = true;
                    autoUpdateHomophonesTable(cipherTextBoxHomophones);
                }
                else
                    tabItemHomophones.IsEnabled = false;
            }
        }

        private void cipherFileRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!(cipherFileStackPanel is null)) { 
                cipherTextBoxStackPanel.Visibility = Visibility.Collapsed;
                decipherTextBoxStackPanel.Visibility = Visibility.Collapsed;
                cipherFileStackPanel.Visibility = Visibility.Visible;
                decipherFileStackPanel.Visibility = Visibility.Visible;

                if (cipherFileDialogHomophones.Count > 0)
                {
                    tabItemHomophones.IsEnabled = true;
                    autoUpdateHomophonesTable(cipherFileDialogHomophones);
                }
                else
                    tabItemHomophones.IsEnabled = false;
            }
        }
        #endregion

        #region Krok po kroku
        //zliczam litery
        private void b1_Click(object sender, RoutedEventArgs e)
        {
            if(t1.Text != "")
            {
                t2.Text = "";
                c2 = mapLetters(t1.Text);
                foreach (var list in c2)
                {
                    t2.Text += ("[" + list.Key + "] = " + list.Value + "\n");
                }
            }
        }

        //sprawdzam ile homofonów można przypisać do liter
        private void b2_Click(object sender, RoutedEventArgs e)
        {
            if(t2.Text != "")
            {
                t3.Text = "";
                c4 = new List<KeyValuePair<string, long>>();
                t1b = true;
                c3 = bindHomophones(c2);
                t1b = false;
                if (c4.Count() > 0)
                {
                    foreach (var x in c4)
                    {
                        t3.Text += "[" + x.Key + "]" + "{ " + x.Value + " }\n";
                    }
                }
                
            }
        }
        //przypisuje homofony do liter
        private void b3_Click(object sender, RoutedEventArgs e)
        {
            if (t3.Text != "")
            {
                t4.Text = "";
                foreach (var list in c3)
                {
                    t4.Text += "[" + list.Key + "]" + "{ ";
                    foreach (var value in list.Value)
                    {
                        t4.Text += value + " ";
                    }
                    t4.Text += "}\n";
                }
            }
        }
        //szyfrowanie tekstu jawnego losowymi homofonami litery
        private void b4_Click(object sender, RoutedEventArgs e)
        {
            if(t4.Text != "")
            {
                t5.Text = cipher(t1.Text, c3);
            }
        }
        #endregion
    }
}








