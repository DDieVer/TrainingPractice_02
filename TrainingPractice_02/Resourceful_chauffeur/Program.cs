using System;
using System.IO;

namespace Resourceful_chauffeur
{
    class Program
    {
        static string[] LabirintFile = File.ReadAllLines("maps/Streets.txt");
        static char[,] map = new char[LabirintFile.Length, LabirintFile[0].Length];//Массив символов/лабиринт

        static string[] Recordsfile = File.ReadAllLines("maps/Rec.txt");//
        static string[] Records = new string[11];
        static int[] debts = { 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999, 999999 };

        static char[,] Or_map = new char[LabirintFile.Length, LabirintFile[0].Length];

        static string[,] way =
        {
            { "┘└v┐","┘└","┌┐┘└v","└","┌┘v","<>","v┐","<┘","└┌" },
            { "┘v^","<>┘┌","┘┌┐","┘└<>","v^┌","><┌","v^┐┘","<└┐","v^" },
            { "v^┐┘","┘>└┌","┘└┌┐","<┘└┌┐","^┘└","","^┘└","┘┌","v^└" },
            { "┘^┐","└┐┌>","┘└┐","<>└┌┐","┘└┐","<>┌┐","┌┘^└┐","<┐┌","└^┐" },
            { "","┌>┐","","┌<>┐","","┌<>┐","","┌<┐","" },
        };

        static int HeroX;//координаты Игрока
        static int HeroY;

        static int traffic_fine = 0;

        static char Mup = ' ';
        static char Mdown = ' ';
        static char Mleft = ' ';
        static char Mright = ' ';
        static char Mup_r = ' ';
        static char Mup_l = ' ';
        static char Mdown_r = ' ';
        static char Mdown_l = ' ';

        static int Fx;//Координаты финиша
        static int Fy;


        static ConsoleKeyInfo key;
        static Random rnd = new Random();
        static bool mist = false;//Проверка на попытку войти в стену



        static string[] ReadRec()
        {
            Recordsfile = File.ReadAllLines("maps/Rec.txt");
            int i = 0;
            if (Recordsfile.Length != 0)
            {
                do
                {
                    Records[(i + 1) / 2] = Recordsfile[i];
                    i++;
                    debts[(i - 1) / 2] = Convert.ToInt32(Recordsfile[i]);
                    i++;
                } while (i < Recordsfile.GetLength(0));

                int n = 10;
                int min = 0;
                do
                {
                    min = 10 - n;
                    for (i = 10 - n; i < 10; i++) if (debts[min] > debts[i]) min = i;
                    int boofi = debts[10 - n];
                    string boofs = Records[10 - n];


                    Records[10 - n] = Records[min];
                    debts[10 - n] = debts[min];

                    debts[min] = boofi;
                    Records[min] = boofs;

                    n--;

                } while (n != 1);
            }
            return Records;
        }

