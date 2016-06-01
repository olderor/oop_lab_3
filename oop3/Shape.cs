using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace oop3
{
    /*
    class Line
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow(End.X - Start.X, 2) + Math.Pow(End.Y - Start.Y, 2));
            }
        }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return "Line: Start: " + Start + ", End: " + End;
        }

    }
    */

    /*
    class Polygon : Shape
    {
        private List<Line> lines;


        public Polygon(params Line[] lines)
        {
            this.lines = lines.ToList();
        }

        public Polygon(params Point[] points)
        {
            lines = new List<Line>();
            for (int i = 1; i < points.Length; i++)
                lines.Add(new Line(points[i], points[i - 1]));
            lines.Add(new Line(points[0], points[points.Length - 1]));
        }

        public override double Area()
        {
            double s = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                s += lines[i].Start.X * lines[i].End.Y;
                s -= lines[i].Start.Y * lines[i].End.X;
            }
            return Math.Abs(s) / 2.0;
        }

        public override double Perimeter()
        {
            double p = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                p += lines[i].Length;
            }
            return p;
        }
    }
    */

    [Serializable]
    class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point() { }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Move(double x, double y)
        {
            X += x;
            Y += y;
        }

        public override string ToString()
        {
            return "Point: X: " + X + ", Y: " + Y;
        }
    }

    [Serializable]
    abstract class Figure
    {
        public Color StrokeColor;

        public Figure()
        {
            StrokeColor = Color.Black;
        }

        public Figure(Color strokeColor)
        {
            StrokeColor = strokeColor;
        }
        
        public abstract double Area();

        // я бы указал эти методы абстрактными или сделал бы интерфейс
        public virtual void Move(double x, double y) { }
        public virtual void MoveTo(double x, double y) { }
        public virtual void MoveTo(Point p) { }

        public virtual void Scale(double factor) { }

        public virtual void CreateDrawing(object sender, PaintEventArgs e) { }

        public abstract Tuple<Point, Point> GetEdgeCoordinates();
        public abstract Tuple<double, double> GetYBy(double x);

        [STAThread]
        public virtual void Draw() { }
        
        public override string ToString()
        {
            return "Figure";
        }
    }

    [Serializable]
    abstract class PlaneFigure : Figure
    {
        public PlaneFigure() : base() { }

        public PlaneFigure(Color strokeColor) : base(strokeColor) { }
        
        public abstract double Perimeter();

        public override string ToString()
        {
            return "PlaneFigure";
        }
    }

    [Serializable]
    abstract class VolumetricFigure : Figure
    {
        public VolumetricFigure() : base() { }

        public VolumetricFigure(Color strokeColor) : base(strokeColor) { }
        
        public abstract double Volume();

        public override string ToString()
        {
            return "VolumetricFigure";
        }
    }
    
    interface FilledFigure
    {
        Color BackgroundColor { get; set; }
    }

    [Serializable]
    class Ellipse : PlaneFigure
    {
        internal double SemiMajorAxe { get; set; }
        internal double SemiMinorAxe { get; set; }
        public Point Centre { get; set; }

        public Point this[double angle]
        {
            get
            {
                angle = angle * Math.PI / 180.0;
                return new Point(SemiMajorAxe * Math.Cos(angle) + Centre.X, SemiMinorAxe * Math.Sin(angle) + Centre.Y);
            }
        }

        public override Tuple<Point, Point> GetEdgeCoordinates()
        {
            Point min = new Point();
            Point max = new Point();
            
            min.X = Centre.X - SemiMajorAxe;
            max.X = Centre.X + SemiMajorAxe;
            min.Y = Centre.Y - SemiMinorAxe;
            max.Y = Centre.Y + SemiMinorAxe;

            return new Tuple<Point, Point>(min, max);
        }

        public override Tuple<double, double> GetYBy(double x)
        {
            x = x - Centre.X;
            double y = Math.Sqrt(Math.Pow(SemiMinorAxe, 2) - Math.Pow(x * SemiMinorAxe, 2) / Math.Pow(SemiMajorAxe, 2));
            return new Tuple<double, double>(-y + Centre.Y, y + Centre.Y);
        }

        public Ellipse(Point centre, double semiMajorAxe, double semiMinorAxe) : base()
        {
            Centre = centre;
            SemiMajorAxe = semiMajorAxe;
            SemiMinorAxe = semiMinorAxe;
        }

        public Ellipse(Point centre, double semiMajorAxe, double semiMinorAxe, Color strokeColor) : base(strokeColor)
        {
            Centre = centre;
            SemiMajorAxe = semiMajorAxe;
            SemiMinorAxe = semiMinorAxe;
        }

        public override double Perimeter()
        {
            return 4 * (SemiMajorAxe * SemiMinorAxe + Math.Pow(SemiMajorAxe - SemiMinorAxe, 2)) / (SemiMajorAxe + SemiMinorAxe);
        }

        public override double Area()
        {
            return Math.PI * SemiMajorAxe * SemiMinorAxe;
        }

        public override void CreateDrawing(object sender, PaintEventArgs e)
        {
            base.CreateDrawing(sender, e);
            Graphics g = e.Graphics;
            g.DrawEllipse(new Pen(StrokeColor), (float)(Centre.X - SemiMajorAxe), (float)(Centre.Y - SemiMinorAxe), (float)SemiMajorAxe * 2, (float)SemiMinorAxe * 2);
        }
        
        public override void Draw()
        {
            Form f = new Form();
            f.Paint += CreateDrawing;
            int width = Convert.ToInt32(Math.Ceiling(Centre.X + SemiMajorAxe * 2 + 10));
            int height = Convert.ToInt32(Math.Ceiling(Centre.Y + SemiMinorAxe * 2 + 10));
            f.ClientSize = new Size(width, height);
            f.ShowDialog();
        }

        public override void Move(double x, double y)
        {
            Centre.Move(x, y);
        }

        public override void MoveTo(double x, double y)
        {
            Centre.X = x;
            Centre.Y = y;
        }

        public override void MoveTo(Point p)
        {
            Centre.X = p.X;
            Centre.Y = p.Y;
        }

        public override void Scale(double factor)
        {
            SemiMajorAxe *= factor;
            SemiMinorAxe *= factor;
        }

        public override string ToString()
        {
            return "Ellipse: Centre: (" + Centre + "), SemiMajorAxe: " + SemiMajorAxe + ", SemiMinorAxe: " + SemiMinorAxe;
        }
    }

    [Serializable]
    class Circle : Ellipse
    {
        public double Radius
        {
            get { return SemiMajorAxe; }
            set { SemiMajorAxe = value; SemiMinorAxe = value; }
        }

        public double Diameter
        {
            get { return 2.0 * Radius; }
            set { Radius = value / 2.0; }
        }

        public Circle(Point centre, double radius) : base(centre, radius, radius) { }

        public Circle(Point centre, double radius, Color strokeColor) : base(centre, radius, radius, strokeColor)
        {
            Centre = centre;
            Radius = radius;
        }

        public double Circumference()
        {
            return Perimeter();
        }

        public override string ToString()
        {
            return "Circle: Centre: (" + Centre + "), Radius: " + Radius;
        }
    }

    [Serializable]
    class FilledCircle : Circle, FilledFigure
    {
        public Color BackgroundColor { get; set; }

        public FilledCircle(Point centre, double radius) : base(centre, radius)
        {
            BackgroundColor = Color.Black;
        }

        public FilledCircle(Point centre, double radius, Color backgroundColor) : base(centre, radius)
        {
            BackgroundColor = backgroundColor;
        }

        public FilledCircle(Point centre, double radius, Color backgroundColor, Color strokeColor) : base(centre, radius, strokeColor)
        {
            BackgroundColor = backgroundColor;
        }

        public override void CreateDrawing(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillEllipse(new SolidBrush(BackgroundColor), (float)(Centre.X - Radius), (float)(Centre.Y - Radius), (float)Radius * 2, (float)Radius * 2);
        }
        
        public override void Draw()
        {
            Form f = new Form();
            f.Paint += CreateDrawing;
            f.Paint += base.CreateDrawing;
            int width = Convert.ToInt32(Math.Ceiling(Centre.X + SemiMajorAxe + 10));
            int height = Convert.ToInt32(Math.Ceiling(Centre.Y + SemiMinorAxe + 10));
            f.ClientSize = new Size(width, height);
            f.ShowDialog();
        }

        public override string ToString()
        {
            return "FilledCircle: Centre: (" + Centre + "), Radius: " + Radius;
        }
    }

    [Serializable]
    class Cone : VolumetricFigure
    {
        public Circle Circle { get; set; }
        public double Height { get; set; }

        public double SlantHeight
        {
            get
            {
                return Math.Sqrt(Math.Pow(Circle.Radius, 2) + Math.Pow(Height, 2));
            }
        }

        public Point Peak
        {
            get
            {
                Point peak = new Point(Circle.Centre.X, Circle.Centre.Y);
                peak.Move(0, -Height);
                return peak;
            }
        }

        public Ellipse Bottom
        {
            get
            {
                return new Ellipse(Circle.Centre, Circle.Radius, Circle.Radius / 3.0, Circle.StrokeColor);
            }
        }

        public override Tuple<Point, Point> GetEdgeCoordinates()
        {
            Tuple<Point, Point> coors = Bottom.GetEdgeCoordinates();
            coors.Item2.Y = Peak.Y;

            return coors;
        }

        public override Tuple<double, double> GetYBy(double x)
        {
            Tuple<double, double> elY = Bottom.GetYBy(x);
            double x1 = Peak.X;
            double y1 = Peak.Y;
            double x2 = Bottom[0].X;
            double y2 = Bottom[0].Y;

            if (x < Peak.X)
            {
                x2 = Bottom[180].X;
                y2 = Bottom[180].Y;
            }

            double y = y2 + (x - x2) * (y2 - y1) / (x2 - x1);
            return new Tuple<double, double>(elY.Item2, y);
        }

        public Cone(Circle circle, double height) : base()
        {
            Circle = circle;
            Height = height;
        }

        public Cone(Circle circle, double height, Color strokeColor) : base(strokeColor)
        {
            Circle = circle;
            Height = height;
        }
        
        public override double Area()
        {
            return Circle.Circumference() + Math.PI * Circle.Radius * SlantHeight;
        }

        public override double Volume()
        {
            return Math.PI * Math.Pow(Circle.Radius, 2) * Height / 3.0;
        }

        protected void DrawLine(Graphics g, Point start, Point end, Color color)
        {
            g.DrawLine(new Pen(color), (float)start.X, (float)start.Y, (float)end.X, (float)end.Y);
        }

        public override void CreateDrawing(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Ellipse el = Bottom;
            el.CreateDrawing(sender, e);


            Point peak = Peak;

            DrawLine(g, el[-30], peak, Color.Black);
            DrawLine(g, el[-30 + 180], peak, Color.Black);
            DrawLine(g, el[0], peak, StrokeColor);
            DrawLine(g, el[180], peak, StrokeColor);
        }

        public override void Draw()
        {
            
            Form f = new Form();
            f.Paint += CreateDrawing;
            f.ShowDialog();
        }
        
        public override void Move(double x, double y)
        {
            Circle.Move(x, y);
        }

        public override void MoveTo(double x, double y)
        {
            Circle.MoveTo(x, y);
        }

        public override void MoveTo(Point p)
        {
            Circle.MoveTo(p);
        }

        public override void Scale(double factor)
        {
            Circle.Scale(factor);
            Height *= factor;
        }

        public override string ToString()
        {
            return "Cone: Circle: (" + Circle + "), Height: " + Height;
        }
    }

    [Serializable]
    class Frustum : Cone
    {
        public double TruncatedHeight { get; set; }

        public Cone TruncatedCone
        {
            get
            {
                Point truncatedCenter = new Point(Circle.Centre.X, Circle.Centre.Y - Height + TruncatedHeight);
                double factor = TruncatedHeight / Height;
                double truncatedRadius = Circle.Radius * factor;
                Circle truncatedCircle = new Circle(truncatedCenter, truncatedRadius);
                Cone truncatedCone = new Cone(truncatedCircle, TruncatedHeight);
                return truncatedCone;
            }
        }

        public Frustum(Circle circle, double height, double truncatedHeight) : base(circle, height)
        {
            TruncatedHeight = truncatedHeight;
        }

        public Frustum(Circle circle, double height, double truncatedHeight, Color strokeColor) : base(circle, height, strokeColor)
        {
            TruncatedHeight = truncatedHeight;
        }

        public override double Volume()
        {
            return base.Area() - TruncatedCone.Area();
        }

        public override double Area()
        {
            return base.Area() - TruncatedCone.Area() + TruncatedCone.Circle.Area();
        }

        public override Tuple<Point, Point> GetEdgeCoordinates()
        {
            Tuple<Point, Point> coors = Bottom.GetEdgeCoordinates();
            coors.Item2.Y = TruncatedCone.Bottom.Centre.Y;

            return coors;
        }

        public override Tuple<double, double> GetYBy(double x)
        {
            Tuple<double, double> elY = Bottom.GetYBy(x);

            Ellipse truncatedBottom = TruncatedCone.Bottom;
            double y = 0;

            if (x < truncatedBottom.Centre.X - truncatedBottom.SemiMajorAxe)
            {
                double x1 = truncatedBottom.Centre.X - truncatedBottom.SemiMajorAxe;
                double y1 = truncatedBottom.Centre.Y;
                double x2 = Bottom[180].X;
                double y2 = Bottom[180].Y;
                y = y2 + (x - x2) * (y2 - y1) / (x2 - x1);
            }
            else if (x > truncatedBottom.Centre.X + truncatedBottom.SemiMajorAxe)
            {
                double x1 = truncatedBottom.Centre.X + truncatedBottom.SemiMajorAxe;
                double y1 = truncatedBottom.Centre.Y;
                double x2 = Bottom[0].X;
                double y2 = Bottom[0].Y;
                y = y2 + (x - x2) * (y2 - y1) / (x2 - x1);
            }
            else
            {
                y = truncatedBottom.GetYBy(x).Item1;
            }

            return new Tuple<double, double>(elY.Item2, y);
        }


        public override void CreateDrawing(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Ellipse bottom = Bottom;
            Ellipse top = TruncatedCone.Bottom;

            bottom.CreateDrawing(sender, e);
            top.CreateDrawing(sender, e);

            DrawLine(g, bottom[-30],       top[-30], StrokeColor);
            DrawLine(g, bottom[-30 + 180], top[-30 + 180], StrokeColor);
            DrawLine(g, bottom[0],         top[0], StrokeColor);
            DrawLine(g, bottom[180],       top[180], StrokeColor);
        }

        public override void Draw()
        {
            Form f = new Form();
            f.Paint += CreateDrawing;
            f.ShowDialog();
        }

        public override void Scale(double factor)
        {
            base.Scale(factor);
            TruncatedHeight *= factor;
        }

        public override string ToString()
        {
            return "Frustum: baseCone: (" + base.ToString() + "), TruncatedHeight: " + TruncatedHeight;
        }

    }

    [Serializable]
    class Image
    {
        public List<Figure> figures;

        public Rectangle Frame;

        public Image()
        {
            figures = new List<Figure>();
            Frame = new Rectangle(0, 0, 1000, 1000);
        }
        public Image(Rectangle frame)
        {
            Frame = new Rectangle(0, 0, 1000, 1000);
            figures = new List<Figure>();
        }
        public Image(int x, int y, int width, int height)
        {
            Frame = new Rectangle(x, y, width, height);
            figures = new List<Figure>();
        }
        public Image(params Figure[] figures)
        {
            this.figures = figures.ToList();
            Frame = new Rectangle(0, 0, 1000, 1000);
        }
        public Image(Rectangle frame, params Figure[] figures)
        {
            Frame = new Rectangle(0, 0, 1000, 1000);
            this.figures = figures.ToList();
        }
        public Image(int x, int y, int width, int height, params Figure[] figures)
        {
            Frame = new Rectangle(x, y, width, height);
            this.figures = figures.ToList();
        }

        public double Perimeter()
        {
            double p = 0;
            for (int i = 0; i < figures.Count; i++)
            {
                PlaneFigure figure = figures[i] as PlaneFigure;
                if (figure != null)
                    p += figure.Perimeter();
            }
            return p;
        }

        public double Area()
        {
            double s = 0;
            for (int i = 0; i < figures.Count; i++)
            {
                s += figures[i].Area();
            }
            return s;
        }

        public double Volume()
        {
            double v = 0;
            for (int i = 0; i < figures.Count; i++)
            {
                VolumetricFigure figure = figures[i] as VolumetricFigure;
                if (figure != null)
                    v += figure.Volume();
            }
            return v;
        }

        public void MoveFigures(double x, double y)
        {
            for (int i = 0; i < figures.Count; i++)
            {
                figures[i].Move(x, y);
            }
        }

        public void MoveFiguresTo(double x, double y)
        {
            for (int i = 0; i < figures.Count; i++)
            {
                figures[i].MoveTo(x, y);
            }
        }

        public void MoveFiguresTo(Point p)
        {
            for (int i = 0; i < figures.Count; i++)
            {
                figures[i].MoveTo(p);
            }
        }

        public void Move(int x, int y)
        {
            Frame.X += x;
            Frame.Y += y;
        }

        public void MoveTo(int x, int y)
        {
            Frame.X = x;
            Frame.Y = y;
        }

        public void MoveTo(Point p)
        {
            Frame.X = (int)p.X;
            Frame.Y = (int)p.Y;
        }
        
        public void Scale(double factor)
        {
            Frame.Width  *= (int)factor;
            Frame.Height *= (int)factor;

            for (int i = 0; i < figures.Count; i++)
                figures[i].Scale(factor);
        }

        public override string ToString()
        {
            string result = "Image: Frame: (X: " + Frame.X + ", Y: " + Frame.Y + "), Images: ";

            if (figures.Count == 0)
                return result + "no images";

            result += "(" + figures[0] + ")";
            for (int i = 1; i < figures.Count; i++)
                result += ", (" + figures[i] + ")";

            return result;
        }

        public void AddImage(Image other)
        {
            figures.AddRange(other.figures);
        }

        public void Draw()
        {
            Form f = new Form();

            for (int i = 0; i < figures.Count; i++)
                f.Paint += figures[i].CreateDrawing;
            f.Bounds = Frame;
            f.ClientSize = Frame.Size;
            f.ShowDialog();
        }

        public void LoadFromFile(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path,
                          FileMode.Open,
                          FileAccess.Read,
                          FileShare.Read);
            Image obj = (Image)formatter.Deserialize(stream);
            figures = obj.figures;
            Frame = obj.Frame;
            stream.Close();
        }

        public void SaveToFile(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }
    }
}
