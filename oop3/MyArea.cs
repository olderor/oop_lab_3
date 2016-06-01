using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oop3
{
    static class MyArea
    {
        static public int Area(Image im)
        {
            List<Figure> l = im.figures;
            /*
            Tuple<double, double> steps = getSteps(l);

            if (steps == null)
                return 0;

            double step = steps.Item1 > steps.Item2 ? steps.Item1 : steps.Item2;*/

            //Tuple<Point, Point> frame = getCoordinates(l);

            bool[,] matrix = new bool[Max + 1, Max + 1];

            int area = 0;

            foreach (Figure f in l)
            {
                bool[,] fig = new bool[Max + 1, Max + 1];
                bool[,] fig2 = new bool[Max + 1, Max + 1];

                for (int i = 0; i < Max; i++)
                {
                    Tuple<Point, Point> coords = f.GetEdgeCoordinates();

                    int firstx = (int)coords.Item1.X;
                    Tuple<double, double> ys = f.GetYBy(firstx);
                    int firstmy1 = (int)(ys.Item1);
                    int firstmy2 = (int)(ys.Item2);

                    int prevy1 = firstmy1;
                    int prevy2 = firstmy2;

                    for (int x = firstx + 1; x <= coords.Item2.X; x++)
                    {
                        // нарисовать линию от у1 до у2 чтобы соединить точки
                        ys = f.GetYBy(x);
                        int mx = (int)(x);
                        int my1 = (int)(ys.Item1);
                        int my2 = (int)(ys.Item2);
                        addLine(x, prevy1, my1, ref fig); //matrix fig
                        addLine(x, prevy2, my2, ref fig); //matrix fig


                        fig2[x, prevy1] = true;
                        fig2[x, prevy2] = true;

                        prevy1 = my1;
                        prevy2 = my2;
                    }

                    addLine(firstx, firstmy1, prevy1, ref fig); //matrix fig
                    addLine(firstx, firstmy2, prevy2, ref fig); //matrix fig



                }
                Console.Clear();
                printMatrix(fig);
                Console.Clear();
                printMatrix(fig2);

                //drawFigure(ref fig, ref matrix, ref area); //
            }

            Console.Clear();
            printMatrix(matrix);

            return area;
        }

        static private void printMatrix(bool[,] matrix)
        {
            for (int i = 0; i < Max; i++)
            {
                for (int j = 0; j < Max; j++)
                    if (matrix[j, i])
                        Console.Write('#');
                    else
                        Console.Write('.');
                Console.WriteLine();
            }

        }

        static public int Max = 100;

        static private Tuple<Point, Point> getCoordinates(List<Figure> l)
        {
            Tuple<Point, Point> res = new Tuple<Point, Point>(new Point(Max, Max), new Point());
            foreach (Figure f in l)
            {
                Tuple<Point, Point> coords = f.GetEdgeCoordinates();
                //min
                if (coords.Item1.X < res.Item1.X)
                    res.Item1.X = coords.Item1.X;
                if (coords.Item1.Y < res.Item1.Y)
                    res.Item1.Y = coords.Item1.Y;
                //max
                if (coords.Item2.X > res.Item2.X)
                    res.Item2.X = coords.Item2.X;
                if (coords.Item2.Y > res.Item2.Y)
                    res.Item2.Y = coords.Item2.Y;
            }

            return res;
        }

        static private Tuple<double, double> getSteps(List<Figure> l)
        {
            Tuple<Point, Point> coords = getCoordinates(l);
            if (coords == null)
                return null;

            double xStep = (coords.Item2.X - coords.Item1.X) / Max;
            double yStep = (coords.Item2.Y - coords.Item1.Y) / Max;

            return new Tuple<double, double>(xStep, yStep);
        }
        
        private static void addLine(int x, int y1, int y2, ref bool[,] matrix)
        {
            if (y1 > y2)
            {
                int t = y1;
                y1 = y2;
                y2 = t;
            }
            for (int i = y1; i <= y2; i++)
            {
                matrix[x, i] = true;
            }
        }

        private static void drawFigure(ref bool[,] figure, ref bool[,] matrix, ref int area)
        {
            //Console.Clear();
            //printMatrix(figure);

            for (int x = 0; x < Max + 1; x++)
            {
                int y1 = 0, y2 = Max;
                for (; y1 < Max + 1; y1++)
                    if (figure[x, y1])
                        break;

                for (; y2 >= 0; y2--)
                    if (figure[x, y2])
                        break;

                for (int i = y1; i <= y2; i++)
                {
                    figure[x, i] = true;
                    if (!matrix[x, i])
                        area++;
                    matrix[x, i] = true;
                }
            }

            //Console.Clear();
            //printMatrix(figure);
        }
    }
}