        static char[,] ReadMap()//Чтение лабиринта в массив
        {
            HeroX = 0;
            HeroY = 0;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = LabirintFile[i][j];

                    if (map[i, j] == '■')
                    {
                        HeroX = i;
                        HeroY = j;
                    }
                    if (map[i, j] == 'F')
                    {
                        Fx = i;
                        Fy = j;
                    }
                }
            }
            return map;
        }

        static void labels()
        {
            bool Fin = false;


            if ((HeroX == 3) && (HeroY == 1))
            {
                Mright = '>';
                Mup_r = '┐';
            }
            else
            {
                Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('<', StringComparison.CurrentCultureIgnoreCase);
                if (Fin == true) Mleft = '<';

                Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('>', StringComparison.CurrentCultureIgnoreCase);
                if (Fin == true) Mright = '>';

                Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('v', StringComparison.CurrentCultureIgnoreCase);
                if (Fin == true) Mdown = 'v';

                Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('^', StringComparison.CurrentCultureIgnoreCase);
                if (Fin == true) Mup = '^';

                Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('┌', StringComparison.CurrentCultureIgnoreCase);
                if (Fin == true) Mup_l = '┌';

                Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('┘', StringComparison.CurrentCultureIgnoreCase);
                if (Fin == true) Mdown_r = '┘';

                Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('└', StringComparison.CurrentCultureIgnoreCase);
                if (Fin == true) Mdown_l = '└';

                Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('┐', StringComparison.CurrentCultureIgnoreCase);
                if (Fin == true) Mup_r = '┐';
            }
        }

        static void Rread()
        {
            using (FileStream fstream = new FileStream($"maps/Rec.txt", FileMode.Create))
            {
                Console.WriteLine("Запись в процессе...");
                int i = 0;
                byte[] array;
                do
                {
                    array = System.Text.Encoding.Default.GetBytes($"{Records[i]}\n");
                    fstream.Write(array, 0, array.Length);

                    array = System.Text.Encoding.Default.GetBytes($"{debts[i]}\n");
                    fstream.Write(array, 0, array.Length);

                    i++;
                    

                } while ((i <= 9) && (Records[i] != null));
                Console.WriteLine("Запись прошла успешно! Вы в таблице лидеров!");
            }
        }

        static void Rin()
        {
            Console.Clear();
            int i = 0;
            if (Recordsfile.Length == 0)
            {
            rename:
                Console.WriteLine($"Ваш результат - штраф на сумму {traffic_fine} рублей!");
                Console.Write("Введите ваше имя для сохранения: ");
                Records[0] = Console.ReadLine();
                if (Records[0].Length == 0) goto rename;

                debts[0] = traffic_fine;

                Rread();
                return;
            }
            else
            {
                do
                {
                    if (debts[i] > traffic_fine)
                    {
                        if (i == 9)
                        {
                        rename1:
                            Console.WriteLine($"Ваш результат - штраф на сумму {traffic_fine} рублей!");
                            Console.Write("Введите ваше имя для сохранения: ");
                            Records[9] = Console.ReadLine();
                            if (Records[9].Length == 0) goto rename1;

                            debts[9] = traffic_fine;

                            Rread();
                            return;
                        }
                        else
                        {
                            int j = i;
                            i = 9;
                            do
                            {
                                Records[i] = Records[i - 1];
                                debts[i] = debts[i - 1];

                                i--;
                            } while (i > j);

                            Console.WriteLine($"Ваш результат - штраф на сумму {traffic_fine} рублей!");
                            Console.Write("Введите ваше имя для сохранения: ");

                            rename2:
                            Records[j] = Console.ReadLine();
                            if (Records[i].Length == 0) goto rename2;
                            debts[j] = traffic_fine;

                            Rread();
                            return;
                        }

                    }
                    i++;

                } while (i < 10);
            }

            Console.WriteLine("Вы не смогли попасть в десятку лучших, попробуйте ещё раз!");
        }

        static void DrawMap(char[,] map)//Функция вывода лабиринта в консоль
        {
            labels();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                Console.Write("\t\t");
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '■') Console.Write($"{Mup_l}  {Mup} {Mup_r}");
                    else if (map[i, j] == '█') for (int l = 0; l < 6; l++) Console.Write(map[i, j]);
                    else if (map[i, j] == 'F') for (int l = 0; l < 6; l++) Console.Write(' ');
                    else for (int l = 0; l < 6; l++) Console.Write(map[i, j]);
                }
                Console.Write('\n');

                Console.Write("\t\t");
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '■') Console.Write($"{Mleft}  ■ {Mright}");
                    else if (map[i, j] == '█') for (int l = 0; l < 6; l++) Console.Write(map[i, j]);
                    else if (map[i, j] == 'F') Console.Write($"   F  ");
                    else for (int l = 0; l < 6; l++) Console.Write(map[i, j]);
                }
                Console.Write('\n');

                Console.Write("\t\t");
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '■') Console.Write($"{Mdown_l}  {Mdown} {Mdown_r}");
                    else if (map[i, j] == '█') for (int l = 0; l < 6; l++) Console.Write(map[i, j]);
                    else if (map[i, j] == 'F') for (int l = 0; l < 6; l++) Console.Write(' ');
                    else for (int l = 0; l < 6; l++) Console.Write(map[i, j]);
                }
                Console.Write('\n');
            }

            Mup = ' ';
            Mdown = ' ';
            Mleft = ' ';
            Mright = ' ';
            Mup_r = ' ';
            Mup_l = ' ';
            Mdown_r = ' ';
            Mdown_l = ' ';
        }

        static void HP_Mistakes() //Правила здоровье и ошибки
        {
            Console.WriteLine("F - Выезд из города.");
            Console.WriteLine("■ - Вы.");
            Console.WriteLine("Управление:");
            Console.WriteLine("Для перемещения используйте стрелочки на клавиатуре.");
            Console.WriteLine("Для начала нажмите на стрелочку, которая направленна на нужный вам перекрёсток.");
            Console.WriteLine("Затем стрелочку, которая направленна в нужном для вас направлении выезда с перекрёстка.");


            //for (int i = 0; i < 10; i++) Console.WriteLine($"{Records[i].Length}\t {Records[i]}\n");
            Console.WriteLine($"Сумма штрафа - {traffic_fine} рублей");
            if (mist == true)
            {
                Console.WriteLine("Тут стена, сюда нельзя!\n");
                mist = false;
            }
        }

        static void Record()
        {

            Console.Clear();
            Records = new string[10];
            Records = ReadRec();
            if (Recordsfile.Length == 0) Console.WriteLine("Рекордов ещё нет!\nСтаньте первым!");
            else
            {
                int i = 0;
                do
                {
                    Console.WriteLine($"{i + 1}: {Records[i]}---{debts[i]}");
                    i++;

                } while ((Recordsfile.GetLength(0) > i*2) && (i < 10));
            }
            Console.ReadKey();
            return;
        }

        static void step()
        {
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (map[HeroX, HeroY - 1] != '█')
                    {
                        key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.LeftArrow)
                        {
                            if ((map[HeroX, HeroY - 2] != '█') && (HeroY - 2 > 0))
                            {
                                bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('<', StringComparison.CurrentCultureIgnoreCase);
                                if (Fin == false) traffic_fine += 1000;

                                map[HeroX, HeroY - 2] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY -= 2;
                            }
                            else mist = true;
                        }
                        else if (key.Key == ConsoleKey.UpArrow)
                        {
                            if (map[HeroX - 1, HeroY - 1] != '█')
                            {
                                bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('┌', StringComparison.CurrentCultureIgnoreCase);
                                if (Fin == false) traffic_fine += 1000;

                                map[HeroX - 1, HeroY - 1] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY -= 1;
                                HeroX -= 1;
                            }
                            else mist = true;
                        }
                        else if (key.Key == ConsoleKey.DownArrow)
                        {
                            if (map[HeroX + 1, HeroY - 1] != '█')
                            {
                                bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('└', StringComparison.CurrentCultureIgnoreCase);
                                if (Fin == false) traffic_fine += 1000;

                                map[HeroX + 1, HeroY - 1] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY -= 1;
                                HeroX += 1;
                            }
                            else mist = true;
                        }
                        else mist = true;
                    }
                    else mist = true;

                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (map[HeroX, HeroY + 1] != '█')
                    {
                        key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.RightArrow)
                        {
                            if ((map[HeroX, HeroY + 2] != '█') && (HeroY + 2 < map.GetLength(1)))
                            {
                                if ((HeroX != 3) || (HeroY != 1))
                                {
                                    bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('>', StringComparison.CurrentCultureIgnoreCase);
                                    if (Fin == false) traffic_fine += 1000;
                                }


                                map[HeroX, HeroY + 2] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY += 2;
                            }
                            else mist = true;
                        }
                        else if (key.Key == ConsoleKey.UpArrow)
                        {
                            if (map[HeroX - 1, HeroY + 1] != '█')
                            {
                                if ((HeroX != 3) || (HeroY != 1))
                                {
                                    bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('┐', StringComparison.CurrentCultureIgnoreCase);
                                    if (Fin == false) traffic_fine += 1000;
                                }


                                map[HeroX - 1, HeroY + 1] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY += 1;
                                HeroX -= 1;
                            }
                            else mist = true;
                        }
                        else if (key.Key == ConsoleKey.DownArrow)
                        {
                            if (map[HeroX + 1, HeroY + 1] != '█')
                            {
                                if ((HeroX == 3) && (HeroY == 1))
                                {
                                    traffic_fine += 1000;
                                }
                                else
                                {
                                    bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('┘', StringComparison.CurrentCultureIgnoreCase);
                                    if (Fin == false) traffic_fine += 1000;
                                }

                                map[HeroX + 1, HeroY + 1] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY += 1;
                                HeroX += 1;
                            }
                            else mist = true;
                        }
                        else mist = true;
                    }
                    else mist = true;

                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (map[HeroX - 1, HeroY] != '█')
                    {
                        key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.UpArrow)
                        {
                            if ((map[HeroX - 2, HeroY] != '█') && (HeroX - 2 > 0))
                            {
                                bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('^', StringComparison.CurrentCultureIgnoreCase);
                                if (Fin == false) traffic_fine += 1000;

                                map[HeroX - 2, HeroY] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroX -= 2;
                            }
                            else mist = true;
                        }
                        else if (key.Key == ConsoleKey.LeftArrow)
                        {
                            if (map[HeroX - 1, HeroY - 1] != '█')
                            {
                                bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('┌', StringComparison.CurrentCultureIgnoreCase);
                                if (Fin == false) traffic_fine += 1000;

                                map[HeroX - 1, HeroY - 1] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY -= 1;
                                HeroX -= 1;
                            }
                            else mist = true;
                        }
                        else if (key.Key == ConsoleKey.RightArrow)
                        {
                            if (map[HeroX - 1, HeroY + 1] != '█')
                            {
                                bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('┐', StringComparison.CurrentCultureIgnoreCase);
                                if (Fin == false) traffic_fine += 1000;

                                map[HeroX - 1, HeroY + 1] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY += 1;
                                HeroX -= 1;
                            }
                            else mist = true;
                        }
                        else mist = true;
                    }
                    else mist = true;

                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (map[HeroX + 1, HeroY] != '█')
                    {
                        key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.DownArrow)
                        {
                            if ((map[HeroX + 2, HeroY] != '█') && (HeroX + 2 < map.GetLength(0)))
                            {
                                bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('v', StringComparison.CurrentCultureIgnoreCase);
                                if (Fin == false) traffic_fine += 1000;

                                map[HeroX + 2, HeroY] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroX += 2;
                            }
                            else mist = true;
                        }
                        else if (key.Key == ConsoleKey.LeftArrow)
                        {
                            if (map[HeroX + 1, HeroY - 1] != '█')
                            {
                                bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('└', StringComparison.CurrentCultureIgnoreCase);
                                if (Fin == false) traffic_fine += 1000;

                                map[HeroX + 1, HeroY - 1] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY -= 1;
                                HeroX += 1;
                            }
                            else mist = true;
                        }
                        else if (key.Key == ConsoleKey.RightArrow)
                        {
                            if (map[HeroX + 1, HeroY + 1] != '█')
                            {
                                bool Fin = way[(HeroX - 1) / 2, HeroY - 2].Contains('┘', StringComparison.CurrentCultureIgnoreCase);
                                if (Fin == false) traffic_fine += 1000;

                                map[HeroX + 1, HeroY + 1] = '■';
                                map[HeroX, HeroY] = ' ';
                                HeroY += 1;
                                HeroX += 1;
                            }
                            else mist = true;
                        }
                        else mist = true;
                    }
                    else mist = true;

                }

            }
        }
        static void Walk()
        {
            do
            {
                Console.Clear();
                HP_Mistakes();
                DrawMap(map);

                step();
            } while (map[HeroX, HeroY] != map[Fx, Fy]);

            Console.Clear();
            Console.WriteLine(@" _    ___      __                  
| |  / (_)____/ /_____  _______  __
| | / / / ___/ __/ __ \/ ___/ / / /
| |/ / / /__/ /_/ /_/ / /  / /_/ / 
|___/_/\___/\__/\____/_/   \__, /  
                          /____/");
            Console.ReadKey();
            Rin();
            traffic_fine = 0;
            map = ReadMap();
            Console.ReadKey();
            return;
        }


        static void Main(string[] args)
        {
            Console.SetWindowSize(105, 50);

            map = ReadMap();
            Records = ReadRec();

            do
            {
                Console.Clear();
                Console.WriteLine(@"    ____                                       ____      __
   / __ \___  _________  __  _______________  / __/_  __/ /
  / /_/ / _ \/ ___/ __ \/ / / / ___/ ___/ _ \/ /_/ / / / / 
 / _, _/  __(__  ) /_/ / /_/ / /  / /__/  __/ __/ /_/ / /  
/_/ |_|\___/____/\____/\__,_/_/   \___/\___/_/  \__,_/_/   
        __                ________                         
  _____/ /_  ____ ___  __/ __/ __/__  __  _______          
 / ___/ __ \/ __ `/ / / / /_/ /_/ _ \/ / / / ___/          
/ /__/ / / / /_/ / /_/ / __/ __/  __/ /_/ / /              
\___/_/ /_/\__,_/\__,_/_/ /_/  \___/\__,_/_/ ");

                Console.WriteLine("\n");
                Console.WriteLine("S - Start");
                Console.WriteLine("R - Records");
                Console.WriteLine("E - Exit");

                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.R) Record();
                else if (key.Key == ConsoleKey.S) Walk();

            } while (key.Key != ConsoleKey.E);

        }
    }
}
