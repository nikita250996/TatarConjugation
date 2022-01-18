using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TatarConjugation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private char _lastLetter;

        // глухие согласные
        private readonly char[] _voicelessedConsonants =
        {
            'п', 'т', 'к', 'ф', 'ч', 'ш', 'щ', 'с', 'х', 'һ', 'ц', 'ъ', 'ь'
        };

        // все согласные
        private readonly char[] _allConsonants =
        {
            'б', 'в', 'г', 'д', 'ж', 'җ', 'з', 'л', 'м', 'н', 'ң', 'р', 'й', 'п', 'т', 'к', 'ф', 'ч', 'ш', 'щ', 'с',
            'х', 'һ', 'ц', 'ъ', 'ь'
        };

        // мягкое ли слово
        private bool _isSoft;

        // содержит ли слово только гласные-хамелеоны
        private bool _containsOnlyChameleonVowels;

        // гласные
        private readonly char[] _vowels = {'ә', 'ө', 'э', 'а', 'о', 'ы', 'е', 'ё', 'я'};

        // согласные
        private readonly char[] _consonants =
        {
            'б', 'в', 'г', 'д', 'ж', 'җ', 'з', 'к', 'л', 'м', 'н', 'ң', 'р', 'с', 'т', 'ф', 'х', 'һ', 'ц', 'ч', 'ш',
            'щ', 'ъ'
        };

        // аффиксы
        private readonly string[] _affixes =
        {
            "а", "ә", "ый", "и", "я", "м", "быз", "без", "сың", "сең", "сыз", "сез", "лар", "ләр", "ды", "де", "ты",
            "те", "к", "ң", "гыз", "гез", "ыр", "ер", "яр", "ар", "әр", "мын", "мен", "ма", "мә", "мый", "ми", "мас",
            "мәс", "мы", "ме", "ган", "гән", "кан", "кән", "нар", "нәр"
        };

        private const char FirstException = 'п';
        private const char SecondException = 'к';
        private const char ThirdException = 'й';
        private const char FourthException = 'и';
        private readonly string[] _people = {"әни", "әби", "әти", "бәби"};
        private const char FifthException = 'ь';
        private const char SixthException = 'ю';
        private const char SeventhException = 'у';
        private readonly char[] _softVowels = {'ә', 'ө', 'ү', 'э', 'и'};
        private readonly char[] _hardVowels = {'а', 'о', 'у', 'ы', 'ё'};
        private readonly char[] _chameleonVowels = {'е', 'ю', 'я'};
        private string _word;
        private readonly string[] _infinitives = new string[4];
        private readonly string[] _basePositiveForms = new string[9];
        private readonly string[] _positiveAffirmativeForms = new string[54];
        private readonly string[] _positiveInterrogativeForms = new string[54];
        private readonly string[] _baseNegativeForms = new string[9];
        private readonly string[] _negativeAffirmativeForms = new string[54];
        private readonly string[] _negativeInterrogativeForms = new string[54];
        private readonly char[] _lastException = {'м', 'н', 'ң'};
        private int _counter;

        // сонорные согласные
        private readonly char[] _sonorantedConsonants =
            {'б', 'в', 'г', 'д', 'ж', 'җ', 'з', 'л', 'м', 'н', 'ң', 'р', 'й'};

        // все глассные
        private readonly char[] _allVowels = {'ә', 'ө', 'и', 'ү', 'э', 'а', 'о', 'у', 'ы', 'е', 'ю', 'я'};

        public MainWindow()
        {
            InitializeComponent();
            Input.Focus();
        }

        private void Conjugate_Click(object sender, EventArgs e)
        {
            Input.Text = Input.Text.TrimStart(' ').TrimEnd(' ').ToLower();
            _word = Input.Text;
            if (_word == "")
            {
                MessageBox.Show("Пожалуйста, введите слово, которое вы хотите проспрягать.");
                return;
            }

            try
            {
                //подсчёт количества слогов
                foreach (char letter in _word)
                {
                    if (_allVowels.Contains(letter))
                    {
                        ++_counter;
                    }
                }

                //проверка слова на мягкость
                _isSoft = IsSoft();

                //получение базовых слов для положительных форм
                GetBasePositiveForms();
                //получение положительных утвердительных слов
                GetPositiveAffirmativeForms();
                //получение положительных вопросительных слов
                GetPositiveInterrogativeForms();
                //получение базовых слов для отрицательных форм
                GetBaseNegativeForms();
                //получение отрицательных утвердительных слов
                GetNegativeAffirmativeForms();
                //получение отрицательных вопросительных слов
                GetNegativeInterrogativeForms();
                //получение инфинитивов
                GetInfinitives();
                Print();
                _counter = 0;
            }
            catch
            {
                MessageBox.Show("Вы ввели некорректное слово. Пожалуйста, исправьте ошибку.");
            }
        }

        private bool IsSoft()
        {
            if (IsInRoundingHarmony())
            {
                return true;
            }

            char lastVowel = GetLastVowel();
            if (_counter == 1 && _containsOnlyChameleonVowels)
            {
                return lastVowel == 'е';
            }

            return _softVowels.Contains(lastVowel) || _containsOnlyChameleonVowels;
        }

        private bool IsInRoundingHarmony()
        {
            bool first = false, second = false;
            foreach (char letter in _word)
            {
                if (first && letter == 'а')
                {
                    return true;
                }

                if (second && letter == 'ә')
                {
                    return true;
                }

                switch (letter)
                {
                    case 'ә':
                        first = true;
                        continue;
                    case 'а':
                        second = true;
                        break;
                    default: continue;
                }
            }

            return false;
        }

        private char GetLastVowel()
        {
            List<char> vowels = new List<char>();
            string buffer = "";
            char lastVowel;
            _containsOnlyChameleonVowels = ContainsChameleonVowelsOnly(ref vowels);
            if (_containsOnlyChameleonVowels)
            {
                lastVowel = vowels.Last();
            }
            else
            {
                while (!_hardVowels.Contains(_word.Last()) && !_softVowels.Contains(_word.Last()) && _word.Length > 1)
                {
                    buffer += _word.Last();
                    _word = _word.Remove(_word.Length - 1);
                }

                lastVowel = _word.Last();

                _word += Reverse(buffer);
                if (_chameleonVowels.Contains(lastVowel))
                {
                    GetLastVowel();
                }
            }

            return lastVowel;
        }

        private bool ContainsChameleonVowelsOnly(ref List<char> vowels)
        {
            foreach (char letter in _word)
            {
                if (_allVowels.Contains(letter))
                {
                    vowels.Add(letter);
                }
            }

            return vowels.All(vowel => !_hardVowels.Contains(vowel) && !_softVowels.Contains(vowel));
        }

        private void GetBasePositiveForms()
        {
            char lastLetter = _word.Last();

            //кайт/көт
            if (_voicelessedConsonants.Contains(lastLetter))
            {
                //кайт
                if (!_isSoft)
                {
                    //кайта
                    _basePositiveForms[0] = _word + "а";
                    //кайтты
                    _basePositiveForms[1] = _word + "ты";
                    //кайткан
                    _basePositiveForms[2] = _word + "кан";
                    //
                    _basePositiveForms[7] = _word + "ачак";
                }
                //көт
                else
                {
                    //көтә
                    _basePositiveForms[0] = _word + "ә";
                    //көтте
                    _basePositiveForms[1] = _word + "те";
                    //көткән
                    _basePositiveForms[2] = _word + "кән";
                    //
                    _basePositiveForms[7] = _word + "әчәк";
                }
            }
            //аша/эзлә/бар
            else
            {
                //аша/эзлә
                if (_vowels.Contains(lastLetter))
                {
                    //аш/эзл
                    string word = _word.Remove(_word.Trim().Length - 1);
                    if (!_isSoft)
                    {
                        //ашый
                        _basePositiveForms[0] = word + "ый";
                        //ашады
                        _basePositiveForms[1] = _word + "ды";
                        //
                        _basePositiveForms[2] = _word + "ган";
                        //
                        _basePositiveForms[7] = _word + "ячак";
                    }
                    else
                    {
                        //эзли
                        _basePositiveForms[0] = word + "и";
                        //эзләде
                        _basePositiveForms[1] = _word + "де";
                        //
                        _basePositiveForms[2] = _word + "гән";
                        //
                        _basePositiveForms[7] = _word + "ячәк";
                    }
                }

                //бар/кил
                if (_consonants.Contains(lastLetter))
                {
                    //бар
                    if (!_isSoft)
                    {
                        //бара
                        _basePositiveForms[0] = _word + "а";
                        //барды
                        _basePositiveForms[1] = _word + "ды";
                        //
                        _basePositiveForms[2] = _word + "ган";
                        //
                        _basePositiveForms[7] = _word + "ачак";
                    }
                    //кил
                    else
                    {
                        //килә
                        _basePositiveForms[0] = _word + "ә";
                        //килде
                        _basePositiveForms[1] = _word + "де";
                        //
                        _basePositiveForms[2] = _word + "гән";
                        //
                        _basePositiveForms[7] = _word + "әчәк";
                    }
                }
            }

            switch (lastLetter)
            {
                //җәй
                case ThirdException:
                {
                    //җә
                    string word = _word.Remove(_word.Trim().Length - 1);
                    if (!_isSoft)
                    {
                        //
                        _basePositiveForms[0] = word + "я";
                        //
                        _basePositiveForms[1] = _word + "ды";
                        //
                        _basePositiveForms[2] = _word + "ган";
                        //
                        _basePositiveForms[7] = word + "ячак";
                    }
                    else
                    {
                        //җәя
                        _basePositiveForms[0] = word + "я";
                        //җәйде
                        _basePositiveForms[1] = _word + "де";
                        //
                        _basePositiveForms[2] = _word + "гән";
                        //
                        _basePositiveForms[7] = word + "ячәк";
                    }

                    break;
                }
                //ки
                case FourthException:
                {
                    //кия
                    _basePositiveForms[0] = _word + "я";
                    //киде
                    _basePositiveForms[1] = _word + "де";
                    //
                    _basePositiveForms[2] = _word + "гән";
                    //
                    _basePositiveForms[7] = _word + "ячәк";
                    break;
                }
                //ю
                case SixthException:
                {
                    _isSoft = false;
                    ConjugateException();
                    break;
                }
                //ку
                case SeventhException:
                {
                    ConjugateException();
                    break;
                }
            }

            if (_counter == 1)
            {
                string word = _word.Remove(_word.Trim().Length - 1);
                if (lastLetter == ThirdException)
                {
                    //җәяр
                    _basePositiveForms[8] = word + "яр";
                }
                else if (lastLetter == FourthException)
                {
                    //кияр
                    _basePositiveForms[8] = _word + "яр";
                }
                else if (lastLetter == 'р' || lastLetter == 'л')
                {
                    //барыр
                    if (!_isSoft)
                    {
                        _basePositiveForms[8] = _word + "ыр";
                    }
                    //килер
                    else
                    {
                        _basePositiveForms[8] = _word + "ер";
                    }
                }
                else if (_word == "кайт")
                {
                    //кайтыр
                    _basePositiveForms[8] = _word + "ыр";
                }
                else if (_word == "эл" || _word == "көл")
                {
                    //эләр/көләр
                    _basePositiveForms[8] = _word + "әр";
                }
                else
                {
                    //юар/куар
                    if (!_isSoft)
                    {
                        _basePositiveForms[8] = _word + "ар";
                    }
                    //көтәр
                    else
                    {
                        _basePositiveForms[8] = _word + "әр";
                    }
                }
            }
            else
            {
                if (_consonants.Contains(lastLetter))
                {
                    if (!_isSoft)
                    {
                        _basePositiveForms[8] = _word + "ыр";
                    }
                    else
                    {
                        _basePositiveForms[8] = _word + "ер";
                    }
                }

                if (_vowels.Contains(lastLetter))
                {
                    //ашар/эзләр
                    _basePositiveForms[8] = _word + "р";
                }
            }

            //
            _basePositiveForms[3] = _basePositiveForms[0] + " иде";
            //
            _basePositiveForms[4] = _basePositiveForms[2] + " иде";
            //
            _basePositiveForms[5] = _basePositiveForms[0] + " торган";
            //
            _basePositiveForms[6] = _basePositiveForms[7] + " иде";

            void ConjugateException()
            {
                //юа/куа
                _basePositiveForms[0] = _word + "а";
                //юды/куды
                _basePositiveForms[1] = _word + "ды";
                //
                _basePositiveForms[2] = _word + "ган";
                //
                _basePositiveForms[7] = _word + "ачак";
            }
        }

        private void GetPositiveAffirmativeForms()
        {
            char lastLetter = _word.Last();

            //аша/эзлә
            if (_vowels.Contains(lastLetter))
            {
                //аша
                if (!_isSoft)
                {
                    ConjugateHard();
                }
                //эзлә
                else
                {
                    ConjugateSoft();
                }
            }

            //бар/көт
            if (_consonants.Contains(lastLetter))
            {
                //бар
                if (!_isSoft)
                {
                    ConjugateHard();
                }
                //көт
                else
                {
                    ConjugateSoft();
                }
            }

            switch (lastLetter)
            {
                case ThirdException:
                {
                    if (!_isSoft)
                    {
                        ConjugateHard();
                    }
                    //җәй
                    else
                    {
                        ConjugateSoft();
                    }

                    break;
                }
                //ки
                case FourthException:
                {
                    ConjugateSoft();
                    break;
                }
                //ю
                case SixthException:
                {
                    ConjugateHard();
                    break;
                }
                //ку
                case SeventhException:
                {
                    ConjugateHard();
                    break;
                }
            }

            void ConjugateHard()
            {
                //ашыйм/барам
                _positiveAffirmativeForms[0] = _basePositiveForms[0] + "м";
                //ашыйбыз/барабыз
                _positiveAffirmativeForms[1] = _basePositiveForms[0] + "быз";
                //ашыйсың/барасың
                _positiveAffirmativeForms[2] = _basePositiveForms[0] + "сың";
                //ашыйсыз/барасыз
                _positiveAffirmativeForms[3] = _basePositiveForms[0] + "сыз";
                //ашый/бара
                _positiveAffirmativeForms[4] = _basePositiveForms[0];
                //ашыйлар/баралар
                _positiveAffirmativeForms[5] = _basePositiveForms[0] + "лар";

                //ашадым/бардым
                _positiveAffirmativeForms[6] = _basePositiveForms[1] + "м";
                //ашадык/бардык
                _positiveAffirmativeForms[7] = _basePositiveForms[1] + "к";
                //ашадың/бардың
                _positiveAffirmativeForms[8] = _basePositiveForms[1] + "ң";
                //ашадыгыз/бардыгыз
                _positiveAffirmativeForms[9] = _basePositiveForms[1] + "гыз";
                //ашады/барды
                _positiveAffirmativeForms[10] = _basePositiveForms[1];
                //ашадылар/бардылар
                _positiveAffirmativeForms[11] = _basePositiveForms[1] + "лар";

                //
                _positiveAffirmativeForms[12] = _basePositiveForms[2] + "мын";
                //
                _positiveAffirmativeForms[13] = _basePositiveForms[2] + "быз";
                //
                _positiveAffirmativeForms[14] = _basePositiveForms[2] + "сың";
                //
                _positiveAffirmativeForms[15] = _basePositiveForms[2] + "сыз";
                //
                _positiveAffirmativeForms[16] = _basePositiveForms[2];
                //
                _positiveAffirmativeForms[17] = _basePositiveForms[2] + "нар";

                //
                _positiveAffirmativeForms[23] = _basePositiveForms[0] + "лар" + " иде";

                //
                _positiveAffirmativeForms[29] = _basePositiveForms[2] + "нар" + " иде";

                //
                _positiveAffirmativeForms[35] = _basePositiveForms[5] + "нар" + " иде";

                //
                _positiveAffirmativeForms[41] = _basePositiveForms[7] + "лар" + " иде";

                //
                _positiveAffirmativeForms[42] = _basePositiveForms[7] + "мын";
                //
                _positiveAffirmativeForms[43] = _basePositiveForms[7] + "быз";
                //
                _positiveAffirmativeForms[44] = _basePositiveForms[7] + "сың";
                //
                _positiveAffirmativeForms[45] = _basePositiveForms[7] + "сыз";
                //
                _positiveAffirmativeForms[46] = _basePositiveForms[7];
                //
                _positiveAffirmativeForms[47] = _basePositiveForms[7] + "лар";

                //ашармын/барырмын
                _positiveAffirmativeForms[48] = _basePositiveForms[8] + "мын";
                //ашарбыз/барырбыз
                _positiveAffirmativeForms[49] = _basePositiveForms[8] + "быз";
                //ашарсың/барырсың
                _positiveAffirmativeForms[50] = _basePositiveForms[8] + "сың";
                //ашарсыз/барырсыз
                _positiveAffirmativeForms[51] = _basePositiveForms[8] + "сыз";
                //ашар/барыр
                _positiveAffirmativeForms[52] = _basePositiveForms[8];
                //ашарлар/барырлар
                _positiveAffirmativeForms[53] = _basePositiveForms[8] + "лар";
            }

            void ConjugateSoft()
            {
                //эзлим/көтәм
                _positiveAffirmativeForms[0] = _basePositiveForms[0] + "м";
                //эзлибез/көтәбез
                _positiveAffirmativeForms[1] = _basePositiveForms[0] + "без";
                //эзлисең/көтәсең
                _positiveAffirmativeForms[2] = _basePositiveForms[0] + "сең";
                //эзлисез/көтәсез
                _positiveAffirmativeForms[3] = _basePositiveForms[0] + "сез";
                //эзли/көтә
                _positiveAffirmativeForms[4] = _basePositiveForms[0];
                //эзлиләр/көтәләр
                _positiveAffirmativeForms[5] = _basePositiveForms[0] + "ләр";

                //эзләдем/көттем
                _positiveAffirmativeForms[6] = _basePositiveForms[1] + "м";
                //эзләдек/көттек
                _positiveAffirmativeForms[7] = _basePositiveForms[1] + "к";
                //эзләдең/көттең
                _positiveAffirmativeForms[8] = _basePositiveForms[1] + "ң";
                //эзләдегез/көттегез
                _positiveAffirmativeForms[9] = _basePositiveForms[1] + "гез";
                //эзләде/көтте
                _positiveAffirmativeForms[10] = _basePositiveForms[1];
                //эзләделәр/көттеләр
                _positiveAffirmativeForms[11] = _basePositiveForms[1] + "ләр";

                //
                _positiveAffirmativeForms[12] = _basePositiveForms[2] + "мен";
                //
                _positiveAffirmativeForms[13] = _basePositiveForms[2] + "без";
                //
                _positiveAffirmativeForms[14] = _basePositiveForms[2] + "сең";
                //
                _positiveAffirmativeForms[15] = _basePositiveForms[2] + "сез";
                //
                _positiveAffirmativeForms[16] = _basePositiveForms[2];
                //
                _positiveAffirmativeForms[17] = _basePositiveForms[2] + "нәр";

                //
                _positiveAffirmativeForms[23] = _basePositiveForms[0] + "ләр" + " иде";

                //
                _positiveAffirmativeForms[29] = _basePositiveForms[2] + "нәр" + " иде";

                //
                _positiveAffirmativeForms[35] = _basePositiveForms[5] + "нәр" + " иде";

                //
                _positiveAffirmativeForms[41] = _basePositiveForms[7] + "ләр" + " иде";

                //
                _positiveAffirmativeForms[42] = _basePositiveForms[7] + "мен";
                //
                _positiveAffirmativeForms[43] = _basePositiveForms[7] + "без";
                //
                _positiveAffirmativeForms[44] = _basePositiveForms[7] + "сең";
                //
                _positiveAffirmativeForms[45] = _basePositiveForms[7] + "сез";
                //
                _positiveAffirmativeForms[46] = _basePositiveForms[7];
                //
                _positiveAffirmativeForms[47] = _basePositiveForms[7] + "ләр";

                //эзләрмен/көтәрмен
                _positiveAffirmativeForms[48] = _basePositiveForms[8] + "мен";
                //эзләрбез/көтәрбез
                _positiveAffirmativeForms[49] = _basePositiveForms[8] + "без";
                //эзләрсең/көтәрсең
                _positiveAffirmativeForms[50] = _basePositiveForms[8] + "сең";
                //эзләрсез/көтәрсез
                _positiveAffirmativeForms[51] = _basePositiveForms[8] + "сез";
                //эзләр/көтәр
                _positiveAffirmativeForms[52] = _basePositiveForms[8];
                //эзләрләр/көтәрләр
                _positiveAffirmativeForms[53] = _basePositiveForms[8] + "ләр";
            }

            //
            _positiveAffirmativeForms[18] = _basePositiveForms[3] + "м";
            //
            _positiveAffirmativeForms[19] = _basePositiveForms[3] + "к";
            //
            _positiveAffirmativeForms[20] = _basePositiveForms[3] + "ң";
            //
            _positiveAffirmativeForms[21] = _basePositiveForms[3] + "гез";
            //
            _positiveAffirmativeForms[22] = _basePositiveForms[3];

            //
            _positiveAffirmativeForms[24] = _basePositiveForms[4] + "м";
            //
            _positiveAffirmativeForms[25] = _basePositiveForms[4] + "к";
            //
            _positiveAffirmativeForms[26] = _basePositiveForms[4] + "ң";
            //
            _positiveAffirmativeForms[27] = _basePositiveForms[4] + "гез";
            //
            _positiveAffirmativeForms[28] = _basePositiveForms[4];

            //
            _positiveAffirmativeForms[30] = _basePositiveForms[5] + " иде" + "м";
            //
            _positiveAffirmativeForms[31] = _basePositiveForms[5] + " иде" + "к";
            //
            _positiveAffirmativeForms[32] = _basePositiveForms[5] + " иде" + "ң";
            //
            _positiveAffirmativeForms[33] = _basePositiveForms[5] + " иде" + "гез";
            //
            _positiveAffirmativeForms[34] = _basePositiveForms[5] + " иде";

            //
            _positiveAffirmativeForms[36] = _basePositiveForms[6] + "м";
            //
            _positiveAffirmativeForms[37] = _basePositiveForms[6] + "к";
            //
            _positiveAffirmativeForms[38] = _basePositiveForms[6] + "ң";
            //
            _positiveAffirmativeForms[39] = _basePositiveForms[6] + "гез";
            //
            _positiveAffirmativeForms[40] = _basePositiveForms[6];
        }

        private void GetPositiveInterrogativeForms()
        {
            if (!_isSoft)
            {
                //
                _positiveInterrogativeForms[23] = _basePositiveForms[0] + "лар" + " иде" + "м" + "ме";

                //
                _positiveInterrogativeForms[29] = _basePositiveForms[2] + "нар" + " иде" + "м" + "ме";

                //
                _positiveInterrogativeForms[35] = _basePositiveForms[5] + "нар" + " иде" + "м" + "ме";

                //
                _positiveInterrogativeForms[41] = _basePositiveForms[7] + "лар" + " иде" + "м" + "ме";

                _basePositiveForms[3] = _basePositiveForms[0] + " иде" + "м" + "ме";
                _basePositiveForms[4] = _basePositiveForms[2] + " иде" + "м" + "ме";
                _basePositiveForms[5] += "мы";
                _basePositiveForms[6] = _basePositiveForms[7] + " иде" + "м" + "ме";

                for (int i = 0; i < 18; i++)
                {
                    _positiveInterrogativeForms[i] = _positiveAffirmativeForms[i] + "мы";
                }

                for (int i = 42; i < 54; i++)
                {
                    _positiveInterrogativeForms[i] = _positiveAffirmativeForms[i] + "мы";
                }
            }
            else
            {
                //киләләрме иде → киләләр идемме
                _positiveInterrogativeForms[23] = _basePositiveForms[0] + "ләр" + " иде" + "м" + "ме";

                //килгәннәрме иде → килгәннәр идемме
                _positiveInterrogativeForms[29] = _basePositiveForms[2] + "нәр" + " иде" + "м" + "ме";

                //килә торганнәрме иде → килә торганнәр идемме
                _positiveInterrogativeForms[35] = _basePositiveForms[5] + "нар" + " иде" + "м" + "ме";

                //киләчәкләрме иде → киләчәкләр идемме
                _positiveInterrogativeForms[41] = _basePositiveForms[7] + "ләр" + " иде" + "м" + "ме";

                //киләме иде → килә идемме
                _basePositiveForms[3] = _basePositiveForms[0] + " иде" + "м" + "ме";
                //килгәнме иде → килгән идемме
                _basePositiveForms[4] = _basePositiveForms[2] + " иде" + "м" + "ме";
                //килә торганме
                //_basePositiveForms[5] += "ме";
                //киләчәкме иде → киләчәк идемме
                _basePositiveForms[6] = _basePositiveForms[7] + " иде" + "м" + "ме";

                for (int i = 0; i < 18; i++)
                {
                    _positiveInterrogativeForms[i] = _positiveAffirmativeForms[i] + "ме";
                }

                for (int i = 42; i < 54; i++)
                {
                    _positiveInterrogativeForms[i] = _positiveAffirmativeForms[i] + "ме";
                }
            }

            //
            _positiveInterrogativeForms[18] = _basePositiveForms[3] + "м";
            //
            _positiveInterrogativeForms[19] = _basePositiveForms[3] + "к";
            //
            _positiveInterrogativeForms[20] = _basePositiveForms[3] + "ң";
            //
            _positiveInterrogativeForms[21] = _basePositiveForms[3] + "гез";
            //
            _positiveInterrogativeForms[22] = _basePositiveForms[3];

            //
            _positiveInterrogativeForms[24] = _basePositiveForms[4] + "м";
            //
            _positiveInterrogativeForms[25] = _basePositiveForms[4] + "к";
            //
            _positiveInterrogativeForms[26] = _basePositiveForms[4] + "ң";
            //
            _positiveInterrogativeForms[27] = _basePositiveForms[4] + "гез";
            //
            _positiveInterrogativeForms[28] = _basePositiveForms[4];

            //
            _positiveInterrogativeForms[30] = _basePositiveForms[5] + " иде" + "м" + "ме" + "м";
            //
            _positiveInterrogativeForms[31] = _basePositiveForms[5] + " иде" + "м" + "ме" + "к";
            //
            _positiveInterrogativeForms[32] = _basePositiveForms[5] + " иде" + "м" + "ме" + "ң";
            //
            _positiveInterrogativeForms[33] = _basePositiveForms[5] + " иде" + "м" + "ме" + "гез";
            //
            _positiveInterrogativeForms[34] = _basePositiveForms[5] + " иде" + "м" + "ме";

            //
            _positiveInterrogativeForms[36] = _basePositiveForms[6] + "м";
            //
            _positiveInterrogativeForms[37] = _basePositiveForms[6] + "к";
            //
            _positiveInterrogativeForms[38] = _basePositiveForms[6] + "ң";
            //
            _positiveInterrogativeForms[39] = _basePositiveForms[6] + "гез";
            //
            _positiveInterrogativeForms[40] = _basePositiveForms[6];
        }

        private void GetBaseNegativeForms()
        {
            char lastLetter = _word.Last();

            if (_voicelessedConsonants.Contains(lastLetter))
            {
                if (!_isSoft)
                {
                    //ашамый
                    _baseNegativeForms[0] = _word + "мый";
                    //ашамады
                    _baseNegativeForms[1] = _word + "ма";
                    //
                    _baseNegativeForms[2] = _word + "ма";
                    //
                    _baseNegativeForms[7] = _word + "ма";
                    //ашамас
                    _baseNegativeForms[8] = _word + "мас";
                }
                else
                {
                    //эзләми
                    _baseNegativeForms[0] = _word + "ми";
                    //эзләмәде
                    _baseNegativeForms[1] = _word + "мә";
                    //
                    _baseNegativeForms[2] = _word + "мә";
                    //
                    _baseNegativeForms[7] = _word + "мә";
                    //эзләмәс
                    _baseNegativeForms[8] = _word + "мәс";
                }
            }
            else
            {
                //аша
                if (!_isSoft)
                {
                    //ашамый
                    _baseNegativeForms[0] = _word + "мый";
                    //ашамады
                    _baseNegativeForms[1] = _word + "ма";
                    //
                    _baseNegativeForms[2] = _word + "ма";
                    //
                    _baseNegativeForms[7] = _word + "ма";
                    //ашамас
                    _baseNegativeForms[8] = _word + "мас";
                }
                //эзлә
                else
                {
                    //эзләми
                    _baseNegativeForms[0] = _word + "ми";
                    //эзләмәде
                    _baseNegativeForms[1] = _word + "мә";
                    //
                    _baseNegativeForms[2] = _word + "мә";
                    //
                    _baseNegativeForms[7] = _word + "мә";
                    //эзләмәс
                    _baseNegativeForms[8] = _word + "мәс";
                }
            }

            lastLetter = _baseNegativeForms[0].Last();

            if (_voicelessedConsonants.Contains(lastLetter))
            {
                if (!_isSoft)
                {
                    //ашамады
                    _baseNegativeForms[1] = _baseNegativeForms[1] + "ды";
                    //
                    _baseNegativeForms[2] = _baseNegativeForms[2] + "кан";
                    //
                    _baseNegativeForms[7] = _baseNegativeForms[7] + "ячак";
                }
                else
                {
                    //эзләмәде
                    _baseNegativeForms[1] = _baseNegativeForms[1] + "де";
                    //
                    _baseNegativeForms[2] = _baseNegativeForms[2] + "кән";
                    //
                    _baseNegativeForms[7] = _baseNegativeForms[7] + "ячәк";
                }
            }

            else
            {
                //аша
                if (!_isSoft)
                {
                    //ашамады
                    _baseNegativeForms[1] = _baseNegativeForms[1] + "ды";
                    //
                    _baseNegativeForms[2] = _baseNegativeForms[2] + "ган";
                    //
                    _baseNegativeForms[7] = _baseNegativeForms[7] + "ячак";
                }
                //эзлә
                else
                {
                    //эзләмәде
                    _baseNegativeForms[1] = _baseNegativeForms[1] + "де";
                    //
                    _baseNegativeForms[2] = _baseNegativeForms[2] + "гән";
                    //
                    _baseNegativeForms[7] = _baseNegativeForms[7] + "ячәк";
                }
            }

            //
            _baseNegativeForms[3] = _baseNegativeForms[0] + " иде";
            //
            _baseNegativeForms[4] = _baseNegativeForms[2] + " иде";
            //
            _baseNegativeForms[5] = _baseNegativeForms[0] + " торган";
            //
            _baseNegativeForms[6] = _baseNegativeForms[7] + " иде";
        }

        private void GetNegativeAffirmativeForms()
        {
            char lastLetter = _word.Last();

            //аша/эзлә
            if (_vowels.Contains(lastLetter) || lastLetter == ThirdException || lastLetter == FourthException)
            {
                //аша
                if (!_isSoft)
                {
                    //ашамыйм
                    _negativeAffirmativeForms[0] = _baseNegativeForms[0] + "м";
                    //ашамыйбыз
                    _negativeAffirmativeForms[1] = _baseNegativeForms[0] + "быз";
                    //ашамыйсың
                    _negativeAffirmativeForms[2] = _baseNegativeForms[0] + "сың";
                    //ашамыйсыз
                    _negativeAffirmativeForms[3] = _baseNegativeForms[0] + "сыз";
                    //ашамый
                    _negativeAffirmativeForms[4] = _baseNegativeForms[0];
                    //ашамыйлар
                    _negativeAffirmativeForms[5] = _baseNegativeForms[0] + "лар";

                    //ашамадым
                    _negativeAffirmativeForms[6] = _baseNegativeForms[1] + "м";
                    //ашамадык
                    _negativeAffirmativeForms[7] = _baseNegativeForms[1] + "к";
                    //ашамадың
                    _negativeAffirmativeForms[8] = _baseNegativeForms[1] + "ң";
                    //ашамадыгыз
                    _negativeAffirmativeForms[9] = _baseNegativeForms[1] + "гыз";
                    //ашамады
                    _negativeAffirmativeForms[10] = _baseNegativeForms[1];
                    //ашамадылар
                    _negativeAffirmativeForms[11] = _baseNegativeForms[1] + "лар";

                    //
                    _negativeAffirmativeForms[12] = _baseNegativeForms[2] + "мын";
                    //
                    _negativeAffirmativeForms[13] = _baseNegativeForms[2] + "быз";
                    //
                    _negativeAffirmativeForms[14] = _baseNegativeForms[2] + "сың";
                    //
                    _negativeAffirmativeForms[15] = _baseNegativeForms[2] + "сыз";
                    //
                    _negativeAffirmativeForms[16] = _baseNegativeForms[2];
                    //
                    _negativeAffirmativeForms[17] = _baseNegativeForms[2] + "нар";

                    //
                    _negativeAffirmativeForms[23] = _baseNegativeForms[0] + "лар" + " иде";

                    //
                    _negativeAffirmativeForms[29] = _baseNegativeForms[2] + "нар" + " иде";

                    //
                    _negativeAffirmativeForms[35] = _baseNegativeForms[5] + "нар" + " иде";

                    //
                    _negativeAffirmativeForms[41] = _baseNegativeForms[7] + "лар" + " иде";

                    //
                    _negativeAffirmativeForms[42] = _baseNegativeForms[7] + "мын";
                    //
                    _negativeAffirmativeForms[43] = _baseNegativeForms[7] + "быз";
                    //
                    _negativeAffirmativeForms[44] = _baseNegativeForms[7] + "сың";
                    //
                    _negativeAffirmativeForms[45] = _baseNegativeForms[7] + "сыз";
                    //
                    _negativeAffirmativeForms[46] = _baseNegativeForms[7];
                    //
                    _negativeAffirmativeForms[47] = _baseNegativeForms[7] + "лар";

                    //ашама
                    string word = _baseNegativeForms[8].Remove(_baseNegativeForms[8].Trim().Length - 1);
                    //ашамам
                    _negativeAffirmativeForms[48] = word + "м";
                    //ашамабыз
                    _negativeAffirmativeForms[49] = word + "быз";
                    //ашамассың
                    _negativeAffirmativeForms[50] = _baseNegativeForms[8] + "сың";
                    //ашамассыз
                    _negativeAffirmativeForms[51] = _baseNegativeForms[8] + "сыз";
                    //ашамас
                    _negativeAffirmativeForms[52] = _baseNegativeForms[8];
                    //ашамаслар
                    _negativeAffirmativeForms[53] = _baseNegativeForms[8] + "лар";
                }
                //эзлә
                else
                {
                    //эзләмим
                    _negativeAffirmativeForms[0] = _baseNegativeForms[0] + "м";
                    //эзләмибез
                    _negativeAffirmativeForms[1] = _baseNegativeForms[0] + "без";
                    //эзләмисең
                    _negativeAffirmativeForms[2] = _baseNegativeForms[0] + "сең";
                    //эзләмисез
                    _negativeAffirmativeForms[3] = _baseNegativeForms[0] + "сез";
                    //эзләми
                    _negativeAffirmativeForms[4] = _baseNegativeForms[0];
                    //эзләмиләр
                    _negativeAffirmativeForms[5] = _baseNegativeForms[0] + "ләр";

                    //эзләмәдем
                    _negativeAffirmativeForms[6] = _baseNegativeForms[1] + "м";
                    //эзләмәдек
                    _negativeAffirmativeForms[7] = _baseNegativeForms[1] + "к";
                    //эзләмәдең
                    _negativeAffirmativeForms[8] = _baseNegativeForms[1] + "ң";
                    //эзләмәдегез
                    _negativeAffirmativeForms[9] = _baseNegativeForms[1] + "гез";
                    //эзләмәде
                    _negativeAffirmativeForms[10] = _baseNegativeForms[1];
                    //эзләмәделәр
                    _negativeAffirmativeForms[11] = _baseNegativeForms[1] + "ләр";

                    //
                    _negativeAffirmativeForms[12] = _baseNegativeForms[2] + "мен";
                    //
                    _negativeAffirmativeForms[13] = _baseNegativeForms[2] + "без";
                    //
                    _negativeAffirmativeForms[14] = _baseNegativeForms[2] + "сең";
                    //
                    _negativeAffirmativeForms[15] = _baseNegativeForms[2] + "сез";
                    //
                    _negativeAffirmativeForms[16] = _baseNegativeForms[2];
                    //
                    _negativeAffirmativeForms[17] = _baseNegativeForms[2] + "нәр";

                    //
                    _negativeAffirmativeForms[23] = _baseNegativeForms[0] + "ләр" + " иде";

                    //
                    _negativeAffirmativeForms[29] = _baseNegativeForms[2] + "нәр" + " иде";

                    //
                    _negativeAffirmativeForms[35] = _baseNegativeForms[5] + "нәр" + " иде";

                    //
                    _negativeAffirmativeForms[41] = _baseNegativeForms[7] + "ләр" + " иде";

                    //
                    _negativeAffirmativeForms[42] = _baseNegativeForms[7] + "мен";
                    //
                    _negativeAffirmativeForms[43] = _baseNegativeForms[7] + "без";
                    //
                    _negativeAffirmativeForms[44] = _baseNegativeForms[7] + "сең";
                    //
                    _negativeAffirmativeForms[45] = _baseNegativeForms[7] + "сез";
                    //
                    _negativeAffirmativeForms[46] = _baseNegativeForms[7];
                    //
                    _negativeAffirmativeForms[47] = _baseNegativeForms[7] + "ләр";

                    //эзләм
                    string word = _baseNegativeForms[8].Remove(_baseNegativeForms[8].Trim().Length - 1);
                    //эзләмәм
                    _negativeAffirmativeForms[48] = word + "м";
                    //эзләмәбез
                    _negativeAffirmativeForms[49] = word + "без";
                    //эзләмәссең
                    _negativeAffirmativeForms[50] = _baseNegativeForms[8] + "сең";
                    //эзләмәссез
                    _negativeAffirmativeForms[51] = _baseNegativeForms[8] + "сез";
                    //эзләмәс
                    _negativeAffirmativeForms[52] = _baseNegativeForms[8];
                    //эзләмәсләр
                    _negativeAffirmativeForms[53] = _baseNegativeForms[8] + "ләр";
                }
            }

            //бар/көт
            if (_consonants.Contains(lastLetter) || lastLetter == SixthException || lastLetter == SeventhException)
            {
                //бар
                if (!_isSoft)
                {
                    //бармыйм
                    _negativeAffirmativeForms[0] = _baseNegativeForms[0] + "м";
                    //бармыйбыз
                    _negativeAffirmativeForms[1] = _baseNegativeForms[0] + "быз";
                    //бармыйсың
                    _negativeAffirmativeForms[2] = _baseNegativeForms[0] + "сың";
                    //бармыйсыз
                    _negativeAffirmativeForms[3] = _baseNegativeForms[0] + "сыз";
                    //бармый
                    _negativeAffirmativeForms[4] = _baseNegativeForms[0];
                    //бармыйлар
                    _negativeAffirmativeForms[5] = _baseNegativeForms[0] + "лар";

                    //бармадым
                    _negativeAffirmativeForms[6] = _baseNegativeForms[1] + "м";
                    //бармадык
                    _negativeAffirmativeForms[7] = _baseNegativeForms[1] + "к";
                    //бармадың
                    _negativeAffirmativeForms[8] = _baseNegativeForms[1] + "ң";
                    //бармадыгыз
                    _negativeAffirmativeForms[9] = _baseNegativeForms[1] + "гыз";
                    //бармады
                    _negativeAffirmativeForms[10] = _baseNegativeForms[1];
                    //бармадылар
                    _negativeAffirmativeForms[11] = _baseNegativeForms[1] + "лар";

                    //
                    _negativeAffirmativeForms[12] = _baseNegativeForms[2] + "мын";
                    //
                    _negativeAffirmativeForms[13] = _baseNegativeForms[2] + "быз";
                    //
                    _negativeAffirmativeForms[14] = _baseNegativeForms[2] + "сың";
                    //
                    _negativeAffirmativeForms[15] = _baseNegativeForms[2] + "сыз";
                    //
                    _negativeAffirmativeForms[16] = _baseNegativeForms[2];
                    //
                    _negativeAffirmativeForms[17] = _baseNegativeForms[2] + "нар";

                    //
                    _negativeAffirmativeForms[23] = _baseNegativeForms[0] + "лар" + " иде";

                    //
                    _negativeAffirmativeForms[29] = _baseNegativeForms[2] + "нар" + " иде";

                    //
                    _negativeAffirmativeForms[35] = _baseNegativeForms[5] + "нар" + " иде";

                    //
                    _negativeAffirmativeForms[41] = _baseNegativeForms[7] + "лар" + " иде";

                    //
                    _negativeAffirmativeForms[42] = _baseNegativeForms[7] + "мын";
                    //
                    _negativeAffirmativeForms[43] = _baseNegativeForms[7] + "быз";
                    //
                    _negativeAffirmativeForms[44] = _baseNegativeForms[7] + "сың";
                    //
                    _negativeAffirmativeForms[45] = _baseNegativeForms[7] + "сыз";
                    //
                    _negativeAffirmativeForms[46] = _baseNegativeForms[7];
                    //
                    _negativeAffirmativeForms[47] = _baseNegativeForms[7] + "лар";

                    //барма
                    string word = _baseNegativeForms[8].Remove(_baseNegativeForms[8].Trim().Length - 1);
                    //бармам
                    _negativeAffirmativeForms[48] = word + "м";
                    //бармабыз
                    _negativeAffirmativeForms[49] = word + "быз";
                    //бармассың
                    _negativeAffirmativeForms[50] = _baseNegativeForms[8] + "сың";
                    //бармассыз
                    _negativeAffirmativeForms[51] = _baseNegativeForms[8] + "сыз";
                    //бармас
                    _negativeAffirmativeForms[52] = _baseNegativeForms[8];
                    //бармаслар
                    _negativeAffirmativeForms[53] = _baseNegativeForms[8] + "лар";
                }
                //көт
                else
                {
                    //көтмим
                    _negativeAffirmativeForms[0] = _baseNegativeForms[0] + "м";
                    //көтмибез
                    _negativeAffirmativeForms[1] = _baseNegativeForms[0] + "без";
                    //көтмисең
                    _negativeAffirmativeForms[2] = _baseNegativeForms[0] + "сең";
                    //көтмисез
                    _negativeAffirmativeForms[3] = _baseNegativeForms[0] + "сез";
                    //көтми
                    _negativeAffirmativeForms[4] = _baseNegativeForms[0];
                    //көтмиләр
                    _negativeAffirmativeForms[5] = _baseNegativeForms[0] + "ләр";

                    //көтмәдем
                    _negativeAffirmativeForms[6] = _baseNegativeForms[1] + "м";
                    //көтмәдек
                    _negativeAffirmativeForms[7] = _baseNegativeForms[1] + "к";
                    //көтмәдең
                    _negativeAffirmativeForms[8] = _baseNegativeForms[1] + "ң";
                    //көтмәдегез
                    _negativeAffirmativeForms[9] = _baseNegativeForms[1] + "гез";
                    //көтмәде
                    _negativeAffirmativeForms[10] = _baseNegativeForms[1];
                    //көтмәделәр
                    _negativeAffirmativeForms[11] = _baseNegativeForms[1] + "ләр";

                    //
                    _negativeAffirmativeForms[12] = _baseNegativeForms[2] + "мен";
                    //
                    _negativeAffirmativeForms[13] = _baseNegativeForms[2] + "без";
                    //
                    _negativeAffirmativeForms[14] = _baseNegativeForms[2] + "сең";
                    //
                    _negativeAffirmativeForms[15] = _baseNegativeForms[2] + "сез";
                    //
                    _negativeAffirmativeForms[16] = _baseNegativeForms[2];
                    //
                    _negativeAffirmativeForms[17] = _baseNegativeForms[2] + "нәр";

                    //
                    _negativeAffirmativeForms[23] = _baseNegativeForms[0] + "ләр" + " иде";

                    //
                    _negativeAffirmativeForms[29] = _baseNegativeForms[2] + "нәр" + " иде";

                    //
                    _negativeAffirmativeForms[35] = _baseNegativeForms[5] + "нәр" + " иде";

                    //
                    _negativeAffirmativeForms[41] = _baseNegativeForms[7] + "ләр" + " иде";

                    //
                    _negativeAffirmativeForms[42] = _baseNegativeForms[7] + "мен";
                    //
                    _negativeAffirmativeForms[43] = _baseNegativeForms[7] + "без";
                    //
                    _negativeAffirmativeForms[44] = _baseNegativeForms[7] + "сең";
                    //
                    _negativeAffirmativeForms[45] = _baseNegativeForms[7] + "сез";
                    //
                    _negativeAffirmativeForms[46] = _baseNegativeForms[7];
                    //
                    _negativeAffirmativeForms[47] = _baseNegativeForms[7] + "ләр";

                    //көтмә
                    string word = _baseNegativeForms[8].Remove(_baseNegativeForms[8].Trim().Length - 1);
                    //көтмәм
                    _negativeAffirmativeForms[48] = word + "м";
                    //көтмәбез
                    _negativeAffirmativeForms[49] = word + "без";
                    //көтмәссең
                    _negativeAffirmativeForms[50] = _baseNegativeForms[8] + "сең";
                    //көтмәссез
                    _negativeAffirmativeForms[51] = _baseNegativeForms[8] + "сез";
                    //көтмәс
                    _negativeAffirmativeForms[52] = _baseNegativeForms[8];
                    //көтмәсләр
                    _negativeAffirmativeForms[53] = _baseNegativeForms[8] + "ләр";
                }
            }

            //
            _negativeAffirmativeForms[18] = _baseNegativeForms[3] + "м";
            //
            _negativeAffirmativeForms[19] = _baseNegativeForms[3] + "к";
            //
            _negativeAffirmativeForms[20] = _baseNegativeForms[3] + "ң";
            //
            _negativeAffirmativeForms[21] = _baseNegativeForms[3] + "гез";
            //
            _negativeAffirmativeForms[22] = _baseNegativeForms[3];

            //
            _negativeAffirmativeForms[24] = _baseNegativeForms[4] + "м";
            //
            _negativeAffirmativeForms[25] = _baseNegativeForms[4] + "к";
            //
            _negativeAffirmativeForms[26] = _baseNegativeForms[4] + "ң";
            //
            _negativeAffirmativeForms[27] = _baseNegativeForms[4] + "гез";
            //
            _negativeAffirmativeForms[28] = _baseNegativeForms[4];

            //
            _negativeAffirmativeForms[30] = _baseNegativeForms[5] + " иде" + "м";
            //
            _negativeAffirmativeForms[31] = _baseNegativeForms[5] + " иде" + "к";
            //
            _negativeAffirmativeForms[32] = _baseNegativeForms[5] + " иде" + "ң";
            //
            _negativeAffirmativeForms[33] = _baseNegativeForms[5] + " иде" + "гез";
            //
            _negativeAffirmativeForms[34] = _baseNegativeForms[5] + " иде";

            //
            _negativeAffirmativeForms[36] = _baseNegativeForms[6] + "м";
            //
            _negativeAffirmativeForms[37] = _baseNegativeForms[6] + "к";
            //
            _negativeAffirmativeForms[38] = _baseNegativeForms[6] + "ң";
            //
            _negativeAffirmativeForms[39] = _baseNegativeForms[6] + "гез";
            //
            _negativeAffirmativeForms[40] = _baseNegativeForms[6];
        }

        private void GetNegativeInterrogativeForms()
        {
            if (!_isSoft)
            {
                //
                _negativeInterrogativeForms[23] = _baseNegativeForms[0] + "лар" + " иде" + "м" + "ме";

                //
                _negativeInterrogativeForms[29] = _baseNegativeForms[2] + "нар" + " иде" + "м" + "ме";

                //
                _negativeInterrogativeForms[35] = _baseNegativeForms[5] + "нар" + " иде" + "м" + "ме";

                //
                _negativeInterrogativeForms[41] = _baseNegativeForms[7] + "лар" + " иде" + "м" + "ме";

                _baseNegativeForms[3] = _baseNegativeForms[0] + " иде" + "м" + "ме";
                _baseNegativeForms[4] = _baseNegativeForms[2] + " иде" + "м" + "ме";
                _baseNegativeForms[5] += "мы";
                _baseNegativeForms[6] = _baseNegativeForms[7] + " иде" + "м" + "ме";

                for (int i = 0; i < 18; i++)
                {
                    _negativeInterrogativeForms[i] = _negativeAffirmativeForms[i] + "мы";
                }

                for (int i = 42; i < 54; i++)
                {
                    _negativeInterrogativeForms[i] = _negativeAffirmativeForms[i] + "мы";
                }
            }
            else
            {
                //
                _negativeInterrogativeForms[23] = _baseNegativeForms[0] + "ләр" + " иде" + "м" + "ме";

                //
                _negativeInterrogativeForms[29] = _baseNegativeForms[2] + "нәр" + " иде" + "м" + "ме";

                //
                _negativeInterrogativeForms[35] = _baseNegativeForms[5] + "нар" + " иде" + "м" + "ме";

                //
                _negativeInterrogativeForms[41] = _baseNegativeForms[7] + "ләр" + " иде" + "м" + "ме";

                _baseNegativeForms[3] = _baseNegativeForms[0] + " иде" + "м" + "ме";
                _baseNegativeForms[4] = _baseNegativeForms[2] + " иде" + "м" + "ме";
                //_baseNegativeForms[5] += "ме";
                _baseNegativeForms[6] = _baseNegativeForms[7] + " иде" + "м" + "ме";

                for (int i = 0; i < 18; i++)
                {
                    _negativeInterrogativeForms[i] = _negativeAffirmativeForms[i] + "ме";
                }

                for (int i = 42; i < 54; i++)
                {
                    _negativeInterrogativeForms[i] = _negativeAffirmativeForms[i] + "ме";
                }
            }

            //
            _negativeInterrogativeForms[18] = _baseNegativeForms[3] + "м";
            //
            _negativeInterrogativeForms[19] = _baseNegativeForms[3] + "к";
            //
            _negativeInterrogativeForms[20] = _baseNegativeForms[3] + "ң";
            //
            _negativeInterrogativeForms[21] = _baseNegativeForms[3] + "гез";
            //
            _negativeInterrogativeForms[22] = _baseNegativeForms[3];

            //
            _negativeInterrogativeForms[24] = _baseNegativeForms[4] + "м";
            //
            _negativeInterrogativeForms[25] = _baseNegativeForms[4] + "к";
            //
            _negativeInterrogativeForms[26] = _baseNegativeForms[4] + "ң";
            //
            _negativeInterrogativeForms[27] = _baseNegativeForms[4] + "гез";
            //
            _negativeInterrogativeForms[28] = _baseNegativeForms[4];

            //
            _negativeInterrogativeForms[30] = _baseNegativeForms[5] + " иде" + "м" + "ме" + "м";
            //
            _negativeInterrogativeForms[31] = _baseNegativeForms[5] + " иде" + "м" + "ме" + "к";
            //
            _negativeInterrogativeForms[32] = _baseNegativeForms[5] + " иде" + "м" + "ме" + "ң";
            //
            _negativeInterrogativeForms[33] = _baseNegativeForms[5] + " иде" + "м" + "ме" + "гез";
            //
            _negativeInterrogativeForms[34] = _baseNegativeForms[5] + " иде" + "м" + "ме";

            //
            _negativeInterrogativeForms[36] = _baseNegativeForms[6] + "м";
            //
            _negativeInterrogativeForms[37] = _baseNegativeForms[6] + "к";
            //
            _negativeInterrogativeForms[38] = _baseNegativeForms[6] + "ң";
            //
            _negativeInterrogativeForms[39] = _baseNegativeForms[6] + "гез";
            //
            _negativeInterrogativeForms[40] = _baseNegativeForms[6];
        }

        private void GetInfinitives()
        {
            char lastLetter = _word.Last();

            if (_counter == 1)
            {
                if (lastLetter == ThirdException || lastLetter == FourthException)
                {
                    string word = _word;
                    if (lastLetter == ThirdException)
                    {
                        word = _word.Remove(_word.Trim().Length - 1);
                    }

                    if (!_isSoft)
                    {
                        _infinitives[0] = word + "ярга";
                        _infinitives[1] = word + "маска";
                        _infinitives[2] = _infinitives[0] + "мы";
                        _infinitives[3] = _infinitives[1] + "мы";
                    }
                    else
                    {
                        _infinitives[0] = word + "яргә";
                        _infinitives[1] = word + "мәскә";
                        _infinitives[2] = _infinitives[0] + "ме";
                        _infinitives[3] = _infinitives[1] + "ме";
                    }
                }
                else if (lastLetter == 'р' || lastLetter == 'л')
                {
                    ConjugateCommon();
                }
                else if (_word == "кайт")
                {
                    _infinitives[0] = _word + "ырга";
                    _infinitives[1] = _word + "маска";
                    _infinitives[2] = _infinitives[0] + "мы";
                    _infinitives[3] = _infinitives[1] + "мы";
                }
                else if (_word == "эл" || _word == "көл")
                {
                    _infinitives[0] = _word + "әргә";
                    _infinitives[1] = _word + "мәскә";
                    _infinitives[2] = _infinitives[0] + "ме";
                    _infinitives[3] = _infinitives[1] + "ме";
                }
                else
                {
                    if (!_isSoft)
                    {
                        _infinitives[0] = _word + "арга";
                        _infinitives[1] = _word + "маска";
                        _infinitives[2] = _infinitives[0] + "мы";
                        _infinitives[3] = _infinitives[1] + "мы";
                    }
                    else
                    {
                        _infinitives[0] = _word + "әргә";
                        _infinitives[1] = _word + "мәскә";
                        _infinitives[2] = _infinitives[0] + "ме";
                        _infinitives[3] = _infinitives[1] + "ме";
                    }
                }
            }
            else
            {
                if (_vowels.Contains(lastLetter))
                {
                    if (!_isSoft)
                    {
                        _infinitives[0] = _word + "рга";
                        _infinitives[1] = _word + "маска";
                        _infinitives[2] = _infinitives[0] + "мы";
                        _infinitives[3] = _infinitives[1] + "мы";
                    }
                    else
                    {
                        _infinitives[0] = _word + "ргә";
                        _infinitives[1] = _word + "мәскә";
                        _infinitives[2] = _infinitives[0] + "ме";
                        _infinitives[3] = _infinitives[1] + "ме";
                    }
                }

                if (_consonants.Contains(lastLetter))
                {
                    ConjugateCommon();
                }
            }

            void ConjugateCommon()
            {
                if (!_isSoft)
                {
                    _infinitives[0] = _word + "ырга";
                    _infinitives[1] = _word + "маска";
                    _infinitives[2] = _infinitives[0] + "мы";
                    _infinitives[3] = _infinitives[1] + "мы";
                }
                else
                {
                    _infinitives[0] = _word + "ергә";
                    _infinitives[1] = _word + "мәскә";
                    _infinitives[2] = _infinitives[0] + "ме";
                    _infinitives[3] = _infinitives[1] + "ме";
                }
            }

            for (int i = 0; i < _infinitives.Length; ++i)
            {
                _infinitives[i] = _infinitives[i].ToUpper();
            }
        }

        private void Print()
        {
            MakeTable(PositiveAffirmativeTable, _positiveAffirmativeForms);
            MakeTable(PositiveInterrogativeTable, _positiveInterrogativeForms);
            MakeTable(NegativeAffirmativeTable, _negativeAffirmativeForms);
            MakeTable(NegativeInterrogativeTable, _negativeInterrogativeForms);

            void MakeTable(DataGrid table, IReadOnlyList<string> forms)
            {
                List<TableRow> tableRows = new List<TableRow>
                {
                    new TableRow
                    {
                        Person = "I",
                        Tense = "Хәзерге заман",
                        Singular = forms[0],
                        Plural = forms[1]
                    },
                    new TableRow
                    {
                        Person = "II",
                        Tense = "Хәзерге заман",
                        Singular = forms[2],
                        Plural = forms[3]
                    },
                    new TableRow
                    {
                        Person = "III",
                        Tense = "Хәзерге заман",
                        Singular = forms[4],
                        Plural = forms[5]
                    },
                    new TableRow(),
                    new TableRow
                    {
                        Person = "I",
                        Tense = "Билгеле үткән заман",
                        Singular = forms[6],
                        Plural = forms[7]
                    },
                    new TableRow
                    {
                        Person = "II",
                        Tense = "Билгеле үткән заман",
                        Singular = forms[8],
                        Plural = forms[9]
                    },
                    new TableRow
                    {
                        Person = "III",
                        Tense = "Билгеле үткән заман",
                        Singular = forms[10],
                        Plural = forms[11]
                    },
                    new TableRow(),
                    new TableRow
                    {
                        Person = "I",
                        Tense = "Билгесез үткән заман",
                        Singular = forms[12],
                        Plural = forms[13]
                    },
                    new TableRow
                    {
                        Person = "II",
                        Tense = "Билгесез үткән заман",
                        Singular = forms[14],
                        Plural = forms[15]
                    },
                    new TableRow
                    {
                        Person = "III",
                        Tense = "Билгесез үткән заман",
                        Singular = forms[16],
                        Plural = forms[17]
                    },
                    new TableRow(),
                    new TableRow
                    {
                        Person = "I",
                        Tense = "Тәмамланмаган үткән заман",
                        Singular = forms[18],
                        Plural = forms[19]
                    },
                    new TableRow
                    {
                        Person = "II",
                        Tense = "Тәмамланмаган үткән заман",
                        Singular = forms[20],
                        Plural = forms[21]
                    },
                    new TableRow
                    {
                        Person = "III",
                        Tense = "Тәмамланмаган үткән заман",
                        Singular = forms[22],
                        Plural = forms[23]
                    },
                    new TableRow(),
                    new TableRow
                    {
                        Person = "I",
                        Tense = "Күптән үткән заман",
                        Singular = forms[24],
                        Plural = forms[25]
                    },
                    new TableRow
                    {
                        Person = "II",
                        Tense = "Күптән үткән заман",
                        Singular = forms[26],
                        Plural = forms[27]
                    },
                    new TableRow
                    {
                        Person = "III",
                        Tense = "Күптән үткән заман",
                        Singular = forms[28],
                        Plural = forms[29]
                    },
                    new TableRow(),
                    new TableRow
                    {
                        Person = "I",
                        Tense = "Кабатлаулы үткән заман",
                        Singular = forms[30],
                        Plural = forms[31]
                    },
                    new TableRow
                    {
                        Person = "II",
                        Tense = "Кабатлаулы үткән заман",
                        Singular = forms[32],
                        Plural = forms[33]
                    },
                    new TableRow
                    {
                        Person = "III",
                        Tense = "Кабатлаулы үткән заман",
                        Singular = forms[34],
                        Plural = forms[35]
                    },
                    new TableRow(),
                    new TableRow
                    {
                        Person = "I",
                        Tense = "Киләчәк-үткән заман",
                        Singular = forms[36],
                        Plural = forms[37]
                    },
                    new TableRow
                    {
                        Person = "II",
                        Tense = "Киләчәк-үткән заман",
                        Singular = forms[38],
                        Plural = forms[39]
                    },
                    new TableRow
                    {
                        Person = "III",
                        Tense = "Киләчәк-үткән заман",
                        Singular = forms[40],
                        Plural = forms[41]
                    },
                    new TableRow(),
                    new TableRow
                    {
                        Person = "I",
                        Tense = "Билгеле киләчәк заман",
                        Singular = forms[42],
                        Plural = forms[43]
                    },
                    new TableRow
                    {
                        Person = "II",
                        Tense = "Билгеле киләчәк заман",
                        Singular = forms[44],
                        Plural = forms[45]
                    },
                    new TableRow
                    {
                        Person = "III",
                        Tense = "Билгеле киләчәк заман",
                        Singular = forms[46],
                        Plural = forms[47]
                    },
                    new TableRow(),
                    new TableRow
                    {
                        Person = "I",
                        Tense = "Билгесез киләчәк заман",
                        Singular = forms[48],
                        Plural = forms[49]
                    },
                    new TableRow
                    {
                        Person = "II",
                        Tense = "Билгесез киләчәк заман",
                        Singular = forms[50],
                        Plural = forms[51]
                    },
                    new TableRow
                    {
                        Person = "III",
                        Tense = "Билгесез киләчәк заман",
                        Singular = forms[52],
                        Plural = forms[53]
                    }
                };
                table.ItemsSource = tableRows;
                /*foreach (DataGridColumn column in table.Columns)
                {
                    column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Pixel);
                    column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);
                }*/
                int[] columns = {0, 2, 3};
                for (int i = 0; i < columns.Length; ++i)
                {
                    table.Columns[columns[i]].Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);
                }
                //table.Columns[1].Width = 10;
            }

            FirstInfinitive.Content = _infinitives[0];
            SecondInfinitive.Content = _infinitives[1];
            ThirdInfinitive.Content = _infinitives[2];
            FourthInfinitive.Content = _infinitives[3];
        }

        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private void FirstSpecificLetter_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += 'ө';
            InputFocus();
        }

        private void SecondSpecificLetter_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += 'ң';
            InputFocus();
        }

        private void ThirdSpecificLetter_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += 'ү';
            InputFocus();
        }

        private void FourthSpecificLetter_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += 'ә';
            InputFocus();
        }

        private void FifthSpecificLetter_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += 'җ';
            InputFocus();
        }

        private void SixthSpecificLetter_Click(object sender, RoutedEventArgs e)
        {
            Input.Text += 'һ';
            InputFocus();
        }

        private void InputFocus()
        {
            Input.Focus();
            Input.CaretIndex = Input.Text.Length;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Input.Text = "";
        }

        private void ControlKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Conjugate_Click(sender, e);
            }

            if (e.Key == Key.Escape)
            {
                Clear_Click(sender, e);
                Input.Focus();
            }
        }
    }

    public class TableRow
    {
        public string Person { get; set; }
        public string Tense { get; set; }
        public string Singular { get; set; }
        public string Plural { get; set; } }
}